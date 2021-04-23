using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace Java_MC_Shape_To_VS_Shape
{
    public partial class MCShapeToVSShape : Form
    {
        public MCShapeToVSShape()
        {
            InitializeComponent();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            var fD = (sender as OpenFileDialog);
            if (fD.FileName == null) return;

            using (TextReader tr = new StreamReader(fD.FileName))
            {
                string data = tr.ReadToEnd();
                try
                {
                    if (fD.SafeFileName.EndsWith(".bbmodel"))
                    {
                        Program.loadedBBModel = JsonConvert.DeserializeObject<BBModelJson>(data);
                    }
                    else
                    {
                        Program.loadedMCModel = JsonConvert.DeserializeObject<MCModelJSON>(data);
                    }
                    textBox1.Text = fD.FileName;
                    this.button2.Enabled = true;
                    this.saveFileDialog1.FileName = fD.FileName;
                }
                catch (Exception ex)
                {
                    textBox1.Text = "Invalid File!";
                    Console.WriteLine(ex.Message);
                    Program.loadedMCModel = null;
                    this.button2.Enabled = false;
                }

                tr.Close();
            }

            Program.ConvertMCToVS();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.saveFileDialog1.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            var fD = sender as SaveFileDialog;
            if (fD.FileName == null) return;

            Program.Save(fD.FileName);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }
    }
}