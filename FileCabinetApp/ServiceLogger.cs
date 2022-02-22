using System.Collections.ObjectModel;
using System.Text;
using FileCabinetApp.Iterators;

namespace FileCabinetApp
{
    /// <summary>
    /// Wrapper for FileCabinet services ещ save logs.
    /// </summary>
    public class ServiceLogger : IFileCabinetService
    {
        private IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLogger"/> class.
        /// </summary>
        /// <param name="cabinetService">one of the FileCabinet services.</param>
        public ServiceLogger(IFileCabinetService cabinetService)
        {
            this.service = cabinetService;
        }

        /// <summary>
        /// Create a new record with inputed parameters.
        /// </summary>
        /// <param name="record">Record to create.</param>
        /// <returns>Id of created record.</returns>
        public int CreateRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), "Instance doesn't exist.");
            }

            DateTime now = DateTime.Now;
            string startLog = $"{now:MM/dd/yyyy HH:mm} - Calling Create() with FirstName = '{record.FirstName}'," +
                $" LastName = '{record.LastName}', DateOfBirth = '{record.DateOfBirth:MM/dd/yyyy}'," +
                $" Children = '{record.Children}', Salary = '{record.AverageSalary}, Sex = '{record.Sex}'";
            Write(startLog);
            var toReturn = this.service.CreateRecord(record);
            now = DateTime.Now;
            string endLog = $"{now:MM/dd/yyyy HH:mm} - Create() returned '{toReturn}'";
            Write(endLog);
            return toReturn;
        }

        /// <summary>
        /// Difragment file with records in FileCabinetFilesystemService.
        /// </summary>
        /// <returns>Number of difragmented records.</returns>
        public int Defragment()
        {
            DateTime now = DateTime.Now;
            string startLog = $"{now:MM/dd/yyyy HH:mm} - Calling Purge()";
            Write(startLog);
            var toReturn = this.service.Defragment();
            now = DateTime.Now;
            string endLog = $"{now:MM/dd/yyyy HH:mm} - Purge() returned '{toReturn}'";
            Write(endLog);
            return toReturn;
        }

        /// <summary>
        /// Edit record by entered Id.
        /// </summary>
        /// <param name="newRecord">New record that replace old record.</param>
        public void EditRecord(FileCabinetRecord newRecord)
        {
            if (newRecord is null)
            {
                throw new ArgumentNullException(nameof(newRecord), "Instance doesn't exist.");
            }

            DateTime now = DateTime.Now;
            string startLog = $"{now:MM/dd/yyyy HH:mm} - Calling Edit() with FirstName = '{newRecord.FirstName}'," +
                $" LastName = '{newRecord.LastName}', DateOfBirth = '{newRecord.DateOfBirth:MM/dd/yyyy}'," +
                $" Children = '{newRecord.Children}', Salary = '{newRecord.AverageSalary}, Sex = '{newRecord.Sex}'";
            Write(startLog);
            this.service.EditRecord(newRecord);
        }

        /// <summary>
        /// Find all records with entered dateofbirth.
        /// </summary>
        /// <param name="birthday">dateofbirth to find.</param>
        /// <returns>all records with entered dateofbirth.</returns>
        public IRecordIterator FindByBirthday(string birthday)
        {
            if (birthday is null)
            {
                throw new ArgumentNullException(nameof(birthday), "Instance doesn't exist.");
            }

            DateTime now = DateTime.Now;
            string startLog = $"{now:MM/dd/yyyy HH:mm} - Calling Find() with DateOfBirth to find = '{birthday}'";
            Write(startLog);
            var toReturn = this.service.FindByBirthday(birthday);
            now = DateTime.Now;
            string endLog = $"{now:MM/dd/yyyy HH:mm} - Create() returned {FromRecordsToString(toReturn)}";
            Write(endLog);
            return toReturn;
        }

        /// <summary>
        /// Find all records with entered firstname.
        /// </summary>
        /// <param name="firstName">firstname to find.</param>
        /// <returns>all records with entered firstname.</returns>
        public IRecordIterator FindByFirstName(string firstName)
        {
            if (firstName is null)
            {
                throw new ArgumentNullException(nameof(firstName), "Instance doesn't exist.");
            }

            DateTime now = DateTime.Now;
            string startLog = $"{now:MM/dd/yyyy HH:mm} - Calling Find() with FirstName to find = '{firstName}'";
            Write(startLog);
            var toReturn = this.service.FindByFirstName(firstName);
            now = DateTime.Now;
            string endLog = $"{now:MM/dd/yyyy HH:mm} - Find() returned {FromRecordsToString(toReturn)}";
            Write(endLog);
            return toReturn;
        }

        /// <summary>
        /// Find all records with entered lastname.
        /// </summary>
        /// <param name="lastName">lastname to find.</param>
        /// <returns>all records with entered lastname.</returns>
        public IRecordIterator FindByLastName(string lastName)
        {
            if (lastName is null)
            {
                throw new ArgumentNullException(nameof(lastName), "Instance doesn't exist.");
            }

            DateTime now = DateTime.Now;
            string startLog = $"{now:MM/dd/yyyy HH:mm} - Calling Find() with FirstName to find = '{lastName}'";
            Write(startLog);
            var toReturn = this.service.FindByFirstName(lastName);
            now = DateTime.Now;
            string endLog = $"{now:MM/dd/yyyy HH:mm} - Find() returned {FromRecordsToString(toReturn)}";
            Write(endLog);
            return toReturn;
        }

        /// <summary>
        /// Return all existing records.
        /// </summary>
        /// <returns>all existing records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            DateTime now = DateTime.Now;
            string startLog = $"{now:MM/dd/yyyy HH:mm} - Calling List()";
            Write(startLog);
            var toReturn = this.service.GetRecords();
            now = DateTime.Now;
            string endLog = $"{now:MM/dd/yyyy HH:mm} - List() returned {FromRecordsToString(toReturn)}";
            Write(endLog);
            return toReturn;
        }

        /// <summary>
        /// Return number of existing records.
        /// </summary>
        /// <returns>number of existing records.</returns>
        /// <param name="writeNumberRemoverRecords">Write or don't write number of removed records.</param>
        public int GetStat(bool writeNumberRemoverRecords = true)
        {
            DateTime now = DateTime.Now;
            string startLog = $"{now:MM/dd/yyyy HH:mm} - Calling Stat()";
            Write(startLog);
            var toReturn = this.service.GetStat();
            now = DateTime.Now;
            string endLog = $"{now:MM/dd/yyyy HH:mm} - Stat() returned '{toReturn}'";
            Write(endLog);
            return toReturn;
        }

        /// <summary>
        /// Check if currient Id exist.
        /// </summary>
        /// <param name="id">Id to check.</param>
        /// <returns>True if record with this id exist, false if not exist.</returns>
        public bool IsIdExist(int id)
        {
            return this.service.IsIdExist(id);
        }

        /// <summary>
        /// Make a snapshot of current state.
        /// </summary>
        /// <returns>instance of FileCabinetServiceSnapshot class.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return this.service.MakeSnapshot();
        }

        /// <summary>
        /// Remove record from service.
        /// </summary>
        /// <param name="recordId">Id of record to remove.</param>
        public void Remove(int recordId)
        {
            DateTime now = DateTime.Now;
            string startLog = $"{now:MM/dd/yyyy HH:mm} - Calling Remove() with recordId = '{recordId}'";
            Write(startLog);
            this.service.Remove(recordId);
        }

        /// <summary>
        /// Add records from to service.
        /// </summary>
        /// <param name="snapshot">item of FileCabinetServiceSnapshot were records read.</param>
        /// <returns>number of imported records.</returns>
        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            DateTime now = DateTime.Now;
            string startLog = $"{now:MM/dd/yyyy HH:mm} - Calling Restore()";
            Write(startLog);
            var toReturn = this.service.Restore(snapshot);
            now = DateTime.Now;
            string endLog = $"{now:MM/dd/yyyy HH:mm} - Restore() returned '{toReturn}'";
            Write(endLog);
            return toReturn;
        }

        private static void Write(string logToWrite)
        {
            string path = "C:\\EPAM-project\\logs.txt";
            FileMode mode = FileMode.Create;
            if (File.Exists(path))
            {
                mode = FileMode.Append;
            }

            using (FileStream stream = new FileStream(path, mode))
            {
                using (StreamWriter writer = new StreamWriter(stream, Encoding.Default))
                {
                    writer.WriteLine(logToWrite);
                }
            }
        }

        private static string FromRecordsToString(ReadOnlyCollection<FileCabinetRecord> records)
        {
            StringBuilder recordLogs = new StringBuilder();
            foreach (FileCabinetRecord record in records)
            {
                recordLogs.AppendLine($"FirstName = '{record.FirstName}', LastName = '{record.LastName}', " +
                    $"DateOfBirth = '{record.DateOfBirth:MM/dd/yyyy}', Children = '{record.Children}'," +
                    $" Salary = '{record.AverageSalary}, Sex = '{record.Sex}'");
            }

            return recordLogs.ToString();
        }

        private static string FromRecordsToString(IRecordIterator iterator)
        {
            StringBuilder recordLogs = new StringBuilder();
            while (iterator.HasMore())
            {
                FileCabinetRecord record = iterator.GetNext();
                recordLogs.AppendLine($"FirstName = '{record.FirstName}', LastName = '{record.LastName}', " +
                    $"DateOfBirth = '{record.DateOfBirth:MM/dd/yyyy}', Children = '{record.Children}'," +
                    $" Salary = '{record.AverageSalary}, Sex = '{record.Sex}'");
            }

            iterator.Reset();
            return recordLogs.ToString();
        }
    }
}
