using System;

namespace Nexwarp.Network
{
    public interface IStreamTransfer
    {
        IStreamCollectionTransfer OnStart(Action<StreamTransferModel> action);
        IStreamCollectionTransfer OnEnd(Action<StreamTransferModel> action);
        IStreamCollectionTransfer OnProgress(Action<StreamTransferModel, double> action);
        IStreamCollectionTransfer OnException(Action<StreamTransferModel> action);
    }
}
