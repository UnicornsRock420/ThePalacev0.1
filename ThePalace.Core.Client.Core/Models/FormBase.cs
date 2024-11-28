using System.Windows.Forms;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Client.Core.Models
{
    public class FormBase : Form
    {
        public IUISessionState SessionState { get; set; } = null;

        public FormBase() { }
        ~FormBase() =>
            base.Dispose(false);

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (HotKeyManager.Current.Invoke(SessionState, keyData, this))
                return true;
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
