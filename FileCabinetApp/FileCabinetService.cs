using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Do manipulations with records.
    /// </summary>
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateofbirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();

        /// <summary>
        /// Create a new record with inputed parameters.
        /// </summary>
        /// <param name="record">Record to create.</param>
        /// <returns>Id of created record.</returns>
        public int CreateRecord(FileCabinetRecord record)
        {
            this.CreateValidator().ValidateParameters(record);

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
        public FileCabinetRecord[] GetRecords()
        {
            FileCabinetRecord[] allRecords = this.list.ToArray();
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
            this.CreateValidator().ValidateParameters(newRecord);

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
        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            FileCabinetRecord[] foundRecords = Array.Empty<FileCabinetRecord>();
            if (!string.IsNullOrEmpty(firstName))
            {
                var selectedRecords = this.firstNameDictionary[firstName.ToLowerInvariant()];
                foundRecords = selectedRecords.ToArray();
                return foundRecords;
            }

            return foundRecords;
        }

        /// <summary>
        /// Find all records with entered lastname.
        /// </summary>
        /// <param name="lastName">lastname to find.</param>
        /// <returns>all records with entered lastname.</returns>
        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            FileCabinetRecord[] foundRecords = Array.Empty<FileCabinetRecord>();
            if (!string.IsNullOrEmpty(lastName))
            {
                var selectedRecords = this.lastNameDictionary[lastName.ToLowerInvariant()];
                foundRecords = selectedRecords.ToArray();
                return foundRecords;
            }

            return foundRecords;
        }

        /// <summary>
        /// Find all records with entered dateofbirth.
        /// </summary>
        /// <param name="birthday">dateofbirth to find.</param>
        /// <returns>all records with entered dateofbirth.</returns>
        public FileCabinetRecord[] FindByBirthday(string birthday)
        {
            DateTime dateToFind;
            if (!DateTime.TryParse(birthday, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out dateToFind))
            {
                Console.WriteLine("Please check your input.");
            }

            var selectedRecords = this.dateofbirthDictionary[dateToFind];
            FileCabinetRecord[] result = selectedRecords.ToArray();
            return result;
        }

        /// <summary>
        /// Create Validator for parameters.
        /// </summary>
        /// <returns>required validator.</returns>
        protected virtual IRecordValidator CreateValidator()
        {
            return new DefaultValidator();
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
