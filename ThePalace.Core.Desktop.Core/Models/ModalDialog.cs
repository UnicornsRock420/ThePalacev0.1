using ThePalace.Core.Client.Core.Models;
using ThePalace.Core.Desktop.Core.Interfaces;

namespace ThePalace.Core.Desktop.Core.Models
{
    public class ModalDialog<T> : FormBase, IFormResult<T>
    {
        public ModalDialog() { }

        public T Result { get; set; }
    }
}
