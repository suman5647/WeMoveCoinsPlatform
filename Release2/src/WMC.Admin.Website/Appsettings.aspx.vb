Imports System.Data
Imports WMC.Logic
Imports WMC.Logic.Models

<WMC.PageAuth(True, "Admin")>
Partial Class Appsettings
    Inherits WMC.SecurePage

    Private Sub GridViewAppSettings_RowDataBound(sender As Object, e As GridViewEditEventArgs) Handles GridViewAppSettings.RowEditing
        If e.NewEditIndex Then
            GridViewAppSettings.EditIndex = e.NewEditIndex
            Me.BindMyData()
        End If
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindMyData()
        End If
    End Sub
    Protected Sub GridViewAppSettings_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridViewAppSettings.RowCommand
        If e.CommandName = "Update" Then
            Dim row As GridViewRow = CType(CType(e.CommandSource, Control).NamingContainer, GridViewRow)
            Dim key = row.Cells(1).Text
            Dim textValue As TextBox = GridViewAppSettings.Rows(Convert.ToInt32(e.CommandArgument)).FindControl("TextConfigValue")
            Dim configValue = textValue.Text
            Dim settingsManager As ISettingsManager = WMC.Logic.SettingsManager.GetDefault()
            Dim DecryptValue As SettingsValue = SettingsManager.Update(key, configValue)

        End If
    End Sub

    Protected Sub GridViewAppSettings_RowCancelingEdit(sender As Object, e As GridViewCancelEditEventArgs) Handles GridViewAppSettings.RowCancelingEdit
        GridViewAppSettings.EditIndex = -1
        Me.BindMyData()
    End Sub

    Protected Sub GridViewAppSettings_RowUpdating(sender As Object, e As GridViewUpdateEventArgs) Handles GridViewAppSettings.RowUpdating
        GridViewAppSettings.EditIndex = -1
        Me.BindMyData()
    End Sub

    Private Sub BindMyData()
        Dim Appsettings = eQuote.GetDataSet("SELECT Id,ConfigKey,ConfigValue,ConfigDescription,IsEncrypted FROM [AppSettings]")
        Dim rowIndex As Integer = 0
        For Each Row As DataRow In Appsettings.Tables(0).Rows
            For Each Coll As DataColumn In Appsettings.Tables(0).Columns
                Dim s As String = Row(Coll.ColumnName).ToString()
                If s = "True" Then
                    Dim configValue = Row.ItemArray(2)
                    Dim DecryptValue = SecurityUtil.Decrypt(configValue)
                    Appsettings.Tables(0).Rows(rowIndex).Item("ConfigValue") = DecryptValue
                End If
            Next
            rowIndex += 1
        Next
        GridViewAppSettings.DataSource = Appsettings
        GridViewAppSettings.DataBind()
    End Sub



End Class
