namespace PEBReplicator.Frame_Types
{
    using PEBReplicator.Geometry.FrameGeoShapes;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Tekla.Structures.Geometry3d;
    using Tekla.Structures.Model;
    using TeklaGeometryExtender;
    using static TeklaGeometryExtender.Transformation;
    using static TeklaGeometryExtender.XIntersection;

    /// <summary>
    /// 
    /// </summary>
    public class PurlinGeoShape
    {
        private FramingOptions options;
        private GableFrameGeoShape shape;

        private List<Part> leftRafterParts;
        private List<Part> rightRafterParts;
        private List<Part> leftColParts;
        private List<Part> rightColParts;

        private List<Component> leftRafterComps;
        private List<Component> rightRafterComps;
        private List<Component> leftColComps;
        private List<Component> rightColComps;

        private Point leftMostPurlinRef;
        private Point rightMostPurlinRef;
        private Point leftTopOfSteelPoint;
        private Point rightTopOfSteelPoint;

        private double inclinedLeftEaveLength;
        private double inclinedRightEaveLength;

        private Vector leftPerpVector;
        private Vector rightPerpVector;
        private Vector leftRafterVector;
        private Vector rightRafterVector;

        private double inclinedLeftEaveOffset;
        private double inclinedRightEaveOffset;

        private double biggestLeftRafterThickness;
        private double biggestRightRafterThickness;
        private double biggestRightColThickness;
        private double biggestLeftColThickness;

        private List<Point> leftColPts;
        private List<Point> leftRfterPts;
        private List<Point> rightRfterPts;
        private List<Point> rightColPts;

        private List<(Point, Component)> leftColValuePair;
        private List<(Point, Component)> leftRfterValuePair;
        private List<(Point, Component)> rightRfterValuePair;
        private List<(Point, Component)> rightColValuePair;

        public List<(Point point, Component component)> LeftColValuePair => leftColValuePair;
        public List<(Point point, Component component)> LeftRfterValuePair => leftRfterValuePair;
        public List<(Point point, Component component)> RightRfterValuePair => rightRfterValuePair;
        public List<(Point point, Component component)> RightColValuePair => rightColValuePair;

        public List<Point> LeftColPts => leftColPts;
        public List<Point> LeftRfterPts => leftRfterPts;
        public List<Point> RightRfterPts => rightRfterPts;
        public List<Point> RightColPts => rightColPts;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="leftRafter"></param>
        /// <param name="rightRafter"></param>
        /// <param name="leftCol"></param>
        /// <param name="rightCol"></param>
        /// <param name="geoShape"></param>
        public PurlinGeoShape(List<Component> leftRafterComps, List<Component> rightRafterComps, List<Component> leftColComps, List<Component> rightColComps, GableFrameGeoShape geoShape, FramingOptions options)
        {
            this.shape = geoShape;

            this.leftRafterComps = leftRafterComps;
            this.rightRafterComps = rightRafterComps;
            this.leftColComps = leftColComps;
            this.rightColComps = rightColComps;

            this.options = options;

            leftColPts = new List<Point>();
            leftRfterPts = new List<Point>();
            rightRfterPts = new List<Point>();
            rightColPts = new List<Point>();

            leftColValuePair = new List<(Point, Component)>();
            leftRfterValuePair = new List<(Point, Component)>();
            rightRfterValuePair = new List<(Point, Component)>();
            rightColValuePair = new List<(Point, Component)>();


            leftRafterParts = new List<Part>();
            rightRafterParts = new List<Part>();
            leftColParts = new List<Part>();
            rightColParts = new List<Part>();

            AssignPartList();
            InitializeGeometricValues();
            AssignRafterData();
        }

        /// <summary>
        /// 
        /// </summary>
        private void AssignPartList()
        {
            foreach (var component in leftRafterComps)
            {
                var list = GetPartsFromComponent(component).Where((p) =>
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
                leftRafterParts.AddRange(list);
            }

            foreach (var component in rightRafterComps)
            {
                var list = GetPartsFromComponent(component).Where((p) =>
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
                rightRafterParts.AddRange(list);
            }

            foreach (var component in leftColComps)
            {
                var list = GetPartsFromComponent(component).Where((p) =>
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
                leftColParts.AddRange(list);
            }

            foreach (var component in rightColComps)
            {
                var list = GetPartsFromComponent(component).Where((p) =>
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
                rightColParts.AddRange(list);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CalculateBiggestThickness()
        {
            biggestLeftRafterThickness = leftRafterParts.Select(p => p.GetProperties().Thickness).Max();
            biggestRightRafterThickness = rightRafterParts.Select(p => p.GetProperties().Thickness).Max();
            biggestLeftColThickness = leftColParts.Select(p => p.GetProperties().Thickness).Max();
            biggestRightColThickness = rightColParts.Select(p => p.GetProperties().Thickness).Max();
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeGeometricValues()
        {
            CalculateBiggestThickness();

            #region Left Rafter
            leftRafterVector = new Vector(shape.FrameRidgePoint - shape.FrameColRftLftIntrsct).GetNormal();

            Vector upDirection = new Vector(0, 0, 1);
            Vector leftRafterInverted = leftRafterVector * -1;
            Vector tempPerp = leftRafterInverted.Cross(upDirection);
            leftPerpVector = leftRafterInverted.Cross(tempPerp).GetNormal();

            inclinedLeftEaveLength = options.LeftEaveLength / Math.Cos(shape.SlopeAngleRad);
            inclinedLeftEaveOffset = options.LeftEaveOffset / Math.Cos(shape.SlopeAngleRad);

            leftTopOfSteelPoint = shape.FrameColRftLftIntrsct.Translate(leftPerpVector * -1 * biggestLeftRafterThickness);

            leftMostPurlinRef = leftTopOfSteelPoint.Translate(-1 * leftRafterVector * inclinedLeftEaveLength);
            #endregion

            #region Right Rafter
            rightRafterVector = new Vector(shape.FrameRidgePoint - shape.FrameColRftRghtIntrsct).GetNormal();

            Vector rightRafterInverted = leftRafterVector * -1;
            Vector tempRightPerp = rightRafterInverted.Cross(upDirection);
            rightPerpVector = rightRafterInverted.Cross(tempRightPerp).GetNormal();

            inclinedRightEaveLength = options.RightEaveOffset / Math.Cos(shape.SlopeAngleRad);
            inclinedRightEaveOffset = options.RightEaveOffset / Math.Cos(shape.SlopeAngleRad);

            rightTopOfSteelPoint = shape.FrameColRftRghtIntrsct.Translate(rightPerpVector * -1 * biggestRightRafterThickness);

            rightMostPurlinRef = rightTopOfSteelPoint.Translate(-1 * rightRafterVector * inclinedLeftEaveLength);
            #endregion

            #region Left Column
            #endregion

            #region Right Column
            #endregion
        }


        /// <summary>
        /// 
        /// </summary>
        private void AssignRafterData()
        {
            AssignLeftRafterPoints();
            AssignLeftRafterTuple();
            AssignRightRafterPoints();
            AssignRightRafterTuple();
        }



        /// <summary>
        /// 
        /// </summary>
        private void AssignLeftRafterPoints()
        {
            Point firstPurlinPoint = leftMostPurlinRef.Translate(inclinedLeftEaveOffset * leftRafterVector);

            leftRfterPts.Add(firstPurlinPoint);

            Point secondPurlinPoint = leftTopOfSteelPoint;

            if (options.AtCoverLeft)
            {
                leftRfterPts.Add(secondPurlinPoint);
                leftRfterPts = leftRfterPts.Distinct().ToList();
            }

            Point firstPoint = leftRfterPts.Last();

            double inclinedADistance = (options.HorizontalDistance / Math.Cos(shape.SlopeAngleRad));
            double purlinSpacingTotalLength = Distance.PointToPoint(firstPoint, shape.FrameRidgePoint);
            int numberOfOffsets = (int)(purlinSpacingTotalLength / inclinedADistance);

            for (int i = 0; i < numberOfOffsets; i++)
            {
                firstPoint = firstPoint.Translate(inclinedADistance * leftRafterVector);
                leftRfterPts.Add(firstPoint);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void AssignLeftRafterTuple()
        {
            Line line;
            int index;
            foreach (var point in leftRfterPts)
            {
                line = new Line(point, leftPerpVector);
                index = leftRafterParts.FindIndex(p => Intersect(p.GetSolid(), line).Count > 0);
                leftRfterValuePair.Add((point, leftRafterComps[index]));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void AssignRightRafterTuple()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        private void AssignRightRafterPoints()
        {
            Point firstPurlinPoint = rightMostPurlinRef.Translate(inclinedRightEaveOffset * rightRafterVector);
            rightRfterPts.Add(firstPurlinPoint);

            Point secondPurlinPoint = rightTopOfSteelPoint;

            if (options.AtCoverLeft)
            {
                rightRfterPts.Add(secondPurlinPoint);
                rightRfterPts = rightRfterPts.Distinct().ToList();
            }

            Point firstPoint = rightRfterPts.Last();
            double inclinedADistance = (options.HorizontalDistance / Math.Cos(shape.SlopeAngleRad));
            double purlinSpacingTotalLength = Distance.PointToPoint(firstPoint, shape.FrameRidgePoint);
            int numberOfOffsets = (int)(purlinSpacingTotalLength / inclinedADistance);
            for (int i = 0; i < numberOfOffsets; i++)
            {
                firstPoint = firstPoint.Translate(inclinedADistance * rightRafterVector);
                rightRfterPts.Add(firstPoint);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comp"></param>
        /// <returns></returns>
        private List<Part> GetPartsFromComponent(Component comp)
        {
            List<Part> partList = new List<Part>();

            var partsEnumerator = comp.GetChildren();
            while (partsEnumerator.MoveNext())
            {
                if (partsEnumerator.Current is Part)
                {
                    partList.Add(partsEnumerator.Current as Part);
                }
            }
            return partList;
        }

        /// <summary>
        /// 
        /// </summary>
        private void AssignColumnData()
        {
            AssignLeftColPoints();
            AssignLeftColTuple();
            AssignRightColPoints();
            AssignRightColTuple();
        }

        /// <summary>
        /// 
        /// </summary>
        private void AssignLeftColPoints() { }
        /// <summary>
        /// 
        /// </summary>
        private void AssignLeftColTuple() { }
        /// <summary>
        /// 
        /// </summary>
        private void AssignRightColPoints() { }
        /// <summary>
        /// 
        /// </summary>
        private void AssignRightColTuple() { }
    }
}

