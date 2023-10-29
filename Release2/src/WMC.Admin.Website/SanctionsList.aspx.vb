
Imports System.Data
Imports System.Data.SqlClient
Imports System
Imports System.Net
Imports WMC.Logic
Imports System.Linq

<WMC.PageAuth(True, "Admin")>
Partial Class SanctionsList
    Inherits WMC.SecurePage

    Protected Sub ButtonSearch_Click(sender As Object, e As EventArgs) Handles ButtonSearch.Click
        Dim userName = TextBoxText.Text
        Dim sanctionList = SanctionsListUtility.SearchByName(userName).ToList()
        ListGridView.DataSource = sanctionList
        ListGridView.DataBind()
    End Sub


    Protected Sub ListGridView_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles ListGridView.RowCommand
        If e.CommandName = "Select" Then
            Dim row As GridViewRow = CType(CType(e.CommandSource, Control).NamingContainer, GridViewRow)
            Dim text As Label = ListGridView.Rows(Convert.ToInt32(e.CommandArgument)).FindControl("SummaryId")
            TextBox1.Visible = True
            TextBox1.Text = text.Text
        End If
    End Sub
End Class
