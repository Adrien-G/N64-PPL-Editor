''' <summary>
'''  The class CSBF allow to read a SBF file
'''  Author : Adrien Garreau
'''  Date : 16/01/2020
''' </summary>

Public Class CSBF1

    Private nbBifComponent(3) As Byte
    Private bifNames As List(Of Byte())
    Private byteArray As Byte()
    Private nbScenes(3) As Byte
    Private sizeHeaderSBF As Integer

    'regroup all the scene present in one sbf file


    'constructor
    Public Sub New(ByVal byteArray1 As Byte())
        byteArray = byteArray1
    End Sub

    Public Sub InitSBF1()
        ' extract number of bif
        Array.Copy(byteArray, 4, nbBifComponent, 0, 2)

        'extract all bif names
        Dim bifName(15) As Byte
        bifNames = New List(Of Byte())
        Dim nbComponents = convertByteArrayToInt(nbBifComponent)
        For index = 0 To nbComponents - 1
            Array.Copy(byteArray, 8 + index * 16, bifName, 0, 16)
            bifNames.Add(bifName)
        Next

        'write number of scene
        Array.Copy(byteArray, 8 + (nbComponents + 1) * 16, nbScenes, 0, 4)
        sizeHeaderSBF = 12 + (nbComponents + 1) * 16

        ExtractAllScenes()
    End Sub

    Private Sub ExtractAllScenes()

    End Sub
End Class
