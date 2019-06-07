namespace PEBReplicator.Frame_Types
{
    using PEBReplicator.Geometry.FrameGeoShapes;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class GableSetting
    {
        private FramingOptions options;
        private GableFrameGeoShape gableGeometry;
        
        public GableSetting(FramingOptions options)
        {
            this.options = options;
            gableGeometry = new GableFrameGeoShape(options);
        }

        public void Draw()
        {
            GableFrame frame = new GableFrame(options,gableGeometry);
            frame.DrawFrame();
        }
    }
}
