/*
    A01745219 Eduardo R. Muller Romero
    A01376228 Sebastian Morales Martin
    A01746645 Guillermo Adrian Urbina A.
*/

namespace Falak
{
    public class SemanticError: Exception
    {
        public SemanticError(string message, Token token):
            base($"Semantic Error: {message} \n"
                 + $"at row {token.Row}, column {token.Column}.") {
        }
    }
}