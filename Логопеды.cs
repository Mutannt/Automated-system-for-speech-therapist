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
using System.Security.Cryptography;

namespace SOA_Client
{
    public partial class Логопеды : Form
    {
        IRest2018 rest_proxy;
        IMyProxy xmlrpc_proxy;

        DataTable table;
        DataColumn col;
        DataRow myrow;

        int id = -1;
        public Логопеды()
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

        private void Логопеды_Load(object sender, EventArgs e)
        {
            //Формирование таблицы со списком языков
            table = new DataTable();
            col = new DataColumn("ID");
            table.Columns.Add(col);
            col = new DataColumn("FIO");
            table.Columns.Add(col);
            col = new DataColumn("Log1n");
            table.Columns.Add(col);
            col = new DataColumn("Pass");
            table.Columns.Add(col);

            //Заполнение таблицы данными
            UpdateGrid();

            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].HeaderText = "ФИО логопеда";
            dataGridView1.Columns[1].Width = 250;

            dataGridView1.Columns[2].HeaderText = "Логин";
            dataGridView1.Columns[2].Width = 130;
            dataGridView1.Columns[3].HeaderText = "Пароль-ХЭШ";
            dataGridView1.Columns[3].Width = 130;
        } // 

        private void UpdateGrid()
        {
            //Обновление таблицы

            //Очистка таблицы
            table.Clear();

            //Подгрузка в таблицу новых данных
            if (MySettings.Default.ProtocolXmlRpc)
            {
                XMLRPC_Logoped[] logopeds = xmlrpc_proxy.ListLogopeds();

                foreach (XMLRPC_Logoped logoped in logopeds)
                {
                    myrow = table.NewRow();
                    myrow["ID"] = logoped.ID;
                    myrow["FIO"] = logoped.FIO;
                    myrow["Log1n"] = logoped.Log1n;
                    myrow["Pass"] = logoped.Pass.Replace(logoped.Pass, new string('*', logoped.Pass.Length - 1));

                    table.Rows.Add(myrow);
                }
            }
            else if (MySettings.Default.ProtocolRest)
            {
                REST_Logoped[] logopeds = rest_proxy.ListLogopeds();
                foreach (REST_Logoped logoped in logopeds)
                {
                    myrow = table.NewRow();
                    myrow["ID"] = logoped.ID;
                    myrow["FIO"] = logoped.FIO;
                    myrow["Log1n"] = logoped.Log1n;
                    myrow["Pass"] = logoped.Pass.Replace(logoped.Pass, new string('*', logoped.Pass.Length - 1));

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
            textBox2.Text = "";
            textBox3.Text = "";
        }
        // ХЭШИРОВАНИЕ, алгоритм MD5
        private string GetHash(string input)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

            return Convert.ToBase64String(hash);
        }

        // Сохранить
        private void btn_Save_Click(object sender, EventArgs e)
        {
            if (id == 1)
            {
                if (textBox1.Text != "Admin") return;
            }
            //Сохранение/Обновление Логопеда
            if (MySettings.Default.ProtocolXmlRpc)
            {
                XMLRPC_Logoped logoped = new XMLRPC_Logoped();
                logoped.FIO = this.textBox1.Text;
                logoped.Log1n = this.textBox2.Text;
                logoped.Pass = GetHash(textBox3.Text);

                if (id < 0) //Если идентификатор не задан (сброшен) - 
                    xmlrpc_proxy.CreateLogoped(logoped); //Создание нового Логопеда
                else //иначе (то есть идентификатор задан) -
                    xmlrpc_proxy.UpdateLogoped(id, logoped); //перезапись существующего Логопеда с заданным идентификатором
            }
            else if (MySettings.Default.ProtocolRest)
            {
                if (id < 0)
                    rest_proxy.CreateLogoped(new REST_Logoped(textBox1.Text, textBox2.Text, GetHash(textBox3.Text)));
                else
                    rest_proxy.UpdateLogoped(id, new REST_Logoped(textBox1.Text, textBox2.Text, GetHash(textBox3.Text)));
            }

            if (id >= 0) id = -1; //Сброс идентификатора

            UpdateGrid(); //Обновление таблицы
            ClearForm(); //Очистка формы 
        }

        // Удалить
        private void btn_Delete_Click(object sender, EventArgs e)
        {
            //Удаление Admina
            if (id == 1) {
                MessageBox.Show("Нельзя удалять администратора!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (id < 0) return; //Если логопед не выбран - выход
            if (MessageBox.Show("Действительно удалить?", "", MessageBoxButtons.YesNo) == DialogResult.No) return;

            if (MySettings.Default.ProtocolXmlRpc)
                xmlrpc_proxy.DeleteLogoped(id);
            else if (MySettings.Default.ProtocolRest)
                rest_proxy.DeleteLogoped(id);

            UpdateGrid(); //Обновление таблицы
            ClearForm(); //Очистка формы
            id = -1; //Сброс идентификатора
        }

        // Выбор ячейки
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dataGridView1.CurrentRow.Index + 1 == this.dataGridView1.Rows.Count)
            {
                //Если щёлкнули в последней строке таблицы - переход в режим
                //добавления новой записи -
                id = -1; //Сброс идентификатора
                ClearForm(); //Очистка формы
                return; //Выход из обработчика
            }

            //Подгрузка в форму данных выбранного в таблице логопеда
            id = Convert.ToInt32(this.dataGridView1.CurrentRow.Cells[0].Value);

            if (MySettings.Default.ProtocolXmlRpc)
            {
                XMLRPC_Logoped logoped = xmlrpc_proxy.ReadLogoped(id);

                tb_ID.Text = logoped.ID.ToString();
                textBox1.Text = logoped.FIO;
                textBox2.Text = logoped.Log1n;
                textBox3.Text = logoped.Pass.Replace(logoped.Pass, new string('*', logoped.Pass.Length - 1));
            }
            else if (MySettings.Default.ProtocolRest)
            {
                REST_Logoped logoped = rest_proxy.ReadLogoped(id);

                tb_ID.Text = logoped.ID.ToString();
                textBox1.Text = logoped.FIO;
                textBox2.Text = logoped.Log1n;
                textBox3.Text = logoped.Pass.Replace(logoped.Pass, new string('*', logoped.Pass.Length - 1));
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

        // end =================================
    }
}
