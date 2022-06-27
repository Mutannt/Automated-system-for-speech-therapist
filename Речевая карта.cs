using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CookComputing.XmlRpc;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Web;

using Word = Microsoft.Office.Interop.Word;
using System.Threading;

namespace SOA_Client
{
    public partial class Речевая_карта : Form
    {
        private Word.Application WordApp;       // Программа Word
        private Word.Documents WordDocuments;   // Документы
        private Word.Document WordDocument; // Документ
        private Word.Paragraphs WordParagraphs; // Параграфы
        private Word.Paragraph WordParagraph;   // Параграф
        private Word.Range WordRange;		// Выделенный диапазон

        IRest2018 rest_proxy;
        IMyProxy xmlrpc_proxy;

        DataTable table;
        DataRow myrow;
        DataColumn col;
        int id = -1;
        List<ItemComboBox> listGroups;
        List<ItemComboBox> listChildren;
        List<ItemComboBox> listLogopeds;
        int IDuser2; // ID пользователя
        public Речевая_карта(int IDuser)
        {
            InitializeComponent();
            IDuser2 = IDuser;
            //Создание прокси-объекта в зависимости от выбранного
            //в настройках протокола
            if (MySettings.Default.ProtocolXmlRpc)
                xmlrpc_proxy = XmlRpcProxyGen.Create<IMyProxy>();
            else if (MySettings.Default.ProtocolRest)
            {
                ChannelFactory<IRest2018> factory;
                factory = new ChannelFactory<IRest2018>("REST2018");
                rest_proxy = factory.CreateChannel();
            }

            //// Начальные значения comboBox - сов
            //for (int i = 4; i < 20; i++)
            //    (Controls["comboBox" + i.ToString()] as ComboBox).SelectedIndex = 0;
        }

        private void Речевая_карта_Load(object sender, EventArgs e)
        {
            //Формирование таблицы со списком детей
            table = new DataTable();
            col = new DataColumn("ID");
            table.Columns.Add(col);
            col = new DataColumn("FIO");
            //col.Caption = "ФИО ребёнка";
            table.Columns.Add(col);

            //Заполнение таблицы данными
            UpdateGrid();

            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].HeaderText = "ФИО ребёнка";
            dataGridView1.Columns[1].Width = 197;
            //dataGridView1.Columns[2].Visible = false;

            //Формирование раскрывающегося списка "Группы"           
            listGroups = new List<ItemComboBox>();
            listGroups.Add(new ItemComboBox(Convert.ToInt32(0), "Выберите"));

            //В зависимости от выбранного протокола 
            //вызов метода ListGroups у соответствующего
            //прокси-объекта
            if (MySettings.Default.ProtocolXmlRpc)
            {
                XMLRPC_Group[] groups;
                // Если пользователь не администратор
                if (IDuser2 != 1)
                    groups = xmlrpc_proxy.ListGroupsUser(IDuser2); // Только те группы к которым отсится
                else
                    groups = xmlrpc_proxy.ListGroups(); // Все группы
                foreach (XMLRPC_Group group in groups)
                    listGroups.Add(new ItemComboBox(group.IDgr, group.NumberGr.ToString()));
            }
            else if (MySettings.Default.ProtocolRest)
            {
                REST_Group[] groups = rest_proxy.ListGroups();
                foreach (REST_Group group in groups)
                    listGroups.Add(new ItemComboBox(group.IDgr, group.NumberGr.ToString()));
            }

            ////Вывод полученного списка групп в раскрывающийся список
            comboBox1.DataSource = listGroups;
            //comboBox1.SelectedItem = null;

            //Формирование раскрывающегося списка "Логопеды"           
            listLogopeds = new List<ItemComboBox>();

            //В зависимости от выбранного протокола 
            //вызов метода ListLogopeds у соответствующего
            //прокси-объекта
            if (MySettings.Default.ProtocolXmlRpc)
            {
                XMLRPC_Logoped[] logopeds = xmlrpc_proxy.ListFioLogopeds();
                foreach (XMLRPC_Logoped logoped in logopeds)
                    listLogopeds.Add(new ItemComboBox(logoped.ID, logoped.FIO));
            }
            else if (MySettings.Default.ProtocolRest)
            {
                listLogopeds.Add(new ItemComboBox(0, "Выберите"));
                REST_Logoped[] logopeds = rest_proxy.ListFioLogopeds();
                foreach (REST_Logoped logoped in logopeds)
                    listLogopeds.Add(new ItemComboBox(logoped.ID, logoped.FIO));
            }
            ////Вывод полученного списка логопедов в раскрывающийся список
            comboBox20.DataSource = listLogopeds;

            //Заполнение часто встречающихся вариантов
            checkedListBox1.SetItemChecked(0, true);
            checkedListBox1.SetItemChecked(1, true);
            checkedListBox2.SetItemChecked(0, true);
            checkedListBox3.SetItemChecked(0, true);
            checkedListBox4.SetItemChecked(0, true);
            checkedListBox4.SetItemChecked(2, true);
            checkedListBox4.SetItemChecked(3, true);
            checkedListBox5.SetItemChecked(0, true);
            checkedListBox5.SetItemChecked(2, true);
            checkedListBox6.SetItemChecked(0, true);
            checkedListBox7.SetItemChecked(0, true);

            // Начальные значения comboBox - сов
            for (int i = 4; i < 20; i++)
                (Controls["comboBox" + i.ToString()] as ComboBox).SelectedIndex = 0;
        }
        // При выборе группы
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((comboBox1.SelectedItem as ItemComboBox).Id != 0)
            {
                //Формирование раскрывающегося списка "Дети"           
                listChildren = new List<ItemComboBox>();

                //В зависимости от выбранного протокола 
                //вызов метода ListLogopeds у соответствующего
                //прокси-объекта
                if (MySettings.Default.ProtocolXmlRpc)
                {
                    XMLRPC_Child[] childrenInGroupLogopunct = xmlrpc_proxy.ListChildrenInGroupLogopunct((comboBox1.SelectedItem as ItemComboBox).Id);
                    foreach (XMLRPC_Child child in childrenInGroupLogopunct)
                        listChildren.Add(new ItemComboBox(child.ID, child.FIO));
                }
                else if (MySettings.Default.ProtocolRest)
                {
                    REST_Child[] childrenInGroupDiagn = rest_proxy.ListChildrenInGroupDiagn((comboBox1.SelectedItem as ItemComboBox).Id); // =================== Нужно список детей зачисленных в логопункт 
                    foreach (REST_Child child in childrenInGroupDiagn)
                    {
                        listChildren.Add(new ItemComboBox(child.ID, child.FIO)); // Заполнение списка
                    }
                }
                //Вывод полученного списка языков в раскрывающийся список
                comboBox2.DataSource = listChildren;
            }
            else
            {
                comboBox2.DataSource = listChildren;
            }
        }

        // При выборе ребёнка
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MySettings.Default.ProtocolXmlRpc)
            {
                XMLRPC_Child child = xmlrpc_proxy.ReadChild((comboBox2.SelectedItem as ItemComboBox).Id);
                //Заполнение даты рождения
                string str = child.DateB.Replace('-', '.');
                dateTimePicker1.Value = Convert.ToDateTime(str);
                // Выбор диагноза
                XMLRPC_Diagnostic diagnostic = xmlrpc_proxy.ReadDiagnosticIDchild((comboBox2.SelectedItem as ItemComboBox).Id);
                comboBox3.SelectedIndex = diagnostic.IDvioal1 - 1; // -1 т.к. с 0 считает
            }
            else if (MySettings.Default.ProtocolRest)
            {
                REST_Child child = rest_proxy.ReadChild((comboBox2.SelectedItem as ItemComboBox).Id);
                //Заполнение даты рождения
                string str = child.DateB.Replace('-', '.');
                dateTimePicker1.Value = Convert.ToDateTime(str);
                // Выбор диагноза
                REST_Diagnostic diagnostic = rest_proxy.ReadDiagnosticIDchild((comboBox2.SelectedItem as ItemComboBox).Id);
                comboBox3.SelectedIndex = diagnostic.IDvioal1 - 1; // -1 т.к. с 0 считает
            }
        }
        private void UpdateGrid()
        {
            //Обновление таблицы

            //Очистка таблицы
            table.Clear();

            //Подгрузка в таблицу новых данных
            if (MySettings.Default.ProtocolXmlRpc)
            {
                XMLRPC_SpeechCard[] speechcards;
                // Если пользователь не администратор
                if (IDuser2 != 1)
                    speechcards = xmlrpc_proxy.ListSpeechCardsUser(IDuser2);
                else
                    speechcards = xmlrpc_proxy.ListSpeechCards();//ListSpeechCardsUser
                foreach (XMLRPC_SpeechCard speechcard in speechcards)
                {
                    myrow = table.NewRow();
                    myrow["ID"] = speechcard.ID;
                    myrow["FIO"] = speechcard.FIO;
                    //myrow["IDchild"] = speechcard.IDchild;
                    table.Rows.Add(myrow);
                }
            }
            else if (MySettings.Default.ProtocolRest)
            {
                REST_SpeechCard[] speechcards = rest_proxy.ListSpeechCards();
                foreach (REST_SpeechCard speechcard in speechcards)
                {
                    myrow = table.NewRow();
                    myrow["ID"] = speechcard.ID;
                    myrow["FIO"] = speechcard.FIO;
                    //myrow["IDchild"] = speechcard.IDchild;
                    table.Rows.Add(myrow);
                }
            }
            dataGridView1.DataSource = table;
        }
        private void ClearForm()
        {
            tb_ID.Text = "";

            //Функция очистки формы
            // Начальные значения comboBox - сов
            //for (int i = 4; i < 20; i++)
            //    (Controls["comboBox" + i.ToString()] as ComboBox).SelectedIndex = 0;

            // Очистка checkedListBox - сов
            for (int i = 1; i < 13; i++)
            {
                (Controls["checkedListBox" + i.ToString()] as CheckedListBox).ClearSelected(); // Снимает синее выделение
                for (int j = 0; j < (Controls["checkedListBox" + i.ToString()] as CheckedListBox).Items.Count; j++)
                {
                    (Controls["checkedListBox" + i.ToString()] as CheckedListBox).SetItemChecked(j, false); // Снимает галочки
                }
            }

            ////Заполнение часто встречающихся вариантов
            //checkedListBox1.SetItemChecked(0, true);
            //checkedListBox1.SetItemChecked(1, true);
            //checkedListBox2.SetItemChecked(0, true);
            //checkedListBox3.SetItemChecked(0, true);
            //checkedListBox4.SetItemChecked(0, true);
            //checkedListBox4.SetItemChecked(2, true);
            //checkedListBox4.SetItemChecked(3, true);
            //checkedListBox5.SetItemChecked(0, true);
            //checkedListBox5.SetItemChecked(2, true);
            //checkedListBox6.SetItemChecked(0, true);
            //checkedListBox7.SetItemChecked(0, true);

            // Выбор текущей даты
            dateTimePicker2.Value = DateTime.Today;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                //Получение даты
                string date;
                date = dateTimePicker2.Value.Year.ToString() + "-" + dateTimePicker2.Value.Month.ToString() + "-" + dateTimePicker2.Value.Day.ToString();

                // Получение индексов нажатых checkBoxes и добавление их в строку
                string lips = "";
                foreach (int s in checkedListBox1.CheckedIndices)
                    lips = lips + s.ToString() + " ";
                // Удаление последнего пробела
                if (lips.Length != 0)
                    lips = lips.Remove(lips.Length - 1);

                // Получение индексов нажатых checkBoxes и добавление их в строку
                string teeth = "";
                foreach (int s in checkedListBox2.CheckedIndices)
                    teeth = teeth + s.ToString() + " ";
                // Удаление последнего пробела
                if (teeth.Length != 0)
                    teeth = teeth.Remove(teeth.Length - 1);

                // Получение индексов нажатых checkBoxes и добавление их в строку
                string bite = "";
                foreach (int s in checkedListBox3.CheckedIndices)
                    bite = bite + s.ToString() + " ";
                // Удаление последнего пробела
                if (bite.Length != 0)
                    bite = bite.Remove(bite.Length - 1);

                // Получение индексов нажатых checkBoxes и добавление их в строку
                string tongue = "";
                foreach (int s in checkedListBox4.CheckedIndices)
                    tongue = tongue + s.ToString() + " ";
                // Удаление последнего пробела
                if (tongue.Length != 0)
                    tongue = tongue.Remove(tongue.Length - 1);

                // Получение индексов нажатых checkBoxes и добавление их в строку
                string hyoidFrenulum = "";
                foreach (int s in checkedListBox5.CheckedIndices)
                    hyoidFrenulum = hyoidFrenulum + s.ToString() + " ";
                // Удаление последнего пробела
                if (hyoidFrenulum.Length != 0)
                    hyoidFrenulum = hyoidFrenulum.Remove(hyoidFrenulum.Length - 1);

                // Получение индексов нажатых checkBoxes и добавление их в строку
                string sky = "";
                foreach (int s in checkedListBox6.CheckedIndices)
                    sky = sky + s.ToString() + " ";
                // Удаление последнего пробела
                if (sky.Length != 0)
                    sky = sky.Remove(sky.Length - 1);

                // Получение индексов нажатых checkBoxes и добавление их в строку
                string salivation = "";
                foreach (int s in checkedListBox7.CheckedIndices)
                    salivation = salivation + s.ToString() + " ";
                // Удаление последнего пробела
                if (salivation.Length != 0)
                    salivation = salivation.Remove(salivation.Length - 1);

                string comboBoxes = "";
                for (int i = 4; i < 20; i++)
                    comboBoxes = comboBoxes + (Controls["comboBox" + i.ToString()] as ComboBox).SelectedIndex.ToString() + " ";
                // Удаление последнего пробела
                comboBoxes = comboBoxes.Remove(comboBoxes.Length - 1);

                // Получение индексов нажатых checkBoxes и добавление их в строку
                string soundPronunciation = "";
                foreach (int s in checkedListBox8.CheckedIndices)
                    soundPronunciation = soundPronunciation + s.ToString() + " ";
                // Удаление последнего пробела
                if (soundPronunciation.Length != 0)
                    soundPronunciation = soundPronunciation.Remove(soundPronunciation.Length - 1);

                // Получение индексов нажатых checkBoxes и добавление их в строку
                string soundDifferentiation = "";
                foreach (int s in checkedListBox9.CheckedIndices)
                    soundDifferentiation = soundDifferentiation + s.ToString() + " ";
                // Удаление последнего пробела
                if (soundDifferentiation.Length != 0)
                    soundDifferentiation = soundDifferentiation.Remove(soundDifferentiation.Length - 1);

                // Получение индексов нажатых checkBoxes и добавление их в строку
                string syllableDifferentiation = "";
                foreach (int s in checkedListBox10.CheckedIndices)
                    syllableDifferentiation = syllableDifferentiation + s.ToString() + " ";
                // Удаление последнего пробела
                if (syllableDifferentiation.Length != 0)
                    syllableDifferentiation = syllableDifferentiation.Remove(syllableDifferentiation.Length - 1);

                // Получение индексов нажатых checkBoxes и добавление их в строку
                string wordDifference = "";
                foreach (int s in checkedListBox11.CheckedIndices)
                    wordDifference = wordDifference + s.ToString() + " ";
                // Удаление последнего пробела
                if (wordDifference.Length != 0)
                    wordDifference = wordDifference.Remove(wordDifference.Length - 1);

                // Получение индексов нажатых checkBoxes и добавление их в строку
                string soundHighlight = "";
                foreach (int s in checkedListBox12.CheckedIndices)
                    soundHighlight = soundHighlight + s.ToString() + " ";
                // Удаление последнего пробела
                if (soundHighlight.Length != 0)
                    soundHighlight = soundHighlight.Remove(soundHighlight.Length - 1);

                //Сохранение/обновление данных, введённых в форму
                if (MySettings.Default.ProtocolXmlRpc)
                {
                    XMLRPC_SpeechCard speechcard = new XMLRPC_SpeechCard();
                    speechcard.IDchild = (comboBox2.SelectedItem as ItemComboBox).Id;
                    speechcard.DateOfExamination = date;
                    speechcard.Lips = lips;
                    speechcard.Teeth = teeth;
                    speechcard.Bite = bite;
                    speechcard.Tongue = tongue;
                    speechcard.HyoidFrenulum = hyoidFrenulum;
                    speechcard.Sky = sky;
                    speechcard.Salivation = salivation;
                    speechcard.ComboBoxes = comboBoxes;
                    speechcard.SoundPronunciation = soundPronunciation;
                    speechcard.SoundDifferentiation = soundDifferentiation;
                    speechcard.SyllableDifferentiation = syllableDifferentiation;
                    speechcard.WordDifference = wordDifference;
                    speechcard.SoundHighlight = soundHighlight;

                    if (id < 0) //Если идентификатор не задан или сброшен - 
                        xmlrpc_proxy.CreateSpeechCard(speechcard);
                    else //иначе (идентификатор задан) -
                        xmlrpc_proxy.UpdateSpeechCard(id,speechcard);
                }
                else if (MySettings.Default.ProtocolRest)
                {
                    if (id < 0)
                        rest_proxy.CreateSpeechCard(new REST_SpeechCard(
                                (comboBox2.SelectedItem as ItemComboBox).Id,
                                date,
                                lips,
                                teeth,
                                bite,
                                tongue,
                                hyoidFrenulum,
                                sky,
                                salivation,
                                comboBoxes,
                                soundPronunciation,
                                soundDifferentiation,
                                syllableDifferentiation,
                                wordDifference,
                                soundHighlight
                        ));
                    else
                        rest_proxy.UpdateSpeechCard(id, new REST_SpeechCard(
                                (comboBox2.SelectedItem as ItemComboBox).Id,
                                date,
                                lips,
                                teeth,
                                bite,
                                tongue,
                                hyoidFrenulum,
                                sky,
                                salivation,
                                comboBoxes,
                                soundPronunciation,
                                soundDifferentiation,
                                syllableDifferentiation,
                                wordDifference,
                                soundHighlight
                        ));
                }
                    
                //По завершении сохранения:
                if (id >= 0) id = -1; //Сброс идентификатора

                UpdateGrid(); //Обновление таблицы
                ClearForm(); //Очистка формы
            }
            catch
            {
                MessageBox.Show("Проверьте поля перед сохранением! Возможно, этот ребёнок уже добавлен.", "Ошибка сохранения", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //    MessageBox.Show(comboBox15.SelectedItem.ToString());

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            //Если идентификатор не задан или сброшен -
            if (id < 0) return; //удалить нельзя, выход.

            //Вывод предупреждения
            if (MessageBox.Show("Действительно удалить?", "", MessageBoxButtons.YesNo) == DialogResult.No) return;

            //Собственно удаление
            if (MySettings.Default.ProtocolXmlRpc)
                xmlrpc_proxy.DeleteSpeechCard(id);
            else if (MySettings.Default.ProtocolRest)
                rest_proxy.DeleteSpeechCard(id);

            id = -1; //Сброс идентификатора
            UpdateGrid(); //Обновление таблицы
            ClearForm(); //Очистка формы
        }

        // При выборе записи в таблице
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ClearForm(); //Очистка формы
            if (dataGridView1.CurrentRow.Index + 1 == dataGridView1.Rows.Count)
            {
                //Если щёлкнули в последней строке таблицы - переход в режим добавления новой записи -
                id = -1; //Сброс идентификатора
                return; //Выход из обработчика
            }

            //Подгрузка в форму данных выбранного в таблице диагностики
            id = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);

            if (MySettings.Default.ProtocolXmlRpc)
            {
                XMLRPC_SpeechCard speechcard = xmlrpc_proxy.ReadSpeechCard(id);

                tb_ID.Text = speechcard.ID.ToString();
                string str2 = speechcard.DateOfExamination.Replace('-', '.');
                dateTimePicker2.Value = Convert.ToDateTime(str2);

                if (speechcard.Lips != "")
                {
                    // Разделение строки
                    string[] lips = speechcard.Lips.Split(new char[] { ' ' });
                    foreach (string s in lips)
                    {
                        checkedListBox1.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                if (speechcard.Teeth != "")
                {
                    // Разделение строки
                    string[] teeths = speechcard.Teeth.Split(new char[] { ' ' });
                    foreach (string s in teeths)
                    {
                        checkedListBox2.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                if (speechcard.Bite != "")
                {
                    // Разделение строки
                    string[] bites = speechcard.Bite.Split(new char[] { ' ' });
                    foreach (string s in bites)
                    {
                        checkedListBox3.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                if (speechcard.Tongue != "")
                {
                    // Разделение строки
                    string[] tongues = speechcard.Tongue.Split(new char[] { ' ' });
                    foreach (string s in tongues)
                    {
                        checkedListBox4.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                if (speechcard.HyoidFrenulum != "")
                {
                    // Разделение строки
                    string[] hyoidFrenulums = speechcard.HyoidFrenulum.Split(new char[] { ' ' });
                    foreach (string s in hyoidFrenulums)
                    {
                        checkedListBox5.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                if (speechcard.Sky != "")
                {
                    // Разделение строки
                    string[] skys = speechcard.Sky.Split(new char[] { ' ' });
                    foreach (string s in skys)
                    {
                        checkedListBox6.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                if (speechcard.Salivation != "")
                {
                    // Разделение строки
                    string[] salivations = speechcard.Salivation.Split(new char[] { ' ' });
                    foreach (string s in salivations)
                    {
                        checkedListBox7.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }


                // Разделение строки
                string[] words = speechcard.ComboBoxes.Split(new char[] { ' ' });

                for (int i = 0; i < words.Length; i++)
                    (Controls["comboBox" + (i + 4).ToString()] as ComboBox).SelectedIndex = Convert.ToInt32(words[i]);

                if (speechcard.SoundPronunciation != "")
                {
                    // Разделение строки
                    string[] soundPronunciations = speechcard.SoundPronunciation.Split(new char[] { ' ' });
                    foreach (string s in soundPronunciations)
                    {
                        checkedListBox8.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                if (speechcard.SoundDifferentiation != "")
                {
                    // Разделение строки
                    string[] soundDifferentiations = speechcard.SoundDifferentiation.Split(new char[] { ' ' });
                    foreach (string s in soundDifferentiations)
                    {
                        checkedListBox9.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                if (speechcard.SyllableDifferentiation != "")
                {
                    // Разделение строки
                    string[] syllableDifferentiations = speechcard.SyllableDifferentiation.Split(new char[] { ' ' });
                    foreach (string s in syllableDifferentiations)
                    {
                        checkedListBox10.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                if (speechcard.WordDifference != "")
                {
                    // Разделение строки
                    string[] wordDifferences = speechcard.WordDifference.Split(new char[] { ' ' });
                    foreach (string s in wordDifferences)
                    {
                        checkedListBox11.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                if (speechcard.SoundHighlight != "")
                {
                    // Разделение строки
                    string[] soundHighlights = speechcard.SoundHighlight.Split(new char[] { ' ' });
                    foreach (string s in soundHighlights)
                    {
                        checkedListBox12.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                XMLRPC_Child child = xmlrpc_proxy.ReadChild(speechcard.IDchild); // Запись выбранного ребёнка
                // Если эта группа ещё не выбрана, выбрать её///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if ((comboBox1.SelectedItem as ItemComboBox).Id != child.IDgr)
                {
                    //Выделение в раскрывающемся списке Группы
                    foreach (ItemComboBox item in listGroups)
                    {
                        if (item.Id == child.IDgr)
                        {
                            this.comboBox1.SelectedItem = item;
                            break;
                        }
                    }
                }

                //Выделение в раскрывающемся списке ФИО ребёнка
                foreach (ItemComboBox item in listChildren)
                {
                    if (item.Id == speechcard.IDchild)
                    {
                        this.comboBox2.SelectedItem = item;
                        break;
                    }
                }
            }
            else if (MySettings.Default.ProtocolRest)
            {
                REST_SpeechCard speechcard = rest_proxy.ReadSpeechCard(id);

                tb_ID.Text = speechcard.ID.ToString();
                string str2 = speechcard.DateOfExamination.Replace('-', '.');
                dateTimePicker2.Value = Convert.ToDateTime(str2);

                if (speechcard.Lips != "")
                {
                    // Разделение строки
                    string[] lips = speechcard.Lips.Split(new char[] { ' ' });
                    foreach (string s in lips)
                    {
                        checkedListBox1.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                if (speechcard.Teeth != "")
                {
                    // Разделение строки
                    string[] teeths = speechcard.Teeth.Split(new char[] { ' ' });
                    foreach (string s in teeths)
                    {
                        checkedListBox2.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                if (speechcard.Bite != "")
                {
                    // Разделение строки
                    string[] bites = speechcard.Bite.Split(new char[] { ' ' });
                    foreach (string s in bites)
                    {
                        checkedListBox3.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                if (speechcard.Tongue != "")
                {
                    // Разделение строки
                    string[] tongues = speechcard.Tongue.Split(new char[] { ' ' });
                    foreach (string s in tongues)
                    {
                        checkedListBox4.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                if (speechcard.HyoidFrenulum != "")
                {
                    // Разделение строки
                    string[] hyoidFrenulums = speechcard.HyoidFrenulum.Split(new char[] { ' ' });
                    foreach (string s in hyoidFrenulums)
                    {
                        checkedListBox5.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                if (speechcard.Sky != "")
                {
                    // Разделение строки
                    string[] skys = speechcard.Sky.Split(new char[] { ' ' });
                    foreach (string s in skys)
                    {
                        checkedListBox6.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                if (speechcard.Salivation != "")
                {
                    // Разделение строки
                    string[] salivations = speechcard.Salivation.Split(new char[] { ' ' });
                    foreach (string s in salivations)
                    {
                        checkedListBox7.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }


                // Разделение строки
                string[] words = speechcard.ComboBoxes.Split(new char[] { ' ' });

                for (int i = 0; i < words.Length; i++)
                    (Controls["comboBox" + (i + 4).ToString()] as ComboBox).SelectedIndex = Convert.ToInt32(words[i]);

                if (speechcard.SoundPronunciation != "")
                {
                    // Разделение строки
                    string[] soundPronunciations = speechcard.SoundPronunciation.Split(new char[] { ' ' });
                    foreach (string s in soundPronunciations)
                    {
                        checkedListBox8.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                if (speechcard.SoundDifferentiation != "")
                {
                    // Разделение строки
                    string[] soundDifferentiations = speechcard.SoundDifferentiation.Split(new char[] { ' ' });
                    foreach (string s in soundDifferentiations)
                    {
                        checkedListBox9.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                if (speechcard.SyllableDifferentiation != "")
                {
                    // Разделение строки
                    string[] syllableDifferentiations = speechcard.SyllableDifferentiation.Split(new char[] { ' ' });
                    foreach (string s in syllableDifferentiations)
                    {
                        checkedListBox10.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                if (speechcard.WordDifference != "")
                {
                    // Разделение строки
                    string[] wordDifferences = speechcard.WordDifference.Split(new char[] { ' ' });
                    foreach (string s in wordDifferences)
                    {
                        checkedListBox11.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                if (speechcard.SoundHighlight != "")
                {
                    // Разделение строки
                    string[] soundHighlights = speechcard.SoundHighlight.Split(new char[] { ' ' });
                    foreach (string s in soundHighlights)
                    {
                        checkedListBox12.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                REST_Child child = rest_proxy.ReadChild(speechcard.IDchild); // Запись выбранного ребёнка
                                                                             // Если эта группа ещё не выбрана, выбрать её///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if ((comboBox1.SelectedItem as ItemComboBox).Id != child.IDgr)
                {
                    //Выделение в раскрывающемся списке Группы
                    foreach (ItemComboBox item in listGroups)
                    {
                        if (item.Id == child.IDgr)
                        {
                            this.comboBox1.SelectedItem = item;
                            break;
                        }
                    }
                }

                //Выделение в раскрывающемся списке ФИО ребёнка
                foreach (ItemComboBox item in listChildren)
                {
                    if (item.Id == speechcard.IDchild)
                    {
                        this.comboBox2.SelectedItem = item;
                        break;
                    }
                }
            }
        }
        
        private void comboBox12_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Если бедный
            if (comboBox12.SelectedIndex == 2)
                comboBox13.Enabled = true;
            else
            {
                comboBox13.Enabled = false;
                comboBox13.SelectedIndex = 0;
            }
        }

        private void tb_ID_TextChanged(object sender, EventArgs e)
        {
            if (tb_ID.Text == "")
            {
                btn_Save.Text = "Добавить";
                btn_Delete.Enabled = false;
            }
            else
            {
                btn_Save.Text = "Обновить";
                btn_Delete.Enabled = true;
            }
        }
        // Очистить
        private void button2_Click(object sender, EventArgs e)
        {
            id = -1; // Сброс идентификатора
            ClearForm(); // Очистка формы

            //Заполнение часто встречающихся вариантов
            checkedListBox1.SetItemChecked(0, true);
            checkedListBox1.SetItemChecked(1, true);
            checkedListBox2.SetItemChecked(0, true);
            checkedListBox3.SetItemChecked(0, true);
            checkedListBox4.SetItemChecked(0, true);
            checkedListBox4.SetItemChecked(2, true);
            checkedListBox4.SetItemChecked(3, true);
            checkedListBox5.SetItemChecked(0, true);
            checkedListBox5.SetItemChecked(2, true);
            checkedListBox6.SetItemChecked(0, true);
            checkedListBox7.SetItemChecked(0, true);

            // Начальные значения comboBox - сов
            for (int i = 4; i < 20; i++)
                (Controls["comboBox" + i.ToString()] as ComboBox).SelectedIndex = 0;
        }

        // Печать
        private void button1_Click(object sender, EventArgs e)
        {
            // Запускаем Word
            WordApp = new Word.Application();
            // Делаем Word видимым
            WordApp.Visible = true;
            //Получаем доступ к объекту все документы
            WordDocuments = WordApp.Documents;
            // Добавляем документ
            WordDocument = WordDocuments.Add();
            // Получаем доступ к объекту все параграфы
            WordParagraphs = WordDocument.Content.Paragraphs;
            // Получаем доступ к объекту первый параграф
            WordParagraph = WordParagraphs[1];
            // Устанавливаем выравнивание по центру
            WordParagraph.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            // Получаем доступ к объекту выделенный участок
            WordRange = WordParagraph.Range;

            // Если ошибка при открытии Ворда
            MessageBox.Show("Если ворд не активирован, дождитесь появления окна с активацией и закройте его." +
                "Только потом нажмите ОК", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            // Сделаем шрифт выделенного участка жирным
            WordRange.Font.Bold = 1;
            // Сделаем размер шрифта выделенного участка равным 16
            WordRange.Font.Size = 11;
            // Шрифт
            WordRange.Font.Name = "Times New Roman";
            // Добавим текст в выделенный участок
            WordRange.InsertAfter("Речевая карта\n");
            // Сбросим выделение участка
            WordRange.Collapse(Word.WdCollapseDirection.wdCollapseEnd);


            // Получаем доступ к объекту 2й параграф
            WordParagraph = WordParagraphs[2];
            // Устанавливаем выравнивание по центру
            WordParagraph.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            // Сейчас выделенным участком будет пустой участок в конце текста
            WordRange = WordParagraph.Range;
            // Сделаем шрифт выделенного участка не жирным
            WordRange.Font.Bold = 0;



            // Добавим текст в выделенный участок
            WordRange.InsertAfter("Фамилия, имя ребенка: ");
            WordRange.InsertAfter(comboBox2.Text);

            //// Подчёркивание
            //WordRange.Font.Underline = Word.WdUnderline.wdUnderlineSingle;
            //// НЕТ Подчёркивания
            //WordRange.Font.Underline = Word.WdUnderline.wdUnderlineNone;

            WordRange.InsertAfter("\nДата рождения ребенка: ");
            WordRange.InsertAfter(dateTimePicker1.Value.ToShortDateString());

            WordRange.InsertAfter("\nДата обследования: ");
            WordRange.InsertAfter(dateTimePicker2.Value.ToShortDateString());


            //REST_Child child = rest_proxy.ReadChild((comboBox2.SelectedItem as ItemComboBox).Id);

            WordRange.InsertAfter("\nДомашний адрес: ");
            WordRange.InsertAfter("___________"); // child.

            WordRange.InsertAfter("\nСостояние общей моторики: ");
            WordRange.InsertAfter(comboBox4.Text);

            WordRange.InsertAfter("\nСостояние мелкой моторики: ");
            WordRange.InsertAfter(comboBox5.Text);

            WordRange.InsertAfter("\nСтроение артикуляционного аппарата \n");
            
            // Сейчас выделенным участком будет пустой участок в конце текста
            WordRange = WordParagraph.Range;

            int numRows = 2; // Количество строк в таблице
            int numColumns = 7; // Количество столбцов в таблице

            //Добавляем таблицу и получаем объект wordtable 
            Word.Table wordtable = WordDocument.Tables.Add(WordRange, numRows, numColumns);
            wordtable.Borders.Enable = 1;

            // Шапка таблицы ====================================================
            Word.Range wordcellrange = WordDocument.Tables[1].Cell(1, 1).Range;
            wordcellrange.Bold = 1;
            wordcellrange.Text = "Губы";

            wordcellrange = wordtable.Cell(1, 2).Range;
            wordcellrange.Bold = 1;
            wordcellrange.Text = "Зубы";

            wordcellrange = wordtable.Cell(1, 3).Range;
            wordcellrange.Bold = 1;
            wordcellrange.Text = "Прикус";

            wordcellrange = wordtable.Cell(1, 4).Range;
            wordcellrange.Bold = 1;
            wordcellrange.Text = "Язык";

            wordcellrange = wordtable.Cell(1, 5).Range;
            wordcellrange.Bold = 1;
            wordcellrange.Text = "Подъязычная уздечка";

            wordcellrange = wordtable.Cell(1, 6).Range;
            wordcellrange.Bold = 1;
            wordcellrange.Text = "Небо";

            wordcellrange = wordtable.Cell(1, 7).Range;
            wordcellrange.Bold = 1;
            wordcellrange.Text = "Саливация";

            // Первый столбец
            // Строка, столбец
            wordcellrange = wordtable.Cell(2, 1).Range;
            // Получение значений нажатых checkBoxes и добавление их в строку
            string str = "";
            foreach (string s in checkedListBox1.CheckedItems)
                str = str + s.ToString() + " ";
            wordcellrange.Text = str;
            wordcellrange = wordtable.Cell(2, 2).Range;

            str = "";
            // Получение значений нажатых checkBoxes и добавление их в строку
            foreach (string s in checkedListBox2.CheckedItems)
                str = str + s.ToString() + " ";
            wordcellrange.Text = str;
            wordcellrange = wordtable.Cell(2, 3).Range;

            str = "";
            // Получение значений нажатых checkBoxes и добавление их в строку
            foreach (string s in checkedListBox3.CheckedItems)
                str = str + s.ToString() + " ";
            wordcellrange.Text = str;
            wordcellrange = wordtable.Cell(2, 4).Range;

            str = "";
            // Получение значений нажатых checkBoxes и добавление их в строку
            foreach (string s in checkedListBox4.CheckedItems)
                str = str + s.ToString() + " ";
            wordcellrange.Text = str;
            wordcellrange = wordtable.Cell(2, 5).Range;

            str = "";
            // Получение значений нажатых checkBoxes и добавление их в строку
            foreach (string s in checkedListBox5.CheckedItems)
                str = str + s.ToString() + " ";
            wordcellrange.Text = str;
            wordcellrange = wordtable.Cell(2, 6).Range;
            
            str = "";
            // Получение значений нажатых checkBoxes и добавление их в строку
            foreach (string s in checkedListBox6.CheckedItems)
                str = str + s.ToString() + " ";
            wordcellrange.Text = str;
            wordcellrange = wordtable.Cell(2, 7).Range;
            
            str = "";
            // Получение значений нажатых checkBoxes и добавление их в строку
            foreach (string s in checkedListBox7.CheckedItems)
                str = str + s.ToString() + " ";
            wordcellrange.Text = str;

            // Сейчас выделенным участком будет пустой участок в конце текста
            WordRange = WordParagraph.Range;

            WordRange.InsertAfter("Общее звучание речи: ");
            WordRange.InsertAfter(comboBox6.Text);

            WordRange.InsertAfter("\nТемп: ");
            WordRange.InsertAfter(comboBox7.Text);

            WordRange.InsertAfter("\nДыхание: ");
            WordRange.InsertAfter(comboBox8.Text);

            WordRange.InsertAfter("\nСила голоса: ");
            WordRange.InsertAfter(comboBox9.Text);

            WordRange.InsertAfter("\nГолос: ");
            WordRange.InsertAfter(comboBox10.Text);

            WordRange.InsertAfter("\nПлавность: ");
            WordRange.InsertAfter(comboBox11.Text);

            WordRange.InsertAfter("\nПонимание речи: ");
            WordRange.InsertAfter(comboBox18.Text);

            WordRange.InsertAfter("\nСловарный запас: ");
            WordRange.InsertAfter(comboBox12.Text);

            if (comboBox12.SelectedIndex == 2)
                WordRange.InsertAfter(" —— " + comboBox13.Text);

            WordRange.InsertAfter("\nСлоговая структура: ");
            WordRange.InsertAfter(comboBox14.Text);

            WordRange.InsertAfter("\nКоррекция звукопроизношения\n");

            // Сейчас выделенным участком будет пустой участок в конце текста
            WordRange = WordParagraph.Range;

            // ТАБЛИЦА Звукопроизношение 
            numRows = 2; // Количество строк в таблице
            numColumns = checkedListBox8.CheckedItems.Count; // Количество столбцов в таблице
            if (numColumns.ToString() != "0")
            {//=================================================ОШИБКА ЕСЛИ ПУСТЫЕ ЭЛЕМЕНТЫ==================================================================================================
                int column = 1; // Счётчик столбцов
                //Добавляем таблицу и получаем объект wordtable 
                Word.Table wordtable2 = WordDocument.Tables.Add(WordRange, numRows, numColumns);
                wordtable2.Borders.Enable = 1;

                // Таблица ====================================================
                Word.Range wordcellrange2 = WordDocument.Tables[2].Cell(1, 1).Range;
                foreach (string item in checkedListBox8.CheckedItems)
                {
                    wordcellrange2 = wordtable2.Cell(1, column).Range;
                    wordcellrange2.Text = item.ToString();
                    column++;
                }
                //// Сейчас выделенным участком будет пустой участок в конце текста
                //WordRange = WordParagraph.Range;
            }
            else {
                //Добавляем пустую таблицу и получаем объект wordtable 
                Word.Table wordtable2 = WordDocument.Tables.Add(WordRange, 1, 1);
                wordtable2.Borders.Enable = 1;
            }
            //WordDocument.Tables.Count;


            WordRange.InsertAfter("Характер нарушений: ");
            WordRange.InsertAfter(comboBox19.Text);

            WordRange.InsertAfter("\nФонематическое восприятие: ");
            WordRange.InsertAfter(comboBox16.Text);

            //=================================================================================================================
            WordRange.InsertAfter("\nДифференциация звуков\n");

            // Сейчас выделенным участком будет пустой участок в конце текста
            WordRange = WordParagraph.Range;

            // ТАБЛИЦА Дифференциация звуков  
            numRows = 2; // Количество строк в таблице
            numColumns = checkedListBox9.CheckedItems.Count; // Количество столбцов в таблице
            if (numColumns.ToString() != "0")
            {
                int column = 1; // Счётчик столбцов
                //Добавляем таблицу и получаем объект wordtable 
                Word.Table wordtable3 = WordDocument.Tables.Add(WordRange, numRows, numColumns);
                wordtable3.Borders.Enable = 1;
                // Таблица ====================================================
                Word.Range wordcellrange3 = WordDocument.Tables[3].Cell(1, 1).Range;
                foreach (string item in checkedListBox9.CheckedItems)
                {
                    wordcellrange3 = wordtable3.Cell(1, column).Range;
                    wordcellrange3.Text = item.ToString();
                    column++;
                }
                // Сейчас выделенным участком будет пустой участок в конце текста
                WordRange = WordParagraph.Range;
            }
            else {
                //Добавляем пустую таблицу и получаем объект wordtable
                Word.Table wordtable3 = WordDocument.Tables.Add(WordRange, 1, 1);
                wordtable3.Borders.Enable = 1;
            }
            //=================================================================================================================
            WordRange.InsertAfter("Дифференциация слогов\n");

            // Сейчас выделенным участком будет пустой участок в конце текста
            WordRange = WordParagraph.Range;

            // ТАБЛИЦА Дифференциация слогов 
            numRows = 3; // Количество строк в таблице
            numColumns = 4; // Количество столбцов в таблице
            //int column = 1; // Счётчик столбцов
            //Добавляем таблицу и получаем объект wordtable 
            Word.Table wordtable4 = WordDocument.Tables.Add(WordRange, numRows, numColumns);
            wordtable4.Borders.Enable = 1;
            // Таблица ====================================================
            // Статические элементы
            Word.Range wordcellrange4 = WordDocument.Tables[4].Cell(1, 1).Range;
            wordcellrange4.Text = "БА-ПА-БА";
            wordcellrange4 = wordtable4.Cell(2, 1).Range;
            wordcellrange4.Text = "ДА-ДА-ТА";
            wordcellrange4 = wordtable4.Cell(3, 1).Range;
            wordcellrange4.Text = "КА-ГА-ГА";
            wordcellrange4 = wordtable4.Cell(1, 3).Range;
            wordcellrange4.Text = "БИМ-БОМ-БУМ";
            wordcellrange4 = wordtable4.Cell(2, 3).Range;
            wordcellrange4.Text = "ТАП-ТОП-ТУП";
            wordcellrange4 = wordtable4.Cell(3, 3).Range;
            wordcellrange4.Text = "ПА-БА-БА-ПА";
            // Определение выделенных элементов
            foreach (int s in checkedListBox10.CheckedIndices)
            {
                // Если индексы меньше 3, то записывать во 2 столбец, иначе в 4
                if (s < 3)
                {
                    wordcellrange4 = wordtable4.Cell(s+1, 2).Range;
                    wordcellrange4.Text = "——";
                }
                else {
                    wordcellrange4 = wordtable4.Cell(s + 1, 4).Range;
                    wordcellrange4.Text = "——";
                }
            }

            // Сейчас выделенным участком будет пустой участок в конце текста
            WordRange = WordParagraph.Range;

            //=================================================================================================================
            WordRange.InsertAfter("Различие слов, близких по звучанию\n");

            // Сейчас выделенным участком будет пустой участок в конце текста
            WordRange = WordParagraph.Range;

            // ТАБЛИЦА Различие слов, близких по звучанию
            numRows = 2; // Количество строк в таблице
            numColumns = 4; // Количество столбцов в таблице
            //int column = 1; // Счётчик столбцов
            //Добавляем таблицу и получаем объект wordtable 
            Word.Table wordtable5 = WordDocument.Tables.Add(WordRange, numRows, numColumns);
            wordtable5.Borders.Enable = 1;
            // Таблица ====================================================
            // Статические элементы
            Word.Range wordcellrange5 = WordDocument.Tables[5].Cell(1, 1).Range;
            wordcellrange5.Text = "точка-мишка-коса";
            wordcellrange5 = wordtable5.Cell(2, 1).Range;
            wordcellrange5.Text = "трава-дочка-миска";
            wordcellrange5 = wordtable5.Cell(1, 3).Range;
            wordcellrange5.Text = "коза-дрова-точка-мишка";
            wordcellrange5 = wordtable5.Cell(2, 3).Range;
            wordcellrange5.Text = "точка-миска-бочка-трава";

            // Определение выделенных элементов
            foreach (int s in checkedListBox11.CheckedIndices)
            {
                // Если индексы меньше 2, то записывать во второй столбец, иначе в 4
                if (s < 2)
                {
                    wordcellrange5 = wordtable5.Cell(s + 1, 2).Range;
                    wordcellrange5.Text = "——";
                }
                else
                {
                    wordcellrange5 = wordtable5.Cell(s + 1, 4).Range;
                    wordcellrange5.Text = "——";
                }
            }

            // Сейчас выделенным участком будет пустой участок в конце текста
            WordRange = WordParagraph.Range;

            //=================================================================================================================
            WordRange.InsertAfter("Выделение первого и последнего звука в словах\n");

            // Сейчас выделенным участком будет пустой участок в конце текста
            WordRange = WordParagraph.Range;

            // ТАБЛИЦА Выделение первого и последнего звука в словах 
            numRows = 3; // Количество строк в таблице
            numColumns = 4; // Количество столбцов в таблице
            //int column = 1; // Счётчик столбцов
            //Добавляем таблицу и получаем объект wordtable 
            Word.Table wordtable6 = WordDocument.Tables.Add(WordRange, numRows, numColumns);
            wordtable6.Borders.Enable = 1;
            // Таблица ====================================================
            // Статические элементы
            Word.Range wordcellrange6 = WordDocument.Tables[6].Cell(1, 1).Range;
            wordcellrange6.Text = "Алик";
            wordcellrange6 = wordtable6.Cell(2, 1).Range;
            wordcellrange6.Text = "Оля";
            wordcellrange6 = wordtable6.Cell(3, 1).Range;
            wordcellrange6.Text = "Ира";
            wordcellrange6 = wordtable6.Cell(1, 3).Range;
            wordcellrange6.Text = "нос";
            wordcellrange6 = wordtable6.Cell(2, 3).Range;
            wordcellrange6.Text = "пень";
            wordcellrange6 = wordtable6.Cell(3, 3).Range;
            wordcellrange6.Text = "крот";
            // Определение выделенных элементов
            foreach (int s in checkedListBox12.CheckedIndices)
            {
                // Если индексы меньше 3, то записывать во 2 столбец, иначе в 4
                if (s < 3)
                {
                    wordcellrange6 = wordtable6.Cell(s + 1, 2).Range;
                    wordcellrange6.Text = "——";
                }
                else
                {
                    wordcellrange6 = wordtable6.Cell(s + 1, 4).Range;
                    wordcellrange6.Text = "——";
                }
            }
            // Сейчас выделенным участком будет пустой участок в конце текста
            WordRange = WordParagraph.Range;


            WordRange.InsertAfter("Грамматический строй речи: ");
            WordRange.InsertAfter(comboBox15.Text);
            WordRange.InsertAfter("\nСвязная речь: ");
            WordRange.InsertAfter(comboBox17.Text);

            WordRange.InsertAfter("\nЗаключение: ");
            WordRange.InsertAfter(comboBox3.Text);
            WordRange.InsertAfter("\nДинамика речевого развития: _______________________________________________________");

            WordRange.InsertAfter("\nЛогопед: ");
            WordRange.InsertAfter(comboBox20.Text);
        }

        //private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    checkedListBox1.SetItemChecked(0, true);
        //}








        //==END============
    }
}
