namespace FileCabinetApp.RecordValidators
{
    public class DateOfBirthValidator : IRecordValidator
    {
        private DateTime from;
        private DateTime to;

        public DateOfBirthValidator(DateTime from, DateTime to)
        {
            this.from = from;
            this.to = to;
        }

        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record.DateOfBirth < this.from || record.DateOfBirth > this.to)
            {
                throw new ArgumentException($"Sorry but minimal date of birth - {this.from} and maxsimum - {this.to}");
            }
        }
    }
}
