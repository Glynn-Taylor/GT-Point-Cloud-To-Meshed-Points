using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PointToMesh
{
    public partial class Form1 : Form
    {
        bool hasInputFile = false;
        MeshData data;

        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void OpenFileBtn(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Supported formats|*.ply;";
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                hasInputFile = true;
                //System.IO.StreamReader sr = new
                   //System.IO.StreamReader(openFileDialog1.FileName);
                fileTextBox.Text = openFileDialog1.FileName;
                //MessageBox.Show(sr.ReadToEnd());
                //sr.Close();
                data = ImporterPCL.Load(openFileDialog1.FileName);
                MessageBox.Show(data.Debug(20));
            }
            else
            {
                hasInputFile = false;
            }
        }

        private void ExportClick(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Supported formats|*.ply;";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK && hasInputFile)
            {
                System.IO.File.WriteAllText(saveFileDialog1.FileName, ImporterPCL.Export(data, saveFileDialog1.FileName));
            }
        }
    }
}
