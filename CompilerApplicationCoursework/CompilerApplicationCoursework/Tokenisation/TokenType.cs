using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using static Compiler.Tokenization.TokenType;

namespace Compiler.Tokenization
{
  
    public enum TokenType
    {
        
        IntLiteral, Identifier, Operator, CharLiteral,

        
        Begin, Const, Do, Else, End, If, In, Let, NoElse, Then, Repeat, Until, Var, While,

    
        FullStop, Comma, QuestionMark, 

     
        EndOfText, Error
    }

    
    public static class TokenTypes
    {
        
        public static ImmutableDictionary<string, TokenType> Keywords { get; } = new Dictionary<string, TokenType>()
        {
            { "begin", Begin },
            { "const", Const },
            { "do", Do },
            { "else", Else },
            { "end", End },
            { "if", If },
            { "in", In },
            { "let", Let },
            { "noelse", NoElse },
            { "then", Then },
            { "repeat", Repeat },
            { "until", Until },
            { "var", Var },
            { "while", While },
        }.ToImmutableDictionary();

        
        public static bool IsKeyword(StringBuilder word)
        {
            return Keywords.ContainsKey(word.ToString());
        }

        public static TokenType GetTokenForKeyword(StringBuilder word)
        {
            if (!IsKeyword(word)) throw new ArgumentException("Word is not a keyword");
            return Keywords[word.ToString()];
        }
    }
}
