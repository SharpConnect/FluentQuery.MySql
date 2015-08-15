namespace MySqlClient
{
    partial class FormTestSuite
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
            this.listboxTestCases = new System.Windows.Forms.ListBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // listboxTestCases
            // 
            this.listboxTestCases.FormattingEnabled = true;
            this.listboxTestCases.Location = new System.Drawing.Point(12, 30);
            this.listboxTestCases.Name = "listboxTestCases";
            this.listboxTestCases.Size = new System.Drawing.Size(270, 394);
            this.listboxTestCases.TabIndex = 2;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(303, 30);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(306, 119);
            this.textBox1.TabIndex = 4;
            // 
            // FormTestSuite
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 503);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.listboxTestCases);
            this.Name = "FormTestSuite";
            this.Text = "FormTestSuite";
            this.Load += new System.EventHandler(this.FormTestSuite_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listboxTestCases;
        private System.Windows.Forms.TextBox textBox1;
    }
}