Imports System.Data.SqlClient
Imports System.Data
Imports System.IO

Partial Class ImageVB
    Inherits System.Web.UI.Page

    Protected Sub form1_Load(sender As Object, e As EventArgs) Handles form1.Load
        If Request.QueryString("ImageID") IsNot Nothing Then

            Dim strConnString As String = System.Configuration.ConfigurationManager.ConnectionStrings("LocalConnectionString").ConnectionString()

            Dim strQuery As String = "select NULL as [File] ,OriginalFilename,UniqueFilename from KycFile where id=@id"

            Dim cmd As SqlCommand = New SqlCommand(strQuery)

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = Convert.ToInt32(Request.QueryString("ImageID"))

            Dim con As New SqlConnection(strConnString)

            Dim sda As New SqlDataAdapter

            cmd.CommandType = CommandType.Text

            cmd.Connection = con

            Dim dt As New DataTable

            Try

                con.Open()

                sda.SelectCommand = cmd

                sda.Fill(dt)

            Catch ex As Exception

                dt = Nothing

            Finally

                con.Close()

                sda.Dispose()

                con.Dispose()

            End Try

            If dt IsNot Nothing Then
                Try
                    Dim fileName = CType(dt.Rows(0)("UniqueFilename"), String)
                    Dim bytes() As Byte = WMC.Logic.KYCFileHandler.GetFile(fileName)

                    Response.Buffer = True

                    Response.Charset = ""

                    Response.Cache.SetCacheability(HttpCacheability.NoCache)

                    'Response.ContentType = dt.Rows(0)("ContentType").ToString()

                    Response.ContentType = "image/png"

                    Response.AddHeader("content-disposition", "attachment;filename=" & dt.Rows(0)("OriginalFilename").ToString())

                    Response.BinaryWrite(bytes)

                    Response.Flush()

                    Response.End()
                Catch ex As Exception

                End Try



            End If

        End If
    End Sub
End Class
