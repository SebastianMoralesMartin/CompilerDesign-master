/*
    A01745219 Eduardo R. Muller Romero
    A01376228 Sebastian Morales Martin
    A01746645 Guillermo Adrian Urbina A.
*/

/*
 *  LL(1):
 *
 *  Program ::= defList
 *  defList ::= (varDef | funDef)* 
 *  varDef ::= 'var' idList ';'
 *  idList ::= id idListCont
 *  idListCont ::= (',' id idListCont)?
 *  *funDef ::= id '(' idList ')' '{' varDefList stmtList '}'
 *
 *  varDefList ::= ( varDef )*
 * 
 *  stmtList ::= ( id ( ( '=' expr ';' ) | ( '(' exprList ';' ')' ) |
 *                  stmtIncr | stmtDecr | stmtIf | stmtWhile |
 *                  stmtDoWhile | stmtBreak | stmtReturn | stmtEmpty )*
 *
 * 
 *          *stmtAssign ::= id '=' expr ';'
 *  stmtIncr ::= 'inc' id ';'
 *  stmtDecr ::= 'dec' id ';'
 *          *stmtFunCall ::= funCall ';'
 *          *funCall ::= id '(' exprList ')'
 *  exprList ::= expr exprListCont
 *  exprListCont ::= ',' expr exprListcont
 *  stmtIf ::= 'if' '(' expr ')' '{' stmtList '}' (elseIfList else)?
 *  elseIfList ::= ( 'elseif' '(' expr ')' '{' stmtList '}' )*
 *  else ::= 'else' '{' stmtlist '}'
 *  stmtWhile ::= 'while' '(' expr ')' '{' stmtList '}'
 *  stmtDoWhile ::= 'do' '{' stmtList '}' 'while' '(' expr ')' ';'
 *  stmtBreak ::= 'break' ';'
 *  stmtReturn ::= 'return' expr ';'
 *  stmtEmpty ';'
 *  expr ::= exprOr
 *
 *
 * SIMPPLIFICADO:   
 *  exprOr ::= exprAnd ( ( '||' | '^' ) exprAnd )*
 *  exprAnd ::= exprComp ( '&&' exprComp )*
 *  exprComp ::= exprRel ( ( '==' | '!=' ) exprRel )*
 *  exprRel ::= exprAdd ( ( '<' | '<=' | '>' | '>=' ) exprAdd )*
 *  exprAdd ::= exprMul ( ( '+' | '-' ) exprMul )*
 *  exprMul ::= exprUnary ( ( '' | '/' | '%' ) exprUnary )
 *  exprUnary ::= exprPrimary ( ( '+' | '-' | '!' ) exprPrimary )* 
 *
 *
 * 
 *  exprPrimary ::= ( id ( '(' exprList ')' )?  | array | lit | ( '(' expr ')' ) )
 *  array ::= '[' exprList ']'
 *  lit ::= ( litBool | litInt | litChar | litStr )
 * 
 */

using System;
using System.Collections.Generic;

namespace Falak
{
    public class ParserTest{
        IEnumerator<Token> tokenStream;

        public ParserTest(IEnumerator<Token> tokenStream) {
            this.tokenStream = tokenStream;
            this.tokenStream.MoveNext();
        }

        public TokenCategory Current {
            get {
                return tokenStream.Current.Category;
            }
        }

        public Token Expect(TokenCategory category) {
            if (Current == category) {
                Token current = tokenStream.Current;
                tokenStream.MoveNext();
                return current;
            } else {
                throw new SyntaxError(category, tokenStream.Current);
            }
        }

        public Node Program()
        {
            var result = defList();    
            Expect(TokenCategory.EOF);
            var newNode = new Program();
            newNode.Add(result);
            return newNode;
        }

        public Node defList()
        {
            var result = new defList();
            switch (Current){
                case TokenCategory.VAR:
                    result.Add(varDef());
                    return result;
                
                case TokenCategory.IDENTIFIER: 
                    result.Add(funDef());
                    return result;
                
                default: throw new SyntaxError(Current, tokenStream.Current);
            }
        }

        public Node varDef()
        {
            var result = new varDef()
            {
                AnchorToken = Expect(TokenCategory.VAR)
            };
            result.Add(idList());
            Expect(TokenCategory.SEMICOLON);
            return result;
        }

        public Node idList()
        {
            var result = new idList();
            if (Current == TokenCategory.IDENTIFIER)
            {
                result.Add(new identifier()
                {
                    AnchorToken = Expect(TokenCategory.IDENTIFIER)
                });
                result.Add(idListCont());
            }

            return result;
        }

        public Node idListCont()
        {
            var result = new idListCont();
            if(Current == TokenCategory.COMMA)
            {
                Expect(TokenCategory.COMMA);
                result.Add(new identifier()
                {
                    AnchorToken = Expect(TokenCategory.IDENTIFIER)
                });
                result.Add(idListCont());
            }
            return result;
        }
        
        public Node funDef()
        {
            var result = new funDef();
            result.Add(new identifier()
            {
                AnchorToken = Expect(TokenCategory.IDENTIFIER)
            });
            Expect(TokenCategory.PARENTHESIS_OPEN);
            
            result.Add(idList());
            
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            
            Expect(TokenCategory.KEY_LEFT);
            result.Add(varDefList());
            
            result.Add(stmtList());
            
            Expect(TokenCategory.KEY_RIGHT);
            return result;
        }

        public Node varDefList()
        {
            var result = new varDefList();
            while (Current == TokenCategory.VAR)
            {
                result.Add(varDef());
                return result;
            }

            return result;
        }

        public Node stmtList()
        {
            var result = new stmtList();
            while (Current != TokenCategory.KEY_RIGHT)
            {
                switch (Current){
                    case TokenCategory.IDENTIFIER:
                        result.Add(new identifier()
                        {
                            AnchorToken = Expect(TokenCategory.IDENTIFIER)
                        });
                        switch (Current){
                            case TokenCategory.ASSIGN:
                                result.Add(new assign()
                                {
                                    AnchorToken = Expect(TokenCategory.ASSIGN)
                                });
                                result.Add(expr());
                                Expect(TokenCategory.SEMICOLON);
                                return result;
                            case TokenCategory.PARENTHESIS_OPEN:
                                Expect(TokenCategory.PARENTHESIS_OPEN);
                                result.Add(exprList());
                                Expect(TokenCategory.PARENTHESIS_CLOSE);
                                Expect(TokenCategory.SEMICOLON);
                                return result;
                            default: throw new SyntaxError(Current, tokenStream.Current);
                        }
                        


                    case TokenCategory.INC:
                        result.Add(stmtIncr());
                        return result;


                    case TokenCategory.DEC:
                        result.Add(stmtDecr());
                        return result;

                    case TokenCategory.IF:
                        result.Add(stmtIf());
                        return result;
                    case TokenCategory.WHILE: 
                        result.Add(stmtWhile());
                        return result;
                    
                    case TokenCategory.DO:
                        result.Add(stmtDoWhile());
                        return result;
                
                    case TokenCategory.BREAK:
                        result.Add(stmtBreak());
                        return result;
                
                    case TokenCategory.RETURN:
                        result.Add(stmtReturn());
                        return result;

                    case TokenCategory.SEMICOLON: //Empty statement case
                        result.Add(stmtEmpty());
                        return result;

                    default: throw new SyntaxError(Current, tokenStream.Current);
                }
            }

            return result;
        }
        public Node stmtIncr()
        {
            var result = new stmtIncr()
            {
                AnchorToken = Expect(TokenCategory.INC)
            };
            result.Add(new identifier()
            {
                AnchorToken = Expect(TokenCategory.IDENTIFIER)
            });
            
            Expect(TokenCategory.SEMICOLON);
            return result;
        }

        public Node stmtDecr(){
            var result = new stmtDecr()
            {
                AnchorToken = Expect(TokenCategory.DEC)
            };
            result.Add(new identifier()
            {
                AnchorToken = Expect(TokenCategory.IDENTIFIER)
            });
            
            Expect(TokenCategory.SEMICOLON);
            return result;
        }

        public Node exprList()
        {
            var result = new exprList();
            result.Add(expr());
            result.Add(exprListCont());
            return result;
        } 

        public Node exprListCont()
        {
            var result = new exprListCont();
            if(Current == TokenCategory.COMMA)
            {
                Expect(TokenCategory.COMMA);
                result.Add(expr());
                result.Add(exprListCont());
            }

            return result;
        }

        public Node stmtIf()
        {
            var result = new stmtIf()
            {
                AnchorToken = Expect(TokenCategory.IF)
            };
            Expect(TokenCategory.PARENTHESIS_OPEN);
            result.Add(expr());
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.KEY_LEFT);
            result.Add(stmtList());
            Expect(TokenCategory.KEY_RIGHT);
            result.Add(elseIfList());
            result.Add(stmtElse());
            return result;
        }   

        public Node elseIfList(){
            var result = new elseIfList(){
                AnchorToken = Expect(TokenCategory.ELSEIF)
            };
            while(Current == TokenCategory.ELSEIF)
            {
                
                
                
                Expect(TokenCategory.PARENTHESIS_OPEN);
                result.Add(expr());
                Expect(TokenCategory.PARENTHESIS_CLOSE);
                Expect(TokenCategory.KEY_LEFT);
                result.Add(stmtList());
                Expect(TokenCategory.KEY_RIGHT);
                
            }return result;

        }

        public Node stmtElse(){
            var result = new stmtElse()
            {
                AnchorToken = Expect(TokenCategory.ELSE)
            };
            if (Current == TokenCategory.ELSE)
            {
                
                
                Expect(TokenCategory.KEY_LEFT);
                result.Add(stmtList());
                Expect(TokenCategory.KEY_RIGHT);  
            }

            return result;
        }

        public Node stmtWhile()
        {
            var result = new stmtWhile()
            {
                AnchorToken = Expect(TokenCategory.WHILE)
            };
            
            Expect(TokenCategory.PARENTHESIS_OPEN);
            result.Add(expr());
            
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.KEY_LEFT);
            result.Add(stmtList());
            Expect(TokenCategory.KEY_RIGHT);
            return result;
        }

        public Node stmtDoWhile()
        {
            var result = new stmtDoWhile()
            {
                AnchorToken = Expect(TokenCategory.DO)
            };
            
            Expect(TokenCategory.KEY_LEFT);
            
            result.Add(stmtList());
            
            Expect(TokenCategory.KEY_RIGHT);
            Expect(TokenCategory.WHILE);
            Expect(TokenCategory.PARENTHESIS_OPEN);
            
            result.Add(stmtList());
            
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.SEMICOLON);
            return result; 
        }

        public Node stmtBreak()
        {
            var result = new stmtBreak()
            {
                AnchorToken = Expect(TokenCategory.BREAK)
            };
            Expect(TokenCategory.SEMICOLON);
            return result;
        }

        public Node stmtReturn()
        {
            var result = new stmtReturn
            {
                AnchorToken = Expect(TokenCategory.RETURN)
            };
            result.Add(expr());
            Expect(TokenCategory.SEMICOLON);
            return result;
        }

        public Node stmtEmpty()
        {
            var result = new stmtEmpty()
            {
                AnchorToken = Expect(TokenCategory.SEMICOLON)
            };
            return result;
        }

        public Node expr()
        {
            var result = new expr();
            result.Add(exprOr());
            return result;
        }

        public Node exprOr()
        {
            var result = new exprOr();
            result.Add(exprAnd());
            while(Current == TokenCategory.OR || 
                    Current == TokenCategory.XOR){
                switch (Current){
                    case TokenCategory.OR:
                        result.Add(new or(){
                            AnchorToken = Expect(TokenCategory.OR)
                        });
                        return result;
                    case TokenCategory.XOR:
                        result.Add(new xor()
                        {
                            AnchorToken = Expect(TokenCategory.XOR)
                        });
                        return result;
                    default: throw new SyntaxError(Current, tokenStream.Current);
                }
            }

            return result;

        }

        public Node exprAnd()
        {
            var result = new exprAnd();
            result.Add(exprComp());
            while(Current == TokenCategory.AND)
            {
                result.Add(new and()
                {
                    AnchorToken = Expect(TokenCategory.AND)
                });
            }
            return result;
        }
        public Node exprComp()
        {
            var result = new exprComp();
            result.Add(exprRel());
            while(Current == TokenCategory.EQUALS_TO || 
                  Current == TokenCategory.NOT_EQUAL){
                switch(Current){
                    case TokenCategory.EQUALS_TO:
                        result.Add(new equals_to()
                        {
                            AnchorToken = Expect(TokenCategory.EQUALS_TO)
                        });
                        return result;
                    case TokenCategory.NOT_EQUAL:
                        result.Add(new not_equal()
                        {
                            AnchorToken = Expect(TokenCategory.NOT_EQUAL)
                        });
                        return result;
                    default: throw new SyntaxError(Current, tokenStream.Current);
                }
            }

            return result;
        }

        public Node exprRel()
        {
            var result = new exprRel();
            result.Add(exprAdd());
            while(Current == TokenCategory.GREATER_THAN || 
                  Current == TokenCategory.GREATER_EQUAL_THAN ||
                  Current == TokenCategory.LESS_THAN || 
                  Current == TokenCategory.LESS_EQUAL_THAN){ 
                switch(Current){
                    case TokenCategory.GREATER_THAN:
                        result.Add(new greater_than()
                        {
                            AnchorToken = Expect(TokenCategory.GREATER_THAN)
                        });
                        return result;
                    case TokenCategory.GREATER_EQUAL_THAN:
                        result.Add(new greater_equal_than()
                        {
                            AnchorToken = Expect(TokenCategory.GREATER_EQUAL_THAN)
                        });
                        return result;
                    case TokenCategory.LESS_THAN:
                        result.Add(new less_than()
                    {
                        AnchorToken = Expect(TokenCategory.LESS_THAN)
                    });
                        return result;
                    case TokenCategory.LESS_EQUAL_THAN:
                        result.Add(new less_equal_than()
                    {
                        AnchorToken = Expect(TokenCategory.LESS_EQUAL_THAN)
                    });
                        return result;
                    default: throw new SyntaxError(Current, tokenStream.Current);
                }
            }

            return result;
        }

        public Node exprAdd()
        {
            var result = new exprAdd();
            result.Add(exprMul());
            while(Current == TokenCategory.PLUS || Current == TokenCategory.NEG){
                switch(Current){
                    case TokenCategory.PLUS:
                        result.Add(new plus()
                        {
                            AnchorToken = Expect(TokenCategory.PLUS)
                        });
                        return result;
                    case TokenCategory.NEG:
                        result.Add(new neg()
                        {
                            AnchorToken = Expect(TokenCategory.NEG)
                        });
                        return result;
                    default: throw new SyntaxError(Current, tokenStream.Current);
                }
            }

            return result;
        }

        public Node exprMul()
        {
            var result = new exprMul();
            result.Add(exprUnary());
            while(Current == TokenCategory.MUL ||
                    Current == TokenCategory.DIV ||
                    Current == TokenCategory.REMAINDER){
                switch(Current){
                    case TokenCategory.MUL:
                        result.Add(new mul()
                        {
                            AnchorToken = Expect(TokenCategory.MUL)
                        });
                        return result;
                    case TokenCategory.DIV:
                        result.Add(new div()
                        {
                            AnchorToken = Expect(TokenCategory.DIV)
                        });
                        return result;
                    case TokenCategory.REMAINDER:
                        result.Add(new remainder()
                        {
                            AnchorToken = Expect(TokenCategory.REMAINDER)
                        });
                        return result;
                    default: throw new SyntaxError(Current, tokenStream.Current);
                }
            }

            return result;
        }

        public Node exprUnary()
        {
            var result = new exprUnary();
            result.Add(exprPrimary());
            while(Current == TokenCategory.PLUS ||
                    Current == TokenCategory.NEG ||
                    Current == TokenCategory.NOT){
                switch(Current){
                    case TokenCategory.PLUS:
                        result.Add(new plus()
                        {
                            AnchorToken = Expect(TokenCategory.PLUS)
                        });
                        return result;
                    case TokenCategory.NEG:
                        result.Add(new neg()
                        {
                            AnchorToken = Expect(TokenCategory.NEG)
                        });
                        return result;
                    case TokenCategory.NOT:
                        result.Add(new not()
                        {
                            AnchorToken = Expect(TokenCategory.NOT)
                        });
                        return result;
                    default: throw new SyntaxError(Current, tokenStream.Current);
                }
            }

            return result;
        }

        public Node exprPrimary()
        {
            var result = new exprPrimary();
            switch (Current){
                case TokenCategory.IDENTIFIER:
                    result.Add(new identifier()
                    {
                        AnchorToken = Expect(TokenCategory.IDENTIFIER)
                    });
                    if(Current == TokenCategory.PARENTHESIS_OPEN){
                        Expect(TokenCategory.PARENTHESIS_OPEN);
                        result.Add(exprList());
                        Expect(TokenCategory.PARENTHESIS_CLOSE);
                        return result;
                    }
                    return result;
                case TokenCategory.BRACKET_LEFT:
                    result.Add(array());
                    return result;
                case TokenCategory.TRUE:
                    result.Add(lit());
                    return result;
                case TokenCategory.FALSE:
                    result.Add(lit());
                    return result;
                case TokenCategory.INT:
                    result.Add(lit());
                    return result;
                case TokenCategory.CHARACTER:
                    result.Add(lit());
                    return result;
                case TokenCategory.STRING:
                    result.Add(lit());
                    return result;
                
                case TokenCategory.PARENTHESIS_OPEN:
                    Expect(TokenCategory.PARENTHESIS_OPEN);
                    result.Add(expr());
                    Expect(TokenCategory.PARENTHESIS_CLOSE);
                    return result;
                default: throw new SyntaxError(Current, tokenStream.Current);
            }
        }

        public Node array()
        {
            var result = new array();
            Expect(TokenCategory.BRACKET_LEFT);
            result.Add(exprList());
            Expect(TokenCategory.BRACKET_RIGHT);
            return result;
        }

        public Node lit()
        {
            var result = new lit();
            switch(Current){
                case TokenCategory.TRUE:
                    result.Add(new TRUE()
                {
                    AnchorToken = Expect(TokenCategory.TRUE)
                });
                    return result;
                case TokenCategory.FALSE:
                    result.Add(new FALSE()
                    {
                        AnchorToken = Expect(TokenCategory.FALSE)
                    });
                    return result;
                case TokenCategory.INT:
                    result.Add(new INT()
                    {
                        AnchorToken = Expect(TokenCategory.INT)
                    });
                    return result;
                case TokenCategory.CHARACTER:
                    result.Add(new character()
                    {
                        AnchorToken = Expect(TokenCategory.CHARACTER)
                    });
                    return result;
                case TokenCategory.STRING:
                    result.Add(new STRING()
                    {
                        AnchorToken = Expect(TokenCategory.STRING)
                    });
                    return result;
                default: throw new SyntaxError(Current, tokenStream.Current);
            }
        }
}
}