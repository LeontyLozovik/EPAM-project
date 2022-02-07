namespace FileCabinetApp.RecordValidators
{
    public class DefaultFirstNameValidator : IRecordValidator
    {
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record.FirstName is null)
            {
                throw new ArgumentNullException(record.FirstName, "First name can't be null.");
            }
            else if (string.IsNullOrWhiteSpace(record.FirstName) || record.FirstName.Length < 2 || record.FirstName.Length > 60)
            {
                throw new ArgumentException("Incorrect first name! First name should be grater then 2, less then 60 and can't be white space.", record.FirstName);
            }
        }
    }
}
