Imports System.Data.SqlClient
Imports System.Data
Imports System.Text


Namespace eQuote

    Public Module DBUtility

        Private moConnection As SqlConnection
        Private moTransaction As SqlTransaction


        Public Function OpenConnection() As SqlConnection

            moConnection = New SqlConnection()

            With moConnection
                .ConnectionString = ConfigurationManager.ConnectionStrings("LocalConnectionString").ToString
                .Open()
            End With

            Return moConnection

        End Function


        Public Sub BeginTransaction()

            If IsNothing(moTransaction) Then
                moTransaction = OpenConnection.BeginTransaction
            End If

        End Sub


        Public Sub CommitTransaction()

            If Not IsNothing(moTransaction) Then
                moTransaction.Commit()

                moTransaction = Nothing

                Closeconnection()
            End If

        End Sub


        Public Sub RollbackTransaction()

            If Not IsNothing(moTransaction) Then
                moTransaction.Rollback()

                moTransaction = Nothing

                Closeconnection()
            End If

        End Sub


        Public Sub Closeconnection()

            If IsNothing(moTransaction) Then
                moConnection.Close()
            End If

        End Sub


        Public Function GetDataReader(ByVal vsSQL As String) As SqlDataReader

            Dim loDataReader As SqlDataReader
            Dim loCommand As New SqlCommand(vsSQL, OpenConnection)

            loDataReader = loCommand.ExecuteReader(CommandBehavior.CloseConnection)

            Return loDataReader

        End Function


        Public Function GetDataSet(ByVal vsSQL As String) As DataSet

            Dim loDataAdapter As New SqlDataAdapter(vsSQL, OpenConnection)
            Dim loDataset As New DataSet()

            loDataAdapter.Fill(loDataset)

            Closeconnection()

            Return loDataset

        End Function


        Public Function Org_ExecuteScalar(ByVal vsSQL As String) As Object

            Dim loCommand As SqlCommand
            Dim loValue As Object

            loCommand = OpenConnection.CreateCommand


            loCommand.CommandTimeout = 120

            loCommand.CommandText = vsSQL
            loValue = loCommand.ExecuteScalar


            Closeconnection()
            loCommand.Connection.Close()

            Return loValue

        End Function



        Public Function ExecuteScalar(ByVal vsSQL As String) As Object

            Dim Connection As New SqlConnection(ConfigurationManager.ConnectionStrings("LocalConnectionString").ToString)
            Dim Command As SqlCommand = New SqlCommand(vsSQL, Connection)
            Dim result As Object = Nothing
            Try
                If Not (Connection.State = ConnectionState.Open) Then
                    Connection.Open()
                End If
                result = Command.ExecuteScalar
            Catch ex As Exception
                Connection.Close()
                Throw ex
            Finally
                Command.Dispose()
                Connection.Close()
            End Try
            Return result

        End Function



        Public Function ExecuteNonQuery(ByVal vsSQL As String) As Integer

            Dim loCommand As SqlCommand
            Dim liRowsAffected As Integer

            loCommand = OpenConnection.CreateCommand

            With loCommand
                If Not IsNothing(moTransaction) Then
                    .Transaction = moTransaction
                End If

                .CommandText = vsSQL
                liRowsAffected = .ExecuteNonQuery()
            End With

            'Closeconnection()
            loCommand.Connection.Close()

            Return liRowsAffected

        End Function

        Public Function ExecuteNonQueryGetNewID(ByVal vsSQL As String) As Integer

            Dim loCommand As SqlCommand
            Dim newID As Integer

            loCommand = OpenConnection.CreateCommand

            With loCommand
                If Not IsNothing(moTransaction) Then
                    .Transaction = moTransaction
                End If

                .CommandText = vsSQL
                .ExecuteNonQuery()
            End With

            newID = GetNewID(loCommand)
            'Closeconnection()
            loCommand.Connection.Close()

            Return newID

        End Function

        Public Sub ExecuteNonQuery(ByVal voCommand As SqlCommand)

            With voCommand
                If Not IsNothing(moTransaction) Then
                    .Transaction = moTransaction
                End If

                .Connection = OpenConnection()
                .ExecuteNonQuery()
            End With

            'Closeconnection()
            voCommand.Connection.Close()

        End Sub


    End Module

End Namespace
