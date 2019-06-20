namespace PEBReplicator.Geometry.FrameGeoShapes
{
    using PEBReplicator.Frame_Types;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Tekla.Structures.Geometry3d;
    using TeklaGeometryExtender;

    /// <summary>
    /// A class calculating all points needed to draw a gable frame with splices.
    /// </summary>
    public class GableFrameGeoShape
    {
        private FramingOptions options;

        private Vector upDirectionVector;
        private Vector frameVector;
        private Vector leftRftVector;
        private Vector rightRftVector;

        private double frameLeftColumnHeight;
        private double frameRightColumnHeight;
        private double slopeAngleRad;
        private double steelLineSlopeHeight;
        private double frameSlopeHeight;
        private double frameTotalLength;
        private double frameHalfLength;
        private double frameLftRftLength;
        private double frameRghtRftLength;
        private double steelLineLeftRafterLength;
        private double steelLineRightRafterLength;

        private Point steelLineLeftBase;
        private Point steelLineColRaftLftIntrsct;
        private Point steelLineRidgePoint;
        private Point steelLineColRafRghtIntrsct;
        private Point steelLineRightBase;

        private Point frameLeftBase;
        private Point frameRightBase;
        private Point frameColRftLftIntrsct;
        private Point frameRidgePoint;
        private Point frameColRftRghtIntrsct;

        private List<double> calibratedCol1Lengths;
        private List<double> calibratedCol2Lengths;
        private List<double> calibratedRaft1Lengths;
        private List<double> calibratedRaft2Lengths;

        private List<Point> leftColPts;
        private List<Point> leftRfterPts;
        private List<Point> rightRfterPts;
        private List<Point> rightColPts;
        private List<Point> leftRafterPurlinPts;
        private List<Point> rightRafterPurlinPts;

        public List<Point> LeftColumnPts => leftColPts;
        public List<Point> LeftRafterPts => leftRfterPts;
        public List<Point> RightRafterPts => rightRfterPts;
        public List<Point> RightColumnPts => rightColPts;
        public List<Point> LeftRafterPurlinPts => leftRafterPurlinPts;
        public List<Point> RightRafterPurlinPts => rightRafterPurlinPts;

        public Point SteelLineLeftBase => steelLineLeftBase;
        public Point SteelLineColRaftLftIntrsct => steelLineColRaftLftIntrsct;
        public Point SteelLineRidgePoint => steelLineRidgePoint;
        public Point SteelLineColRafRghtIntrsct => steelLineColRafRghtIntrsct;
        public Point SteelLineRightBase => steelLineRightBase;

        public Point FrameLeftBase => frameLeftBase;
        public Point FrameRightBase => frameRightBase;
        public Point FrameColRftLftIntrsct => frameColRftLftIntrsct;
        public Point FrameRidgePoint => frameRidgePoint;
        public Point FrameColRftRghtIntrsct => frameColRftRghtIntrsct;

        public double FrameLeftColumnHeight => frameLeftColumnHeight;
        public double FrameRightColumnHeight => frameRightColumnHeight;
        public double SlopeAngleRad => slopeAngleRad;
        public double SteelLineSlopeHeight => steelLineSlopeHeight;
        public double FrameSlopeHeight => frameSlopeHeight;
        public double FrameTotalLength => frameTotalLength;
        public double FrameHalfLength => frameHalfLength;
        public double FrameLftRftLength => frameLftRftLength;
        public double FrameRghtRftLength => frameRghtRftLength;
        public double SteelLineLeftRafterLength => steelLineLeftRafterLength;
        public double SteelLineRightRafterLength => steelLineRightRafterLength;

        public FramingOptions Options { get => options; set => options = value; }

        /// <summary>
        /// Instantiates a geometric shape object by the given options.
        /// </summary>
        /// <param name="options"></param>
        public GableFrameGeoShape(FramingOptions options)
        {
            this.options = options;

            this.leftColPts = new List<Point>();
            this.rightColPts = new List<Point>();
            this.leftRfterPts = new List<Point>();
            this.rightRfterPts = new List<Point>();
            this.leftRafterPurlinPts = new List<Point>();
            this.rightRafterPurlinPts = new List<Point>();


            this.calibratedCol1Lengths = new List<double>();
            this.calibratedCol2Lengths = new List<double>();
            this.calibratedRaft1Lengths = new List<double>();
            this.calibratedRaft2Lengths = new List<double>();

            InitializePoints();
            PopulateCalibratedLists();
            PopulateColRftPts();
        }

        /// <summary>
        /// Calibrating all the given lengths for the ridge to be responsive.
        /// </summary>
        private void PopulateCalibratedLists()
        {
            calibratedCol1Lengths = CalibrateLengths(options.Col1SpliceLengths, frameLeftColumnHeight);
            calibratedCol2Lengths = CalibrateLengths(options.Col2SpliceLengths, frameRightColumnHeight);
            calibratedRaft1Lengths = CalibrateLengths(options.Raf1SpliceLengths, frameLftRftLength);
            calibratedRaft2Lengths = CalibrateLengths(options.Raf2SpliceLengths, frameRghtRftLength);
            calibratedRaft2Lengths.Reverse();
        }

        /// <summary>
        /// Calibrating the original lengths for the ridge to be responsive.
        /// </summary>
        /// <param name="originalLengths"></param>
        /// <param name="totalLength"></param>
        /// <returns></returns>
        private List<double> CalibrateLengths(List<double> originalLengths, double totalLength)
        {
            var list = new List<double>();
            double counter = 0;
            foreach (var length in originalLengths)
            {
                counter += length;
                if (counter < totalLength)
                    list.Add(length);
            }
            var sum = totalLength - list.Sum();
            if (sum > 0)
                list.Add(sum);
            return list;
        }

        /// <summary>
        /// Initializes all needed geometric points to draw the P.E.B gable frame.
        /// </summary>
        private void InitializePoints()
        {
            steelLineSlopeHeight = options.GableSteelLineHalfLength / options.Slope;
            frameTotalLength = options.GableSteelLineFullLength - 2 * options.GableColumnOffset;
            frameHalfLength = frameTotalLength / 2;
            frameSlopeHeight = frameHalfLength / options.Slope;
            slopeAngleRad = Math.Atan(1 / options.Slope);

            upDirectionVector = new Vector(0, 0, 1000).GetNormal();
            frameVector = options.FrameVector.GetNormal();
            steelLineLeftBase = options.Origin;

            steelLineColRaftLftIntrsct = steelLineLeftBase.Translate(upDirectionVector * options.GableSteelLineColumnHeight);
            steelLineRidgePoint = (steelLineColRaftLftIntrsct.Translate(frameVector * options.GableSteelLineHalfLength)).Translate(upDirectionVector * steelLineSlopeHeight);
            steelLineColRafRghtIntrsct = steelLineColRaftLftIntrsct.Translate(frameVector * options.GableSteelLineFullLength);
            steelLineRightBase = steelLineLeftBase.Translate(frameVector * options.GableSteelLineFullLength);

            frameLeftBase = steelLineLeftBase.Translate(upDirectionVector * options.GableLeftBaseOffset).Translate(frameVector * options.GableColumnOffset);
            frameRidgePoint = steelLineRidgePoint.Translate(-1 * upDirectionVector * (options.GableRafterOffset / Math.Cos(slopeAngleRad)));
            frameColRftLftIntrsct = frameRidgePoint.Translate(-1 * upDirectionVector * frameSlopeHeight).Translate(-1 * frameVector * frameHalfLength);

            frameColRftRghtIntrsct = frameColRftLftIntrsct.Translate(frameVector * frameTotalLength);
            frameRightBase = steelLineRightBase.Translate(frameVector * -1 * options.GableColumnOffset).Translate(upDirectionVector * options.GableRightBaseOffset);
            frameLeftColumnHeight = Distance.PointToPoint(frameLeftBase, frameColRftLftIntrsct);
            frameRightColumnHeight = Distance.PointToPoint(frameRightBase, frameColRftRghtIntrsct);
            frameLftRftLength = Distance.PointToPoint(frameRidgePoint, frameColRftLftIntrsct);
            frameRghtRftLength = Distance.PointToPoint(frameRidgePoint, frameColRftRghtIntrsct);
            leftRftVector = new Vector(frameRidgePoint - frameColRftLftIntrsct).GetNormal();
            rightRftVector = new Vector(frameColRftRghtIntrsct - frameRidgePoint).GetNormal();
            steelLineLeftRafterLength = Distance.PointToPoint(steelLineColRaftLftIntrsct, steelLineRidgePoint);
            steelLineRightRafterLength = Distance.PointToPoint(steelLineRidgePoint, steelLineColRafRghtIntrsct);
        }

        /// <summary>
        /// Populate the list of columns and rafters.
        /// </summary>
        private void PopulateColRftPts()
        {
            leftColPts.Clear();
            leftRfterPts.Clear();
            rightRfterPts.Clear();
            rightColPts.Clear();

            //Goes from bottom to top from the left base point to the left col/rafter intersection.
            Point tempPoint = frameLeftBase.ToGlobal();
            leftColPts.Add(tempPoint);
            foreach (var length in calibratedCol1Lengths)
            {
                tempPoint = tempPoint.Translate(upDirectionVector * length).ToGlobal();
                leftColPts.Add(tempPoint);
            }


            //Goes from left to right from the left col/rafter intersection to the ridge.
            tempPoint = frameColRftLftIntrsct.ToGlobal();
            leftRfterPts.Add(tempPoint);
            foreach (var length in calibratedRaft1Lengths)
            {
                tempPoint = tempPoint.Translate(leftRftVector * length).ToGlobal();
                leftRfterPts.Add(tempPoint);
            }


            //Goes from left to right from the ridge to the right col/rafter intersection.
            tempPoint = frameRidgePoint.ToGlobal();
            rightRfterPts.Add(tempPoint);
            foreach (var length in calibratedRaft2Lengths)
            {
                tempPoint = tempPoint.Translate(rightRftVector * length).ToGlobal();
                rightRfterPts.Add(tempPoint);
            }


            //Goes from bottom to top from the right base point to the right col/rafter intersection.
            tempPoint = frameRightBase.ToGlobal();
            rightColPts.Add(tempPoint);
            foreach (var length in calibratedCol2Lengths)
            {
                tempPoint = tempPoint.Translate(upDirectionVector * length).ToGlobal();
                rightColPts.Add(tempPoint);
            }
        }

        /// <summary>
        /// Calculates all the parameters according to the given options.
        /// </summary>
        public void CalculateParameters()
        {
            InitializePoints();
            PopulateColRftPts();
        }

        /// <summary>
        /// Translate the whole geometric shape by a given vector.
        /// </summary>
        /// <param name="v"></param>
        public void Translate(Vector v)
        {
            steelLineLeftBase = steelLineLeftBase.Translate(v);
            steelLineColRaftLftIntrsct = steelLineColRaftLftIntrsct.Translate(v);
            steelLineRidgePoint = steelLineRidgePoint.Translate(v);
            steelLineColRafRghtIntrsct = steelLineColRafRghtIntrsct.Translate(v);
            steelLineRightBase = steelLineRightBase.Translate(v);

            frameLeftBase = frameLeftBase.Translate(v);
            frameColRftLftIntrsct = frameColRftLftIntrsct.Translate(v);
            frameRidgePoint = frameRidgePoint.Translate(v);
            frameColRftRghtIntrsct = frameColRftRghtIntrsct.Translate(v);
            frameRightBase = frameRightBase.Translate(v);

            PopulateColRftPts();
        }
    }

    public static class Translation
    {
        public static Point Translate(this Point point, Vector v)
        {
            return new Point(point.X + v.X, point.Y + v.Y, point.Z + v.Z);
        }
    }
}
