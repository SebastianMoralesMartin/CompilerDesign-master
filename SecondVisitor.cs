/*
    A01745219 Eduardo R. Muller Romero
    A01376228 Sebastian Morales Martin
    A01746645 Guillermo Adrian Urbina A.
*/

namespace Falak
{
    public class SecondVisitor
    {
        public Type container;
        SymbolTableLocal globalTableFuncs;
        SymbolTableGlobalVars globalTableVars;
        SymbolTableLocal localFuncTable;
        List<int> numberParameters = new List<int>();
        int numberLoops = 0;
        bool inFunction = false;
        bool inFunctionCall = false;
        int numberFunctionCall = 0;
        
        public LocalTable globalFunctions{
            get;
            private set;
        }

        public GlobalVarTable globalVariables{
            get;
            private set;
        }
        
        //Constructor---------------------------------------------------------------------------------------------------
        
        public SecondVisitor(LocalTable globalFunctions, GlobalVarTable globalVariables)
        {
            this.globalFunctions = globalFunctions;
            this.globalVariables = globalVariables;
        }
        
        //VisitChildren func -------------------------------------------------------------------------------------------
        
        void VisitChildren(Node node)
        {
            foreach (var n in node)
            {
                Visit((dynamic)n);
            }
        }

        public void Visit(Program node)
        {
            Visit((dynamic) node[0]);
        }

        public void Visit(defList node)
        {
            VisitChildren(node);
        }

        public void Visit(idList node)
        {
            VisitChildren(node);
        }

        public void Visit(funDef node)
        {
            inFunction = true;
            var name = node[0].AnchorToken.Lexeme;
            container = Type.GetFromSet(globalFunctions.getTable(), name);
            container.CustomArray[3] = new LocalTable();
            localFuncTable = (SymbolTableLocal) container.CustonmArray[3];
            VisitChildren(node);
            globalFunctions.Add(container);
            inFunction = false;

        }
        
        public void Visit(ParameterList node)
        {
            int children = 0;
            int parameters = 0;
            foreach (var n in node)
            {
                children += 1;
            }

            if (children > 0)
            {
                foreach (var n in node[0])
                {
                    var paramName = n.AnchorToken.Lexeme;
                    if (localFuncTable.Contains(paramName))
                    {
                        throw new SemanticError(
                            "Duplicated variable " + paramName,
                            node.AnchorToken);
                    }

                    else
                    {
                        localFuncTable.Add(new Type(new List<object>() { parameterName, "Parameter" }));
                        parameters += 1;
                    }
                }
            }

            if (parameters == 0)
            {
                var paramName = node[0].AnchorToken.Lexeme;
                if (localFuncTable.Contains(parameterName))
                {
                    throw new SemanticError(
                        "Duplicated variable " + paramName,
                        node[0].AnchorToken);
                }
                else
                {
                    localFuncTable.Add(new Type(new List<object>() { parameterName, "parameter"}));
                }
            }
        }

        public void Visit(varDef node)
        {
            if (inFunction)
            {
                var name = node.AnchorToken.Lexeme;
                if (localFuncTable.Contains(name))
                {
                    throw new SemanticError(
                        "Duplicated variable " + varName,
                        node.AnchorToken);
                }else
                {
                    localFuncTable.Add(new Type(new List<object>() {varName, "Local Variable"}));
                }
            }
        }
        
        
        public void Visit(varDefList node)
        {
            VisitChildren(node);
        }

        public void Visit(stmtList node)
        {
            VisitChildren(node);
        }
        
        public void Visit(exprFunCall node)
        {
            
        }
        
        public void Visit(assign node)
        {
            
        }
        
        public void Visit(stmtWhile node)
        {
            
        }

        public void Visit(stmtDoWhile node)
        {
            
        }

        public void Visit(stmtIf node)
        {
            VisitChildren(node);
        }
        
        public void Visit(elseIfList node)
        {
            VisitChildren(node);
        }

        public void Visit(stmtElse node)
        {
            VisitChildren(node);
        }

        public void Visit(stmtIncr node)
        {
            //empty
        }
        
        public void Visit(stmtDecr node)
        {
            //empty
        }
        
        public void Visit(stmtReturn node)
        {
            VisitChildren(node);
        }
        
        public void Visit(stmtEmpty node)
        {
            //empty
        }



        public void Visit(exprList node)
        {
            
        }

        public void Visit(exprOr node)
        {
            VisitChildren(node);
        }

        public void Visit(exprAnd node)
        {
            VisitChildren(node);
        }
        
        public void Visit(identifier node)
        {
            
        }
        
        public void Visit(INT node)
        {
            
        }
        public void Visit(greater_than node)
        {
            VisitChildren(node);
        }
        
        public void Visit(greater_equal_than node)
        {
            VisitChildren(node);
        }
        
        public void Visit(less_than node)
        {
            VisitChildren(node);
        }
        
        public void Visit(less_equal_than node)
        {
            VisitChildren(node);
        }
        
        public void Visit(equals_to node)
        {
            VisitChildren(node);
        }
        
        public void Visit(not_equal node)
        {
            VisitChildren(node);
        }

        public void Visit(Plus node)
        {
            VisitChildren(node);
        }
        
        public void Visit(Neg node)
        {
            VisitChildren(node);
        }
        
        public void Visit(Mul node)
        {
            VisitChildren(node);
        }
        
        public void Visit(Div node)
        {
            VisitChildren(node);
        }
        
        public void Visit(Remainder node)
        {
            VisitChildren(node);
        }

        public void Visit(exprUnary node)
        {
            VisitChildren(node);
        }

        public void Visit(array node)
        {
            Visit((dynamic) node[0]);
        }
        
        public void Visit(STRING node){}
        public void Visit(Character node){}
        
        public void Visit(TRUE node){}
        public void Visit(FALSE node){}
    }
}