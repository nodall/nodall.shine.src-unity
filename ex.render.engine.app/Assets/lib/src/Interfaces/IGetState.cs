using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGetState {
    object GetState();
}

public interface IGetState<T> : IGetState
{
}
