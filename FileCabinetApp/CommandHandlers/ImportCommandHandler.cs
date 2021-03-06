using System.Text;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle import command.
    /// </summary>
    public class ImportCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCommandHandler"/> class.
        /// </summary>
        /// <param name="service">service to work with.</param>
        public ImportCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Handle import command.
        /// </summary>
        /// <param name="request">request with command and param.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.Equals(request.Command, "import", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(request.Parameters))
                {
                    Console.WriteLine("Command 'import' should contain type and path of file from witch read.");
                }
                else
                {
                    string[] input = request.Parameters.Split(' ', 2);
                    if (input.Length != 2)
                    {
                        Console.WriteLine("Please check you input. Parameters of import command should contain type of file (xml or csv) and file path.");
                    }
                    else
                    {
                        string typeOfFile = input[0].ToUpperInvariant();
                        string filePath = input[1];
                        switch (typeOfFile)
                        {
                            case "CSV":
                                if (!filePath.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                                {
                                    filePath = string.Concat(filePath, ".csv");
                                }

                                if (!File.Exists(filePath))
                                {
                                    Console.WriteLine($"Import error: file {filePath} is not exist");
                                    break;
                                }

                                try
                                {
                                    FileStream fileStream = new FileStream(filePath, FileMode.Open);
                                    StreamReader streamReader = new StreamReader(fileStream, Encoding.Default);
                                    var snapshot = service.MakeSnapshot();
                                    snapshot.LoadFromCsv(streamReader);
                                    int amount = service.Restore(snapshot);
                                    Console.WriteLine($"{amount} records were imported from file {filePath}");
                                    fileStream.Close();
                                    streamReader.Close();
                                }
                                catch (ArgumentNullException ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }

                                break;
                            case "XML":
                                if (!filePath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                                {
                                    filePath = string.Concat(filePath, ".xml");
                                }

                                if (!File.Exists(filePath))
                                {
                                    Console.WriteLine($"Import error: file {filePath} is not exist");
                                    break;
                                }

                                try
                                {
                                    FileStream fileStream = new FileStream(filePath, FileMode.Open);
                                    StreamReader streamReader = new StreamReader(fileStream, Encoding.Default);
                                    var snapshot = service.MakeSnapshot();
                                    snapshot.LoadFromXml(streamReader);
                                    int amount = service.Restore(snapshot);
                                    Console.WriteLine($"{amount} records were imported from file {filePath}");
                                    fileStream.Close();
                                    streamReader.Close();
                                }
                                catch (ArgumentNullException ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }

                                break;
                            default:
                                Console.WriteLine($"Unknown or unsupported type of file - {typeOfFile}");
                                break;
                        }
                    }
                }
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(request);
            }
        }
    }
}
