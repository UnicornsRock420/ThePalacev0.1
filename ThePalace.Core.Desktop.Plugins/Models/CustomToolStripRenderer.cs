using System.Timers;
using ThePalace.Core.Client.Core;
using ThePalace.Core.Client.Core.Models.Ribbon;

namespace ThePalace.Core.Desktop.Plugins.Models
{
    public sealed class CustomToolStripRenderer : ToolStripProfessionalRenderer
    {
        private volatile System.Timers.Timer _timer = new(250);
        private volatile ToolStripItem _item1 = null;
        private volatile ItemBase _item2 = null;

        public CustomToolStripRenderer()
        {
            _timer.Elapsed += new ElapsedEventHandler(
                (sender, e) =>
                {
                    if (_item1 != null &&
                        _item2?.HoverFrames != null)
                        try
                        {
                            _item1.Image = _item2.NextFrame();
                        }
                        catch
                        {
                            _timer.Stop();

                            _item1 = null;
                            _item2 = null;
                        }
                });
        }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderButtonBackground(e);

            var ribbonItem = SettingsManager.SystemSettings.Ribbon[e.Item.Name];
            if (ribbonItem?.HoverFrames != null)
            {
                if (!e.Item.Selected)
                {
                    _timer.Stop();

                    _item1 = null;
                    _item2 = null;
                }
                else
                {
                    _item1 = e.Item;
                    _item2 = ribbonItem;
                    _item2.ResetFrames();

                    _timer.Start();
                }
            }
        }
    }
}
