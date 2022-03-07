namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle missed command.
    /// </summary>
    public class MissedCommandHandler : CommandHandlerBase
    {
        private List<string> commands = new List<string> { "help", "exit", "stat", "create", "list", "find", "export", "import", "purge", "insert", "delete", "update" };

        /// <summary>
        /// Handle missed command.
        /// </summary>
        /// <param name="request">request with command and param.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            Console.WriteLine($"There is no '{request.Command}' command. See 'help' command.");
            if (request.Command is null)
            {
                throw new ArgumentNullException("No command inputed.", nameof(request.Command));
            }

            var mostComman = this.CheckSequence(request.Command);
            Console.WriteLine("\nMost similar commands are: ");
            foreach (var item in mostComman)
            {
                Console.WriteLine($"\t{this.commands[item]}");
            }

            Console.WriteLine();
        }

        private static int Minimum(int a, int b, int c) => (a = a < b ? a : b) < c ? a : c;

        private static int LevenshteinDistance(string firstWord, string secondWord)
        {
            var n = firstWord.Length + 1;
            var m = secondWord.Length + 1;
            var matrixD = new int[n, m];

            const int deletionCost = 1;
            const int insertionCost = 1;

            for (var i = 0; i < n; i++)
            {
                matrixD[i, 0] = i;
            }

            for (var j = 0; j < m; j++)
            {
                matrixD[0, j] = j;
            }

            for (var i = 1; i < n; i++)
            {
                for (var j = 1; j < m; j++)
                {
                    var substitutionCost = firstWord[i - 1] == secondWord[j - 1] ? 0 : 1;

                    matrixD[i, j] = Minimum(
                        matrixD[i - 1, j] + deletionCost,          // удаление
                        matrixD[i, j - 1] + insertionCost,         // вставка
                        matrixD[i - 1, j - 1] + substitutionCost); // замена
                }
            }

            return matrixD[n - 1, m - 1];
        }

        private List<int> CheckSequence(string missedCommand)
        {
            List<int> weight = new List<int>();
            foreach (var command in this.commands)
            {
                int count = LevenshteinDistance(command, missedCommand);
                weight.Add(count);
            }

            var result = new List<int>();
            int index = -1;
            do
            {
                index = weight.IndexOf(weight.Min(), index + 1);
                result.Add(index);
            }
            while (index != -1);
            result.Remove(-1);

            foreach (var command in this.commands)
            {
                if (command.Contains(missedCommand, StringComparison.OrdinalIgnoreCase))
                {
                    index = this.commands.IndexOf(command);
                    if (!result.Contains(index))
                    {
                        result.Add(index);
                    }
                }
            }

            return result;
        }
    }
}