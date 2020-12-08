using static System.Console;

namespace Compiler.IO
{
  
    public class ErrorReporter
    {
        public int ErrorCount { get; private set; } = 0;

        public bool HasErrors { get { return ErrorCount > 0; } }

        public void ReportError(string message)
        {
            ErrorCount += 1;
            WriteLine($"ERROR: {message}");
        }
    }
}
