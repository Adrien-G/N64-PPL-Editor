''' <summary>
'''  The class CSBFTextScene manage text in a scene
'''  Author : Adrien Garreau
'''  Date : 16/01/2020
''' </summary>

Public Class CSBFTextScene

    Public nbText(3) As Byte
    Public textLength(3) As Byte
    Public lineTextCount As Byte
    Public sizeOfText(3) As Byte
    Public sizeX1(3) As Byte
    Public sizeX2(3) As Byte
    Public sizeY1(3) As Byte
    Public sizeY2(3) As Byte
    Public colorInBound(3) As Byte
    Public colorOutBound(3) As Byte
    Public text As Byte()


    'constructor
    Public Sub New(ByVal byteArray1 As Byte())

    End Sub




End Class
