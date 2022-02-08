namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Discribes the rules for creating validation classes.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Validate incoming parameters of record.
        /// </summary>
        /// <param name="record">record whose parametrs should be validate.</param>
        public void ValidateParameters(FileCabinetRecord record);
    }
}
