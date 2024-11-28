using ThePalace.Core.Client.Core.Models;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Desktop.MsgBubble
{
    public partial class Program : FormBase
    {
        private Client.Core.Models.MsgBubble msgBubble = null;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Program());
        }

        public Program()
        {
            InitializeComponent();

            imgScreen_Click(null, null);
        }

        private void imgScreen_Click(object sender, EventArgs e)
        {
            //var types = new string[] { "", "!", ":", "^" };
            //var type = types[RndGenerator.Next(types.Length)];
            var type = ":";
            var count = RndGenerator.Next(15, 45);
            var words = new List<string>();
            for (var j = 0; j < count; j++)
                words.Add("test");

            msgBubble = new Client.Core.Models.MsgBubble(1, $"{type}{words.Join(" ")}");

            imgScreen.Image = msgBubble.Render();
        }
    }
}
