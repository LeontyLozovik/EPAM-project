using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle import command.
    /// </summary>
    public class ImportCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Handle import command.
        /// </summary>
        /// <param name="request">request with command and param.</param>
        public override void Handle(AppCommandRequest request)
        {
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
                        Console.WriteLine("Please check you input.");
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
                                    var snapshot = Program.fileCabinetService.MakeSnapshot();
                                    snapshot.LoadFromCsv(streamReader);
                                    int amount = Program.fileCabinetService.Restore(snapshot);
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
                                    var snapshot = Program.fileCabinetService.MakeSnapshot();
                                    snapshot.LoadFromXml(streamReader);
                                    int amount = Program.fileCabinetService.Restore(snapshot);
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
