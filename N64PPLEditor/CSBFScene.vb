''' <summary>
'''  The class CSBFScene allow to read a scene inside a SBF file (composed of input, text and texture content)
'''  Author : Adrien Garreau
'''  Date : 16/01/2020
''' </summary>

Public Class CSBFScene

    '3 elements in one scene
    Private listInputScene As List(Of CSBFSceneInput)
    Private listTextScene As List(Of CSBFSceneText)
    Private listTextureScene As List(Of CSBFSceneTexture)
    Private byteArray As Byte()

    'constructor
    Public Sub New(ByVal byteArray1 As Byte())
        byteArray = byteArray1
    End Sub

    Private Sub ExtractAScene()


        'create Objects
        listInputScene = New List(Of CSBFSceneInput)
        listTextScene = New List(Of CSBFSceneText)
        listTextureScene = New List(Of CSBFSceneTexture)
        Dim seek As Integer = 0

        'verify if data is present... (inputScene)


        'verify if Text is present... (textScene)


        'verify if texture is present... (texture scene)

        'get new adresse
    End Sub
End Class
