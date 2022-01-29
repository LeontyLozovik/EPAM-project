using System.Collections.ObjectModel;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Do manipulations with records in file.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
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
            short recordSize = 277;
            record.Id = ((int)fileInfo.Length / recordSize) + 1;
            short status = 1;
            using (BinaryWriter binaryWriter = new BinaryWriter(this.fileStream, Encoding.Default, true))
            {
                binaryWriter.Seek((int)fileInfo.Length, SeekOrigin.Begin);
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
            short recordSize = 277;
            int numberOfRecords = (int)fileInfo.Length / recordSize;
            this.fileStream.Seek(0, SeekOrigin.Begin);
            for (int i = 0; i < numberOfRecords; i++)
            {
                list.Add(this.GetOneRecord());
            }

            ReadOnlyCollection<FileCabinetRecord> allRecords = new ReadOnlyCollection<FileCabinetRecord>(list);
            return allRecords;
        }

        /// <summary>
        /// Edit record by entered Id.
        /// </summary>
        /// <param name="newRecord">New record that replace old record.</param>
        public void EditRecord(FileCabinetRecord newRecord)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Find all records with entered dateofbirth.
        /// </summary>
        /// <param name="birthday">dateofbirth to find.</param>
        /// <returns>all records with entered dateofbirth.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByBirthday(string birthday)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Find all records with entered firstname.
        /// </summary>
        /// <param name="firstName">firstname to find.</param>
        /// <returns>all records with entered firstname.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Find all records with entered lastname.
        /// </summary>
        /// <param name="lastName">lastname to find.</param>
        /// <returns>all records with entered lastname.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return number of existing records.
        /// </summary>
        /// <returns>number of existing records.</returns>
        public int GetStat()
        {
            throw new NotImplementedException();
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
