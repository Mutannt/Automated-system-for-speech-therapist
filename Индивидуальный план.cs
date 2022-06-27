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
using System.Reflection;

namespace SOA_Client
{
    public partial class Индивидуальный_план : Form
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
        int IDuser2; // ID пользователя
        public Индивидуальный_план(int IDuser)
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
        }
        private void Индивидуальный_план_Load(object sender, EventArgs e)
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
            listGroups.Add(new ItemComboBox(0, "Выберите"));

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

            //Заполнение часто встречающихся вариантов
            checkedListBox3.SetItemChecked(3, true);
            checkedListBox4.SetItemChecked(4, true);
            checkedListBox4.SetItemChecked(5, true);
            checkedListBox4.SetItemChecked(6, true);
            checkedListBox5.SetItemChecked(2, true);
            checkedListBox5.SetItemChecked(3, true);
        }

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
                comboBox1.DataSource = listChildren;
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
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((comboBox3.SelectedIndex == 3)|| (comboBox3.SelectedIndex == 4)|| (comboBox3.SelectedIndex == 5)|| (comboBox3.SelectedIndex == 6))
            {
                // Если диагнозы - ОНРы
                richTextBox1.Text = "Подготовительный этап на фоне медикаментозного воздействия, физиотерапии, лечебной физкультуры, логопедического массажа, " +
                    "логоритмики, нетрадиционных форм воздействия (ароматерапии, криотерапии, тестотерапии, арттерапии, и др.)\n" +
                    "- установить контакт с ребенком, формировать потребность в речевом общении, формировать интерес к логопедическим занятиям, потребность в них;\n" +
                    "- уточнить и развивать пассивный и активный словарь;\n" +
                    "- развивать произвольное внимание, память, мышление;\n" +
                    "- развивать речевое дыхание;\n" +
                    "- развивать слуховое восприятие и сенсорные функции;\n" +
                    "- коррекция голоса;\n" +
                    "- развивать ощущения артикуляторных поз и движений;\n" +
                    "- развивать подвижность мышц речевого аппарата, лицевой и мимической мускулатуры;\n" +
                    "- развивать фонематическое восприятие и сенсорные функции;\n" +
                    "- развивать тонкую моторику в процессе систематических тренировок, пальчиковой гимнастики;\n" +
                    "- укреплять физическое здоровье, консультации врачей, лечение;";
            }
            else if (comboBox3.SelectedIndex == 1) {
                // Если диагноз - НПОЗ
                richTextBox1.Text = "- установить контакт с ребенком, формировать интерес к логопедическим занятиям, потребность в них;\n" +
                    "- развивать произвольное внимание, память, мышление;\n" +
                    "- развивать речевое дыхание;\n" +
                    "- развивать осознанный звуковой анализ и навык контроля за произношением;\n" +
                    "- развивать артикуляторную моторику.";
            }
            else if (comboBox3.SelectedIndex == 2) {
                // Если диагноз - ФФНР
                richTextBox1.Text = "- установить контакт с ребенком, формировать интерес к логопедическим занятиям, потребность в них;\n" +
                    "- развивать произвольное внимание, память, мышление;\n" +
                    "- развивать артикуляторную моторику;\n" +
                    "- развивать фонематическое восприятие;\n" +
                    "- развивать осознанный звуковой анализ и навык контроля за произношением;\n" +
                    "- развивать тонкую моторику в процессе систематических тренировок, пальчиковой гимнастики;";
            }
            else
            {
                // Если диагноз - НОРМА
                richTextBox1.Text = "";
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
                XMLRPC_IndividPlan[] individplans;
                // Если пользователь не администратор
                if (IDuser2 != 1)
                    individplans = xmlrpc_proxy.ListIndividPlansUser(IDuser2);
                else
                    individplans = xmlrpc_proxy.ListIndividPlans();
                foreach (XMLRPC_IndividPlan individplan in individplans)
                {
                    myrow = table.NewRow();
                    myrow["ID"] = individplan.ID;
                    myrow["FIO"] = individplan.FIO;
                    table.Rows.Add(myrow);
                }
            }
            else if (MySettings.Default.ProtocolRest)
            {
                REST_IndividPlan[] individplans = rest_proxy.ListIndividPlans();
                foreach (REST_IndividPlan individplan in individplans)
                {
                    myrow = table.NewRow();
                    myrow["ID"] = individplan.ID;
                    myrow["FIO"] = individplan.FIO;
                    table.Rows.Add(myrow);
                }
            }
            this.dataGridView1.DataSource = table;
        }
        private void ClearForm()
        {
            tb_ID.Text = "";

            //Функция очистки формы
            // Очистка checkedListBox - сов
            for (int i = 1; i < 6; i++)
            {
                (Controls["checkedListBox" + i.ToString()] as CheckedListBox).ClearSelected(); // Снимает синее выделение
                for (int j = 0; j < (Controls["checkedListBox" + i.ToString()] as CheckedListBox).Items.Count; j++)
                {
                    (Controls["checkedListBox" + i.ToString()] as CheckedListBox).SetItemChecked(j, false); // Снимает галочки
                }
            }

            // Выбор текущей даты
            dateTimePicker1.Value = DateTime.Today;
        }

        // Добавить/Обновить
        private void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                // Получение индексов нажатых checkBoxes и добавление их в строку
                string SettingSounds = "";
                foreach (int s in checkedListBox1.CheckedIndices)
                    SettingSounds = SettingSounds + s.ToString() + " ";
                // Удаление последнего пробела
                if (SettingSounds.Length != 0)
                    SettingSounds = SettingSounds.Remove(SettingSounds.Length - 1);

                // Получение индексов нажатых checkBoxes и добавление их в строку
                string SoundDifferentiation = "";
                foreach (int s in checkedListBox2.CheckedIndices)
                    SoundDifferentiation = SoundDifferentiation + s.ToString() + " ";
                // Удаление последнего пробела
                if (SoundDifferentiation.Length != 0)
                    SoundDifferentiation = SoundDifferentiation.Remove(SoundDifferentiation.Length - 1);

                // Получение индексов нажатых checkBoxes и добавление их в строку
                string VocabularyEnrichment = "";
                foreach (int s in checkedListBox3.CheckedIndices)
                    VocabularyEnrichment = VocabularyEnrichment + s.ToString() + " ";
                // Удаление последнего пробела
                if (VocabularyEnrichment.Length != 0)
                    VocabularyEnrichment = VocabularyEnrichment.Remove(VocabularyEnrichment.Length - 1);

                // Получение индексов нажатых checkBoxes и добавление их в строку
                string DevelopmentGrammatical = "";
                foreach (int s in checkedListBox4.CheckedIndices)
                    DevelopmentGrammatical = DevelopmentGrammatical + s.ToString() + " ";
                // Удаление последнего пробела
                if (DevelopmentGrammatical.Length != 0)
                    DevelopmentGrammatical = DevelopmentGrammatical.Remove(DevelopmentGrammatical.Length - 1);

                // Получение индексов нажатых checkBoxes и добавление их в строку
                string FormationCoherentSpeech = "";
                foreach (int s in checkedListBox5.CheckedIndices)
                    FormationCoherentSpeech = FormationCoherentSpeech + s.ToString() + " ";
                // Удаление последнего пробела
                if (FormationCoherentSpeech.Length != 0)
                    FormationCoherentSpeech = FormationCoherentSpeech.Remove(FormationCoherentSpeech.Length - 1);


                //Сохранение/обновление данных, введённых в форму
                if (MySettings.Default.ProtocolXmlRpc)
                {
                    XMLRPC_IndividPlan individplan = new XMLRPC_IndividPlan();
                    individplan.SettingSounds = SettingSounds;
                    individplan.SoundDifferentiation = SoundDifferentiation;
                    individplan.VocabularyEnrichment = VocabularyEnrichment;
                    individplan.DevelopmentGrammatical = DevelopmentGrammatical;
                    individplan.FormationCoherentSpeech = FormationCoherentSpeech;
                    individplan.IDchild = (comboBox2.SelectedItem as ItemComboBox).Id;

                    if (id < 0) //Если идентификатор не задан или сброшен - 
                        xmlrpc_proxy.CreateIndividPlan(individplan);
                    else //иначе (идентификатор задан) -
                        xmlrpc_proxy.UpdateIndividPlan(id, individplan);
                }
                else if (MySettings.Default.ProtocolRest)
                {
                    if (id < 0)
                        rest_proxy.CreateIndividPlan(new REST_IndividPlan(
                                SettingSounds,
                                SoundDifferentiation,
                                VocabularyEnrichment,
                                DevelopmentGrammatical,
                                FormationCoherentSpeech,
                                (comboBox2.SelectedItem as ItemComboBox).Id
                        ));
                    else
                        rest_proxy.UpdateIndividPlan(id, new REST_IndividPlan(
                                SettingSounds,
                                SoundDifferentiation,
                                VocabularyEnrichment,
                                DevelopmentGrammatical,
                                FormationCoherentSpeech,
                                (comboBox2.SelectedItem as ItemComboBox).Id
                        ));
                }

                //По завершении сохранения:
                if (id >= 0) id = -1; //Сброс идентификатора

                UpdateGrid(); //Обновление таблицы
                ClearForm(); //Очистка форма
            }
            catch
            {
                MessageBox.Show("Проверьте поля перед сохранением! Возможно, этот ребёнок уже добавлен.", "Ошибка сохранения", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Удалить
        private void btn_Delete_Click(object sender, EventArgs e)
        {
            //Если идентификатор не задан или сброшен -
            if (id < 0) return; //удалить нельзя, выход.

            //Вывод предупреждения
            if (MessageBox.Show("Действительно удалить?", "", MessageBoxButtons.YesNo) == DialogResult.No) return;

            //Собственно удаление
            if (MySettings.Default.ProtocolXmlRpc)
                xmlrpc_proxy.DeleteIndividPlan(id);
            else if (MySettings.Default.ProtocolRest)
                rest_proxy.DeleteIndividPlan(id);

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
                XMLRPC_IndividPlan individplan = xmlrpc_proxy.ReadIndividPlan(id);

                tb_ID.Text = individplan.ID.ToString();

                if (individplan.SettingSounds != "")
                {
                    // Разделение строки
                    string[] settingSounds = individplan.SettingSounds.Split(new char[] { ' ' });
                    foreach (string s in settingSounds)
                    {
                        checkedListBox1.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                if (individplan.SoundDifferentiation != "")
                {
                    // Разделение строки
                    string[] sounDifferentiations = individplan.SoundDifferentiation.Split(new char[] { ' ' });
                    foreach (string s in sounDifferentiations)
                    {
                        checkedListBox2.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                if (individplan.VocabularyEnrichment != "")
                {
                    // Разделение строки
                    string[] vocabularyEnrichments = individplan.VocabularyEnrichment.Split(new char[] { ' ' });
                    foreach (string s in vocabularyEnrichments)
                    {
                        checkedListBox3.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                if (individplan.DevelopmentGrammatical != "")
                {
                    // Разделение строки
                    string[] developmentGrammaticals = individplan.DevelopmentGrammatical.Split(new char[] { ' ' });
                    foreach (string s in developmentGrammaticals)
                    {
                        checkedListBox4.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                if (individplan.FormationCoherentSpeech != "")
                {
                    // Разделение строки
                    string[] formationCoherentSpeechs = individplan.FormationCoherentSpeech.Split(new char[] { ' ' });
                    foreach (string s in formationCoherentSpeechs)
                    {
                        checkedListBox5.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                //Подгрузка в форму данных выбранного в таблице
                XMLRPC_Child child = xmlrpc_proxy.ReadChild(individplan.IDchild); // Запись выбранного ребёнка
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
                    if (item.Id == individplan.IDchild)
                    {
                        comboBox2.SelectedItem = item;
                        break;
                    }

                }

            }
            else if (MySettings.Default.ProtocolRest)
            {
                REST_IndividPlan individplan = rest_proxy.ReadIndividPlan(id);

                tb_ID.Text = individplan.ID.ToString();

                if (individplan.SettingSounds != "")
                {
                    // Разделение строки
                    string[] settingSounds = individplan.SettingSounds.Split(new char[] { ' ' });
                    foreach (string s in settingSounds)
                    {
                        checkedListBox1.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                if (individplan.SoundDifferentiation != "")
                {
                    // Разделение строки
                    string[] sounDifferentiations = individplan.SoundDifferentiation.Split(new char[] { ' ' });
                    foreach (string s in sounDifferentiations)
                    {
                        checkedListBox2.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                if (individplan.VocabularyEnrichment != "")
                {
                    // Разделение строки
                    string[] vocabularyEnrichments = individplan.VocabularyEnrichment.Split(new char[] { ' ' });
                    foreach (string s in vocabularyEnrichments)
                    {
                        checkedListBox3.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                if (individplan.DevelopmentGrammatical != "")
                {
                    // Разделение строки
                    string[] developmentGrammaticals = individplan.DevelopmentGrammatical.Split(new char[] { ' ' });
                    foreach (string s in developmentGrammaticals)
                    {
                        checkedListBox4.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                if (individplan.FormationCoherentSpeech != "")
                {
                    // Разделение строки
                    string[] formationCoherentSpeechs = individplan.FormationCoherentSpeech.Split(new char[] { ' ' });
                    foreach (string s in formationCoherentSpeechs)
                    {
                        checkedListBox5.SetItemChecked(Convert.ToInt32(s), true);
                    }
                }

                //Подгрузка в форму данных выбранного в таблице
                REST_Child child = rest_proxy.ReadChild(individplan.IDchild); // Запись выбранного ребёнка
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
                    if (item.Id == individplan.IDchild)
                    {
                        comboBox2.SelectedItem = item;
                        break;
                    }
                }
            }
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
            WordRange.Font.Size = 14;
            // Шрифт
            WordRange.Font.Name = "Times New Roman";
            // Добавим текст в выделенный участок
            WordRange.InsertAfter("План  логопедической индивидуальной образовательной траектории развития на _________________________ учебный  год\n");
            // Сбросим выделение участка
            WordRange.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

            // Получаем доступ к объекту 2й параграф
            WordParagraph = WordParagraphs[2];
            // Устанавливаем выравнивание по центру
            WordParagraph.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            // Сейчас выделенным участком будет пустой участок в конце текста
            WordRange = WordParagraph.Range;
            WordRange.Font.Bold = 0;
            // Добавим текст в выделенный участок
            WordRange.InsertAfter("Фамилия, имя ребенка: ");
            WordRange.InsertAfter(comboBox2.Text);

            WordRange.InsertAfter("\nГруппа: ");
            WordRange.InsertAfter(comboBox1.Text);

            WordRange.InsertAfter("\nДата рождения ребенка: ");
            WordRange.InsertAfter(dateTimePicker1.Value.ToShortDateString());

            WordRange.InsertAfter("\nЛогопедическое заключение: ");
            WordRange.InsertAfter(comboBox3.Text);

            
            WordRange.InsertAfter("\n1. Подготовительный этап\n");

            //WordApp.Selection.SetRange(236, 260);
            //WordApp.Selection.Font.Bold = 1;

            // обьектные строки для Word
            //object strToFindObj = strToFind;
            //Word.Find wordFindObj = WordRange.Find;
            WordApp.Selection.Find.Text = "1. Подготовительный этап";
            //WordApp.Selection.Find.Font.Bold = 1;
            //WordApp.Selection.Find.

            // Сделаем шрифт выделенного участка не жирным
            WordRange.InsertAfter(richTextBox1.Text + "\n");

            //WordRange.Font.Bold = 1;
            // Добавим текст в выделенный участок
            WordRange.InsertAfter("\n2. Формирование произносительных умений и навыков" + "\nПостановка звуков\n");
            //WordRange.Font.Bold = 0;
            string str="";
            // Получение значений нажатых checkBoxes и добавление их в строку
            foreach (string s in checkedListBox1.CheckedItems)
                str = str + s.ToString() + "\n";
            WordRange.InsertAfter(str);

            // Добавим текст в выделенный участок
            WordRange.InsertAfter("\nАвтоматизация  поставленных  звуков  в слогах словах предложениях, в связном тексте." + "\nДифференциация звуков\n");
            str = "";
            // Получение значений нажатых checkBoxes и добавление их в строку
            foreach (string s in checkedListBox2.CheckedItems)
                str = str + s.ToString() + ", ";
            WordRange.InsertAfter(str);

            // Добавим текст в выделенный участок
            WordRange.InsertAfter("\n3. Формирование фонематического восприятия и навыков звукового анализа и синтеза" +
                "\n4.Работа над слоговой структурой речи" + "\n5.Обогащение словарного запаса\n");
            str = "";
            // Получение значений нажатых checkBoxes и добавление их в строку
            foreach (string s in checkedListBox3.CheckedItems)
                str = str + s.ToString() + "\n";
            WordRange.InsertAfter(str);

            // Добавим текст в выделенный участок
            WordRange.InsertAfter("\n6. Развитие грамматического строя речи\n");
            str = "";
            // Получение значений нажатых checkBoxes и добавление их в строку
            foreach (string s in checkedListBox4.CheckedItems)
                str = str + s.ToString() + "\n";
            WordRange.InsertAfter(str);

            // Добавим текст в выделенный участок
            WordRange.InsertAfter("\n7. Формирование связной речи\n");
            str = "";
            // Получение значений нажатых checkBoxes и добавление их в строку
            foreach (string s in checkedListBox5.CheckedItems)
                str = str + s.ToString() + "\n";
            WordRange.InsertAfter(str);

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

        // Очистить поля
        private void button2_Click(object sender, EventArgs e)
        {
            id = -1; // Сброс идентификатора
            ClearForm(); // Очистка формы
            //Заполнение часто встречающихся вариантов
            checkedListBox3.SetItemChecked(3, true);
            checkedListBox4.SetItemChecked(4, true);
            checkedListBox4.SetItemChecked(5, true);
            checkedListBox4.SetItemChecked(6, true);
            checkedListBox5.SetItemChecked(2, true);
            checkedListBox5.SetItemChecked(3, true);
        }














        // END
    }
}
