using ThePalace.Core.Client.Core.Models;
using ThePalace.Core.Factories;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.PropBags
{
    public partial class GUI : FormBase
    {
        private static readonly List<AssetRec> _assets = new List<AssetRec>();

        //private static void Convert(string filePath, int width, int height, byte[] data)
        //{
        //    var pixelIndex = 0;
        //    var counter = 0;
        //    var ofst = 0;

        //    using (var img = new Bitmap(width, height))
        //    {
        //        for (var y = height - 1; ofst < data.Length && y >= 0; y--)
        //        {
        //            for (var x = width; ofst < data.Length && x > 0;)
        //            {
        //                var cb = (byte)(data[ofst++] & 0xFF);
        //                var pc = (byte)(cb & 0x0F);
        //                var mc = (byte)(cb >> 4);

        //                x -= mc + pc;

        //                if (x < 0 || counter++ > 6000)
        //                {
        //                    throw new Exception("Bad Prop");
        //                }

        //                pixelIndex += mc;

        //                while (pc-- > 0 && ofst < data.Length)
        //                {
        //                    cb = (byte)(data[ofst++] & 0xFF);

        //                    img.SetPixel(pixelIndex % width, pixelIndex / height, Color.FromArgb((Int32)AssetConstants.PalacePalette[cb]));

        //                    pixelIndex++;
        //                }
        //            }
        //        }

        //        img.Save(filePath);
        //    }
        //}

        public GUI()
        {
            InitializeComponent();
        }

        private void loadPRPs_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = @"PRP Files|*.prp|PIDS Files|*.pids";
            openFileDialog.ShowDialog();

            if ((openFileDialog.FileNames?.Length ?? -1) < 1) return;

            foreach (var filePath in openFileDialog.FileNames)
            {
                var fileExt = Path.GetExtension(filePath).ToUpperInvariant();

                if (fileExt.EndsWith("PRP"))
                {
                    try
                    {
                        using (var fileReader = new PropPRPStream())
                            if (fileReader.Open(filePath))
                            {
                                fileReader.Read(out List<AssetRec> _assets);

                                foreach (var _asset in _assets)
                                {
                                    _asset.md5 = _asset.data.ComputeMd5();

                                    var skip = GUI._assets.Any(asset =>
                                        asset.assetSpec.id == _asset.assetSpec.id ||
                                        (asset.assetSpec.crc == _asset.assetSpec.crc &&
                                        asset.md5 == _asset.md5));

                                    if (!skip)
                                        GUI._assets.Add(_asset);
                                }
                            }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else if (fileExt.EndsWith("PIDS"))
                {
                    var fileDir = Path.GetDirectoryName(filePath);
                    var fileName2 = Path.GetFileName(filePath)
                        .ToUpperInvariant()
                        .Replace("PIDS", "PROPS");
                    var filePath2 = Path.Combine(fileDir, fileName2);

                    if (File.Exists(filePath) &&
                        File.Exists(filePath2))
                        try
                        {
                            using (var fileReader = new PropPIDSStream())
                            using (var fileReader2 = new PropPROPSStream())
                                if (fileReader.Open(filePath) &&
                                    fileReader2.Open(filePath2))
                                {
                                    fileReader.Read(fileReader2, out List<AssetRec> _assets);

                                    foreach (var _asset in _assets)
                                    {
                                        _asset.md5 = _asset.data.ComputeMd5();

                                        var skip = GUI._assets.Any(asset =>
                                            asset.assetSpec.id == _asset.assetSpec.id ||
                                            (asset.assetSpec.crc == _asset.assetSpec.crc &&
                                            asset.md5 == _asset.md5));

                                        if (!skip)
                                            GUI._assets.Add(_asset);
                                    }
                                }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                }
            }

            this.Text = $"Prop Extractor: {_assets.Count} Loaded";
        }

        private void exportPRP_Click(object sender, EventArgs e)
        {
            saveFileDialog.Filter = @"PRP Files|*.prp|PIDS Files|*.pids";
            saveFileDialog.ShowDialog();

            if ((saveFileDialog.FileName?.Length ?? -1) < 1) return;

            var filePath = saveFileDialog.FileName;
            var fileExt = Path.GetExtension(filePath).ToUpperInvariant();

            if (fileExt.EndsWith("PRP"))
            {
                try
                {
                    using (var fileReader = new PropPRPStream())
                        if (fileReader.Open(filePath, true))
                            fileReader.Write(_assets.ToArray());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else if (fileExt.EndsWith("PIDS"))
            {
                var fileDir = Path.GetDirectoryName(filePath);
                var fileName = Path.GetFileName(filePath).ToUpperInvariant();
                var fileName2 = fileName.Replace("PIDS", "PROPS");
                var filePath2 = Path.Combine(fileDir, fileName2);
                var fileName3 = fileName.Replace("PIDS", "FAVS");
                var filePath3 = Path.Combine(fileDir, fileName3);

                try
                {
                    using (var fileReader = new PropPIDSStream())
                    using (var fileReader2 = new PropPROPSStream())
                        if (fileReader.Open(filePath, true) &&
                            fileReader2.Open(filePath2, true))
                            fileReader.Write(fileReader2, _assets.ToArray());

                    File.Copy(filePath, filePath3, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void exportPropIDs_Click(object sender, EventArgs e)
        {
            saveFileDialog.Filter = @"TXT Files|*.txt";
            saveFileDialog.ShowDialog();

            using (var fileStream = new StreamWriter(saveFileDialog.FileName))
                foreach (var asset in _assets)
                {
                    fileStream.WriteLine("PropID: {0}", asset.assetSpec.id);
                }
        }

        private void exportPNGs_Click(object sender, EventArgs e)
        {
            saveFileDialog.Filter = @"PNG Files|*.png";
            saveFileDialog.ShowDialog();

            var dirPath = Path.GetDirectoryName(saveFileDialog.FileName);

            foreach (var asset in _assets)
            {
                var path = Path.Combine(dirPath, $"Prop_{asset.assetSpec.id}.png");

                try
                {
                    using (var img = AssetRec.Render(asset))
                    {
                        img.Save(path);
                    }

                    //Convert(path, 44, 44, asset.data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}: {1}", asset.assetSpec.id, ex.Message);
                }
            }
        }
    }
}
