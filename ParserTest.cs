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
            var result = defList();
            var newNode = new Program();
            newNode.Add(result);
            return newNode;
        }

        public Node defList()
        {
            var result = 0;
            switch (Current){
                case TokenCategory.VAR: 
                    var result = varDef();
                    var newNode = new defList();
                    newNode.Add(result);
                    return newNode;
                    break;
                
                case TokenCategory.IDENTIFIER: 
                    var result = funDef();
                    var newNode = new defList();
                    newNode.Add(result);
                    return newNode;
                    break;
                    
                default: throw new SyntaxError(Current, tokenStream.Current);
            }
        }

        public Node varDef(){
            Expect(TokenCategory.VAR);
            var result = idList();
            Expect(TokenCategory.SEMICOLON);
            return result;
        }

        public Node idList(){
            if (Current == TokenCategory.IDENTIFIER)
            {
                Expect(TokenCategory.IDENTIFIER);
                var result = idListCont();
            }

            //return result;
        }

        public Node idListCont(){
            if(Current == TokenCategory.COMMA)
            {
                Expect(TokenCategory.COMMA);
                Expect(TokenCategory.IDENTIFIER);
                var result = idListCont();
            }

            return result;
        }
        
        public Node funDef()
        {
            var newNode = new funDef();
            Expect(TokenCategory.IDENTIFIER);
            Expect(TokenCategory.PARENTHESIS_OPEN);
            var result = idList();
            newNode.Add(result);
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
                result = varDef();
                return result;
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
                                result = expr();
                                Expect(TokenCategory.SEMICOLON);
                                return result;
                                break;
                            case TokenCategory.PARENTHESIS_OPEN:
                                Expect(TokenCategory.PARENTHESIS_OPEN);
                                result = exprList();
                                Expect(TokenCategory.PARENTHESIS_CLOSE);
                                Expect(TokenCategory.SEMICOLON);
                                return result;
                                break;
                            default: throw new SyntaxError(Current, tokenStream.Current);
                        }
                        break;


                    case TokenCategory.INC:
                        result = stmtIncr();
                        return result;
                        break;


                    case TokenCategory.DEC:
                        result = stmtDecr();
                        return result;
                        break;

                    case TokenCategory.IF:
                        result = stmtIf();
                        return result;
                        break;

                    case TokenCategory.WHILE: 
                        result = stmtWhile();
                        return result;
                        break;

                    case TokenCategory.DO:
                        result = stmtDoWhile();
                        return result;
                        break;
                
                    case TokenCategory.BREAK:
                        result = stmtBreak();
                        return result;
                        break;
                
                    case TokenCategory.RETURN:
                        result = stmtReturn(); 
                        return result;
                        break;

                    case TokenCategory.SEMICOLON:
                        result = stmtEmpty();
                        return result;
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
            result = expr();
            Expect(TokenCategory.SEMICOLON);
            return result;
        }

        public Node stmtEmpty(){
            var token = Expect(TokenCategory.SEMICOLON);
            var newNode = new semicolon();
            newNode.AnchorToken = token;
            return newNode;
        }

        public Node expr(){
            result = exprOr();
            return result;
        }

        public Node exprOr(){
            result = exprAnd();
            while(Current == TokenCategory.OR || 
                    Current == TokenCategory.XOR){
                switch (Current){
                    case TokenCategory.OR:
                        Expect(TokenCategory.OR);
                        var newNode = new exprOr();
                        newNode.AnchorToken = token;
                        newNode.Add(result);
                        newNode.Add(exprAnd());
                        result =  newNode;
                        break;
                    case TokenCategory.XOR:
                        Expect(TokenCategory.XOR);
                        var newNode = new exprXor();
                        newNode.AnchorToken = token;
                        newNode.Add(result);
                        newNode.Add(exprAnd());
                        result =  newNode;
                        break;
                    default: throw new SyntaxError(Current, tokenStream.Current);
                }
                return result;
            }

        }

        public Node exprAnd(){
            result = exprComp();
            while(Current == TokenCategory.AND){
                var token = Expect(TokenCategory.AND);
                var newNode = new exprAnd();
                newNode.AnchorToken = token;
                newNode.Add(result);
                newNode.Add(exprComp());
                result =  newNode;
            }
            return result;
        }

        public Node exprComp(){
            result = exprRel();
            while(Current == TokenCategory.EQUALS_TO || 
                    Current == TokenCategory.NOT_EQUAL){
                switch(Current){
                    case TokenCategory.EQUALS_TO:
                        var token = Expect(TokenCategory.EQUALS_TO);
                        var newNode = new equals_to();
                        newNode.AnchorToken = token;
                        newNode.Add(result);
                        newNode.Add(exprRel());
                        result =  newNode;
                        break;
                    case TokenCategory.NOT_EQUAL:
                        Expect(TokenCategory.NOT_EQUAL);
                        var newNode = new not_equal();
                        newNode.AnchorToken = token;
                        newNode.Add(result);
                        newNode.Add(exprRel());
                        result =  newNode;
                        break;
                    default: throw new SyntaxError(Current, tokenStream.Current);
                }
                return result;
            }
        }

        public Node exprRel(){
            var result = exprAdd();
            while(Current == TokenCategory.GREATER_THAN ||
                    Current == TokenCategory.GREATER_EQUAL_THAN ||
                    Current == TokenCategory.LESS_THAN || 
                    Current == TokenCategory.LESS_EQUAL_THAN){
                switch(Current){
                    case TokenCategory.GREATER_THAN:
                        var token = Expect(TokenCategory.GREATER_THAN);
                        var newNode = new greater_than();
                        newNode.AnchorToken = token;
                        newNode.Add(result);
                        newNode.Add(exprAdd());
                        result = newNode;
                        break;
                    case TokenCategory.GREATER_EQUAL_THAN:
                        Expect(TokenCategory.GREATER_EQUAL_THAN);
                        var newNode = new greater_equal_than();
                        newNode.AnchorToken = token;
                        newNode.Add(result);
                        newNode.Add(exprAdd());
                        result = newNode;
                        break;
                    case TokenCategory.LESS_THAN:
                        Expect(TokenCategory.LESS_THAN);
                        var newNode = new less_than();
                        newNode.AnchorToken = token;
                        newNode.Add(result);
                        newNode.Add(exprAdd());
                        result = newNode;
                        break;
                    case TokenCategory.LESS_EQUAL_THAN:
                        Expect(TokenCategory.LESS_EQUAL_THAN);
                        var newNode = new less_equal_than();
                        newNode.AnchorToken = token;
                        newNode.Add(result);
                        newNode.Add(exprAdd());
                        result = newNode;
                        break;
                    default: throw new SyntaxError(Current, tokenStream.Current);
                }
                return result;
            }
        }

        public Node exprAdd(){
            var result = exprMul();
            while(Current == TokenCategory.PLUS || Current == TokenCategory.NEG){
                switch(Current){
                    case TokenCategory.PLUS:
                        var token = Expect(TokenCategory.PLUS);
                        var newNode = new plus();
                        newNode.AnchorToken = token;
                        newNode.Add(result);
                        newNode.Add(exprMul());
                        result = newNode;
                        break;
                    case TokenCategory.NEG:
                        var token = Expect(TokenCategory.NEG);
                        var newNode = new neg();
                        newNode.AnchorToken = token;
                        newNode.Add(result);
                        newNode.Add(exprMul());
                        result = newNode;
                        break;
                    default: throw new SyntaxError(Current, tokenStream.Current);
                }
                return result;
            }
        }

        public Node exprMul(){
            var result = exprUnary();
            while(Current == TokenCategory.MUL ||
                    Current == TokenCategory.DIV ||
                    Current == TokenCategory.REMAINDER){
                switch(Current){
                    case TokenCategory.MUL:
                        var token = Expect(TokenCategory.MUL);
                        var newNode = new mul();
                        newNode.AnchorToken = token;
                        newNode.Add(result);
                        newNode.Add(exprUnary());
                        result = newNode;
                        break;
                    case TokenCategory.DIV:
                        var token = Expect(TokenCategory.DIV);
                        var newNode = new div();
                        newNode.AnchorToken = token;
                        newNode.Add(result);
                        newNode.Add(exprUnary);
                        result = newNode;
                        break;
                    case TokenCategory.REMAINDER:
                        var token = Expect(TokenCategory.REMAINDER);
                        var newNode = new remainder();
                        newNode.AnchorToken = token;
                        newNode.Add(result);
                        newNode.Add(exprUnary());
                        result = newNode;
                        break;
                    default: throw new SyntaxError(Current, tokenStream.Current);
                }
                return result;
            }
        }

        public Node exprUnary(){
            var result = exprPrimary();
            while(Current == TokenCategory.PLUS ||
                    Current == TokenCategory.NEG ||
                    Current == TokenCategory.NOT){
                switch(Current){
                    case TokenCategory.PLUS:
                        var token = Expect(TokenCategory.PLUS);
                        var newNode = new plus();
                        newNode.AnchorToken = token;
                        newNode.Add(result);
                        newNode.Add(exprPrimary());
                        result = newNode;
                        break;
                    case TokenCategory.NEG:
                        var token = Expect(TokenCategory.NEG);
                        var newNode = new neg();
                        newNode.AnchorToken = token;
                        newNode.Add(result);
                        newNode.Add(exprPrimary());
                        result = newNode;
                        break;
                    case TokenCategory.NOT:
                        var token = Expect(TokenCategory.NOT);
                        var newNode = new not();
                        newNode.AnchorToken = token;
                        newNode.Add(result);
                        newNode.Add(exprPrimary());
                        result = newNode;
                        break;
                    default: throw new SyntaxError(Current, tokenStream.Current);
                }
                return result;    
            }
        }

        public Node exprPrimary(){
            switch (Current){
                case TokenCategory.IDENTIFIER:
                    Expect(TokenCategory.IDENTIFIER);
                    if(Current == TokenCategory.PARENTHESIS_OPEN){
                        Expect(TokenCategory.PARENTHESIS_OPEN);
                        var result = exprList();
                        Expect(TokenCategory.PARENTHESIS_CLOSE);
                        return result;
                    }
                    break;
                case TokenCategory.BRACKET_LEFT:
                    result = array();
                    return result;
                    break;
                case TokenCategory.TRUE:
                    result = lit();
                    return result;
                    break;
                case TokenCategory.FALSE:
                    result = lit();
                    return result;
                    break;
                case TokenCategory.INT:
                    result = lit();
                    return result;
                    break;
                case TokenCategory.CHARACTER:
                    result = lit();
                    return result;
                    break;
                case TokenCategory.STRING:
                    result = lit();
                    return result;
                    break;

                case TokenCategory.PARENTHESIS_OPEN:
                    Expect(TokenCategory.PARENTHESIS_OPEN);
                    var result = expr();
                    Expect(TokenCategory.PARENTHESIS_CLOSE);
                    return result;
                    break;
                default: throw new SyntaxError(Current, tokenStream.Current);
            }
        }

        public Node array(){
            Expect(TokenCategory.BRACKET_LEFT);
            var result = exprList();
            Expect(TokenCategory.BRACKET_RIGHT);
            return result;
        }

        public Node lit(){
            switch(Current){
                case TokenCategory.TRUE:
                    var token = Expect(TokenCategory.TRUE);
                    var newNode = new TRUE();
                    newNode.AnchorToken = token;
                    return newNode;
                    break;
                case TokenCategory.FALSE:
                    var token = Expect(TokenCategory.FALSE);
                    var newNode = new FALSE();
                    newNode.AnchorToken = token;
                    return newNode;
                    break;
                case TokenCategory.INT:
                    var token = Expect(TokenCategory.INT);
                    var newNode = new INT();
                    newNode.AnchorToken = token;
                    return newNode;
                    break;
                case TokenCategory.CHARACTER:
                    var token = Expect(TokenCategory.CHARACTER);
                    var newNode = new character();
                    newNode.AnchorToken = token;
                    return newNode; 
                    break;
                case TokenCategory.STRING:
                    var token = Expect(TokenCategory.STRING);
                    var newNode = new STRING();
                    newNode.AnchorToken = token;
                    return newNode;
                    break;
                default: throw new SyntaxError(Current, tokenStream.Current);
            }
        }
}
}