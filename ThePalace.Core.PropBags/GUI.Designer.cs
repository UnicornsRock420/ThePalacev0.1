namespace ThePalace.Core.PropBags
{
    partial class GUI
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
            this.browseForPRP = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.exportPRP = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.exportPropIDs = new System.Windows.Forms.Button();
            this.exportPNGs = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // browseForPRP
            // 
            this.browseForPRP.Location = new System.Drawing.Point(12, 12);
            this.browseForPRP.Name = "browseForPRP";
            this.browseForPRP.Size = new System.Drawing.Size(75, 40);
            this.browseForPRP.TabIndex = 0;
            this.browseForPRP.Text = "Load PRP(s)";
            this.browseForPRP.UseVisualStyleBackColor = true;
            this.browseForPRP.Click += new System.EventHandler(this.loadPRPs_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "PRP Files|*.prp";
            this.openFileDialog.Multiselect = true;
            // 
            // exportPRP
            // 
            this.exportPRP.Location = new System.Drawing.Point(93, 12);
            this.exportPRP.Name = "exportPRP";
            this.exportPRP.Size = new System.Drawing.Size(75, 40);
            this.exportPRP.TabIndex = 1;
            this.exportPRP.Text = "Export PRP";
            this.exportPRP.UseVisualStyleBackColor = true;
            this.exportPRP.Click += new System.EventHandler(this.exportPRP_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "PRP Files|*.prp";
            // 
            // exportPropIDs
            // 
            this.exportPropIDs.Location = new System.Drawing.Point(174, 12);
            this.exportPropIDs.Name = "exportPropIDs";
            this.exportPropIDs.Size = new System.Drawing.Size(75, 40);
            this.exportPropIDs.TabIndex = 2;
            this.exportPropIDs.Text = "Export PropIDs";
            this.exportPropIDs.UseVisualStyleBackColor = true;
            this.exportPropIDs.Click += new System.EventHandler(this.exportPropIDs_Click);
            // 
            // exportPNGs
            // 
            this.exportPNGs.Location = new System.Drawing.Point(255, 12);
            this.exportPNGs.Name = "exportPNGs";
            this.exportPNGs.Size = new System.Drawing.Size(75, 40);
            this.exportPNGs.TabIndex = 3;
            this.exportPNGs.Text = "Export PNGs";
            this.exportPNGs.UseVisualStyleBackColor = true;
            this.exportPNGs.Click += new System.EventHandler(this.exportPNGs_Click);
            // 
            // AppForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(343, 64);
            this.Controls.Add(this.exportPNGs);
            this.Controls.Add(this.exportPropIDs);
            this.Controls.Add(this.exportPRP);
            this.Controls.Add(this.browseForPRP);
            this.Name = "AppForm";
            this.Text = "Prop Extractor";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button browseForPRP;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button exportPRP;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Button exportPropIDs;
        private System.Windows.Forms.Button exportPNGs;
    }
}

