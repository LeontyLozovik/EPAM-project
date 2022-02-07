namespace FileCabinetApp.RecordValidators
{
    public class DefaultNumberOfChildrenValidator : IRecordValidator
    {
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record.Children < 0)
            {
                throw new ArgumentException("Number of children can't be less then 0.");
            }
        }
    }
}
