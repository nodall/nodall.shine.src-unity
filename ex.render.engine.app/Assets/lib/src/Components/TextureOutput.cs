using UnityEngine;
using System.Collections;
using System;

namespace nexcode.nwcore
{
    public class TextureOutput : ComponentBase<ComponentProps>
    {
        public bool isBlackout;
        public MediaCompositorBase<MediaCompositorProps> gridCompositor;

        bool isGridEnabled = false;
        ITexture gridTexture;


        public ComponentBase CurrentMedia
        {
            get
            {
                return inputs.Length > 0 ? inputs[0] : null;
            }
            set
            {
                SetInput(value);
            }
        }

        public Texture Texture
        {
            get
            {
                return inputs.Length > 0 ? inputs[0].texture : null;
            }
        }

        private void Start()
        {
            gridCompositor = ComponentManager.New<FadeMediaCompositor>(new MediaCompositorProps { width = 1920, height = 1080 });
            gridCompositor.inputs = new ComponentBase[2];
        }

        public void Blackout(bool value)
        {
            isBlackout = value;
        }

        public void EnableGrid(bool enabled)
        {
            if (isGridEnabled != enabled)
            {
                isGridEnabled = enabled;
            }
        }

        public void SetGrid(ITexture gridTexture)
        {
            this.gridTexture = gridTexture;
        }

        public override void OnUpdate()
        {
            texture = Texture;
        }

        /*
        private void Update()
        {
            if (current is IUpdateable)
                (current as IUpdateable).Update();

            //current.Opacity = 1;

            //gridTexture.Opacity = Mathf.Lerp(gridTexture.Opacity, isGridEnabled ? 1 : 0, 0.1f);

            //gridCompositor.inputs[0] = current as ComponentBase;
            //gridCompositor.inputs[1] = gridTexture as ComponentBase;
            //gridCompositor.Update();
        }*/
    }
}