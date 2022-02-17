namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Validate lastname.
    /// </summary>
    public class LastNameValidator : IRecordValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LastNameValidator"/> class.
        /// </summary>
        public LastNameValidator()
        {
            this.MaxLenght = 0;
            this.MinLenght = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LastNameValidator"/> class.
        /// </summary>
        /// <param name="min">min lenght of lastname.</param>
        /// <param name="max">max lenght of lastname.</param>
        public LastNameValidator(int min, int max)
        {
            this.MaxLenght = max;
            this.MinLenght = min;
        }

        /// <summary>
        /// Gets or sets MaxLenght.
        /// </summary>
        /// <value>max lenght of lastname.</value>
        public int MaxLenght { get; set; }

        /// <summary>
        /// Gets or sets MinLenght.
        /// </summary>
        /// <value>min lenght of firstname.</value>
        public int MinLenght { get; set; }

        /// <summary>
        /// Validate lastname.
        /// </summary>
        /// <param name="record">record to validate.</param>
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (record.LastName is null)
            {
                throw new ArgumentNullException(record.LastName, "Last name can't be null.");
            }
            else if (string.IsNullOrWhiteSpace(record.LastName) || record.LastName.Length < this.MinLenght || record.LastName.Length > this.MaxLenght)
            {
                throw new ArgumentException($"Incorrect last name! Last name should be grater then {this.MinLenght}, less then {this.MaxLenght} and can't be white space.", record.LastName);
            }
        }
    }
}
