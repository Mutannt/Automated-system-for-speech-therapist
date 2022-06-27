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

using System.Windows.Forms.DataVisualization.Charting;

namespace SOA_Client
{
    public partial class Diagrams : Form
    {
        IRest2018 rest_proxy;
        IMyProxy xmlrpc_proxy;
        List<ItemComboBox> listGroups;
        List<ItemComboBox> listChildren;
        public Diagrams()
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

        private void Diagrams_Load(object sender, EventArgs e)
        {
            checkBox1.Checked = true;
            checkBox2.Checked = true;

            //Формирование раскрывающегося списка "Группы"           
            listGroups = new List<ItemComboBox>();
            listGroups.Add(new ItemComboBox(0, "Выберите"));
            //В зависимости от выбранного протокола 
            //вызов метода ListLogopeds у соответствующего
            //прокси-объекта
            if (MySettings.Default.ProtocolXmlRpc)
            {
                XMLRPC_Group[] groups = xmlrpc_proxy.ListGroups();
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
                    // Дети в группе, которые прошли диагностику
                    REST_Child[] childrenInGroupDiagn = rest_proxy.ListChildrenInGroupDiagn((comboBox1.SelectedItem as ItemComboBox).Id);
                    foreach (REST_Child child in childrenInGroupDiagn)
                    {
                        listChildren.Add(new ItemComboBox(child.ID, child.FIO)); // Заполнение списка
                    }
                }

                //Вывод полученного списка Детей в раскрывающийся список
                comboBox2.DataSource = listChildren;
            }
        }
        // График по ребёнку
        private void button1_Click(object sender, EventArgs e)
        {
            try {
                // Очистить серии
                chart1.Series.Clear();

                chart1.Series.Add("S1");
                //chart1.Series["S1"].ChartType = SeriesChartType.Radar;
                chart1.Series["S1"].LegendText = "В начале года";
                
                chart1.Series.Add("S2");
                //chart1.Series["S2"].ChartType = SeriesChartType.Radar;
                chart1.Series["S2"].LegendText = "В конце года";
                // Установка заголовка диаграммы
                chart1.Titles.Clear();
                chart1.Titles.Add("Динамика развития речи ребёнка");
                // Устанвока максимального значения
                chart1.ChartAreas[0].AxisY.Maximum = 4;

                if (MySettings.Default.ProtocolXmlRpc)
                {
                    if (checkBox1.Checked)
                    {
                        XMLRPC_DiagnosticPoints diagnosticPointsStart = xmlrpc_proxy.GetDiagnosticPointsDiagram((comboBox2.SelectedItem as ItemComboBox).Id, "Start");
                        chart1.Series["S1"].Points.AddXY("Звукопроизношение", diagnosticPointsStart.SoundPronunciation); // Звукопроизношение
                        chart1.Series["S1"].Points.AddXY("Слоговая структура", diagnosticPointsStart.SyllabicStructure); // Слоговая структура
                        chart1.Series["S1"].Points.AddXY("Фонематические пред-я", diagnosticPointsStart.PhonemicRepresentations); // Фонематические представления
                        chart1.Series["S1"].Points.AddXY("Грамматический строй", diagnosticPointsStart.Grammar); // Граматический строй
                        chart1.Series["S1"].Points.AddXY("Лексический запас", diagnosticPointsStart.LexicalStock); // Лексический запас
                        chart1.Series["S1"].Points.AddXY("Понимание речи", diagnosticPointsStart.SpeechUnderstanding); // Понимание речи
                        chart1.Series["S1"].Points.AddXY("Связная речь", diagnosticPointsStart.ConnectedSpeech); // Лексический запас
                    }
                    if (checkBox2.Checked)
                    {
                        XMLRPC_DiagnosticPoints diagnosticPointsEnd = xmlrpc_proxy.GetDiagnosticPointsDiagram((comboBox2.SelectedItem as ItemComboBox).Id, "End"); // Получить запись в связанной таблице ID+1
                        chart1.Series["S2"].Points.AddXY("Звукопроизношение", diagnosticPointsEnd.SoundPronunciation); // Звукопроизношение
                        chart1.Series["S2"].Points.AddXY("Слоговая структура", diagnosticPointsEnd.SyllabicStructure); // Слоговая структура
                        chart1.Series["S2"].Points.AddXY("Фонематические пред-я", diagnosticPointsEnd.PhonemicRepresentations); // Фонематические представления
                        chart1.Series["S2"].Points.AddXY("Грамматический строй", diagnosticPointsEnd.Grammar); // Граматический строй
                        chart1.Series["S2"].Points.AddXY("Лексический запас", diagnosticPointsEnd.LexicalStock); // Лексический запас
                        chart1.Series["S2"].Points.AddXY("Понимание речи", diagnosticPointsEnd.SpeechUnderstanding); // Понимание речи
                        chart1.Series["S2"].Points.AddXY("Связная речь", diagnosticPointsEnd.ConnectedSpeech); // Лексический запас
                    }
                }
                else if (MySettings.Default.ProtocolRest)
                {
                    REST_DiagnosticPoints diagnosticPointsStart = rest_proxy.GetDiagnosticPointsStartDiagram((comboBox2.SelectedItem as ItemComboBox).Id);
                    if (checkBox1.Checked)
                    {
                        chart1.Series["S1"].Points.AddXY("Звукопроизношение", diagnosticPointsStart.SoundPronunciation); // Звукопроизношение
                        chart1.Series["S1"].Points.AddXY("Слоговая структура", diagnosticPointsStart.SyllabicStructure); // Слоговая структура
                        chart1.Series["S1"].Points.AddXY("Фонематические пред-я", diagnosticPointsStart.PhonemicRepresentations); // Фонематические представления
                        chart1.Series["S1"].Points.AddXY("Грамматический строй", diagnosticPointsStart.Grammar); // Граматический строй
                        chart1.Series["S1"].Points.AddXY("Лексический запас", diagnosticPointsStart.LexicalStock); // Лексический запас
                        chart1.Series["S1"].Points.AddXY("Понимание речи", diagnosticPointsStart.SpeechUnderstanding); // Понимание речи
                        chart1.Series["S1"].Points.AddXY("Связная речь", diagnosticPointsStart.ConnectedSpeech); // Лексический запас
                    }
                    if (checkBox2.Checked)
                    {
                        REST_DiagnosticPoints diagnosticPointsEnd = rest_proxy.GetDiagnosticPointsEnd2(diagnosticPointsStart.ID + 1); // Получить запись в связанной таблице ID+1
                        chart1.Series["S2"].Points.AddXY("Звукопроизношение", diagnosticPointsEnd.SoundPronunciation); // Звукопроизношение
                        chart1.Series["S2"].Points.AddXY("Слоговая структура", diagnosticPointsEnd.SyllabicStructure); // Слоговая структура
                        chart1.Series["S2"].Points.AddXY("Фонематические пред-я", diagnosticPointsEnd.PhonemicRepresentations); // Фонематические представления
                        chart1.Series["S2"].Points.AddXY("Грамматический строй", diagnosticPointsEnd.Grammar); // Граматический строй
                        chart1.Series["S2"].Points.AddXY("Лексический запас", diagnosticPointsEnd.LexicalStock); // Лексический запас
                        chart1.Series["S2"].Points.AddXY("Понимание речи", diagnosticPointsEnd.SpeechUnderstanding); // Понимание речи
                        chart1.Series["S2"].Points.AddXY("Связная речь", diagnosticPointsEnd.ConnectedSpeech); // Лексический запас
                    }
                }
            }
            catch {
                MessageBox.Show("Группа не выбрана!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        // Общий по детем
        private void button3_Click(object sender, EventArgs e)
        {
            //try
            //{
                // Очистить серии
                chart1.Series.Clear();

                chart1.Series.Add("S1");
                //chart1.Series["S1"].ChartType = SeriesChartType.Radar;
                chart1.Series["S1"].LegendText = "В начале года";

                chart1.Series.Add("S2");
                //chart1.Series["S2"].ChartType = SeriesChartType.Radar;
                chart1.Series["S2"].LegendText = "В конце года";


                // Установка заголовка диаграммы
                chart1.Titles.Clear();
                chart1.Titles.Add("Динамика развития речи детей");
                // Устанвока максимального значения
                chart1.ChartAreas[0].AxisY.Maximum = 4;

                if (MySettings.Default.ProtocolXmlRpc)
                {
                    if (checkBox1.Checked)
                    {
                        XMLRPC_Diagrams DiagramsStart = xmlrpc_proxy.GetDiagnosticPointsAvgDiagram("Start");
                        chart1.Series["S1"].Points.AddXY("Звукопроизношение", DiagramsStart.AvgSoundPronunciation); // Звукопроизношение
                        chart1.Series["S1"].Points.AddXY("Слоговая структура", DiagramsStart.AvgSyllabicStructure); // Слоговая структура
                        chart1.Series["S1"].Points.AddXY("Фонематические пред-я", DiagramsStart.AvgPhonemicRepresentations); // Фонематические представления
                        chart1.Series["S1"].Points.AddXY("Грамматический строй", DiagramsStart.AvgGrammar); // Граматический строй
                        chart1.Series["S1"].Points.AddXY("Лексический запас", DiagramsStart.AvgLexicalStock); // Лексический запас
                        chart1.Series["S1"].Points.AddXY("Понимание речи", DiagramsStart.AvgSpeechUnderstanding); // Понимание речи
                        chart1.Series["S1"].Points.AddXY("Связная речь", DiagramsStart.AvgConnectedSpeech); // Лексический запас
                    }
                    if (checkBox2.Checked)
                    {
                        XMLRPC_Diagrams DiagramsEnd = xmlrpc_proxy.GetDiagnosticPointsAvgDiagram("End");
                        chart1.Series["S2"].Points.AddXY("Звукопроизношение", DiagramsEnd.AvgSoundPronunciation); // Звукопроизношение
                        chart1.Series["S2"].Points.AddXY("Слоговая структура", DiagramsEnd.AvgSyllabicStructure); // Слоговая структура
                        chart1.Series["S2"].Points.AddXY("Фонематические пред-я", DiagramsEnd.AvgPhonemicRepresentations); // Фонематические представления
                        chart1.Series["S2"].Points.AddXY("Грамматический строй", DiagramsEnd.AvgGrammar); // Граматический строй
                        chart1.Series["S2"].Points.AddXY("Лексический запас", DiagramsEnd.AvgLexicalStock); // Лексический запас
                        chart1.Series["S2"].Points.AddXY("Понимание речи", DiagramsEnd.AvgSpeechUnderstanding); // Понимание речи
                        chart1.Series["S2"].Points.AddXY("Связная речь", DiagramsEnd.AvgConnectedSpeech); // Лексический запас
                    }
                }
                else if (MySettings.Default.ProtocolRest)
                {
                    if (checkBox1.Checked)
                    {
                        REST_Diagrams DiagramsStart = rest_proxy.GetDiagnosticPointsAvgStartDiagram();
                        chart1.Series["S1"].Points.AddXY("Звукопроизношение", DiagramsStart.AvgSoundPronunciation); // Звукопроизношение
                        chart1.Series["S1"].Points.AddXY("Слоговая структура", DiagramsStart.AvgSyllabicStructure); // Слоговая структура
                        chart1.Series["S1"].Points.AddXY("Фонематические пред-я", DiagramsStart.AvgPhonemicRepresentations); // Фонематические представления
                        chart1.Series["S1"].Points.AddXY("Грамматический строй", DiagramsStart.AvgGrammar); // Граматический строй
                        chart1.Series["S1"].Points.AddXY("Лексический запас", DiagramsStart.AvgLexicalStock); // Лексический запас
                        chart1.Series["S1"].Points.AddXY("Понимание речи", DiagramsStart.AvgSpeechUnderstanding); // Понимание речи
                        chart1.Series["S1"].Points.AddXY("Связная речь", DiagramsStart.AvgConnectedSpeech); // Лексический запас
                    }
                    if (checkBox2.Checked)
                    {
                        REST_Diagrams DiagramsEnd = rest_proxy.GetDiagnosticPointsAvgEndDiagram();
                        chart1.Series["S2"].Points.AddXY("Звукопроизношение", DiagramsEnd.AvgSoundPronunciation); // Звукопроизношение
                        chart1.Series["S2"].Points.AddXY("Слоговая структура", DiagramsEnd.AvgSyllabicStructure); // Слоговая структура
                        chart1.Series["S2"].Points.AddXY("Фонематические пред-я", DiagramsEnd.AvgPhonemicRepresentations); // Фонематические представления
                        chart1.Series["S2"].Points.AddXY("Грамматический строй", DiagramsEnd.AvgGrammar); // Граматический строй
                        chart1.Series["S2"].Points.AddXY("Лексический запас", DiagramsEnd.AvgLexicalStock); // Лексический запас
                        chart1.Series["S2"].Points.AddXY("Понимание речи", DiagramsEnd.AvgSpeechUnderstanding); // Понимание речи
                        chart1.Series["S2"].Points.AddXY("Связная речь", DiagramsEnd.AvgConnectedSpeech); // Лексический запас
                    }
                }
            //}
            //catch
            //{
            //    MessageBox.Show("Что-то не то!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //}

            

            
        }

    // END
    }
}
