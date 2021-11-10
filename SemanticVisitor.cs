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
            };
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
        public void Visit(Program node) {
            Visit((dynamic) node[0]);
            Visit((dynamic) node[1]);
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public void Visit(DeclarationList node) {
            VisitChildren(node);
            return Type.VOID;
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
        public void Visit(StatementList node) {
            VisitChildren(node);
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
        public void Visit(Print node) {
            // Print expects a node of type int or bool, so we can
            // safely ignore the return type of this Visit call.
            Visit((dynamic) node[0]);
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public void Visit(If node) {
            if (Visit((dynamic) node[0]) != Type.BOOL) {
                throw new SemanticError(
                    $"Expecting type {Type.BOOL} in conditional statement",
                    node.AnchorToken);
            }
            VisitChildren(node[1]);
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public void Visit(Identifier node) {

            var variableName = node.AnchorToken.Lexeme;

            if (Table.ContainsKey(variableName)) {
                return Table[variableName];
            }

            throw new SemanticError(
                $"Undeclared variable: {variableName}",
                node.AnchorToken);
        }

        //-----------------------------------------------------------
        public void Visit(IntLiteral node) {

            var intStr = node.AnchorToken.Lexeme;
            int value;

            if (!Int32.TryParse(intStr, out value)) {
                throw new SemanticError(
                    $"Integer literal too large: {intStr}",
                    node.AnchorToken);
            }

            return Type.INT;
        }

        //-----------------------------------------------------------
        public void Visit(True node) {
            return Type.BOOL;
        }

        //-----------------------------------------------------------
        public void Visit(False node) {
            return Type.BOOL;
        }

        //-----------------------------------------------------------
        public void Visit(Neg node) {
            if (Visit((dynamic) node[0]) != Type.INT) {
                throw new SemanticError(
                    $"Operator - requires an operand of type {Type.INT}",
                    node.AnchorToken);
            }
            return Type.INT;
        }

        //-----------------------------------------------------------
        public void Visit(And node) {
            VisitBinaryOperator('&', node, Type.BOOL);
            return Type.BOOL;
        }

        //-----------------------------------------------------------
        public void Visit(Less node) {
            VisitBinaryOperator('<', node, Type.INT);
            return Type.BOOL;
        }

        //-----------------------------------------------------------
        public void Visit(Plus node) {
            VisitBinaryOperator('+', node, Type.INT);
            return Type.INT;
        }

        //-----------------------------------------------------------
        public void Visit(Mul node) {
            VisitBinaryOperator('*', node, Type.INT);
            return Type.INT;
        }

        //-----------------------------------------------------------
        void VisitChildren(Node node) {
            foreach (var n in node) {
                Visit((dynamic) n);
            }
        }

        //-----------------------------------------------------------
        void VisitBinaryOperator(char op, Node node, Type type) {
            if (Visit((dynamic) node[0]) != type ||
                Visit((dynamic) node[1]) != type) {
                throw new SemanticError(
                    $"Operator {op} requires two operands of type {type}",
                    node.AnchorToken);
            }
        }

                //--------------------------Specific Nodes----------------------------
                public void Visit(Program node) {
            Visit((dynamic) node[0]);
            Visit((dynamic) node[1]);
        }

        public void Visit(defList node) {
            VisitChildren(node);
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
            
        }

        public void Visit(varDefList node) {   
            
        }

        public void Visit(stmtList node) {   
            VisitChildren(node);
        }

        public void Visit(stmtIncr node) {   
            
        }

        public void Visit(stmtDecr node) {   
            
        }

        public void Visit(exprList node)
        {
            VisitChildren(node);
        }

        //public void Visit(exprListCont node) { }
        public void Visit(stmtIf node) {   
            
        }
        public void Visit(elseIfList node) {   
            
        }
        public void Visit(stmtElse node) {   
            
        }
        public void Visit(stmtWhile node) {   
            
        }
        public void Visit(stmtDoWhile node) {
            
        }
        public void Visit(stmtBreak node) {   
            
        }
        public void Visit(stmtRetur node) {   
            
        }
        public void Visit(stmtEmpty node) {   
            
        }
        public void Visit(expr node) {   
            
        }
        public void Visit(exprOr node) {   
            
        }
        public void Visit(exprAnd node) {   
            
        }
        public void Visit(exprComp node) {   
            
        }
        public void Visit(exprRel node) {   
            
        }
        public void Visit(exprAdd node) {   
            
        }
        public void Visit(exprMul node) {   
            
        }
        public void Visit(exprUnary node) {   
            
        }
        public void Visit(exprPrimary node) {   
            
        }
        public void Visit(array node) {   
            
        }
        public void Visit(lit node) {   
            
        }
    }

    public struct Primitives()
    {
        // primitives, arity, reference

        private string name;
        private int arity;
        private bool primitive;
        private HashSet<string> reference = null;

        public Primitives(string inputName, bool inputPrimitive, int inputArity, HashSet<string> inputReference)
        {
            name = inputName;
            primitive = inputPrimitive;
            arity = inputArity;
            reference = inputReference;

        }
        
        constructor
    }
}