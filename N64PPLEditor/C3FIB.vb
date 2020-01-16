Imports System.IO

''' <summary>
'''  The class C3FIB allow to read a FIB file
'''  Author : Adrien Garreau
'''  Date : 13/01/2020
''' </summary>

Public Class C3FIB
    Private bff2childs As List(Of CBFF2)
    Public bifName As String
    Private textureType As Byte
    Private bffCount As Byte
    Private fibNameSize As Byte
    Private fibName(16) As Byte
    Private otherByteArray As Byte()
    Private header As Byte()


    'constructor
    Public Sub New(Optional ByVal nameBif As String = "")
        bifName = nameBif
    End Sub

    Public Sub OtherInit(ByVal otherByteArray1 As Byte())
        otherByteArray = otherByteArray1
        fibNameSize = 9
        ReDim fibName(fibNameSize)
        fibName = System.Text.Encoding.UTF8.GetBytes("(no Name)")
    End Sub

    Public Sub FIBInit(ByVal byteArray As Byte())
        'load texture type, bff count and fib name size
        textureType = byteArray(4)
        bffCount = byteArray(12)
        fibNameSize = byteArray(16)

        'load fibName
        ReDim fibName(fibNameSize - 1)
        Array.Copy(byteArray, 20, fibName, 0, fibNameSize)

        'save header
        ReDim header(fibNameSize + 19)
        Array.Copy(byteArray, 0, header, 0, header.Length)

        'remove 3fib header and send to loadBFF function
        Dim bffData(byteArray.Length - fibNameSize - 21) As Byte
        Array.Copy(byteArray, 20 + fibNameSize, bffData, 0, byteArray.Length - fibNameSize - 20)
        loadBFF(bffData)
    End Sub

    Private Sub loadBFF(ByVal byteArray As Byte())
        Dim sizeOfBFF2(bffCount - 1) As Integer
        Dim indexOfBFF2(bffCount - 1) As Integer
        Dim index2 = 0
        'starting to 13 for avoid first BFF selection
        sizeOfBFF2(0) = 13

        'calculate BFF2 size...
        For index = 13 To byteArray.Length - 4
            If (byteArray(index) = 66 And byteArray(index + 1) = 70 And byteArray(index + 2) = 70 And byteArray(index + 3) = 50) Then 'BFF2
                index2 += 1
                sizeOfBFF2(index2 - 1) -= 12
                sizeOfBFF2(index2) += 13
                indexOfBFF2(index2) = indexOfBFF2(index2 - 1) + sizeOfBFF2(index2 - 1)
            Else
                sizeOfBFF2(index2) += 1
            End If
        Next

        'add 3 left index for the last element (skipped in for loop)
        sizeOfBFF2(bffCount - 1) += 3

        'load bff2 childs
        bff2childs = New List(Of CBFF2)
        For index = 0 To bffCount - 1
            Dim arrayBFF2(sizeOfBFF2(index) - 1) As Byte
            Array.Copy(byteArray, indexOfBFF2(index), arrayBFF2, 0, sizeOfBFF2(index))
            bff2childs.Add(New CBFF2(arrayBFF2))
            bff2childs.Item(index).BFF2Init()
        Next
    End Sub

    Public Function getFibName() As String
        Return System.Text.Encoding.UTF8.GetString(fibName)
    End Function

    Public Function getBifName() As String
        Return bifName
    End Function

    Public Function getBffCount() As Integer
        Return bffCount
    End Function

    Public Function getBFFData(ByVal index) As CBFF2
        Return bff2childs(index)
    End Function

    Public Sub addBFF2(ByVal byteArray As Byte())
        bff2childs.Add(New CBFF2(byteArray))
        bff2childs(bff2childs.Count).BFF2Init()
    End Sub

    Public Sub removeBFF2(ByVal index)
        bff2childs.RemoveAt(index)
        bffCount -= 1
    End Sub

    Public Sub replaceBFF2(ByVal index As Integer, ByVal data As Byte())
        bff2childs(index).replaceBFF2(data)
        bff2childs(index).BFF2Init()
    End Sub

    Public Function getSize() As Integer
        If bff2childs Is Nothing Then
            Return otherByteArray.Length
        Else
            Dim sizeTotal = fibNameSize + 20
            For index = 0 To bff2childs.Count - 1
                sizeTotal += bff2childs(index).getSize()
            Next
            Return sizeTotal

        End If
    End Function

    Public Function get3FibContainerData() As Byte()
        If (bff2childs Is Nothing) Then
            Return otherByteArray
        Else
            Dim bffSize As Integer = 0
            Dim bffSizeB(bffCount - 1) As Integer
            For index = 0 To bffCount - 1
                Dim Fsize As Integer = bff2childs(index).getSize()
                bffSize += Fsize
                bffSizeB(index) = Fsize
            Next
            Dim fibContainer(bffSize + fibNameSize + 19) As Byte
            'set header of 3FIB
            Array.Copy(header, 0, fibContainer, 0, 20 + fibNameSize)
            'set data of BFF2
            Dim indexBff As Integer = 20 + fibNameSize

            For index = 0 To bffCount - 1
                Array.Copy(bff2childs(index).getBFFContainerData, 0, fibContainer, indexBff, bffSizeB(index))
                indexBff += bff2childs(index).getSize()
            Next
            Return fibContainer

        End If
    End Function
End Class
