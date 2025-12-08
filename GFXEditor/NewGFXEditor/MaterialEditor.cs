using LibGFX.Graphics.Materials;
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
    public partial class MaterialEditor : Form
    {
        public SGMaterial Material { get; set; }

        public MaterialEditor(SGMaterial material)
        {
            InitializeComponent();

            this.textBox1.Text = material.Name;

            this.Material = material;
            this.numericUpDown1.Value = (decimal)material.Color.X * 255;
            this.numericUpDown2.Value = (decimal)material.Color.Y * 255;
            this.numericUpDown3.Value = (decimal)material.Color.Z * 255;
            this.numericUpDown4.Value = (decimal)material.Color.W * 255;

            var diffuseBitmap = material.DiffuseTexture.ToBitmap();
            this.pictureBox1.Image = diffuseBitmap;

            var normalBitmap = material.NormalTexture.ToBitmap();
            this.pictureBox2.Image = normalBitmap;

            var specularBitmap = material.SpecularTexture.ToBitmap();
            this.pictureBox3.Image = specularBitmap;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void MaterialEditor_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.tga";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.textBox2.Text = openFileDialog.FileName;
                var bitmap = new Bitmap(openFileDialog.FileName);
                this.pictureBox1.Image = bitmap;
                this.Material.DiffuseTexture = new LibGFX.Graphics.Texture(openFileDialog.FileName);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.tga";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.textBox3.Text = openFileDialog.FileName;
                var bitmap = new Bitmap(openFileDialog.FileName);
                this.pictureBox2.Image = bitmap;
                this.Material.NormalTexture = new LibGFX.Graphics.Texture(openFileDialog.FileName);
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.tga";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.textBox4.Text = openFileDialog.FileName;
                var bitmap = new Bitmap(openFileDialog.FileName);
                this.pictureBox3.Image = bitmap;
                this.Material.SpecularTexture = new LibGFX.Graphics.Texture(openFileDialog.FileName);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = Color.Red;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                var color = colorDialog.Color;
                this.numericUpDown1.Value = color.R;
                this.numericUpDown2.Value = color.G;
                this.numericUpDown3.Value = color.B;
                this.numericUpDown4.Value = color.A;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Material.Name = this.textBox1.Text;
            float r = (float)(this.numericUpDown1.Value) / 255.0f;
            float g = (float)(this.numericUpDown2.Value) / 255.0f;
            float b = (float)(this.numericUpDown3.Value) / 255.0f;
            float a = (float)(this.numericUpDown4.Value) / 255.0f;
            this.Material.Color = new OpenTK.Mathematics.Vector4(r, g, b, a);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
