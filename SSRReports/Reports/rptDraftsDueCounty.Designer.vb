<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class rptDraftsDueCounty 
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
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(rptDraftsDueCounty))
        Me.PageHeader1 = New GrapeCity.ActiveReports.SectionReportModel.PageHeader
        Me.Detail1 = New GrapeCity.ActiveReports.SectionReportModel.Detail
        Me.txtCounty = New GrapeCity.ActiveReports.SectionReportModel.TextBox
        Me.PageFooter1 = New GrapeCity.ActiveReports.SectionReportModel.PageFooter
        CType(Me.txtCounty, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'PageHeader1
        '
        Me.PageHeader1.Height = 0.0!
        Me.PageHeader1.Name = "PageHeader1"
        '
        'Detail1
        '
        Me.Detail1.ColumnSpacing = 0.0!
        Me.Detail1.Controls.AddRange(New GrapeCity.ActiveReports.SectionReportModel.ARControl() {Me.txtCounty})
        Me.Detail1.Height = 0.1876667!
        Me.Detail1.Name = "Detail1"
        '
        'txtCounty
        '
        Me.txtCounty.DataField = "County"
        Me.txtCounty.Height = 0.2!
        Me.txtCounty.Left = 0.0!
        Me.txtCounty.Name = "txtCounty"
        Me.txtCounty.Style = "font-family: Tahoma; font-size: 9pt"
        Me.txtCounty.Text = "txtCounty"
        Me.txtCounty.Top = 0.0!
        Me.txtCounty.Width = 1.31!
        '
        'PageFooter1
        '
        Me.PageFooter1.Height = 0.0!
        Me.PageFooter1.Name = "PageFooter1"
        '
        'rptDraftsDueCounty
        '
        Me.MasterReport = False
        Me.PageSettings.PaperHeight = 11.0!
        Me.PageSettings.PaperWidth = 8.5!
        Me.PrintWidth = 1.31!
        Me.Sections.Add(Me.PageHeader1)
        Me.Sections.Add(Me.Detail1)
        Me.Sections.Add(Me.PageFooter1)

        CType(Me.txtCounty, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub
    Private WithEvents txtCounty As GrapeCity.ActiveReports.SectionReportModel.TextBox
End Class 
