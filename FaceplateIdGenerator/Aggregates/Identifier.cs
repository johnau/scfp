namespace FaceplateIdGenerator.Aggregates
{
    internal class Identifier
    {
        private readonly int _batchSize = 24;
        private string _prefix;
        private int _current;

        public Identifier(string prefix, int startNumber = 100)
        {
            _prefix = prefix;
            _current = startNumber;
        }

        /// <summary>
        /// Gets the current Id
        /// </summary>
        /// <returns></returns>
        public string GetId()
        {
            return _prefix + _current.ToString("D3");
        }

        /// <summary>
        /// Gets the next Id
        /// </summary>
        /// <returns></returns>
        public string IncrementId(int amount = 1)
        {
            if (amount < 1) 
                throw new ArgumentException("Amount must be greater than or equal to 1");

            _current++;

            if (amount > 1)
            {
                for (int i = 2; i <= amount; i++)
                {
                    _current++;
                }
            }
            return GetId();
        }

        public void EndBatch()
        {
            _current = _batchSize - (_current % _batchSize) + _current;
        }
    }
}
