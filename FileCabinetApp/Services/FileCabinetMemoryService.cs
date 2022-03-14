using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using FileCabinetApp.Iterators;
using FileCabinetApp.RecordValidators;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Do manipulations with records in memory.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private readonly List<FileCabinetRecord> listOfRecords = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateofbirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();
        private IRecordValidator validator;
        private Dictionary<string, ReadOnlyCollection<FileCabinetRecord>> cache = new Dictionary<string, ReadOnlyCollection<FileCabinetRecord>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="validator">required validator.</param>
        public FileCabinetMemoryService(IRecordValidator validator)
        {
            this.validator = validator;
        }

        /// <summary>
        /// Check if currient Id exist.
        /// </summary>
        /// /// <param name="id">Id to check.</param>
        /// <returns>True if record with this id exist, false if not exist.</returns>
        public bool IsIdExist(int id)
        {
            return this.listOfRecords.Exists(record => record.Id == id);
        }

        /// <summary>
        /// Return record with current id if it exist.
        /// </summary>
        /// <param name="id">id to find.</param>
        /// <returns>Record with current id.</returns>
        public FileCabinetRecord? GetRecordById(int id)
        {
            return this.listOfRecords.Find(record => record.Id == id);
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

            this.validator.ValidateParameters(record);
            if (record.Id == 0)
            {
                record.Id = this.FirstFreeId();
            }

            this.listOfRecords.Add(record);
            AddNamesToDictionary(record.FirstName, record, this.firstNameDictionary);
            AddNamesToDictionary(record.LastName, record, this.lastNameDictionary);
            AddDateToDictionary(record.DateOfBirth, record, this.dateofbirthDictionary);
            return record.Id;
        }

        /// <summary>
        /// Return all existing records.
        /// </summary>
        /// <returns>all existing records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            ReadOnlyCollection<FileCabinetRecord> allRecords = new ReadOnlyCollection<FileCabinetRecord>(this.listOfRecords);
            return allRecords;
        }

        /// <summary>
        /// Return number of existing records.
        /// </summary>
        /// <returns>number of existing records.</returns>
        /// <param name="writeNumberRemoverRecords">Write or don't write number of removedrecords.</param>>
        public int GetStat(bool writeNumberRemoverRecords = true)
        {
            if (writeNumberRemoverRecords)
            {
                Console.WriteLine("0 records removed.");
            }

            return this.listOfRecords.Count;
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

            this.validator.ValidateParameters(newRecord);
            FileCabinetRecord? incorrectRecord = this.listOfRecords.Find(record => record.Id == newRecord.Id);
            if (incorrectRecord is null)
            {
                throw new ArgumentException($"There is no record with id = {newRecord.Id}.");
            }

            int index = this.listOfRecords.IndexOf(incorrectRecord);
            this.listOfRecords.RemoveAt(index);
            this.listOfRecords.Insert(index, newRecord);
            this.RemoveFromDictionaries(incorrectRecord);
            AddNamesToDictionary(newRecord.FirstName, newRecord, this.firstNameDictionary);
            AddNamesToDictionary(newRecord.LastName, newRecord, this.lastNameDictionary);
            AddDateToDictionary(newRecord.DateOfBirth, newRecord, this.dateofbirthDictionary);
        }

        /// <summary>
        /// Find all records with entered firstname.
        /// </summary>
        /// <param name="firstName">firstname to find.</param>
        /// <returns>all records with entered firstname.</returns>
        public IEnumerable FindByFirstName(string firstName)
        {
            if (firstName is null)
            {
                throw new ArgumentNullException(nameof(firstName), "Instance doesn't exist.");
            }

            if (!this.firstNameDictionary.ContainsKey(firstName.ToUpperInvariant()))
            {
                throw new ArgumentException("Nothing found for your request.");
            }

            var selectedRecords = this.firstNameDictionary[firstName.ToUpperInvariant()];
            ReadOnlyCollection<FileCabinetRecord> foundRecords = new ReadOnlyCollection<FileCabinetRecord>(selectedRecords);
            MemoryIterator iterator = new MemoryIterator(foundRecords);
            return iterator;
        }

        /// <summary>
        /// Find all records with entered lastname.
        /// </summary>
        /// <param name="lastName">lastname to find.</param>
        /// <returns>all records with entered lastname.</returns>
        public IEnumerable FindByLastName(string lastName)
        {
            if (lastName is null)
            {
                throw new ArgumentNullException(nameof(lastName), "Instance doesn't exist.");
            }

            if (!this.lastNameDictionary.ContainsKey(lastName.ToUpperInvariant()))
            {
                throw new ArgumentException("Nothing found for your request.");
            }

            var selectedRecords = this.lastNameDictionary[lastName.ToUpperInvariant()];
            ReadOnlyCollection<FileCabinetRecord> foundRecords = new ReadOnlyCollection<FileCabinetRecord>(selectedRecords);
            MemoryIterator iterator = new MemoryIterator(foundRecords);
            return iterator;
        }

        /// <summary>
        /// Find all records with entered dateofbirth.
        /// </summary>
        /// <param name="birthday">dateofbirth to find.</param>
        /// <returns>all records with entered dateofbirth.</returns>
        public IEnumerable FindByBirthday(string birthday)
        {
            DateTime dateToFind;
            if (!DateTime.TryParse(birthday, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out dateToFind))
            {
                throw new ArgumentException("Please check your input.");
            }
            else if (!this.dateofbirthDictionary.ContainsKey(dateToFind))
            {
                throw new ArgumentException("Nothing found for your request.");
            }

            var selectedRecords = this.dateofbirthDictionary[dateToFind];
            ReadOnlyCollection<FileCabinetRecord> foundRecords = new ReadOnlyCollection<FileCabinetRecord>(selectedRecords);
            MemoryIterator iterator = new MemoryIterator(foundRecords);
            return iterator;
        }

        /// <summary>
        /// Make a snapshot of current state.
        /// </summary>
        /// <returns>instance of FileCabinetServiceSnapshot class.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.listOfRecords.ToArray());
        }

        /// <summary>
        /// Add records to service.
        /// </summary>
        /// <param name="snapshot">item of FileCabinetServiceSnapshot were records read.</param>
        /// <returns>number of imported records.</returns>
        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot is null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            var recordsFromFile = snapshot.Records;
            if (recordsFromFile is null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            var recordEnumerator = recordsFromFile.GetEnumerator();
            recordEnumerator.MoveNext();
            int numberOfRecords = recordsFromFile.Count;

            for (int i = 0; i < numberOfRecords; i++)
            {
                if (this.IsIdExist(recordEnumerator.Current.Id))
                {
                    try
                    {
                        this.EditRecord(recordEnumerator.Current);
                    }
                    catch (ArgumentException)
                    {
                        Console.WriteLine($"{recordEnumerator.Current.Id} record break data rules and will be skipped");
                    }

                    recordEnumerator.MoveNext();
                }
                else
                {
                    try
                    {
                        recordEnumerator.Current.Id = this.FirstFreeId();
                        this.CreateRecord(recordEnumerator.Current);
                    }
                    catch (ArgumentException)
                    {
                        Console.WriteLine($"{recordEnumerator.Current.Id} record break data rules and will be skipped");
                    }

                    recordEnumerator.MoveNext();
                }
            }

            return numberOfRecords;
        }

        /// <summary>
        /// Remove record from service.
        /// </summary>
        /// <param name="recordId">Id of record to remove.</param>
        public void Remove(int recordId)
        {
            FileCabinetRecord? incorrectRecord = this.listOfRecords.Find(record => record.Id == recordId);
            if (incorrectRecord is null)
            {
                throw new ArgumentNullException($"Record #{recordId} doesn't exists");
            }

            this.listOfRecords.Remove(incorrectRecord);
            this.RemoveFromDictionaries(incorrectRecord);
        }

        /// <summary>
        /// Difragment file with records in FileCabinetFilesystemService.
        /// </summary>
        /// <returns>0 - because there is nothing to defragment.</returns>
        public int Defragment()
        {
            Console.WriteLine("Memory service don't support 'purge' command");
            return 0;
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

            bool succsses = true;
            if (this.IsIdExist(record.Id))
            {
                Console.WriteLine("Record with this Id already exists. Rewrite? [y/n]");
                bool notEnd = true;
                do
                {
                    switch (Console.ReadLine())
                    {
                        case "y":
                            try
                            {
                                this.EditRecord(record);
                            }
                            catch (ArgumentException ex)
                            {
                                Console.WriteLine(ex.Message);
                                succsses = false;
                            }

                            notEnd = false;
                            break;
                        case "n":
                            notEnd = false;
                            break;
                    }
                }
                while (notEnd);
            }
            else
            {
                this.CreateRecord(record);
            }

            this.cache.Clear();
            return succsses;
        }

        /// <summary>
        /// Delete records.
        /// </summary>
        /// <param name="ids">Ids of records to delete.</param>
        /// <returns>Ids of deleted records.</returns>
        public ReadOnlyCollection<int> Delete(ReadOnlyCollection<int> ids)
        {
            List<int> idofDeletedRecords = new List<int>();
            if (ids is null)
            {
                return new ReadOnlyCollection<int>(idofDeletedRecords);
            }

            foreach (var id in ids)
            {
                try
                {
                    this.Remove(id);
                    idofDeletedRecords.Add(id);
                }
                catch (ArgumentNullException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            this.cache.Clear();
            return new ReadOnlyCollection<int>(idofDeletedRecords);
        }

        /// <summary>
        /// Update record.
        /// </summary>
        /// <param name="records">list of new records.</param>
        /// <returns>true - updated successfuly, false - not successfuly.</returns>
        public bool Update(ReadOnlyCollection<FileCabinetRecord> records)
        {
            if (records is null)
            {
                throw new ArgumentNullException(nameof(records), "Instance doesn't exist.");
            }

            bool allUpdeted = true;
            foreach (var record in records)
            {
                try
                {
                    this.EditRecord(record);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                    allUpdeted = false;
                }
            }

            this.cache.Clear();
            return allUpdeted;
        }

        /// <summary>
        /// Select records by criteries.
        /// </summary>
        /// <param name="fildsToFind">filds to find by.</param>
        /// <param name="andKeyword">true - use 'and' keyword, false - use 'or' keyword.</param>
        /// <returns>selected records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> SelectCommand(string[] fildsToFind, bool andKeyword)
        {
            if (fildsToFind is null)
            {
                throw new ArgumentNullException(nameof(fildsToFind), "Filds to select can't be null.");
            }

            StringBuilder stringBuilderKey = new StringBuilder(andKeyword.ToString());
            foreach (var fild in fildsToFind)
            {
                stringBuilderKey.Append(fild.ToUpperInvariant());
            }

            string key = stringBuilderKey.ToString();
            if (this.cache.ContainsKey(key))
            {
                return this.cache[key];
            }

            int id = 0;
            int count = 1;
            var listOfRecords = new List<FileCabinetRecord>();
            for (int i = 0; i < fildsToFind.Length; i++)
            {
                if (string.Equals("and", fildsToFind[i], StringComparison.OrdinalIgnoreCase))
                {
                    count++;
                }
                else if (!string.Equals("or", fildsToFind[i], StringComparison.OrdinalIgnoreCase))
                {
                    switch (fildsToFind[i].ToUpperInvariant())
                    {
                        case "ID":
                            if (!int.TryParse(fildsToFind[i + 1], out id))
                            {
                                throw new ArgumentException("Incorrect Id.");
                            }

                            FileCabinetRecord? recordById = this.GetRecordById(id);
                            if (!(recordById is null))
                            {
                                listOfRecords.Add(recordById);
                            }

                            i++;
                            break;
                        case "FIRSTNAME":
                            try
                            {
                                var withFirstnameRecords = this.FindByFirstName(fildsToFind[i + 1]);
                                foreach (FileCabinetRecord record in withFirstnameRecords)
                                {
                                    listOfRecords.Add(record);
                                }
                            }
                            catch (ArgumentNullException exeption)
                            {
                                Console.WriteLine(exeption.Message);
                            }
                            catch (ArgumentException)
                            {
                            }

                            i++;
                            break;
                        case "LASTNAME":
                            try
                            {
                                var withLastnameRecords = this.FindByLastName(fildsToFind[i + 1]);
                                foreach (FileCabinetRecord record in withLastnameRecords)
                                {
                                    listOfRecords.Add(record);
                                }
                            }
                            catch (ArgumentNullException exeption)
                            {
                                Console.WriteLine(exeption.Message);
                            }
                            catch (ArgumentException)
                            {
                            }

                            i++;
                            break;
                        case "DATEOFBIRTH":
                            try
                            {
                                var withDateOfBirthRecords = this.FindByBirthday(fildsToFind[i + 1]);
                                foreach (FileCabinetRecord record in withDateOfBirthRecords)
                                {
                                    listOfRecords.Add(record);
                                }
                            }
                            catch (ArgumentNullException exeption)
                            {
                                Console.WriteLine(exeption.Message);
                            }
                            catch (ArgumentException)
                            {
                            }

                            i++;
                            break;
                        default:
                            Console.WriteLine($"Incorrect name of field: {fildsToFind[i]}");
                            i++;
                            break;
                    }
                }
            }

            if (andKeyword)
            {
                listOfRecords.Sort((x, y) =>
                {
                    return x.Id.CompareTo(y.Id);
                });
                var result = GetRecordsToSelect(new ReadOnlyCollection<FileCabinetRecord>(listOfRecords), id, count);
                this.cache.Add(key, result);
                return result;
            }
            else
            {
                List<FileCabinetRecord> uniqueList = new List<FileCabinetRecord>();
                foreach (var record in listOfRecords)
                {
                    if (!uniqueList.Exists(item => item.Id == record.Id))
                    {
                        uniqueList.Add(record);
                    }
                }

                var result = new ReadOnlyCollection<FileCabinetRecord>(uniqueList);
                this.cache.Add(key, result);
                return result;
            }
        }

        /// <summary>
        /// Add records with firstname or lastname key to the dictionary.
        /// </summary>
        /// <param name="name">firstname or lastname key.</param>
        /// <param name="record">record to add.</param>
        /// <param name="dictionary">dictionary to add.</param>
        private static void AddNamesToDictionary(string name, FileCabinetRecord record, Dictionary<string, List<FileCabinetRecord>> dictionary)
        {
            string keyName = name.ToUpperInvariant();
            if (dictionary.ContainsKey(keyName))
            {
                List<FileCabinetRecord> valueList = dictionary[keyName];
                valueList.Add(record);
                dictionary[keyName] = valueList;
            }
            else
            {
                List<FileCabinetRecord> valueList = new List<FileCabinetRecord>();
                valueList.Add(record);
                dictionary.Add(keyName, valueList);
            }
        }

        /// <summary>
        /// Add records with dateofbirth key to the dictionary.
        /// </summary>
        /// <param name="date">dateofbirth key.</param>
        /// <param name="record">record to add.</param>
        /// <param name="dictionary">dictionary to add.</param>
        private static void AddDateToDictionary(DateTime date, FileCabinetRecord record, Dictionary<DateTime, List<FileCabinetRecord>> dictionary)
        {
            if (dictionary.ContainsKey(date))
            {
                List<FileCabinetRecord> valueList = dictionary[date];
                valueList.Add(record);
                dictionary[date] = valueList;
            }
            else
            {
                List<FileCabinetRecord> valueList = new List<FileCabinetRecord>();
                valueList.Add(record);
                dictionary.Add(date, valueList);
            }
        }

        private static ReadOnlyCollection<FileCabinetRecord> GetRecordsToSelect(ReadOnlyCollection<FileCabinetRecord> listOfRecords, int id, int minCount)
        {
            var result = new List<FileCabinetRecord>();
            if (listOfRecords.Count < minCount)
            {
                return new ReadOnlyCollection<FileCabinetRecord>(result);
            }

            if (listOfRecords.Count == 1)
            {
                return listOfRecords;
            }

            int counter = 1;
            int maxCount = 0;
            for (int i = 0; i < listOfRecords.Count; i++)
            {
                if (i + 1 == listOfRecords.Count)
                {
                    if (listOfRecords[i].Id == listOfRecords[i - 1].Id)
                    {
                        if (counter == maxCount)
                        {
                            if (counter >= minCount)
                            {
                                result.Add(listOfRecords[i]);
                            }

                            counter = 1;
                        }
                        else if (counter > maxCount)
                        {
                            if (counter >= minCount)
                            {
                                result.Clear();
                                result.Add(listOfRecords[i]);
                            }

                            maxCount = counter;
                            counter = 1;
                        }
                    }
                    else
                    {
                        if (counter == maxCount)
                        {
                            if (counter >= minCount)
                            {
                                result.Add(listOfRecords[i]);
                            }

                            counter = 1;
                        }
                        else if (counter > maxCount)
                        {
                            if (counter >= minCount)
                            {
                                result.Clear();
                                result.Add(listOfRecords[i]);
                            }

                            maxCount = counter;
                            counter = 1;
                        }
                    }

                    return new ReadOnlyCollection<FileCabinetRecord>(result);
                }

                if (listOfRecords[i].Id == listOfRecords[i + 1].Id)
                {
                    counter++;
                }
                else
                {
                    if (counter == maxCount)
                    {
                        if (counter >= minCount)
                        {
                            result.Add(listOfRecords[i]);
                        }

                        counter = 1;
                    }
                    else if (counter > maxCount)
                    {
                        if (counter >= minCount)
                        {
                            result.Clear();
                            result.Add(listOfRecords[i]);
                        }

                        maxCount = counter;
                        counter = 1;
                    }
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(result);
        }

        private int FirstFreeId()
        {
            List<int> freeId = new List<int>() { 1 };
            int nextExpectedId = 1;
            foreach (var record in this.listOfRecords)
            {
                if (freeId.Contains(record.Id))
                {
                    freeId.Remove(record.Id);
                }
                else if (record.Id != nextExpectedId)
                {
                    nextExpectedId++;
                }

                nextExpectedId++;
                freeId.Add(nextExpectedId);
            }

            return freeId[0];
        }

        /// <summary>
        /// Edit values in all dictionaries after user edited record.
        /// </summary>
        /// <param name="incorrectRecord">Record to remove from dictionaries.</param>
        private void RemoveFromDictionaries(FileCabinetRecord incorrectRecord)
        {
            List<FileCabinetRecord> recordToRemove = this.firstNameDictionary[incorrectRecord.FirstName.ToUpperInvariant()];
            int removeIndex = recordToRemove.FindIndex(0, recordToRemove.Count, record => record.Id == incorrectRecord.Id);
            recordToRemove.RemoveAt(removeIndex);

            recordToRemove = this.lastNameDictionary[incorrectRecord.LastName.ToUpperInvariant()];
            removeIndex = recordToRemove.FindIndex(0, recordToRemove.Count, record => record.Id == incorrectRecord.Id);
            recordToRemove.RemoveAt(removeIndex);

            recordToRemove = this.dateofbirthDictionary[incorrectRecord.DateOfBirth];
            removeIndex = recordToRemove.FindIndex(0, recordToRemove.Count, record => record.Id == incorrectRecord.Id);
            recordToRemove.RemoveAt(removeIndex);
        }
    }
}
