''' <summary>
'''  The class CSBFScene allow to read a scene inside a SBF file (composed of input, text and texture content)
'''  Author : Adrien Garreau
'''  Date : 16/01/2020
''' </summary>

Public Class CSBFScene

    'unknown data for now... need to explore the binary N64 PPL rom...
    'just know enought for "estimating" size of unknow data...
    Private Class InputScene

    End Class


    '3 elements in one scene
    Private listInputScene As List(Of InputScene)
    Private listTextScene As List(Of CSBFTextScene)
    Private listTextureScene As List(Of CSBFTextureScene)

    Private Sub ExtractAScene()


        'create Objects
        listInputScene = New List(Of InputScene)
        listTextScene = New List(Of CSBFTextScene)
        listTextureScene = New List(Of CSBFTextureScene)
        Dim seek As Integer = 0

        'verify if data is present... (inputScene)


        'verify if Text is present... (textScene)


        'verify if texture is present... (texture scene)

        'get new adresse
    End Sub
End Class
