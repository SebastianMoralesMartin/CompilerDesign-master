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

        Type tempContainer;

        // Table creation

        public LocalTable GlobalFunctionsTable{
            get;
            private set;
        }

        public GlobalVarTable GlobalVars{
            get;
            private set;
        }


        public FirstVisitor(){
            GlobalFunctionsTable = new LocalTable();
            GlobalVars = new GlobalVarTable();

            GlobalFunctionsTable.Add(new Type(new List<object>() { "printi", "true", "1", "null" }));
            GlobalFunctionsTable.Add(new Type(new List<object>() { "printc", "true", "1", "null" }));
            GlobalFunctionsTable.Add(new Type(new List<object>() { "prints", "true", "1", "null" }));
            GlobalFunctionsTable.Add(new Type(new List<object>() { "println", "true", "0", "null" }));
            GlobalFunctionsTable.Add(new Type(new List<object>() { "readi", "true", "0", "null" }));
            GlobalFunctionsTable.Add(new Type(new List<object>() { "reads", "true", "0", "null" }));
            GlobalFunctionsTable.Add(new Type(new List<object>() { "new", "true", "1", "null" }));
            GlobalFunctionsTable.Add(new Type(new List<object>() { "size", "true", "1", "null" }));
            GlobalFunctionsTable.Add(new Type(new List<object>() { "add", "true", "2", "null" }));
            GlobalFunctionsTable.Add(new Type(new List<object>() { "get", "true", "2", "null" }));
            GlobalFunctionsTable.Add(new Type(new List<object>() { "set", "true", "3", "null" }));
        }

        
        //TODO: Crear tabla VGST
        
        /*public IDictionary<string, FunCollection> FGST =
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
            };*/

        public void Visit(Program node)
        {
            Visit((dynamic)node[0]);
            
            if (!mainPresent)
            {
                throw new SemanticError(
                    "No main"
                );
            }
        }

        public void Visit(defList node){
            VisitChildren(node); 
        }

        public void Visit(idList node)
        {
            VisitChildren(node); //Can go to vardef or fundef
        }
        
        public void Visit(funDef node) {  
 
            string functionName = node.AnchorToken.Lexeme;

            //Console.WriteLine(functionName);

            if (functionName == "main")
            {
                mainPresent = true;
                visitingMain = true;
            }
            if (GlobalFunctionsTable.Contains(functionName)) {
                throw new SemanticError(
                    "Duplicated function: " + functionName,
                    node.AnchorToken);

            } else {

                tempContainer = new Type(new List<object>(){functionName, "false"});
                //Console.WriteLine("En la funcion " + functionName + "\nQuieres Visitar a " + node[1]);
                Visit((dynamic) node[0]);
            }
			
        }

        public void Visit(varDef node){
            var variableName = node.AnchorToken.Lexeme;

            if(GlobalVars.Contains(variableName)){
                throw new SemanticError(
                    "Duplicated variable: " + variableName,
                    node.AnchorToken);
            }
            else{
                GlobalVars.Add(variableName);
            }
            
        }
        
        public void Visit(ParameterList node)
        {
            //Console.WriteLine("This should print");
            int children = 0;
            int parameters = 0;
            foreach (var n in node)
            {
                children += 1;
            }
            if (children > 0)
            {
                if (visitingMain)
                {
                    throw new SemanticError(
                    "Main function must not accept any variable.");
                }
                foreach (var n in node[0])
                {
                    parameters += 1;
                }
                if (parameters > 0)
                {
                    tempContainer.CustomArray.Add(parameters.ToString());
                }
                else
                {
                    tempContainer.CustomArray.Add("1");
                }
            }
            else
            {
                
                tempContainer.CustomArray.Add("0");
            }

            tempContainer.CustomArray.Add(new LocalTable());
            GlobalFunctionsTable.Add(tempContainer);
            visitingMain = false;
            /*Console.WriteLine("A verr Sebo :O \nChildren: " + children + "/nParam: " + parameters);
            Console.WriteLine("Tabla: \n" + GlobalFunctionsTable.ToString());
            Console.WriteLine("Tabla Variables \n" + GlobalVars.ToString());*/
        }

        public void Visit(identifier node)
        {
            
        }



        public void VisitChildren(Node node){
            foreach(var i in node)
            {
                Visit((dynamic)i);
                
            }
        }
    }
}