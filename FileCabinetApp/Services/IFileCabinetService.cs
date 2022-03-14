using System.Collections;
using System.Collections.ObjectModel;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Represent rules for working with FileCabinetRecord.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// Check if currient Id exist.
        /// </summary>
        /// <param name="id">Id to check.</param>
        /// <returns>True if record with this id exist, false if not exist.</returns>
        public bool IsIdExist(int id);

        /// <summary>
        /// Return record with current id if it exist.
        /// </summary>
        /// <param name="id">id to find.</param>
        /// <returns>Record with current id.</returns>
        public FileCabinetRecord? GetRecordById(int id);

        /// <summary>
        /// Create a new record with inputed parameters.
        /// </summary>
        /// <param name="record">Record to create.</param>
        /// <returns>Id of created record.</returns>
        public int CreateRecord(FileCabinetRecord record);

        /// <summary>
        /// Return all existing records.
        /// </summary>
        /// <returns>all existing records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>
        /// Return number of existing records.
        /// </summary>
        /// <returns>number of existing records.</returns>
        /// <param name="writeNumberRemoverRecords">Write or don't write number of removedrecords.</param>
        public int GetStat(bool writeNumberRemoverRecords = true);

        /// <summary>
        /// Edit record by entered Id.
        /// </summary>
        /// <param name="newRecord">New record that replace old record.</param>
        public void EditRecord(FileCabinetRecord newRecord);

        /// <summary>
        /// Find all records with entered firstname.
        /// </summary>
        /// <param name="firstName">firstname to find.</param>
        /// <returns>all records with entered firstname.</returns>
        public IEnumerable FindByFirstName(string firstName);

        /// <summary>
        /// Find all records with entered lastname.
        /// </summary>
        /// <param name="lastName">lastname to find.</param>
        /// <returns>all records with entered lastname.</returns>
        public IEnumerable FindByLastName(string lastName);

        /// <summary>
        /// Find all records with entered dateofbirth.
        /// </summary>
        /// <param name="birthday">dateofbirth to find.</param>
        /// <returns>all records with entered dateofbirth.</returns>
        public IEnumerable FindByBirthday(string birthday);

        /// <summary>
        /// Make a snapshot of current state.
        /// </summary>
        /// <returns>instance of FileCabinetServiceSnapshot class.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot();

        /// <summary>
        /// Add records from to service.
        /// </summary>
        /// <param name="snapshot">item of FileCabinetServiceSnapshot were records read.</param>
        /// <returns>number of imported records.</returns>
        public int Restore(FileCabinetServiceSnapshot snapshot);

        /// <summary>
        /// Remove record from service.
        /// </summary>
        /// <param name="recordId">Id of record to remove.</param>
        public void Remove(int recordId);

        /// <summary>
        /// Difragment file with records in FileCabinetFilesystemService.
        /// </summary>
        /// <returns>Number of difragmented records.</returns>
        public int Defragment();

        /// <summary>
        /// Insert records with given filds and values.
        /// </summary>
        /// <param name="record">record to insert.</param>
        /// <returns>true - inserted successfuly, false - not successfuly.</returns>
        public bool Insert(FileCabinetRecord record);

        /// <summary>
        /// Delete record with given values.
        /// </summary>
        /// <param name="ids">Ids of records to delete.</param>
        /// <returns>Ids of deleted records.</returns>
        public ReadOnlyCollection<int> Delete(ReadOnlyCollection<int> ids);

        /// <summary>
        /// Update record.
        /// </summary>
        /// <param name="records">list of new records.</param>
        /// <returns>true - updated successfuly, false - not successfuly.</returns>
        public bool Update(ReadOnlyCollection<FileCabinetRecord> records);

        /// <summary>
        /// Select records by criteries.
        /// </summary>
        /// <param name="fildsToFind">filds to find by.</param>
        /// <param name="andKeyword">true - use 'and' keyword, false - use 'or' keyword.</param>
        /// <returns>selected records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> SelectCommand(string[] fildsToFind, bool andKeyword);
    }
}
