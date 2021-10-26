/*
A01745219 Eduardo R. Muller Romero
A01376228 Sebastian Morales Martin
A01746645 Guillermo Adrian Urbina A.
*/

namespace Falak
{
    class Program : Node { }
    class defList : Node { }
    class varDef: Node{ }
    class idList: Node{ }
    class idListCont: Node{ }
    class funDef: Node{ }
    
    class exprFunCall: Node { }

    class funCall: Node { }
    class varDefList: Node{ }
    class stmtList: Node{ }
    class stmtIncr: Node{ }
    class Inc: Node { }
    class Minus: Node { }
    class stmtDecr: Node{ }
    class exprList: Node{ }
    class exprListCont: Node{ }
    class stmtIf: Node{ }
    class elseIfList: Node{ }
    class stmtElse: Node{ }
    class Var: Node { }
    class identifier: Node { }
    class assign: Node { }
    class Plus: Node { }
    class Neg: Node { }
    class Mul: Node { }
    class Div: Node { }
    class Remainder: Node { }
    class Not: Node { }
    class TRUE: Node { }
    class FALSE: Node { }
    class INT: Node { }
    class Character: Node { }
    class STRING: Node { }
    class greater_than: Node { }
    class greater_equal_than: Node { }
    class less_than: Node { }
    class less_equal_than: Node { }
    class equals_to: Node { }
    class not_equal: Node { }
    class semicolon: Node { }
    class stmtWhile: Node{ }
    class stmtDoWhile: Node{ }
    class stmtBreak: Node{ }
    class stmtReturn: Node{ }
    class stmtEmpty: Node{ }
    class expr: Node{ }
    class exprOr: Node{ }

    class Or : Node { }

    class Xor: Node { }
    class exprAnd: Node{ }

    class And : Node { }

    class exprComp: Node{ }
    class exprRel: Node{ }
    class exprAdd: Node{ }
    class exprMul: Node{ }
    class exprUnary: Node{ }
    class exprPrimary: Node{ }
    class Positive: Node { }
    class Negative: Node { }
    
    class array: Node{ }
    class lit: Node{ }

}