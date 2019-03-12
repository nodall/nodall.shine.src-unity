using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace nexcode.nwcore
{
    public class ComponentManager
    {
        class ParentForType
        {
            public Type type;
            public GameObject parent;
        }

        public class TypeAndId
        {
            public string _type;
            public string id;
        }

        public class ComponentEventArgs : EventArgs
        {
            public ComponentBase Component { get; private set; }

            public ComponentEventArgs(ComponentBase component)
                : base()
            {
                Component = component;
            }
        }

        static Dictionary<Type, Type> components = new Dictionary<Type, Type>();
        static List<ParentForType> componentsPath = new List<ParentForType>();
        static Dictionary<string, int> instanceIds = new Dictionary<string, int>();

        static Dictionary<string, ComponentBase> componentInstances = new Dictionary<string, ComponentBase>();

        static public event EventHandler<ComponentEventArgs> OnComponentCreate;
        static public event EventHandler<ComponentEventArgs> OnComponentDestroy;

        static ComponentManager()
        {
        }

        public static void DefineParentFor<T>(GameObject parentObject) where T: class
        {
            var type = typeof(T);
            componentsPath.Add(new ParentForType { type = type, parent = parentObject });
        }

        public static void Register<T>(Type componentType) where T: class
        {
            var type = typeof(T);
            if (!components.ContainsKey(type))
            {
                components.Add(type, componentType);
            }
        }

        public static T Add<T>(GameObject obj) where T: class
        {
            var type = typeof(T);
            if (components.ContainsKey(type))
            {
                var typeToInstatiante = components[type];
                return obj.AddComponent(typeToInstatiante) as T;
            }
            return default(T);
        }

        public static ComponentBase New(Type type, ComponentProps props)
        {
            return New(type, GetInstanceId(type.Name), props);
        }

        public static ComponentBase New(Type type, string name, ComponentProps props)
        {
            var obj = new GameObject(name);
            var res = obj.AddComponent(type) as ComponentBase;
            res.id = name;

            componentInstances.Add(obj.name, res);

            if (props == null)
                props = Activator.CreateInstance(res.PropsType) as ComponentProps;

            if (res.PropsType != props.GetType())
                throw new Exception("Props type mismatch");

            foreach (var item in componentsPath)
            {
                if (item.type.IsInstanceOfType(res))
                {
                    obj.transform.SetParent(item.parent.transform, false);
                }
            }

            res.SetProps(props);

            if (OnComponentCreate != null)
                OnComponentCreate(res, new ComponentEventArgs(res));

            return res as ComponentBase;
        }


        public static T New<T>() where T : ComponentBase
        {
            return New<T>(new ComponentProps { });
        }

        public static T New<T>(ComponentProps props) where T : ComponentBase
        {
            var type = typeof(T);
            var obj = new GameObject(GetInstanceId(type.Name));
            var res = obj.AddComponent<T>();
            res.id = obj.name;

            componentInstances.Add(obj.name, res);

            if (res.PropsType != props.GetType())
                throw new Exception("Props type mismatch");

            foreach (var item in componentsPath)
            {
                if (item.type.IsInstanceOfType(res))
                {
                    obj.transform.SetParent(item.parent.transform, false);
                }
            }

            res.SetProps(props);

            if (OnComponentCreate != null)
                OnComponentCreate(res, new ComponentEventArgs(res));

            return res;
        }

        public static T New<T>(string name, ComponentProps props) where T : ComponentBase
        {
            var type = typeof(T);
            var obj = new GameObject(name);
            var res = obj.AddComponent<T>();
            res.id = name;

            componentInstances.Add(obj.name, res);

            if (res.PropsType != props.GetType())
                throw new Exception("Props type mismatch");

            foreach (var item in componentsPath)
            {
                if (item.type.IsInstanceOfType(res))
                {
                    obj.transform.SetParent(item.parent.transform, false);
                }
            }


            res.SetProps(props);

            if (res.Props == null || res.Props.GetType() != props.GetType())
                throw new Exception("Arg types error");

            if (OnComponentCreate != null)
                OnComponentCreate(res, new ComponentEventArgs(res));

            return res;
        }

        public static ComponentBase GetById(string id)
        {
            if (componentInstances.ContainsKey(id))
                return componentInstances[id];
            else return null;
        }

        public static void DestroyComponent(ComponentBase c)
        {
            if (c != null)
            {
                GameObject.DestroyObject(c.gameObject);
                componentInstances.Remove(c.name);
                OnComponentDestroy.Invoke(c, new ComponentEventArgs(c));
            }
        }

        public static void DestroyComponent(string id)
        {
            var c = GetById(id);
            if (c != null)
                DestroyComponent(c);
        }

        protected static string GetInstanceId(string type)
        {
            if (instanceIds.ContainsKey(type))
            {
                ++instanceIds[type];
                return type + " " + instanceIds[type] + "-"+ DateTime.Now.Ticks;
            }
            else
            {
                instanceIds.Add(type, 0);
                return type + " 0" + "-"+ DateTime.Now.Ticks;
            }
        }

        public static List<TypeAndId> GetAllComponents(string type=null)
        {
            if (type == null)
                return componentInstances.Select(x => new TypeAndId { id = x.Key, _type = x.Value.GetType().ToString() }).ToList();
            else
                return componentInstances.Where((x) => x.Value.GetType().ToString() == type).Select(x => new TypeAndId { id = x.Key, _type = x.Value.GetType().ToString() }).ToList();
        }

        public static IEnumerable<T> GetAll<T>() where T : ComponentBase
        {
            return componentInstances.Where(x => x.Value.GetType() == typeof(T)).Select(x => x.Value as T);
        }

    }

}
