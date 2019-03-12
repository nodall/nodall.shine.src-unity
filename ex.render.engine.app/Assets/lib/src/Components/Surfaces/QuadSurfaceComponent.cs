using System;
using UnityEngine;
using System.Collections;
using Nex.Math;
using Newtonsoft.Json;

namespace nexcode.nwcore
{
    [Serializable()]
    public class QuadSurfaceComponentProps : ComponentProps
    {
        public string displayName = "Surface";
        public Rect blendRect = new Rect(0, 0, 0, 0);
        public Rect textureRect = new Rect(0, 0, 1, 1);
        public Bezier2D bezier = new Bezier2D(new Rectangle2D(0, 0, 1, 1));
        public float depth = 20;
    }

    public class QuadSurfaceComponent : ComponentBase<QuadSurfaceComponentProps>
    {

        [Range(0, 1)]
        public float opacity = 1f;

        public float width = 1.0f;
        public float length = 1.0f;


        Rect lastTextureRect;
        Bezier2D lastBezier;
        string lastBezierStr;

        //public Vector2[] bezierPoints = new Vector2[16];

        int widthSegments;
        int lengthSegments;

        Texture inputTexture = null;
        public bool neededVerticalFlip = false;

        public Material material;
        public Material defMaterial;

        // Use this for initialization
        void Start()
        {

            if (Props.bezier == null)
            {
                Props.bezier = new Bezier2D(new Rectangle2D(0, 0, 1, 1));
            }

            var meshFilter = gameObject.AddComponent<MeshFilter>();
            var renderer = gameObject.AddComponent<MeshRenderer>();

            material = Resources.Load("FX/MatFadeTexGridFX") as Material;
            renderer.material = material;
            renderer.material.color = new Color(1f, 1f, 1f, opacity);

            defMaterial = material;

            widthSegments = 16;
            lengthSegments = 16;
            meshFilter.mesh = CreatePlaneMesh();

            OnPropsChanged += (s, e) =>
            {
                // recalculate surface points if quad or quadperspective
                props.bezier = JsonConvert.DeserializeObject<Bezier2D>(JsonConvert.SerializeObject(props.bezier));
                props.bezier[0] = props.bezier[0];
                props.bezier[3] = props.bezier[3];
                props.bezier[12] = props.bezier[12];
                props.bezier[15] = props.bezier[15];

                transform.position = new Vector3(0, 0, props.depth+100);

                DeformWithBezier();
                if (props.textureRect != lastTextureRect)
                {
                    UpdateTextureCoordinates();
                    lastTextureRect = Props.textureRect;
                }

            };

            OnInputChanged += (s, e) =>
            {
                var tex = inputs[0];
                var needsFlipY = tex is INeedsVerticalFlip ? (tex as INeedsVerticalFlip).NeedsVerticalFlip : false;
                renderer.material.SetFloat("_FlipY", needsFlipY ? 1 : 0);

                neededVerticalFlip = needsFlipY;
            };

            SetProps(props);
        }

        public override void OnUpdate()
        {
            var cam = Camera.current;
            if (cam != null) transform.localScale = new Vector3(cam.aspect, 1, 1);
            var renderer = gameObject.GetComponent<MeshRenderer>();

            if (renderer == null) return;

            if (inputs.Length > 0)
            {
                var tex = inputs[0];
                var needsFlipY = tex is INeedsVerticalFlip ? (tex as INeedsVerticalFlip).NeedsVerticalFlip : false;

                if (neededVerticalFlip != needsFlipY)
                    renderer.material.SetFloat("_FlipY", needsFlipY ? 1 : 0);
            }

            renderer.material.color = new Color(1f, 1f, 1f, opacity);

            inputTexture = null;
            if (inputs.Length != 0 && inputs[0] != null)
                inputTexture = inputs[0].texture;

            if (inputTexture == null)
                renderer.enabled = false;
            else
            {
                if (!renderer.enabled)
                    renderer.enabled = true;

                renderer.material.mainTexture = inputTexture;
            }
        }

        public enum Orientation
        {
            Horizontal,
            Vertical
        }

        public enum AnchorPoint
        {
            TopLeft,
            TopHalf,
            TopRight,
            RightHalf,
            BottomRight,
            BottomHalf,
            BottomLeft,
            LeftHalf,
            Center
        }

        public AnchorPoint anchor = AnchorPoint.Center;
        public bool twoSided = true;

        Mesh CreatePlaneMesh()
        {
            Vector2 anchorOffset;
            switch (anchor)
            {
                case AnchorPoint.TopLeft:
                    anchorOffset = new Vector2(-width / 2.0f, length / 2.0f);
                    break;
                case AnchorPoint.TopHalf:
                    anchorOffset = new Vector2(0.0f, length / 2.0f);
                    break;
                case AnchorPoint.TopRight:
                    anchorOffset = new Vector2(width / 2.0f, length / 2.0f);
                    break;
                case AnchorPoint.RightHalf:
                    anchorOffset = new Vector2(width / 2.0f, 0.0f);
                    break;
                case AnchorPoint.BottomRight:
                    anchorOffset = new Vector2(width / 2.0f, -length / 2.0f);
                    break;
                case AnchorPoint.BottomHalf:
                    anchorOffset = new Vector2(0.0f, -length / 2.0f);
                    break;
                case AnchorPoint.BottomLeft:
                    anchorOffset = new Vector2(-width / 2.0f, -length / 2.0f);
                    break;
                case AnchorPoint.LeftHalf:
                    anchorOffset = new Vector2(-width / 2.0f, 0.0f);
                    break;
                case AnchorPoint.Center:
                default:
                    anchorOffset = Vector2.zero;
                    break;
            }

            Mesh m = new Mesh();
            m.name = "Plane";

            int hCount2 = widthSegments + 1;
            int vCount2 = lengthSegments + 1;
            int numTriangles = widthSegments * lengthSegments * 6;
            if (twoSided)
            {
                numTriangles *= 2;
            }
            int numVertices = hCount2 * vCount2;

            Vector3[] vertices = new Vector3[numVertices];
            Vector2[] uvs = new Vector2[numVertices];
            int[] triangles = new int[numTriangles];

            int index = 0;
            float uvFactorX = 1.0f / widthSegments;
            float uvFactorY = 1.0f / lengthSegments;
            float scaleX = width / widthSegments;
            float scaleY = length / lengthSegments;
            for (float y = 0.0f; y < vCount2; y++)
            {
                for (float x = 0.0f; x < hCount2; x++)
                {
                    vertices[index] = new Vector3(x * scaleX - width / 2f - anchorOffset.x, y * scaleY - length / 2f - anchorOffset.y, 0.0f);
                    uvs[index++] = new Vector2(x * uvFactorX, y * uvFactorY);
                }
            }

            index = 0;
            for (int y = 0; y < lengthSegments; y++)
            {
                for (int x = 0; x < widthSegments; x++)
                {
                    triangles[index] = (y * hCount2) + x;
                    triangles[index + 1] = ((y + 1) * hCount2) + x;
                    triangles[index + 2] = (y * hCount2) + x + 1;

                    triangles[index + 3] = ((y + 1) * hCount2) + x;
                    triangles[index + 4] = ((y + 1) * hCount2) + x + 1;
                    triangles[index + 5] = (y * hCount2) + x + 1;
                    index += 6;
                }
                if (twoSided)
                {
                    // Same tri vertices with order reversed, so normals point in the opposite direction
                    for (int x = 0; x < widthSegments; x++)
                    {
                        triangles[index] = (y * hCount2) + x;
                        triangles[index + 1] = (y * hCount2) + x + 1;
                        triangles[index + 2] = ((y + 1) * hCount2) + x;

                        triangles[index + 3] = ((y + 1) * hCount2) + x;
                        triangles[index + 4] = (y * hCount2) + x + 1;
                        triangles[index + 5] = ((y + 1) * hCount2) + x + 1;
                        index += 6;
                    }
                }
            }

            m.vertices = vertices;
            m.uv = uvs;
            m.triangles = triangles;
            m.RecalculateNormals();
            m.RecalculateBounds();

            return m;
        }

        public void DeformWithBezier()
        {
            if (Props.bezier == null) return;

            var m = GetComponent<MeshFilter>().mesh;

            Vector2 anchorOffset = Vector2.zero;
            float scaleX = width / widthSegments;
            float scaleY = length / lengthSegments;
            int hCount2 = widthSegments + 1;
            int vCount2 = lengthSegments + 1;
            int index = 0;
            int numVertices = hCount2 * vCount2;
            Vector3[] vertices = new Vector3[numVertices];
            for (float y = 0.0f; y < vCount2; y++)
            {
                for (float x = 0.0f; x < hCount2; x++)
                {
                    var pt = Props.bezier.Compute((x / widthSegments), 1f - (y / lengthSegments));
                    vertices[index] = new Vector3((float)pt.x * width - width / 2f, (float)-pt.y * length + length / 2f, 0f);
                    index++;
                }
            }

            m.vertices = vertices;

            m.RecalculateNormals();
            m.RecalculateBounds();
        }

        public void UpdateTextureCoordinates()
        {
            int hCount2 = widthSegments + 1;
            int vCount2 = lengthSegments + 1;
            int numVertices = hCount2 * vCount2;

            Vector2[] uvs = new Vector2[numVertices];

            int index = 0;
            for (float y = 0.0f; y < vCount2; y++)
            {
                float ty = y / lengthSegments;
                float v = ty * (Props.textureRect.yMin - Props.textureRect.yMax) + Props.textureRect.yMax;

                for (float x = 0.0f; x < hCount2; x++)
                {
                    float tx = x / widthSegments;
                    float u = tx * (Props.textureRect.xMax - Props.textureRect.xMin) + Props.textureRect.xMin;
                    uvs[index++] = new Vector2(u, 1-v);
                }
            }
            var m = GetComponent<MeshFilter>().mesh;
            m.uv = uvs;
        }

        private void SetFlip()
        {
            if (inputs.Length > 0)
            {
                var renderer = gameObject.GetComponent<MeshRenderer>();
                var tex = inputs[0];
                var needsFlipY = tex is INeedsVerticalFlip ? (tex as INeedsVerticalFlip).NeedsVerticalFlip : false;
                renderer.material.SetFloat("_FlipY", needsFlipY ? 1 : 0);
            }
        }

        public void SetFx(ShaderFxManager.FxDef fx)
        {
            var renderer = gameObject.GetComponent<MeshRenderer>();
            if (fx == null || fx.name == null)
            {
                material = defMaterial;
                renderer.material = defMaterial;
                SetFlip();
                return;
            }

            var fxName = fx.name.ToLowerInvariant();
            var matName = material.name.ToLowerInvariant();

            if (fxName != matName)
            {
                var newMat = ShaderFxManager.instance.GetMaterial(fx);
                if (newMat != null)
                {
                    material = Instantiate(newMat);
                    material.name = newMat.name;
                    renderer.material = material;
                    renderer.material.name = material.name.ToLowerInvariant();
                    matName = material.name.ToLowerInvariant();
                    SetFlip();
                }
            }

            //Debug.Log("Mat name:" + matName + "   fxName:" + fxName);

            if (fxName == matName)
            {
                foreach (var prop in fx.props)
                {
                    if (prop.type.StartsWith("float"))
                    {
                        renderer.material.SetFloat(prop.name, float.Parse(prop.value));
                    }
                    else if (prop.type.StartsWith("rgb"))
                    {
                        Color col;
                        ColorUtility.TryParseHtmlString(prop.value, out col);
                        renderer.material.SetColor(prop.name, col);
                    }
                }
            }
        }
    }
}