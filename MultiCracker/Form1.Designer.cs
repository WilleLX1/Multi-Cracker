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
            SuspendLayout();
            // 
            // txtOutput
            // 
            txtOutput.Dock = DockStyle.Right;
            txtOutput.Location = new Point(289, 0);
            txtOutput.Multiline = true;
            txtOutput.Name = "txtOutput";
            txtOutput.Size = new Size(511, 450);
            txtOutput.TabIndex = 0;
            // 
            // txtTargetHash
            // 
            txtTargetHash.Location = new Point(12, 12);
            txtTargetHash.Name = "txtTargetHash";
            txtTargetHash.Size = new Size(271, 27);
            txtTargetHash.TabIndex = 1;
            // 
            // txtDonePassword
            // 
            txtDonePassword.Location = new Point(12, 411);
            txtDonePassword.Name = "txtDonePassword";
            txtDonePassword.Size = new Size(271, 27);
            txtDonePassword.TabIndex = 2;
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
            txtGuessedPassword.Location = new Point(12, 80);
            txtGuessedPassword.Name = "txtGuessedPassword";
            txtGuessedPassword.Size = new Size(271, 27);
            txtGuessedPassword.TabIndex = 4;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(txtGuessedPassword);
            Controls.Add(btnStartCracking);
            Controls.Add(txtDonePassword);
            Controls.Add(txtTargetHash);
            Controls.Add(txtOutput);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtOutput;
        private TextBox txtTargetHash;
        private TextBox txtDonePassword;
        private Button btnStartCracking;
        private TextBox txtGuessedPassword;
    }
}