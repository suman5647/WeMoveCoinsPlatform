Imports System.Globalization
Imports System.Collections.Generic
Imports System
Imports System.Data.Entity
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports WMC.Logic

<WMC.PageAuth(True, "Admin")>
Partial Class TransactionList
    Inherits WMC.SecurePage

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.PopulateYear(DropDownListYear)
            Me.PopulateMonth(DropDownListMonth, DropDownListYear.SelectedValue)

            Me.PopulateYear(ddlYear)
            Me.PopulateMonth(ddlMonth, ddlYear.SelectedValue)
            Me.PopulateDay(ddlDay, ddlYear.SelectedValue, ddlMonth.SelectedValue)
        End If
    End Sub

    Protected Sub DropDownMonthOrYear_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlMonth.SelectedIndexChanged, ddlYear.SelectedIndexChanged
        ddlDay.ClearSelection()
        Me.PopulateDay(ddlDay, ddlYear.SelectedValue, ddlMonth.SelectedValue)
    End Sub

    Protected Sub btntoCsv_Click(sender As Object, e As EventArgs) Handles btntoCsv.Click
        Dim _date As Date = SelectedDate
        Dim dayNumber = WMC.Logic.Accounting.AccountingUtil.GetDayNumber(_date)
        Dim strData = ExportToCSV(_date, chkIncludeHeader.Checked)
        Dim fileName = String.Format("DAY_{0:yyyyMMdd}_{1}.csv", _date, dayNumber)

        Response.Clear()
        Response.Buffer = True
        Response.AddHeader("content-disposition", String.Format("attachment;filename={0}", fileName))
        Response.Charset = ""
        Response.ContentType = "application/text"
        Response.Output.Write(strData)
        Response.Flush()
        Response.[End]()
    End Sub

    Private Function ExportToCSV(_date As DateTime, Optional includeHeader As Boolean = False) As String
        Try
            AuditLog.log("Generation report " + _date, 2, 4)  'log(message, status, auditTrailLevel)
            Dim sBuilder As StringBuilder = New System.Text.StringBuilder()
            If (includeHeader) Then
                sBuilder.Append(String.Join(WMC.Logic.Accounting.DaybookRecord.Seperator, WMC.Logic.Accounting.DaybookRecord.Columns))
                sBuilder.Append(Environment.NewLine)
            End If

            Dim dayBookData = WMC.Logic.Accounting.AccountingUtil.GetDayBook(_date)
            dayBookData = dayBookData.ToList().OrderBy(Function(s) s.AccountText)

            For Each item As WMC.Logic.Accounting.DaybookRecord In dayBookData
                item.Amount = (item.Amount).ToString("N2")
                item.Amount2 = (item.Amount2).ToString("N2")
                sBuilder.Append(item.ToString())
                sBuilder.Append(Environment.NewLine)
            Next
            Return sBuilder.ToString()

        Catch ex As Exception
            Dim auditStatus As Long = eQuote.ExecuteScalar("SELECT Id FROM AuditTrailStatus WHERE Text = N'ApplicationError'")
            Dim auditLevel As Long = eQuote.ExecuteScalar("SELECT Id FROM AuditTrailLevel WHERE Text = N'Error'")
            AuditLog.log(ex.ToString, auditStatus, auditLevel)
            Throw ex
        End Try

    End Function

    Private Sub PopulateDay(ddRef As DropDownList, _year As Integer, _month As Integer)
        ddRef.Items.Clear()
        Dim lt As ListItem = New ListItem
        lt.Text = "DD"
        lt.Value = "0"
        ddlDay.Items.Add(lt)

        If _year <= 1900 Then _year = DateTime.Now.Year
        If _month <= 0 Then _month = DateTime.Now.Month

        Dim days As Integer = DateTime.DaysInMonth(_year, _month)
        Dim i As Integer = 1
        Do While (i <= days)
            lt = New ListItem
            lt.Text = i.ToString
            lt.Value = i.ToString
            ddRef.Items.Add(lt)

            If (ddRef.SelectedIndex <= 0 And Date.Today.Day = lt.Value) Then
                ddRef.SelectedValue = lt.Value
            End If

            i = (i + 1)
        Loop
    End Sub

    Private Sub PopulateMonth(ddRef As DropDownList, _year As Integer)
        ddRef.Items.Clear()
        Dim lt As ListItem = New ListItem
        lt.Text = "MM"
        lt.Value = "0"
        ddRef.Items.Add(lt)
        Dim i As Integer = 1
        Do While (i <= 12)
            lt = New ListItem
            lt.Text = New DateTime(_year, i, 1).ToString("MMMM")
            lt.Value = i.ToString
            ddRef.Items.Add(lt)

            If (ddRef.SelectedIndex <= 0 And Date.Today.Month = lt.Value) Then
                ddRef.SelectedValue = lt.Value
            End If

            i = (i + 1)
        Loop
    End Sub

    Private Sub PopulateYear(ddRef As DropDownList)
        ddRef.Items.Clear()

        Dim lt As ListItem = New ListItem
        lt.Text = "YYYY"
        lt.Value = "0"
        ddlYear.Items.Add(lt)
        Dim i As Integer = DateTime.Now.Year
        Do While (i >= 1950)
            lt = New ListItem
            lt.Text = i.ToString
            lt.Value = i.ToString

            ddRef.Items.Add(lt)

            If (ddRef.SelectedIndex <= 0 And Date.Today.Year = lt.Value) Then
                ddRef.SelectedValue = lt.Value
            End If

            i = (i - 1)
        Loop
    End Sub

    Private Property Day() As Integer
        Get
            If (Not (Request.Form(ddlDay.UniqueID)) Is Nothing) Then
                Return Integer.Parse(Request.Form(ddlDay.UniqueID))
            Else
                Return Integer.Parse(ddlDay.SelectedItem.Value)
            End If
        End Get
        Set(ByVal value As Integer)
            Me.PopulateDay(ddlDay, Me.Year, Me.Month)
            ddlDay.ClearSelection()
            ddlDay.Items.FindByValue(value.ToString).Selected = True
        End Set
    End Property

    Private Property Month() As Integer
        Get
            Return Integer.Parse(ddlMonth.SelectedItem.Value)
        End Get
        Set(ByVal value As Integer)
            Me.PopulateMonth(ddlMonth, Me.Year)
            ddlMonth.ClearSelection()
            ddlMonth.Items.FindByValue(value.ToString).Selected = True
        End Set
    End Property

    Private Property Year() As Integer
        Get
            Return Integer.Parse(ddlYear.SelectedItem.Value)
        End Get
        Set(ByVal value As Integer)
            Me.PopulateYear(ddlYear)
            ddlYear.ClearSelection()
            ddlYear.Items.FindByValue(value.ToString).Selected = True
        End Set
    End Property

    Public Property SelectedDate() As DateTime
        Get
            Try
                Return New DateTime(Me.Year, Me.Month, Me.Day)
            Catch ex As Exception
                Return DateTime.MinValue
            End Try
        End Get
        Set(ByVal value As DateTime)
            If Not value.Equals(DateTime.MinValue) Then
                Me.Year = value.Year
                Me.Month = value.Month
                Me.Day = value.Day
            End If
        End Set
    End Property
End Class
