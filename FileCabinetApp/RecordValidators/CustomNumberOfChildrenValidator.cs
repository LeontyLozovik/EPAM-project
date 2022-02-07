namespace FileCabinetApp.RecordValidators
{
    public class CustomNumberOfChildrenValidator : IRecordValidator
    {
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record.Children < 1)
            {
                throw new ArgumentException("Number of children can't be less then 0. You should have a least one child.");
            }
        }
    }
}
