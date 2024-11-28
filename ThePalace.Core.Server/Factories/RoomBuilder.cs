using Microsoft.Data.SqlClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.ExtensionMethods;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Server.Core;
using ThePalace.Core.Server.Core.Factories;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Factories
{
    public partial class RoomBuilder : ChangeTracking, IProtocolReceive, IProtocolSend, IDisposable
    {
        #region Members
        // Data Mappings
        private Int32 roomFlags;
        //public Int32 serverRoomFlags;
        private Int32 facesID;
        private Int16 roomID;
        private Int16 roomNameOfst;
        private Int16 pictNameOfst;
        private Int16 artistNameOfst;
        private Int16 passwordOfst;
        private Int16 nbrHotspots;
        private Int16 hotspotOfst;
        private Int16 nbrPictures;
        private Int16 pictureOfst;
        private Int16 nbrDrawCmds;
        private Int16 firstDrawCmd;
        //private Int16 nbrPeople;
        private Int16 nbrLProps;
        private Int16 firstLProp;
        private Int16 reserved;
        private Int16 lenVars;

        // Accessible strings and data
        private string roomName;
        private string roomPicture;
        private string roomArtist;
        private string roomPassword;
        private Int16 roomMaxOccupancy;
        private bool roomNotFound;
        private Int32 _height;
        private Int32 _width;
        public bool HasUnsavedAuthorChanges = false;

        // Collections
        private List<HotspotRec> _hotspots;
        private List<PictureRec> _pictures;
        private List<DrawCmdRec> _drawCmds;
        private List<LoosePropRec> _looseProps;
        #endregion

        #region Properties
        public List<HotspotRec> Hotspots
        {
            get => _hotspots;
            set
            {
                _hotspots = value;
                NotifyPropertyChanged(nameof(Hotspots));
            }
        }
        public List<PictureRec> Pictures
        {
            get => _pictures;
            set
            {
                _pictures = value;
                NotifyPropertyChanged(nameof(Pictures));
            }
        }
        public List<DrawCmdRec> DrawCommands
        {
            get => _drawCmds;
            set
            {
                _drawCmds = value;
                NotifyPropertyChanged(nameof(DrawCommands));
            }
        }

        public List<LoosePropRec> LooseProps
        {
            get => _looseProps;
            set
            {
                _looseProps = value;
                NotifyPropertyChanged(nameof(LooseProps));
            }
        }

        public string Name
        {
            get => roomName;
            set
            {
                if (roomName != value)
                {
                    roomName = value;
                    NotifyPropertyChanged(nameof(Name));
                }
            }
        }

        public string Picture
        {
            get => roomPicture;
            set
            {
                if (roomPicture != value)
                {
                    _height = 0;
                    _width = 0;

                    roomPicture = value;
                    NotifyPropertyChanged(nameof(Picture));
                }
            }
        }

        public string Artist
        {
            get => roomArtist;
            set
            {
                if (roomArtist != value)
                {
                    roomArtist = value;
                    NotifyPropertyChanged(nameof(Artist));
                }
            }
        }

        public string Password
        {
            get => roomPassword;
            set
            {
                if (roomPassword != value)
                {
                    roomPassword = value;
                    NotifyPropertyChanged(nameof(Password));
                }
            }
        }

        public Int16 ID
        {
            get => roomID;
            set
            {
                if (roomID != value)
                {
                    roomID = value;
                    NotifyPropertyChanged(nameof(ID));
                }
            }
        }

        public Int16 MaxOccupancy
        {
            get => roomMaxOccupancy;
            set
            {
                if (roomMaxOccupancy != value)
                {
                    roomMaxOccupancy = value;
                    NotifyPropertyChanged(nameof(MaxOccupancy));
                }
            }
        }

        public Int32 Flags
        {
            get
            {
                return roomFlags;
            }
            set
            {
                if (roomFlags != value)
                {
                    roomFlags = value;
                    NotifyPropertyChanged(nameof(Flags));
                }
            }
        }

        public Int32 FacesID
        {
            get
            {
                return facesID;
            }
            set
            {
                if (facesID != value)
                {
                    facesID = value;
                    NotifyPropertyChanged(nameof(FacesID));
                }
            }
        }

        public bool NotFound
        {
            get
            {
                return roomNotFound;
            }
            private set
            {
                roomNotFound = value;
            }
        }

        public Int32 Width
        {
            get
            {
                if (_width > 0)
                {
                    return _width;
                }
                else if (!string.IsNullOrWhiteSpace(roomPicture))
                {
                    var path = Path.Combine(Environment.CurrentDirectory, "Media", roomPicture);

                    if (File.Exists(path))
                    {
                        using (var image = Image.FromFile(path))
                        {
                            if (image.Width > 0)
                            {
                                _width = image.Width;
                                _height = image.Height;
                            }
                        }
                    }

                    if (_width < 512)
                    {
                        _width = 512;
                    }

                    return _width;
                }
                else
                {
                    return 512;
                }
            }
        }

        public Int32 Height
        {
            get
            {
                if (_height > 0)
                {
                    return _height;
                }
                else if (!string.IsNullOrWhiteSpace(roomPicture))
                {
                    var path = Path.Combine(Environment.CurrentDirectory, "Media", roomPicture);

                    if (File.Exists(path))
                    {
                        using (var image = Image.FromFile(path))
                        {
                            if (image.Height > 0)
                            {
                                _width = image.Width;
                                _height = image.Height;
                            }
                        }
                    }

                    if (_height < 384)
                    {
                        _height = 384;
                    }

                    return _height;
                }
                else
                {
                    return 384;
                }
            }
        }
        #endregion

        #region Constructor

        public RoomBuilder() : base()
        {
            _hotspots = new List<HotspotRec>();
            _pictures = new List<PictureRec>();
            _drawCmds = new List<DrawCmdRec>();
            _looseProps = new List<LoosePropRec>();
            isDirty = false;
        }

        public RoomBuilder(RoomRec room) : this()
        {
            roomFlags = room.roomFlags;
            facesID = room.facesID;
            roomID = room.roomID;
            roomName = room.roomName;
            roomPicture = room.roomPicture;
            roomArtist = room.roomArtist;
            roomPassword = room.roomPassword;
            roomMaxOccupancy = room.roomMaxOccupancy;

            _hotspots = room.HotSpots;
            _pictures = room.Pictures;
            _drawCmds = room.DrawCmds;
            _looseProps = room.LooseProps;
        }

        public RoomBuilder(IEnumerable<byte> data)
        {
            SetData(data);

            _hotspots = new List<HotspotRec>();
            _pictures = new List<PictureRec>();
            _drawCmds = new List<DrawCmdRec>();
            _looseProps = new List<LoosePropRec>();
            isDirty = false;
        }

        public RoomBuilder(Packet packet) : base()
        {
            _data = new List<byte>(packet.GetData());

            _hotspots = new List<HotspotRec>();
            _pictures = new List<PictureRec>();
            _drawCmds = new List<DrawCmdRec>();
            _looseProps = new List<LoosePropRec>();
            isDirty = false;
        }

        public RoomBuilder(Int16 RoomID) : base()
        {
            _hotspots = new List<HotspotRec>();
            _pictures = new List<PictureRec>();
            _drawCmds = new List<DrawCmdRec>();
            _looseProps = new List<LoosePropRec>();
            isDirty = false;

            ID = RoomID;
        }

        public new void Dispose()
        {
            base.Dispose();

            _hotspots.Clear();
            _looseProps.Clear();
            _drawCmds.Clear();
            _pictures.Clear();
            _hotspots = null;
            _looseProps = null;
            _drawCmds = null;
            _pictures = null;
        }

        #endregion

        public void Peek(ThePalaceEntities dbContext)
        {
            if (Core.Factories.RoomDataExts.Peek(dbContext, ID, out RoomRec roomData))
            {
                roomFlags = roomData.roomFlags;
                roomName = roomData.roomName;
                roomMaxOccupancy = roomData.roomMaxOccupancy;
                LastModified = roomData.roomLastModified;
                //roomNotFound = roomData.roomNotFound;
            }
            else
            {
                roomNotFound = true;
            }
        }

        public void Read(ThePalaceEntities dbContext)
        {
            try
            {
                if (Core.Factories.RoomDataExts.ReadRoom(dbContext, ID, out RoomRec roomData))
                {
                    roomFlags = roomData.roomFlags;
                    roomName = roomData.roomName;
                    roomMaxOccupancy = roomData.roomMaxOccupancy;
                    LastModified = roomData.roomLastModified;
                    //roomNotFound = roomData.roomNotFound;

                    facesID = roomData.facesID;
                    roomPicture = roomData.roomPicture;
                    roomArtist = roomData.roomArtist;
                    roomPassword = roomData.roomPassword;

                    Core.Factories.RoomDataExts.ReadHotspots(dbContext, ID, out _hotspots);
                    Core.Factories.RoomDataExts.ReadPictures(dbContext, ID, out _pictures);
                    Core.Factories.RoomDataExts.ReadLooseProps(dbContext, ID, out _looseProps);
                    Core.Factories.RoomDataExts.ReadDrawCmds(dbContext, ID, out _drawCmds);

                    isDirty = false;
                }
                else
                {
                    roomNotFound = true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            HasUnsavedAuthorChanges = false;
        }

        public void Write(ThePalaceEntities dbContext)
        {
            try
            {
                Core.Factories.RoomDataExts.WriteRoom(dbContext, ID, new RoomRec
                {
                    roomFlags = roomFlags,
                    roomName = roomName,
                    roomMaxOccupancy = roomMaxOccupancy,
                    roomLastModified = LastModified,
                    //roomNotFound = roomNotFound,

                    facesID = facesID,
                    roomPicture = roomPicture,
                    roomArtist = roomArtist,
                    roomPassword = roomPassword,
                });

                dbContext.ExecStoredProcedure("EXEC Rooms.FlushExtendedRoomDetails",
                    new SqlParameter("@roomID", ID),
                    new SqlParameter("@hasUnsavedAuthorChanges", HasUnsavedAuthorChanges));

                if (HasUnsavedAuthorChanges)
                {
                    lock (_hotspots)
                    {
                        Core.Factories.RoomDataExts.WriteHotspots(dbContext, ID, _hotspots);
                    }

                    lock (_pictures)
                    {
                        Core.Factories.RoomDataExts.WritePictures(dbContext, ID, _pictures);
                    }
                }

                lock (_looseProps)
                {
                    Core.Factories.RoomDataExts.WriteLooseProps(dbContext, ID, _looseProps);
                }

                lock (_drawCmds)
                {
                    Core.Factories.RoomDataExts.WriteDrawCmds(dbContext, ID, _drawCmds);
                }

                if (dbContext.HasUnsavedChanges())
                {
                    dbContext.SaveChanges();
                }

                AcceptChanges();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                AcceptChanges();
            }

            HasUnsavedAuthorChanges = false;
        }

        public void Delete(ThePalaceEntities dbContext)
        {
            lock (ServerState.roomsCache)
            {
                var room = dbContext.Rooms
                    .Where(r => r.RoomId == ID)
                    .SingleOrDefault();
                dbContext.Rooms.Remove(room);

                var roomData = dbContext.RoomData
                    .Where(r => r.RoomId == ID)
                    .SingleOrDefault();
                dbContext.RoomData.Remove(roomData);

                dbContext.SaveChanges();

                dbContext.ExecStoredProcedure("EXEC Rooms.FlushExtendedRoomDetails",
                    new SqlParameter("@roomID", ID),
                    new SqlParameter("@hasUnsavedAuthorChanges", HasUnsavedAuthorChanges));

                ServerState.roomsCache.Remove(ID);
            }
        }
    }
}
