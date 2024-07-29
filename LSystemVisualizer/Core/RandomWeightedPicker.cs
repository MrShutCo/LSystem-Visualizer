namespace ShutCo.UI.Core;

/// <summary>
    /// Pick up an element reflecting its weight.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RandomWeightedPicker
    {
        private readonly IEnumerable<float> items;
        private readonly float totalWeight;
        private Random random;

        /// <summary>
        /// Initiliaze the structure. O(1) or O(n) depending by the options, default O(n).
        /// </summary>
        /// <param name="items">The items</param>
        /// <param name="checkWeights">If <c>true</c> will check that the weights are positive. O(N)</param>
        /// <param name="shallowCopy">If <c>true</c> will copy the original collection structure (not the items). Keep in mind that items lifecycle is impacted.</param>
        public RandomWeightedPicker(IEnumerable<float> items, Random random)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (!items.Any()) throw new ArgumentException("items cannot be empty");
            this.random = random;
            this.items = items;
            totalWeight = this.items.Sum(i => i);
        }
        /// <summary>
        /// Pick a random item based on its chance. O(n)
        /// </summary>
        /// <param name="defaultValue">The value returned in case the element has not been found</param>
        /// <returns></returns>
        public int PickAnItem()
        {
            var rnd = (float)random.NextDouble()*totalWeight;
            int idx = 0;
            foreach (var item in items)
            {
                if ((rnd -= item) < 0) return idx;
                idx++;
            }

            return items.Count()-1;
        }

        /// <summary>
        /// Resets the internal random generator. O(1)
        /// </summary>
        /// <param name="seed"></param>
        public void ResetRandomGenerator(int? seed)
        {
            random = seed.HasValue ? new Random(seed.Value) : new Random();
        }
    }