using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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

        int groups_count = 0;

        int[,] g_settings = new int[8, 5];
        // настройки групп в виде: [группа (1-8), настройки (кол-во клиентов/текущий клиент/текущий период автопинга/текущее кол-во запросов/текущий таймаут)]

        string pinging, check_connection, fill_clients_list,
            date = DateTime.Now.Date.ToString().Substring(0, 11);

        string[] ip_g1, ip_g2, ip_g3, ip_g4, ip_g5, ip_g6, ip_g7, ip_g8, //списки ip адресов каждой группы
            al; // сырой список абонентов из файла

        string[,][] g_lists = new string[8, 11][]; // включает в себя вышеописанную строку кода, а также заменяет нижеописанную структуру данных client
        // трёхмерный список клиентов в виде: [группа (1-8), аргументы (имя/dns/ip/время 1 опроса/ответ 1 опроса/время 2 опроса/.../ответ 4 опроса)] [клиент]

        static Ping ping_g1 = new Ping();
        static Ping ping_g2 = new Ping();
        static Ping ping_g3 = new Ping();
        static Ping ping_g4 = new Ping();
        static Ping ping_g5 = new Ping();
        static Ping ping_g6 = new Ping();
        static Ping ping_g7 = new Ping();
        static Ping ping_g8 = new Ping();

        static AutoResetEvent waiter1 = new AutoResetEvent(false);
        static AutoResetEvent waiter2 = new AutoResetEvent(false);
        static AutoResetEvent waiter3 = new AutoResetEvent(false);
        static AutoResetEvent waiter4 = new AutoResetEvent(false);
        static AutoResetEvent waiter5 = new AutoResetEvent(false);
        static AutoResetEvent waiter6 = new AutoResetEvent(false);
        static AutoResetEvent waiter7 = new AutoResetEvent(false);
        static AutoResetEvent waiter8 = new AutoResetEvent(false);

        PingReply reply_g1;
        PingReply reply_g2;
        PingReply reply_g3;
        PingReply reply_g4;
        PingReply reply_g5;
        PingReply reply_g6;
        PingReply reply_g7;
        PingReply reply_g8;

        Settings settings;
        Tracking tr;

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

        private void PreProcessing()
        {
            Translate();
            CopyText();
            CheckConfig();
            CheckLog();

            File.AppendAllText(date + " log.txt", "Программа запущена " + date + " в " + DateTime.Now.ToString().Substring(11) + "." + DateTime.Now.Millisecond.ToString() + Environment.NewLine);
            File.AppendAllText(date + " log.txt", Environment.NewLine);

            if (!File.Exists("Clients.txt"))
            {
                FileStream f = File.Create("Clients.txt");
                f.Close();
                File.WriteAllLines("Clients.txt", StandartList);
            }

            al = File.ReadAllLines("Clients.txt");

            if (al.Count() > 0)
            {
                groups_count++;

                for (int s = 0; s < al.Count(); s++)
                {
                    if (al[s] == "")
                        groups_count++;
                    else
                        g_settings[groups_count - 1, 0]++;
                }

                for(int i = 0; i < 8; i++)
                {
                    g_lists[1, 0] = new string[g_settings[i, 0]];
                }
            }
            else
                MessageBox.Show(fill_clients_list);

            ping_g1.PingCompleted += new PingCompletedEventHandler(Received_reply_g1);
            ping_g2.PingCompleted += new PingCompletedEventHandler(Received_reply_g2);
            ping_g3.PingCompleted += new PingCompletedEventHandler(Received_reply_g3);
            ping_g4.PingCompleted += new PingCompletedEventHandler(Received_reply_g4);
            ping_g5.PingCompleted += new PingCompletedEventHandler(Received_reply_g5);
            ping_g6.PingCompleted += new PingCompletedEventHandler(Received_reply_g6);
            ping_g7.PingCompleted += new PingCompletedEventHandler(Received_reply_g7);
            ping_g8.PingCompleted += new PingCompletedEventHandler(Received_reply_g8);

            FillClientsList();
            FillGrids();
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

                checkBox1.Text = "Autoping 1st group";
                checkBox2.Text = "Autoping 2nd group";
                checkBox3.Text = "Autoping 3rd group";
                checkBox4.Text = "Autoping 4th group";

                button11.Text = "Ping 1st group";
                button12.Text = "Ping 2nd group";
                button13.Text = "Ping 3rd group";
                button14.Text = "Ping 4th group";

                pinging = "Pinging...";
                check_connection = "No connection to the network." + Environment.NewLine + "Check cable connection or network/firewall settings.";
                fill_clients_list = "Fill clients list.";
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

                checkBox1.Text = "Автопинг 1 группы";
                checkBox2.Text = "Автопинг 2 группы";
                checkBox3.Text = "Автопинг 3 группы";
                checkBox4.Text = "Автопинг 4 группы";

                button11.Text = "Пинг 1 группы";
                button12.Text = "Пинг 2 группы";
                button13.Text = "Пинг 3 группы";
                button14.Text = "Пинг 4 группы";

                pinging = "Опрос...";
                check_connection = "Нет подключения к сети" + Environment.NewLine + "Проверьте подключение сетевого кабеля или настройки сети/фаерволла";
                fill_clients_list = "Заполните список клиентов.";
            }
        }

        private void CopyText()
        {
            Column2a.HeaderText = Column1a.HeaderText;
            Column3a.HeaderText = Column1a.HeaderText;
            Column4a.HeaderText = Column1a.HeaderText;
            //Column5a.HeaderText = Column1a.HeaderText;
            //Column6a.HeaderText = Column1a.HeaderText;
            //Column7a.HeaderText = Column1a.HeaderText;
            //Column8a.HeaderText = Column1a.HeaderText;

            Column2b.HeaderText = Column1b.HeaderText;
            Column3b.HeaderText = Column1b.HeaderText;
            Column4b.HeaderText = Column1b.HeaderText;
            //Column5b.HeaderText = Column1b.HeaderText;
            //Column6b.HeaderText = Column1b.HeaderText;
            //Column7b.HeaderText = Column1b.HeaderText;
            //Column8b.HeaderText = Column1b.HeaderText;

            Column2c.HeaderText = Column1c.HeaderText;
            Column3c.HeaderText = Column1c.HeaderText;
            Column4c.HeaderText = Column1c.HeaderText;
            //Column5c.HeaderText = Column1c.HeaderText;
            //Column6c.HeaderText = Column1c.HeaderText;
            //Column7c.HeaderText = Column1c.HeaderText;
            //Column8c.HeaderText = Column1c.HeaderText;

            Column2d.HeaderText = Column1d.HeaderText;
            Column3d.HeaderText = Column1d.HeaderText;
            Column4d.HeaderText = Column1d.HeaderText;
            //Column5d.HeaderText = Column1d.HeaderText;
            //Column6d.HeaderText = Column1d.HeaderText;
            //Column7d.HeaderText = Column1d.HeaderText;
            //Column8d.HeaderText = Column1d.HeaderText;

            Column2e.HeaderText = Column1e.HeaderText;
            Column3e.HeaderText = Column1e.HeaderText;
            Column4e.HeaderText = Column1e.HeaderText;
            //Column5e.HeaderText = Column1e.HeaderText;
            //Column6e.HeaderText = Column1e.HeaderText;
            //Column7e.HeaderText = Column1e.HeaderText;
            //Column8e.HeaderText = Column1e.HeaderText;
        }

        private void CheckConfig()
        {
            if (!File.Exists("Config.ini"))
            {
                FileStream f = File.Create("Config.ini");
                f.Close();
            }

            string[] config = File.ReadAllLines("Config.ini");

            for (int i = 0; i < config.Count(); i++)
            {
                if (config[i] == "Language: eng")
                    is_english = true;
                else if (config[i] == "Language: rus")
                    is_english = false;

                if(config[i].Length >= 15 && config[i].Substring(0, 15) == "Show DNS names:" && int.TryParse(config[i].Substring(16), out _))
                    if (int.Parse(config[i].Substring(16)) <= 0)
                    {
                        Column1b.Visible = false;
                        Column2b.Visible = false;
                        Column3b.Visible = false;
                        Column4b.Visible = false;
                        //Column5b.Visible = false;
                        //Column6b.Visible = false;
                        //Column7b.Visible = false;
                        //Column8b.Visible = false;
                    }
                    else
                    {
                        Column1b.Visible = true;
                        Column2b.Visible = true;
                        Column3b.Visible = true;
                        Column4b.Visible = true;
                        //Column5b.Visible = true;
                        //Column6b.Visible = true;
                        //Column7b.Visible = true;
                        //Column8b.Visible = true;
                    }

                if (config[i].Length >= 10 && config[i].Substring(0, 10) == "Show IP's:" && int.TryParse(config[i].Substring(11), out _))
                    if (int.Parse(config[i].Substring(11)) <= 0)
                    {
                        Column1c.Visible = false;
                        Column2c.Visible = false;
                        Column3c.Visible = false;
                        Column4c.Visible = false;
                        //Column5c.Visible = false;
                        //Column6c.Visible = false;
                        //Column7c.Visible = false;
                        //Column8c.Visible = false;
                    }
                    else
                    {
                        Column1c.Visible = true;
                        Column2c.Visible = true;
                        Column3c.Visible = true;
                        Column4c.Visible = true;
                        //Column5c.Visible = true;
                        //Column6c.Visible = true;
                        //Column7c.Visible = true;
                        //Column8c.Visible = true;
                    }

                if (config[i].Length >= 15 && config[i].Substring(0, 17) == "Show reply times:" && int.TryParse(config[i].Substring(18), out _))
                    if (Convert.ToInt32(config[i].Substring(18)) <= 0)
                    {
                        Column1d.Visible = false;
                        Column2d.Visible = false;
                        Column3d.Visible = false;
                        Column4d.Visible = false;
                        //Column5d.Visible = false;
                        //Column6d.Visible = false;
                        //Column7d.Visible = false;
                        //Column8d.Visible = false;
                    }
                    else
                    {
                        Column1d.Visible = true;
                        Column2d.Visible = true;
                        Column3d.Visible = true;
                        Column4d.Visible = true;
                        //Column5d.Visible = true;
                        //Column6d.Visible = true;
                        //Column7d.Visible = true;
                        //Column8d.Visible = true;
                    }



                if (config[i].Length >= 29 && config[i].Substring(0, 29) == "Group 1 autoping timer (min):" && int.TryParse(config[i].Substring(30), out _))
                    g_settings[0, 2] = int.Parse(config[i].Substring(30));

                if (config[i].Length >= 22 && config[i].Substring(0, 22) == "Group 1 timeout (sec):" && int.TryParse(config[i].Substring(23), out _))
                    g_settings[0, 3] = int.Parse(config[i].Substring(23));

                if (config[i].Length >= 22 && config[i].Substring(0, 22) == "Group 1 packets count:" && int.TryParse(config[i].Substring(23), out _))
                    g_settings[0, 4] = int.Parse(config[i].Substring(23));



                if (config[i].Length >= 29 && config[i].Substring(0, 29) == "Group 2 autoping timer (min):" && int.TryParse(config[i].Substring(30), out _))
                    g_settings[1, 2] = int.Parse(config[i].Substring(30));

                if (config[i].Length >= 22 && config[i].Substring(0, 22) == "Group 2 timeout (sec):" && int.TryParse(config[i].Substring(23), out _))
                    g_settings[1, 3] = int.Parse(config[i].Substring(23));

                if (config[i].Length >= 22 && config[i].Substring(0, 22) == "Group 2 packets count:" && int.TryParse(config[i].Substring(23), out _))
                    g_settings[1, 4] = int.Parse(config[i].Substring(23));



                if (config[i].Length >= 29 && config[i].Substring(0, 29) == "Group 3 autoping timer (min):" && int.TryParse(config[i].Substring(30), out _))
                    g_settings[2, 2] = int.Parse(config[i].Substring(30));

                if (config[i].Length >= 22 && config[i].Substring(0, 22) == "Group 3 timeout (sec):" && int.TryParse(config[i].Substring(23), out _))
                    g_settings[2, 3] = int.Parse(config[i].Substring(23));

                if (config[i].Length >= 22 && config[i].Substring(0, 22) == "Group 3 packets count:" && int.TryParse(config[i].Substring(23), out _))
                    g_settings[2, 4] = int.Parse(config[i].Substring(23));



                if (config[i].Length >= 29 && config[i].Substring(0, 29) == "Group 4 autoping timer (min):" && int.TryParse(config[i].Substring(30), out _))
                    g_settings[3, 2] = int.Parse(config[i].Substring(30));

                if (config[i].Length >= 22 && config[i].Substring(0, 22) == "Group 4 timeout (sec):" && int.TryParse(config[i].Substring(23), out _))
                    g_settings[3, 3] = int.Parse(config[i].Substring(23));

                if (config[i].Length >= 22 && config[i].Substring(0, 22) == "Group 4 packets count:" && int.TryParse(config[i].Substring(23), out _))
                    g_settings[3, 4] = int.Parse(config[i].Substring(23));



                if (config[i].Length >= 29 && config[i].Substring(0, 29) == "Group 5 autoping timer (min):" && int.TryParse(config[i].Substring(30), out _))
                    g_settings[4, 2] = int.Parse(config[i].Substring(30));

                if (config[i].Length >= 22 && config[i].Substring(0, 22) == "Group 5 timeout (sec):" && int.TryParse(config[i].Substring(23), out _))
                    g_settings[4, 3] = int.Parse(config[i].Substring(23));

                if (config[i].Length >= 22 && config[i].Substring(0, 22) == "Group 5 packets count:" && int.TryParse(config[i].Substring(23), out _))
                    g_settings[4, 4] = int.Parse(config[i].Substring(23));



                if (config[i].Length >= 29 && config[i].Substring(0, 29) == "Group 6 autoping timer (min):" && int.TryParse(config[i].Substring(30), out _))
                    g_settings[5, 2] = int.Parse(config[i].Substring(30));

                if (config[i].Length >= 22 && config[i].Substring(0, 22) == "Group 6 timeout (sec):" && int.TryParse(config[i].Substring(23), out _))
                    g_settings[5, 3] = int.Parse(config[i].Substring(23));

                if (config[i].Length >= 22 && config[i].Substring(0, 22) == "Group 6 packets count:" && int.TryParse(config[i].Substring(23), out _))
                    g_settings[5, 4] = int.Parse(config[i].Substring(23));



                if (config[i].Length >= 29 && config[i].Substring(0, 29) == "Group 7 autoping timer (min):" && int.TryParse(config[i].Substring(30), out _))
                    g_settings[6, 2] = int.Parse(config[i].Substring(30));

                if (config[i].Length >= 22 && config[i].Substring(0, 22) == "Group 7 timeout (sec):" && int.TryParse(config[i].Substring(23), out _))
                    g_settings[6, 3] = int.Parse(config[i].Substring(23));

                if (config[i].Length >= 22 && config[i].Substring(0, 22) == "Group 7 packets count:" && int.TryParse(config[i].Substring(23), out _))
                    g_settings[6, 4] = int.Parse(config[i].Substring(23));



                if (config[i].Length >= 29 && config[i].Substring(0, 29) == "Group 8 autoping timer (min):" && int.TryParse(config[i].Substring(30), out _))
                    g_settings[7, 2] = int.Parse(config[i].Substring(30));

                if (config[i].Length >= 22 && config[i].Substring(0, 22) == "Group 8 timeout (sec):" && int.TryParse(config[i].Substring(23), out _))
                    g_settings[7, 3] = int.Parse(config[i].Substring(23));

                if (config[i].Length >= 22 && config[i].Substring(0, 22) == "Group 8 packets count:" && int.TryParse(config[i].Substring(23), out _))
                    g_settings[7, 4] = int.Parse(config[i].Substring(23));
            }
        }
        
        private void CheckLog()
        {
            if (!File.Exists(date + " log.txt"))
            {
                FileStream f = File.Create(date + " log.txt");
                f.Close();
            }
        }

        //
        // Исправить
        //
        private void FillClientsList()
        {
            ab = new client[al.Count()];

            //g_lists[1, 0] = new string[322];
            //g_lists[1, 0][228] = "kek";
            //g_lists[1, 0].Count;



            if (g_settings[0, 0] >= 15)
                dataGridView1.Rows.Add(g_settings[0, 0]);
            else
                dataGridView1.Rows.Add(15);

            if (g_settings[1, 0] >= 15)
                dataGridView2.Rows.Add(g_settings[1, 0]);
            else
                dataGridView2.Rows.Add(15);

            if (g_settings[2, 0] >= 15)
                dataGridView3.Rows.Add(g_settings[2, 0]);
            else
                dataGridView3.Rows.Add(15);

            if (g_settings[3, 0] >= 15)
                dataGridView4.Rows.Add(g_settings[3, 0]);
            else
                dataGridView4.Rows.Add(15);

            //if (g_settings[4, 0] >= 15)
            //    dataGridView5.Rows.Add(g_settings[4, 0]);
            //else
            //    dataGridView5.Rows.Add(15);

            //if (g_settings[5, 0] >= 15)
            //    dataGridView6.Rows.Add(g_settings[5, 0]);
            //else
            //    dataGridView6.Rows.Add(15);

            //if (g_settings[6, 0] >= 15)
            //    dataGridView7.Rows.Add(g_settings[6, 0]);
            //else
            //    dataGridView7.Rows.Add(15);

            //if (g_settings[7, 0] >= 15)
            //    dataGridView8.Rows.Add(g_settings[7, 0]);
            //else
            //    dataGridView8.Rows.Add(15);

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

        //
        // исправить
        //
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
                    PingGroup(group, dataGridView1, button11, checkBox1, ping_g1, g_settings[group - 1, 4] * C_sec, waiter1);
                    break;
                case 2:
                    PingGroup(group, dataGridView2, button12, checkBox2, ping_g2, g_settings[group - 1, 4] * C_sec, waiter2);
                    break;
                case 3:
                    PingGroup(group, dataGridView3, button13, checkBox3, ping_g3, g_settings[group - 1, 4] * C_sec, waiter3);
                    break;
                case 4:
                    PingGroup(group, dataGridView4, button14, checkBox4, ping_g4, g_settings[group - 1, 4] * C_sec, waiter4);
                    break;
                case 5:
                    //PingGroup(group, dataGridView5, button15, checkBox5, ping_g5, g_settings[group - 1, 4] * C_sec, waiter5);
                    break;
                case 6:
                    //PingGroup(group, dataGridView6, button16, checkBox6, ping_g6, g_settings[group - 1, 4] * C_sec, waiter6);
                    break;
                case 7:
                    //PingGroup(group, dataGridView7, button17, checkBox7, ping_g7, g_settings[group - 1, 4] * C_sec, waiter7);
                    break;
                case 8:
                    //PingGroup(group, dataGridView8, button18, checkBox8, ping_g8, g_settings[group - 1, 4] * C_sec, waiter8);
                    break;
            }
        }

        private void PingGroup(int current_group, DataGridView grid, Button button, CheckBox check, Ping ping, int timeout, AutoResetEvent waiter)
        {
            current_group--;
            toolStripButton2.Enabled = false;

            if (g_settings[current_group, 1] < g_settings[current_group, 0])
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    if(grid[0, 0].Value.ToString() == "")
                    {
                        button.Enabled = false;
                        check.Enabled = false;

                        grid[0, g_settings[current_group, 1]].Selected = true;
                        grid[3, g_settings[current_group, 1]].Value = pinging;
                        grid[3, g_settings[current_group, 1]].Style.BackColor = Color.Cyan;
                        grid[3, g_settings[current_group, 1]].Style.SelectionForeColor = Color.White;
                        grid[3, g_settings[current_group, 1]].Style.SelectionBackColor = Color.DarkCyan;

                        PingCl(ping, grid[2, g_settings[current_group, 1]].Value.ToString(), timeout, waiter);
                    }
                    else
                        MessageBox.Show("Список клиентов пуст");
                }
                else
                    MessageBox.Show(check_connection);
            }
            else
            {
                CheckLog();
            }
        }

        private static void PingCl(Ping ping, string address, int timeout, AutoResetEvent waiter)
        {
            ping.SendAsync(address, timeout, waiter);
        }

        private void SortReply(int group)
        {
            CheckLog();
            toolStripButton2.Enabled = true;

            switch (group)
            {
                case 1:
                    DisplayReply(group, dataGridView1, reply_g1, button11, checkBox1);
                    break;
                case 2:
                    DisplayReply(group, dataGridView2, reply_g2, button12, checkBox2);
                    break;
                case 3:
                    DisplayReply(group, dataGridView3, reply_g3, button13, checkBox3);
                    break;
                case 4:
                    DisplayReply(group, dataGridView4, reply_g4, button14, checkBox4);
                    break;
                case 5:
                    //DisplayReply(group, dataGridView5, reply_g5, button15, checkBox5);
                    break;
                case 6:
                    //DisplayReply(group, dataGridView6, reply_g6, button16, checkBox6);
                    break;
                case 7:
                    //DisplayReply(group, dataGridView7, reply_g7, button17, checkBox7);
                    break;
                case 8:
                    //DisplayReply(group, dataGridView8, reply_g8, button18, checkBox8);
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
        private void DisplayReply(int current_group, DataGridView grid, PingReply reply, Button button, CheckBox check)
        {
            current_group--;

            grid[3, g_settings[current_group, 1]].Value = reply.Status;

            string c = "Опрос " + dataGridView1[0, g_settings[current_group, 1]].Value + " " + dataGridView1[1, g_settings[current_group, 1]].Value + " " + dataGridView1[2, g_settings[current_group, 1]].Value + Environment.NewLine;

            File.AppendAllText(date + " log.txt", c);

            c = DateTime.Now.ToString().Substring(11) + "." + DateTime.Now.Millisecond.ToString();
            c += " " + reply_g1.Status;

            if (reply_g1.Status == IPStatus.Success)
            {
                grid[3, g_settings[current_group, 1]].Style.BackColor = Color.GreenYellow;
                grid[3, g_settings[current_group, 1]].Style.SelectionBackColor = Color.DarkGreen;
                c += " " + reply_g1.RoundtripTime + " ms";

                //IPAddressInformation iP;
                //long huh = 228;
                //IPAddress kek = new IPAddress(huh);
            }
            else
            {
                grid[3, g_settings[current_group, 1]].Style.BackColor = Color.Red;
                grid[3, g_settings[current_group, 1]].Style.SelectionBackColor = Color.DarkRed;
            }

            File.AppendAllText(date + " log.txt", c + Environment.NewLine);

            if (g_settings[current_group, 1] == g_settings[current_group, 4])
            {
                g_settings[current_group, 1]++;
                g_settings[current_group, 1] = 0;
            }
            else
                g_settings[current_group, 1]++;

            if (g_settings[current_group, 1] < g_settings[current_group, 0])
                SortPing(current_group + 1);
            else
            {
                g_settings[current_group, 1] = 0;
            }
            button.Enabled = true;
            check.Enabled = true;
        }

        private void CheckChange(DataGridView grid, System.Windows.Forms.Timer timer, CheckBox check, int currrent_group, int count)
        {
            if (timer.Enabled)
                timer.Stop();

            if (check.Checked)
            {
                timer.Interval = g_settings[currrent_group - 1, 2] * C_min;
                timer.Start();

                for (int i = 0; i < 8; i++)
                {
                    g_settings[i, 1] = 0;
                }

                ClearGrid(grid, count);
                SortPing(currrent_group);
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

        //
        // дописать
        //
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

            button11.Location = new Point(groupBox1.Size.Width - 105, 60);
            button12.Location = new Point(groupBox2.Size.Width - 105, 60);
            button13.Location = new Point(groupBox3.Size.Width - 105, 60);
            button14.Location = new Point(groupBox4.Size.Width - 105, 60);

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
            PreProcessing();

            Size = new Size(916, 739);
            FormResized();

            //if (NetworkInterface.GetIsNetworkAvailable())
            //{
                //SortPing(1);
                //SortPing(2);
                //SortPing(3);
                //SortPing(4);
            //}
            //else
            //    MessageBox.Show(check_connection);

            Application.DoEvents();
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

        private void timer5_Tick(object sender, EventArgs e)
        {
            //ClearGrid(dataGridView5, g_settings[4, 0]);
            SortPing(5);
        }

        private void timer6_Tick(object sender, EventArgs e)
        {
            //ClearGrid(dataGridView6, g_settings[5, 0]);
            SortPing(6);
        }

        private void timer7_Tick(object sender, EventArgs e)
        {
            //ClearGrid(dataGridView7, g_settings[6, 0]);
            SortPing(7);
        }

        private void timer8_Tick(object sender, EventArgs e)
        {
            //ClearGrid(dataGridView8, g_settings[7, 0]);
            SortPing(8);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CheckChange(dataGridView1, Timer1, checkBox1, 1, g_settings[0, 0]);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            CheckChange(dataGridView2, Timer2, checkBox2, 2, g_settings[1, 0]);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            CheckChange(dataGridView3, Timer3, checkBox3, 3, g_settings[2, 0]);
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            CheckChange(dataGridView4, Timer4, checkBox4, 4, g_settings[3, 0]);
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            //CheckChange(dataGridView5, Timer5, checkBox5, 5, g_settings[4, 0]);
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            //CheckChange(dataGridView6, Timer6, checkBox6, 6, g_settings[5, 0]);
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            //CheckChange(dataGridView7, Timer7, checkBox7, 7, g_settings[6, 0]);
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            //CheckChange(dataGridView8, Timer8, checkBox8, 8, g_settings[7, 0]);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (Timer1.Enabled)
                Timer1.Stop();

            ClearGrid(dataGridView1, g_settings[0, 0]);
            SortPing(1);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (Timer2.Enabled)
                Timer2.Stop();

            ClearGrid(dataGridView2, g_settings[1, 0]);
            SortPing(2);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (Timer3.Enabled)
                Timer3.Stop();

            ClearGrid(dataGridView3, g_settings[2, 0]);
            SortPing(3);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (Timer4.Enabled)
                Timer4.Stop();

            ClearGrid(dataGridView4, g_settings[3, 0]);
            SortPing(4);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (Timer5.Enabled)
                Timer5.Stop();

            //ClearGrid(dataGridView5, g_settings[4, 0]);
            SortPing(5);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (Timer6.Enabled)
                Timer6.Stop();

            //ClearGrid(dataGridView6, g_settings[5, 0]);
            SortPing(6);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            if (Timer7.Enabled)
                Timer7.Stop();

            //ClearGrid(dataGridView7, g_settings[6, 0]);
            SortPing(7);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (Timer8.Enabled)
                Timer8.Stop();

            //ClearGrid(dataGridView8, g_settings[7, 0]);
            SortPing(8);
        }

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int selected_row = dataGridView1.SelectedCells[0].RowIndex;
            string s_name = dataGridView1[0, selected_row].Value.ToString();
            string s_dns = dataGridView1[1, selected_row].Value.ToString();
            string s_ip = dataGridView1[2, selected_row].Value.ToString();

            if (!tracking)
            {
                tr = new Tracking(is_english, s_name, s_dns, s_ip);
                tr.FormClosed += new FormClosedEventHandler(Tracking_Closed);
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
                tr = new Tracking(is_english, s_name, s_dns, s_ip);
                tr.FormClosed += new FormClosedEventHandler(Tracking_Closed);
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
                tr = new Tracking(is_english, s_name, s_dns, s_ip);
                tr.FormClosed += new FormClosedEventHandler(Tracking_Closed);
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
                tr = new Tracking(is_english, s_name, s_dns, s_ip);
                tr.FormClosed += new FormClosedEventHandler(Tracking_Closed);
                toolStripButton3.Enabled = false;
                tracking = true;
                tr.Show();
            }
        }

        private void Open_iniTSMitem_Click(object sender, EventArgs e)
        {
            //if (File.Exists("D:\\Амир\\Уст. программы\\Notepad++\\notepad++.exe"))
                //Process.Start("D:\\Амир\\Уст. программы\\Notepad++\\notepad++.exe", "Config.ini");
            //else
                Process.Start("C:\\Windows\\System32\\notepad.exe", "Config.ini");
        }

        private void Open_logTSMitem_Click(object sender, EventArgs e)
        {
            //if (File.Exists("D:\\Амир\\Уст. программы\\Notepad++\\notepad++.exe") && File.Exists("Log" + ".txt"))
                //Process.Start("D:\\Амир\\Уст. программы\\Notepad++\\notepad++.exe", "Log" + ".txt");
            //else
                Process.Start("C:\\Windows\\System32\\notepad.exe", date + " log.txt");
        }

        private void Open_clientsTSMitem_Click(object sender, EventArgs e)
        {
            //if (File.Exists("D:\\Амир\\Уст. программы\\Notepad++\\notepad++.exe") && File.Exists("Clients.txt"))
                //Process.Start("D:\\Амир\\Уст. программы\\Notepad++\\notepad++.exe", "Clients.txt");
            //else
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
            tr = new Tracking(is_english, "", "", "");
            tr.FormClosed += new FormClosedEventHandler(Tracking_Closed);
            toolStripButton3.Enabled = false;
            tracking = true;
            tr.Show();
        }

        private void Tracking_Closed(object sender, FormClosedEventArgs e)
        {
            toolStripButton3.Enabled = true;
            tracking = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            settings = new Settings(is_english, 0, 3, 60, 1);
            settings.FormClosed += new FormClosedEventHandler(Settings_Closed);
            settings.Show();
        }

        private void Settings_Closed(object sender, FormClosedEventArgs e)
        {
            g_settings[settings.Group, 2] = settings.Period;
            g_settings[settings.Group, 3] = settings.Timeout;
            g_settings[settings.Group, 4] = settings.Packets;
        }

        private void Lang_rusTSMitem_Click(object sender, EventArgs e)
        {
            is_english = false;
            Translate();
            CopyText();
        }

        private void Lang_engTSMitem_Click(object sender, EventArgs e)
        {
            is_english = true;
            Translate();
            CopyText();
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
        
        private void Received_reply_g1(object sender, PingCompletedEventArgs e)
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
        
        private void Received_reply_g2(object sender, PingCompletedEventArgs e)
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
        
        private void Received_reply_g3(object sender, PingCompletedEventArgs e)
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
        
        private void Received_reply_g4(object sender, PingCompletedEventArgs e)
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
        
        private void Received_reply_g5(object sender, PingCompletedEventArgs e)
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
        
        private void Received_reply_g6(object sender, PingCompletedEventArgs e)
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
        
        private void Received_reply_g7(object sender, PingCompletedEventArgs e)
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
        
        private void Received_reply_g8(object sender, PingCompletedEventArgs e)
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            File.AppendAllText(date + " log.txt", "Программа закрыта " + date + " в " + DateTime.Now.ToString().Substring(11) + "." + DateTime.Now.Millisecond.ToString() + Environment.NewLine);
            File.AppendAllText(date + " log.txt", Environment.NewLine);
        }
        #endregion События
    }
}
