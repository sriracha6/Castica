
# Castica Language

A simple interpreted language.
I will make it compile to something later. It'll probably compile to my own computer design.

It's simple how it works. There is a pointer that points to the current line. Instructions like
for, if, else, while, repeat, etc., change the pointer. It then executes at the pointer.

TODO:
[X] Language Design
[ ] Lexer
    [X] Lex Tokens
    [X] Lex Comments
    [X] Lex Whitespace
    [ ] Lex Bug: Operators
    [ ] Lex Bug: Endlines
    [ ] Lex Numbers/Floats
[ ] Parser
[ ] Executor
[ ] Syntax Highlighting
