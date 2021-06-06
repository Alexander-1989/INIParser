using System;
using System.IniParser;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace INIParser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
        }

        INIFile INI = new INIFile();

        private void Form1_Load(object sender, EventArgs e)
        {
            if (INI.FileExists == false) return;
            Location = new Point(INI.Parse<int>("Main", "X"), INI.Parse<int>("Main", "Y"));
            textBox1.Text = INI.Read("Main", "Text");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            INI.Write("Main", "X", Location.X);
            INI.Write("Main", "Y", Location.Y);
            INI.Write("Main", "Text", textBox1.Text);
        }
    }
}
