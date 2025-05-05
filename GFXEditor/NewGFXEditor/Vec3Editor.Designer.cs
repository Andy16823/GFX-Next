namespace NewGFXEditor
{
    partial class Vec3Editor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            xTextbox = new TextBox();
            yTextbox = new TextBox();
            label2 = new Label();
            zTextbox = new TextBox();
            label3 = new Label();
            button1 = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(17, 24);
            label1.Name = "label1";
            label1.Size = new Size(40, 15);
            label1.TabIndex = 0;
            label1.Text = "X-Axis";
            // 
            // xTextbox
            // 
            xTextbox.Location = new Point(91, 21);
            xTextbox.Name = "xTextbox";
            xTextbox.Size = new Size(248, 23);
            xTextbox.TabIndex = 1;
            // 
            // yTextbox
            // 
            yTextbox.Location = new Point(91, 50);
            yTextbox.Name = "yTextbox";
            yTextbox.Size = new Size(248, 23);
            yTextbox.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(17, 53);
            label2.Name = "label2";
            label2.Size = new Size(40, 15);
            label2.TabIndex = 2;
            label2.Text = "Y-Axis";
            // 
            // zTextbox
            // 
            zTextbox.Location = new Point(91, 79);
            zTextbox.Name = "zTextbox";
            zTextbox.Size = new Size(248, 23);
            zTextbox.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(17, 82);
            label3.Name = "label3";
            label3.Size = new Size(40, 15);
            label3.TabIndex = 4;
            label3.Text = "Z-Axis";
            // 
            // button1
            // 
            button1.Location = new Point(213, 121);
            button1.Name = "button1";
            button1.Size = new Size(126, 23);
            button1.TabIndex = 6;
            button1.Text = "Save";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // Vec3Editor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(351, 156);
            Controls.Add(button1);
            Controls.Add(zTextbox);
            Controls.Add(label3);
            Controls.Add(yTextbox);
            Controls.Add(label2);
            Controls.Add(xTextbox);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Vec3Editor";
            Text = "Vec3Editor";
            Load += Vec3Editor_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox xTextbox;
        private TextBox yTextbox;
        private Label label2;
        private TextBox zTextbox;
        private Label label3;
        private Button button1;
    }
}