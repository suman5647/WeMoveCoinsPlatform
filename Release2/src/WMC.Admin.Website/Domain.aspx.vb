
Partial Class AdminDomainUser
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not (IsNumeric(Session("UserId")) And Session("UserRole") = "Admin") Then Response.Redirect("Default.aspx")
    End Sub

    'Protected Sub ButtonDeleteOrder_Click(sender As Object, e As EventArgs) Handles ButtonDeleteOrder.Click
    '    Dim o As String = TextBoxOrderId.Text
    '    If IsNumeric(o) Then
    '        eQuote.ExecuteNonQuery("DELETE FROM AuditTrail WHERE  OrderId =" + o)
    '        eQuote.ExecuteNonQuery("DELETE FROM OrderKycfile WHERE  OrderId =" + o)
    '        eQuote.ExecuteNonQuery("DELETE FROM [Transaction] WHERE  OrderId =" + o)
    '        eQuote.ExecuteNonQuery("DELETE FROM [Order] WHERE  Id =" + o)
    '    End If
    'End Sub
End Class
