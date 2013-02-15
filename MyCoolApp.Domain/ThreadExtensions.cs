using System.Threading;
using System.Threading.Tasks;

namespace MyCoolApp.Domain
{
    public static class Sleeper
    {
        public static Task SleepAsync(int millisecondsTimeout)
        {
            TaskCompletionSource<bool> tcs = null;
            var t = new Timer(unusedState => tcs.TrySetResult(true), null, -1, -1);
            tcs = new TaskCompletionSource<bool>(t);
            t.Change(millisecondsTimeout, -1);
            return tcs.Task;
        }
    }
}