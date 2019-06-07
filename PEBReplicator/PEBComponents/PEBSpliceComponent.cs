namespace PEBReplicator.PEBComponents
{
    using System.Collections.Generic;
    using Tekla.Structures;
    using Tekla.Structures.Model;

    /// <summary>
    /// Splice connection component caller.
    /// </summary>
    class PEBSpliceComponent : ComponentCaller
    {
        private List<Part> partIdentifiers;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeFile"></param>
        /// <param name="firstMember"></param>
        /// <param name="secondMember"></param>
        public PEBSpliceComponent(string attributeFile,List<Part> partIdentifiers)
        {
            this.name = "P.E.B Splice Connection";
            this.attributeFile = attributeFile;
            this.partIdentifiers = partIdentifiers;
            component = new Component();
        }


        public override void Insert()
        {
            component.Number = -100000;
            component.Name = this.name;
            component.LoadAttributesFromFile(this.attributeFile);

            ComponentInput I = new ComponentInput();

            foreach (var part in partIdentifiers)
            {
                I.AddInputObject(part);
            }

            component.SetComponentInput(I);

            component.Insert();
            this.ID = component.Identifier.ID;
        }
    }
}
