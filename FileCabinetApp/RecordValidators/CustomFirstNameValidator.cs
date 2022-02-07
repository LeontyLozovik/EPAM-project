namespace FileCabinetApp.RecordValidators
{
    public class CustomFirstNameValidator : IRecordValidator
    {
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record.FirstName is null)
            {
                throw new ArgumentNullException(record.FirstName, "First name can't be null.");
            }
            else if (string.IsNullOrWhiteSpace(record.FirstName) || record.FirstName.Length < 1 || record.FirstName.Length > 20)
            {
                throw new ArgumentException("Incorrect first name! First name should be grater then 1, less then 20 and can't be white space. ", record.FirstName);
            }
        }
    }
}
