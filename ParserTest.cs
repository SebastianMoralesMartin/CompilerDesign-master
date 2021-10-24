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
    class ParserTest{
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
            var newNode = new Program();
            newNode.Add(result);
            return newNode;
        }

        public Node defList()
        {
            var result = new defList();
            while (Current == TokenCategory.VAR || 
                   Current == TokenCategory.IDENTIFIER)
            {
                switch (Current){
                    case TokenCategory.VAR:
                        result.Add(varDef());
                        break;
                
                    case TokenCategory.IDENTIFIER: 
                        result.Add(funDef());
                        break;
                
                    default: throw new SyntaxError(Current, tokenStream.Current);
                }
            }
            

            return result;
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
                if (Current == TokenCategory.COMMA)
                {
                    result.Add(idListCont()); 
                }
                
            }

            return result;
        }

        public Node idListCont()
        {
            var result = new idListCont();
            if(Current == TokenCategory.COMMA)
            {
                Console.WriteLine("idListCon inside if (current == comma), Current: " + Current);
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
                                Console.WriteLine("Case Assign Current: " + Current);
                                result.Add(new assign()
                                {
                                    AnchorToken = Expect(TokenCategory.ASSIGN)
                                });
                                
                                result.Add(expr());
                                Console.WriteLine("After ADD ASIGN Current: " + Current);
                                //Expect(TokenCategory.SEMICOLON);
                                break;
                            case TokenCategory.PARENTHESIS_OPEN: //funCall
                                Expect(TokenCategory.PARENTHESIS_OPEN);
                                result.Add(exprList());
                                Expect(TokenCategory.PARENTHESIS_CLOSE);
                                //Expect(TokenCategory.SEMICOLON);
                                break;
                            default: throw new SyntaxError(Current, tokenStream.Current);
                        }

                        break;
                        


                    case TokenCategory.INC:
                        Console.WriteLine("case INC Current: " + Current);
                        result.Add(stmtIncr());
                        break;


                    case TokenCategory.DEC:
                        result.Add(stmtDecr());
                        break;

                    case TokenCategory.IF:
                        result.Add(stmtIf());
                        break;
                    case TokenCategory.WHILE: 
                        result.Add(stmtWhile());
                        break;
                    
                    case TokenCategory.DO:
                        result.Add(stmtDoWhile());
                        break;
                
                    case TokenCategory.BREAK:
                        result.Add(stmtBreak());
                        break;
                
                    case TokenCategory.RETURN:
                        result.Add(stmtReturn());
                        break;

                    case TokenCategory.SEMICOLON: //Empty statement case
                        result.Add(stmtEmpty());
                        break;

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
            Console.WriteLine("exprList Running Current: " + Current);
            var result = new exprList();
            if (Current != TokenCategory.PARENTHESIS_CLOSE)
            {
                result.Add(expr());
                result.Add(exprListCont());
            }
            return result;
        } 

        public Node exprListCont()
        {
            Console.WriteLine("exprListCont running Current: " + Current);
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
            if (Current == TokenCategory.ELSEIF)
            {
                result.Add(elseIfList());
            }

            if (Current == TokenCategory.ELSE)
            {
                result.Add(stmtElse());
            }



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
                
            }
            
            return result;

        }

        public Node stmtElse(){
            var result = new stmtElse()
            {
                AnchorToken = Expect(TokenCategory.ELSE)
            };
            Expect(TokenCategory.KEY_LEFT);
            result.Add(stmtList());
            Expect(TokenCategory.KEY_RIGHT);

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
            Console.WriteLine("expr running Current: "+  Current);
            var result = new expr();
            result.Add(exprOr());
            return result;
        }

        public Node exprOr()
        {
            var result = exprAnd();
            
            while(Current == TokenCategory.OR || 
                    Current == TokenCategory.XOR){
                switch (Current){
                    case TokenCategory.OR:
                        var node1 = new Or()
                        {
                            AnchorToken = Expect(TokenCategory.OR)
                        };
                        node1.Add(result);
                        node1.Add(exprAnd());
                        result = node1;
                        break;
                    case TokenCategory.XOR:
                        var node2 = new Xor()
                        {
                            AnchorToken = Expect(TokenCategory.EQUALS_TO)
                        };
                        node2.Add(result);
                        node2.Add(exprAnd());
                        result = node2;
                        break;
                    default: throw new SyntaxError(Current, tokenStream.Current);
                }
            }

            return result;

        }

        public Node exprAnd()
        {
            var result = exprComp();
            
            while(Current == TokenCategory.AND)
            {
                var node = new And()
                {
                    AnchorToken = Expect(TokenCategory.AND)
                };
                node.Add(result);
                node.Add(exprComp());
                result = node;
            }

            return result;
        }
        public Node exprComp()
        {
            
            var result = exprRel();
            Console.WriteLine("EQUALSTO Current: " + Current);
            while(Current == TokenCategory.EQUALS_TO || 
                  Current == TokenCategory.NOT_EQUAL){
                Console.WriteLine("Before Switch EQUALSTO Current: " + Current);
                switch(Current){
                    case TokenCategory.EQUALS_TO:
                        Console.WriteLine("EQUALSTO Current: " + Current);
                        var node1 = new equals_to()
                        {
                            AnchorToken = Expect(TokenCategory.EQUALS_TO)
                        };
                        node1.Add(result);
                        node1.Add(exprRel());
                        result = node1;
                        break;
                    case TokenCategory.NOT_EQUAL:
                        var node2 = new not_equal()
                        {
                            AnchorToken = Expect(TokenCategory.NOT_EQUAL)
                        };
                        node2.Add(result);
                        node2.Add(exprRel());
                        result = node2;
                        break;
                    default: throw new SyntaxError(Current, tokenStream.Current);
                }
            }

            return result;
        }

        public Node exprRel()
        {
            var result = exprAdd();
            
            while(Current == TokenCategory.GREATER_THAN || 
                  Current == TokenCategory.GREATER_EQUAL_THAN ||
                  Current == TokenCategory.LESS_THAN || 
                  Current == TokenCategory.LESS_EQUAL_THAN){ 
                switch(Current){
                    case TokenCategory.GREATER_THAN:
                        var node1 = new greater_than()
                        {
                            AnchorToken = Expect(TokenCategory.GREATER_THAN)
                        };
                        node1.Add(result);
                        node1.Add(exprAdd());
                        result = node1;
                        break;
                    case TokenCategory.GREATER_EQUAL_THAN:
                        var node2 = new greater_equal_than()
                        {
                            AnchorToken = Expect(TokenCategory.GREATER_EQUAL_THAN)
                        };
                        node2.Add(result);
                        node2.Add(exprAdd());
                        result = node2;
                        break;
                    case TokenCategory.LESS_THAN:
                        var node3 = new less_than()
                        {
                            AnchorToken = Expect(TokenCategory.LESS_THAN)
                        };
                        node3.Add(result);
                        node3.Add(exprAdd());
                        result = node3;
                        break;
                    case TokenCategory.LESS_EQUAL_THAN:
                        var node4 = new less_equal_than()
                        {
                            AnchorToken = Expect(TokenCategory.LESS_EQUAL_THAN)
                        };
                        node4.Add(result);
                        node4.Add(exprAdd());
                        result = node4;
                        break;
                    default: throw new SyntaxError(Current, tokenStream.Current);
                }
            }
            return result;
        }

        public Node exprAdd()
        {
            var result = exprMul();
            
            while(Current == TokenCategory.PLUS || Current == TokenCategory.NEG)
            {
                
                switch(Current){
                    case TokenCategory.PLUS:
                        Console.WriteLine("case plus Current: " + Current);
                       var node1 = new Plus()
                        {
                            AnchorToken = Expect(TokenCategory.PLUS)
                        };
                        node1.Add(result);
                        node1.Add(exprMul());
                        result = node1;
                        break;
                    case TokenCategory.NEG:
                        var node2 = new Minus()
                        {
                            AnchorToken = Expect(TokenCategory.NEG)
                        };
                        node2.Add(result);
                        node2.Add(exprMul());
                        result = node2;
                        break;
                    default: throw new SyntaxError(Current, tokenStream.Current);
                }
            }

            
            return result;
        }

        public Node exprMul()
        {
            var result = exprUnary();
            
            while(Current == TokenCategory.MUL ||
                    Current == TokenCategory.DIV ||
                    Current == TokenCategory.REMAINDER){
                switch(Current){
                    case TokenCategory.MUL:
                        var node1 = new Mul()
                        {
                            AnchorToken = Expect(TokenCategory.MUL)
                        };
                        node1.Add(result);
                        node1.Add(exprUnary());
                        result = node1;
                        break;
                    case TokenCategory.DIV:
                        var node2 = new Div()
                        {
                            AnchorToken = Expect(TokenCategory.DIV)
                        };
                        node2.Add(result);
                        node2.Add(exprUnary());
                        result = node2;
                        break;
                    case TokenCategory.REMAINDER:
                        var node3 = new Remainder()
                        {
                            AnchorToken = Expect(TokenCategory.REMAINDER)
                        };
                        node3.Add(result);
                        node3.Add(exprUnary());
                        result = node3;
                        break;
                    default: throw new SyntaxError(Current, tokenStream.Current);
                }
            }
            return result;
        }

        public Node exprUnary()
        {
            var result =  exprPrimary();
            //result.Add(exprPrimary());
            while(Current == TokenCategory.PLUS ||
                    Current == TokenCategory.NEG ||
                    Current == TokenCategory.NOT){
                switch(Current){
                    case TokenCategory.PLUS:
                        var posTokenPlus = Expect(TokenCategory.PLUS);
                        var exprPlus = exprPrimary();
                        result = new Positive() {exprPlus};
                        result.AnchorToken = posTokenPlus;
                        return result;
                    case TokenCategory.NEG:
                        var posTokenNeg = Expect(TokenCategory.NEG);
                        var exprNeg = exprPrimary();
                        result = new Negative() {exprNeg};
                        result.AnchorToken = posTokenNeg;
                        return result;
                    case TokenCategory.NOT:
                        var posTokenNot = Expect(TokenCategory.NOT);
                        var exprNot = exprPrimary();
                        result = new Not() {exprNot};
                        result.AnchorToken = posTokenNot;
                        return result;
                    default: throw new SyntaxError(Current, tokenStream.Current);
                }
            }
            return result;
        }

        public Node exprPrimary()
        {
            switch (Current){
                case TokenCategory.IDENTIFIER:
                    var nodeId = new identifier()
                    {
                        AnchorToken = Expect(TokenCategory.IDENTIFIER)
                    };
                    if(Current == TokenCategory.PARENTHESIS_OPEN){
                        Expect(TokenCategory.PARENTHESIS_OPEN);
                        nodeId.Add(exprList());
                        Expect(TokenCategory.PARENTHESIS_CLOSE);
                    }
                    return nodeId;
                case TokenCategory.BRACKET_LEFT:
                    var nodeArr = array();
                    return nodeArr;
                case TokenCategory.TRUE:
                    return lit();
                case TokenCategory.FALSE:
                    return lit();
                case TokenCategory.INT:
                    return lit();
                case TokenCategory.CHARACTER:
                    return lit();
                case TokenCategory.STRING:
                    return lit();
                
                case TokenCategory.PARENTHESIS_OPEN:
                    Expect(TokenCategory.PARENTHESIS_OPEN);
                    var nodePar = exprList();
                    Expect(TokenCategory.PARENTHESIS_CLOSE);
                    return nodePar;
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
                    break;
                case TokenCategory.FALSE:
                    result.Add(new FALSE()
                    {
                        AnchorToken = Expect(TokenCategory.FALSE)
                    });
                    break;
                case TokenCategory.INT:
                    result.Add(new INT()
                    {
                        AnchorToken = Expect(TokenCategory.INT)
                    });
                    break;
                case TokenCategory.CHARACTER:
                    result.Add(new Character()
                    {
                        AnchorToken = Expect(TokenCategory.CHARACTER)
                    });
                    break;
                case TokenCategory.STRING:
                    result.Add(new STRING()
                    {
                        AnchorToken = Expect(TokenCategory.STRING)
                    });
                    break;
                default: throw new SyntaxError(Current, tokenStream.Current);
            }

            return result;
        }
}
}
//pa pushear
//2