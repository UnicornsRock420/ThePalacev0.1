using ThePalace.Core.Client.Core.Models;

namespace ThePalace.Core.Desktop.MsgBubble
{
    public partial class Program : FormBase
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
            this.imgScreen = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.imgScreen)).BeginInit();
            this.SuspendLayout();
            // 
            // imgScreen
            // 
            this.imgScreen.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imgScreen.Location = new System.Drawing.Point(0, 0);
            this.imgScreen.Margin = new System.Windows.Forms.Padding(0);
            this.imgScreen.Name = "imgScreen";
            this.imgScreen.Size = new System.Drawing.Size(512, 384);
            this.imgScreen.TabIndex = 1;
            this.imgScreen.TabStop = false;
            this.imgScreen.Click += new System.EventHandler(this.imgScreen_Click);
            // 
            // Program
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(512, 384);
            this.Controls.Add(this.imgScreen);
            this.Name = "Program";
            this.Text = "MsgBubble Designer";
            ((System.ComponentModel.ISupportInitialize)(this.imgScreen)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private PictureBox imgScreen;
    }
}