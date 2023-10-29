Imports System.Collections.Generic
Imports System.Threading
Imports System.Linq
Imports Jitbit.Utils
Imports WMC.Utilities

Public Class YourPayReport
    Inherits Page
    Protected Sub ButtonSearch_Click(sender As Object, e As EventArgs)
        Thread.CurrentThread.CurrentCulture = New Globalization.CultureInfo("en-US")
        GridView1.DataSource = FetchMerchantData(Date.Parse(txtStartDate.Text), Date.Parse(txtEndDate.Text)).
            Select(Function(q)
                       Return New With {
                            Key .DateId = q.dateid,
                            Key .AccountId = q.accountid,
                            Key .CapturedAmount = q.captured_amount,
                            Key .CapturedFee = q.captured_fee,
                            Key .ReleasedAmount = q.released_amount,
                            Key .RefundAmount = q.refund_amount,
                            Key .ManualAdjustments = q.manual_adjustments,
                            Key .DailyPercentage = q.daily_percentage,
                            Key .DailySettlementPeriod = q.daily_settlement_period,
                            Key .DateStart = q.DateStart,
                            Key .DateExpectedRelease = q.DateExpectedRelease,
                            Key .ConversionRate = q.conversionrate
                        }
                   End Function).ToList()
        GridView1.DataBind()
    End Sub

    Protected Sub ButtonExport_Click(sender As Object, e As EventArgs)
        Dim SearchResults = FetchMerchantData(Date.Parse(txtStartDate.Text), Date.Parse(txtEndDate.Text)).
            Select(Function(q)
                       Return New With {
                            Key .DateId = q.dateid,
                            Key .AccountId = q.accountid,
                            Key .CapturedAmount = q.captured_amount,
                            Key .Captured_fee = q.captured_fee,
                            Key .Released_amount = q.released_amount,
                            Key .Refund_amount = q.refund_amount,
                            Key .Manual_adjustments = q.manual_adjustments,
                            Key .DailyPercentage = q.daily_percentage,
                            Key .DailySettlement_period = q.daily_settlement_period,
                            Key .DateStart = q.DateStart,
                            Key .DateExpectedRelease = q.DateExpectedRelease,
                            Key .conversionrate = q.conversionrate
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