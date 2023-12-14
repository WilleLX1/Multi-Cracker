namespace MultiCracker
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtOutput = new TextBox();
            txtTargetHash = new TextBox();
            txtDonePassword = new TextBox();
            btnStartCracking = new Button();
            txtGuessedPassword = new TextBox();
            txtMin = new TextBox();
            groupBox1 = new GroupBox();
            textBox5 = new TextBox();
            textBox6 = new TextBox();
            textBox3 = new TextBox();
            txtMax = new TextBox();
            textBox2 = new TextBox();
            btnStopCracking = new Button();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // txtOutput
            // 
            txtOutput.Dock = DockStyle.Right;
            txtOutput.Location = new Point(289, 0);
            txtOutput.Multiline = true;
            txtOutput.Name = "txtOutput";
            txtOutput.ScrollBars = ScrollBars.Vertical;
            txtOutput.Size = new Size(511, 451);
            txtOutput.TabIndex = 0;
            // 
            // txtTargetHash
            // 
            txtTargetHash.Location = new Point(11, 12);
            txtTargetHash.Name = "txtTargetHash";
            txtTargetHash.Size = new Size(271, 27);
            txtTargetHash.TabIndex = 1;
            txtTargetHash.TextChanged += txtTargetHash_TextChanged;
            // 
            // txtDonePassword
            // 
            txtDonePassword.Location = new Point(11, 411);
            txtDonePassword.Name = "txtDonePassword";
            txtDonePassword.Size = new Size(271, 27);
            txtDonePassword.TabIndex = 2;
            txtDonePassword.TextChanged += txtDonePassword_TextChanged;
            // 
            // btnStartCracking
            // 
            btnStartCracking.Location = new Point(189, 45);
            btnStartCracking.Name = "btnStartCracking";
            btnStartCracking.Size = new Size(94, 29);
            btnStartCracking.TabIndex = 3;
            btnStartCracking.Text = "Crack";
            btnStartCracking.UseVisualStyleBackColor = true;
            btnStartCracking.Click += btnStartCracking_Click;
            // 
            // txtGuessedPassword
            // 
            txtGuessedPassword.Location = new Point(11, 80);
            txtGuessedPassword.Name = "txtGuessedPassword";
            txtGuessedPassword.Size = new Size(271, 27);
            txtGuessedPassword.TabIndex = 4;
            // 
            // txtMin
            // 
            txtMin.Location = new Point(31, 59);
            txtMin.Margin = new Padding(3, 4, 3, 4);
            txtMin.Name = "txtMin";
            txtMin.Size = new Size(33, 27);
            txtMin.TabIndex = 5;
            txtMin.Text = "1";
            txtMin.TextAlign = HorizontalAlignment.Center;
            txtMin.TextChanged += txtMin_TextChanged;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(textBox5);
            groupBox1.Controls.Add(textBox6);
            groupBox1.Controls.Add(textBox3);
            groupBox1.Controls.Add(txtMax);
            groupBox1.Controls.Add(textBox2);
            groupBox1.Controls.Add(txtMin);
            groupBox1.Location = new Point(14, 117);
            groupBox1.Margin = new Padding(3, 4, 3, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(3, 4, 3, 4);
            groupBox1.Size = new Size(267, 244);
            groupBox1.TabIndex = 6;
            groupBox1.TabStop = false;
            groupBox1.Text = "groupBox1";
            // 
            // textBox5
            // 
            textBox5.BackColor = SystemColors.Control;
            textBox5.BorderStyle = BorderStyle.None;
            textBox5.Location = new Point(102, 29);
            textBox5.Margin = new Padding(3, 4, 3, 4);
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(64, 20);
            textBox5.TabIndex = 10;
            textBox5.Text = "Algorithm";
            textBox5.TextAlign = HorizontalAlignment.Center;
            // 
            // textBox6
            // 
            textBox6.Location = new Point(102, 59);
            textBox6.Margin = new Padding(3, 4, 3, 4);
            textBox6.Name = "textBox6";
            textBox6.Size = new Size(63, 27);
            textBox6.TabIndex = 9;
            textBox6.Text = "SHA256";
            textBox6.TextAlign = HorizontalAlignment.Center;
            // 
            // textBox3
            // 
            textBox3.BackColor = SystemColors.Control;
            textBox3.BorderStyle = BorderStyle.None;
            textBox3.Location = new Point(173, 29);
            textBox3.Margin = new Padding(3, 4, 3, 4);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(88, 20);
            textBox3.TabIndex = 8;
            textBox3.Text = "Max Length";
            textBox3.TextAlign = HorizontalAlignment.Center;
            // 
            // txtMax
            // 
            txtMax.Location = new Point(197, 59);
            txtMax.Margin = new Padding(3, 4, 3, 4);
            txtMax.Name = "txtMax";
            txtMax.Size = new Size(33, 27);
            txtMax.TabIndex = 7;
            txtMax.Text = "5";
            txtMax.TextAlign = HorizontalAlignment.Center;
            txtMax.TextChanged += txtMax_TextChanged;
            // 
            // textBox2
            // 
            textBox2.BackColor = SystemColors.Control;
            textBox2.BorderStyle = BorderStyle.None;
            textBox2.Location = new Point(7, 29);
            textBox2.Margin = new Padding(3, 4, 3, 4);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(88, 20);
            textBox2.TabIndex = 6;
            textBox2.Text = "Min Length";
            textBox2.TextAlign = HorizontalAlignment.Center;
            // 
            // btnStopCracking
            // 
            btnStopCracking.Location = new Point(11, 45);
            btnStopCracking.Name = "btnStopCracking";
            btnStopCracking.Size = new Size(94, 29);
            btnStopCracking.TabIndex = 11;
            btnStopCracking.Text = "STOP";
            btnStopCracking.UseVisualStyleBackColor = true;
            btnStopCracking.Click += btnStopCracking_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 451);
            Controls.Add(btnStopCracking);
            Controls.Add(groupBox1);
            Controls.Add(txtGuessedPassword);
            Controls.Add(btnStartCracking);
            Controls.Add(txtDonePassword);
            Controls.Add(txtTargetHash);
            Controls.Add(txtOutput);
            Name = "Form1";
            Text = "Form1";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtOutput;
        private TextBox txtTargetHash;
        private TextBox txtDonePassword;
        private Button btnStartCracking;
        private TextBox txtGuessedPassword;
        private TextBox txtMin;
        private GroupBox groupBox1;
        private TextBox textBox3;
        private TextBox txtMax;
        private TextBox textBox2;
        private TextBox textBox5;
        private TextBox textBox6;
        private Button btnStopCracking;
    }
}