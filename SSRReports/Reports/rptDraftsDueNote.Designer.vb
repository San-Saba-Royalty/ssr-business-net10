<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class rptDraftsDueNote 
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
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(rptDraftsDueNote))
        Me.PageHeader1 = New GrapeCity.ActiveReports.SectionReportModel.PageHeader
        Me.Detail1 = New GrapeCity.ActiveReports.SectionReportModel.Detail
        Me.txtNote = New GrapeCity.ActiveReports.SectionReportModel.TextBox
        Me.txtNoteCreatedBy = New GrapeCity.ActiveReports.SectionReportModel.TextBox
        Me.txtNoteCreatedOn = New GrapeCity.ActiveReports.SectionReportModel.TextBox
        Me.PageFooter1 = New GrapeCity.ActiveReports.SectionReportModel.PageFooter
        CType(Me.txtNote, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtNoteCreatedBy, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtNoteCreatedOn, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.Detail1.Controls.AddRange(New GrapeCity.ActiveReports.SectionReportModel.ARControl() {Me.txtNote, Me.txtNoteCreatedBy, Me.txtNoteCreatedOn})
        Me.Detail1.Height = 0.188!
        Me.Detail1.Name = "Detail1"
        '
        'txtNote
        '
        Me.txtNote.DataField = "Note"
        Me.txtNote.Height = 0.2!
        Me.txtNote.Left = 4.204004!
        Me.txtNote.Name = "txtNote"
        Me.txtNote.Style = "font-family: Tahoma; font-size: 9pt"
        Me.txtNote.Text = "txtNote"
        Me.txtNote.Top = 0.0!
        Me.txtNote.Width = 8.478996!
        '
        'txtNoteCreatedBy
        '
        Me.txtNoteCreatedBy.DataField = "NoteCreatedBy"
        Me.txtNoteCreatedBy.Height = 0.2!
        Me.txtNoteCreatedBy.Left = 2.6!
        Me.txtNoteCreatedBy.Name = "txtNoteCreatedBy"
        Me.txtNoteCreatedBy.Style = "font-family: Tahoma; font-size: 9pt"
        Me.txtNoteCreatedBy.Text = "txtNoteCreatedBy"
        Me.txtNoteCreatedBy.Top = 0.0!
        Me.txtNoteCreatedBy.Width = 1.552!
        '
        'txtNoteCreatedOn
        '
        Me.txtNoteCreatedOn.DataField = "NoteCreatedOn"
        Me.txtNoteCreatedOn.Height = 0.2!
        Me.txtNoteCreatedOn.Left = 0.96!
        Me.txtNoteCreatedOn.Name = "txtNoteCreatedOn"
        Me.txtNoteCreatedOn.OutputFormat = resources.GetString("txtNoteCreatedOn.OutputFormat")
        Me.txtNoteCreatedOn.Style = "font-family: Tahoma; font-size: 9pt"
        Me.txtNoteCreatedOn.Text = "txtNoteCreatedOn"
        Me.txtNoteCreatedOn.Top = 0.0!
        Me.txtNoteCreatedOn.Width = 1.583!
        '
        'PageFooter1
        '
        Me.PageFooter1.Height = 0.0!
        Me.PageFooter1.Name = "PageFooter1"
        '
        'rptDraftsDueNote
        '
        Me.MasterReport = False
        Me.PageSettings.PaperHeight = 11.0!
        Me.PageSettings.PaperWidth = 8.5!
        Me.PrintWidth = 12.8!
        Me.Sections.Add(Me.PageHeader1)
        Me.Sections.Add(Me.Detail1)
        Me.Sections.Add(Me.PageFooter1)

        CType(Me.txtNote, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtNoteCreatedBy, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtNoteCreatedOn, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub
    Private WithEvents txtNote As GrapeCity.ActiveReports.SectionReportModel.TextBox
    Private WithEvents txtNoteCreatedBy As GrapeCity.ActiveReports.SectionReportModel.TextBox
    Private WithEvents txtNoteCreatedOn As GrapeCity.ActiveReports.SectionReportModel.TextBox
End Class 
