using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using ThePalace.Core.Client.Core;
using ThePalace.Core.Client.Core.Constants;
using ThePalace.Core.Client.Core.Enums;
using ThePalace.Core.Client.Core.Models;
using ThePalace.Core.Client.Core.Utility;
using ThePalace.Core.Constants;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Desktop.Core.Models
{
    public sealed partial class SessionState : Disposable, IUISessionState
    {
        private static readonly List<ScreenLayers> _layerTypes = Enum.GetValues<ScreenLayers>().ToList();

        private DisposableDictionary<ScreenLayers, ScreenLayer> _uiLayers = new();
        public IReadOnlyDictionary<ScreenLayers, ScreenLayer> UILayers => this._uiLayers;

        private System.Timers.Timer _layerMessagesTimer = new(350);

        public void RefreshScreen(params ScreenLayers[] layers)
        {
            if (layers.Length > 0)
                this.RefreshLayers(layers);

            try
            {
                var isConnected = this.ConnectionState?.IsConnected ?? false;
                if (!isConnected)
                {
                    foreach (var layer in _layerTypes)
                    {
                        if (layer == ScreenLayers.Base) continue;

                        this._uiLayers[layer].Unload();
                    }

                    this._uiLayers[ScreenLayers.Base].Load(this, LayerLoadingTypes.Resource, "ThePalace.Core.Desktop.Core.Resources.backgrounds.aephixcorelogo.png");
                }
                else if (layers.Contains(ScreenLayers.Base))
                {
                    var filePath = null as string;

                    if (!string.IsNullOrWhiteSpace(this.MediaUrl) &&
                        !string.IsNullOrWhiteSpace(this.ServerName) &&
                        !string.IsNullOrWhiteSpace(this.RoomInfo.roomPicture))
                    {
                        var fileName = FilesystemConstants.REGEX_FILESYSTEMCHARS.Replace(this.RoomInfo.roomPicture, @"_");
                        filePath = Path.Combine(Environment.CurrentDirectory, "Media", fileName);

                        if (!File.Exists(filePath))
                        {
                            var _serverName = FilesystemConstants.REGEX_FILESYSTEMCHARS.Replace(this.ServerName, @" ").Trim();
                            filePath = Path.Combine(Environment.CurrentDirectory, "Media", _serverName, fileName);
                        }

                        if (File.Exists(filePath))
                            this._uiLayers[ScreenLayers.Base].Load(this, LayerLoadingTypes.Filesystem, filePath);
                    }

                    if (this.RoomInfo.roomPicture !=
                        this._uiLayers[ScreenLayers.Base].Image?.Tag?.ToString())
                        this._uiLayers[ScreenLayers.Base].Load(this, LayerLoadingTypes.Resource, "ThePalace.Core.Desktop.Core.Resources.backgrounds.clouds.jpg");
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"RefreshScreen.Base: {ex.Message}");
#endif
            }

            var imgScreen = GetControl("imgScreen") as PictureBox;
            if (imgScreen != null)
                try
                {
                    //var img = null as Bitmap;
                    //if (imgScreen.Image == null ||
                    //    imgScreen.Image.Width != this.ScreenWidth ||
                    //    imgScreen.Image.Height != this.ScreenHeight)
                    //{
                    //    try { imgScreen.Image?.Dispose(); } catch { }
                    var img = new Bitmap(this.ScreenWidth, this.ScreenHeight);
                    img.MakeTransparent(Color.Transparent);
                    //}

                    using (var g = Graphics.FromImage(img))
                    {
                        g.InterpolationMode = SettingsManager.Current.GetOption<InterpolationMode>(@"\GUI\General\" + nameof(InterpolationMode));
                        g.PixelOffsetMode = SettingsManager.Current.GetOption<PixelOffsetMode>(@"\GUI\General\" + nameof(PixelOffsetMode));
                        g.SmoothingMode = SettingsManager.Current.GetOption<SmoothingMode>(@"\GUI\General\" + nameof(SmoothingMode));

                        g.Clear(Color.Transparent);

                        foreach (var layer in _layerTypes)
                        {
                            if (!this._uiLayers[layer].Visible ||
                                this._uiLayers[layer].Opacity == 0F ||
                                this._uiLayers[layer].Image == null) continue;

                            lock (this._uiLayers[layer])
                            {
                                var imgAttributes = null as ImageAttributes;

                                if (this._uiLayers[layer].Opacity < 1.0F)
                                {
                                    var matrix = new ColorMatrix
                                    {
                                        Matrix33 = this._uiLayers[layer].Opacity,
                                    };

                                    imgAttributes = new();
                                    imgAttributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                                }

                                g.DrawImage(
                                    this._uiLayers[layer].Image,
                                    new Rectangle(
                                        0, 0,
                                        img.Width,
                                        img.Height),
                                    0, 0,
                                    this._uiLayers[layer].Image.Width,
                                    this._uiLayers[layer].Image.Height,
                                    GraphicsUnit.Pixel,
                                    imgAttributes);
                            }
                        }

                        g.Save();
                    }

                    imgScreen.Image = img;
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine(ex.Message);
#endif
                }
        }

        public void LayerVisibility(bool visible, params ScreenLayers[] layers)
        {
            foreach (var layer in layers)
            {
                if (layer == ScreenLayers.Base) continue;

                this._uiLayers[layer].Visible = visible;
            }
        }
        public void LayerOpacity(float opacity, params ScreenLayers[] layers)
        {
            foreach (var layer in layers)
            {
                if (layer == ScreenLayers.Base) continue;

                this._uiLayers[layer].Opacity = opacity;
                this._uiLayers[layer].Visible = opacity != 0F;
            }
        }

        private void RefreshLayers(params ScreenLayers[] layers)
        {
            if (!this.Visible ||
                this.ScreenWidth < 1 ||
                this.ScreenHeight < 1) return;

            try
            {
                foreach (var layer in layers)
                {
                    lock (this._uiLayers[layer])
                    {
                        if (layer == ScreenLayers.Base) continue;

                        switch (layer)
                        {
                            case ScreenLayers.DimRoom:
                                if (this._uiLayers[layer].Opacity == 1F)
                                {
                                    this._uiLayers[layer].Unload();

                                    break;
                                }
                                else if (this._uiLayers[layer].Image != null) break;

                                goto default;
                            default:
                                using (var g = this._uiLayers[layer].Initialize(this.ScreenWidth, this.ScreenHeight))
                                {
                                    if (!this._uiLayers[layer].Visible) continue;

                                    g.InterpolationMode = SettingsManager.Current.GetOption<InterpolationMode>(@"\GUI\General\" + nameof(InterpolationMode));
                                    g.PixelOffsetMode = SettingsManager.Current.GetOption<PixelOffsetMode>(@"\GUI\General\" + nameof(PixelOffsetMode));
                                    g.SmoothingMode = SettingsManager.Current.GetOption<SmoothingMode>(@"\GUI\General\" + nameof(SmoothingMode));

                                    switch (layer)
                                    {
                                        case ScreenLayers.LooseProp: this.ScreenLayer_LooseProp(g); break;
                                        case ScreenLayers.SpotImage: this.ScreenLayer_SpotImage(g); break;
                                        case ScreenLayers.BottomPaint: this.ScreenLayer_BottomPaint(g); break;
                                        case ScreenLayers.SpotNametag: this.ScreenLayer_SpotNametag(g); break;
                                        case ScreenLayers.UserProp: this.ScreenLayer_UserProp(g); break;
                                        case ScreenLayers.UserNametag: this.ScreenLayer_UserNametag(g); break;
                                        case ScreenLayers.ScriptedImage: this.ScreenLayer_ScriptedImage(g); break;
                                        case ScreenLayers.ScriptedText: this.ScreenLayer_ScriptedText(g); break;
                                        case ScreenLayers.SpotBorder: this.ScreenLayer_SpotBorder(g); break;
                                        case ScreenLayers.TopPaint: this.ScreenLayer_TopPaint(g); break;
                                        case ScreenLayers.DimRoom: this.ScreenLayer_DimRoom(g); break;
                                        case ScreenLayers.Messages: this.ScreenLayer_Messages(g); break;
                                    }

                                    g.Save();
                                }

                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
            }
        }
        private void ScreenLayer_LooseProp(Graphics g)
        {
            var looseProps = this.RoomInfo?.LooseProps
                ?.ToList() ?? new();
            if (looseProps.Count > 0)
                foreach (var looseProp in looseProps)
                {
                    var prop = AssetsManager.Current.GetAsset(this, looseProp.assetSpec);
                    if (prop == null) continue;

                    if (prop.Image == null)
                        try
                        {
                            prop.Image = AssetRec.Render(prop);
                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            Debug.WriteLine(ex.Message);
#endif

                            continue;
                        }

                    if (prop.Image == null) continue;

                    //var x = looseProp.loc.h - (prop.Image.Width / 2);
                    //var y = looseProp.loc.v - (prop.Image.Height / 2);
                    var x = looseProp.loc.h;
                    var y = looseProp.loc.v;

                    //if (x < 0 ||
                    //    y < 0) continue;

                    if (prop.Image != null)
                        g.DrawImage(
                        prop.Image,
                        new Rectangle(
                            x,
                            y,
                            prop.Image.Width,
                            prop.Image.Height),
                        0, 0,
                        prop.Image.Width,
                        prop.Image.Height,
                        GraphicsUnit.Pixel);
                }
        }
        private void ScreenLayer_SpotImage(Graphics g)
        {
            if ((this.RoomInfo?.HotSpots?.Count ?? 0) > 0)
                foreach (var spot in this.RoomInfo.HotSpots)
                {
                    var nbrStates = spot.States?.Count ?? 0;
                    if (nbrStates < 1 ||
                        spot.state < 0 ||
                        spot.state >= nbrStates) continue;

                    var state = spot.States[spot.state];
                    if (state == null ||
                        state.pictID < 1) continue;

                    var pictName = this.RoomInfo?.Pictures
                        ?.Where(p => p.picID == state.pictID)
                        ?.Select(p => p.name)
                        ?.FirstOrDefault();
                    if (pictName == null) continue;

                    var _pictName = FilesystemConstants.REGEX_FILESYSTEMCHARS.Replace(pictName, @"_");
                    var filePath = Path.Combine(Environment.CurrentDirectory, "Media", _pictName);

                    if (!File.Exists(filePath))
                    {
                        var _serverName = FilesystemConstants.REGEX_FILESYSTEMCHARS.Replace(this.ServerName, @" ").Trim();
                        filePath = Path.Combine(Environment.CurrentDirectory, "Media", _serverName, _pictName);
                    }

                    if (!File.Exists(filePath)) continue;

                    using (var pict = new Bitmap(filePath))
                        g.DrawImage(
                            pict,
                            new Rectangle(
                                spot.loc.h - state.picLoc.h - (pict.Width / 2),
                                spot.loc.v - state.picLoc.v - (pict.Height / 2),
                                pict.Width,
                                pict.Height),
                            0, 0,
                            pict.Width,
                            pict.Height,
                            GraphicsUnit.Pixel);
                }
        }
        private void ScreenLayer_BottomPaint(Graphics g) =>
            ScreenLayer_Paint(g, false);
        private void ScreenLayer_SpotNametag(Graphics g)
        {
            var spots = this.RoomInfo?.HotSpots
                ?.Where(h => ((HotspotFlags)h.flags & HotspotFlags.HS_ShowName) == HotspotFlags.HS_ShowName)
                ?.ToList() ?? new();
            if (spots.Count > 0)
            {
                var padding = 2;

                using (var font = new Font("Arial", 11))
                {
                    foreach (var spot in spots)
                    {
                        if (!string.IsNullOrWhiteSpace(spot.name)) continue;

                        var textSize = TextRenderer.MeasureText(spot.name, font);
                        var halfNameTagWidth = textSize.Width / 2;
                        var halfNameTagHeight = textSize.Height / 2;

                        var x = spot.loc.h - halfNameTagWidth - padding * 2;
                        var y = spot.loc.v + halfNameTagHeight * 3 - padding * 2;

                        //if (x < 0 ||
                        //    y < 0) continue;

                        g.FillRectangle(
                            Brushes.White,
                            new Rectangle(
                                x,
                                y,
                                textSize.Width + padding,
                                textSize.Height + padding));

                        g.DrawString(
                            spot.name,
                            font,
                            Brushes.Black,
                            x, y);
                    }
                }
            }
        }
        private void ScreenLayer_UserProp(Graphics g)
        {
            var halfPropWidth = AssetConstants.DefaultPropWidth / 2;
            var halfPropHeight = AssetConstants.DefaultPropHeight / 2;

            var users = null as List<UserRec>;
            lock (this.RoomUsersInfo)
                users = this.RoomUsersInfo.Values
                    .Where(u =>
                        !(u.userID < 1 ||
                        u.roomPos == null))
                    .ToList();
            if (users.Count > 0)
                foreach (var u in users)
                {
                    var x = u.roomPos.h - halfPropWidth;
                    var y = u.roomPos.v - halfPropHeight;

                    if (x < -halfPropWidth) x = -halfPropWidth;
                    else if (x > this.ScreenWidth + halfPropWidth) x = this.ScreenWidth + halfPropWidth;

                    if (y < -halfPropHeight) y = -halfPropHeight;
                    else if (y > this.ScreenHeight + halfPropHeight) y = this.ScreenHeight + halfPropHeight;

                    //if (x < 0 ||
                    //    y < 0) continue;

                    var animatedProps = new List<AssetRec>();
                    var stillProps = new List<AssetRec>();
                    var hasPalindromeProp = false;
                    var hasAnimatedProp = false;
                    var hasHeadProp = false;

                    var assetSpecs = u.assetSpec?.ToList() ?? new();
                    if (assetSpecs.Count > 0)
                        foreach (var assetSpec in assetSpecs)
                        {
                            var asset = AssetsManager.Current.GetAsset(this, assetSpec);
                            if (asset == null) continue;

                            hasPalindromeProp |= asset.IsPalindrome;
                            hasAnimatedProp |= asset.IsAnimate;
                            hasHeadProp |= asset.IsHead;

                            if (asset.Image == null)
                                try
                                {
                                    asset.Image = AssetRec.Render(asset);
                                }
                                catch (Exception ex)
                                {
#if DEBUG
                                    Debug.WriteLine(ex.Message);
#endif
                                }

                            if (asset.Image != null)
                            {
                                if (asset.IsAnimate)
                                    animatedProps.Add(asset);
                                else
                                    stillProps.Add(asset);
                            }
                        }

                    if (!hasHeadProp)
                    {
                        var index = (uint)0;
                        index += (uint)(u.faceNbr % UIConstants.MaxNbrFaces);
                        index += (uint)(u.colorNbr % UIConstants.MaxNbrColors) << 8;
                        var smileyFace = AssetsManager.Current.SmileyFaces[index];

                        g.DrawImage(
                            smileyFace,
                            new Rectangle(
                                x, y,
                                smileyFace.Width,
                                smileyFace.Height),
                            0, 0,
                            smileyFace.Width,
                            smileyFace.Height,
                            GraphicsUnit.Pixel);
                    }

                    foreach (var prop in stillProps)
                        if (prop.Image != null)
                        {
                            var imgAttributes = null as ImageAttributes;

                            if (prop.IsGhost)
                            {
                                var matrix = new ColorMatrix
                                {
                                    Matrix33 = 0.5F,
                                };

                                imgAttributes = new();
                                imgAttributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                            }

                            g.DrawImage(
                                prop.Image,
                                new Rectangle(
                                    x + prop.Offset.h,
                                    y + prop.Offset.v,
                                    prop.Image.Width,
                                    prop.Image.Height),
                                0, 0,
                                prop.Image.Width,
                                prop.Image.Height,
                                GraphicsUnit.Pixel,
                                imgAttributes);
                        }
                }
        }
        private void ScreenLayer_UserNametag(Graphics g)
        {
            var halfPropWidth = AssetConstants.DefaultPropWidth / 2;
            var halfPropHeight = AssetConstants.DefaultPropHeight / 2;

            var font = new Font("Arial", 11);
            var padding = 2;

            var users = null as List<UserRec>;
            lock (this.RoomUsersInfo)
                users = this.RoomUsersInfo.Values.ToList();

            if ((users?.Count ?? 0) > 0)
                foreach (var u in users)
                {
                    if (u.userID < 1 ||
                        u.roomPos == null) continue;

                    var colour = UIConstants.NbrToColor(u.colorNbr);
                    using (var colourBrush = new SolidBrush(colour))
                    {
                        var textSize = TextRenderer.MeasureText(u.name, font);
                        var halfNameTagWidth = textSize.Width / 2;
                        var halfNameTagHeight = textSize.Height / 2;

                        var x = u.roomPos.h - halfNameTagWidth - padding * 2;
                        var y = u.roomPos.v + halfNameTagHeight * 3 - padding * 2;

                        if (x < -halfPropWidth) x = -halfPropWidth;
                        else if (x > this.ScreenWidth + halfPropWidth) x = this.ScreenWidth + halfPropWidth;

                        if (y < -halfPropHeight) y = -halfPropHeight;
                        else if (y > this.ScreenHeight + halfPropHeight) y = this.ScreenHeight + halfPropHeight;

                        //if (x < 0 ||
                        //    y < 0) continue;

                        g.FillRectangle(
                            Brushes.Black,
                            new Rectangle(
                                x,
                                y,
                                textSize.Width + padding,
                                textSize.Height + padding));

                        g.DrawString(
                            u.name,
                            font,
                            colourBrush,
                            x, y);
                    }
                }
        }
        private void ScreenLayer_ScriptedImage(Graphics g)
        {
        }
        private void ScreenLayer_ScriptedText(Graphics g)
        {
        }
        private void ScreenLayer_SpotBorder(Graphics g)
        {
            var spots = this.RoomInfo?.HotSpots
                ?.Where(h => ((HotspotFlags)h.flags & HotspotFlags.HS_ShowFrame) == HotspotFlags.HS_ShowFrame)
                ?.ToList() ?? new();
            if (spots.Count > 0)
            {
                var pen = Pens.Black;
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                foreach (var h in spots)
                    g.DrawPolygon(pen, h.Vortexes
                        .Select(v => new Point(v.h, v.v))
                        .ToArray());
            }
        }
        private void ScreenLayer_TopPaint(Graphics g) =>
            ScreenLayer_Paint(g, true);
        private void ScreenLayer_DimRoom(Graphics g)
        {
            g.FillRectangle(Brushes.Black, 0, 0, this.ScreenWidth, this.ScreenHeight);
        }
        private void ScreenLayer_Messages(Graphics g)
        {
            var users = null as List<UserRec>;
            lock (this.RoomUsersInfo)
                users = this.RoomUsersInfo.Values.ToList();

            if ((users?.Count ?? 0) > 0)
                foreach (var u in users)
                {
                    if (u.roomPos == null) continue;

                    var queue = u.Extended["MessageQueue"] as DisposableQueue<MsgBubble>;
                    if (queue == null) continue;

                    var msg = u.Extended["CurrentMessage"] as MsgBubble;
                    if (msg != null)
                    {
                        if ((msg.Type != BubbleTypes.Sticky || queue.Count > 0) &&
                            DateTime.Now.Subtract(msg.Accessed).TotalMilliseconds >= msg.Duration)
                        {
                            u.Extended["CurrentMessage"] = null;
                            msg.Dispose();
                        }
                    }

                    if (msg == null)
                    {
                        if (queue.Count < 1) continue;
                        else queue.TryDequeue(out msg);

                        if (msg == null) continue;

                        u.Extended["CurrentMessage"] = msg;
                    }

                    if (msg == null) continue;
                    else if (!msg.Visible) continue;

                    var loc = msg.Origin;

                    //var halfPropWidth = AssetConstants.DefaultPropWidth / 2;
                    //var halfPropHeight = AssetConstants.DefaultPropHeight / 2;

                    //if (x < -halfPropWidth) x = (short)-halfPropWidth;
                    //else if (x > (sessionState.ScreenWidth + halfPropWidth)) x = (short)(sessionState.ScreenWidth + halfPropWidth);

                    //if (y < -halfPropHeight) y = (short)-halfPropHeight;
                    //else if (y > (sessionState.ScreenHeight + halfPropHeight)) y = (short)(sessionState.ScreenHeight + halfPropHeight);

                    //if (x < 0 ||
                    //    y < 0) continue;

                    var image = msg.Render();

                    // TODO:
                }
        }
        private void ScreenLayer_Paint(Graphics g, bool layer)
        {
            var helper = new GraphicsHelper(g);

            var drawCmds = this.RoomInfo?.DrawCmds
                ?.Where(d => d.layer == layer)
                ?.Where(d => (d.Points?.Count ?? 0) > 0)
                ?.ToList() ?? new();
            if (drawCmds.Count > 0)
                foreach (var dc in drawCmds)
                    switch (dc.type)
                    {
                        case DrawCmdTypes.DC_Path:
                            {
                                var colour = Color.FromArgb(255, dc.red, dc.green, dc.blue);
                                using (var penColour = new Pen(colour, dc.penSize))
                                using (var brushColour = new SolidBrush(colour))
                                {
                                    penColour.StartCap = LineCap.Round;
                                    penColour.EndCap = LineCap.Round;

                                    helper.SetBrush(brushColour);
                                    helper.SetPen(penColour);

                                    if (dc.filled)
                                        helper.BeginPath();

                                    var x = dc.pos.h;
                                    var y = dc.pos.v;

                                    helper.MoveTo(x, y);

                                    foreach (var p in dc.Points)
                                    {
                                        x += p.h;
                                        y += p.v;

                                        helper.LineTo(x, y);
                                    }

                                    if (dc.filled)
                                        helper.Fill();

                                    helper.Stroke();
                                }
                            }

                            break;
                        case DrawCmdTypes.DC_Ellipse:
                            {
                                //var colour = Color.FromArgb(255, dc.red, dc.green, dc.blue);
                                //using (var penColour = new Pen(colour, dc.penSize))
                                //using (var brushColour = new SolidBrush(colour))
                                //{
                                //    penColour.StartCap = LineCap.Round;
                                //    penColour.EndCap = LineCap.Round;

                                //    helper.SetBrush(brushColour);
                                //    helper.SetPen(penColour);

                                //    helper.BeginPath();

                                //    helper.DrawEllipse(dc.Rect);

                                //    if (dc.filled)
                                //        helper.Fill();

                                //    helper.Stroke();
                                //}

                                throw new NotImplementedException(nameof(DrawCmdTypes.DC_Ellipse));
                            }

                            break;
                        case DrawCmdTypes.DC_Text:
                            {
                                //var colour = Color.FromArgb(255, dc.red, dc.green, dc.blue);
                                //using (var penColour = new Pen(colour, dc.penSize))
                                //using (var brushColour = new SolidBrush(colour))
                                //{
                                //    penColour.StartCap = LineCap.Round;
                                //    penColour.EndCap = LineCap.Round;

                                //    helper.SetBrush(brushColour);
                                //    helper.SetPen(penColour);

                                //    helper.BeginPath();

                                //    helper.DrawText(dc.text, dc.pos.h, dc.pos.v);

                                //    if (dc.filled)
                                //        helper.Fill();

                                //    helper.Stroke();
                                //}

                                throw new NotImplementedException(nameof(DrawCmdTypes.DC_Text));
                            }

                            break;
                        case DrawCmdTypes.DC_Shape:
                            {
                                //var colour = Color.FromArgb(255, dc.red, dc.green, dc.blue);
                                //using (var penColour = new Pen(colour, dc.penSize))
                                //using (var brushColour = new SolidBrush(colour))
                                //{
                                //    penColour.StartCap = LineCap.Round;
                                //    penColour.EndCap = LineCap.Round;

                                //    helper.SetBrush(brushColour);
                                //    helper.SetPen(penColour);

                                //    helper.BeginPath();

                                //    // TODO:

                                //    if (dc.filled)
                                //        helper.Fill();

                                //    helper.Stroke();
                                //}

                                throw new NotImplementedException(nameof(DrawCmdTypes.DC_Shape));
                            }

                            break;
                    }
        }
    }
}
