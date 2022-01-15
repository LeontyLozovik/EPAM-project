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
            if (record.FirstName is null)
            {
                throw new ArgumentNullException(record.FirstName, "First name can't be null.");
            }
            else if (string.IsNullOrWhiteSpace(record.FirstName) || record.FirstName.Length < 2 || record.FirstName.Length > 60)
            {
                throw new ArgumentException("Incorrect first name! First name should be grater then 2, less then 60 and can't be white space.", record.FirstName);
            }

            if (record.LastName is null)
            {
                throw new ArgumentNullException(record.LastName, "Last name can't be null.");
            }
            else if (string.IsNullOrWhiteSpace(record.LastName) || record.LastName.Length < 2 || record.LastName.Length > 60)
            {
                throw new ArgumentException("Incorrect last name! Last name should be grater then 2, less then 60 and can't be white space.", record.LastName);
            }

            if (record.Children < 0)
            {
                throw new ArgumentException("Number of children can't be less then 0.");
            }

            if (record.AverageSalary < 0 || record.AverageSalary > 1000000000)
            {
                throw new ArgumentException("Average salary can't be less then 0 or grater then 1 billion.");
            }

            if (record.Sex != 'm' && record.Sex != 'w')
            {
                throw new ArgumentException("Sorry, but your sex can be m - men or w - women only.");
            }

            DateTime oldest = new DateTime(1950, 1, 1);
            DateTime now = DateTime.Now;
            if (record.DateOfBirth < oldest || record.DateOfBirth > now)
            {
                throw new ArgumentException("Sorry but minimal date of birth - 01-Jan-1950 and maxsimum - current date");
            }

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
            if (newRecord.FirstName is null)
            {
                throw new ArgumentNullException(newRecord.FirstName, "First name can't be null.");
            }
            else if (string.IsNullOrWhiteSpace(newRecord.FirstName) || newRecord.FirstName.Length < 2 || newRecord.FirstName.Length > 60)
            {
                throw new ArgumentException("Incorrect first name! First name should be grater then 2, less then 60 and can't be white space.", newRecord.FirstName);
            }

            if (newRecord.LastName is null)
            {
                throw new ArgumentNullException(newRecord.LastName, "Last name can't be null.");
            }
            else if (string.IsNullOrWhiteSpace(newRecord.LastName) || newRecord.LastName.Length < 2 || newRecord.LastName.Length > 60)
            {
                throw new ArgumentException("Incorrect last name! Last name should be grater then 2, less then 60 and can't be white space.", newRecord.LastName);
            }

            if (newRecord.Children < 0)
            {
                throw new ArgumentException("Number of children can't be less then 0.");
            }

            if (newRecord.AverageSalary < 0 || newRecord.AverageSalary > 1000000000)
            {
                throw new ArgumentException("Average salary can't be less then 0 or grater then 1 billion.");
            }

            if (newRecord.Sex != 'm' && newRecord.Sex != 'w')
            {
                throw new ArgumentException("Sorry, but your sex can be m - men or w - women only.");
            }

            DateTime oldest = new DateTime(1950, 1, 1);
            DateTime now = DateTime.Now;
            if (newRecord.DateOfBirth < oldest || newRecord.DateOfBirth > now)
            {
                throw new ArgumentException("Sorry but minimal date of birth - 01-Jan-1950 and maxsimum - current date");
            }

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
