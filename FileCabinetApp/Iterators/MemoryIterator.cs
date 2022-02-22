using System.Collections.ObjectModel;

namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Iterrator for FileCabinetMemoryService.
    /// </summary>
    public class MemoryIterator : IRecordIterator
    {
        private ReadOnlyCollection<FileCabinetRecord> collection;
        private int index;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryIterator"/> class.
        /// </summary>
        /// <param name="collection">collection to iterate.</param>
        public MemoryIterator(ReadOnlyCollection<FileCabinetRecord> collection)
        {
            this.collection = collection;
            this.index = -1;
        }

        /// <summary>
        /// Get next element in collection.
        /// </summary>
        /// <returns>collection element with current index.</returns>
        public FileCabinetRecord GetNext()
        {
            return this.collection[++this.index];
        }

        /// <summary>
        /// Return if contain any elements more.
        /// </summary>
        /// <returns>true - contains more, false - not contains more.</returns>
        public bool HasMore()
        {
            if (this.index + 1 < this.collection.Count)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Reset index.
        /// </summary>
        public void Reset()
        {
            this.index = -1;
        }
    }
}
