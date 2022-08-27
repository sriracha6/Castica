using System;

namespace Lexer
{
    public enum TokenType { Token, String, Number, MathMisc, Endline, StartBrace, EndBrace }
    public enum TokenValue { Using }
    public struct Token
    {
        public TokenType TokenType { get; private set; }
        public string Value { get; private set; }
        // todo: dumb solution: A getter property that enumerates the TokenValue enum and gets the TokenValueType

        public Token(TokenType tt, string value)
        {
            this.TokenType = tt;
            this.Value = value;
        }
    }
}