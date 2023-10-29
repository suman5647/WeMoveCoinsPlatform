Imports System.Net.Mail

Partial Class TestEmailCredentials
    Inherits System.Web.UI.Page

    Protected Sub ButtonSend_Click(sender As Object, e As EventArgs) Handles ButtonSend.Click
        Dim SMTP As New SmtpClient
        Dim Message As New MailMessage

        SMTP.Credentials = New System.Net.NetworkCredential(TextBoxFromAddress.Text, TextBoxPassword.Text)
        SMTP.Port = TextBoxPort.Text
        SMTP.Host = TextBoxHost.Text
        Message.From = New System.Net.Mail.MailAddress(TextBoxFromAddress.Text)
        SMTP.EnableSsl = CheckBoxEnableSSL.Checked
        SMTP.DeliveryMethod = SmtpDeliveryMethod.Network
        With Message
            .To.Add(TextBoxSendTo.Text)
            .Subject = "Test - Send email from wemovecons"
            .IsBodyHtml = True
            .Body = "Test - Send email from wemovecons"
        End With
        Try
            SMTP.Send(Message)
            LabelResponse.Text = "Worked!"
            LabelResponse.ForeColor = Drawing.Color.Green
        Catch ex As Exception
            LabelResponse.Text = ex.Message
            LabelResponse.ForeColor = Drawing.Color.Red
        End Try
        LabelResponse.Visible = True
    End Sub
End Class
