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
            button2 = new Button();
            button1 = new Button();
            textBox5 = new TextBox();
            textBox6 = new TextBox();
            textBox3 = new TextBox();
            txtMax = new TextBox();
            textBox2 = new TextBox();
            btnStopCracking = new Button();
            btnElevate = new Button();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // txtOutput
            // 
            txtOutput.Dock = DockStyle.Right;
            txtOutput.Location = new Point(252, 0);
            txtOutput.Margin = new Padding(3, 2, 3, 2);
            txtOutput.Multiline = true;
            txtOutput.Name = "txtOutput";
            txtOutput.ScrollBars = ScrollBars.Vertical;
            txtOutput.Size = new Size(448, 338);
            txtOutput.TabIndex = 0;
            // 
            // txtTargetHash
            // 
            txtTargetHash.Location = new Point(10, 9);
            txtTargetHash.Margin = new Padding(3, 2, 3, 2);
            txtTargetHash.Name = "txtTargetHash";
            txtTargetHash.Size = new Size(238, 23);
            txtTargetHash.TabIndex = 1;
            txtTargetHash.TextChanged += txtTargetHash_TextChanged;
            // 
            // txtDonePassword
            // 
            txtDonePassword.Location = new Point(10, 308);
            txtDonePassword.Margin = new Padding(3, 2, 3, 2);
            txtDonePassword.Name = "txtDonePassword";
            txtDonePassword.Size = new Size(238, 23);
            txtDonePassword.TabIndex = 2;
            txtDonePassword.TextChanged += txtDonePassword_TextChanged;
            // 
            // btnStartCracking
            // 
            btnStartCracking.Location = new Point(165, 34);
            btnStartCracking.Margin = new Padding(3, 2, 3, 2);
            btnStartCracking.Name = "btnStartCracking";
            btnStartCracking.Size = new Size(82, 22);
            btnStartCracking.TabIndex = 3;
            btnStartCracking.Text = "Crack";
            btnStartCracking.UseVisualStyleBackColor = true;
            btnStartCracking.Click += btnStartCracking_Click;
            // 
            // txtGuessedPassword
            // 
            txtGuessedPassword.Location = new Point(10, 60);
            txtGuessedPassword.Margin = new Padding(3, 2, 3, 2);
            txtGuessedPassword.Name = "txtGuessedPassword";
            txtGuessedPassword.Size = new Size(238, 23);
            txtGuessedPassword.TabIndex = 4;
            // 
            // txtMin
            // 
            txtMin.Location = new Point(27, 44);
            txtMin.Name = "txtMin";
            txtMin.Size = new Size(29, 23);
            txtMin.TabIndex = 5;
            txtMin.Text = "1";
            txtMin.TextAlign = HorizontalAlignment.Center;
            txtMin.TextChanged += txtMin_TextChanged;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnElevate);
            groupBox1.Controls.Add(button2);
            groupBox1.Controls.Add(button1);
            groupBox1.Controls.Add(textBox5);
            groupBox1.Controls.Add(textBox6);
            groupBox1.Controls.Add(textBox3);
            groupBox1.Controls.Add(txtMax);
            groupBox1.Controls.Add(textBox2);
            groupBox1.Controls.Add(txtMin);
            groupBox1.Location = new Point(12, 88);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(234, 183);
            groupBox1.TabIndex = 6;
            groupBox1.TabStop = false;
            groupBox1.Text = "groupBox1";
            // 
            // button2
            // 
            button2.Location = new Point(119, 77);
            button2.Margin = new Padding(3, 2, 3, 2);
            button2.Name = "button2";
            button2.Size = new Size(82, 22);
            button2.TabIndex = 13;
            button2.Text = "Heartbeat";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button1
            // 
            button1.Location = new Point(27, 77);
            button1.Margin = new Padding(3, 2, 3, 2);
            button1.Name = "button1";
            button1.Size = new Size(82, 22);
            button1.TabIndex = 12;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // textBox5
            // 
            textBox5.BackColor = SystemColors.Control;
            textBox5.BorderStyle = BorderStyle.None;
            textBox5.Location = new Point(89, 22);
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(56, 16);
            textBox5.TabIndex = 10;
            textBox5.Text = "Algorithm";
            textBox5.TextAlign = HorizontalAlignment.Center;
            // 
            // textBox6
            // 
            textBox6.Location = new Point(89, 44);
            textBox6.Name = "textBox6";
            textBox6.Size = new Size(56, 23);
            textBox6.TabIndex = 9;
            textBox6.Text = "SHA256";
            textBox6.TextAlign = HorizontalAlignment.Center;
            // 
            // textBox3
            // 
            textBox3.BackColor = SystemColors.Control;
            textBox3.BorderStyle = BorderStyle.None;
            textBox3.Location = new Point(151, 22);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(77, 16);
            textBox3.TabIndex = 8;
            textBox3.Text = "Max Length";
            textBox3.TextAlign = HorizontalAlignment.Center;
            // 
            // txtMax
            // 
            txtMax.Location = new Point(172, 44);
            txtMax.Name = "txtMax";
            txtMax.Size = new Size(29, 23);
            txtMax.TabIndex = 7;
            txtMax.Text = "3";
            txtMax.TextAlign = HorizontalAlignment.Center;
            txtMax.TextChanged += txtMax_TextChanged;
            // 
            // textBox2
            // 
            textBox2.BackColor = SystemColors.Control;
            textBox2.BorderStyle = BorderStyle.None;
            textBox2.Location = new Point(6, 22);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(77, 16);
            textBox2.TabIndex = 6;
            textBox2.Text = "Min Length";
            textBox2.TextAlign = HorizontalAlignment.Center;
            // 
            // btnStopCracking
            // 
            btnStopCracking.Location = new Point(10, 34);
            btnStopCracking.Margin = new Padding(3, 2, 3, 2);
            btnStopCracking.Name = "btnStopCracking";
            btnStopCracking.Size = new Size(82, 22);
            btnStopCracking.TabIndex = 11;
            btnStopCracking.Text = "STOP";
            btnStopCracking.UseVisualStyleBackColor = true;
            btnStopCracking.Click += btnStopCracking_Click;
            // 
            // btnElevate
            // 
            btnElevate.Location = new Point(27, 154);
            btnElevate.Name = "btnElevate";
            btnElevate.Size = new Size(174, 23);
            btnElevate.TabIndex = 14;
            btnElevate.Text = "ELEVATE PRIVLEDGES";
            btnElevate.UseVisualStyleBackColor = true;
            btnElevate.Click += btnElevate_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 338);
            Controls.Add(btnStopCracking);
            Controls.Add(groupBox1);
            Controls.Add(txtGuessedPassword);
            Controls.Add(btnStartCracking);
            Controls.Add(txtDonePassword);
            Controls.Add(txtTargetHash);
            Controls.Add(txtOutput);
            Margin = new Padding(3, 2, 3, 2);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
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
        private Button button1;
        private Button button2;
        private Button btnElevate;
    }
}