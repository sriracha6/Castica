using System;
using System.IO;
using Lexer;

namespace Castica
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string file = File.ReadAllText("sample.cas");//File.ReadAllText(args[0]);
            Lexer.Lexer.Lexify(file);
            foreach(Token t in Lexer.Lexer.Tokens)
                Console.WriteLine(t.TokenType + " | " + t.Value);
        }
    }
}