namespace Falak
{
    public class SemanticVisitor
    {
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
            return Type.ADD;
        }

        public void Visit(AND node) {
            return Type.AND;
        }

        public void Visit(ASSIGN node) {   
            return Type.ASSIGN;
        }

        public void Visit(BACK_SLASH node) {   
            return Type.BACK_SLASH;
        }

        public void Visit(BOOL node) {   
            return Type.BOOL;
        }

        public void Visit(BRACKET_LEFT node) {   
            return Type.BRACKET_LEFT;
        }

        public void Visit(BRACKET_RIGHT node) {   
            return Type.BRACKET_RIGHT;
        }

        public void Visit(BREAK node) {   
            return Type.BREAR;
        }

        public void Visit(CARRIAGE_RETURN node) {   
            return Type.CARRIAGE_RETURN;
        }

        public void Visit(CHARACTER node) {   
            return Type.CHARACTER;
        }

        public void Visit(COMMA node) {   
            return Type.COMMA;
        }

        public void Visit(DEC node) {   
            return Type.DEC;
        }
        public void Visit(DIV node) {   
            return Type.DIV;
        }
        public void Visit(DO node) {   
            return Type.DO;
        }
        public void Visit(DOUBLE_QUOTE node) {   
            return Type.DOUBLE_QUOTE;
        }
        public void Visit(DOUBLE_POINTS node) {   
            return Type.DOUBLE_POINTS;
        }
        public void Visit(ELSE node) {   
            return Type.ELSE;
        }
        public void Visit(ELSEIF node) {   
            return Type.ELSEIF;
        }
        public void Visit(EQUALS_TO node) {   
            return Type.EQUALS_TO;
        }
        public void Visit(END node) {   
            return Type.END;
        }
        public void Visit(EOF node) {   
            return Type.EOF;
        }
        public void Visit(FALSE node) {   
            return Type.FALSE;
        }
        public void Visit(FUNCTION node) {   
            return Type.FUNCTION;
        }
        public void Visit(GET node) {   
            return Type.GET;
        }
        public void Visit(GREATER_THAN node) {   
            return Type.GREATER_THAN;
        }
        public void Visit(GREATER_EQUAL_THAN node) {   
            return Type.GREATER_EQUAL_THAN;
        }
        public void Visit(IDENTIFIER node) {   
            return Type.IDENTIFIER;
        }
        public void Visit(IF node) {   
            return Type.IF;
        }
        public void Visit(INC node) {   
            return Type.INC;
        }
        public void Visit(INT node) {   
            return Type.INT;
        }
        public void Visit(INT_LITERAL node) {   
            return Type.INT_LITERAL;
        }
        public void Visit(KEY_LEFT node) {   
            return Type.KEY_LEFT;
        }
        public void Visit(KEY_RIGHT node) {   
            return Type.KEY_RIGHT;
        }
        public void Visit(LESS_THAN node) {   
            return Type.LESS_THAN;
        }
        public void Visit(LESS_EQUAL_THAN node) {   
            return Type.LESS_EQUAL_THAN;
        }
        public void Visit(MAIN node) {   
            return Type.MAIN;
        }
        public void Visit(MUL node) {   
            return Type.MUL;
        }
        public void Visit(NEW_LINE node) {   
            return Type.NEW_LINE;
        }
        public void Visit(NEG node) {   
            return Type.NEG;
        }
        public void Visit(NEW node) {   
            return Type.NEW;
        }
        public void Visit(NOT node) {   
            return Type.NOT;
        }
        public void Visit(NOT_EQUAL node) {   
            return Type.NOT_EQUAL;
        }
        public void Visit(OR node) {   
            return Type.OR;
        }
        public void Visit(PARENTHESIS_OPEN node) {   
            return Type.PARENTHESIS_OPEN;
        }
        public void Visit(PARENTHESIS_CLOSE node) {   
            return Type.PARENTHESIS_CLOSE;
        }
        public void Visit(PLUS node) {   
            return Type.PLUS;
        }
        public void Visit(PRINT node) {   
            return Type.PRINT;
        }
        public void Visit(PRINT_I node) {   
            return Type.PRINT_I;
        }
        public void Visit(PRINT_C node) {   
            return Type.PRINC_C;
        }
        public void Visit(PRINT_S node) {   
            return Type.PRINT_S;
        }
        public void Visit(PRINT_LINE node) {   
            return Type.PRINT_LINE;
        }
        public void Visit(READ_I node) {   
            return Type.READ_I;
        }
        public void Visit(READ_S node) {   
            return Type.READ_S;
        }
        public void Visit(REMINDER node) {   
            return Type.REMINDER;
        }
        public void Visit(RETURN node) {   
            return Type.RETURN;
        }
        public void Visit(SEMICOLON node) {   
            return Type.SEMICOLON;
        }
        public void Visit(SET node) {   
            return Type.SET;
        }
        public void Visit(SINGLE_QUOTE node) {   
            return Type.SINGLE_QUOTE;
        }
        public void Visit(SIZE node) {   
            return Type.SIZE;
        }
        public void Visit(STRING node) {   
            return Type.STRING;
        }
        public void Visit(TAB node) {   
            return Type.TAB;
        }
        public void Visit(THEN node) {   
            return Type.THEN;
        }
        public void Visit(TRUE node) {   
            return Type.TRUE;
        }
        public void Visit(UNARY_PLUS node) {   
            return Type.UNARY_PLUS;
        }
        public void Visit(UNARY_MINUS node) {   
            return Type.UNARY_MINUS;
        }
        public void Visit(VAR node) {   
            return Type.VAR;
        }
        public void Visit(WHILE node) {   
            return Type.WHILE;
        }
        public void Visit(WHITE_SPACE node) {   
            return Type.WHITE_SPACE;
        }
        public void Visit(XOR node) {   
            return Type.XOR;
        }
        public void Visit(ILLEGAL_CHAR node) {   
            return Type.ILLEGAL_CHAR;
        }
    }
}