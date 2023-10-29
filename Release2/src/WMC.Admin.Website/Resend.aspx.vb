Imports WMC.Logic
Imports System.Data
Imports System.Data.SqlClient
Imports WMC.Data

<WMC.PageAuth(True, "Admin")>
Partial Class Resend
    Inherits WMC.SecurePage

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles ResendButton.Click
        Select Case DropDownListWhereType.SelectedValue
            Case "Number"
                Dim orderNumber = TextBoxValue.Text
                Dim oNo As String = orderNumber.ToString
                Using dr As SqlDataReader = eQuote.GetDataReader("SELECT UserId, OrderStatus.Text AS Status FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id WHERE Number=" + orderNumber.ToString)
                    dr.Read()
                    If dr.HasRows Then
                        If (dr!UserId And dr!Status = "Completed") Then
                            ResendEmailHelper.ResendMail(orderNumber)
                            lblMessage.Text = "Order Completed email sent to user."
                        ElseIf (dr!UserId) Then
                            lblMessage.Text = "Given Order Number is not yet completed. Please try after sometime. Current Order status is " & dr!Status
                        End If
                    Else
                        lblMessage.Text = "Given Order Number not found."
                    End If
                    dr.Close()
                End Using

            Case "OrderId"
                Dim orderId As Integer = (TextBoxValue.Text)
                Using dr As SqlDataReader = eQuote.GetDataReader("SELECT UserId, OrderStatus.Text AS Status FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id WHERE [Order].Id=" + orderId.ToString)
                    dr.Read()
                    If dr.HasRows Then
                        If (dr!UserId And dr!Status = "Completed") Then
                            ResendEmailHelper.ResendMail(orderId)
                            lblMessage.Text = "Order Completed email sent to user."
                        ElseIf (dr!UserId) Then
                            lblMessage.Text = "Given Order Id is not yet completed. Please try after sometime. Current Order status is " & dr!Status
                        End If
                    Else
                        lblMessage.Text = "Given Order Id not found."
                    End If
                    dr.Close()
                End Using
        End Select
    End Sub
End Class
