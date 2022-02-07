namespace FileCabinetApp.RecordValidators
{
    public class LastNameValidator : IRecordValidator
    {
        private int maxLenght;
        private int minLenght;

        public LastNameValidator(int min, int max)
        {
            this.maxLenght = max;
            this.minLenght = min;
        }

        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record.LastName is null)
            {
                throw new ArgumentNullException(record.LastName, "Last name can't be null.");
            }
            else if (string.IsNullOrWhiteSpace(record.LastName) || record.LastName.Length < this.minLenght || record.LastName.Length > this.maxLenght)
            {
                throw new ArgumentException($"Incorrect last name! Last name should be grater then {this.minLenght}, less then {this.maxLenght} and can't be white space.", record.LastName);
            }
        }
    }
}
