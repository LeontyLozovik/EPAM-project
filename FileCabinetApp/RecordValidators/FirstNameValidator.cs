namespace FileCabinetApp.RecordValidators
{
    public class FirstNameValidator : IRecordValidator
    {
        private int maxLenght;
        private int minLenght;

        public FirstNameValidator(int min, int max)
        {
            this.maxLenght = max;
            this.minLenght = min;
        }

        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record.FirstName is null)
            {
                throw new ArgumentNullException(record.FirstName, "First name can't be null.");
            }
            else if (string.IsNullOrWhiteSpace(record.FirstName) || record.FirstName.Length < this.minLenght || record.FirstName.Length > this.maxLenght)
            {
                throw new ArgumentException($"Incorrect first name! First name should be grater then {this.minLenght}, less then {this.maxLenght} and can't be white space. ", record.FirstName);
            }
        }
    }
}
