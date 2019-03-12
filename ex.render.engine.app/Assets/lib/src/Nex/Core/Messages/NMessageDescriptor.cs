using System;
using System.Reflection;
using System.Linq;
using Nex.Types;

namespace Nex.Core
{
    public class NMessageDescriptor
    {
        #region [ STATIC METHODS ]

        static public NMessageDescriptor[] Get(Type type)
        {
#if !UNITY_EDITOR && !UNITY_STANDALONE && !UNITY_ANDROID && !UNITY_IOS
            FieldInfo[] fieldInfos = type.GetTypeInfo().DeclaredFields.Where((info) => info.IsStatic && info.IsPublic).ToArray();
#else
            FieldInfo[] fieldInfos = type.GetTypeInfo().GetFields().Where((info) => info.IsStatic && info.IsPublic).ToArray();
#endif
            var result = (from field in fieldInfos
                          where field.FieldType == typeof(NMessageDescriptor) ||
                          field.FieldType.GetTypeInfo().IsSubclassOf(typeof(NMessageDescriptor))
                          select (NMessageDescriptor)field.GetValue(null)).ToArray();

            return result;
        }

        #endregion

        #region [ PROPERTIES ]
        public NMessageArgument[] Arguments { get; internal set; }
        #endregion

        #region [ CONSTRUCTORS ]
        public NMessageDescriptor(params NMessageArgument[] args)
        {
            Arguments = args.ToArray(); ;
        }
        #endregion

        #region [ PUBLIC METHODS && INDEXER ]
        public int IndexOf(string argName)
        {
            for (int i = 0; i < Arguments.Length; i++)
            {
                if (argName == Arguments[i].Name)
                    return i;
            }
            throw new ArgumentException("Argument not found");
        }
        public Type this[string argName]
        {
            get
            {
                var result = from arg in Arguments where argName == arg.Name select arg;
                if (result.Count() == 0)
                    throw new ArgumentException(String.Format("Argument '{0}' not found.", argName));
                else
                    return result.First().Type;
            }
        }
        #endregion

        #region [ ToString() ]
        public override string ToString()
        {
            string str = String.Format("['{0}',", GetType().Name);
            foreach (var type in Arguments)
                str += type;

            str += "]";
            return str;
        }
        #endregion
    }


    public class NMessageDescriptor<T> : NMessageDescriptor
    {
        #region [ CONSTRUCTORS ]
        public NMessageDescriptor()
            : base(new NMessageArgument<T>("Value"))
        { }
        #endregion
    }
}
