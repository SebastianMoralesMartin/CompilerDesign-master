/*
A01745219 Eduardo R. Muller Romero
A01376228 Sebastian Morales Martin
A01746645 Guillermo Adrian Urbina A.
*/

namespace Falak {

    public enum TokenCategory {
        ADD,
        AND,
        ASSIGN,
        BACK_SLASH,
        BOOL,
        BRACKET_LEFT,
        BRACKET_RIGHT,
        BREAK,
        CARRIAGE_RETURN,
        CHARACTER,
        COMMA,
        //COMMENT,              Used only for debugging Purposes, now deprecated
        DEC,
        DIV,
        DO,
        DOUBLE_QUOTE,
        DOUBLE_POINTS,
        ELSE,
        ELSEIF,
        EQUALS_TO,
        END,
        EOF,
        FALSE,
        FUNCTION,
        GET,
        GREATER_THAN,
        GREATER_EQUAL_THAN,
        IDENTIFIER,
        IF,
        INC,
        INT,
        INT_LITERAL,
        KEY_LEFT,
        KEY_RIGHT,
        LESS_THAN,
        LESS_EQUAL_THAN,
        MAIN,
        MUL,
        //MULTI_LINE_COMMENT,   used only for debugging purposes, now deprecated
        NEW_LINE,
        NEG,
        NEW,
        NOT,
        NOT_EQUAL,
        OR,
        PARENTHESIS_OPEN,
        PARENTHESIS_CLOSE,
        PLUS,
        PRINT,
        PRINT_I,
        PRINT_C,
        PRINT_S,
        PRINT_LINE,
        READ_I,
        READ_S,
        REMAINDER,
        RETURN,
        SEMICOLON,
        SET,
        SINGLE_QUOTE,
        SIZE,
        STRING,
        TAB,
        THEN,
        TRUE,
        UNARY_PLUS,
        UNARY_MINUS,
        VAR,
        WHILE,
        WHITE_SPACE,
        XOR,
        ILLEGAL_CHAR
    }
}