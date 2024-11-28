using System.Drawing;

namespace ThePalace.Core.Client.Core.Models.Ribbon
{
    public abstract class StandardItem : ItemBase
    {
        public string Icon { get; set; }
        public Bitmap Image { get; set; }
    }
}
