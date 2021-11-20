/*
    A01745219 Eduardo R. Muller Romero
    A01376228 Sebastian Morales Martin
    A01746645 Guillermo Adrian Urbina A.
*/

using System;
using System.Collections.Generic;

namespace Falak
{
    public class FirstVisitor
    {
        public bool mainPresent = false;
        public bool visitingMain = false;
        
        //TODO: Crear tabla VGST
        
        public IDictionary<string, FunCollection> FGST =
            new SortedDictionary<string, FunCollection>() {
                { "printI", new FunCollection(true, 1, null)},
                { "printC", new FunCollection(true, 1, null)},
                { "printS", new FunCollection(true, 1, null)},
                { "printLN", new FunCollection( true, 0, null)},
                { "readI", new FunCollection(true, 0, null)},
                { "readS", new FunCollection(true, 0, null)},
                { "new", new FunCollection(true, 1, null)},
                { "size", new FunCollection(true, 1, null)},
                { "add", new FunCollection( true, 2, null)},
                { "get", new FunCollection(true, 2, null)},
                { "set", new FunCollection(true, 3, null)},
            };

        public void Visit(Program node)
        {
            Visit((dynamic)node[0]);
            if (!mainPresent)
            {
                throw new SemanticError(
                    "No main function found"
                );
            }
        }

        public void Visit(idList node)
        {
            VisitChildren(node); //Can go to vardef or fundef
        }
        
        public void Visit(funDef node) {  
 
            var functionName = node.AnchorToken.Lexeme;

            if (functionName == "main")
            {
                mainPresent = true;
                visitingMain = true;
            }
            if (Table.ContainsKey(functionName)) {
                throw new SemanticError(
                    "Duplicated function: " + functionName,
                    node.AnchorToken);

            } else {
                int idListCount = 0;
                FGST[functionName] = new FunCollection(false, idListCount, null);
                VisitChildren(node);
            }
			
        }
    }
}