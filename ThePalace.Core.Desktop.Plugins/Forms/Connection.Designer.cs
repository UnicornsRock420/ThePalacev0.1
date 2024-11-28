using ThePalace.Core.Client.Core.Models;

namespace ThePalace.Core.Desktop.Plugins.Forms
{
    public partial class Connection : FormBase
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
            this.labelUsername = new System.Windows.Forms.Label();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.comboBoxUsernames = new System.Windows.Forms.ComboBox();
            this.comboBoxAddresses = new System.Windows.Forms.ComboBox();
            this.labelAddress = new System.Windows.Forms.Label();
            this.labelRoomID = new System.Windows.Forms.Label();
            this.textBoxRoomID = new System.Windows.Forms.TextBox();
            this.checkBoxNewTab = new System.Windows.Forms.CheckBox();
            this.buttonDisconnect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelUsername
            // 
            this.labelUsername.AutoSize = true;
            this.labelUsername.Location = new System.Drawing.Point(12, 12);
            this.labelUsername.Name = "labelUsername";
            this.labelUsername.Size = new System.Drawing.Size(63, 15);
            this.labelUsername.TabIndex = 0;
            this.labelUsername.Text = "Username:";
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(103, 138);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(75, 23);
            this.buttonConnect.TabIndex = 7;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(205, 138);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // comboBoxUsernames
            // 
            this.comboBoxUsernames.FormattingEnabled = true;
            this.comboBoxUsernames.ItemHeight = 15;
            this.comboBoxUsernames.Location = new System.Drawing.Point(103, 9);
            this.comboBoxUsernames.MaxLength = 32;
            this.comboBoxUsernames.Name = "comboBoxUsernames";
            this.comboBoxUsernames.Size = new System.Drawing.Size(177, 23);
            this.comboBoxUsernames.TabIndex = 1;
            // 
            // comboBoxAddresses
            // 
            this.comboBoxAddresses.FormattingEnabled = true;
            this.comboBoxAddresses.ItemHeight = 15;
            this.comboBoxAddresses.Location = new System.Drawing.Point(103, 49);
            this.comboBoxAddresses.Name = "comboBoxAddresses";
            this.comboBoxAddresses.Size = new System.Drawing.Size(177, 23);
            this.comboBoxAddresses.TabIndex = 3;
            // 
            // labelAddress
            // 
            this.labelAddress.AutoSize = true;
            this.labelAddress.Location = new System.Drawing.Point(12, 52);
            this.labelAddress.Name = "labelAddress";
            this.labelAddress.Size = new System.Drawing.Size(52, 15);
            this.labelAddress.TabIndex = 2;
            this.labelAddress.Text = "Address:";
            // 
            // labelRoomID
            // 
            this.labelRoomID.AutoSize = true;
            this.labelRoomID.Location = new System.Drawing.Point(12, 96);
            this.labelRoomID.Name = "labelRoomID";
            this.labelRoomID.Size = new System.Drawing.Size(56, 15);
            this.labelRoomID.TabIndex = 4;
            this.labelRoomID.Text = "Room ID:";
            // 
            // textBoxRoomID
            // 
            this.textBoxRoomID.Location = new System.Drawing.Point(103, 93);
            this.textBoxRoomID.Name = "textBoxRoomID";
            this.textBoxRoomID.Size = new System.Drawing.Size(74, 23);
            this.textBoxRoomID.TabIndex = 5;
            // 
            // checkBoxNewTab
            // 
            this.checkBoxNewTab.AutoSize = true;
            this.checkBoxNewTab.Location = new System.Drawing.Point(197, 97);
            this.checkBoxNewTab.Name = "checkBoxNewTab";
            this.checkBoxNewTab.Size = new System.Drawing.Size(71, 19);
            this.checkBoxNewTab.TabIndex = 6;
            this.checkBoxNewTab.Text = "New Tab";
            this.checkBoxNewTab.UseVisualStyleBackColor = true;
            // 
            // buttonDisconnect
            // 
            this.buttonDisconnect.Location = new System.Drawing.Point(12, 138);
            this.buttonDisconnect.Name = "buttonDisconnect";
            this.buttonDisconnect.Size = new System.Drawing.Size(75, 23);
            this.buttonDisconnect.TabIndex = 9;
            this.buttonDisconnect.Text = "Disconnect";
            this.buttonDisconnect.UseVisualStyleBackColor = true;
            this.buttonDisconnect.Visible = false;
            // 
            // Connection
            // 
            this.AcceptButton = this.buttonConnect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(303, 182);
            this.Controls.Add(this.buttonDisconnect);
            this.Controls.Add(this.checkBoxNewTab);
            this.Controls.Add(this.textBoxRoomID);
            this.Controls.Add(this.labelRoomID);
            this.Controls.Add(this.comboBoxAddresses);
            this.Controls.Add(this.labelAddress);
            this.Controls.Add(this.comboBoxUsernames);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.labelUsername);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Connection";
            this.Text = "New Connection";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label labelUsername;
        private Button buttonConnect;
        private Button buttonCancel;
        private ComboBox comboBoxUsernames;
        private ComboBox comboBoxAddresses;
        private Label labelAddress;
        private Label labelRoomID;
        private TextBox textBoxRoomID;
        private CheckBox checkBoxNewTab;
        private Button buttonDisconnect;
    }
}