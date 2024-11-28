using ThePalace.Core.Client.Core.Models;

namespace ThePalace.Core.Desktop.Plugins.Forms
{
    public partial class Logging : FormBase
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
            this.logWindow = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // logWindow
            // 
            this.logWindow.Location = new System.Drawing.Point(0, 0);
            this.logWindow.Margin = new System.Windows.Forms.Padding(0);
            this.logWindow.Name = "logWindow";
            this.logWindow.ReadOnly = true;
            this.logWindow.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.logWindow.Size = new System.Drawing.Size(257, 442);
            this.logWindow.TabIndex = 0;
            this.logWindow.Text = "";
            // 
            // Logging
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(256, 441);
            this.Controls.Add(this.logWindow);
            this.Name = "Logging";
            this.Text = "Logging";
            this.ResumeLayout(false);

        }

        #endregion

        private RichTextBox logWindow;
    }
}