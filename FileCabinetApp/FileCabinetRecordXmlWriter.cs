using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// Write record to file in xml format.
    /// </summary>
    public class FileCabinetRecordXmlWriter
    {
        /// <summary>
        /// Writer - write record to the file.
        /// </summary>
        private readonly XmlWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        /// <param name="xmlWriter">param to initialize fild writer.</param>
        public FileCabinetRecordXmlWriter(XmlWriter xmlWriter)
        {
            this.writer = xmlWriter;
        }

        /// <summary>
        /// Write record to csv file.
        /// </summary>
        /// <param name="record">record to write.</param>
        public void Write(FileCabinetRecord record)
        {
            this.writer.WriteStartElement("record");
            this.writer.WriteAttributeString("id", record.Id.ToString());

            this.writer.WriteStartElement("name");
            this.writer.WriteAttributeString("first", record.FirstName);
            this.writer.WriteAttributeString("last", record.LastName);
            this.writer.WriteEndElement();

            this.writer.WriteStartElement("dateofbirth");
            this.writer.WriteString(record.DateOfBirth.ToString("dd.MM.yyyy"));
            this.writer.WriteEndElement();

            this.writer.WriteStartElement("children");
            this.writer.WriteString(record.Children.ToString());
            this.writer.WriteEndElement();

            this.writer.WriteStartElement("salary");
            this.writer.WriteString(record.AverageSalary.ToString());
            this.writer.WriteEndElement();

            this.writer.WriteStartElement("sex");
            this.writer.WriteString(record.Sex.ToString());
            this.writer.WriteEndElement();

            this.writer.WriteEndElement();
        }

        /// <summary>
        /// Print start of the xml file.
        /// </summary>
        public void Start()
        {
            this.writer.WriteStartDocument();
            this.writer.WriteStartElement("records");
        }

        /// <summary>
        /// Close XmlWriter and due to settings write end of xml file.
        /// </summary>
        public void End()
        {
            this.writer.Close();
        }
    }
}
