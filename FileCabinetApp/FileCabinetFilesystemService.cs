using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using FileCabinetApp.Iterators;
using FileCabinetApp.RecordValidators;

namespace FileCabinetApp
{
    /// <summary>
    /// Do manipulations with records in file.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService, IDisposable
    {
        private const long RECORDSIZE = 277;
        private readonly SortedDictionary<string, List<long>> firstNameDictionary = new SortedDictionary<string, List<long>>();
        private readonly SortedDictionary<string, List<long>> lastNameDictionary = new SortedDictionary<string, List<long>>();
        private readonly SortedDictionary<DateTime, List<long>> dateofbirthDictionary = new SortedDictionary<DateTime, List<long>>();
        private FileStream fileStream;
        private IRecordValidator validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="fileStream">param to initialization fileStream fild.</param>
        /// <param name="validator">param to initialization validator fild.</param>
        public FileCabinetFilesystemService(FileStream fileStream, IRecordValidator validator)
        {
            this.fileStream = fileStream;
            this.validator = validator;
        }

        /// <summary>
        /// Check if currient Id exist.
        /// </summary>
        /// /// <param name="id">Id to check.</param>
        /// <returns>True if record with this id exist, false if not exist.</returns>
        public bool IsIdExist(int id)
        {
            if (this.GetOffsetOfRecord(id) > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Return record with current id if it exist.
        /// </summary>
        /// <param name="id">id to find.</param>
        /// <returns>Record with current id.</returns>
        public FileCabinetRecord? GetRecordById(int id)
        {
            using (BinaryReader binaryReader = new BinaryReader(this.fileStream, Encoding.Default, true))
            {
                this.fileStream.Seek(0, SeekOrigin.Begin);
                FileInfo info = new FileInfo(this.fileStream.Name);
                long currentOffset = 0;
                do
                {
                    short status = binaryReader.ReadInt16();
                    if (status == 0)
                    {
                        int currentId = binaryReader.ReadInt32();
                        if (currentId == id)
                        {
                            this.fileStream.Seek(-sizeof(short) - sizeof(int), SeekOrigin.Current);
                            return this.GetOneRecord();
                        }
                    }

                    currentOffset += RECORDSIZE - sizeof(short) - sizeof(int);
                }
                while (info.Length > currentOffset);
                return null;
            }
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
                throw new ArgumentNullException(nameof(record));
            }

            this.validator.ValidateParameters(record);
            if (record.Id == 0)
            {
                record.Id = this.FirstFreeId();
            }

            long offset = this.fileStream.Seek(0, SeekOrigin.End);
            this.WriteOneRecord(record);
            AddNamesToDictionary(record.FirstName, offset, this.firstNameDictionary);
            AddNamesToDictionary(record.LastName, offset, this.lastNameDictionary);
            AddDateToDictionary(record.DateOfBirth, offset, this.dateofbirthDictionary);
            return record.Id;
        }

        /// <summary>
        /// Return all existing records.
        /// </summary>
        /// <returns>all existing records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            List<FileCabinetRecord> list = new List<FileCabinetRecord>();
            FileInfo fileInfo = new FileInfo(this.fileStream.Name);
            int numberOfRecords = (int)(fileInfo.Length / RECORDSIZE);
            this.fileStream.Seek(0, SeekOrigin.Begin);
            for (int i = 0; i < numberOfRecords; i++)
            {
                try
                {
                    list.Add(this.GetOneRecord());
                }
                catch (ArgumentException)
                {
                    continue;
                }
            }

            ReadOnlyCollection<FileCabinetRecord> allRecords = new ReadOnlyCollection<FileCabinetRecord>(list);
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
                Console.WriteLine($"{this.GetNumberOfRemovedRecords()} records removed.");
            }

            FileInfo fileInfo = new FileInfo(this.fileStream.Name);
            return (int)(fileInfo.Length / RECORDSIZE);
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

            long offset = this.GetOffsetOfRecord(newRecord.Id);
            if (offset < 0)
            {
                throw new ArgumentException("Record with this id didn't exist.");
            }

            this.fileStream.Seek(offset, SeekOrigin.Begin);
            FileCabinetRecord incorrectRecord = this.GetOneRecord();
            this.fileStream.Seek(offset, SeekOrigin.Begin);
            this.WriteOneRecord(newRecord);

            this.RemoveFromDictionaries(incorrectRecord, offset);
            AddNamesToDictionary(newRecord.FirstName, offset, this.firstNameDictionary);
            AddNamesToDictionary(newRecord.LastName, offset, this.lastNameDictionary);
            AddDateToDictionary(newRecord.DateOfBirth, offset, this.dateofbirthDictionary);
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

            var selectedOffsets = this.firstNameDictionary[firstName.ToUpperInvariant()];
            ReadOnlyCollection<long> offsets = new ReadOnlyCollection<long>(selectedOffsets);
            FilesystemIterator iterator = new FilesystemIterator(offsets, this.fileStream);
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

            var selectedOffsets = this.lastNameDictionary[lastName.ToUpperInvariant()];
            ReadOnlyCollection<long> offsets = new ReadOnlyCollection<long>(selectedOffsets);
            FilesystemIterator iterator = new FilesystemIterator(offsets, this.fileStream);
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

            var selectedOffsets = this.dateofbirthDictionary[dateToFind];
            ReadOnlyCollection<long> offsets = new ReadOnlyCollection<long>(selectedOffsets);
            FilesystemIterator iterator = new FilesystemIterator(offsets, this.fileStream);
            return iterator;
        }

        /// <summary>
        /// Make a snapshot of current state.
        /// </summary>
        /// <returns>instance of FileCabinetServiceSnapshot class.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.GetRecords().ToArray());
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
            long offset = this.GetOffsetOfRecord(recordId);
            if (offset < 0)
            {
                throw new ArgumentNullException(nameof(recordId));
            }

            this.fileStream.Seek(offset, SeekOrigin.Begin);
            using (BinaryWriter binaryWriter = new BinaryWriter(this.fileStream, Encoding.Default, true))
            {
                binaryWriter.Write((short)1);
            }
        }

        /// <summary>
        /// Difragment file with records in FileCabinetFilesystemService.
        /// </summary>
        /// <returns>Number of difragmented records.</returns>
        public int Defragment()
        {
            this.SetCorrectOrder();
            FileInfo fileInfo = new FileInfo(this.fileStream.Name);
            List<FileCabinetRecord> listOfExistingRecords = new List<FileCabinetRecord>();
            int numberOfRecords = (int)(fileInfo.Length / RECORDSIZE);
            this.fileStream.Seek(0, SeekOrigin.Begin);
            int numberOfRemovedRecords = 0;
            for (int i = 0; i < numberOfRecords; i++)
            {
                try
                {
                    listOfExistingRecords.Add(this.GetOneRecord());
                }
                catch (ArgumentException)
                {
                    numberOfRemovedRecords++;
                    continue;
                }
            }

            this.Dispose();
            this.fileStream = new FileStream("C:\\Epam-project\\FileCabinetApp\\FileDataBase\\cabinet-records.db", FileMode.Truncate);
            this.Dispose();
            this.fileStream = new FileStream("C:\\Epam-project\\FileCabinetApp\\FileDataBase\\cabinet-records.db", FileMode.Open);
            for (int i = 0; i < numberOfRecords - numberOfRemovedRecords; i++)
            {
                this.WriteOneRecord(listOfExistingRecords[i]);
            }

            return numberOfRemovedRecords;
        }

        /// <summary>
        /// Implement method of IDispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Insert records with given filds and values.
        /// </summary>
        /// <param name="record">record to insert.</param>
        public void Insert(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), "Instance doesn't exist.");
            }

            if (this.IsIdExist(record.Id))
            {
                Console.WriteLine("Record with this Id already exists. Rewrite? [y/n]");
                bool notEnd = true;
                do
                {
                    switch (Console.ReadLine())
                    {
                        case "y":
                            this.EditRecord(record);
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

            return allUpdeted;
        }

        /// <summary>
        /// Implement method of IDispose.
        /// </summary>
        /// <param name="disposing">dispose or not dispose.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.fileStream.Dispose();
            }
        }

        /// <summary>
        /// Add records with firstname or lastname key to the dictionary.
        /// </summary>
        /// <param name="name">firstname or lastname key.</param>
        /// <param name="offset">offset to add.</param>
        /// <param name="dictionary">dictionary to add.</param>
        private static void AddNamesToDictionary(string name, long offset, SortedDictionary<string, List<long>> dictionary)
        {
            string keyName = name.ToUpperInvariant();
            if (dictionary.ContainsKey(keyName))
            {
                List<long> valueList = dictionary[keyName];
                valueList.Add(offset);
                dictionary[keyName] = valueList;
            }
            else
            {
                List<long> valueList = new List<long>();
                valueList.Add(offset);
                dictionary.Add(keyName, valueList);
            }
        }

        /// <summary>
        /// Add records with dateofbirth key to the dictionary.
        /// </summary>
        /// <param name="date">dateofbirth key.</param>
        /// <param name="offset">offset to add.</param>
        /// <param name="dictionary">dictionary to add.</param>
        private static void AddDateToDictionary(DateTime date, long offset, SortedDictionary<DateTime, List<long>> dictionary)
        {
            if (dictionary.ContainsKey(date))
            {
                List<long> valueList = dictionary[date];
                valueList.Add(offset);
                dictionary[date] = valueList;
            }
            else
            {
                List<long> valueList = new List<long>();
                valueList.Add(offset);
                dictionary.Add(date, valueList);
            }
        }

        /// <summary>
        /// Edit values in all dictionaries after user edited record.
        /// </summary>
        /// <param name="incorrectRecord">Record with incorrect params.</param>
        /// <param name="offset">offset to remove from dictionaries.</param>
        private void RemoveFromDictionaries(FileCabinetRecord incorrectRecord, long offset)
        {
            List<long> offsetToRemove = this.firstNameDictionary[incorrectRecord.FirstName.ToUpperInvariant()];
            int removeIndex = offsetToRemove.FindIndex(0, offsetToRemove.Count, value => value == offset);
            offsetToRemove.RemoveAt(removeIndex);

            offsetToRemove = this.lastNameDictionary[incorrectRecord.LastName.ToUpperInvariant()];
            removeIndex = offsetToRemove.FindIndex(0, offsetToRemove.Count, value => value == offset);
            offsetToRemove.RemoveAt(removeIndex);

            offsetToRemove = this.dateofbirthDictionary[incorrectRecord.DateOfBirth];
            removeIndex = offsetToRemove.FindIndex(0, offsetToRemove.Count, value => value == offset);
            offsetToRemove.RemoveAt(removeIndex);
        }

        /// <summary>
        /// Write one record to file from currient position.
        /// </summary>
        /// <param name="record">recort to write.</param>
        private void WriteOneRecord(FileCabinetRecord record)
        {
            short status = 0;
            using (BinaryWriter binaryWriter = new BinaryWriter(this.fileStream, Encoding.Default, true))
            {
                binaryWriter.Write(status);
                binaryWriter.Write(record.Id);
                binaryWriter.Write(record.FirstName);
                binaryWriter.Seek(120 - (record.FirstName.Length + 1), SeekOrigin.Current);
                binaryWriter.Write(record.LastName);
                binaryWriter.Seek(120 - (record.LastName.Length + 1), SeekOrigin.Current);
                binaryWriter.Write(record.DateOfBirth.Year);
                binaryWriter.Write(record.DateOfBirth.Month);
                binaryWriter.Write(record.DateOfBirth.Day);
                binaryWriter.Write(record.Children);
                binaryWriter.Write(record.AverageSalary);
                binaryWriter.Write(record.Sex);
            }
        }

        /// <summary>
        /// Get one record from the file starting from currient position.
        /// </summary>
        /// <returns>One record from file.</returns>
        private FileCabinetRecord GetOneRecord()
        {
            using (BinaryReader binaryReader = new BinaryReader(this.fileStream, Encoding.Default, true))
            {
                short status = binaryReader.ReadInt16();
                if (status == 1)
                {
                    this.fileStream.Seek(RECORDSIZE - sizeof(short), SeekOrigin.Current);
                    throw new ArgumentException();
                }

                int id = binaryReader.ReadInt32();
                string firstname = binaryReader.ReadString();
                this.fileStream.Seek(120 - (firstname.Length + 1), SeekOrigin.Current);
                string lastname = binaryReader.ReadString();
                this.fileStream.Seek(120 - (lastname.Length + 1), SeekOrigin.Current);
                int year = binaryReader.ReadInt32();
                int month = binaryReader.ReadInt32();
                int day = binaryReader.ReadInt32();
                short children = binaryReader.ReadInt16();
                decimal salary = binaryReader.ReadDecimal();
                char sex = binaryReader.ReadChar();
                DateTime birthday = new DateTime(year, month, day);
                FileCabinetRecord record = new FileCabinetRecord
                {
                    Id = id,
                    FirstName = firstname,
                    LastName = lastname,
                    DateOfBirth = birthday,
                    Children = children,
                    AverageSalary = salary,
                    Sex = sex,
                };
                return record;
            }
        }

        /// <summary>
        /// Check if there is any free Id of records which were removed.
        /// </summary>
        /// <returns>Next id to use.</returns>
        private int FirstFreeId()
        {
            FileInfo fileInfo = new FileInfo(this.fileStream.Name);
            int numberOfRecords = (int)(fileInfo.Length / RECORDSIZE);
            this.fileStream.Seek(0, SeekOrigin.Begin);
            List<int> posibleIds = new List<int>();
            int bigestId = 0;
            using (BinaryReader binaryReader = new BinaryReader(this.fileStream, Encoding.Default, true))
            {
                for (int i = 0; i < numberOfRecords; i++)
                {
                    short status = binaryReader.ReadInt16();
                    if (status == 1)
                    {
                        int id = binaryReader.ReadInt32();
                        if (!posibleIds.Contains(id))
                        {
                            posibleIds.Add(id);
                        }
                    }
                    else
                    {
                        int id = binaryReader.ReadInt32();
                        if (id > bigestId)
                        {
                            bigestId = id;
                        }

                        if (posibleIds.Contains(id))
                        {
                            posibleIds.Remove(id);
                        }
                    }

                    this.fileStream.Seek(RECORDSIZE - sizeof(short) - sizeof(int), SeekOrigin.Current);
                }
            }

            if (posibleIds.Count == 0)
            {
                return bigestId + 1;
            }

            return posibleIds[0];
        }

        /// <summary>
        /// Returned offset of record with given Id.
        /// </summary>
        /// <param name="id">Id of record.</param>
        /// <returns>Offset of record with this Id.</returns>
        private long GetOffsetOfRecord(int id)
        {
            FileInfo fileInfo = new FileInfo(this.fileStream.Name);
            int numberOfRecords = (int)(fileInfo.Length / RECORDSIZE);
            this.fileStream.Seek(0, SeekOrigin.Begin);
            using (BinaryReader binaryReader = new BinaryReader(this.fileStream, Encoding.Default, true))
            {
                for (int i = 0; i < numberOfRecords; i++)
                {
                    short status = binaryReader.ReadInt16();
                    if (status == 0)
                    {
                        if (id == binaryReader.ReadInt32())
                        {
                            return this.fileStream.Position - sizeof(int) - sizeof(short);
                        }

                        this.fileStream.Seek(-sizeof(int), SeekOrigin.Current);
                    }

                    this.fileStream.Seek(RECORDSIZE - sizeof(short), SeekOrigin.Current);
                }
            }

            return -1;
        }

        /// <summary>
        /// Set correct oder of records before difragmentation.
        /// </summary>
        private void SetCorrectOrder()
        {
            FileInfo fileInfo = new FileInfo(this.fileStream.Name);
            int numberOfRecords = (int)(fileInfo.Length / RECORDSIZE);
            this.fileStream.Seek(0, SeekOrigin.Begin);
            List<int> chanchedId = new List<int>();
            using (BinaryReader binaryReader = new BinaryReader(this.fileStream, Encoding.Default, true))
            {
                for (int i = 0; i < numberOfRecords; i++)
                {
                    short status = binaryReader.ReadInt16();
                    if (status == 1)
                    {
                        int id = binaryReader.ReadInt32();
                        long offsetOfThisRecord = this.fileStream.Seek(-sizeof(short) - sizeof(int), SeekOrigin.Current);
                        if (this.IsIdExist(id) && !chanchedId.Contains(id))
                        {
                            chanchedId.Add(id);
                            this.fileStream.Seek(this.GetOffsetOfRecord(id), SeekOrigin.Begin);
                            var recordToMove = this.GetOneRecord();
                            using (BinaryWriter binaryWriter = new BinaryWriter(this.fileStream, Encoding.Default, true))
                            {
                                this.fileStream.Seek(-RECORDSIZE, SeekOrigin.Current);
                                binaryWriter.Write((short)1);
                            }

                            this.fileStream.Seek(offsetOfThisRecord, SeekOrigin.Begin);
                            this.WriteOneRecord(recordToMove);
                            this.fileStream.Seek(-RECORDSIZE + sizeof(short), SeekOrigin.Current);
                        }
                        else
                        {
                            this.fileStream.Seek(offsetOfThisRecord + sizeof(short), SeekOrigin.Begin);
                        }
                    }

                    this.fileStream.Seek(RECORDSIZE - sizeof(short), SeekOrigin.Current);
                }
            }
        }

        /// <summary>
        /// Get number of removed records in file.
        /// </summary>
        /// <returns>number of removed records.</returns>
        private int GetNumberOfRemovedRecords()
        {
            FileInfo fileInfo = new FileInfo(this.fileStream.Name);
            int numberOfRecords = (int)(fileInfo.Length / RECORDSIZE);
            var realRecords = this.GetRecords().Count;
            int removedRecords = numberOfRecords - realRecords;
            return removedRecords;
        }
    }
}
