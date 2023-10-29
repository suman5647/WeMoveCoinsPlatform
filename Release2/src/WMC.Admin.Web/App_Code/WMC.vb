Imports Microsoft.VisualBasic
Imports System.Xml
Imports System.Data.SqlClient
Imports System.IO
Imports System.Data
Imports System.Configuration.ConfigurationManager
Imports Microsoft.Win32
Imports System.IO.Compression
Imports System.Net
Imports System.Net.Mail
Imports Paylike.NET
Imports Paylike.NET.RequestModels.Transactions
Imports Newtonsoft.Json
Imports System.Web.UI.WebControls
Imports System
Imports System.Text

Public Module WMCData


    '************** START MOVE TO DLL *************************************************************************************************************************

    Function TrustLevelCalculation(ByVal OrderId As Integer, ByVal Execute As Boolean, Optional ByRef c As Label = Nothing) As String
        Dim UserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + OrderId.ToString)
        Dim UserIsTrusted As Boolean = IsUserTrusted(OrderId)
        Dim CardNumber As String = eQuote.ExecuteScalar("SELECT CardNumber FROM [Order] WHERE Id =" + OrderId.ToString).ToString.Trim
        Dim CardIsApproved As Boolean = IsCardApproved(OrderId)
        Dim PhoneIsVirtual As Boolean = IsPhoneVitual(OrderId)
        Dim CardUsedElsewhere As Integer = GetCardUsedElsewhere(OrderId)
        Dim EmailUsedElsewhere As Integer = GetEmailUsedElsewhere(OrderId)
        Dim CountryTrust As String = GetCountryTrustLevel(OrderId)
        Dim SiteTrust As String = GetSiteTrustLevel(OrderId)
        Dim TxLimitEUR As Integer = 150
        Dim OrderSumEUR As Decimal = GetOrderSumEUR(OrderId)
        Dim OrderSumTotalEUR As Decimal = GetOrderSumTotalEUR(OrderId, CardNumber)
        Dim message As String = ""

        If IsCardUS(OrderId) Then
            If Execute Then
                If IsOrderStatus("Payout awaits approval", OrderId) Then
                    Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'KYC Decline'")
                    eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
                End If
            End If
            message = "Decline - US Card"
            If Not IsNothing(c) Then c.BackColor = Drawing.Color.FromName("#FFA8C5")
        Else
            If (Not CardIsApproved And ((OrderSumEUR + OrderSumTotalEUR) > TxLimitEUR * CountryTrust * SiteTrust)) Then
                If Execute Then
                    If IsOrderStatus("Payout awaits approval", OrderId) Then
                        Dim dr As SqlDataReader = eQuote.GetDataReader("SELECT TOP (1) [Order].Id, [Order].Number, [User].Fname, [Order].Amount, Currency.Code, [Order].Quoted, [Order].CryptoAddress, [Order].Name, [Order].Email, [Order].IP,  OrderStatus.Text AS Status, [Order].Note, [Order].Rate, [Order].CommissionProcent, [Order].CardNumber, [Order].SiteId, SUBSTRING([Order].CountryCode, 1, 5) AS BrowserCulture, [Order].TxSecret FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN [Order] AS Order_1 ON [User].Id = Order_1.UserId WHERE (Order_1.Id = " + OrderId.ToString + ") AND (NOT ([Order].TxSecret IS NULL)) AND (NOT ([Order].CardNumber IS NULL)) ORDER BY [Order].Quoted")
                        If dr.HasRows Then
                            dr.Read()
                            SendEmail("RequestTxSecret.htm", dr!SiteId, dr!Name, dr!Email, "CREDIT CARD VERIFICATION OF ORDER #" + dr!Number, dr!Number, String.Format("{0:d/M/yyyy}", dr!Quoted), String.Format("{0:n0}", dr!Amount) + " " + dr!Code, dr!CardNumber)
                            Dim status As Integer = eQuote.ExecuteScalar("SELECT Id FROM AuditTrailStatus WHERE Text = N'SentEmail'")
                            message = "RequestTxSecret.htm" + vbCrLf + "Number:" + dr!Number + ", TxSecret:" + dr!TxSecret
                            eQuote.ExecuteNonQuery("INSERT INTO AuditTrail(OrderId, Status, Message, Created) VALUES (" + OrderId.ToString + ", " + status.ToString + ", N'" + message + "', { fn NOW() })")
                            status = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Customer Response Pending'")
                            eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + status.ToString + " WHERE (Id = " + OrderId.ToString + ")")
                            AddNote(OrderId, "Tx-Secret Requested")
                        End If
                        dr.Close()
                    End If
                End If
                message = "Request Tx from client"
                If Not IsNothing(c) Then c.BackColor = Drawing.Color.LightYellow
            Else
                If PhoneIsVirtual Or (CardUsedElsewhere > 0 Or EmailUsedElsewhere > 0) Or (Not PhoneCardOrigin_Match(OrderId)) Or (Not PhoneIP_Match(OrderId)) Then
                    message = ""
                    If PhoneIsVirtual Then message += "Virtual Number, "
                    If (CardUsedElsewhere > 0 Or EmailUsedElsewhere > 0) Then message += "Data Used Elsewhere, "
                    If Not PhoneCardOrigin_Match(OrderId) Then message += "Phone/Card Mismatch, "
                    If Not PhoneIP_Match(OrderId) Then message += "  - Phone/IP Mismatch, "
                    If UserIsTrusted Then
                        If Not IsNothing(c) Then c.BackColor = Drawing.Color.FromName("#66FF66")
                        message = "Approved although:" + message
                        If Execute Then
                            If IsOrderStatus("Payout awaits approval", OrderId) Then
                                Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Payout approved'")
                                eQuote.ExecuteNonQuery("UPDATE [Order] SET Approved = { fn NOW() }, Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
                            End If
                        End If
                    Else
                        message = "Manual Approval: " + message
                        If Not IsNothing(c) Then c.BackColor = Drawing.Color.FromName("#FFFF66")
                    End If
                Else
                    If Not IsNothing(c) Then c.BackColor = Drawing.Color.FromName("#66FF66")
                    message = "Approved: " + message
                    If Execute Then
                        If IsOrderStatus("Payout awaits approval", OrderId) Then
                            Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Payout approved'")
                            eQuote.ExecuteNonQuery("UPDATE [Order] SET Approved = { fn NOW() }, Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
                        End If
                    End If
                End If
            End If
        End If
        Return message
    End Function

    '#FFA8C5 = RED
    '#FFFF66 = YELLOW
    '#66FF66 = GREEN

    Function IsCardApproved(OrderId As Integer, Optional ByRef c As Label = Nothing) As Boolean
        Dim CardNumber As String = eQuote.ExecuteScalar("SELECT CardNumber FROM [Order] WHERE Id =" + OrderId.ToString)
        Dim n As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE CardNumber = N'" + CardNumber + "' AND NOT (CardApproved IS NULL)")
        If n = 0 Then
            Return False
        Else
            If Not IsNothing(c) Then c.BackColor = Drawing.Color.FromName("#66FF66")
            Return True
        End If
    End Function

    Function GetCountryTrustLevel(OrderId As Integer) As Decimal
        Dim CountryCode As String = GetPhoneCountry(OrderId)
        Return eQuote.ExecuteScalar("SELECT TrustValue FROM Country WHERE Code = N'" + CountryCode + "'")
    End Function

    Function GetSiteTrustLevel(OrderId As Integer) As Decimal
        Dim SiteId As Integer = eQuote.ExecuteScalar("SELECT SiteId FROM [Order] WHERE Id = " + OrderId.ToString)
        If SiteId = 1 Then Return 0.8 Else Return 1
    End Function

    Function IsPhoneVitual(OrderId As Integer) As Boolean
        Dim names As Integer = GetDifferentNameUser(OrderId)
        Dim emails As Integer = GetDifferentEmailUser(OrderId)
        If (names > 1 And emails > 1) Or names > 2 Or emails > 2 Then Return True Else Return False
    End Function

    Function PhoneCardOrigin_Match(OrderId As Integer) As Boolean
        If GetPhoneCountry(OrderId) = GetCardCountry(OrderId) Then Return True Else Return False
    End Function

    Function PhoneIP_Match(OrderId As Integer) As Boolean
        If GetPhoneCountry(OrderId) = GetIpCountry(OrderId) Then Return True Else Return False
    End Function

    Function GetCardUsedElsewhere(OrderId As Integer, Optional ByRef c As LinkButton = Nothing) As Integer
        Dim UserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + OrderId.ToString)
        Dim dr As SqlDataReader = eQuote.GetDataReader("SELECT [Transaction].Info, [Order].UserId, [User].Phone, [Order].CardNumber, [Order].CountryCode, [Order].Amount, [Order].CurrencyId, AuditTrail.Message AS Phone_Message, AuditTrail_1.Message AS IP_Message, [Order].CryptoAddress, [User].Email, [Order].IP  FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN AuditTrail ON [Order].Id = AuditTrail.OrderId INNER JOIN AuditTrail AS AuditTrail_1 ON [Order].Id = AuditTrail_1.OrderId WHERE ([Order].Id = " + OrderId.ToString + ") AND ([Transaction].Type = 1) AND (AuditTrail.Status = 3) AND (AuditTrail_1.Status = 4)")
        Dim n As Integer
        If dr.HasRows Then
            dr.Read()
            Try
                n = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE  (NOT (UserId = " + UserId.ToString + ")) AND (CardNumber = N'" + dr!CardNumber + "')").ToString
                If Not IsNothing(c) Then
                    c.CommandArgument = dr!CardNumber
                    If n > 0 Then c.BackColor = Drawing.Color.FromName("#FFFF66")
                End If
            Catch ex As Exception
                n = -1
            End Try
        End If
        dr.Close()
        Return n
    End Function

    Function GetEmailUsedElsewhere(OrderId As Integer, Optional ByRef c As LinkButton = Nothing) As Integer
        Dim UserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + OrderId.ToString)
        Dim dr As SqlDataReader = eQuote.GetDataReader("SELECT [Transaction].Info, [Order].UserId, [User].Phone, [Order].CardNumber, [Order].CountryCode, [Order].Amount, [Order].CurrencyId, AuditTrail.Message AS Phone_Message, AuditTrail_1.Message AS IP_Message, [Order].CryptoAddress, [User].Email, [Order].IP  FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN AuditTrail ON [Order].Id = AuditTrail.OrderId INNER JOIN AuditTrail AS AuditTrail_1 ON [Order].Id = AuditTrail_1.OrderId WHERE ([Order].Id = " + OrderId.ToString + ") AND ([Transaction].Type = 1) AND (AuditTrail.Status = 3) AND (AuditTrail_1.Status = 4)")
        Dim n As Integer
        If dr.HasRows Then
            dr.Read()
            Try
                n = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE  (NOT (UserId = " + UserId.ToString + ")) AND (Email = N'" + dr!Email + "')").ToString
                If Not IsNothing(c) Then
                    c.CommandArgument = dr!Email
                    If n > 1 Then c.BackColor = Drawing.Color.FromName("#FFFF66")
                End If
            Catch ex As Exception
                n = -1
            End Try
        End If
        dr.Close()
        Return n
    End Function

    Function IsCardUS(OrderId As Integer, Optional ByRef c As Label = Nothing) As Boolean
        If GetCardCountry(OrderId) = "US" Then
            Return True
            If Not IsNothing(c) Then c.BackColor = Drawing.Color.FromName("#FFA8C5")
        Else
            Return False
        End If
    End Function

    '--------------------------------------------------------------------------------------------------

    Function GetOrdersByStatus(ByVal OrderId As Integer, Status As String) As Integer
        Dim UserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + OrderId.ToString)
        Dim n As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id WHERE OrderStatus.Text = N'" + Status + "' AND [Order].UserId = " + UserId.ToString).ToString
        If IsNumeric(n) Then Return n Else Return 0
    End Function

    Function GetOrderSpan(OrderId As Integer, Optional CardNumber As String = "") As Integer
        Dim WhereCard As String = ""
        If CardNumber.Length > 0 Then WhereCard = " AND CardNumber = N'" + CardNumber + "'"
        Dim UserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + OrderId.ToString)
        Dim Obj As Object = eQuote.ExecuteScalar("SELECT DATEDIFF(DD, MIN(Quoted), MAX(Quoted)) AS dif FROM [Order] WHERE (UserId = " + UserId.ToString + ") AND (Status = 17 OR Status = 11)" + WhereCard)
        If IsDBNull(Obj) Then Return 0 Else Return CInt(Obj)
    End Function

    Function GetOrderSumEUR(OrderId As Integer) As Decimal
        Dim AmountEUR As Decimal
        Try
            AmountEUR = eQuote.ExecuteScalar("SELECT Amount / RateHome AS AmountEUR FROM [Order] WHERE Id = " + OrderId.ToString)
        Catch ex As Exception
            Dim CurrencyCode As String = eQuote.ExecuteScalar("SELECT Currency.Code FROM [Order] INNER JOIN Currency ON [Order].CurrencyId = Currency.Id WHERE [Order].Id = " + OrderId.ToString)
            Dim LocalAmount As Decimal = eQuote.ExecuteScalar("SELECT Amount FROM [Order] WHERE Id = " + OrderId.ToString)
            Select Case CurrencyCode
                Case "DKK"
                    AmountEUR = LocalAmount / 7.44
                Case "GBP"
                    AmountEUR = LocalAmount / 0.852829
                Case "INR"
                    AmountEUR = LocalAmount / 72.76
                Case "SEK"
                    AmountEUR = LocalAmount / 9.45
                Case "EUR"
                    AmountEUR = LocalAmount
            End Select
        End Try
        Return AmountEUR
    End Function

    Function GetOrderSumTotalEUR(OrderId As Integer, Optional CardNumber As String = "") As Decimal
        Dim UserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + OrderId.ToString)
        Dim AmountEUR As Decimal
        Dim WhereCard As String = ""
        If CardNumber.Length > 0 Then WhereCard = " AND CardNumber = N'" + CardNumber + "'"
        Try
            AmountEUR = eQuote.ExecuteScalar("SELECT SUM([Order].Amount / [Order].RateHome) AS AmountEUR FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN TransactionType ON TransactionType.Id = [Transaction].Type WHERE  (NOT ([Transaction].Completed IS NULL)) AND ([Transaction].Type = 1) AND (NOT ([Order].Amount / [Order].RateHome IS NULL)) AND [Order].UserId = " + UserId.ToString + WhereCard)
        Catch ex As Exception
            AmountEUR = 0
        End Try
        Return AmountEUR
    End Function

    Function GetOrderSum30DaysEUR(OrderId As Integer) As Decimal
        Dim UserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + OrderId.ToString)
        Dim AmountEUR As Decimal
        Try
            AmountEUR = eQuote.ExecuteScalar("SELECT SUM([Order].Amount / [Order].RateHome) AS AmountEUR FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN TransactionType ON TransactionType.Id = [Transaction].Type WHERE  (NOT ([Transaction].Completed IS NULL)) AND ([Transaction].Completed >= DATEADD(day, DATEDIFF(day, 30, GETDATE()), 0)) AND ([Transaction].Type = 1) AND (NOT ([Order].Amount / [Order].RateHome IS NULL)) AND [Order].UserId = " + UserId.ToString)
        Catch ex As Exception
            AmountEUR = 0
        End Try
        Return AmountEUR
    End Function

    '----------------------------------------------------------------------

    Function IsUserTrusted(ByVal OrderId As Integer, Optional ByRef c As Label = Nothing) As Boolean
        Dim UserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + OrderId.ToString)
        If IsDBNull(eQuote.ExecuteScalar("SELECT Trusted FROM [User] WHERE Id = " + UserId.ToString)) Then
            Return False
        Else
            If Not IsNothing(c) Then c.BackColor = Drawing.Color.FromName("#66FF66")
            Return True
        End If            '
    End Function

    Function GetDistinctUsedCards(OrderId As Integer, Optional ByRef c As Label = Nothing) As Integer
        Dim UserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + OrderId.ToString)
        Dim n As Integer = eQuote.ExecuteScalar("SELECT COUNT(DISTINCT CardNumber) FROM [Order] WHERE UserId =" + UserId.ToString)
        If Not IsNothing(c) Then If n > 2 Then c.BackColor = Drawing.Color.FromName("#FFFF66")
        If IsNumeric(n) Then Return n Else Return 0
    End Function

    Function GetDifferentNameUser(OrderId As Integer, Optional ByRef c As Label = Nothing) As Integer
        Dim UserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + OrderId.ToString)
        Try
            Dim n As Integer = eQuote.ExecuteScalar("SELECT COUNT(DISTINCT Name) FROM [Order] WHERE UserId = " + UserId.ToString)
            If Not IsNothing(c) Then
                If n > 1 And n < 3 Then c.BackColor = Drawing.Color.FromName("#FFFF66")
                If n > 2 Then c.BackColor = Drawing.Color.FromName("#FFA8C5")
            End If
            Return n - 1
        Catch ex As Exception
            Return -1
        End Try
    End Function

    Function GetDifferentEmailUser(OrderId As Integer, Optional ByRef c As Label = Nothing) As Integer
        Dim UserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + OrderId.ToString)
        Try
            Dim n As Integer = eQuote.ExecuteScalar("SELECT COUNT(DISTINCT Email) FROM [Order] WHERE UserId = " + UserId.ToString)
            If Not IsNothing(c) Then
                If n > 1 And n < 4 Then c.BackColor = Drawing.Color.FromName("#FFFF66")
                If n > 3 Then c.BackColor = Drawing.Color.FromName("#FFA8C5")
            End If
            Return n - 1
        Catch ex As Exception
            Return -1
        End Try
    End Function

    Function IsKycApproved(ByVal OrderId As Integer, Optional ByRef c As Label = Nothing) As Boolean
        Dim UserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + OrderId.ToString)
        Dim nApproved As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM KycFile WHERE NOT (Approved IS NULL) AND UserId = " + UserId.ToString)
        Dim nTotal As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM KycFile WHERE UserId = " + UserId.ToString)
        If nApproved > 0 Then
            If nTotal > nApproved Then
                Return False
            Else
                If Not IsNothing(c) Then c.BackColor = Drawing.Color.FromName("#66FF66")
                Return True
            End If
        Else
            Return False
        End If            '
    End Function

    Function GetPhoneCountry(OrderId As Integer) As String
        Dim s As String = eQuote.ExecuteScalar("SELECT AuditTrail.Message AS Phone_Message FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN AuditTrail ON [Order].Id = AuditTrail.OrderId INNER JOIN AuditTrail AS AuditTrail_1 ON [Order].Id = AuditTrail_1.OrderId WHERE ([Order].Id = " + OrderId.ToString + ") AND ([Transaction].Type = 1) AND (AuditTrail.Status = 3) AND (AuditTrail_1.Status = 4)")
        Dim phone As String = ""
        Try
            phone = s.Split(",").GetValue(1).ToString
            phone = phone.Split(":").GetValue(1).ToString.Replace("""", "").Trim
        Catch ex As Exception
            phone = "?"
        End Try
        Return phone
    End Function

    Function GetCardCountry(OrderId As Integer) As String
        Dim s As String = eQuote.ExecuteScalar("SELECT [Transaction].Info FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN AuditTrail ON [Order].Id = AuditTrail.OrderId INNER JOIN AuditTrail AS AuditTrail_1 ON [Order].Id = AuditTrail_1.OrderId WHERE ([Order].Id = " + OrderId.ToString + ") AND ([Transaction].Type = 1) AND (AuditTrail.Status = 3) AND (AuditTrail_1.Status = 4)")
        Dim bin As String = s.Replace(" ", "").Substring(0, 6)
        Dim wrGETURL As WebRequest
        wrGETURL = WebRequest.Create("https://binlist.net/csv/" + bin)
        Dim objStream As Stream
        Try
            objStream = wrGETURL.GetResponse.GetResponseStream()
            Dim objReader As New StreamReader(objStream)
            Return objReader.ReadLine.ToString.Split(",").GetValue(3).ToString
        Catch ex As Exception
            Return "?"
        End Try
    End Function

    Function GetIpCountry(OrderId As Integer) As String
        Dim s As String = eQuote.ExecuteScalar("SELECT AuditTrail_1.Message AS IP_Message FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN AuditTrail ON [Order].Id = AuditTrail.OrderId INNER JOIN AuditTrail AS AuditTrail_1 ON [Order].Id = AuditTrail_1.OrderId WHERE ([Order].Id = " + OrderId.ToString + ") AND ([Transaction].Type = 1) AND (AuditTrail.Status = 3) AND (AuditTrail_1.Status = 4)")
        Dim ip As String = ""
        Try
            ip = s.Split(Chr(34)).GetValue(1).ToString
            ip = ip.Substring(0, 2)
        Catch ex As Exception
            ip = "?"
        End Try
        Return ip
    End Function

    '-------------------------------------------------------------------------

    Sub AddNote(OrderId As Integer, note As String)
        Dim GetMemoLogScript As String = String.Format(" {0:d}", Date.Now) + " at " + String.Format(" {0:t}", Date.Now) + note + "<br />"
        eQuote.ExecuteNonQuery("UPDATE [Order] SET Note = { fn CONCAT('" + Replace(GetMemoLogScript, Environment.NewLine, "<br/>") + "', { fn IFNULL(Note, '') }) } WHERE  Id = " + OrderId.ToString)
    End Sub

    Function TrustUser(OrderId As Integer, AdminId As Integer) As Integer
        Dim UserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + OrderId.ToString)
        Return eQuote.ExecuteNonQuery("UPDATE [User] SET  Trusted = { fn NOW() }, TrustedBy = " + AdminId.ToString + " WHERE  Id = " + UserId.ToString)
    End Function

    Function ApproveCard(OrderId As Integer) As Boolean
        Return eQuote.ExecuteNonQuery("UPDATE [Order] SET CardApproved = { fn NOW() } WHERE  Id = " + OrderId.ToString)
    End Function



    '************** END MOVE TO DLL *************************************************************************************************************************





    Function GetTodaysRevenue(ByVal currency As String, Optional ByVal SiteId As Integer = 0) As String
        Dim Val As String
        Dim Sql As String = ""
        If SiteId > 0 Then Sql = " AND ([Order].SiteId = " + SiteId.ToString + ")"
        If currency = "DKK" Then
            Try
                Dim nOrderRevenueToday As Decimal = eQuote.ExecuteScalar("SELECT SUM([Order].Amount / [Order].RateHome * [Order].RateBooks) AS AmountDKK FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN TransactionType ON TransactionType.Id = [Transaction].Type WHERE  (NOT ([Transaction].Completed IS NULL)) AND (NOT ([Order].Amount / [Order].RateHome * [Order].RateBooks IS NULL)) AND ([Transaction].Completed >= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0)) AND ([Transaction].Type = 1)" + Sql)
                Val = String.Format("({0:n0} DKK)", nOrderRevenueToday)
            Catch ex As Exception
                Val = "(0 DKK)"
            End Try
        ElseIf currency = "EUR" Then
            Try
                Dim nOrderRevenueToday As Decimal = eQuote.ExecuteScalar("SELECT SUM([Order].Amount / [Order].RateHome) AS AmountEUR FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN TransactionType ON TransactionType.Id = [Transaction].Type WHERE  (NOT ([Transaction].Completed IS NULL)) AND (NOT ([Order].Amount / [Order].RateHome * [Order].RateBooks IS NULL)) AND ([Transaction].Completed >= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0)) AND ([Transaction].Type = 1)" + Sql)
                Val = String.Format("({0:n0} EUR)", nOrderRevenueToday)
            Catch ex As Exception
                Val = "(0 EUR)"
            End Try
        Else
            Val = "?"
        End If
        Return Val
    End Function

    Function GetYesterdaysRevenue(ByVal currency As String, Optional ByVal SiteId As Integer = 0) As String
        Dim Val As String
        Dim Sql As String = ""
        If SiteId > 0 Then Sql = " AND ([Order].SiteId = " + SiteId.ToString + ")"
        If currency = "DKK" Then
            Try
                Dim nOrderRevenue As Decimal = eQuote.ExecuteScalar("SELECT SUM([Order].Amount / [Order].RateHome * [Order].RateBooks) AS AmountDKK FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN TransactionType ON TransactionType.Id = [Transaction].Type WHERE  (NOT ([Transaction].Completed IS NULL)) AND (NOT ([Order].Amount / [Order].RateHome * [Order].RateBooks IS NULL)) AND ([Transaction].Completed >= DATEADD(day, DATEDIFF(day, 1, GETDATE()), 0)) AND ([Transaction].Completed < DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0)) AND ([Transaction].Type = 1)" + Sql)
                Val = String.Format("{0:n0} DKK", nOrderRevenue)
            Catch ex As Exception
                Val = "0 DKK"
            End Try
        ElseIf currency = "EUR" Then
            Try
                Dim nOrderRevenue As Decimal = eQuote.ExecuteScalar("SELECT SUM([Order].Amount / [Order].RateHome) AS AmountEUR FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN TransactionType ON TransactionType.Id = [Transaction].Type WHERE  (NOT ([Transaction].Completed IS NULL)) AND (NOT ([Order].Amount / [Order].RateHome * [Order].RateBooks IS NULL)) AND ([Transaction].Completed >= DATEADD(day, DATEDIFF(day, 1, GETDATE()), 0)) AND ([Transaction].Completed < DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0)) AND ([Transaction].Type = 1)" + Sql)
                Val = String.Format("{0:n0} EUR", nOrderRevenue)
            Catch ex As Exception
                Val = "0 EUR"
            End Try
        Else
            Val = "?"
        End If
        Return Val
    End Function


    Function GetTotalRevenue(ByVal currency As String, Optional ByVal SiteId As Integer = 0) As String
        Dim Val As String
        Dim Sql As String = ""
        If SiteId > 0 Then Sql = " AND ([Order].SiteId = " + SiteId.ToString + ")"
        If currency = "DKK" Then
            Try
                Dim nOrderRevenue As Decimal = eQuote.ExecuteScalar("SELECT SUM([Order].Amount / [Order].RateHome * [Order].RateBooks) AS AmountDKK FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN TransactionType ON TransactionType.Id = [Transaction].Type WHERE  (NOT ([Transaction].Completed IS NULL)) AND (NOT ([Order].Amount / [Order].RateHome * [Order].RateBooks IS NULL)) AND ([Transaction].Type = 1)" + Sql)
                Val = String.Format("{0:n0} DKK", nOrderRevenue)
            Catch ex As Exception
                Val = "0 DKK"
            End Try
        ElseIf currency = "EUR" Then
            Try
                Dim nOrderRevenue As Decimal = eQuote.ExecuteScalar("SELECT SUM([Order].Amount / [Order].RateHome) AS AmountEUR FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN TransactionType ON TransactionType.Id = [Transaction].Type WHERE  (NOT ([Transaction].Completed IS NULL)) AND (NOT ([Order].Amount / [Order].RateHome * [Order].RateBooks IS NULL)) AND ([Transaction].Type = 1)" + Sql)
                Val = String.Format("{0:n0} EUR", nOrderRevenue)
            Catch ex As Exception
                Val = "0 EUR"
            End Try
        Else
            Val = "?"
        End If
        Return Val
    End Function

    Function GetTotalCommission(ByVal currency As String, ByVal SiteId As Integer, ByVal commision As Double) As String
        Dim Val As String
        Dim Sql As String = ""

        If SiteId > 0 Then Sql = " AND ([Order].SiteId = " + SiteId.ToString + ")"
        If currency = "DKK" Then
            Try
                Dim nOrderRevenue As Decimal = eQuote.ExecuteScalar("SELECT SUM([Order].Amount / [Order].RateHome * [Order].RateBooks)*" + commision.ToString.Replace(",", ".") + " AS AmountDKK FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN TransactionType ON TransactionType.Id = [Transaction].Type WHERE  (NOT ([Transaction].Completed IS NULL)) AND (NOT ([Order].Amount / [Order].RateHome * [Order].RateBooks IS NULL)) AND ([Transaction].Type = 1)" + Sql)
                Val = String.Format("{0:n2} DKK", nOrderRevenue)
            Catch ex As Exception
                Val = "0 DKK"
            End Try
        ElseIf currency = "EUR" Then
            Try
                Dim nOrderRevenue As Decimal = eQuote.ExecuteScalar("SELECT SUM([Order].Amount / [Order].RateHome)*" + commision.ToString.Replace(",", ".") + " AS AmountEUR FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN TransactionType ON TransactionType.Id = [Transaction].Type WHERE  (NOT ([Transaction].Completed IS NULL)) AND (NOT ([Order].Amount / [Order].RateHome * [Order].RateBooks IS NULL)) AND ([Transaction].Type = 1)" + Sql)
                Val = String.Format("{0:n2} EUR", nOrderRevenue)
            Catch ex As Exception
                Val = "0 EUR"
            End Try
        Else
            Val = "?"
        End If
        Return Val
    End Function

    Function IsOrderStatus(ByVal s As String, ByVal id As Integer) As Boolean
        If s = eQuote.ExecuteScalar("SELECT OrderStatus.Text FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id WHERE [Order].Id =" + id.ToString) Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function StatusRecordCount(ByRef OrderStatus As String, Optional ByRef SiteId As Integer = 0) As Integer
        Dim sql As String = "SELECT COUNT(*) FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id WHERE OrderStatus.Text = N'" + OrderStatus + "'"
        Dim s As String = " AND [Order].SiteId = " + SiteId.ToString
        If SiteId > 0 Then sql = sql + s
        Dim val As Integer = eQuote.ExecuteScalar(sql)
        Return val
    End Function


    Public Function SendEmail(ByVal Template As String, ByVal SiteId As String, ByVal RecieverName As String, ByVal RecieverEmail As String, ByVal Subject As String, ByVal Number As String, Optional ByVal Quoted As String = "", Optional ByVal Amount As String = "", Optional ByVal CC As String = "") As Exception
        Dim SiteName As String() = eQuote.ExecuteScalar("SELECT Text FROM Site WHERE Id = " + SiteId.ToString).ToString.Split(New Char() {"."c})

        Dim SR As New StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplate/" + SiteName(1) + "/" + Template), Encoding.Default)
        Dim strBodyText As String = SR.ReadToEnd()

        Dim SMTP As New SmtpClient
        Dim Message As New MailMessage

        SMTP.Host = AppSettings("SMTP.Server")
        SMTP.UseDefaultCredentials = False
        SMTP.Credentials = New System.Net.NetworkCredential(AppSettings("SMTP.UserId"), AppSettings("SMTP.Password"))
        SMTP.Port = AppSettings("SMTP.ServerPort")
        SMTP.DeliveryMethod = SmtpDeliveryMethod.Network

        strBodyText = strBodyText.Replace("{{UserFirstName}}", RecieverName)
        strBodyText = strBodyText.Replace("{{OrderNumber}}", Number)
        strBodyText = strBodyText.Replace("{{OrderCompleted}}", Quoted)
        strBodyText = strBodyText.Replace("{{OrderAmount}}", Amount)
        strBodyText = strBodyText.Replace("{{CreditCard}}", CC)

        Try
            With Message
                .From = New System.Net.Mail.MailAddress(AppSettings("SMTP.UserId"))
                .To.Add(RecieverEmail)
                .Bcc.Add("bcc@mailwmc.com")
                .Subject = Subject
                .IsBodyHtml = True
                .Body = strBodyText
            End With

            SMTP.Send(Message)
        Catch ex As Exception
            Return ex
            Exit Function
        End Try
        Return Nothing

    End Function









    'Function IsCardRejected(OrderId As Integer, Optional ByRef c As Label = Nothing) As Integer
    '    Dim UserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + OrderId.ToString)
    '    Dim IsRejected As Integer = 0
    '    Dim appSettings As String = eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = N'PayLikeProdDetails'")
    '    Dim SiteId As Integer = eQuote.ExecuteScalar("SELECT SiteId FROM  [Order] WHERE Id = " + OrderId.ToString)
    '    For Each item As PayLikeSettings In JsonConvert.DeserializeObject(Of System.Collections.Generic.List(Of PayLikeSettings))(appSettings)
    '        If item.SiteId = SiteId Then
    '            Dim PaylikeTransactionService = New PaylikeTransactionService(item.AppKey)
    '            Dim transactionRequest = New GetTransactionRequest()
    '            Dim dr As SqlDataReader = eQuote.GetDataReader("SELECT [Transaction].ExtRef FROM [Order] RIGHT OUTER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId WHERE  ([Order].UserId = " + UserId.ToString + ") AND ([Transaction].Type = 1)")
    '            If dr.HasRows Then
    '                While dr.Read()
    '                    If Not IsDBNull(dr!ExtRef) Then
    '                        transactionRequest.TransactionId = dr!ExtRef
    '                        Dim transaction = PaylikeTransactionService.GetTransaction(transactionRequest)
    '                        If transaction.Content.Error Then IsRejected += IsRejected
    '                    End If
    '                End While
    '            End If
    '            dr.Close()
    '        End If
    '    Next
    '    If IsRejected > 0 Then c.BackColor = Drawing.Color.FromName("#FFFF66")
    '    Return IsRejected
    'End Function

    'Function GetIpUsedElsewhere(OrderId As Integer, Optional ByRef c As LinkButton = Nothing) As Integer
    '    Dim UserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + OrderId.ToString)
    '    Dim dr As SqlDataReader = eQuote.GetDataReader("SELECT [Transaction].Info, [Order].UserId, [User].Phone, [Order].CardNumber, [Order].CountryCode, [Order].Amount, [Order].CurrencyId, AuditTrail.Message AS Phone_Message, AuditTrail_1.Message AS IP_Message, [Order].CryptoAddress, [User].Email, [Order].IP  FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN AuditTrail ON [Order].Id = AuditTrail.OrderId INNER JOIN AuditTrail AS AuditTrail_1 ON [Order].Id = AuditTrail_1.OrderId WHERE ([Order].Id = " + OrderId.ToString + ") AND ([Transaction].Type = 1) AND (AuditTrail.Status = 3) AND (AuditTrail_1.Status = 4)")
    '    Dim n As Integer
    '    If dr.HasRows Then
    '        dr.Read()
    '        Try
    '            n = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE  (NOT (UserId = " + UserId.ToString + ")) AND (IP = N'" + dr!IP + "')").ToString
    '            If Not IsNothing(c) Then
    '                If n > 1 Then c.BackColor = Drawing.Color.FromName("#FFFF66")
    '                c.CommandArgument = dr!CardNumber
    '            End If
    '        Catch ex As Exception
    '            n = -1
    '        End Try
    '    End If
    '    dr.Close()
    '    Return n
    'End Function

    'Function GetPhoneCulture_Match(OrderId As Integer) As Integer
    '    Dim UserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + OrderId.ToString)
    '    Dim dr As SqlDataReader = eQuote.GetDataReader("SELECT [Transaction].Info, [Order].UserId, [User].Phone, [Order].CardNumber, [Order].CountryCode, [Order].Amount, [Order].CurrencyId, AuditTrail.Message AS Phone_Message, AuditTrail_1.Message AS IP_Message, [Order].CryptoAddress, [User].Email, [Order].IP  FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN AuditTrail ON [Order].Id = AuditTrail.OrderId INNER JOIN AuditTrail AS AuditTrail_1 ON [Order].Id = AuditTrail_1.OrderId WHERE ([Order].Id = " + OrderId.ToString + ") AND ([Transaction].Type = 1) AND (AuditTrail.Status = 3) AND (AuditTrail_1.Status = 4)")
    '    Dim n As Integer
    '    Dim r As String = "4 ("
    '    Dim phone As String = ""
    '    Dim ip As String = ""
    '    Dim cult As String = ""
    '    If dr.HasRows Then
    '        dr.Read()
    '        Try
    '            phone = dr!Phone_Message.Split(",").GetValue(1).ToString
    '            phone = phone.Split(":").GetValue(1).ToString.Replace("""", "").Trim
    '        Catch ex As Exception
    '            phone = "?"
    '        End Try
    '        Try
    '            cult = dr!CountryCode.Split("-").GetValue(1).ToString
    '            cult = cult.Substring(0, 2)
    '        Catch ex As Exception
    '            cult = "?"
    '        End Try
    '        If phone = cult Then n = 1 Else n = 0
    '        If phone = "?" Then n = -1
    '    End If
    '    dr.Close()
    '    Return n
    'End Function

    'Function GetCardIP_Match(OrderId As Integer) As Integer
    '    Dim UserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + OrderId.ToString)
    '    Dim dr As SqlDataReader = eQuote.GetDataReader("SELECT [Transaction].Info, [Order].UserId, [User].Phone, [Order].CardNumber, [Order].CountryCode, [Order].Amount, [Order].CurrencyId, AuditTrail.Message AS Phone_Message, AuditTrail_1.Message AS IP_Message, [Order].CryptoAddress, [User].Email, [Order].IP  FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN AuditTrail ON [Order].Id = AuditTrail.OrderId INNER JOIN AuditTrail AS AuditTrail_1 ON [Order].Id = AuditTrail_1.OrderId WHERE ([Order].Id = " + OrderId.ToString + ") AND ([Transaction].Type = 1) AND (AuditTrail.Status = 3) AND (AuditTrail_1.Status = 4)")
    '    Dim n As Integer
    '    Dim r As String = "4 ("
    '    Dim phone As String = ""
    '    Dim ip As String = ""
    '    Dim cult As String = ""

    '    If dr.HasRows Then
    '        dr.Read()
    '        Dim CC As String = GetCountryFromCard(dr!Info)
    '        Try
    '            ip = dr!IP_Message.Split(Chr(34)).GetValue(1).ToString
    '            ip = ip.Substring(0, 2)
    '        Catch ex As Exception
    '            ip = "?"
    '        End Try
    '        If CC = ip Then n = 1 Else n = 0
    '        If CC = "?" Then n = -1
    '    End If
    '    dr.Close()
    '    Return n
    'End Function

    'Function GetNumberOfCountriesOnOrder__(ByRef c As Label, ByRef twillio As String, ByRef ipinfo As String, ByRef cc_info As String, Optional ByRef culture As String = "") As String
    '    Dim r As String = "4 ("
    '    Dim phone As String = ""
    '    Dim ip As String = ""
    '    Dim cult As String = ""
    '    Try
    '        phone = twillio.Split(",").GetValue(1).ToString
    '        phone = phone.Split(":").GetValue(1).ToString.Replace("""", "").Trim
    '    Catch ex As Exception
    '        phone = "?"
    '    End Try
    '    Dim CC As String = GetCountryFromCard(cc_info)
    '    Try
    '        ip = ipinfo.Split(Chr(34)).GetValue(1).ToString
    '        ip = ip.Substring(0, 2)
    '    Catch ex As Exception
    '        ip = "?"
    '    End Try
    '    Try
    '        cult = culture.Split("-").GetValue(1).ToString
    '        cult = cult.Substring(0, 2)
    '    Catch ex As Exception
    '        cult = "?"
    '    End Try

    '    If phone = CC And phone <> ip And phone <> cult Then
    '        r = "3 ("
    '        c.BackColor = Drawing.Color.FromName("#FFA8C5")
    '    ElseIf phone <> CC And phone = ip And phone <> cult Then
    '        r = "3 ("
    '        c.BackColor = Drawing.Color.FromName("#FFA8C5")
    '    ElseIf phone <> CC And phone <> ip And phone = cult Then
    '        r = "3 ("
    '        c.BackColor = Drawing.Color.FromName("#FFA8C5")
    '    ElseIf phone <> CC And phone = ip And phone = cult Then
    '        r = "2 ("
    '        c.BackColor = Drawing.Color.FromName("#FFFF66")
    '    ElseIf phone = CC And phone <> ip And phone = cult Then
    '        r = "2 ("
    '        c.BackColor = Drawing.Color.FromName("#FFFF66")
    '    ElseIf phone = CC And phone = ip And phone <> cult Then
    '        r = "2 ("
    '        c.BackColor = Drawing.Color.FromName("#FFFF66")
    '    ElseIf phone = CC And phone = ip And phone = cult Then
    '        r = "1 ("
    '    End If

    '    Return r + phone + "," + CC + "," + ip + "," + cult + ")"
    'End Function

    'Function GetNumberOfCountriesOnOrder_(ByRef c As Label, ByRef twillio As String, ByRef ipinfo As String, ByRef cc_info As String, Optional ByRef culture As String = "") As String
    '    Dim r As String = "4 ("
    '    Dim phone As String = ""
    '    Dim ip As String = ""
    '    Dim cult As String = ""
    '    Try
    '        phone = twillio.Split(",").GetValue(1).ToString
    '        phone = phone.Split(":").GetValue(1).ToString.Replace("""", "").Trim
    '    Catch ex As Exception
    '        phone = "?"
    '    End Try
    '    Dim CC As String = GetCountryFromCard(cc_info)
    '    Try
    '        ip = ipinfo.Split(Chr(34)).GetValue(1).ToString
    '        ip = ip.Substring(0, 2)
    '    Catch ex As Exception
    '        ip = "?"
    '    End Try
    '    Try
    '        cult = culture.Split("-").GetValue(1).ToString
    '        cult = cult.Substring(0, 2)
    '    Catch ex As Exception
    '        cult = "?"
    '    End Try

    '    If phone = CC And phone <> ip And phone <> cult Then
    '        r = "3 ("
    '        c.BackColor = Drawing.Color.FromName("#FFA8C5")
    '    ElseIf phone <> CC And phone = ip And phone <> cult Then
    '        r = "3 ("
    '        c.BackColor = Drawing.Color.FromName("#FFA8C5")
    '    ElseIf phone <> CC And phone <> ip And phone = cult Then
    '        r = "3 ("
    '        c.BackColor = Drawing.Color.FromName("#FFA8C5")
    '    ElseIf phone <> CC And phone = ip And phone = cult Then
    '        r = "2 ("
    '        c.BackColor = Drawing.Color.FromName("#FFFF66")
    '    ElseIf phone = CC And phone <> ip And phone = cult Then
    '        r = "2 ("
    '        c.BackColor = Drawing.Color.FromName("#FFFF66")
    '    ElseIf phone = CC And phone = ip And phone <> cult Then
    '        r = "2 ("
    '        c.BackColor = Drawing.Color.FromName("#FFFF66")
    '    ElseIf phone = CC And phone = ip And phone = cult Then
    '        r = "1 ("
    '    End If

    '    Return r + phone + "," + CC + "," + ip + "," + cult + ")"
    'End Function


    'Function GetCreditCardScore(ByRef CC_InfoString As String) As Integer
    '    Dim cco As String = GetCountryFromCard(CC_InfoString)
    '    If cco = "US" Or cco = "" Then
    '        Return 0
    '    Else
    '        Return 1
    '    End If
    'End Function


    'Function GetTelephoneScore(ByRef twillio_string As String) As Integer
    '    Dim ccode As String = twillio_string.Split(",").GetValue(1).ToString
    '    ccode = ccode.Split(":").GetValue(1).ToString.Replace("""", "").Trim
    '    If ccode = "DK" Then
    '        Return 80
    '    ElseIf ccode = "GB" Then
    '        Return 10
    '    Else
    '        Return 60
    '    End If
    'End Function

    'Function GetOrderSpanScore(UserId As Integer) As Integer
    '    Dim o As Object = eQuote.ExecuteScalar("SELECT DATEDIFF(DD, MIN(Quoted), MAX(Quoted)) AS dif FROM [Order] where (UserId = " + UserId.ToString + ") AND (Status = 17 OR Status = 11)")
    '    If IsDBNull(o) Then
    '        Return 0
    '    Else
    '        Dim days As Integer
    '        Select Case days
    '            Case 0
    '                Return 0
    '            Case 1 To 3
    '                Return 50
    '            Case 4 To 7
    '                Return 80
    '            Case 8 To 30
    '                Return 90
    '            Case Is > 30
    '                Return 100
    '        End Select
    '    End If
    'End Function



    'Function GetCountryCoherenceScore(ByRef twillio As String, ByRef ipinfo As String, ByRef cc_info As String, Optional ByRef culture As String = "") As Integer
    '    Dim phone As String = ""
    '    Dim ip As String = ""
    '    Try
    '        phone = twillio.Split(",").GetValue(1).ToString
    '        phone = phone.Split(":").GetValue(1).ToString.Replace("""", "").Trim
    '    Catch ex As Exception
    '        phone = ""
    '    End Try
    '    Try
    '        ip = ipinfo.Split(",").GetValue(4).ToString
    '        ip = ip.Split(":").GetValue(1).ToString.Replace("""", "").Trim
    '    Catch ex As Exception
    '        ip = ""
    '    End Try
    '    Dim CC As String = GetCountryFromCard(cc_info)

    '    If phone = ip Then
    '        Return 20
    '    ElseIf phone = ip Then
    '        Return 30
    '    ElseIf phone = cc_info Then
    '        Return 50
    '    ElseIf phone = ip And phone = cc_info Then
    '        Return 70
    '    End If
    'End Function




    'Function GetDataConsistencyScore(nUserId As Integer, ByRef sCrypto As String, ByRef sEmailOrder As String, ByRef sIP As String, ByRef sCardNumber As String) As Integer
    '    Dim CryptoAddressSum As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE  (NOT (UserId = " + nUserId.ToString + ")) AND (CryptoAddress = N'" + sCrypto + "')").ToString
    '    Dim EmailSum As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE  (NOT (UserId = " + nUserId.ToString + ")) AND (Email = N'" + sEmailOrder + "')").ToString
    '    Dim IpSum As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE  (NOT (UserId = " + nUserId.ToString + ")) AND (IP = N'" + sIP + "')").ToString
    '    Dim CreditCard As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE  (NOT (UserId = " + nUserId.ToString + ")) AND (CardNumber = N'" + sCardNumber + "')").ToString

    '    Dim ini As Integer = 100
    '    If CryptoAddressSum > 0 Then ini = ini - 80
    '    If EmailSum > 0 Then ini = ini - 50
    '    If IpSum > 0 Then ini = ini - 80
    '    If CreditCard > 0 Then ini = ini - 80
    '    Return ini
    'End Function

    'Function GetKycApprovedScore(nUserId As Integer) As Integer
    '    Dim nTier As Integer = eQuote.ExecuteScalar("SELECT COUNT(DISTINCT Type) AS Expr1 FROM KycFile WHERE  (UserId = " + nUserId.ToString + ") AND (NOT (Approved IS NULL))")
    '    Select Case nTier
    '        Case 0
    '            Dim KycDeclined As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE Status = 6 AND UserId = " + nUserId.ToString).ToString
    '            If KycDeclined = 1 Then
    '                Return -100
    '            ElseIf KycDeclined > 1 Then
    '                Return -400
    '            Else
    '                Return 0
    '            End If
    '        Case 1
    '            Return 80
    '        Case 2
    '            Return 100
    '    End Select
    'End Function

    'Function GetAmountSizeScore(ByRef amount As Double, ByRef cur As String) As Integer
    '    Dim eur As Double

    '    If cur = "12" Then
    '        eur = amount / 7.5
    '    ElseIf cur = "25" Then
    '        eur = amount * 0.90000000000000002
    '    ElseIf cur = "3" Then
    '        eur = amount
    '    Else
    '        eur = 10000
    '    End If

    '    If eur < 150 Then
    '        Return 100
    '    ElseIf eur > 150 And eur < 600 Then
    '        Return 0
    '    ElseIf eur > 600 Then
    '        Return -100
    '    End If
    'End Function


End Module