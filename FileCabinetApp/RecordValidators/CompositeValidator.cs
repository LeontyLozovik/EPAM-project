namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Execute set of validation.
    /// </summary>
    public class CompositeValidator : IRecordValidator
    {
        private List<IRecordValidator> validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidator"/> class.
        /// </summary>
        /// <param name="validators">set of validations.</param>
        protected CompositeValidator(IEnumerable<IRecordValidator> validators)
        {
            this.validators = (List<IRecordValidator>)validators;
        }

        /// <summary>
        /// Execute validation.
        /// </summary>
        /// <param name="record">record to vilidate.</param>
        public void ValidateParameters(FileCabinetRecord record)
        {
            foreach (var validator in this.validators)
            {
                validator.ValidateParameters(record);
            }
        }
    }
}
