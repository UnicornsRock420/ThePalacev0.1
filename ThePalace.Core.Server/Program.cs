using ThePalace.Core.Server.Core;

namespace ThePalace.Core.Server
{
    public class Program
    {
        public static int Main(string[] args)
        {
            ServerState.Initialize();

            return 0;
        }
    }
}
