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
    public partial class CollisionTypeProperty : Form
    {
        public String CollisionType { get; set; }

        private List<String> collisionTypes = new List<String>()
        {
            "None",
            "BoxRigidBody",
            "SphereRigidBody",
            "CapsuleRigidBody",
            "MeshRigidBody",
            "BoxCollider",
            "SphereCollider",
            "CapsuleCollider",
            "MeshCollider",
            "BoxTrigger",
            "SphereTrigger",
            "CapsuleTrigger",
            "MeshTrigger"
        };

        public CollisionTypeProperty(String type)
        {
            InitializeComponent();
            CollisionType = type;
        }

        private void CollisionTypeProperty_Load(object sender, EventArgs e)
        {
            this.comboBox1.Items.Clear();
            this.comboBox1.Items.AddRange(collisionTypes.ToArray());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedItem != null)
            {
                CollisionType = this.comboBox1.SelectedItem?.ToString() ?? string.Empty;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Please select a collision type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
    }
}
