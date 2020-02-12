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
    public partial class Settings : Form
    {
        bool is_eng;

        public bool DNS { get; private set; }
        public bool IP { get; private set; }
        public bool RT { get; private set; }

        public int Group { get; private set; }
        public int Period { get; private set; }
        public int Timeout { get; private set; }
        public int Packets { get; private set; }

        public Settings(bool loc_eng, bool dns, bool ip, bool rt, int group, int period, int timeout, int packets)
        {
            is_eng = loc_eng;
            DNS = dns;
            IP = ip;
            RT = rt;

            Group = group;
            Period = period;
            Timeout = timeout;
            Packets = packets;

            InitializeComponent();
        }

        private void Preprocessing()
        {
            numericUpDown1.Value = Timeout;
            numericUpDown2.Value = Period;
            numericUpDown3.Value = Packets;
        }

        private void Translate()
        {
            if (is_eng)
            {
                Text = "Settings of " + (Group + 1) + " clients group";
                label1.Text = "Ping ever:";
                label2.Text = "Timeout:";
                label3.Text = "Packets count:";
                label4.Text = "sec";
                label5.Text = "min";
                button1.Text = "Apply";
            }
            else
            {
                Text = "Настройки " + (Group + 1) + " группы клиентов";

                label1.Text = "Период автопинга:";
                label2.Text = "Время ожидания:";
                label3.Text = "Кол-во пакетов:";
                label4.Text = "сек";
                label5.Text = "мин";
                button1.Text = "Применить";
            }
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            Preprocessing();
            Translate();
        }
    }
}
