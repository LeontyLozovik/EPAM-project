namespace FileCabinetApp.RecordValidators
{
    public class CustomDateOfBirthValidator : IRecordValidator
    {
        public void ValidateParameters(FileCabinetRecord record)
        {
            DateTime oldest = new DateTime(1900, 1, 1);
            DateTime now = DateTime.Now;
            if (record.DateOfBirth < oldest || record.DateOfBirth > now)
            {
                throw new ArgumentException("Sorry but minimal date of birth - 01-Jan-1950 and maxsimum - current date");
            }
        }
    }
}
