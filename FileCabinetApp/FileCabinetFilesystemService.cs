using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using FileCabinetApp.RecordValidators;

namespace FileCabinetApp
{
    /// <summary>
    /// Do manipulations with records in file.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService, IDisposable
    {
        private const long RECORDSIZE = 277;
        private FileStream fileStream;
        private IRecordValidator validator = new DefaultValidator();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="fileStream">param to initialization fileStream fild.</param>
        public FileCabinetFilesystemService(FileStream fileStream)
        {
            this.fileStream = fileStream;
        }

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
        /// Returns validator.
        /// </summary>
        /// <returns>type of validation.</returns>
        public IRecordValidator GetValidationType()
        {
            return this.validator;
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
            record.Id = this.FirstFreeId();
            this.fileStream.Seek(0, SeekOrigin.End);
            this.WriteOneRecord(record);
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
            this.WriteOneRecord(newRecord);
        }

        /// <summary>
        /// Find all records with entered firstname.
        /// </summary>
        /// <param name="firstName">firstname to find.</param>
        /// <returns>all records with entered firstname.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            List<FileCabinetRecord> list = new List<FileCabinetRecord>();
            FileInfo fileInfo = new FileInfo(this.fileStream.Name);
            long offset = 6;
            do
            {
                this.fileStream.Seek(offset, SeekOrigin.Begin);
                using (BinaryReader reader = new BinaryReader(this.fileStream, Encoding.Default, true))
                {
                    string firstnameInRecord = reader.ReadString();
                    if (string.Equals(firstName, firstnameInRecord, StringComparison.OrdinalIgnoreCase))
                    {
                        this.fileStream.Seek(-6 - (firstnameInRecord.Length + 1), SeekOrigin.Current);
                        try
                        {
                            FileCabinetRecord foundRecord = this.GetOneRecord();
                            list.Add(foundRecord);
                        }
                        catch (ArgumentException)
                        {
                            offset += RECORDSIZE;
                            continue;
                        }
                    }

                    offset += RECORDSIZE;
                }
            }
            while (offset < fileInfo.Length);
            ReadOnlyCollection<FileCabinetRecord> foundRecords = new ReadOnlyCollection<FileCabinetRecord>(list);
            return foundRecords;
        }

        /// <summary>
        /// Find all records with entered lastname.
        /// </summary>
        /// <param name="lastName">lastname to find.</param>
        /// <returns>all records with entered lastname.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            List<FileCabinetRecord> list = new List<FileCabinetRecord>();
            FileInfo fileInfo = new FileInfo(this.fileStream.Name);
            long offset = 126;
            do
            {
                this.fileStream.Seek(offset, SeekOrigin.Begin);
                using (BinaryReader reader = new BinaryReader(this.fileStream, Encoding.Default, true))
                {
                    string lastnameInRecord = reader.ReadString();
                    if (string.Equals(lastName, lastnameInRecord, StringComparison.OrdinalIgnoreCase))
                    {
                        this.fileStream.Seek(-126 - (lastnameInRecord.Length + 1), SeekOrigin.Current);
                        try
                        {
                            FileCabinetRecord foundRecord = this.GetOneRecord();
                            list.Add(foundRecord);
                        }
                        catch (ArgumentException)
                        {
                            offset += RECORDSIZE;
                            continue;
                        }
                    }

                    offset += RECORDSIZE;
                }
            }
            while (offset < fileInfo.Length);
            ReadOnlyCollection<FileCabinetRecord> foundRecords = new ReadOnlyCollection<FileCabinetRecord>(list);
            return foundRecords;
        }

        /// <summary>
        /// Find all records with entered dateofbirth.
        /// </summary>
        /// <param name="birthday">dateofbirth to find.</param>
        /// <returns>all records with entered dateofbirth.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByBirthday(string birthday)
        {
            List<FileCabinetRecord> list = new List<FileCabinetRecord>();
            DateTime dateToFind;
            if (!DateTime.TryParse(birthday, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out dateToFind))
            {
                Console.WriteLine("Please check your input.");
            }
            else
            {
                FileInfo fileInfo = new FileInfo(this.fileStream.Name);
                long offset = 246;
                do
                {
                    this.fileStream.Seek(offset, SeekOrigin.Begin);
                    using (BinaryReader reader = new BinaryReader(this.fileStream, Encoding.Default, true))
                    {
                        int year = reader.ReadInt32();
                        int month = reader.ReadInt32();
                        int day = reader.ReadInt32();
                        DateTime dateInRecord = new DateTime(year, month, day);
                        if (DateTime.Compare(dateToFind, dateInRecord) == 0)
                        {
                            this.fileStream.Seek(-258, SeekOrigin.Current);
                            try
                            {
                                FileCabinetRecord foundRecord = this.GetOneRecord();
                                list.Add(foundRecord);
                            }
                            catch (ArgumentException)
                            {
                                offset += RECORDSIZE;
                                continue;
                            }
                        }

                        offset += RECORDSIZE;
                    }
                }
                while (offset < fileInfo.Length);
            }

            ReadOnlyCollection<FileCabinetRecord> foundRecords = new ReadOnlyCollection<FileCabinetRecord>(list);
            return foundRecords;
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
