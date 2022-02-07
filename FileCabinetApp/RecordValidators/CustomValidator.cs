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

            new CustomFirstNameValidator().ValidateParameters(record);
            new CustomLastNameValidator().ValidateParameters(record);
            new CustomDateOfBirthValidator().ValidateParameters(record);
            new CustomNumberOfChildrenValidator().ValidateParameters(record);
            new CustomAverageSalaryValidator().ValidateParameters(record);
            new CustomSexValidator().ValidateParameters(record);
        }
    }
}
