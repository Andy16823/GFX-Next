using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NewGFXEditor.Dialogs
{
    /// <summary>
    /// Represents a dialog form that allows users to view and edit a string property consisting of a name and value.
    /// </summary>
    /// <remarks>Use this form to prompt users for a string-based property, such as a key-value pair or
    /// configuration setting. The edited property values are accessible through the Data property after the dialog is
    /// closed with an OK result.</remarks>
    public partial class StringProperty : Form
    {
        public StringPropertyData Data { get; set; } = new StringPropertyData();

        public StringProperty(String name, String value)
        {
            InitializeComponent();
            Data = new StringPropertyData()
            {
                Name = name,
                Value = value
            };
        }

        private void StringProperty_Load(object sender, EventArgs e)
        {
            textBox1.Text = Data.Name;
            textBox2.Text = Data.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Data.Name = textBox1.Text;
            Data.Value = textBox2.Text;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
