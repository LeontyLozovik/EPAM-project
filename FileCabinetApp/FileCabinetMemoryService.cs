using System.Collections.ObjectModel;
using System.Globalization;

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
        /// Returns validator.
        /// </summary>
        /// <returns>type of validation.</returns>
        public IRecordValidator GetValidationType()
        {
            return this.validator;
        }

        /// <summary>
        /// Create a new record with inputed parameters.
        /// </summary>
        /// <param name="record">Record to create.</param>
        /// <returns>Id of created record.</returns>
        public int CreateRecord(FileCabinetRecord record)
        {
            this.validator.ValidateParameters(record);
            record.Id = this.list.Count + 1;

            this.list.Add(record);
            this.AddNamesToDictionary(record.FirstName, record, this.firstNameDictionary);
            this.AddNamesToDictionary(record.LastName, record, this.lastNameDictionary);
            this.AddDateToDictionary(record.DateOfBirth, record, this.dateofbirthDictionary);
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
        public int GetStat()
        {
            return this.list.Count;
        }

        /// <summary>
        /// Edit record by entered Id.
        /// </summary>
        /// <param name="newRecord">New record that replace old record.</param>
        public void EditRecord(FileCabinetRecord newRecord)
        {
            this.validator.ValidateParameters(newRecord);
            if (newRecord.Id > this.list.Count)
            {
                throw new ArgumentException("There is no record with this Id.");
            }

            FileCabinetRecord incorrectRecord = this.list[newRecord.Id - 1];

            this.list.RemoveAt(newRecord.Id - 1);
            this.list.Insert(newRecord.Id - 1, newRecord);
            this.EditDictionaries(newRecord.FirstName, newRecord.LastName, newRecord.DateOfBirth, newRecord, incorrectRecord);
        }

        /// <summary>
        /// Find all records with entered firstname.
        /// </summary>
        /// <param name="firstName">firstname to find.</param>
        /// <returns>all records with entered firstname.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (!this.firstNameDictionary.ContainsKey(firstName.ToLowerInvariant()))
            {
                throw new ArgumentException("Nothing found for your request.");
            }

            var selectedRecords = this.firstNameDictionary[firstName.ToLowerInvariant()];
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
            if (!this.lastNameDictionary.ContainsKey(lastName.ToLowerInvariant()))
            {
                throw new ArgumentException("Nothing found for your request.");
            }

            var selectedRecords = this.lastNameDictionary[lastName.ToLowerInvariant()];
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
                Console.WriteLine("Please check your input.");
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
                throw new ArgumentNullException(nameof(FileCabinetServiceSnapshot));
            }

            var recordsFromFile = snapshot.Records;
            if (recordsFromFile is null)
            {
                throw new ArgumentNullException(nameof(IReadOnlyCollection<FileCabinetRecord>));
            }

            int numberOfRecords = recordsFromFile.Count;
            var recordEnumerator = recordsFromFile.GetEnumerator();
            recordEnumerator.MoveNext();

            for (int i = 0; i < numberOfRecords; i++)
            {
                if (recordEnumerator.Current.Id <= this.GetStat())
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
        /// Add records with firtname or lastname key to the dictionary.
        /// </summary>
        /// <param name="name">firstname or lastname key.</param>
        /// <param name="record">record to add.</param>
        /// <param name="dictionary">dictionary to add.</param>
        private void AddNamesToDictionary(string name, FileCabinetRecord record, Dictionary<string, List<FileCabinetRecord>> dictionary)
        {
            string keyName = name.ToLowerInvariant();
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
        private void AddDateToDictionary(DateTime date, FileCabinetRecord record, Dictionary<DateTime, List<FileCabinetRecord>> dictionary)
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
        /// Edit values in all dictionaries after user edited record.
        /// </summary>
        /// <param name="firstName">New firstname.</param>
        /// <param name="lastName">New lastname.</param>
        /// <param name="dateOfBirth">New dateofbirth.</param>
        /// <param name="newRecord">New record.</param>
        /// <param name="incorrectRecord">This record before etion.</param>
        private void EditDictionaries(string firstName, string lastName, DateTime dateOfBirth, FileCabinetRecord newRecord, FileCabinetRecord incorrectRecord)
        {
            List<FileCabinetRecord> recordToRemove = this.firstNameDictionary[incorrectRecord.FirstName.ToLowerInvariant()];
            int removeIndex = recordToRemove.FindIndex(0, recordToRemove.Count, record => record.Id == incorrectRecord.Id);
            recordToRemove.RemoveAt(removeIndex);
            this.AddNamesToDictionary(firstName, newRecord, this.firstNameDictionary);

            recordToRemove = this.lastNameDictionary[incorrectRecord.LastName.ToLowerInvariant()];
            removeIndex = recordToRemove.FindIndex(0, recordToRemove.Count, record => record.Id == incorrectRecord.Id);
            recordToRemove.RemoveAt(removeIndex);
            this.AddNamesToDictionary(lastName, newRecord, this.lastNameDictionary);

            recordToRemove = this.dateofbirthDictionary[incorrectRecord.DateOfBirth];
            removeIndex = recordToRemove.FindIndex(0, recordToRemove.Count, record => record.Id == incorrectRecord.Id);
            recordToRemove.RemoveAt(removeIndex);
            this.AddDateToDictionary(dateOfBirth, newRecord, this.dateofbirthDictionary);
        }
    }
}
