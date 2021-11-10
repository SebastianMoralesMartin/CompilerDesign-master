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
        public IDictionary<string, Type> Table {
            get;
            private set;
        }

        //-----------------------------------------------------------
        public SemanticVisitor() {
            Table = new SortedDictionary<string, Type>();
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

                //--------------------------NODOS----------------------------
        public void Visit(ADD node) {
            
        }

        public void Visit(AND node) {
            
        }

        public void Visit(ASSIGN node) {   
            
        }

        public void Visit(BACK_SLASH node) {   
            
        }

        public void Visit(BOOL node) {   
            
        }

        public void Visit(BRACKET_LEFT node) {   
            
        }

        public void Visit(BRACKET_RIGHT node) {   
            
        }

        public void Visit(BREAK node) {   
            
        }

        public void Visit(CARRIAGE_RETURN node) {   
            
        }

        public void Visit(CHARACTER node) {   
            
        }

        public void Visit(COMMA node) {   
            
        }

        public void Visit(DEC node) {   
            
        }
        public void Visit(DIV node) {   
            
        }
        public void Visit(DO node) {   
            
        }
        public void Visit(DOUBLE_QUOTE node) {   
            
        }
        public void Visit(DOUBLE_POINTS node) {   
            
        }
        public void Visit(ELSE node) {
            
        }
        public void Visit(ELSEIF node) {   
            
        }
        public void Visit(EQUALS_TO node) {   
            
        }
        public void Visit(END node) {   
            
        }
        public void Visit(EOF node) {   
            
        }
        public void Visit(FALSE node) {   
            
        }
        public void Visit(FUNCTION node) {   
            
        }
        public void Visit(GET node) {   
            
        }
        public void Visit(GREATER_THAN node) {   
            
        }
        public void Visit(GREATER_EQUAL_THAN node) {   
            
        }
        public void Visit(IDENTIFIER node) {   
            
        }
        public void Visit(IF node) {   
            
        }
        public void Visit(INC node) {   
            
        }
        public void Visit(INT node) {   
            
        }
        public void Visit(INT_LITERAL node) {   
            
        }
        public void Visit(KEY_LEFT node) {   
            
        }
        public void Visit(KEY_RIGHT node) {   
            
        }
        public void Visit(LESS_THAN node) {   
            
        }
        public void Visit(LESS_EQUAL_THAN node) {   
            
        }
        public void Visit(MAIN node) {   
            
        }
        public void Visit(MUL node) {   
            
        }
        public void Visit(NEW_LINE node) {   
            
        }
        public void Visit(NEG node) {   
            
        }
        public void Visit(NEW node) {   
            
        }
        public void Visit(NOT node) {
            
        }
        public void Visit(NOT_EQUAL node) {   
            
        }
        public void Visit(OR node) {   
            
        }
        public void Visit(PARENTHESIS_OPEN node) {   
            
        }
        public void Visit(PARENTHESIS_CLOSE node) {   
            
        }
        public void Visit(PLUS node) {   
            
        }
        public void Visit(PRINT node) {   
            
        }
        public void Visit(PRINT_I node) {   
            
        }
        public void Visit(PRINT_C node) {   
            
        }
        public void Visit(PRINT_S node) {   
            
        }
        public void Visit(PRINT_LINE node) {   
            
        }
        public void Visit(READ_I node) {   
            
        }
        public void Visit(READ_S node) {   
            
        }
        public void Visit(REMINDER node) {   
            
        }
        public void Visit(RETURN node) {   
            
        }
        public void Visit(SEMICOLON node) {   
            
        }
        public void Visit(SET node) {   
            
        }
        public void Visit(SINGLE_QUOTE node) {   
            
        }
        public void Visit(SIZE node) {   
            
        }
        public void Visit(STRING node) {   
            
        }
        public void Visit(TAB node) {   
            
        }
        public void Visit(THEN node) {   
            
        }
        public void Visit(TRUE node) {   
            
        }
        public void Visit(UNARY_PLUS node) {   
            
        }
        public void Visit(UNARY_MINUS node) {   
            
        }
        public void Visit(VAR node) {   
            
        }
        public void Visit(WHILE node) {
            
        }
        public void Visit(WHITE_SPACE node) {   
            
        }
        public void Visit(XOR node) {   
            
        }
        public void Visit(ILLEGAL_CHAR node) {   
            
        }
    }
}