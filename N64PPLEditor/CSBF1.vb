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
    Private listScenes As List(Of CSBFScene)

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

        Dim arrayWithoutHeader(byteArray.Length - sizeHeaderSBF - 1) As Byte
        Array.Copy(byteArray, 0, arrayWithoutHeader, 0, byteArray.Length - sizeHeaderSBF)
        ExtractAllScenes(arrayWithoutHeader)
    End Sub

    'start extracting scenes one by one
    Private Sub ExtractAllScenes(ByVal byteArray As Byte())
        Dim sceneSeek As Integer = sizeHeaderSBF

        'create Objects
        listScenes = New List(Of CSBFScene)

        For index = 0 To convertByteArrayToInt(nbScenes)
            listScenes.Add(New CSBFScene(byteArray))
            listScenes(index).Init()
            sceneSeek += listScenes(index).SizeScene()
        Next
    End Sub
End Class
