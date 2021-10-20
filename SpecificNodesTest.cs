/*
A01745219 Eduardo R. Muller Romero
A01376228 Sebastian Morales Martin
A01746645 Guillermo Adrian Urbina A.
*/

namespace Falak
{
    public class SpecificNodesTest
    {
        public class Program : Node { }

        public class defList : Node { }
        public class varDef: Node{ }
        public class idList: Node{ }
        public class idListCont: Node{ }
        public class funDef: Node{ }
        public class varDefList: Node{ }
        public class stmtList: Node{ }
        public class stmtIncr: Node{ }
        public class stmtDecr: Node{ }
        public class exprList: Node{ }
        public class exprListCont: Node{ }
        public class stmtIf: Node{ }
        public class elseIfList: Node{ }
        public class stmtElse: Node{ }
        public class var: Node { }
        public class identifier: Node { }
        public class assign: Node { }
        public class plus: Node { }
        public class neg: Node { }
        public class mul: Node { }
        public class div: Node { }
        public class remainder: Node { }
        public class not: Node { }
        public class TRUE: Node { }
        public class FALSE: Node { }
        public class INT: Node { }
        public class character: Node { }
        public class STRING: Node { }
        public class greater_than: Node { }
        public class greater_equal_than: Node { }
        public class less_than: Node { }
        public class less_equal_than: Node { }
        public class equals_to: Node { }
        public class not_equal: Node { }
        public class semicolon: Node { }
        public class stmtWhile: Node{ }
        public class stmtDoWhile: Node{ }
        public class stmtBreak: Node{ }
        public class stmtReturn: Node{ }
        public class stmtEmpty: Node{ }
        public class expr: Node{ }
        public class exprOr: Node{ }

        public class or : Node { }

        public class xor: Node { }
        public class exprAnd: Node{ }

        public class and : Node { }

        public class exprComp: Node{ }
        public class exprRel: Node{ }
        public class exprAdd: Node{ }
        public class exprMul: Node{ }
        public class exprUnary: Node{ }
        public class exprPrimary: Node{ }
        public class array: Node{ }
        public class lit: Node{ }
    }
}