namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Build validator.
    /// </summary>
    public class ValidatorBuilder
    {
        private readonly List<IRecordValidator> validators = new List<IRecordValidator>();

        /// <summary>
        /// Add firstname validator to list of validators.
        /// </summary>
        /// <param name="minLenght">min lenghts of firstname.</param>
        /// <param name="maxLenght">max lenghts of firstname.</param>
        /// <returns>Object of ValidatorBuilder with added firstname validator.</returns>
        public ValidatorBuilder ValidateFirstName(int minLenght, int maxLenght)
        {
            this.validators.Add(new FirstNameValidator(minLenght, maxLenght));
            return this;
        }

        /// <summary>
        /// Add lastname validator to list of validators.
        /// </summary>
        /// <param name="minLenght">min lenghts of lastname.</param>
        /// <param name="maxLenght">max lenghts of lastname.</param>
        /// <returns>Object of ValidatorBuilder with added lastname validator.</returns>
        public ValidatorBuilder ValidateLastName(int minLenght, int maxLenght)
        {
            this.validators.Add(new LastNameValidator(minLenght, maxLenght));
            return this;
        }

        /// <summary>
        /// Add date of birth validator to list of validators.
        /// </summary>
        /// <param name="from">earliest date.</param>
        /// <param name="to">latest date.</param>
        /// <returns>Object of ValidatorBuilder with added date of birth validator.</returns>
        public ValidatorBuilder ValidateDateOfBirth(DateTime from, DateTime to)
        {
            this.validators.Add(new DateOfBirthValidator(from, to));
            return this;
        }

        /// <summary>
        /// Add number of children validator to list of validators.
        /// </summary>
        /// <param name="minNumber">min number of children.</param>
        /// <returns>Object of ValidatorBuilder with added number of children validator.</returns>
        public ValidatorBuilder ValidateNumberOfChildren(int minNumber)
        {
            this.validators.Add(new NumberOfChildrenValidator(minNumber));
            return this;
        }

        /// <summary>
        /// Add average salary validator to list of validators.
        /// </summary>
        /// <param name="min">min salary.</param>
        /// <param name="max">max salary.</param>
        /// <returns>Object of ValidatorBuilder with added average salary validator.</returns>
        public ValidatorBuilder ValidateAveragesalary(int min, int max)
        {
            this.validators.Add(new AverageSalaryValidator(min, max));
            return this;
        }

        /// <summary>
        /// Add sex validator to list of validators.
        /// </summary>
        /// <returns>Object of ValidatorBuilder with added sex validator.</returns>
        public ValidatorBuilder ValidateSex()
        {
            this.validators.Add(new SexValidator());
            return this;
        }

        /// <summary>
        /// Create validator with all supvalidators.
        /// </summary>
        /// <returns>validator.</returns>
        public IRecordValidator Create()
        {
            return new CompositeValidator(this.validators);
        }
    }
}
