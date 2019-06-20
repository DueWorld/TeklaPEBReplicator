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
    using static TeklaGeometryExtender.XIntersection;

    class MonoPurlinGeoShape
    {
        private FramingOptions options;
        private MonoSlopeGeoShape shape;

        private List<Part> leftRafterParts;
        private List<Part> rightRafterParts;
        private List<Part> leftColParts;
        private List<Part> rightColParts;

        private List<Component> leftRafterComps;
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
        private Vector rafterVector;

        private double inclinedLeftEaveOffset;
        private double inclinedRightEaveOffset;

        private double biggestRafterThickness;
        private double biggestRightColThickness;
        private double biggestLeftColThickness;

        private List<Point> leftColPts;
        private List<Point> rfterPts;
        private List<Point> rightColPts;

        private List<(Point, Component)> leftColValuePair;
        private List<(Point, Component)> rfterValuePair;
        private List<(Point, Component)> rightColValuePair;

        public List<(Point point, Component component)> LeftColValuePair => leftColValuePair;
        public List<(Point point, Component component)> LeftRfterValuePair => rfterValuePair;
        public List<(Point point, Component component)> RightColValuePair => rightColValuePair;

        public List<Point> LeftColPts => leftColPts;
        public List<Point> LeftRfterPts => rfterPts;
        public List<Point> RightColPts => rightColPts;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="leftRafter"></param>
        /// <param name="rightRafter"></param>
        /// <param name="leftCol"></param>
        /// <param name="rightCol"></param>
        /// <param name="geoShape"></param>
        public MonoPurlinGeoShape(List<Component> leftRafterComps, List<Component> leftColComps, List<Component> rightColComps, MonoSlopeGeoShape geoShape, FramingOptions options)
        {
            this.shape = geoShape;

            this.leftRafterComps = leftRafterComps;
            this.leftColComps = leftColComps;
            this.rightColComps = rightColComps;

            this.options = options;

            leftColPts = new List<Point>();
            rfterPts = new List<Point>();
            rightColPts = new List<Point>();

            leftColValuePair = new List<(Point, Component)>();
            rfterValuePair = new List<(Point, Component)>();
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
            biggestRafterThickness = leftRafterParts.Select(p => p.GetProperties().Thickness).Max();
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
            rafterVector = new Vector(shape.FrameColRftRghtIntrsct - shape.FrameColRftLftIntrsct).GetNormal();

            Vector upDirection = new Vector(0, 0, 1);
            Vector leftRafterInverted = rafterVector * -1;
            Vector tempPerp = leftRafterInverted.Cross(upDirection);
            leftPerpVector = leftRafterInverted.Cross(tempPerp).GetNormal();

            inclinedLeftEaveLength = options.LeftEaveLength / Math.Cos(shape.SlopeAngleRad);
            inclinedLeftEaveOffset = options.LeftEaveOffset / Math.Cos(shape.SlopeAngleRad);

            leftTopOfSteelPoint = shape.FrameColRftLftIntrsct.Translate(leftPerpVector * -1 * biggestRafterThickness);

            leftMostPurlinRef = leftTopOfSteelPoint.Translate(-1 * rafterVector * inclinedLeftEaveLength);
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
        }



        /// <summary>
        /// 
        /// </summary>
        private void AssignLeftRafterPoints()
        {
            Point firstPurlinPoint = leftMostPurlinRef.Translate(inclinedLeftEaveOffset * rafterVector);

            rfterPts.Add(firstPurlinPoint);

            Point secondPurlinPoint = leftTopOfSteelPoint;

            if (options.AtCoverLeft)
            {
                rfterPts.Add(secondPurlinPoint);
                rfterPts = rfterPts.Distinct().ToList();
            }

            Point firstPoint = rfterPts.Last();

            double inclinedADistance = (options.HorizontalDistance / Math.Cos(shape.SlopeAngleRad));
            double purlinSpacingTotalLength = Distance.PointToPoint(firstPoint, shape.FrameColRftRghtIntrsct);
            int numberOfOffsets = (int)(purlinSpacingTotalLength / inclinedADistance);

            for (int i = 0; i < numberOfOffsets; i++)
            {
                firstPoint = firstPoint.Translate(inclinedADistance * rafterVector);
                rfterPts.Add(firstPoint);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void AssignLeftRafterTuple()
        {
            Line line;
            int index;
            foreach (var point in rfterPts)
            {
                line = new Line(point, leftPerpVector);
                index = leftRafterParts.FindIndex(p => Intersect(p.GetSolid(), line).Count > 0);
                rfterValuePair.Add((point, leftRafterComps[index]));
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
