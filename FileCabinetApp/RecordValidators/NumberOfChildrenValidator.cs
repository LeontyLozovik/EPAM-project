namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Validate number of children.
    /// </summary>
    public class NumberOfChildrenValidator : IRecordValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NumberOfChildrenValidator"/> class.
        /// </summary>
        public NumberOfChildrenValidator()
        {
            this.MinNumber = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberOfChildrenValidator"/> class.
        /// </summary>
        /// <param name="number">min number of children.</param>
        public NumberOfChildrenValidator(int number)
        {
            this.MinNumber = number;
        }

        /// <summary>
        /// Gets or sets min number of children.
        /// </summary>
        /// <value>number of children.</value>
        public int MinNumber { get; set; }

        /// <summary>
        /// Validate number of children.
        /// </summary>
        /// <param name="record">record to validate.</param>
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (record.Children < this.MinNumber)
            {
                throw new ArgumentException($"Number of children can't be less then {this.MinNumber}.");
            }
        }
    }
}
