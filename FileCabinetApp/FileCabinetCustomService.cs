namespace FileCabinetApp
{
    /// <summary>
    /// Custom validation parameters of record.
    /// </summary>
    public class FileCabinetCustomService : FileCabinetService
    {
        /// <summary>
        /// Create instance of CustomValidator.
        /// </summary>
        /// <returns>instance of CustomValidator.</returns>
        protected override IRecordValidator CreateValidator()
        {
            return new CustomValidator();
        }
    }
}
