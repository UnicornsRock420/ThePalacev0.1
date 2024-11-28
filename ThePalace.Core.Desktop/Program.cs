using ThePalace.Core.Client.Core;
using ThePalace.Core.Client.Core.Enums;
using ThePalace.Core.Client.Core.Interfaces;
using ThePalace.Core.Client.Core.Models;
using ThePalace.Core.Desktop.Core;
using ThePalace.Core.Desktop.Core.Interfaces;
using ThePalace.Core.Desktop.Core.Models;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Desktop
{
    public sealed partial class Program : FormBase, IFormDialog
    {
        public Program()
        {
            ThreadManager.Current
                .Initialize()
                .Enqueue(ThreadQueues.Core, args =>
                {
                    AssetsManager.Current.ResourceType = typeof(FormsManager);
                    AssetsManager.Current.LoadSmilies("palaceV4.png");
                    PluginManager.Current.LoadPlugins();

                    var feature = null as IFeature;
                    var types = PluginManager.Current.GetTypes()
                        .Where(p => p?.FullName?.Contains("Features") == true)
                        .ToList();
                    foreach (var type in types)
                    {
                        try { feature = type.GetInstance<IFeature>(); } catch { continue; }
                        if (feature == null) continue;

                        if (feature.Features.Contains(FeatureTypes.CORE))
                            ThreadManager.Current.RegisterFeature(ThreadQueues.Core, feature);

                        if (feature.Features.Contains(FeatureTypes.GUI))
                            ThreadManager.Current.RegisterFeature(ThreadQueues.GUI, feature);

                        if (feature.Features.Contains(FeatureTypes.MEDIA))
                            ThreadManager.Current.RegisterFeature(ThreadQueues.Media, feature);

                        if (feature.Features.Contains(FeatureTypes.NETWORK))
                            ThreadManager.Current.RegisterFeature(ThreadQueues.Network, feature);
                    }

                    SettingsManager.Current
                        .Initialize()
                        .Load(typeof(FormsManager).Assembly);

                    var sessionState = SessionManager.Current.CreateSession<SessionState>() as IUISessionState;
                    sessionState.RegisterForm(nameof(Program), this);

                    ThreadManager.Current.Threads[ThreadQueues.GUI].Providers
                        .ForEach(f => f?.Provide(sessionState));

                    return null;
                });
        }
        ~Program() =>
            this.Dispose(false);

        public new void Dispose()
        {
            base.Dispose();

            ThreadManager.Current?.Dispose();
        }

        [STAThread]
        internal static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ApplicationConfiguration.Initialize();

            Cipher.InitializeTable();

            NetworkManager.Current.ProtocolsType = typeof(ThreadManager);
            NetworkManager.Current.SessionStateType = typeof(SessionState);
            NetworkManager.Current.ConnectionStateType = typeof(PalaceConnectionState);

            var form = FormsManager.Current.CreateForm<Program>(
                new FormCfg
                {
                    WindowState = FormWindowState.Minimized,
                    AutoScaleMode = AutoScaleMode.None,
                    AutoScaleDimensions = new Size(0, 0),
                    Margin = new Padding(0, 0, 0, 0),
                    Size = new Size(0, 0),
                    Visible = false,
                });
            form.FormClosed += new FormClosedEventHandler((sender, e) => ThreadManager.Current?.Dispose());

            Application.Run(FormsManager.Current);
        }
    }
}
