using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace FMCReadHistory
{
    public partial class Form1 : Form
    {
        private string filePath = string.Empty;

        private string[] files;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.textBox1.AllowDrop = true;
            textBox1.DragEnter += panel1_DragEnter;
            textBox1.DragDrop += panel1_DragDrop;

        }

        void panel1_DragDrop(object sender, DragEventArgs e)
        {
            textBox1.Text = string.Empty;
            files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (var item in files)
            {
                textBox1.Text += item + Environment.NewLine;
            }
        }

        void panel1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void Go_Click(object sender, EventArgs e)
        {
            Go.Enabled = false;

            foreach (var item in files)
            {
                if (radioButton1.Checked)
                    HistoryReader.Run(item);
                else if (radioButton2.Checked)
                    GelanshanshuiReader.Run(item);
            }
            Go.Enabled = true;
            textBox1.Text = "请把文件拖过来！";
        }
    }
}
