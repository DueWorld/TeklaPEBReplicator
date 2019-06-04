namespace PEBReplicator
{
    using System;
    using System.Collections.Generic;
    using System.Collections;
    using System.Linq;
    using System.Windows.Forms;
    using Tekla.Structures.Geometry3d;
    using Tekla.Structures.Model.UI;
    using Tekla.Structures;
    using Tekla.Structures.Model;
    using Tekla.Structures.Dialog;
    using Tekla.Structures.Dialog.UIControls;

    public partial class TeklaForm : ApplicationFormBase
    {
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
        }

        private bool ValidateLengthTextBoxes()
        {
            foreach (var txtBox in lengthTextBoxes)
            {
                if (!double.TryParse(txtBox.Text, out double result))
                    return false;
            }
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

            this.cmbBoxEndFlangeBrce.SelectedIndex = 0;
            this.cmbBoxFlangeBrace.SelectedIndex = 0;
            this.cmbBoxFlngBrcMode.SelectedIndex = 0;
            this.cmbBoxSpliceNoCol1.SelectedIndex = 0;
            this.cmbBoxSpliceNoCol2.SelectedIndex = 0;
            this.cmbBoxSpliceNoRft1.SelectedIndex = 0;
            this.cmbBoxSpliceNoRft2.SelectedIndex = 0;
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

        private void btnCol1_Click(object sender, EventArgs e)
        {
            groupBoxCol1.Visible = true;
            groupBoxCol2.Visible = false;
            groupBoxRf2.Visible = false;
            groupBoxRf1.Visible = false;
            groupBoxCol1.Location = new System.Drawing.Point(161, 6);
        }

        private void InitializeFrameProp()
        {
            groupBoxCol1.Visible = true;
            groupBoxCol2.Visible = false;
            groupBoxRf2.Visible = false;
            groupBoxRf1.Visible = false;
            groupBoxCol1.Location = new System.Drawing.Point(161, 6);
        }

        private void btnRft1_Click(object sender, EventArgs e)
        {
            groupBoxCol1.Visible = false;
            groupBoxCol2.Visible = false;
            groupBoxRf2.Visible = false;
            groupBoxRf1.Visible = true;
            groupBoxRf1.Location = new System.Drawing.Point(161, 6);
        }

        private void btnRft2_Click(object sender, EventArgs e)
        {
            groupBoxCol1.Visible = false;
            groupBoxCol2.Visible = false;
            groupBoxRf2.Visible = true;
            groupBoxRf1.Visible = false;
            groupBoxRf2.Location = new System.Drawing.Point(161, 6);

        }

        private void btnCol2_Click(object sender, EventArgs e)
        {
            groupBoxCol1.Visible = false;
            groupBoxCol2.Visible = true;
            groupBoxRf2.Visible = false;
            groupBoxRf1.Visible = false;
            groupBoxCol2.Location = new System.Drawing.Point(161, 6);

        }
    }
}
