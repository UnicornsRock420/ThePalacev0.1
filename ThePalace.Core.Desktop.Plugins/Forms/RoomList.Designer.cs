using ThePalace.Core.Client.Core.Models;

namespace ThePalace.Core.Desktop.Plugins.Forms
{
    public partial class RoomList : FormBase
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
            this.gridRoomList = new System.Windows.Forms.DataGridView();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUsers = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridRoomList)).BeginInit();
            this.SuspendLayout();
            // 
            // gridRoomList
            // 
            this.gridRoomList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridRoomList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colUsers});
            this.gridRoomList.Location = new System.Drawing.Point(0, 0);
            this.gridRoomList.Margin = new System.Windows.Forms.Padding(0);
            this.gridRoomList.Name = "gridRoomList";
            this.gridRoomList.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.gridRoomList.RowHeadersWidth = 4;
            this.gridRoomList.RowTemplate.Height = 25;
            this.gridRoomList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gridRoomList.Size = new System.Drawing.Size(240, 188);
            this.gridRoomList.TabIndex = 0;
            // 
            // colName
            // 
            this.colName.HeaderText = "Name";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            // 
            // colUsers
            // 
            this.colUsers.HeaderText = "Users";
            this.colUsers.Name = "colUsers";
            this.colUsers.ReadOnly = true;
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(162, 195);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(68, 23);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Location = new System.Drawing.Point(84, 195);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(68, 23);
            this.buttonRefresh.TabIndex = 2;
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(8, 195);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(68, 23);
            this.buttonDelete.TabIndex = 3;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Visible = false;
            // 
            // RoomList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 227);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.gridRoomList);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RoomList";
            this.Text = "RoomList";
            ((System.ComponentModel.ISupportInitialize)(this.gridRoomList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DataGridView gridRoomList;
        private DataGridViewTextBoxColumn colName;
        private DataGridViewTextBoxColumn colUsers;
        private Button buttonClose;
        private Button buttonRefresh;
        private Button buttonDelete;
    }
}