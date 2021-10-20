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
            var result = 0;
            var newNode = defList();
            switch (Current){
                case TokenCategory.VAR: 
                    result = varDef();
                    newNode.Add(result);
                    return newNode;
                    break;
                
                case TokenCategory.IDENTIFIER: 
                    result = funDef();
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

        public Node idList()
        {
            var newNode = new idList();
            if (Current == TokenCategory.IDENTIFIER)
            {
                Expect(TokenCategory.IDENTIFIER);
                var result = idListCont();
                newNode.Add(result);
            }

            return newNode;
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
            var result = varDefList();
            newNode.Add(result);
            var result = stmtList();
            newNode.Add(result);
            Expect(TokenCategory.KEY_RIGHT);
            return newNode;
        }

        public Node varDefList()
        {
            while (Current == TokenCategory.VAR)
            {
                result = varDef();
                return result;
            }
        }

        public Node stmtList()
        {
            var newNode = new stmtList();
            while (Current != TokenCategory.KEY_RIGHT)
            {
                switch (Current){
                    case TokenCategory.IDENTIFIER:
                        Expect(TokenCategory.IDENTIFIER);
                        switch (Current){
                            case TokenCategory.ASSIGN:
                                Expect(TokenCategory.ASSIGN);
                                var result = expr();
                                newNode.Add(result);
                                Expect(TokenCategory.SEMICOLON);
                                return newNode;
                            case TokenCategory.PARENTHESIS_OPEN:
                                Expect(TokenCategory.PARENTHESIS_OPEN);
                                var result = exprList();
                                newNode.Add(result);
                                Expect(TokenCategory.PARENTHESIS_CLOSE);
                                Expect(TokenCategory.SEMICOLON);
                                return newNode;
                            default: throw new SyntaxError(Current, tokenStream.Current);
                        }
                        break;


                    case TokenCategory.INC:
                        var result = stmtIncr();
                        newNode.Add(result);
                        return newNode;


                    case TokenCategory.DEC:
                        var result = stmtDecr();
                        newNode.Add(result);
                        return newNode;

                    case TokenCategory.IF:
                        var result = stmtIf();
                        newNode.Add(result);
                        return newNode;
                    case TokenCategory.WHILE: 
                        var result = stmtWhile();
                        newNode.Add(result);
                        return newNode;

                    case TokenCategory.DO:
                        var result = stmtDoWhile();
                        newNode.Add(result);
                        return newNode;
                
                    case TokenCategory.BREAK:
                        var result = stmtBreak();
                        newNode.Add(result);
                        return newNode;
                
                    case TokenCategory.RETURN:
                        var result = stmtReturn();
                        newNode.Add(result);
                        return newNode;

                    case TokenCategory.SEMICOLON:
                        var result = stmtEmpty();
                        newNode.Add(result);
                        return newNode;

                    default: throw new SyntaxError(Current, tokenStream.Current);
                }
            }
        }
// --------------------------------------------------------
        public Node stmtIncr(){
            var result = new stmtIncr()
            {
                AnchorToken = Expect(TokenCategory.INC);
            }
            result.Add(new identifier()
            {
                AnchorToken = Expect(TokenCategory.IDENTIFIER);
            })        
            
            Expect(TokenCategory.SEMICOLON);
        }

        public Node stmtDecr(){
            var result = stmtDecr()
            {
                AnchorToken = Expect(TokenCategory.DEC);
            }
            result.Add(new identifier()
            {
                AnchorToken = Expect(TokenCategory.IDENTIFIER);
            })
            
            Expect(TokenCategory.SEMICOLON);
        }

        public Node exprList()
        {
            var newNode = new exprList();
            var result = expr();
            newNode.Add(result);
            var result = exprListCont();
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
            var result = new stmtIf()
            {
                AnchorToken = Expect(TokenCategory.IF);
            }
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
            while(Current == TokenCategory.ELSEIF){
                var result = new elseIfList()
                {
                    AnchorToken = Expect(TokenCategory.ELSEIF);
                }
                
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
            if (Current == TokenCategory.ELSE)
            {
                var result = new stmtElse()
                {
                    AnchorToken = Expect(TokenCategory.ELSE);
                }
                
                Expect(TokenCategory.KEY_LEFT);
                result.Add(stmtList());
                Expect(TokenCategory.KEY_RIGHT);  
            }
            
        }

        public Node stmtWhile(){
            var result = new stmtWhile()
            {
                AnchorToken = Expect(TokenCategory.WHILE);
            }
            
            Expect(TokenCategory.PARENTHESIS_OPEN);
            result.Add(expr());
            
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.KEY_LEFT);
            result.Add(stmtList());
            Expect(TokenCategory.KEY_RIGHT);
            return result;
        }

        public Node stmtDoWhile(){
            var result = new stmtDoWhile()
            {
                AnchorToken = Expect(TokenCategory.DO);
            }
            
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
                AnchorToken = Expect(TokenCategory.BREAK);
            }
            Expect(TokenCategory.SEMICOLON);
            return result;
        }

        public Node stmtReturn(){
            var result = new stmtReturn
            {
                AnchorToken = Expect(TokenCategory.RETURN);
            }
            result.Add(expr());
            Expect(TokenCategory.SEMICOLON);
            return result;
        }

        public Node stmtEmpty(){
            var result = new stmtEmpty()
            {
                AnchorToken = Expect(TokenCategory.SEMICOLON);
            }
            return result;
        }

        public Node expr(){
            result = exprOr();
            return result;
        }

        public Node exprOr()
        {
            var newNode = null;
            result = exprAnd();
            while(Current == TokenCategory.OR || 
                    Current == TokenCategory.XOR){
                switch (Current){
                    case TokenCategory.OR:
                        AnchorToken = Expect(TokenCategory.OR);
                        newNode = new exprOr();
                        newNode.AnchorToken = token;
                        newNode.Add(result);
                        newNode.Add(exprAnd());
                        result =  newNode;
                        break;
                    case TokenCategory.XOR:
                        Expect(TokenCategory.XOR);
                        newNode = new exprXor();
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
        public Node exprComp()
        {
            var token = null;
            var NewNode = null;
            result = exprRel();
            while(Current == TokenCategory.EQUALS_TO || 
                    Current == TokenCategory.NOT_EQUAL){
                switch(Current){
                    case TokenCategory.EQUALS_TO:
                        token = Expect(TokenCategory.EQUALS_TO);
                        newNode = new equals_to();
                        newNode.AnchorToken = token;
                        newNode.Add(result);
                        newNode.Add(exprRel());
                        result =  newNode;
                        break;
                    case TokenCategory.NOT_EQUAL:
                        token = Expect(TokenCategory.NOT_EQUAL);
                        newNode = new not_equal();
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

        public Node exprRel()
        {
            var newNode = null;
            var token = null;
            var result = exprAdd();
            while(Current == TokenCategory.GREATER_THAN ||
                    Current == TokenCategory.GREATER_EQUAL_THAN ||
                    Current == TokenCategory.LESS_THAN || 
                    Current == TokenCategory.LESS_EQUAL_THAN){
                switch(Current){
                    case TokenCategory.GREATER_THAN:
                        token = Expect(TokenCategory.GREATER_THAN);
                        newNode = new greater_than();
                        newNode.AnchorToken = token;
                        newNode.Add(result);
                        newNode.Add(exprAdd());
                        result = newNode;
                        break;
                    case TokenCategory.GREATER_EQUAL_THAN:
                        token = Expect(TokenCategory.GREATER_EQUAL_THAN);
                        newNode = new greater_equal_than();
                        newNode.AnchorToken = token;
                        newNode.Add(result);
                        newNode.Add(exprAdd());
                        result = newNode;
                        break;
                    case TokenCategory.LESS_THAN:
                        token = Expect(TokenCategory.LESS_THAN);
                        newNode = new less_than();
                        newNode.AnchorToken = token;
                        newNode.Add(result);
                        newNode.Add(exprAdd());
                        result = newNode;
                        break;
                    case TokenCategory.LESS_EQUAL_THAN:
                        token = Expect(TokenCategory.LESS_EQUAL_THAN);
                        newNode = new less_equal_than();
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

        public Node exprAdd()
        {
            var NewNode, token = null;
            var result = exprMul();
            while(Current == TokenCategory.PLUS || Current == TokenCategory.NEG){
                switch(Current){
                    case TokenCategory.PLUS:
                        token = Expect(TokenCategory.PLUS);
                        newNode = new plus();
                        newNode.AnchorToken = token;
                        newNode.Add(result);
                        newNode.Add(exprMul());
                        result = newNode;
                        break;
                    case TokenCategory.NEG:
                        token = Expect(TokenCategory.NEG);
                        newNode = new neg();
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
            var NewNode, token = null;
            var result = exprUnary();
            while(Current == TokenCategory.MUL ||
                    Current == TokenCategory.DIV ||
                    Current == TokenCategory.REMAINDER){
                switch(Current){
                    case TokenCategory.MUL:
                        token = Expect(TokenCategory.MUL);
                        newNode = new mul();
                        newNode.AnchorToken = token;
                        newNode.Add(result);
                        newNode.Add(exprUnary());
                        result = newNode;
                        break;
                    case TokenCategory.DIV:
                        token = Expect(TokenCategory.DIV);
                        newNode = new div();
                        newNode.AnchorToken = token;
                        newNode.Add(result);
                        newNode.Add(exprUnary);
                        result = newNode;
                        break;
                    case TokenCategory.REMAINDER:
                        token = Expect(TokenCategory.REMAINDER);
                        newNode = new remainder();
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
            var NewNode, token = null;
            var result = exprPrimary();
            while(Current == TokenCategory.PLUS ||
                    Current == TokenCategory.NEG ||
                    Current == TokenCategory.NOT){
                switch(Current){
                    case TokenCategory.PLUS:
                        token = Expect(TokenCategory.PLUS);
                        newNode = new plus();
                        newNode.AnchorToken = token;
                        newNode.Add(result);
                        newNode.Add(exprPrimary());
                        result = newNode;
                        break;
                    case TokenCategory.NEG:
                        token = Expect(TokenCategory.NEG);
                        newNode = new neg();
                        newNode.AnchorToken = token;
                        newNode.Add(result);
                        newNode.Add(exprPrimary());
                        result = newNode;
                        break;
                    case TokenCategory.NOT:
                        token = Expect(TokenCategory.NOT);
                        newNode = new not();
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
            var NewNode, token, result = null;
            switch (Current){
                case TokenCategory.IDENTIFIER:
                    Expect(TokenCategory.IDENTIFIER);
                    if(Current == TokenCategory.PARENTHESIS_OPEN){
                        Expect(TokenCategory.PARENTHESIS_OPEN);
                        result = exprList();
                        Expect(TokenCategory.PARENTHESIS_CLOSE);
                        return result;
                    }
                    break;
                case TokenCategory.BRACKET_LEFT:
                    result = array();
                    return result;
                case TokenCategory.TRUE:
                    result = lit();
                    return result;
                case TokenCategory.FALSE:
                    result = lit();
                    return result;
                case TokenCategory.INT:
                    result = lit();
                    return result;
                case TokenCategory.CHARACTER:
                    result = lit();
                    return result;
                case TokenCategory.STRING:
                    result = lit();
                    return result;
                case TokenCategory.PARENTHESIS_OPEN:
                    Expect(TokenCategory.PARENTHESIS_OPEN);
                    result = expr();
                    Expect(TokenCategory.PARENTHESIS_CLOSE);
                    return result;
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
            var NewNode, token = null;
            switch(Current){
                case TokenCategory.TRUE:
                    token = Expect(TokenCategory.TRUE);
                    newNode = new TRUE();
                    newNode.AnchorToken = token;
                    return newNode;
                case TokenCategory.FALSE:
                    token = Expect(TokenCategory.FALSE);
                    newNode = new FALSE();
                    newNode.AnchorToken = token;
                    return newNode;
                case TokenCategory.INT:
                    token = Expect(TokenCategory.INT);
                    newNode = new INT();
                    newNode.AnchorToken = token;
                    return newNode;
                case TokenCategory.CHARACTER:
                    token = Expect(TokenCategory.CHARACTER);
                    newNode = new character();
                    newNode.AnchorToken = token;
                    return newNode;
                case TokenCategory.STRING:
                    token = Expect(TokenCategory.STRING);
                    newNode = new STRING();
                    newNode.AnchorToken = token;
                    return newNode;
                default: throw new SyntaxError(Current, tokenStream.Current);
            }
        }
}
}