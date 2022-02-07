namespace FileCabinetApp.RecordValidators
{
    public class NumberOfChildrenValidator : IRecordValidator
    {
        private int minNumber;

        public NumberOfChildrenValidator(int number)
        {
            this.minNumber = number;
        }

        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record.Children < this.minNumber)
            {
                throw new ArgumentException($"Number of children can't be less then {this.minNumber}.");
            }
        }
    }
}
