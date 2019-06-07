using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace PEBReplicator.PEBComponents
{
    using Tekla.Structures.Geometry3d;
    using Tekla.Structures.Model;

    public class PEBMemberComponent : ComponentCaller
    {
        private Point startPoint;
        private Point endPoint;

        public PEBMemberComponent(string attributeFile, Point startPoint, Point endPoint)
        {
            this.name = "Straight Member P.E.B";
            this.attributeFile = attributeFile;
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            component = new Component();
        }
       
        public override void Insert()
        {
            component.Number = -100000;
            component.Name = this.name;
            component.LoadAttributesFromFile(this.attributeFile);

            ComponentInput I = new ComponentInput();

            I.AddOneInputPosition(startPoint);
            I.AddOneInputPosition(endPoint);

            component.SetComponentInput(I);

            component.Insert();
            this.ID = component.Identifier.ID;
        }

    }
}
