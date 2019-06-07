namespace PEBReplicator.PEBComponents
{
    using Tekla.Structures.Model;

    /// <summary>
    /// A class to call a component.
    /// </summary>
    public abstract class ComponentCaller
    {
        protected Component component;
        protected const int number = -10000;
        protected string name;
        protected string attributeFile;

        public int ID { get; protected set; }
        public Component Component => component;
        public string Name => name;
        public string AttributeFile => attributeFile;

        /// <summary>
        /// Set an attribute to the component.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetAttribute(string name, int value)
        {
            component.SetAttribute(name, value);
        }

        /// <summary>
        /// Set an attribute to the component.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetAttribute(string name, double value)
        {
            component.SetAttribute(name, value);
        }

        /// <summary>
        /// Set an attribute to the component.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetAttribute(string name, string value)
        {
            component.SetAttribute(name, value);
        }

        /// <summary>
        /// Inserts the component in the model.
        /// </summary>
        public abstract void Insert();
    }
}
