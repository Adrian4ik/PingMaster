using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        bool is_english = false, // проверка на использование английской версии программы
            tracking = false; // проверка открытого окна слежения (для того, чтобы не создавалось более 1 окна слежения)

        int[,] g_settings = new int[8, 4]; // включает в себя вышеописанные строки кода
        // настройки групп в виде: [группа (1-8), настройки (кол-во клиентов/кол-во запросов/текущий клиент/текущий таймаут)]

        private string pinging, check_connection;

        string[] ip_g1, ip_g2, ip_g3, ip_g4, ip_g5, ip_g6, ip_g7, ip_g8, //списки ip адресов каждой группы
            al; // сырой список абонентов из файла

        string[,][] g_lists = new string[8,13][]; // включает в себя вышеописанную строку кода, а также заменяет нижеописанную структуру данных client
        // трёхмерный список клиентов в виде: [группа (1-8), аргументы (имя/dns/ip/время 1 опроса/ответ 1 опроса/время 2 опроса/.../ответ 5 опроса)] [клиент]

        static System.Net.NetworkInformation.Ping ping_g1 = new System.Net.NetworkInformation.Ping();
        static System.Net.NetworkInformation.Ping ping_g2 = new System.Net.NetworkInformation.Ping();
        static System.Net.NetworkInformation.Ping ping_g3 = new System.Net.NetworkInformation.Ping();
        static System.Net.NetworkInformation.Ping ping_g4 = new System.Net.NetworkInformation.Ping();
        static System.Net.NetworkInformation.Ping ping_g5 = new System.Net.NetworkInformation.Ping();
        static System.Net.NetworkInformation.Ping ping_g6 = new System.Net.NetworkInformation.Ping();
        static System.Net.NetworkInformation.Ping ping_g7 = new System.Net.NetworkInformation.Ping();
        static System.Net.NetworkInformation.Ping ping_g8 = new System.Net.NetworkInformation.Ping();

        System.Net.NetworkInformation.PingReply reply_g1;
        System.Net.NetworkInformation.PingReply reply_g2;
        System.Net.NetworkInformation.PingReply reply_g3;
        System.Net.NetworkInformation.PingReply reply_g4;
        System.Net.NetworkInformation.PingReply reply_g5;
        System.Net.NetworkInformation.PingReply reply_g6;
        System.Net.NetworkInformation.PingReply reply_g7;
        System.Net.NetworkInformation.PingReply reply_g8;

        static AutoResetEvent waiter1 = new AutoResetEvent(false);
        static AutoResetEvent waiter2 = new AutoResetEvent(false);
        static AutoResetEvent waiter3 = new AutoResetEvent(false);
        static AutoResetEvent waiter4 = new AutoResetEvent(false);
        static AutoResetEvent waiter5 = new AutoResetEvent(false);
        static AutoResetEvent waiter6 = new AutoResetEvent(false);
        static AutoResetEvent waiter7 = new AutoResetEvent(false);
        static AutoResetEvent waiter8 = new AutoResetEvent(false);

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
        private const int C_sec = 1000, C_min = 60000;

        private string[] StandartList = new string[]
            {"Loopback/ /127.0.0.1", "БРИ-1/ /10.1.1.254", "БРИ-2/ /10.1.2.254", "БРИ-3/ /192.168.60.254", "SM BelAir WAP/ /192.168.68.73", "АСП/ /10.1.2.250", "",
            "USL ER-SWB-J1,J2/ /192.168.67.250", "USL ER-SWA/ /192.168.60.253", "ISS Server 1/ /192.168.60.51", "LS1/ /192.168.60.53", "Lab printer/ /192.168.60.82", "",
            "RSE1/ /10.1.1.3", "RSK1/ /10.1.1.4", "RSK2/ /10.1.1.5", "RSS1/ /10.1.2.1", "RSS2/ /10.1.1.1", "RSE-Med/ /10.1.1.7", "Mediaserver AGAT/ /10.1.1.80", "SM Printer/ /192.168.60.81", "",
            "FS1/ /10.1.1.100", "ТВМ1-Н/ /10.1.3.1", "БПИ-НЧ (TRPU)/ /192.168.249.1", "БЗУ/ /10.1.11.5", "MDM (ШСС)/ /10.1.3.50"};
        #endregion Константы

        #region Методы
        public Form1()
        {
            InitializeComponent();
        }

        private void Translate()
        {
            if (is_english)
            {
                toolStripButton1.Text = "File";
                Open_iniTSMitem.Text = "Open .INI file";
                Open_logTSMitem.Text = "Open log file";
                Open_clientsTSMitem.Text = "Open clients list";
                toolStripButton2.Text = "Ping all";
                toolStripButton3.Text = "Tracking";
                toolStripButton4.Text = "Settings";
                LanguageTSMitem.Text = "Language";
                Lang_rusTSMitem.Text = "Russian";
                Lang_engTSMitem.Text = "English";
                TablesTSMitem.Text = "Tables";
                Switch_dnsTSMitem.Text = "On/Off DNS";
                Switch_ipTSMitem.Text = "On/Off IP";
                Switch_timeTSMitem.Text = "On/Off Time";
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

                pinging = "Pinging...";
                check_connection = "No connection to the network." + Environment.NewLine + "Check cable connection or network/firewall settings.";
            }
            else
            {
                toolStripButton1.Text = "Файл";
                Open_iniTSMitem.Text = "Открыть .INI файл";
                Open_logTSMitem.Text = "Открыть лог файл";
                Open_clientsTSMitem.Text = "Открыть список клиентов";
                toolStripButton2.Text = "Пинг всех";
                toolStripButton3.Text = "Слежение";
                toolStripButton4.Text = "Настройки";
                LanguageTSMitem.Text = "Язык";
                Lang_rusTSMitem.Text = "Русский";
                Lang_engTSMitem.Text = "Английский";
                TablesTSMitem.Text = "Таблицы";
                Switch_dnsTSMitem.Text = "Вкл/Выкл DNS";
                Switch_ipTSMitem.Text = "Вкл/Выкл IP";
                Switch_timeTSMitem.Text = "Вкл/Выкл Время";
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

                pinging = "Опрос...";
                check_connection = "Нет подключения к сети" + Environment.NewLine + "Проверьте подключение сетевого кабеля или настройки сети/фаерволла";
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
            Config();

            if (!File.Exists("Log" + ".txt"))
            {
                FileStream f = File.Create("Log" + ".txt");
                f.Close();
            }
            File.AppendAllText("Log" + ".txt", "Программа запущена " + DateTime.Now.Date.ToString().Substring(0, 11) + " в " + DateTime.Now.ToString().Substring(11) + "." + DateTime.Now.Millisecond.ToString() + Environment.NewLine);
            File.AppendAllText("Log" + ".txt", Environment.NewLine);

            if (!File.Exists("Clients.txt"))
            {
                FileStream f = File.Create("Clients.txt");
                f.Close();
                File.WriteAllLines("Clients.txt", StandartList);
                al = StandartList;
            }

            al = File.ReadAllLines("Clients.txt");
            ab = new client[al.Count()];

            g_lists[1, 0] = new string[322];
            g_lists[1, 0][228] = "kek";

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

        private void Config()
        {
            if (!File.Exists("Config.ini"))
            {
                FileStream f = File.Create("Config.ini");
                f.Close();
            }

            Timer1.Interval = (int)numericUpDown2.Value * C_min;
            Timer2.Interval = (int)numericUpDown5.Value * C_min;
            Timer3.Interval = (int)numericUpDown8.Value * C_min;
            Timer4.Interval = (int)numericUpDown11.Value * C_min;

            is_english = false;

            Column1b.Visible = false;
            Column1e.Visible = false;

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
        }

        private void FillClientsList()
        {
            for (int s = 0, grid = 0; s < al.Count(); s++)
            {
                if (al[s] == "")
                {
                    ab[s].Name = "";
                    grid++;
                }
                else
                {
                    string str = al[s];

                    for (int c = 0, flag = 1; c < al[s].Length; c++)
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
                ab[s].Group = grid + 1;

                switch (grid)
                {
                    case 0:
                        ab[s].PosInGr = s;
                        g_settings[0, 0]++;
                        break;
                    case 1:
                        ab[s].PosInGr = s - g_settings[0, 0];
                        g_settings[1, 0]++;
                        break;
                    case 2:
                        ab[s].PosInGr = s - g_settings[0, 0] - g_settings[1, 0];
                        g_settings[2, 0]++;
                        break;
                    case 3:
                        ab[s].PosInGr = s - g_settings[0, 0] - g_settings[1, 0] - g_settings[2, 0];
                        g_settings[3, 0]++;
                        break;
                }
            }
            g_settings[1, 0]--;
            g_settings[2, 0]--;
            g_settings[3, 0]--;

            ip_g1 = new string[g_settings[0, 0]];
            ip_g2 = new string[g_settings[1, 0]];
            ip_g3 = new string[g_settings[2, 0]];
            ip_g4 = new string[g_settings[3, 0]];
        }

        private void FillGrids()
        {
            for (int i = 0, j = 0; i < ab.Count(); i++, j++)
            {
                if (ab[i].Name == "")
                    j = -1;
                else
                {
                    switch (ab[i].Group)
                    {
                        case 1:
                            dataGridView1[0, j].Value = ab[i].Name;
                            dataGridView1[1, j].Value = ab[i].DNS;
                            dataGridView1[2, j].Value = ab[i].IP;
                            ip_g1[j] = ab[i].IP;
                            break;
                        case 2:
                            dataGridView2[0, j].Value = ab[i].Name;
                            dataGridView2[1, j].Value = ab[i].DNS;
                            dataGridView2[2, j].Value = ab[i].IP;
                            ip_g2[j] = ab[i].IP;
                            break;
                        case 3:
                            dataGridView3[0, j].Value = ab[i].Name;
                            dataGridView3[1, j].Value = ab[i].DNS;
                            dataGridView3[2, j].Value = ab[i].IP;
                            ip_g3[j] = ab[i].IP;
                            break;
                        case 4:
                            dataGridView4[0, j].Value = ab[i].Name;
                            dataGridView4[1, j].Value = ab[i].DNS;
                            dataGridView4[2, j].Value = ab[i].IP;
                            ip_g4[j] = ab[i].IP;
                            break;
                    }
                }
            }
        }

        private void SortPing(int group)
        {
            switch(group)
            {
                case 1:
                    PingGroup(group, dataGridView1, button1, checkBox1, ping_g1, (int)numericUpDown1.Value * C_sec, waiter1);
                    break;
                case 2:
                    PingGroup(group, dataGridView2, button2, checkBox1, ping_g2, (int)numericUpDown4.Value * C_sec, waiter2);
                    break;
                case 3:
                    PingGroup(group, dataGridView3, button3, checkBox1, ping_g3, (int)numericUpDown7.Value * C_sec, waiter3);
                    break;
                case 4:
                    PingGroup(group, dataGridView4, button4, checkBox1, ping_g4, (int)numericUpDown10.Value * C_sec, waiter4);
                    break;
            }
        }

        private void PingGroup(int current_group, DataGridView grid, Button button, CheckBox check, System.Net.NetworkInformation.Ping ping, int timeout, AutoResetEvent waiter)
        {
            current_group--;
            toolStripButton2.Enabled = false;

            if (g_settings[current_group, 2] < g_settings[current_group, 0])
            {
                if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                {
                    button.Enabled = false;
                    check.Enabled = false;

                    grid[0, g_settings[current_group, 2]].Selected = true;
                    grid[3, g_settings[current_group, 2]].Value = pinging;
                    grid[3, g_settings[current_group, 2]].Style.BackColor = Color.Cyan;
                    grid[3, g_settings[current_group, 2]].Style.SelectionForeColor = Color.White;
                    grid[3, g_settings[current_group, 2]].Style.SelectionBackColor = Color.DarkCyan;

                    PingCl(ping, grid[2, g_settings[current_group, 2]].Value.ToString(), timeout, waiter);
                }
                else
                    MessageBox.Show(check_connection);
            }
            else
            {
                if (!File.Exists("Log" + ".txt"))
                {
                    FileStream f = File.Create("Log" + ".txt");
                    f.Close();
                    File.AppendAllText("Log" + ".txt", "Программа запущена " + DateTime.Now.Date.ToString().Substring(0, 11) + " в " + DateTime.Now.ToString().Substring(11) + "." + DateTime.Now.Millisecond.ToString() + Environment.NewLine);
                    File.AppendAllText("Log" + ".txt", Environment.NewLine);
                }
                else
                    File.AppendAllText("Log" + ".txt", Environment.NewLine);
            }
        }

        private static void PingCl(System.Net.NetworkInformation.Ping ping, string address, int timeout, AutoResetEvent waiter)
        {
            ping.SendAsync(address, timeout, waiter);
        }

        private void SortReply(int group)
        {
            if (!File.Exists("Log" + ".txt"))
            {
                FileStream f = File.Create("Log" + ".txt");
                f.Close();
                File.AppendAllText("Log" + ".txt", "Программа запущена " + DateTime.Now.Date.ToString().Substring(0, 11) + " в " + DateTime.Now.ToString().Substring(11) + "." + DateTime.Now.Millisecond.ToString() + Environment.NewLine);
                File.AppendAllText("Log" + ".txt", Environment.NewLine);
            }
            toolStripButton2.Enabled = true;

            switch (group)
            {
                case 1:
                    DisplayReply(group, dataGridView1, reply_g1, button1, checkBox1);
                    break;
                case 2:
                    DisplayReply(group, dataGridView2, reply_g2, button2, checkBox2);
                    break;
                case 3:
                    DisplayReply(group, dataGridView3, reply_g3, button3, checkBox3);
                    break;
                case 4:
                    DisplayReply(group, dataGridView4, reply_g4, button4, checkBox4);
                    break;
            }
        }
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        private void DisplayReply(int current_group, DataGridView grid, System.Net.NetworkInformation.PingReply reply, Button button, CheckBox check)
        {
            current_group--;

            grid[3, g_settings[current_group, 2]].Value = reply.Status;

            string c = "Опрос " + dataGridView1[0, g_settings[current_group, 2]].Value + " " + dataGridView1[1, g_settings[current_group, 2]].Value + " " + dataGridView1[2, g_settings[current_group, 2]].Value + Environment.NewLine;

            File.AppendAllText("Log.txt", c);

            c = DateTime.Now.ToString().Substring(11) + "." + DateTime.Now.Millisecond.ToString();
            c += " " + reply_g1.Status;

            if (reply_g1.Status == System.Net.NetworkInformation.IPStatus.Success)
            {
                grid[3, g_settings[current_group, 2]].Style.BackColor = Color.GreenYellow;
                grid[3, g_settings[current_group, 2]].Style.SelectionBackColor = Color.DarkGreen;
                c += " " + reply_g1.RoundtripTime + " ms";

                //System.Net.NetworkInformation.IPAddressInformation iP;
                //long huh = 228;
                //System.Net.IPAddress kek = new System.Net.IPAddress(huh);
            }
            else
            {
                grid[3, g_settings[current_group, 2]].Style.BackColor = Color.Red;
                grid[3, g_settings[current_group, 2]].Style.SelectionBackColor = Color.DarkRed;
            }

            File.AppendAllText("Log.txt", c + Environment.NewLine);

            if (g_settings[current_group, 2] == g_settings[2, 1])
            {
                g_settings[current_group, 2]++;
                g_settings[current_group, 2] = 0;
            }
            else
                g_settings[current_group, 2]++;

            if (g_settings[current_group, 2] < g_settings[current_group, 0])
                SortPing(current_group + 1);
            else
            {
                g_settings[current_group, 2] = 0;
            }
            button.Enabled = true;
            check.Enabled = true;
        }

        private void CheckChange(DataGridView grid, System.Windows.Forms.Timer timer, CheckBox check, NumericUpDown numupdown, Label label_a, Label label_b, int currrent_group, int count)
        {
            if (timer.Enabled)
                timer.Stop();

            if (check.Checked)
            {
                label_a.Enabled = true;
                label_b.Enabled = true;
                numupdown.Enabled = true;

                timer.Interval = (int)numupdown.Value * C_min;
                timer.Start();

                for (int i = 0; i < 8; i++)
                {
                    g_settings[i, 2] = 0;
                }

                ClearGrid(grid, count);
                SortPing(currrent_group);
            }
            else
            {
                label_a.Enabled = false;
                label_b.Enabled = false;
                numupdown.Enabled = false;
            }
        }

        private void ClearGrid(DataGridView grid, int count)
        {
            for (int i = 0; i < count; i++)
            {
                grid[3, i].Value = "";
                grid[4, i].Value = "";

                grid[3, i].Style.BackColor = Color.White;
                grid[3, i].Style.SelectionForeColor = Color.Black;
                grid[3, i].Style.SelectionBackColor = Color.LightGray;
                grid[4, i].Style.BackColor = Color.White;
                grid[4, i].Style.SelectionBackColor = Color.LightGray;
            }
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
                Column1b.Width = (dataGridView1.Width - 205) / 2;
                Column2b.Width = (dataGridView1.Width - 230) / 2;
                Column3b.Width = (dataGridView1.Width - 230) / 2;
                Column4b.Width = (dataGridView1.Width - 230) / 2;

                Column1d.Width = Column1b.Width;
                Column2d.Width = Column1b.Width;
                Column3d.Width = Column1b.Width;
                Column4d.Width = Column1b.Width;
            }
            else
            {
                Column1d.Width = dataGridView1.Width - 205;
                Column2d.Width = dataGridView1.Width - 230;
                Column3d.Width = dataGridView1.Width - 230;
                Column4d.Width = dataGridView1.Width - 230;
            }
        }
        #endregion Методы

        #region События
        private void Form1_Load(object sender, EventArgs e)
        {
            ping_g1.PingCompleted += new System.Net.NetworkInformation.PingCompletedEventHandler(Received_reply_g1);
            ping_g2.PingCompleted += new System.Net.NetworkInformation.PingCompletedEventHandler(Received_reply_g2);
            ping_g3.PingCompleted += new System.Net.NetworkInformation.PingCompletedEventHandler(Received_reply_g3);
            ping_g4.PingCompleted += new System.Net.NetworkInformation.PingCompletedEventHandler(Received_reply_g4);
            ping_g5.PingCompleted += new System.Net.NetworkInformation.PingCompletedEventHandler(Received_reply_g5);
            ping_g6.PingCompleted += new System.Net.NetworkInformation.PingCompletedEventHandler(Received_reply_g6);
            ping_g7.PingCompleted += new System.Net.NetworkInformation.PingCompletedEventHandler(Received_reply_g7);
            ping_g8.PingCompleted += new System.Net.NetworkInformation.PingCompletedEventHandler(Received_reply_g8);

            Translate();
            CopyNames();
            PreProcessing();
            FillClientsList();
            FillGrids();

            Size = new Size(816, 639);
            FormResized();

            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                SortPing(1);
                SortPing(2);
                SortPing(3);
                SortPing(4);
            }
            else
                MessageBox.Show(check_connection);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ClearGrid(dataGridView1, g_settings[0, 0]);
            SortPing(1);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            ClearGrid(dataGridView2, g_settings[1, 0]);
            SortPing(2);
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            ClearGrid(dataGridView3, g_settings[2, 0]);
            SortPing(3);
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            ClearGrid(dataGridView4, g_settings[3, 0]);
            SortPing(4);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CheckChange(dataGridView1, Timer1, checkBox1, numericUpDown2, label2, label12, 1, g_settings[0, 0]);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            CheckChange(dataGridView2, Timer2, checkBox2, numericUpDown5, label4, label14, 2, g_settings[1, 0]);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            CheckChange(dataGridView3, Timer3, checkBox3, numericUpDown8, label6, label16, 3, g_settings[2, 0]);
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            CheckChange(dataGridView4, Timer4, checkBox4, numericUpDown11, label8, label18, 4, g_settings[3, 0]);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            Timer1.Interval = (int)numericUpDown2.Value * C_min;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            g_settings[0, 1] = (int)numericUpDown3.Value;
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            Timer2.Interval = (int)numericUpDown5.Value * C_min;
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            g_settings[1, 1] = (int)numericUpDown6.Value;
        }

        private void numericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            Timer3.Interval = (int)numericUpDown8.Value * C_min;
        }

        private void numericUpDown9_ValueChanged(object sender, EventArgs e)
        {
            g_settings[2, 1] = (int)numericUpDown9.Value;
        }

        private void numericUpDown11_ValueChanged(object sender, EventArgs e)
        {
            Timer4.Interval = (int)numericUpDown11.Value * C_min;
        }

        private void numericUpDown12_ValueChanged(object sender, EventArgs e)
        {
            g_settings[3, 1] = (int)numericUpDown12.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Timer1.Enabled)
                Timer1.Stop();

            ClearGrid(dataGridView1, g_settings[0, 0]);
            SortPing(1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Timer2.Enabled)
                Timer2.Stop();

            ClearGrid(dataGridView2, g_settings[1, 0]);
            SortPing(2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Timer3.Enabled)
                Timer3.Stop();

            ClearGrid(dataGridView3, g_settings[2, 0]);
            SortPing(3);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (Timer4.Enabled)
                Timer4.Stop();

            ClearGrid(dataGridView4, g_settings[3, 0]);
            SortPing(4);
        }

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int selected_row = dataGridView1.SelectedCells[0].RowIndex;
            string s_name = dataGridView1[0, selected_row].Value.ToString();
            string s_dns = dataGridView1[1, selected_row].Value.ToString();
            string s_ip = dataGridView1[2, selected_row].Value.ToString();

            if (!tracking)
            {
                Tracking tr = new Tracking(is_english, s_name, s_dns, s_ip);
                tr.FormClosed += new System.Windows.Forms.FormClosedEventHandler(Tracking_Closed);
                toolStripButton3.Enabled = false;
                tracking = true;
                tr.Show();
            }
        }

        private void dataGridView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int selected_row = dataGridView2.SelectedCells[0].RowIndex;
            string s_name = dataGridView2[0, selected_row].Value.ToString();
            string s_dns = dataGridView2[1, selected_row].Value.ToString();
            string s_ip = dataGridView2[2, selected_row].Value.ToString();

            if (!tracking)
            {
                Tracking tr = new Tracking(is_english, s_name, s_dns, s_ip);
                tr.FormClosed += new System.Windows.Forms.FormClosedEventHandler(Tracking_Closed);
                toolStripButton3.Enabled = false;
                tracking = true;
                tr.Show();
            }
        }

        private void dataGridView3_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int selected_row = dataGridView3.SelectedCells[0].RowIndex;
            string s_name = dataGridView3[0, selected_row].Value.ToString();
            string s_dns = dataGridView3[1, selected_row].Value.ToString();
            string s_ip = dataGridView3[2, selected_row].Value.ToString();

            if (!tracking)
            {
                Tracking tr = new Tracking(is_english, s_name, s_dns, s_ip);
                tr.FormClosed += new System.Windows.Forms.FormClosedEventHandler(Tracking_Closed);
                toolStripButton3.Enabled = false;
                tracking = true;
                tr.Show();
            }
        }

        private void dataGridView4_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int selected_row = dataGridView4.SelectedCells[0].RowIndex;
            string s_name = dataGridView4[0, selected_row].Value.ToString();
            string s_dns = dataGridView4[1, selected_row].Value.ToString();
            string s_ip = dataGridView4[2, selected_row].Value.ToString();

            if (!tracking)
            {
                Tracking tr = new Tracking(is_english, s_name, s_dns, s_ip);
                tr.FormClosed += new System.Windows.Forms.FormClosedEventHandler(Tracking_Closed);
                toolStripButton3.Enabled = false;
                tracking = true;
                tr.Show();
            }
        }

        private void Open_iniTSMitem_Click(object sender, EventArgs e)
        {
            if (File.Exists("D:\\Амир\\Уст. программы\\Notepad++\\notepad++.exe"))
                Process.Start("D:\\Амир\\Уст. программы\\Notepad++\\notepad++.exe", "Config.ini");
            else
                Process.Start("C:\\Windows\\System32\\notepad.exe", "Config.ini");
        }

        private void Open_logTSMitem_Click(object sender, EventArgs e)
        {
            if (File.Exists("D:\\Амир\\Уст. программы\\Notepad++\\notepad++.exe") && File.Exists("Log" + ".txt"))
                Process.Start("D:\\Амир\\Уст. программы\\Notepad++\\notepad++.exe", "Log" + ".txt");
            else
                Process.Start("C:\\Windows\\System32\\notepad.exe", "Log" + ".txt");
        }

        private void Open_clientsTSMitem_Click(object sender, EventArgs e)
        {
            if (File.Exists("D:\\Амир\\Уст. программы\\Notepad++\\notepad++.exe") && File.Exists("Clients.txt"))
                Process.Start("D:\\Амир\\Уст. программы\\Notepad++\\notepad++.exe", "Clients.txt");
            else
                Process.Start("C:\\Windows\\System32\\notepad.exe", "Clients.txt");
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            ClearGrid(dataGridView1, g_settings[0, 0]);
            ClearGrid(dataGridView2, g_settings[1, 0]);
            ClearGrid(dataGridView3, g_settings[2, 0]);
            ClearGrid(dataGridView4, g_settings[3, 0]);

            SortPing(1);
            SortPing(2);
            SortPing(3);
            SortPing(4);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Tracking tr = new Tracking(is_english, "", "", "");
            tr.FormClosed += new System.Windows.Forms.FormClosedEventHandler(Tracking_Closed);
            toolStripButton3.Enabled = false;
            tracking = true;
            tr.Show();
        }

        private void Tracking_Closed(object sender, FormClosedEventArgs e)
        {
            toolStripButton3.Enabled = true;
            tracking = false;
        }

        private void Lang_rusTSMitem_Click(object sender, EventArgs e)
        {
            is_english = false;
            Translate();
            CopyNames();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            File.AppendAllText("Log" + ".txt", "Программа закрыта " + DateTime.Now.Date.ToString().Substring(0, 11) + " в " + DateTime.Now.ToString().Substring(11) + "." + DateTime.Now.Millisecond.ToString() + Environment.NewLine);
            File.AppendAllText("Log" + ".txt", Environment.NewLine);
        }

        private void Lang_engTSMitem_Click(object sender, EventArgs e)
        {
            is_english = true;
            Translate();
            CopyNames();
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
        
        private void Received_reply_g1(object sender, System.Net.NetworkInformation.PingCompletedEventArgs e)
        {
            if (e.Cancelled)
                ((AutoResetEvent)e.UserState).Set();

            if (e.Error != null)
                ((AutoResetEvent)e.UserState).Set();

            // Let the main thread resume.
            ((AutoResetEvent)e.UserState).Set();

            reply_g1 = e.Reply;

            SortReply(1);
        }
        
        private void Received_reply_g2(object sender, System.Net.NetworkInformation.PingCompletedEventArgs e)
        {
            if (e.Cancelled)
                ((AutoResetEvent)e.UserState).Set();

            if (e.Error != null)
                ((AutoResetEvent)e.UserState).Set();

            // Let the main thread resume.
            ((AutoResetEvent)e.UserState).Set();

            reply_g2 = e.Reply;

            SortReply(2);
        }
        
        private void Received_reply_g3(object sender, System.Net.NetworkInformation.PingCompletedEventArgs e)
        {
            if (e.Cancelled)
                ((AutoResetEvent)e.UserState).Set();

            if (e.Error != null)
                ((AutoResetEvent)e.UserState).Set();

            // Let the main thread resume.
            ((AutoResetEvent)e.UserState).Set();

            reply_g3 = e.Reply;

            SortReply(3);
        }
        
        private void Received_reply_g4(object sender, System.Net.NetworkInformation.PingCompletedEventArgs e)
        {
            if (e.Cancelled)
                ((AutoResetEvent)e.UserState).Set();

            if (e.Error != null)
                ((AutoResetEvent)e.UserState).Set();

            // Let the main thread resume.
            ((AutoResetEvent)e.UserState).Set();

            reply_g4 = e.Reply;

            SortReply(4);
        }
        
        private void Received_reply_g5(object sender, System.Net.NetworkInformation.PingCompletedEventArgs e)
        {
            if (e.Cancelled)
                ((AutoResetEvent)e.UserState).Set();

            if (e.Error != null)
                ((AutoResetEvent)e.UserState).Set();

            // Let the main thread resume.
            ((AutoResetEvent)e.UserState).Set();

            reply_g5 = e.Reply;

            SortReply(5);
        }
        
        private void Received_reply_g6(object sender, System.Net.NetworkInformation.PingCompletedEventArgs e)
        {
            if (e.Cancelled)
                ((AutoResetEvent)e.UserState).Set();

            if (e.Error != null)
                ((AutoResetEvent)e.UserState).Set();

            // Let the main thread resume.
            ((AutoResetEvent)e.UserState).Set();

            reply_g6 = e.Reply;

            SortReply(6);
        }
        
        private void Received_reply_g7(object sender, System.Net.NetworkInformation.PingCompletedEventArgs e)
        {
            if (e.Cancelled)
                ((AutoResetEvent)e.UserState).Set();

            if (e.Error != null)
                ((AutoResetEvent)e.UserState).Set();

            // Let the main thread resume.
            ((AutoResetEvent)e.UserState).Set();

            reply_g7 = e.Reply;

            SortReply(7);
        }
        
        private void Received_reply_g8(object sender, System.Net.NetworkInformation.PingCompletedEventArgs e)
        {
            if (e.Cancelled)
                ((AutoResetEvent)e.UserState).Set();

            if (e.Error != null)
                ((AutoResetEvent)e.UserState).Set();

            // Let the main thread resume.
            ((AutoResetEvent)e.UserState).Set();

            reply_g8 = e.Reply;

            SortReply(8);
        }
        
        private void Form_ChangedSize(object sender, EventArgs e)
        {
            FormResized();
        }
        #endregion События
    }
}
