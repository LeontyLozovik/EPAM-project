using Microsoft.Extensions.Configuration;

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
            var allValidators = ReturnValidators(ValidationType.Default);
            var validator = validatorBuilder.ValidateFirstName(allValidators.Item1)
                .ValidateLastName(allValidators.Item2)
                .ValidateDateOfBirth(allValidators.Item3)
                .ValidateNumberOfChildren(allValidators.Item4)
                .ValidateAveragesalary(allValidators.Item5)
                .ValidateSex(new SexValidator())
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

            var allValidators = ReturnValidators(ValidationType.Default);
            var validator = validatorBuilder.ValidateFirstName(allValidators.Item1)
                .ValidateLastName(allValidators.Item2)
                .ValidateDateOfBirth(allValidators.Item3)
                .ValidateNumberOfChildren(allValidators.Item4)
                .ValidateAveragesalary(allValidators.Item5)
                .ValidateSex(new SexValidator())
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
            var allValidators = ReturnValidators(ValidationType.Custom);
            var validator = validatorBuilder.ValidateFirstName(allValidators.Item1)
                .ValidateLastName(allValidators.Item2)
                .ValidateDateOfBirth(allValidators.Item3)
                .ValidateNumberOfChildren(allValidators.Item4)
                .ValidateAveragesalary(allValidators.Item5)
                .ValidateSex(new SexValidator())
                .Create();
            return (CompositeValidator)validator;
        }

        private static Tuple<FirstNameValidator, LastNameValidator, DateOfBirthValidator, NumberOfChildrenValidator, AverageSalaryValidator> ReturnValidators(ValidationType type)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("validation-rules.json")
                .Build();
            IConfigurationSection section;
            if (type == ValidationType.Default)
            {
                section = builder.GetSection("default");
            }
            else
            {
                section = builder.GetSection("custom");
            }

            var firstnameValidator = section.GetSection("firstName").Get<FirstNameValidator>();
            var lastnnameValidator = section.GetSection("lastName").Get<LastNameValidator>();
            var birthdayValidator = section.GetSection("dateOfBirth").Get<DateOfBirthValidator>();
            var childrenValidator = section.GetSection("numberOfChildren").Get<NumberOfChildrenValidator>();
            var salaryValidator = section.GetSection("averageSalary").Get<AverageSalaryValidator>();
            return new Tuple<FirstNameValidator, LastNameValidator, DateOfBirthValidator, NumberOfChildrenValidator,
                AverageSalaryValidator>(firstnameValidator, lastnnameValidator, birthdayValidator, childrenValidator, salaryValidator);
        }
    }
}
