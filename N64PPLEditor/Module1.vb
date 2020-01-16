Imports System.IO

Module Module1

    Public pathExtractedTexture As String = My.Application.Info.DirectoryPath & "\extractedTexture\"
    Public pathCompressedTexture As String = My.Application.Info.DirectoryPath & "\compressedTexture\"
    Public pathReplacedTexture As String = My.Application.Info.DirectoryPath & "\replacedTexture\"

    Public Function convertByteArrayToInt(ByVal bytearray) As Integer
        Dim FinalNumber As Int32 = 0
        For index = 0 To bytearray.length - 1
            FinalNumber = FinalNumber << 8
            FinalNumber += Convert.ToInt32(bytearray(index))
        Next
        Return FinalNumber
    End Function

    Public Sub verifyExistingPath()
        If Not File.Exists(pathExtractedTexture) Then
            Directory.CreateDirectory(pathExtractedTexture)
        End If
        If Not File.Exists(pathCompressedTexture) Then
            Directory.CreateDirectory(pathCompressedTexture)
        End If
        If Not File.Exists(pathReplacedTexture) Then
            Directory.CreateDirectory(pathReplacedTexture)
        End If

    End Sub
End Module

