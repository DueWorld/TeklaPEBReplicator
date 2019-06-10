namespace PEBReplicator.Frame_Types
{
    using PEBReplicator.Geometry.FrameGeoShapes;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// A class to draw gable frames with purlins and flange brace connections.
    /// </summary>
    class GableSetting
    {
        private FramingOptions options;
        private GableFrameGeoShape gableGeometry;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public GableSetting(FramingOptions options)
        {
            this.options = options;
            gableGeometry = new GableFrameGeoShape(options);
        }

        /// <summary>
        /// Main draw function.
        /// </summary>
        public void Draw()
        {
            List<GableFrameGeoShape> gables = new List<GableFrameGeoShape>();
            GableFrameGeoShape gableGeo;
            double counter = 0;
            foreach (var length in options.CopyLengths)
            {
                counter += length;
                gableGeo = new GableFrameGeoShape(options);
                gableGeo.Translate(options.ReplicationVector * counter);
                gables.Add(gableGeo);
            }


            GableFrame frame = new GableFrame(options, gableGeometry);
            frame.DrawFrame();


            GableFrame gableFrame;
            foreach (var gable in gables)
            {
                gableFrame = new GableFrame(options, gable);
                gableFrame.DrawFrame();

            }
        }
    }
}
