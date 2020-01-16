Public Class CBFF2
    Private unknownStuff As Byte = 0
    Private textureShowedLength As Byte
    Private textureType As Byte
    Private transparencyPixelIndex As Byte
    Private textureWidth(1) As Byte
    Private texturePixelWidth(1) As Byte
    Private textureHeigth(1) As Byte
    Private instructionLineLength(3) As Byte
    Private nameLength As Byte
    Private name As Byte()
    Private compressedTexture As Boolean = True

    Private byteArray As Byte()
    Private bffHeader As Byte()
    Private bffPalette As Byte()
    Private bffData As Byte()

    'constructor
    Public Sub New(ByVal byteArray1 As Byte())
        byteArray = byteArray1
    End Sub

    Public Sub BFF2Init()
        'set textureShowedLength
        textureShowedLength = byteArray(8)

        'set transparencyPixelIndex and compressedTexture value
        Dim byte1 = byteArray(17)
        Dim byte2 = byteArray(18)
        transparencyPixelIndex = byte1 Mod 16 << 4
        transparencyPixelIndex += byte2 / 16
        IIf(byte2 Mod 16 > 7, compressedTexture = True, compressedTexture = False)

        'set textureType value
        textureType = byteArray(19)

        'set textureShowedLength value
        Array.Copy(byteArray, 20, textureWidth, 0, 2)

        'set textureWidth value
        Array.Copy(byteArray, 22, texturePixelWidth, 0, 2)

        'set texturePixelWidth value
        Array.Copy(byteArray, 26, textureWidth, 0, 2)

        'set textureheigth value
        Array.Copy(byteArray, 30, textureHeigth, 0, 2)

        'set inlstruction length value
        Array.Copy(byteArray, 32, instructionLineLength, 0, 4)

        'set name length value
        nameLength = byteArray(35)

        'read name of BFF2
        ReDim name(nameLength - 1)
        Array.Copy(byteArray, 36, name, 0, nameLength)

    End Sub

    Public Function getBFFName() As String
        Return System.Text.Encoding.UTF8.GetString(name)
    End Function

    Public Function getSize() As Integer
        Return byteArray.Length
    End Function

    Public Sub replaceBFF2(ByVal data As Byte())
        ReDim byteArray(data.Length - 1)
        byteArray = data
    End Sub

    Public Function getBFFContainerData() As Byte()
        Return byteArray
    End Function
End Class
