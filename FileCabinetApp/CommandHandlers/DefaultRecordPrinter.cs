namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Print records in default way.
    /// </summary>
    public class DefaultRecordPrinter : IRecordPrinter
    {
        /// <summary>
        /// Print records in default way.
        /// </summary>
        /// <param name="records">Records to print.</param>
        public void Print(IEnumerable<FileCabinetRecord> records)
        {
            foreach (var record in records)
            {
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth:yyyy-MMM-dd}, {record.Children}, {record.AverageSalary}, {record.Sex}");
            }
        }
    }
}
