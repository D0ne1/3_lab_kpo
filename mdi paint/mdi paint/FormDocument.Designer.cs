namespace mdi_paint
{
    partial class FormDocument
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
            this.SuspendLayout();
            // 
            // FormDocument
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1173, 545);
            this.DoubleBuffered = true;
            this.Name = "FormDocument";
            this.Text = "fooorm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDocument_FormClosing_1);
            this.Load += new System.EventHandler(this.FormDocument_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormDocument_MouseDown);
            this.MouseLeave += new System.EventHandler(this.FormDocument_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormDocument_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FormDocument_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}