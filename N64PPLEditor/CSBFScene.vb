''' <summary>
'''  The class CSBFScene allow to read a scene inside a SBF file (composed of input, text and texture content)
'''  Author : Adrien Garreau
'''  Date : 16/01/2020
''' </summary>

Public Class CSBFScene

    Private sceneName As Byte()
    Private byteArray As Byte()
    Private sceneSize As Integer

    '3 different elements by scene
    Private listInputScene As List(Of CSBFSceneInput)
    Private listTextScene As List(Of CSBFSceneText)
    Private listTextureScene As List(Of CSBFSceneTexture)

    'constructor
    Public Sub New(ByVal byteArray1 As Byte())
        byteArray = byteArray1
    End Sub

    Public Sub Init()
        'get size of scene name
        Dim sizeSceneName(3) As Byte
        Array.Copy(byteArray, 4, sizeSceneName, 0, 4)

        'grab the name
        Array.Copy(byteArray, 0, sceneName, 0, convertByteArrayToInt(sizeSceneName))
    End Sub

    Private Sub ExtractAScene()

        Dim generalSeek As Integer = 8 + sceneName.Length

        'import the Input scenes
        listInputScene = New List(Of CSBFSceneInput)
        ExtractSceneInput(generalSeek)

        'import Text scenes
        listTextScene = New List(Of CSBFSceneText)
        ExtractSceneText(generalSeek)

        'import Texture scenes
        listTextureScene = New List(Of CSBFSceneTexture)
        ExtractSceneTexture(generalSeek)

        'store scene in the byte array
        Dim tmpByteArray(generalSeek - 1) As Byte
        Array.Copy(byteArray, 0, tmpByteArray, 0, generalSeek)
        ReDim byteArray(generalSeek - 1)
        byteArray = tmpByteArray

        'store the size of the scene
        sceneSize = generalSeek
    End Sub

    Private Sub ExtractSceneInput(ByRef generalSeek)
        'determine number of input in the scene
        Dim nbInputScene(3) As Byte
        Array.Copy(byteArray, generalSeek, nbInputScene, 0, 4)
        generalSeek += 4

        'import the Input scenes
        If convertByteArrayToInt(nbInputScene) > 0 Then
            'import input scenes
            For index = 0 To convertByteArrayToInt(nbInputScene)
                'create array for input item
                Dim arraySceneInput(byteArray.Length - generalSeek - 1) As Byte
                Array.Copy(byteArray, generalSeek, arraySceneInput, 0, byteArray.Length - generalSeek)

                listInputScene.Add(New CSBFSceneInput(arraySceneInput))
                listInputScene(index).Init()
                generalSeek += listInputScene(index).GetSize()
            Next
        End If

    End Sub

    Private Sub ExtractSceneText(ByRef generalSeek)
        'determine number of input in the scene
        Dim nbTextScene(3) As Byte
        Array.Copy(byteArray, generalSeek, nbTextScene, 0, 4)
        generalSeek += 4

        'import the Input scenes
        If convertByteArrayToInt(nbTextScene) > 0 Then
            'import input scenes
            For index = 0 To convertByteArrayToInt(nbTextScene)
                'create array for input item
                Dim arraySceneText(byteArray.Length - generalSeek - 1) As Byte
                Array.Copy(byteArray, generalSeek, arraySceneText, 0, byteArray.Length - generalSeek)

                listTextScene.Add(New CSBFSceneText(arraySceneText))
                listTextScene(index).Init()
                generalSeek += listTextScene(index).GetSize()
            Next
        End If
    End Sub

    Private Sub ExtractSceneTexture(ByRef generalSeek)
        'determine number of input in the scene
        Dim nbTextureScene(3) As Byte
        Array.Copy(byteArray, generalSeek, nbTextureScene, 0, 4)
        generalSeek += 4

        'import the Input scenes
        If convertByteArrayToInt(nbTextureScene) > 0 Then
            'import input scenes
            For index = 0 To convertByteArrayToInt(nbTextureScene)
                'create array for input item
                Dim arraySceneTexture(byteArray.Length - generalSeek - 1) As Byte
                Array.Copy(byteArray, generalSeek, arraySceneTexture, 0, byteArray.Length - generalSeek)

                listTextureScene.Add(New CSBFSceneText(arraySceneTexture))
                listTextureScene(index).Init()
                generalSeek += listTextureScene(index).GetSize()
            Next
        End If
    End Sub



    Public Function SizeScene()
        Return sceneSize
    End Function
End Class
