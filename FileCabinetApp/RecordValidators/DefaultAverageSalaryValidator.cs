namespace FileCabinetApp.RecordValidators
{
    public class DefaultAverageSalaryValidator : IRecordValidator
    {
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record.AverageSalary < 0 || record.AverageSalary > 1000000000)
            {
                throw new ArgumentException("Average salary can't be less then 0 or grater then 1 billion.");
            }
        }
    }
}
