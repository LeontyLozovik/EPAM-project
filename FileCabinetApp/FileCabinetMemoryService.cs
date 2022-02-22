﻿using System.Collections.ObjectModel;
using System.Globalization;
using FileCabinetApp.RecordValidators;

namespace FileCabinetApp
{
    /// <summary>
    /// Do manipulations with records in memory.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateofbirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();
        private IRecordValidator validator;

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
            if (this.GetStat(false) >= id)
            {
                return true;
            }

            return false;
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
            record.Id = this.list.Count + 1;

            this.list.Add(record);
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
            ReadOnlyCollection<FileCabinetRecord> allRecords = new ReadOnlyCollection<FileCabinetRecord>(this.list);
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

            return this.list.Count;
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
            if (newRecord.Id > this.list.Count)
            {
                throw new ArgumentException("There is no record with this Id.");
            }

            FileCabinetRecord incorrectRecord = this.list[newRecord.Id - 1];

            this.list.RemoveAt(newRecord.Id - 1);
            this.list.Insert(newRecord.Id - 1, newRecord);
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
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
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
            return foundRecords;
        }

        /// <summary>
        /// Find all records with entered lastname.
        /// </summary>
        /// <param name="lastName">lastname to find.</param>
        /// <returns>all records with entered lastname.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
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
            return foundRecords;
        }

        /// <summary>
        /// Find all records with entered dateofbirth.
        /// </summary>
        /// <param name="birthday">dateofbirth to find.</param>
        /// <returns>all records with entered dateofbirth.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByBirthday(string birthday)
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
            return foundRecords;
        }

        /// <summary>
        /// Make a snapshot of current state.
        /// </summary>
        /// <returns>instance of FileCabinetServiceSnapshot class.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.list.ToArray());
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
                if (recordEnumerator.Current.Id <= this.GetStat(false))
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
            FileCabinetRecord? incorrectRecord = this.list[recordId - 1];
            if (incorrectRecord is null)
            {
                throw new ArgumentNullException($"Record #{recordId} doesn't exists");
            }

            this.list.Remove(incorrectRecord);
            this.RemoveFromDictionaries(incorrectRecord);
            this.ReindecsingId();
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

        /// <summary>
        /// Reindecsing records in list.
        /// </summary>
        private void ReindecsingId()
        {
            int startId = 1;
            foreach (var record in this.list)
            {
                record.Id = startId;
                startId++;
            }
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
