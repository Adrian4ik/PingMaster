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
        string received_name, received_dns, received_ip;

        public Tracking(string name, string dns, string ip)
        {
            label1.Text = name + " " + dns + " " + ip;

            InitializeComponent();
        }

        private void Tracking_Load(object sender, EventArgs e)
        {
            this.dataGridView1.Rows.Add("11:56:21:488", "<1 ms", "Host may be sometimes unreachable");
            dataGridView1.Rows.Add();
            dataGridView1.Rows.Add();
            dataGridView1.Rows.Add();
            dataGridView1.Rows.Add();

            dataGridView1[0, 0].Value = "11:56:21:488 22.08.2019";
            dataGridView1[0, 0].Value = "<1 ms";
            dataGridView1[0, 0].Value = "Host may be sometimes unreachable";
        }
    }
}
