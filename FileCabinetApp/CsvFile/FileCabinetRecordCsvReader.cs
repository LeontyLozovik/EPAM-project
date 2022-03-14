using System.Globalization;

namespace FileCabinetApp.CsvFile
{
    /// <summary>
    /// Read records from csv file.
    /// </summary>
    public class FileCabinetRecordCsvReader
    {
        private StreamReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvReader"/> class.
        /// </summary>
        /// <param name="reader">indent to inicializ this StramReader.</param>
        public FileCabinetRecordCsvReader(StreamReader reader)
        {
            this.reader = reader;
        }

        /// <summary>
        /// Read records from csv file.
        /// </summary>
        /// <returns>list of readed records.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            List<FileCabinetRecord> recordsFromFile = new List<FileCabinetRecord>();
            string? stringRecord;
            while ((stringRecord = this.reader.ReadLine()) != null)
            {
                var fildsOfRecord = stringRecord.Split(',');
                int id = int.Parse(fildsOfRecord[0], CultureInfo.InvariantCulture);
                string firstname = fildsOfRecord[1];
                string lastname = fildsOfRecord[2];
                var date = fildsOfRecord[3].Split(".");
                int day = int.Parse(date[0], CultureInfo.InvariantCulture);
                int month = int.Parse(date[1], CultureInfo.InvariantCulture);
                int year = int.Parse(date[2], CultureInfo.InvariantCulture);
                DateTime dateOfBirth = new DateTime(year, month, day);
                short children = short.Parse(fildsOfRecord[4], CultureInfo.InvariantCulture);
                decimal salary = decimal.Parse(fildsOfRecord[5], CultureInfo.InvariantCulture);
                char sex = char.Parse(fildsOfRecord[6]);
                FileCabinetRecord record = new FileCabinetRecord
                {
                    Id = id,
                    FirstName = firstname,
                    LastName = lastname,
                    DateOfBirth = dateOfBirth,
                    Children = children,
                    AverageSalary = salary,
                    Sex = sex,
                };

                recordsFromFile.Add(record);
            }

            return recordsFromFile;
        }
    }
}
