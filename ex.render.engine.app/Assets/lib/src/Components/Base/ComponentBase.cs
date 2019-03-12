using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

namespace nexcode.nwcore
{
    [Serializable()]
    public class Prop
    {
        public string name;
        public object value;

        public Prop(string name, object value)
        {
            this.name = name;
            this.value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class ReadOnlyAttribute : PropertyAttribute { }


    public class ComponentProps
    {
        public ComponentProps Clone()
        {
            return this.MemberwiseClone() as ComponentProps;
        }
    }

    public class InputChangedEventArgs : EventArgs
    {
        public int Input { get; private set; }
        public ComponentBase Component { get; private set; }

        public InputChangedEventArgs(int input, ComponentBase component)
        {
            Input = input;
            Component = component;
        }
    }

    public class PropsChangedEventArgs : EventArgs
    {
        public string[] PropsChanged { get; private set; }
        public ComponentProps NewProps { get; private set; }

        public PropsChangedEventArgs(string propChanged, ComponentProps newArgs)
            : base()
        {
            PropsChanged = new string[] { propChanged };
            NewProps = newArgs;
        }

        public PropsChangedEventArgs(string[] propsChanged, ComponentProps newArgs)
            : base()
        {
            PropsChanged = propsChanged;
            NewProps = newArgs;
        }
    }

    public class NotifyEventArgs : EventArgs
    {
        public string EventMessage { get; private set; }

        public NotifyEventArgs(string eventMessage)
        {
            EventMessage = eventMessage;
        }
    }

    public abstract class ComponentBase<TProps> : ComponentBase where TProps : ComponentProps
    {
        public new TProps props;
        public new TProps Props { get { return base.Props as TProps; } protected set { base.Props = value; base.Props = value; } }

        protected override void SetInnerProps(ComponentProps props)
        {
            this.props = props as TProps;
        }

        public override Type PropsType
        {
            get { return typeof(TProps); }
        }

        private void OnValidate()
        {
            //Debug.Log("Validating props in " + name);
            SetProps(props);
        }
    }

    /// <summary>
    /// Base for all components, includes inputs, args and instance name.
    /// </summary>
    public abstract class ComponentBase : MonoBehaviour, IUpdateable
    {
        public string id;

        [SerializeField]
        public ComponentBase[] inputs = new ComponentBase[0];
        public ComponentProps props;
        public ComponentProps Props { get { return props; } protected set { props = value; SetInnerProps(value); } }

        public abstract Type PropsType { get; }
        public Texture texture;
        public virtual bool HasTexture { get { return false; } }

        public float volume;
        public virtual bool HasAudio { get { return false; } }

        public event EventHandler<InputChangedEventArgs> OnInputChanged;
        public event EventHandler<PropsChangedEventArgs> OnPropsChanged;
        public event EventHandler<NotifyEventArgs> OnNotify;

        public bool IsUpdated { get; protected set; }

        public ComponentBase[] GetInputs()
        {
            return inputs;
        }

        public void SetInput(ComponentBase input, int inputIndex=0)
        {
            if (inputs.Length <= inputIndex)
            {
                var newInputs = new ComponentBase[inputIndex + 1];
                for (int i = 0; i < inputs.Length; ++i)
                    newInputs[i] = inputs[i];

                newInputs[inputIndex] = input;
                inputs = newInputs;
            }
            else
                inputs[inputIndex] = input;

            if (OnInputChanged != null)
                OnInputChanged(this, new InputChangedEventArgs(inputIndex, input));
        }

        public void ShiftInputs()
        {
            if (inputs.Length > 0)
                inputs = inputs.Skip(1).ToArray();

            for (int i = 0; i < inputs.Length; ++i)
                SetInput(inputs[i], i);
        }

        public void RemoveAndShiftInputs(int pos)
        {
            var tmpList = new List<ComponentBase>(inputs);
            tmpList.RemoveAt(pos);
            inputs = tmpList.ToArray();

            for (int i = pos; i < inputs.Length; ++i)
                SetInput(inputs[i], i);
        }

        protected virtual void SetInnerProps(ComponentProps props)
        {
            this.props = props;
        }

        public void SetProps(ComponentProps props)
        {
            Props = props;
            if (OnPropsChanged != null)
                OnPropsChanged(this, new PropsChangedEventArgs(new string[] { "all" }, Props));
        }

        public void SetProps(params Prop [] props)
        {
            // Check with args have changed
            var propsChanged = props.Select(p => p.name).ToArray();
            foreach (var p in props)
            {
                SetPropValue(p.name, p.value);
            }

            if (OnPropsChanged != null)
                OnPropsChanged(this, new PropsChangedEventArgs(propsChanged, Props));
        }

        public void SetProp(string name, object value)
        {
            SetPropValue(name, value);

            if (OnPropsChanged != null)
                OnPropsChanged(this, new PropsChangedEventArgs(name, Props));
        }

        private void SetPropValue(string name, object value)
        {
            var arg = Props.GetType().GetFields().Where(x => x.IsPublic && name.ToLower() == x.Name.ToLowerInvariant()).FirstOrDefault();

            if (arg == null)
                throw new Exception("Prop value " + name + " not found");

            arg.SetValue(Props, value);
        }

        public ComponentProps GetProps()
        {
            return Props;
        }

        protected void Notify(string notifyMessage)
        {
            if (OnNotify != null)
                OnNotify(this, new NotifyEventArgs(notifyMessage));
        }

        public virtual void OnUpdate()
        {
        }

        public void Update()
        {
            IsUpdated = true;

            foreach (var input in inputs)
            {
                if (input && !input.IsUpdated)
                    input.Update();
            }

            OnUpdate();
        }

        protected void LastUpdate()
        {
            IsUpdated = false;
        }

    }
}