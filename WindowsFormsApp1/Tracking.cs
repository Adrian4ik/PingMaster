using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Tracking : Form
    {
        bool is_eng, to_ping = false, to_clear = false;
        int cur_row = 0;
        string received_name, received_dns, received_ip;

        DataGridViewTextBoxColumn Col0 = new DataGridViewTextBoxColumn();
        DataGridViewTextBoxColumn Col1 = new DataGridViewTextBoxColumn();
        DataGridViewTextBoxColumn Col2 = new DataGridViewTextBoxColumn();
        DataGridViewTextBoxColumn Col3 = new DataGridViewTextBoxColumn();

        System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
        System.Net.NetworkInformation.PingReply reply;
        static AutoResetEvent waiter = new AutoResetEvent(false);

        public Tracking(bool loc_eng, string name, string dns, string ip)
        {
            received_name = name;
            received_dns = dns;
            received_ip = ip;
            is_eng = loc_eng;

            InitializeComponent();
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

            Col0.ReadOnly = true;

            Col1.ReadOnly = true;
            Col1.Width = 60;

            Col2.ReadOnly = true;
            Col2.Width = 195;

            Col3.ReadOnly = true;
            Col3.Width = 25;

            dataGridView1.Columns.Add(Col0);
            dataGridView1.Columns.Add(Col1);
            dataGridView1.Columns.Add(Col2);
            dataGridView1.Columns.Add(Col3);
        }

        private void Translate()
        {
            if (is_eng)
            {
                Text = "Tracking";
                label1.Text = "Name/DNS";
                label2.Text = "Type ip that will ping:";
                Col0.HeaderText = "Reply time";
                Col1.HeaderText = "Reply";
                Col2.HeaderText = "Status";
                button1.Text = "Start";
            }
            else
            {
                Text = "Слежение";
                label1.Text = "Имя/DNS";
                label2.Text = "Введите пингуемый ip адрес:";
                Col0.HeaderText = "Время ответа";
                Col1.HeaderText = "Ответ";
                Col2.HeaderText = "Статус";
                button1.Text = "Старт";
            }

            Col3.HeaderText = "";
        }

        private void Ping_cl()
        {
            if (textBox1.Text != "" && textBox1.Text != "0.0.0.0")
            {
                dataGridView1.Rows.Add();
                ping.SendAsync(textBox1.Text, waiter);
            }
        }

        private void Message_error()
        {
            if (is_eng)
                MessageBox.Show("Input correct ip address");
            else
                MessageBox.Show("Введите правильный ip адрес");
        }

        private void Tracking_FormClosing(object sender, FormClosingEventArgs e)
        {
            /*if (closef > 0)
            {
                e.Cancel = false;
                this.Close();
                //Tracking_FormClosed(this, Empty);
            }
            else
            {*/
                e.Cancel = true;

                to_clear = true;

                ping.SendAsyncCancel();

                FormClosing -= new FormClosingEventHandler(Tracking_FormClosing);
                Close();
            //}

            /*dataGridView1.Columns.Add(Col0);
            dataGridView1.Columns.Add(Col1);
            dataGridView1.Columns.Add(Col2);
            dataGridView1.Columns.Add(Col3);*/
        }

        private void Display_reply()
        {
            if (to_clear)
            {
                cur_row = 0;
                dataGridView1.Rows.Clear();
            }
            else
            {
                dataGridView1[0, cur_row].Value = DateTime.Now.ToString().Substring(11) + "." + DateTime.Now.Millisecond.ToString();

                if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                {
                    if (reply.RoundtripTime > 999)
                    {
                        if (reply.RoundtripTime.ToString().Substring(1)[0] == '0' && reply.RoundtripTime.ToString().Substring(1)[1] == '0')
                            dataGridView1[1, cur_row].Value = reply.RoundtripTime.ToString().Substring(0, 1) + " s " + reply.RoundtripTime.ToString().Substring(3) + " ms";
                        else if (reply.RoundtripTime.ToString().Substring(1)[0] == '0')
                            dataGridView1[1, cur_row].Value = reply.RoundtripTime.ToString().Substring(0, 1) + " s " + reply.RoundtripTime.ToString().Substring(2) + " ms";
                        else
                            dataGridView1[1, cur_row].Value = reply.RoundtripTime.ToString().Substring(0, 1) + " s " + reply.RoundtripTime.ToString().Substring(1) + " ms";
                    }
                    else if (reply.RoundtripTime == 0)
                        dataGridView1[1, cur_row].Value = "<1 ms";
                    else
                        dataGridView1[1, cur_row].Value = reply.RoundtripTime.ToString() + " ms";
                    dataGridView1[3, cur_row].Style.BackColor = Color.GreenYellow;
                }
                else
                {
                    dataGridView1[1, cur_row].Value = "---";
                    dataGridView1[2, cur_row].Value = reply.Status;
                    dataGridView1[3, cur_row].Style.BackColor = Color.Red;
                }

                if(cur_row == 0)
                    dataGridView1[0, cur_row].Selected = true;
                else
                    dataGridView1[0, cur_row - 1].Selected = true;

                cur_row++;

                if (to_ping)
                    Ping_cl();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ping.SendAsyncCancel();

            switch (is_eng)
            {
                case true:
                    button1.Text = "Start";
                    break;
                case false:
                    button1.Text = "Старт";
                    break;
            }

            to_clear = true;

            if (!to_ping)
            {
                cur_row = 0;
                dataGridView1.Rows.Clear();
            }

            if (textBox1.Text == "127.0.0.1")
                label1.Text = "Loopback";
            else if (textBox1.Text == "8.8.8.8")
                label1.Text = "Google";
            else if (textBox1.Text == "4.2.2.2" || textBox1.Text == "77.88.21.11" || textBox1.Text == "5.255.255.50")
                label1.Text = "Яндекс";
            else
                label1.Text = textBox1.Text;
        }

        private void Tracking_Load(object sender, EventArgs e)
        {
            Preprocessing();
            Translate();

            ping.PingCompleted += new System.Net.NetworkInformation.PingCompletedEventHandler(Received_reply);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(button1.Text == "Старт" || button1.Text == "Start")
            {
                int num = 0;

                if (textBox1.TextLength < 7)
                {
                    if (is_eng)
                        MessageBox.Show("Input correct ip address");
                    else
                        MessageBox.Show("Введите правильный ip адрес");
                }
                else
                {
                    bool norm = true;
                    for (int i = 0, j = 0, dot = 0; i < textBox1.Text.Length; i++)
                    {
                        if (textBox1.Text[i] == '0' || textBox1.Text[i] == '1' || textBox1.Text[i] == '2' || textBox1.Text[i] == '3' || textBox1.Text[i] == '4' || textBox1.Text[i] == '5' || textBox1.Text[i] == '6' || textBox1.Text[i] == '7' || textBox1.Text[i] == '8' || textBox1.Text[i] == '9')
                        {
                            if (j == 0)
                                num = textBox1.Text[i];
                            else if (j > 0 && j < 3)
                                num = num * 10 + textBox1.Text[i];
                            else
                            {
                                norm = false;
                                Message_error();
                            }

                            j++;
                        }
                        else if (textBox1.Text[i] == '.' && dot < 3 && (num >= 0 || num < 255))
                        {
                            if (j > 3)
                            {
                                norm = false;
                                Message_error();
                            }

                            num = 0;
                            j = 0;
                            dot++;
                        }
                        else
                        {
                            norm = false;
                            Message_error();
                        }
                    }

                    if(norm)
                    {
                        switch (is_eng)
                        {
                            case true:
                                button1.Text = "Stop";
                                break;
                            case false:
                                button1.Text = "Стоп";
                                break;
                        }

                        to_ping = true;
                        to_clear = false;
                        Ping_cl();
                    }
                }
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

                to_ping = false;
            }
        }

        private void Received_reply(object sender, System.Net.NetworkInformation.PingCompletedEventArgs e)
        {
            if (e.Cancelled)
                ((AutoResetEvent)e.UserState).Set();

            if (e.Error != null)
                ((AutoResetEvent)e.UserState).Set();

            // Let the main thread resume.
            ((AutoResetEvent)e.UserState).Set();

            reply = e.Reply;

            if(!to_clear && to_ping)
                Display_reply();
        }
    }
}
