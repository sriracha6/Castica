using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexer
{
    public static partial class Lexer
    {
        private enum LexMode { Whitespace, Token, String, Number, MultiComment, SingleComment }
        static LexMode lexMode;

        public static List<Token> Tokens = new();

        public static readonly char[] DELIMITERS = new char[] {' ', '\n', '\r', '\t'};
        public static readonly char[] OPERATORS = new char[] {'(', ';', ')', '{', '}', '[', ']', STRING_CHAR};
        public const char STRING_CHAR = '"';
        public const char END_LINE = ';';
        public const string SINGLE_COMMENT = "//";
        public const string MULTI_COMMENT_START = "/*";
        public const string MULTI_COMMENT_END = "*/";

        public static void Lexify(string text)
        {
            lexMode = LexMode.Token;

            string currentToken = "";
            bool singleSlashAlready = false;
            bool operatorMode = false;
            for(int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if(c == MULTI_COMMENT_END[1] && i - 1 >= 0 && text[i-1] == MULTI_COMMENT_END[0]) 
                    lexMode = LexMode.Whitespace;
                if((lexMode == LexMode.SingleComment && c != '\n') || lexMode == LexMode.MultiComment)
                    continue;
                else if (c == '\n')// end single line comment. this is a really messy function
                {
                    lexMode = LexMode.Whitespace; // dont add token because it aint important
                    singleSlashAlready = false;
                }
                if(lexMode == LexMode.Token && (DELIMITERS.Contains(c) || OPERATORS.Contains(c)))
                {
                    Tokens.Add(new Token(operatorMode ? TokenType.Operator : TokenType.Token, currentToken));
                    currentToken = "";
                    lexMode = LexMode.Whitespace;
                }
                if(lexMode == LexMode.Token) currentToken += c;
                if(lexMode == LexMode.String && c == STRING_CHAR)
                {
                    Tokens.Add(new Token(TokenType.String, currentToken));
                    currentToken = "";
                    lexMode = LexMode.Whitespace;
                    continue;
                }
                if(lexMode == LexMode.String) 
                {
                    if(operatorMode && lexMode == LexMode.Token && currentToken.Length == 1)
                    {
                        Tokens.Add(new Token(TokenType.Operator, currentToken));
                        currentToken = "";
                        lexMode = LexMode.Whitespace;
                    }
                    else currentToken += c;
                }
                if(c == STRING_CHAR)
                {
                    lexMode = LexMode.String;
                    continue;
                }
                if(c == SINGLE_COMMENT[0])
                {
                    if(singleSlashAlready)
                        lexMode = LexMode.SingleComment;
                    singleSlashAlready = true;
                }
                if(c == MULTI_COMMENT_START[1] && i - 1 >= 0 && text[i-1] == MULTI_COMMENT_START[0]) 
                    lexMode = LexMode.MultiComment;
                if(lexMode != LexMode.String && i - 1 >= 0 && OPERATORS.Contains(text[i-1]) && c == END_LINE)
                    Tokens.Add(new Token(TokenType.Endline, END_LINE.ToString()));
                if(lexMode == LexMode.Whitespace && !DELIMITERS.Contains(c) && c != END_LINE)
                {
                    operatorMode = OPERATORS.Contains(c);
                    lexMode = LexMode.Token;
                    currentToken += c;
                }
            }
        }
    }
}