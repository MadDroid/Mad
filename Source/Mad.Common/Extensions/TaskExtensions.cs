using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mad.Common.Extensions
{
    public static class TaskExtensions
    {
        public static void FireAndForget(this Task task) => _ = task;

        public static ConfiguredTaskAwaitable<T> Configure<T>(this Task<T> task, bool continueOnCapturedContext = false)
            => task.ConfigureAwait(continueOnCapturedContext);

        public static ConfiguredTaskAwaitable Configure(this Task task, bool continueOnCapturedContext = false)
            => task.ConfigureAwait(continueOnCapturedContext);
    }
}
