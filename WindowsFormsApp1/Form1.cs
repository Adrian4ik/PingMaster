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
        private string abonent;

        private const int sec = 1000, min = 60000;

        private string[] abonentslist;

        /*private const string[] StandartList = new string[27] {"127.0.0.1", "10.1.1.254", "10.1.2.254", "192.168.60.254", "192.168.68.73", "10.1.2.250", "",
            "192.168.67.25", "192.168.60.253", "192.168.60.51", "192.168.60.53", "192.168.60.82", "",
            "10.1.1.3", "10.1.1.4", "10.1.1.5", "10.1.2.1", "10.1.1.1", "192.168.60.81", "10.1.1.80", "10.1.1.7", "",
            "10.1.1.100", "10.1.3.4", "10.1.3.1", "192.168.249.1", "10.1.11.5"};*/

        #region Методы
        public Form1()
        {
            InitializeComponent();
        }

        private void SomeMethod()
        {
            /*if (textBox1.Text != "")
            {
                System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                System.Net.NetworkInformation.PingReply pingReply = ping.Send(textBox1.Text);

                label2.Text = pingReply.RoundtripTime.ToString() + " ms";
                label3.Text = pingReply.Status.ToString();
            }*/
            //Count();
        }

        /*public static void Count(Object obj)
        {
            //int x = (int)obj;
            //for(int i = 1; i < 9; i++, x++)
            //{ }
            SomeMethod();
        }*/

        private void PreProcessing()
        {
            if (File.Exists("Abonents.txt"))
            {
                //int oldw = int.Parse(File.ReadAllText("OldW.txt"));
                abonentslist = File.ReadAllLines("Abonents.txt");
            }
            //else
                //File.WriteAllLines("Abonents.txt", StandartList); // .ToString()

            /*this.dataGridView1.Rows.Add("Laptop 1", "192.168.0.1", "Online");
            this.dataGridView1.Rows.Add("Laptop 2", "192.168.51.86", "Online");
            this.dataGridView1.Rows.Add("Laptop 3", "127.0.0.1", "Offline");
            this.dataGridView1.Rows.Add("Laptop 4", "Some ip", "Offline");
            this.dataGridView1.Rows.Add("Laptop 5", "Some ip", "Online");
            this.dataGridView1.Rows.Add("Laptop 6", abonent, "Online");*/

            t0 = 3 * sec;
            t1a = 3 * sec;
            t1b = 1 * min;
            t2a = 3 * sec;
            t2b = 1 * min;
            t3a = 3 * sec;
            t3b = 1 * min;
            t4a = 3 * sec;
            t4b = 1 * min;
        }

        private void FormResized()
        {
            /*if (ClientSize.Height - 60 < label1.Location.X - 44)
            {
                pictureBox1.Size = new Size(ClientSize.Height / 2 - 60, ClientSize.Height / 2 - 60);
                pictureBox2.Size = new Size(ClientSize.Height / 2 - 60, ClientSize.Height / 2 - 60);
                pictureBox3.Size = new Size(ClientSize.Height / 2 - 60, ClientSize.Height / 2 - 60);
                pictureBox4.Size = new Size(ClientSize.Height / 2 - 60, ClientSize.Height / 2 - 60);
            }
            else
            {
                pictureBox1.Size = new Size(label1.Location.X / 2 - 44, label1.Location.X / 2 - 44);
                pictureBox2.Size = new Size(label1.Location.X / 2 - 44, label1.Location.X / 2 - 44);
                pictureBox3.Size = new Size(label1.Location.X / 2 - 44, label1.Location.X / 2 - 44);
                pictureBox4.Size = new Size(label1.Location.X / 2 - 44, label1.Location.X / 2 - 44);
            }

            pictureBox5.Size = new Size(1, 1);
            pictureBox2.Location = new Point(pictureBox1.Size.Width + 32, 28);
            pictureBox3.Location = new Point(12, pictureBox1.Size.Height + 48);
            pictureBox4.Location = new Point(pictureBox1.Size.Width + 32, pictureBox1.Size.Height + 48);
            pictureBox5.Location = new Point(pictureBox1.Size.Width * 2 + 52, pictureBox4.Location.Y);*/
        }
        #endregion Методы

        #region События
        private void Form1_Load(object sender, EventArgs e)
        {
            PreProcessing();

            Timer0.Start();
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
            Timer4b.Interval = 1;
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

        private void button1_Click(object sender, EventArgs e)
        {
            //SomeMethod();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form_ChangedSize(object sender, EventArgs e)
        {
            FormResized();
        }
        #endregion События
    }
}
