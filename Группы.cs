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
    public partial class Группы : Form
    {
        IRest2018 rest_proxy;
        IMyProxy xmlrpc_proxy;

        DataTable table;
        DataRow myrow;
        DataColumn col;
        int id = -1;
        List<ItemComboBox> list;

        public Группы()
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

        private void Группы_Load(object sender, EventArgs e)
        {
            //Формирование таблицы со списком языков
            table = new DataTable();
            col = new DataColumn("IDgr");
            table.Columns.Add(col);
            col = new DataColumn("NumberGr");
            col.Caption = "Номер группы";
            table.Columns.Add(col);
            col = new DataColumn("Logoped");
            col.Caption = "Логопед";
            table.Columns.Add(col);
            //Заполнение таблицы данными
            UpdateGrid();

            dgv_groups.Columns[0].Visible = false;
            dgv_groups.Columns[1].HeaderText = "Номер группы";
            dgv_groups.Columns[1].Width = 50;
            dgv_groups.Columns[2].HeaderText = "Логопед";
            dgv_groups.Columns[2].Width = 250;

            //Формирование раскрывающегося списка "Логопеды"           
            list = new List<ItemComboBox>();

            //В зависимости от выбранного протокола 
            //вызов метода ListLogopeds у соответствующего
            //прокси-объекта
            if (MySettings.Default.ProtocolXmlRpc)
            {
                XMLRPC_Logoped[] logopeds = xmlrpc_proxy.ListFioLogopeds();
                foreach (XMLRPC_Logoped logoped in logopeds)
                    list.Add(new ItemComboBox(Convert.ToInt32(logoped.ID), logoped.FIO));
            }
            else if (MySettings.Default.ProtocolRest)
            {
                //вызов метода ListLogopeds у соответствующего прокси-объекта
                REST_Logoped[] logopeds = rest_proxy.ListFioLogopeds();
                foreach (REST_Logoped logoped in logopeds)
                    list.Add(new ItemComboBox(Convert.ToInt32(logoped.ID), logoped.FIO));
            }

           

            //Вывод полученного списка языков в раскрывающийся список
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
                XMLRPC_Group[] groups = xmlrpc_proxy.ListGroups();

                foreach (XMLRPC_Group group in groups)
                {
                    myrow = table.NewRow();
                    myrow["IDgr"] = group.IDgr;
                    myrow["NumberGr"] = group.NumberGr;
                    myrow["logoped"] = group.Logoped;

                    table.Rows.Add(myrow);
                }
            }
            else if (MySettings.Default.ProtocolRest)
            {
                REST_Group[] groups = rest_proxy.ListGroups();
                foreach (REST_Group group in groups)
                {
                    myrow = table.NewRow();
                    myrow["IDgr"] = group.IDgr;
                    myrow["NumberGr"] = group.NumberGr;
                    myrow["logoped"] = group.Logoped;

                    table.Rows.Add(myrow);
                }
            }
            this.dgv_groups.DataSource = table;
        }
        private void ClearForm()
        {
            //Функция очистки формы
            tb_ID.Text = "";
            tb_Number.Text = "";
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            //Сохранение/Обновление данных, введённых в форму
            if (MySettings.Default.ProtocolXmlRpc)
            {
                XMLRPC_Group group = new XMLRPC_Group();
                group.NumberGr = Convert.ToInt32(tb_Number.Text);
                group.IDlog = (comboBox1.SelectedItem as ItemComboBox).Id;

                if (id < 0) //Если идентификатор не задан или сброшен - 
                    xmlrpc_proxy.CreateGroup(group); //создание новой группы
                else //иначе (идентификатор задан) -
                    //обновление группы с заданным идентификатором:
                    xmlrpc_proxy.UpdateGroup(id, group);
            }
            else if (MySettings.Default.ProtocolRest)
            {
                if (id < 0)
                    rest_proxy.CreateGroup(new REST_Group(
                                Convert.ToInt32(tb_Number.Text),
                                (comboBox1.SelectedItem as ItemComboBox).Id
                        ));
                else
                    rest_proxy.UpdateGroup(id, new REST_Group(
                            Convert.ToInt32(tb_Number.Text),
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
            //Собственно удаление
            if (MySettings.Default.ProtocolXmlRpc)
                xmlrpc_proxy.DeleteGroup(id);
            else if (MySettings.Default.ProtocolRest)
                rest_proxy.DeleteGroup(id);

            id = -1; //Сброс идентификатора
            UpdateGrid(); //Обновление таблицы
            ClearForm(); //Очистка формы
        }

        //
        private void dgv_groups_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgv_groups.CurrentRow.Index + 1 == dgv_groups.Rows.Count)
            {
                //Если щёлкнули в последней строке таблицы - переход в режим
                //добавления новой записи -
                id = -1; //Сброс идентификатора
                ClearForm(); //Очистка формы
                return; //Выход из обработчика
            }

            //Подгрузка в форму данных выбранного в таблице человека
            id = Convert.ToInt32(dgv_groups.CurrentRow.Cells[0].Value);

            if (MySettings.Default.ProtocolXmlRpc)
            {
                XMLRPC_Group group = xmlrpc_proxy.ReadGroup(id);

                tb_ID.Text = group.IDgr.ToString();
                tb_Number.Text = group.NumberGr.ToString();

                //Выделение в раскрывающемся списке Логопеда
                foreach (ItemComboBox item in list)
                {
                    if (item.Id == group.IDlog)
                    {
                        this.comboBox1.SelectedItem = item;
                        break;
                    }
                }
            }
            else if (MySettings.Default.ProtocolRest)
            {
                REST_Group group = rest_proxy.ReadGroup(id);

                tb_ID.Text = group.IDgr.ToString();
                tb_Number.Text = group.NumberGr.ToString();

                //Выделение в раскрывающемся списке Логопеда
                foreach (ItemComboBox item in list)
                {
                    if (item.Id == group.IDlog)
                    {
                        this.comboBox1.SelectedItem = item;
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


        // ====================================
    }
}
