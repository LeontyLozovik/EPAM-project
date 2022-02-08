namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Validate number of children.
    /// </summary>
    public class NumberOfChildrenValidator : IRecordValidator
    {
        private int minNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberOfChildrenValidator"/> class.
        /// </summary>
        /// <param name="number">min number of children.</param>
        public NumberOfChildrenValidator(int number)
        {
            this.minNumber = number;
        }

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

            if (record.Children < this.minNumber)
            {
                throw new ArgumentException($"Number of children can't be less then {this.minNumber}.");
            }
        }
    }
}
