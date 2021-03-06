﻿namespace PEBReplicator.Frame_Types
{
    using PEBReplicator.Geometry.FrameGeoShapes;
    using PEBReplicator.PEBComponents;
    using System.Collections.Generic;
    using System.Linq;
    using Tekla.Structures.Geometry3d;
    using Tekla.Structures.Model;
    using static TeklaGeometryExtender.Transformation;
    using TeklaGeometryExtender;

    /// <summary>
    /// A class to draw a single gable frame with all its details.
    /// </summary>
    public class GableFrame
    {
        private Model teklaModel;
        private FramingOptions options;
        private GableFrameGeoShape geometricFrame;
        private List<Component> rightColumnComponents;
        private List<Component> leftColumnComponents;
        private List<Component> leftRafterComponents;
        private List<Component> rightRafterComponents;
        private List<Component> basePlates;
        private List<Component> rightColumnSpliceConnections;
        private List<Component> leftColumnSpliceConnections;
        private List<Component> rightRafterSpliceConnections;
        private List<Component> leftRafterSpliceConnections;
        private List<Component> kneeConnections;
        private Component ridgeSplice;
        private PurlinGeoShape purlinDrawer;

        public PurlinGeoShape PurlinDrawer => purlinDrawer;

        /// <summary>
        /// Instantiate a gable frame by the framing options and gable frame geometric points.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="geometricFrame"></param>
        public GableFrame(FramingOptions options, GableFrameGeoShape geometricFrame)
        {
            this.options = options;
            this.geometricFrame = geometricFrame;
            rightColumnComponents = new List<Component>();
            leftColumnComponents = new List<Component>();
            rightRafterComponents = new List<Component>();
            leftRafterComponents = new List<Component>();
            basePlates = new List<Component>();
            rightColumnSpliceConnections = new List<Component>();
            leftColumnSpliceConnections = new List<Component>();
            rightRafterSpliceConnections = new List<Component>();
            leftRafterSpliceConnections = new List<Component>();
            kneeConnections = new List<Component>();
            teklaModel = new Model();
        }

        /// <summary>
        /// Draw steel lines.
        /// </summary>
        private void DrawSteelLine()
        {
            ControlLine line = new ControlLine(new Tekla.Structures.Geometry3d.LineSegment(geometricFrame.SteelLineLeftBase, geometricFrame.SteelLineColRaftLftIntrsct), true);
            ControlLine line2 = new ControlLine(new Tekla.Structures.Geometry3d.LineSegment(geometricFrame.SteelLineColRaftLftIntrsct, geometricFrame.SteelLineRidgePoint), true);
            ControlLine line3 = new ControlLine(new Tekla.Structures.Geometry3d.LineSegment(geometricFrame.SteelLineRidgePoint, geometricFrame.SteelLineColRafRghtIntrsct), true);
            ControlLine line4 = new ControlLine(new Tekla.Structures.Geometry3d.LineSegment(geometricFrame.SteelLineColRafRghtIntrsct, geometricFrame.SteelLineRightBase), true);
            line.Insert();
            line2.Insert();
            line3.Insert();
            line4.Insert();
        }

        /// <summary>
        /// Draw the entire frame.
        /// </summary>
        public void DrawFrame()
        {
            DrawSteelLine();
            var currentPlane = GetCurrentCorSystem().TransformFromCurrentToGlobal();
            SetPlane();
            CoordinateSystem coordSys = new CoordinateSystem();
            coordSys.Origin = options.Origin;
            coordSys.AxisX = options.FrameVector;
            coordSys.AxisY = new Vector(0, 0, 1000);
            SetPlane(coordSys, TeklaGeometryExtender.ReferencePlane.GLOBAL);
            DrawPEBMembers();
            AssignBasePlate();
            AssignKneeConnections();
            ModifyPEBMembers();
            AssignSpliceConnections();
            AssignRidgeSplice();
            SetPlane(currentPlane, TeklaGeometryExtender.ReferencePlane.GLOBAL);
            AssignPurlinNodes();
        }

        /// <summary>
        /// Assign this frame's purlin nodes.
        /// </summary>
        private void AssignPurlinNodes()
        {
            options.EaveExtendedLeft = true;
            options.EaveExtendedRight = true;

            double leftKneeExtension = 100d;
            double rightKneeExtension = 100d;

            kneeConnections[0].GetAttribute("covPlEdge", ref leftKneeExtension);
            kneeConnections[1].GetAttribute("covPlEdge", ref rightKneeExtension);

            options.LeftEaveLength = leftKneeExtension;
            options.RightEaveLength = rightKneeExtension;

            if (leftKneeExtension == 0)
            {
                options.EaveExtendedLeft = false;
                options.LeftEaveOffset = 0;
            }

            if (rightKneeExtension == 0)
            {
                options.EaveExtendedRight = false;
                options.RightEaveOffset = 0;
            }

            purlinDrawer = new PurlinGeoShape(leftRafterComponents, rightRafterComponents, leftColumnComponents, rightColumnComponents, geometricFrame, options);
        }

        /// <summary>
        /// Modify P.E.B member.
        /// </summary>
        private void ModifyPEBMembers()
        {
            foreach (var component in leftColumnComponents)
            {
                component.Select();
                component.Modify();
            }
            foreach (var component in rightColumnComponents)
            {
                component.Select();
                component.Modify();
            }
            foreach (var component in leftRafterComponents)
            {
                component.Select();
                component.Modify();
            }
            foreach (var component in rightRafterComponents)
            {
                component.Select();
                component.Modify();
            }
        }

        /// <summary>
        /// Draw the PEB member.
        /// </summary>
        private void DrawPEBMembers()
        {
            PEBMemberComponent PEBcomponent;

            for (int i = 0; i < geometricFrame.LeftColumnPts.Count - 1; i++)
            {
                PEBcomponent = new PEBMemberComponent(options.Col1MemberAttribs[i], geometricFrame.LeftColumnPts[i].ToLocal(), geometricFrame.LeftColumnPts[i + 1].ToLocal());
                PEBcomponent.SetAttribute("rotational", 1);
                PEBcomponent.Insert();
                teklaModel.CommitChanges();

                leftColumnComponents.Add(PEBcomponent.Component);
            }

            for (int i = 0; i < geometricFrame.LeftRafterPts.Count - 1; i++)
            {
                PEBcomponent = new PEBMemberComponent(options.Raf1MemberAttribs[i], geometricFrame.LeftRafterPts[i].ToLocal(), geometricFrame.LeftRafterPts[i + 1].ToLocal());
                PEBcomponent.SetAttribute("rotational", 1);
                PEBcomponent.Insert();
                teklaModel.CommitChanges();

                leftRafterComponents.Add(PEBcomponent.Component);
            }

            for (int i = 0; i < geometricFrame.RightRafterPts.Count - 1; i++)
            {
                PEBcomponent = new PEBMemberComponent(options.Raf2MemberAttribs[i], geometricFrame.RightRafterPts[i].ToLocal(), geometricFrame.RightRafterPts[i + 1].ToLocal());
                PEBcomponent.SetAttribute("rotational", 1);
                PEBcomponent.Insert();
                teklaModel.CommitChanges();

                rightRafterComponents.Add(PEBcomponent.Component);
            }

            for (int i = 0; i < geometricFrame.RightColumnPts.Count - 1; i++)
            {
                PEBcomponent = new PEBMemberComponent(options.Col2MemberAttribs[i], geometricFrame.RightColumnPts[i].ToLocal(), geometricFrame.RightColumnPts[i + 1].ToLocal());
                PEBcomponent.SetAttribute("rotational", 3);
                PEBcomponent.Insert();
                teklaModel.CommitChanges();

                rightColumnComponents.Add(PEBcomponent.Component);
            }
        }

        /// <summary>
        /// Assigning base plate connection.
        /// </summary>
        private void AssignBasePlate()
        {
            PEBBplComponent leftColBasePlate = new PEBBplComponent(options.Col1BplAttrib, leftColumnComponents.First());
            leftColBasePlate.Insert();
            teklaModel.CommitChanges();

            basePlates.Add(leftColBasePlate.Component);

            PEBBplComponent rightColBasePlate = new PEBBplComponent(options.Col2BplAttrib, rightColumnComponents.First());
            rightColBasePlate.Insert();
            teklaModel.CommitChanges();

            basePlates.Add(rightColBasePlate.Component);
        }

        /// <summary>
        /// Assigning splice connection.
        /// </summary>
        private void AssignSpliceConnections()
        {
            AssignSpliceForElement(leftColumnComponents, options.Col1SpliceAttribs, out leftColumnSpliceConnections);
            AssignSpliceForElement(rightColumnComponents, options.Col2SpliceAttribs, out rightColumnSpliceConnections);
            AssignSpliceForElement(leftRafterComponents, options.Raf1SpliceAttribs, out leftRafterSpliceConnections);
            AssignSpliceForElement(rightRafterComponents, options.Raf2SpliceAttribs, out rightRafterSpliceConnections);

        }

        /// <summary>
        /// Assigning ridge connection.
        /// </summary>
        private void AssignRidgeSplice()
        {
            PEBSpliceComponent spliceComp = new PEBSpliceComponent(options.RidgeSpliceAttrib, leftRafterComponents.Last(), rightRafterComponents.First());
            spliceComp.SetAttribute("AlignType", 1);
            spliceComp.Insert();
            teklaModel.CommitChanges();
            ridgeSplice = spliceComp.Component;
        }

        /// <summary>
        /// Assign splice connections for each element.
        /// </summary>
        /// <param name="elementComponents"></param>
        /// <param name="attributes"></param>
        /// <param name="splices"></param>
        private void AssignSpliceForElement(List<Component> elementComponents, List<string> attributes, out List<Component> splices)
        {
            splices = new List<Component>();

            if (elementComponents.Count < 2)
            {
                return;
            }

            PEBSpliceComponent spliceComp;

            for (int i = 0; i < elementComponents.Count - 1; i++)
            {
                spliceComp = new PEBSpliceComponent(attributes[i], elementComponents[i], elementComponents[i + 1]);
                spliceComp.Insert();
                teklaModel.CommitChanges();
                splices.Add(spliceComp.Component);
            }
        }

        /// <summary>
        /// Extract relevant parts from components.
        /// </summary>
        /// <param name="Comp"></param>
        /// <param name="outerFlanges"></param>
        /// <param name="webs"></param>
        /// <param name="innerFlanges"></param>
        private void ExtractRelevantParts(Component Comp, out List<Part> outerFlanges, out List<Part> webs, out List<Part> innerFlanges)
        {
            List<Part> partList = new List<Part>();

            var partsEnumerator = Comp.GetChildren();
            while (partsEnumerator.MoveNext())
            {
                if (partsEnumerator.Current is Part)
                {
                    partList.Add(partsEnumerator.Current as Part);
                }
            }

            webs = partList.Where((p) =>
            {
                string retriever = string.Empty;
                p.GetUserProperty("PEBLABEL", ref retriever);
                if (retriever == "WEB")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }).ToList();
            innerFlanges = partList.Where((p) =>
            {
                string retriever = string.Empty;
                p.GetUserProperty("PEBLABEL", ref retriever);
                if (retriever == "INNERFLANGE")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }).ToList();
            outerFlanges = partList.Where((p) =>
            {
                string retriever = string.Empty;
                p.GetUserProperty("PEBLABEL", ref retriever);
                if (retriever == "OUTERFLANGE")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }).ToList();
            webs.Sort((p1, p2) =>
            {
                int serial1 = 0;
                int serial2 = 0;
                p1.GetUserProperty("PEBSERIAL", ref serial1);
                p2.GetUserProperty("PEBSERIAL", ref serial2);
                if (serial1 > serial2)
                {
                    return 1;
                }
                else if (serial1 == serial2)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }

            });
            innerFlanges.Sort((p1, p2) =>
            {
                int serial1 = 0;
                int serial2 = 0;
                p1.GetUserProperty("PEBSERIAL", ref serial1);
                p2.GetUserProperty("PEBSERIAL", ref serial2);
                if (serial1 > serial2)
                {
                    return 1;
                }
                else if (serial1 == serial2)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }

            });
            outerFlanges.Sort((p1, p2) =>
            {
                int serial1 = 0;
                int serial2 = 0;
                p1.GetUserProperty("PEBSERIAL", ref serial1);
                p2.GetUserProperty("PEBSERIAL", ref serial2);
                if (serial1 > serial2)
                {
                    return 1;
                }
                else if (serial1 == serial2)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }

            });
        }


        /// <summary>
        /// Assigning Knee connection.
        /// </summary>
        private void AssignKneeConnections()
        {
            Component column = leftColumnComponents.Last();
            Component rafter = leftRafterComponents.First();
            PEBKneeComponent kneeComponentLeft = new PEBKneeComponent(options.Col1KneeAttrib, column, rafter);
            kneeComponentLeft.Insert();
            kneeConnections.Add(kneeComponentLeft.Component);

            column = rightColumnComponents.Last();
            rafter = rightRafterComponents.Last();
            PEBKneeComponent kneeComponentright = new PEBKneeComponent(options.Col2KneeAttrib, column, rafter);
            kneeComponentright.Insert();
            kneeConnections.Add(kneeComponentright.Component);
        }
    }
}
