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
    public partial class Form1 : Form
    {
        IRest2018 rest_proxy;
        IMyProxy xmlrpc_proxy;
        int IDuser;

        public Form1()
        {
            InitializeComponent();

            //Создание прокси-объекта 
            CreateProxy();
        }
        //Создание прокси-объекта в зависимости от выбранного
        //в настройках протокола
        private void CreateProxy()
        {
            if (MySettings.Default.ProtocolXmlRpc)
                xmlrpc_proxy = XmlRpcProxyGen.Create<IMyProxy>();
            else if (MySettings.Default.ProtocolRest)
            {
                ChannelFactory<IRest2018> factory;
                factory = new ChannelFactory<IRest2018>("REST2018");
                rest_proxy = factory.CreateChannel();
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            FrmSettings frm = new FrmSettings();
            frm.ShowDialog();
            CreateProxy();
        }


        // Форма логопеды
        private void button8_Click(object sender, EventArgs e)
        {
            Логопеды frm = new Логопеды();
            frm.ShowDialog();
        }

        // Форма группы
        private void button5_Click(object sender, EventArgs e)
        {
            Группы frm = new Группы();
            frm.ShowDialog();
        }

        // Форма дети
        private void button6_Click(object sender, EventArgs e)
        {
            Дети frm = new Дети(IDuser);
            frm.ShowDialog();
        }

        // Форма диагностика
        private void button7_Click(object sender, EventArgs e)
        {
            Диагностика frm = new Диагностика(IDuser);
            frm.ShowDialog();
        }

        // Выход
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Форма Речевая карта
        private void button10_Click(object sender, EventArgs e)
        {
            Речевая_карта frm = new Речевая_карта(IDuser);
            frm.ShowDialog();
        }

        // Форма Индивидуальный план
        private void button11_Click(object sender, EventArgs e)
        {
            Индивидуальный_план frm = new Индивидуальный_план(IDuser);
            frm.ShowDialog();
        }

        // Хэширование
        private string GetHash(string input)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

            return Convert.ToBase64String(hash);
        }


        // Войти
        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (MySettings.Default.ProtocolXmlRpc)
                {
                    //Проверка логина и пароля
                    XMLRPC_Logoped[] logopeds = xmlrpc_proxy.ListLogopeds();
                    foreach (XMLRPC_Logoped logoped in logopeds)
                    {
                        if ((textBox1.Text == logoped.Log1n) && (GetHash(textBox2.Text) == logoped.Pass))
                        {
                            panel1.Visible = false;
                            panel2.Visible = true;
                            this.Text = "Главное меню";
                            IDuser = logoped.ID;
                            // Если не Администратор
                            if (logoped.ID != 1)
                            {
                                button8.Visible = false;
                                button5.Visible = false;
                            }
                            return;
                        }
                    }
                    // Если нет, вывести ошибку
                    label3.Visible = true;
                }
                else if (MySettings.Default.ProtocolRest)
                {
                    //Проверка логина и пароля
                    REST_Logoped[] logopeds = rest_proxy.ListLogopeds();
                    foreach (REST_Logoped logoped in logopeds)
                    {
                        if ((textBox1.Text == logoped.Log1n) && (GetHash(textBox2.Text) == logoped.Pass))
                        {
                            panel1.Visible = false;
                            panel2.Visible = true;
                            this.Text = "Главное меню";
                            // Если Администратор показать 
                            if (logoped.ID != 1)
                            {
                                button8.Visible = false;
                                button5.Visible = false;
                            }
                            return;
                        }
                    }
                    // Если нет, вывести ошибку
                    label3.Visible = true;
                }
            }
            catch {
                MessageBox.Show("Веб-сервер недоступен!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }



        // END
    }
}
