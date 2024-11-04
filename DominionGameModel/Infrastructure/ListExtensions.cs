namespace GameModel.Infrastructure
{
    public static class ListExtensions
    {
        private static Random _random = new Random();
        /// <summary>
        /// Shuffles the element order of the specified list.
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static T? Pop<T>(this IList<T> list)
        {
            T? first = list.FirstOrDefault();
            if (first != null)
            {
                list.Remove(first);
            }
            return first;
        }

        public static List<T> Get<T>(this IList<T> list, int count)
        {
            List<T> result = new();
            for (int i = 0; i < count; i++)
            {
                var el = list.Pop();
                if (el != null)
                {
                    result.Add(el);
                }
            }

            return result;
        }
    }
}
