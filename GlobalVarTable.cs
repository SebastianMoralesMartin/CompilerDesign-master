using System;
using System.Text;
using System.Collections.Generic;

namespace Falak
{
    public class GlobalVarTable
    {
        HashSet<string> table;
    

    public GlobalVarTable(){
        table = new HashSet<string>();
    }
    
    public override string ToString()
    {
    var sb = new StringBuilder();
    sb.Append("---------GLOBAL-VARS------------------\n");
    foreach (string entry in table)
    {
        sb.Append(String.Format("{0}\n",
            entry));
    }
    sb.Append("--------------------------------------\n");
    return sb.ToString();
    }

    public bool Contains (string name){
        return table.Contains(name);
    }

    public bool Add (string name){
        return table.Add(name);
    }

    public IEnumerator<string> GetEnumerator()
        {
            return table.GetEnumerator();
        }
}}