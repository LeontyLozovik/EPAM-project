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
        /// <param name="firstName">firstname to record.</param>
        /// <param name="lastName">lastname to record.</param>
        /// <param name="dateOfBirth">dateofbirth to record.</param>
        /// <param name="children">number of children to record.</param>
        /// <param name="salary">average salary to record.</param>
        /// <param name="sex">sex to record.</param>
        /// <returns>Id of created record.</returns>
        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short children, decimal salary, char sex)
        {
            if (firstName is null)
            {
                throw new ArgumentNullException(firstName, "First name can't be null.");
            }
            else if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < 2 || firstName.Length > 60)
            {
                throw new ArgumentException("Incorrect first name! First name should be grater then 2, less then 60 and can't be white space.", firstName);
            }

            if (lastName is null)
            {
                throw new ArgumentNullException(lastName, "Last name can't be null.");
            }
            else if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < 2 || lastName.Length > 60)
            {
                throw new ArgumentException("Incorrect last name! Last name should be grater then 2, less then 60 and can't be white space.", lastName);
            }

            if (children < 0)
            {
                throw new ArgumentException("Number of children can't be less then 0.");
            }

            if (salary < 0 || salary > 1000000000)
            {
                throw new ArgumentException("Average salary can't be less then 0 or grater then 1 billion.");
            }

            if (sex != 'm' && sex != 'w')
            {
                throw new ArgumentException("Sorry, but your sex can be m - men or w - women only.");
            }

            DateTime oldest = new DateTime(1950, 1, 1);
            DateTime now = DateTime.Now;
            if (dateOfBirth < oldest || dateOfBirth > now)
            {
                throw new ArgumentException("Sorry but minimal date of birth - 01-Jan-1950 and maxsimum - current date");
            }

            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Children = children,
                AverageSalary = salary,
                Sex = sex,
            };

            this.list.Add(record);
            this.AddNamesToDictionary(firstName, record, this.firstNameDictionary);
            this.AddNamesToDictionary(lastName, record, this.lastNameDictionary);
            this.AddDateToDictionary(dateOfBirth, record, this.dateofbirthDictionary);
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
        /// <param name="id">Id of record to edit.</param>
        /// <param name="firstName">New firstname.</param>
        /// <param name="lastName">New lastname.</param>
        /// <param name="dateOfBirth">New dateofbirth.</param>
        /// <param name="children">New number of children.</param>
        /// <param name="salary">New average salary.</param>
        /// <param name="sex">New sex.</param>
        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, short children, decimal salary, char sex)
        {
            if (firstName is null)
            {
                throw new ArgumentNullException(firstName, "First name can't be null.");
            }
            else if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < 2 || firstName.Length > 60)
            {
                throw new ArgumentException("Incorrect first name! First name should be grater then 2, less then 60 and can't be white space.", firstName);
            }

            if (lastName is null)
            {
                throw new ArgumentNullException(lastName, "Last name can't be null.");
            }
            else if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < 2 || lastName.Length > 60)
            {
                throw new ArgumentException("Incorrect last name! Last name should be grater then 2, less then 60 and can't be white space.", lastName);
            }

            if (children < 0)
            {
                throw new ArgumentException("Number of children can't be less then 0.");
            }

            if (salary < 0 || salary > 1000000000)
            {
                throw new ArgumentException("Average salary can't be less then 0 or grater then 1 billion.");
            }

            if (sex != 'm' && sex != 'w')
            {
                throw new ArgumentException("Sorry, but your sex can be m - men or w - women only.");
            }

            DateTime oldest = new DateTime(1950, 1, 1);
            DateTime now = DateTime.Now;
            if (dateOfBirth < oldest || dateOfBirth > now)
            {
                throw new ArgumentException("Sorry but minimal date of birth - 01-Jan-1950 and maxsimum - current date");
            }

            if (id > this.list.Count)
            {
                throw new ArgumentException("There is no record with this Id.");
            }

            var newRecord = new FileCabinetRecord
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Children = children,
                AverageSalary = salary,
                Sex = sex,
            };

            FileCabinetRecord incorrectRecord = this.list[id - 1];

            this.list.RemoveAt(id - 1);
            this.list.Insert(id - 1, newRecord);
            this.EditDictionaries(firstName, lastName, dateOfBirth, newRecord, incorrectRecord);
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
