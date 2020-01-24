using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private int t0, t1a, t1b, t2a, t2b, t3a, t3b, t4a, t4b;
        //private string abonent;
        //List<string> testlist = new List<string>();

        public struct abonent
        {
            public int Group;

            public string Name;
            public string DNS;
            public string IP;
        }

        public abonent[] ab = new abonent[103];

        public struct reply
        {
            public int Time;
            public int TTT;
            public string IP;
            public string Rep;
        }

        #region Константы
        public const int C_sec = 1000, C_min = 60000;

        private string[] StandartList = new string[]
            {"Loopback/DNS name/127.0.0.1", "БРИ-1/DNS name/10.1.1.254", "БРИ-2/DNS name/10.1.2.254", "БРИ-3/DNS name/192.168.60.254", "Что-то/DNS name/192.168.68.73", "АСП/DNS name/10.1.2.250", "",
            "Ещё что-то/ /192.168.67.25", "USL ER-SWA1,2/ /192.168.60.253", "ISS Server 1/ /192.168.60.51", "LS1/ /192.168.60.53", "Lab printer/ /192.168.60.82", "",
            "RSE1/ /10.1.1.3", "RSK1/ /10.1.1.4", "RSK2/ /10.1.1.5", "RSS1/ /10.1.2.1", "RSS2/ /10.1.1.1", "SM Printer/ /192.168.60.81", "Mediaserver AGAT/ /10.1.1.80", "RSE-Med/ /10.1.1.7", "",
            "FS1/ /10.1.1.100", "БСПН (PLS)/ /10.1.3.4", "ТВМ1-Н/ /10.1.3.1", "БПИ-НЧ (TRPU)/ /192.168.249.1", "БЗУ/ /10.1.11.5"};
        #endregion Константы

        #region Методы
        public Form1()
        {
            InitializeComponent();
        }

        private void Translate(bool is_english)
        {
            if(is_english)
            {
                toolStripButton1.Text = "File";
                    Open_iniTSMitem.Text = "Open .INI file";
                    Open_logTSMitem.Text = "Open log file";
                toolStripButton2.Text = "Tracking";
                toolStripButton3.Text = "Settings";
                    LanguageTSMitem.Text = "Language";
                        Lang_rusTSMitem.Text = "Russian";
                        Lang_engTSMitem.Text = "English";
                    ViewTSMitem.Text = "View";
                        Switch_idTSMitem.Text = "On/Off ID";
                        Switch_dnsTSMitem.Text = "On/Off DNS";
                        Switch_ipTSMitem.Text = "On/Off IP";
                toolStripButton4.Text = "Help";
                    User_guideTSMitem.Text = "User's guide";
                    AboutTSMitem.Text = "About";

                groupBox1.Text = "Group 1";
                groupBox2.Text = "Group 2";
                groupBox3.Text = "Group 3";
                groupBox4.Text = "Group 4";

                Column1b.HeaderText = "Name";
                Column1e.HeaderText = "Status";

                label1.Text = "Timeout:";
                label2.Text = "Ping ever:";
                label21.Text = "Packets count:";
                label11.Text = "sec";
                label12.Text = "min";

                checkBox1.Text = "Autoping 1st group";
                checkBox2.Text = "Autoping 2nd group";
                checkBox3.Text = "Autoping 3rd group";
                checkBox4.Text = "Autoping 4th group";

                button1.Text = "Ping 1st group";
                button2.Text = "Ping 2nd group";
                button3.Text = "Ping 3rd group";
                button4.Text = "Ping 4th group";
            }
            else
            {
                toolStripButton1.Text = "Файл";
                    Open_iniTSMitem.Text = "Открыть .INI файл";
                    Open_logTSMitem.Text = "Открыть лог файл";
                toolStripButton2.Text = "Слежение";
                toolStripButton3.Text = "Настройки";
                    LanguageTSMitem.Text = "Язык";
                        Lang_rusTSMitem.Text = "Русский";
                        Lang_engTSMitem.Text = "Английский";
                    ViewTSMitem.Text = "Вид";
                        Switch_idTSMitem.Text = "Вкл/Выкл ID";
                        Switch_dnsTSMitem.Text = "Вкл/Выкл DNS";
                        Switch_ipTSMitem.Text = "Вкл/Выкл IP";
                toolStripButton4.Text = "Помощь";
                    User_guideTSMitem.Text = "Руководство пользователя";
                    AboutTSMitem.Text = "О программе";

                groupBox1.Text = "Группа 1";
                groupBox2.Text = "Группа 2";
                groupBox3.Text = "Группа 3";
                groupBox4.Text = "Группа 4";

                Column1b.HeaderText = "Имя";
                Column1e.HeaderText = "Статус";

                label1.Text = "Время ожидания:";
                label2.Text = "Период автопинга:";
                label21.Text = "Кол-во пакетов:";
                label11.Text = "сек";
                label12.Text = "мин";

                checkBox1.Text = "Автопинг 1 группы";
                checkBox2.Text = "Автопинг 2 группы";
                checkBox3.Text = "Автопинг 3 группы";
                checkBox4.Text = "Автопинг 4 группы";

                button1.Text = "Пинг 1 группы";
                button2.Text = "Пинг 2 группы";
                button3.Text = "Пинг 3 группы";
                button4.Text = "Пинг 4 группы";
            }
        }

        private void CopyElements()
        {
            Column2a.HeaderText = Column1a.HeaderText;
            Column3a.HeaderText = Column1a.HeaderText;
            Column4a.HeaderText = Column1a.HeaderText;

            Column2b.HeaderText = Column1b.HeaderText;
            Column3b.HeaderText = Column1b.HeaderText;
            Column4b.HeaderText = Column1b.HeaderText;

            Column2c.HeaderText = Column1c.HeaderText;
            Column3c.HeaderText = Column1c.HeaderText;
            Column4c.HeaderText = Column1c.HeaderText;

            Column2d.HeaderText = Column1d.HeaderText;
            Column3d.HeaderText = Column1d.HeaderText;
            Column4d.HeaderText = Column1d.HeaderText;

            Column2e.HeaderText = Column1e.HeaderText;
            Column3e.HeaderText = Column1e.HeaderText;
            Column4e.HeaderText = Column1e.HeaderText;

            Column2f.HeaderText = Column1f.HeaderText;
            Column3f.HeaderText = Column1f.HeaderText;
            Column4f.HeaderText = Column1f.HeaderText;

            label3.Text = label1.Text;
            label5.Text = label1.Text;
            label7.Text = label1.Text;

            label4.Text = label2.Text;
            label6.Text = label2.Text;
            label8.Text = label2.Text;

            label13.Text = label11.Text;
            label15.Text = label11.Text;
            label17.Text = label11.Text;

            label14.Text = label12.Text;
            label16.Text = label12.Text;
            label18.Text = label12.Text;

            label22.Text = label21.Text;
            label23.Text = label21.Text;
            label24.Text = label21.Text;
        }

        private string[] PreProcessing()
        {
            int k;
            string[] al = new string[100];

            if (File.Exists("Abonents.txt"))
                al = File.ReadAllLines("Abonents.txt");
            else
            {
                File.Create("Abonents.txt");
                File.WriteAllLines("Abonents.txt", StandartList);
                al = StandartList;
            }

            t0 = 3 * C_sec;
            t1a = 3 * C_sec;
            t1b = 1 * C_min;
            t2a = 3 * C_sec;
            t2b = 1 * C_min;
            t3a = 3 * C_sec;
            t3b = 1 * C_min;
            t4a = 3 * C_sec;
            t4b = 1 * C_min;

            if (al.Count() <= 60)
                k = 15;
            else
                k = 25;

            for (int i = 0; i < k; i++)
            {
                dataGridView1.Rows.Add();
                dataGridView2.Rows.Add();
                dataGridView3.Rows.Add();
                dataGridView4.Rows.Add();
            }

            return al;
        }

        private void FillAL(string[] al)
        {
            for (int s = 0, grid = 1; s < al.Count(); s++)
            {
                if (al[s] == "")
                {
                    ab[s].Name = "";
                    grid++;
                }
                else
                {
                    string str = al[s];

                    for(int c = 0, flag = 1; c < al[s].Length; c++) // Правило разбиения строки на компоненты абонента (Имя абонента/DNS/IP)
                    {
                        char a = str[c];
                        if (str[c] == '/' && str[c + 1] == '/')
                        {
                            flag++;
                            c++;
                            //a[s].Name = "";
                            //a[s].DNS = "";
                            //a[s].IP = "";
                        }
                        else
                        {
                            switch (flag)
                            {
                                case 1:
                                    ab[s].Name = ab[s].Name + str[c];
                                    break;
                                case 2:
                                    ab[s].DNS = ab[s].DNS + str[c];
                                    break;
                                case 3:
                                    ab[s].IP = ab[s].IP + str[c];
                                    break;
                            }
                        }
                    }
                }
                ab[s].Group = grid; // хрень с первыми строками
            }
        }

        private void FillGrids()
        {
            //ab.Name = "Kek";

            /*dataGridView1.Rows.Add("", "Laptop 1", "", "192.168.0.1", "Online");
            dataGridView1.Rows.Add("", "Laptop 2", "", "192.168.51.86", "Online");
            dataGridView1.Rows.Add("", "Laptop 3", "", "127.0.0.1", "Offline");
            dataGridView1.Rows.Add("", "Laptop 4", "", "Some ip", "Offline");
            dataGridView1.Rows.Add("", "Laptop 5", "", "Some ip", "Online");

            dataGridView2.Rows.Add("", "Laptop 1", "", "192.168.0.1", "Online");
            dataGridView2.Rows.Add("", "Laptop 2", "", "192.168.51.86", "Online");
            dataGridView2.Rows.Add("", "Laptop 3", "", "127.0.0.1", "Offline");
            dataGridView2.Rows.Add("", "Laptop 4", "", "Some ip", "Offline");
            dataGridView2.Rows.Add("", "Laptop 5", "", "Some ip", "Online");

            dataGridView3.Rows.Add("", "Laptop 1", "", "192.168.0.1", "Online");
            dataGridView3.Rows.Add("", "Laptop 2", "", "192.168.51.86", "Online");
            dataGridView3.Rows.Add("", "Laptop 3", "", "127.0.0.1", "Offline");
            dataGridView3.Rows.Add("", "Laptop 4", "", "Some ip", "Offline");
            dataGridView3.Rows.Add("", "Laptop 5", "", "Some ip", "Online");

            dataGridView4.Rows.Add("", "Laptop 1", "", "192.168.0.1", "Online");
            dataGridView4.Rows.Add("", "Laptop 2", "", "192.168.51.86", "Online");
            dataGridView4.Rows.Add("", "Laptop 3", "", "127.0.0.1", "Offline");
            dataGridView4.Rows.Add("", "Laptop 4", "", "Some ip", "Offline");
            dataGridView4.Rows.Add("", "Laptop 5", "", "Some ip", "Online");*/
            //dataGridView1.Rows.Add("Laptop 6", ab.IP, "Online");

            /*dataGridView1[5, 0].Style.BackColor = Color.GreenYellow;
            dataGridView1[5, 1].Style.BackColor = Color.GreenYellow;
            dataGridView1[5, 2].Style.BackColor = Color.Red;
            dataGridView1[5, 3].Style.BackColor = Color.Red;
            dataGridView1[5, 4].Style.BackColor = Color.Red;
            dataGridView1[5, 5].Style.BackColor = Color.Cyan;*/

            //dataGridView1[0, 0]. = false;

            //dataGridView1[1, 0].Value = ab[0].Name;
            for(int i = 0, j = 0; i < ab.Count(); i++, j++)
            {
                if(ab[i].Name == "")
                    j = -1;
                else
                {
                    switch (ab[i].Group)
                    {
                        case 1:
                            dataGridView1[1, j].Value = ab[i].Name;
                            dataGridView1[2, j].Value = ab[i].DNS; //ab[i].Name; ab[i].DNS; ab[i].IP;
                            dataGridView1[3, j].Value = ab[i].IP;
                            break;
                        case 2:
                            dataGridView2[1, j].Value = ab[i].Name;
                            dataGridView2[2, j].Value = ab[i].DNS;
                            dataGridView2[3, j].Value = ab[i].IP;
                            break;
                        case 3:
                            dataGridView3[1, j].Value = ab[i].Name;
                            dataGridView3[2, j].Value = ab[i].DNS;
                            dataGridView3[3, j].Value = ab[i].IP;
                            break;
                        case 4:
                            dataGridView4[1, j].Value = ab[i].Name;
                            dataGridView4[2, j].Value = ab[i].DNS;
                            dataGridView4[3, j].Value = ab[i].IP;
                            break;
                    }
                }
            }
        }
        // дописать
        private reply Ping_cl(abonent a)
        {
            reply r;

            r.TTT = 0;
            r.Time = 0;
            r.IP = "null";
            r.Rep = "null";

            /*if (textBox1.Text != "")
            {
                System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                System.Net.NetworkInformation.PingReply pingReply = ping.Send(textBox1.Text);

                label2.Text = pingReply.RoundtripTime.ToString() + " ms";
                label3.Text = pingReply.Status.ToString();
            }*/
            //Count();
            return r;
        }

        private void FormResized() // standart form size w916; h639;
        {
            groupBox1.Size = new Size((ClientSize.Width - 30) / 2, (ClientSize.Height - 50) / 2);
            groupBox2.Size = new Size((ClientSize.Width - 30) / 2, (ClientSize.Height - 50) / 2);
            groupBox2.Location = new Point(groupBox1.Width + 20, 25);
            groupBox3.Size = new Size((ClientSize.Width - 30) / 2, (ClientSize.Height - 50) / 2);
            groupBox3.Location = new Point(10, groupBox1.Height + 35);
            groupBox4.Size = new Size((ClientSize.Width - 30) / 2, (ClientSize.Height - 50) / 2);
            groupBox4.Location = new Point(groupBox1.Width + 20, groupBox1.Height + 35);

            dataGridView1.Size = new Size(groupBox1.Size.Width - 10, groupBox1.Size.Height - 95);
            dataGridView2.Size = new Size(groupBox1.Size.Width - 10, groupBox1.Size.Height - 95);
            dataGridView3.Size = new Size(groupBox1.Size.Width - 10, groupBox1.Size.Height - 95);
            dataGridView4.Size = new Size(groupBox1.Size.Width - 10, groupBox1.Size.Height - 95);

            checkBox1.Location = new Point(groupBox1.Size.Width - 125, 20);
            checkBox2.Location = new Point(groupBox2.Size.Width - 125, 20);
            checkBox3.Location = new Point(groupBox3.Size.Width - 125, 20);
            checkBox4.Location = new Point(groupBox4.Size.Width - 125, 20);

            button1.Location = new Point(groupBox1.Size.Width - 105, 60);
            button2.Location = new Point(groupBox2.Size.Width - 105, 60);
            button3.Location = new Point(groupBox3.Size.Width - 105, 60);
            button4.Location = new Point(groupBox4.Size.Width - 105, 60);

            if(Column1a.Visible || Column1c.Visible)
            {

            }
            else
            {                
                Column1b.Width = dataGridView1.Width - 195;
                //label1.Text = dataGridView1.VerticalScrollingOffset.ToString();
            }
            if (Column2a.Visible || Column1c.Visible)
            {

            }
            else
            {
                Column2b.Width = dataGridView2.Width - 195;
            }
            if (Column3a.Visible || Column1c.Visible)
            {

            }
            else
            {
                Column3b.Width = dataGridView3.Width - 195;
            }
            if (Column4a.Visible || Column1c.Visible)
            {

            }
            else
            {
                Column4b.Width = dataGridView4.Width - 195;
            }

            //label1.Text = "Form size: " + ClientSize.Width + "x" + ClientSize.Height + " gb size: " + groupBox1.Width + "x" + groupBox1.Height + " dgv size: " + dataGridView1.Width + "x" + dataGridView1.Height;
            //dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            //label1.Text = checkBox1.Location.X.ToString();
        }
        #endregion Методы

        #region События
        private void Form1_Load(object sender, EventArgs e)
        {
            //MinimumSize = new System.Drawing.Size(816, 639); // 800*600

            string[] abonentslist = new string[100];

            //FormResized();
            Translate(false);
            CopyElements();

            abonentslist = PreProcessing();
            FillAL(abonentslist);
            FillGrids();

            /*Timer0.Start();*/
            Timer0.Interval = 1;
            Timer1a.Interval = (int)numericUpDown1.Value * C_sec;
            Timer1b.Interval = (int)numericUpDown2.Value * C_min;
            Timer2a.Interval = (int)numericUpDown4.Value * C_sec;
            Timer2b.Interval = (int)numericUpDown5.Value * C_min;
            Timer3a.Interval = (int)numericUpDown7.Value * C_sec;
            Timer3b.Interval = (int)numericUpDown8.Value * C_min;
            Timer4a.Interval = (int)numericUpDown10.Value * C_sec;
            Timer4b.Interval = (int)numericUpDown11.Value * C_min;
        }

        #region Таймеры
        private void timer0_Tick(object sender, EventArgs e)
        {

        }

        private void timer1a_Tick(object sender, EventArgs e)
        {

        }

        private void timer1b_Tick(object sender, EventArgs e)
        {

        }

        private void timer2a_Tick(object sender, EventArgs e)
        {

        }

        private void timer2b_Tick(object sender, EventArgs e)
        {

        }

        private void timer3a_Tick(object sender, EventArgs e)
        {

        }

        private void timer3b_Tick(object sender, EventArgs e)
        {

        }

        private void timer4a_Tick(object sender, EventArgs e)
        {

        }

        private void timer4b_Tick(object sender, EventArgs e)
        {

        }
        #endregion Таймеры

        private void Lang_rusTSMitem_Click(object sender, EventArgs e)
        {
            Translate(false);
            CopyElements();
        }

        private void Lang_engTSMitem_Click(object sender, EventArgs e)
        {
            Translate(true);
            CopyElements();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                numericUpDown2.Enabled = true;
            }
            else
            {
                numericUpDown2.Enabled = false;
            }
            if (Timer1b.Enabled)
                Timer1b.Stop();
            Timer1b.Start();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                numericUpDown5.Enabled = true;
            }
            else
            {
                numericUpDown5.Enabled = false;
            }
            if (Timer2b.Enabled)
                Timer2b.Stop();
            Timer2b.Start();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                numericUpDown8.Enabled = true;
            }
            else
            {
                numericUpDown8.Enabled = false;
            }
            if (Timer3b.Enabled)
                Timer3b.Stop();
            Timer3b.Start();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                numericUpDown11.Enabled = true;
            }
            else
            {
                numericUpDown11.Enabled = false;
            }
            if (Timer4b.Enabled)
                Timer4b.Stop();
            Timer4b.Start();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Timer1a.Interval = (int)numericUpDown1.Value * C_sec;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            Timer1b.Interval = (int)numericUpDown2.Value * C_min;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            Timer2a.Interval = (int)numericUpDown4.Value * C_sec;
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            Timer2b.Interval = (int)numericUpDown5.Value * C_min;
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            Timer3a.Interval = (int)numericUpDown7.Value * C_sec;
        }

        private void numericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            Timer3b.Interval = (int)numericUpDown8.Value * C_min;
        }

        private void numericUpDown9_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown10_ValueChanged(object sender, EventArgs e)
        {
            Timer4a.Interval = (int)numericUpDown10.Value * C_sec;
        }

        private void numericUpDown11_ValueChanged(object sender, EventArgs e)
        {
            Timer4b.Interval = (int)numericUpDown11.Value * C_min;
        }

        private void numericUpDown12_ValueChanged(object sender, EventArgs e)
        {

        }
        // дописать
        private void button1_Click(object sender, EventArgs e)
        {
            if (Timer1a.Enabled)
                Timer1a.Stop();
            Timer1a.Start();

            label1.Text = ab[1].Name;
        }
        // дописать
        private void button2_Click(object sender, EventArgs e)
        {
            if (Timer2a.Enabled)
                Timer2a.Stop();
            Timer2a.Start();
        }
        // дописать
        private void button3_Click(object sender, EventArgs e)
        {
            if (Timer3a.Enabled)
                Timer3a.Stop();
            Timer3a.Start();
        }
        // дописать
        private void button4_Click(object sender, EventArgs e)
        {
            if (Timer4a.Enabled)
                Timer4a.Stop();
            Timer4a.Start();
        }
        // дописать
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //label1.Text = e.RowIndex.ToString();
        }
        // дописать
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //label1.Text = e.RowIndex.ToString();
        }
        // дописать
        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //label1.Text = e.RowIndex.ToString();
        }
        // дописать
        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //label1.Text = e.RowIndex.ToString();
        }

        private void Form_ChangedSize(object sender, EventArgs e)
        {
            FormResized();
        }
        #endregion События
    }
}
