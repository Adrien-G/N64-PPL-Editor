Imports System.IO

Public Class Form2

    Dim FStream As IO.FileStream
    Dim startRessourcesIndex As Integer = 0
    Dim endRessourceIndex As Integer = 0
    Dim fibStorage As CBIFList


    Private Sub Form2_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        FormSelectAppli.Show()
        Try
            FStream.Close()
        Catch
        End Try
    End Sub
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Try
            FStream = New FileStream(TextBoxPPLRom.Text, IO.FileMode.Open, IO.FileAccess.ReadWrite)
            Button1.Enabled = False
            Button1.Text = "ROM Loaded"
            Timer1.Start()
        Catch ex As Exception
            MsgBox("Unable to open the file :(" & vbCrLf & "error details : " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Public Function ReadHeader(ByVal fstream2 As IO.FileStream, ByVal nameHeader As String, ByVal position As String) As String
        Dim tmpPos = fstream2.Position
        Dim nameSize As Byte
        Dim name As String = ""
        If nameHeader = "3FIB" Then
            'get length of title
            fstream2.Position = position + 16
            nameSize = fstream2.ReadByte()
            If nameSize = 0 Then
                Return ""
            End If
            'get title

            fstream2.Position = position + 20
            Dim nameFile(nameSize - 1) As Byte

            fstream2.Read(nameFile, 0, nameSize)

            name = System.Text.Encoding.UTF8.GetString(nameFile)
            If (nameFile(nameSize - 1) = 0) Then
                name = name.Substring(0, name.Length - 1)
            End If
        End If
        If nameHeader = "BFF2" Then
            'get length of title
            fstream2.Position = position + 23
            nameSize = fstream2.ReadByte()

            If nameSize = 0 Then
                MsgBox("Unable to find BFF2 entry !", MsgBoxStyle.Exclamation)
            Else
                'get title
                Dim nameFile(nameSize - 1) As Byte
                fstream2.Read(nameFile, 0, nameSize)
                name = System.Text.Encoding.UTF8.GetString(nameFile)
                If (nameFile(nameSize - 1) = 0) Then
                    name = name.Substring(0, name.Length - 1)
                End If
            End If
        End If
        FStream.Position = tmpPos
        Return name
    End Function

    Public Sub OperateArray(ByVal indexPos)
        FStream.Position = indexPos
        Dim nbElement(3) As Byte
        FStream.Read(nbElement, 0, 4)
        fibStorage = New CBIFList(FStream.Position, endRessourceIndex, convertByteArrayToInt(nbElement), FStream)
        fibStorage.Load3FIBList()
        fibStorage.Set3fibChunks()
        For index = 0 To fibStorage.GetNbItem - 1
            TreeView1.Nodes.Add(fibStorage.Get3FIBData(index).GetFibName)
            For index2 = 0 To fibStorage.Get3FIBData(index).GetBffCount() - 1
                TreeView1.Nodes(index).Nodes.Add(fibStorage.Get3FIBData(index).GetBFFData(index2).GetBFFName())
            Next
        Next
        Label3.Text = Hex(endRessourceIndex - startRessourcesIndex - fibStorage.GetSize())
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Timer1.Stop()
        FormLoading.Show()
        Dim qtRead As Integer = 1024 * 1024 * 30 '300 
        Dim octets(qtRead) As Byte
        Dim sizeRead = FStream.Read(octets, 0, qtRead)


        For index = 0 To qtRead - 15
            'seach ABRA.BIF
            If (octets(index) = 65 And octets(index + 1) = 66 And octets(index + 2) = 82 And octets(index + 3) = 65 And octets(index + 4) = 46 And octets(index + 5) = 66 And octets(index + 6) = 73 And octets(index + 7) = 70 And octets(index + 8) = 0 And octets(index + 9) = 0 And octets(index + 10) = 0 And octets(index + 11) = 0 And octets(index + 12) = 0 And octets(index + 13) = 0 And octets(index + 14) = 0) Then
                startRessourcesIndex = FStream.Position - qtRead + index - 12
                Label1.Text = Hex(startRessourcesIndex)
            End If
            'Search N64 PtrTable
            If (octets(index) = 78 And octets(index + 1) = 54 And octets(index + 2) = 52 And octets(index + 3) = 32 And octets(index + 4) = 80 And octets(index + 5) = 116 And octets(index + 6) = 114 And octets(index + 7) = 84 And octets(index + 8) = 97 And octets(index + 9) = 98 And octets(index + 10) = 108 And octets(index + 11) = 101 And octets(index + 12) = 115 And octets(index + 13) = 86 And octets(index + 14) = 50) Then
                endRessourceIndex = FStream.Position - qtRead + index - 16
                Label2.Text = Hex(endRessourceIndex)
                Exit For
            End If
        Next
        If sizeRead > 0 And endRessourceIndex = 0 Then
            Timer1.Start()
        Else
            OperateArray(startRessourcesIndex)
            FormLoading.Close()
            TreeView1.SelectedNode = TreeView1.Nodes(0)
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        fibStorage.Remove3fib(TreeView1.SelectedNode.Index)
        TreeView1.SelectedNode.Remove()
        Label3.Text = Hex(endRessourceIndex - startRessourcesIndex - fibStorage.GetSize())
    End Sub

    Private Sub TreeView1_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles TreeView1.AfterSelect
        If TreeView1.SelectedNode.Level = 0 Then
            Button2.Enabled = True
            Button4.Enabled = False
            Button6.Enabled = False
        ElseIf TreeView1.SelectedNode.Level = 1 Then
            Button2.Enabled = False
            Button4.Enabled = True
            Button6.Enabled = True
        End If
    End Sub

    Private Function AskFileToUser(ByVal Optional filter As String = "ppl compressed texture (*.ppltexture)|*.ppltexture") As Byte()
        Dim openFileDialog1 As New OpenFileDialog()
        openFileDialog1.InitialDirectory = pathCompressedTexture
        openFileDialog1.Filter = filter
        openFileDialog1.FilterIndex = 1

        If openFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            Try
                Dim dataBytes As Byte() = FileIO.FileSystem.ReadAllBytes(openFileDialog1.FileName)
                Return dataBytes

            Catch ex As Exception
                MsgBox("Unable to read file :S" & vbCrLf & "Details : " & ex.Message)
            End Try
        End If
        Return Nothing
    End Function

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Dim dataBytes As Byte() = AskFileToUser()
        fibStorage.ReplaceBFF2(TreeView1.SelectedNode.Parent.Index, TreeView1.SelectedNode.Index, dataBytes)
        Label3.Text = Hex(endRessourceIndex - startRessourcesIndex - fibStorage.GetSize())
    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        fibStorage.Removebff2(TreeView1.SelectedNode.Parent.Index, TreeView1.SelectedNode.Index)
        TreeView1.SelectedNode.Remove()
        Label3.Text = Hex(endRessourceIndex - startRessourcesIndex - fibStorage.GetSize())
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        If TreeView1.SelectedNode.Level = 0 Then
            TreeView1.SelectedNode.Nodes.Add("(empty)")
            TreeView1.SelectedNode.Expand()
            Dim dataBytes As Byte() = AskFileToUser()
            fibStorage.ReplaceBFF2(TreeView1.SelectedNode.Index, TreeView1.SelectedNode.Nodes(TreeView1.SelectedNode.Nodes.Count - 1).Index, dataBytes)
        Else
            TreeView1.Nodes(TreeView1.SelectedNode.Parent.Index).Nodes.Add("(empty)")
            Dim dataBytes As Byte() = AskFileToUser()
            fibStorage.ReplaceBFF2(TreeView1.SelectedNode.Parent.Index, TreeView1.SelectedNode.Index, dataBytes)
        End If
    End Sub

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        fibStorage.WriteAllData()
        Button7.BackColor = Color.LimeGreen
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        Dim dataBytes As Byte() = AskFileToUser("ppl movie (*.hvqm)|*.hvqm")
        fibStorage.fillRawData(TreeView1.SelectedNode.Index, dataBytes)
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        My.Computer.FileSystem.WriteAllBytes(pathOtherContent & fibStorage.Get3FIBData(TreeView1.SelectedNode.Index).bifName, fibStorage.GetRawData(TreeView1.SelectedNode.Index), False)

    End Sub
End Class