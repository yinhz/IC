using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IC.Core
{
    public class Singleton<T>
        where T : new()
    {
        public static T Instance
        {
            get { return _Singleton._Instance; }
        }

        internal class _Singleton
        {
            internal static readonly T _Instance = new T();
        }
    }
}
