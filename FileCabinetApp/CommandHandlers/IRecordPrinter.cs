namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represet rules for records printers.
    /// </summary>
    public interface IRecordPrinter
    {
        /// <summary>
        /// Print records.
        /// </summary>
        /// <param name="records">Records to print.</param>
        public void Print(IEnumerable<FileCabinetRecord> records);
    }
}
