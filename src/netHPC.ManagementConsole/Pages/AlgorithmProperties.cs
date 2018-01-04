
using netHPC.ManagementConsole.Nodes;
using netHPC.Shared;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace netHPC.ManagementConsole.Pages
{
    public partial class AlgorithmProperties : UserControl
    {
        private AlgorithmNode m_algorithmNode;
        private Algorithm m_algorithm;
        private Byte[] m_assemblyBytes;

        public AlgorithmProperties()
        {
            InitializeComponent();
        }

        internal AlgorithmNode AlgorithmNode
        {
            get { return m_algorithmNode; }
            set { m_algorithmNode = value; }
        }

        public Algorithm Algorithm
        {
            get { return m_algorithm; }
            set { m_algorithm = value; }
        }

        private void AlgorithmProperties_Load(object sender, EventArgs e)
        {
            textBoxName.Text = m_algorithm.Name;
            textBoxDescription.Text = m_algorithm.Description;
            labelCreatedAt.Text = m_algorithm.DateCreated.ToString();
            labelModifiedAt.Text = m_algorithm.DateModified.ToString();
        }

        private void buttonDownload_Click(object sender, EventArgs e)
        {            
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Select where you want to save the algorithm's assembly";
            saveFileDialog.DefaultExt = "*.dll";            
            saveFileDialog.Filter = "netHPC Algorithms|*.dll";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                BinaryWriter binaryWriter = new BinaryWriter(File.Open(saveFileDialog.FileName, FileMode.OpenOrCreate));
                binaryWriter.Write(m_algorithm.Assembly);
                binaryWriter.Close();
            }
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select the assembly containing the algoritm to be distributed";
            openFileDialog.CheckPathExists = true;
            openFileDialog.CheckFileExists = true;
            openFileDialog.DefaultExt = "*.dll";
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "netHPC Algorithms|*.dll";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                BinaryReader binaryReader = new BinaryReader(File.Open(openFileDialog.FileName, FileMode.Open));
                m_assemblyBytes = binaryReader.ReadBytes((Int32)binaryReader.BaseStream.Length);
                binaryReader.Close();
            }
        }

        public Byte[] Assembly
        {
            get { return m_assemblyBytes; }
        }

        public String AlgorithmName
        {
            get { return textBoxName.Text; }
        }

        public String AlgorithmDescription
        {
            get { return textBoxDescription.Text; }
        }
    }
}
