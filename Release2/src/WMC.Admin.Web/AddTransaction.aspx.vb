
Partial Class AddTransaction
    Inherits System.Web.UI.Page

    Protected Sub SqlDataSourceTransaction_Inserted(sender As Object, e As SqlDataSourceStatusEventArgs) Handles SqlDataSourceTransaction.Inserted
        GridViewTransaction.DataBind()
    End Sub

    Protected Sub ButtonFindByOrderID_Click(sender As Object, e As EventArgs) Handles ButtonFindByOrderID.Click
        GridViewOrder.DataBind()
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not (IsNumeric(Session("UserId")) And Session("UserRole") = "Admin") Then Response.Redirect("Default.aspx")
    End Sub
End Class
