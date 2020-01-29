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
        bool is_english = false;
        string[] al;
        int p1, p2, p3, p4;

        static bool fl = true;
        static int curcl, curgr, g1, g2, g3, g4, t1, t2, t3, t4;
        static string[] ip1, ip2, ip3, ip4;

        static System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
        static System.Net.NetworkInformation.PingReply reply;
        static AutoResetEvent waiter = new AutoResetEvent(false);
        public static void ResultCallback(int lineCount)
        {
            Console.WriteLine(
                "Independent task printed {0} lines.", lineCount);
        }

        public struct client
        {
            public int Group;
            public int PosInGr;

            public string Name;
            public string DNS;
            public string IP;
            public string State;
        }

        public client[] ab;

        #region Константы
        public const int C_sec = 1000, C_min = 60000;

        private string[] StandartList = new string[]
            {"Loopback/DNS name/127.0.0.1", "БРИ-1/ /10.1.1.254", "БРИ-2/ /10.1.2.254", "БРИ-3/ /192.168.60.254", "Что-то/ /192.168.68.73", "АСП/ /10.1.2.250", "",
            "Ещё что-то/ /192.168.67.25", "USL ER-SWA1,2/ /192.168.60.253", "ISS Server 1/ /192.168.60.51", "LS1/ /192.168.60.53", "Lab printer/ /192.168.60.82", "",
            "RSE1/ /10.1.1.3", "RSK1/ /10.1.1.4", "RSK2/ /10.1.1.5", "RSS1/ /10.1.2.1", "RSS2/ /10.1.1.1", "SM Printer/ /192.168.60.81", "Mediaserver AGAT/ /10.1.1.80", "RSE-Med/ /10.1.1.7", "",
            "FS1/ /10.1.1.100", "БСПН (PLS)/ /10.1.3.4", "ТВМ1-Н/ /10.1.3.1", "БПИ-НЧ (TRPU)/ /192.168.249.1", "БЗУ/ /10.1.11.5"};
        #endregion Константы

        #region Методы
        public Form1()
        {
            InitializeComponent();
        }

        private void Translate()
        {
            if(is_english)
            {
                toolStripButton1.Text = "File";
                    Open_iniTSMitem.Text = "Open .INI file";
                    Open_logTSMitem.Text = "Open log file";
                toolStripButton2.Text = "Ping all";
                toolStripButton3.Text = "Tracking";
                toolStripButton4.Text = "Settings";
                    LanguageTSMitem.Text = "Language";
                        Lang_rusTSMitem.Text = "Russian";
                        Lang_engTSMitem.Text = "English";
                    ViewTSMitem.Text = "View";
                        Switch_dnsTSMitem.Text = "On/Off DNS";
                toolStripButton5.Text = "Help";
                    User_guideTSMitem.Text = "User's guide";
                    AboutTSMitem.Text = "About";

                groupBox1.Text = "Group 1";
                groupBox2.Text = "Group 2";
                groupBox3.Text = "Group 3";
                groupBox4.Text = "Group 4";

                Column1a.HeaderText = "Name";
                Column1d.HeaderText = "Status";

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
                toolStripButton2.Text = "Пинг всех";
                toolStripButton3.Text = "Слежение";
                toolStripButton4.Text = "Настройки";
                    LanguageTSMitem.Text = "Язык";
                        Lang_rusTSMitem.Text = "Русский";
                        Lang_engTSMitem.Text = "Английский";
                    ViewTSMitem.Text = "Вид";
                        Switch_dnsTSMitem.Text = "Вкл/Выкл DNS";
                toolStripButton5.Text = "Помощь";
                    User_guideTSMitem.Text = "Руководство пользователя";
                    AboutTSMitem.Text = "О программе";

                groupBox1.Text = "Группа 1";
                groupBox2.Text = "Группа 2";
                groupBox3.Text = "Группа 3";
                groupBox4.Text = "Группа 4";

                Column1a.HeaderText = "Имя";
                Column1d.HeaderText = "Статус";

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

        private void CopyNames()
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

        private void PreProcessing()
        {
            if (!File.Exists("Clients.txt"))
            {
                FileStream f = File.Create("Clients.txt");
                f.Close();
                File.WriteAllLines("Clients.txt", StandartList);
                al = StandartList;
            }
            al = File.ReadAllLines("Clients.txt");
            ab = new client[al.Count()];

            Timer1.Interval = (int)numericUpDown2.Value * C_min;
            Timer2.Interval = (int)numericUpDown5.Value * C_min;
            Timer3.Interval = (int)numericUpDown8.Value * C_min;
            Timer4.Interval = (int)numericUpDown11.Value * C_min;

            int c;
            if (al.Count() <= 60)
                c = 15;
            else
                c = 25;

            dataGridView1.Rows.Add(c);
            dataGridView2.Rows.Add(c);
            dataGridView3.Rows.Add(c);
            dataGridView4.Rows.Add(c);
        }

        private void FillClientsList()
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

                    for(int c = 0, flag = 1; c < al[s].Length; c++)
                    {
                        if (str[c] == '/') // Правило разбиения строки на компоненты абонента (Имя абонента/DNS/IP)
                            flag++;
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
                ab[s].Group = grid;

                switch (grid)
                {
                    case 1:
                        ab[s].PosInGr = s;
                        g1++;
                        break;
                    case 2:
                        ab[s].PosInGr = s - g1;
                        g2++;
                        break;
                    case 3:
                        ab[s].PosInGr = s - g1 - g2;
                        g3++;
                        break;
                    case 4:
                        ab[s].PosInGr = s - g1 - g2 - g3;
                        g4++;
                        break;
                }
            }
            g2--;
            g3--;
            g4--;

            ip1 = new string[g1];
            ip2 = new string[g2];
            ip3 = new string[g3];
            ip4 = new string[g4];
        }

        private void FillGrids()
        {
            for(int i = 0, j = 0; i < ab.Count(); i++, j++)
            {
                if(ab[i].Name == "")
                    j = -1;
                else
                {
                    switch (ab[i].Group)
                    {
                        case 1:
                            dataGridView1[0, j].Value = ab[i].Name;
                            dataGridView1[1, j].Value = ab[i].DNS;
                            dataGridView1[2, j].Value = ab[i].IP;
                            ip1[j] = ab[i].IP;
                            break;
                        case 2:
                            dataGridView2[0, j].Value = ab[i].Name;
                            dataGridView2[1, j].Value = ab[i].DNS;
                            dataGridView2[2, j].Value = ab[i].IP;
                            ip2[j] = ab[i].IP;
                            break;
                        case 3:
                            dataGridView3[0, j].Value = ab[i].Name;
                            dataGridView3[1, j].Value = ab[i].DNS;
                            dataGridView3[2, j].Value = ab[i].IP;
                            ip3[j] = ab[i].IP;
                            break;
                        case 4:
                            dataGridView4[0, j].Value = ab[i].Name;
                            dataGridView4[1, j].Value = ab[i].DNS;
                            dataGridView4[2, j].Value = ab[i].IP;
                            ip4[j] = ab[i].IP;
                            break;
                    }
                }
            }
        }

        private void PingAll()
        {
            int group = 1;
            if (fl)
            {
                int i = 0;
                switch (group)
                {
                    case 1:
                        if (i < g1)
                        {
                            ping.SendAsync(ip1[i], t1, waiter); // (int)numericUpDown1.Value * C_sec

                            /*
                            dataGridView1[3, i].Value = reply.Status;

                            if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                            {
                                dataGridView1[4, i].Style.BackColor = Color.GreenYellow;
                            }
                            else
                            {
                                dataGridView1[4, i].Style.BackColor = Color.Red;
                                //dataGridView1[4, 0].Style.BackColor = Color.GreenYellow;
                                //dataGridView1[4, 2].Style.BackColor = Color.Red;
                                //dataGridView1[4, 5].Style.BackColor = Color.Cyan;
                            }
                            */
                        }
                        break;
                    case 2:
                        if (i < g2)
                        {
                            ping.SendAsync(ip2[i], t2, waiter); // (int)numericUpDown4.Value * C_sec

                            /*
                            dataGridView2[3, i].Value = reply.Status;

                            if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                            {
                                dataGridView2[4, i].Style.BackColor = Color.GreenYellow;
                            }
                            else
                            {
                                dataGridView2[4, i].Style.BackColor = Color.Red;
                            }
                            */
                        }
                        break;
                    case 3:
                        if (i < g3)
                        {
                            ping.SendAsync(ip3[i], t3, waiter); // (int)numericUpDown7.Value * C_sec

                            /*
                            dataGridView3[3, i].Value = reply.Status;

                            if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                            {
                                dataGridView3[4, i].Style.BackColor = Color.GreenYellow;
                            }
                            else
                            {
                                dataGridView3[4, i].Style.BackColor = Color.Red;
                            }
                            */
                        }
                        break;
                    case 4:
                        if (i < g4)
                        {
                            ping.SendAsync(ip4[i], t4, waiter); // (int)numericUpDown10.Value * C_sec

                            /*
                            dataGridView4[3, i].Value = reply.Status;

                            if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                            {
                                dataGridView4[4, i].Style.BackColor = Color.GreenYellow;
                            }
                            else
                            {
                                dataGridView4[4, i].Style.BackColor = Color.Red;
                            }
                            */
                        }
                        break;
                }
                i++;

                fl = false;
            }
            else
            {
                switch (group)
                {
                    case 1:
                        if (curcl == g1)
                            fl = true;
                        else
                            fl = false; // Продолжение программы (занесение ответа в таблицу и запрос нового пинга)
                        break;
                    case 2:
                        if (curcl == g2)
                            fl = true;
                        break;
                    case 3:
                        if (curcl == g3)
                            fl = true;
                        break;
                    case 4:
                        if (curcl == g4)
                            fl = true;
                        break;
                }
            }
        }

        private void PingGroup()
        {
            if(fl)
            {
                switch (curgr)
                {
                    case 1:
                        if (curcl < g1)
                        {
                            if(is_english)
                                dataGridView1[3, curcl].Value = "Pinging...";
                            else
                                dataGridView1[3, curcl].Value = "Опрос...";
                            dataGridView1[4, curcl].Style.BackColor = Color.Cyan;

                            PingCl(dataGridView1[2, curcl].Value.ToString(), (int)numericUpDown1.Value * C_sec);

                            /*dataGridView1[3, i].Value = reply.Status;

                            if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                            {
                                dataGridView1[4, i].Style.BackColor = Color.GreenYellow;
                            }
                            else
                            {
                                dataGridView1[4, i].Style.BackColor = Color.Red;
                                //dataGridView1[4, 0].Style.BackColor = Color.GreenYellow;
                                //dataGridView1[4, 2].Style.BackColor = Color.Red;
                                //dataGridView1[4, 5].Style.BackColor = Color.Cyan;
                            }*/
                        }
                        break;
                    case 2:
                        if(curcl < g2)
                        {
                            PingCl(dataGridView2[2, curcl].Value.ToString(), (int)numericUpDown4.Value * C_sec);

                            /*dataGridView2[3, i].Value = reply.Status;

                            if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                            {
                                dataGridView2[4, i].Style.BackColor = Color.GreenYellow;
                            }
                            else
                            {
                                dataGridView2[4, i].Style.BackColor = Color.Red;
                            }*/
                        }
                        break;
                    case 3:
                        if(curcl < g3)
                        {
                            PingCl(dataGridView3[2, curcl].Value.ToString(), (int)numericUpDown7.Value * C_sec);

                            /*dataGridView3[3, i].Value = reply.Status;

                            if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                            {
                                dataGridView3[4, i].Style.BackColor = Color.GreenYellow;
                            }
                            else
                            {
                                dataGridView3[4, i].Style.BackColor = Color.Red;
                            }*/
                        }
                        break;
                    case 4:
                        if(curcl < g4)
                        {
                            PingCl(dataGridView4[2, curcl].Value.ToString(), (int)numericUpDown10.Value * C_sec);

                            /*dataGridView4[3, i].Value = reply.Status;

                            if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                            {
                                dataGridView4[4, i].Style.BackColor = Color.GreenYellow;
                            }
                            else
                            {
                                dataGridView4[4, i].Style.BackColor = Color.Red;
                            }*/
                        }
                        break;
                }

                fl = false;
            }
            else
            {
                // 1. Отправка пинга по второму потоку, перекрашивание ячейки в бирюзовый цвет
                // 2. При получении ответа отправка данных обратно в таблицу
                // 3. Заполение таблицы на основе полученных данных
                // 4. Переключение на нового клиента
                // 5. Повтор операций 1-4 до тех пор пока не заполнится группа
                // 6. Повтор операций 1-5 до тех пор пока не заполнятся все таблицы
                switch (curgr)
                {
                    case 1:
                        if (curcl == g1)
                            fl = true;
                        else
                            fl = false; // Продолжение программы (занесение ответа в таблицу и запрос нового пинга)
                        break;
                    case 2:
                        if (curcl == g2)
                            fl = true;
                        break;
                    case 3:
                        if (curcl == g3)
                            fl = true;
                        break;
                    case 4:
                        if (curcl == g4)
                            fl = true;
                        break;
                }
            }
        }
        
        private static void PingCl(string address, int timeout)
        {
            ping.SendAsync(address, timeout, waiter);

            waiter.WaitOne();
        }

        private void change(System.Windows.Forms.Timer timer, CheckBox check, NumericUpDown numupdown)
        {
            if (timer.Enabled)
                timer.Stop();

            if (check.Checked)
            {
                numupdown.Enabled = true;

                timer.Interval = (int)numupdown.Value * C_min;
                timer.Start();
                
                PingGroup();
            }
            else
                numupdown.Enabled = false;
        }

        private void FormResized() // form adds height and width to client size: x16, y39
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

            if(Column1b.Visible)
            {
                Column1a.Width = (dataGridView1.Width - 195) / 2;
                Column2a.Width = (dataGridView1.Width - 195) / 2;
                Column3a.Width = (dataGridView1.Width - 195) / 2;
                Column4a.Width = (dataGridView1.Width - 195) / 2;

                Column1b.Width = Column1a.Width;
                Column2b.Width = Column2a.Width;
                Column3b.Width = Column3a.Width;
                Column4b.Width = Column4a.Width;
            }
            else
            {
                Column1a.Width = dataGridView1.Width - 195;
                Column2a.Width = dataGridView1.Width - 195;
                Column3a.Width = dataGridView1.Width - 195;
                Column4a.Width = dataGridView1.Width - 195;
            }
        }
        #endregion Методы

        #region События
        private void Form1_Load(object sender, EventArgs e)
        {
            is_english = false;
            Column1b.Visible = false;
            Column2b.Visible = false;
            Column3b.Visible = false;
            Column4b.Visible = false;

            /*
            numericUpDown1.Value = ;
            numericUpDown2.Value = ;
            numericUpDown3.Value = ;
            numericUpDown4.Value = ;
            numericUpDown5.Value = ;
            numericUpDown6.Value = ;
            numericUpDown7.Value = ;
            numericUpDown8.Value = ;
            numericUpDown9.Value = ;
            numericUpDown10.Value = ;
            numericUpDown11.Value = ;
            numericUpDown12.Value = ;
            */

            Translate();
            CopyNames();
            PreProcessing();
            FillClientsList();
            FillGrids();

            //ping.PingCompleted += new System.Net.NetworkInformation.PingCompletedEventHandler(Received_reply); // обращаться к строчке 1167 дизайнера



            // Supply the state information required by the task.
            //ThreadWithState tws = new ThreadWithState("This report displays the number {0}.", 42, new ExampleCallback(ResultCallback));

            //Thread t1 = new Thread(new ThreadStart(tws.ThreadProc));//PingGroup
            //t1.Start();
            //t1.Join();



            //PingGroup();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            curgr = 1;
            PingGroup();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            curgr = 2;
            PingGroup();
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            curgr = 3;
            PingGroup();
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            curgr = 4;
            PingGroup();
        }

        private void Lang_rusTSMitem_Click(object sender, EventArgs e)
        {
            is_english = false;
            Translate();
            CopyNames();
        }

        private void Lang_engTSMitem_Click(object sender, EventArgs e)
        {
            is_english = true;
            Translate();
            CopyNames();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            curgr = 1;
            change(Timer1, checkBox1, numericUpDown2);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            curgr = 2;
            change(Timer2, checkBox2, numericUpDown5);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            curgr = 3;
            change(Timer3, checkBox3, numericUpDown8);
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            curgr = 4;
            change(Timer4, checkBox4, numericUpDown11);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            Timer1.Interval = (int)numericUpDown2.Value * C_min;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            p1 = (int)numericUpDown3.Value;
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            Timer2.Interval = (int)numericUpDown5.Value * C_min;
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            p2 = (int)numericUpDown6.Value;
        }

        private void numericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            Timer3.Interval = (int)numericUpDown8.Value * C_min;
        }

        private void numericUpDown9_ValueChanged(object sender, EventArgs e)
        {
            p3 = (int)numericUpDown9.Value;
        }

        private void numericUpDown11_ValueChanged(object sender, EventArgs e)
        {
            Timer4.Interval = (int)numericUpDown11.Value * C_min;
        }

        private void numericUpDown12_ValueChanged(object sender, EventArgs e)
        {
            p4 = (int)numericUpDown12.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Timer1.Enabled)
                Timer1.Stop();
            
            curgr = 1;
            PingGroup();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Timer2.Enabled)
                Timer2.Stop();
            
            curgr = 2;
            PingGroup();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Timer3.Enabled)
                Timer3.Stop();
            
            curgr = 3;
            PingGroup();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (Timer4.Enabled)
                Timer4.Stop();
            
            curgr = 4;
            PingGroup();
        }

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int selected_row = dataGridView1.SelectedCells[0].RowIndex;
            string s_name = dataGridView1[0, selected_row].Value.ToString();
            string s_dns = dataGridView1[1, selected_row].Value.ToString();
            string s_ip = dataGridView1[2, selected_row].Value.ToString();

            Tracking tr = new Tracking(is_english, s_name, s_dns, s_ip);
            tr.Show();
        }

        private void dataGridView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int selected_row = dataGridView2.SelectedCells[0].RowIndex;
            string s_name = dataGridView2[0, selected_row].Value.ToString();
            string s_dns = dataGridView2[1, selected_row].Value.ToString();
            string s_ip = dataGridView2[2, selected_row].Value.ToString();

            Tracking tr = new Tracking(is_english, s_name, s_dns, s_ip);
            tr.Show();
        }

        private void dataGridView3_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int selected_row = dataGridView3.SelectedCells[0].RowIndex;
            string s_name = dataGridView3[0, selected_row].Value.ToString();
            string s_dns = dataGridView3[1, selected_row].Value.ToString();
            string s_ip = dataGridView3[2, selected_row].Value.ToString();

            Tracking tr = new Tracking(is_english, s_name, s_dns, s_ip);
            tr.Show();
        }

        private void dataGridView4_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int selected_row = dataGridView4.SelectedCells[0].RowIndex;
            string s_name = dataGridView4[0, selected_row].Value.ToString();
            string s_dns = dataGridView4[1, selected_row].Value.ToString();
            string s_ip = dataGridView4[2, selected_row].Value.ToString();

            Tracking tr = new Tracking(is_english, s_name, s_dns, s_ip);
            tr.Show();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            PingGroup();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Tracking tr = new Tracking(is_english, "", "", "");
            tr.Show();
        }

        private void Switch_dnsTSMitem_Click(object sender, EventArgs e)
        {
            if (Column1b.Visible)
            {
                Column1b.Visible = false;
                Column2b.Visible = false;
                Column3b.Visible = false;
                Column4b.Visible = false;

                MinimumSize = new Size(656, 519);
            }
            else
            {
                Column1b.Visible = true;
                Column2b.Visible = true;
                Column3b.Visible = true;
                Column4b.Visible = true;

                MinimumSize = new Size(896, 519);
            }
            FormResized();
        }
        
        private void Received_reply(object sender, System.Net.NetworkInformation.PingCompletedEventArgs e)
        {
            //this.ping.PingCompleted += new System.Net.NetworkInformation.PingCompletedEventHandler(Received_reply);
            if (e.Cancelled)
                ((AutoResetEvent)e.UserState).Set();

            if (e.Error != null)
                ((AutoResetEvent)e.UserState).Set();

            // Let the main thread resume.
            ((AutoResetEvent)e.UserState).Set();

            reply = e.Reply;

            DisplayReply();
        }

        private void DisplayReply()
        {
            switch(curgr)
            {
                case 1:
                    dataGridView1[3, curcl].Value = reply.Status;

                    if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                        dataGridView1[4, curcl].Style.BackColor = Color.GreenYellow;
                    else
                        dataGridView1[4, curcl].Style.BackColor = Color.Red;

                    if(curcl < g1)
                        PingGroup();
                    break;
                case 2:
                    dataGridView2[3, curcl].Value = reply.Status;

                    if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                        dataGridView2[4, curcl].Style.BackColor = Color.GreenYellow;
                    else
                        dataGridView2[4, curcl].Style.BackColor = Color.Red;

                    if(curcl < g2)
                        PingGroup();
                    break;
                case 3:
                    dataGridView3[3, curcl].Value = reply.Status;

                    if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                        dataGridView3[4, curcl].Style.BackColor = Color.GreenYellow;
                    else
                        dataGridView3[4, curcl].Style.BackColor = Color.Red;

                    if(curcl < g3)
                        PingGroup();
                    break;
                case 4:
                    dataGridView4[3, curcl].Value = reply.Status;

                    if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                        dataGridView4[4, curcl].Style.BackColor = Color.GreenYellow;
                    else
                        dataGridView4[4, curcl].Style.BackColor = Color.Red;

                    if(curcl < g4)
                        PingGroup();
                    break;
            }
        }
        
        private void Form_ChangedSize(object sender, EventArgs e)
        {
            FormResized();
        }
        #endregion События
    }

    class CurrentClient
    {
        public int counter { get; set; }
        public int group { get; set; }
        public string ip { get; set; }
        public string status { get; set; }

        public delegate void MethodContainer(); // создание делегата для события

        public event MethodContainer OnCount; // создание события с типом делегата

        public void Count()
        {
            OnCount(); // вызов события
        }
    }

    public class ThreadWithState
    {
        // State information used in the task.
        private string boilerplate;
        private int numberValue;

        // Delegate used to execute the callback method when the
        // task is complete.
        private ExampleCallback callback;

        // The constructor obtains the state information and the
        // callback delegate.
        public ThreadWithState(string text, int number,
            ExampleCallback callbackDelegate)
        {
            boilerplate = text;
            numberValue = number;
            callback = callbackDelegate;
        }

        // The thread procedure performs the task, such as
        // formatting and printing a document, and then invokes
        // the callback delegate with the number of lines printed.
        public void ThreadProc()
        {
            Console.WriteLine(boilerplate, numberValue);
            if (callback != null)
                callback(1);
        }
    }

    // Delegate that defines the signature for the callback method.
    //
    public delegate void ExampleCallback(int lineCount);
}
