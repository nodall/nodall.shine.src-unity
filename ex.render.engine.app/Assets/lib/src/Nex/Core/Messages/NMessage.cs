using System;
using System.Reflection;
using System.Linq;
using Nex.Types;

namespace Nex.Core
{
    public class NMessage : EventArgs
    {
        #region [ PROPERTIES ]
        public NMessageDescriptor Descriptor { get; internal set; }
        public object[] Arguments { get; internal set; }
        public object Source { get; internal set; }
        #endregion

        #region [ CONSTRUCTOR ]
        public NMessage(object source, NMessageDescriptor descriptor, params object[] args)
        {
            Source = source;
            Descriptor = descriptor;

            if (Descriptor.Arguments.Length != args.Length)
                throw new ArgumentException("Arguments numbers error");

            for (int i = 0; i < args.Length; i++)
            {
                if ((args[i] == null && Descriptor.Arguments[i].Type.GetTypeInfo().IsClass) ||
                    args[i] == null && Descriptor.Arguments[i].Type.GetTypeInfo().IsInterface ||
                    args[i].GetType() == Descriptor.Arguments[i].Type ||
#if !UNITY_EDITOR && !UNITY_STANDALONE && !UNITY_ANDROID && !UNITY_IOS
                    args[i].GetType().GetTypeInfo().ImplementedInterfaces.Contains(Descriptor.Arguments[i].Type) ||
#else
                    args[i].GetType().GetTypeInfo().GetInterfaces().Contains(Descriptor.Arguments[i].Type) ||
#endif
                    args[i].GetType().GetTypeInfo().IsSubclassOf(Descriptor.Arguments[i].Type))
                    continue;
                else
                    throw new Exception("Param type error: " + Descriptor.Arguments[i]);
            }

            Arguments = args.ToArray();
        }
        #endregion

        #region [ INDEXER ]
        public object this[string argName] { get { return Arguments[Descriptor.IndexOf(argName)]; } }
        public object this[int index] { get { return Arguments[index]; } }
        public T Get<T>(string name) { return (T)this[name]; }
        public T Get<T>() { return (T)this["Value"]; }
        public T Get<T>(int index) { return (T)this[index]; }
        #endregion

        #region [ ToString() ]
        public override string ToString()
        {
            string str = String.Format("[{0}, ", Descriptor.GetType().FullName);
            for (int i = 0; i < Arguments.Count(); i++)
                str += String.Format("[{0}, {1}, {2}]", Descriptor.Arguments[i].Name, Descriptor.Arguments[i].Type, Arguments[i]);

            str += "]";
            return str;
        }
        #endregion

    }

    public class NMessage<T> : NMessage
    {
        #region [ CONSTRUCTOR ]
        public NMessage(object source, NMessageDescriptor<T> descriptor, T arg)
            : base(source, descriptor, arg)
        { }
        #endregion
    }
}
