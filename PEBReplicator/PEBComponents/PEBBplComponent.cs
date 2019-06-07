namespace PEBReplicator.PEBComponents
{
    using Tekla.Structures.Model;

    /// <summary>
    /// Base plate component caller.
    /// </summary>
    class PEBBplComponent : ComponentCaller
    {
        private Component column;

        public PEBBplComponent(string attributeFile, Component column)
        {
            this.name = "Base Plate P.E.B";
            this.attributeFile = attributeFile;
            this.column = column;
            component = new Component();
        }

        public override void Insert()
        {
            component.Number = -100000;
            component.Name = this.name;
            component.LoadAttributesFromFile(this.attributeFile);

            ComponentInput I = new ComponentInput();

            I.AddInputObject(column);

            component.SetComponentInput(I);

            component.Insert();
            this.ID = component.Identifier.ID;
        }
    }
}
