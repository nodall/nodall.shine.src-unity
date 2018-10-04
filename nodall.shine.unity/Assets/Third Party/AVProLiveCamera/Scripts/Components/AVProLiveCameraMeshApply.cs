using UnityEngine;

//-----------------------------------------------------------------------------
// Copyright 2012-2018 RenderHeads Ltd.  All rights reserverd.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProLiveCamera
{
	[AddComponentMenu("AVPro Live Camera/Mesh Apply")]
	public class AVProLiveCameraMeshApply : MonoBehaviour
	{
		public MeshRenderer _mesh;
		public AVProLiveCamera _liveCamera;

		void Start()
		{
			if (_liveCamera != null && _liveCamera.OutputTexture != null)
			{
				ApplyMapping(_liveCamera.OutputTexture);
			}
		}

		void Update()
		{
			if (_liveCamera != null && _liveCamera.OutputTexture != null)
			{
				ApplyMapping(_liveCamera.OutputTexture);
			}
		}

		private void ApplyMapping(Texture texture)
		{
			if (_mesh != null)
			{
				Material[] materials = _mesh.materials;
				for (int i = 0; i < materials.Length; i++)
				{
					materials[i].mainTexture = texture;
				}
			}
		}

		public void OnDisable()
		{
			ApplyMapping(null);
		}
	}
}