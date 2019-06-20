namespace PEBReplicator.Geometry.FrameGeoShapes
{
    using PEBReplicator.Frame_Types;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Tekla.Structures.Geometry3d;
    using TeklaGeometryExtender;

    class MonoSlopeGeoShape
    {
        private FramingOptions options;

        private Vector upDirectionVector;
        private Vector frameVector;
        private Vector rftVector;

        private double frameLeftColumnHeight;
        private double frameRightColumnHeight;
        private double slopeAngleRad;
        private double steelLineSlopeHeight;
        private double frameSlopeHeight;
        private double frameTotalLength;
        private double frameHalfLength;
        private double frameRftLength;
        private double steelLineRafterLength;

        private Point steelLineLeftBase;
        private Point steelLineColRaftLftIntrsct;
        private Point steelLineColRafRghtIntrsct;
        private Point steelLineRightBase;

        private Point frameLeftBase;
        private Point frameRightBase;
        private Point frameColRftLftIntrsct;
        private Point frameColRftRghtIntrsct;

        private List<double> calibratedCol1Lengths;
        private List<double> calibratedCol2Lengths;
        private List<double> calibratedRaft1Lengths;

        private List<Point> leftColPts;
        private List<Point> rfterPts;
        private List<Point> rightColPts;
        private List<Point> rafterPurlinPts;

        public List<Point> LeftColumnPts => leftColPts;
        public List<Point> RafterPts => rfterPts;
        public List<Point> RightColumnPts => rightColPts;
        public List<Point> RafterPurlinPts => rafterPurlinPts;

        public Point SteelLineLeftBase => steelLineLeftBase;
        public Point SteelLineColRaftLftIntrsct => steelLineColRaftLftIntrsct;
        public Point SteelLineColRafRghtIntrsct => steelLineColRafRghtIntrsct;
        public Point SteelLineRightBase => steelLineRightBase;

        public Point FrameLeftBase => frameLeftBase;
        public Point FrameRightBase => frameRightBase;
        public Point FrameColRftLftIntrsct => frameColRftLftIntrsct;
        public Point FrameColRftRghtIntrsct => frameColRftRghtIntrsct;

        public double FrameLeftColumnHeight => frameLeftColumnHeight;
        public double FrameRightColumnHeight => frameRightColumnHeight;
        public double SlopeAngleRad => slopeAngleRad;
        public double SteelLineSlopeHeight => steelLineSlopeHeight;
        public double FrameSlopeHeight => frameSlopeHeight;
        public double FrameTotalLength => frameTotalLength;
        public double FrameHalfLength => frameHalfLength;
        public double FrameRftLength => frameRftLength;
        public double SteelLineRafterLength => steelLineRafterLength;

        public FramingOptions Options { get => options; set => options = value; }

        /// <summary>
        /// Instantiates a geometric shape object by the given options.
        /// </summary>
        /// <param name="options"></param>
        public MonoSlopeGeoShape(FramingOptions options)
        {
            this.options = options;

            this.leftColPts = new List<Point>();
            this.rightColPts = new List<Point>();
            this.rfterPts = new List<Point>();
            this.rafterPurlinPts = new List<Point>();

            this.calibratedCol1Lengths = new List<double>();
            this.calibratedCol2Lengths = new List<double>();
            this.calibratedRaft1Lengths = new List<double>();

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
            calibratedRaft1Lengths = CalibrateLengths(options.Raf1SpliceLengths, frameRftLength);
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
            steelLineSlopeHeight = options.MonoSlopeSteelLineFullLength / options.Slope;
            frameTotalLength = options.MonoSlopeSteelLineFullLength - 2 * options.MonoSlopeColumnOffset;
            frameHalfLength = frameTotalLength / 2;
            frameSlopeHeight = frameHalfLength / options.Slope;
            slopeAngleRad = Math.Atan(1 / options.Slope);

            upDirectionVector = new Vector(0, 0, 1000).GetNormal();
            frameVector = options.FrameVector.GetNormal();
            steelLineLeftBase = options.Origin;

            steelLineColRaftLftIntrsct = steelLineLeftBase.Translate(upDirectionVector * options.MonoSlopeSteelLineColumnHeight);
            steelLineColRafRghtIntrsct = steelLineColRaftLftIntrsct.Translate(frameVector * options.MonoSlopeSteelLineFullLength).Translate(upDirectionVector * steelLineSlopeHeight);
            steelLineRightBase = steelLineLeftBase.Translate(frameVector * options.MonoSlopeSteelLineFullLength);

            frameLeftBase = steelLineLeftBase.Translate(upDirectionVector * options.MonoSlopeLeftBaseOffset).Translate(frameVector * options.MonoSlopeColumnOffset);
            frameRightBase = steelLineRightBase.Translate(frameVector * -1 * options.MonoSlopeColumnOffset).Translate(upDirectionVector * options.MonoSlopeRightBaseOffset);

            Vector alongRafterVector = new Vector(steelLineColRafRghtIntrsct - steelLineColRaftLftIntrsct).GetNormal();
            Vector tempPerpForRafter = alongRafterVector.Cross(upDirectionVector);
            Vector perpVectorOnRafter = alongRafterVector.Cross(tempPerpForRafter).GetNormal();

            Line frameLine = new Line(
                steelLineColRaftLftIntrsct.Translate(perpVectorOnRafter * options.MonoSlopeRafterOffset),
                steelLineColRafRghtIntrsct.Translate(perpVectorOnRafter * options.MonoSlopeRafterOffset)
                                     );

            Line leftColLine = new Line(frameLeftBase, upDirectionVector);
            Line rightColLine = new Line(frameRightBase, upDirectionVector);
            

            frameColRftLftIntrsct = Intersection.LineToLine(leftColLine, frameLine).Point1;
            frameColRftRghtIntrsct = Intersection.LineToLine(rightColLine, frameLine).Point1;
            

            frameLeftColumnHeight = Distance.PointToPoint(frameLeftBase, frameColRftLftIntrsct);
            frameRightColumnHeight = Distance.PointToPoint(frameRightBase, frameColRftRghtIntrsct);
            frameRftLength = Distance.PointToPoint(frameColRftLftIntrsct, frameColRftRghtIntrsct);
            rftVector = new Vector(frameColRftRghtIntrsct - frameColRftLftIntrsct).GetNormal();
            steelLineRafterLength = Distance.PointToPoint(steelLineColRaftLftIntrsct, steelLineColRafRghtIntrsct);
        }

        /// <summary>
        /// Populate the list of columns and rafters.
        /// </summary>
        private void PopulateColRftPts()
        {
            leftColPts.Clear();
            rfterPts.Clear();
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
            rfterPts.Add(tempPoint);
            foreach (var length in calibratedRaft1Lengths)
            {
                tempPoint = tempPoint.Translate(rftVector * length).ToGlobal();
                rfterPts.Add(tempPoint);
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
            steelLineColRafRghtIntrsct = steelLineColRafRghtIntrsct.Translate(v);
            steelLineRightBase = steelLineRightBase.Translate(v);

            frameLeftBase = frameLeftBase.Translate(v);
            frameColRftLftIntrsct = frameColRftLftIntrsct.Translate(v);
            frameColRftRghtIntrsct = frameColRftRghtIntrsct.Translate(v);
            frameRightBase = frameRightBase.Translate(v);

            PopulateColRftPts();
        }
    }
}
