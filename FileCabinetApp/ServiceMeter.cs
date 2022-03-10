using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FileCabinetApp
{
    /// <summary>
    /// Wrapper for FileCabinet services for measuring run time.
    /// </summary>
    public class ServiceMeter : IFileCabinetService
    {
        private IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMeter"/> class.
        /// </summary>
        /// <param name="service">one of the FileCabinet services.</param>
        public ServiceMeter(IFileCabinetService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Create a new record with inputed parameters.
        /// </summary>
        /// <param name="record">Record to create.</param>
        /// <returns>Id of created record.</returns>
        public int CreateRecord(FileCabinetRecord record)
        {
            var watches = new Stopwatch();
            watches.Start();
            int toReturn = this.service.CreateRecord(record);
            watches.Stop();
            Console.WriteLine($"Create method execution duration is {watches.ElapsedTicks} ticks.");
            return toReturn;
        }

        /// <summary>
        /// Difragment file with records in FileCabinetFilesystemService.
        /// </summary>
        /// <returns>Number of difragmented records.</returns>
        public int Defragment()
        {
            var watches = new Stopwatch();
            watches.Start();
            int toReturn = this.service.Defragment();
            watches.Stop();
            Console.WriteLine($"Purge method execution duration is {watches.ElapsedTicks} ticks.");
            return toReturn;
        }

        /// <summary>
        /// Edit record by entered Id.
        /// </summary>
        /// <param name="newRecord">New record that replace old record.</param>
        public void EditRecord(FileCabinetRecord newRecord)
        {
            var watches = new Stopwatch();
            watches.Start();
            this.service.EditRecord(newRecord);
            watches.Stop();
            Console.WriteLine($"Edit method execution duration is {watches.ElapsedTicks} ticks.");
        }

        /// <summary>
        /// Find all records with entered dateofbirth.
        /// </summary>
        /// <param name="birthday">dateofbirth to find.</param>
        /// <returns>all records with entered dateofbirth.</returns>
        public IEnumerable FindByBirthday(string birthday)
        {
            var watches = new Stopwatch();
            watches.Start();
            var toReturn = this.service.FindByBirthday(birthday);
            watches.Stop();
            Console.WriteLine($"Find method execution duration is {watches.ElapsedTicks} ticks.");
            return toReturn;
        }

        /// <summary>
        /// Find all records with entered firstname.
        /// </summary>
        /// <param name="firstName">firstname to find.</param>
        /// <returns>all records with entered firstname.</returns>
        public IEnumerable FindByFirstName(string firstName)
        {
            var watches = new Stopwatch();
            watches.Start();
            var toReturn = this.service.FindByFirstName(firstName);
            watches.Stop();
            Console.WriteLine($"Find method execution duration is {watches.ElapsedTicks} ticks.");
            return toReturn;
        }

        /// <summary>
        /// Find all records with entered lastname.
        /// </summary>
        /// <param name="lastName">lastname to find.</param>
        /// <returns>all records with entered lastname.</returns>
        public IEnumerable FindByLastName(string lastName)
        {
            var watches = new Stopwatch();
            watches.Start();
            var toReturn = this.service.FindByLastName(lastName);
            watches.Stop();
            Console.WriteLine($"Find method execution duration is {watches.ElapsedTicks} ticks.");
            return toReturn;
        }

        /// <summary>
        /// Return all existing records.
        /// </summary>
        /// <returns>all existing records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            var watches = new Stopwatch();
            watches.Start();
            var toReturn = this.service.GetRecords();
            watches.Stop();
            Console.WriteLine($"List method execution duration is {watches.ElapsedTicks} ticks.");
            return toReturn;
        }

        /// <summary>
        /// Return number of existing records.
        /// </summary>
        /// <returns>number of existing records.</returns>
        /// <param name="writeNumberRemoverRecords">Write or don't write number of removed records.</param>
        public int GetStat(bool writeNumberRemoverRecords = true)
        {
            var watches = new Stopwatch();
            watches.Start();
            var toReturn = this.service.GetStat(writeNumberRemoverRecords);
            watches.Stop();
            Console.WriteLine($"Stat method execution duration is {watches.ElapsedTicks} ticks.");
            return toReturn;
        }

        /// <summary>
        /// Insert records with given filds and values.
        /// </summary>
        /// <param name="record">record to insert.</param>
        public void Insert(FileCabinetRecord record)
        {
            var watches = new Stopwatch();
            watches.Start();
            this.service.Insert(record);
            watches.Stop();
            Console.WriteLine($"Insert method execution duration is {watches.ElapsedTicks} ticks");
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
            var watches = new Stopwatch();
            watches.Start();
            this.service.Remove(recordId);
            watches.Stop();
            Console.WriteLine($"Remove method execution duration is {watches.ElapsedTicks} ticks.");
        }

        /// <summary>
        /// Add records from to service.
        /// </summary>
        /// <param name="snapshot">item of FileCabinetServiceSnapshot were records read.</param>
        /// <returns>number of imported records.</returns>
        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            var watches = new Stopwatch();
            watches.Start();
            var toReturn = this.service.Restore(snapshot);
            watches.Stop();
            Console.WriteLine($"Restore method execution duration is {watches.ElapsedTicks} ticks.");
            return toReturn;
        }

        /// <summary>
        /// Delete records.
        /// </summary>
        /// <param name="ids">Ids of records to delete.</param>
        /// <returns>Ids of deleted records.</returns>
        public ReadOnlyCollection<int> Delete(ReadOnlyCollection<int> ids)
        {
            var watches = new Stopwatch();
            watches.Start();
            var toReturn = this.service.Delete(ids);
            watches.Stop();
            Console.WriteLine($"Delete method execution duration is {watches.ElapsedTicks} ticks.");
            return toReturn;
        }

        /// <summary>
        /// Update record.
        /// </summary>
        /// <param name="records">list of new records.</param>
        /// <returns>true - updated successfuly, false - not successfuly.</returns>
        public bool Update(ReadOnlyCollection<FileCabinetRecord> records)
        {
            var watches = new Stopwatch();
            watches.Start();
            var toReturn = this.service.Update(records);
            watches.Stop();
            Console.WriteLine($"Update method execution duration is {watches.ElapsedTicks} ticks.");
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
            var watches = new Stopwatch();
            watches.Start();
            var toReturn = this.service.SelectCommand(fildsToFind, andKeyword);
            watches.Stop();
            Console.WriteLine($"List method execution duration is {watches.ElapsedTicks} ticks.");
            return toReturn;
        }
    }
}
