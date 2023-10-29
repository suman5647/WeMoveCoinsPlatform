Imports System.Linq
Imports System.Data.SqlClient
Imports WMC.Logic
Imports WMC.Logic.Models
Imports System.IO
Imports System.Data

<WMC.PageAuth(False)>
Partial Class _Default
    Inherits System.Web.UI.Page

    Protected Function GetWalletBalance(Optional ByVal currencycode As String = "BTC") As Double
        Dim cryptoCurrency = New WMC.Data.DataUnitOfWork(New WMC.Data.RepositoryProvider(New WMC.Data.RepositoryFactories())).Currencies.Get(Function(x) x.Code = currencycode).FirstOrDefault()

        Dim settingsManager As ISettingsManager = WMC.Logic.SettingsManager.GetDefault()
        Dim bitgo_AccessCode As SettingsValue = settingsManager.Get("BitGoAccessCode", True)
        Dim bitGoAccessSettings As BitGoAccessSettings = bitgo_AccessCode.GetJsonData(Of BitGoAccessSettings)()
        Dim cryptoCurrencyBitGoSettings = Newtonsoft.Json.JsonConvert.DeserializeObject(Of BitGoCurrencySettings)(cryptoCurrency.BitgoSettings)

        Dim bitgoClient = New BitGoAccess(bitGoAccessSettings, cryptoCurrency.Code)
        Dim wallet = bitgoClient.GetWallet(cryptoCurrencyBitGoSettings.DefaultWalletId)
        Dim balance = wallet.balance / 100000000
        Return balance
    End Function

    Function Login(userid As Long, password As String) As Boolean
        Dim crds_matched As Boolean

        Using drUser As SqlDataReader = eQuote.GetDataReader("SELECT [User].Password, [User].PasswordSalt, UserRole.Text AS UserRole, [User].Id AS UserId,  [User].Id, [User].Fname, [User].UserType FROM UserRole INNER JOIN [User] ON UserRole.Id = [User].RoleId WHERE [User].Id = " + CStr(userid))
            Try
                drUser.Read()
                crds_matched = WMC.Logic.SecurityUtil.ValidatePassword(password, drUser!PasswordSalt, drUser!Password)
            Finally
                drUser.Close()
            End Try
        End Using
        Return crds_matched
    End Function

    Function ChangePassword(userid As Long, password As String, newPassword As String) As Boolean
        Dim crds_matched As Boolean

        Using drUser As SqlDataReader = eQuote.GetDataReader("SELECT [User].Password, [User].PasswordSalt, UserRole.Text AS UserRole, [User].Id AS UserId,  [User].Id, [User].Fname, [User].UserType FROM UserRole INNER JOIN [User] ON UserRole.Id = [User].RoleId WHERE [User].Id = " + CStr(userid))
            Try
                drUser.Read()
                crds_matched = WMC.Logic.SecurityUtil.ValidatePassword(password, drUser!PasswordSalt, drUser!Password)
                If crds_matched Then
                    Dim salt As String = ""
                    Dim passwordHash As String = SecurityUtil.EncryptPassword(newPassword, salt)
                    eQuote.ExecuteScalar("UPDATE [User] set Password = '" + passwordHash + "', PasswordSalt ='" & salt & "' WHERE Id=" + CStr(userid))

                End If
            Catch
                crds_matched = False '' update filed
            Finally
                drUser.Close()
            End Try
        End Using
        Return crds_matched
    End Function

    Protected Sub ButtonLogin_Click(sender As Object, e As EventArgs) Handles ButtonLogin.Click
        If Not Login(DropDownListUser.SelectedValue, TextBoxPassword.Text) Then
            '' password not matched
            LabelWrongPasswordText.Visible = True
            Return
        End If

        Dim w As String = ""
        Dim w2 As String = ""
        'Dim s As New StringBuilder
        ''s.Append("SELECT UserRole.Text AS UserRole, [User].Id AS UserId,  [User].Id, [User].Fname, [User].UserType FROM UserRole INNER JOIN [User] ON UserRole.Id = [User].RoleId WHERE [User].Id = " + DropDownListUser.SelectedValue + " AND Password = @password ")
        '' c.Parameters.AddWithValue("password", TextBoxPassword.Text)
        's.Append()
        'Dim c As New SqlCommand(s.ToString, eQuote.OpenConnection)
        Using drUser As SqlDataReader = eQuote.GetDataReader("SELECT UserRole.Text AS UserRole, [User].Id AS UserId,  [User].Id, [User].Fname, [User].UserType FROM UserRole INNER JOIN [User] ON UserRole.Id = [User].RoleId WHERE [User].Id = " + DropDownListUser.SelectedValue)
            ButtonLockBitGo.Attributes.Add("onClick", "javascript: return confirm('THIS IS FOR ONLY CRITICAL SITUATIONS. ARE YOU SURE YOU WANT TO LOCK?')")
            Try

                If CInt(DropDownListUser.SelectedValue) > 0 AndAlso drUser.Read() Then
                    PanelAccess.Visible = True
                    PanelLogin.Visible = False
                    LabelWrongPasswordText.Visible = False

                    Session("UserId") = drUser!UserId
                    Session("UserName") = drUser!Fname
                    Session("UserRole") = drUser!UserRole
                    Session("UserType") = drUser!UserType
                    Session("Status") = eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'YourPayTestOrProd'")
                    'Session("OperationalStatus") = eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'OperationalStatus'")
                    Session("ComplianceAvailability") = eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'ComplianceAvailability'")
                    Session("CreditCardGatewayName") = eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'CreditCardGatewayName'")

                    Dim settingManager = WMC.Logic.CurrencySettingsManager.GetDefault()
                    Dim settingsData As SettingsValue = settingManager.Get("BTC")


                    Dim bitgo_MinersFeeSettings As WMC.Logic.Models.Settings.BitGoCurrencySettings = settingsData.GetJsonData(Of WMC.Logic.Models.Settings.BitGoCurrencySettings)("bitgo")
                    'NEW MINERS FEE JSON
                    '' Dim bitGoMinersFeeSettingsJSON As String = eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'BitGoMinersFeeSettings'")
                    '' SHI Commented '' Dim bitGoMinersFeeSettings As WMC.Logic.BitGoMinersFeeSettings = Newtonsoft.Json.JsonConvert.DeserializeObject(Of WMC.Logic.BitGoMinersFeeSettings)(bitGoMinersFeeSettingsJSON)
                    Session("MinersFee") = bitgo_MinersFeeSettings.MinersFee.Fee.ToString.Replace(".", ",")  '' "0,0037" '' SHI Commented '' " '' SHI Commented '' bitGoMinersFeeSettings.MinersFee.ToString.Replace(".", ",")
                    DropDownListMinersFee.DataValueField = Session("MinersFee")
                    DropDownListMinersFee.DataTextField = Session("MinersFee")
                    If (DropDownListMinersFee.Items.Cast(Of ListItem).Any(Function(li As ListItem)
                                                                              Return li.Value = Session("MinersFee")
                                                                          End Function)) Then
                        DropDownListMinersFee.SelectedValue = Session("MinersFee")
                    End If

                    'Session("MinersFee") = eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'MinersFee'")
                    'DropDownListMinersFee.SelectedValue = Session("MinersFee").ToString.Replace(".", ",")

                    'DropDownListOperationalStatus.SelectedValue = Session("OperationalStatus")
                    DropDownListOperationalStatus.SelectedValue = eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'OperationalStatus'")
                    DropDownListComplianceAvailability.SelectedValue = Session("ComplianceAvailability")
                    If Session("ComplianceAvailability") = "Away" Then DropDownListComplianceAvailability.BackColor = Drawing.Color.LightPink
                    DropDownListCreditCardGatewayName.SelectedValue = Session("CreditCardGatewayName")

                    Dim nUsers As Integer = eQuote.ExecuteScalar("SELECT COUNT(DISTINCT [Order].Id) FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId  INNER JOIN OrderStatus ON (OrderStatus.Id = [Order].Status)  INNER JOIN OrderType ON ([Order].Type = 1 OR [Order].Type=2) INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN Country ON [User].CountryId = Country.Id INNER JOIN UserRiskLevelType ON (UserRiskLevelType.Id = [User].UserRiskLevel) WHERE ([User].Tier = 3 AND [Order].Status = 3) OR  ([Order].Status = 26) OR ([User].UserRiskLevel = 2 AND [Order].Status = 7 ) ")

                    Dim nUsersCount As Integer = eQuote.ExecuteScalar("SELECT COUNT(DISTINCT [Order].Id) FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN Country ON [User].CountryId = Country.Id WHERE (OrderStatus.Text = 'Compliance Officer Approval') OR ([User].UserRiskLevel = 1 AND[Order].Status = 7) ")

                    ButtonEnhancedDueDiligence.Text = "Enhanced Due Diligence (" + nUsers.ToString + ")"
                    If nUsers > 0 Then ButtonEnhancedDueDiligence.BackColor = Drawing.Color.FromName("#66FF66")

                    ButtonComplianceOfficerApproval.Text = "Compliance Officer Approval (" + nUsersCount.ToString + ")"
                    If WMC.StatusRecordCount("Compliance Officer Approval") > 0 Then ButtonComplianceOfficerApproval.BackColor = Drawing.Color.FromName("#66FF66")

                    ButtonCustomerResponsePending.Text = "Customer Response Pending (" + WMC.StatusRecordCount("Customer Response Pending").ToString + ")"

                    ButtonKYCApprovalPending.Text = "KYC Approval Pending (" + WMC.StatusRecordCount("KYC Approval Pending").ToString + ")"
                    If WMC.StatusRecordCount("KYC Approval Pending") > 0 Then ButtonKYCApprovalPending.BackColor = Drawing.Color.FromName("#66FF66")

                    ButtonApproveSell2Bank.Text = "Received Crypto Payment (" + WMC.StatusRecordCount("Received Crypto Payment").ToString + ")"
                    If WMC.StatusRecordCount("Received Crypto Payment") > 0 Then ButtonApproveSell2Bank.BackColor = Drawing.Color.FromName("#66FF66")

                    Try
                        Dim val As Double = GetWalletBalance() '' SHI Commented ''  WMC.Logic.BitGoUtil.GetBalance() / 100000000
                        LabelBitGoBalance.Text = String.Format("{0:0.00000000} BTC", (val))
                        If val > 0.5 And val < 1 Then LabelBitGoBalance.BackColor = Drawing.Color.Yellow
                        If val <= 0.5 Then LabelBitGoBalance.BackColor = Drawing.Color.Red
                    Catch ex As Exception
                        LabelBitGoBalance.Text = "?"
                    End Try
                    If WMC.StatusRecordCount("Payment Aborted") > 0 Then
                        LabelPaymentAborted19.Text = "Payment Aborted (" + WMC.StatusRecordCount("Payment Aborted").ToString + ")" : LabelPaymentAborted19.Visible = True
                    Else
                        LabelPaymentAborted19.Visible = False
                    End If
                    If WMC.StatusRecordCount("Sending Aborted") > 0 Then
                        LabelSendingAborted12.Text = "Sending Aborted (" + WMC.StatusRecordCount("Sending Aborted").ToString + ")" : LabelSendingAborted12.Visible = True
                    Else
                        LabelSendingAborted12.Visible = False
                    End If
                    If WMC.StatusRecordCount("Capture Errored") > 0 Then
                        LabelCaptureErrored20.Text = "Capture Errored (" + WMC.StatusRecordCount("Capture Errored").ToString + ")" : LabelCaptureErrored20.Visible = True
                    Else
                        LabelCaptureErrored20.Visible = False
                    End If
                    If WMC.StatusRecordCount("Releasing payment Aborted") > 0 Then
                        LabelReleasingPaymentAborted15.Text = "Releasing payment Aborted (" + WMC.StatusRecordCount("Releasing payment Aborted").ToString + ")" : LabelReleasingPaymentAborted15.Visible = True
                    Else
                        LabelReleasingPaymentAborted15.Visible = False
                    End If
                    If WMC.StatusRecordCount("Paid") > 0 Then
                        LabelPaid3.Text = "Paid (" + WMC.StatusRecordCount("Paid").ToString + ")" : LabelPaid3.Visible = True
                    Else
                        LabelPaid3.Visible = False
                    End If
                    If WMC.StatusRecordCount("Released payment") > 0 Then
                        LabelReleasedPayment16.Text = "Released payment (" + WMC.StatusRecordCount("Released payment").ToString + ")" : LabelReleasedPayment16.Visible = True
                    Else
                        LabelReleasedPayment16.Visible = False
                    End If
                    If WMC.StatusRecordCount("Payout awaits approval") > 0 Then
                        LabelPayoutAwaitsApproval11.Text = "Payout awaits approval (" + WMC.StatusRecordCount("Payout awaits approval").ToString + ")" : LabelPayoutAwaitsApproval11.Visible = True
                    Else
                        LabelPayoutAwaitsApproval11.Visible = False
                    End If
                    'Dim nDoublePayoutsTotal As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM (SELECT COUNT(*) AS Antal FROM [Transaction] GROUP BY OrderId, Type, FromAccount HAVING  (Type = 2) AND (FromAccount = 3) AND(COUNT(*) > 1)) AS derivedtbl_1")
                    'If nDoublePayoutsTotal > 0 Then LabelDoublePayouts.Visible = True : LabelDoublePayouts.Text = LabelDoublePayouts.Text + " (" + nDoublePayoutsTotal.ToString + ")"
                    Dim nDoublePaymentsTotal As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM (SELECT COUNT(*) AS Antal FROM [Transaction] GROUP BY OrderId, Type HAVING  (Type = 1) AND (COUNT(*) > 1)) AS derivedtbl_1")
                    If nDoublePaymentsTotal > 0 Then
                        Dim t As String = ""
                        Dim dr As SqlDataReader = eQuote.GetDataReader("SELECT OrderId FROM [Transaction] GROUP BY OrderId, Type HAVING (Type = 1) AND (COUNT(*) > 1)")
                        If dr.HasRows Then
                            While dr.Read()
                                t += dr!OrderId.ToString + ", "
                            End While
                        End If
                        dr.Close()
                        LabelDoublePayments.Visible = True : LabelDoublePayments.Text = LabelDoublePayments.Text + " (" + nDoublePaymentsTotal.ToString + ") " + t
                    End If
                    Dim nBank As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM  OrderStatus INNER JOIN [Order] ON OrderStatus.Id = [Order].Status INNER JOIN PaymentType ON [Order].PaymentType = PaymentType.Id WHERE  (OrderStatus.Text = N'Quoted' AND [order].Type=1 ) AND (PaymentType.Text = N'Bank')")
                    ButtonApproveBankPayment.Text = "Approve Bank Payment (" + nBank.ToString + ")"
                    Dim nExceptions As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM AuditTrail WHERE (AuditTrailLevelId > 4)").ToString
                    LinkButtonExceptions.Text = nExceptions.ToString
                    If nExceptions > 0 Then LinkButtonExceptions.ForeColor = Drawing.Color.Red Else LinkButtonExceptions.ForeColor = Drawing.Color.Green

                    If Session("UserRole") = "SiteMgr" Then
                        PanelSiteMgrTools.Visible = True
                        PanelAdminTools.Visible = False
                        DropDownListComplianceAvailability.Visible = False
                        Session("SiteId") = eQuote.ExecuteScalar("SELECT UserType.Text FROM [User] INNER JOIN UserType ON [User].UserType = UserType.Id WHERE [User].Id = " + Session("UserId").ToString)
                        w = " WHERE SiteId = " + Session("SiteId")
                        w2 = " AND SiteId = " + Session("SiteId")
                        DropDownListOperationalStatus.Enabled = True
                        LinkButtonUserKYC.Enabled = False

                        Dim AffiliateMarketeerCommission As Double = 0.01104

                        LabelYesterdaysRev2.Text = WMC.GetYesterdaysRevenue("EUR", 2)
                        LabelTodaysRev2.Text = WMC.GetTodaysRevenue("EUR", 2)
                        LabelTotalRev2.Text = WMC.GetTotalRevenue("EUR", 2)
                        LabelTotalCommision2.Text = WMC.GetTotalCommission("EUR", 2, AffiliateMarketeerCommission)

                        LabelYesterdaysRev15.Text = WMC.GetYesterdaysRevenue("EUR", 15)
                        LabelTodaysRev15.Text = WMC.GetTodaysRevenue("EUR", 15)
                        LabelTotalRev15.Text = WMC.GetTotalRevenue("EUR", 15)
                        LabelTotalCommision15.Text = WMC.GetTotalCommission("EUR", 15, AffiliateMarketeerCommission)

                        LabelYesterdaysRev16.Text = WMC.GetYesterdaysRevenue("EUR", 16)
                        LabelTodaysRev16.Text = WMC.GetTodaysRevenue("EUR", 16)
                        LabelTotalRev16.Text = WMC.GetTotalRevenue("EUR", 16)
                        LabelTotalCommision16.Text = WMC.GetTotalCommission("EUR", 16, AffiliateMarketeerCommission)

                        LabelYesterdaysRev19.Text = WMC.GetYesterdaysRevenue("EUR", 19)
                        LabelTodaysRev19.Text = WMC.GetTodaysRevenue("EUR", 19)
                        LabelTotalRev19.Text = WMC.GetTotalRevenue("EUR", 19)

                    Else
                        If Session("UserType") = 1 Then
                            PanelBackEndStuff.Visible = True
                            ButtonApproveBankPayment.Visible = True
                            ButtonApproveSell2Bank.Visible = True
                            ButtonEnhancedDueDiligence.Enabled = True
                        End If
                        PanelSiteMgrTools.Visible = False
                        PanelAdminTools.Visible = True
                        Dim nKYC As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM KycFile INNER JOIN [User] ON KycFile.UserId = [User].Id WHERE (KycFile.Obsolete IS NULL) AND (KycFile.Approved IS NULL) AND (NOT (KycFile.UniqueFilename = N'EnforceKYC')) AND (KycFile.Rejected IS NULL)")
                        LinkButtonUserKYC.Text = "KYC(" + nKYC.ToString + ")"
                        If nKYC > 0 Then LinkButtonUserKYC.ForeColor = Drawing.Color.Red Else LinkButtonUserKYC.ForeColor = Drawing.Color.Green

                        Dim nEDD As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [User] WHERE [User].Tier=3")
                        LinkButtonEDDUser.Text = "EDD(" + nEDD.ToString + ")"
                        If nEDD > 0 Then LinkButtonEDDUser.ForeColor = Drawing.Color.Red Else LinkButtonUserKYC.ForeColor = Drawing.Color.Green


                        Dim quickPayAccpeted As Integer = eQuote.ExecuteScalar("SELECT COUNT(DISTINCT OrderId) FROM [AuditTrail] WHERE AuditTrailLevelId = 6 AND Created >= DATEADD(day, -1, GETDATE())")
                        LinkButtonAccepted.Text = "Accepted(" + quickPayAccpeted.ToString + ")"
                        LinkButtonAccepted.ForeColor = Drawing.Color.Green

                        Dim quickPayRejected As Integer = eQuote.ExecuteScalar("SELECT COUNT(DISTINCT OrderId) FROM [AuditTrail] WHERE AuditTrailLevelId = 7 AND Created >= DATEADD(day, -1, GETDATE())")
                        LinkButtonRejected.Text = "Rejected(" + quickPayRejected.ToString + ")"
                        LinkButtonRejected.ForeColor = Drawing.Color.Orange

                        Dim quickPayTerminated As Integer = eQuote.ExecuteScalar("SELECT COUNT(DISTINCT OrderId) FROM [AuditTrail] WHERE AuditTrailLevelId = 8 AND Created >= DATEADD(day, -1, GETDATE())")
                        LinkButtonTerminated.Text = "Terminted(" + quickPayTerminated.ToString + ")"
                        LinkButtonTerminated.ForeColor = Drawing.Color.Red

                        LabelRevenueYesterday.Text = WMC.GetYesterdaysRevenue("DKK")
                        LabelRevenueThisMonth.Text = WMC.GetThisMonthRevenue("DKK")
                        LabelRevenueToday.Text = WMC.GetTodaysRevenue("DKK")


                        Dim nOrderTotal As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order]" + w)
                        LabelOrderTotal.Text = nOrderTotal.ToString

                        Dim nKycDeclined As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id WHERE  (OrderStatus.Text = N'KYC Declined')" + w2)
                        'LinkButtonKycDeclined.Text = nKycDeclined.ToString

                        Dim nCompleted As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id WHERE  (OrderStatus.Text = N'Completed')" + w2)
                        LabelCompleted.Text = nCompleted.ToString

                        Dim nCustomers As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [User]")
                        LinkButtonCustomers.Text = nCustomers.ToString
                    End If
                    LabelMe.Text = Session("UserName")

                End If
                LabelWrongPasswordText.Visible = True
                If Session("UserRole") = "Admin" Then LabelRole.Text = "Administrator" Else LabelRole.Text = "SiteManager"

            Finally
                drUser.Close()

            End Try
        End Using
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If IsNumeric(Session("UserId")) Then
            PanelAccess.Visible = True
            PanelLogin.Visible = False
            LabelMe.Text = Session("UserName")
            If Session("UserRole") = "Admin" Then LabelRole.Text = "Administrator" Else LabelRole.Text = "SiteManager"
            ButtonLockBitGo.Attributes.Add("onClick", "javascript: return confirm('THIS IS FOR ONLY CRITICAL SITUATIONS. ARE YOU SURE YOU WANT TO LOCK?')")
            If Session("UserRole") = "SiteMgr" Then

                PanelSiteMgrTools.Visible = True
                PanelAdminTools.Visible = False
                DropDownListComplianceAvailability.Visible = False
                Session("SiteId") = eQuote.ExecuteScalar("SELECT UserType.Text FROM [User] INNER JOIN UserType ON [User].UserType = UserType.Id WHERE [User].Id = " + Session("UserId").ToString)

                Dim AffiliateMarketeerCommission As Double = 0.01104

                LabelYesterdaysRev2.Text = WMC.GetYesterdaysRevenue("EUR", 2)
                LabelTodaysRev2.Text = WMC.GetTodaysRevenue("EUR", 2)
                LabelTotalRev2.Text = WMC.GetTotalRevenue("EUR", 2)
                LabelTotalCommision2.Text = WMC.GetTotalCommission("EUR", 2, AffiliateMarketeerCommission)

                LabelYesterdaysRev15.Text = WMC.GetYesterdaysRevenue("EUR", 15)
                LabelTodaysRev15.Text = WMC.GetTodaysRevenue("EUR", 15)
                LabelTotalRev15.Text = WMC.GetTotalRevenue("EUR", 15)
                LabelTotalCommision15.Text = WMC.GetTotalCommission("EUR", 15, AffiliateMarketeerCommission)

                LabelYesterdaysRev16.Text = WMC.GetYesterdaysRevenue("EUR", 16)
                LabelTodaysRev16.Text = WMC.GetTodaysRevenue("EUR", 16)
                LabelTotalRev16.Text = WMC.GetTotalRevenue("EUR", 16)
                LabelTotalCommision16.Text = WMC.GetTotalCommission("EUR", 16, AffiliateMarketeerCommission)

                LabelYesterdaysRev19.Text = WMC.GetYesterdaysRevenue("EUR", 19)
                LabelTodaysRev19.Text = WMC.GetTodaysRevenue("EUR", 19)
                LabelTotalRev19.Text = WMC.GetTotalRevenue("EUR", 19)
            ElseIf Session("UserRole") = "Admin" Then
                If Session("UserType") = 1 Then
                    PanelBackEndStuff.Visible = True
                    ButtonApproveBankPayment.Visible = True
                    ButtonApproveSell2Bank.Visible = True
                    ButtonEnhancedDueDiligence.Enabled = True
                End If
                PanelSiteMgrTools.Visible = False
                PanelAdminTools.Visible = True
                Dim nKYC As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM KycFile INNER JOIN [User] ON KycFile.UserId = [User].Id WHERE (KycFile.Obsolete IS NULL) AND (KycFile.Approved IS NULL) AND (NOT (KycFile.UniqueFilename = N'EnforceKYC')) AND (KycFile.Rejected IS NULL)") 'SELECT COUNT(DISTINCT KycFile.UserId)FROM KycFile INNER JOIN [User] ON KycFile.UserId = [User].Id WHERE  (KycFile.Obsolete IS NULL) AND (KycFile.Approved IS NULL)
                LinkButtonUserKYC.Text = "KYC(" + nKYC.ToString + ")"
                If nKYC > 0 Then LinkButtonUserKYC.ForeColor = Drawing.Color.Red Else LinkButtonUserKYC.ForeColor = Drawing.Color.Green

                Dim nEDD As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [User] WHERE [User].Tier=3")
                LinkButtonEDDUser.Text = "EDD(" + nEDD.ToString + ")"
                If nEDD > 0 Then LinkButtonEDDUser.ForeColor = Drawing.Color.Red Else LinkButtonUserKYC.ForeColor = Drawing.Color.Green

                'Get last 24 hours quickpay logs
                Dim quickPayAccpeted As Integer = eQuote.ExecuteScalar("SELECT COUNT(DISTINCT OrderId) FROM [AuditTrail] WHERE AuditTrailLevelId = 6 AND Created >= DATEADD(day, -1, GETDATE())")
                LinkButtonAccepted.Text = "Accepted(" + quickPayAccpeted.ToString + ")"
                LinkButtonAccepted.ForeColor = Drawing.Color.Green

                Dim quickPayRejected As Integer = eQuote.ExecuteScalar("SELECT COUNT(DISTINCT OrderId) FROM [AuditTrail] WHERE AuditTrailLevelId = 7 AND Created >= DATEADD(day, -1, GETDATE())")
                LinkButtonRejected.Text = "Rejected(" + quickPayRejected.ToString + ")"
                LinkButtonRejected.ForeColor = Drawing.Color.Orange

                Dim quickPayTerminated As Integer = eQuote.ExecuteScalar("SELECT COUNT(DISTINCT OrderId) FROM [AuditTrail] WHERE AuditTrailLevelId = 8 AND Created >= DATEADD(day, -1, GETDATE())")
                LinkButtonTerminated.Text = "Terminted(" + quickPayTerminated.ToString + ")"
                LinkButtonTerminated.ForeColor = Drawing.Color.Red

                If Not IsPostBack Then

                    'Session("OperationalStatus") = eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'OperationalStatus'")
                    'DropDownListOperationalStatus.SelectedValue = Session("OperationalStatus")
                    DropDownListOperationalStatus.SelectedValue = eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'OperationalStatus'")

                    Session("ComplianceAvailability") = eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'ComplianceAvailability'")
                    DropDownListComplianceAvailability.SelectedValue = Session("ComplianceAvailability")
                    If Session("ComplianceAvailability") = "Away" Then DropDownListComplianceAvailability.BackColor = Drawing.Color.LightPink
                    Session("CreditCardGatewayName") = eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'CreditCardGatewayName'")
                    DropDownListCreditCardGatewayName.SelectedValue = Session("CreditCardGatewayName")

                    'NEW MINERS FEE JSON
                    Dim settingManager = WMC.Logic.CurrencySettingsManager.GetDefault()
                    Dim settingsData As SettingsValue = settingManager.Get("BTC")


                    Dim bitgo_MinersFeeSettings As WMC.Logic.Models.Settings.BitGoCurrencySettings = settingsData.GetJsonData(Of WMC.Logic.Models.Settings.BitGoCurrencySettings)("bitgo")
                    'NEW MINERS FEE JSON
                    '' Dim bitGoMinersFeeSettingsJSON As String = eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'BitGoMinersFeeSettings'")
                    '' SHI Commented '' Dim bitGoMinersFeeSettings As WMC.Logic.BitGoMinersFeeSettings = Newtonsoft.Json.JsonConvert.DeserializeObject(Of WMC.Logic.BitGoMinersFeeSettings)(bitGoMinersFeeSettingsJSON)
                    Session("MinersFee") = bitgo_MinersFeeSettings.MinersFee.Fee.ToString.Replace(".", ",")  '' "0,0037" '' SHI Commented '' " '' SHI Commented '' bitGoMinersFeeSettings.MinersFee.ToString.Replace(".", ",")
                    'DropDownListMinersFee.DataValueField = Session("MinersFee")
                    'DropDownListMinersFee.DataTextField = Session("MinersFee")
                    If (DropDownListMinersFee.Items.Cast(Of ListItem).Any(Function(li As ListItem)
                                                                              Return li.Value = Session("MinersFee")
                                                                          End Function)) Then
                        DropDownListMinersFee.SelectedValue = Session("MinersFee")
                    End If


                    'Session("MinersFee") = eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'MinersFee'")
                    'DropDownListMinersFee.SelectedValue = Session("MinersFee").ToString.Replace(".", ",")
                    Try
                        Dim val As Double = GetWalletBalance()  '' SHI Commented '' WMC.Logic.BitGoUtil.GetBalance() / 100000000
                        LabelBitGoBalance.Text = String.Format("{0:0.00} BTC", (val))
                        If val > 0.5 And val < 1 Then LabelBitGoBalance.BackColor = Drawing.Color.Yellow
                        If val <= 0.5 Then LabelBitGoBalance.BackColor = Drawing.Color.Red
                    Catch ex As Exception
                        LabelBitGoBalance.Text = "?"
                    End Try
                End If

                Dim nExceptions As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM AuditTrail WHERE (AuditTrailLevelId > 4)").ToString
                LinkButtonExceptions.Text = nExceptions.ToString
                If nExceptions > 0 Then LinkButtonExceptions.ForeColor = Drawing.Color.Red Else LinkButtonExceptions.ForeColor = Drawing.Color.Green

                LabelRevenueYesterday.Text = WMC.GetYesterdaysRevenue("DKK")
                LabelRevenueThisMonth.Text = WMC.GetThisMonthRevenue("DKK")
                LabelRevenueToday.Text = WMC.GetTodaysRevenue("DKK")

                Try
                    Dim nOrderRevenueToday As Decimal = eQuote.ExecuteScalar("SELECT SUM([Order].Amount / [Order].RateHome * [Order].RateBooks) AS AmountDKK FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN TransactionType ON TransactionType.Id = [Transaction].Type WHERE  (NOT ([Transaction].Completed IS NULL)) AND (NOT ([Order].Amount / [Order].RateHome * [Order].RateBooks IS NULL)) AND ([Transaction].Completed >= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0)) AND ([Transaction].Type = 1)")
                    LabelRevenueToday.Text = String.Format("({0:n0} DKK)", nOrderRevenueToday)
                Catch ex As Exception
                    LabelRevenueToday.Text = "(0 DKK)"
                End Try

                Dim nOrderTotal As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order]") '+ w
                LabelOrderTotal.Text = nOrderTotal.ToString

                Dim nKycDeclined As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id WHERE  (OrderStatus.Text = N'KYC Declined')") '+ w2
                'LinkButtonKycDeclined.Text = nKycDeclined.ToString

                Dim nCompleted As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id WHERE  (OrderStatus.Text = N'Completed')") '+ w2
                LabelCompleted.Text = nCompleted.ToString

                Dim nCustomers As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [User] ")
                LinkButtonCustomers.Text = nCustomers.ToString

                Dim nUsers As Integer = eQuote.ExecuteScalar("SELECT COUNT(DISTINCT [Order].Id) FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId  INNER JOIN OrderStatus ON (OrderStatus.Id = [Order].Status)  INNER JOIN OrderType ON ([Order].Type = 1 OR [Order].Type=2) INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN Country ON [User].CountryId = Country.Id INNER JOIN UserRiskLevelType ON (UserRiskLevelType.Id = [User].UserRiskLevel) WHERE ([User].Tier = 3 AND [Order].Status = 3) OR  ([Order].Status = 26) OR ([User].UserRiskLevel = 2 AND [Order].Status = 7 )")

                Dim nUsersCount As Integer = eQuote.ExecuteScalar("SELECT COUNT(DISTINCT [Order].Id) FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN Country ON [User].CountryId = Country.Id WHERE (OrderStatus.Text = 'Compliance Officer Approval') OR ([User].UserRiskLevel = 1 AND[Order].Status = 7) ")

                ButtonEnhancedDueDiligence.Text = "Enhanced Due Diligence (" + nUsers.ToString + ")"
                If nUsers > 0 Then ButtonEnhancedDueDiligence.BackColor = Drawing.Color.FromName("#66FF66")

                ButtonComplianceOfficerApproval.Text = "Compliance Officer Approval (" + nUsersCount.ToString + ")"
                If WMC.StatusRecordCount("Compliance Officer Approval") > 0 Then ButtonComplianceOfficerApproval.BackColor = Drawing.Color.FromName("#66FF66")

                ButtonCustomerResponsePending.Text = "Customer Response Pending (" + WMC.StatusRecordCount("Customer Response Pending").ToString + ")"

                ButtonKYCApprovalPending.Text = "KYC Approval Pending (" + WMC.StatusRecordCount("KYC Approval Pending").ToString + ")"
                If WMC.StatusRecordCount("KYC Approval Pending") > 0 Then ButtonKYCApprovalPending.BackColor = Drawing.Color.FromName("#66FF66")

                Dim nBank As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM  OrderStatus INNER JOIN [Order] ON OrderStatus.Id = [Order].Status INNER JOIN PaymentType ON [Order].PaymentType = PaymentType.Id WHERE  (OrderStatus.Text = N'Quoted' AND [order].Type=1) AND (PaymentType.Text = N'Bank')")
                ButtonApproveBankPayment.Text = "Approve Bank Payment (" + nBank.ToString + ")"

                ButtonApproveSell2Bank.Text = "Received Crypto Payment (" + WMC.StatusRecordCount("Received Crypto Payment").ToString + ")"
                If WMC.StatusRecordCount("Received Crypto Payment") > 0 Then ButtonApproveSell2Bank.BackColor = Drawing.Color.FromName("#66FF66")

                If WMC.StatusRecordCount("Payment Aborted") > 0 Then
                    LabelPaymentAborted19.Text = "Payment Aborted (" + WMC.StatusRecordCount("Payment Aborted").ToString + ")" : LabelPaymentAborted19.Visible = True
                Else
                    LabelPaymentAborted19.Visible = False
                End If
                If WMC.StatusRecordCount("Sending Aborted") > 0 Then
                    LabelSendingAborted12.Text = "Sending Aborted (" + WMC.StatusRecordCount("Sending Aborted").ToString + ")" : LabelSendingAborted12.Visible = True
                Else
                    LabelSendingAborted12.Visible = False
                End If
                If WMC.StatusRecordCount("Capture Errored") > 0 Then
                    LabelCaptureErrored20.Text = "Capture Errored (" + WMC.StatusRecordCount("Capture Errored").ToString + ")" : LabelCaptureErrored20.Visible = True
                Else
                    LabelCaptureErrored20.Visible = False
                End If
                If WMC.StatusRecordCount("Releasing payment Aborted") > 0 Then
                    LabelReleasingPaymentAborted15.Text = "Releasing payment Aborted (" + WMC.StatusRecordCount("Releasing payment Aborted").ToString + ")" : LabelReleasingPaymentAborted15.Visible = True
                Else
                    LabelReleasingPaymentAborted15.Visible = False
                End If
                If WMC.StatusRecordCount("Paid") > 0 Then
                    LabelPaid3.Text = "Paid (" + WMC.StatusRecordCount("Paid").ToString + ")" : LabelPaid3.Visible = True
                Else
                    LabelPaid3.Visible = False
                End If
                If WMC.StatusRecordCount("Released payment") > 0 Then
                    LabelReleasedPayment16.Text = "Released payment (" + WMC.StatusRecordCount("Released payment").ToString + ")" : LabelReleasedPayment16.Visible = True
                Else
                    LabelReleasedPayment16.Visible = False
                End If
                If WMC.StatusRecordCount("Payout awaits approval") > 0 Then
                    LabelPayoutAwaitsApproval11.Text = "Payout awaits approval (" + WMC.StatusRecordCount("Payout awaits approval").ToString + ")" : LabelPayoutAwaitsApproval11.Visible = True
                Else
                    LabelPayoutAwaitsApproval11.Visible = False
                End If
                'Dim nDoublePayoutsTotal As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM (SELECT COUNT(*) AS Antal FROM [Transaction] GROUP BY OrderId, Type HAVING  (Type = 2) AND (COUNT(*) > 4)) AS derivedtbl_1")
                'If nDoublePayoutsTotal > 0 Then LabelDoublePayouts.Visible = True : LabelDoublePayouts.Text = LabelDoublePayouts.Text + " (" + nDoublePayoutsTotal.ToString + ")"
                Dim nDoublePaymentsTotal As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM (SELECT COUNT(*) AS Antal FROM [Transaction] GROUP BY OrderId, Type HAVING  (Type = 1) AND (COUNT(*) > 1)) AS derivedtbl_1")
                If nDoublePaymentsTotal > 0 Then
                    Dim t As String = ""
                    Using dr As SqlDataReader = eQuote.GetDataReader("SELECT OrderId FROM [Transaction] GROUP BY OrderId, Type HAVING (Type = 1) AND (COUNT(*) > 1)")
                        Try
                            If dr.HasRows Then
                                While dr.Read()
                                    t += dr!OrderId.ToString + ", "
                                End While
                            End If
                        Finally
                            dr.Close()
                        End Try
                    End Using
                    LabelDoublePayments.Visible = True : LabelDoublePayments.Text = LabelDoublePayments.Text + " (" + nDoublePaymentsTotal.ToString + ") " + t
                End If
                'If nDoublePaymentsTotal > 0 Then LabelDoublePayments.Visible = True : LabelDoublePayments.Text = LabelDoublePayments.Text + " (" + nDoublePaymentsTotal.ToString + ")"
            End If
        End If
    End Sub

    Protected Sub LinkButtonChangePassword_Click(sender As Object, e As EventArgs) Handles LinkButtonChangePassword.Click
        PanelAccess.Visible = False
        PanelChangePassword.Visible = True
    End Sub

    Protected Sub ButtonCancelPasswordChange_Click(sender As Object, e As EventArgs) Handles ButtonCancelPasswordChange.Click
        PanelAccess.Visible = True
        PanelChangePassword.Visible = False
    End Sub

    Protected Sub ButtonChangesPassword_Click(sender As Object, e As EventArgs) Handles ButtonChangesPassword.Click
        LabelPasswordChangeFailed.Visible = False
        Dim changed_success = ChangePassword(Session("UserId").ToString, TextBoxCurrentPassword.Text.Trim, TextBoxNewPassword.Text.Trim)
        If changed_success = False Then
            LabelPasswordChangeFailed.Visible = True
            PanelAccess.Visible = False
            Return
        Else
            PanelAccess.Visible = True
            PanelChangePassword.Visible = False
        End If
        '' eQuote.ExecuteScalar("UPDATE [User] set Password = '" + TextBoxNewPassword.Text.Trim + "' WHERE Id=" + Session("UserId").ToString)

    End Sub

    Protected Sub ButtonUpdateStatus_Click(sender As Object, e As EventArgs) Handles ButtonUpdateStatus.Click
        Session("Status") = DropDownStatus.SelectedValue
        eQuote.ExecuteScalar("UPDATE AppSettings SET ConfigValue = '" + DropDownStatus.SelectedValue + "' WHERE ConfigKey = 'YourPayTestOrProd'")

        PanelAccess.Visible = True
        PanelUpdateStatus.Visible = False
    End Sub

    Protected Sub ButtonYourPayStatus_Click(sender As Object, e As EventArgs) Handles ButtonYourPayStatus.Click
        LabelCurrentStatus.Text = Session("Status")
        DropDownStatus.SelectedValue = Session("Status")
        PanelAccess.Visible = False
        PanelUpdateStatus.Visible = True
    End Sub

    Protected Sub ButtonCancelYourPayStatus_Click(sender As Object, e As EventArgs) Handles ButtonCancelYourPayStatus.Click
        PanelAccess.Visible = True
        PanelUpdateStatus.Visible = False
    End Sub



    Protected Sub LinkButtonLogout_Click(sender As Object, e As EventArgs) Handles LinkButtonLogout.Click
        Session("UserId") = Nothing
        Response.Redirect("Default.aspx")
    End Sub

    Protected Sub DropDownListCreditCardGatewayName_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownListCreditCardGatewayName.SelectedIndexChanged
        Session("CreditCardGatewayName") = DropDownListCreditCardGatewayName.SelectedValue
        eQuote.ExecuteScalar("UPDATE AppSettings SET ConfigValue = '" + DropDownListCreditCardGatewayName.SelectedValue + "' WHERE ConfigKey = 'CreditCardGatewayName'")
    End Sub

    Protected Sub DropDownListOperationalStatus_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownListOperationalStatus.SelectedIndexChanged
        'Session("OperationalStatus") = DropDownListOperationalStatus.SelectedValue
        eQuote.ExecuteScalar("UPDATE AppSettings SET ConfigValue = '" + DropDownListOperationalStatus.SelectedValue + "' WHERE ConfigKey = 'OperationalStatus'")
    End Sub

    Protected Sub DropDownListComplianceAvailability_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownListComplianceAvailability.SelectedIndexChanged
        Session("ComplianceAvailability") = DropDownListComplianceAvailability.SelectedValue
        eQuote.ExecuteScalar("UPDATE AppSettings SET ConfigValue = '" + DropDownListComplianceAvailability.SelectedValue + "' WHERE ConfigKey = 'ComplianceAvailability'")
    End Sub

    'Protected Sub DropDownListMinersFee_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownListMinersFee.SelectedIndexChanged
    '    Session("MinersFee") = DropDownListMinersFee.SelectedValue
    '    eQuote.ExecuteScalar("UPDATE AppSettings SET ConfigValue = '" + DropDownListMinersFee.SelectedValue.Replace(",", ".") + "' WHERE ConfigKey = 'MinersFee'")
    'End Sub

    Protected Sub DropDownListMinersFee_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownListMinersFee.SelectedIndexChanged
        Session("MinersFee") = DropDownListMinersFee.SelectedValue

        Dim settingManager = WMC.Logic.CurrencySettingsManager.GetDefault()
        Dim settingsData As CurrencySettingsValue = settingManager.Get("BTC")

        Dim bitgo_MinersFeeSettings As WMC.Logic.Models.Settings.BitGoCurrencySettings = settingsData.GetJsonData(Of WMC.Logic.Models.Settings.BitGoCurrencySettings)("bitgo")
        bitgo_MinersFeeSettings.MinersFee.Fee = Convert.ToDecimal(DropDownListMinersFee.SelectedValue)
        settingsData.UpdateJsonData(Of WMC.Logic.Models.Settings.BitGoCurrencySettings)(bitgo_MinersFeeSettings)
        settingManager.Update(settingsData.Key, settingsData.BitgoSettings)


        '' SHI Commented '' Dim bitGoMinersFeeSettings As New WMC.Logic.BitGoMinersFeeSettings
        '' SHI Commented '' bitGoMinersFeeSettings.MinersFee = DropDownListMinersFee.SelectedValue
        '' SHI Commented '' Dim bitGoMinersFeeSettingsJSON As String = Newtonsoft.Json.JsonConvert.SerializeObject(bitGoMinersFeeSettings)
        '' SHI Commented '' eQuote.ExecuteScalar("UPDATE AppSettings SET ConfigValue = '" + bitGoMinersFeeSettingsJSON + "' WHERE ConfigKey = 'BitGoMinersFeeSettings'")
    End Sub
    Protected Sub btnUpload_Click(sender As Object, e As EventArgs) Handles btnUpload.Click
        '' TODO: replace this logic with new
        Dim fs As Stream = FileUpload1.PostedFile.InputStream
        Dim filePath As String = FileUpload1.PostedFile.FileName
        Dim filename As String = Path.GetFileName(filePath)
        Dim ext As String = Path.GetExtension(filename).ToLower
        If ext.Contains(".csv") Or ext.Contains(".xml") Then
            Dim csvSourceId = DropDownListCSVFile.SelectedValue
            Dim sourceId = eQuote.ExecuteScalar("SELECT  DISTINCT Fromsource from SanctionsList WHere Fromsource=" + csvSourceId)
            If IsNumeric(sourceId) Then
                eQuote.ExecuteScalar("DELETE FROM SanctionsList WHERE Fromsource=" + csvSourceId)
            End If
            Using reader As IWMCDataReader = SanctionList.SanctionListDataReader.Create(csvSourceId, fs)
                Dim csvData As DataTable = New DataTable()
                csvData.Columns.Add("Name1")
                csvData.Columns.Add("Name2")
                csvData.Columns.Add("Name3")
                csvData.Columns.Add("Name4")
                csvData.Columns.Add("Name5")
                csvData.Columns.Add("Name6")
                csvData.Columns.Add("DOB")
                csvData.Columns.Add("CountryOfResidance")
                csvData.Columns.Add("Summary")
                csvData.Columns.Add("FromSource")
                While reader.Read()
                    Dim csvRow = reader.CurrentRow
                    If reader.IsValid Then
                        Dim dob = CType(reader, IWMCDateParser)
                        If csvRow(6) IsNot Nothing Then
                            csvRow(6) = dob.ToDateTime(csvRow(6))
                        End If
                        csvData.Rows.Add(csvRow)
                    End If
                End While
                Dim conString As String = System.Configuration.ConfigurationManager.ConnectionStrings("LocalConnectionString").ConnectionString()

                Using dbConnection As SqlConnection = New SqlConnection(conString)

                    dbConnection.Open()
                    Using bulkCopy As SqlBulkCopy = New SqlBulkCopy(dbConnection)

                        bulkCopy.DestinationTableName = "SanctionsList"
                        bulkCopy.ColumnMappings.Add("Name1", "Name1")
                        bulkCopy.ColumnMappings.Add("Name2", "Name2")
                        bulkCopy.ColumnMappings.Add("Name3", "Name3")
                        bulkCopy.ColumnMappings.Add("Name4", "Name4")
                        bulkCopy.ColumnMappings.Add("Name5", "Name5")
                        bulkCopy.ColumnMappings.Add("Name6", "Name6")
                        bulkCopy.ColumnMappings.Add("DOB", "DOB")
                        bulkCopy.ColumnMappings.Add("CountryOfResidance", "CountryOfResidance")
                        bulkCopy.ColumnMappings.Add("Summary", "Summary")
                        bulkCopy.ColumnMappings.Add("FromSource", "FromSource")
                        bulkCopy.WriteToServer(csvData)
                        bulkCopy.Close()

                    End Using
                    lblMessage.ForeColor = System.Drawing.Color.Green
                    lblMessage.Text = "Records inserted Successfully"
                End Using

            End Using
        Else
            lblMessage.ForeColor = System.Drawing.Color.Red
            lblMessage.Text = "File format not recognised." & " Upload csv/xml formats"
        End If

    End Sub

    Protected Sub ButtonLockBitGo_Click(sender As Object, e As EventArgs) Handles ButtonLockBitGo.Click
        WMC.LockBitGo()
    End Sub
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
    Protected Sub ButtonApproveSell2Bank_Click(sender As Object, e As EventArgs) Handles ButtonApproveSell2Bank.Click
        Session("ComplianceView") = "Received Crypto Payment"
        Response.Redirect("~/Compliance.aspx")
    End Sub
End Class


