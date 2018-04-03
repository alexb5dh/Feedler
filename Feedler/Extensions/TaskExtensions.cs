using System.Threading.Tasks;

namespace Feedler.Extensions
{
    public static class TaskExtensions
    {
        public static T Await<T>(this Task<T> task) => task.GetAwaiter().GetResult();

        public static void Await(this Task task) => task.GetAwaiter().GetResult();
    }
}