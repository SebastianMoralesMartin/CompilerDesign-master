/*
    A01745219 Eduardo R. Muller Romero
    A01376228 Sebastian Morales Martin
    A01746645 Guillermo Adrian Urbina A.
*/

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Falak {

    class Scanner {

        readonly string input;

        static readonly Regex newLineRegex = new Regex(
            @"(?<Newline>   \n         )"
            ,RegexOptions.IgnorePatternWhitespace
                | RegexOptions.Compiled
                | RegexOptions.Multiline

        );

        static readonly Regex regex = new Regex(
 

            @" (?<MultiLineComment> <\#[\S\s]*?\#>  )
            |  (?<Comment>   \#.*       )
            |  (?<Newline>   \n         )            
            |  (?<WhiteSpace> \s        )
            |  (?<Character>     (([\'][\\][nrt\\'""][\'])| ([\'].[\']) |([\'][\\][u][a-fA-F0-9]{6,}[\'])))
            |  (?<String> ""([^""\n\\]|\\([nrt\\'""]|u[0-9a-fA-F]{6,}))*"")
            |  (?<Integer> [+-]?\b[0-9]+\b )
            |  (?<And>        [&][&]    )
            |  (?<Plus>       [+]       )
            |  (?<Mul>        [*]       )
            |  (?<Neg>        [-]       )
            |  (?<Div>        [/]       )
            |  (?<ParLeft>    [(]       )
            |  (?<ParRight>   [)]       )
            |  (?<BracketLeft> [[]      )
            |  (?<BracketRight> []]     )
            |  (?<KeyLeft> [{]          )
            |  (?<KeyRight> [}]         )
            |  (?<Comma>    [,]         )
            |  (?<Semicolon>    [;]     )
            |  (?<DoublePoints> [:]     )            
            |  (?<equalsTo>  [=][=]     )
            |  (?<notEqual> [!][=]      )
            |  (?<Assign>     [=]       )
            |  (?<Break> break\b        ) 
            |  (?<Decrease> dec\b       ) 
            |  (?<Do> do\b              ) 
            |  (?<Else> else\b          ) 
            |  (?<ElseIf> elseif\b      ) 
            |  (?<False> false\b        )
            |  (?<True> true\b          ) 
            |  (?<If> if\b              ) 
            |  (?<Increase> inc\b       ) 
            |  (?<Return> return\b      ) 
            |  (?<Var> var\b            ) 
            |  (?<While> while\b        )
            |  (?<remainder> [%]        )
            |  (?<not> [!]              )
            |  (?<or> [\|][\|]          )
            |  (?<xor> [\^]             )            
            |  (?<grtrEqualThan> [>][=] )
            |  (?<lssEqualThan>  [<][=] )
            |  (?<grtrThan> [>]         )
            |  (?<lssThan>  [<]         )
            |  (?<Identifier> ([a-zA-Z])([a-zA-Z0-9_])* ) 
            |  (?<Other> .              )
            ",
            RegexOptions.IgnorePatternWhitespace
                | RegexOptions.Compiled
                | RegexOptions.Multiline
            );


        static readonly IDictionary<string, TokenCategory> tokenMap =
            new Dictionary<string, TokenCategory>() {
                {"And", TokenCategory.AND},
                {"lssThan", TokenCategory.LESS_THAN},
                {"Plus", TokenCategory.PLUS},
                {"Mul", TokenCategory.MUL},
                {"Neg", TokenCategory.NEG},
                {"ParLeft", TokenCategory.PARENTHESIS_OPEN},
                {"ParRight", TokenCategory.PARENTHESIS_CLOSE},
                {"Assign", TokenCategory.ASSIGN},
                {"True", TokenCategory.TRUE},
                {"False", TokenCategory.FALSE},
                {"If", TokenCategory.IF},
                {"Identifier", TokenCategory.IDENTIFIER},
                {"Break", TokenCategory.BREAK},
                {"Increase", TokenCategory.INC},
                {"Decrease", TokenCategory.DEC},
                {"Do", TokenCategory.DO},
                {"Var", TokenCategory.VAR},
                {"While", TokenCategory.WHILE},
                {"Return", TokenCategory.RETURN},
                {"Else", TokenCategory.ELSE},
                {"ElseIf", TokenCategory.ELSEIF},
                {"remainder", TokenCategory.REMAINDER},
                {"not", TokenCategory.NOT},
                {"or", TokenCategory.OR},
                {"xor", TokenCategory.XOR},
                {"equalsTo", TokenCategory.EQUALS_TO},
                {"notEqual", TokenCategory.NOT_EQUAL},
                {"grtrThan", TokenCategory.GREATER_THAN},
                {"grtrEqualThan", TokenCategory.GREATER_EQUAL_THAN},
                {"lssEqualThan", TokenCategory.LESS_EQUAL_THAN},
                {"BracketLeft", TokenCategory.BRACKET_LEFT},
                {"BracketRight", TokenCategory.BRACKET_RIGHT},
                {"KeyLeft", TokenCategory.KEY_LEFT},
                {"KeyRight", TokenCategory.KEY_RIGHT},
                {"Semicolon", TokenCategory.SEMICOLON},
                {"DoublePoints", TokenCategory.DOUBLE_POINTS},
                {"Comma", TokenCategory.COMMA},
                {"Character", TokenCategory.CHARACTER},
                {"String", TokenCategory.STRING},
                {"Newline", TokenCategory.NEW_LINE},
                {"WhiteSpace", TokenCategory.WHITE_SPACE},
                {"Integer", TokenCategory.INT},
                {"Div", TokenCategory.DIV}

            };

        public Scanner(string input) {
            this.input = input;
        }

        public IEnumerable<Token> Scan() {

            var result = new LinkedList<Token>();
            var row = 1;
            var columnStart = 0;

            foreach (Match m in regex.Matches(input)) {

                if (m.Groups["Newline"].Success) {

                    row++;
                    columnStart = m.Index + m.Length;

                } else if (m.Groups["WhiteSpace"].Success
                    || m.Groups["Comment"].Success ) {

                    // Skip white space and comments.

                } else if (m.Groups["Other"].Success) {
                    

                    // Found an illegal character.
                    result.AddLast(
                        new Token(m.Value,
                            TokenCategory.ILLEGAL_CHAR,
                            row,
                            m.Index - columnStart + 1));

                }else if (m.Groups["MultiLineComment"].Success){

                    // A multiline comment is detected and counts the newlines within it
                    var newLineCount = CountComentLines(m.Value);
                    row+= newLineCount;


                } else {
                    // Must be any of the other tokens.
                    result.AddLast(FindToken(m, row, columnStart));
                }
            }

            result.AddLast(
                new Token(null,
                    TokenCategory.EOF,
                    row,
                    input.Length - columnStart + 1));

            return result;
        }

        private int CountComentLines(String comment){
            var newLines = 0;
            foreach(Match m in newLineRegex.Matches(comment)){
                newLines ++;
            }
            return newLines;
        }

        Token FindToken(Match m, int row, int columnStart) {
            foreach (var name in tokenMap.Keys) {
                if (m.Groups[name].Success) {
                    return new Token(m.Value,
                        tokenMap[name],
                        row,
                        m.Index - columnStart + 1);
                }
            }
            throw new InvalidOperationException(
                "regex and tokenMap are inconsistent: " + m.Value);
        }
    }
}

