namespace GameModel.Infrastructure
{
    public class Shuffler
    {
        private Random _random;

        public Shuffler(int? seed = null)
        {
            if(seed == null)
            {
                _random = new Random();
            }
            else
            {
                _random = new Random(seed.Value);
            }
        }

        /// <summary>
        /// Shuffles the element order of the specified list.
        /// </summary>
        public IList<T> Shuffle<T>(IList<T> list)
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

            return list;
        }
    }
}
