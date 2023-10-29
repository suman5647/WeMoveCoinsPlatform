Option Strict Off
Imports BitGoSharp
Imports RestSharp
Imports System.Data
Imports System.Data.SqlClient
Imports System
Imports System.Net
Imports System.IO
Imports WMC.Logic

Partial Class ApprovePayout
    Inherits System.Web.UI.Page

    Dim client As BitGoClient
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not (IsNumeric(Session("UserId")) And (Session("UserRole") = "Admin" Or Session("UserRole") = "SiteMgr")) Then Response.Redirect("Default.aspx")
        Dim bitGoAccessCodeJson As String = eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'BitGoAccessCode'")
        Dim bitGoAccessCode = SimpleJson.DeserializeObject(bitGoAccessCodeJson)
        client = New BitGoClient(bitGoAccessCode.Environment, bitGoAccessCode.AccessCode)

        If Session("UserRole") = "SiteMgr" Then

        End If
        Try
            If IsNumeric(Request.QueryString("id")) Or Request.QueryString("phone").Length > 0 Then
                Dim sqlSiteManagerWhere As String = ""
                If Session("UserRole") = "SiteMgr" Then
                    If Session("SiteId") <> 1 Then
                        sqlSiteManagerWhere = " [Order].SiteId = " + Session("SiteId").ToString + " AND "
                    End If
                End If
                GridViewOrder.DataSource = SqlDataSourceOrder
                Dim s As String = SqlDataSourceOrder.SelectCommand
                Dim w As String = ""
                If IsNumeric(Request.QueryString("id")) Then
                    w = "WHERE" + sqlSiteManagerWhere + " [Order].Number = N'" + Request.QueryString("id") + "' "
                ElseIf Request.QueryString("phone").Length > 0 Then
                    w = "WHERE" + sqlSiteManagerWhere + " [User].Phone = N'+" + Request.QueryString("phone").Trim + "' "
                End If

                SqlDataSourceOrder.SelectCommand = s.Insert(s.IndexOf("ORDER BY"), w)
                GridViewOrder.DataBind()
            End If
        Catch ex As Exception

        End Try


    End Sub

    Protected Sub GridViewOrder_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridViewOrder.RowCommand

        Dim OrderId As Integer = GridViewOrder.DataKeys(Convert.ToInt32(e.CommandArgument)).Value
        If e.CommandName = "MoveCompliance" Then
            Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Compliance Officer Approval'")
            eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
            GridViewOrder.SelectedIndex = -1
            GridViewOrder.DataBind()
        ElseIf e.CommandName = "CancelOrder" Then
            Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Cancel'")
            eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
            GridViewOrder.SelectedIndex = -1
            GridViewOrder.DataBind()
        ElseIf e.CommandName = "KYCDeclined" Then
            Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'KYC Decline'")
            eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
            GridViewOrder.SelectedIndex = -1
            GridViewOrder.DataBind()
        ElseIf e.CommandName = "Select" Then
        End If
    End Sub

    Protected Sub ButtonAddNote_Click(sender As Object, e As EventArgs) Handles ButtonAddNote.Click
        Dim sqlSiteManagerWhere As String = ""
        If Session("UserRole") = "SiteMgr" Then
            If Session("SiteId") <> 1 Then
                sqlSiteManagerWhere = " [Order].SiteId = " + Session("SiteId").ToString + " AND "
            End If
        End If
        Dim nLineId As Integer = GridViewOrder.SelectedValue
        Dim GetMemoLogScript As String = String.Format(" {0:d}", Date.Now) + " at " + String.Format(" {0:t}", Date.Now) + " by " + eQuote.ExecuteScalar("SELECT Lname FROM [User] WHERE Id = " + Session("UserId").ToString) + ":<br />" + TextBoxNote.Text + "<br />"
        eQuote.ExecuteNonQuery("UPDATE [Order] SET Note = { fn CONCAT('" + Replace(GetMemoLogScript, Environment.NewLine, "<br/>") + "', { fn IFNULL(Note, '') }) } WHERE  Id = " + nLineId.ToString)
        GridViewOrder.DataSource = SqlDataSourceOrder
        Dim s As String = SqlDataSourceOrder.SelectCommand
        Dim w As String = ""
        Select Case DropDownListWhereType.SelectedValue
            Case "Number"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].Number = N'" + TextBoxOrderID.Text + "' "
            Case "CC Number"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].CardNumber LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "Email"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].Email LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "Name"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].Name LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "YourPay ID"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].ExtRef LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "TransactionHash"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].TransactionHash LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "CryptoAddress"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].CryptoAddress LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "TxSecret"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].TxSecret LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "Phone"
                w = "WHERE" + sqlSiteManagerWhere + " [User].Phone LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "SiteID"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].SiteId LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "PartnerID"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].PartnerId LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "UserId"
                w = "WHERE" + sqlSiteManagerWhere + " [User].Id LIKE '" + TextBoxOrderID.Text + "' "
                'Case "Trusted Only"
                '    w = "WHERE" + sqlSiteManagerWhere + " [User].Trusted IS NOT NULL "
            Case "OrderId"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].Id = " + TextBoxOrderID.Text
        End Select
        SqlDataSourceOrder.SelectCommand = s.Insert(s.IndexOf("ORDER BY"), w)
        GridViewOrder.DataBind()
        TextBoxNote.Text = ""
        DropDownListNoteOptions.SelectedIndex = 0
    End Sub

    Protected Sub DropDownListNoteOptions_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownListNoteOptions.SelectedIndexChanged
        If DropDownListNoteOptions.SelectedIndex > 0 Then
            TextBoxNote.Text = DropDownListNoteOptions.SelectedValue
        End If
    End Sub

    ' ********** PanelTrustCalculation ***************************************


    ' ********** PanelTrustCalculation ***************************************





    Protected Sub GridViewOrder_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewOrder.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            If e.Row.RowState = DataControlRowState.Edit Or e.Row.RowState = 5 Or e.Row.RowState = 6 Then
                Dim GetMemoLogScript As String = vbNewLine + vbNewLine + String.Format(" {0:d}", Date.Now) + " at " + String.Format(" {0:t}", Date.Now) + " by " + eQuote.ExecuteScalar("SELECT Lname FROM [User] WHERE Id = " + Session("UserId").ToString) + vbNewLine + "------" + vbNewLine
                If CType(e.Row.FindControl("TextBoxNote"), TextBox).Text.Length > 0 Then
                    CType(e.Row.FindControl("TextBoxNote"), TextBox).Text = GetMemoLogScript + CType(e.Row.FindControl("TextBoxNote"), TextBox).Text
                Else
                    CType(e.Row.FindControl("TextBoxNote"), TextBox).Text = GetMemoLogScript
                End If
                CType(e.Row.FindControl("TextBoxNote"), TextBox).Focus()
            ElseIf e.Row.RowState = DataControlRowState.Selected Or e.Row.RowState = DataControlRowState.Normal Or e.Row.RowState = DataControlRowState.Alternate Then
                CType(e.Row.FindControl("ButtonMoveCompliance"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonMoveCompliance"), Button).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
                CType(e.Row.FindControl("ButtonCancelOrder"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonCancelOrder"), Button).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
                CType(e.Row.FindControl("ButtonKYCDeclined"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonKYCDeclined"), Button).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
                Dim lb As HyperLink = CType(e.Row.FindControl("HyperLinkOrder"), HyperLink)
                lb.NavigateUrl = "FindOrders.aspx?id=" + lb.NavigateUrl
            End If
        End If
    End Sub

    'Protected Sub GridViewUser_DataBound(sender As Object, e As EventArgs) Handles GridViewUser.DataBound
    '    'https://ACd32f9f403312208a96707ec9356eb2ea:74dfe135a1259269c3a7817ffd9fd6e6@lookups.twilio.com/v1/PhoneNumbers/+447492195433?AddOns=whitepages_pro_phone_rep
    'End Sub

    'Protected Sub GridViewUser_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewUser.RowDataBound
    '    If e.Row.RowType = DataControlRowType.DataRow Then
    '        If e.Row.RowState = DataControlRowState.Edit Or e.Row.RowState = 5 Then
    '        Else
    '            Dim lb As HyperLink = CType(e.Row.FindControl("HyperLinkTwilio"), HyperLink)
    '            If lb.NavigateUrl.Length Then
    '                lb.NavigateUrl = "https://www.google.dk/#q=%2B" + lb.NavigateUrl.ToString + ""
    '            End If
    '        End If

    '    End If
    'End Sub


    Protected Sub GridViewTransaction_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewTransaction.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Try
                Dim lb As Label = CType(e.Row.FindControl("LabelInfo"), Label)
                Dim lb2 As Label = CType(e.Row.FindControl("LabelBINInfo"), Label)
                Dim bin As String = lb.Text.Replace(" ", "").Substring(0, 6)
                Dim wrGETURL As WebRequest
                wrGETURL = WebRequest.Create("https://lookup.binlist.net/" + bin)
                wrGETURL.Headers.Add("accept-version", "3")
                Dim objStream As Stream
                Try
                    objStream = wrGETURL.GetResponse.GetResponseStream()
                    Dim objReader As New StreamReader(objStream)
                    lb2.Text = objReader.ReadLine
                Catch ex As Exception
                    lb2.Text = "no match"
                End Try
            Catch ex As Exception
            End Try
        End If
    End Sub



    Protected Sub ButtonFindByOrderID_Click(sender As Object, e As EventArgs) Handles ButtonFindByOrderID.Click
        Dim sqlSiteManagerWhere As String = ""
        If Session("UserRole") = "SiteMgr" Then
            If Session("SiteId") <> 1 Then
                sqlSiteManagerWhere = " [Order].SiteId = " + Session("SiteId").ToString + " AND "
            End If
        End If
        GridViewOrder.DataSource = SqlDataSourceOrder
        Dim s As String = SqlDataSourceOrder.SelectCommand
        Dim w As String = ""
        Select Case DropDownListWhereType.SelectedValue
            Case "Number"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].Number = N'" + TextBoxOrderID.Text + "' "
            Case "CC Number"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].CardNumber LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "Email"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].Email LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "Name"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].Name LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "YourPay ID"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].ExtRef LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "CryptoAddress"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].CryptoAddress LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "TransactionHash"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].TransactionHash LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "CryptoAddress"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].CryptoAddress LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "TxSecret"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].TxSecret LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "Phone"
                w = "WHERE" + sqlSiteManagerWhere + " [User].Phone LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "SiteID"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].SiteId LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "PartnerID"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].PartnerId LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "UserId"
                w = "WHERE" + sqlSiteManagerWhere + " [User].Id LIKE '" + TextBoxOrderID.Text + "' "
                'Case "Trusted Only"
                '    w = "WHERE" + sqlSiteManagerWhere + " [User].Trusted IS NOT NULL "
            Case "OrderId"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].Id = " + TextBoxOrderID.Text
        End Select
        SqlDataSourceOrder.SelectCommand = s.Insert(s.IndexOf("ORDER BY"), w)
        GridViewOrder.DataBind()
    End Sub










End Class
