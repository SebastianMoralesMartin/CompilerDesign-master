namespace Falak
{
    public class LocalTable
    {
        HashSet<Type> table = new HashSet<Type>();
        public override string ToString()
        {
            var sb = new StringBuilder();
            
            foreach (Type entry in tableSet) {
                sb.Append("|-|" + String.Join(", ",entry.CustomArray) + "|-|\n");
            }
            return sb.ToString();
        }

        public bool Contains(string key)
        {
            return table.Contains(new Type(new List<object>() { key }));
        }

        public bool Add(Type container)
        {
            return table.Add(container);
        }

        public HashSet<Type> getTable()
        {
            return table;
        }
        
        public IEnumerator<Type> GetEnumerator()
        {
            return table.GetEnumerator();
        }
    }
}