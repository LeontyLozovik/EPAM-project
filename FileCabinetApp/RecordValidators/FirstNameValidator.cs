namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Validate firstname.
    /// </summary>
    public class FirstNameValidator : IRecordValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirstNameValidator"/> class.
        /// </summary>
        public FirstNameValidator()
        {
            this.MaxLenght = 0;
            this.MinLenght = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirstNameValidator"/> class.
        /// </summary>
        /// <param name="min">min lenght of firstname.</param>
        /// <param name="max">max lenght of firstname.</param>
        public FirstNameValidator(int min, int max)
        {
            this.MaxLenght = max;
            this.MinLenght = min;
        }

        /// <summary>
        /// Gets or sets MaxLenght.
        /// </summary>
        /// <value>max lenght of firstname.</value>
        public int MaxLenght { get; set; }

        /// <summary>
        /// Gets or sets MinLenght.
        /// </summary>
        /// <value>min lenght of firstname.</value>
        public int MinLenght { get; set; }

        /// <summary>
        /// Validate firstname.
        /// </summary>
        /// <param name="record">record to validate.</param>
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (record.FirstName is null)
            {
                throw new ArgumentNullException(record.FirstName, "First name can't be null.");
            }
            else if (string.IsNullOrWhiteSpace(record.FirstName) || record.FirstName.Length < this.MinLenght || record.FirstName.Length > this.MaxLenght)
            {
                throw new ArgumentException($"Incorrect first name! First name should be grater then {this.MinLenght}, less then {this.MaxLenght} and can't be white space. ", record.FirstName);
            }
        }
    }
}
