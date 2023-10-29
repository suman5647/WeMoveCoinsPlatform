Imports System.Globalization

Partial Class TransactionList
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not (IsNumeric(Session("UserId")) And Session("UserRole") = "Admin") Then Response.Redirect("Default.aspx")
        'If DropDownListMonth.Items.Count = 0 Then
        '    Dim months = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames
        '    For i As Integer = 0 To months.Length - 1
        '        DropDownListMonth.Items.Add(New ListItem(months(i), (i + 1).ToString()))
        '    Next
        'End If
    End Sub

    'Protected Sub DropDownListMonth_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownListMonth.SelectedIndexChanged
    '    GridView1.DataBind()
    'End Sub

    'Protected Sub DropDownListYear_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownCountry.SelectedIndexChanged
    '    GridView1.DataBind()
    'End Sub
End Class
