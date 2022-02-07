namespace FileCabinetApp.RecordValidators
{
    public class DefaultLastNameValidator : IRecordValidator
    {
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record.LastName is null)
            {
                throw new ArgumentNullException(record.LastName, "Last name can't be null.");
            }
            else if (string.IsNullOrWhiteSpace(record.LastName) || record.LastName.Length < 2 || record.LastName.Length > 60)
            {
                throw new ArgumentException("Incorrect last name! Last name should be grater then 2, less then 60 and can't be white space.", record.LastName);
            }
        }
    }
}
