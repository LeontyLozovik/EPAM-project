namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Validate average selary.
    /// </summary>
    public class AverageSalaryValidator : IRecordValidator
    {
        private int minSalary;
        private int maxSalary;

        /// <summary>
        /// Initializes a new instance of the <see cref="AverageSalaryValidator"/> class.
        /// </summary>
        /// <param name="min">min salary.</param>
        /// <param name="max">max selary.</param>
        public AverageSalaryValidator(int min, int max)
        {
            this.minSalary = min;
            this.maxSalary = max;
        }

        /// <summary>
        /// Validate average salary.
        /// </summary>
        /// <param name="record">record to validate.</param>
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record.AverageSalary < this.minSalary || record.AverageSalary > this.maxSalary)
            {
                throw new ArgumentException($"Average salary can't be less then {this.minSalary} or grater then {this.maxSalary}.");
            }
        }
    }
}
