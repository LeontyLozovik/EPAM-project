using System.Collections;
using System.Collections.ObjectModel;
using System.Text;

namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Iterrator for FileCabinetFilesystemService.
    /// </summary>
    public class FilesystemIterator : IEnumerator, IEnumerable
    {
        private ReadOnlyCollection<long> collection;
        private FileStream fileStream;
        private int index = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesystemIterator"/> class.
        /// </summary>
        /// <param name="collection">collection to iterrate.</param>
        /// <param name="fileStream">filestream where find record.</param>
        public FilesystemIterator(ReadOnlyCollection<long> collection, FileStream fileStream)
        {
            this.collection = collection;
            this.fileStream = fileStream;
        }

        /// <summary>
        /// Gets current element of collection.
        /// </summary>
        /// <value>Current element of collection.</value>
        public object Current
        {
            get
            {
                long offset = this.collection[this.index];
                this.fileStream.Seek(offset, SeekOrigin.Begin);
                return this.GetOneRecord();
            }
        }

        /// <summary>
        /// Returns enumerator.
        /// </summary>
        /// <returns>Enumerator.</returns>
        public IEnumerator GetEnumerator()
        {
            return this;
        }

        /// <summary>
        /// Moves the pointer to the next element.
        /// </summary>
        /// <returns>true - if element exist, false if not.</returns>
        public bool MoveNext()
        {
            if (this.index + 1 < this.collection.Count)
            {
                this.index++;
                return true;
            }
            else
            {
                this.Reset();
                return false;
            }
        }

        /// <summary>
        /// Reset index.
        /// </summary>
        public void Reset()
        {
            this.index = -1;
        }

        private FileCabinetRecord GetOneRecord()
        {
            long recordSize = 277;
            using (BinaryReader binaryReader = new BinaryReader(this.fileStream, Encoding.Default, true))
            {
                short status = binaryReader.ReadInt16();
                if (status == 1)
                {
                    this.fileStream.Seek(recordSize - sizeof(short), SeekOrigin.Current);
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
    }
}
