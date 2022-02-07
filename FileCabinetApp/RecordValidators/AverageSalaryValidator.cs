namespace FileCabinetApp.RecordValidators
{
    public class AverageSalaryValidator : IRecordValidator
    {
        private int minSalary;
        private int maxSalary;

        public AverageSalaryValidator(int min, int max)
        {
            this.minSalary = min;
            this.maxSalary = max;
        }

        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record.AverageSalary < this.minSalary || record.AverageSalary > this.maxSalary)
            {
                throw new ArgumentException($"Average salary can't be less then {this.minSalary} or grater then {this.maxSalary}.");
            }
        }
    }
}
