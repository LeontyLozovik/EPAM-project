namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Custom validation parameters of record.
    /// </summary>
    public class CustomValidator : CompositeValidator
    {
        public CustomValidator()
            : base (new IRecordValidator[] {
            new FirstNameValidator(1, 20),
            new LastNameValidator(1, 20),
            new DateOfBirthValidator(new DateTime(1900, 1, 1), DateTime.Now),
            new NumberOfChildrenValidator(1),
            new AverageSalaryValidator(500, 1000000),
            new SexValidator(),
        })
        {
        }
    }
}
