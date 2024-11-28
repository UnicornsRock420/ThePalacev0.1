using System.Threading.Tasks;

namespace System
{
    public static class TaskExts
    {
        public static class Types
        {
            public static readonly Type Task = typeof(Task);
        }

        //static TaskExts() { }

        public static void Forget(this Task task, int delay = 500) =>
            Task.Delay(delay).ConfigureAwait(false);
    }
}
