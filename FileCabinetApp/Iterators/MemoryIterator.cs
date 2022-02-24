using System.Collections;
using System.Collections.ObjectModel;

namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Iterrator for FileCabinetMemoryService.
    /// </summary>
    public class MemoryIterator : IEnumerator, IEnumerable
    {
        private ReadOnlyCollection<FileCabinetRecord> collection;
        private int index = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryIterator"/> class.
        /// </summary>
        /// <param name="collection">collection to iterate.</param>
        public MemoryIterator(ReadOnlyCollection<FileCabinetRecord> collection)
        {
            this.collection = collection;
        }

        /// <summary>
        /// Gets current element of collection.
        /// </summary>
        /// <value>Current element of collection.</value>
        public object Current
        {
            get
            {
                return this.collection[this.index];
            }
        }

        /// <summary>
        /// Returns enumerator.
        /// </summary>
        /// <returns>Enumerator.</returns>
        public IEnumerator GetEnumerator()
        {
            return this;
        }

        /// <summary>
        /// Moves the pointer to the next element.
        /// </summary>
        /// <returns>true - if element exist, false if not.</returns>
        public bool MoveNext()
        {
            if (this.index + 1 < this.collection.Count)
            {
                this.index++;
                return true;
            }
            else
            {
                this.Reset();
                return false;
            }
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
