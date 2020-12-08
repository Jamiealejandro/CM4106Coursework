namespace Compiler
{

    public class Position
    {
        
        public int LineNumber { get; }

        public int PositionInLine { get; }

        public Position(int lineNumber, int positionInLine)
        {
            LineNumber = lineNumber;
            PositionInLine = positionInLine;
        }

        public override string ToString()
        {
            if (this == BuiltIn)
                return "System defined";
            else
                return $"Line {LineNumber}, Column {PositionInLine}";
        }

        public static Position BuiltIn { get; } = new Position(-1, -1);
    }
}