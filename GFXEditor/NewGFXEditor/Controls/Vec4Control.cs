using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NewGFXEditor.Controls
{
    public partial class Vec4Control : UserControl
    {
        public Vector4 Value { get => _value; set => SetValue(value) ; }
        private Vector4 _value;

        public Vec4Control()
        {
            InitializeComponent();
            Value = Vector4.Zero;
            this.textBox1.Text = "0";
            this.textBox2.Text = "0";
            this.textBox3.Text = "0";   
            this.textBox4.Text = "0";
        }

        private void SetValue(Vector4 value)
        {
            _value = value;
            this.textBox1.Text = this.Value.X.ToString(CultureInfo.InvariantCulture);
            this.textBox2.Text = this.Value.Y.ToString(CultureInfo.InvariantCulture);
            this.textBox3.Text = this.Value.Z.ToString(CultureInfo.InvariantCulture);
            this.textBox4.Text = this.Value.W.ToString(CultureInfo.InvariantCulture);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            var value = float.TryParse(this.textBox1.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var result);
            if (value)
            {
                this.Value = new Vector4(result, this.Value.Y, this.Value.Z, this.Value.W);
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            var value = float.TryParse(this.textBox2.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var result);
            if (value)
            {
                this.Value = new Vector4(this.Value.X, result, this.Value.Z, this.Value.W);
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            var value = float.TryParse(this.textBox3.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var result);
            if (value)
            {
                this.Value = new Vector4(this.Value.X, this.Value.Y, result, this.Value.W);
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            var value = float.TryParse(this.textBox4.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var result);
            if (value)
            {
                this.Value = new Vector4(this.Value.X, this.Value.Y, this.Value.Z, result);
            }
        }
    }
}
