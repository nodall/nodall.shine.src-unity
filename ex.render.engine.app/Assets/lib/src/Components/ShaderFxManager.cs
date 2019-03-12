using Newtonsoft.Json;
using nexcode.network.http.rest;
using nexcode.nwcore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ShaderFxManager : MonoBehaviour {

    public static ShaderFxManager instance;

    public Material[] materials;

    public List<FxDef> shadersDef;

    [TextArea(5, 20)]
    public string jsonShaderDef;

    [Serializable]
    public class FxDef
    {
        public string name;
        public string displayName;
        public List<FxDefProp> props;
    }

    [Serializable]
    public class FxDefProp
    {
        public string name;
        public string displayName;
        public string type;
        public string value;
    }

    // Use this for initialization

    void Awake()
    {
        instance = this;

        try
        {
            materials = Resources.LoadAll<Material>("FX");
            jsonShaderDef = File.ReadAllText(Application.streamingAssetsPath + "/fx-def.json");

            shadersDef = JsonConvert.DeserializeObject<List<FxDef>>(File.ReadAllText(Application.streamingAssetsPath + "/fx-def.json"));
            jsonShaderDef = JsonConvert.SerializeObject(shadersDef, Formatting.Indented);

            foreach (var s in materials)
            {
                if (shadersDef.Count(sd => sd.name == s.name) == 0)
                    Debug.LogWarning("Shader definition (in fx-def.json) not found for shader: " + s.name);
            }

            foreach (var s in shadersDef)
            {
                if (materials.Count(sh => sh.name != s.name) == 0)
                    Debug.LogWarning("Shader not found for shader definition: " + s.name);
            }
        } catch (Exception e)
        {
            Debug.LogError("[ShaderFxManager] Could not load fx-def in streaming assets.");
            shadersDef = new List<FxDef>();
        }

        Debug.Log("<b><color=white>[ShaderFxManager] Defining REST FX</color></b>");

        NWCoreBase.rest.Define("fx")
            .On("getState", "List all shaders", (msg) =>
            {
                Debug.LogWarning("Request list of shaders");
                return new RestResponse(shadersDef);
            });
    }

    public Material GetMaterial(FxDef fx)
    {
        Debug.LogWarning("Getting material " + fx.name);
        var mat = materials.FirstOrDefault(m => m.name == fx.name);
        Debug.LogWarning("Material got " + mat != null ? mat.name : "null");
        return mat;
    }
}
