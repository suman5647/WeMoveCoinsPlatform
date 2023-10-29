Imports System.Globalization
Imports System.Data
Imports System.Data.SqlClient

Partial Class RiskList
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not (IsNumeric(Session("UserId")) And Session("UserRole") = "Admin") Then Response.Redirect("Default.aspx")
        Dim OrderId As Integer
        Dim dt As New DataTable()
        dt.Columns.Add("order_number")
        dt.Columns.Add("risk_score")
        dt.Columns.Add("CompletedOrders")
        dt.Columns.Add("KycDeclined")
        dt.Columns.Add("OrderSpan")
        dt.Columns.Add("OrderSumEUR")
        dt.Columns.Add("OrderSumTotalEUR")
        dt.Columns.Add("OrderSum30Days")

        dt.Columns.Add("CC_IsUS")
        dt.Columns.Add("NumberOfCC")
        dt.Columns.Add("CreditCardUsedElsewhere")
        dt.Columns.Add("EmailUsedElsewhere")
        dt.Columns.Add("IpUsedElsewhere")
        dt.Columns.Add("PhoneCC")
        dt.Columns.Add("PhoneIP")
        dt.Columns.Add("PhoneCulture")
        dt.Columns.Add("CcIP")
        dt.Columns.Add("DifferentNames")
        dt.Columns.Add("DifferentEmails")

        Dim Top As Integer = 0
        Dim TopText As String
        If DropDownListTop.SelectedValue = "ALL" Then
            TopText = ""
        Else
            Top = CInt(DropDownListTop.SelectedValue) / 2
            TopText = "TOP (" + Top.ToString + ")"
        End If



        Dim drCompleted As SqlDataReader = eQuote.GetDataReader("SELECT " + TopText + " [Order].Id, [Order].Number, OrderStatus.Text FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id WHERE  (OrderStatus.Text = N'Completed') ORDER BY [Order].Id DESC")
        If drCompleted.HasRows Then
            While drCompleted.Read()
                Dim dr = dt.NewRow()
                OrderId = drCompleted!Id

                dr("order_number") = drCompleted!Number
                If drCompleted!Text = "Completed" Then dr("risk_score") = 1 Else dr("risk_score") = 0
                dr("CompletedOrders") = WMCData.GetOrdersByStatus(OrderId, "Completed")
                dr("KycDeclined") = WMCData.GetOrdersByStatus(OrderId, "KYC Decline")
                dr("OrderSpan") = WMCData.GetOrderSpan(OrderId)
                If WMCData.GetOrderSumEUR(OrderId) = 0 Then
                    dr("OrderSumEUR") = ""
                Else
                    dr("OrderSumEUR") = String.Format("{0:n0}", WMCData.GetOrderSumEUR(OrderId))
                End If
                dr("OrderSumTotalEUR") = String.Format("{0:n0}", WMCData.GetOrderSumTotalEUR(OrderId))
                dr("OrderSum30Days") = String.Format("{0:n0}", WMCData.GetOrderSum30DaysEUR(OrderId))
                dr("CC_IsUS") = WMCData.IsCardUS(OrderId)
                dr("NumberOfCC") = WMCData.GetDistinctUsedCards(OrderId)
                dr("CreditCardUsedElsewhere") = WMCData.GetCardUsedElsewhere(OrderId)
                dr("EmailUsedElsewhere") = WMCData.GetEmailUsedElsewhere(OrderId)
                'dr("IpUsedElsewhere") = WMCData.GetIpUsedElsewhere(OrderId)
                dr("PhoneCC") = WMCData.PhoneCardOrigin_Match(OrderId)
                dr("PhoneIP") = WMCData.PhoneIP_Match(OrderId)
                'dr("PhoneCulture") = WMCData.GetPhoneCulture_Match(OrderId)
                'dr("CcIP") = WMCData.GetPhoneCulture_Match(OrderId)
                dr("DifferentNames") = WMCData.GetDifferentNameUser(OrderId)
                dr("DifferentEmails") = WMCData.GetDifferentEmailUser(OrderId)
                dt.Rows.Add(dr)
            End While
        End If
        drCompleted.Close()

        Dim drDeclined As SqlDataReader = eQuote.GetDataReader("SELECT " + TopText + " [Order].Id, [Order].Number, OrderStatus.Text FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id WHERE (OrderStatus.Text = N'KYC Declined') ORDER BY [Order].Id DESC")
        If drDeclined.HasRows Then
            While drDeclined.Read()
                Dim dr = dt.NewRow()
                OrderId = drDeclined!Id

                dr("order_number") = drDeclined!Number
                If drDeclined!Text = "Completed" Then dr("risk_score") = 1 Else dr("risk_score") = 0
                dr("CompletedOrders") = WMCData.GetOrdersByStatus(OrderId, "Completed")
                dr("KycDeclined") = WMCData.GetOrdersByStatus(OrderId, "KYC Decline")
                dr("OrderSpan") = WMCData.GetOrderSpan(OrderId)
                If WMCData.GetOrderSumEUR(OrderId) = 0 Then
                    dr("OrderSumEUR") = ""
                Else
                    dr("OrderSumEUR") = String.Format("{0:n0}", WMCData.GetOrderSumEUR(OrderId))
                End If
                dr("OrderSumTotalEUR") = String.Format("{0:n0}", WMCData.GetOrderSumTotalEUR(OrderId))
                dr("OrderSum30Days") = String.Format("{0:n0}", WMCData.GetOrderSum30DaysEUR(OrderId))
                dr("CC_IsUS") = WMCData.IsCardUS(OrderId)
                dr("NumberOfCC") = WMCData.GetDistinctUsedCards(OrderId)
                dr("CreditCardUsedElsewhere") = WMCData.GetCardUsedElsewhere(OrderId)
                dr("EmailUsedElsewhere") = WMCData.GetEmailUsedElsewhere(OrderId)
                'dr("IpUsedElsewhere") = WMCData.GetIpUsedElsewhere(OrderId)
                dr("PhoneCC") = WMCData.PhoneCardOrigin_Match(OrderId)
                dr("PhoneIP") = WMCData.PhoneIP_Match(OrderId)
                'dr("PhoneCulture") = WMCData.GetPhoneCulture_Match(OrderId)
                'dr("CcIP") = WMCData.GetPhoneCulture_Match(OrderId)
                dr("DifferentNames") = WMCData.GetDifferentNameUser(OrderId)
                dr("DifferentEmails") = WMCData.GetDifferentEmailUser(OrderId)
                dt.Rows.Add(dr)
            End While
        End If
        drDeclined.Close()

        GridViewRiskScore.DataSource = dt
        GridViewRiskScore.DataBind()
    End Sub


End Class
