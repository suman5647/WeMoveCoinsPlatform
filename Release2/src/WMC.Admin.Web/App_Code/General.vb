Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.IO
Imports System.Configuration.ConfigurationManager
Imports System.Net.Mail
Imports System.Text
Imports System

Namespace eQuote
    Public Module General
        Public Function GetExchangeValue(ByRef CurrencyIn As String, ByRef CurrencyOut As String) As Double
            Dim val As Double = ExecuteScalar("SELECT cr_conversion_value FROM cc_conversion_rate WHERE cr_currency_code_to = '" + CurrencyOut + "' AND cr_currency_code_from = '" + CurrencyIn + "'")
            Return val
        End Function


        Public Function ExchangeAmount(ByRef CurrencyIn As String, ByRef CurrencyOut As String, ByRef Amount As Double) As Double
            Dim val As Double = ExecuteScalar("SELECT cr_conversion_value FROM cc_conversion_rate WHERE cr_currency_code_to = '" + CurrencyOut + "' AND cr_currency_code_from = '" + CurrencyIn + "'")
            Return val * Amount
        End Function

        Public Function SetConfirmDonationIfSufficientCredit(ByRef OrderId As Integer) As Boolean
            Dim UserId As Integer = eQuote.ExecuteScalar("SELECT or_user_id FROM cc_order WHERE or_id=" + OrderId.ToString)

            Dim dr As SqlDataReader = eQuote.GetDataReader("SELECT  tr_order_id, tr_amount, or_currency FROM cc_transaction INNER JOIN cc_order ON tr_order_id = or_id WHERE (tr_type_id = 3) AND (tr_complete_date IS NULL) AND (or_user_id = " + UserId.ToString + ")")
            If dr.HasRows Then
                While dr.Read()
                    If -1 * dr!tr_amount <= GetBalancePrefered(UserId, dr!or_currency, System.Configuration.ConfigurationManager.AppSettings("CurrencySystem")) Then
                        ConfirmTransaction(3, dr!tr_order_id, UserId)
                        SetCompleteOrderIfSubTransactionsAreCompleted(dr!tr_order_id)
                    End If
                End While
            End If
            dr.Close()
            Return True
        End Function

        Public Function SetCompleteOrderIfSubTransactionsAreCompleted(ByRef OrderId As Integer) As Boolean
            Dim b As Boolean = False
            Dim n = eQuote.ExecuteScalar("SELECT COUNT(*) FROM cc_transaction WHERE (tr_order_id = " + OrderId.ToString + ") GROUP BY tr_complete_date HAVING tr_complete_date IS NULL")
            If Not IsNumeric(n) Then
                eQuote.ExecuteScalar("UPDATE cc_order SET or_complete_date = { fn NOW() } WHERE or_id = " + OrderId.ToString)
                b = True
            End If
            Return b
        End Function


        Public Function ConfirmTransaction(ByRef TypeId As Integer, ByRef OrderId As Integer, ByRef AdminUserId As Integer, Optional ByRef ActualAmount As Double = Nothing) As Boolean
            Dim b As Boolean = False
            If TypeId = 2 Then
                If eQuote.ExecuteScalar("SELECT COUNT(*) FROM cc_transaction GROUP BY tr_complete_date, tr_type_id HAVING (NOT (tr_complete_date IS NULL)) AND (tr_type_id = 1)") = 1 Then
                    If IsNumeric(ActualAmount) Then
                        eQuote.ExecuteScalar("UPDATE cc_transaction SET tr_amount = " + CInt(ActualAmount).ToString + ", tr_complete_date = { fn NOW() }, tr_complete_user_id = " + AdminUserId.ToString + " WHERE tr_order_id = " + OrderId.ToString + " AND tr_type_id = " + TypeId.ToString)
                    Else
                        eQuote.ExecuteScalar("UPDATE cc_transaction SET tr_complete_date = { fn NOW() }, tr_complete_user_id = " + AdminUserId.ToString + " WHERE tr_order_id = " + OrderId.ToString + " AND tr_type_id = " + TypeId.ToString)
                    End If
                    b = True
                Else
                    b = False
                End If
            Else
                If TypeId = 6 Then
                    Dim dr As SqlDataReader = GetDataReader("SELECT us_fname, us_email, or_amount, or_currency FROM cc_order INNER JOIN cc_user ON or_user_id = us_id WHERE cc_order.or_id = " + OrderId.ToString)
                    If dr.Read() Then
                        SendEmail2(dr!us_fname, dr!us_email, "You have received a donation from CommonCollection", "...your donation", "Congrats! We have transfered " + CInt(dr!or_amount).ToString + " " + dr!or_currency + " to your account. Depending on your choosen payout method, you must expect some delay before the donation hits your account. When you receive the donation, we strongly urge you to post a message on your case so your donators can rest in peace.", "EmailPayment")
                    End If
                    dr.Close()
                End If
                eQuote.ExecuteScalar("UPDATE cc_transaction SET tr_complete_date = { fn NOW() }, tr_complete_user_id = " + AdminUserId.ToString + " WHERE tr_order_id = " + OrderId.ToString + " AND tr_type_id = " + TypeId.ToString)
                b = True
            End If
            Return b
        End Function


        Public Function CreateTransaction(ByRef OrderId As Integer, ByRef Type As Integer, ByRef Amount As Double, ByRef Currency As String, Optional ByRef ProjectId As Integer = Nothing) As Integer
            Dim sPlatformCurrency As String = System.Configuration.ConfigurationManager.AppSettings("CurrencySystem")

            Dim cmd As New SqlCommand
            cmd.Connection = eQuote.OpenConnection
            cmd.CommandText = "INSERT INTO cc_transaction (tr_order_id, tr_project_id, tr_type_id, tr_amount, tr_currency, tr_start_date) VALUES (@tr_order_id,@tr_project_id,@tr_type_id,@tr_amount,@tr_currency, { fn NOW() })"
            With cmd.Parameters
                .AddWithValue("tr_order_id", OrderId)
                .AddWithValue("tr_project_id", ProjectId)
                .AddWithValue("tr_type_id", Type)
                .AddWithValue("tr_amount", ExchangeAmount(Currency, sPlatformCurrency, Amount))
                .AddWithValue("tr_currency", sPlatformCurrency)
            End With
            cmd.ExecuteNonQuery()
            Dim NewId As Integer = eQuote.GetNewID(cmd).ToString
            cmd.Connection.Close()

            Return NewId

        End Function


        Public Function CreateOrder(ByRef DonatorId As Integer, ByRef OrderType As Integer, ByRef Amount As Double, ByRef Currency As String, Optional ByRef ProjectId As Integer = Nothing) As Integer
            Dim sPlatformCurrency As String = System.Configuration.ConfigurationManager.AppSettings("CurrencySystem")

            Dim cmd As New SqlCommand
            cmd.Connection = eQuote.OpenConnection
            cmd.CommandText = "INSERT INTO cc_order(or_user_id, or_type_id, or_project_id, or_amount, or_currency, or_start_date) VALUES (@or_user_id,@or_type_id,@or_project_id, @or_amount,@or_currency, { fn NOW() })"
            With cmd.Parameters
                .AddWithValue("or_user_id", DonatorId)
                .AddWithValue("or_type_id", OrderType)
                .AddWithValue("or_project_id", ProjectId)
                .AddWithValue("or_amount", Amount)
                .AddWithValue("or_currency", Currency)
            End With
            cmd.ExecuteNonQuery()
            Dim NewId As Integer = eQuote.GetNewID(cmd).ToString
            cmd.Connection.Close()

            Dim dr As SqlDataReader = GetDataReader("SELECT us_fname, us_lname, us_email FROM cc_user WHERE us_id = " + DonatorId.ToString)
            If dr.Read() Then
                If IsNumeric(NewId) Then
                    If OrderType = 1 Then                                                           'DONATION
                        CreateTransaction(NewId, 3, ExchangeAmount(Currency, sPlatformCurrency, Amount), sPlatformCurrency, ProjectId)
                    ElseIf OrderType = 2 Then
                        CreateTransaction(NewId, 1, ExchangeAmount(Currency, sPlatformCurrency, Amount), sPlatformCurrency, Nothing)                      'PAYMENT
                        CreateTransaction(NewId, 2, ExchangeAmount(Currency, sPlatformCurrency, Amount), sPlatformCurrency, Nothing)
                        SendEmail2(dr!us_fname, dr!us_email, "Please make payment using this reference number: " + NewId.ToString, "...your payment", "Please make payment using this reference number: " + NewId.ToString + ". When your payment is received, we will update your cc-account, send you an email and you will be ready to make donations.", "EmailPayment")
                    ElseIf OrderType = 3 Then                                                       'REQUEST AND PAYOUT
                        CreateTransaction(NewId, 4, ExchangeAmount(Currency, sPlatformCurrency, Amount), sPlatformCurrency, ProjectId)
                        CreateTransaction(NewId, 6, ExchangeAmount(Currency, sPlatformCurrency, Amount), sPlatformCurrency, ProjectId)
                    ElseIf OrderType = 4 Then                                                       'REQUEST AND PAYOUT
                        CreateTransaction(NewId, 5, ExchangeAmount(Currency, sPlatformCurrency, Amount), sPlatformCurrency, Nothing)
                    End If
                End If

            End If
            dr.Close()
            Return NewId
        End Function



        Public Function GetBalancePreferedOld(ByVal UserId As Integer, ByRef currencyIn As String, ByRef currencyOut As String) As Double
            Dim dr As SqlDataReader = eQuote.GetDataReader("SELECT SUM(or_amount) AS Balance FROM cc_order WHERE (NOT (cc_order.or_complete_date IS NULL)) AND (cc_order.or_type_id = 1 OR cc_order.or_type_id = 2)  AND or_user_id = " + UserId.ToString)
            If dr.Read() Then
                If IsDBNull(dr!Balance) Then
                    Return 0
                Else
                    Return eQuote.GetExchangeValue(currencyIn, currencyOut) * dr!Balance
                End If
            End If
            dr.Close()
            Return 0

        End Function

        Public Function GetBalancePrefered(ByVal UserId As Integer, ByRef currencyIn As String, ByRef currencyOut As String) As Double
            Dim dr As SqlDataReader = eQuote.GetDataReader("SELECT SUM(tr_amount) AS Balance FROM cc_order INNER JOIN cc_transaction ON cc_order.or_id = cc_transaction.tr_order_id WHERE (NOT (tr_complete_date IS NULL)) AND (cc_transaction.tr_type_id = 2 OR cc_transaction.tr_type_id = 3)  AND or_user_id = " + UserId.ToString)
            Dim r As Double = 0
            If dr.Read() Then
                If Not IsDBNull(dr!Balance) Then
                    r = eQuote.GetExchangeValue(currencyIn, currencyOut) * dr!Balance
                End If
            End If
            dr.Close()
            Return r

        End Function

        Public Function GetBalance(ByVal UserId As Integer) As Double
            Dim dr As SqlDataReader = eQuote.GetDataReader("SELECT SUM(or_amount) AS Balance FROM cc_order WHERE (NOT (cc_order.or_complete_date IS NULL)) AND (cc_order.or_type_id = 1 OR cc_order.or_type_id = 2)  AND or_user_id = " + UserId.ToString)
            Dim r As Double = 0
            If dr.Read() Then
                If Not IsDBNull(dr!Balance) Then
                    r = dr!Balance
                End If
            End If
            dr.Close()
            Return r

        End Function

        Public Function GetProjectBalance(ByVal UserId As Integer) As Double
            Dim dr As SqlDataReader = eQuote.GetDataReader("SELECT SUM(or_amount) AS Balance FROM cc_order WHERE (NOT (cc_order.or_complete_date IS NULL)) AND or_user_id = " + UserId.ToString)
            Dim r As Double = 0
            If dr.Read() Then
                If IsDBNull(dr!Balance) Then r = 0 Else r = dr!Balance
            Else
                r = 0
            End If
            dr.Close()
            Return r

        End Function


        Public Sub StampLoggingInfo(ByVal UserId As Integer, ByVal TaskType As Integer, ByVal SessionID As String, Optional ByVal HostName As String = "", Optional ByVal Language As String = "", Optional ByVal RequestID As String = "", Optional ByVal SubjectID As Integer = 0)

            eQuote.ExecuteNonQuery("INSERT INTO hb_user_tracking (ut_person_id, ut_activity, ut_session_id, ut_logging_time, ut_request_host_name, ut_request_language, ut_request_id, ut_subject_id) VALUES (" + UserId.ToString + ", " + TaskType.ToString + ", N'" + SessionID + "', { fn NOW() }, N'" + HostName + "', N'" + Language + "', N'" + RequestID + "', " + SubjectID.ToString + ")")

        End Sub


        Public Function SendEmail2(ByVal RecieverName As String, ByVal RecieverEmail As String, ByVal Subject As String, ByVal Head As String, ByVal Text As String, ByVal Template As String) As Exception
            Dim SR As New StreamReader(AppSettings("Email_Template_Dir") + AppSettings(Template), Encoding.Default)
            Dim strBodyText As String = SR.ReadToEnd()

            Dim SMTP As New SmtpClient
            Dim Message As New MailMessage

            SMTP.Host = System.Configuration.ConfigurationManager.AppSettings("SMTPHost")
            SMTP.UseDefaultCredentials = False
            SMTP.Credentials = New System.Net.NetworkCredential("thorkild@grothe-moller.dk", "Golf2004")
            SMTP.Port = "2525"
            SMTP.DeliveryMethod = SmtpDeliveryMethod.Network

            strBodyText = strBodyText.Replace("@name", "Hi " + RecieverName)
            strBodyText = strBodyText.Replace("@email", RecieverEmail)
            strBodyText = strBodyText.Replace("@head", Head)
            strBodyText = strBodyText.Replace("@text", Text)

            'strBodyText = System.Web.HttpUtility.HtmlEncode(strBodyText)

            Try
                With Message
                    .From = New System.Net.Mail.MailAddress("thorkild@grothe-moller.dk")
                    .To.Add(RecieverEmail)
                    .Bcc.Add("thorkild@grothe-moller.dk")
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

        Public Function SendEmail(ByVal RecieverName As String, ByVal RecieverEmail As String, ByVal Subject As String, ByVal Text1 As String, ByVal Text2 As String, ByVal Regards As String) As Exception
            Dim SR As New StreamReader(AppSettings("Email_Template_Dir") + AppSettings("EmailTemplate"), Encoding.Default)
            Dim strBodyText As String = SR.ReadToEnd()

            Dim SMTP As New SmtpClient
            Dim Message As New MailMessage

            SMTP.Host = System.Configuration.ConfigurationManager.AppSettings("SMTPHost")
            SMTP.UseDefaultCredentials = False
            SMTP.Credentials = New System.Net.NetworkCredential("thorkild@grothe-moller.dk", "Golf2004")
            SMTP.Port = "2525"
            SMTP.DeliveryMethod = SmtpDeliveryMethod.Network
            'SMTP.EnableSsl = True
            'SMTP.

            strBodyText = strBodyText.Replace("xxxName", RecieverName)
            strBodyText = strBodyText.Replace("xxxText", Text1)
            strBodyText = strBodyText.Replace("xxxSubText", Text2)
            strBodyText = strBodyText.Replace("xxxRegards", Regards)

            'strBodyText = System.Web.HttpUtility.HtmlEncode(strBodyText)

            Try
                With Message
                    .From = New System.Net.Mail.MailAddress("thorkild@grothe-moller.dk")
                    .To.Add(RecieverEmail)
                    .Bcc.Add("thorkild@grothe-moller.dk")
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



        Public Function FileExists(ByVal FileFullPath As String) As Boolean

            Dim f As New IO.FileInfo(FileFullPath)
            Return f.Exists

        End Function



        ''' <summary>
        ''' Checks a string to be valid for a password or login 
        ''' </summary>
        ''' <param name="str">String to be cheched</param>
        ''' <returns>Returns True if valid - False if not</returns>
        ''' <remarks>Added scecurity checking for user validation</remarks>
        Public Function CheckString(ByVal str As String) As Boolean

            CheckString = True
            If InStr(str, "'") Or InStr(str, "''") Then
                CheckString = False
                Exit Function
            End If

        End Function

        Public Function GetNewID(ByVal Comm As System.Data.SqlClient.SqlCommand) As Integer

            Comm.CommandText = "SELECT @@IDENTITY"
            Try
                Return Comm.ExecuteScalar
            Catch ex As Exception
                Return False
            End Try


        End Function

    End Module
End Namespace