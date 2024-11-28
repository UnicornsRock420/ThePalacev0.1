﻿namespace ThePalace.Core.Database.Core.Model
{
    public partial class Rooms1
    {
        public int UserId { get; set; }
        public short RoomId { get; set; }
        public string Name { get; set; }
        public int Flags { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? LastModified { get; set; }
        public short MaxOccupancy { get; set; }
    }
}