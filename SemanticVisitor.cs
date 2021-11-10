/*
    A01745219 Eduardo R. Muller Romero
    A01376228 Sebastian Morales Martin
    A01746645 Guillermo Adrian Urbina A.
*/

using System;
using System.Collections.Generic;

namespace Falak
{
    public class SemanticVisitor {
        static readonly IDictionary<TokenCategory, Type> typeMapper =
            new Dictionary<TokenCategory, Type>() {
                { TokenCategory.BOOL, Type.BOOL },
                { TokenCategory.INT, Type.INT }
            };//Se omite
        //-----------------------------------------------------------
        public IDictionary<string, string> Table {
            get;
            private set;
        }

        //-----------------------------------------------------------
        public SemanticVisitor() {
            Table = new SortedDictionary<string, string>();
        }

        //-----------------------------------------------------------
        public void Visit(Declaration node) {

            var variableName = node[0].AnchorToken.Lexeme;

            if (Table.ContainsKey(variableName)) {
                throw new SemanticError(
                    "Duplicated variable: " + variableName,
                    node[0].AnchorToken);

            } else {
                Table[variableName] =
                    typeMapper[node.AnchorToken.Category];
            }

            return Type.VOID;
        }

        //-----------------------------------------------------------
        public void Visit(Assignment node) {

            var variableName = node.AnchorToken.Lexeme;

            if (Table.ContainsKey(variableName)) {

                var expectedType = Table[variableName];

                if (expectedType != Visit((dynamic) node[0])) {
                    throw new SemanticError(
                        "Expecting type " + expectedType
                        + " in assignment statement",
                        node.AnchorToken);
                }

            } else {
                throw new SemanticError(
                    "Undeclared variable: " + variableName,
                    node.AnchorToken);
            }

            return Type.VOID;
        }

        //-----------------------------------------------------------
        public void Visit(identifier node) {

            var variableName = node.AnchorToken.Lexeme;

            if (Table.ContainsKey(variableName)) {
                return Table[variableName];
            }

            throw new SemanticError(
                $"Undeclared variable: {variableName}",
                node.AnchorToken);
        }

        //-----------------------------------------------------------
        public void Visit(INT node) {

            var intStr = node.AnchorToken.Lexeme;
            int value;

            if (!Int32.TryParse(intStr, out value)) {
                throw new SemanticError(
                    $"Integer literal too large: {intStr}",
                    node.AnchorToken);
            }

        }

        //-----------------------------------------------------------
        void VisitChildren(Node node) {
            foreach (var n in node) {
                Visit((dynamic) n);
            }
        }

        //-----------------------------------------------------------
        void VisitBinaryOperator(string op, Node node, TokenCategory type) {
            if (Visit((dynamic) node[0]) != type ||
                Visit((dynamic) node[1]) != type) {
                throw new SemanticError(
                    $"Operator {op} requires two operands of type {type}",
                    node.AnchorToken);
            }
        }


//----------------------------------------------------------------------------------------------------------------
        //--------------------------Falak Specific Nodes----------------------------
//----------------------------------------------------------------------------------------------------------------




        public void Visit(Program node) {
            Visit((dynamic) node[0]);
            Visit((dynamic) node[1]);
            return Type.VOID;
        }

        public void Visit(defList node) {
            VisitChildren(node);
            return Type.VOID;
        }

        public void Visit(varDef node) {   
            var variableName = node[0].AnchorToken.Lexeme;

            if (Table.ContainsKey(variableName)) {
                throw new SemanticError(
                    "Duplicated variable: " + variableName,
                    node[0].AnchorToken);

            } else {
                Table[variableName] = node[0].AnchorToken;
            }
        }

        public void Visit(idList node) {   
            VisitChildren(node);
        }

        //public void Visit(idListCont node) { }

        public void Visit(funDef node) {   
            var variableName = node[0].AnchorToken.Lexeme;

            if (Table.ContainsKey(variableName)) {
                throw new SemanticError(
                    "Duplicated function: " + variableName,
                    node[0].AnchorToken);

            } else {
                Table[variableName] = node[0].AnchorToken;
            }
        }

        public void Visit(varDefList node)
        {
            VisitChildren(node);
        }

        public void Visit(stmtList node) 
        {   
            VisitChildren(node);
            return Type.VOID;
        }

        public void Visit(stmtIncr node) 
        {   
            VisitChildren(node);
            return Type.VOID;
        }

        public void Visit(stmtDecr node) 
        {   
            VisitChildren(node);
            return Type.VOID;
        }

        public void Visit(exprList node)
        {
            VisitChildren(node);
        }

        //public void Visit(exprListCont node) { }
        
        public void Visit(stmtIf node) 
        {   
            if (Visit((dynamic) node[0]) != Type.BOOL) {
                throw new SemanticError(
                    $"Expecting type {Type.BOOL} in conditional statement",
                    node.AnchorToken);
            }
            VisitChildren(node[1]);
            return Type.VOID;
        }
        public void Visit(elseIfList node)
        {
            if (Visit((dynamic) node[0]) != Type.BOOL) {
                throw new SemanticError(
                    $"Expecting type {Type.BOOL} in conditional statement",
                    node.AnchorToken);
            }
            VisitChildren(node[1]);
        }
        public void Visit(stmtElse node) 
        {   
            if (Visit((dynamic) node[0]) != stmtList) {
                throw new SemanticError(
                    $"Expecting type {stmtList} in statement",
                    node.AnchorToken);
            }
            VisitChildren(node[1]);
        }
        public void Visit(stmtWhile node) 
        {   
            if (Visit((dynamic) node[0]) != Type.BOOL) {
                throw new SemanticError(
                    $"Expecting type {Type.BOOL} in conditional statement",
                    node.AnchorToken);
            }
            VisitChildren(node[1]);
        }
        public void Visit(stmtDoWhile node) 
        {
            if (Visit((dynamic) node[0]) != stmtList) {
                throw new SemanticError(
                    $"Expecting type {stmtList} in statement",
                    node.AnchorToken);
            }
            VisitChildren(node[1]);
        }
        public void Visit(stmtBreak node) 
        {   
            VisitChildren(node);
        }
        public void Visit(stmtReturn node) 
        {   
            if (Visit((dynamic) node[0]) != expr) {
                throw new SemanticError(
                    $"Expecting type {expr} in statement",
                    node.AnchorToken);
            }
            VisitChildren(node[1]);
        }
        public void Visit(stmtEmpty node) 
        {   
            VisitChildren(node);
        }
        public void Visit(expr node) 
        {   
            if (Visit((dynamic) node[0]) != exprAdd | exprAnd | exprComp | exprRel | exprMul | exprUnary) {
                throw new SemanticError(
                    $"Expecting type {expr} in conditional statement",
                    node.AnchorToken);
            }
            VisitChildren(node[1]);
        }
        public void Visit(exprOr node) 
        {   
            if (Visit((dynamic) node[0]) != Type.BOOL) {
                throw new SemanticError(
                    $"Expecting type {Type.BOOL} in conditional statement",
                    node.AnchorToken);
            }
            VisitChildren(node[1]);
        }
        public void Visit(exprAnd node) 
        {   
            if (Visit((dynamic) node[0]) != Type.BOOL) {
                throw new SemanticError(
                    $"Expecting type {Type.BOOL} in conditional statement",
                    node.AnchorToken);
            }
            VisitChildren(node[1]);
        }
        public void Visit(exprComp node) 
        {   
            if (Visit((dynamic) node[0]) != Type.BOOL) {
                throw new SemanticError(
                    $"Expecting type {Type.BOOL} in conditional statement",
                    node.AnchorToken);
            }
            VisitChildren(node[1]);
        }
        public void Visit(exprRel node) 
        {   
            if (Visit((dynamic) node[0]) != Type.BOOL) {
                throw new SemanticError(
                    $"Expecting type {Type.BOOL} in conditional statement",
                    node.AnchorToken);
            }
            VisitChildren(node[1]);
        }
        public void Visit(exprAdd node) 
        {   
            if (Visit((dynamic) node[0]) != Type.BOOL) {
                throw new SemanticError(
                    $"Expecting type {Type.BOOL} in conditional statement",
                    node.AnchorToken);
            }
            VisitChildren(node[1]);
        }
        public void Visit(exprMul node) 
        {   
            if (Visit((dynamic) node[0]) != Type.BOOL) {
                throw new SemanticError(
                    $"Expecting type {Type.BOOL} in conditional statement",
                    node.AnchorToken);
            }
            VisitChildren(node[1]);
        }
        public void Visit(exprUnary node) 
        {   
            if (Visit((dynamic) node[0]) != Type.INT) {
                throw new SemanticError(
                    $"Expecting type {Type.INT} in conditional statement",
                    node.AnchorToken);
            }
            VisitChildren(node[1]);
        }
        public void Visit(exprPrimary node) 
        {   
            if (Visit((dynamic) node[0]) != Type.BOOL) {
                throw new SemanticError(
                    $"Expecting type {Type.BOOL} in conditional statement",
                    node.AnchorToken);
            }
            VisitChildren(node[1]);
        }
        public void Visit(array node) 
        {   
            VisitChildren(node);
        }
        public void Visit(lit node) 
        {   
            VisitChildren(node);
        }

        public void Visit(exprFunCall node)
        {
            
        }
        public void Visit(funCall node)
        {
            
        }
        public void Visit(assign node)
        {
            var variableName = node.AnchorToken.Lexeme;

            if (Table.ContainsKey(variableName)) {

                var expectedType = Table[variableName];

                if (expectedType != Visit((dynamic) node[0])) {
                    throw new SemanticError(
                        "Expecting type " + expectedType
                        + " in assignment statement",
                        node.AnchorToken);
                }

            } else {
                throw new SemanticError(
                    "Undeclared variable: " + variableName,
                    node.AnchorToken);
            }

        }
        
        public void Visit(Plus node)
        {
            if (Visit((dynamic) node[0]) != Type.INT) {
                throw new SemanticError(
                    $"Operator - requires an operand of type {Type.INT}",
                    node.AnchorToken);
            }
        }        
        public void Visit(Neg node)
        {
            if (Visit((dynamic) node[0]) != Type.INT) {
                throw new SemanticError(
                    $"Operator - requires an operand of type {Type.INT}",
                    node.AnchorToken);
            }
        }
        }       
        public void Visit(Div node)
        {
            VisitBinaryOperator('/', node, Type.INT);
        }        
        public void Visit(Inc node) {
            VisitBinaryOperator('+', node, Type.INT);
        }
        public void Visit(Minus node) {
            VisitBinaryOperator('-', node, Type.INT);
        }
        public void Visit(Remainder node)
        {
            VisitBinaryOperator('%', node, Type.INT);
        }        
        public void Visit(Not node)
        {
            VisitBinaryOperator('!', node, Type.BOOL);
        }        
        public void Visit(True node)
        {
            //return Type.BOOL;
        }        
        public void Visit(False node)
        {
            //return Type.BOOL;
        }
        public void Visit(greater_than node)
        {
            VisitBinaryOperator('>', node, Type.BOOL);
            //return Type.BOOL;
        }
        public void Visit(greater_equal_than node)
        {
            VisitBinaryOperator('>' + '=', node, Type.BOOL);
            //return Type.BOOL;
        }
        public void Visit(less_than node)
        {
            VisitBinaryOperator('<', node, Type.BOOL);
            //return Type.BOOL;
        }        
        public void Visit(less_equal_than node)
        {
            VisitBinaryOperator('<' + '=', node, Type.BOOL);
            //return Type.BOOL;
        }        
        public void Visit(equals_to node)
        {
            VisitBinaryOperator('=' + '=', node, Type.BOOL);
            //return Type.BOOL;
        }        
        public void Visit(not_equal node)
        {
            VisitBinaryOperator('!' + '=', node, Type.BOOL);
            //return Type.BOOL;
        }        
        public void Visit(Positive node)
        {
            VisitBinaryOperator('+', node, Type.BOOL);
            //return Type.BOOL;
        }
        public void Visit(Negative node)
        {
            VisitBinaryOperator('-', node, Type.BOOL);
            //return Type.BOOL;
        }
        public void Visit(Or node)
        {
            VisitBinaryOperator('||', node, Type.BOOL);
            //return Type.BOOL;
        }
		
        public void Visit(Xor node)
        {
            VisitBinaryOperator('^', node, Type.BOOL);
            //return Type.BOOL;
        }
        public void Visit(And node)
        {
            VisitBinaryOperator("&&", node, Type.BOOL);
            //return Type.BOOL;
        }
        //API primitives
        public void Visit(printI node)
        {
            Visit((dynamic) node[0]);
            //return Type.VOID;
        }
        public void Visit(printC node)
        {
            Visit((dynamic) node[0]);
            //return Type.VOID;
        }
        public void Visit(printS node)
        {
            Visit((dynamic) node[0]);
            //return Type.VOID;
        }
        public void Visit(printLN node)
        {
            Visit((dynamic) node[0]);
            //return Type.VOID;
        }
        public void Visit(readI node)
        {
            Visit((dynamic) node[0]);
            //return Type.VOID;
        }
        public void Visit(readS node)
        {
            Visit((dynamic) node[0]);
            //return Type.VOID;
        }
        public void Visit(New node)
        {

        }
        public void Visit(Size node)
        {

        }
        public void Visit(Add node)
        {

        }
        public void Visit(Get node)
        {

        }        
        public void Visit(Set node)
        {

        }
    }

    public class FunCollection
        {
        
        // primitives, arity, reference
        private string name;
        private int arity;
        private bool primitive;
        private HashSet<string> reference = null;
        public FunCollection(string inputName, bool inputPrimitive, int inputArity, HashSet<string> inputReference)
        {
            name = inputName;
            primitive = inputPrimitive;
            arity = inputArity;
            reference = inputReference;
        }
        constructor
    }
}