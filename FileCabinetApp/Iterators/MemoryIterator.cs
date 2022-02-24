using System.Collections;
using System.Collections.ObjectModel;

namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Iterrator for FileCabinetMemoryService.
    /// </summary>
    public class MemoryIterator : IEnumerable
    {
        private ReadOnlyCollection<FileCabinetRecord> collection;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryIterator"/> class.
        /// </summary>
        /// <param name="collection">collection to iterate.</param>
        public MemoryIterator(ReadOnlyCollection<FileCabinetRecord> collection)
        {
            this.collection = collection;
        }

        /// <summary>
        /// Returns enumerator.
        /// </summary>
        /// <returns>Enumerator.</returns>
        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < this.collection.Count; i++)
            {
                yield return this.collection[i];
            }
        }
    }
}
