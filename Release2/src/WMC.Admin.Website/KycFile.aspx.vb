
Imports System.Data.SqlClient

<WMC.PageAuth(True, "Admin")>
Partial Class KycFile
    Inherits WMC.SecurePage
    Protected Sub GridViewKycFile_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridViewKycFile.RowCommand
        On Error Resume Next
        If e.CommandName = "Edit" Then GridViewKycFile.SelectedIndex = -1
        Dim nLineId As Integer = GridViewKycFile.DataKeys(Convert.ToInt32(e.CommandArgument)).Value
        If e.CommandName = "Select" Then
            Dim btn As LinkButton = GridViewKycFile.Rows(Convert.ToInt32(e.CommandArgument)).FindControl("LinkButtonSelect")
            btn.Focus()
            Dim strOname As String = eQuote.ExecuteScalar("SELECT [OriginalFilename] FROM KycFile WHERE Id =" + nLineId.ToString())
            Dim ext = strOname.Split(".").GetValue(1)
            If ext = "pdf" Then
                Dim embed As String = "<object data=""{0}{1}"" type=""application/pdf"" width=""500px"" height=""450px"">"
                embed += "</object>"
                ltEmbed.Visible = True
                ModalPopupExtender1.Show()
                GridView1.Visible = False
                ltEmbed.Text = String.Format(embed, ResolveUrl("KYCImageHandler.ashx?ImageId="), nLineId.ToString())
            Else
                GridView1.Visible = True
                ltEmbed.Visible = False
                ModalPopupExtender1.Show()
            End If
        End If
        If e.CommandName = "Reject" Then
            eQuote.ExecuteNonQuery("UPDATE KycFile SET Approved = NULL, ApprovedBy = NULL, Rejected = { fn NOW() }, RejectedBy = " + Session("UserId").ToString + " where Id= " + nLineId.ToString)
            GridViewKycFile.EditIndex = -1
        End If
        If e.CommandName = "Approve" Then
            eQuote.ExecuteNonQuery("UPDATE KycFile SET Approved = { fn NOW() }, ApprovedBy = " + Session("UserId").ToString + ", Rejected = NULL, RejectedBy = NULL where Id= " + nLineId.ToString)
            GridViewKycFile.EditIndex = -1
            GridViewUserToBeVerified.DataBind()
        End If
        If e.CommandName = "ApproveNow" Then
            Dim userId As Integer = eQuote.ExecuteScalar("Select UserId from KycFile where Id= " + nLineId.ToString)
            eQuote.ExecuteNonQuery("UPDATE KycFile Set Approved = { fn NOW() }, ApprovedBy = " + Session("UserId").ToString + ", Rejected = NULL, RejectedBy = NULL where Id= " + nLineId.ToString)
            Dim kycNotApprovedCount As Integer = eQuote.ExecuteScalar("SELECT Count(*) FROM [KycFile] Where [KycFile].UserId=" + userId.ToString + " AND [KycFile].Approved IS NULL")
            If kycNotApprovedCount = 0 Then
                eQuote.ExecuteNonQuery("UPDATE [User]  Set TierTwoApproved = { fn NOW() }, TierTwoApprovedBy = " + Session("UserId").ToString + " where [User].Id= " + userId.ToString)
            End If
            GridViewKycFile.DataBind()
            GridViewKycFile.EditIndex = -1
        End If
    End Sub
    Protected Sub btncancel_Click(sender As Object, e As EventArgs)
        btnCancel.Focus()
        ModalPopupExtender1.Hide()
        GridViewKycFile.DataBind()
    End Sub

    Protected Sub GridViewKycFile_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewKycFile.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            CType(e.Row.FindControl("ButtonApproveNow"), Button).CommandArgument = e.Row.RowIndex
            If e.Row.RowState = DataControlRowState.Edit Or e.Row.RowState = 5 Then
                CType(e.Row.FindControl("ButtonReject"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonApprove"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonReject"), Button).Attributes.Add("onClick", "javascript: return confirm('Er du sikker?')")
                CType(e.Row.FindControl("ButtonApprove"), Button).Attributes.Add("onClick", "javascript: return confirm('Er du sikker?')")
                CType(e.Row.FindControl("LinkButtonSelect"), LinkButton).Focus()
                'Dim lb As HyperLink = CType(e.Row.FindControl("HyperLinkPhone"), HyperLink)
                'lb.NavigateUrl = "FindOrders.aspx?phone=" + e.Row.RowIndex
            ElseIf e.Row.RowState = DataControlRowState.Selected Or e.Row.RowState = DataControlRowState.Normal Or e.Row.RowState = DataControlRowState.Alternate Then
                CType(e.Row.FindControl("LinkButtonDelete"), LinkButton).Attributes.Add("onClick", "javascript: return confirm('Er du sikker?')")
                'Dim lb As HyperLink = CType(e.Row.FindControl("HyperLinkPhone"), HyperLink)
                'lb.NavigateUrl = "FindOrders.aspx?phone=" + lb.NavigateUrl


                If CType(e.Row.FindControl("LabelKycType"), Label).Text = "CardApproval" Then
                    e.Row.BackColor = Drawing.Color.GreenYellow
                    CType(e.Row.FindControl("LinkButtonSelect"), LinkButton).Enabled = False
                    CType(e.Row.FindControl("ButtonApproveNow"), Button).Enabled = False
                Else
                    'e.Row.BackColor = Drawing.Color.Pink

                End If
            End If
        End If
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        'If Not (IsNumeric(Session("UserId")) And Session("UserRole") = "Admin") Then Response.Redirect("Default.aspx")

    End Sub

    Protected Sub GridViewUserToBeVerified_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewUserToBeVerified.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            If e.Row.RowState = DataControlRowState.Selected Or e.Row.RowState = DataControlRowState.Normal Or e.Row.RowState = DataControlRowState.Alternate Then
                Dim lb As HyperLink = CType(e.Row.FindControl("HyperLinkPhone"), HyperLink)
                lb.NavigateUrl = "FindOrders.aspx?phone=" + lb.NavigateUrl
            End If
        End If
    End Sub

    Protected Sub ButtonDeleteKyc_Click(sender As Object, e As EventArgs) Handles ButtonDeleteKyc.Click
        Dim KycFileId As String
        For Each item As GridViewRow In GridViewKycFile.Rows
            If TryCast(item.Cells(0).FindControl("cbSelectKyc"), CheckBox).Checked Then
                KycFileId = item.Cells(1).Text
                eQuote.ExecuteNonQuery("DELETE FROM [KycFile] WHERE [Id] = N'" + KycFileId.ToString + "'")
            End If
        Next
        GridViewKycFile.DataBind()
    End Sub
    Protected Sub GridViewUser_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridViewUser.RowCommand
        If e.CommandName = "ApproveUser" Then
            Dim rowIndex = e.CommandArgument
            Dim userId As Integer = GridViewUser.DataKeys(Convert.ToInt32(e.CommandArgument)).Value

            Dim kycNotApprovedCount As Integer = eQuote.ExecuteScalar("SELECT Count(*) FROM [KycFile] Where [KycFile].UserId=" + userId.ToString + " AND [KycFile].Approved IS NULL")
            If kycNotApprovedCount > 0 Then
                Dim message As String = "Please Approve the KYC before proceding"
                Dim sb As New System.Text.StringBuilder()
                sb.Append("<script type = 'text/javascript'>")
                sb.Append("window.onload=function(){")
                sb.Append("alert('")
                sb.Append(message)
                sb.Append("')};")
                sb.Append("</script>")
                ClientScript.RegisterClientScriptBlock(Me.GetType(), "alert", sb.ToString())
            End If
            If kycNotApprovedCount = 0 Then
                Using dr As SqlDataReader = eQuote.GetDataReader("SELECT Tier,Id,TierTwoApproved ,TierTwoApprovedBy FROM [User] WHERE [User].Id = " + userId.ToString + "")
                    Try
                        dr.Read()
                        WMC.AddStatusChange(dr!Id, WMC.Data.Enums.CustomerTier.Tier3, dr!Tier, Session("UserId"))
                        Dim UserTierLevel As Integer = WMC.Data.Enums.CustomerTier.Tier3
                        eQuote.ExecuteNonQuery("UPDATE [User] SET Tier = " + UserTierLevel.ToString + " WHERE [User].Id= " + dr!Id.ToString + "")
                        eQuote.ExecuteNonQuery("UPDATE [User]  SET TierThreeApproved = { fn NOW() }, TierThreeApprovedBy = " + Session("UserId").ToString + " where [User].Id= " + dr!Id.ToString)
                        If dr!TierTwoApprovedBy.Equals(DBNull.Value) Then
                            eQuote.ExecuteNonQuery("UPDATE [User]  SET TierTwoApproved = { fn NOW() }, TierTwoApprovedBy = " + Session("UserId").ToString + " where [User].Id= " + dr!Id.ToString)
                        End If
                        GridViewUser.SelectedIndex = -1
                        GridViewUser.DataBind()
                    Finally
                        dr.Close()
                    End Try
                End Using
            End If

        End If
    End Sub
    Protected Sub GridViewUser_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewUser.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            If e.Row.RowState = DataControlRowState.Selected Or e.Row.RowState = DataControlRowState.Normal Or e.Row.RowState = DataControlRowState.Alternate Then
                Dim lb As HyperLink = CType(e.Row.FindControl("HyperLinkPhone"), HyperLink)
                lb.NavigateUrl = "FindOrders.aspx?phone=" + lb.NavigateUrl
            End If
        End If
    End Sub
End Class
