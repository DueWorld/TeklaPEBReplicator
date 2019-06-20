using PEBReplicator.Geometry.FrameGeoShapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeklaGeometryExtender;
using static TeklaGeometryExtender.GeometryUtils;
using Tekla.Structures.Model;

namespace PEBReplicator.Frame_Types
{
    class MonoSlopeSetting
    {
        private FramingOptions options;
        private MonoSlopeGeoShape gableGeometry;
        private List<MonoSlopeFrame> frames;

        private List<List<(Beam purlin, Component component)>> purlinSettingLeftRafter;
        private List<List<Beam>> purlinSettingRightRafter;
        private List<List<Beam>> purlinSettingLeftCol;
        private List<List<Beam>> purlinSettingRightCol;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public MonoSlopeSetting(FramingOptions options)
        {
            this.options = options;
            gableGeometry = new MonoSlopeGeoShape(options);
            frames = new List<MonoSlopeFrame>();

            purlinSettingLeftRafter = new List<List<(Beam purlin, Component component)>>();
            purlinSettingRightRafter = new List<List<Beam>>();
            purlinSettingLeftCol = new List<List<Beam>>();
            purlinSettingRightCol = new List<List<Beam>>();
        }

        /// <summary>
        /// Main draw function.
        /// </summary>
        public void Draw()
        {
            List<MonoSlopeGeoShape> gables = new List<MonoSlopeGeoShape>();
            MonoSlopeGeoShape gableGeo;
            double counter = 0;
            foreach (var length in options.CopyLengths)
            {
                counter += length;
                gableGeo = new MonoSlopeGeoShape(options);
                gableGeo.Translate(options.ReplicationVector * counter);
                gables.Add(gableGeo);
            }
            MonoSlopeFrame frame = new MonoSlopeFrame(options, gableGeometry);
            frame.DrawFrame();
            frames.Add(frame);
            MonoSlopeFrame gableFrame;
            foreach (var gable in gables)
            {
                gableFrame = new MonoSlopeFrame(options, gable);
                gableFrame.DrawFrame();
                frames.Add(gableFrame);
            }
            DrawPurlins();
        }

        /// <summary>
        /// 
        /// </summary>
        private void DrawPurlins()
        {
            //Left Purlins:
            var slopeAngleRad = Math.Atan(1 / options.Slope);
            if (frames.Count < 2)
                return;

            var size = frames[0].PurlinDrawer.LeftRfterPts.Count;
            Beam beam = new Beam();
            beam.Profile.ProfileString = options.PurlinSection;
            beam.Material.MaterialString = options.PurlinMaterial;
            beam.Position.Depth = Position.DepthEnum.FRONT;
            beam.Position.RotationOffset = ToDegree(-slopeAngleRad);
            List<(Beam purlin, Component component)> beamList = default(List<(Beam, Component)>);
            List<Beam> finalPurlins = new List<Beam>();
            for (int i = 0; i < frames.Count - 1; i++)
            {
                beamList = new List<(Beam, Component)>();
                for (int j = 0; j < size; j++)
                {
                    beam.StartPoint = frames[i].PurlinDrawer.LeftRfterValuePair[j].point;
                    beam.EndPoint = frames[i + 1].PurlinDrawer.LeftRfterValuePair[j].point;
                    beam.Insert();
                    beamList.Add((beam, frames[i].PurlinDrawer.LeftRfterValuePair[j].component));
                    new Model().CommitChanges();

                    if (i == (frames.Count - 2))
                        finalPurlins.Add(beam);
                }

                purlinSettingLeftRafter.Add(beamList);
            }
            beamList.Clear();
            for (int i = 0; i < frames.Last().PurlinDrawer.LeftRfterValuePair.Count; i++)
            {
                beamList.Add((finalPurlins[i], frames.Last().PurlinDrawer.LeftRfterValuePair[i].component));
            }
            purlinSettingLeftRafter.Add(beamList);
        }

        /// <summary>
        /// 
        /// </summary>
        private void AssignFlangedBrace()
        {

        }
    }
}
