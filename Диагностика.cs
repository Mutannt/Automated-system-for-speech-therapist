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

namespace SOA_Client
{
    public partial class Диагностика : Form
    {
        IRest2018 rest_proxy;
        IMyProxy xmlrpc_proxy;

        DataTable table;
        DataRow myrow;
        DataColumn col;
        int id = -1;
        List<ItemComboBox> listGroups;
        List<ItemComboBox> listChildren;
        int IDuser2; // ID пользователя

        private Word.Application WordApp;       // Программа Word
        private Word.Documents WordDocuments;   // Документы
        private Word.Document WordDocument; // Документ
        private Word.Paragraphs WordParagraphs; // Параграфы
        private Word.Paragraph WordParagraph;   // Параграф
        private Word.Range WordRange;		// Выделенный диапазон
        public Диагностика(int IDuser)
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
            // Начальные значения
            comboBox2.SelectedIndex = 7;
            comboBox4.SelectedIndex = 7;
        }

        // При загрузке формы
        private void Диагностика_Load(object sender, EventArgs e)
        {
            // Начальное редактирование DGV2 и DGV3, подсказки
            AddToolsTipText();

            //Формирование таблицы со списком языков
            table = new DataTable();
            col = new DataColumn("ID");
            table.Columns.Add(col);
            col = new DataColumn("FIOchild");
            col.Caption = "ФИО ребёнка";
            table.Columns.Add(col);
            col = new DataColumn("ItogScore1");
            col.Caption = "Итоговый балл1";
            table.Columns.Add(col);
            col = new DataColumn("Violation1");
            col.Caption = "Нарушение1";
            table.Columns.Add(col);
            col = new DataColumn("ItogScore2");
            col.Caption = "Итоговый балл2";
            table.Columns.Add(col);
            col = new DataColumn("Violation2");
            col.Caption = "Нарушение2";
            table.Columns.Add(col);
            col = new DataColumn("NeedsHelp");
            col.Caption = "Нуждается в логопед. помощи";
            table.Columns.Add(col);
            col = new DataColumn("EnrollmentInLogocentre");
            col.Caption = "Зачислен в логопункт";
            table.Columns.Add(col);
            col = new DataColumn("DateEnrollment");
            col.Caption = "Дата зачисления";
            table.Columns.Add(col);
            col = new DataColumn("Releas");
            col.Caption = "Выведен";
            table.Columns.Add(col);
            col = new DataColumn("DateReleas");
            col.Caption = "Дата выведения";
            table.Columns.Add(col);

            //Заполнение таблицы данными
            UpdateGrid();

            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].HeaderText = "ФИО ребёнка";
            dataGridView1.Columns[1].Width = 220;
            dataGridView1.Columns[2].HeaderText = "Итоговый балл в начале";
            dataGridView1.Columns[2].Width = 58;
            dataGridView1.Columns[3].HeaderText = "Заключение в начале";
            dataGridView1.Columns[3].Width = 70;
            dataGridView1.Columns[4].HeaderText = "Итоговый балл в конце";
            dataGridView1.Columns[4].Width = 58;
            dataGridView1.Columns[5].HeaderText = "Заключение в конце";
            dataGridView1.Columns[5].Width = 70;
            dataGridView1.Columns[6].HeaderText = "Нуждается в логопед. помощи";
            dataGridView1.Columns[6].Width = 67;
            dataGridView1.Columns[7].HeaderText = "Зачислен в логопункт";
            dataGridView1.Columns[7].Width = 60;
            dataGridView1.Columns[8].HeaderText = "Дата зачисления";
            dataGridView1.Columns[8].Width = 67;
            dataGridView1.Columns[9].HeaderText = "Выведен";
            dataGridView1.Columns[9].Width = 58;
            dataGridView1.Columns[10].HeaderText = "Дата выведения";
            dataGridView1.Columns[10].Width = 67;

            //dataGridView1.Sort(this.dataGridView1.Columns[0], ListSortDirection.Ascending); // Сортировка

            //Формирование раскрывающегося списка "Группы"           
            listGroups = new List<ItemComboBox>();
            listGroups.Add(new ItemComboBox(0, "Выберите"));

            //В зависимости от выбранного протокола 
            //вызов метода ListListGroups у соответствующего
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

            //Вывод полученного списка групп в раскрывающийся список
            comboBox3.DataSource = listGroups;
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Формирование раскрывающегося списка "Дети"           
            listChildren = new List<ItemComboBox>();

            if ((comboBox3.SelectedItem as ItemComboBox).Id != 0)
            {
                //В зависимости от выбранного протокола 
                //вызов метода ListLogopeds у соответствующего
                //прокси-объекта
                if (MySettings.Default.ProtocolXmlRpc)
                {
                    XMLRPC_Child[] childrenInGroup = xmlrpc_proxy.ListChildrenInGroup((comboBox3.SelectedItem as ItemComboBox).Id);
                    foreach (XMLRPC_Child child in childrenInGroup)
                        listChildren.Add(new ItemComboBox(child.ID, child.FIO));
                }
                else if (MySettings.Default.ProtocolRest)
                {
                    REST_Child[] childrenInGroup = rest_proxy.ListChildrenInGroup((comboBox3.SelectedItem as ItemComboBox).Id);
                    foreach (REST_Child child in childrenInGroup)
                        listChildren.Add(new ItemComboBox(child.ID, child.FIO));
                }
                comboBox1.DataSource = listChildren;
            }
            else {
                comboBox1.DataSource = listChildren;
            }
        }
        // При выборе ребёнка
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //В зависимости от выбранного протокола 
            //вызов метода ListLogopeds у соответствующего
            //прокси-объекта
            if (MySettings.Default.ProtocolXmlRpc)
            {
                XMLRPC_Child child = xmlrpc_proxy.ReadChild((comboBox1.SelectedItem as ItemComboBox).Id);
                // Заполнение даты рождения
                string str = child.DateB.Replace('-', '.');
                dateTimePicker3.Value = Convert.ToDateTime(str);
            }
            else if (MySettings.Default.ProtocolRest)
            {
                REST_Child child = rest_proxy.ReadChild((comboBox1.SelectedItem as ItemComboBox).Id);
                // Заполнение даты рождения
                string str = child.DateB.Replace('-', '.');
                dateTimePicker3.Value = Convert.ToDateTime(str);
            }          
        }

        // Заполнение начальных значений таблицы
        private void AddToolsTipText()
        {
            // Заполнение первых ячеек в dataGridView2
            //dataGridView2.Rows.Add();
            dataGridView2.Rows[0].Cells[0].Value = "Начало";
            dataGridView3.Rows[0].Cells[0].Value = "Конец";

            //Добавить вспывающие подсказки
            for (int j = 2; j <= 3; j++)
            {
                // Звукопроизношение
                (Controls["dataGridView" + j.ToString()] as DataGridView)[1, 0].ToolTipText = "0 - Звукопроизношение не нарушено.\n" +
                    "1 - Нарушено произношение одной группы звуков. \n" +
                    "2 - Нарушено произношение 2-х групп звуков. \n" +
                    "3 - Нарушено произношение 3-х групп звуков. \n" +
                    "4 - Нарушено произношение 4-х и более групп звуков; дефекты звонкости, мягкости; нетрадиционные замены.";
                // Слоговая структура
                (Controls["dataGridView" + j.ToString()] as DataGridView)[2, 0].ToolTipText = "0 - Слоговую структуру слов воспроизводит без ошибок.\n" +
                    "1 - Нарушения редки, главным образом в мало знакомых словах. \n" +
                    "2 - Нарушения в предложениях. В словах - незначительные.\n" +
                    "3 - Нарушения грубые, на уровне слов (упрощения, перестановки, уподобления слогов и тд.) \n" +
                    "4 - Звукокомплексы, звукоподрожания.";
                // Фонематические представления
                (Controls["dataGridView" + j.ToString()] as DataGridView)[3, 0].ToolTipText = "0 - Сформированы соответственно возрасту.\n" +
                    "1 - Самокоррекция или коррекция после стимулируюшей помощи взрослого.\n" +
                    "2 - Только половину заданий на свой возраст выполняет верно.\n" +
                    "3 - Выпоняет правильно только задания для более младшего возраста, с более трудными не справляется.\n" +
                    "4 - Не сформированы. Не может выполнить ни одного задания.";
                // Грамматический строй
                (Controls["dataGridView" + j.ToString()] as DataGridView)[4, 0].ToolTipText = "0 - Грамматические категории использует без затруднений.\n" +
                    "1 - Редкие аграмматизмы.\n" +
                    "2 - Ошибки в совообразовании и словоизменении, но типичные.\n" +
                    "3 - Ошибки многочисленные, стойкие, специфические аграмматизмы, невозможность образовать формы слов.\n" +
                    "4 - Грамматический строй не сформирован по возрасту.";
                // Лексический запас
                (Controls["dataGridView" + j.ToString()] as DataGridView)[5, 0].ToolTipText = "0 - Лексический запас сформирован по возрасту.\n" +
                    "1 - Запас в пределах обихода (обычно номинативный словарь, умение подобрать антонимы).\n" +
                    "2 - Лексический запас беден. Выполняет только половину заданий.\n" +
                    "3 - Лексический запас резко ограничен. Не выпоняет и половины заданий.\n" +
                    "4 - Лексика отсутствует.";
                // Понимание речи
                (Controls["dataGridView" + j.ToString()] as DataGridView)[6, 0].ToolTipText = "0 - В поном объёме.\n" +
                    "1 - Понимание на уровне целостного текста или рассказа. Для ответов на вопросы по смыслу требуется помощь взрослого.\n" +
                    "2 - Понимание грамматических форм, предложно-падежных конструкций, временных и пространственных отношений на уровне фразы.\n" +
                    "3 - Понимание ситуативное, только на уровне знакомых слов.\n" +
                    "4 - Обращённая речь малопонятна для ребёнка, он не может выполнить даже простых поручений.";
                // Связная речь
                (Controls["dataGridView" + j.ToString()] as DataGridView)[7, 0].ToolTipText = "0 - Без затруднений.\n" +
                    "1 - Рассказ бедный. Требуется помощь взросого, наводящие вопросы. Присутствует некоторая смысловая неточность.\n" +
                    "2 - Синтаксические конструкции фраз бедные. Нарушена последовательность в передаче сюжета.\n" +
                    "3 - Простая аграмматичная фраза со структурными нарушениями.\n" +
                    "4 - Связной речи нет.";
            }
        }


        // Обновление основной таблицы
        private void UpdateGrid()
        {
            //Обновление таблицы

            //Очистка таблицы
            table.Clear();

            //Подгрузка в таблицу новых данных
            if (MySettings.Default.ProtocolXmlRpc)
            {
                XMLRPC_Diagnostic[] diagnostics;
                // Если пользователь не администратор
                if (IDuser2 != 1)
                    diagnostics = xmlrpc_proxy.ListDiagnosticsUser(IDuser2);
                else
                    diagnostics = xmlrpc_proxy.ListDiagnostics();

                foreach (XMLRPC_Diagnostic diagnostic in diagnostics)
                {
                    myrow = table.NewRow();
                    myrow["ID"] = diagnostic.ID;
                    myrow["FIOchild"] = diagnostic.FIOchild;
                    myrow["ItogScore1"] = diagnostic.ItogScore1;

                    if (diagnostic.IDvioal1 == 1)
                    {
                        myrow["Violation1"] = "Норма";
                    }
                    else if (diagnostic.IDvioal1 == 2)
                    {
                        myrow["Violation1"] = "НПОЗ";
                    }
                    else if (diagnostic.IDvioal1 == 3)
                    {
                        myrow["Violation1"] = "ФФНР";
                    }
                    else if (diagnostic.IDvioal1 == 4)
                    {
                        myrow["Violation1"] = "ОНР 4";
                    }
                    else if (diagnostic.IDvioal1 == 5)
                    {
                        myrow["Violation1"] = "ОНР 3";
                    }
                    else if (diagnostic.IDvioal1 == 6)
                    {
                        myrow["Violation1"] = "ОНР 2";
                    }
                    else if (diagnostic.IDvioal1 == 7)
                    {
                        myrow["Violation1"] = "ОНР 1";
                    }
                    else if (diagnostic.IDvioal1 == 8)
                    {
                        myrow["Violation1"] = "Не обследован";
                    }

                    myrow["ItogScore2"] = diagnostic.ItogScore2;

                    if (diagnostic.IDvioal2 == 1)
                    {
                        myrow["Violation2"] = "Норма";
                    }
                    else if (diagnostic.IDvioal2 == 2)
                    {
                        myrow["Violation2"] = "НПОЗ";
                    }
                    else if (diagnostic.IDvioal2 == 3)
                    {
                        myrow["Violation2"] = "ФФНР";
                    }
                    else if (diagnostic.IDvioal2 == 4)
                    {
                        myrow["Violation2"] = "ОНР 4";
                    }
                    else if (diagnostic.IDvioal2 == 5)
                    {
                        myrow["Violation2"] = "ОНР 3";
                    }
                    else if (diagnostic.IDvioal2 == 6)
                    {
                        myrow["Violation2"] = "ОНР 2";
                    }
                    else if (diagnostic.IDvioal2 == 7)
                    {
                        myrow["Violation2"] = "ОНР 1";
                    }
                    else if (diagnostic.IDvioal2 == 8)
                    {
                        myrow["Violation2"] = "Не обследован";
                    }

                    if (diagnostic.NeedsHelp == true)
                        myrow["NeedsHelp"] = "Нуждается";
                    else myrow["NeedsHelp"] = "Не нуждается";
                    if (diagnostic.EnrollmentInLogocentre == true)
                        myrow["EnrollmentInLogocentre"] = "Зачислен";
                    else myrow["EnrollmentInLogocentre"] = "Не зачислен";
                    myrow["DateEnrollment"] = diagnostic.DateEnrollment;
                    if (diagnostic.Releas == true)
                        myrow["Releas"] = "Да";
                    else myrow["Releas"] = "Нет";
                    myrow["DateReleas"] = diagnostic.DateReleas;

                    table.Rows.Add(myrow);
                }
            }
            else if (MySettings.Default.ProtocolRest)
            {
                //Подгрузка в таблицу новых данных
                REST_Diagnostic[] diagnostics = rest_proxy.ListDiagnostics();
                foreach (REST_Diagnostic diagnostic in diagnostics)
                {
                    myrow = table.NewRow();
                    myrow["ID"] = diagnostic.ID;
                    myrow["FIOchild"] = diagnostic.FIOchild;
                    myrow["ItogScore1"] = diagnostic.ItogScore1;

                    if (diagnostic.IDvioal1 == 1)
                    {
                        myrow["Violation1"] = "Норма";
                    }
                    else if (diagnostic.IDvioal1 == 2)
                    {
                        myrow["Violation1"] = "НПОЗ";
                    }
                    else if (diagnostic.IDvioal1 == 3)
                    {
                        myrow["Violation1"] = "ФФНР";
                    }
                    else if (diagnostic.IDvioal1 == 4)
                    {
                        myrow["Violation1"] = "ОНР 4";
                    }
                    else if (diagnostic.IDvioal1 == 5)
                    {
                        myrow["Violation1"] = "ОНР 3";
                    }
                    else if (diagnostic.IDvioal1 == 6)
                    {
                        myrow["Violation1"] = "ОНР 2";
                    }
                    else if (diagnostic.IDvioal1 == 7)
                    {
                        myrow["Violation1"] = "ОНР 1";
                    }
                    else if (diagnostic.IDvioal1 == 8)
                    {
                        myrow["Violation1"] = "Не обследован";
                    }

                    myrow["ItogScore2"] = diagnostic.ItogScore2;

                    if (diagnostic.IDvioal2 == 1)
                    {
                        myrow["Violation2"] = "Норма";
                    }
                    else if (diagnostic.IDvioal2 == 2)
                    {
                        myrow["Violation2"] = "НПОЗ";
                    }
                    else if (diagnostic.IDvioal2 == 3)
                    {
                        myrow["Violation2"] = "ФФНР";
                    }
                    else if (diagnostic.IDvioal2 == 4)
                    {
                        myrow["Violation2"] = "ОНР 4";
                    }
                    else if (diagnostic.IDvioal2 == 5)
                    {
                        myrow["Violation2"] = "ОНР 3";
                    }
                    else if (diagnostic.IDvioal2 == 6)
                    {
                        myrow["Violation2"] = "ОНР 2";
                    }
                    else if (diagnostic.IDvioal2 == 7)
                    {
                        myrow["Violation2"] = "ОНР 1";
                    }
                    else if (diagnostic.IDvioal2 == 8)
                    {
                        myrow["Violation2"] = "Не обследован";
                    }

                    if (diagnostic.NeedsHelp == true)
                        myrow["NeedsHelp"] = "Нуждается";
                    else myrow["NeedsHelp"] = "Не нуждается";
                    if (diagnostic.EnrollmentInLogocentre == true)
                        myrow["EnrollmentInLogocentre"] = "Зачислен";
                    else myrow["EnrollmentInLogocentre"] = "Не зачислен";
                    myrow["DateEnrollment"] = diagnostic.DateEnrollment;
                    if (diagnostic.Releas == true)
                        myrow["Releas"] = "Да";
                    else myrow["Releas"] = "Нет";
                    myrow["DateReleas"] = diagnostic.DateReleas;

                    table.Rows.Add(myrow);
                }
            }
            this.dataGridView1.DataSource = table;
        }

        // Очистка формы
        private void ClearForm()
        {
            //Функция очистки формы
            tb_ID.Text = "";
            textBox1.Text = "";
            textBox2.Text = "";
            checkBox1.Checked = true;
            checkBox2.Checked = false;
            checkBox3.Checked = false;
            checkBox4.Checked = false;
            checkBox5.Checked = false;
            checkBox6.Checked = false;
            checkBox7.Checked = false;
            dateTimePicker1.Value = DateTime.Today;
            dateTimePicker2.Value = DateTime.Today;
            comboBox2.SelectedIndex = 7;
            comboBox4.SelectedIndex = 7;
            dataGridView2.Rows.Clear();
            dataGridView3.Rows.Clear();

            // Начальное редактирование DGV2 и DGV3, подсказки
            AddToolsTipText();
        }

        // При окончании выбора баллоа
        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            double itog = Convert.ToInt32(dataGridView2.Rows[0].Cells[1].Value) + Convert.ToInt32(dataGridView2.Rows[0].Cells[2].Value) + Convert.ToInt32(dataGridView2.Rows[0].Cells[3].Value) +
                Convert.ToInt32(dataGridView2.Rows[0].Cells[4].Value) + Convert.ToInt32(dataGridView2.Rows[0].Cells[5].Value) + Convert.ToInt32(dataGridView2.Rows[0].Cells[6].Value) +
                Convert.ToInt32(dataGridView2.Rows[0].Cells[7].Value);
            textBox1.Text = itog.ToString();
        }
        private void dataGridView3_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            double itog = Convert.ToInt32(dataGridView3.Rows[0].Cells[1].Value) + Convert.ToInt32(dataGridView3.Rows[0].Cells[2].Value) + Convert.ToInt32(dataGridView3.Rows[0].Cells[3].Value) +
                Convert.ToInt32(dataGridView3.Rows[0].Cells[4].Value) + Convert.ToInt32(dataGridView3.Rows[0].Cells[5].Value) + Convert.ToInt32(dataGridView3.Rows[0].Cells[6].Value) +
                Convert.ToInt32(dataGridView3.Rows[0].Cells[7].Value);
            textBox2.Text = itog.ToString();
        }
        private void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                //Получение даты
                string date1;
                string date2;
                if (checkBox2.Checked == false)
                    date1 = "0000-00-00";
                else date1 = dateTimePicker1.Value.Year.ToString() + "-" + dateTimePicker1.Value.Month.ToString() + "-" + dateTimePicker1.Value.Day.ToString();
                if (checkBox3.Checked == false)
                    date2 = "0000-00-00";
                else date2 = dateTimePicker2.Value.Year.ToString() + "-" + dateTimePicker2.Value.Month.ToString() + "-" + dateTimePicker2.Value.Day.ToString();
                if (textBox1.Text == "")
                    textBox1.Text = "0";
                if (textBox2.Text == "")
                    textBox2.Text = "0";

                //Сохранение/обновление данных, введённых в форму
                if (MySettings.Default.ProtocolXmlRpc)
                {
                    XMLRPC_Diagnostic diagnostic = new XMLRPC_Diagnostic();
                    diagnostic.IDchild = (comboBox1.SelectedItem as ItemComboBox).Id;
                    diagnostic.ItogScore1 = Convert.ToInt32(textBox1.Text);
                    diagnostic.IDvioal1 = comboBox2.SelectedIndex + 1;
                    diagnostic.ItogScore2 = Convert.ToInt32(textBox2.Text);
                    diagnostic.IDvioal2 = comboBox4.SelectedIndex + 1;
                    diagnostic.NeedsHelp = checkBox1.Checked;
                    diagnostic.SpecialInstitution = checkBox4.Checked;
                    diagnostic.EnrollmentInLogocentre = checkBox2.Checked;
                    diagnostic.DateEnrollment = date1;
                    diagnostic.Releas = checkBox3.Checked;
                    diagnostic.ReleasInSchool = checkBox5.Checked;
                    diagnostic.ReleasOther = checkBox6.Checked;
                    diagnostic.DateReleas = date2;
                    diagnostic.SchoolLogocentre = checkBox7.Checked;

                    if (id < 0)//Если идентификатор не задан или сброшен - 
                    {
                        int IDdiagn = xmlrpc_proxy.CreateDiagnostic(diagnostic); //создание нового/ IDiagn - добавленной записи
                        UpdateGrid(); //Обновление таблицы dataGridView1
                        // ================== Добавление данных в связанную таблицу баллов                 
                        XMLRPC_DiagnosticPoints DiagnosticPoints = new XMLRPC_DiagnosticPoints();
                        DiagnosticPoints.IDdiagn = IDdiagn;
                        DiagnosticPoints.StartEnd = "Start";
                        DiagnosticPoints.SoundPronunciation = Convert.ToInt32(dataGridView2[1, 0].Value);
                        DiagnosticPoints.SyllabicStructure = Convert.ToInt32(dataGridView2[2, 0].Value);
                        DiagnosticPoints.PhonemicRepresentations = Convert.ToInt32(dataGridView2[3, 0].Value);
                        DiagnosticPoints.Grammar = Convert.ToInt32(dataGridView2[4, 0].Value);
                        DiagnosticPoints.LexicalStock = Convert.ToInt32(dataGridView2[5, 0].Value);
                        DiagnosticPoints.SpeechUnderstanding = Convert.ToInt32(dataGridView2[6, 0].Value);
                        DiagnosticPoints.ConnectedSpeech = Convert.ToInt32(dataGridView2[7, 0].Value);
                        xmlrpc_proxy.CreateDiagnosticPoints(DiagnosticPoints);

                        DiagnosticPoints.IDdiagn = IDdiagn;
                        DiagnosticPoints.StartEnd = "End";
                        DiagnosticPoints.SoundPronunciation = Convert.ToInt32(dataGridView3[1, 0].Value);
                        DiagnosticPoints.SyllabicStructure = Convert.ToInt32(dataGridView3[2, 0].Value);
                        DiagnosticPoints.PhonemicRepresentations = Convert.ToInt32(dataGridView3[3, 0].Value);
                        DiagnosticPoints.Grammar = Convert.ToInt32(dataGridView3[4, 0].Value);
                        DiagnosticPoints.LexicalStock = Convert.ToInt32(dataGridView3[5, 0].Value);
                        DiagnosticPoints.SpeechUnderstanding = Convert.ToInt32(dataGridView3[6, 0].Value);
                        DiagnosticPoints.ConnectedSpeech = Convert.ToInt32(dataGridView3[7, 0].Value);
                        xmlrpc_proxy.CreateDiagnosticPoints(DiagnosticPoints);
                    }
                    else//иначе (идентификатор задан) - обновление человека с заданным идентификатором:
                    {
                        xmlrpc_proxy.UpdateDiagnostic(id, diagnostic);
                        UpdateGrid(); //Обновление таблицы dataGridView1
                        // ================== Обновление данных в связанную таблицу баллов                 
                        XMLRPC_DiagnosticPoints DiagnosticPoints = xmlrpc_proxy.GetDiagnosticPoints(id, "Start"); // Получить ID обновляемой записи в связанной таблице
                        int IDdiagnPoints = DiagnosticPoints.ID; // ID обновляемой записи в таблице DiagnosticPoints

                        //XMLRPC_DiagnosticPoints DiagnosticPoints = new XMLRPC_DiagnosticPoints();
                        DiagnosticPoints.IDdiagn = id;
                        DiagnosticPoints.StartEnd = "Start";
                        DiagnosticPoints.SoundPronunciation = Convert.ToInt32(dataGridView2[1, 0].Value);
                        DiagnosticPoints.SyllabicStructure = Convert.ToInt32(dataGridView2[2, 0].Value);
                        DiagnosticPoints.PhonemicRepresentations = Convert.ToInt32(dataGridView2[3, 0].Value);
                        DiagnosticPoints.Grammar = Convert.ToInt32(dataGridView2[4, 0].Value);
                        DiagnosticPoints.LexicalStock = Convert.ToInt32(dataGridView2[5, 0].Value);
                        DiagnosticPoints.SpeechUnderstanding = Convert.ToInt32(dataGridView2[6, 0].Value);
                        DiagnosticPoints.ConnectedSpeech = Convert.ToInt32(dataGridView2[7, 0].Value);
                        xmlrpc_proxy.UpdateDiagnosticPoints(IDdiagnPoints,DiagnosticPoints);

                        DiagnosticPoints.IDdiagn = id;
                        DiagnosticPoints.StartEnd = "End";
                        DiagnosticPoints.SoundPronunciation = Convert.ToInt32(dataGridView3[1, 0].Value);
                        DiagnosticPoints.SyllabicStructure = Convert.ToInt32(dataGridView3[2, 0].Value);
                        DiagnosticPoints.PhonemicRepresentations = Convert.ToInt32(dataGridView3[3, 0].Value);
                        DiagnosticPoints.Grammar = Convert.ToInt32(dataGridView3[4, 0].Value);
                        DiagnosticPoints.LexicalStock = Convert.ToInt32(dataGridView3[5, 0].Value);
                        DiagnosticPoints.SpeechUnderstanding = Convert.ToInt32(dataGridView3[6, 0].Value);
                        DiagnosticPoints.ConnectedSpeech = Convert.ToInt32(dataGridView3[7, 0].Value);
                        xmlrpc_proxy.UpdateDiagnosticPoints(IDdiagnPoints+1,DiagnosticPoints);
                    }
                }
                else if (MySettings.Default.ProtocolRest)
                {
                    if (id < 0)
                    {
                        rest_proxy.CreateDiagnostic(new REST_Diagnostic(
                            (comboBox1.SelectedItem as ItemComboBox).Id,
                            Convert.ToInt32(textBox1.Text),
                            comboBox2.SelectedIndex + 1, // т.к. в базе с единицы , а не с нуля
                            Convert.ToInt32(textBox2.Text),
                            comboBox4.SelectedIndex + 1,
                            checkBox1.Checked,
                            checkBox4.Checked,
                            checkBox2.Checked,
                            date1,
                            checkBox3.Checked,
                            checkBox5.Checked,
                            checkBox6.Checked,
                            date2,
                            checkBox7.Checked
                        ));
                        UpdateGrid(); //Обновление таблицы dataGridView1
                        // Добавление данных в связанную таблицу баллов
                        dataGridView1.Sort(this.dataGridView1.Columns[0], ListSortDirection.Ascending); // Сортировка, чтобы узнать ID последеней добавленной записи
                        int IDdiagn = Convert.ToInt32(dataGridView1.Rows[dataGridView1.Rows.Count - 2].Cells[0].Value);
                        rest_proxy.CreateDiagnosticPoints(new REST_DiagnosticPoints(IDdiagn, "Start", Convert.ToInt32(dataGridView2[1, 0].Value), Convert.ToInt32(dataGridView2[2, 0].Value), Convert.ToInt32(dataGridView2[3, 0].Value), Convert.ToInt32(dataGridView2[4, 0].Value), Convert.ToInt32(dataGridView2[5, 0].Value), Convert.ToInt32(dataGridView2[6, 0].Value), Convert.ToInt32(dataGridView2[7, 0].Value)
                        ));
                        rest_proxy.CreateDiagnosticPoints(new REST_DiagnosticPoints(IDdiagn, "End", Convert.ToInt32(dataGridView3[1, 0].Value), Convert.ToInt32(dataGridView3[2, 0].Value), Convert.ToInt32(dataGridView3[3, 0].Value), Convert.ToInt32(dataGridView3[4, 0].Value), Convert.ToInt32(dataGridView3[5, 0].Value), Convert.ToInt32(dataGridView3[6, 0].Value), Convert.ToInt32(dataGridView3[7, 0].Value)
                        ));
                    }
                    else
                    {
                        rest_proxy.UpdateDiagnostic(id, new REST_Diagnostic(
                            (comboBox1.SelectedItem as ItemComboBox).Id,
                            Convert.ToInt32(textBox1.Text),
                            comboBox2.SelectedIndex + 1, // т.к. в базе с единицы , а не с нуля
                            Convert.ToInt32(textBox2.Text),
                            comboBox4.SelectedIndex + 1,
                            checkBox1.Checked,
                            checkBox4.Checked,
                            checkBox2.Checked,
                            date1,
                            checkBox3.Checked,
                            checkBox5.Checked,
                            checkBox6.Checked,
                            date2,
                            checkBox7.Checked
                        ));
                        UpdateGrid(); //Обновление таблицы
                        // ================== Обновление данных в связанную таблицу баллов                 
                        REST_DiagnosticPoints diagnosticPoints = rest_proxy.GetDiagnosticPointsStart(id); // Получить ID обновляемой записи в связанной таблице
                        int IDdiagnPoints = diagnosticPoints.ID;

                        rest_proxy.UpdateDiagnosticPoints(IDdiagnPoints, new REST_DiagnosticPoints(id, "Start", Convert.ToInt32(dataGridView2[1, 0].Value), Convert.ToInt32(dataGridView2[2, 0].Value), Convert.ToInt32(dataGridView2[3, 0].Value), Convert.ToInt32(dataGridView2[4, 0].Value), Convert.ToInt32(dataGridView2[5, 0].Value), Convert.ToInt32(dataGridView2[6, 0].Value), Convert.ToInt32(dataGridView2[7, 0].Value)
                        ));
                        rest_proxy.UpdateDiagnosticPoints(IDdiagnPoints + 1, new REST_DiagnosticPoints(id, "End", Convert.ToInt32(dataGridView3[1, 0].Value), Convert.ToInt32(dataGridView3[2, 0].Value), Convert.ToInt32(dataGridView3[3, 0].Value), Convert.ToInt32(dataGridView3[4, 0].Value), Convert.ToInt32(dataGridView3[5, 0].Value), Convert.ToInt32(dataGridView3[6, 0].Value), Convert.ToInt32(dataGridView3[7, 0].Value)
                        ));
                        //=====================================
                    }
                }
                //По завершении сохранения:

                if (id >= 0) id = -1; //Сброс идентификатора

                //UpdateGrid(); //Обновление таблицы
                ClearForm(); //Очистка форма
            }
            catch
            {
                MessageBox.Show("Проверьте поля перед сохранением!  Возможно, этот ребёнок уже добавлен.", "Ошибка сохранения", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            //Если идентификатор не задан или сброшен -
            if (id < 0) return; //удалить нельзя, выход.

            //Вывод предупреждения
            if (MessageBox.Show("Действительно удалить?", "Внимание!", MessageBoxButtons.YesNo) == DialogResult.No) return;

            //Собственно удаление
            if (MySettings.Default.ProtocolXmlRpc)
                xmlrpc_proxy.DeleteDiagnostic(id);
            else if (MySettings.Default.ProtocolRest)
                rest_proxy.DeleteDiagnostic(id);

            id = -1; //Сброс идентификатора
            UpdateGrid(); //Обновление таблицы
            ClearForm(); //Очистка формы
        }

        // При выборе записи в основной таблице
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentRow.Index + 1 == dataGridView1.Rows.Count)
            {
                //Если щёлкнули в последней строке таблицы - переход в режим добавления новой записи -
                id = -1; //Сброс идентификатора
                ClearForm(); //Очистка формы
                return; //Выход из обработчика
            }

            //Подгрузка в форму данных выбранного в таблице диагностики
            id = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value); // id диагностики

            if (MySettings.Default.ProtocolXmlRpc)
            {
                XMLRPC_Diagnostic diagnostic = xmlrpc_proxy.ReadDiagnostic(id);

                tb_ID.Text = diagnostic.ID.ToString();
                textBox1.Text = diagnostic.ItogScore1.ToString();
                textBox2.Text = diagnostic.ItogScore2.ToString();
                checkBox1.Checked = diagnostic.NeedsHelp;
                checkBox2.Checked = diagnostic.EnrollmentInLogocentre;
                checkBox3.Checked = diagnostic.Releas;
                checkBox4.Checked = diagnostic.SpecialInstitution;
                checkBox5.Checked = diagnostic.ReleasInSchool;
                checkBox6.Checked = diagnostic.ReleasOther;
                checkBox7.Checked = diagnostic.SchoolLogocentre;

                if (diagnostic.EnrollmentInLogocentre == true)
                {
                    string str = diagnostic.DateEnrollment.Replace('-', '.');
                    dateTimePicker1.Value = Convert.ToDateTime(str);
                }
                else dateTimePicker1.Value = DateTime.Today;

                if (diagnostic.Releas == true)
                {
                    string str2 = diagnostic.DateReleas.Replace('-', '.');
                    dateTimePicker2.Value = Convert.ToDateTime(str2);
                }
                else dateTimePicker2.Value = DateTime.Today;

                XMLRPC_Child child = xmlrpc_proxy.ReadChild(diagnostic.IDchild); // Запись выбранного ребёнка
                // Если эта группа ещё не выбрана, выбрать её///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if ((comboBox3.SelectedItem as ItemComboBox).Id != child.IDgr)
                {
                    //Выделение в раскрывающемся списке Группы
                    foreach (ItemComboBox item in listGroups)
                    {
                        if (item.Id == child.IDgr)
                        {
                            this.comboBox3.SelectedItem = item;
                            break;
                        }
                    }
                }

                //Выделение в раскрывающемся списке ФИО ребёнка
                foreach (ItemComboBox item in listChildren)
                {
                    if (item.Id == diagnostic.IDchild)
                    {
                        this.comboBox1.SelectedItem = item;
                        break;
                    }
                }

                //Выделение в раскрывающемся списке Заключения
                comboBox2.SelectedIndex = diagnostic.IDvioal1 - 1; // т.к. в базе с единицы , а не с нуля
                comboBox4.SelectedIndex = diagnostic.IDvioal2 - 1;

                // Связанная таблица =======================================================================
                XMLRPC_DiagnosticPoints diagnosticPointsStart = xmlrpc_proxy.GetDiagnosticPoints(id, "Start"); // Получить запись в связанной таблице id-ИД диагностики
                XMLRPC_DiagnosticPoints diagnosticPointsEnd = xmlrpc_proxy.GetDiagnosticPoints(id, "End"); // Получить запись в связанной таблице id-ИД диагностики
                //XMLRPC_DiagnosticPoints diagnosticPointsEnd = xmlrpc_proxy.GetDiagnosticPointsEnd2(diagnosticPointsStart.ID + 1); // Получить запись в связанной таблице ID+1

                DataGridViewComboBoxCell dgv2_cbcValue_cell1 = (DataGridViewComboBoxCell)dataGridView2.Rows[0].Cells[1];
                dgv2_cbcValue_cell1.Value = diagnosticPointsStart.SoundPronunciation.ToString(); // Звукопроизношение
                DataGridViewComboBoxCell dgv2_cbcValue_cell2 = (DataGridViewComboBoxCell)dataGridView2.Rows[0].Cells[2];
                dgv2_cbcValue_cell2.Value = diagnosticPointsStart.SyllabicStructure.ToString(); // Слоговая структура
                DataGridViewComboBoxCell dgv2_cbcValue_cell3 = (DataGridViewComboBoxCell)dataGridView2.Rows[0].Cells[3];
                dgv2_cbcValue_cell3.Value = diagnosticPointsStart.PhonemicRepresentations.ToString(); // Фонематические представления
                DataGridViewComboBoxCell dgv2_cbcValue_cell4 = (DataGridViewComboBoxCell)dataGridView2.Rows[0].Cells[4];
                dgv2_cbcValue_cell4.Value = diagnosticPointsStart.Grammar.ToString(); // Граматический строй
                DataGridViewComboBoxCell dgv2_cbcValue_cell5 = (DataGridViewComboBoxCell)dataGridView2.Rows[0].Cells[5];
                dgv2_cbcValue_cell5.Value = diagnosticPointsStart.LexicalStock.ToString(); // Лексический запас
                DataGridViewComboBoxCell dgv2_cbcValue_cell6 = (DataGridViewComboBoxCell)dataGridView2.Rows[0].Cells[6];
                dgv2_cbcValue_cell6.Value = diagnosticPointsStart.SpeechUnderstanding.ToString(); // Понимание речи
                DataGridViewComboBoxCell dgv2_cbcValue_cell7 = (DataGridViewComboBoxCell)dataGridView2.Rows[0].Cells[7];
                dgv2_cbcValue_cell7.Value = diagnosticPointsStart.ConnectedSpeech.ToString(); // Лексический запас

                DataGridViewComboBoxCell dgv3_cbcValue_cell1 = (DataGridViewComboBoxCell)dataGridView3.Rows[0].Cells[1];
                dgv3_cbcValue_cell1.Value = diagnosticPointsEnd.SoundPronunciation.ToString();
                DataGridViewComboBoxCell dgv3_cbcValue_cell2 = (DataGridViewComboBoxCell)dataGridView3.Rows[0].Cells[2];
                dgv3_cbcValue_cell2.Value = diagnosticPointsEnd.SyllabicStructure.ToString();
                DataGridViewComboBoxCell dgv3_cbcValue_cell3 = (DataGridViewComboBoxCell)dataGridView3.Rows[0].Cells[3];
                dgv3_cbcValue_cell3.Value = diagnosticPointsEnd.PhonemicRepresentations.ToString();
                DataGridViewComboBoxCell dgv3_cbcValue_cell4 = (DataGridViewComboBoxCell)dataGridView3.Rows[0].Cells[4];
                dgv3_cbcValue_cell4.Value = diagnosticPointsEnd.Grammar.ToString();
                DataGridViewComboBoxCell dgv3_cbcValue_cell5 = (DataGridViewComboBoxCell)dataGridView3.Rows[0].Cells[5];
                dgv3_cbcValue_cell5.Value = diagnosticPointsEnd.LexicalStock.ToString();
                DataGridViewComboBoxCell dgv3_cbcValue_cell6 = (DataGridViewComboBoxCell)dataGridView3.Rows[0].Cells[6];
                dgv3_cbcValue_cell6.Value = diagnosticPointsEnd.SpeechUnderstanding.ToString();
                DataGridViewComboBoxCell dgv3_cbcValue_cell7 = (DataGridViewComboBoxCell)dataGridView3.Rows[0].Cells[7];
                dgv3_cbcValue_cell7.Value = diagnosticPointsEnd.ConnectedSpeech.ToString();
            }
            else if (MySettings.Default.ProtocolRest)
            {
                REST_Diagnostic diagnostic = rest_proxy.ReadDiagnostic(id);

                tb_ID.Text = diagnostic.ID.ToString();
                textBox1.Text = diagnostic.ItogScore1.ToString();
                textBox2.Text = diagnostic.ItogScore2.ToString();
                checkBox1.Checked = diagnostic.NeedsHelp;
                checkBox2.Checked = diagnostic.EnrollmentInLogocentre;
                checkBox3.Checked = diagnostic.Releas;
                checkBox4.Checked = diagnostic.SpecialInstitution;
                checkBox5.Checked = diagnostic.ReleasInSchool;
                checkBox6.Checked = diagnostic.ReleasOther;
                checkBox7.Checked = diagnostic.SchoolLogocentre;

                if (diagnostic.EnrollmentInLogocentre == true)
                {
                    string str = diagnostic.DateEnrollment.Replace('-', '.');
                    dateTimePicker1.Value = Convert.ToDateTime(str);
                }
                else dateTimePicker1.Value = DateTime.Today;

                if (diagnostic.Releas == true)
                {
                    string str2 = diagnostic.DateReleas.Replace('-', '.');
                    dateTimePicker2.Value = Convert.ToDateTime(str2);
                }
                else dateTimePicker2.Value = DateTime.Today;

                REST_Child child = rest_proxy.ReadChild(diagnostic.IDchild); // Запись выбранного ребёнка
                                                                             // Если эта группа ещё не выбрана, выбрать её///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if ((comboBox3.SelectedItem as ItemComboBox).Id != child.IDgr)
                {
                    //Выделение в раскрывающемся списке Группы
                    foreach (ItemComboBox item in listGroups)
                    {
                        if (item.Id == child.IDgr)
                        {
                            this.comboBox3.SelectedItem = item;
                            break;
                        }
                    }
                }

                //Выделение в раскрывающемся списке ФИО ребёнка
                foreach (ItemComboBox item in listChildren)
                {
                    if (item.Id == diagnostic.IDchild)
                    {
                        this.comboBox1.SelectedItem = item;
                        break;
                    }
                }

                //Выделение в раскрывающемся списке Заключения
                comboBox2.SelectedIndex = diagnostic.IDvioal1 - 1; // т.к. в базе с единицы , а не с нуля
                comboBox4.SelectedIndex = diagnostic.IDvioal2 - 1;

                // Связанная таблица =======================================================================
                REST_DiagnosticPoints diagnosticPointsStart = rest_proxy.GetDiagnosticPointsStart(id); // Получить запись в связанной таблице id-ИД диагностики
                REST_DiagnosticPoints diagnosticPointsEnd = rest_proxy.GetDiagnosticPointsEnd2(diagnosticPointsStart.ID + 1); // Получить запись в связанной таблице ID+1

                DataGridViewComboBoxCell dgv2_cbcValue_cell1 = (DataGridViewComboBoxCell)dataGridView2.Rows[0].Cells[1];
                dgv2_cbcValue_cell1.Value = diagnosticPointsStart.SoundPronunciation.ToString(); // Звукопроизношение
                DataGridViewComboBoxCell dgv2_cbcValue_cell2 = (DataGridViewComboBoxCell)dataGridView2.Rows[0].Cells[2];
                dgv2_cbcValue_cell2.Value = diagnosticPointsStart.SyllabicStructure.ToString(); // Слоговая структура
                DataGridViewComboBoxCell dgv2_cbcValue_cell3 = (DataGridViewComboBoxCell)dataGridView2.Rows[0].Cells[3];
                dgv2_cbcValue_cell3.Value = diagnosticPointsStart.PhonemicRepresentations.ToString(); // Фонематические представления
                DataGridViewComboBoxCell dgv2_cbcValue_cell4 = (DataGridViewComboBoxCell)dataGridView2.Rows[0].Cells[4];
                dgv2_cbcValue_cell4.Value = diagnosticPointsStart.Grammar.ToString(); // Граматический строй
                DataGridViewComboBoxCell dgv2_cbcValue_cell5 = (DataGridViewComboBoxCell)dataGridView2.Rows[0].Cells[5];
                dgv2_cbcValue_cell5.Value = diagnosticPointsStart.LexicalStock.ToString(); // Лексический запас
                DataGridViewComboBoxCell dgv2_cbcValue_cell6 = (DataGridViewComboBoxCell)dataGridView2.Rows[0].Cells[6];
                dgv2_cbcValue_cell6.Value = diagnosticPointsStart.SpeechUnderstanding.ToString(); // Понимание речи
                DataGridViewComboBoxCell dgv2_cbcValue_cell7 = (DataGridViewComboBoxCell)dataGridView2.Rows[0].Cells[7];
                dgv2_cbcValue_cell7.Value = diagnosticPointsStart.ConnectedSpeech.ToString(); // Лексический запас

                DataGridViewComboBoxCell dgv3_cbcValue_cell1 = (DataGridViewComboBoxCell)dataGridView3.Rows[0].Cells[1];
                dgv3_cbcValue_cell1.Value = diagnosticPointsEnd.SoundPronunciation.ToString();
                DataGridViewComboBoxCell dgv3_cbcValue_cell2 = (DataGridViewComboBoxCell)dataGridView3.Rows[0].Cells[2];
                dgv3_cbcValue_cell2.Value = diagnosticPointsEnd.SyllabicStructure.ToString();
                DataGridViewComboBoxCell dgv3_cbcValue_cell3 = (DataGridViewComboBoxCell)dataGridView3.Rows[0].Cells[3];
                dgv3_cbcValue_cell3.Value = diagnosticPointsEnd.PhonemicRepresentations.ToString();
                DataGridViewComboBoxCell dgv3_cbcValue_cell4 = (DataGridViewComboBoxCell)dataGridView3.Rows[0].Cells[4];
                dgv3_cbcValue_cell4.Value = diagnosticPointsEnd.Grammar.ToString();
                DataGridViewComboBoxCell dgv3_cbcValue_cell5 = (DataGridViewComboBoxCell)dataGridView3.Rows[0].Cells[5];
                dgv3_cbcValue_cell5.Value = diagnosticPointsEnd.LexicalStock.ToString();
                DataGridViewComboBoxCell dgv3_cbcValue_cell6 = (DataGridViewComboBoxCell)dataGridView3.Rows[0].Cells[6];
                dgv3_cbcValue_cell6.Value = diagnosticPointsEnd.SpeechUnderstanding.ToString();
                DataGridViewComboBoxCell dgv3_cbcValue_cell7 = (DataGridViewComboBoxCell)dataGridView3.Rows[0].Cells[7];
                dgv3_cbcValue_cell7.Value = diagnosticPointsEnd.ConnectedSpeech.ToString();
            }
        }


        // При изменении итогового бала
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "0" || textBox1.Text == "")
            {
                comboBox2.SelectedIndex = 0; // Норма
                return;
            }
            else if (Convert.ToInt32(textBox1.Text) >= 1 && Convert.ToInt32(textBox1.Text) <= 3)
            {
                comboBox2.SelectedIndex = 1; // НПОЗ
                return;
            }
            else if (Convert.ToInt32(textBox1.Text) >= 4 && Convert.ToInt32(textBox1.Text) <= 6)
            {
                comboBox2.SelectedIndex = 2; // ФФНР
                return;
            }
            else if (Convert.ToInt32(textBox1.Text) >= 7 && Convert.ToInt32(textBox1.Text) <= 12)
            {
                comboBox2.SelectedIndex = 3; // ОНР 4
                return;
            }
            else if (Convert.ToInt32(textBox1.Text) >= 13 && Convert.ToInt32(textBox1.Text) <= 18)
            {
                comboBox2.SelectedIndex = 4; // ОНР 3
                return;
            }
            else if (Convert.ToInt32(textBox1.Text) >= 19 && Convert.ToInt32(textBox1.Text) <= 24)
            {
                comboBox2.SelectedIndex = 5; // ОНР 2
                return;
            }
            else if (Convert.ToInt32(textBox1.Text) >= 25 && Convert.ToInt32(textBox1.Text) <= 28)
            {
                comboBox2.SelectedIndex = 6; // ОНР 1
                return;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text == "0" || textBox2.Text == "")
            {
                comboBox4.SelectedIndex = 0; // Норма
                return;
            }
            else if (Convert.ToInt32(textBox2.Text) >= 1 && Convert.ToInt32(textBox2.Text) <= 3)
            {
                comboBox4.SelectedIndex = 1; // НПОЗ
                return;
            }
            else if (Convert.ToInt32(textBox2.Text) >= 4 && Convert.ToInt32(textBox2.Text) <= 6)
            {
                comboBox4.SelectedIndex = 2; // ФФНР
                return;
            }
            else if (Convert.ToInt32(textBox2.Text) >= 7 && Convert.ToInt32(textBox2.Text) <= 12)
            {
                comboBox4.SelectedIndex = 3; // ОНР 4
                return;
            }
            else if (Convert.ToInt32(textBox2.Text) >= 13 && Convert.ToInt32(textBox2.Text) <= 18)
            {
                comboBox4.SelectedIndex = 4; // ОНР 3
                return;
            }
            else if (Convert.ToInt32(textBox2.Text) >= 19 && Convert.ToInt32(textBox2.Text) <= 24)
            {
                comboBox4.SelectedIndex = 5; // ОНР 2
                return;
            }
            else if (Convert.ToInt32(textBox2.Text) >= 25 && Convert.ToInt32(textBox2.Text) <= 28)
            {
                comboBox4.SelectedIndex = 6; // ОНР 1
                return;
            }
        }

        // Возможно редактировать время только при включенном checkBox
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
                dateTimePicker1.Enabled = true;
            else dateTimePicker1.Enabled = false;
        }
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked == true)
                dateTimePicker2.Enabled = true;
            else dateTimePicker2.Enabled = false;
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
        private void button5_Click(object sender, EventArgs e)
        {
            id = -1; // Сброс идентификатора
            ClearForm(); // Очистка формы
        }

        // Список нуждающихся
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

            // Если ошибка при открытии Ворда
            MessageBox.Show("Если ворд не активирован, дождитесь появления окна с активацией и закройте его." +
                "Только потом нажмите ОК", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);


            // Получаем доступ к объекту первый параграф
            WordParagraph = WordParagraphs[1];
            // Устанавливаем выравнивание по центру
            WordParagraph.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            // Получаем доступ к объекту выделенный участок
            WordRange = WordParagraph.Range;
            // Шрифт
            WordRange.Font.Name = "Times New Roman";
            // Добавим текст в выделенный участок
            WordRange.InsertAfter("Список детей, нуждающихся в помощи логопеда\n");
            // Сделаем шрифт выделенного участка жирным
            WordRange.Font.Bold = 1;
            // Сделаем размер шрифта выделенного участка равным 16
            WordRange.Font.Size = 16;
            // Сбросим выделение участка
            WordRange.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
            // Сейчас выделенным участком будет пустой участок в конце текста
            WordRange = WordParagraph.Range;
            // Добавим текст, он будет выделенным участком.
            WordRange.InsertAfter("по состоянию на " +
               DateTime.Today.ToLongDateString() + "\n");
            // Сделаем шрифт выделенного участка нежирным
            WordRange.Font.Bold = 0;
            // Сделаем размер шрифта выделенного участка равным 14
            WordRange.Font.Size = 14;

            int numRows = dataGridView1.RowCount;
            //MessageBox.Show(numRows.ToString());
            // Цикл по записям таблицы
            int k = 1; // Счётчик записей
            for (int i = 0; i < numRows - 1; i++)
            {
                if (dataGridView1[6, i].Value.ToString() == "Нуждается")
                {
                    // Добавим параграф
                    WordParagraph = WordParagraphs.Add();
                    // Устанавливаем выравнивание по левой границе
                    WordParagraph.Alignment =
                        Word.WdParagraphAlignment.wdAlignParagraphLeft;
                    // Получим доступ к выделенному участку нового параграфа
                    WordRange = WordParagraph.Range;
                    // Установим шрифт выделенного участка нового параграфа
                    WordRange.Font.Bold = 0;
                    WordRange.Font.Size = 14;


                    // Добавим текст в новый параграф
                    WordRange.InsertAfter(Convert.ToString(k + ". " + dataGridView1[1, i].Value.ToString()));
                    k++;
                }
            } // for
        }

        // Список зачисленных
        private void button2_Click(object sender, EventArgs e)
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

            // Если ошибка при открытии Ворда
            MessageBox.Show("Если ворд не активирован, дождитесь появления окна с активацией и закройте его." +
                "Только потом нажмите ОК", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            // Получаем доступ к объекту первый параграф
            WordParagraph = WordParagraphs[1];
            // Устанавливаем выравнивание по центру
            WordParagraph.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            // Получаем доступ к объекту выделенный участок
            WordRange = WordParagraph.Range;
            // Шрифт
            WordRange.Font.Name = "Times New Roman";
            // Добавим текст в выделенный участок
            WordRange.InsertAfter("Список детей, зачисленных в логопункт\n");
            // Сделаем шрифт выделенного участка жирным
            WordRange.Font.Bold = 1;
            // Сделаем размер шрифта выделенного участка равным 16
            WordRange.Font.Size = 16;
            // Сбросим выделение участка
            WordRange.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
            // Сейчас выделенным участком будет пустой участок в конце текста
            WordRange = WordParagraph.Range;
            // Добавим текст, он будет выделенным участком.
            WordRange.InsertAfter("по состоянию на " +
               DateTime.Today.ToLongDateString() + "\n");
            // Сделаем шрифт выделенного участка нежирным
            WordRange.Font.Bold = 0;
            // Сделаем размер шрифта выделенного участка равным 14
            WordRange.Font.Size = 14;


            int numRows = dataGridView1.RowCount;
            int numColumns = 5;

            //Добавляем таблицу и получаем объект wordtable 
            Word.Table wordtable = WordDocument.Tables.Add(WordRange, numRows, numColumns);
            wordtable.Borders.Enable = 1;

            // Шапка таблицы ====================================================
            Word.Range wordcellrange = WordDocument.Tables[1].Cell(1, 1).Range;
            wordcellrange.Bold = 1;
            wordcellrange.Text = "№ п/п";

            wordcellrange = wordtable.Cell(1, 2).Range;
            wordcellrange.Bold = 1;
            wordcellrange.Text = "ФИО";

            wordcellrange = wordtable.Cell(1, 3).Range;
            wordcellrange.Bold = 1;
            wordcellrange.Text = "Дата рождения";

            wordcellrange = wordtable.Cell(1, 4).Range;
            wordcellrange.Bold = 1;
            wordcellrange.Text = "Дата зачисления";

            wordcellrange = wordtable.Cell(1, 5).Range;
            wordcellrange.Bold = 1;
            wordcellrange.Text = "№ группы";

            int k = 2; // Номер заполняемой строчки в таблице  ворда
                       // Заполнение ячеек ====================================================
            for (int i = 1; i < wordtable.Rows.Count; i++)// Первая строка для Шапки таблицы поэтому i=1
            {
                //MessageBox.Show(btf1.dataGridView1[9, i - 1].Value.ToString() + "btf");
                //MessageBox.Show(this.dateTimePicker1.Value.ToShortDateString());
                if (dataGridView1[7, i-1].Value.ToString() == "Зачислен")
                {
                    //Программный клик по строчке
                    dataGridView1.CurrentCell = dataGridView1.Rows[i - 1].Cells[1];
                    dataGridView1_CellClick(this.dataGridView1, new DataGridViewCellEventArgs(0, 0));
                    for (int j = 0; j < wordtable.Columns.Count; j++)
                    {
                        wordcellrange = wordtable.Cell(k, j + 1).Range;
                        //wordcellrange.Text = "Ячейка" + Convert.ToString(i + 1) + " "
                        //                         + Convert.ToString(j + 1);

                        // [Столбец, строка]
                        if (j == 0) // № Номер
                            wordcellrange.Text = i.ToString();
                        if (j == 1) // ФИО
                            wordcellrange.Text = dataGridView1[1, i - 1].Value.ToString();
                        if (j == 2) // Дата рождения
                            wordcellrange.Text = dateTimePicker3.Value.ToShortDateString();
                        if (j == 3) // Дата зачисления
                            wordcellrange.Text = dateTimePicker1.Value.ToShortDateString();
                        if (j == 4) // № группы
                            wordcellrange.Text = comboBox3.SelectedValue.ToString();
                    }
                    k++;
                }
            }
        }

        // Годовой отчёт
        private void button4_Click(object sender, EventArgs e)
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


            // Если ошибка при открытии Ворда
            MessageBox.Show("Если ворд не активирован, дождитесь появления окна с активацией и закройте его."+
                "Только потом нажмите ОК", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);


            // Получаем доступ к объекту первый параграф
            WordParagraph = WordParagraphs[1];
            // Устанавливаем выравнивание по центру
            WordParagraph.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            // Получаем доступ к объекту выделенный участок
            WordRange = WordParagraph.Range;
            WordRange.Font.Name = "Times New Roman";
            // Добавим текст в выделенный участок
            WordRange.InsertAfter("Годовой отчет\nо коррекционно-речевой работе учителя-логопеда\nБиктимирова Мурата\n за 20___ - 20___ учебный год\n");
            // Сделаем шрифт выделенного участка жирным
            WordRange.Font.Bold = 1;
            // Сделаем размер шрифта выделенного участка равным 16
            WordRange.Font.Size = 16;
            // Сбросим выделение участка
            WordRange.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
            // Сейчас выделенным участком будет пустой участок в конце текста
            WordRange = WordParagraph.Range;
            //// Добавим текст, он будет выделенным участком.
            //WordRange.InsertAfter("по состоянию на " +
            //   DateTime.Today.ToLongDateString() + "\n");
            // Сделаем шрифт выделенного участка нежирным
            WordRange.Font.Bold = 0;
            // Сделаем размер шрифта выделенного участка равным 14
            WordRange.Font.Size = 14;

            // Устанавливаем выравнивание по левому краю
            WordParagraph.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            WordParagraph.SpaceAfter = 0;
            WordParagraph.Space1(); // Одинарный интервал
            // Добавим текст, он будет выделенным участком.
            WordRange.InsertAfter("Утверждаю ____________Г.Л.Муликова\nЗаведующий МАДОУ Детский сад № 4\n«Дельфин» МР Учалинский р - н РБ\n____________\n\n");
            // Сейчас выделенным участком будет пустой участок в конце текста
            WordRange = WordParagraph.Range;
            // Сделаем размер шрифта выделенного участка равным 16
            WordRange.Font.Size = 12;

            int numRows = 11; // Количество строк в таблице
            int numColumns = 5; // Количество столбцов в таблице

            //Добавляем таблицу и получаем объект wordtable 
            Word.Table wordtable = WordDocument.Tables.Add(WordRange, numRows, numColumns);
            wordtable.Borders.Enable = 1;

            // Шапка таблицы ====================================================
            Word.Range wordcellrange = WordDocument.Tables[1].Cell(1, 1).Range;
            wordcellrange.Bold = 1;
            wordcellrange.Text = "";

            wordcellrange = wordtable.Cell(1, 2).Range;
            wordcellrange.Bold = 1;
            wordcellrange.Text = "Всего";

            wordcellrange = wordtable.Cell(1, 3).Range;
            wordcellrange.Bold = 1;
            wordcellrange.Text = "ФНР\n(НПОЗ)(дислалия)";

            wordcellrange = wordtable.Cell(1, 4).Range;
            wordcellrange.Bold = 1;
            wordcellrange.Text = "ФФНР\n(сложная дислалия)";

            wordcellrange = wordtable.Cell(1, 5).Range;
            wordcellrange.Bold = 1;
            wordcellrange.Text = "ТНР – ОНР (ОВЗ)\n(дизартрия, ЗПР,\nЗРР, заикание..)";

            // Первый столбец
            // Строка, столбец
            wordcellrange = wordtable.Cell(2, 1).Range;
            wordcellrange.Text = "Обследовано";
            wordcellrange = wordtable.Cell(3, 1).Range;
            wordcellrange.Text = "Выявлено детей с нарушениями речи";
            wordcellrange = wordtable.Cell(4, 1).Range;
            wordcellrange.Text = "Зачислено на логопункт в течение уч.года";
            wordcellrange = wordtable.Cell(5, 1).Range;
            wordcellrange.Text = "Выведено с занятий";
            wordcellrange = wordtable.Cell(6, 1).Range;
            wordcellrange.Text = "Оставлено для продолжения коррекционно-речевых занятий в детском саду";
            wordcellrange = wordtable.Cell(7, 1).Range;
            wordcellrange.Text = "Выпущено в школу";
            wordcellrange = wordtable.Cell(8, 1).Range;
            wordcellrange.Text = "Нуждаются в продолжении коррекционно-речевых занятий в школьном логопункте";
            wordcellrange = wordtable.Cell(9, 1).Range;
            wordcellrange.Text = "Рекомендовано направить в специализированную группу спец-го учреждения";
            wordcellrange = wordtable.Cell(10, 1).Range;
            wordcellrange.Text = "Выбыло по другим причинам";
            wordcellrange = wordtable.Cell(11, 1).Range;
            wordcellrange.Text = "Ожидают зачисления ";


            // Заполнение ячеек ====================================================
            // Всего обследовано
            wordcellrange = wordtable.Cell(2, 2).Range;
            wordcellrange.Text = (dataGridView1.RowCount-1).ToString();

            int AllWithViolations = 0; // Всего с нарушшениями

            if (MySettings.Default.ProtocolXmlRpc)
            {
                // Выявлено с НПОЗ
                XMLRPC_Diagnostic diagnostic1 = xmlrpc_proxy.GetCountNPOZ_FFNR(2); // НПОЗ Idvioal=2
                wordcellrange = wordtable.Cell(3, 3).Range;
                wordcellrange.Text = diagnostic1.Count.ToString();
                AllWithViolations += diagnostic1.Count;
                // Выявлено с ФФНР
                diagnostic1 = xmlrpc_proxy.GetCountNPOZ_FFNR(3); // ФФНР Idvioal=3
                wordcellrange = wordtable.Cell(3, 4).Range;
                wordcellrange.Text = diagnostic1.Count.ToString();
                AllWithViolations += diagnostic1.Count;
                // Выявлено с ОНР
                diagnostic1 = xmlrpc_proxy.GetCountONRs();
                wordcellrange = wordtable.Cell(3, 5).Range;
                wordcellrange.Text = diagnostic1.Count.ToString();
                AllWithViolations += diagnostic1.Count;

                // Для вывода в конце
                int Conclusion = diagnostic1.Count;

                // Выявлено с нарушениями речи ВСЕГО
                wordcellrange = wordtable.Cell(3, 2).Range;
                wordcellrange.Text = AllWithViolations.ToString();

                int AllWithViolations2 = 0; // Всего ЗАЧИСЛЕНО в логопункт
                // Зачислено в логопункт с НПОЗ
                diagnostic1 = xmlrpc_proxy.GetCountInLogocentreNPOZ_FFNR(2); // НПОЗ Idvioal=2
                wordcellrange = wordtable.Cell(4, 3).Range;
                wordcellrange.Text = diagnostic1.Count.ToString();
                AllWithViolations2 += diagnostic1.Count;
                // Зачислено в логопункт с ФФНР
                diagnostic1 = xmlrpc_proxy.GetCountInLogocentreNPOZ_FFNR(3); // ФФНР Idvioal=3
                wordcellrange = wordtable.Cell(4, 4).Range;
                wordcellrange.Text = diagnostic1.Count.ToString();
                AllWithViolations2 += diagnostic1.Count;
                // Зачислено в логопункт с ОНР
                diagnostic1 = xmlrpc_proxy.GetCountInLogocentreONRs();
                wordcellrange = wordtable.Cell(4, 5).Range;
                wordcellrange.Text = diagnostic1.Count.ToString();
                AllWithViolations2 += diagnostic1.Count;
                // Зачислено в логопункт ОБЩЕЕ
                wordcellrange = wordtable.Cell(4, 2).Range;
                wordcellrange.Text = AllWithViolations2.ToString();

                // Выведено с занятий
                diagnostic1 = xmlrpc_proxy.GetCountReleas();
                wordcellrange = wordtable.Cell(5, 2).Range;
                wordcellrange.Text = diagnostic1.Count.ToString();
                // Оставлено для продолжения занятий
                wordcellrange = wordtable.Cell(6, 2).Range;
                wordcellrange.Text = (AllWithViolations2 - diagnostic1.Count).ToString(); // Зачислено - выведено
                                                                                          // Выпущено в школу
                diagnostic1 = xmlrpc_proxy.GetCountReleasInSchool();
                wordcellrange = wordtable.Cell(7, 2).Range;
                wordcellrange.Text = diagnostic1.Count.ToString();
                // Количество детей, нуждающихся в продолжении занятий в школе
                diagnostic1 = xmlrpc_proxy.GetCountSchoolLogocentre();
                wordcellrange = wordtable.Cell(8, 2).Range;
                wordcellrange.Text = diagnostic1.Count.ToString();
                // Количество детей, направленных в спец. учреждение
                diagnostic1 = xmlrpc_proxy.GetCountSpecialInstitution();
                wordcellrange = wordtable.Cell(9, 2).Range;
                wordcellrange.Text = diagnostic1.Count.ToString();
                // Выбыл по др. причинам ОБЩЕЕ
                diagnostic1 = xmlrpc_proxy.GetCountReleasOther();
                wordcellrange = wordtable.Cell(10, 2).Range;
                wordcellrange.Text = diagnostic1.Count.ToString();
                // Ожидают зачисления
                wordcellrange = wordtable.Cell(11, 2).Range;
                wordcellrange.Text = (AllWithViolations - AllWithViolations2).ToString();

                // Сейчас выделенным участком будет пустой участок в конце текста
                WordRange = WordParagraph.Range;
                // Сделаем размер шрифта выделенного участка равным 16
                WordRange.Font.Size = 14;
                // Добавим текст, он будет выделенным участком.
                WordRange.InsertAfter("По заключению ПМПК ограниченные возможности здоровья - ОВЗ имеют " + Conclusion.ToString() + " детей - дети с тяжелыми нарушениями речи - ОНР - ТНР.\n\n" +
                    "Все дети с ОНР - ТНР зачислены на логопункт в первую очередь.");
            }
            else if (MySettings.Default.ProtocolRest)
            {
                // Выявлено с НПОЗ
                REST_Diagnostic diagnostic1 = rest_proxy.GetCountNPOZ();
                wordcellrange = wordtable.Cell(3, 3).Range;
                wordcellrange.Text = diagnostic1.Count.ToString();
                AllWithViolations += diagnostic1.Count;
                // Выявлено с ФФНР
                diagnostic1 = rest_proxy.GetCountFFNR();
                wordcellrange = wordtable.Cell(3, 4).Range;
                wordcellrange.Text = diagnostic1.Count.ToString();
                AllWithViolations += diagnostic1.Count;
                // Выявлено с ОНР
                diagnostic1 = rest_proxy.GetCountONRs();
                wordcellrange = wordtable.Cell(3, 5).Range;
                wordcellrange.Text = diagnostic1.Count.ToString();
                AllWithViolations += diagnostic1.Count;

                // Для вывода в конце
                int Conclusion = diagnostic1.Count;

                // Выявлено с нарушениями речи ВСЕГО
                wordcellrange = wordtable.Cell(3, 2).Range;
                wordcellrange.Text = AllWithViolations.ToString();

                int AllWithViolations2 = 0; // Всего ЗАЧИСЛЕНО в логопункт
                // Зачислено в логопункт с НПОЗ
                diagnostic1 = rest_proxy.GetCountInLogocentreNPOZ();
                wordcellrange = wordtable.Cell(4, 3).Range;
                wordcellrange.Text = diagnostic1.Count.ToString();
                AllWithViolations2 += diagnostic1.Count;
                // Зачислено в логопункт с ФФНР
                diagnostic1 = rest_proxy.GetCountInLogocentreFFNR();
                wordcellrange = wordtable.Cell(4, 4).Range;
                wordcellrange.Text = diagnostic1.Count.ToString();
                AllWithViolations2 += diagnostic1.Count;
                // Зачислено в логопункт с ОНР
                diagnostic1 = rest_proxy.GetCountInLogocentreONRs();
                wordcellrange = wordtable.Cell(4, 5).Range;
                wordcellrange.Text = diagnostic1.Count.ToString();
                AllWithViolations2 += diagnostic1.Count;
                // Зачислено в логопункт ОБЩЕЕ
                wordcellrange = wordtable.Cell(4, 2).Range;
                wordcellrange.Text = AllWithViolations2.ToString();

                // Выведено с занятий
                diagnostic1 = rest_proxy.GetCountReleas();
                wordcellrange = wordtable.Cell(5, 2).Range;
                wordcellrange.Text = diagnostic1.Count.ToString();
                // Оставлено для продолжения занятий
                wordcellrange = wordtable.Cell(6, 2).Range;
                wordcellrange.Text = (AllWithViolations2 - diagnostic1.Count).ToString(); // Зачислено - выведено
                                                                                          // Выпущено в школу
                diagnostic1 = rest_proxy.GetCountReleasInSchool();
                wordcellrange = wordtable.Cell(7, 2).Range;
                wordcellrange.Text = diagnostic1.Count.ToString();
                // Количество детей, нуждающихся в продолжении занятий в школе
                diagnostic1 = rest_proxy.GetCountSchoolLogocentre();
                wordcellrange = wordtable.Cell(8, 2).Range;
                wordcellrange.Text = diagnostic1.Count.ToString();
                // Количество детей, направленных в спец. учреждение
                diagnostic1 = rest_proxy.GetCountSpecialInstitution();
                wordcellrange = wordtable.Cell(9, 2).Range;
                wordcellrange.Text = diagnostic1.Count.ToString();
                // Выбыл по др. причинам ОБЩЕЕ
                diagnostic1 = rest_proxy.GetCountReleasOther();
                wordcellrange = wordtable.Cell(10, 2).Range;
                wordcellrange.Text = diagnostic1.Count.ToString();
                // Ожидают зачисления
                wordcellrange = wordtable.Cell(11, 2).Range;
                wordcellrange.Text = (AllWithViolations - AllWithViolations2).ToString();

                // Сейчас выделенным участком будет пустой участок в конце текста
                WordRange = WordParagraph.Range;
                // Сделаем размер шрифта выделенного участка равным 16
                WordRange.Font.Size = 14;
                // Добавим текст, он будет выделенным участком.
                WordRange.InsertAfter("По заключению ПМПК ограниченные возможности здоровья - ОВЗ имеют " + Conclusion.ToString() + " детей - дети с тяжелыми нарушениями речи - ОНР - ТНР.\n\n" +
                    "Все дети с ОНР - ТНР зачислены на логопункт в первую очередь.");
            }  
        }

        // Диаграмма
        private void button3_Click(object sender, EventArgs e)
        {
            Diagrams frm = new Diagrams();
            frm.ShowDialog();
        }
        
        //===========================================
    }
}
