using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using Arwiki_Data_Extraction;

namespace Wikipedia_Interface
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog2 = new OpenFileDialog();

            openFileDialog2.InitialDirectory = @"C:\D\work\Dos Digital Library (Manual)\Database_backup_dumps\arwiki\20100123";
            openFileDialog2.Filter = "SQL files (*.sql)|*.sql|All files (*.*)|*.*";
            openFileDialog2.FilterIndex = 2;
            openFileDialog2.RestoreDirectory = true;

            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    textBox2.Text = openFileDialog2.FileName;

                    if (!string.IsNullOrEmpty(textBox2.Text.Trim()))
                    {


                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = @"C:\D\work\Dos Digital Library (Manual)\Database_backup_dumps\arwiki\20100123";
            openFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    textBox1.Text = openFileDialog1.FileName;

                    if (!string.IsNullOrEmpty(textBox1.Text.Trim()))
                    {

                    }
                    //if ((myStream = openFileDialog1.OpenFile()) != null)
                    //{
                    //    using (myStream)
                    //    {
                    //        // Insert code to read the stream here.
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog3 = new OpenFileDialog();

            openFileDialog3.InitialDirectory = @"C:\D\work\Dos Digital Library (Manual)\Database_backup_dumps\arwiki\20100123";
            openFileDialog3.Filter = "SQL files (*.sql)|*.sql|All files (*.*)|*.*";
            openFileDialog3.FilterIndex = 2;
            openFileDialog3.RestoreDirectory = true;

            if (openFileDialog3.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    textBox3.Text = openFileDialog3.FileName;

                    if (!string.IsNullOrEmpty(textBox3.Text.Trim()))
                    {
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Arwiki wiki = new Arwiki(textBox1.Text.Trim(), textBox2.Text.Trim(), textBox3.Text.Trim(), textBox4.Text.Trim(), textBox5.Text.Trim());
            wiki.create_wiki_statistics();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Arwiki wiki = new Arwiki();
            wiki.generate_term_concepts_list_index(textBox6.Text.Trim());
        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            Dictionary<string, bool> custom_concepts_list = new Dictionary<string, bool>();
            using (StreamReader sr = new StreamReader(textBox7.Text.Trim()))
            {
                string c_id;
                while ((c_id = sr.ReadLine()) != null)
                {
                    if (!custom_concepts_list.ContainsKey(c_id))
                        custom_concepts_list.Add(c_id, true);
                }
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(textBox6.Text.Trim());
            XmlElement root = xmlDoc.DocumentElement;

            XmlNodeList concepts_elems = root.GetElementsByTagName("c");

            string temp = "<concepts>";
            foreach (XmlElement concept_elem in concepts_elems)
            {
                if (custom_concepts_list.ContainsKey(concept_elem.GetAttribute("c_id")))
                    temp += concept_elem.OuterXml;
            }
            temp += "</concepts>";
            XmlDocument xmlDoc2 = new XmlDocument();
            xmlDoc2.LoadXml(temp);
            xmlDoc2.Save(Path.GetDirectoryName(textBox7.Text.Trim()) + "\\cust_concepts.xml");
        }
    }
}