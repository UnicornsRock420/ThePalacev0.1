﻿namespace ThePalace.Core.Database.Core.Model
{
    public partial class RoomData
    {
        public short RoomId { get; set; }
        public int FacesId { get; set; }
        public string PictureName { get; set; }
        public string ArtistName { get; set; }
        public string Password { get; set; }
    }
}