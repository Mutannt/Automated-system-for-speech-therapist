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

namespace SOA_Client
{
    public partial class Дети : Form
    {
        IRest2018 rest_proxy;
        IMyProxy xmlrpc_proxy;

        DataTable table;
        DataRow myrow;
        DataColumn col;
        int id = -1;
        List<ItemComboBox> list;
        public Дети(int IDuser)
        {
            InitializeComponent();
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

        private void Дети_Load(object sender, EventArgs e)
        {
            //Формирование таблицы со списком детей
            table = new DataTable();
            col = new DataColumn("ID");
            table.Columns.Add(col);
            col = new DataColumn("FIO");
            col.Caption = "ФИО ребёнка";
            table.Columns.Add(col);
            col = new DataColumn("DateB");
            col.Caption = "Дата рождения";
            table.Columns.Add(col);
            col = new DataColumn("FIOMam");
            col.Caption = "ФИО мамы";
            table.Columns.Add(col);
            col = new DataColumn("TelMam");
            col.Caption = "Телефон мамы";
            table.Columns.Add(col);
            col = new DataColumn("FioPap");
            col.Caption = "ФИО папы";
            table.Columns.Add(col);
            col = new DataColumn("TelPap");
            col.Caption = "Телефон папы";
            table.Columns.Add(col);
            col = new DataColumn("Email");
            col.Caption = "Email";
            table.Columns.Add(col);
            col = new DataColumn("NumberGr");
            col.Caption = "Номер группы";
            table.Columns.Add(col);
            //Заполнение таблицы данными
            UpdateGrid();

            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].HeaderText = "ФИО ребёнка";
            dataGridView1.Columns[1].Width = 230;
            dataGridView1.Columns[2].HeaderText = "Дата рождения";
            dataGridView1.Columns[2].Width = 67;
            dataGridView1.Columns[3].HeaderText = "ФИО мамы";
            dataGridView1.Columns[4].HeaderText = "Телефон мамы";
            dataGridView1.Columns[4].Width = 75;
            dataGridView1.Columns[5].HeaderText = "ФИО папы";
            dataGridView1.Columns[6].HeaderText = "Телефон папы";
            dataGridView1.Columns[6].Width = 75;
            dataGridView1.Columns[8].HeaderText = "Номер группы";
            dataGridView1.Columns[8].Width = 50;

            //Формирование раскрывающегося списка "Группы"           
            list = new List<ItemComboBox>();

            //В зависимости от выбранного протокола 
            //вызов метода ListLogopeds у соответствующего
            //прокси-объекта
            if (MySettings.Default.ProtocolXmlRpc)
            {
                XMLRPC_Group[] groups = xmlrpc_proxy.ListGroups();
                foreach (XMLRPC_Group group in groups)
                    list.Add(new ItemComboBox(Convert.ToInt32(group.IDgr), group.NumberGr.ToString()));
            }
            else if (MySettings.Default.ProtocolRest)
            {
                REST_Group[] groups = rest_proxy.ListGroups();
                foreach (REST_Group group in groups)
                    list.Add(new ItemComboBox(Convert.ToInt32(group.IDgr), group.NumberGr.ToString()));
            }

            //Вывод полученного списка групп в раскрывающийся список
            comboBox1.DataSource = list;
        }
        private void UpdateGrid()
        {
            //Обновление таблицы

            //Очистка таблицы
            table.Clear();


            //Подгрузка в таблицу новых данных
            if (MySettings.Default.ProtocolXmlRpc)
            {
                XMLRPC_Child[] children = xmlrpc_proxy.ListChildren();

                foreach (XMLRPC_Child child in children)
                {
                    myrow = table.NewRow();
                    myrow["ID"] = child.ID;
                    myrow["FIO"] = child.FIO;
                    myrow["DateB"] = child.DateB;
                    myrow["FIOMam"] = child.FIOMam;
                    myrow["TelMam"] = child.TelMam;
                    myrow["FioPap"] = child.FioPap;
                    myrow["TelPap"] = child.TelPap;
                    myrow["Email"] = child.Email;
                    myrow["NumberGr"] = child.NumberGr;

                    table.Rows.Add(myrow);
                }
            }
            else if (MySettings.Default.ProtocolRest)
            {
                REST_Child[] children = rest_proxy.ListChildren();
                foreach (REST_Child child in children)
                {
                    myrow = table.NewRow();
                    myrow["ID"] = child.ID;
                    myrow["FIO"] = child.FIO;
                    myrow["DateB"] = child.DateB;
                    myrow["FIOMam"] = child.FIOMam;
                    myrow["TelMam"] = child.TelMam;
                    myrow["FioPap"] = child.FioPap;
                    myrow["TelPap"] = child.TelPap;
                    myrow["Email"] = child.Email;
                    myrow["NumberGr"] = child.NumberGr;

                    table.Rows.Add(myrow);
                }
            }
            this.dataGridView1.DataSource = table;
        }
        private void ClearForm()
        {
            //Функция очистки формы
            tb_ID.Text = "";
            textBox1.Text = "";
            dateTimePicker1.Value = DateTime.Today;
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            textBox8.Text = "";
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            //Получение даты
            string date1 = dateTimePicker1.Value.Year.ToString() + "-" + dateTimePicker1.Value.Month.ToString() + "-" + dateTimePicker1.Value.Day.ToString();
            //Сохранение данных, введённых в форму
            if (MySettings.Default.ProtocolXmlRpc)
            {
                XMLRPC_Child child = new XMLRPC_Child();
                child.FIO = textBox1.Text;
                child.DateB = date1;
                child.FIOMam = textBox4.Text;
                child.TelMam = textBox5.Text;
                child.FioPap = textBox6.Text;
                child.TelPap = textBox7.Text;
                child.Email = textBox8.Text;
                child.IDgr = (comboBox1.SelectedItem as ItemComboBox).Id;

                if (id < 0) //Если идентификатор не задан или сброшен - 
                    xmlrpc_proxy.CreateChild(child);
                else //иначе (идентификатор задан) -
                    xmlrpc_proxy.UpdateChild(id, child);
            }
            else if (MySettings.Default.ProtocolRest)
            {
                if (id < 0)
                    rest_proxy.CreateChild(new REST_Child(
                            textBox1.Text,
                            date1,
                            textBox4.Text,
                            textBox5.Text,
                            textBox6.Text,
                            textBox7.Text,
                            textBox8.Text,
                            (comboBox1.SelectedItem as ItemComboBox).Id
                    ));
                else
                    rest_proxy.UpdateChild(id, new REST_Child(
                            textBox1.Text,
                            date1,
                            textBox4.Text,
                            textBox5.Text,
                            textBox6.Text,
                            textBox7.Text,
                            textBox8.Text,
                            (comboBox1.SelectedItem as ItemComboBox).Id
                    ));
            }

            //По завершении сохранения:
            if (id >= 0) id = -1; //Сброс идентификатора

            UpdateGrid(); //Обновление таблицы
            ClearForm(); //Очистка форма
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            //Если идентификатор не задан или сброшен -
            if (id < 0) return; //удалить нельзя, выход.

            //Вывод предупреждения
            if (MessageBox.Show("Действительно удалить?", "", MessageBoxButtons.YesNo) == DialogResult.No) return;

            //Собственно удаление
            if (MySettings.Default.ProtocolXmlRpc)
                xmlrpc_proxy.DeleteChild(id);
            else if (MySettings.Default.ProtocolRest)
                rest_proxy.DeleteChild(id);

            id = -1; //Сброс идентификатора
            UpdateGrid(); //Обновление таблицы
            ClearForm(); //Очистка формы
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentRow.Index + 1 == dataGridView1.Rows.Count)
            {
                //Если щёлкнули в последней строке таблицы - переход в режим
                //добавления новой записи -
                id = -1; //Сброс идентификатора
                ClearForm(); //Очистка формы
                return; //Выход из обработчика
            }

            //Подгрузка в форму данных выбранного в таблице человека
            id = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);

            if (MySettings.Default.ProtocolXmlRpc)
            {
                XMLRPC_Child child = xmlrpc_proxy.ReadChild(id);

                tb_ID.Text = child.ID.ToString();
                textBox1.Text = child.FIO.ToString();
                //textBox3.Text = child.DateB.ToString();
                string str = child.DateB.Replace('-', '.');
                dateTimePicker1.Value = Convert.ToDateTime(str);
                textBox4.Text = child.FIOMam.ToString();
                textBox5.Text = child.TelMam.ToString();
                textBox6.Text = child.FioPap.ToString();
                textBox7.Text = child.TelPap.ToString();
                textBox8.Text = child.Email.ToString();

                //Выделение в раскрывающемся списке Группы
                foreach (ItemComboBox item in list)
                {
                    if (item.Id == child.IDgr)
                    {
                        comboBox1.SelectedItem = item;
                        break;
                    }
                }
            }
            else if (MySettings.Default.ProtocolRest)
            {
                REST_Child child = rest_proxy.ReadChild(id);

                tb_ID.Text = child.ID.ToString();
                textBox1.Text = child.FIO.ToString();
                //textBox3.Text = child.DateB.ToString();
                string str = child.DateB.Replace('-', '.');
                dateTimePicker1.Value = Convert.ToDateTime(str);
                textBox4.Text = child.FIOMam.ToString();
                textBox5.Text = child.TelMam.ToString();
                textBox6.Text = child.FioPap.ToString();
                textBox7.Text = child.TelPap.ToString();
                textBox8.Text = child.Email.ToString();

                //Выделение в раскрывающемся списке Группы
                foreach (ItemComboBox item in list)
                {
                    if (item.Id == child.IDgr)
                    {
                        comboBox1.SelectedItem = item;
                        break;
                    }
                }
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
        private void button1_Click(object sender, EventArgs e)
        {
            id = -1; // Сброс идентификатора
            ClearForm(); // Очистка формы
        }


        //============================================
    }
}
