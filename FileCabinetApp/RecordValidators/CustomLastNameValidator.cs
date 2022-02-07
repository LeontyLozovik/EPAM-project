namespace FileCabinetApp.RecordValidators
{
    public class CustomLastNameValidator : IRecordValidator
    {
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record.LastName is null)
            {
                throw new ArgumentNullException(record.LastName, "Last name can't be null.");
            }
            else if (string.IsNullOrWhiteSpace(record.LastName) || record.LastName.Length < 1 || record.LastName.Length > 20)
            {
                throw new ArgumentException("Incorrect last name! Last name should be grater then 1, less then 20 and can't be white space.", record.LastName);
            }
        }
    }
}
