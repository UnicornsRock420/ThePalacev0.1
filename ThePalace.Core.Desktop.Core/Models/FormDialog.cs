using System.Windows.Forms;
using ThePalace.Core.Client.Core.Models;
using ThePalace.Core.Desktop.Core.Interfaces;

namespace ThePalace.Core.Desktop.Core.Models
{
    public class FormDialog : FormBase, IFormDialog
    {
        private const int WM_PARENTNOTIFY = 0x0210;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_PARENTNOTIFY &&
                !Focused) Activate();
            base.WndProc(ref m);
        }

        public FormDialog() { }
    }
}
