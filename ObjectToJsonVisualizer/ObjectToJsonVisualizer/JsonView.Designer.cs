namespace ObjectToJsonVisualizer
{
    partial class JsonView
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
            this.m_richTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // m_richTextBox
            // 
            this.m_richTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_richTextBox.Location = new System.Drawing.Point(0, 0);
            this.m_richTextBox.Name = "m_richTextBox";
            this.m_richTextBox.Size = new System.Drawing.Size(800, 450);
            this.m_richTextBox.TabIndex = 0;
            this.m_richTextBox.Text = "";
            // 
            // JsonView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.m_richTextBox);
            this.Name = "JsonView";
            this.Text = "JsonView";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox m_richTextBox;
    }
}