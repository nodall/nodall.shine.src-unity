using System;

namespace Nex.Core
{
    public interface IValue<T>
    {
        T Value { get; set; }
    }
}
