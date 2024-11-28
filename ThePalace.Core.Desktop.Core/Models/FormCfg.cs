using System.Drawing;
using System.Windows.Forms;

namespace ThePalace.Core.Desktop.Core.Models
{
    public sealed class FormCfg : ControlCfg
    {
        public SizeF AutoScaleDimensions { get; set; }
        public AutoScaleMode AutoScaleMode { get; set; }
        public FormStartPosition StartPosition { get; set; }
        public FormWindowState WindowState { get; set; }
        public bool Focus { get; set; }

        public EventHandler Activated { get; set; }
        public EventHandler Load { get; set; }
        public EventHandler Shown { get; set; }
        public EventHandler GotFocus { get; set; }
        public MouseEventHandler MouseMove { get; set; }
        public FormClosedEventHandler FormClosed { get; set; }
    }
}
