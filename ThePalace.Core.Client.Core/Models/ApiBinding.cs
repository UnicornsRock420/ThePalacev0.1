using System;

namespace ThePalace.Core.Client.Core.Models
{
    public sealed class ApiBinding
    {
        public string Name { get; set; } = null;
        public EventHandler Binding { get; set; } = null;
    }
}
