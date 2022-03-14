using System.Collections;
using System.Collections.ObjectModel;
using System.Text;

namespace FileCabinetApp.Services
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
            this.service.EditRecord(newRecord);
        }

        /// <summary>
        /// Find all records with entered dateofbirth.
        /// </summary>
        /// <param name="birthday">dateofbirth to find.</param>
        /// <returns>all records with entered dateofbirth.</returns>
        public IEnumerable FindByBirthday(string birthday)
        {
            return this.service.FindByBirthday(birthday);
        }

        /// <summary>
        /// Find all records with entered firstname.
        /// </summary>
        /// <param name="firstName">firstname to find.</param>
        /// <returns>all records with entered firstname.</returns>
        public IEnumerable FindByFirstName(string firstName)
        {
            return this.service.FindByFirstName(firstName);
        }

        /// <summary>
        /// Find all records with entered lastname.
        /// </summary>
        /// <param name="lastName">lastname to find.</param>
        /// <returns>all records with entered lastname.</returns>
        public IEnumerable FindByLastName(string lastName)
        {
            return this.service.FindByLastName(lastName);
        }

        /// <summary>
        /// Return all existing records.
        /// </summary>
        /// <returns>all existing records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return this.service.GetRecords();
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
        /// Return record with current id if it exist.
        /// </summary>
        /// <param name="id">id to find.</param>
        /// <returns>Record with current id.</returns>
        public FileCabinetRecord? GetRecordById(int id)
        {
            return this.service.GetRecordById(id);
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
            string startLog = $"{now:MM/dd/yyyy HH:mm} - Calling Import()";
            Write(startLog);
            var toReturn = this.service.Restore(snapshot);
            now = DateTime.Now;
            string endLog = $"{now:MM/dd/yyyy HH:mm} - Import() returned '{toReturn}'";
            Write(endLog);
            return toReturn;
        }

        /// <summary>
        /// Insert records with given filds and values.
        /// </summary>
        /// <param name="record">record to insert.</param>
        /// <returns>true - inserted successfuly, false - not successfuly.</returns>
        public bool Insert(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), "Instance doesn't exist.");
            }

            DateTime now = DateTime.Now;
            string startLog = $"{now:MM/dd/yyyy HH:mm} - Calling Insert() with FirstName = '{record.FirstName}'," +
                $" LastName = '{record.LastName}', DateOfBirth = '{record.DateOfBirth:MM/dd/yyyy}'," +
                $" Children = '{record.Children}', Salary = '{record.AverageSalary}, Sex = '{record.Sex}'";
            Write(startLog);
            var toReturn = this.service.Insert(record);
            now = DateTime.Now;
            string endLog = $"{now:MM/dd/yyyy HH:mm} - Insert() returned '{toReturn}'";
            Write(endLog);
            return toReturn;
        }

        /// <summary>
        /// Delete records.
        /// </summary>
        /// <param name="ids">Ids of records to delete.</param>
        /// <returns>Ids of deleted records.</returns>
        public ReadOnlyCollection<int> Delete(ReadOnlyCollection<int> ids)
        {
            DateTime now = DateTime.Now;
            string startLog = $"{now:MM/dd/yyyy HH:mm} - Calling Delete()";
            Write(startLog);
            var toReturn = this.service.Delete(ids);
            now = DateTime.Now;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in toReturn)
            {
                stringBuilder.Append(item);
            }

            string endLog = $"{now:MM/dd/yyyy HH:mm} - Delete() returned '{stringBuilder}'";
            Write(endLog);
            return toReturn;
        }

        /// <summary>
        /// Update record.
        /// </summary>
        /// <param name="records">list of new records.</param>
        /// <returns>true - updated successfuly, false - not successfuly.</returns>
        public bool Update(ReadOnlyCollection<FileCabinetRecord> records)
        {
            DateTime now = DateTime.Now;
            string startLog = $"{now:MM/dd/yyyy HH:mm} - Calling Update()";
            Write(startLog);
            var toReturn = this.service.Update(records);
            now = DateTime.Now;
            string endLog = $"{now:MM/dd/yyyy HH:mm} - Update() returned '{toReturn}'";
            Write(endLog);
            return toReturn;
        }

        /// <summary>
        /// Select records by criteries.
        /// </summary>
        /// <param name="fildsToFind">filds to find by.</param>
        /// <param name="andKeyword">true - use 'and' keyword, false - use 'or' keyword.</param>
        /// <returns>selected records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> SelectCommand(string[] fildsToFind, bool andKeyword)
        {
            DateTime now = DateTime.Now;
            string startLog = $"{now:MM/dd/yyyy HH:mm} - Calling Select()";
            Write(startLog);
            var toReturn = this.service.SelectCommand(fildsToFind, andKeyword);
            now = DateTime.Now;
            string endLog = $"{now:MM/dd/yyyy HH:mm} - Select() returned {FromRecordsToString(toReturn)}";
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
    }
}
