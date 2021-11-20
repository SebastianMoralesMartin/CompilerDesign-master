namespace Falak
{
    public class GlobalVarTable
    {
        private HashSet<Type> table;
    }
    public GlobalVarTable(){
        table = new HashSet<Type>();
    }
    
    public override string ToString()
    {
    var sb = new StringBuilder();
    sb.Append("---------GLOBAL-VARS------------------\n");
    foreach (string entry in tableSet)
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

    public bool Add (string Name){
        return table.Add(name);
    }

    public IEnumerator<string> GetEnumerator()
        {
            return tableSet.GetEnumerator();
        }
}