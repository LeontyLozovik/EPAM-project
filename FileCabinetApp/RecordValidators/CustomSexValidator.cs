namespace FileCabinetApp.RecordValidators
{
    public class CustomSexValidator : IRecordValidator
    {
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record.Sex != 'm' && record.Sex != 'w')
            {
                throw new ArgumentException("Sorry, but your sex can be m - men or w - women only.");
            }
        }
    }
}
