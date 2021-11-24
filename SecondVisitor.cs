/*
    A01745219 Eduardo R. Muller Romero
    A01376228 Sebastian Morales Martin
    A01746645 Guillermo Adrian Urbina A.
*/

using System;
using System.Collections.Generic;

namespace Falak
{
    public class SecondVisitor
    {
        public Type container;
        public LocalTable globalFunctions;
        public GlobalVarTable globalVariables;
        public LocalTable localFuncTable;
        List<int> numberParameters = new List<int>();
        int loops = 0;
        bool inFunction = false;
        bool inFunctionCall = false;
        int numberFunctionCall = 0;
        
        public LocalTable globalFunctionsParam{
            get;
            private set;
        }

        public GlobalVarTable globalVariablesParam{
            get;
            private set;
        }
        
        //Constructor---------------------------------------------------------------------------------------------------
        
        public SecondVisitor(LocalTable globalFunctionsParam, GlobalVarTable globalVariablesParam)
        {
            globalFunctions = globalFunctionsParam;
            globalVariables = globalVariablesParam;
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
            // Console.WriteLine("Inside the Second Visit. \nFunctions: ");
            // Console.WriteLine(globalFunctions.ToString());
            // Console.WriteLine("Global Variables: ");
            // Console.WriteLine(globalVariables.ToString());
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
            var name = node.AnchorToken.Lexeme;
            container = Type.GetFromSet(globalFunctions.getTable(), name);
            Console.WriteLine("Container " + container);
            container.CustomArray[3] = new LocalTable();
            localFuncTable = (LocalTable) container.CustomArray[3];
            Console.WriteLine(localFuncTable.ToString());
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
                            "Duplicated parameter " + paramName +
                            node.AnchorToken);
                    }

                    else
                    {
                        localFuncTable.Add(new Type(new List<object>() { paramName, "Parameter" }));
                        parameters += 1;
                    }
                }
            }

            /*if (parameters > 0)
            {
                var paramName = node[0].AnchorToken.Lexeme;
                if (localFuncTable.Contains(paramName))
                {
                    throw new SemanticError(
                        "Duplicated parameter2 " + paramName +
                        node[0].AnchorToken);
                }
                else
                {
                    localFuncTable.Add(new Type(new List<object>() { paramName, "parameter"}));
                }
            }*/
        }

        public void Visit(varDef node)
        {
            if (inFunction)
            {
                var name = node.AnchorToken.Lexeme;
                if (localFuncTable.Contains(name))
                {
                    throw new SemanticError(
                        "Duplicated variable " + name,
                        node.AnchorToken);
                }else
                {
                    localFuncTable.Add(new Type(new List<object>() {name, "Local Variable"}));
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
        
        public void Visit(funCall node){
            var name = node.AnchorToken.Lexeme;
            Type containerFun;
            numberFunctionCall+=1;

            if (numberFunctionCall > numberParameters.Count){
                numberParameters.Add(0);
            }
            
            numberParameters[numberFunctionCall - 1] = 0;

            if(! globalFunctions.Contains(name)){
                if(! ((string) container.CustomArray[0]).Equals(name)){
                    throw new SemanticError("Undeclared Function " + name + "\n" + node.AnchorToken); 
                } else{
                    inFunctionCall = true;
                    numberParameters[numberFunctionCall - 1] +=1;
                    Visit((dynamic) node[0]);
                    inFunctionCall = false;
                    int arity = 0;
                    Console.WriteLine("Before foreach1" + name);
                    foreach (var i in node[0]){
                        arity +=1;
                    }
                    if( ! ( (string)container.CustomArray[2] ).Equals(arity.ToString())){
                        throw new SemanticError("Parameter error 1" + name + node.AnchorToken);
                    }
                }

            } else {
                containerFun = Type.GetFromSet(globalFunctions.getTable(), name);
                inFunctionCall = true;
                numberParameters[numberFunctionCall - 1] += 1;
                Visit((dynamic) node[0]);
                inFunctionCall = false;
                int arity = 0;
                Console.WriteLine("Before foreach2  " + (string)containerFun.CustomArray[2] );
                if((string)containerFun.CustomArray[2] != "0"){
                    foreach (var i in node[0]){
                        arity +=1;
                    }
                }
                if(!( (string)containerFun.CustomArray[2] ).Equals(arity.ToString())){
                    Console.WriteLine((string)containerFun.CustomArray[2] + "=" + arity.ToString());
                    throw new SemanticError("Parameter error 2" + name + node.AnchorToken);
                }
                globalFunctions.Add(containerFun);
            }
            numberFunctionCall -= 1;
        }

        public void Visit(exprFunCall node)
        {
            var name = node.AnchorToken.Lexeme;
            Type containerFun;
            numberFunctionCall += 1;

            if (numberFunctionCall > numberParameters.Count){
                numberParameters.Add(0);
            }

            numberParameters[numberFunctionCall - 1] = 0;

            if( ! globalFunctions.Contains(name)){
                if( ! ( (string) container.CustomArray[0] ).Equals(name)){
                    throw new SemanticError("Undeclared Function " + name + "\n" + node.AnchorToken);
                } else {
                    inFunctionCall = true;
                    numberParameters[numberFunctionCall - 1] = 1;
                    Visit((dynamic) node[0]);
                    inFunctionCall = false;
                    int arity = 0;
                    foreach (var i in node[0]){arity += 1;}
                    if( ! ( (string)container.CustomArray[2]).Equals(arity.ToString() ) ){
                        throw new SemanticError("The number of parameters in " + name + " does not match");
                    }

                }
            } else {
                containerFun = Type.GetFromSet(globalFunctions.getTable(), name);
                inFunctionCall = true;
                numberParameters[numberFunctionCall - 1] += 1;
                Visit( (dynamic) node[0]);
                inFunctionCall = false;
                int arity = 0;
                foreach (var item in node[0]) { arity += 1; }
                if (!((string)containerFun.CustomArray[2]).Equals(arity.ToString()))
                {
                    throw new SemanticError(
                        "Parameters do not match in " + name);
                }
                globalFunctions.Add(containerFun);
            }
            numberFunctionCall -= 1;
        }

        
        
        public void Visit(assign node)
        {
            var name = node[0].AnchorToken.Lexeme;
            if (!localFuncTable.Contains(name) && !globalVariables.Contains(name)){
                Console.WriteLine("In assign: " + name + " \n" + localFuncTable.ToString() + "\n" + globalVariables.ToString());
                throw new SemanticError("Error in assign " + name + "  " + node.AnchorToken);
            }
            Visit( (dynamic) node[0]);
        }
        
        public void Visit(stmtWhile node)
        {
            loops += 1;
            VisitChildren(node);
            loops -= 1;
        }

        public void Visit(stmtDoWhile node)
        {
            loops += 1;
            VisitChildren(node);
            loops -= 1;
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
        
        public void Visit(stmtReturn node)
        {
            VisitChildren(node);
        }

        public void Visit(exprList node)
        {
            if(inFunctionCall){
                numberParameters[numberFunctionCall - 1] = 0;
                foreach (var n in node){
                    Visit( (dynamic) n);
                    numberParameters[numberFunctionCall - 1] += 1;
                }
            } else {
                VisitChildren(node);
            }
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
            /*var id = node.AnchorToken.Lexeme;
            if ( ! localFuncTable.Contains(id)){
                if( ! globalVariables.Contains(id)){
                    throw new SemanticError("undeclared identifier " + id + node.AnchorToken);
                       //if (inFunction && ! globalFunctions.Contains(id)){}
                       
                }
            }*/
        }
        
        public void Visit(INT node)
        {
            var n = node.AnchorToken.Lexeme;
            int result;
            if( ! Int32.TryParse(n, out result)){
                throw new SemanticError("Int it's beyond limits" + n + node.AnchorToken);
            }
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
        
        // ---Bunch of empty stuff --------------------------------------------
        
        public void Visit(stmtIncr node){}
        public void Visit(stmtDecr node){}
        public void Visit(STRING node){}
        public void Visit(Character node){}
        public void Visit(TRUE node){}
        public void Visit(FALSE node){}
        public void Visit(stmtEmpty node){}
        public void Visit(stmtBreak node){}
        public void Visit(Positive node){}
        public void Visit(Negative node){}
    }
}