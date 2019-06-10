namespace PEBReplicator.PEBComponents
{
    using Tekla.Structures.Model;

    class PEBKneeComponent : ComponentCaller
    {
        private Component column;

        private Component rafter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeFile"></param>
        /// <param name="column"></param>
        /// <param name="rafter"></param>
        public PEBKneeComponent(string attributeFile, Component column,Component rafter)
        {
            this.name = "Knee Connection P.E.B";
            this.attributeFile = attributeFile;
            this.column = column;
            this.rafter = rafter;
            component = new Component();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Insert()
        {
            component.Number = -100000;
            component.Name = this.name;
            component.LoadAttributesFromFile(this.attributeFile);

            ComponentInput I = new ComponentInput();

            I.AddInputObject(column);
            I.AddInputObject(rafter);


            component.SetComponentInput(I);

            component.Insert();
            this.ID = component.Identifier.ID;
        }
    }
}
