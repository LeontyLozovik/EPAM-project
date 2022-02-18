namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Validate date of birth.
    /// </summary>
    public class DateOfBirthValidator : IRecordValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateOfBirthValidator"/> class.
        /// </summary>
        public DateOfBirthValidator()
        {
            this.From = DateTime.Now;
            this.To = DateTime.Now;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateOfBirthValidator"/> class.
        /// </summary>
        /// <param name="from">earliest date.</param>
        /// <param name="to">latest date.</param>
        public DateOfBirthValidator(DateTime from, DateTime to)
        {
            this.From = from;
            this.To = to;
        }

        /// <summary>
        /// Gets or sets latest date.
        /// </summary>
        /// <value>latest date.</value>
        public DateTime From { get; set; }

        /// <summary>
        /// Gets or sets earliest date.
        /// </summary>
        /// <value>earliest date.</value>
        public DateTime To { get; set; }

        /// <summary>
        /// Validate date of birth.
        /// </summary>
        /// <param name="record">record to validate.</param>
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (record.DateOfBirth < this.From || record.DateOfBirth > this.To)
            {
                throw new ArgumentException($"Sorry but minimal date of birth - {this.From} and maxsimum - {this.To}");
            }
        }
    }
}
