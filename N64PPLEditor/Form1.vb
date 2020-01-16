Imports System.IO


Public Class Form1
    Dim FStream As IO.FileStream
    Dim FStreamPalette As IO.FileStream





    'load rom
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Try
            FStream = New FileStream(TextBoxPPLRom.Text, IO.FileMode.Open, IO.FileAccess.Read)
            FStreamPalette = New IO.FileStream(TextBoxPPLRom.Text, IO.FileMode.Open, IO.FileAccess.Read)
            Button1.Enabled = False
            Button1.Text = "ROM Loaded"
            Timer1.Start()
        Catch ex As Exception
            MsgBox("Unable to open the file :(" & vbCrLf & "error details : " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    'quick function for reading names on BFF2 header.
    Public Function readBFF2Name(ByVal fstream2 As IO.FileStream, ByVal position As String)
        Dim tmpPos = fstream2.Position
        Dim nameSize As Byte
        Dim nameF As String = ""

        'get length of title
        fstream2.Position = position + 23
        nameSize = fstream2.ReadByte()

        'get title
        Dim nameFile(nameSize - 1) As Byte
        fstream2.Read(nameFile, 0, nameSize)
        nameF = System.Text.Encoding.UTF8.GetString(nameFile)

        If (nameFile(nameSize - 1) = 0) Then
            nameF = nameF.Substring(0, nameF.Length - 1)
        End If

        FStream.Position = tmpPos
        Return nameF
    End Function

    Private Function convertByteArrayToInt(ByVal bytearray) As Integer
        Dim FinalNumber As Int32 = 0
        For index = 0 To bytearray.length - 2
            FinalNumber += Convert.ToInt32(bytearray(index)) << 8
        Next
        FinalNumber += Convert.ToInt32(bytearray(bytearray.length - 1))
        Return FinalNumber
    End Function

    Private Function getBFFSize(ByVal positionDebutData As String)
        FStream.Position = Convert.ToInt32(positionDebutData, 16)
        Dim sizeBFF As Integer = 0
        Dim qtRead As Integer = 1024 * 300
        Dim octets(qtRead) As Byte
        Dim readedSize As Int32 = 0
        Do
            readedSize = FStream.Read(octets, 0, qtRead)
            For index = 0 To qtRead - 4
                If (octets(index) = 51 And octets(index + 1) = 70 And octets(index + 2) = 73 And octets(index + 3) = 66) Then '3FIB
                    Exit Do
                ElseIf (octets(index) = 66 And octets(index + 1) = 70 And octets(index + 2) = 70 And octets(index + 3) = 50) Then 'BFF2
                    sizeBFF -= 12
                    Exit Do
                ElseIf (octets(index) = 72 And octets(index + 1) = 86 And octets(index + 2) = 81 And octets(index + 3) = 77) Then 'HQVM
                    Exit Do
                ElseIf (octets(index) = 78 And octets(index + 1) = 54 And octets(index + 2) = 52 And octets(index + 3) = 32) Then 'N64 
                    Exit Do
                ElseIf (octets(index) = 83 And octets(index + 1) = 66 And octets(index + 2) = 70 And octets(index + 3) = 49) Then 'SBF1
                    Exit Do
                ElseIf (octets(index) = 80 And octets(index + 1) = 80 And octets(index + 2) = 76 And octets(index + 3) = 80) Then 'PPLP
                    Exit Do
                End If
                sizeBFF += 1
            Next
        Loop While readedSize > 0
        Return sizeBFF
    End Function

    Public Structure dataHeader
        Public BFFName As String
        Public nameLength As Integer
        Public texturePrintedLength As Byte
        Public textureType As Byte
        Public bytesPerPixel As Byte
        Public colorMode As Byte
        Public pixelBitLength As Byte
        Public isCompressed As Boolean
        Public indexedColor As Boolean
        Public sizeX, sizeY As Integer
        Public startingPalettePos As Integer
        Public startingDataPos As Integer
        Public paletteSize As Integer
        Public globalIdent As Integer
    End Structure

    Public Function readBFF2Header(ByVal fstream2 As IO.FileStream, ByVal positionStartBFF2 As String) As dataHeader
        Dim HeaderBFF2 As New dataHeader

        fstream2.Position = Convert.ToInt32(positionStartBFF2, 16) + 8
        HeaderBFF2.texturePrintedLength = fstream2.ReadByte()

        fstream2.Position = Convert.ToInt32(positionStartBFF2, 16) + 18

        Dim isCompressedValue = fstream2.ReadByte()
        If (isCompressedValue Mod 16 < 7) Then
            HeaderBFF2.isCompressed = False
        Else
            HeaderBFF2.isCompressed = True
        End If
        HeaderBFF2.textureType = fstream2.ReadByte()

        Select Case HeaderBFF2.textureType
            Case Convert.ToInt32(22, 16) ' RGBA : 4
                HeaderBFF2.colorMode = 4
                HeaderBFF2.bytesPerPixel = 0
                HeaderBFF2.pixelBitLength = 4
                HeaderBFF2.indexedColor = False
            Case Convert.ToInt32(23, 16) ' RGBA : 8
                HeaderBFF2.colorMode = 8
                HeaderBFF2.bytesPerPixel = 1
                HeaderBFF2.pixelBitLength = 8
                HeaderBFF2.indexedColor = False
            Case Convert.ToInt32(24, 16) ' RGB:8 (greyscale,RGB shared),A:8
                HeaderBFF2.colorMode = 16
                HeaderBFF2.bytesPerPixel = 2
                HeaderBFF2.pixelBitLength = 16
                HeaderBFF2.indexedColor = False
            Case Convert.ToInt32(32, 16) ' R:8,G:8,B:8,A:8 (16 bit) indexed in palette
                HeaderBFF2.colorMode = 32
                HeaderBFF2.bytesPerPixel = 4
                HeaderBFF2.pixelBitLength = 4
                HeaderBFF2.indexedColor = True
            Case Convert.ToInt32(33, 16) ' R:8,G:8,B:8,A:8 (32 bit) indexed in palette
                HeaderBFF2.colorMode = 32
                HeaderBFF2.bytesPerPixel = 4
                HeaderBFF2.pixelBitLength = 8
                HeaderBFF2.indexedColor = True
            Case Convert.ToInt32(54, 16) ' R:5,G:5,B:5,A:1 (16 bits)
                HeaderBFF2.colorMode = 16
                HeaderBFF2.bytesPerPixel = 2
                HeaderBFF2.pixelBitLength = 16
                HeaderBFF2.indexedColor = False
            Case Convert.ToInt32(55, 16) ' R:8,G:8,B:8,A:8 (32 bits)
                HeaderBFF2.colorMode = 32
                HeaderBFF2.bytesPerPixel = 4
                HeaderBFF2.pixelBitLength = 32
                HeaderBFF2.indexedColor = False
        End Select

        'read size of picture
        Dim bytesWidth(1) As Byte
        Dim bytesHeigth(1) As Byte

        fstream2.Position += 2
        fstream2.Read(bytesWidth, 0, 2)
        fstream2.Position += 2
        fstream2.Read(bytesHeigth, 0, 2)

        HeaderBFF2.sizeX = convertByteArrayToInt(bytesWidth)
        HeaderBFF2.sizeY = convertByteArrayToInt(bytesHeigth)

        'read nameSize for finding index of palette...
        fstream2.Position += 7
        Dim sizeName = fstream2.ReadByte()
        Dim nameF As String = ""
        Dim nameFile(sizeName - 1) As Byte
        fstream2.Read(nameFile, 0, sizeName)
        nameF = System.Text.Encoding.UTF8.GetString(nameFile)
        If (nameFile(sizeName - 1) = 0) Then
            nameF = nameF.Substring(0, nameF.Length - 1)
        End If
        HeaderBFF2.nameLength = sizeName
        HeaderBFF2.BFFName = nameF

        If HeaderBFF2.textureType = Convert.ToInt32(32, 16) Or HeaderBFF2.textureType = Convert.ToInt32(33, 16) Then
            Dim paletteSize(3) As Byte
            fstream2.Read(paletteSize, 0, 4)
            HeaderBFF2.paletteSize = convertByteArrayToInt(paletteSize) * HeaderBFF2.bytesPerPixel
            HeaderBFF2.startingDataPos = fstream2.Position + Convert.ToInt32(HeaderBFF2.paletteSize)
        Else
            HeaderBFF2.paletteSize = 0
            HeaderBFF2.startingDataPos = fstream2.Position
        End If
        HeaderBFF2.startingPalettePos = fstream2.Position
        HeaderBFF2.globalIdent = TreeView1.SelectedNode.Index

        Return HeaderBFF2
    End Function

    Public Function getColorFromPalette(ByVal position As Int32) As Byte()
        Dim tabPixel(3) As Byte
        Dim initPos = FStreamPalette.Position
        FStreamPalette.Position += position * 4

        FStreamPalette.Read(tabPixel, 0, 4)
        FStreamPalette.Position = initPos
        Return tabPixel
    End Function

    Public Function selectorGoodValue(ByVal readedByte) As Byte()
        Dim tab(1) As Byte
        If (readedByte >= 128 And readedByte <= 135) Then
            tab(0) = 1
            tab(1) = readedByte - 126
        ElseIf readedByte >= 192 And readedByte <= 199 Then
            tab(0) = 2
            tab(1) = readedByte - 190
        ElseIf readedByte >= 200 And readedByte <= 207 Then
            tab(0) = 3
            tab(1) = readedByte - 198
        ElseIf readedByte >= 208 And readedByte <= 215 Then
            tab(0) = 4
            tab(1) = readedByte - 206
        ElseIf readedByte >= 216 And readedByte <= 223 Then
            tab(0) = 5
            tab(1) = readedByte - 214
        ElseIf readedByte >= 224 And readedByte <= 231 Then
            tab(0) = 6
            tab(1) = readedByte - 222
        ElseIf readedByte >= 232 And readedByte <= 239 Then
            tab(0) = 7
            tab(1) = readedByte - 230
        ElseIf readedByte >= 240 And readedByte <= 247 Then
            tab(0) = 8
            tab(1) = readedByte - 238
        ElseIf readedByte >= 248 And readedByte <= 255 Then
            tab(0) = 9
            tab(1) = readedByte - 246
        End If
        Return tab
    End Function

    Public Sub readUncompressedContent(ByRef cursorCompressed, ByVal compressedTexture, ByVal nbByteToRead, ByRef cursorDecompressed, ByRef decompressedTexture, ByVal headerBFF2)
        cursorCompressed += 1
        Dim tabPixel(3) As Byte

        For counter = 0 To nbByteToRead
            If headerBFF2.indexedColor Then
                If headerBFF2.textureType = Convert.ToInt32(33, 16) Then
                    tabPixel = getColorFromPalette(compressedTexture(cursorCompressed))
                    'extract texture pixel from palette
                    For index = 0 To 3
                        decompressedTexture(cursorDecompressed + index) = tabPixel(index)
                    Next
                    cursorDecompressed += 4
                Else
                    tabPixel = getColorFromPalette(compressedTexture(cursorCompressed) / 16)

                    For index = 0 To 3
                        decompressedTexture(cursorDecompressed + index) = tabPixel(index)
                    Next
                    tabPixel = getColorFromPalette(compressedTexture(cursorCompressed) Mod 16)
                    For index = 0 To 3
                        decompressedTexture(cursorDecompressed + index + 4) = tabPixel(index)
                    Next
                    cursorDecompressed += 8
                End If
            Else
                decompressedTexture(cursorDecompressed) = compressedTexture(cursorCompressed)
                cursorDecompressed += 1
            End If
            cursorCompressed += 1
        Next
    End Sub

    Public Sub readCompressedContent(ByRef cursorCompressed, ByVal compressedTexture, ByVal totalQtPaquet, ByVal totalMultiplicator, ByRef cursorDecompressed, ByRef decompressedTexture, ByVal headerBFF2)
        cursorCompressed += 1
        Dim tabPixel(3) As Byte
        For multiplicator = 0 To totalMultiplicator - 1
            For qtPaquet = 0 To totalQtPaquet - 1
                'If bytePerPixel = 1 Then
                If headerBFF2.indexedColor Then
                    If headerBFF2.textureType = Convert.ToInt32(33, 16) Then
                        tabPixel = getColorFromPalette(compressedTexture(cursorCompressed + qtPaquet))
                        'extract texture pixel from palette
                        For index = 0 To 3
                            decompressedTexture(cursorDecompressed + index) = tabPixel(index)
                        Next
                        cursorDecompressed += 4
                    Else
                        tabPixel = getColorFromPalette(compressedTexture(cursorCompressed + qtPaquet) / 16)
                        For index = 0 To 3
                            decompressedTexture(cursorDecompressed + index) = tabPixel(index)
                        Next
                        tabPixel = getColorFromPalette(compressedTexture(cursorCompressed + qtPaquet) Mod 16)
                        For index = 0 To 3
                            decompressedTexture(cursorDecompressed + index + 4) = tabPixel(index)
                        Next
                        cursorDecompressed += 8
                    End If
                    
                Else
                    decompressedTexture(cursorDecompressed) = compressedTexture(cursorCompressed + qtPaquet)
                    cursorDecompressed += 1
                End If
            Next
        Next
        cursorCompressed += totalQtPaquet
    End Sub

    'a few data were not compressed...
    Public Function loadUncompressedData(ByVal headerBFF2 As dataHeader, ByVal textureSize As Int32) As Byte()
        'load final texture
        Dim texture(headerBFF2.sizeX * headerBFF2.sizeY * headerBFF2.bytesPerPixel) As Byte

        If (headerBFF2.indexedColor) Then
            'load indexedTexture
            FStream.Position = headerBFF2.startingDataPos
            Dim textureIndexed(headerBFF2.sizeX * headerBFF2.sizeY) As Byte
            FStream.Read(textureIndexed, 0, textureSize)

            FStreamPalette.Position = headerBFF2.startingPalettePos

            For index = 0 To textureSize - 1
                Dim tabPixel(3) As Byte
                tabPixel = getColorFromPalette(textureIndexed(index))
                texture(0 + 4 * index) = tabPixel(0)
                texture(1 + 4 * index) = tabPixel(1)
                texture(2 + 4 * index) = tabPixel(2)
                texture(3 + 4 * index) = tabPixel(3)
            Next
        Else
            FStream.Position = headerBFF2.startingDataPos
            FStream.Read(texture, 0, textureSize)
        End If
        Return texture
    End Function

    Private Function decompressTexture(ByVal headerBFF2 As dataHeader, ByVal textureSize As Int32) As Byte()
        If Not headerBFF2.isCompressed Then
            Return loadUncompressedData(headerBFF2, textureSize)
        End If

        FStream.Position = headerBFF2.startingDataPos
        FStreamPalette.Position = headerBFF2.startingPalettePos
        Dim cursorCompressed As Int32 = 0
        Dim cursorDecompressed As Int32 = 0

        Dim nibbleValues(1) As Byte
        Dim nibble1 As Byte = 0
        Dim nibble2 As Byte = 0

        Dim compressedTexture(textureSize) As Byte
        FStream.Read(compressedTexture, 0, textureSize)
        Dim decompressedTexture(headerBFF2.sizeX * headerBFF2.sizeY * headerBFF2.bytesPerPixel) As Byte
        Try
            Do
                If compressedTexture(cursorCompressed) < 128 Then
                    readUncompressedContent(cursorCompressed, compressedTexture, compressedTexture(cursorCompressed), cursorDecompressed, decompressedTexture, headerBFF2)
                Else
                    nibbleValues = selectorGoodValue(compressedTexture(cursorCompressed))
                    nibble1 = nibbleValues(0)
                    nibble2 = nibbleValues(1)
                    readCompressedContent(cursorCompressed, compressedTexture, nibble1, nibble2, cursorDecompressed, decompressedTexture, headerBFF2)
                End If
            Loop While cursorCompressed < textureSize - 1
        Catch ex As Exception
            ' MsgBox("The texture contains defaults. (BFF size too long)", MsgBoxStyle.Information)
        End Try
        Return decompressedTexture
    End Function

    Private Sub loadTexture(ByVal decompressedTexture As Byte(), ByVal headerBFF2 As dataHeader)
        Dim bmp = New Bitmap(headerBFF2.sizeX, headerBFF2.sizeY)
        Dim readingCursor As Int32 = 0
        Dim posX As Int32 = 0
        Dim posY As Int32 = 0

        Dim RGBAValues(3) As Byte

        Select Case headerBFF2.textureType
            Case Convert.ToInt32(22, 16) ' RGBA : 4
                ' ?? (unknow texture color repartition)
            Case Convert.ToInt32(23, 16) ' RGBA : 8
                ' ?? (unknow texture color repartition)
                'For i = 0 To decompressedTexture.Length - 1
                '    RGBAValues(0) = decompressedTexture(readingCursor)

                '    For index = 0 To 1
                '        Dim Red = RGBAValues(0) >> 6
                '        Dim Green = RGBAValues(0) << 2 >> 6
                '        Dim Blue = RGBAValues(0) << 4 >> 6
                '        Dim Alpha = RGBAValues(0) << 6 >> 6
                '        bmp.SetPixel(posX, posY, Color.FromArgb(Alpha * 64, Red * 64, Green * 64, Blue * 64))

                '        posX += 1
                '        If (posX = headerBFF2.sizeX) Then
                '            posX = 0
                '            posY += 1
                '        End If
                '    Next
                '    readingCursor += 1
                'Next
            Case Convert.ToInt32(24, 16) ' RGB:8 (greyscale,RGB shared),A:8
                For i = 0 To decompressedTexture.Length - 3 Step 2
                    RGBAValues(0) = decompressedTexture(readingCursor)
                    RGBAValues(1) = decompressedTexture(readingCursor + 1)
                    bmp.SetPixel(posX, posY, Color.FromArgb(RGBAValues(1), RGBAValues(0), RGBAValues(0), RGBAValues(0)))

                    posX += 1
                    If (posX = headerBFF2.sizeX) Then
                        posX = 0
                        posY += 1
                    End If
                    readingCursor += 2
                Next
            Case Convert.ToInt32(54, 16)  ' R:5,G:5,B:5,A:1 (16 bits)
                Dim Red, Green, Blue, Alpha As Int32

                For i = 0 To decompressedTexture.Length - 3 Step 2
                    RGBAValues(0) = decompressedTexture(readingCursor)
                    RGBAValues(1) = decompressedTexture(readingCursor + 1)

                    Red = RGBAValues(0) >> 3
                    Green = (RGBAValues(0) << 5 >> 3) + (RGBAValues(1) >> 6)
                    Blue = RGBAValues(1) << 2 >> 3
                    Alpha = RGBAValues(1) << 7 >> 7
                    bmp.SetPixel(posX, posY, Color.FromArgb(Alpha * 255, Red * 8, Green * 8, Blue * 8))

                    posX += 1
                    If (posX = headerBFF2.sizeX) Then
                        posX = 0
                        posY += 1
                    End If
                    readingCursor += 2
                Next
            Case Convert.ToInt32(32, 16), Convert.ToInt32(33, 16), Convert.ToInt32(55, 16) ' R:8,G:8,B:8,A:8 (32 bits)
                For i = 0 To decompressedTexture.Length - 5 Step 4
                    RGBAValues(0) = decompressedTexture(readingCursor)
                    RGBAValues(1) = decompressedTexture(readingCursor + 1)
                    RGBAValues(2) = decompressedTexture(readingCursor + 2)
                    RGBAValues(3) = decompressedTexture(readingCursor + 3)
                    bmp.SetPixel(posX, posY, Color.FromArgb(RGBAValues(3), RGBAValues(0), RGBAValues(1), RGBAValues(2)))
                    posX += 1
                    If (posX = headerBFF2.sizeX) Then
                        posX = 0
                        posY += 1
                    End If
                    readingCursor += 4
                Next
        End Select
        PictureBox1.Width = headerBFF2.sizeX
        PictureBox1.Height = headerBFF2.sizeY
        PictureBox1.Image = bmp
        bmp.Save(pathExtractedTexture & headerBFF2.globalIdent & "," & headerBFF2.BFFName & ".png", System.Drawing.Imaging.ImageFormat.Png)
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Timer1.Stop()
        FormLoading.Show()
        Dim qtRead As Integer = 1024 * 300
        Dim octets(qtRead) As Byte
        Dim sizeRead = FStream.Read(octets, 0, qtRead)

        'check when BFF2 or PPLP 
        For index = 0 To qtRead - 4
            If (octets(index) = 66 And octets(index + 1) = 70 And octets(index + 2) = 70 And octets(index + 3) = 50) Then 'BFF2
                TreeView1.Nodes.Add("BFF2 " & TreeView1.Nodes.Count & "_" & readBFF2Name(FStream, FStream.Position - qtRead + index) & "," & Hex(FStream.Position - qtRead + index - 12))
            ElseIf (octets(index) = 80 And octets(index + 1) = 80 And octets(index + 2) = 76 And octets(index + 3) = 80) Then 'PPLP
                TreeView1.Nodes(TreeView1.Nodes.Count - 1).ForeColor = Color.Green
            End If
        Next
        If sizeRead > 0 Then
            Timer1.Start()
        Else
            FormLoading.Close()
            GroupBox2.Enabled = True
            GroupBox3.Enabled = True
            GroupBox4.Enabled = True
            GroupBox5.Enabled = True
            GroupBox6.Enabled = True
            TreeView1.Enabled = True
            TreeView1.SelectedNode = TreeView1.Nodes(0)
        End If

    End Sub

    Private Sub TreeView1_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles TreeView1.AfterSelect
        Dim headerBFF2 As dataHeader = readBFF2Header(FStream, Split(TreeView1.SelectedNode.Text, ",")(1))
        Label2.Text = Hex(headerBFF2.bytesPerPixel)
        Label4.Text = Hex(headerBFF2.paletteSize)
        Label6.Text = Hex(headerBFF2.startingPalettePos)
        Label8.Text = Hex(headerBFF2.startingDataPos)
        Label10.Text = headerBFF2.sizeX
        Label12.Text = headerBFF2.sizeY
        Label14.Text = Hex(getBFFSize(Hex(headerBFF2.startingDataPos)))
        Label18.Text = headerBFF2.colorMode
        Label20.Text = headerBFF2.pixelBitLength
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim headerBFF2 = readBFF2Header(FStream, Split(TreeView1.SelectedNode.Text, ",")(1))
        FormLoading.Show()
        Dim decompressedTexture = decompressTexture(headerBFF2, getBFFSize(Hex(headerBFF2.startingDataPos)))
        loadTexture(decompressedTexture, headerBFF2)
        FormLoading.Hide()
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        TreeView1.SelectedNode = TreeView1.Nodes(0)
        FormLoading.Show()
        Timer2.Start()
    End Sub

    Private Sub Timer2_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer2.Tick
        Dim headerBFF2 = readBFF2Header(FStream, Split(TreeView1.SelectedNode.Text, ",")(1))
        Dim decompressedTexture = decompressTexture(headerBFF2, getBFFSize(Hex(headerBFF2.startingDataPos)))
        loadTexture(decompressedTexture, headerBFF2)
        If TreeView1.SelectedNode.Index < TreeView1.Nodes.Count - 1 Then
            Button4.Text = TreeView1.SelectedNode.Index & "/" & TreeView1.Nodes.Count
            TreeView1.SelectedNode = TreeView1.Nodes(TreeView1.SelectedNode.Index + 1)
        Else
            FormLoading.Hide()
            Button4.Text = "Extract all texture"
            Timer2.Stop()
            MsgBox(TreeView1.Nodes.Count & " textures extracted !", MsgBoxStyle.Information)
            Process.Start(pathExtractedTexture)
        End If
    End Sub

    Public Function findGreenAlpha() As Integer
        Dim posX = 0
        Dim posY = 0
        Dim bmp = New Bitmap(PictureBox1.Width, PictureBox1.Height)
        Dim palette(PictureBox1.Width * PictureBox1.Height) As Color
        Dim color As Color
        Dim doAdd As Boolean
        Dim indexAdd As Integer = 0
        bmp = PictureBox1.Image

        For indexY = 0 To bmp.Height - 1
            For indexX = 0 To bmp.Width - 1
                color = bmp.GetPixel(indexX, indexY)
                doAdd = True
                For index1 = 0 To indexAdd
                    If (color.R = palette(index1).R And color.G = palette(index1).G And color.B = palette(index1).B And color.A = palette(index1).A) Then
                        doAdd = False
                        Exit For
                    End If
                Next
                If doAdd Then
                    palette(indexAdd) = color
                    If (color.R = 0 And color.G = 255 And color.B = 0 And color.A = 255) Then
                        Return indexAdd
                    End If
                    indexAdd += 1
                End If
            Next
        Next
        Return 0
    End Function

    'Dim Headerdata(27 + BFFHeaderInfo.nameLength) As Byte
    Private Sub AddCompressorHeader(ByRef memoryTexture As MemoryStream, ByVal BFFHeaderInfo As dataHeader)
        'write BFF Header
        For index = 0 To 7
            memoryTexture.WriteByte(0)
        Next
        memoryTexture.WriteByte(BFFHeaderInfo.texturePrintedLength)
        For index = 0 To 2
            memoryTexture.WriteByte(0)
        Next
        ' write BFF2
        memoryTexture.WriteByte(66)
        memoryTexture.WriteByte(70)
        memoryTexture.WriteByte(70)
        memoryTexture.WriteByte(50)
        memoryTexture.WriteByte(0)

        'not always equal to 2...
        Dim nibble1 = 2

        Dim nibble2 = 0
        Dim nibble3 = 0
        'not sure what it was in binary...
        Dim nibble4 = 10

        If (BFFHeaderInfo.textureType = Convert.ToInt32(32, 16) Or BFFHeaderInfo.textureType = Convert.ToInt32(33, 16)) Then
            Dim greenColorIndex As Byte = findGreenAlpha()
            nibble2 = greenColorIndex / 16
            nibble3 = greenColorIndex Mod 16
        End If

        Dim byte1 As Byte = nibble1 << 4
        byte1 += nibble2

        Dim byte2 As Byte = nibble3 << 4
        byte2 += nibble4

        memoryTexture.WriteByte(byte1)
        memoryTexture.WriteByte(byte2)

        memoryTexture.WriteByte(BFFHeaderInfo.textureType)




        Dim sizeX As Short = BFFHeaderInfo.sizeX
        Dim texLarg1(1) As Byte
        texLarg1 = BitConverter.GetBytes(sizeX)
        If BitConverter.IsLittleEndian Then
            Array.Reverse(texLarg1)
        End If
        memoryTexture.Write(texLarg1, 0, 2)
        memoryTexture.Write(texLarg1, 0, 2)

        Dim texHeight1(3) As Byte
        texHeight1 = BitConverter.GetBytes(BFFHeaderInfo.sizeY)
        If BitConverter.IsLittleEndian Then
            Array.Reverse(texHeight1)
        End If
        memoryTexture.Write(texHeight1, 0, 4)

        Dim texLongLen(3) As Byte
        If (BFFHeaderInfo.textureType = Convert.ToInt32(32, 16)) Then
            texLongLen = BitConverter.GetBytes(BFFHeaderInfo.sizeX / 2)
        ElseIf BFFHeaderInfo.textureType = Convert.ToInt32(33, 16) Then
            texLongLen = BitConverter.GetBytes(BFFHeaderInfo.sizeX)
        Else
            texLongLen = BitConverter.GetBytes(BFFHeaderInfo.sizeX * BFFHeaderInfo.bytesPerPixel)
        End If

        If BitConverter.IsLittleEndian Then
            Array.Reverse(texLongLen)
        End If
        memoryTexture.Write(texLongLen, 0, 4)

        Dim nameLen(3) As Byte
        nameLen = BitConverter.GetBytes(BFFHeaderInfo.nameLength)
        If BitConverter.IsLittleEndian Then
            Array.Reverse(nameLen)
        End If
        memoryTexture.Write(nameLen, 0, 4)

        Dim nameStr(BFFHeaderInfo.nameLength) As Byte
        nameStr = System.Text.Encoding.ASCII.GetBytes(BFFHeaderInfo.BFFName)
        If nameStr.Length Mod 2 <> 0 Then
            ReDim Preserve nameStr(nameStr.Length + 1)
        End If
        memoryTexture.Write(nameStr, 0, BFFHeaderInfo.nameLength)


    End Sub

    Public Function getIndexFromColorPalette(ByVal paletteColors As Color(), ByVal searchedColor As Color) As Integer
        For index = 0 To paletteColors.Length - 1
            If (searchedColor.R = paletteColors(index).R And searchedColor.G = paletteColors(index).G And searchedColor.B = paletteColors(index).B And searchedColor.A = paletteColors(index).A) Then
                Return index
            End If
        Next
        Return 0
    End Function

    Private Function fillGoodFormatInMemory(ByVal decompressedTextureAsPicture As System.Drawing.Image, ByVal bffHeader As dataHeader, Optional ByVal palette As Color() = Nothing) As Byte()
        Dim totalSizeTex = PictureBox1.Width * PictureBox1.Height * bffHeader.bytesPerPixel
        Dim dataSource(totalSizeTex) As Byte

        Select Case bffHeader.textureType
            Case Convert.ToInt32(22, 16)
                ' ?? (unknow storage format...)
            Case Convert.ToInt32(23, 16)
                ' ?? (unknow storage format...)
            Case Convert.ToInt32(24, 16)
                Dim posX = 0
                Dim posY = 0
                Dim bmp = New Bitmap(PictureBox1.Width, PictureBox1.Height)
                bmp = PictureBox1.Image
                For index = 0 To totalSizeTex - 3 Step 2
                    Dim pixel As Color = bmp.GetPixel(posX, posY)
                    dataSource(index) = pixel.R
                    dataSource(index + 1) = pixel.A
                    posX += 1
                    If (posX = bmp.Width) Then
                        posX = 0
                        posY += 1
                    End If
                Next
            Case Convert.ToInt32(32, 16)
                ' TO-DO
            Case Convert.ToInt32(33, 16)
                ReDim dataSource(PictureBox1.Width * PictureBox1.Height)

                'write new data in palette
                Dim posX = 0
                Dim posY = 0
                Dim bmp = New Bitmap(PictureBox1.Width, PictureBox1.Height)
                bmp = PictureBox1.Image

                For index = 0 To PictureBox1.Width * PictureBox1.Height - 1
                    Dim pixel As Color = bmp.GetPixel(posX, posY)
                    Dim a = getIndexFromColorPalette(palette, pixel)
                    dataSource(index) = a

                    posX += 1
                    If (posX = bmp.Width) Then
                        posX = 0
                        posY += 1
                    End If
                Next

            Case Convert.ToInt32(54, 16)
                Dim posX = 0
                Dim posY = 0
                Dim bmp = New Bitmap(PictureBox1.Width, PictureBox1.Height)
                bmp = PictureBox1.Image
                For index = 0 To totalSizeTex - 3 Step 2
                    Dim pixel As Color = bmp.GetPixel(posX, posY)
                    Dim R, G1, G2, B, A As Byte
                    R = Math.Floor(pixel.R / 8)
                    G1 = Math.Floor(pixel.G / 8)
                    G1 = G1 >> 2
                    G2 = Math.Floor(pixel.G / 8)
                    G2 = G2 << 6
                    B = Math.Floor(pixel.B / 8)
                    A = Math.Floor(pixel.A / 255)
                    dataSource(index) = (R << 3)
                    dataSource(index) += G1
                    dataSource(index + 1) = G2 >> 1
                    dataSource(index + 1) += B
                    dataSource(index + 1) = dataSource(index + 1) << 1
                    dataSource(index + 1) += A
                    posX += 1
                    If (posX = bmp.Width) Then
                        posX = 0
                        posY += 1
                    End If
                Next
            Case Convert.ToInt32(55, 16)
                Dim posX = 0
                Dim posY = 0
                Dim bmp = New Bitmap(PictureBox1.Width, PictureBox1.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                bmp = PictureBox1.Image
                For index = 0 To totalSizeTex - 5 Step 4
                    Dim pixel As Color = bmp.GetPixel(posX, posY)
                    dataSource(index) = pixel.R
                    dataSource(index + 1) = pixel.G
                    dataSource(index + 2) = pixel.B
                    dataSource(index + 3) = pixel.A

                    posX += 1
                    If (posX = bmp.Width) Then
                        posX = 0
                        posY += 1
                    End If
                Next
        End Select

        Return dataSource
    End Function

    Private Function checkSameArray(ByVal array, ByVal size, ByVal index1, ByVal index2) As Boolean
        For increm = 0 To size - 1
            If (array(index1 + increm) <> array(index2 + increm)) Then
                Return False
            End If
        Next
        Return True
    End Function

    Public Function GetOctetForCompression(ByVal qtPixLu, ByVal qtRepet) As Byte
        Dim nib1 = 0
        Dim nib2 = 0

        Select Case qtPixLu
            Case 1
                nib1 = 8
            Case 2, 3
                nib1 = 12
            Case 4, 5
                nib1 = 13
            Case 6, 7
                nib1 = 14
            Case 8, 9
                nib1 = 15
        End Select

        nib2 = qtRepet - 2
        'si pair
        If qtPixLu Mod 2 And nib1 <> 8 Then
            nib2 += 8
        End If
        Dim finalByte As Byte = 0
        finalByte = nib1 << 4
        finalByte += nib2
        Return finalByte
    End Function

    Private Sub writeCompressedStream(ByRef compressedTexture, ByRef textureByte, ByVal indexGlobal, ByVal QtOctetLu, Optional ByVal QtRepetition = 0)
        If QtRepetition = 0 Then
            'not compressed part
            compressedTexture.writeByte(QtOctetLu - 1)
            compressedTexture.write(textureByte, indexGlobal - QtOctetLu, QtOctetLu)
        Else
            'compressed part
            compressedTexture.writeByte(GetOctetForCompression(QtOctetLu, QtRepetition))
            compressedTexture.write(textureByte, indexGlobal, QtOctetLu)
        End If
    End Sub

    Public Function addHeaderPalette(ByRef memoryTexture As MemoryStream) As Color()
        Dim posX = 0
        Dim posY = 0
        Dim bmp = New Bitmap(PictureBox1.Width, PictureBox1.Height)
        Dim palette(PictureBox1.Width * PictureBox1.Height) As Color
        Dim color As Color
        Dim doAdd As Boolean
        Dim indexAdd As Integer = 0
        bmp = PictureBox1.Image

        For indexY = 0 To bmp.Height - 1
            For indexX = 0 To bmp.Width - 1
                color = bmp.GetPixel(indexX, indexY)
                doAdd = True
                For index1 = 0 To indexAdd
                    If (color.R = palette(index1).R And color.G = palette(index1).G And color.B = palette(index1).B And color.A = palette(index1).A) Then
                        doAdd = False
                        Exit For
                    End If
                Next
                If doAdd Then
                    palette(indexAdd) = color
                    indexAdd += 1
                End If
            Next
        Next
        ReDim Preserve palette(indexAdd - 1)


        Dim headerSize(4) As Byte
        headerSize = BitConverter.GetBytes(indexAdd)
        If BitConverter.IsLittleEndian Then
            Array.Reverse(headerSize)
        End If

        'write palette in memorystream
        memoryTexture.Write(headerSize, 0, 4)

        'write colors...
        For index = 0 To palette.Length - 1
            memoryTexture.WriteByte(palette(index).R)
            memoryTexture.WriteByte(palette(index).G)
            memoryTexture.WriteByte(palette(index).B)
            memoryTexture.WriteByte(palette(index).A)
        Next

        Return palette
    End Function

    Private Sub PerformCompressionTask(ByVal texture As System.Drawing.Image, ByVal BFFHeaderInfo As dataHeader)

        'create a 20Mo memory storage...
        Dim compressedTexture As New MemoryStream(1024 * 1024 * 20)

        'add BFF2 header 
        AddCompressorHeader(compressedTexture, BFFHeaderInfo)

        Dim textureByte As Byte()
        If (BFFHeaderInfo.textureType = Convert.ToInt32(32, 16) Or BFFHeaderInfo.textureType = Convert.ToInt32(33, 16)) Then
            Dim palette = addHeaderPalette(compressedTexture)
            textureByte = fillGoodFormatInMemory(texture, BFFHeaderInfo, palette)
        Else
            textureByte = fillGoodFormatInMemory(texture, BFFHeaderInfo)
        End If

        'compress texture
        Dim sameArray = True
        Dim bound1Index As Integer = 0
        Dim boundSize As Byte = 0
        Dim bound2Index As Integer = 0
        Dim indexGlobal As Integer = 0
        Dim maxiQtOctetLu = 0
        Dim maxiQtRepetition = 0
        Dim lonelyPixelCount = 0

        Do
            For QtOctetLu = 1 To 9
                sameArray = True

                'set bounds for first compare
                bound1Index = indexGlobal
                boundSize = QtOctetLu

                'verify enought space whend end is coming...
                If (indexGlobal + QtOctetLu > textureByte.Length) Then
                    Exit For
                End If

                'check similarity
                For QtRepetition = 2 To 9
                    'set bounds for second compare
                    bound2Index = QtOctetLu * (QtRepetition - 1) + indexGlobal

                    'verify enought space whend end is coming...
                    If (QtOctetLu * (QtRepetition - 1) + indexGlobal + QtOctetLu > textureByte.Length) Then
                        Exit For
                    End If

                    'compare both array
                    If Not (checkSameArray(textureByte, boundSize, bound1Index, bound2Index)) Then
                        sameArray = False
                    End If

                    If sameArray Then
                        'check if bigger...
                        If ((maxiQtOctetLu * maxiQtRepetition) < QtOctetLu * QtRepetition) Then
                            maxiQtOctetLu = QtOctetLu
                            maxiQtRepetition = QtRepetition
                        End If
                    End If
                Next
            Next

            'lost pixel ?
            If (maxiQtOctetLu = 0 And maxiQtRepetition = 0) Then
                lonelyPixelCount += 1
                indexGlobal += 1
                If lonelyPixelCount = 128 Then
                    writeCompressedStream(compressedTexture, textureByte, indexGlobal, lonelyPixelCount)
                    lonelyPixelCount = 0
                End If
            Else
                If lonelyPixelCount > 0 Then
                    writeCompressedStream(compressedTexture, textureByte, indexGlobal, lonelyPixelCount)
                    lonelyPixelCount = 0
                End If
                writeCompressedStream(compressedTexture, textureByte, indexGlobal, maxiQtOctetLu, maxiQtRepetition)
                indexGlobal += maxiQtOctetLu * maxiQtRepetition
            End If
            maxiQtOctetLu = 0
            maxiQtRepetition = 0
        Loop While indexGlobal < textureByte.Length

        If lonelyPixelCount > 0 Then
            writeCompressedStream(compressedTexture, textureByte, indexGlobal, lonelyPixelCount)
        End If

        My.Computer.FileSystem.WriteAllBytes(pathCompressedTexture & BFFHeaderInfo.globalIdent & "," & BFFHeaderInfo.BFFName & ".ppltexture", compressedTexture.ToArray, False)
    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        Dim bffHeaderInfo As dataHeader = readBFF2Header(FStream, Split(TreeView1.SelectedNode.Text, ",")(1))
        Dim bmp
        Dim errorInLoad = False
        Try
            bmp = New Bitmap(pathReplacedTexture & bffHeaderInfo.globalIdent & "," & bffHeaderInfo.BFFName & ".png")
        Catch
            MsgBox("Unable to find PNG file.." & vbCrLf & vbCrLf & "Please put the modified texture in the replacedTexture folder with the same name. (" & bffHeaderInfo.globalIdent & "," & bffHeaderInfo.BFFName & ".png)", MsgBoxStyle.Exclamation)
            Process.Start(pathReplacedTexture)
            errorInLoad = True
        End Try
        If Not errorInLoad Then
            PictureBox1.Image = bmp
            PictureBox1.Width = bmp.Width
            PictureBox1.Height = bmp.Height
            FormLoading.Show()
            PerformCompressionTask(PictureBox1.Image, bffHeaderInfo)
            FormLoading.Hide()
        End If
    End Sub

    Private Sub Timer3_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer3.Tick
        Dim bffHeaderInfo As dataHeader = readBFF2Header(FStream, Split(TreeView1.SelectedNode.Text, ",")(1))
        Dim bmp
        Dim errorInLoad = False
        Try
            bmp = New Bitmap(pathReplacedTexture & bffHeaderInfo.globalIdent & "," & bffHeaderInfo.BFFName & ".png")
        Catch
            errorInLoad = True
        End Try
        If Not errorInLoad Then
            PictureBox1.Image = bmp
            PictureBox1.Width = bmp.Width
            PictureBox1.Height = bmp.Height
            FormLoading.Show()
            PerformCompressionTask(PictureBox1.Image, bffHeaderInfo)
            FormLoading.Hide()
        End If
        If TreeView1.SelectedNode.Index < TreeView1.Nodes.Count - 1 Then
            TreeView1.SelectedNode = TreeView1.Nodes(TreeView1.SelectedNode.Index + 1)
        Else
            Timer3.Stop()
        End If

    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        TreeView1.SelectedNode = TreeView1.Nodes(0)
        Timer3.Start()
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        Dim bffHeaderInfo As dataHeader
        Dim openFileDialog1 As New OpenFileDialog()
        openFileDialog1.InitialDirectory = pathReplacedTexture
        openFileDialog1.Filter = "png file (*.png)|*.png|bmp file (*.bmp)|*.bmp"
        openFileDialog1.FilterIndex = 1
        openFileDialog1.RestoreDirectory = True

        If openFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            ' Try
            Dim myStream As Stream = openFileDialog1.OpenFile()
            If (myStream IsNot Nothing) Then
                Dim bffName As String = openFileDialog1.SafeFileName
                bffHeaderInfo.BFFName = Split(bffName, ",")(1)
                bffHeaderInfo.BFFName = Split(bffHeaderInfo.BFFName, ".")(0)
                bffHeaderInfo.globalIdent = 9999
                Select Case ComboBox1.SelectedIndex
                    Case 0 ' 22
                        bffHeaderInfo.textureType = Convert.ToInt32(22, 16)
                        bffHeaderInfo.colorMode = 4
                        bffHeaderInfo.bytesPerPixel = 0
                        bffHeaderInfo.pixelBitLength = 4
                        bffHeaderInfo.indexedColor = False
                    Case 1 ' 23
                        bffHeaderInfo.textureType = Convert.ToInt32(23, 16)
                        bffHeaderInfo.colorMode = 8
                        bffHeaderInfo.bytesPerPixel = 1
                        bffHeaderInfo.pixelBitLength = 8
                        bffHeaderInfo.indexedColor = False
                    Case 2 ' 24
                        bffHeaderInfo.textureType = Convert.ToInt32(24, 16)
                        bffHeaderInfo.colorMode = 16
                        bffHeaderInfo.bytesPerPixel = 2
                        bffHeaderInfo.pixelBitLength = 16
                        bffHeaderInfo.indexedColor = False
                    Case 3 ' 32
                        bffHeaderInfo.textureType = Convert.ToInt32(32, 16)
                        bffHeaderInfo.colorMode = 32
                        bffHeaderInfo.bytesPerPixel = 4
                        bffHeaderInfo.pixelBitLength = 4
                        bffHeaderInfo.indexedColor = True
                    Case 4 ' 33
                        bffHeaderInfo.textureType = Convert.ToInt32(33, 16)
                        bffHeaderInfo.colorMode = 32
                        bffHeaderInfo.bytesPerPixel = 4
                        bffHeaderInfo.pixelBitLength = 8
                        bffHeaderInfo.indexedColor = True
                    Case 5 ' 54
                        bffHeaderInfo.textureType = Convert.ToInt32(54, 16)
                        bffHeaderInfo.colorMode = 16
                        bffHeaderInfo.bytesPerPixel = 2
                        bffHeaderInfo.pixelBitLength = 16
                        bffHeaderInfo.indexedColor = False
                    Case 6 ' 55
                        bffHeaderInfo.textureType = Convert.ToInt32(55, 16)
                        bffHeaderInfo.colorMode = 32
                        bffHeaderInfo.bytesPerPixel = 4
                        bffHeaderInfo.pixelBitLength = 32
                        bffHeaderInfo.indexedColor = False
                End Select
                bffHeaderInfo.isCompressed = True
                bffHeaderInfo.startingDataPos = 0
                bffHeaderInfo.startingPalettePos = 0
                bffHeaderInfo.paletteSize = 0
                bffHeaderInfo.nameLength = bffHeaderInfo.BFFName.Length
                If Not (bffHeaderInfo.BFFName.Length Mod 2) Then
                    bffHeaderInfo.nameLength += 1
                End If
                bffHeaderInfo.texturePrintedLength = 1


                Dim bmp = New Bitmap(openFileDialog1.FileName)

                bffHeaderInfo.sizeX = bmp.Width
                bffHeaderInfo.sizeY = bmp.Height

                PictureBox1.Image = bmp
                PictureBox1.Width = bmp.Width
                PictureBox1.Height = bmp.Height
                PerformCompressionTask(PictureBox1.Image, bffHeaderInfo)


            End If
            ' Catch ex As Exception
            '    MessageBox.Show("Cannot read file from disk. " & vbCrLf & "Original error: " & ex.Message)
            'End Try
        End If
    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        FormSelectAppli.Show()
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ComboBox1.SelectedIndex = 0
    End Sub
End Class
