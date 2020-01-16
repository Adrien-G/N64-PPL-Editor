Public Class CSBF1

    Private nbBifComponent(3) As Byte
    Private bifNames As List(Of Byte())
    Private byteArray As Byte()
    Private nbScenes(3) As Byte
    Private sizeHeaderSBF As Integer

    Private listInputScene As List(Of InputScene)
    Private listTextScene As List(Of CSBFTextScene)
    Private listTextureScene As List(Of CSBFTextureScene)


    'unknown data for now... need to explore the binary N64 PPL rom...
    'just know enought for "estimating" size of unknow data...
    Private Class InputScene

    End Class

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

        extractAllScenes()
    End Sub

    Private Sub extractAllScenes()
        'create Objects
        listInputScene = New List(Of InputScene)
        listTextScene = New List(Of CSBFTextScene)
        listTextureScene = New List(Of CSBFTextureScene)

        'extract all scene one by one
        For index = 0 To convertByteArrayToInt(nbScenes)

            'verify if data is present... (inputScene)


            'verify if Text is present... (textScene)


            'verify if texture is present... (texture scene)

            'get new adresse
        Next
    End Sub

    Private Sub InitSBFText()

    End Sub


End Class
