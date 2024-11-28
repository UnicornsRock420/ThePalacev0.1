using ThePalace.Core.Client.Core.Models;

namespace ThePalace.Core.Desktop.Plugins.Forms
{
    public partial class Tabs : FormBase
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
            this.Sessions = new System.Windows.Forms.TabControl();
            this.Session1 = new System.Windows.Forms.TabPage();
            this.Session2 = new System.Windows.Forms.TabPage();
            this.Sessions.SuspendLayout();
            this.SuspendLayout();
            // 
            // Sessions
            // 
            this.Sessions.Controls.Add(this.Session1);
            this.Sessions.Controls.Add(this.Session2);
            this.Sessions.Location = new System.Drawing.Point(0, 0);
            this.Sessions.Margin = new System.Windows.Forms.Padding(0);
            this.Sessions.Name = "Sessions";
            this.Sessions.SelectedIndex = 0;
            this.Sessions.Size = new System.Drawing.Size(512, 384);
            this.Sessions.TabIndex = 0;
            // 
            // Session1
            // 
            this.Session1.Location = new System.Drawing.Point(4, 24);
            this.Session1.Name = "Session1";
            this.Session1.Padding = new System.Windows.Forms.Padding(3);
            this.Session1.Size = new System.Drawing.Size(504, 356);
            this.Session1.TabIndex = 0;
            this.Session1.Text = "Session1";
            this.Session1.UseVisualStyleBackColor = true;
            // 
            // Session2
            // 
            this.Session2.Location = new System.Drawing.Point(4, 24);
            this.Session2.Name = "Session2";
            this.Session2.Padding = new System.Windows.Forms.Padding(3);
            this.Session2.Size = new System.Drawing.Size(504, 356);
            this.Session2.TabIndex = 1;
            this.Session2.Text = "Session2";
            this.Session2.UseVisualStyleBackColor = true;
            // 
            // Tabs
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(512, 384);
            this.ControlBox = false;
            this.Controls.Add(this.Sessions);
            this.Name = "Tabs";
            this.Text = "Tabs";
            this.Sessions.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TabControl Sessions;
        private TabPage Session1;
        private TabPage Session2;
    }
}