namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Default validation parameters of record.
    /// </summary>
    public class DefaultValidator : IRecordValidator
    {
        /// <summary>
        /// Validate incoming parameters of record.
        /// </summary>
        /// <param name="record">record whose parametrs should be validate.</param>
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), "Instance doesn't exist.");
            }

            new DefaultFirstNameValidator().ValidateParameters(record);
            new DefaultLastNameValidator().ValidateParameters(record);
            new DefaultDateOfBirthValidator().ValidateParameters(record);
            new DefaultNumberOfChildrenValidator().ValidateParameters(record);
            new DefaultAverageSalaryValidator().ValidateParameters(record);
            new DefaultSexValidator().ValidateParameters(record);
        }
    }
}
