namespace EvoClient.Utils
{
    internal static class ArrayExtensions
    {
        private static readonly Random _random = new();

        public static T GetRandom<T>(this T[] array)
        {
            int el = _random.Next(0, array.Length);
            return array[el];
        }
    }
}
