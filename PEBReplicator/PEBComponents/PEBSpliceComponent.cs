﻿namespace PEBReplicator.PEBComponents
{
    using Tekla.Structures.Model;

    /// <summary>
    /// Splice connection component caller.
    /// </summary>
    class PEBSpliceComponent : ComponentCaller
    {
        private Component firstMember;
        private Component secondMember;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeFile"></param>
        /// <param name="firstMember"></param>
        /// <param name="secondMember"></param>
        public PEBSpliceComponent(string attributeFile, Component firstMember, Component secondMember)
        {
            this.name = "P.E.B Splice Connection";
            this.attributeFile = attributeFile;
            this.firstMember = firstMember;
            this.secondMember = secondMember;
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

            I.AddInputObject(firstMember);
            I.AddInputObject(secondMember);

            component.SetComponentInput(I);

            component.Insert();
            this.ID = component.Identifier.ID;
        }
    }
}
