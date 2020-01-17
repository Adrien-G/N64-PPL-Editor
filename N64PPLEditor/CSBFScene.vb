''' <summary>
'''  The class CSBFScene allow to read a scene inside a SBF file (composed of input, text and texture content)
'''  Author : Adrien Garreau
'''  Date : 16/01/2020
''' </summary>

Public Class CSBFScene

    Private sceneName As Byte()
    Private byteArray As Byte()

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
        'create Objects
        listInputScene = New List(Of CSBFSceneInput)
        listTextScene = New List(Of CSBFSceneText)
        listTextureScene = New List(Of CSBFSceneTexture)

        Dim generalSeek As Integer = 8 + sceneName.Length

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

        'verify if Text is present... (textScene)


        'verify if texture is present... (texture scene)

        'get new adresse
    End Sub
End Class
