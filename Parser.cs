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
 *  exprMul ::= exprUnary ( ( '*' | '/' | '%' ) exprUnary )*
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
    class Parser{
        IEnumerator<Token> tokenStream;

        public Parser(IEnumerator<Token> tokenStream) {
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
            defList();
        }

        public Node defList()
        {
            switch (Current){
                case TokenCategory.VAR: 
                    varDef();
                    break;
                
                case TokenCategory.IDENTIFIER: 
                    funDef();
                    break;
                
                default: throw new SyntaxError(Current, tokenStream.Current);

            }
        }

        public Node varDef(){
            Expect(TokenCategory.VAR);
            idList();
            Expect(TokenCategory.SEMICOLON);
        }

        public Node idList(){
            if (Current == TokenCategory.IDENTIFIER)
            {
                Expect(TokenCategory.IDENTIFIER);
                idListCont();
            }
            
        }

        public Node idListCont(){
            if(Current == TokenCategory.COMMA)
            {
                Expect(TokenCategory.COMMA);
                Expect(TokenCategory.IDENTIFIER);
                idListCont();
            }
        }
        
        public Node funDef(){
            Expect(TokenCategory.IDENTIFIER);
            Expect(TokenCategory.PARENTHESIS_OPEN);
            idList();
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.KEY_LEFT);
            varDefList();
            stmtList();
            Expect(TokenCategory.KEY_RIGHT); 
        }

        public Node varDefList()
        {
            while (Current == TokenCategory.VAR)
            {
                varDef();
            }
        }

        public Node stmtList(){
            while (Current != TokenCategory.KEY_RIGHT)
            {
                switch (Current){
                    case TokenCategory.IDENTIFIER:
                        Expect(TokenCategory.IDENTIFIER);
                        switch (Current){
                            case TokenCategory.ASSIGN:
                                Expect(TokenCategory.ASSIGN);
                                expr();
                                Expect(TokenCategory.SEMICOLON);
                                break;
                            case TokenCategory.PARENTHESIS_OPEN:
                                Expect(TokenCategory.PARENTHESIS_OPEN);
                                exprList();
                                Expect(TokenCategory.PARENTHESIS_CLOSE);
                                Expect(TokenCategory.SEMICOLON);
                                break;
                            default: throw new SyntaxError(Current, tokenStream.Current);
                        }
                        break;


                    case TokenCategory.INC:
                        stmtIncr();
                        break;


                    case TokenCategory.DEC:
                        stmtDecr();
                        break;

                    case TokenCategory.IF:
                        stmtIf();
                        break;

                    case TokenCategory.WHILE: 
                        stmtWhile();
                        break;

                    case TokenCategory.DO:
                        stmtDoWhile();
                        break;
                
                    case TokenCategory.BREAK:
                        stmtBreak();
                        break;
                
                    case TokenCategory.RETURN:
                        stmtReturn(); 
                        break;

                    case TokenCategory.SEMICOLON:
                        stmtEmpty();
                        break;

                    default: throw new SyntaxError(Current, tokenStream.Current);
                }
            }
        }

        public Node stmtIncr(){
            Expect(TokenCategory.INC);
            Expect(TokenCategory.IDENTIFIER);
            Expect(TokenCategory.SEMICOLON);
        }

        public Node stmtDecr(){
            Expect(TokenCategory.DEC);
            Expect(TokenCategory.IDENTIFIER);
            Expect(TokenCategory.SEMICOLON);
        }

        public Node exprList(){
            expr();
            exprListCont();
        } 

        public Node exprListCont(){
            if(Current == TokenCategory.COMMA)
            {
                Expect(TokenCategory.COMMA);
                expr();
                exprListCont();
            }
        }

        public Node stmtIf(){
            Expect(TokenCategory.IF);
            Expect(TokenCategory.PARENTHESIS_OPEN);
            expr();
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.KEY_LEFT);
            stmtList();
            Expect(TokenCategory.KEY_RIGHT);
            elseIfList();
            stmtElse();
        }   

        public Node elseIfList(){
            while(Current == TokenCategory.ELSEIF){
                Expect(TokenCategory.ELSEIF);
                Expect(TokenCategory.PARENTHESIS_OPEN);
                expr();
                Expect(TokenCategory.PARENTHESIS_CLOSE);
                Expect(TokenCategory.KEY_LEFT);
                stmtList();
                Expect(TokenCategory.KEY_RIGHT);
            }
        }

        public Node stmtElse(){
            if (Current == TokenCategory.ELSE)
            {
                Expect(TokenCategory.ELSE);
                Expect(TokenCategory.KEY_LEFT);
                stmtList();
                Expect(TokenCategory.KEY_RIGHT);  
            }
            
        }

        public Node stmtWhile(){
            Expect(TokenCategory.WHILE);
            Expect(TokenCategory.PARENTHESIS_OPEN);
            expr();
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.KEY_LEFT);
            stmtList();
            Expect(TokenCategory.KEY_RIGHT);
        }

        public Node stmtDoWhile(){
            Expect(TokenCategory.DO);
            Expect(TokenCategory.KEY_LEFT);
            stmtList();
            Expect(TokenCategory.KEY_RIGHT);
            Expect(TokenCategory.WHILE);
            Expect(TokenCategory.PARENTHESIS_OPEN);
            stmtList();
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.SEMICOLON);
        }

        public Node stmtBreak(){
            Expect(TokenCategory.BREAK);
            Expect(TokenCategory.SEMICOLON);
        }

        public Node stmtReturn(){
            Expect(TokenCategory.RETURN);
            expr();
            Expect(TokenCategory.SEMICOLON);
        }

        public Node stmtEmpty(){
            Expect(TokenCategory.SEMICOLON);
        }

        public Node expr(){
            exprOr();
        }

        public Node exprOr(){
            exprAnd();
            while(Current == TokenCategory.OR || 
                    Current == TokenCategory.XOR){
                switch (Current){
                    case TokenCategory.OR:
                        Expect(TokenCategory.OR);
                        break;
                    case TokenCategory.XOR:
                        Expect(TokenCategory.XOR);
                        break;
                    default: throw new SyntaxError(Current, tokenStream.Current);
                }
                exprAnd();
            }

        }

        public Node exprAnd(){
            exprComp();
            while(Current == TokenCategory.AND){
                Expect(TokenCategory.AND);
                exprComp();
            }
        }

        public Node exprComp(){
            exprRel();
            while(Current == TokenCategory.EQUALS_TO || 
                    Current == TokenCategory.NOT_EQUAL){
                switch(Current){
                    case TokenCategory.EQUALS_TO:
                        Expect(TokenCategory.EQUALS_TO);
                        break;
                    case TokenCategory.NOT_EQUAL:
                        Expect(TokenCategory.NOT_EQUAL);
                        break;
                    default: throw new SyntaxError(Current, tokenStream.Current);
                }
                exprRel();
            }
        }

        public Node exprRel(){
            exprAdd();
            while(Current == TokenCategory.GREATER_THAN ||
                    Current == TokenCategory.GREATER_EQUAL_THAN ||
                    Current == TokenCategory.LESS_THAN || 
                    Current == TokenCategory.LESS_EQUAL_THAN){
                switch(Current){
                    case TokenCategory.GREATER_THAN:
                        Expect(TokenCategory.GREATER_THAN);
                        break;
                    case TokenCategory.GREATER_EQUAL_THAN:
                        Expect(TokenCategory.GREATER_EQUAL_THAN);
                        break;
                    case TokenCategory.LESS_THAN:
                        Expect(TokenCategory.LESS_THAN);
                        break;
                    case TokenCategory.LESS_EQUAL_THAN:
                        Expect(TokenCategory.LESS_EQUAL_THAN);
                        break;
                    default: throw new SyntaxError(Current, tokenStream.Current);
                }
                exprAdd();
            }
        }

        public Node exprAdd(){
            exprMul();
            while(Current == TokenCategory.PLUS || Current == TokenCategory.NEG){
                switch(Current){
                    case TokenCategory.PLUS:
                        Expect(TokenCategory.PLUS);
                        break;
                    case TokenCategory.NEG:
                        Expect(TokenCategory.NEG);
                        break;
                    default: throw new SyntaxError(Current, tokenStream.Current);
                }
                exprMul();
            }
        }

        public Node exprMul(){
            exprUnary();
            while(Current == TokenCategory.MUL ||
                    Current == TokenCategory.DIV ||
                    Current == TokenCategory.REMAINDER){
                switch(Current){
                    case TokenCategory.MUL:
                        Expect(TokenCategory.MUL);
                        break;
                    case TokenCategory.DIV:
                        Expect(TokenCategory.DIV);
                        break;
                    case TokenCategory.REMAINDER:
                        Expect(TokenCategory.REMAINDER);
                        break;
                    default: throw new SyntaxError(Current, tokenStream.Current);
                }
                exprUnary();
            }
        }

        public Node exprUnary(){
            exprPrimary();
            while(Current == TokenCategory.PLUS ||
                    Current == TokenCategory.NEG ||
                    Current == TokenCategory.NOT){
                switch(Current){
                    case TokenCategory.PLUS:
                        Expect(TokenCategory.PLUS);
                        break;
                    case TokenCategory.NEG:
                        Expect(TokenCategory.NEG);
                        break;
                    case TokenCategory.NOT:
                        Expect(TokenCategory.NOT);
                        break;
                    default: throw new SyntaxError(Current, tokenStream.Current);
                }
                exprPrimary();    
            }
        }

        public Node exprPrimary(){
            switch (Current){
                case TokenCategory.IDENTIFIER:
                    Expect(TokenCategory.IDENTIFIER);
                    if(Current == TokenCategory.PARENTHESIS_OPEN){
                        Expect(TokenCategory.PARENTHESIS_OPEN);
                        exprList();
                        Expect(TokenCategory.PARENTHESIS_CLOSE);
                    }
                    break;
                case TokenCategory.BRACKET_LEFT:
                    array();
                    break;
                case TokenCategory.TRUE:
                    lit();
                    break;
                case TokenCategory.FALSE:
                    lit();
                    break;
                case TokenCategory.INT:
                    lit();
                    break;
                case TokenCategory.CHARACTER:
                    lit();
                    break;
                case TokenCategory.STRING:
                    lit();
                    break;

                case TokenCategory.PARENTHESIS_OPEN:
                    Expect(TokenCategory.PARENTHESIS_OPEN);
                    expr();
                    Expect(TokenCategory.PARENTHESIS_CLOSE);
                    break;
                default: throw new SyntaxError(Current, tokenStream.Current);
            }
        }

        public Node array(){
            Expect(TokenCategory.BRACKET_LEFT);
            exprList();
            Expect(TokenCategory.BRACKET_RIGHT);
        }

        public Node lit(){
            switch(Current){
                case TokenCategory.TRUE:
                    Expect(TokenCategory.TRUE);
                    break;
                case TokenCategory.FALSE:
                    Expect(TokenCategory.FALSE);
                    break;
                case TokenCategory.INT:
                    Expect(TokenCategory.INT);
                    break;
                case TokenCategory.CHARACTER:
                    Expect(TokenCategory.CHARACTER);
                    break;
                case TokenCategory.STRING:
                    Expect(TokenCategory.STRING);
                    break;
                default: throw new SyntaxError(Current, tokenStream.Current);
            }
        }



}
}
