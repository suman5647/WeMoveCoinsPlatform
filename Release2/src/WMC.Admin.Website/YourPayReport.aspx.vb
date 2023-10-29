Imports System.Threading
Imports WMC.Utilities
Imports System.Collections.Generic
Imports System.Linq

Partial Class YourPayReport
    Inherits System.Web.UI.Page
    Protected Sub ButtonSearch_Click(sender As Object, e As EventArgs)
        Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("en-US")
        Try
            GridView1.DataSource = FetchMerchantData(Date.Parse(txtStartDate.Text), Date.Parse(txtEndDate.Text)).
            Select(Function(q)
                       Return New With {
                    Key .DateId = q.dateid,
                    Key .AccountId = q.accountid,
                    Key .CapturedAmount = q.captured_amount,
                    Key .CapturedFee = q.captured_fee,
                    Key .ReleasedAmount = q.released_amount,
                    Key .RefundAmount = q.refund_amount,
                    Key .Currency = q.currency,
                    Key .ManualAdjustments = q.manual_adjustments,
                    Key .DailyPercentage = q.daily_percentage,
                    Key .DailySettlementPeriod = q.daily_settlement_period,
                    Key .DateStart = q.DateStart,
                    Key .DateExpectedRelease = q.DateExpectedRelease,
                    Key .ConversionRate = q.conversionrate,
                    Key .ActionID = q.ActionID,
                    Key .amount = q.amount,
                    Key .captured = q.captured,
                    Key .handlingtype = q.handlingtype,
                    Key .orderID = q.orderID,
                    Key .PaymentID = q.PaymentID,
                    Key .ReqTimestamp = q.ReqTimestamp
                }
                   End Function).ToList()
            GridView1.DataBind()
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub

    Protected Sub ButtonExport_Click(sender As Object, e As EventArgs)
        Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("en-US")
        Dim SearchResults = FetchMerchantData(Date.Parse(txtStartDate.Text), Date.Parse(txtEndDate.Text)).
            Select(Function(q)
                       Return New With {
                            Key .DateId = q.dateid,
                            Key .AccountId = q.accountid,
                            Key .CapturedAmount = q.captured_amount,
                            Key .CapturedFee = q.captured_fee,
                            Key .ReleasedAmount = q.released_amount,
                            Key .RefundAmount = q.refund_amount,
                            Key .Currency = q.currency,
                            Key .ManualAdjustments = q.manual_adjustments,
                            Key .DailyPercentage = q.daily_percentage,
                            Key .DailySettlementPeriod = q.daily_settlement_period,
                            Key .DateStart = q.DateStart,
                            Key .DateExpectedRelease = q.DateExpectedRelease,
                            Key .ConversionRate = q.conversionrate,
                            Key .ActionID = q.ActionID,
                            Key .amount = q.amount,
                            Key .captured = q.captured,
                            Key .handlingtype = q.handlingtype,
                            Key .orderID = q.orderID,
                            Key .PaymentID = q.PaymentID,
                            Key .ReqTimestamp = q.ReqTimestamp
                        }
                   End Function).ToList()
        Dim csvexport = New CsvExport(",", False)
        csvexport.IncludeHeaders = True
        If SearchResults.Count > 0 Then
            csvexport.AddRows(SearchResults)
        End If
        Dim Content As Byte() = Encoding.[Default].GetBytes(csvexport.Export())
        Response.ContentType = "text/csv"
        Response.AddHeader("content-disposition", "attachment; filename=" + "Paylike export " + txtStartDate.Text + " to " + txtEndDate.Text + ".csv")
        Response.BufferOutput = True
        Response.OutputStream.Write(Content, 0, Content.Length)
        Response.[End]()
    End Sub

    Private Function FetchMerchantData(startdate As Date, enddate As Date, Optional lastPageNumber As Integer = Nothing) As List(Of YourPayTransactionsOutgoingOverviewList)
        'Dim result = New List(Of YourPayTransactionList)()
        'Dim resultCount = 0
        'Do
        '    Dim tempResult = YourpayService.TransactionList(DropDownListType.SelectedValue, 100, lastPageNumber, startdate, enddate)
        '    result.AddRange(tempResult)
        '    resultCount = tempResult.Count
        '    lastPageNumber += 100
        'Loop While (resultCount > 0)
        'Return result
        Dim siteId As String = DropDownListSite.SelectedValue.Split(";").GetValue(0).ToString
        Return YourpayService.TransactionsOutgoingOverview(startdate, enddate, siteId)
    End Function
End Class
