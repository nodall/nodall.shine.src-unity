using System;
using System.Collections.Generic;

namespace Nexwarp.Network
{
    public interface IStreamCollectionTransfer
    {
        IStreamCollectionTransfer OnStart(Action<IEnumerable<IStreamTransfer>> action);
        IStreamCollectionTransfer OnEnd(Action action);
        IStreamCollectionTransfer OnException(Action action);
    }
}
