using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Read records from xml file.
    /// </summary>
    public class FileCabinetRecordXmlReader
    {
        private StreamReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.
        /// </summary>
        /// <param name="reader">indent to inicializ this StramReader.</param>
        public FileCabinetRecordXmlReader(StreamReader reader)
        {
            this.reader = reader;
        }

        /// <summary>
        /// Read records from csv file.
        /// </summary>
        /// <returns>list of readed records.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<FileCabinetRecord>));
            using (XmlReader xmlReader = XmlReader.Create(this.reader))
            {
                List<FileCabinetRecord>? recordsFromFile = (List<FileCabinetRecord>?)serializer.Deserialize(xmlReader);
                if (recordsFromFile is null)
                {
                    throw new ArgumentNullException();
                }

                return recordsFromFile;
            }
        }
    }
}
