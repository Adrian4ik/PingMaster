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

        private struct abonent
        {
            public int Group;

            public string Name;
            public string DNS;
            public string IP;
        }

        private abonent[] ab = new abonent[100];

        private struct reply
        {

        }

        #region Константы
        private const int С_sec = 1000, С_min = 60000;

        private string[] StandartList = new string[]
            {"Loopback//DNS name//127.0.0.1", "БРИ-1//DNS name//10.1.1.254", "БРИ-2//DNS name//10.1.2.254", "БРИ-3//DNS name//192.168.60.254", "Что-то//DNS name//192.168.68.73", "АСП//DNS name//10.1.2.250", "",
            "Ещё что-то// //192.168.67.25", "USL ER-SWA1,2// //192.168.60.253", "ISS Server 1// //192.168.60.51", "LS1// //192.168.60.53", "Lab printer// //192.168.60.82", "",
            "RSE1// //10.1.1.3", "RSK1// //10.1.1.4", "RSK2// //10.1.1.5", "RSS1// //10.1.2.1", "RSS2// //10.1.1.1", "SM Printer// //192.168.60.81", "Mediaserver AGAT// //10.1.1.80", "RSE-Med// //10.1.1.7", "",
            "FS1// //10.1.1.100", "БСПН (PLS)// //10.1.3.4", "ТВМ1-Н// //10.1.3.1", "БПИ-НЧ (TRPU)// //192.168.249.1", "БЗУ// //10.1.11.5"};
        #endregion Константы

        #region Методы
        public Form1()
        {
            InitializeComponent();
        }

        private string[] PreProcessing()
        {
            string[] al = new string[100];

            if (File.Exists("Abonents.txt"))
                al = File.ReadAllLines("Abonents.txt");
            else
            {
                File.Create("Abonents.txt");
                File.WriteAllLines("Abonents.txt", StandartList);
                al = StandartList;
            }

            t0 = 3 * С_sec;
            t1a = 3 * С_sec;
            t1b = 1 * С_min;
            t2a = 3 * С_sec;
            t2b = 1 * С_min;
            t3a = 3 * С_sec;
            t3b = 1 * С_min;
            t4a = 3 * С_sec;
            t4b = 1 * С_min;

            return al;
        }

        private void FillGrids()
        {
            //int s = 0; // Счётчик строк
            /*for(int s = 0; s < 50; s++)
            {
                int flag = 0;
                while (abonentslist[s] != "")
                {
                    string str = abonentslist[s];
                    int start_id = 0;

                    int c = 0; // Счётчик символов
                    while (str[c].ToString() != "//")
                    {
                        c++;
                    }

                    switch (flag)
                    {
                        case 0:
                            ab[c].Name = str.Substring(start_id, c);
                            break;
                        case 1:
                            ab[c].DNS = str;
                            break;
                        case 2:
                            ab[c].IP = str;
                            break;
                    }
                    flag++;
                }
                s++;
            }*/

            //ab.Name = "Kek";

            dataGridView1.Rows.Add("", "Laptop 1", "", "192.168.0.1", "Online");
            dataGridView1.Rows.Add("", "Laptop 2", "", "192.168.51.86", "Online");
            dataGridView1.Rows.Add("", "Laptop 3", "", "127.0.0.1", "Offline");
            dataGridView1.Rows.Add("", "Laptop 4", "", "Some ip", "Offline");
            dataGridView1.Rows.Add("", "Laptop 5", "", "Some ip", "Online");
            //dataGridView1.Rows.Add("Laptop 6", ab.IP, "Online");
            dataGridView1.Rows.Add("", "", "");

            dataGridView1[5, 0].Style.BackColor = Color.GreenYellow;
            dataGridView1[5, 1].Style.BackColor = Color.GreenYellow;
            dataGridView1[5, 2].Style.BackColor = Color.Red;
            dataGridView1[5, 3].Style.BackColor = Color.Red;
            dataGridView1[5, 4].Style.BackColor = Color.Red;
            dataGridView1[5, 5].Style.BackColor = Color.Cyan;

            //dataGridView1[1, 0].Value = ab[0].Name;
        }
        private reply Ping_cl(abonent a)
        {
            reply r;
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

            //label1.Text = "Form size: " + ClientSize.Width + "x" + ClientSize.Height + " gb size: " + groupBox1.Width + "x" + groupBox1.Height + " dgv size: " + dataGridView1.Width + "x" + dataGridView1.Height;
            //dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        }
        #endregion Методы

        #region События
        private void Form1_Load(object sender, EventArgs e)
        {
            Column1a.Visible = false;
            Column1c.Visible = false;
            Column2a.Visible = false;
            Column2c.Visible = false;
            Column3a.Visible = false;
            Column3c.Visible = false;
            Column4a.Visible = false;
            Column4c.Visible = false;

            string[] abonentslist = new string[100];

            FormResized();
            abonentslist = PreProcessing();
            FillGrids();

            /*Timer0.Start();
            Timer1a.Start();
            Timer1b.Start();
            Timer2a.Start();
            Timer2b.Start();
            Timer3a.Start();
            Timer3b.Start();
            Timer4a.Start();
            Timer4b.Start();

            Timer0.Interval = 1;
            Timer1a.Interval = 1;
            Timer1b.Interval = 1;
            Timer2a.Interval = 1;
            Timer2b.Interval = 1;
            Timer3a.Interval = 1;
            Timer3b.Interval = 1;
            Timer4a.Interval = 1;
            Timer4b.Interval = 1;*/
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

        private void numericUpDown9_ValueChanged(object sender, EventArgs e)
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

        private void button1_Click(object sender, EventArgs e)
        {
            //SomeMethod();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            label1.Text = e.RowIndex.ToString();
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //label1.Text = e.RowIndex.ToString();
        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //label1.Text = e.RowIndex.ToString();
        }

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
