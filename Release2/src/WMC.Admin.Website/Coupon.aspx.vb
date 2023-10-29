
<WMC.PageAuth(True, "Admin")>
Partial Class SystemExceptions
    Inherits WMC.SecurePage
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsNumeric(Session("UserId")) Then Response.Redirect("Default.aspx")

    End Sub


    Protected Sub LinkButtonInsert_Click(sender As Object, e As EventArgs) Handles LinkButtonInsert.Click
        PanelInsert.Visible = True
        PanelSelect.Visible = False
    End Sub

    Private Sub DetailsViewCoupons_ItemCommand(sender As Object, e As DetailsViewCommandEventArgs) Handles DetailsViewCoupons.ItemCommand
        If e.CommandName = "Insert" Or e.CommandName = "Cancel" Then
            PanelInsert.Visible = False
            PanelSelect.Visible = True
        End If
    End Sub

    Private Sub GridViewCoupons_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewCoupons.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            If e.Row.RowState = DataControlRowState.Selected Or e.Row.RowState = DataControlRowState.Normal Or e.Row.RowState = DataControlRowState.Alternate Then
                CType(e.Row.FindControl("LinkButtonDelete"), LinkButton).Attributes.Add("onClick", "javascript: return confirm('Er du sikker?')")
            End If
        End If
    End Sub
End Class
