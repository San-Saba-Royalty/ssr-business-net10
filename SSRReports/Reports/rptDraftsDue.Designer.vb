<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class rptDraftsDue 
    Inherits GrapeCity.ActiveReports.SectionReport

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
        End If
        MyBase.Dispose(disposing)
    End Sub
    
    'NOTE: The following procedure is required by the ActiveReports Designer
    'It can be modified using the ActiveReports Designer.
    'Do not modify it using the code editor.
    Private WithEvents PageHeader1 As GrapeCity.ActiveReports.SectionReportModel.PageHeader
    Private WithEvents Detail1 As GrapeCity.ActiveReports.SectionReportModel.Detail
    Private WithEvents PageFooter1 As GrapeCity.ActiveReports.SectionReportModel.PageFooter
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(rptDraftsDue))
        Me.PageHeader1 = New GrapeCity.ActiveReports.SectionReportModel.PageHeader
        Me.lblReportName = New GrapeCity.ActiveReports.SectionReportModel.Label
        Me.lblSellerHeader = New GrapeCity.ActiveReports.SectionReportModel.Label
        Me.lblDueDateHeader = New GrapeCity.ActiveReports.SectionReportModel.Label
        Me.lblAmountHeader = New GrapeCity.ActiveReports.SectionReportModel.Label
        Me.lblDraftCheckNumber = New GrapeCity.ActiveReports.SectionReportModel.Label
        Me.lblDealStatus = New GrapeCity.ActiveReports.SectionReportModel.Label
        Me.lblCheckStubHeader = New GrapeCity.ActiveReports.SectionReportModel.Label
        Me.lblInvoiceNumberHeader = New GrapeCity.ActiveReports.SectionReportModel.Label
        Me.lblCountyHeader = New GrapeCity.ActiveReports.SectionReportModel.Label
        Me.lblBuyerHeader = New GrapeCity.ActiveReports.SectionReportModel.Label
        Me.txtRunDateTime = New GrapeCity.ActiveReports.SectionReportModel.TextBox
        Me.lblAssigneeHeader = New GrapeCity.ActiveReports.SectionReportModel.Label
        Me.lblLandManHeader = New GrapeCity.ActiveReports.SectionReportModel.Label
        Me.Detail1 = New GrapeCity.ActiveReports.SectionReportModel.Detail
        Me.txtSellerName = New GrapeCity.ActiveReports.SectionReportModel.TextBox
        Me.txtDueDate = New GrapeCity.ActiveReports.SectionReportModel.TextBox
        Me.txtTotalBonus = New GrapeCity.ActiveReports.SectionReportModel.TextBox
        Me.txtDraftCheckNumber = New GrapeCity.ActiveReports.SectionReportModel.TextBox
        Me.txtDealStatus = New GrapeCity.ActiveReports.SectionReportModel.TextBox
        Me.txtHaveCheckStub = New GrapeCity.ActiveReports.SectionReportModel.TextBox
        Me.txtInvoiceNumber = New GrapeCity.ActiveReports.SectionReportModel.TextBox
        Me.subrptCounty = New GrapeCity.ActiveReports.SectionReportModel.SubReport
        Me.subrptNote = New GrapeCity.ActiveReports.SectionReportModel.SubReport
        Me.txtAcquisitionID = New GrapeCity.ActiveReports.SectionReportModel.TextBox
        Me.Line1 = New GrapeCity.ActiveReports.SectionReportModel.Line
        Me.txtBuyer = New GrapeCity.ActiveReports.SectionReportModel.TextBox
        Me.txtAssignee = New GrapeCity.ActiveReports.SectionReportModel.TextBox
        Me.txtLandMan = New GrapeCity.ActiveReports.SectionReportModel.TextBox
        Me.PageFooter1 = New GrapeCity.ActiveReports.SectionReportModel.PageFooter
        Me.lblPageFooterPageNumber = New GrapeCity.ActiveReports.SectionReportModel.Label
        Me.txtPageNumber = New GrapeCity.ActiveReports.SectionReportModel.TextBox
        Me.ReportHeader1 = New GrapeCity.ActiveReports.SectionReportModel.ReportHeader
        Me.ReportFooter1 = New GrapeCity.ActiveReports.SectionReportModel.ReportFooter
        Me.lblTotalAmount = New GrapeCity.ActiveReports.SectionReportModel.Label
        Me.Label1 = New GrapeCity.ActiveReports.SectionReportModel.Label
        Me.txtTotalBonusFooter = New GrapeCity.ActiveReports.SectionReportModel.TextBox
        CType(Me.lblReportName, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.lblSellerHeader, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.lblDueDateHeader, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.lblAmountHeader, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.lblDraftCheckNumber, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.lblDealStatus, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.lblCheckStubHeader, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.lblInvoiceNumberHeader, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.lblCountyHeader, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.lblBuyerHeader, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtRunDateTime, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.lblAssigneeHeader, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.lblLandManHeader, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtSellerName, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtDueDate, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtTotalBonus, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtDraftCheckNumber, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtDealStatus, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtHaveCheckStub, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtInvoiceNumber, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtAcquisitionID, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtBuyer, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtAssignee, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtLandMan, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.lblPageFooterPageNumber, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtPageNumber, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.lblTotalAmount, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Label1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtTotalBonusFooter, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'PageHeader1
        '
        Me.PageHeader1.Controls.AddRange(New GrapeCity.ActiveReports.SectionReportModel.ARControl() {Me.lblReportName, Me.lblSellerHeader, Me.lblDueDateHeader, Me.lblAmountHeader, Me.lblDraftCheckNumber, Me.lblDealStatus, Me.lblCheckStubHeader, Me.lblInvoiceNumberHeader, Me.lblCountyHeader, Me.lblBuyerHeader, Me.txtRunDateTime, Me.lblAssigneeHeader, Me.lblLandManHeader})
        Me.PageHeader1.Height = 0.9082501!
        Me.PageHeader1.Name = "PageHeader1"
        '
        'lblReportName
        '
        Me.lblReportName.Height = 0.324!
        Me.lblReportName.HyperLink = Nothing
        Me.lblReportName.Left = 0.1289999!
        Me.lblReportName.Name = "lblReportName"
        Me.lblReportName.Style = "color: #1C3A70; font-family: Tahoma; font-size: 18pt; text-align: left"
        Me.lblReportName.Text = "Drafts Due"
        Me.lblReportName.Top = 0.0!
        Me.lblReportName.Width = 8.06!
        '
        'lblSellerHeader
        '
        Me.lblSellerHeader.Border.BottomColor = System.Drawing.Color.White
        Me.lblSellerHeader.Border.LeftColor = System.Drawing.Color.White
        Me.lblSellerHeader.Border.RightColor = System.Drawing.Color.White
        Me.lblSellerHeader.Border.TopColor = System.Drawing.Color.White
        Me.lblSellerHeader.Height = 0.336!
        Me.lblSellerHeader.HyperLink = Nothing
        Me.lblSellerHeader.Left = 0.129!
        Me.lblSellerHeader.Name = "lblSellerHeader"
        Me.lblSellerHeader.Style = "background-color: #1C3A70; color: White; font-family: Tahoma; font-size: 9pt; fon" & _
            "t-weight: bold; vertical-align: bottom; ddo-char-set: 0"
        Me.lblSellerHeader.Text = "Seller"
        Me.lblSellerHeader.Top = 0.466!
        Me.lblSellerHeader.Width = 1.641!
        '
        'lblDueDateHeader
        '
        Me.lblDueDateHeader.Border.BottomColor = System.Drawing.Color.White
        Me.lblDueDateHeader.Border.LeftColor = System.Drawing.Color.White
        Me.lblDueDateHeader.Border.RightColor = System.Drawing.Color.White
        Me.lblDueDateHeader.Border.TopColor = System.Drawing.Color.White
        Me.lblDueDateHeader.Height = 0.336!
        Me.lblDueDateHeader.HyperLink = Nothing
        Me.lblDueDateHeader.Left = 1.816993!
        Me.lblDueDateHeader.Name = "lblDueDateHeader"
        Me.lblDueDateHeader.Style = "background-color: #1C3A70; color: White; font-family: Tahoma; font-size: 9pt; fon" & _
            "t-weight: bold; vertical-align: bottom; ddo-char-set: 0"
        Me.lblDueDateHeader.Text = "Due Date"
        Me.lblDueDateHeader.Top = 0.466!
        Me.lblDueDateHeader.Width = 0.7140002!
        '
        'lblAmountHeader
        '
        Me.lblAmountHeader.Border.BottomColor = System.Drawing.Color.White
        Me.lblAmountHeader.Border.LeftColor = System.Drawing.Color.White
        Me.lblAmountHeader.Border.RightColor = System.Drawing.Color.White
        Me.lblAmountHeader.Border.TopColor = System.Drawing.Color.White
        Me.lblAmountHeader.Height = 0.336!
        Me.lblAmountHeader.HyperLink = Nothing
        Me.lblAmountHeader.Left = 2.592993!
        Me.lblAmountHeader.Name = "lblAmountHeader"
        Me.lblAmountHeader.Style = "background-color: #1C3A70; color: White; font-family: Tahoma; font-size: 9pt; fon" & _
            "t-weight: bold; text-align: right; vertical-align: bottom; ddo-char-set: 0"
        Me.lblAmountHeader.Text = "Amount"
        Me.lblAmountHeader.Top = 0.466!
        Me.lblAmountHeader.Width = 1.058!
        '
        'lblDraftCheckNumber
        '
        Me.lblDraftCheckNumber.Border.BottomColor = System.Drawing.Color.White
        Me.lblDraftCheckNumber.Border.LeftColor = System.Drawing.Color.White
        Me.lblDraftCheckNumber.Border.RightColor = System.Drawing.Color.White
        Me.lblDraftCheckNumber.Border.TopColor = System.Drawing.Color.White
        Me.lblDraftCheckNumber.Height = 0.336!
        Me.lblDraftCheckNumber.HyperLink = Nothing
        Me.lblDraftCheckNumber.Left = 3.703!
        Me.lblDraftCheckNumber.Name = "lblDraftCheckNumber"
        Me.lblDraftCheckNumber.Style = "background-color: #1C3A70; color: White; font-family: Tahoma; font-size: 9pt; fon" & _
            "t-weight: bold; vertical-align: bottom; ddo-char-set: 0"
        Me.lblDraftCheckNumber.Text = "Draft Check #"
        Me.lblDraftCheckNumber.Top = 0.466!
        Me.lblDraftCheckNumber.Width = 1.193!
        '
        'lblDealStatus
        '
        Me.lblDealStatus.Border.BottomColor = System.Drawing.Color.White
        Me.lblDealStatus.Border.LeftColor = System.Drawing.Color.White
        Me.lblDealStatus.Border.RightColor = System.Drawing.Color.White
        Me.lblDealStatus.Border.TopColor = System.Drawing.Color.White
        Me.lblDealStatus.Height = 0.336!
        Me.lblDealStatus.HyperLink = Nothing
        Me.lblDealStatus.Left = 4.938011!
        Me.lblDealStatus.Name = "lblDealStatus"
        Me.lblDealStatus.Style = "background-color: #1C3A70; color: White; font-family: Tahoma; font-size: 9pt; fon" & _
            "t-weight: bold; vertical-align: bottom; ddo-char-set: 0"
        Me.lblDealStatus.Text = "Deal Status"
        Me.lblDealStatus.Top = 0.466!
        Me.lblDealStatus.Width = 1.253975!
        '
        'lblCheckStubHeader
        '
        Me.lblCheckStubHeader.Border.BottomColor = System.Drawing.Color.White
        Me.lblCheckStubHeader.Border.LeftColor = System.Drawing.Color.White
        Me.lblCheckStubHeader.Border.RightColor = System.Drawing.Color.White
        Me.lblCheckStubHeader.Border.TopColor = System.Drawing.Color.White
        Me.lblCheckStubHeader.Height = 0.336!
        Me.lblCheckStubHeader.HyperLink = Nothing
        Me.lblCheckStubHeader.Left = 6.235987!
        Me.lblCheckStubHeader.Name = "lblCheckStubHeader"
        Me.lblCheckStubHeader.Style = "background-color: #1C3A70; color: White; font-family: Tahoma; font-size: 9pt; fon" & _
            "t-weight: bold; vertical-align: bottom; ddo-char-set: 0"
        Me.lblCheckStubHeader.Text = "Check Stub"
        Me.lblCheckStubHeader.Top = 0.466!
        Me.lblCheckStubHeader.Width = 0.432013!
        '
        'lblInvoiceNumberHeader
        '
        Me.lblInvoiceNumberHeader.Border.BottomColor = System.Drawing.Color.White
        Me.lblInvoiceNumberHeader.Border.LeftColor = System.Drawing.Color.White
        Me.lblInvoiceNumberHeader.Border.RightColor = System.Drawing.Color.White
        Me.lblInvoiceNumberHeader.Border.TopColor = System.Drawing.Color.White
        Me.lblInvoiceNumberHeader.Height = 0.3360001!
        Me.lblInvoiceNumberHeader.HyperLink = Nothing
        Me.lblInvoiceNumberHeader.Left = 9.762001!
        Me.lblInvoiceNumberHeader.Name = "lblInvoiceNumberHeader"
        Me.lblInvoiceNumberHeader.Style = "background-color: #1C3A70; color: White; font-family: Tahoma; font-size: 9pt; fon" & _
            "t-weight: bold; vertical-align: bottom; ddo-char-set: 0"
        Me.lblInvoiceNumberHeader.Text = "Inv #"
        Me.lblInvoiceNumberHeader.Top = 0.466!
        Me.lblInvoiceNumberHeader.Width = 0.4629993!
        '
        'lblCountyHeader
        '
        Me.lblCountyHeader.Border.BottomColor = System.Drawing.Color.White
        Me.lblCountyHeader.Border.LeftColor = System.Drawing.Color.White
        Me.lblCountyHeader.Border.RightColor = System.Drawing.Color.White
        Me.lblCountyHeader.Border.TopColor = System.Drawing.Color.White
        Me.lblCountyHeader.Height = 0.336!
        Me.lblCountyHeader.HyperLink = Nothing
        Me.lblCountyHeader.Left = 10.275!
        Me.lblCountyHeader.Name = "lblCountyHeader"
        Me.lblCountyHeader.Style = "background-color: #1C3A70; color: White; font-family: Tahoma; font-size: 9pt; fon" & _
            "t-weight: bold; vertical-align: bottom; ddo-char-set: 0"
        Me.lblCountyHeader.Text = "County"
        Me.lblCountyHeader.Top = 0.466!
        Me.lblCountyHeader.Width = 1.313!
        '
        'lblBuyerHeader
        '
        Me.lblBuyerHeader.Height = 0.3360001!
        Me.lblBuyerHeader.HyperLink = Nothing
        Me.lblBuyerHeader.Left = 6.740012!
        Me.lblBuyerHeader.Name = "lblBuyerHeader"
        Me.lblBuyerHeader.Style = "background-color: #1C3A70; color: White; font-family: Tahoma; font-size: 9pt; fon" & _
            "t-weight: bold; vertical-align: bottom; ddo-char-set: 0"
        Me.lblBuyerHeader.Text = "Buyer"
        Me.lblBuyerHeader.Top = 0.466!
        Me.lblBuyerHeader.Width = 1.748989!
        '
        'txtRunDateTime
        '
        Me.txtRunDateTime.Height = 0.2!
        Me.txtRunDateTime.Left = 11.089!
        Me.txtRunDateTime.Name = "txtRunDateTime"
        Me.txtRunDateTime.Style = "font-family: Tahoma; font-size: 8.25pt; font-weight: normal; text-align: right; d" & _
            "do-char-set: 0"
        Me.txtRunDateTime.Text = "txtRunDateTime"
        Me.txtRunDateTime.Top = 0.0!
        Me.txtRunDateTime.Width = 1.839!
        '
        'lblAssigneeHeader
        '
        Me.lblAssigneeHeader.Height = 0.3360001!
        Me.lblAssigneeHeader.HyperLink = Nothing
        Me.lblAssigneeHeader.Left = 8.541001!
        Me.lblAssigneeHeader.Name = "lblAssigneeHeader"
        Me.lblAssigneeHeader.Style = "background-color: #1C3A70; color: White; font-family: Tahoma; font-size: 9pt; fon" & _
            "t-weight: bold; vertical-align: bottom; ddo-char-set: 0"
        Me.lblAssigneeHeader.Text = "Assignee"
        Me.lblAssigneeHeader.Top = 0.466!
        Me.lblAssigneeHeader.Width = 1.165999!
        '
        'lblLandManHeader
        '
        Me.lblLandManHeader.Height = 0.3360001!
        Me.lblLandManHeader.HyperLink = Nothing
        Me.lblLandManHeader.Left = 11.637!
        Me.lblLandManHeader.Name = "lblLandManHeader"
        Me.lblLandManHeader.Style = "background-color: #1C3A70; color: White; font-family: Tahoma; font-size: 9pt; fon" & _
            "t-weight: bold; vertical-align: bottom; ddo-char-set: 0"
        Me.lblLandManHeader.Text = "LandMan"
        Me.lblLandManHeader.Top = 0.466!
        Me.lblLandManHeader.Width = 1.291!
        '
        'Detail1
        '
        Me.Detail1.CanShrink = True
        Me.Detail1.ColumnSpacing = 0.0!
        Me.Detail1.Controls.AddRange(New GrapeCity.ActiveReports.SectionReportModel.ARControl() {Me.txtSellerName, Me.txtDueDate, Me.txtTotalBonus, Me.txtDraftCheckNumber, Me.txtDealStatus, Me.txtHaveCheckStub, Me.txtInvoiceNumber, Me.subrptCounty, Me.subrptNote, Me.txtAcquisitionID, Me.Line1, Me.txtBuyer, Me.txtAssignee, Me.txtLandMan})
        Me.Detail1.Height = 1.094!
        Me.Detail1.KeepTogether = True
        Me.Detail1.Name = "Detail1"
        '
        'txtSellerName
        '
        Me.txtSellerName.DataField = "SellerName"
        Me.txtSellerName.Height = 0.2!
        Me.txtSellerName.Left = 0.129!
        Me.txtSellerName.Name = "txtSellerName"
        Me.txtSellerName.Style = "font-family: Tahoma; font-size: 9pt; ddo-char-set: 0"
        Me.txtSellerName.Text = "txtSellerName"
        Me.txtSellerName.Top = 0.07!
        Me.txtSellerName.Width = 1.641!
        '
        'txtDueDate
        '
        Me.txtDueDate.DataField = "DueDate"
        Me.txtDueDate.Height = 0.2!
        Me.txtDueDate.Left = 1.81698!
        Me.txtDueDate.Name = "txtDueDate"
        Me.txtDueDate.OutputFormat = resources.GetString("txtDueDate.OutputFormat")
        Me.txtDueDate.Style = "font-family: Tahoma; font-size: 9pt; ddo-char-set: 0"
        Me.txtDueDate.Text = "txtDueDate"
        Me.txtDueDate.Top = 0.06999999!
        Me.txtDueDate.Width = 0.7139999!
        '
        'txtTotalBonus
        '
        Me.txtTotalBonus.DataField = "TotalBonus"
        Me.txtTotalBonus.Height = 0.2!
        Me.txtTotalBonus.Left = 2.59298!
        Me.txtTotalBonus.Name = "txtTotalBonus"
        Me.txtTotalBonus.OutputFormat = resources.GetString("txtTotalBonus.OutputFormat")
        Me.txtTotalBonus.Style = "font-family: Tahoma; font-size: 9pt; text-align: right; ddo-char-set: 0"
        Me.txtTotalBonus.Text = "txtTotalBonus"
        Me.txtTotalBonus.Top = 0.06999999!
        Me.txtTotalBonus.Width = 1.058!
        '
        'txtDraftCheckNumber
        '
        Me.txtDraftCheckNumber.DataField = "DraftCheckNumber"
        Me.txtDraftCheckNumber.Height = 0.2!
        Me.txtDraftCheckNumber.Left = 3.702987!
        Me.txtDraftCheckNumber.Name = "txtDraftCheckNumber"
        Me.txtDraftCheckNumber.OutputFormat = resources.GetString("txtDraftCheckNumber.OutputFormat")
        Me.txtDraftCheckNumber.Style = "font-family: Tahoma; font-size: 9pt; text-align: left; ddo-char-set: 0"
        Me.txtDraftCheckNumber.Text = "txtDraftCheckNumber"
        Me.txtDraftCheckNumber.Top = 0.06999999!
        Me.txtDraftCheckNumber.Width = 1.193!
        '
        'txtDealStatus
        '
        Me.txtDealStatus.DataField = "DealStatus"
        Me.txtDealStatus.Height = 0.2!
        Me.txtDealStatus.Left = 4.938!
        Me.txtDealStatus.Name = "txtDealStatus"
        Me.txtDealStatus.OutputFormat = resources.GetString("txtDealStatus.OutputFormat")
        Me.txtDealStatus.Style = "font-family: Tahoma; font-size: 9pt; text-align: left; ddo-char-set: 0"
        Me.txtDealStatus.Text = "txtDealStatus"
        Me.txtDealStatus.Top = 0.06999999!
        Me.txtDealStatus.Width = 1.253975!
        '
        'txtHaveCheckStub
        '
        Me.txtHaveCheckStub.DataField = "HaveCheckStub"
        Me.txtHaveCheckStub.Height = 0.2!
        Me.txtHaveCheckStub.Left = 6.236!
        Me.txtHaveCheckStub.Name = "txtHaveCheckStub"
        Me.txtHaveCheckStub.OutputFormat = resources.GetString("txtHaveCheckStub.OutputFormat")
        Me.txtHaveCheckStub.Style = "font-family: Tahoma; font-size: 9pt; text-align: left; ddo-char-set: 0"
        Me.txtHaveCheckStub.Text = "txtHaveCheckStub"
        Me.txtHaveCheckStub.Top = 0.07!
        Me.txtHaveCheckStub.Width = 0.4320002!
        '
        'txtInvoiceNumber
        '
        Me.txtInvoiceNumber.DataField = "InvoiceNumber"
        Me.txtInvoiceNumber.Height = 0.2!
        Me.txtInvoiceNumber.Left = 9.762!
        Me.txtInvoiceNumber.Name = "txtInvoiceNumber"
        Me.txtInvoiceNumber.OutputFormat = resources.GetString("txtInvoiceNumber.OutputFormat")
        Me.txtInvoiceNumber.Style = "font-family: Tahoma; font-size: 9pt; text-align: left; ddo-char-set: 0"
        Me.txtInvoiceNumber.Text = "txtInvoiceNumber"
        Me.txtInvoiceNumber.Top = 0.07000007!
        Me.txtInvoiceNumber.Width = 0.4630003!
        '
        'subrptCounty
        '
        Me.subrptCounty.CloseBorder = False
        Me.subrptCounty.Height = 0.2!
        Me.subrptCounty.Left = 10.275!
        Me.subrptCounty.Name = "subrptCounty"
        Me.subrptCounty.Report = Nothing
        Me.subrptCounty.ReportName = "rptDraftsDueCounty"
        Me.subrptCounty.Top = 0.07000005!
        Me.subrptCounty.Width = 1.313001!
        '
        'subrptNote
        '
        Me.subrptNote.CloseBorder = False
        Me.subrptNote.Height = 0.2!
        Me.subrptNote.Left = 0.1289997!
        Me.subrptNote.Name = "subrptNote"
        Me.subrptNote.Report = Nothing
        Me.subrptNote.ReportName = "rptDraftsDueNote"
        Me.subrptNote.Top = 0.33!
        Me.subrptNote.Width = 12.799!
        '
        'txtAcquisitionID
        '
        Me.txtAcquisitionID.DataField = "AcquisitionID"
        Me.txtAcquisitionID.Height = 0.2!
        Me.txtAcquisitionID.Left = 0.129!
        Me.txtAcquisitionID.Name = "txtAcquisitionID"
        Me.txtAcquisitionID.Text = "txtAcquisitionID"
        Me.txtAcquisitionID.Top = 0.8940001!
        Me.txtAcquisitionID.Visible = False
        Me.txtAcquisitionID.Width = 1.0!
        '
        'Line1
        '
        Me.Line1.Height = 0.0!
        Me.Line1.Left = 0.129!
        Me.Line1.LineWeight = 1.0!
        Me.Line1.Name = "Line1"
        Me.Line1.Top = 0.0!
        Me.Line1.Width = 12.799!
        Me.Line1.X1 = 0.129!
        Me.Line1.X2 = 12.928!
        Me.Line1.Y1 = 0.0!
        Me.Line1.Y2 = 0.0!
        '
        'txtBuyer
        '
        Me.txtBuyer.DataField = "BuyerName"
        Me.txtBuyer.Height = 0.2!
        Me.txtBuyer.Left = 6.739999!
        Me.txtBuyer.Name = "txtBuyer"
        Me.txtBuyer.OutputFormat = resources.GetString("txtBuyer.OutputFormat")
        Me.txtBuyer.Style = "font-family: Tahoma; font-size: 9pt; text-align: left; ddo-char-set: 0"
        Me.txtBuyer.Text = "txtBuyer"
        Me.txtBuyer.Top = 0.06999998!
        Me.txtBuyer.Width = 1.749!
        '
        'txtAssignee
        '
        Me.txtAssignee.DataField = "Assignee"
        Me.txtAssignee.Height = 0.2!
        Me.txtAssignee.Left = 8.541!
        Me.txtAssignee.Name = "txtAssignee"
        Me.txtAssignee.OutputFormat = resources.GetString("txtAssignee.OutputFormat")
        Me.txtAssignee.Style = "font-family: Tahoma; font-size: 9pt; text-align: left; ddo-char-set: 0"
        Me.txtAssignee.Text = "txtAssignee"
        Me.txtAssignee.Top = 0.07!
        Me.txtAssignee.Width = 1.166!
        '
        'txtLandMan
        '
        Me.txtLandMan.DataField = "LandMan"
        Me.txtLandMan.Height = 0.2!
        Me.txtLandMan.Left = 11.637!
        Me.txtLandMan.Name = "txtLandMan"
        Me.txtLandMan.OutputFormat = resources.GetString("txtLandMan.OutputFormat")
        Me.txtLandMan.Style = "font-family: Tahoma; font-size: 9pt; text-align: left; ddo-char-set: 0"
        Me.txtLandMan.Text = "txtLandMan"
        Me.txtLandMan.Top = 0.07!
        Me.txtLandMan.Width = 1.291!
        '
        'PageFooter1
        '
        Me.PageFooter1.Controls.AddRange(New GrapeCity.ActiveReports.SectionReportModel.ARControl() {Me.lblPageFooterPageNumber, Me.txtPageNumber})
        Me.PageFooter1.Height = 0.3230833!
        Me.PageFooter1.Name = "PageFooter1"
        '
        'lblPageFooterPageNumber
        '
        Me.lblPageFooterPageNumber.Height = 0.2!
        Me.lblPageFooterPageNumber.HyperLink = Nothing
        Me.lblPageFooterPageNumber.Left = 11.911!
        Me.lblPageFooterPageNumber.Name = "lblPageFooterPageNumber"
        Me.lblPageFooterPageNumber.Style = "font-family: Tahoma; font-size: 9pt; text-align: right; ddo-char-set: 0"
        Me.lblPageFooterPageNumber.Text = "Page:"
        Me.lblPageFooterPageNumber.Top = 0.071!
        Me.lblPageFooterPageNumber.Width = 0.5099993!
        '
        'txtPageNumber
        '
        Me.txtPageNumber.Height = 0.2!
        Me.txtPageNumber.Left = 12.462!
        Me.txtPageNumber.Name = "txtPageNumber"
        Me.txtPageNumber.Style = "font-family: Tahoma; font-size: 9pt; text-align: right; ddo-char-set: 0"
        Me.txtPageNumber.SummaryFunc = GrapeCity.ActiveReports.SectionReportModel.SummaryFunc.Count
        Me.txtPageNumber.SummaryRunning = GrapeCity.ActiveReports.SectionReportModel.SummaryRunning.All
        Me.txtPageNumber.SummaryType = GrapeCity.ActiveReports.SectionReportModel.SummaryType.PageCount
        Me.txtPageNumber.Text = "txtPageNumber"
        Me.txtPageNumber.Top = 0.071!
        Me.txtPageNumber.Width = 0.4659996!
        '
        'ReportHeader1
        '
        Me.ReportHeader1.Height = 0.0!
        Me.ReportHeader1.Name = "ReportHeader1"
        '
        'ReportFooter1
        '
        Me.ReportFooter1.Controls.AddRange(New GrapeCity.ActiveReports.SectionReportModel.ARControl() {Me.lblTotalAmount, Me.Label1, Me.txtTotalBonusFooter})
        Me.ReportFooter1.Height = 0.28125!
        Me.ReportFooter1.Name = "ReportFooter1"
        '
        'lblTotalAmount
        '
        Me.lblTotalAmount.Height = 0.2!
        Me.lblTotalAmount.HyperLink = Nothing
        Me.lblTotalAmount.Left = 0.129!
        Me.lblTotalAmount.Name = "lblTotalAmount"
        Me.lblTotalAmount.Style = "background-color: #60759B; color: White; font-family: Tahoma; font-size: 9pt; fon" & _
            "t-weight: bold; vertical-align: middle; ddo-char-set: 0"
        Me.lblTotalAmount.Text = "Total Amount"
        Me.lblTotalAmount.Top = 0.0!
        Me.lblTotalAmount.Width = 1.308!
        '
        'Label1
        '
        Me.Label1.Height = 0.2!
        Me.Label1.HyperLink = Nothing
        Me.Label1.Left = 1.437!
        Me.Label1.Name = "Label1"
        Me.Label1.Style = "background-color: #60759B; color: White; font-family: Tahoma; font-size: 9pt; fon" & _
            "t-weight: bold; vertical-align: middle; ddo-char-set: 0"
        Me.Label1.Text = ""
        Me.Label1.Top = 0.0!
        Me.Label1.Width = 11.491!
        '
        'txtTotalBonusFooter
        '
        Me.txtTotalBonusFooter.DataField = "TotalBonus"
        Me.txtTotalBonusFooter.Height = 0.2!
        Me.txtTotalBonusFooter.Left = 1.817!
        Me.txtTotalBonusFooter.Name = "txtTotalBonusFooter"
        Me.txtTotalBonusFooter.OutputFormat = resources.GetString("txtTotalBonusFooter.OutputFormat")
        Me.txtTotalBonusFooter.Style = "background-color: #60759B; color: White; font-family: Tahoma; font-size: 9pt; fon" & _
            "t-weight: bold; text-align: right; vertical-align: middle; ddo-char-set: 0; ddo-" & _
            "font-vertical: none"
        Me.txtTotalBonusFooter.SummaryRunning = GrapeCity.ActiveReports.SectionReportModel.SummaryRunning.All
        Me.txtTotalBonusFooter.SummaryType = GrapeCity.ActiveReports.SectionReportModel.SummaryType.GrandTotal
        Me.txtTotalBonusFooter.Text = "txtTotalBonusFooter"
        Me.txtTotalBonusFooter.Top = 0.0!
        Me.txtTotalBonusFooter.Width = 1.834!
        '
        'rptDraftsDue
        '
        Me.MasterReport = False
        Me.PageSettings.Margins.Bottom = 0.3!
        Me.PageSettings.Margins.Left = 0.3!
        Me.PageSettings.Margins.Right = 0.3!
        Me.PageSettings.Margins.Top = 0.3!
        Me.PageSettings.Orientation = GrapeCity.ActiveReports.Document.Section.PageOrientation.Landscape
        Me.PageSettings.PaperHeight = 11.0!
        Me.PageSettings.PaperWidth = 8.5!
        Me.PrintWidth = 13.01083!
        Me.Sections.Add(Me.ReportHeader1)
        Me.Sections.Add(Me.PageHeader1)
        Me.Sections.Add(Me.Detail1)
        Me.Sections.Add(Me.PageFooter1)
        Me.Sections.Add(Me.ReportFooter1)

        CType(Me.lblReportName, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.lblSellerHeader, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.lblDueDateHeader, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.lblAmountHeader, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.lblDraftCheckNumber, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.lblDealStatus, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.lblCheckStubHeader, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.lblInvoiceNumberHeader, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.lblCountyHeader, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.lblBuyerHeader, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtRunDateTime, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.lblAssigneeHeader, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.lblLandManHeader, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtSellerName, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtDueDate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtTotalBonus, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtDraftCheckNumber, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtDealStatus, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtHaveCheckStub, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtInvoiceNumber, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtAcquisitionID, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtBuyer, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtAssignee, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtLandMan, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.lblPageFooterPageNumber, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtPageNumber, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.lblTotalAmount, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Label1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtTotalBonusFooter, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub
    Private WithEvents lblReportName As GrapeCity.ActiveReports.SectionReportModel.Label
    Private WithEvents lblInvoiceNumberHeader As GrapeCity.ActiveReports.SectionReportModel.Label
    Private WithEvents lblPageFooterPageNumber As GrapeCity.ActiveReports.SectionReportModel.Label
    Private WithEvents ReportHeader1 As GrapeCity.ActiveReports.SectionReportModel.ReportHeader
    Private WithEvents ReportFooter1 As GrapeCity.ActiveReports.SectionReportModel.ReportFooter
    Private WithEvents txtPageNumber As GrapeCity.ActiveReports.SectionReportModel.TextBox
    Private WithEvents txtSellerName As GrapeCity.ActiveReports.SectionReportModel.TextBox
    Private WithEvents txtDueDate As GrapeCity.ActiveReports.SectionReportModel.TextBox
    Private WithEvents txtTotalBonus As GrapeCity.ActiveReports.SectionReportModel.TextBox
    Private WithEvents txtDraftCheckNumber As GrapeCity.ActiveReports.SectionReportModel.TextBox
    Private WithEvents txtDealStatus As GrapeCity.ActiveReports.SectionReportModel.TextBox
    Private WithEvents txtHaveCheckStub As GrapeCity.ActiveReports.SectionReportModel.TextBox
    Private WithEvents txtInvoiceNumber As GrapeCity.ActiveReports.SectionReportModel.TextBox
    Private WithEvents subrptCounty As GrapeCity.ActiveReports.SectionReportModel.SubReport
    Private WithEvents subrptNote As GrapeCity.ActiveReports.SectionReportModel.SubReport
    Private WithEvents txtAcquisitionID As GrapeCity.ActiveReports.SectionReportModel.TextBox
    Private WithEvents lblTotalAmount As GrapeCity.ActiveReports.SectionReportModel.Label
    Private WithEvents txtTotalBonusFooter As GrapeCity.ActiveReports.SectionReportModel.TextBox
    Private WithEvents lblBuyerHeader As GrapeCity.ActiveReports.SectionReportModel.Label
    Private WithEvents txtBuyer As GrapeCity.ActiveReports.SectionReportModel.TextBox
    Private WithEvents Line1 As GrapeCity.ActiveReports.SectionReportModel.Line
    Private WithEvents txtRunDateTime As GrapeCity.ActiveReports.SectionReportModel.TextBox
    Private WithEvents lblAssigneeHeader As GrapeCity.ActiveReports.SectionReportModel.Label
    Private WithEvents txtAssignee As GrapeCity.ActiveReports.SectionReportModel.TextBox
    Private WithEvents txtLandMan As GrapeCity.ActiveReports.SectionReportModel.TextBox
    Private WithEvents Label1 As GrapeCity.ActiveReports.SectionReportModel.Label
    Private WithEvents lblSellerHeader As GrapeCity.ActiveReports.SectionReportModel.Label
    Private WithEvents lblDueDateHeader As GrapeCity.ActiveReports.SectionReportModel.Label
    Private WithEvents lblAmountHeader As GrapeCity.ActiveReports.SectionReportModel.Label
    Private WithEvents lblDraftCheckNumber As GrapeCity.ActiveReports.SectionReportModel.Label
    Private WithEvents lblDealStatus As GrapeCity.ActiveReports.SectionReportModel.Label
    Private WithEvents lblCheckStubHeader As GrapeCity.ActiveReports.SectionReportModel.Label
    Private WithEvents lblCountyHeader As GrapeCity.ActiveReports.SectionReportModel.Label
    Private WithEvents lblLandManHeader As GrapeCity.ActiveReports.SectionReportModel.Label
End Class
