namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Represent rules for creating iterators.
    /// </summary>
    public interface IRecordIterator
    {
        /// <summary>
        /// Get next element in collection.
        /// </summary>
        /// <returns>collection element with current index.</returns>
        public FileCabinetRecord GetNext();

        /// <summary>
        /// Return if contain any elements more.
        /// </summary>
        /// <returns>true - contains more, false - not contains more.</returns>
        public bool HasMore();

        /// <summary>
        /// Reset index.
        /// </summary>
        public void Reset();
    }
}
