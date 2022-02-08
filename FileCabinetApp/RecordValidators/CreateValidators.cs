namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Extantion methods for creation validators.
    /// </summary>
    public static class CreateValidators
    {
        /// <summary>
        /// Create default validator.
        /// </summary>
        /// <param name="validatorBuilder">Instance to build validator.</param>
        /// <param name="setValidation">Function that set validation type.</param>
        /// <returns>List of validators.</returns>
        public static CompositeValidator CreateDefault(this ValidatorBuilder validatorBuilder, Action<ValidationType> setValidation)
        {
            if (validatorBuilder is null)
            {
                throw new ArgumentNullException(nameof(validatorBuilder));
            }
            else if (setValidation is null)
            {
                throw new ArgumentNullException(nameof(setValidation));
            }

            setValidation.Invoke(ValidationType.Default);
            var validator = validatorBuilder.ValidateFirstName(2, 60)
                .ValidateLastName(2, 60)
                .ValidateDateOfBirth(new DateTime(1950, 1, 1), DateTime.Now)
                .ValidateNumberOfChildren(0)
                .ValidateAveragesalary(0, 100000000)
                .ValidateSex()
                .Create();
            return (CompositeValidator)validator;
        }

        /// <summary>
        /// Create default validator.
        /// </summary>
        /// <param name="validatorBuilder">Instance to build validator.</param>
        /// <returns>List of validators.</returns>
        public static CompositeValidator CreateDefault(this ValidatorBuilder validatorBuilder)
        {
            if (validatorBuilder is null)
            {
                throw new ArgumentNullException(nameof(validatorBuilder));
            }

            var validator = validatorBuilder.ValidateFirstName(2, 60)
                .ValidateLastName(2, 60)
                .ValidateDateOfBirth(new DateTime(1950, 1, 1), DateTime.Now)
                .ValidateNumberOfChildren(0)
                .ValidateAveragesalary(0, 100000000)
                .ValidateSex()
                .Create();
            return (CompositeValidator)validator;
        }

        /// <summary>
        /// Create custom validator.
        /// </summary>
        /// <param name="validatorBuilder">Instance to build validator.</param>
        /// /// <param name="setValidation">Function that set validation type.</param>
        /// <returns>List of validators.</returns>
        public static CompositeValidator CreateCustom(this ValidatorBuilder validatorBuilder, Action<ValidationType> setValidation)
        {
            if (validatorBuilder is null)
            {
                throw new ArgumentNullException(nameof(validatorBuilder));
            }
            else if (setValidation is null)
            {
                throw new ArgumentNullException(nameof(setValidation));
            }

            setValidation.Invoke(ValidationType.Custom);
            var validator = validatorBuilder.ValidateFirstName(1, 20)
                .ValidateLastName(1, 20)
                .ValidateDateOfBirth(new DateTime(1900, 1, 1), DateTime.Now)
                .ValidateNumberOfChildren(1)
                .ValidateAveragesalary(500, 100000)
                .ValidateSex()
                .Create();
            return (CompositeValidator)validator;
        }
    }
}
