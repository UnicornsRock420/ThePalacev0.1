using ThePalace.Core.Client.Core.Models;

namespace ThePalace.Core.Desktop.Plugins.Forms
{
    public partial class Test : FormBase
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Test));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.tsButtonConnection = new System.Windows.Forms.ToolStripButton();
            this.tsDDBBookmarks = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsButtonLiveDirectory = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsButtonBack = new System.Windows.Forms.ToolStripButton();
            this.tsDDBBack = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsMIPreviousRoom = new System.Windows.Forms.ToolStripMenuItem();
            this.tsButtonForward = new System.Windows.Forms.ToolStripButton();
            this.tsDDBForward = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsButtonUserlist = new System.Windows.Forms.ToolStripButton();
            this.tsDDBUserlist = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsButtonRoomlist = new System.Windows.Forms.ToolStripButton();
            this.tsDDBRoomlist = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsButtonSounds = new System.Windows.Forms.ToolStripButton();
            this.tsDDBSounds = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsButtonDraw = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsButtonDoorOutlines = new System.Windows.Forms.ToolStripButton();
            this.tsButtonUserNametags = new System.Windows.Forms.ToolStripButton();
            this.tsButtonChatlog = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsButtonTabsOff = new System.Windows.Forms.ToolStripButton();
            this.tsButtonTabsOn = new System.Windows.Forms.ToolStripButton();
            this.tsButtonTerminal = new System.Windows.Forms.ToolStripButton();
            this.tsButtonSuperUser = new System.Windows.Forms.ToolStripButton();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(38, 38);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsButtonConnection,
            this.tsDDBBookmarks,
            this.tsButtonLiveDirectory,
            this.toolStripSeparator1,
            this.tsButtonBack,
            this.tsDDBBack,
            this.tsButtonForward,
            this.tsDDBForward,
            this.toolStripSeparator2,
            this.tsButtonUserlist,
            this.tsDDBUserlist,
            this.tsButtonRoomlist,
            this.tsDDBRoomlist,
            this.tsButtonSounds,
            this.tsDDBSounds,
            this.tsButtonDraw,
            this.toolStripSeparator3,
            this.tsButtonDoorOutlines,
            this.tsButtonUserNametags,
            this.tsButtonChatlog,
            this.toolStripSeparator4,
            this.tsButtonTabsOff,
            this.tsButtonTabsOn,
            this.tsButtonTerminal,
            this.tsButtonSuperUser});
            this.toolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Padding = new System.Windows.Forms.Padding(0);
            this.toolStrip.Size = new System.Drawing.Size(1034, 45);
            this.toolStrip.Stretch = true;
            this.toolStrip.TabIndex = 0;
            this.toolStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStrip_ItemClicked);
            // 
            // tsButtonConnection
            // 
            this.tsButtonConnection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsButtonConnection.Image = ((System.Drawing.Image)(resources.GetObject("tsButtonConnection.Image")));
            this.tsButtonConnection.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsButtonConnection.Name = "tsButtonConnection";
            this.tsButtonConnection.Size = new System.Drawing.Size(42, 42);
            this.tsButtonConnection.Text = "Connection";
            this.tsButtonConnection.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsButtonConnection.ToolTipText = "Connection";
            // 
            // tsDDBBookmarks
            // 
            this.tsDDBBookmarks.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsDDBBookmarks.Image = ((System.Drawing.Image)(resources.GetObject("tsDDBBookmarks.Image")));
            this.tsDDBBookmarks.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsDDBBookmarks.Name = "tsDDBBookmarks";
            this.tsDDBBookmarks.Size = new System.Drawing.Size(51, 42);
            this.tsDDBBookmarks.Text = "Bookmarks";
            this.tsDDBBookmarks.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsDDBBookmarks.ToolTipText = "Bookmarks";
            this.tsDDBBookmarks.Click += new System.EventHandler(this.toolStripMenuItem_Click);
            // 
            // tsButtonLiveDirectory
            // 
            this.tsButtonLiveDirectory.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsButtonLiveDirectory.Image = ((System.Drawing.Image)(resources.GetObject("tsButtonLiveDirectory.Image")));
            this.tsButtonLiveDirectory.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsButtonLiveDirectory.Name = "tsButtonLiveDirectory";
            this.tsButtonLiveDirectory.Size = new System.Drawing.Size(42, 42);
            this.tsButtonLiveDirectory.Text = "Live Directory";
            this.tsButtonLiveDirectory.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsButtonLiveDirectory.ToolTipText = "Live Directory";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.AutoSize = false;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 45);
            // 
            // tsButtonBack
            // 
            this.tsButtonBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsButtonBack.Image = ((System.Drawing.Image)(resources.GetObject("tsButtonBack.Image")));
            this.tsButtonBack.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsButtonBack.Name = "tsButtonBack";
            this.tsButtonBack.Size = new System.Drawing.Size(42, 42);
            this.tsButtonBack.Text = "Go Back";
            this.tsButtonBack.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsButtonBack.ToolTipText = "Go Back";
            // 
            // tsDDBBack
            // 
            this.tsDDBBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsDDBBack.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMIPreviousRoom});
            this.tsDDBBack.Image = ((System.Drawing.Image)(resources.GetObject("tsDDBBack.Image")));
            this.tsDDBBack.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsDDBBack.Name = "tsDDBBack";
            this.tsDDBBack.Size = new System.Drawing.Size(51, 42);
            this.tsDDBBack.Text = "Go Back";
            this.tsDDBBack.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsDDBBack.ToolTipText = "Go Back";
            // 
            // tsMIPreviousRoom
            // 
            this.tsMIPreviousRoom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsMIPreviousRoom.Name = "tsMIPreviousRoom";
            this.tsMIPreviousRoom.Size = new System.Drawing.Size(180, 22);
            this.tsMIPreviousRoom.Text = "Previous Room";
            this.tsMIPreviousRoom.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsMIPreviousRoom.ToolTipText = "Previous Room";
            this.tsMIPreviousRoom.Click += new System.EventHandler(this.toolStripMenuItem_Click);
            // 
            // tsButtonForward
            // 
            this.tsButtonForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsButtonForward.Image = ((System.Drawing.Image)(resources.GetObject("tsButtonForward.Image")));
            this.tsButtonForward.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsButtonForward.Name = "tsButtonForward";
            this.tsButtonForward.Size = new System.Drawing.Size(42, 42);
            this.tsButtonForward.Text = "Go Forward";
            this.tsButtonForward.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsButtonForward.ToolTipText = "Go Forward";
            // 
            // tsDDBForward
            // 
            this.tsDDBForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsDDBForward.Image = ((System.Drawing.Image)(resources.GetObject("tsDDBForward.Image")));
            this.tsDDBForward.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsDDBForward.Name = "tsDDBForward";
            this.tsDDBForward.Size = new System.Drawing.Size(51, 42);
            this.tsDDBForward.Text = "Go Forward";
            this.tsDDBForward.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsDDBForward.ToolTipText = "Go Forward";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.AutoSize = false;
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 45);
            // 
            // tsButtonUserlist
            // 
            this.tsButtonUserlist.CheckOnClick = true;
            this.tsButtonUserlist.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsButtonUserlist.Image = ((System.Drawing.Image)(resources.GetObject("tsButtonUserlist.Image")));
            this.tsButtonUserlist.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsButtonUserlist.Name = "tsButtonUserlist";
            this.tsButtonUserlist.Size = new System.Drawing.Size(42, 42);
            this.tsButtonUserlist.Text = "Users List";
            this.tsButtonUserlist.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsButtonUserlist.ToolTipText = "Users List";
            // 
            // tsDDBUserlist
            // 
            this.tsDDBUserlist.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsDDBUserlist.Image = ((System.Drawing.Image)(resources.GetObject("tsDDBUserlist.Image")));
            this.tsDDBUserlist.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsDDBUserlist.Name = "tsDDBUserlist";
            this.tsDDBUserlist.Size = new System.Drawing.Size(51, 42);
            this.tsDDBUserlist.Text = "Users List";
            this.tsDDBUserlist.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsDDBUserlist.ToolTipText = "Users List";
            // 
            // tsButtonRoomlist
            // 
            this.tsButtonRoomlist.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsButtonRoomlist.Image = ((System.Drawing.Image)(resources.GetObject("tsButtonRoomlist.Image")));
            this.tsButtonRoomlist.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsButtonRoomlist.Name = "tsButtonRoomlist";
            this.tsButtonRoomlist.Size = new System.Drawing.Size(42, 42);
            this.tsButtonRoomlist.Text = "Rooms List";
            this.tsButtonRoomlist.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsButtonRoomlist.ToolTipText = "Rooms List";
            // 
            // tsDDBRoomlist
            // 
            this.tsDDBRoomlist.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsDDBRoomlist.Image = ((System.Drawing.Image)(resources.GetObject("tsDDBRoomlist.Image")));
            this.tsDDBRoomlist.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsDDBRoomlist.Name = "tsDDBRoomlist";
            this.tsDDBRoomlist.Size = new System.Drawing.Size(51, 42);
            this.tsDDBRoomlist.Text = "Rooms List";
            this.tsDDBRoomlist.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsDDBRoomlist.ToolTipText = "Rooms List";
            // 
            // tsButtonSounds
            // 
            this.tsButtonSounds.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsButtonSounds.Image = ((System.Drawing.Image)(resources.GetObject("tsButtonSounds.Image")));
            this.tsButtonSounds.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsButtonSounds.Name = "tsButtonSounds";
            this.tsButtonSounds.Size = new System.Drawing.Size(42, 42);
            this.tsButtonSounds.Text = "Sounds";
            this.tsButtonSounds.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsButtonSounds.ToolTipText = "Sounds";
            // 
            // tsDDBSounds
            // 
            this.tsDDBSounds.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsDDBSounds.Image = ((System.Drawing.Image)(resources.GetObject("tsDDBSounds.Image")));
            this.tsDDBSounds.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsDDBSounds.Name = "tsDDBSounds";
            this.tsDDBSounds.Size = new System.Drawing.Size(51, 42);
            this.tsDDBSounds.Text = "Sounds";
            this.tsDDBSounds.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsDDBSounds.ToolTipText = "Sounds";
            // 
            // tsButtonDraw
            // 
            this.tsButtonDraw.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsButtonDraw.Image = ((System.Drawing.Image)(resources.GetObject("tsButtonDraw.Image")));
            this.tsButtonDraw.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsButtonDraw.Name = "tsButtonDraw";
            this.tsButtonDraw.Size = new System.Drawing.Size(42, 42);
            this.tsButtonDraw.Text = "Draw";
            this.tsButtonDraw.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsButtonDraw.ToolTipText = "Draw";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.AutoSize = false;
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 45);
            // 
            // tsButtonDoorOutlines
            // 
            this.tsButtonDoorOutlines.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsButtonDoorOutlines.Image = ((System.Drawing.Image)(resources.GetObject("tsButtonDoorOutlines.Image")));
            this.tsButtonDoorOutlines.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsButtonDoorOutlines.Name = "tsButtonDoorOutlines";
            this.tsButtonDoorOutlines.Size = new System.Drawing.Size(42, 42);
            this.tsButtonDoorOutlines.Text = "Door Outlines";
            this.tsButtonDoorOutlines.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsButtonDoorOutlines.ToolTipText = "Door Outlines";
            // 
            // tsButtonUserNametags
            // 
            this.tsButtonUserNametags.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsButtonUserNametags.Image = ((System.Drawing.Image)(resources.GetObject("tsButtonUserNametags.Image")));
            this.tsButtonUserNametags.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsButtonUserNametags.Name = "tsButtonUserNametags";
            this.tsButtonUserNametags.Size = new System.Drawing.Size(42, 42);
            this.tsButtonUserNametags.Text = "User Nametags";
            this.tsButtonUserNametags.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsButtonUserNametags.ToolTipText = "User Nametags";
            // 
            // tsButtonChatlog
            // 
            this.tsButtonChatlog.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsButtonChatlog.Image = ((System.Drawing.Image)(resources.GetObject("tsButtonChatlog.Image")));
            this.tsButtonChatlog.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsButtonChatlog.Name = "tsButtonChatlog";
            this.tsButtonChatlog.Size = new System.Drawing.Size(42, 42);
            this.tsButtonChatlog.Text = "Chatlog";
            this.tsButtonChatlog.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsButtonChatlog.ToolTipText = "Chatlog";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.AutoSize = false;
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 45);
            // 
            // tsButtonTabsOff
            // 
            this.tsButtonTabsOff.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsButtonTabsOff.Image = ((System.Drawing.Image)(resources.GetObject("tsButtonTabsOff.Image")));
            this.tsButtonTabsOff.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsButtonTabsOff.Name = "tsButtonTabsOff";
            this.tsButtonTabsOff.Size = new System.Drawing.Size(42, 42);
            this.tsButtonTabsOff.Text = "Windowed UI";
            this.tsButtonTabsOff.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsButtonTabsOff.ToolTipText = "Windowed UI";
            // 
            // tsButtonTabsOn
            // 
            this.tsButtonTabsOn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsButtonTabsOn.Image = ((System.Drawing.Image)(resources.GetObject("tsButtonTabsOn.Image")));
            this.tsButtonTabsOn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsButtonTabsOn.Name = "tsButtonTabsOn";
            this.tsButtonTabsOn.Size = new System.Drawing.Size(42, 42);
            this.tsButtonTabsOn.Text = "Tabbed UI";
            this.tsButtonTabsOn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsButtonTabsOn.ToolTipText = "Tabbed UI";
            // 
            // tsButtonTerminal
            // 
            this.tsButtonTerminal.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsButtonTerminal.Image = ((System.Drawing.Image)(resources.GetObject("tsButtonTerminal.Image")));
            this.tsButtonTerminal.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsButtonTerminal.Name = "tsButtonTerminal";
            this.tsButtonTerminal.Size = new System.Drawing.Size(42, 42);
            this.tsButtonTerminal.Text = "Terminal";
            this.tsButtonTerminal.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsButtonTerminal.ToolTipText = "Terminal";
            // 
            // tsButtonSuperUser
            // 
            this.tsButtonSuperUser.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsButtonSuperUser.Image = ((System.Drawing.Image)(resources.GetObject("tsButtonSuperUser.Image")));
            this.tsButtonSuperUser.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsButtonSuperUser.Name = "tsButtonSuperUser";
            this.tsButtonSuperUser.Size = new System.Drawing.Size(42, 42);
            this.tsButtonSuperUser.Text = "SuperUser";
            this.tsButtonSuperUser.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsButtonSuperUser.ToolTipText = "SuperUser";
            // 
            // Test
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1034, 450);
            this.Controls.Add(this.toolStrip);
            this.Name = "Test";
            this.Text = "Test";
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ToolStrip toolStrip;
        private ToolStripDropDownButton tsDDBBack;
        private ToolStripDropDownButton tsDDBForward;
        private ToolStripButton tsButtonUserNametags;
        private ToolStripButton tsButtonLiveDirectory;
        private ToolStripButton tsButtonDoorOutlines;
        private ToolStripButton tsButtonSounds;
        private ToolStripButton tsButtonBack;
        private ToolStripButton tsButtonForward;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton tsButtonUserlist;
        private ToolStripButton tsButtonRoomlist;
        private ToolStripButton tsButtonDraw;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton tsButtonChatlog;
        private ToolStripButton tsButtonTerminal;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton tsButtonSuperUser;
        private ToolStripDropDownButton tsDDBBookmarks;
        private ToolStripDropDownButton tsDDBUserlist;
        private ToolStripDropDownButton tsDDBRoomlist;
        private ToolStripDropDownButton tsDDBSounds;
        private ToolStripMenuItem tsMIPreviousRoom;
        private ToolStripButton tsButtonTabsOn;
        private ToolStripButton tsButtonTabsOff;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripButton tsButtonConnection;
    }
}