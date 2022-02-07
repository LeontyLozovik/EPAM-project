namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Custom validation parameters of record.
    /// </summary>
    public class CustomValidator : IRecordValidator
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

            new FirstNameValidator(1, 20).ValidateParameters(record);
            new LastNameValidator(1, 20).ValidateParameters(record);
            new DateOfBirthValidator(new DateTime(1900, 1, 1), DateTime.Now).ValidateParameters(record);
            new NumberOfChildrenValidator(1).ValidateParameters(record);
            new AverageSalaryValidator(500, 1000000).ValidateParameters(record);
            new CustomSexValidator().ValidateParameters(record);
        }
    }
}
