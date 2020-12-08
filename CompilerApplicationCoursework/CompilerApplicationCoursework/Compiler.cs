usiusing Compiler.IO;
using Compiler.Tokenization;
using System;
using System.Collections.Generic;
using System.IO;
using static System.Console;
using Compuiler.Nodes;
using Compiler.SemanticAnalysis;
using Compiler.SyntacticAnalysis;
using Compiler.CodeGeneration;

namespace Compiler
{
  
    public class Compiler
    {
       
        public ErrorReporter Reporter { get; }

        public IFileReader Reader { get; }

        public Tokenizer Tokenizer { get; }

        public Parser Parser { get; }

        public DeclarationIdentifier Identifier { get; }

        public TypeChecker Checker { get: }

        public CodeGenerator Generator { get; }

        public TargetCodeWriter Writer { get; }
      
        public Compiler(string inputFile)
        {
            Reporter = new ErrorReporter();
            Reader = new FileReader(inputFile);
            Tokenizer = new Tokenizer(Reader, Reporter);
            Parser = new Parser(Reporter);
            Identifier = new DeclarationIdentifier(Reporter);
            Checker = new TypeChecker(Reporter);
            Generator = new CodeGenerator(Reporter);
            Writer = new TargetCodeWriter(binaryOutputFile, textOutputFile, Reporter);
        }

    
        public void Compile()
        {
            
            Write("Tokenising...");
            List<Token> tokens = Tokenizer.GetAllTokens();
            if (Reporter.HasErrors) return;
            WriteLine("Done");

            WriteLine(string.Join("\n", tokens));

            WriteFinalMessage("Parsing...");
            ProgramNode tree = Parser.Parse(tokens);
            if (Reporter.HasErrors) return;
            WriteLine("Done");

            WriteLine(TreePrinter.ToString(tree));

            Write("Identifying...");
            Identifier.PerformIdentification(tree);
            if (Reporter.HasErrors) return;
            WriteLine("Done");

            Write("Type Checking...");
            Checker.PerformTypeChecking(tree);
            if (Reporter.HasErrors) return;
            WriteLine("Done");

            Write("Generating code...");
            TargetCode targetCode = Generator.GenerateCodeFor(tree);
            if (Reporter.HasErrors) return;
            WriteLine("Done");

            Write("Writing to file...");
            Writer.WriteToFiles(targetCode);
            if (Reporter.HasErrors) return;
            WriteLine("Done");
        }

   
        private void WriteFinalMessage()
        {
            
        }

       
        public static void Main(string[] args)
        {
            if (args == null || args.Length != 1 || args[0] == null)
                WriteLine("ERROR: Must call the program with exactly one argument, the input file (*.tri)");
            else if (!File.Exists(args[0]))
                WriteLine($"ERROR: The input file \"{Path.GetFullPath(args[0])}\" does not exist");
            else
            {
                string inputFile = args[0];
                Compiler compiler = new Compiler(inputFile);
                WriteLine("Compiling...");
                compiler.Compile();
                compiler.WriteFinalMessage();
            }
        }
    }
}
