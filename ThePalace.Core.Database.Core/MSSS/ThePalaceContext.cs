using Microsoft.EntityFrameworkCore;

namespace ThePalace.Core.Database.Core.MSSS
{
    public partial class ThePalaceEntities : DbContext
    {
        public ThePalaceEntities()
        {
        }

        public ThePalaceEntities(DbContextOptions<ThePalaceEntities> options)
            : base(options)
        {
        }

        public virtual DbSet<Assets> Assets { get; set; }
        public virtual DbSet<Assets1> Assets1 { get; set; }
        public virtual DbSet<Auth> Auth { get; set; }
        public virtual DbSet<Bans> Bans { get; set; }
        public virtual DbSet<Config> Config { get; set; }
        public virtual DbSet<Crons> Crons { get; set; }
        public virtual DbSet<DrawCmds> DrawCmds { get; set; }
        public virtual DbSet<DrawCmds1> DrawCmds1 { get; set; }
        public virtual DbSet<DrawCmds2> DrawCmds2 { get; set; }
        public virtual DbSet<GroupRoles> GroupRoles { get; set; }
        public virtual DbSet<Groups> Groups { get; set; }
        public virtual DbSet<GroupUsers> GroupUsers { get; set; }
        public virtual DbSet<Hotspots> Hotspots { get; set; }
        public virtual DbSet<Hotspots1> Hotspots1 { get; set; }
        public virtual DbSet<Hotspots2> Hotspots2 { get; set; }
        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<LooseProps> LooseProps { get; set; }
        public virtual DbSet<LooseProps1> LooseProps1 { get; set; }
        public virtual DbSet<LooseProps2> LooseProps2 { get; set; }
        public virtual DbSet<Metadata> Metadata { get; set; }
        public virtual DbSet<Pictures> Pictures { get; set; }
        public virtual DbSet<Pictures1> Pictures1 { get; set; }
        public virtual DbSet<Pictures2> Pictures2 { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<RoomData> RoomData { get; set; }
        public virtual DbSet<RoomData1> RoomData1 { get; set; }
        public virtual DbSet<RoomData2> RoomData2 { get; set; }
        public virtual DbSet<Rooms> Rooms { get; set; }
        public virtual DbSet<Rooms1> Rooms1 { get; set; }
        public virtual DbSet<Rooms2> Rooms2 { get; set; }
        public virtual DbSet<Sessions> Sessions { get; set; }
        public virtual DbSet<States> States { get; set; }
        public virtual DbSet<UserData> UserData { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Users1> Users1 { get; set; }
        public virtual DbSet<Vortexes> Vortexes { get; set; }
        public virtual DbSet<Vortexes1> Vortexes1 { get; set; }
        public virtual DbSet<Vortexes2> Vortexes2 { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(""); // ConnectionString
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Assets>(entity =>
            {
                entity.HasKey(e => e.AssetId);

                entity.ToTable("Assets", "Assets");

                entity.Property(e => e.AssetId)
                    .HasColumnName("AssetID")
                    .ValueGeneratedNever();

                entity.Property(e => e.LastUsed).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1024);
            });

            modelBuilder.Entity<Assets1>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.AssetIndex });

                entity.ToTable("Assets", "Users");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.AssetId).HasColumnName("AssetID");
            });

            modelBuilder.Entity<Auth>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("Auth", "Admin");

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Value).HasMaxLength(255);
            });

            modelBuilder.Entity<Bans>(entity =>
            {
                entity.HasKey(e => e.BanId);

                entity.ToTable("Bans", "Admin");

                entity.Property(e => e.BanId).HasColumnName("BanID");

                entity.Property(e => e.Ipaddress)
                    .IsRequired()
                    .HasColumnName("IPAddress")
                    .HasMaxLength(50);

                entity.Property(e => e.Puidcrc).HasColumnName("PUIDCrc");

                entity.Property(e => e.Puidctr).HasColumnName("PUIDCtr");

                entity.Property(e => e.UntilDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Config>(entity =>
            {
                entity.HasKey(e => e.Key);

                entity.ToTable("Config", "Admin");

                entity.Property(e => e.Key)
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.Value).HasMaxLength(100);
            });

            modelBuilder.Entity<Crons>(entity =>
            {
                entity.HasKey(e => e.CronId);

                entity.ToTable("Crons", "Crons");

                entity.Property(e => e.CronId).HasColumnName("CronID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.SpName)
                    .IsRequired()
                    .HasMaxLength(1024);
            });

            modelBuilder.Entity<DrawCmds>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoomId, e.DrawCmdId });

                entity.ToTable("DrawCmds", "Saves");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.RoomId).HasColumnName("RoomID");

                entity.Property(e => e.DrawCmdId).HasColumnName("DrawCmdID");
            });

            modelBuilder.Entity<DrawCmds1>(entity =>
            {
                entity.HasKey(e => new { e.TemplateId, e.DrawCmdId });

                entity.ToTable("DrawCmds", "Templates");

                entity.Property(e => e.TemplateId).HasColumnName("TemplateID");

                entity.Property(e => e.DrawCmdId).HasColumnName("DrawCmdID");
            });

            modelBuilder.Entity<DrawCmds2>(entity =>
            {
                entity.HasKey(e => new { e.RoomId, e.DrawCmdId });

                entity.ToTable("DrawCmds", "Rooms");

                entity.Property(e => e.RoomId).HasColumnName("RoomID");

                entity.Property(e => e.DrawCmdId).HasColumnName("DrawCmdID");
            });

            modelBuilder.Entity<GroupRoles>(entity =>
            {
                entity.HasKey(e => new { e.GroupId, e.RoleId });

                entity.ToTable("GroupRoles", "Admin");

                entity.Property(e => e.GroupId).HasColumnName("GroupID");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");
            });

            modelBuilder.Entity<Groups>(entity =>
            {
                entity.HasKey(e => e.GroupId);

                entity.ToTable("Groups", "Admin");

                entity.Property(e => e.GroupId).HasColumnName("GroupID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<GroupUsers>(entity =>
            {
                entity.HasKey(e => new { e.GroupId, e.UserId });

                entity.ToTable("GroupUsers", "Admin");

                entity.Property(e => e.GroupId).HasColumnName("GroupID");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<Hotspots>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoomId, e.HotspotId });

                entity.ToTable("Hotspots", "Saves");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.RoomId).HasColumnName("RoomID");

                entity.Property(e => e.HotspotId).HasColumnName("HotspotID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1024);
            });

            modelBuilder.Entity<Hotspots1>(entity =>
            {
                entity.HasKey(e => new { e.TemplateId, e.HotspotId });

                entity.ToTable("Hotspots", "Templates");

                entity.Property(e => e.TemplateId).HasColumnName("TemplateID");

                entity.Property(e => e.HotspotId).HasColumnName("HotspotID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1024);
            });

            modelBuilder.Entity<Hotspots2>(entity =>
            {
                entity.HasKey(e => new { e.RoomId, e.HotspotId });

                entity.ToTable("Hotspots", "Hotspots");

                entity.Property(e => e.RoomId).HasColumnName("RoomID");

                entity.Property(e => e.HotspotId).HasColumnName("HotspotID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1024);
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.ToTable("Log", "Admin");

                entity.Property(e => e.LogId).HasColumnName("LogID");

                entity.Property(e => e.ApplicationName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.MachineName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Message).IsRequired();

                entity.Property(e => e.MessageType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ProcessId).HasColumnName("ProcessID");
            });

            modelBuilder.Entity<LooseProps>(entity =>
            {
                entity.HasKey(e => new { e.TemplateId, e.OrderId });

                entity.ToTable("LooseProps", "Templates");

                entity.Property(e => e.TemplateId).HasColumnName("TemplateID");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.AssetCrc).HasColumnName("AssetCRC");

                entity.Property(e => e.AssetId).HasColumnName("AssetID");
            });

            modelBuilder.Entity<LooseProps1>(entity =>
            {
                entity.HasKey(e => new { e.RoomId, e.OrderId });

                entity.ToTable("LooseProps", "Rooms");

                entity.Property(e => e.RoomId).HasColumnName("RoomID");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.AssetCrc).HasColumnName("AssetCRC");

                entity.Property(e => e.AssetId).HasColumnName("AssetID");
            });

            modelBuilder.Entity<LooseProps2>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoomId, e.OrderId });

                entity.ToTable("LooseProps", "Saves");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.RoomId).HasColumnName("RoomID");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.AssetCrc).HasColumnName("AssetCRC");

                entity.Property(e => e.AssetId).HasColumnName("AssetID");
            });

            modelBuilder.Entity<Metadata>(entity =>
            {
                entity.HasKey(e => e.AssetId);

                entity.ToTable("Metadata", "Assets");

                entity.Property(e => e.AssetId)
                    .HasColumnName("AssetID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Format)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Pictures>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoomId, e.PictureId });

                entity.ToTable("Pictures", "Saves");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.RoomId).HasColumnName("RoomID");

                entity.Property(e => e.PictureId).HasColumnName("PictureID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1024);
            });

            modelBuilder.Entity<Pictures1>(entity =>
            {
                entity.HasKey(e => new { e.TemplateId, e.PictureId });

                entity.ToTable("Pictures", "Templates");

                entity.Property(e => e.TemplateId).HasColumnName("TemplateID");

                entity.Property(e => e.PictureId).HasColumnName("PictureID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1024);
            });

            modelBuilder.Entity<Pictures2>(entity =>
            {
                entity.HasKey(e => new { e.RoomId, e.PictureId });

                entity.ToTable("Pictures", "Hotspots");

                entity.Property(e => e.RoomId).HasColumnName("RoomID");

                entity.Property(e => e.PictureId).HasColumnName("PictureID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1024);
            });

            modelBuilder.Entity<Roles>(entity =>
            {
                entity.HasKey(e => e.RoleId);

                entity.ToTable("Roles", "Admin");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<RoomData>(entity =>
            {
                entity.HasKey(e => e.RoomId);

                entity.ToTable("RoomData", "Rooms");

                entity.Property(e => e.RoomId)
                    .HasColumnName("RoomID")
                    .ValueGeneratedNever();

                entity.Property(e => e.ArtistName).HasMaxLength(1024);

                entity.Property(e => e.FacesId).HasColumnName("FacesID");

                entity.Property(e => e.Password).HasMaxLength(1024);

                entity.Property(e => e.PictureName).HasMaxLength(1024);
            });

            modelBuilder.Entity<RoomData1>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoomId });

                entity.ToTable("RoomData", "Saves");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.RoomId).HasColumnName("RoomID");

                entity.Property(e => e.ArtistName).HasMaxLength(1024);

                entity.Property(e => e.FacesId).HasColumnName("FacesID");

                entity.Property(e => e.Password).HasMaxLength(1024);

                entity.Property(e => e.PictureName).HasMaxLength(1024);
            });

            modelBuilder.Entity<RoomData2>(entity =>
            {
                entity.HasKey(e => e.TemplateId);

                entity.ToTable("RoomData", "Templates");

                entity.Property(e => e.TemplateId)
                    .HasColumnName("TemplateID")
                    .ValueGeneratedNever();

                entity.Property(e => e.ArtistName).HasMaxLength(1024);

                entity.Property(e => e.FacesId).HasColumnName("FacesID");

                entity.Property(e => e.Password).HasMaxLength(1024);

                entity.Property(e => e.PictureName).HasMaxLength(1024);
            });

            modelBuilder.Entity<Rooms>(entity =>
            {
                entity.HasKey(e => e.RoomId);

                entity.ToTable("Rooms", "Rooms");

                entity.Property(e => e.RoomId)
                    .HasColumnName("RoomID")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.LastModified).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1024);
            });

            modelBuilder.Entity<Rooms1>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoomId });

                entity.ToTable("Rooms", "Saves");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.RoomId).HasColumnName("RoomID");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.LastModified).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1024);
            });

            modelBuilder.Entity<Rooms2>(entity =>
            {
                entity.HasKey(e => e.TemplateId);

                entity.ToTable("Rooms", "Templates");

                entity.Property(e => e.TemplateId).HasColumnName("TemplateID");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.LastModified).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1024);
            });

            modelBuilder.Entity<Sessions>(entity =>
            {
                entity.HasKey(e => new { e.UserId });

                entity.ToTable("Sessions", "Admin");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<States>(entity =>
            {
                entity.HasKey(e => new { e.RoomId, e.HotspotId, e.StateId });

                entity.ToTable("States", "Hotspots");

                entity.Property(e => e.RoomId).HasColumnName("RoomID");

                entity.Property(e => e.HotspotId).HasColumnName("HotspotID");

                entity.Property(e => e.StateId).HasColumnName("StateID");

                entity.Property(e => e.PictureId).HasColumnName("PictureID");
            });

            modelBuilder.Entity<UserData>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("UserData", "Users");

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .ValueGeneratedNever();
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("Users", "Admin");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.LastLogin).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Users1>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("Users", "Users");

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.RoomId).HasColumnName("RoomID");
            });

            modelBuilder.Entity<Vortexes>(entity =>
            {
                entity.HasKey(e => new { e.RoomId, e.HotspotId, e.VortexId });

                entity.ToTable("Vortexes", "Hotspots");

                entity.Property(e => e.RoomId).HasColumnName("RoomID");

                entity.Property(e => e.HotspotId).HasColumnName("HotspotID");

                entity.Property(e => e.VortexId).HasColumnName("VortexID");
            });

            modelBuilder.Entity<Vortexes1>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoomId, e.HotspotId, e.VortexId });

                entity.ToTable("Vortexes", "Saves");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.RoomId).HasColumnName("RoomID");

                entity.Property(e => e.HotspotId).HasColumnName("HotspotID");

                entity.Property(e => e.VortexId).HasColumnName("VortexID");
            });

            modelBuilder.Entity<Vortexes2>(entity =>
            {
                entity.HasKey(e => new { e.TemplateId, e.HotspotId, e.VortexId });

                entity.ToTable("Vortexes", "Templates");

                entity.Property(e => e.TemplateId).HasColumnName("TemplateID");

                entity.Property(e => e.HotspotId).HasColumnName("HotspotID");

                entity.Property(e => e.VortexId).HasColumnName("VortexID");
            });
        }
    }
}
