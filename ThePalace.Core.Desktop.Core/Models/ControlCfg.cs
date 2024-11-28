using System.Drawing;
using System.Windows.Forms;

namespace ThePalace.Core.Desktop.Core.Models
{
    public delegate EventHandler Event(object sender, EventArgs e);

    public class ControlCfg
    {
        public int TabIndex { get; set; }
        public string Text { get; set; }
        public Size Size { get; set; }
        public Padding Margin { get; set; }
        public Padding Padding { get; set; }
        public Point Location { get; set; }
        public Color BackColor { get; set; }
        public Color ForeColor { get; set; }
        public bool Visible { get; set; } = true;
        public bool TabStop { get; set; } = false;

        public BorderStyle BorderStyle { get; set; }

        public bool UseVisualStyleBackColor { get; set; } = true;

        public bool Multiline { get; set; }
        public int MaxLength { get; set; }
        public int Value { get; set; }

        public EventHandler Click { get; set; }
        public ScrollEventHandler Scroll { get; set; }
    }
}
