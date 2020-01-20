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
        string abonent;
        //static TimerCallback tm = new TimerCallback(Count);
        //System.Threading.Timer tim = new System.Threading.Timer(tm, 0, 0, 1);

        // Методы
        public Form1()
        {
            InitializeComponent();
        }

        public void SomeMethod()
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

        // События
        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists("Abonents.txt"))
            {
                //int oldw = int.Parse(File.ReadAllText("OldW.txt"));
                abonent = File.ReadAllText("Abonents.txt");
            }
            else
                File.WriteAllText("Abonents.txt", "Some ip"); // .ToString()

            dataGridView1.Rows.Add("Laptop 1", "192.168.0.1", "Online");
            dataGridView1.Rows.Add("Laptop 2", "192.168.51.86", "Online");
            dataGridView1.Rows.Add("Laptop 3", "127.0.0.1", "Offline");
            dataGridView1.Rows.Add("Laptop 4", "Some ip", "Offline");
            dataGridView1.Rows.Add("Laptop 5", "Some ip", "Online");
            dataGridView1.Rows.Add("Laptop 6", abonent, "Online");
        }

        private void timer1_Tick(object sender, EventArgs e)
        { SomeMethod(); }

        private void button1_Click(object sender, EventArgs e)
        {
            //SomeMethod();

            Timer1.Start();
            Timer2.Start();
            Timer1.Interval = 1;
            Timer2.Interval = 1;
        }
    }
}
