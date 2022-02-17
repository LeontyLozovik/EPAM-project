namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Validate average selary.
    /// </summary>
    public class AverageSalaryValidator : IRecordValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AverageSalaryValidator"/> class.
        /// </summary>
        public AverageSalaryValidator()
        {
            this.MinSalary = 0;
            this.MaxSalary = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AverageSalaryValidator"/> class.
        /// </summary>
        /// <param name="min">min salary.</param>
        /// <param name="max">max selary.</param>
        public AverageSalaryValidator(int min, int max)
        {
            this.MinSalary = min;
            this.MaxSalary = max;
        }

        /// <summary>
        /// Gets or sets MaxSalary.
        /// </summary>
        /// <value>max selary.</value>
        public int MaxSalary { get; set; }

        /// <summary>
        /// Gets or sets MinSalary.
        /// </summary>
        /// <value>min salary.</value>
        public int MinSalary { get; set; }

        /// <summary>
        /// Validate average salary.
        /// </summary>
        /// <param name="record">record to validate.</param>
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (record.AverageSalary < this.MinSalary || record.AverageSalary > this.MaxSalary)
            {
                throw new ArgumentException($"Average salary can't be less then {this.MinSalary} or grater then {this.MaxSalary}.");
            }
        }
    }
}
