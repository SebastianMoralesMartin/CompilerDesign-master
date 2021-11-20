/*
    A01745219 Eduardo R. Muller Romero
    A01376228 Sebastian Morales Martin
    A01746645 Guillermo Adrian Urbina A.
*/

using System;

namespace Falak
{
    public class SemanticError: Exception
    {
        public SemanticError(string message, Token token):
            base($"Semantic Error: {message} \n"
                 + $"at row {token.Row}, column {token.Column}.") {
        }

        public SemanticError(string message) :
            base("$Semantic Error: Dont know what went wrong :/")
        {
            
        }
    }
}