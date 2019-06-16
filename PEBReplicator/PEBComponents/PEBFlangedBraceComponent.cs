namespace PEBReplicator.PEBComponents
{
    using Tekla.Structures.Model;

    class PEBFlangedBraceComponent : ComponentCaller
    {
        private Component preEngineeredComponent;
        private Part firstPurlin;
        private Part secondPurlin;

        public PEBFlangedBraceComponent(string attributeFile, Part firstPurlin, Part secondPurlin, Component preEngineeredComponent)
        {
            this.name = "Base Plate P.E.B";
            this.attributeFile = attributeFile;
            this.preEngineeredComponent = preEngineeredComponent;
            this.firstPurlin = firstPurlin;
            this.secondPurlin = secondPurlin;
            component = new Component();
        }

        public override void Insert()
        {
            component.Number = -100000;
            component.Name = this.name;
            component.LoadAttributesFromFile(this.attributeFile);

            ComponentInput I = new ComponentInput();

            I.AddInputObject(firstPurlin);
            I.AddInputObject(secondPurlin);
            I.AddInputObject(component);

            component.SetComponentInput(I);

            component.Insert();
            this.ID = component.Identifier.ID;
        }
    }
}
