using System.Collections.ObjectModel;
using System.Xml;
using FileCabinetApp.CsvFile;
using FileCabinetApp.XmlFile;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Service class for crating snapshots.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private readonly FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">records to initialize new istance of this class.</param>
        public FileCabinetServiceSnapshot(FileCabinetRecord[] records)
        {
            this.records = records;
        }

        /// <summary>
        /// Gets readOnlyColection of records.
        /// </summary>
        /// <value>Collection of records readed from file.</value>
        public IReadOnlyCollection<FileCabinetRecord>? Records { get; private set; }

        /// <summary>
        /// Save all records to csv file.
        /// </summary>
        /// <param name="streamWriter">Stream Writer to write records to file.</param>
        public void SaveToCsv(StreamWriter streamWriter)
        {
            FileCabinetRecordCsvWriter fileWriter = new FileCabinetRecordCsvWriter(streamWriter);
            fileWriter.WriteTemplate();
            foreach (var record in this.records)
            {
                fileWriter.Write(record);
            }
        }

        /// <summary>
        /// Save all records to xml file.
        /// </summary>
        /// <param name="streamWriter">Stream Writer to write records to file.</param>
        public void SaveToXml(StreamWriter streamWriter)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.WriteEndDocumentOnClose = true;
            XmlWriter xmlWriter = XmlWriter.Create(streamWriter, settings);
            FileCabinetRecordXmlWriter fileWriter = new FileCabinetRecordXmlWriter(xmlWriter);
            fileWriter.Start();
            foreach (var record in this.records)
            {
                fileWriter.Write(record);
            }

            fileWriter.End();
        }

        /// <summary>
        /// Loads from records from csv file.
        /// </summary>
        /// <param name="streamReader">stream to read.</param>
        public void LoadFromCsv(StreamReader streamReader)
        {
            FileCabinetRecordCsvReader reader = new FileCabinetRecordCsvReader(streamReader);
            this.Records = new ReadOnlyCollection<FileCabinetRecord>(reader.ReadAll());
        }

        /// <summary>
        /// Loads from records from xml file.
        /// </summary>
        /// <param name="streamReader">stream to read.</param>
        public void LoadFromXml(StreamReader streamReader)
        {
            FileCabinetRecordXmlReader reader = new FileCabinetRecordXmlReader(streamReader);
            try
            {
                this.Records = new ReadOnlyCollection<FileCabinetRecord>(reader.ReadAll());
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("File is empty");
            }
        }
    }
}
