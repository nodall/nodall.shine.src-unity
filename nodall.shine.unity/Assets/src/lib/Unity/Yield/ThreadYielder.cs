using System;
using UnityEngine;

class ThreadYielder<T> : CustomYieldInstruction
{
    #region [ properties ]
    public T Result { get; set; }
    public bool IsFinished { get; set; }
    public Exception Exception { get; set; }
    #endregion

    #region [ CustomYieldInstruction ]
    public override bool keepWaiting { get { return !IsFinished; } }
    #endregion
}
