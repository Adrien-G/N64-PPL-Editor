''' <summary>
'''  The class CSBFInputScene manage input in a scene
'''  Author : Adrien Garreau
'''  Date : 16/01/2020
''' </summary>

Public Class CSBFSceneInput

    'unknown data for now... need to explore the binary N64 PPL rom...
    'just know enought for "estimating" size of unknow data...
    Private nbItem(3) As Byte
    Private size As Integer
    Private byteArray As Byte()

    'constructor
    Public Sub New(ByVal byteArray1 As Byte())
        byteArray = byteArray1
    End Sub

    'for now only prepare the size.. because nothing else data are known..
    Public Sub Init()
        ' get amount of input items in scene
        Array.Copy(byteArray, 0, nbItem, 0, 3)

        'check amount
        If convertByteArrayToInt(nbItem) = 0 Then
            size = 4
        Else
            ' get the "size" index for left data
            Dim idx(3) As Byte
            Array.Copy(byteArray, 44, idx, 0, 3)
            Select Case convertByteArrayToInt(idx)
                Case 0 To 2
                    size = 56
                Case 3 To 25
                    size = 52
                Case 26 To 105
                    size = 56
                Case 106 To 130
                    size = 60
                Case 131 To 170
                    size = 56
                Case 170 To 11000000
                    size = 72
                Case 11000001 To 12000000
                    size = 80
                Case Else
                    size = 72
            End Select

        End If
    End Sub

    Public Function GetSize()
        Return size
    End Function



End Class
