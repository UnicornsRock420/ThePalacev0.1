using System;
using System.Drawing;
using System.IO;
using ThePalace.Core.Client.Core.Enums;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Client.Core.Models
{
    public sealed class ScreenLayer : Disposable
    {
        public ScreenLayers Type { get; private set; }
        public Type ResourceType { get; set; }

        public int Width => Image?.Width ?? 0;
        public int Height => Image?.Height ?? 0;
        public Bitmap Image { get; set; } = null;
        public float Opacity { get; set; } = 1.0F;
        public bool Visible { get; set; } = true;

        public ScreenLayer(ScreenLayers type)
        {
            Type = type;
        }
        ~ScreenLayer() =>
            this.Dispose(false);

        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();

            Unload();
        }

        public void Unload()
        {
            try { this.Image?.Dispose(); this.Image = null; } catch { }
        }

        public Graphics Initialize(int width, int height)
        {
            if (this.Image != null &&
                (this.Image.Width != width ||
                    this.Image.Height != height))
                Unload();

            if (this.Image == null)
            {
                this.Image = new Bitmap(width, height);
                this.Image.MakeTransparent(Color.Transparent);
            }

            var g = Graphics.FromImage(this.Image);
            g.Clear(Color.Transparent);

            return g;
        }

        public void Load(IUISessionState sessionState, LayerLoadingTypes type, string srcPath, int width = 0, int height = 0)
        {
            if (srcPath == null) throw new ArgumentNullException(nameof(srcPath));

            lock (this)
            {
                var backgroundImage = null as Bitmap;

                switch (type)
                {
                    case LayerLoadingTypes.Filesystem:
                        if (File.Exists(srcPath))
                            try { backgroundImage = new Bitmap(srcPath); } catch { }

                        break;
                    case LayerLoadingTypes.Resource:
                        using (var stream = ResourceType
                            ?.Assembly
                            ?.GetManifestResourceStream(srcPath))
                        {
                            if (stream == null) return;

                            try { backgroundImage = new Bitmap(stream); } catch { }
                        }

                        break;
                }
                if (backgroundImage == null) return;

                Unload();

                this.Image = backgroundImage;
                this.Image.Tag = Path.GetFileName(srcPath);

                if (Type == ScreenLayers.Base)
                {
                    sessionState.ScreenWidth = backgroundImage.Width;
                    sessionState.ScreenHeight = backgroundImage.Height;
                }
            }
        }
    }
}
