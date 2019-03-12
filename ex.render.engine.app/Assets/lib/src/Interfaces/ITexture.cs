using System;
using UnityEngine;

namespace nexcode.nwcore
{
    public interface ITexture
    {        
        Texture Texture { get; }
        float Opacity { get; set; }
    }
}
