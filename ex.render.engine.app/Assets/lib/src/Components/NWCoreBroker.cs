using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using nexcode.network.http.websocket;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WanzyeeStudio;
using nexcode.json;
using nexcode.network.http.rest;
using System.Linq;
using System.Threading;

namespace nexcode.nwcore
{

    public static class MessageTokenExtensions
    {
        public static object ToTypedObject(this JToken token)
        {
            if (token["_type"] == null)
                throw new Exception("Missing _type in ToTypedObject deserialization.");

            var type = Type.GetType(token["_type"].ToString());
            if (token["_value"] != null)
                return token["_value"].ToObject(type);
                
            return token.ToObject(type);
        }

        public static string SerializeTyped(this object obj)
        {
            var str = JsonConvert.SerializeObject(obj);
            var jobj = JObject.Parse(str);
            jobj["_type"] = obj.GetType().ToString();
            return jobj.ToString();
        }

        public static object AsTypedObject(this object obj)
        {
            var str = JsonConvert.SerializeObject(obj);
            JToken jobj = null;
            try
            {
                jobj = JObject.Parse(str);
            }
            catch (Exception e)
            {
                jobj = new JObject();
                jobj["_value"] = str;
            }
            jobj["_type"] = obj.GetType().ToString();
            return JsonConvert.DeserializeObject(jobj.ToString());
        }
    }



    public class NWCoreBroker : MonoBehaviour
    {
        // Use this for initialization
        void Awake()
        {
            //var rest = NWCoreBase.hub.Client.Contracts.Get<RestClientContract>();

            NWCoreBase.rest.Define("nwcore")
                .On("getProps", "Get Props of a component by id", (msg) =>
                {
                    var id = msg["id"].ToString();
                    var component = ComponentManager.GetById(id);

                    Debug.Log("[RestRequest:nwcore] getProps");

                    return new RestResponse(new
                    {
                        id = id,
                        props = component.props.AsTypedObject()
                    });
                })
                .On("listAll", "List all components", (msg) =>
                {
                    string type = null;
                    if (msg["type"] != null)
                        type = msg["type"].ToString();

                    return new RestResponse(ComponentManager.GetAllComponents(type));
                })
                .On("listAllWithProps", "List all components with its props", (msg) =>
                {
                    string type = null;
                    if (msg["type"] != null)
                        type = msg["type"].ToString();

                    Debug.Log("Listing all props of type: " + type);

                    var all = ComponentManager.GetAllComponents(type);
                    var list = new List<object>();

                    foreach (var cid in all)
                    {
                        var comp = ComponentManager.GetById(cid.id);
                        var props = comp.Props;
                        Debug.Log(props);

                        string[] inputs = new string[0];
                        try
                        {
                            inputs = comp.inputs.Select((v) => v.id).ToArray();
                        }
                        catch (Exception e) { }


                        list.Add(new
                        {
                            cid._type,
                            cid.id,
                            inputs = inputs,
                            props = props.AsTypedObject()
                        });
                    }

                    return new RestResponse(new { components = list });

                })
                .On("create", "Creates a component. Returns new created component.", (msg) =>
                {
                    var type = msg["type"].ToString();
                    object props = null;
                    if (msg["props"] != null)
                        props = msg["props"].ToTypedObject();

                    //Debug.Log(JsonConvert.SerializeObject(props));

                   return RunOnMainThreadAndWait(() =>
                   {
                       var comp = ComponentManager.New(Type.GetType(type), null); // props as ComponentProps);
                       return new RestResponse(new
                       {
                           _type = type,
                           id = comp.name,
                           inputs = new List<string>(),
                           props = comp.props.AsTypedObject()
                       });
                   });

                })
                .On("delete", "Deletes a component.", (msg) =>
                {
                    var id = msg["id"].ToString();

                    return RunOnMainThreadAndWait(() =>
                    {
                        ComponentManager.DestroyComponent(id);
                        return new RestResponse(new { });
                    });
                });


            NWCoreBase.hub.Subscribe("nwcore")
                .On("create", (msg) => Create(msg))
                .On("delete", (msg) =>
                {
                    var id = msg["id"].ToString();
                    ComponentManager.DestroyComponent(id);
                })
                .On("setInput", (msg) => SetInput(msg))
                .On("setProp", (msg) => SetProp(msg))
                .On("setProps", (msg) => SetProps(msg))
                .On("reqListAllSurfaces", (msg) =>
                {
                    string type = "nexcode.nwcore.QuadSurfaceComponent";
                    Debug.Log("Listing all props of type: " + type);

                    var all = ComponentManager.GetAllComponents(type);
                    var list = new List<object>();

                    foreach (var cid in all)
                    {
                        var comp = ComponentManager.GetById(cid.id);
                        var props = comp.Props;
                        //Debug.Log(props);

                        list.Add(new
                        {
                            cid._type,
                            cid.id,
                            inputs = comp.inputs.Select((v) => v.id).ToArray(),
                            props = props.AsTypedObject()
                        });
                    }


                    NWCoreBase.hub.Publish("nwcore", "listAllSurfaces",  new { components = list } );
                });


            ComponentManager.OnComponentCreate += (s, e) =>
            {
                Debug.Log("[ComponentManagerBroker] OnComponentCreate " + e.Component.name);

                e.Component.OnInputChanged += Component_OnInputChanged;
                e.Component.OnPropsChanged += Component_OnPropsChanged;
                e.Component.OnNotify += Component_OnNotify;

                NWCoreBase.hub.Publish("nwcore", "created", new
                {
                    id = e.Component.name,
                    type = e.Component.GetType().ToString(),
                    props = e.Component.props == null ? null : e.Component.props.AsTypedObject()
                });

            };

            ComponentManager.OnComponentDestroy += (s, e) =>
            {
                Debug.Log("[ComponentManagerBroker] OnComponentDestroy " + e.Component.name);

                NWCoreBase.hub.Publish("nwcore", "destroyed", new
                {
                    id = e.Component.name,
                    type = e.Component.GetType().ToString()
                });

            };
        }

        private void Component_OnNotify(object sender, NotifyEventArgs e)
        {
            NWCoreBase.hub.Publish("nwcore", "notify", new
            {
                id = (sender as ComponentBase).name,
                message = e.EventMessage
            });
        }

        private void Component_OnPropsChanged(object sender, PropsChangedEventArgs e)
        {
            NWCoreBase.hub.Publish("nwcore", "propsChanged", new
            {
                id = (sender as ComponentBase).name,
                propsChanged = e.PropsChanged,
                props = (sender as ComponentBase).props.AsTypedObject()                
            });
        }

        private void Component_OnInputChanged(object sender, InputChangedEventArgs e)
        {
            NWCoreBase.hub.Publish("nwcore", "inputsChanged", new
            {
                id = (sender as ComponentBase).name,
                input = e.Input,
                inputComponentId = e.Component.name
            });
        }

        public void Create(JToken msg)
        {
            var type = msg["type"].ToString();
            var id = msg["id"].ToString();
            var props = msg["props"].ToTypedObject();

            Debug.Log(JsonConvert.SerializeObject(props));

            ComponentManager.New(Type.GetType(type), id, props as ComponentProps);
        }

        public void SetInput(JToken msg)
        {
            Debug.Log(msg.ToString());
            var id = msg["id"].ToString();
            var input = msg.Value<int>("input");
            var linkId = msg["linkId"].ToString();
            var component = ComponentManager.GetById(id);
            var linkComponent = ComponentManager.GetById(linkId);

            if (component != null)
                component.SetInput(linkComponent, input);
        }

        public void SetProp(JToken msg)
        {
            //Debug.Log(msg.ToString());
            var id = msg["id"].ToString();
            var name = msg.Value<string>("name");
            var value = msg["value"].ToTypedObject();
            var component = ComponentManager.GetById(id);

            if (component != null)
                component.SetProp(name, value);
        }

        public void SetProps(JToken msg)
        {
            var id = msg["id"].ToString();
            var props = msg["props"].ToTypedObject() as ComponentProps;
            var component = ComponentManager.GetById(id);

            if (component != null)
                component.SetProps(props);
        }


        // TODO Find better solution

        private Func<RestResponse> _restToExecute = null;
        private RestResponse _restResponse;

        public RestResponse RunOnMainThreadAndWait(Func<RestResponse> func)
        {
            while (_restToExecute != null)
                Thread.Sleep(1);

            _restResponse = null;
            _restToExecute = func;

            while (_restResponse == null)
                Thread.Sleep(1);

            return _restResponse;
        }

        private void Update()
        {
            if (_restToExecute != null)
            {
                _restResponse = _restToExecute.Invoke();
                _restToExecute = null;
            }
        }

    }
}