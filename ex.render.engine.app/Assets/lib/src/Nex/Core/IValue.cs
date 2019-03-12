using System;

namespace Nex.Core
{
    public interface IValue
    {
        Type ValueType { get; }
        object Value { get; set; }
    }
}
