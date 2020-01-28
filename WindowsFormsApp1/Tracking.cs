using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Tracking : Form
    {
        bool is_eng;
        string received_name, received_dns, received_ip;
        int cur_row = 0;

        System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();

        public Tracking(bool loc_eng, string name, string dns, string ip)
        {
            received_name = name;
            received_dns = dns;
            received_ip = ip;
            is_eng = loc_eng;

            InitializeComponent();
        }
        
        private void Translate()
        {
            if (is_eng)
            {
                Text = "Tracking";
                label1.Text = "Name/DNS";
                label2.Text = "Type ip that will pinging:";
                Col0.HeaderText = "Time";
                Col1.HeaderText = "Reply";
                Col2.HeaderText = "Status";
                button1.Text = "Start";
            }
            else
            {
                Text = "Слежение";
                label1.Text = "Имя/DNS";
                label2.Text = "Введите пингуемый ip адрес:";
                Col0.HeaderText = "Время";
                Col1.HeaderText = "Ответ";
                Col2.HeaderText = "Статус";
                button1.Text = "Старт";
            }
        }

        private void Preprocessing()
        {
            if (received_ip != "")
            {
                textBox1.Text = received_ip;
                label1.Text = received_name + " " + received_dns;
            }
            else
            {
                textBox1.Text = "127.0.0.1";
                label1.Text = "Loopback";
            }

            timer1.Interval = 1;
        }
        private void Ping_cl()
        {
            dataGridView1.Rows.Add();

            if (textBox1.Text != "" && textBox1.Text != "0.0.0.0")
            {
                System.Net.NetworkInformation.PingReply pingReply = ping.Send(textBox1.Text);

                dataGridView1[0, cur_row].Value = DateTime.Now.ToString().Substring(11) + "." + DateTime.Now.Millisecond.ToString();

                dataGridView1[2, cur_row].Value = pingReply.Status.ToString();

                if (pingReply.Status == System.Net.NetworkInformation.IPStatus.Success)
                {
                    dataGridView1[1, cur_row].Value = pingReply.RoundtripTime.ToString() + " ms";
                    dataGridView1[3, cur_row].Style.BackColor = Color.GreenYellow;
                }
                else
                {
                    dataGridView1[1, cur_row].Value = "---";
                    dataGridView1[3, cur_row].Style.BackColor = Color.Red;
                }

                if(cur_row < 20)
                    dataGridView1.FirstDisplayedScrollingRowIndex = 0;
                else
                    dataGridView1.FirstDisplayedScrollingRowIndex = cur_row - 19;

                cur_row++;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label1.Text = textBox1.Text;

            if (timer1.Enabled)
            {
                switch (is_eng)
                {
                    case true:
                        button1.Text = "Start";
                        break;
                    case false:
                        button1.Text = "Старт";
                        break;
                }
                timer1.Stop();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Ping_cl();
        }

        private void Tracking_Load(object sender, EventArgs e)
        {
            Translate();
            Preprocessing();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(button1.Text == "Старт" || button1.Text == "Start")
            {
                switch(is_eng)
                {
                    case true:
                        button1.Text = "Stop";
                        break;
                    case false:
                        button1.Text = "Стоп";
                        break;
                }
                timer1.Start();
            }
            else
            {
                switch (is_eng)
                {
                    case true:
                        button1.Text = "Start";
                        break;
                    case false:
                        button1.Text = "Старт";
                        break;
                }
                timer1.Stop();
            }
        }
    }
}
