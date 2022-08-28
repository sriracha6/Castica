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
        public static readonly char[] OPERATORS = new char[] {'(', ';', ')', '{', '}', '[', ']', '+', '-', '/', '*', '>', '=', '<', '%', '&', '|', '^', '!', '.', ',', STRING_CHAR};
        public const char STRING_CHAR = '"';
        public const char END_LINE = ';';
        public const string SINGLE_COMMENT = "//";
        public const string MULTI_COMMENT_START = "/*";
        public const string MULTI_COMMENT_END = "*/";

        public static void Lexify(string text)
        {
            lexMode = LexMode.Whitespace;

            string currentToken = "";
            bool operatorMode = false;
            bool escapeMode = false;
            for(int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if(c == MULTI_COMMENT_END[0] && i + 1 < text.Length && text[i+1] == MULTI_COMMENT_END[1]) 
                {
                    i++;   
                    lexMode = LexMode.Whitespace;
                    continue;
                }
                if((lexMode == LexMode.SingleComment && c != '\n') || lexMode == LexMode.MultiComment)
                    continue;
                else if (c == '\n' && lexMode == LexMode.SingleComment) // end single line comment. this is a really messy function
                    lexMode = LexMode.Whitespace; // dont add token because it aint important

                if(c == SINGLE_COMMENT[0] && i + 1 < text.Length && text[i + 1] == SINGLE_COMMENT[1])
                {    lexMode = LexMode.SingleComment; continue;}
                if(c == MULTI_COMMENT_START[0] && i + 1 < text.Length && text[i+1] == MULTI_COMMENT_START[1]) 
                {    lexMode = LexMode.MultiComment; continue; }

                if(lexMode == LexMode.Token && (DELIMITERS.Contains(c) || OPERATORS.Contains(text[i])))
                {
                    if(int.TryParse(currentToken, out int resint) || float.TryParse(currentToken, out float resfloat))
                        Tokens.Add(new Token(TokenType.Number, currentToken));
                    else
                        Tokens.Add(new Token(operatorMode ? TokenType.Operator : TokenType.Token, currentToken));
                    currentToken = "";
                    lexMode = LexMode.Whitespace;
                    if(OPERATORS.Contains(text[i]))
                        Tokens.Add(new Token(TokenType.Operator, c.ToString()));
                    continue;
                }
                if(lexMode == LexMode.Token)
                {
                    if(operatorMode && lexMode == LexMode.Token && currentToken.Length == 1)
                    {
                        operatorMode = false;
                        Tokens.Add(new Token(TokenType.Operator, currentToken));
                        currentToken = "";
                        lexMode = LexMode.Whitespace;
                        continue;
                    }
                    else currentToken += c;
                }
                if(lexMode == LexMode.String && c == STRING_CHAR)
                {
                    Tokens.Add(new Token(TokenType.String, currentToken));
                    currentToken = "";
                    lexMode = LexMode.Whitespace;
                    escapeMode = false;
                    continue;
                }
                if(lexMode == LexMode.String) 
                {
                    if(escapeMode)
                    {
                        escapeMode = false;
                        currentToken += ParseEscape(c).ToString();
                        continue;
                    }
                    if(c == '\\') escapeMode = true;
                    else currentToken += c;
                }
                if(c == STRING_CHAR)
                {
                    lexMode = LexMode.String;
                    continue;
                }
                if(lexMode != LexMode.String && i - 1 >= 0 && OPERATORS.Contains(text[i-1]) && c == END_LINE)
                    Tokens.Add(new Token(TokenType.Endline, END_LINE.ToString()));
                if(lexMode == LexMode.Whitespace && !DELIMITERS.Contains(c) && c != END_LINE)
                {
                    operatorMode = OPERATORS.Contains(c);
                    lexMode = LexMode.Token;
                    currentToken += c;
                }
            }
            // this is a terrible solution
            if(OPERATORS.Contains(text[text.Length - 1]))
                Tokens.Add(new Token(TokenType.Operator, text[text.Length - 1].ToString()));
        }

        public static char ParseEscape(char escape)
        {
            return escape switch
            {
                'n' => '\n',
                'r' => '\r',
                'b' => '\b',
                't' => '\t',
                '0' => '\0',
                '\\' => '\\',
                '\"' => '\"',
                'v' => '\v',
                '\'' => '\'',
                'f' => '\f',
                _ => '\0'
            };
        }
    }
}