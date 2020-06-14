using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace lab8_1
{
    public partial class Form1 : Form
    {
        DataSet dataSet = new DataSet();
      
        string filePath = "";
        bool needsSaving = false;

        public Form1()
        {
            InitializeComponent();
            dataSet.ReadXmlSchema("student_scheme.xsd");
            tabControl1.Enabled = true;

            exitToolStripMenuItem.Click += (s, e) => Close();
            saveFileDialog1.FileOk += (s, e) => Save(saveFileDialog1.FileName);
        }
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == System.Windows.Forms.DialogResult.OK &&
                 openFileDialog1.FileName.Length > 0)
            {
                filePath = openFileDialog1.FileName;
               
                if (Path.GetExtension(filePath) == ".xml")
                {
                    dataSet.Clear();
                    tabControl1.Enabled = true;
                   
                    dataSet.ReadXml(filePath);
                    needsSaving = false;
                    tabControl1.Controls.Clear();
                    InitDataGridPages();
                    
                    this.Text = openFileDialog1.FileName;

                }
                else
                {
                    MessageBox.Show("this not xml type");
                }

            }
            
        }
        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (needsSaving)
            {
                saveFileDialog1.FileName = filePath;
                saveFileDialog1.ShowDialog();
            }
            else
            {
                Save(filePath);
            }
        }

        private void Save(string fileName)
        {
            if (Path.GetExtension(fileName) == ".xml")
            {
                
                dataSet.WriteXml(fileName);
                filePath = fileName;
                needsSaving = false;
            }
            else
            {
                MessageBox.Show("this not xml type");
            }

        }

        private void InitDataGridPages()
        {
            foreach (var table in dataSet.Tables)
            {
                this.tabControl1.TabPages.Add(table.ToString());
                var page = tabControl1.TabPages[tabControl1.TabPages.Count - 1];
                var dataGrid = new DataGridView();
                page.Controls.Add(dataGrid);
                dataGrid.Dock = DockStyle.Fill;
                dataGrid.DataSource = table;

                var t = table as DataTable;
                foreach (var c in t.Columns)
                {
                    var col = c as DataColumn;
                    if (col.ColumnMapping == MappingType.Hidden)
                        col.ColumnMapping = MappingType.Attribute;
                }

                dataGrid.RowValidating += RowValidating;
                dataGrid.DataError += DataError;
            }
        }

        private void DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            var dataGrid = sender as DataGridView;
            dataGrid.Rows[e.RowIndex].ErrorText = e.Exception.Message;
            MessageBox.Show(e.Exception.Message, "DataError", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            
            var dataGrid = sender as DataGridView;
            dataGrid.Rows[e.RowIndex].ErrorText = "";

            string headerText = dataGrid.Columns[e.ColumnIndex].HeaderText;
            if (headerText != "Name") return;

            string Name = dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            if (Name.Contains("@"))
            {
                e.Cancel = true;
                dataGrid.Rows[e.RowIndex].ErrorText = "Wrong Symbol";
            }
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var newFileDialog = new NewFileForm();
            if (newFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                filePath = newFileDialog.FileName;
                dataSet.Clear();
                tabControl1.Enabled = true;
                
                needsSaving = true;
                tabControl1.Controls.Clear();
                InitDataGridPages();
            }
        }
    }
}
