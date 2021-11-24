using System;
using System.Collections.Generic;

namespace Falak
{
    public class Type
    {
        public List<object> CustomArray { get; set; }

        public Type(List<object> arrStored)
        {
            this.CustomArray = arrStored;
        }

        // Obtenemos los valores de nuestra tabla para funCall y fundef
        public static Type GetFromSet(HashSet<Type> table, string name)
        {
            foreach (Type element in table)
            {
                // Checamos si son la misma funcion
                if (((string)element.CustomArray[0]).Equals(name))
                {
                    return element;
                }
            }
            return null;
        }

        public override bool Equals(Object obj)
        {
            var containerEvaluated = obj as Type;
            if (containerEvaluated == null) {
                return false;
            }
            return ((string)this.CustomArray[0]).Equals((string)containerEvaluated.CustomArray[0]);
        }


        public override int GetHashCode()
        {
            return 0;
        }
    }
}