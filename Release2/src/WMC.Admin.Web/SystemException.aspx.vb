
Partial Class SystemExceptions
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsNumeric(Session("UserId")) Then Response.Redirect("Default.aspx")
        ButtonDeleteAll.Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
    End Sub

    Protected Sub ButtonDeleteAll_Click(sender As Object, e As EventArgs) Handles ButtonDeleteAll.Click
        eQuote.ExecuteNonQuery("DELETE TOP (10000) FROM AuditTrail FROM AuditTrail INNER JOIN AuditTrailStatus ON AuditTrail.Status = AuditTrailStatus.Id WHERE  (AuditTrailStatus.Text = N'Application Error')")
        GridViewException.DataBind()
    End Sub
End Class
