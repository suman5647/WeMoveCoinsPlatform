
Partial Class SystemExceptions
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim days = Request.QueryString("days")
        Dim fromDate As DateTime
        If String.IsNullOrEmpty(days) Then
            fromDate = Date.Now().AddDays(-1)
        Else
            fromDate = Date.Now().AddDays(-days)
        End If
        SqlDataSourceException.SelectParameters.Item("fromDate").DefaultValue = fromDate
        If Not IsNumeric(Session("UserId")) Then Response.Redirect("Default.aspx")
        ButtonDeleteAll.Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
    End Sub

    Protected Sub ButtonDeleteAll_Click(sender As Object, e As EventArgs) Handles ButtonDeleteAll.Click
        eQuote.ExecuteNonQuery("DELETE TOP (10000) FROM AuditTrail FROM AuditTrail INNER JOIN AuditTrailStatus ON AuditTrail.Status = AuditTrailStatus.Id WHERE  (AuditTrailStatus.Text = N'Application Error')")
        GridViewException.DataBind()
    End Sub

    Protected Sub ButtonSearch_Click(sender As Object, e As EventArgs) Handles ButtonSearch.Click
        GridViewException.DataBind()
    End Sub
End Class
