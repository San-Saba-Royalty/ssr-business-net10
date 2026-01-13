Imports GrapeCity.ActiveReports 
Imports GrapeCity.ActiveReports.Document 
Imports SSRBusiness.BusinessClasses

Public Class rptDraftsDue
    ' Implements ISsrReport ' Removed for now

    Private _sortOrder As String
    Public Property SortOrder() As String
        Get
            Return _sortOrder
        End Get
        Set(ByVal value As String)
            _sortOrder = value
        End Set
    End Property

    Private _includeNotes As Boolean = True
    Public Property IncludeNotes() As Boolean
        Get
            Return _includeNotes
        End Get
        Set(ByVal value As Boolean)
            _includeNotes = value
        End Set
    End Property

    ' Data Injection
    Public Property ReportData As List(Of ReportDraftsDue)

    Private _acqIdList As List(Of String)
    Public Property AcquisitionIDList() As List(Of String)
        Get
            Return _acqIdList
        End Get
        Set(ByVal value As List(Of String))
            _acqIdList = value
        End Set
    End Property

    Private _eof As Boolean = False
    Private _totalNumberRecords As Integer = 0

    Private _runDateTime As String

    Private Sub rptDraftsDue_DataInitialize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.DataInitialize
        If ReportData IsNot Nothing Then
            Me.DataSource = ReportData
        End If
    End Sub

    Private Sub rptDraftsDue_FetchData(ByVal sender As Object, ByVal eArgs As GrapeCity.ActiveReports.SectionReport.FetchEventArgs) Handles Me.FetchData
        If eArgs.EOF = False Then
            _totalNumberRecords += 1
        Else
            _eof = True
        End If
    End Sub

    Private Sub Detail1_Format(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Detail1.Format
        ' Subreport Counties due to object binding
        Dim counties = Fields("Counties").Value
        subrptCounty.Report = New rptDraftsDueCounty
        subrptCounty.Report.DataSource = counties

        If _includeNotes Then
            Dim notes = Fields("Notes").Value
            subrptNote.Visible = True
            subrptNote.Report = New rptDraftsDueNote
            subrptNote.Report.DataSource = notes
        Else
            subrptNote.Visible = False
        End If

    End Sub

    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
    End Sub

    Private Sub PageHeader1_Format(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PageHeader1.Format
        Me.txtRunDateTime.Text = _runDateTime
    End Sub

    Private Sub rptDraftsDue_ReportStart(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ReportStart
        _runDateTime = Format(Now(), "MM/dd/yyyy hh:mm:ss tt")
    End Sub

End Class
