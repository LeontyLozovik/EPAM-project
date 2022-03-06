using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents records.
    /// </summary>
    [XmlType(TypeName = "record")]
    public class FileCabinetRecord : ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecord"/> class.
        /// </summary>
        public FileCabinetRecord()
        {
            this.Id = 0;
            this.FirstName = string.Empty;
            this.LastName = string.Empty;
            this.DateOfBirth = default(DateTime);
            this.Children = 0;
            this.AverageSalary = 0;
            this.Sex = 'm';
        }

        /// <summary>
        /// Gets or sets id of record.
        /// </summary>
        /// <value> Id of record.</value>
        [XmlAttribute]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets firstname in record.
        /// </summary>
        /// <value>Firstname in record.</value>
        [XmlElement]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets lastname in record.
        /// </summary>
        /// <value>Lastname in record.</value>
        [XmlElement]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets dateofbirth in record.
        /// </summary>
        /// <value>Dateofbirth in record.</value>
        [XmlElement]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets number of childrens in record.
        /// </summary>
        /// <value>Number of childrens in record.</value>
        [XmlElement]
        public short Children { get; set; }

        /// <summary>
        /// Gets or sets average salary in record.
        /// </summary>
        /// <value>Average salary in record.</value>
        [XmlElement("salary")]
        public decimal AverageSalary { get; set; }

        /// <summary>
        /// Gets or sets sex in record.
        /// </summary>
        /// <value>Sex in record.</value>
        [XmlElement]
        public char Sex { get; set; }

        /// <summary>
        /// Return copy of object.
        /// </summary>
        /// <returns>Copy of record.</returns>
        public object Clone()
        {
            var newRecord = new FileCabinetRecord();
            newRecord.Id = this.Id;
            newRecord.FirstName = this.FirstName;
            newRecord.LastName = this.LastName;
            newRecord.DateOfBirth = this.DateOfBirth;
            newRecord.Children = this.Children;
            newRecord.AverageSalary = this.AverageSalary;
            newRecord.Sex = this.Sex;
            return newRecord;
        }
    }
}