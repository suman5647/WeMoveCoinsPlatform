Imports System
Imports System.Collections.Generic
Imports System.Data.SqlTypes
Imports System.IO
Imports System.Linq
Imports System.Reflection
Imports System.Text
Imports Microsoft.VisualBasic

Public Class CsvExport
    ''' <summary>
    ''' To keep the ordered list of column names
    ''' </summary>
    Private fields As New List(Of String)()

    ''' <summary>
    ''' The list of rows
    ''' </summary>
    Private rows As New List(Of Dictionary(Of String, Object))()

    Public Property IncludeHeaders() As Boolean
        Get
            Return m_IncludeHeaders
        End Get
        Set
            m_IncludeHeaders = Value
        End Set
    End Property
    Private m_IncludeHeaders As Boolean
    ''' <summary>
    ''' The current row
    ''' </summary>
    Private ReadOnly Property currentRow() As Dictionary(Of String, Object)
        Get
            Return rows(rows.Count - 1)
        End Get
    End Property

    ''' <summary>
    ''' The string used to separate columns in the output
    ''' </summary>
    Private ReadOnly columnSeparator As String

    ''' <summary>
    ''' Whether to include the preamble that declares which column separator is used in the output
    ''' </summary>
    Private ReadOnly includeColumnSeparatorDefinitionPreamble As Boolean

    ''' <summary>
    ''' Initializes a new instance of the <see cref="Jitbit.Utils.CsvExport"/> class.
    ''' </summary>
    ''' <param name="columnSeparator">
    ''' The string used to separate columns in the output.
    ''' By default this is a comma so that the generated output is a CSV file.
    ''' </param>
    ''' <param name="includeColumnSeparatorDefinitionPreamble">
    ''' Whether to include the preamble that declares which column separator is used in the output.
    ''' By default this is <c>true</c> so that Excel can open the generated CSV
    ''' without asking the user to specify the delimiter used in the file.
    ''' </param>
    Public Sub New(Optional columnSeparator As String = ",", Optional includeColumnSeparatorDefinitionPreamble As Boolean = True)
        Me.columnSeparator = columnSeparator
        Me.includeColumnSeparatorDefinitionPreamble = includeColumnSeparatorDefinitionPreamble
    End Sub

    ''' <summary>
    ''' Set a value on this column
    ''' </summary>
    Default Public WriteOnly Property Item(field As String) As Object
        Set
            ' Keep track of the field names, because the dictionary loses the ordering
            If Not fields.Contains(field) Then
                fields.Add(field)
            End If
            currentRow(field) = Value
        End Set
    End Property

    ''' <summary>
    ''' Call this before setting any fields on a row
    ''' </summary>
    Public Sub AddRow()
        rows.Add(New Dictionary(Of String, Object)())
    End Sub

    ''' <summary>
    ''' Add a list of typed objects, maps object properties to CsvFields
    ''' </summary>
    Public Sub AddRows(Of T)(list As IEnumerable(Of T))
        If list.Any() Then
            For Each obj As T In list
                AddRow()
                Dim values = obj.[GetType]().GetProperties()
                For Each value As PropertyInfo In values
                    Me(value.Name) = value.GetValue(obj, Nothing)
                Next
            Next
        End If
    End Sub

    ''' <summary>
    ''' Converts a value to how it should output in a csv file
    ''' If it has a comma, it needs surrounding with double quotes
    ''' Eg Sydney, Australia -> "Sydney, Australia"
    ''' Also if it contains any double quotes ("), then they need to be replaced with quad quotes[sic] ("")
    ''' Eg "Dangerous Dan" McGrew -> """Dangerous Dan"" McGrew"
    ''' </summary>
    ''' <param name="columnSeparator">
    ''' The string used to separate columns in the output.
    ''' By default this is a comma so that the generated output is a CSV document.
    ''' </param>
    Public Shared Function MakeValueCsvFriendly(value As Object, Optional columnSeparator As String = ",") As String
        If value Is Nothing Then
            Return ""
        End If
        If TypeOf value Is INullable AndAlso DirectCast(value, INullable).IsNull Then
            Return ""
        End If
        If TypeOf value Is DateTime Then
            If DirectCast(value, DateTime).TimeOfDay.TotalSeconds = 0 Then
                Return DirectCast(value, DateTime).ToString("yyyy-MM-dd")
            End If
            Return DirectCast(value, DateTime).ToString("yyyy-MM-dd HH:mm:ss")
        End If
        Dim output As String = value.ToString().Trim()
        If output.Contains(columnSeparator) OrElse output.Contains("""") OrElse output.Contains(vbLf) OrElse output.Contains(vbCr) Then
            output = """"c + output.Replace("""", """""") + """"c
        End If

        If output.Length > 30000 Then
            'cropping value for stupid Excel
            If output.EndsWith("""") Then
                output = output.Substring(0, 30000)
                If output.EndsWith("""") AndAlso Not output.EndsWith("""""") Then
                    'rare situation when cropped line ends with a '"'
                    output += """"
                End If
                'add another '"' to escape it
                output += """"
            Else
                output = output.Substring(0, 30000)
            End If
        End If
        Return output
    End Function

    ''' <summary>
    ''' Outputs all rows as a CSV, returning one string at a time
    ''' </summary>
    Private Function ExportToLines() As IEnumerable(Of String)
        Dim result = New List(Of String)()
        If includeColumnSeparatorDefinitionPreamble Then
            result.Add("sep=" + columnSeparator)
        End If

        ' The header
        If IncludeHeaders Then
            result.Add(String.Join(columnSeparator, fields))
        End If

        ' The rows
        For Each row As Dictionary(Of String, Object) In rows
            For Each k As String In fields.Where(Function(f) Not row.ContainsKey(f))
                row(k) = Nothing
            Next
            result.Add(String.Join(columnSeparator, fields.[Select](Function(field) MakeValueCsvFriendly(row(field), columnSeparator))))
        Next
        Return result

        '' The header
        'If IncludeHeaders Then
        '    Return String.Join(columnSeparator, fields)
        'End If

        '' The rows
        'For Each row As Dictionary(Of String, Object) In rows
        '    For Each k As String In fields.Where(Function(f) Not row.ContainsKey(f))
        '        row(k) = Nothing
        '    Next
        '    String.Join(columnSeparator, fields.Select(Function(field) MakeValueCsvFriendly(row(field), columnSeparator)))
        '    Return String.Join(columnSeparator, fields.Select(Function(field) MakeValueCsvFriendly(row(field), columnSeparator)))
        'Next
    End Function

    Public Function ExportHeader() As String
        Return String.Join(columnSeparator, fields)
    End Function

    ''' <summary>
    ''' Output all rows as a CSV returning a string
    ''' </summary>
    Public Function Export() As String
        Dim sb As New StringBuilder()

        For Each line As String In ExportToLines()
            sb.AppendLine(line)
        Next

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' Exports to a file
    ''' </summary>
    Public Sub ExportToFile(path As String)
        File.WriteAllLines(path, ExportToLines(), Encoding.UTF8)
    End Sub

    ''' <summary>
    ''' Exports as raw UTF8 bytes
    ''' </summary>
    Public Function ExportToBytes() As Byte()
        Dim data = Encoding.UTF8.GetBytes(Export())
        Return Encoding.UTF8.GetPreamble().Concat(data).ToArray()
    End Function
End Class
