using Compiler.IO;
using System;
using System.Collections.Generic;
using System.Text;


namespace Compiler.Tokenization
{
  
    public class Tokenizer
    {
       
        public ErrorReporter Reporter { get; }

       
        private IFileReader Reader { get; }

  
        private StringBuilder TokenSpelling { get; } = new StringBuilder();

        public Tokenizer(IFileReader reader, ErrorReporter reporter)
        {
            Reader = reader;
            Reporter = reporter;
        }

        public List<Token> GetAllTokens()
        {
            List<Token> tokens = new List<Token>();
            Token token = GetNextToken();
            while (token.Type != TokenType.EndOfText)
            {
                tokens.Add(token);
                token = GetNextToken();
            }
            tokens.Add(token);
            Reader.Close();
            return tokens;
        }

       
        private Token GetNextToken()
        {
            
            SkipSeparators();

            
            Position tokenStartPosition = Reader.CurrentPosition;

           
            TokenType tokenType = ScanToken();

            Token token = new Token(tokenType, TokenSpelling.ToString(), tokenStartPosition);
            Debugger.Write($"Scanned {token}");

            if (tokenType == TokenType.Error)
            {
  
            }

            return token;
        }

        private void SkipSeparators()
        {
            while (Reader.Current == '!' || IsWhiteSpace(Reader.Current))
            {
                if (Reader.Current == '!')
                    Reader.SkipRestOfLine();
                else
                    Reader.MoveNext();
            }
        }

       
        private TokenType ScanToken()
        {
            TokenSpelling.Clear();
            if (char.IsLetter(Reader.Current))
            {
                
                TakeIt();
                while (char.IsLetterOrDigit(Reader.Current))
                    TakeIt();
                if (TokenTypes.IsKeyword(TokenSpelling))
                    return TokenTypes.GetTokenForKeyword(TokenSpelling);
                else
                    return TokenType.Identifier;
            }
            else if (char.IsDigit(Reader.Current))
            {
                
                TakeIt();
                while (char.IsDigit(Reader.Current))
                    TakeIt();
                return TokenType.IntLiteral;
            }
            else if (IsOperator(Reader.Current))
            {
                
                TakeIt();
                return TokenType.Operator;
            }
            else if (Reader.Current == '.')
            {
                
                TakeIt();
                return TokenType.FullStop;
                }
            }
            else if (Reader.Current == ',')
            {
             
                TakeIt();
                return TokenType.Comma;
            }
            else if (Reader.Current == '?')
            {
             
                TakeIt();
                return TokenType.QuestionMark;
            }
            else if (Reader.Current == '\'')
            {
                TakeIt();
               
                TakeIt();
                
                if (Reader.Current == '\'')
                {
                    TakeIt();
                    return TokenType.CharLiteral;
                }
                else
                {
                    
                    return TokenType.Error;
                }
            }
            else if (Reader.Current == default(char))
            {
                
                TakeIt();
                return TokenType.EndOfText;
            }
            else
            {
                
                TakeIt();
                return TokenType.Error;
            }
        }

        
        private void TakeIt()
        {
            TokenSpelling.Append(Reader.Current);
            Reader.MoveNext();
        }

        private static bool IsWhiteSpace(char c)
        {
            return c == ' ' || c == '\t' || c == '\n';
        }

        
        private static bool IsOperator(char c)
        {
            switch (c)
            {
                case '+':
                case '-':
                case '*':
                case '/':
                case '<':
                case '>':
                case '=':
                case '\':
                case '>=':
                case '<=':
                    return true;
                default:
                    return false;
            }
        }
    }
}
