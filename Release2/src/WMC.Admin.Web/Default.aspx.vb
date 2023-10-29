Imports System.Data.SqlClient

Partial Class _Default
    Inherits System.Web.UI.Page
    Protected Sub ButtonLogin_Click(sender As Object, e As EventArgs) Handles ButtonLogin.Click
        Dim w As String = ""
        Dim w2 As String = ""
        Dim s As New StringBuilder
        s.Append("SELECT UserRole.Text AS UserRole, [User].Id AS UserId,  [User].Id, [User].Fname FROM UserRole INNER JOIN [User] ON UserRole.Id = [User].RoleId WHERE [User].Id = " + DropDownListUser.SelectedValue + " AND Password = @password ")
        Dim c As New SqlCommand(s.ToString, eQuote.OpenConnection)
        c.Parameters.AddWithValue("password", TextBoxPassword.Text)
        Dim drUser As SqlDataReader = c.ExecuteReader

        If CInt(DropDownListUser.SelectedValue) > 0 AndAlso drUser.Read() Then
            PanelAccess.Visible = True
            PanelLogin.Visible = False
            LabelWrongPasswordText.Visible = False

            Session("UserId") = drUser!UserId
            Session("UserName") = drUser!Fname
            Session("UserRole") = drUser!UserRole
            Session("Status") = eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'YourPayTestOrProd'")
            Session("OperationalStatus") = eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'OperationalStatus'")
            Session("CreditCardGatewayName") = eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'CreditCardGatewayName'")
            DropDownListOperationalStatus.SelectedValue = Session("OperationalStatus")
            DropDownListCreditCardGatewayName.SelectedValue = Session("CreditCardGatewayName")
            ButtonCompliance.Text = "Compliance Handling (" + WMCData.StatusRecordCount("Compliance Officer Approval").ToString + ")"
            ButtonApprovePayout.Text = "Approve Payout (" + WMCData.StatusRecordCount("Payout awaits approval").ToString + ")"
            Dim nBank As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM  OrderStatus INNER JOIN [Order] ON OrderStatus.Id = [Order].Status INNER JOIN PaymentType ON [Order].PaymentType = PaymentType.Id WHERE  (OrderStatus.Text = N'Quoted') AND (PaymentType.Text = N'Bank')")
            ButtonApproveBankPayment.Text = "Approve Bank Payment (" + nBank.ToString + ")"
            If nBank > 0 Then ButtonApproveBankPayment.BackColor = Drawing.Color.Yellow
            Dim nExceptions As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM AuditTrail INNER JOIN AuditTrailStatus ON AuditTrail.Status = AuditTrailStatus.Id WHERE AuditTrailStatus.Text = N'Application Error'").ToString
            LinkButtonExceptions.Text = nExceptions.ToString
            If nExceptions > 0 Then LinkButtonExceptions.ForeColor = Drawing.Color.Red Else LinkButtonExceptions.ForeColor = Drawing.Color.Green

            If Session("UserRole") = "SiteMgr" Then
                PanelSiteMgrTools.Visible = True
                PanelAdminTools.Visible = False
                Session("SiteId") = eQuote.ExecuteScalar("SELECT UserType.Text FROM [User] INNER JOIN UserType ON [User].UserType = UserType.Id WHERE [User].Id = " + Session("UserId").ToString)
                w = " WHERE SiteId = " + Session("SiteId")
                w2 = " AND SiteId = " + Session("SiteId")
                DropDownListOperationalStatus.Enabled = False
                LinkButtonUserKYC.Enabled = False

                Dim AffiliateMarketeerCommission As Double = 0.011039999999999999

                LabelYesterdaysRev15.Text = WMCData.GetYesterdaysRevenue("EUR", 15)
                LabelTodaysRev15.Text = WMCData.GetTodaysRevenue("EUR", 15)
                LabelTotalRev15.Text = WMCData.GetTotalRevenue("EUR", 15)
                LabelTotalCommision15.Text = WMCData.GetTotalCommission("EUR", 15, AffiliateMarketeerCommission)

                LabelYesterdaysRev16.Text = WMCData.GetYesterdaysRevenue("EUR", 16)
                LabelTodaysRev16.Text = WMCData.GetTodaysRevenue("EUR", 16)
                LabelTotalRev16.Text = WMCData.GetTotalRevenue("EUR", 16)
                LabelTotalCommision16.Text = WMCData.GetTotalCommission("EUR", 16, AffiliateMarketeerCommission)

                LabelYesterdaysRev17.Text = WMCData.GetYesterdaysRevenue("EUR", 17)
                LabelTodaysRev17.Text = WMCData.GetTodaysRevenue("EUR", 17)
                LabelTotalRev17.Text = WMCData.GetTotalRevenue("EUR", 17)
                LabelTotalCommision17.Text = WMCData.GetTotalCommission("EUR", 17, AffiliateMarketeerCommission)
            Else


                PanelSiteMgrTools.Visible = False
                PanelAdminTools.Visible = True
                Dim nKYC As Integer = eQuote.ExecuteScalar("SELECT COUNT(DISTINCT KycFile.UserId)FROM KycFile INNER JOIN [User] ON KycFile.UserId = [User].Id WHERE  (KycFile.Obsolete IS NULL) AND (KycFile.Approved IS NULL)")
                LinkButtonUserKYC.Text = nKYC.ToString
                If nKYC > 0 Then LinkButtonUserKYC.ForeColor = Drawing.Color.Red Else LinkButtonUserKYC.ForeColor = Drawing.Color.Green

                'Try
                '    Dim nOrderRevenueYesterday As Decimal = eQuote.ExecuteScalar("SELECT SUM([Order].Amount / [Order].RateHome * [Order].RateBooks) AS AmountDKK FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN TransactionType ON TransactionType.Id = [Transaction].Type WHERE  (NOT ([Transaction].Completed IS NULL)) AND (NOT ([Order].Amount / [Order].RateHome * [Order].RateBooks IS NULL)) AND ([Transaction].Completed >= DATEADD(day, DATEDIFF(day, 1, GETDATE()), 0)) AND ([Transaction].Completed < DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0)) AND ([Transaction].Type = 1)")
                '    LabelRevenueYesterday.Text = String.Format("{0:n0} DKK", nOrderRevenueYesterday)
                'Catch ex As Exception
                '    LabelRevenueYesterday.Text = "0 DKK"
                'End Try
                LabelRevenueYesterday.Text = WMCData.GetYesterdaysRevenue("DKK")
                'Try
                '    Dim nOrderRevenueToday As Decimal = eQuote.ExecuteScalar("SELECT SUM([Order].Amount / [Order].RateHome * [Order].RateBooks) AS AmountDKK FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN TransactionType ON TransactionType.Id = [Transaction].Type WHERE  (NOT ([Transaction].Completed IS NULL)) AND (NOT ([Order].Amount / [Order].RateHome * [Order].RateBooks IS NULL)) AND ([Transaction].Completed >= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0)) AND ([Transaction].Type = 1)")
                '    LabelRevenueToday.Text = String.Format("({0:n0} DKK)", nOrderRevenueToday)
                'Catch ex As Exception
                '    LabelRevenueToday.Text = "(0 DKK)"
                'End Try
                LabelRevenueToday.Text = WMCData.GetTodaysRevenue("DKK")


                Dim nOrderTotal As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order]" + w)
                LinkButtonOrderTotal.Text = nOrderTotal.ToString

                Dim nKycDeclined As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id WHERE  (OrderStatus.Text = N'KYC Declined')" + w2)
                LinkButtonKycDeclined.Text = nKycDeclined.ToString

                Dim nCompleted As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id WHERE  (OrderStatus.Text = N'Completed')" + w2)
                LinkButtonCompleted.Text = nCompleted.ToString

                Dim nCustomers As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [User]")
                LinkButtonCustomers.Text = nCustomers.ToString
            End If
            LabelMe.Text = Session("UserName")

        End If
        LabelWrongPasswordText.Visible = True
        If Session("UserRole") = "Admin" Then LabelRole.Text = "Administrator" Else LabelRole.Text = "SiteManager"

        drUser.Close()
        c.Connection.Close()
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
        eQuote.ExecuteScalar("UPDATE [User] set Password = '" + TextBoxNewPassword.Text.Trim + "' WHERE Id=" + Session("UserId").ToString)
        PanelAccess.Visible = True
        PanelChangePassword.Visible = False
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

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If IsNumeric(Session("UserId")) Then
            PanelAccess.Visible = True
            PanelLogin.Visible = False
            LabelMe.Text = Session("UserName")
            If Session("UserRole") = "Admin" Then LabelRole.Text = "Administrator" Else LabelRole.Text = "SiteManager"

            If Session("UserRole") = "SiteMgr" Then
                
                PanelSiteMgrTools.Visible = True
                PanelAdminTools.Visible = False
                Session("SiteId") = eQuote.ExecuteScalar("SELECT UserType.Text FROM [User] INNER JOIN UserType ON [User].UserType = UserType.Id WHERE [User].Id = " + Session("UserId").ToString)

                Dim AffiliateMarketeerCommission As Double = 0.017999999999999999

                LabelYesterdaysRev15.Text = WMCData.GetYesterdaysRevenue("EUR", 15)
                LabelTodaysRev15.Text = WMCData.GetTodaysRevenue("EUR", 15)
                LabelTotalRev15.Text = WMCData.GetTotalRevenue("EUR", 15)
                LabelTotalCommision15.Text = WMCData.GetTotalCommission("EUR", 15, AffiliateMarketeerCommission)

                LabelYesterdaysRev16.Text = WMCData.GetYesterdaysRevenue("EUR", 16)
                LabelTodaysRev16.Text = WMCData.GetTodaysRevenue("EUR", 16)
                LabelTotalRev16.Text = WMCData.GetTotalRevenue("EUR", 16)
                LabelTotalCommision16.Text = WMCData.GetTotalCommission("EUR", 16, AffiliateMarketeerCommission)

                LabelYesterdaysRev17.Text = WMCData.GetYesterdaysRevenue("EUR", 17)
                LabelTodaysRev17.Text = WMCData.GetTodaysRevenue("EUR", 17)
                LabelTotalRev17.Text = WMCData.GetTotalRevenue("EUR", 17)
                LabelTotalCommision17.Text = WMCData.GetTotalCommission("EUR", 17, AffiliateMarketeerCommission)
            ElseIf Session("UserRole") = "Admin" Then
                PanelSiteMgrTools.Visible = False
                PanelAdminTools.Visible = True
                Dim nKYC As Integer = eQuote.ExecuteScalar("SELECT COUNT(DISTINCT KycFile.UserId)FROM KycFile INNER JOIN [User] ON KycFile.UserId = [User].Id WHERE  (KycFile.Obsolete IS NULL) AND (KycFile.Approved IS NULL)")
                LinkButtonUserKYC.Text = nKYC.ToString
                If nKYC > 0 Then LinkButtonUserKYC.ForeColor = Drawing.Color.Red Else LinkButtonUserKYC.ForeColor = Drawing.Color.Green
                If Not IsPostBack Then
                    Session("OperationalStatus") = eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'OperationalStatus'")
                    DropDownListOperationalStatus.SelectedValue = Session("OperationalStatus")
                    Session("CreditCardGatewayName") = eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'CreditCardGatewayName'")
                    DropDownListCreditCardGatewayName.SelectedValue = Session("CreditCardGatewayName")
                End If

                Dim nExceptions As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM AuditTrail INNER JOIN AuditTrailStatus ON AuditTrail.Status = AuditTrailStatus.Id WHERE AuditTrailStatus.Text = N'Application Error'").ToString
                LinkButtonExceptions.Text = nExceptions.ToString
                If nExceptions > 0 Then LinkButtonExceptions.ForeColor = Drawing.Color.Red Else LinkButtonExceptions.ForeColor = Drawing.Color.Green

                'If Not IsPostBack Then
                '    ButtonCaptureErrored.Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
                '    ButtonSendingAborted.Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
                '    ButtonReleasingPaymentAborted.Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
                'End If

                'Dim n As Integer
                'n = WMC.StatusRecordCount("Capture Errored")
                'If n > 0 Then LinkButtonCaptureErrored.Visible = True : LinkButtonCaptureErrored.Text = "Capture Errored: " + n.ToString : ButtonCaptureErrored.Visible = True Else LinkButtonCaptureErrored.Visible = False
                'n = WMC.StatusRecordCount("Sending Aborted")
                'If n > 0 Then LinkButtonSendingAborted.Visible = True : LinkButtonSendingAborted.Text = "Sending Aborted: " + n.ToString : ButtonSendingAborted.Visible = True Else LinkButtonSendingAborted.Visible = False
                'n = WMC.StatusRecordCount("Releasing payment Aborted")
                'If n > 0 Then LinkButtonReleasingPaymentAborted.Visible = True : LinkButtonReleasingPaymentAborted.Text = "Releasing payment Aborted: " + n.ToString : ButtonReleasingPaymentAborted.Visible = True Else LinkButtonReleasingPaymentAborted.Visible = False

                Try
                    Dim nOrderRevenueYesterday As Decimal = eQuote.ExecuteScalar("SELECT SUM([Order].Amount / [Order].RateHome * [Order].RateBooks) AS AmountDKK FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN TransactionType ON TransactionType.Id = [Transaction].Type WHERE  (NOT ([Transaction].Completed IS NULL)) AND (NOT ([Order].Amount / [Order].RateHome * [Order].RateBooks IS NULL)) AND ([Transaction].Completed >= DATEADD(day, DATEDIFF(day, 1, GETDATE()), 0)) AND ([Transaction].Completed < DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0)) AND ([Transaction].Type = 1)")
                    LabelRevenueYesterday.Text = String.Format("{0:n0} DKK", nOrderRevenueYesterday)
                Catch ex As Exception
                    LabelRevenueYesterday.Text = "0 DKK"
                End Try

                Try
                    Dim nOrderRevenueToday As Decimal = eQuote.ExecuteScalar("SELECT SUM([Order].Amount / [Order].RateHome * [Order].RateBooks) AS AmountDKK FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN TransactionType ON TransactionType.Id = [Transaction].Type WHERE  (NOT ([Transaction].Completed IS NULL)) AND (NOT ([Order].Amount / [Order].RateHome * [Order].RateBooks IS NULL)) AND ([Transaction].Completed >= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0)) AND ([Transaction].Type = 1)")
                    LabelRevenueToday.Text = String.Format("({0:n0} DKK)", nOrderRevenueToday)
                Catch ex As Exception
                    LabelRevenueToday.Text = "(0 DKK)"
                End Try

                Dim nOrderTotal As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order]") '+ w
                LinkButtonOrderTotal.Text = nOrderTotal.ToString

                Dim nKycDeclined As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id WHERE  (OrderStatus.Text = N'KYC Declined')") '+ w2
                LinkButtonKycDeclined.Text = nKycDeclined.ToString

                Dim nCompleted As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id WHERE  (OrderStatus.Text = N'Completed')") '+ w2
                LinkButtonCompleted.Text = nCompleted.ToString

                'Dim nCancelled As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id WHERE  (OrderStatus.Text = N'Cancel')") '+ w2
                'LinkButtonCancelled.Text = nCancelled.ToString

                'Dim nRejected As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id WHERE  (OrderStatus.Text = N'Quoted')") '+ w2
                'LinkButtonRejected.Text = nRejected.ToString

                Dim nCustomers As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [User]")
                LinkButtonCustomers.Text = nCustomers.ToString

                ButtonCompliance.Text = "Compliance Handling (" + WMCData.StatusRecordCount("Compliance Officer Approval").ToString + ")"
                ButtonApprovePayout.Text = "Approve Payout (" + WMCData.StatusRecordCount("Payout awaits approval").ToString + ")"
                Dim nBank As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM  OrderStatus INNER JOIN [Order] ON OrderStatus.Id = [Order].Status INNER JOIN PaymentType ON [Order].PaymentType = PaymentType.Id WHERE  (OrderStatus.Text = N'Quoted') AND (PaymentType.Text = N'Bank')")
                ButtonApproveBankPayment.Text = "Approve Bank Payment (" + nBank.ToString + ")"
                If nBank > 0 Then ButtonApproveBankPayment.BackColor = Drawing.Color.Yellow
                'ButtonApproveBankPayment.Text = "Approve Bank Payment (" + eQuote.ExecuteScalar("SELECT COUNT(*) FROM  OrderStatus INNER JOIN [Order] ON OrderStatus.Id = [Order].Status INNER JOIN PaymentType ON [Order].PaymentType = PaymentType.Id WHERE  (OrderStatus.Text = N'Quoted') AND (PaymentType.Text = N'Bank')").ToString + ")"
            End If
        End If
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
        Session("OperationalStatus") = DropDownListOperationalStatus.SelectedValue
        eQuote.ExecuteScalar("UPDATE AppSettings SET ConfigValue = '" + DropDownListOperationalStatus.SelectedValue + "' WHERE ConfigKey = 'OperationalStatus'")
    End Sub

    'Protected Sub ButtonCaptureErrored_Click(sender As Object, e As EventArgs) Handles ButtonCaptureErrored.Click
    '    ' Errored Capture (20) -> Payout Approved (18)
    '    eQuote.ExecuteNonQuery("UPDATE [Order] SET Status =18 FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id WHERE OrderStatus.Text = N'Capture Errored'")
    'End Sub

    'Protected Sub ButtonSendingAborted_Click(sender As Object, e As EventArgs) Handles ButtonSendingAborted.Click
    '    'Sending Aborted (12) ->  Released Payment (16) 
    '    eQuote.ExecuteNonQuery("UPDATE [Order] SET Status =16 FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id WHERE OrderStatus.Text = N'Sending Aborted'")
    'End Sub

    'Protected Sub ButtonReleasingPaymentAborted_Click(sender As Object, e As EventArgs) Handles ButtonReleasingPaymentAborted.Click
    '    'Releasing Payment Aborted (15)  ->  Released Payment (16) 
    '    eQuote.ExecuteNonQuery("UPDATE [Order] SET Status =16 FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id WHERE OrderStatus.Text = N'Releasing Payment Aborted (15)'")
    'End Sub
End Class


