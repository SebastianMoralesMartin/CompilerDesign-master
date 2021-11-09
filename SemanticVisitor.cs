namespace Falak
{
    public class SemanticVisitor
    {
        static readonly IDictionary<TokenCategory, Type> typeMapper =
            new Dictionary<TokenCategory, Type>() {
                { TokenCategory.BOOL, Type.BOOL },
                { TokenCategory.INT, Type.INT }
            };
        
    }
}