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

            new FirstNameValidator(2, 60).ValidateParameters(record);
            new LastNameValidator(2, 60).ValidateParameters(record);
            new DateOfBirthValidator(new DateTime(1950, 1, 1), DateTime.Now).ValidateParameters(record);
            new NumberOfChildrenValidator(0).ValidateParameters(record);
            new AverageSalaryValidator(0, 1000000000).ValidateParameters(record);
            new DefaultSexValidator().ValidateParameters(record);
        }
    }
}
