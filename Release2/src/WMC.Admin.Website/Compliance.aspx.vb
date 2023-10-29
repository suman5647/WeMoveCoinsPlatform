Option Strict Off
Imports BitGoSharp
Imports RestSharp
Imports System.Data
Imports System.Data.SqlClient
Imports System
Imports System.Net
Imports System.IO
Imports System.Collections.Generic
Imports System.Globalization

<WMC.PageAuth(True, "Admin")>
Partial Class Compliance
    Inherits WMC.SecurePage

    Dim client As BitGoClient
    Public WeightCcOrigin As Integer = 1
    Public WeightPhoneOrigin As Integer = 50
    Public WeightOrderSpan As Integer = 50
    Public WeightCountryCoherence As Integer = 50
    Public WeightDataConsistency As Integer = 30
    Public WeightKycApprove As Integer = 20
    Public WeighAmountSize As Integer = 50

    'Protected Sub btnUnlock_Click(sender As Object, e As EventArgs) Handles BtnUnlock.Click
    '    Dim response As IRestResponse = client.Unlock(txtOTP.Text, CInt(DropDownListUnlockPeriode.SelectedValue))
    '    Label1.Text = response.StatusCode.ToString
    '    eQuote.ExecuteScalar("UPDATE AppSettings SET ConfigValue = { fn NOW() } WHERE ConfigKey = 'BitGoUnlocked'")
    'End Sub

    Sub SetCountStatus()
        Dim nUsers As Integer = eQuote.ExecuteScalar("SELECT COUNT(DISTINCT [Order].Id) FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId  INNER JOIN OrderStatus ON (OrderStatus.Id = [Order].Status)  INNER JOIN OrderType ON ([Order].Type = 1 OR [Order].Type=2) INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN Country ON [User].CountryId = Country.Id INNER JOIN UserRiskLevelType ON (UserRiskLevelType.Id = [User].UserRiskLevel) WHERE ([User].Tier = 3 AND [Order].Status = 3) OR  ([Order].Status = 26) OR ([User].UserRiskLevel = 2 AND [Order].Status = 7 ) ")

        Dim nUsersCount As Integer = eQuote.ExecuteScalar("SELECT COUNT(DISTINCT [Order].Id) FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN Country ON [User].CountryId = Country.Id WHERE (OrderStatus.Text = 'Compliance Officer Approval') OR ([User].UserRiskLevel = 1 AND[Order].Status = 7) ")

        ButtonEnhancedDueDiligence.Text = "Enhanced Due Diligence (" + nUsers.ToString + ")"
        ButtonComplianceOfficerApproval.Text = "Compliance Officer Approval (" + nUsersCount.ToString + ")"
        ButtonCustomerResponsePending.Text = "Customer Response Pending (" + WMC.StatusRecordCount("Customer Response Pending").ToString + ")"
        ButtonKYCApprovalPending.Text = "KYC Approval Pending (" + WMC.StatusRecordCount("KYC Approval Pending").ToString + ")"
    End Sub


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        '' If Not (IsNumeric(Session("UserId")) And Session("UserRole") = "Admin") Then Response.Redirect("Default.aspx")
        SetCountStatus()

        btnApproveCardDoc.Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")

        If Session("ComplianceView") = "Enhanced Due Diligence" Then ButtonEnhancedDueDiligence.BackColor = Drawing.Color.DarkGray
        If Session("ComplianceView") = "Compliance Officer Approval" Then ButtonComplianceOfficerApproval.BackColor = Drawing.Color.DarkGray
        If Session("ComplianceView") = "Customer Response Pending" Then ButtonCustomerResponsePending.BackColor = Drawing.Color.DarkGray
        If Session("ComplianceView") = "KYC Approval Pending" Then ButtonKYCApprovalPending.BackColor = Drawing.Color.DarkGray



        If Session("ComplianceView") = "Enhanced Due Diligence" Then
            GridViewOrder.Columns(12).Visible = True      'Approve
            GridViewOrder.Columns(13).Visible = True      'MoveToCompliance
            GridViewOrder.Columns(14).Visible = True      'CustomerPending
            GridViewOrder.Columns(15).Visible = False     'MoveToEDDe
            GridViewOrder.Columns(16).Visible = True      'ReviewKYC
            GridViewOrder.Columns(17).Visible = True      'TxSecret
            GridViewOrder.Columns(18).Visible = True      'KYCDeclined
            GridViewOrder.Columns(19).Visible = True      'CancelOrder
            GridViewOrder.Columns(20).Visible = True      'ApproveEDD
            GridViewOrder.DataSourceID = SqlDataSourceOrder1.ID
            GridViewOrder.DataBind()
            ButtonDeleteSelected.Visible = False
            ButtonUpdateMinersFee.Visible = False
            TextBoxMinersFee.Visible = False
            GridViewOrder.Columns(0).Visible = False
        ElseIf Session("ComplianceView") = "Compliance Officer Approval" Then
            GridViewOrder.Columns(12).Visible = True      'Approve
            GridViewOrder.Columns(13).Visible = False       'MoveToCompliance
            GridViewOrder.Columns(14).Visible = True        'CustomerPending
            GridViewOrder.Columns(15).Visible = True        'MoveToEDDe
            GridViewOrder.Columns(16).Visible = True        'ReviewKYC
            GridViewOrder.Columns(17).Visible = True        'TxSecret
            GridViewOrder.Columns(18).Visible = True        'KYCDeclined
            GridViewOrder.Columns(19).Visible = True        'CancelOrder
            GridViewOrder.Columns(20).Visible = False      'ApproveEDD
            GridViewOrder.DataSourceID = SqlDataSourceOrder2.ID
            GridViewOrder.DataBind()
            ButtonDeleteSelected.Visible = False
            ButtonUpdateMinersFee.Visible = False
            TextBoxMinersFee.Visible = False
            GridViewOrder.Columns(0).Visible = True
        ElseIf Session("ComplianceView") = "Customer Response Pending" Then
            GridViewOrder.Columns(12).Visible = True      'Approve
            GridViewOrder.Columns(13).Visible = True
            GridViewOrder.Columns(14).Visible = False
            GridViewOrder.Columns(15).Visible = False
            GridViewOrder.Columns(16).Visible = False
            GridViewOrder.Columns(17).Visible = True
            GridViewOrder.Columns(18).Visible = True
            GridViewOrder.Columns(19).Visible = True
            GridViewOrder.Columns(20).Visible = False
            GridViewOrder.Columns(21).Visible = False
            GridViewOrder.DataSourceID = SqlDataSourceOrder.ID
            GridViewOrder.DataBind()
            ButtonDeleteSelected.Visible = True
            ButtonUpdateMinersFee.Visible = False
            TextBoxMinersFee.Visible = False
            GridViewOrder.Columns(0).Visible = True
        ElseIf Session("ComplianceView") = "KYC Approval Pending" Then
            GridViewOrder.Columns(12).Visible = False      'Approve
            GridViewOrder.Columns(13).Visible = True
            GridViewOrder.Columns(14).Visible = True
            GridViewOrder.Columns(15).Visible = False
            GridViewOrder.Columns(16).Visible = False
            GridViewOrder.Columns(17).Visible = False
            GridViewOrder.Columns(18).Visible = False
            GridViewOrder.Columns(19).Visible = False
            GridViewOrder.Columns(20).Visible = False
            GridViewOrder.Columns(21).Visible = False
            GridViewOrder.DataSourceID = SqlDataSourceOrder.ID
            GridViewOrder.DataBind()
            ButtonDeleteSelected.Visible = False
            ButtonUpdateMinersFee.Visible = False
            TextBoxMinersFee.Visible = False
            GridViewOrder.Columns(0).Visible = False
        ElseIf Session("ComplianceView") = "Received Crypto Payment" Then
            GridViewOrder.Columns(12).Visible = False      'Approve
            GridViewOrder.Columns(13).Visible = True
            GridViewOrder.Columns(14).Visible = True
            GridViewOrder.Columns(15).Visible = False
            GridViewOrder.Columns(16).Visible = False
            GridViewOrder.Columns(17).Visible = False
            GridViewOrder.Columns(18).Visible = False
            GridViewOrder.Columns(19).Visible = False
            GridViewOrder.Columns(20).Visible = False
            GridViewOrder.Columns(21).Visible = False
            GridViewOrder.DataSourceID = SqlDataSourceOrder.ID
            GridViewOrder.DataBind()
            ButtonDeleteSelected.Visible = False
            ButtonUpdateMinersFee.Visible = False
            TextBoxMinersFee.Visible = False
            GridViewOrder.Columns(0).Visible = False
        End If
    End Sub

    Protected Sub SqlDataSourceOrderChildNote_Updated(sender As Object, e As SqlDataSourceStatusEventArgs) Handles SqlDataSourceOrderChildNote.Updated
        GridViewOrder.SelectedIndex = -1
        GridViewOrder.DataBind()
        GridViewSubOrders.SelectedIndex = -1
        GridViewSubOrders.DataBind()
    End Sub

    Protected Sub GridViewOrder_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridViewOrder.RowCommand
        PanelKeyIndicators.Visible = False
        Dim OrderId As Integer = GridViewOrder.DataKeys(Convert.ToInt32(e.CommandArgument)).Value
        Dim statusOrder As String = Session("ComplianceView")
        Dim statusOrder1 As String = "AML Approval Pending"
        If e.CommandName = "MoveToCompliance" Then
            Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Compliance Officer Approval'")
            If WMC.IsOrderStatus(statusOrder, OrderId) Then
                WMC.AddStatusChange(OrderId, statusOrder, s, Session("UserId"))
                eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
                GridViewOrder.SelectedIndex = -1
                GridViewOrder.DataBind()
            End If
            If WMC.IsOrderStatus(statusOrder1, OrderId) Then
                WMC.AddStatusChange(OrderId, statusOrder1, s, Session("UserId"))
                eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
                GridViewOrder.SelectedIndex = -1
                GridViewOrder.DataBind()
            End If
        ElseIf e.CommandName = "CustomerPending" Then
            Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Customer Response Pending'")
            If WMC.IsOrderStatus(statusOrder, OrderId) Then
                WMC.AddStatusChange(OrderId, statusOrder, s, Session("UserId"))
                eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
                GridViewOrder.SelectedIndex = -1
                GridViewOrder.DataBind()
            End If
            If WMC.IsOrderStatus(statusOrder1, OrderId) Then
                WMC.AddStatusChange(OrderId, statusOrder1, s, Session("UserId"))
                eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
                GridViewOrder.SelectedIndex = -1
                GridViewOrder.DataBind()
            End If
        ElseIf e.CommandName = "Approve" Then
            Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Payout approved'")
            Using dr As SqlDataReader = eQuote.GetDataReader("SELECT UserId FROM [Order] WHERE [Order].Id = " + OrderId.ToString + "")
                Try
                    dr.Read()
                    Dim IsKYCApproved As Boolean = WMC.Logic.CheckKyc.isKYCApproved(OrderId)
                    If Not IsKYCApproved Then
                        Dim message As String = "Please Approve the KYC before proceding"
                        Dim sb As New System.Text.StringBuilder()
                        sb.Append("<script type = 'text/javascript'>")
                        sb.Append("window.onload=function(){")
                        sb.Append("alert('")
                        sb.Append(message)
                        sb.Append("')};")
                        sb.Append("</script>")
                        ClientScript.RegisterClientScriptBlock(Me.GetType(), "alert", sb.ToString())
                    Else
                        If WMC.IsOrderStatus(statusOrder, OrderId) Then
                            WMC.AddStatusChange(OrderId, statusOrder, s, Session("UserId"))
                            eQuote.ExecuteNonQuery("UPDATE [Order] SET Approved = { fn NOW() }, ApprovedBy = " + Session("UserId").ToString + ", Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
                            GridViewOrder.SelectedIndex = -1
                            GridViewOrder.DataBind()
                            PanelNote.Visible = False
                            PanelTrustCalculation.Visible = False
                        End If
                        If WMC.IsOrderStatus(statusOrder1, OrderId) Then
                            WMC.AddStatusChange(OrderId, statusOrder1, s, Session("UserId"))
                            eQuote.ExecuteNonQuery("UPDATE [Order] SET Approved = { fn NOW() }, ApprovedBy = " + Session("UserId").ToString + ", Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
                            GridViewOrder.SelectedIndex = -1
                            GridViewOrder.DataBind()
                            PanelNote.Visible = False
                            PanelTrustCalculation.Visible = False
                        End If
                    End If
                Finally
                    dr.Close()
                End Try
            End Using
        ElseIf e.CommandName = "ReviewKYC" Then
            Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Customer Response Pending'")
            If WMC.IsOrderStatus(statusOrder, OrderId) Or WMC.IsOrderStatus(statusOrder1, OrderId) Then
                Dim Status
                If WMC.IsOrderStatus(statusOrder, OrderId) Then
                    Status = statusOrder
                Else
                    Status = statusOrder1
                End If
                WMC.AddStatusChange(OrderId, Status, s, Session("UserId"))
                eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
                Dim nUserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + OrderId.ToString)
                eQuote.ExecuteNonQuery("INSERT INTO KycFile (UserId, Type, UniqueFilename, OriginalFilename) VALUES (" + nUserId.ToString + ", 1, N'EnforceKYC', N'EnforceKYC')")
                Using dr As SqlDataReader = eQuote.GetDataReader("SELECT [Order].Id, [Order].SiteId, [Site].[Text], [Order].CreditCardUserIdentity, [Order].Number, [Order].Amount, Currency.Code, [Order].Quoted, [Order].Name, [Order].Email, [Order].bccAddress, [Order].CardNumber, [Order].TxSecret FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN [Site] ON [Order].SiteId = [Site].Id WHERE [Order].Id =" + OrderId.ToString)
                    Try
                        If dr.HasRows Then
                            While dr.Read()
                                WMC.SendEmail("KYCRequest", dr!SiteId, dr!Name, dr!Email, dr!Number)
                                s = eQuote.ExecuteScalar("SELECT Id FROM AuditTrailStatus WHERE Text = N'SentEmail'")
                                Dim m As String = "KYCRequest" + vbCrLf + "Number:" + dr!Number
                                eQuote.ExecuteNonQuery("INSERT INTO AuditTrail(OrderId, Status, Message, Created) VALUES (" + dr!Id.ToString + ", " + s.ToString + ", N'" + m + "', { fn NOW() })")
                                GridViewAuditTrail.DataBind()
                            End While
                        End If

                        'If dr.HasRows Then
                        '    While dr.Read()
                        '        WMC.Utilities.EmailHelper.SendEmail(dr!Email, "RequestKycDocuments", New Dictionary(Of String, Object) From
                        '           {
                        '               {"UserIdentity", dr!CreditCardUserIdentity},
                        '               {"SiteName", dr!Text},
                        '               {"UserFirstName", dr!Name},
                        '               {"OrderNumber", dr!Number},
                        '               {"OrderCompleted", dr!Quoted},
                        '               {"OrderAmount", dr!Amount},
                        '               {"OrderCurrency", dr!Code},
                        '               {"CreditCard", dr!CardNumber}
                        '            }, dr!Text)
                        '        'WMC.SendEmail("RequestKycDocuments.htm", dr!SiteId, dr!Name, dr!Email, "YOUR ORDER #" + dr!Number + " IS NOW PAUSED", dr!Number)
                        '        s = eQuote.ExecuteScalar("SELECT Id FROM AuditTrailStatus WHERE Text = N'SentEmail'")
                        '        Dim m As String = "RequestKycDocuments.htm" + vbCrLf + "Number:" + dr!Number
                        '        eQuote.ExecuteNonQuery("INSERT INTO AuditTrail(OrderId, Status, Message, Created) VALUES (" + dr!Id.ToString + ", " + s.ToString + ", N'" + m + "', { fn NOW() })")
                        '        GridViewAuditTrail.DataBind()
                        '    End While
                        'End If
                    Finally
                        dr.Close()
                    End Try
                    AddNote(OrderId, "KYC Documents Requested")
                    GridViewOrder.SelectedIndex = -1
                    GridViewOrder.DataBind()
                End Using
            End If

        ElseIf e.CommandName = "TxSecret" Then
            Using dr As SqlDataReader = eQuote.GetDataReader("SELECT [Order].Id, [Order].SiteId, [Site].[Text], [Order].CreditCardUserIdentity, [Order].Number, [Order].Amount, Currency.Code, [Order].Quoted, [Order].Name, [Order].Email, [Order].bccAddress, [Order].CardNumber, [Order].TxSecret FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN [Site] ON [Order].SiteId = [Site].Id WHERE [Order].Id = " + OrderId.ToString)
                Try
                    If dr.HasRows Then
                        While dr.Read()
                            WMC.Utilities.EmailHelper.SendEmail(dr!Email, "RequestTxSecret", New Dictionary(Of String, Object) From
                                   {
                                       {"UserIdentity", dr!CreditCardUserIdentity},
                                       {"SiteName", dr!Text},
                                       {"UserFirstName", dr!Name},
                                       {"OrderNumber", dr!Number},
                                       {"OrderCompleted", dr!Quoted},
                                       {"OrderAmount", dr!Amount},
                                       {"OrderCurrency", dr!Code},
                                       {"CreditCard", dr!CardNumber}
                                    }, dr!Text)
                        End While
                    End If
                Finally
                    dr.Close()
                End Try
            End Using
            Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Customer Response Pending'")
            WMC.AddStatusChange(OrderId, statusOrder, s, Session("UserId"))
            eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
            AddNote(GridViewOrder.SelectedValue, "Tx Secret Requested")
            GridViewOrder.SelectedIndex = -1
            GridViewOrder.DataBind()
        ElseIf e.CommandName = "MoveToEDD" Then
            Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Enhanced Due Diligence'")
            If WMC.IsOrderStatus(statusOrder, OrderId) Then
                WMC.AddStatusChange(OrderId, statusOrder, s, Session("UserId"))
                eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
                GridViewOrder.SelectedIndex = -1
                GridViewOrder.DataBind()
            End If
            If WMC.IsOrderStatus(statusOrder1, OrderId) Then
                WMC.AddStatusChange(OrderId, statusOrder1, s, Session("UserId"))
                eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
                GridViewOrder.SelectedIndex = -1
                GridViewOrder.DataBind()
            End If
            'ElseIf e.CommandName = "TxApproved" Then
            '    ApproveCard(OrderId)
            '    GridViewOrder.DataBind()
            '    GridViewOrder.SelectedIndex = -1
            '    PanelNote.Visible = False
            '    PanelTrustCalculation.Visible = False
        ElseIf e.CommandName = "CancelOrder" Then
            Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Cancel'")
            If WMC.IsOrderStatus(statusOrder, OrderId) Then
                WMC.AddStatusChange(OrderId, statusOrder, s, Session("UserId"))
                eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
                GridViewOrder.SelectedIndex = -1
                GridViewOrder.DataBind()
            End If
            If WMC.IsOrderStatus(statusOrder1, OrderId) Then
                WMC.AddStatusChange(OrderId, statusOrder1, s, Session("UserId"))
                eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
                GridViewOrder.SelectedIndex = -1
                GridViewOrder.DataBind()
            End If
            Using dr As SqlDataReader = eQuote.GetDataReader("SELECT [Order].Id, [Order].SiteId, [Site].[Text], [Order].CreditCardUserIdentity, [Order].Number, [Order].Amount, Currency.Code, [Order].Quoted, [Order].Name, [Order].Email, [Order].bccAddress, [Order].CardNumber, [Order].TxSecret FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN [Site] ON [Order].SiteId = [Site].Id WHERE [Order].Id =" + OrderId.ToString)
                Try
                    If dr.HasRows Then
                        While dr.Read()
                            WMC.SendEmail("OrderCancelled", dr!SiteId, dr!Name, dr!Email, dr!Number)
                            s = eQuote.ExecuteScalar("SELECT Id FROM AuditTrailStatus WHERE Text = N'SentEmail'")
                            Dim m As String = "RequestKycDocuments.htm" + vbCrLf + "Number:" + dr!Number
                            eQuote.ExecuteNonQuery("INSERT INTO AuditTrail(OrderId, Status, Message, Created) VALUES (" + dr!Id.ToString + ", " + s.ToString + ", N'" + m + "', { fn NOW() })")
                            GridViewAuditTrail.DataBind()
                        End While
                    End If
                Finally
                    dr.Close()
                End Try
            End Using
        ElseIf e.CommandName = "KYCDeclined" Then
            Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'KYC Decline'")
            If WMC.IsOrderStatus(statusOrder, OrderId) Then
                WMC.AddStatusChange(OrderId, statusOrder, s, Session("UserId"))
                eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
                GridViewOrder.SelectedIndex = -1
                GridViewOrder.DataBind()
            End If
            If WMC.IsOrderStatus(statusOrder1, OrderId) Then
                WMC.AddStatusChange(OrderId, statusOrder1, s, Session("UserId"))
                eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
                GridViewOrder.SelectedIndex = -1
                GridViewOrder.DataBind()
            End If
            Using dr As SqlDataReader = eQuote.GetDataReader("SELECT [Order].Id, [Order].SiteId, [Site].[Text], [Order].CreditCardUserIdentity, [Order].Number, [Order].Amount, Currency.Code, [Order].Quoted, [Order].Name, [Order].Email, [Order].bccAddress, [Order].CardNumber, [Order].TxSecret FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN [Site] ON [Order].SiteId = [Site].Id WHERE [Order].Id =" + OrderId.ToString)
                Try
                    If dr.HasRows Then
                        While dr.Read()
                            WMC.SendEmail("KYCDeclined", dr!SiteId, dr!Name, dr!Email, dr!Number)
                            s = eQuote.ExecuteScalar("SELECT Id FROM AuditTrailStatus WHERE Text = N'SentEmail'")
                            Dim m As String = "RequestKycDocuments.htm" + vbCrLf + "Number:" + dr!Number
                            eQuote.ExecuteNonQuery("INSERT INTO AuditTrail(OrderId, Status, Message, Created) VALUES (" + dr!Id.ToString + ", " + s.ToString + ", N'" + m + "', { fn NOW() })")
                            GridViewAuditTrail.DataBind()
                        End While
                    End If
                Finally
                    dr.Close()
                End Try
            End Using
        ElseIf e.CommandName = "ApproveEDD" Then
            Using dr As SqlDataReader = eQuote.GetDataReader("SELECT UserId FROM [Order] WHERE [Order].Id = " + OrderId.ToString + "")
                dr.Read()
                Dim UserId As Integer = dr!UserId
                Using dr1 As SqlDataReader = eQuote.GetDataReader("SELECT Tier,Id,TierTwoApproved ,TierTwoApprovedBy FROM [User] WHERE [User].Id = " + UserId.ToString + "")
                    Try
                        dr1.Read()
                        Dim kycNotApprovedCount As Integer = eQuote.ExecuteScalar("SELECT Count(*) FROM [KycFile] Where [KycFile].UserId=" + UserId.ToString + " AND [KycFile].Approved IS NULL")
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
                            WMC.AddStatusChange(dr1!Id, WMC.Data.Enums.CustomerTier.Tier3, dr1!Tier, Session("UserId"))
                            Dim UserTierLevel As Integer = WMC.Data.Enums.CustomerTier.Tier3
                            eQuote.ExecuteNonQuery("UPDATE [User] SET Tier = " + UserTierLevel.ToString + " WHERE [User].Id= " + dr1!Id.ToString + "")
                            eQuote.ExecuteNonQuery("UPDATE [User]  SET TierThreeApproved = { fn NOW() }, TierThreeApprovedBy = " + Session("UserId").ToString + " where [User].Id= " + dr1!Id.ToString)
                            If dr1!TierTwoApprovedBy.Equals(DBNull.Value) Then
                                eQuote.ExecuteNonQuery("UPDATE [User]  SET TierTwoApproved = { fn NOW() }, TierTwoApprovedBy = " + Session("UserId").ToString + " where [User].Id= " + dr1!Id.ToString)
                            End If
                            GridViewOrder.SelectedIndex = -1
                            GridViewOrder.DataBind()
                        End If
                    Finally
                        dr1.Close()
                        dr.Close()
                    End Try
                End Using
            End Using
        ElseIf e.CommandName = "Select" Then
            Dim tl = New WMC.Logic.TrustLogic.TrustLogic
            ImageDoc.Visible = False
            PanelNote.Visible = True
            PanelKeyIndicators.Visible = False
            PanelTrustCalculation.Visible = True
            Dim UserId1 As Integer = eQuote.ExecuteScalar("SELECT [Order].UserId FROM [Order] WHERE [Order].Id =" + OrderId.ToString)
            Dim UserLogs = eQuote.GetDataSet("SELECT AuditTrail.OrderId, AuditTrail.Status, AuditTrail.Message, AuditTrail.Created, AuditTrailStatus.Text FROM AuditTrail INNER JOIN AuditTrailStatus ON AuditTrail.Status = AuditTrailStatus.Id WHERE AuditTrail.Message LIKE 'CUSTOMER:'+  CAST(" & UserId1 & " AS NVARCHAR(10)) + '%' ORDER BY AuditTrail.Created")
            GridViewUserAuditTrail.DataSource = UserLogs.Tables(0)
            GridViewUserAuditTrail.DataBind()
            Using dr As SqlDataReader = eQuote.GetDataReader("SELECT [Order].UserId, [Order].CryptoAddress,[Order].PaymentType, [Order].CardNumber, [Order].IP, [Order].Id, [User].Email, [User].Phone, [Order].CreditCardUserIdentity FROM [Order] INNER JOIN [User] ON [Order].UserId = [User].Id WHERE [Order].Id =" + OrderId.ToString)
                Try
                    dr.Read()
                    Dim UserId As Integer = dr!UserId
                    Dim PhoneNumber As String = dr!Phone
                    Dim PaymentType As Integer = dr!PaymentType
                    Dim CardNumber As String = ""
                    If PaymentType = WMC.Data.Enums.OrderPaymentType.CreditCard Then
CardNumber:             If Session("ComplianceView") = "Received Crypto Payment" Then CardNumber = "" Else CardNumber = dr!CardNumber
                    End If
                    Dim IP As String = dr!IP
                    Dim Email As String = dr!Email
                    Dim CryptoAddress As String = dr!CryptoAddress
                    Dim CreditCardUserIdentity As String = dr!CreditCardUserIdentity.ToString
                    Dim TxLimitEUR As Integer = 150
                    Dim OrderSumTotalCardEUR As Decimal
                    If CardNumber.Length > 0 Then OrderSumTotalCardEUR = tl.GetOrderSumTotalEUR(OrderId, CardNumber)
                    Dim OrderSumEUR As Decimal = tl.GetOrderSumEUR(OrderId)
                    Dim OrderSumTotalEUR As Decimal = WMC.GetOrderSumTotalEUR(OrderId)
                    Dim CardApproved As Boolean = tl.IsCardApproved(OrderId)
                    Dim IsPhoneVitual As Boolean = tl.IsPhoneVitual(OrderId)
                    'Dim SpeedAlert As Boolean = tl.CrossedSpeedLimit(OrderId)
                    Dim CardUsedElsewhere As Integer = tl.GetCardUsedElsewhere(OrderId)
                    Dim EmailUsedElsewhere As Integer = tl.GetEmailUsedElsewhere(OrderId)
                    Dim IpUsedElsewhere As Integer = tl.GetIpUsedElsewhere(OrderId).ToString
                    Dim OrdersByStatus As Integer = WMC.GetOrdersByStatus(UserId, OrderId, "Completed")
                    Dim OrderSpan As Integer = CInt(WMC.GetOrderSpan(OrderId))
                    Dim OrderSpanCard As Integer
                    If CardNumber.Length > 0 Then OrderSpanCard = CInt(WMC.GetOrderSpan(OrderId, CardNumber))
                    Dim IsUserTrusted As Boolean = tl.IsUserTrusted(OrderId)
                    Dim IsKycApproved As Boolean = tl.IsKycApproved(OrderId)
                    Dim PhoneCountry As String = WMC.GetPhoneCountry(OrderId)
                    'Dim PhoneCountry As String = tl.GetPhoneCountry(OrderId)
                    Dim CardCountry As String = ""
                    If CardNumber.Length > 0 Then CardCountry = tl.GetCardCountry(OrderId)
                    Dim IpCountry As String
                    Try
                        IpCountry = tl.GetIpCountry(OrderId)
                    Catch ex As Exception
                        IpCountry = "?"
                    End Try

                    'Dim CountryTrust As String = tl.GetCountryTrustLevel(OrderId)
                    Dim CountryTrust As String = WMC.GetCountryTrustLevel(OrderId)
                    Dim SiteTrust As String = tl.GetSiteTrustLevel(OrderId)


                    '--------------Trust Level Calculation

                    LabelCardApproved.Text = CardApproved : If CardApproved Then LabelCardApproved.BackColor = Drawing.Color.FromName("#66FF66")
                    LabelTxLimit.Text = CInt(TxLimitEUR * CountryTrust * SiteTrust).ToString
                    If CardNumber.Length > 0 Then LabelCardTotal.Text = CInt(OrderSumTotalCardEUR + OrderSumEUR).ToString  ' Removed + OrderSumTotalEUR

                    If CardNumber.Length > 0 Then
                        If CInt(OrderSumTotalCardEUR + OrderSumTotalCardEUR) > CInt(TxLimitEUR * CountryTrust * SiteTrust) And Not CardApproved Then
                            LabelCardTotal.BackColor = Drawing.Color.FromName("#FFFF66")
                        Else
                            LabelCardTotal.BackColor = Drawing.Color.FromName("#66FF66")
                        End If
                    End If

                    LabelPhoneVirtual.Text = IsPhoneVitual : If IsPhoneVitual Then LabelPhoneVirtual.BackColor = Drawing.Color.FromName("#FFA8C5")
                    'LabelSpeedAlert.Text = SpeedAlert : If SpeedAlert Then LabelSpeedAlert.BackColor = Drawing.Color.FromName("#FFA8C5")

                    LinkButtonCreditCardUsedElsewhere.Text = CardUsedElsewhere.ToString : If CardUsedElsewhere > 0 Then LinkButtonCreditCardUsedElsewhere.CommandArgument = CardNumber : LinkButtonCreditCardUsedElsewhere.BackColor = Drawing.Color.FromName("#FFFF66")
                    LinkButtonEmailUsedElsewhere.Text = EmailUsedElsewhere.ToString : If EmailUsedElsewhere > 0 Then LinkButtonEmailUsedElsewhere.CommandArgument = Email : LinkButtonEmailUsedElsewhere.BackColor = Drawing.Color.FromName("#FFFF66")
                    LinkButtonIpUsedElsewhere.Text = IpUsedElsewhere.ToString : If IpUsedElsewhere > 0 Then LinkButtonIpUsedElsewhere.CommandArgument = IP : LinkButtonIpUsedElsewhere.BackColor = Drawing.Color.FromName("#FFFF66")

                    'LinkButtonIpUsedElsewhere.Text = WMC.GetIpUsedElsewhere(OrderId, LinkButtonIpUsedElsewhere)
                    'LinkButtonIpUsedElsewhere.Text = tl.GetIpUsedElsewhere(OrderId).ToString

                    'LabelCardUS.Text = WMC.IsCardUS(OrderId, LabelCardUS)

                    LabelCompleted.Text = WMC.GetOrdersByStatus(UserId, OrderId, "Completed")

                    WMC.GetChainalysisWithdrawalAddress(CryptoAddress, UserId, OrderId, LabelChainalysis)
                    HyperLinkChainalysis.NavigateUrl = "https://www.chainalysis.com/reactor/info-panel/cluster/" + CryptoAddress.ToString + "?tab=ejected&offset=0"

                    LabelOrderSpan.Text = OrderSpan.ToString
                    'LabelOrderSpan.Text = CInt(tl.).ToString

                    If OrderSpan >= 45 Then CheckBoxIsUserTrusted.BackColor = Drawing.Color.LightGreen Else CheckBoxIsUserTrusted.BackColor = Drawing.Color.White
                    'LabelCardSpan.Text = CInt(WMC.GetOrderSpan(OrderId, CardNumber, LabelCardSpan)).ToString
                    If CardNumber.Length > 0 Then LabelCardSpan.Text = OrderSpanCard.ToString

                    If CInt(WMC.GetOrderSpan(OrderId, CardNumber)) >= 45 And Not WMC.IsCardApproved(OrderId) Then ButtonApproveCard.Visible = True : ButtonApproveCard.CommandArgument = OrderId.ToString Else ButtonApproveCard.Visible = False

                    If OrderSpan >= 45 And Not CardApproved Then ButtonApproveCard.Visible = True : ButtonApproveCard.CommandArgument = OrderId.ToString Else ButtonApproveCard.Visible = False
                    If CardNumber.Length > 0 Then
                        If Not CardApproved Then
                            'ButtonApproveCard.CommandArgument = OrderId.ToString
                            'ButtonApproveCard.Visible = True
                            If OrderSpanCard >= 45 Then
                                ButtonApproveCard.BackColor = Drawing.Color.LightGreen
                                ButtonApproveCard.CommandName = " (By CardSpan)"
                            Else
                                ButtonApproveCard.BackColor = Drawing.Color.LightGray
                                ButtonApproveCard.CommandName = " (By Admin)"
                            End If
                            'Else
                            '    ButtonApproveCard.Visible = False
                        End If
                    End If


                    ButtonResetTxAttempt.CommandArgument = OrderId.ToString
                    LabelOrderSUM.Text = String.Format("{0:n0}", OrderSumTotalEUR)
                    'LabelOrderTotal.Text = String.Format("{0:n0}", WMC.GetOrderSumTotalEUR(OrderId))
                    'LabelOrder30Days.Text = String.Format("{0:n0}", WMC.GetOrderSum30DaysEUR(OrderId))

                    'CheckBoxIsUserTrusted.Checked = WMC.IsUserTrusted(OrderId, LabelUserIsTrusted) : LabelUserIsTrusted.Text = CheckBoxIsUserTrusted.Checked
                    CheckBoxIsUserTrusted.Checked = IsUserTrusted : LabelUserIsTrusted.Text = CheckBoxIsUserTrusted.Checked

                    'WMC.GetOrigin(OrderId, LabelOrigin)

                    LabelCards.Text = CInt(WMC.GetDistinctUsedCards(UserId, OrderId, LabelCards)).ToString
                    LabelKYCDeclined.Text = WMC.GetOrdersByStatus(UserId, OrderId, "KYC Decline")


                    'LabelNames.Text = CInt(WMC.GetDifferentNameUser(OrderId, LabelNames)).ToString
                    'LabelEmails.Text = CInt(WMC.GetDifferentEmailUser(OrderId, LabelEmails)).ToString
                    'LabelKycApproved.Text = WMC.IsKycApproved(UserId, OrderId, LabelKycApproved)
                    LabelKycApproved.Text = IsKycApproved


                    'LabelPhoneCountry.Text = WMC.GetPhoneCountry(OrderId)

                    LabelPhoneCountry.Text = PhoneCountry
                    LabelCardCountry.Text = CardCountry
                    LabelIPCountry.Text = IpCountry

                    PayoutAmountUpdate(OrderId)

                    HyperLinkTx.NavigateUrl = "https://app.wemovecoins.com/TxSecretRequest?useridentity=" + CreditCardUserIdentity



                    'Dim riskscore As Decimal = WMC.TrustLevelCalculation(OrderId, LabelTrustMessageNew)
                    'Dim riskscore As Decimal = tl.TrustLevelCalculation(OrderId, LabelTrustMessageNew.Text)

                    'LabelTrustMessage.Text = "RISKSCORE: " + riskscore.ToString
                    Try
                        Dim TrustMessage As String = eQuote.ExecuteScalar("SELECT TOP (1) Message FROM AuditTrail WHERE OrderId = " + OrderId.ToString + " AND Status = 11 ORDER BY Created DESC")
                        Dim tstart As Integer = TrustMessage.IndexOf("TrustMessage:")
                        Dim tend As Integer = TrustMessage.IndexOf("RiskScore:")
                        LabelTrustMessage.Text = TrustMessage.Substring(tstart, tend - tstart)

                    Catch ex As Exception

                    End Try

                    PanelSellDetails.Visible = True
                Finally
                    dr.Close()
                End Try

            End Using
        End If
        SetCountStatus()
    End Sub

    Protected Sub ButtonDeleteSelected_Click(sender As Object, e As EventArgs) Handles ButtonDeleteSelected.Click
        Dim OrderId As String
        For Each item As GridViewRow In GridViewOrder.Rows
            If TryCast(item.Cells(0).FindControl("cbSelect"), CheckBox).Checked Then
                OrderId = item.Cells(4).Text
                eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = 0 WHERE Number = N'" + OrderId.ToString + "'")
            End If
        Next
        GridViewOrder.DataBind()
    End Sub

    Protected Sub btnApproveCardDoc_Click(sender As Object, e As EventArgs) Handles btnApproveCardDoc.Click
        Dim OrderId As Integer = GridViewOrder.SelectedValue
        Dim CC As String = eQuote.ExecuteScalar("SELECT CardNumber FROM [Order] WHERE Id = " + OrderId.ToString)
        eQuote.ExecuteNonQuery("UPDATE KycFile SET Approved = { fn NOW() }, ApprovedBy = " + Session("UserId").ToString + ", Note = N'" + CC + "' WHERE Id IN (SELECT KycFile_2.Id FROM KycFile AS KycFile_2 INNER JOIN KycType ON KycFile_2.Type = KycType.Id INNER JOIN [Order] ON KycFile_2.UserId = [Order].UserId CROSS JOIN KycFile AS KycFile_1 WHERE ([Order].Id = " + OrderId.ToString + ") AND (KycType.Text = N'CardApproval') AND (KycFile_2.Approved IS NULL))")
        ApproveCard(GridViewOrder.SelectedValue, " by CardDoc uploads")
        Response.Redirect("Compliance.aspx")
        'GridViewOrder.DataBind()
        'GridViewOrder.SelectedIndex = -1
    End Sub

    Sub AddNote(id As Integer, note As String)
        Dim GetMemoLogScript As String = String.Format(" {0:d}", Date.Now) + " at " + String.Format(" {0:t}", Date.Now) + " by " + eQuote.ExecuteScalar("SELECT Lname FROM [User] WHERE Id = " + Session("UserId").ToString) + ":<br />" + note + "<br />"
        eQuote.ExecuteNonQuery("UPDATE [Order] SET Note = { fn CONCAT('" + Replace(GetMemoLogScript, Environment.NewLine, "<br/>") + "', { fn IFNULL(Note, '') }) } WHERE  Id = " + id.ToString)
    End Sub

    Function ApproveCard(OrderId As Integer, Optional ByVal Txt As String = "") As Boolean
        AddNote(OrderId, " CC Approved" + Txt)
        Return eQuote.ExecuteNonQuery("UPDATE [Order] SET CardApproved = { fn NOW() } WHERE  Id = " + OrderId.ToString)
    End Function

    ' ********** PanelTrustCalculation ***************************************
    Protected Sub CheckBoxIsUserTrusted_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxIsUserTrusted.CheckedChanged
        WMC.TrustUser(GridViewOrder.SelectedValue, Session("UserId"), CheckBoxIsUserTrusted.Checked)
        GridViewOrder.SelectedIndex = -1
        GridViewOrder.DataBind()
        PanelNote.Visible = False
        PanelTrustCalculation.Visible = False
    End Sub

    Protected Sub ButtonApproveCard_Click(sender As Object, e As EventArgs) Handles ButtonApproveCard.Click
        ApproveCard(CInt(ButtonApproveCard.CommandArgument), ButtonApproveCard.CommandName)
        GridViewOrder.SelectedIndex = -1
        GridViewOrder.DataBind()
        PanelNote.Visible = False
        PanelTrustCalculation.Visible = False
    End Sub

    Protected Sub ButtonResetTxAttempt_Click(sender As Object, e As EventArgs) Handles ButtonResetTxAttempt.Click
        WMC.ResetTxAttempt(CInt(ButtonResetTxAttempt.CommandArgument))
    End Sub

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
                CType(e.Row.FindControl("ButtonApprove"), LinkButton).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonApprove"), LinkButton).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")

                CType(e.Row.FindControl("ButtonMoveToCompliance"), LinkButton).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonMoveToCompliance"), LinkButton).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
                CType(e.Row.FindControl("ButtonCustomerPending"), LinkButton).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonCustomerPending"), LinkButton).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
                CType(e.Row.FindControl("ButtonMoveToEDD"), LinkButton).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonMoveToEDD"), LinkButton).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")

                CType(e.Row.FindControl("ButtonReviewKYC"), LinkButton).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonReviewKYC"), LinkButton).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
                CType(e.Row.FindControl("ButtonTxSecret"), LinkButton).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonTxSecret"), LinkButton).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")

                CType(e.Row.FindControl("ButtonKYCDeclined"), LinkButton).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonKYCDeclined"), LinkButton).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
                CType(e.Row.FindControl("ButtonCancelOrder"), LinkButton).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonCancelOrder"), LinkButton).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
                CType(e.Row.FindControl("ButtonApproveEDD"), LinkButton).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonApproveEDD"), LinkButton).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
            End If
        End If
        If Session("ComplianceView") = "Enhanced Due Diligence" Or Session("ComplianceView") = "Compliance Officer Approval" Then
            If e.Row.RowType = DataControlRowType.DataRow Then

                If (e.Row.Cells(21).Text = "ElevatedRisk") Then
                    e.Row.Cells(21).ForeColor = System.Drawing.Color.Orange
                    e.Row.Cells(21).Font.Bold = True
                End If
                If (e.Row.Cells(21).Text = "HighRisk") Then
                    e.Row.Cells(21).ForeColor = System.Drawing.Color.Red
                    e.Row.Cells(21).Font.Bold = True
                End If
            End If
        End If

    End Sub

    Protected Sub GridViewUser_DataBound(sender As Object, e As EventArgs) Handles GridViewUser.DataBound
        'https://ACd32f9f403312208a96707ec9356eb2ea:74dfe135a1259269c3a7817ffd9fd6e6@lookups.twilio.com/v1/PhoneNumbers/+447492195433?AddOns=whitepages_pro_phone_rep
    End Sub

    Protected Sub GridViewUser_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewUser.RowDataBound
        'If e.Row.RowType = DataControlRowType.DataRow Then
        '    Dim hl As HyperLink = CType(e.Row.FindControl("HyperLinkUserLookup"), HyperLink)
        '    If hl.NavigateUrl.Length Then
        '        hl.NavigateUrl = "https://www.chainalysis.com/reactor/info-panel/custom-cluster/" + hl.Text + "?tab=ccEjected&orgCC=true&offset=0"
        '    End If
        'End If
        If e.Row.RowType = DataControlRowType.DataRow Then
            If e.Row.RowState = DataControlRowState.Selected Or e.Row.RowState = DataControlRowState.Normal Or e.Row.RowState = DataControlRowState.Alternate Then
                Dim lb As HyperLink = CType(e.Row.FindControl("HyperLinkUserLookup"), HyperLink)
                lb.NavigateUrl = "User.aspx?Id=" + lb.NavigateUrl
            End If
        End If
    End Sub

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

    Protected Sub ButtonPanelClose_Click(sender As Object, e As EventArgs) Handles ButtonPanelClose.Click
        PanelKeyIndicators.Visible = False
    End Sub

    Protected Sub LinkButtonEmailUsedElsewhere_Click(sender As Object, e As EventArgs) Handles LinkButtonEmailUsedElsewhere.Click
        Dim sql As String = "SELECT [User].Fname AS UserName, [User].Phone, [Order].Number, [Order].Quoted, [Order].Rate, CONVERT(varchar(40), CAST([Order].Amount AS integer)) + ' ' + Currency.Code AS Amount, [Order].Name AS OrderName, [Order].Note, [Order].CryptoAddress, [Order].Email, [Order].IP, [Order].CardNumber, OrderStatus.Text AS Status, [Order].SiteId FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN    Currency ON [Order].CurrencyId = Currency.Id INNER JOIN [User] ON [Order].UserId = [User].Id"
        Dim nLineId As Integer = GridViewOrder.SelectedDataKey(0)
        Dim nUserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + nLineId.ToString)

        PanelKeyIndicators.Visible = True
        GridViewKeyIndicator.DataSource = eQuote.GetDataReader(sql + " WHERE  ([Order].Email = N'" + LinkButtonEmailUsedElsewhere.CommandArgument + "') AND (NOT (UserId = " + nUserId.ToString + ")) ORDER BY [Order].Id DESC")
        GridViewKeyIndicator.DataBind()
    End Sub

    Protected Sub LinkButtonCreditCardUsedElsewhere_Click(sender As Object, e As EventArgs) Handles LinkButtonCreditCardUsedElsewhere.Click
        Dim sql As String = "SELECT [User].Fname AS UserName, [User].Phone, [Order].Number, [Order].Quoted, [Order].Rate, CONVERT(varchar(40), CAST([Order].Amount AS integer)) + ' ' + Currency.Code AS Amount, [Order].Name AS OrderName, [Order].Note, [Order].CryptoAddress, [Order].Email, [Order].IP, [Order].CardNumber, OrderStatus.Text AS Status, [Order].SiteId FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN    Currency ON [Order].CurrencyId = Currency.Id INNER JOIN [User] ON [Order].UserId = [User].Id"
        Dim nLineId As Integer = GridViewOrder.SelectedDataKey(0)
        Dim nUserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + nLineId.ToString)

        PanelKeyIndicators.Visible = True
        Using dr As SqlDataReader = eQuote.GetDataReader(sql + " WHERE  ([Order].CardNumber = N'" + LinkButtonCreditCardUsedElsewhere.CommandArgument + "') AND (NOT (UserId = " + nUserId.ToString + ")) ORDER BY [Order].Id DESC")
            Try
                GridViewKeyIndicator.DataSource = dr
                GridViewKeyIndicator.DataBind()
            Finally
                dr.Close()
            End Try
        End Using
    End Sub

    Protected Sub LinkButtonIpUsedElsewhere_Click(sender As Object, e As EventArgs) Handles LinkButtonIpUsedElsewhere.Click
        Dim sql As String = "SELECT [User].Fname AS UserName, [User].Phone, [Order].Number, [Order].Quoted, [Order].Rate,CONVERT(varchar(40), CAST([Order].Amount AS integer)) + ' ' + Currency.Code AS Amount, [Order].Name AS OrderName, [Order].Note, [Order].CryptoAddress, [Order].Email, [Order].IP, [Order].CardNumber, OrderStatus.Text AS Status, [Order].SiteId FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN    Currency ON [Order].CurrencyId = Currency.Id INNER JOIN [User] ON [Order].UserId = [User].Id"
        Dim nLineId As Integer = GridViewOrder.SelectedDataKey(0)
        Dim nUserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + nLineId.ToString)

        PanelKeyIndicators.Visible = True
        Using dr As SqlDataReader = eQuote.GetDataReader(sql + " WHERE  ([Order].IP = N'" + LinkButtonIpUsedElsewhere.CommandArgument + "') AND (NOT (UserId = " + nUserId.ToString + ")) ORDER BY [Order].Id DESC")
            Try
                GridViewKeyIndicator.DataSource = dr
                GridViewKeyIndicator.DataBind()
            Finally
                dr.Close()
            End Try
        End Using
    End Sub

    Protected Sub ButtonAddNote_Click(sender As Object, e As EventArgs) Handles ButtonAddNote.Click
        Dim nLineId As Integer = GridViewOrder.SelectedValue
        AddNote(nLineId, TextBoxNote.Text)
        Response.Redirect("Compliance.aspx")

        'Dim GetMemoLogScript As String = String.Format(" {0:d}", Date.Now) + " at " + String.Format(" {0:t}", Date.Now) + " by " + eQuote.ExecuteScalar("SELECT Lname FROM [User] WHERE Id = " + Session("UserId").ToString) + ":<br />" + TextBoxNote.Text + "<br />"
        'eQuote.ExecuteNonQuery("UPDATE [Order] SET Note = { fn CONCAT('" + Replace(GetMemoLogScript, Environment.NewLine, "<br/>") + "', { fn IFNULL(Note, '') }) } WHERE  Id = " + nLineId.ToString)


        'GridViewOrder.DataBind()
        'GridViewOrder.SelectedIndex = -1
        'TextBoxNote.Text = ""
        'DropDownListNoteOptions.SelectedIndex = 0
    End Sub

    Protected Sub DropDownListNoteOptions_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownListNoteOptions.SelectedIndexChanged
        If DropDownListNoteOptions.SelectedIndex > 0 Then
            TextBoxNote.Text = DropDownListNoteOptions.SelectedValue
        End If
    End Sub

    Protected Sub btnUpload_Click(sender As Object, e As EventArgs) Handles btnUpload.Click
        Dim filePath As String = FileUpload1.PostedFile.FileName
        Dim filename As String = Path.GetFileName(filePath)
        Dim ext As String = Path.GetExtension(filename).ToLower
        Dim contenttype As String = String.Empty
        Select Case ext
            Case ".doc"
                contenttype = "application/vnd.ms-word"
                Exit Select
            Case ".docx"
                contenttype = "application/vnd.ms-word"
                Exit Select
            Case ".xls"
                contenttype = "application/vnd.ms-excel"
                Exit Select
            Case ".xlsx"
                contenttype = "application/vnd.ms-excel"
                Exit Select
            Case ".jpeg"
                contenttype = "image/jpg"
                Exit Select
            Case ".jpg"
                contenttype = "image/jpg"
                Exit Select
            Case ".png"
                contenttype = "image/png"
                Exit Select
            Case ".gif"
                contenttype = "image/gif"
                Exit Select
            Case ".pdf"
                contenttype = "application/pdf"
                Exit Select
        End Select
        If contenttype <> String.Empty Then
            Dim fs As Stream = FileUpload1.PostedFile.InputStream
            Dim userId = GridViewUser.DataKeys(0).Value
            Dim kycTypeId = DropDownListKycType.SelectedValue
            WMC.Logic.KYCFileHandler.AddNewKYCFile(fs, userId, filename, kycTypeId)
            'Dim br As New BinaryReader(fs)
            'Dim bytes As Byte() = br.ReadBytes(fs.Length)
            'Dim strQuery As String = "INSERT INTO KycFile(UserId, [File], Type, UniqueFilename, OriginalFilename) VALUES (@UserId, @File, @Type, @UniqueFilename, @OriginalFilename)"
            'Dim cmd As New SqlCommand(strQuery)
            'Dim n As Integer = GridViewUser.DataKeys(0).Value
            'cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = n
            'cmd.Parameters.Add("@File", SqlDbType.Binary).Value = bytes
            'cmd.Parameters.Add("@Type", SqlDbType.Int).Value() = DropDownListKycType.SelectedValue
            'cmd.Parameters.Add("@UniqueFilename", SqlDbType.VarChar).Value = filename
            'cmd.Parameters.Add("@OriginalFilename", SqlDbType.VarChar).Value = filename
            'eQuote.ExecuteNonQuery(cmd)
            lblMessage.ForeColor = System.Drawing.Color.Green
            lblMessage.Text = "File Uploaded Successfully"
            GridViewKycFile.DataBind()
        Else
            lblMessage.ForeColor = System.Drawing.Color.Red
            lblMessage.Text = "File format not recognised." & " Upload Image/Word/PDF/Excel formats"
        End If
    End Sub

    Protected Sub GridViewKycFile_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridViewKycFile.RowCommand
        On Error Resume Next
        If e.CommandName = "Edit" Then GridViewKycFile.SelectedIndex = -1

        Dim nLineId As Integer = GridViewKycFile.DataKeys(Convert.ToInt32(e.CommandArgument)).Value
        If e.CommandName = "Reject" Then
            eQuote.ExecuteNonQuery("UPDATE KycFile SET Approved = NULL, ApprovedBy = NULL, Rejected = { fn NOW() }, RejectedBy = " + Session("UserId").ToString + " where Id= " + nLineId.ToString)
            GridViewKycFile.EditIndex = -1
        End If
        If e.CommandName = "UndoReject" Then
            eQuote.ExecuteNonQuery("UPDATE KycFile SET Approved = NULL, ApprovedBy = NULL, Rejected = NULL, RejectedBy = NULL where Id= " + nLineId.ToString)
            GridViewKycFile.EditIndex = -1
        End If
        If e.CommandName = "ApproveNow" Then
            eQuote.ExecuteNonQuery("UPDATE KycFile SET Approved = { fn NOW() }, ApprovedBy = " + Session("UserId").ToString + ", Rejected = NULL, RejectedBy = NULL where Id= " + nLineId.ToString)
            GridViewKycFile.DataBind()
            GridViewKycFile.EditIndex = -1
        End If
        If e.CommandName = "UndoApprove" Then
            eQuote.ExecuteNonQuery("UPDATE KycFile SET Approved = NULL, ApprovedBy = NULL, Rejected = NULL, RejectedBy = NULL where Id= " + nLineId.ToString)
            GridViewKycFile.EditIndex = -1
        End If
        If e.CommandName = "Obsolete" Then
            eQuote.ExecuteNonQuery("UPDATE KycFile SET Obsolete = { fn NOW() }, ObsoleteBy = " + Session("UserId").ToString + " where Id= " + nLineId.ToString)
            GridViewKycFile.EditIndex = -1
        End If
        If e.CommandName = "UndoObsolete" Then
            eQuote.ExecuteNonQuery("UPDATE KycFile SET Obsolete = NULL where Id= " + nLineId.ToString)
            GridViewKycFile.EditIndex = -1
        End If
        If e.CommandName = "Select" Then
            Dim btn As LinkButton = GridViewKycFile.Rows(Convert.ToInt32(e.CommandArgument)).FindControl("LinkButton1")
            btn.Focus()
            Dim strOname = eQuote.ExecuteScalar("SELECT OriginalFilename FROM KycFile WHERE Id = " + nLineId.ToString).ToString()
            Dim ext = strOname.Split(".").GetValue(1)
            If ext = "pdf" Then
                Dim embed As String = "<object data=""{0}{1}"" type=""application/pdf"" width=""550px"" height=""500px"">"
                embed += "</object>"
                ltEmbed.Visible = True
                ModalPopupExtender1.Show()
                imagePreview.Visible = False
                ltEmbed.Text = String.Format(embed, ResolveUrl("KYCImageHandler.ashx?ImageId="), nLineId.ToString)
            Else
                ltEmbed.Visible = False
                editImageBtn.Visible = True
                imagePreview.Visible = True
                ModalPopupExtender1.Show()
                imagePreview.Src = String.Format("KYCImageHandler.ashx?ImageId={0}", nLineId.ToString)
                imageId.Value = e.CommandArgument
            End If
        End If
        If e.CommandName = "ShowDoc" Then
            ' + nLineId.ToString)
            Dim strImg = eQuote.ExecuteScalar("SELECT UniqueFilename FROM KycFile WHERE Id = " + nLineId.ToString)
            If Not IsDBNull(strImg) Then
                If eQuote.FileExists(strImg) Then
                    ImageDoc.Visible = True
                    ImageDoc.ImageUrl = strImg
                End If
            End If
        End If
        If e.CommandName = "Delete" Then
            Dim KycFileId = e.CommandArgument
            eQuote.ExecuteNonQuery("DELETE FROM [OrderKycfile] WHERE [KycfileId] = N'" + KycFileId.ToString + "'")
            eQuote.ExecuteNonQuery("DELETE FROM [KycFile] WHERE [Id] = N'" + KycFileId.ToString + "'")
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

                CType(e.Row.FindControl("ButtonUndoReject"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonUndoApprove"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonObsolete"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonUndoObsolete"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonReject"), Button).Attributes.Add("onClick", "javascript: return confirm('Er du sikker?')")
                CType(e.Row.FindControl("ButtonApprove"), Button).Attributes.Add("onClick", "javascript: return confirm('Er du sikker?')")
            ElseIf e.Row.RowState = DataControlRowState.Selected Or e.Row.RowState = DataControlRowState.Normal Or e.Row.RowState = DataControlRowState.Alternate Then
                CType(e.Row.FindControl("LinkButtonDelete"), LinkButton).Attributes.Add("onClick", "javascript: return confirm('Er du sikker?')")
                If CType(e.Row.FindControl("LabelKycType"), Label).Text = "CardApproval" Then
                    e.Row.BackColor = Drawing.Color.GreenYellow
                    CType(e.Row.FindControl("ButtonApproveNow"), Button).Enabled = False
                End If
            End If
        End If
    End Sub

    Protected Sub GridViewSubOrders_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewSubOrders.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim lb As HyperLink = CType(e.Row.FindControl("HyperLinkIP"), HyperLink)
            If lb.NavigateUrl.Length Then
                lb.NavigateUrl = "http://www.infosniper.net/index.php?ip_address=" + lb.NavigateUrl.ToString + ""
            End If
        End If
    End Sub

    'Protected Sub DropDownListStatus_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownListStatus.SelectedIndexChanged
    '    GridViewOrder.DataBind()
    '    GridViewOrder.SelectedIndex = -1
    '    PanelNote.Visible = False
    '    PanelTrustCalculation.Visible = False
    'End Sub

    Protected Sub ButtonEnhancedDueDiligence_Click(sender As Object, e As EventArgs) Handles ButtonEnhancedDueDiligence.Click
        Session("ComplianceView") = "Enhanced Due Diligence"
        Response.Redirect("~/Compliance.aspx")
    End Sub
    Protected Sub ButtonComplianceOfficerApproval_Click(sender As Object, e As EventArgs) Handles ButtonComplianceOfficerApproval.Click
        Session("ComplianceView") = "Compliance Officer Approval"
        Response.Redirect("~/Compliance.aspx")
    End Sub
    Protected Sub ButtonCustomerResponsePending_Click(sender As Object, e As EventArgs) Handles ButtonCustomerResponsePending.Click
        Session("ComplianceView") = "Customer Response Pending"
        Response.Redirect("~/Compliance.aspx")
    End Sub
    Protected Sub ButtonKYCApprovalPending_Click(sender As Object, e As EventArgs) Handles ButtonKYCApprovalPending.Click

        Session("ComplianceView") = "KYC Approval Pending"
        Response.Redirect("~/Compliance.aspx")
    End Sub

    'Private Sub Compliance_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
    '    If ButtonEnhancedDueDiligence.Text = Session("ComplianceView") Then ButtonEnhancedDueDiligence.Font.Bold = True
    '    If ButtonComplianceOfficerApproval.Text = Session("ComplianceView") Then ButtonComplianceOfficerApproval.Font.Bold = True
    '    If ButtonCustomerResponsePending.Text = Session("ComplianceView") Then ButtonCustomerResponsePending.Font.Bold = True
    '    If ButtonKYCApprovalPending.Text = Session("ComplianceView") Then ButtonKYCApprovalPending.Font.Bold = True
    'End Sub

    'Protected Sub ButtonUpdateMinersFee_Click(sender As Object, e As EventArgs) Handles ButtonUpdateMinersFee.Click
    '    Dim OrderId As String
    '    For Each item As GridViewRow In GridViewOrder.Rows
    '        If TryCast(item.Cells(0).FindControl("cbSelect"), CheckBox).Checked Then
    '            OrderId = item.Cells(4).Text
    '            eQuote.ExecuteNonQuery("UPDATE [Order] SET MinersFee = " + TextBoxMinersFee.Text.Replace(",", ".") + " WHERE  Number = N'" + OrderId.ToString + "'")
    '            'eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = 0 WHERE Number = N'" + OrderId.ToString + "'")
    '        End If
    '    Next
    '    GridViewOrder.DataBind()
    'End Sub
    Protected Sub ButtonDeleteKyc_Click(sender As Object, e As EventArgs) Handles ButtonDeleteKyc.Click
        Dim KycFileId As String
        For Each item As GridViewRow In GridViewKycFile.Rows
            If TryCast(item.Cells(0).FindControl("cbSelectKyc"), CheckBox).Checked Then
                KycFileId = item.Cells(1).Text
                eQuote.ExecuteNonQuery("DELETE FROM [OrderKycfile] WHERE [KycfileId] = N'" + KycFileId.ToString + "'")
                eQuote.ExecuteNonQuery("DELETE FROM [KycFile] WHERE [Id] = N'" + KycFileId.ToString + "'")
            End If
        Next
        GridViewKycFile.DataBind()
    End Sub
    Protected Sub BtnUpdateOrderRates_Click(sender As Object, e As EventArgs) Handles BtnUpdateOrderRates.Click
        Dim orderId As Integer = GridViewOrder.SelectedValue
        Dim strQuery As String = "SELECT [Order].Amount, [Order].Rate, [Order].RateBase, [Order].RateHome, [Order].RateBooks, [Order].Number FROM [order] WHERE [Order].Id =" + orderId.ToString()
        Dim _amount As Decimal
        Dim _rate As Decimal
        Dim _ratebase As Decimal
        Dim _ratehome As Decimal
        Dim _ratebooks As Decimal
        Dim _ordernumber As Integer

        Dim dr As SqlDataReader = eQuote.GetDataReader(strQuery)
        If dr.HasRows Then
            If dr.Read() Then
                _amount = dr!Amount
                _rate = dr!Rate
                _ratebase = dr!RateBase
                _ratehome = dr!RateHome
                _ratebooks = dr!RateBooks
                _ordernumber = dr!Number
            End If
        End If
        dr.Close()
        Dim o_amount = TxtOrderAmount.Text
        Dim o_rate = TxtOrderRate.Text
        Dim o_ratebase = TxtOrderRateBase.Text
        Dim o_ratehome = TxtOrderRateHome.Text
        Dim o_ratebooks = TxtOrderRateBooks.Text

        strQuery = "UPDATE [Order] SET [Order].Amount=@Amount, [Order].Rate=@Rate , [Order].RateBase=@RateBase, " &
                                 "[Order].RateHome=@RateHome, [Order].RateBooks=@RateBooks WHERE [Order].Id =@OrderId"
        Dim cmd As New SqlCommand(strQuery)
        cmd.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId
        cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = o_amount
        cmd.Parameters.Add("@Rate", SqlDbType.Decimal).Value = o_rate
        cmd.Parameters.Add("@RateBase", SqlDbType.Decimal).Value = o_ratebase
        cmd.Parameters.Add("@RateHome", SqlDbType.Decimal).Value = o_ratehome
        cmd.Parameters.Add("@RateBooks", SqlDbType.Decimal).Value = o_ratebooks
        eQuote.ExecuteNonQuery(cmd)
        'Adding audit trail
        strQuery = "INSERT INTO AuditTrail(OrderId, Status, Message, Created) VALUES (@OrderId,@Status,@Message,GETUTCDATE())"
        cmd = New SqlCommand(strQuery)
        cmd.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId
        cmd.Parameters.Add("@Status", SqlDbType.Int).Value = 13 'Admin status is 13
        cmd.Parameters.Add("@Message", SqlDbType.Text).Value = String.Format("Payout values updated for Number: {10}, Amount:{0},Rate: {1}, RateBase: {2}, RateBooks: {3}, RateHome : {4}, Old Amount:{5}, Old Rate: {6},Old RateBase: {7}, Old RateBooks: {8}, Old RateHome : {9}", o_amount, o_rate, o_ratebase,
                                                                             o_ratebooks, o_ratehome, _amount, _rate, _ratebase,
                                                                             _ratebooks, _ratehome, _ordernumber)
        eQuote.ExecuteNonQuery(cmd)
        GridViewSellDetails.DataBind()
        GridViewOrder.DataBind()
        PayoutAmountUpdate(orderId)
    End Sub

    Protected Sub ButtonSellApprove_Click(sender As Object, e As EventArgs) Handles ButtonSellApprove.Click
        Dim OrderId As Integer = GridViewOrder.SelectedValue
        Using dr As SqlDataReader = eQuote.GetDataReader("SELECT [Order].TransactionHash, [Order].Id, [Order].CurrencyId, [Order].ExtRef, [User].Fname, [Order].Number, [Order].Amount, Currency.Code, [Order].CardNumber, [Order].CommissionProcent,[Order].PaymentType,[Order].OurFee, [Order].Rate, [Order].CryptoAddress, [Order].BTCAmount, [Order].MinersFee,[Order].FixedFee,[Order].AccountNumber,[Order].IBAN,[Order].CurrencyCode, [User].Email, Site.Text FROM [Order] INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN Currency ON [Order].CurrencyId = Currency.Id INNER JOIN Site ON [Order].SiteId = Site.Id WHERE  [Order].Id = " + OrderId.ToString)
            Dim fromAccount = eQuote.ExecuteScalar("SELECT [Account].Id FROM [Account] WHERE [Account].ValueFor = N'FromAccount' AND [Account].TransactionType = 2 AND [Account].Currency = " + DropDownListSellAmountCurrency.SelectedItem.Value)
            Try
                Dim dateNow = Now()
                If dr.HasRows Then
                    While dr.Read()
                        Dim strQuery As String = "INSERT INTO [Transaction] (OrderId, MethodId, Type, ExtRef, Amount, Currency, Completed, FromAccount, ToAccount) VALUES (@OrderId, 1, 2, @ExtRef, @Amount, @Currency, @CurrentDateTime, @FromAccount, 26)"
                        Dim cmd As New SqlCommand(strQuery)
                        cmd.Parameters.Add("@OrderId", SqlDbType.Int).Value = dr!Id
                        cmd.Parameters.Add("@ExtRef", SqlDbType.VarChar).Value = dr!ExtRef
                        cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = TextBoxSellAmount.Text   'dr!Amount
                        cmd.Parameters.Add("@Currency", SqlDbType.Int).Value = DropDownListSellAmountCurrency.SelectedItem.Value   'dr!CurrencyId
                        cmd.Parameters.Add("@FromAccount", SqlDbType.BigInt).Value = fromAccount
                        cmd.Parameters.Add("@CurrentDateTime", SqlDbType.DateTime).Value = dateNow
                        eQuote.ExecuteNonQuery(cmd)

                        Dim DataUnitOfWork = New WMC.Data.DataUnitOfWork(New WMC.Data.RepositoryProvider(New WMC.Data.RepositoryFactories()))
                        Dim cultureInfo = DataUnitOfWork.Countries.GetCultureCodeByCurrency(dr!Code)
                        Dim ci = New CultureInfo(cultureInfo)

                        Dim template = IIf(dr!PaymentType = 1, "SellOrderCompleted", "OrderSellCompleted")
                        Dim payoutAccNo = IIf(dr!CurrencyCode = "EUR", dr!IBAN, dr!AccountNumber)
                        WMC.Utilities.EmailHelper.SendEmail(dr!Email, template, New Dictionary(Of String, Object) From
                        {
                        {"UserFirstName", dr!Fname},
                        {"OrderNumber", dr!Number},
                        {"OrderAmount", CDec(dr!Amount).ToString("N2", ci) + dr!Code},
                        {"TransactionExtRef", dr!TransactionHash},
                        {"OrderCurrency", dr!Code},
                        {"CardNumber", dr!CardNumber},
                        {"OrderCommission", CDec(dr!Amount * (dr!CommissionProcent / 100)).ToString("N2", ci)},
                        {"OrderOurFee", CDec(dr!Amount * (dr!OurFee / 100)).ToString("N2", ci) + " " + dr!CurrencyCode},
                        {"OrderRate", CDec(dr!Rate).ToString("N2", ci)},
                        {"CryptoAddress", dr!CryptoAddress},
                        {"TxAmount", CDec(dr!BTCAmount).ToString("N8", ci)},
                        {"MinersFee", (CDec(dr!MinersFee).ToString("N8", ci)) + " BTC"},
                        {"PayoutAmount", CDec((dr!Amount) - (dr!Amount * (dr!OurFee / 100))).ToString("N2", ci) + " " + dr!CurrencyCode},
                        {"PayoutDestination", payoutAccNo},
                        {"FixedFee", CDec(dr!FixedFee).ToString("N2", ci)}
                    }, dr!Text)

                    End While
                End If
                Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Completed'")
                WMC.AddStatusChange(OrderId, "Received Crypto Payment", s, Session("UserId"))
                eQuote.ExecuteNonQuery("UPDATE [Order] SET Approved = { fn NOW() }, ApprovedBy = " + Session("UserId").ToString + ", Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
                'Updating the Bank Sell incoming tx with completed value as current date time'
                'eQuote.ExecuteNonQuery("UPDATE [Transaction] SET Completed = '" + dateNow.ToString("YYYY-MM-DDTHH:mm:ss") + "' WHERE OrderId = " + OrderId.ToString + " AND Type = 1 AND MethodId = 3")

                Using cmd As New SqlCommand("UPDATE [Transaction] SET Completed = @CurrentDateTime WHERE OrderId = @OrderId AND Type = @Type AND MethodId = @MethodId")
                    cmd.Parameters.Add("@CurrentDateTime", SqlDbType.DateTime).Value = dateNow
                    cmd.Parameters.Add("@OrderId", SqlDbType.Int).Value = OrderId
                    cmd.Parameters.Add("@Type", SqlDbType.Int).Value = 1 'Incoming Tx
                    cmd.Parameters.Add("@MethodId", SqlDbType.Int).Value = 3 'BTC payment
                    eQuote.ExecuteNonQuery(cmd)
                End Using
                GridViewOrder.DataBind()
                PanelSellDetails.Visible = False
            Finally
                dr.Close()
            End Try
        End Using
    End Sub

    Function CheckKYC(OrderId As Integer) As Boolean
        Dim type As Integer = eQuote.ExecuteScalar("SELECT PaymentType FROM [Order] WHERE Id = " + OrderId.ToString)
        Dim userId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM [Order] WHERE Id = " + OrderId.ToString)
        Dim DataUnitOfWork = New WMC.Data.DataUnitOfWork(New WMC.Data.RepositoryProvider(New WMC.Data.RepositoryFactories()))

        Dim type1Kyc As Boolean = DataUnitOfWork.KycFiles.CheckKycFile(userId, WMC.Data.Enums.KYCFileTypes.PhotoID)
        Dim type4Kyc As Boolean = DataUnitOfWork.KycFiles.CheckKycFile(userId, WMC.Data.Enums.KYCFileTypes.SelfieID)

        'For PaymentType CreditCard then one of the PhotoID and SelfieID is enough to be considered as approved KYC
        If type = WMC.Data.Enums.OrderPaymentType.Bank Then
            If type1Kyc Then
                Return True
            Else
                Return False
            End If

            'For PaymentType CreditCard then one of the PhotoID and SelfieID is enough to be considered as approved KYC
        ElseIf type = WMC.Data.Enums.OrderPaymentType.CreditCard Then
            If type1Kyc And type4Kyc Then
                Return True
            Else
                Return False
            End If
        End If
        Return False
    End Function

    Private Sub DropDownListSellAmountCurrency_DataBound(sender As Object, e As EventArgs) Handles DropDownListSellAmountCurrency.DataBound
        Dim OrderId As Integer = GridViewOrder.SelectedValue
        PayoutAmountUpdate(OrderId)
    End Sub
    Sub PayoutAmountUpdate(orderid)
        Using dr As SqlDataReader = eQuote.GetDataReader("SELECT [Order].Id, BTCAmount,[Order].Amount, FixedFee, [Order].Rate, [Order].RateBase, [Order].RateHome, [Order].RateBooks, CommissionProcent, OurFee, CurrencyId, CASE When[order].CouponId IS Null THEN 0.00 ELSE Discount END As Discount FROM [Order] LEFT JOIN [Coupon] on [Order].CouponId = [Coupon].Id  WHERE [Order].Id =" + orderid.ToString)
            Try
                If dr.HasRows Then
                    While dr.Read()
                        Dim o_amount = dr!Amount
                        Dim o_rate = dr!Rate
                        Dim o_ratebase = dr!RateBase
                        Dim o_ratehome = dr!RateHome
                        Dim o_ratebooks = dr!RateBooks
                        TxtOrderAmount.Text = o_amount
                        TxtOrderRate.Text = o_rate
                        TxtOrderRateBase.Text = o_ratebase
                        TxtOrderRateHome.Text = o_ratehome
                        TxtOrderRateBooks.Text = o_ratebooks

                        Dim payoutAmount = ((dr!Amount - (dr!Amount - dr!FixedFee * dr!Rate) / 100 * dr!CommissionProcent) - dr!Amount / 100 * dr!OurFee * (1 - dr!Discount / 100) - dr!FixedFee * dr!Rate).ToString()
                        TextBoxSellAmount.Text = payoutAmount
                        Dim itemTobeSelected = DropDownListSellAmountCurrency.Items.FindByValue(dr!CurrencyId)
                        If Not itemTobeSelected Is Nothing Then
                            Dim selindex = DropDownListSellAmountCurrency.Items.IndexOf(itemTobeSelected)
                            DropDownListSellAmountCurrency.SelectedIndex = selindex
                        End If
                    End While

                End If
            Finally
                dr.Close()
            End Try
        End Using
    End Sub
End Class
