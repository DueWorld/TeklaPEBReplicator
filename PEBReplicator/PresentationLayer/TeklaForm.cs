namespace PEBReplicator
{
    using System;
    using System.Collections.Generic;
    using System.Collections;
    using System.Windows.Forms;
    using Tekla.Structures.Geometry3d;
    using Tekla.Structures.Model.UI;
    using Tekla.Structures.Model;
    using Tekla.Structures.Dialog;
    using Tekla.Structures.Dialog.UIControls;
    using PEBReplicator.Frame_Types;
    using static TeklaGeometryExtender.Transformation;
    using System.Globalization;
    using Tekla.Structures.Datatype;

    /// <summary>
    /// Main Form.
    /// </summary>
    public partial class TeklaForm : ApplicationFormBase
    {
        private Model teklaModel;
        private Vector alongFrameVector;
        private Vector replicationVector;

        private List<Control> radialRelated;
        private List<Control> monoSlopeRelated;
        private List<Control> gableRelated;
        private List<Control> lengthTextBoxes;

        private List<string> PEBKneeConnectionAttrs;
        private List<string> PEBFlangedBraceAttrs;
        private List<string> PEBSpliceConnectionAttrs;
        private List<string> PEBStraightMemberAttrs;
        private List<string> PEBRadialMemberAttrs;
        private List<string> PEBBasePlateAttrs;

        private const string PEBKneeConnectionAttr = "PEBKneeConnection.Presentation_Layer.PluginInterface.xml";
        private const string PEBFlangedBraceAttr = "FlangeBraceBeta.Presentation.PluginInterface.xml";
        private const string PEBSpliceConnectionAttr = "teklaPEBSpliceConnection.Presentation.Plugin.PluginForm.xml";
        private const string PEBStraightMemberAttr = "TeklaPEBStraightMember.Presentation_Layer.PluginInterface.xml";
        private const string PEBRadialMemberAttr = "TeklaPEBCurvedMember.Presentation_Layer.PluginInterface.xml";
        private const string PEBBasePlateAttr = "PEBBasePlate.Presentation_Layer.PluginInterface.xml";

        public TeklaForm()
        {
            teklaModel = new Model();
            InitializeComponent();
            PopulateControls();
            InitializeFrameProp();
            PopulateAttributes();
            AssignComboBox();
            PopulateLengthTextBoxes();
            AssignLengthTextBox();
        }

        private void PopulateControls()
        {
            radialRelated = new List<Control>();
            monoSlopeRelated = new List<Control>();
            gableRelated = new List<Control>();
            PopulateRadial();
            PopulateGableRelatedCntrls();
            PopulateMonoSlopeCntrls();
        }

        private void PopulateRadial()
        {

        }


        private void PopulateMonoSlopeCntrls()
        {
            monoSlopeRelated.Add(txtBoxMnoColHt);
            monoSlopeRelated.Add(txtBoxMnoColOffst);
            monoSlopeRelated.Add(txtBoxMnoFrmLength);
            monoSlopeRelated.Add(txtBoxMnoLftBsOffst);
            monoSlopeRelated.Add(txtBoxMnoRftOffst);
            monoSlopeRelated.Add(txtBoxMnoRghtBsOffst);
        }

        private void PopulateGableRelatedCntrls()
        {
            gableRelated.Add(txtBoxGblColHt);
            gableRelated.Add(txtBoxGblColOffst);
            gableRelated.Add(txtBoxGblFrmLength);
            gableRelated.Add(txtBoxGblHalfRft);
            gableRelated.Add(txtBoxGblLftBsOffst);
            gableRelated.Add(txtBoxGblRftOffst);
            gableRelated.Add(txtBoxGblRghtBsOffst);
        }

        private void PopulateLengthTextBoxes()
        {
            lengthTextBoxes = new List<Control>();

            lengthTextBoxes.AddRange(monoSlopeRelated);
            lengthTextBoxes.AddRange(radialRelated);
            lengthTextBoxes.AddRange(gableRelated);
            lengthTextBoxes.Add(txtBoxCol1SplLn1);
            lengthTextBoxes.Add(txtBoxCol1SplLn2);
            lengthTextBoxes.Add(txtBoxCol1SplLn3);

            lengthTextBoxes.Add(txtBoxCol2SplLn1);
            lengthTextBoxes.Add(txtBoxCol2SplLn2);
            lengthTextBoxes.Add(txtBoxCol2SplLn3);

            lengthTextBoxes.Add(txtBoxRft2SplLn1);
            lengthTextBoxes.Add(txtBoxRft2SplLn2);
            lengthTextBoxes.Add(txtBoxRft2SplLn3);
            lengthTextBoxes.Add(txtBoxRft2SplLn4);
            lengthTextBoxes.Add(txtBoxRft2SplLn5);
            lengthTextBoxes.Add(txtBoxRft2SplLn6);

            lengthTextBoxes.Add(txtBoxRft1SplLn1);
            lengthTextBoxes.Add(txtBoxRft1SplLn2);
            lengthTextBoxes.Add(txtBoxRft1SplLn3);
            lengthTextBoxes.Add(txtBoxRft1SplLn4);
            lengthTextBoxes.Add(txtBoxRft1SplLn5);
            lengthTextBoxes.Add(txtBoxRft1SplLn6);
            lengthTextBoxes.Add(txtBoxSlope);
            lengthTextBoxes.Add(txtBoxHorzDist);
            lengthTextBoxes.Add(txtBoxLeftEaveOffst);
            lengthTextBoxes.Add(txtBoxRightEaveOffset);

        }

        private bool ValidateLengthTextBoxes()
        {
            foreach (var txtBox in lengthTextBoxes)
            {
                if (!double.TryParse(txtBox.Text, out double result))
                    return false;
            }

            var dList = DistanceList.Parse(txtBoxCopies.Text, CultureInfo.CurrentCulture, Tekla.Structures.Datatype.Distance.UnitType.Millimeter);
            if (dList.Count == 0)
                return false;

            if (txtBoxPrlnProf.Text == string.Empty)
                return false;
            if (txtBoxPrlnMtrl.Text == string.Empty)
                return false;

            return true;
        }

        private void AssignLengthTextBox()
        {
            txtBoxGblColHt.Text = "12000.00";
            txtBoxGblColOffst.Text = "200";
            txtBoxGblFrmLength.Text = "30000.00";
            txtBoxGblHalfRft.Text = "15000.00";
            txtBoxGblLftBsOffst.Text = "50";
            txtBoxGblRftOffst.Text = "200";
            txtBoxGblRghtBsOffst.Text = "50";
            txtBoxCopies.Text = "2*6000";

            txtBoxMnoColHt.Text = "12000.00";
            txtBoxMnoColOffst.Text = "200";
            txtBoxMnoFrmLength.Text = "30000.00";
            txtBoxMnoLftBsOffst.Text = "50";
            txtBoxMnoRftOffst.Text = "200";
            txtBoxMnoRghtBsOffst.Text = "50";
            txtBoxSlope.Text = "15";
            txtBoxCol1SplLn1.Text = "6000";
            txtBoxCol1SplLn2.Text = "6000";
            txtBoxCol1SplLn3.Text = "6000";

            txtBoxCol2SplLn1.Text = "6000";
            txtBoxCol2SplLn2.Text = "6000";
            txtBoxCol2SplLn3.Text = "6000";

            txtBoxRft2SplLn1.Text = "6000";
            txtBoxRft2SplLn2.Text = "6000";
            txtBoxRft2SplLn3.Text = "6000";
            txtBoxRft2SplLn4.Text = "6000";
            txtBoxRft2SplLn5.Text = "6000";
            txtBoxRft2SplLn6.Text = "6000";

            txtBoxRft1SplLn1.Text = "6000";
            txtBoxRft1SplLn2.Text = "6000";
            txtBoxRft1SplLn3.Text = "6000";
            txtBoxRft1SplLn4.Text = "6000";
            txtBoxRft1SplLn5.Text = "6000";
            txtBoxRft1SplLn6.Text = "6000";
            txtBoxLeftEaveOffst.Text = "50";
            txtBoxRightEaveOffset.Text = "50";
            txtBoxHorzDist.Text = "1500";
            txtBoxPrlnMtrl.Text = "S235JR";
            txtBoxPrlnProf.Text = "Z200/1.5";
        }

        private void Hide(List<Control> list)
        {
            foreach (var control in list)
            {
                control.Visible = false;
            }
        }

        private void UnHide(List<Control> list)
        {
            foreach (var control in list)
            {
                control.Visible = true;
            }
        }

        private void PopulateAttributes()
        {
            PEBKneeConnectionAttrs = EnvironmentFiles.GetMultiDirectoryFileList(EnvironmentFiles.GetStandardPropertyFileDirectories(), PEBKneeConnectionAttr);
            PEBFlangedBraceAttrs = EnvironmentFiles.GetMultiDirectoryFileList(EnvironmentFiles.GetStandardPropertyFileDirectories(), PEBFlangedBraceAttr);
            PEBStraightMemberAttrs = EnvironmentFiles.GetMultiDirectoryFileList(EnvironmentFiles.GetStandardPropertyFileDirectories(), PEBStraightMemberAttr);
            PEBRadialMemberAttrs = EnvironmentFiles.GetMultiDirectoryFileList(EnvironmentFiles.GetStandardPropertyFileDirectories(), PEBRadialMemberAttr);
            PEBSpliceConnectionAttrs = EnvironmentFiles.GetMultiDirectoryFileList(EnvironmentFiles.GetStandardPropertyFileDirectories(), PEBSpliceConnectionAttr);
            PEBBasePlateAttrs = EnvironmentFiles.GetMultiDirectoryFileList(EnvironmentFiles.GetStandardPropertyFileDirectories(), PEBBasePlateAttr);
        }

        private void AssignComboBox()
        {
            #region Add range
            this.cmbBoxClipAttribute.Items.AddRange(PEBFlangedBraceAttrs.ToArray());
            this.cmbBoxCol1SplAFile1.Items.AddRange(PEBSpliceConnectionAttrs.ToArray());
            this.cmbBoxCol1SplAFile2.Items.AddRange(PEBSpliceConnectionAttrs.ToArray());
            this.cmbBoxCol1SplAFile3.Items.AddRange(PEBSpliceConnectionAttrs.ToArray());
            this.cmbBoxCol2SplAFile1.Items.AddRange(PEBSpliceConnectionAttrs.ToArray());
            this.cmbBoxCol2SplAFile2.Items.AddRange(PEBSpliceConnectionAttrs.ToArray());
            this.cmbBoxCol2SplAFile3.Items.AddRange(PEBSpliceConnectionAttrs.ToArray());
            this.cmbBoxRft1SplAFile1.Items.AddRange(PEBSpliceConnectionAttrs.ToArray());
            this.cmbBoxRft1SplAFile2.Items.AddRange(PEBSpliceConnectionAttrs.ToArray());
            this.cmbBoxRft1SplAFile3.Items.AddRange(PEBSpliceConnectionAttrs.ToArray());
            this.cmbBoxRft1SplAFile4.Items.AddRange(PEBSpliceConnectionAttrs.ToArray());
            this.cmbBoxRft1SplAFile6.Items.AddRange(PEBSpliceConnectionAttrs.ToArray());
            this.cmbBoxRft1SplAFile5.Items.AddRange(PEBSpliceConnectionAttrs.ToArray());
            this.cmbBoxRft2SplAFile1.Items.AddRange(PEBSpliceConnectionAttrs.ToArray());
            this.cmbBoxRft2SplAFile2.Items.AddRange(PEBSpliceConnectionAttrs.ToArray());
            this.cmbBoxRft2SplAFile3.Items.AddRange(PEBSpliceConnectionAttrs.ToArray());
            this.cmbBoxRft2SplAFile4.Items.AddRange(PEBSpliceConnectionAttrs.ToArray());
            this.cmbBoxRft2SplAFile6.Items.AddRange(PEBSpliceConnectionAttrs.ToArray());
            this.cmbBoxRft2SplAFile5.Items.AddRange(PEBSpliceConnectionAttrs.ToArray());
            this.cmbBoxRidgeSplice.Items.AddRange(PEBSpliceConnectionAttrs.ToArray());
            this.cmbBoxCol2BplAFile.Items.AddRange(PEBBasePlateAttrs.ToArray());
            this.cmbBoxCol1BplAFile.Items.AddRange(PEBBasePlateAttrs.ToArray());

            this.cmbBoxCol2KneeAFile.Items.AddRange(PEBKneeConnectionAttrs.ToArray());
            this.cmbBoxCol1KneeAFile.Items.AddRange(PEBKneeConnectionAttrs.ToArray());

            this.cmbBoxCol1AFile1.Items.AddRange(PEBStraightMemberAttrs.ToArray());
            this.cmbBoxCol1AFile2.Items.AddRange(PEBStraightMemberAttrs.ToArray());
            this.cmbBoxCol1AFile3.Items.AddRange(PEBStraightMemberAttrs.ToArray());
            this.cmbBoxCol1AFile4.Items.AddRange(PEBStraightMemberAttrs.ToArray());
            this.cmbBoxCol2AFile1.Items.AddRange(PEBStraightMemberAttrs.ToArray());
            this.cmbBoxCol2AFile2.Items.AddRange(PEBStraightMemberAttrs.ToArray());
            this.cmbBoxCol2AFile3.Items.AddRange(PEBStraightMemberAttrs.ToArray());
            this.cmbBoxCol2AFile4.Items.AddRange(PEBStraightMemberAttrs.ToArray());
            this.cmbBoxRft1AFile1.Items.AddRange(PEBStraightMemberAttrs.ToArray());
            this.cmbBoxRft1AFile2.Items.AddRange(PEBStraightMemberAttrs.ToArray());
            this.cmbBoxRft1AFile3.Items.AddRange(PEBStraightMemberAttrs.ToArray());
            this.cmbBoxRft1AFile4.Items.AddRange(PEBStraightMemberAttrs.ToArray());
            this.cmbBoxRft1AFile5.Items.AddRange(PEBStraightMemberAttrs.ToArray());
            this.cmbBoxRft1AFile6.Items.AddRange(PEBStraightMemberAttrs.ToArray());
            this.cmbBoxRft1AFile7.Items.AddRange(PEBStraightMemberAttrs.ToArray());
            this.cmbBoxRft2AFile1.Items.AddRange(PEBStraightMemberAttrs.ToArray());
            this.cmbBoxRft2AFile2.Items.AddRange(PEBStraightMemberAttrs.ToArray());
            this.cmbBoxRft2AFile3.Items.AddRange(PEBStraightMemberAttrs.ToArray());
            this.cmbBoxRft2AFile4.Items.AddRange(PEBStraightMemberAttrs.ToArray());
            this.cmbBoxRft2AFile5.Items.AddRange(PEBStraightMemberAttrs.ToArray());
            this.cmbBoxRft2AFile6.Items.AddRange(PEBStraightMemberAttrs.ToArray());
            this.cmbBoxRft2AFile7.Items.AddRange(PEBStraightMemberAttrs.ToArray());

            this.cmbBoxEndFlangeBrce.Items.AddRange(PEBFlangedBraceAttrs.ToArray());
            this.cmbBoxFlangeBrace.Items.AddRange(PEBFlangedBraceAttrs.ToArray());
            #endregion

            #region Initialize Standard
            this.cmbBoxRight.SelectedIndex = 0;
            this.cmbBoxCol1SplAFile1.SelectedIndex = 0;
            this.cmbBoxCol1SplAFile2.SelectedIndex = 0;
            this.cmbBoxCol1SplAFile3.SelectedIndex = 0;
            this.cmbBoxCol2SplAFile1.SelectedIndex = 0;
            this.cmbBoxCol2SplAFile2.SelectedIndex = 0;
            this.cmbBoxCol2SplAFile3.SelectedIndex = 0;
            this.cmbBoxRft1SplAFile1.SelectedIndex = 0;
            this.cmbBoxRft1SplAFile2.SelectedIndex = 0;
            this.cmbBoxRft1SplAFile3.SelectedIndex = 0;
            this.cmbBoxRft1SplAFile4.SelectedIndex = 0;
            this.cmbBoxRft1SplAFile6.SelectedIndex = 0;
            this.cmbBoxRft1SplAFile5.SelectedIndex = 0;
            this.cmbBoxRft2SplAFile1.SelectedIndex = 0;
            this.cmbBoxRft2SplAFile2.SelectedIndex = 0;
            this.cmbBoxRft2SplAFile3.SelectedIndex = 0;
            this.cmbBoxRft2SplAFile4.SelectedIndex = 0;
            this.cmbBoxRft2SplAFile6.SelectedIndex = 0;
            this.cmbBoxRft2SplAFile5.SelectedIndex = 0;

            this.cmbBoxCol2BplAFile.SelectedIndex = 0;
            this.cmbBoxCol1BplAFile.SelectedIndex = 0;

            this.cmbBoxCol2KneeAFile.SelectedIndex = 0;
            this.cmbBoxCol1KneeAFile.SelectedIndex = 0;

            this.cmbBoxCol1AFile1.SelectedIndex = 0;
            this.cmbBoxCol1AFile2.SelectedIndex = 0;
            this.cmbBoxCol1AFile3.SelectedIndex = 0;
            this.cmbBoxCol1AFile4.SelectedIndex = 0;
            this.cmbBoxCol2AFile1.SelectedIndex = 0;
            this.cmbBoxCol2AFile2.SelectedIndex = 0;
            this.cmbBoxCol2AFile3.SelectedIndex = 0;
            this.cmbBoxCol2AFile4.SelectedIndex = 0;
            this.cmbBoxRft1AFile1.SelectedIndex = 0;
            this.cmbBoxRft1AFile2.SelectedIndex = 0;
            this.cmbBoxRft1AFile3.SelectedIndex = 0;
            this.cmbBoxRft1AFile4.SelectedIndex = 0;
            this.cmbBoxRft1AFile5.SelectedIndex = 0;
            this.cmbBoxRft1AFile6.SelectedIndex = 0;
            this.cmbBoxRft1AFile7.SelectedIndex = 0;
            this.cmbBoxRft2AFile1.SelectedIndex = 0;
            this.cmbBoxRft2AFile2.SelectedIndex = 0;
            this.cmbBoxRft2AFile3.SelectedIndex = 0;
            this.cmbBoxRft2AFile4.SelectedIndex = 0;
            this.cmbBoxRft2AFile5.SelectedIndex = 0;
            this.cmbBoxRft2AFile6.SelectedIndex = 0;
            this.cmbBoxRft2AFile7.SelectedIndex = 0;
            this.cmbBoxLeft.SelectedIndex = 0;
            this.cmbBoxEndFlangeBrce.SelectedIndex = 0;
            this.cmbBoxFlangeBrace.SelectedIndex = 0;
            this.cmbBoxFlngBrcMode.SelectedIndex = 0;
            this.cmbBoxSpliceNoCol1.SelectedIndex = 0;
            this.cmbBoxSpliceNoCol2.SelectedIndex = 0;
            this.cmbBoxSpliceNoRft1.SelectedIndex = 0;
            this.cmbBoxSpliceNoRft2.SelectedIndex = 0;
            this.cmbBoxRidgeSplice.SelectedIndex = 0;
            this.cmbBoxClipAttribute.SelectedIndex = 0;
            #endregion
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!ValidateLengthTextBoxes())
            {
                ErrorDialog.Show("Alert", "Inputs are not valid!", ErrorDialog.Severity.ERROR);
                return;
            }
            this.Apply();
            this.Close();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (!ValidateLengthTextBoxes())
            {
                ErrorDialog.Show("Alert", "Inputs are not valid!", ErrorDialog.Severity.ERROR);
                return;
            }
            this.Apply();

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void imgListCmbBoxFrameMode_ImageListComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (imgListCmbBoxFrameMode.SelectedIndex == 0)
            {
                UnHide(gableRelated);
                Hide(radialRelated);
                Hide(monoSlopeRelated);
                this.pictureBoxDim.Image = global::PEBReplicator.Properties.Resources.r001;
                this.btnRft2.Visible = true;

            }
            else if (imgListCmbBoxFrameMode.SelectedIndex == 1)
            {
                UnHide(monoSlopeRelated);
                Hide(radialRelated);
                Hide(gableRelated);
                this.pictureBoxDim.Image = global::PEBReplicator.Properties.Resources.r003;
                this.btnRft2.Visible = false;

            }
            else
            {
                UnHide(radialRelated);
                Hide(monoSlopeRelated);
                Hide(monoSlopeRelated);
                this.pictureBoxDim.Image = global::PEBReplicator.Properties.Resources.r002;
                this.btnRft2.Visible = false;
                this.groupBoxRf2.Visible = false;

            }
        }

        private void InitializeFrameProp()
        {
            groupBoxCol2.Location = new System.Drawing.Point(39, 380);
            groupBoxRf2.Location = new System.Drawing.Point(40, 380);
            groupBoxRf1.Location = new System.Drawing.Point(41, 380);
            groupBoxCol1.Location = new System.Drawing.Point(161, 6);
        }

        private void btnCol1_Click(object sender, EventArgs e)
        {
            groupBoxCol2.Location = new System.Drawing.Point(39, 380);
            groupBoxRf2.Location = new System.Drawing.Point(40, 380);
            groupBoxRf1.Location = new System.Drawing.Point(41, 380);
            groupBoxCol1.Location = new System.Drawing.Point(161, 6);
        }

        private void btnRft1_Click(object sender, EventArgs e)
        {
            groupBoxCol1.Location = new System.Drawing.Point(38, 380);
            groupBoxCol2.Location = new System.Drawing.Point(39, 380);
            groupBoxRf2.Location = new System.Drawing.Point(40, 380);
            groupBoxRf1.Location = new System.Drawing.Point(161, 6);
        }

        private void btnRft2_Click(object sender, EventArgs e)
        {
            groupBoxCol1.Location = new System.Drawing.Point(38, 380);
            groupBoxCol2.Location = new System.Drawing.Point(38, 380);
            groupBoxRf1.Location = new System.Drawing.Point(38, 380);
            groupBoxRf2.Location = new System.Drawing.Point(161, 6);

        }

        private void btnCol2_Click(object sender, EventArgs e)
        {
            groupBoxCol1.Location = new System.Drawing.Point(38, 380);
            groupBoxRf2.Location = new System.Drawing.Point(40, 380);
            groupBoxRf1.Location = new System.Drawing.Point(38, 380);
            groupBoxCol2.Location = new System.Drawing.Point(161, 6);

        }

        private FramingOptions PopulateFramingOption(Point origin)
        {
            #region Instantiating lists
            List<string> col1MemberAttribs = new List<string>()
            { cmbBoxCol1AFile1.Text, cmbBoxCol1AFile2.Text,
                cmbBoxCol1AFile3.Text, cmbBoxCol1AFile4.Text };

            List<string> col1SpliceAttribs = new List<string>()
            {cmbBoxCol1SplAFile1.Text,cmbBoxCol1SplAFile2.Text,cmbBoxCol1SplAFile3.Text};

            List<double> col1SpliceLengths = new List<double>()
            { double.Parse(txtBoxCol1SplLn1.Text),double.Parse(txtBoxCol1SplLn2.Text),double.Parse(txtBoxCol1SplLn3.Text)};

            List<string> col2MemberAttribs = new List<string>()
            { cmbBoxCol2AFile1.Text, cmbBoxCol2AFile2.Text,
                cmbBoxCol2AFile3.Text, cmbBoxCol2AFile4.Text };

            List<string> col2SpliceAttribs = new List<string>()
            {cmbBoxCol2SplAFile1.Text,cmbBoxCol2SplAFile2.Text,cmbBoxCol2SplAFile3.Text};

            List<double> col2SpliceLengths = new List<double>()
            { double.Parse(txtBoxCol2SplLn1.Text),double.Parse(txtBoxCol2SplLn2.Text),double.Parse(txtBoxCol2SplLn3.Text)};


            List<string> raf1MemberAttribs = new List<string>()
            { cmbBoxRft1AFile1.Text,cmbBoxRft1AFile2.Text,cmbBoxRft1AFile3.Text,cmbBoxRft1AFile4.Text,
                cmbBoxRft1AFile5.Text,cmbBoxRft1AFile6.Text,cmbBoxRft1AFile7.Text};

            List<string> raf1SpliceAttribs = new List<string>()
            { cmbBoxRft1SplAFile1.Text,cmbBoxRft1SplAFile2.Text,cmbBoxRft1SplAFile3.Text,
              cmbBoxRft1SplAFile4.Text,cmbBoxRft1SplAFile5.Text,cmbBoxRft1SplAFile6.Text};


            List<double> raf1SpliceLengths = new List<double>()
            { double.Parse(txtBoxRft1SplLn1.Text),double.Parse(txtBoxRft1SplLn2.Text),double.Parse(txtBoxRft1SplLn3.Text),
              double.Parse(txtBoxRft1SplLn4.Text),double.Parse(txtBoxRft1SplLn5.Text),double.Parse(txtBoxRft1SplLn6.Text)};


            List<string> raf2MemberAttribs = new List<string>()
            { cmbBoxRft2AFile1.Text,cmbBoxRft2AFile2.Text,cmbBoxRft2AFile3.Text,cmbBoxRft2AFile4.Text,
              cmbBoxRft2AFile5.Text,cmbBoxRft2AFile6.Text,cmbBoxRft2AFile7.Text};

            List<string> raf2SpliceAttribs = new List<string>()
            { cmbBoxRft2SplAFile1.Text,cmbBoxRft2SplAFile2.Text,cmbBoxRft2SplAFile3.Text,
              cmbBoxRft2SplAFile4.Text,cmbBoxRft2SplAFile5.Text,cmbBoxRft2SplAFile6.Text};


            List<double> raf2SpliceLengths = new List<double>()
            { double.Parse(txtBoxRft2SplLn1.Text),double.Parse(txtBoxRft2SplLn2.Text),double.Parse(txtBoxRft2SplLn3.Text),
              double.Parse(txtBoxRft2SplLn4.Text),double.Parse(txtBoxRft2SplLn5.Text),double.Parse(txtBoxRft2SplLn6.Text)};

            #endregion

            #region Modifying lists
            int col1SpliceQuantity = int.TryParse(cmbBoxSpliceNoCol1.Text, out int quantity1) ? quantity1 : 0;
            int col2SpliceQuantity = int.TryParse(cmbBoxSpliceNoCol2.Text, out int quantity2) ? quantity2 : 0;
            int raf1SpliceQuantity = int.TryParse(cmbBoxSpliceNoRft1.Text, out int quantity3) ? quantity3 : 0;
            int raf2SpliceQuantity = int.TryParse(cmbBoxSpliceNoRft2.Text, out int quantity4) ? quantity4 : 0;


            col1MemberAttribs.RemoveRange(col1SpliceQuantity + 1, col1MemberAttribs.Count - (col1SpliceQuantity + 1));
            col1SpliceAttribs.RemoveRange(col1SpliceQuantity, col1SpliceAttribs.Count - col1SpliceQuantity);
            col1SpliceLengths.RemoveRange(col1SpliceQuantity, col1SpliceLengths.Count - col1SpliceQuantity);

            col2MemberAttribs.RemoveRange(col2SpliceQuantity + 1, col2MemberAttribs.Count - (col2SpliceQuantity + 1));
            col2SpliceAttribs.RemoveRange(col2SpliceQuantity, col2SpliceAttribs.Count - col2SpliceQuantity);
            col2SpliceLengths.RemoveRange(col2SpliceQuantity, col2SpliceLengths.Count - col2SpliceQuantity);

            raf1MemberAttribs.RemoveRange(raf1SpliceQuantity + 1, raf1MemberAttribs.Count - (raf1SpliceQuantity + 1));
            raf1SpliceAttribs.RemoveRange(raf1SpliceQuantity, raf1SpliceAttribs.Count - raf1SpliceQuantity);
            raf1SpliceLengths.RemoveRange(raf1SpliceQuantity, raf1SpliceLengths.Count - raf1SpliceQuantity);

            raf2MemberAttribs.RemoveRange(raf2SpliceQuantity + 1, raf2MemberAttribs.Count - (raf2SpliceQuantity + 1));
            raf2SpliceAttribs.RemoveRange(raf2SpliceQuantity, raf2SpliceAttribs.Count - raf2SpliceQuantity);
            raf2SpliceLengths.RemoveRange(raf2SpliceQuantity, raf2SpliceLengths.Count - raf2SpliceQuantity);
            #endregion

            var dList = DistanceList.Parse(txtBoxCopies.Text, CultureInfo.CurrentCulture, Tekla.Structures.Datatype.Distance.UnitType.Millimeter);
            var flagOfCoverRight = cmbBoxRight.SelectedIndex == 0 ? true : false;
            var flagOfCoverLeft = cmbBoxLeft.SelectedIndex == 0 ? true : false;

            List<double> copies = new List<double>();

            foreach (var distance in dList)
            {
                copies.Add(distance.Value);
            }
            copies.RemoveAll(p => p == 0);

            FramingOptions options = new FramingOptions()
            {
                FrameVector = alongFrameVector,
                ReplicationVector = replicationVector,
                Col1BplAttrib = cmbBoxCol1BplAFile.Text,
                Col1KneeAttrib = cmbBoxCol1KneeAFile.Text,
                Col1SpliceNumber = col1SpliceQuantity,
                Col2BplAttrib = cmbBoxCol2BplAFile.Text,
                Col2KneeAttrib = cmbBoxCol2KneeAFile.Text,
                Col2SpliceNumber = col2SpliceQuantity,
                Origin = origin,
                Slope = double.Parse(txtBoxSlope.Text),
                GableSteelLineFullLength = double.Parse(txtBoxGblFrmLength.Text),
                GableSteelLineHalfLength = double.Parse(txtBoxGblHalfRft.Text),
                GableSteelLineColumnHeight = double.Parse(txtBoxGblColHt.Text),
                GableColumnOffset = double.Parse(txtBoxGblColOffst.Text),
                GableRafterOffset = double.Parse(txtBoxGblRftOffst.Text),
                GableLeftBaseOffset = double.Parse(txtBoxGblLftBsOffst.Text),
                GableRightBaseOffset = double.Parse(txtBoxGblRghtBsOffst.Text),
                RidgeSpliceAttrib = cmbBoxRidgeSplice.Text,

                MonoSlopeSteelLineFullLength = double.Parse(txtBoxMnoFrmLength.Text),
                MonoSlopeSteelLineColumnHeight = double.Parse(txtBoxMnoColHt.Text),
                MonoSlopeColumnOffset = double.Parse(txtBoxMnoColOffst.Text),
                MonoSlopeRafterOffset = double.Parse(txtBoxMnoRftOffst.Text),
                MonoSlopeLeftBaseOffset = double.Parse(txtBoxMnoLftBsOffst.Text),
                MonoSlopeRightBaseOffset = double.Parse(txtBoxMnoRghtBsOffst.Text),

                BracingMode = (FlangeBraceMode)cmbBoxFlngBrcMode.SelectedIndex,

                Raf1SpliceNumber = raf1SpliceQuantity,
                Raf2SpliceNumber = raf2SpliceQuantity,

                EndBraceAttrib = cmbBoxEndFlangeBrce.Text,
                GeneralBraceAttrib = cmbBoxFlangeBrace.Text,

                Col1MemberAttribs = col1MemberAttribs,
                Col1SpliceAttribs = col1SpliceAttribs,
                Col1SpliceLengths = col1SpliceLengths,

                Col2MemberAttribs = col2MemberAttribs,
                Col2SpliceAttribs = col2SpliceAttribs,
                Col2SpliceLengths = col2SpliceLengths,

                Raf1MemberAttribs = raf1MemberAttribs,
                Raf1SpliceAttribs = raf1SpliceAttribs,
                Raf1SpliceLengths = raf1SpliceLengths,

                Raf2MemberAttribs = raf2MemberAttribs,
                Raf2SpliceAttribs = raf2SpliceAttribs,
                Raf2SpliceLengths = raf2SpliceLengths,
                LeftEaveOffset = double.Parse(txtBoxLeftEaveOffst.Text),
                RightEaveOffset = double.Parse(txtBoxRightEaveOffset.Text),
                HorizontalDistance = double.Parse(txtBoxHorzDist.Text),
                PurlinSection = txtBoxPrlnProf.Text,
                PurlinMaterial = txtBoxPrlnMtrl.Text,
                AtCoverLeft = flagOfCoverLeft,
                AtCoverRight = flagOfCoverRight,
                ClipAngleAttrib = cmbBoxClipAttribute.Text,
                CopyLengths = copies
            };

            return options;
        }

        private void btnDraw_Click(object sender, EventArgs e)
        {
            if (!ValidateLengthTextBoxes())
            {
                ErrorDialog.Show("Alert", "Inputs are not valid!", ErrorDialog.Severity.ERROR);
                return;
            }

            var currentPlane = GetCurrentCorSystem().TransformFromCurrentToGlobal();
            SetPlane();
            Picker pickMe = new Picker();
            ArrayList frameList = pickMe.PickPoints(Picker.PickPointEnum.PICK_TWO_POINTS, "Please choose frame direction");
            alongFrameVector = new Vector((frameList[1] as Point) - (frameList[0] as Point));

            ArrayList replicationList = pickMe.PickPoints(Picker.PickPointEnum.PICK_TWO_POINTS, "Please choose replication direction");
            replicationVector = new Vector((replicationList[1] as Point) - (replicationList[0] as Point)).GetNormal();
            FramingOptions options = PopulateFramingOption(frameList[0] as Point);
            if (imgListCmbBoxFrameMode.SelectedIndex == 0)
            {
                GableSetting gableSystem = new GableSetting(options);
                gableSystem.Draw();
            }
            else if (imgListCmbBoxFrameMode.SelectedIndex == 1)
            {
                MonoSlopeSetting monoSystem = new MonoSlopeSetting(options);
                monoSystem.Draw();
            }
            else
            {
                RadialFrameSetting radialSystem = new RadialFrameSetting(options);
                radialSystem.Draw();
            }
            SetPlane(currentPlane, TeklaGeometryExtender.ReferencePlane.GLOBAL);
            teklaModel.CommitChanges();
        }

        private void profileCatalog1_SelectionDone(object sender, EventArgs e)
        {
            txtBoxPrlnProf.Text = profileCatalog1.SelectedProfile;
        }

        private void materialCatalog1_SelectionDone(object sender, EventArgs e)
        {
            txtBoxPrlnMtrl.Text = materialCatalog1.SelectedMaterial;
        }
    }
}
