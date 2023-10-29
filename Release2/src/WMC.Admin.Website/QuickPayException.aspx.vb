
Imports System.Data
Imports System.Data.SqlClient

Partial Class QuickPayException
    Inherits System.Web.UI.Page

    Public Property DropDownListSellAmountCurrency As Object

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim status = Request.QueryString("status")
        Dim Command = SqlDataSourceQuickOrders.SelectCommand
        Dim days = Request.QueryString("days")
        Dim fromDate As DateTime
        If String.IsNullOrEmpty(days) Then
            fromDate = Date.Now().AddDays(-1)
        Else
            fromDate = Date.Now().AddDays(-days)
        End If

        SqlDataSourceQuickOrders.SelectParameters.Item("status").DefaultValue = status
        SqlDataSourceQuickOrders.SelectParameters.Item("fromDate").DefaultValue = fromDate
        SqlDataSourceQuickOrders.DataBind()
        GridViewException.AllowPaging = False

    End Sub

    Protected Sub GridViewUser_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewException.RowDataBound

        If e.Row.RowType = DataControlRowType.DataRow Then
            If e.Row.RowState = DataControlRowState.Selected Or e.Row.RowState = DataControlRowState.Normal Or e.Row.RowState = DataControlRowState.Alternate Then
                Dim lb As HyperLink = CType(e.Row.FindControl("HyperLinkOrderLookup"), HyperLink)
                lb.NavigateUrl = "FindOrders.aspx?orderId=" + lb.NavigateUrl
            End If
        End If
    End Sub
End Class


