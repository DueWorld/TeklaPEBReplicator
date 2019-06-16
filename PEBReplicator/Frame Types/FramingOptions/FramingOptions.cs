namespace PEBReplicator.Frame_Types
{
    using System.Collections.Generic;
    using Tekla.Structures.Geometry3d;

    /// <summary>
    /// Flange bracing mode for purlins.
    /// </summary>
    public enum FlangeBraceMode
    {
        NONE = 0,
        EVERYPURLIN = 1,
        EVERYOTHERPURLIN = 2
    }

    /// <summary>
    /// A class encapsulating all the possible options for a framing system.
    /// </summary>
    public class FramingOptions
    {
        public Point Origin { get; set; }
        public Vector FrameVector { get; set; }
        public Vector ReplicationVector { get; set; }
        public double Slope { get; set; }
        public string EndBraceAttrib { get; set; }
        public string GeneralBraceAttrib { get; set; }
        public string RidgeSpliceAttrib { get; set; }
        public string ClipAngleAttrib { get; set; }
        public FlangeBraceMode BracingMode { get; set; }
        public List<double> CopyLengths { get; set; }

        public double GableSteelLineFullLength { get; set; }
        public double GableSteelLineHalfLength { get; set; }
        public double GableSteelLineColumnHeight { get; set; }
        public double GableColumnOffset { get; set; }
        public double GableRafterOffset { get; set; }
        public double GableLeftBaseOffset { get; set; }
        public double GableRightBaseOffset { get; set; }
        
        public double MonoSlopeSteelLineFullLength { get; set; }
        public double MonoSlopeSteelLineColumnHeight { get; set; }
        public double MonoSlopeColumnOffset { get; set; }
        public double MonoSlopeRafterOffset { get; set; }
        public double MonoSlopeLeftBaseOffset { get; set; }
        public double MonoSlopeRightBaseOffset { get; set; }

        public double Col1SpliceNumber { get; set; }
        public double Col2SpliceNumber { get; set; }
        public double Raf1SpliceNumber { get; set; }
        public double Raf2SpliceNumber { get; set; }

        public List<double> Col1SpliceLengths { get; set; }
        public List<string> Col1SpliceAttribs { get; set; }
        public List<string> Col1MemberAttribs { get; set; }

        public string Col1KneeAttrib { get; set; }
        public string Col1BplAttrib { get; set; }

        public List<double> Col2SpliceLengths { get; set; }
        public List<string> Col2SpliceAttribs { get; set; }
        public List<string> Col2MemberAttribs { get; set; }

        public string Col2KneeAttrib { get; set; }
        public string Col2BplAttrib { get; set; }

        public List<double> Raf1SpliceLengths { get; set; }
        public List<string> Raf1SpliceAttribs { get; set; }
        public List<string> Raf1MemberAttribs { get; set; }

        public List<double> Raf2SpliceLengths { get; set; }
        public List<string> Raf2SpliceAttribs { get; set; }
        public List<string> Raf2MemberAttribs { get; set; }

        public double LeftEaveOffset { get; set; }
        public double RightEaveOffset { get; set; }

        public double LeftEaveLength { get; set; }
        public double RightEaveLength { get; set; }

        public double HorizontalDistance { get; set; }

        public bool AtCoverRight { get; set; }
        public bool AtCoverLeft { get; set; }

        public bool EaveExtendedLeft { get; set; }
        public bool EaveExtendedRight { get; set; }

        public string PurlinSection { get; set; }
        public string PurlinMaterial { get; set; }
    }
}
