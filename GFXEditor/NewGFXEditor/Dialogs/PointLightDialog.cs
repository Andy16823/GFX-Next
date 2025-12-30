using LibGFX.Graphics;
using LibGFX.Graphics.Lights;
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
    public partial class PointLightDialog : Form
    {
        public PointLight3D LightSource { get; set; }

        public PointLightDialog(PointLight3D light)
        {
            InitializeComponent();
            this.LightSource = light;
            this.textBox1.Text = light.Name;
            this.textBox2.Text = light.ID.ToString();
            this.vec4Control1.Value = light.Color;
            this.numericUpDown1.Value = (decimal)light.Range;
            this.numericUpDown2.Value = (decimal)light.Intensity;
        }

        private void PointLightDialog_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            this.LightSource.Name = this.textBox1.Text;
            this.LightSource.Color = this.vec4Control1.Value;
            this.LightSource.Range = (float)this.numericUpDown1.Value;
            this.LightSource.Intensity = (float)this.numericUpDown2.Value;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                // Set the background color of the button to the selected color
                var vec4 = ColorPresets.FromRgb(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
                this.vec4Control1.Value = vec4;
            }
        }
    }
}
