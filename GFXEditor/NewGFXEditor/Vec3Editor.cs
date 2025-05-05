using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NewGFXEditor
{
    public partial class Vec3Editor : Form
    {
        public Vector3 Value { get; set; }

        public Vec3Editor(Vector3 value)
        {
            InitializeComponent();
            this.Value = value;

            this.xTextbox.Text = value.X.ToString();
            this.yTextbox.Text = value.Y.ToString();
            this.zTextbox.Text = value.Z.ToString();
        }

        private void Vec3Editor_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Value = new Vector3(
                float.Parse(this.xTextbox.Text),
                float.Parse(this.yTextbox.Text),
                float.Parse(this.zTextbox.Text)
            );
            this.Close();
        }
    }
}
