namespace FileCabinetApp
{
    /// <summary>
    /// Default validation parameters of record.
    /// </summary>
    public class FileCabinetDefaultService : FileCabinetService
    {
        /// <summary>
        /// Create instance of DefaultValidator.
        /// </summary>
        /// <returns>instance of DefaultValidator.</returns>
        protected override IRecordValidator CreateValidator()
        {
            return new DefaultValidator();
        }
    }
}
