namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Validate date of birth.
    /// </summary>
    public class DateOfBirthValidator : IRecordValidator
    {
        private DateTime from;
        private DateTime to;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateOfBirthValidator"/> class.
        /// </summary>
        /// <param name="from">earliest date.</param>
        /// <param name="to">latest date.</param>
        public DateOfBirthValidator(DateTime from, DateTime to)
        {
            this.from = from;
            this.to = to;
        }

        /// <summary>
        /// Validate date of birth.
        /// </summary>
        /// <param name="record">record to validate.</param>
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record.DateOfBirth < this.from || record.DateOfBirth > this.to)
            {
                throw new ArgumentException($"Sorry but minimal date of birth - {this.from} and maxsimum - {this.to}");
            }
        }
    }
}
