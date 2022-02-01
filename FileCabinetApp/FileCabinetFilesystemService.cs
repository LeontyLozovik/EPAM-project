using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Do manipulations with records in file.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const long RECORDSIZE = 277;
        private readonly FileStream fileStream;
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
        /// Create a new record with inputed parameters.
        /// </summary>
        /// <param name="record">Record to create.</param>
        /// <returns>Id of created record.</returns>
        public int CreateRecord(FileCabinetRecord record)
        {
            this.validator.ValidateParameters(record);
            FileInfo fileInfo = new FileInfo(this.fileStream.Name);
            record.Id = ((int)(fileInfo.Length / RECORDSIZE)) + 1;
            this.fileStream.Seek((int)fileInfo.Length, SeekOrigin.Begin);
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
                list.Add(this.GetOneRecord());
            }

            ReadOnlyCollection<FileCabinetRecord> allRecords = new ReadOnlyCollection<FileCabinetRecord>(list);
            return allRecords;
        }

        /// <summary>
        /// Return number of existing records.
        /// </summary>
        /// <returns>number of existing records.</returns>
        public int GetStat()
        {
            FileInfo fileInfo = new FileInfo(this.fileStream.Name);
            return (int)(fileInfo.Length / RECORDSIZE);
        }

        /// <summary>
        /// Edit record by entered Id.
        /// </summary>
        /// <param name="newRecord">New record that replace old record.</param>
        public void EditRecord(FileCabinetRecord newRecord)
        {
            int offset = (newRecord.Id - 1) * (int)RECORDSIZE;
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
                        FileCabinetRecord foundRecord = this.GetOneRecord();
                        list.Add(foundRecord);
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
                        FileCabinetRecord foundRecord = this.GetOneRecord();
                        list.Add(foundRecord);
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
                            FileCabinetRecord foundRecord = this.GetOneRecord();
                            list.Add(foundRecord);
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add records to service.
        /// </summary>
        /// <param name="snapshot">item of FileCabinetServiceSnapshot were records read.</param>
        /// <returns>number of imported records.</returns>
        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Write one record to file from currient position.
        /// </summary>
        /// <param name="record">recort to write.</param>
        private void WriteOneRecord(FileCabinetRecord record)
        {
            short status = 1;
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
    }
}
