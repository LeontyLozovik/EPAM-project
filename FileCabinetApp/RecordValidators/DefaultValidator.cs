namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Default validation parameters of record.
    /// </summary>
    public class DefaultValidator : CompositeValidator
    {
        public DefaultValidator()
            : base(new IRecordValidator[] {
            new FirstNameValidator(2, 60),
            new LastNameValidator(2, 60),
            new DateOfBirthValidator(new DateTime(1950, 1, 1), DateTime.Now),
            new NumberOfChildrenValidator(0),
            new AverageSalaryValidator(0, 1000000000),
            new SexValidator(),
        })
        {
        }
    }
}
