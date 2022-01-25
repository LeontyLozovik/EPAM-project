using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Write record to file in csv format.
    /// </summary>
    public class FileCabinetRecordCsvWriter
    {
        /// <summary>
        /// Writer - write record to the file.
        /// </summary>
        private readonly TextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
        /// </summary>
        /// <param name="textWriter">param to initialize fild writer.</param>
        public FileCabinetRecordCsvWriter(TextWriter textWriter)
        {
            this.writer = textWriter;
        }

        /// <summary>
        /// Write record to csv file.
        /// </summary>
        /// <param name="record">record to write.</param>
        public void Write(FileCabinetRecord record)
        {
            StringBuilder stringToWrite = new StringBuilder();
            object[] fildsOfRecord =
            {
                record.Id, record.FirstName, record.LastName, record.DateOfBirth.ToString("dd/MM/yyyy"),
                record.Children, record.AverageSalary, record.Sex,
            };
            stringToWrite.AppendJoin(',', fildsOfRecord);
            this.writer.WriteLine(stringToWrite);
        }

        /// <summary>
        /// Write name of all filds of record.
        /// </summary>
        public void WriteTemplate()
        {
            string template = "Id,First name,Last name,Date of birth,Number of children,Average salary,Sex";
            this.writer.WriteLine(template);
        }
    }
}
