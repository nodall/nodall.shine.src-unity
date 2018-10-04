using UnityEngine;
using System.Collections;
using nexcode.nwcore;
using Components;
using Newtonsoft.Json.Linq;
using System.IO;
using RenderHeads.Media.AVProLiveCamera;

public class NWCore : NWCoreBase {

    ShineMediaCompositor shineCompositor;

    private void Awake()
    {
        // Register components
        ComponentManager.Register<IVideoPlayer>(typeof(AvproVideoPlayer));
        //ComponentFactory.Register<IBrowser>(typeof(ZFBrowser));

        Debug.Log("NWCore::Init");
        Initialise();

        var width = Settings.Root["Shine"]["MediaCompositor"].Value<int>("width");
        var height = Settings.Root["Shine"]["MediaCompositor"].Value<int>("height");

        shineCompositor = ComponentManager.New<ShineMediaCompositor>(new MediaCompositorProps { width = width, height = height });

        Connect();
    }

    private void Start()
    {
        // Create surface from file
        var quadSurface = ComponentManager.New<QuadSurfaceComponent>(new QuadSurfaceComponentProps { /*bezier = new Bezier2D(new Rectangle2D())*/   });
        var surfaceFile = "surface.json";

        try
        {
            var propsJson = JObject.Parse(File.ReadAllText(surfaceFile));
            Debug.Log("Surface: " + propsJson);
            var props = propsJson.ToTypedObject() as QuadSurfaceComponentProps;
            if (props != null)
                quadSurface.SetProps(props);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error while trying to load surface props.");
        }

        quadSurface.OnPropsChanged += (s, e) =>
        {
            try
            {
                File.WriteAllText(surfaceFile, quadSurface.props.SerializeTyped());
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error while trying to store surface props.");
            }
        };

        quadSurface.SetInput(shineCompositor);
    }
}
