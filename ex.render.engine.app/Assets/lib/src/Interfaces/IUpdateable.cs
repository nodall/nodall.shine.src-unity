using UnityEngine;
using System.Collections;

namespace nexcode.nwcore
{
    public interface IUpdateable
    {
        bool IsUpdated { get; }
        void Update();
    }
}