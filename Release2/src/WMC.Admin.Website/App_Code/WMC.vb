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
Imports WMC.Logic
Imports System.Collections.Generic
Imports WMC.Utilities
Imports System.Linq
Imports WMC.Logic.Models

Namespace WMC

    Public Class SecurePage
        Inherits System.Web.UI.Page

        Protected Overrides Sub OnInit(e As EventArgs)
            MyBase.OnInit(e)
            '' validate authenticity
            Dim authAttr As PageAuthAttribute = CType(Attribute.GetCustomAttribute(Me.GetType(), GetType(PageAuthAttribute)), PageAuthAttribute)
            If Not authAttr Is Nothing Then
                If Not authAttr.Anonymous Then
                    '' not anonymous
                    If IsNumeric(Session("UserId")) Then
                        Dim userid = Long.Parse(Session("UserId"))
                        Dim role = Session("UserRole").ToString()
                        If Not authAttr.Role.Contains(role) Then
                            '' role not matched
                            Response.Redirect("Default.aspx")
                        End If
                    Else
                        Response.Redirect("Default.aspx")
                    End If

                End If
            Else
                '' Auth Attribute is must
                Response.Redirect("Default.aspx")
            End If
        End Sub
    End Class

    <AttributeUsage(AttributeTargets.Class)>
    Public Class PageAuthAttribute
        Inherits Attribute

        Sub New(authRequire As Boolean, ParamArray roles As String())
            Anonymous = Not authRequire
            Role = roles
        End Sub

        Public Property Role() As String()
        Public Property Anonymous() As Boolean

    End Class


    Public Module WMC
        Function TrustCalculationExecutionLabel(ByVal RiskScore As Decimal) As String
            Dim result As String = ""
            If RiskScore = 0 Then
                result = "DECLINED"
            ElseIf RiskScore > 0 And RiskScore < 0.2 Then
                result = "TX-SECRET REQUEST"
            ElseIf RiskScore >= 0.2 And RiskScore < 0.9 Then
                result = "COMPLIANCE REVIEW"
            ElseIf RiskScore >= 0.9 Then
                result = "AUTO APPROVED"
            End If
            Return result
        End Function


        '************** START MOVE TO DLL *************************************************************************************************************************


        Function TrustCalculationExecution(ByVal OrderId As Integer, ByVal ActualOrderStatus As String, ByVal RiskScore As Decimal) As Boolean
            Dim status As Integer = 0
            Dim result As Boolean = True
            Dim message As String
            If IsOrderStatus(ActualOrderStatus, OrderId) Then
                If RiskScore = 0 Then
                    Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'KYC Decline'")
                    eQuote.ExecuteNonQuery("UPDATE [Order] SET RiskScore = " + RiskScore.ToString.Replace(",", ".") + ", Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
                ElseIf RiskScore > 0 And RiskScore < 0.2 Then
                    Dim dr As SqlDataReader = eQuote.GetDataReader("SELECT TOP (1) [Order].Id, [Order].Number, [User].Fname, [Order].Amount, Currency.Code, [Order].Quoted, [Order].CryptoAddress, [Order].Name, [Order].Email, [Order].BccAddress, [Order].IP,  OrderStatus.Text AS Status, [Order].Note, [Order].Rate, [Order].CommissionProcent, [Order].CardNumber, [Order].SiteId, [Site].[Text] as SiteText, SUBSTRING([Order].CountryCode, 1, 5) AS BrowserCulture, [Order].TxSecret FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN [Site] ON Site.Id = [Order].SiteId INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN [Order] AS Order_1 ON [User].Id = Order_1.UserId WHERE ([Order].Status = 4 OR [Order].Status = 17 OR [Order].Status = 21 OR [Order].Status = 22) AND (Order_1.Id = " + OrderId.ToString + ") AND (NOT ([Order].TxSecret IS NULL)) AND (NOT ([Order].CardNumber IS NULL)) ORDER BY [Order].Quoted")
                    If dr.HasRows Then
                        dr.Read()
                        'EmailHelper.SendEmail(dr!Email, "RequestTxSecret", New Dictionary(Of String, Object) From
                        '    {
                        '        {"UserIdentity", dr!CreditCardUserIdentity},
                        '        {"SiteName", dr!SiteText},
                        '        {"UserFirstName", dr!Name},
                        '        {"OrderNumber", dr!Number},
                        '        {"OrderCompleted", dr!Quoted},
                        '        {"OrderAmount", dr!Amount},
                        '        {"OrderCurrency", dr!Code},
                        '        {"CreditCard", dr!CardNumber},
                        '    }, dr!SiteText, dr!BccAddress)
                        Dim ex As Exception = SendEmail("RequestTxSecret", dr!SiteId, dr!Name, dr!Email, dr!Number, String.Format("{0:d/M/yyyy}", dr!Quoted), String.Format("{0:n0}", dr!Amount) + " " + dr!Code, dr!CardNumber)
                        status = eQuote.ExecuteScalar("SELECT Id FROM AuditTrailStatus WHERE Text = N'SentEmail'")
                        message = "RequestTxSecret.htm" + vbCrLf + "Number:" + dr!Number + ", TxSecret:" + dr!TxSecret
                        eQuote.ExecuteNonQuery("INSERT INTO AuditTrail(OrderId, Status, Message, Created) VALUES (" + OrderId.ToString + ", " + status.ToString + ", N'" + message + "', { fn NOW() })")
                        status = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Customer Response Pending'")
                        eQuote.ExecuteNonQuery("UPDATE [Order] SET RiskScore = " + RiskScore.ToString.Replace(",", ".") + ", Status = " + status.ToString + " WHERE (Id = " + OrderId.ToString + ")")
                        AddNote(OrderId, " Tx-Secret Requested")
                    Else
                        AddNote(OrderId, " NOT SUCCESSFULL Tx-Secret Request")
                        result = False
                    End If
                    dr.Close()

                ElseIf RiskScore >= 0.2 And RiskScore < 0.9 Then
                    Dim dr As SqlDataReader = eQuote.GetDataReader("SELECT [Order].Id, [Order].Number, [Order].SiteId, [Order].Amount, Currency.Code, [Order].Quoted, [Order].Name, [Order].Email, [Order].CardNumber, [Order].TxSecret FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN [User] ON [Order].UserId = [User].Id WHERE [Order].Id =" + OrderId.ToString)
                    If dr.HasRows Then
                        If dr.HasRows Then
                            dr.Read()
                            If eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'OperationalStatus'") = "Away" Then
                                SendEmail("OrderComplianceInspection", dr!SiteId, dr!Name, dr!Email, dr!Number)
                            End If
                            status = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Compliance Officer Approval'")
                            eQuote.ExecuteNonQuery("UPDATE [Order] SET RiskScore = " + RiskScore.ToString.Replace(",", ".") + ", Status = " + status.ToString + " WHERE (Id = " + OrderId.ToString + ")")
                        Else
                            AddNote(OrderId, " NOT SUCCESSFULL OrderComplianceInspection Notification")
                            result = False
                        End If
                    End If
                    dr.Close()

                ElseIf RiskScore >= 0.9 Then
                    Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Payout approved'")
                    eQuote.ExecuteNonQuery("UPDATE [Order] SET RiskScore = " + RiskScore.ToString.Replace(",", ".") + ", Approved = { fn NOW() }, Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
                End If
            End If
            Return result
        End Function


        Function TrustLevelCalculation(ByVal OrderId As Integer, Optional ByRef c As Label = Nothing) As Decimal
            Dim UserIsTrusted As Boolean = IsUserTrusted(OrderId)
            Dim AdminIsAway As Boolean = eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'ComplianceAvailability'") = "Away"
            Dim CardNumber As String = eQuote.ExecuteScalar("SELECT CardNumber FROM [Order] WHERE Id =" + OrderId.ToString).ToString.Trim
            Dim CardIsApproved As Boolean = IsCardApproved(OrderId)
            Dim PhoneIsVirtual As Boolean = IsPhoneVitual(OrderId)
            Dim CardUsedElsewhere As Integer = GetCardUsedElsewhere(OrderId)
            Dim EmailUsedElsewhere As Integer = GetEmailUsedElsewhere(OrderId)
            Dim IpUsedElsewhere As Integer = 0 'GetIpUsedElsewhere(OrderId)
            Dim CountryTrust As String = GetCountryTrustLevel(OrderId)
            Dim SiteTrust As String = GetSiteTrustLevel(OrderId)
            Dim PhoneCountry As String = GetPhoneCountry(OrderId)
            Dim TxLimitEUR As Integer = CInt(eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'TxLimitEUR'"))
            Dim OrderSumEUR As Decimal = GetOrderSumEUR(OrderId)
            Dim OrderSumTotalEUR As Decimal = GetOrderSumTotalEUR(OrderId, CardNumber)
            Dim status As Integer = 0
            Dim message As String = ""
            Dim RiskScore As Decimal = 0

            If IsCardUS(OrderId) Then
                'KYC DECLINE(24)
                message = "Decline - US Card"
                RiskScore = 0
                If Not IsNothing(c) Then c.BackColor = Drawing.Color.FromName("#FFA8C5") : c.Text = message
            Else
                If (Not CardIsApproved And ((OrderSumEUR + OrderSumTotalEUR) > TxLimitEUR * CountryTrust * SiteTrust)) Then
                    'TX SECRET -> CUSTOMER RESPONSE PENDING(22)
                    If PhoneCountry = "DK" And Not AdminIsAway Then
                        message = "(DK) Manual Approval: Tx Limit is Exceeded "
                        RiskScore = 0.2
                        If Not IsNothing(c) Then c.BackColor = Drawing.Color.FromName("#FFFF66") : c.Text = message
                    Else
                        RiskScore = 0.1
                        message = "Ask for Tx Secret"
                        If Not IsNothing(c) Then c.BackColor = Drawing.Color.LightYellow : c.Text = message
                    End If
                Else
                    If PhoneIsVirtual Or (CardUsedElsewhere > 0 Or EmailUsedElsewhere > 0 Or IpUsedElsewhere > 0) Or (Not PhoneCardOrigin_Match(OrderId)) Or (Not PhoneIP_Match(OrderId)) Then
                        If PhoneIsVirtual Then message += "Virtual Number, "
                        If (CardUsedElsewhere > 0 Or EmailUsedElsewhere > 0 Or IpUsedElsewhere > 0) Then message += "Data Used Elsewhere, "
                        If Not PhoneCardOrigin_Match(OrderId) Then message += "Phone/Card Mismatch, "
                        If Not PhoneIP_Match(OrderId) Then message += "  - Phone/IP Mismatch, "
                        If UserIsTrusted Then
                            'PAYOUT APPROVED(18)
                            message = "Approved although:" + message
                            RiskScore = 0.9
                            If Not IsNothing(c) Then c.BackColor = Drawing.Color.FromName("#66FF66") : c.Text = message
                        Else
                            'COMPLIANCE OFFICER APPROVAL(21)
                            message = "Manual Approval: " + message
                            RiskScore = 0.2
                            If Not IsNothing(c) Then c.BackColor = Drawing.Color.FromName("#FFFF66") : c.Text = message
                        End If
                    Else
                        'PAYOUT APPROVED(18)
                        message = "Approved"
                        RiskScore = 1
                        If Not IsNothing(c) Then c.BackColor = Drawing.Color.FromName("#66FF66") : c.Text = message
                    End If
                End If
            End If
            'status = eQuote.ExecuteScalar("SELECT Id FROM AuditTrailStatus WHERE Text = N'TrustLogic'")
            'eQuote.ExecuteNonQuery("INSERT INTO AuditTrail(OrderId, Status, Message, Created) VALUES (" + OrderId.ToString + ", " + status.ToString + ", N'" + message + "', { fn NOW() })")
            Return RiskScore
        End Function

        '#FFA8C5 = RED
        '#FFFF66 = YELLOW
        '#66FF66 = GREEN

        Function IsCardApproved(OrderId As Integer, Optional ByRef c As Label = Nothing) As Boolean
            Dim CardNumber As Object = eQuote.ExecuteScalar("SELECT CardNumber FROM [Order] WHERE Id =" + OrderId.ToString)
            If IsDBNull(CardNumber) Then Return False
            Dim n As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE CardNumber = N'" + CardNumber.ToString + "' AND NOT (CardApproved IS NULL)")
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

        Function IsPhoneVitual(OrderId As Integer, Optional ByRef c As Label = Nothing) As Boolean
            Dim names As Integer = GetDifferentNameUser(OrderId)
            Dim emails As Integer = GetDifferentEmailUser(OrderId)
            If (names > 1 And emails > 1) Or names > 2 Or emails > 2 Then
                If Not IsNothing(c) Then c.BackColor = Drawing.Color.FromName("#FFA8C5")
                Return True
            Else
                Return False
            End If
        End Function

        Function PhoneCardOrigin_Match(OrderId As Integer) As Boolean
            If GetPhoneCountry(OrderId) = GetCardCountry(OrderId) Or GetCardCountry(OrderId) = "GB" Then Return True Else Return False
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

        Function GetIpUsedElsewhere(OrderId As Integer, Optional ByRef c As LinkButton = Nothing) As Integer
            Dim UserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + OrderId.ToString)
            Dim dr As SqlDataReader = eQuote.GetDataReader("SELECT [Transaction].Info, [Order].UserId, [User].Phone, [Order].CardNumber, [Order].CountryCode, [Order].Amount, [Order].CurrencyId, AuditTrail.Message AS Phone_Message, AuditTrail_1.Message AS IP_Message, [Order].CryptoAddress, [User].Email, [Order].IP  FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN AuditTrail ON [Order].Id = AuditTrail.OrderId INNER JOIN AuditTrail AS AuditTrail_1 ON [Order].Id = AuditTrail_1.OrderId WHERE ([Order].Id = " + OrderId.ToString + ") AND ([Transaction].Type = 1) AND (AuditTrail.Status = 3) AND (AuditTrail_1.Status = 4)")
            Dim n As Integer
            If dr.HasRows Then
                dr.Read()
                Try
                    n = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE  (NOT (UserId = " + UserId.ToString + ")) AND (IP = N'" + dr!IP + "')").ToString
                    If Not IsNothing(c) Then
                        If n > 1 Then c.BackColor = Drawing.Color.FromName("#FFFF66")
                        c.CommandArgument = dr!IP
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
                If Not IsNothing(c) Then c.BackColor = Drawing.Color.FromName("#FFA8C5")
                Return True
            Else
                Return False
            End If
        End Function

        '--------------------------------------------------------------------------------------------------

        Function GetChainalysisWithdrawalAddress(ByVal CryptoAddress As String, ByVal UserId As Integer, ByVal OrderId As Integer, Optional ByRef c As Label = Nothing) As Integer
            Dim wd As WithDrawalAddressAndScore = ChainalysisInterface.GetWithdrawalAddress(UserId.ToString, CryptoAddress)
            If Not IsNothing(wd) Then
                c.Text = wd.score
                If wd.score = "red" Then c.BackColor = Drawing.Color.FromName("#FFA8C5")
                If wd.score = "amber" Then c.BackColor = Drawing.Color.FromName("#ffbf00")
                If wd.score = "green" Then c.BackColor = Drawing.Color.FromName("#66FF66")
            Else
                c.Text = "no data :-("
            End If

            '#FFA8C5 = RED
            '#ffbf00 = AMBER
            '#66FF66 = GREEN
            Return 0
        End Function

        Function GetOrdersByStatus(ByVal UserId As Integer, ByVal OrderId As Integer, Status As String) As Integer
            Dim n As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id WHERE OrderStatus.Text = N'" + Status + "' AND [Order].UserId = " + UserId.ToString).ToString
            'If Not IsNothing(c) Then
            '    If n = 0 Then c.BackColor = Drawing.Color.FromName("#FFFF66")
            'End If
            If IsNumeric(n) Then Return n Else Return 0
        End Function

        Function GetOrderSpan(OrderId As Integer, Optional CardNumber As String = "", Optional ByRef c As Label = Nothing) As Integer
            Dim Obj As Object
            If CardNumber.Length > 0 Then
                Obj = eQuote.ExecuteScalar("SELECT DATEDIFF(DD, MIN(Quoted), MAX(Quoted)) AS dif FROM [Order] WHERE (Status = 17 OR Status = 4 OR Status = 21 OR Status = 22) AND CardNumber = N'" + CardNumber + "'")
            Else
                Dim UserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + OrderId.ToString)
                Obj = eQuote.ExecuteScalar("SELECT DATEDIFF(DD, MIN(Quoted), MAX(Quoted)) AS dif FROM [Order] WHERE (UserId = " + UserId.ToString + ") AND (Status = 17  OR Status = 4 OR Status = 21 OR Status = 22)")
            End If
            If Not IsNothing(c) Then
                If Not IsDBNull(Obj) AndAlso CInt(Obj) > 45 Then c.BackColor = Drawing.Color.FromName("#66FF66")
            End If
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
                    Case "NOK"
                        AmountEUR = LocalAmount / 9.16749086
                    Case "FIM"
                        AmountEUR = LocalAmount / 0.168188
                    Case "USD"
                        AmountEUR = LocalAmount / 6.3
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
                AmountEUR = eQuote.ExecuteScalar("SELECT SUM([Order].Amount / [Order].RateHome) AS AmountEUR FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN TransactionType  ON TransactionType.Id = [Transaction].Type INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id WHERE  (NOT ([Transaction].Completed IS NULL)) AND ([Transaction].Type = 1) AND (NOT ([Order].Amount / [Order].RateHome IS NULL)) AND (OrderStatus.Text = N'Completed') AND [Order].UserId = " + UserId.ToString + WhereCard)
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

        Function GetDistinctUsedCards(UserId As Integer, OrderId As Integer, Optional ByRef c As Label = Nothing) As Integer
            Dim n As Integer = eQuote.ExecuteScalar("SELECT COUNT(DISTINCT CardNumber) FROM [Order] WHERE UserId =" + UserId.ToString)
            If Not IsNothing(c) Then If n > 5 Then c.BackColor = Drawing.Color.FromName("#FFFF66")
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

        Function IsKycApproved(ByVal UserId As Integer, ByVal OrderId As Integer, Optional ByRef c As Label = Nothing) As Boolean
            Dim nApproved As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM KycFile INNER JOIN KycType ON KycFile.Type = KycType.Id WHERE NOT (Approved IS NULL) AND UserId = " + UserId.ToString + " AND (KycType.Text = N'ProofOfRecidency' OR KycType.Text = N'PhotoID') ")
            Dim nTotal As Integer = eQuote.ExecuteScalar("SELECT COUNT(*)FROM KycFile INNER JOIN KycType ON KycFile.Type = KycType.Id WHERE (KycType.Text = N'ProofOfRecidency' OR KycType.Text = N'PhotoID') AND UserId = " + UserId.ToString)
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
            Dim phoneCode As String = eQuote.ExecuteScalar("SELECT Country.Code FROM Country INNER JOIN [User] ON Country.Id = [User].CountryId INNER JOIN [Order] ON [User].Id = [Order].UserId WHERE [Order].Id = " + OrderId.ToString)
            Return phoneCode
        End Function

        Sub LockBitGo()

            Dim bitGoAccessCode = SettingsManager.GetDefault.Get("BitGoAccessCode", True).GetJsonData(Of BitGoAccessSettings)
            Dim BitGoToken = bitGoAccessCode.AccessCode

            Dim wrPOSTURL As WebRequest

            wrPOSTURL = WebRequest.Create("https://www.bitgo.com/api/v1/user/lock")
            wrPOSTURL.Method = "POST"
            wrPOSTURL.Headers.Add("Authorization", "Bearer " + BitGoToken)
            Try
                wrPOSTURL.Timeout = 1000
                Dim response As WebResponse = wrPOSTURL.GetResponse()
            Catch ex As Exception
            End Try

        End Sub


        Function GetCardCountry(OrderId As Integer) As String
            Dim Result As Object = eQuote.ExecuteScalar("SELECT [Transaction].Info FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN AuditTrail ON [Order].Id = AuditTrail.OrderId INNER JOIN AuditTrail AS AuditTrail_1 ON [Order].Id = AuditTrail_1.OrderId WHERE ([Order].Id = " + OrderId.ToString + ") AND ([Transaction].Type = 1) AND (AuditTrail.Status = 3) AND (AuditTrail_1.Status = 4)")
            If IsDBNull(Result) Or IsNothing(Result) Then Return "?"
            Dim bin As String = Result.ToString.Replace(" ", "").Substring(0, 6)
            Dim wrGETURL As WebRequest
            wrGETURL = WebRequest.Create("https://lookup.binlist.net/" + bin)
            wrGETURL.Headers.Add("accept-version", "3")
            'wrGETURL = WebRequest.Create("https://binlist.net/csv/" + bin)
            Dim objStream As Stream
            Try
                wrGETURL.Timeout = 1000
                objStream = wrGETURL.GetResponse.GetResponseStream()
                Dim objReader As New StreamReader(objStream)
                'Return objReader.ReadLine.ToString.Split(",").GetValue(3).ToString
                Dim s1 As String
                s1 = objReader.ReadLine.ToString
                s1 = s1.Substring(s1.IndexOf("alpha2") + 9, 2)
                Return s1
            Catch ex As Exception
                Return "?"
            End Try
        End Function

        Function GetIpCountry(OrderId As Integer) As String
            'Dim s As String = eQuote.ExecuteScalar("SELECT AuditTrail_1.Message AS IP_Message FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN AuditTrail ON [Order].Id = AuditTrail.OrderId INNER JOIN AuditTrail AS AuditTrail_1 ON [Order].Id = AuditTrail_1.OrderId WHERE ([Order].Id = " + OrderId.ToString + ") AND ([Transaction].Type = 1) AND (AuditTrail.Status = 3) AND (AuditTrail_1.Status = 4)")
            Dim ip As Object = eQuote.ExecuteScalar("SELECT IpCode FROM [Order] WHERE [Order].Id = " + OrderId.ToString)
            If IsDBNull(ip) Then
                Return "?"
            Else
                Return ip.ToString
            End If
            'Dim ip As String = ""
            'Try
            '    ip = s.Split(Chr(34)).GetValue(1).ToString
            '    ip = ip.Substring(0, 2)
            'Catch ex As Exception
            '    ip = "?"
            'End Try

        End Function

        '-------------------------------------------------------------------------
        Sub AddStatusChange(OrderId As Integer, CurrentStatus As String, NewStatus As Integer, By As String)
            Dim ns As String = eQuote.ExecuteScalar("SELECT Text FROM OrderStatus WHERE Id = " + NewStatus.ToString)
            Dim status As String = eQuote.ExecuteScalar("SELECT Id FROM AuditTrailStatus WHERE Text = N'Admin'")
            Dim message As String = "Status change: " + CurrentStatus + " -> " + ns + " (" + eQuote.ExecuteScalar("SELECT Lname FROM [User] WHERE Id = " + By) + ")"

            'eQuote.ExecuteNonQuery("INSERT INTO AuditTrail(OrderId, Status, Message, Created) VALUES (" + OrderId.ToString + ", " + status.ToString + ", N'" + message + "', { fn NOW() })")
            AuditLog.Log(message, status, 2, OrderId)
        End Sub

        Function KYCCheck(UserID As Integer) As Integer
            Return eQuote.ExecuteScalar("SELECT Count(*) FROM [KycFile] Where [KycFile].UserId=" + UserID.ToString + " AND [KycFile].Approved IS NULL")
        End Function

        Sub AddNote(OrderId As Integer, note As String)
            Dim GetMemoLogScript As String = String.Format(" {0:d}", Date.Now) + " at " + String.Format(" {0:t}", Date.Now) + note ' + "<br />"
            eQuote.ExecuteNonQuery("UPDATE [Order] SET Note = { fn CONCAT('" + Replace(GetMemoLogScript, Environment.NewLine, "<br/>") + "', { fn IFNULL(Note, '') }) } WHERE  Id = " + OrderId.ToString)
        End Sub

        Function TrustUser(OrderId As Integer, AdminId As Integer, IsTrusted As Boolean) As Integer
            Dim UserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + OrderId.ToString)
            If IsTrusted Then
                Return eQuote.ExecuteNonQuery("UPDATE [User] SET  Trusted = { fn NOW() }, TrustedBy = " + AdminId.ToString + " WHERE  Id = " + UserId.ToString)
            Else
                Return eQuote.ExecuteNonQuery("UPDATE [User] SET  Trusted = NULL, TrustedBy = NULL WHERE  Id = " + UserId.ToString)
            End If
        End Function

        'Function ApproveCard(OrderId As Integer, Optional ByVal Txt As String = "") As Boolean
        '    AddNote(OrderId, " Tx-Secret Approved" + Txt)
        '    Return eQuote.ExecuteNonQuery("UPDATE [Order] SET CardApproved = { fn NOW() } WHERE  Id = " + OrderId.ToString)
        'End Function

        Function ResetTxAttempt(OrderId As Integer) As Boolean
            Return eQuote.ExecuteNonQuery("UPDATE [Order] SET TxSecrectVerificationAttempts = NULL WHERE Id = " + OrderId.ToString)
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

        Function GetThisMonthRevenue(ByVal currency As String, Optional ByVal SiteId As Integer = 0) As String
            Dim Val As String
            Dim Sql As String = ""
            If SiteId > 0 Then Sql = " AND ([Order].SiteId = " + SiteId.ToString + ")"
            If currency = "DKK" Then
                Try
                    Dim nOrderRevenue As Decimal = eQuote.ExecuteScalar("SELECT SUM([Order].Amount / [Order].RateHome * [Order].RateBooks) AS AmountDKK FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN TransactionType ON TransactionType.Id = [Transaction].Type WHERE  (NOT ([Transaction].Completed IS NULL)) AND (NOT ([Order].Amount / [Order].RateHome * [Order].RateBooks IS NULL)) AND MONTH(Completed) = MONTH(GETDATE()) AND YEAR(Completed) = YEAR(GETDATE()) AND ([Transaction].Type = 1)" + Sql)
                    Val = String.Format("{0:n0} DKK", nOrderRevenue)
                Catch ex As Exception
                    Val = "0 DKK"
                End Try
            ElseIf currency = "EUR" Then
                Try
                    Dim nOrderRevenue As Decimal = eQuote.ExecuteScalar("SELECT SUM([Order].Amount / [Order].RateHome) AS AmountEUR FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN TransactionType ON TransactionType.Id = [Transaction].Type WHERE  (NOT ([Transaction].Completed IS NULL)) AND (NOT ([Order].Amount / [Order].RateHome * [Order].RateBooks IS NULL)) AND MONTH(Completed) = MONTH(GETDATE()) AND YEAR(Completed) = YEAR(GETDATE()) AND ([Transaction].Type = 1)" + Sql)
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

        Function GetOrigin(OrderId As Integer, Optional ByRef c As Label = Nothing) As String
            Dim r As Object = eQuote.ExecuteScalar("SELECT Origin FROM [Order] WHERE Id =" + OrderId.ToString)
            If IsNothing(r) Then
                Return ""
            Else
                c.Text = "From: " + r.ToString
                Return "From: " + r.ToString
            End If
        End Function

        Public Function StatusRecordCount(ByRef OrderStatus As String, Optional ByRef SiteId As Integer = 0) As Integer
            Dim sql As String = "SELECT COUNT(*) FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id WHERE OrderStatus.Text = N'" + OrderStatus + "'"
            Dim s As String = " AND [Order].SiteId = " + SiteId.ToString
            If SiteId > 0 Then sql = sql + s
            Dim val As Integer = eQuote.ExecuteScalar(sql)
            Return val
        End Function

        Public Function GetTimeDiffMinuttes(ByRef Time As String, Optional ByRef c As Label = Nothing) As Integer
            Dim diff As Integer = eQuote.ExecuteScalar("SELECT DATEDIFF(mi, '" + Time + "', { fn NOW() })")
            If Not IsNothing(c) Then
                If IsNumeric(diff) Then
                    If diff > 60 Then
                        c.BackColor = Drawing.Color.FromName("#FFA8C5")
                    Else
                        c.BackColor = Drawing.Color.FromName("#66FF66")
                    End If
                    c.Text = "Unlocked " + diff.ToString + " minuttes ago"
                End If
            End If
            Return diff
        End Function

        Public Function SendEmail(ByVal Template As String, ByVal SiteId As String, ByVal RecieverName As String, ByVal RecieverEmail As String, ByVal Number As String, Optional ByVal Quoted As String = "", Optional ByVal Amount As String = "", Optional ByVal CC As String = "") As Exception
            Dim SiteName As String = eQuote.ExecuteScalar("SELECT Text FROM Site WHERE Id = " + SiteId.ToString) '' .ToString.Split(New Char() {"."c})

            Dim params1 = New Dictionary(Of String, Object)
            params1.Add("UserFirstName", RecieverName)
            params1.Add("OrderNumber", Number)
            params1.Add("OrderCompleted", Quoted)
            params1.Add("OrderAmount", Amount)
            params1.Add("CreditCard", CC)

            Try
                '' Dim len1 = SiteName.Length
                EmailHelper.SendEmail(RecieverEmail, Template, params1, SiteName) ' , "bcc@mailwmc.com")
            Catch ex As Exception
                Return ex
            End Try
            Return Nothing

            '' Dim SiteName As String() = eQuote.ExecuteScalar("SELECT Text FROM Site WHERE Id = " + SiteId.ToString).ToString.Split(New Char() {"."c})

            ''Dim parantFolder = New DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName
            ''Dim _websiteName = ConfigurationManager.AppSettings("SiteName")
            ''Dim templatePath = String.Format("{0}/{1}/{0}/{0}/{0}", parantFolder, _websiteName, SiteName(1), Template)

            'Dim SR As New StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplate/" + SiteName(1) + "/" + Template), Encoding.Default)
            'Dim strBodyText As String = SR.ReadToEnd()

            'Dim SMTP As New SmtpClient
            'Dim Message As New MailMessage


            'SMTP.UseDefaultCredentials = False
            ''SMTP.Credentials = New System.Net.NetworkCredential(AppSettings("SMTP.UserId"), AppSettings("SMTP.Password"))
            'If SiteId = "1" Then
            '    SMTP.Credentials = New System.Net.NetworkCredential("wemovecoins@gmail.com", "oCZm7WsOeH")
            '    SMTP.Port = "587"
            '    SMTP.Host = "smtp.gmail.com"
            '    SMTP.EnableSsl = True
            '    Message.From = New System.Net.Mail.MailAddress("support@wemovecoins.com")
            '    Message.ReplyTo = New System.Net.Mail.MailAddress("support@wemovecoins.com")
            '    'If SiteId = "1" Then
            '    '    SMTP.Credentials = New System.Net.NetworkCredential("support@wemovecoins.com", "krnNpz7hFDlxARGF")
            '    '    SMTP.Port = "587"
            '    '    SMTP.Host = "asmtp.unoeuro.com"
            '    '    Message.From = New System.Net.Mail.MailAddress("support@wemovecoins.com")
            '    '    Message.ReplyTo = New System.Net.Mail.MailAddress("support@wemovecoins.com")
            'ElseIf SiteId = "2" Then
            '    'SMTP.Credentials = New System.Net.NetworkCredential("support@123bitcoin.dk", "sdZwtFc1Xe")
            '    'SMTP.Port = "587"
            '    'SMTP.Host = "asmtp.unoeuro.com"
            '    SMTP.Credentials = New System.Net.NetworkCredential("wemovecoins@gmail.com", "oCZm7WsOeH")
            '    SMTP.Port = "587"
            '    SMTP.Host = "smtp.gmail.com"
            '    SMTP.EnableSsl = True
            '    Message.From = New System.Net.Mail.MailAddress("support@123bitcoin.dk")
            '    Message.ReplyTo = New System.Net.Mail.MailAddress("support@123bitcoin.dk")
            'ElseIf SiteId = "15" Then
            '    'SMTP.Credentials = New System.Net.NetworkCredential("support@wemovecoins.com", "krnNpz7hFDlxARGF")
            '    'SMTP.Port = "587"
            '    'SMTP.Host = "asmtp.unoeuro.com"
            '    'Message.From = New System.Net.Mail.MailAddress("support@wemovecoins.com")
            '    'ElseIf SiteId = "15" Then
            '    SMTP.Credentials = New System.Net.NetworkCredential("service@simplekoin.com", "wpuX8qbNZwuTkmfkj34A")
            '    SMTP.Port = "587"
            '    SMTP.EnableSsl = True
            '    SMTP.Host = "smtp.gmail.com"
            '    Message.From = New System.Net.Mail.MailAddress("service@simplekoin.com")
            'ElseIf SiteId = "16" Then
            '    SMTP.Credentials = New System.Net.NetworkCredential("service@mycoins.fr", "M27jR3Pd6")
            '    SMTP.Port = "587"
            '    SMTP.Host = "asmtp.unoeuro.com"

            '    Message.From = New System.Net.Mail.MailAddress("service@mycoins.fr")
            'ElseIf SiteId = "17" Then
            '    SMTP.Credentials = New System.Net.NetworkCredential("support@wemovecoins.com", "krnNpz7hFDlxARGF")
            '    SMTP.Port = "587"
            '    SMTP.Host = "asmtp.unoeuro.com"
            '    Message.From = New System.Net.Mail.MailAddress("support@wemovecoins.com")
            'End If
            'SMTP.DeliveryMethod = SmtpDeliveryMethod.Network
            'strBodyText = strBodyText.Replace("{{UserFirstName}}", RecieverName)
            'strBodyText = strBodyText.Replace("{{OrderNumber}}", Number)
            'strBodyText = strBodyText.Replace("{{OrderCompleted}}", Quoted)
            'strBodyText = strBodyText.Replace("{{OrderAmount}}", Amount)
            'strBodyText = strBodyText.Replace("{{CreditCard}}", CC)
            'Try
            '    With Message
            '        '.From = New System.Net.Mail.MailAddress(AppSettings("SMTP.UserId"))
            '        .To.Add(RecieverEmail)
            '        .Bcc.Add("bcc@mailwmc.com")
            '        .Subject = Subject
            '        .IsBodyHtml = True
            '        .Body = strBodyText
            '    End With

            '    SMTP.Send(Message)
            'Catch ex As Exception
            '    Return ex
            '    Exit Function
            'End Try
            'Return Nothing
        End Function
    End Module
End Namespace
