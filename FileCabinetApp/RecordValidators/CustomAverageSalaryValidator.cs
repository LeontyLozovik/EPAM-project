namespace FileCabinetApp.RecordValidators
{
    public class CustomAverageSalaryValidator : IRecordValidator
    {
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record.AverageSalary < 500 || record.AverageSalary > 1000000)
            {
                throw new ArgumentException("Average salary can't be less then 500 or grater then 1 million.");
            }
        }
    }
}
