namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Valiate sex.
    /// </summary>
    public class SexValidator : IRecordValidator
    {
        /// <summary>
        /// Validate sex.
        /// </summary>
        /// <param name="record">record to validate.</param>
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (record.Sex != 'm' && record.Sex != 'w')
            {
                throw new ArgumentException("Sorry, but your sex can be m - men or w - women only.");
            }
        }
    }
}
