using UnityEngine;
using QRCoder;
using UnityEngine.UI;
using System;
using Components;
using nexcode.nwcore;

namespace Shine.Components
{
    [Serializable]
    public class QRComponentProps : ComponentProps
    {
        public string qrData = "-";
        public Color backColor = Color.black;
        public Color foreColor = Color.white;
    }

    public class QRGeneratorComponent : TextureComponentBase<QRComponentProps>
    {
        #region [ public fields ]
        public Color lastBackColor, lastForeColor;
        public string lastQrData = "";
        #endregion

        void Awake()
        {
            props = new QRComponentProps();
        }

        void GenerateCode()
        {
            var qrGen = new QRCodeGenerator();
            var qrData = qrGen.CreateQrCode(props.qrData, QRCodeGenerator.ECCLevel.H);
            texture = new UnityQRCode(qrData).GetGraphic(10, props.backColor, props.foreColor);
        }

        #region [ MonoBehaviour ]
        void Start()
        {
            OnPropsChanged += (src, ev) =>
            {
                if (lastQrData != props.qrData || lastForeColor != props.foreColor || lastBackColor != props.backColor)
                {
                    GenerateCode();
                    lastQrData = props.qrData;
                    lastForeColor = props.foreColor;
                    lastBackColor = props.backColor;
                }
            };
        }
        #endregion
    }
}
