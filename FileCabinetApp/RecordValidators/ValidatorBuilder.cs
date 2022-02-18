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
        /// <param name="validator">validator to add.</param>
        /// <returns>Object of ValidatorBuilder with added firstname validator.</returns>
        public ValidatorBuilder ValidateFirstName(FirstNameValidator validator) // сюда можно сразу передавать FirstNameValidator
        {
            this.validators.Add(validator);
            return this;
        }

        /// <summary>
        /// Add lastname validator to list of validators.
        /// </summary>
        /// <param name="validator">validator to add.</param>
        /// <returns>Object of ValidatorBuilder with added lastname validator.</returns>
        public ValidatorBuilder ValidateLastName(LastNameValidator validator)
        {
            this.validators.Add(validator);
            return this;
        }

        /// <summary>
        /// Add date of birth validator to list of validators.
        /// </summary>
        /// <param name="validator">validator to add.</param>
        /// <returns>Object of ValidatorBuilder with added date of birth validator.</returns>
        public ValidatorBuilder ValidateDateOfBirth(DateOfBirthValidator validator)
        {
            this.validators.Add(validator);
            return this;
        }

        /// <summary>
        /// Add number of children validator to list of validators.
        /// </summary>
        /// <param name="validator">validator to add.</param>
        /// <returns>Object of ValidatorBuilder with added number of children validator.</returns>
        public ValidatorBuilder ValidateNumberOfChildren(NumberOfChildrenValidator validator)
        {
            this.validators.Add(validator);
            return this;
        }

        /// <summary>
        /// Add average salary validator to list of validators.
        /// </summary>
        /// <param name="validator">validator to add.</param>
        /// <returns>Object of ValidatorBuilder with added average salary validator.</returns>
        public ValidatorBuilder ValidateAveragesalary(AverageSalaryValidator validator)
        {
            this.validators.Add(validator);
            return this;
        }

        /// <summary>
        /// Add sex validator to list of validators.
        /// </summary>
        /// <param name="validator">validator to add.</param>
        /// <returns>Object of ValidatorBuilder with added sex validator.</returns>
        public ValidatorBuilder ValidateSex(SexValidator validator)
        {
            this.validators.Add(validator);
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
