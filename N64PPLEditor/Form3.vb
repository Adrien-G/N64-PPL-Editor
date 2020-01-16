Imports System.IO

Public Class Form3

    Dim FStream As IO.FileStream

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Try
            FStream = New FileStream(TextBoxPPLRom.Text, IO.FileMode.Open, IO.FileAccess.Read)
            Button1.Enabled = False
            Button1.Text = "ROM Loaded"
            Timer1.Start()
        Catch ex As Exception
            MsgBox("Unable to open the file :(" & vbCrLf & "error details : " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Function convertByteArrayToInt(ByVal bytearray) As Integer
        Dim FinalNumber As Int32 = 0
        For index = 0 To bytearray.length - 2
            FinalNumber += Convert.ToInt32(bytearray(index)) << 8
        Next
        FinalNumber += Convert.ToInt32(bytearray(bytearray.length - 1))
        Return FinalNumber
    End Function

    Public Sub decompressDataHeader(ByVal fstream2 As System.IO.Stream, ByVal position As Integer)
        Dim tmpPos = fstream2.Position
        fstream2.Position = position + 4
        Dim itemsCount(3) As Byte
        For index = 0 To itemsCount.Length - 1
            itemsCount(index) = fstream.ReadByte()
        Next
        Dim nbItems = convertByteArrayToInt(itemsCount)
        For index = 1 To nbItems
            Dim name(16) As Byte
            FStream.Read(name, 0, 16)
            Dim nameS As String = System.Text.Encoding.UTF8.GetString(name)

            TreeView1.Nodes(TreeView1.Nodes.Count - 1).Nodes.Add(nameS)
        Next
        fstream2.Position = tmpPos
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Timer1.Stop()
        FormLoading.Show()
        Dim qtRead As Integer = 1024 * 300
        Dim octets(qtRead) As Byte
        Dim sizeRead = FStream.Read(octets, 0, qtRead)

        'check when BFF2 or PPLP 
        For index = 0 To qtRead - 4
            If (octets(index) = 83 And octets(index + 1) = 66 And octets(index + 2) = 70 And octets(index + 3) = 49) Then 'SBF1
                TreeView1.Nodes.Add("SBF1 SCENE" & TreeView1.Nodes.Count + 1 & "," & Hex(FStream.Position - qtRead + index))
                decompressDataHeader(FStream, FStream.Position - qtRead + index)
            ElseIf (octets(index) = 80 And octets(index + 1) = 80 And octets(index + 2) = 76 And octets(index + 3) = 80) Then 'PPLP
                Try
                    TreeView1.Nodes(TreeView1.Nodes.Count - 1).ForeColor = Color.Green
                Catch
                End Try
            End If
        Next
        If sizeRead > 0 Then
            Timer1.Start()
        Else
            FormLoading.Close()
            TreeView1.Enabled = True
            TreeView1.SelectedNode = TreeView1.Nodes(0)
        End If

    End Sub
End Class