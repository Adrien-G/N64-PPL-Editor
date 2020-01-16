Public Class CBIFList


    Private fib3childs As List(Of C3FIB)
    Private nbElement As Integer
    Private indexStartingList As Integer
    Private indexEndingList As Integer
    Private fstream As IO.FileStream

    Private index3FIB As Integer()
    Private size3FIB As Integer()

    'constructor
    Public Sub New(ByVal indexStartingList1 As Integer, ByVal indexEndingList1 As Integer, ByVal numberElement As Integer, ByVal fstream2 As IO.FileStream)
        indexStartingList = indexStartingList1
        indexEndingList = indexEndingList1
        nbElement = numberElement
        fstream = fstream2
        ReDim index3FIB(nbElement - 1)
        ReDim size3FIB(nbElement - 1)
        fib3childs = New List(Of C3FIB)
    End Sub

    Public Sub load3FIBList()
        fstream.Position = indexStartingList
        Dim index3FIBlocal(3) As Byte
        Dim size3FIBlocal(3) As Byte
        Dim nameBIF(15) As Byte

        For index = 0 To nbElement - 1
            fstream.Read(size3FIBlocal, 0, 4)
            fstream.Read(index3FIBlocal, 0, 4)
            fstream.Read(nameBIF, 0, 16)

            index3FIB(index) = convertByteArrayToInt(index3FIBlocal) - 4
            size3FIB(index) = convertByteArrayToInt(size3FIBlocal)

            Dim name = System.Text.Encoding.UTF8.GetString(nameBIF)
            name = name.Replace(Convert.ToChar(0), "")
            fib3childs.Add(New C3FIB(name))
        Next
    End Sub

    Public Sub Set3fibChunks()
        For index = 0 To nbElement - 1
            fstream.Position = indexStartingList + index3FIB(index)


            Dim headerName(3) As Byte
            fstream.Read(headerName, 0, 4)

            'read content of 3FIB
            Dim data3FIB(size3FIB(index) - 1) As Byte
            fstream.Position = indexStartingList + index3FIB(index)
            fstream.Read(data3FIB, 0, size3FIB(index))

            'verify if it's a 3FIB or not...
            If (headerName(0) = 51 And headerName(1) = 70 And headerName(2) = 73 And headerName(3) = 66) Then '3FIB
                fib3childs(index).FIBInit(data3FIB)
            Else
                fib3childs(index).OtherInit(data3FIB)
            End If

        Next
    End Sub

    Public Function Get3FIBData(ByVal index) As C3FIB
        Return fib3childs(index)
    End Function

    Public Function getSize()
        Dim totalSize As Integer = 0
        For index = 0 To nbElement - 1
            totalSize += fib3childs(index).getSize()
            If (fib3childs(index).getSize() Mod 2 = 1) Then
                totalSize += 1
            End If
        Next
        totalSize += (fib3childs.Count * 23) + 4

        Return totalSize
    End Function

    Public Function getNbItem() As Integer
        Return nbElement
    End Function

    Public Sub remove3fib(ByVal index As Integer)
        nbElement -= 1
        fib3childs.RemoveAt(index)
    End Sub

    Public Sub removebff2(ByVal index3fib As Integer, ByVal indexbff As Integer)
        fib3childs(index3fib).removeBFF2(indexbff)
    End Sub

    Public Sub replaceBFF2(ByVal fibIndex As Integer, ByVal bffIndex As Integer, ByVal data As Byte())
        fib3childs(fibIndex).replaceBFF2(bffIndex, data)
    End Sub

    Public Sub writeAllData()
        fstream.Position = indexStartingList - 4

        'write number of elements
        Dim nbElementB = BitConverter.GetBytes(nbElement)
        If BitConverter.IsLittleEndian Then
            Array.Reverse(nbElementB)
        End If
        fstream.Write(nbElementB, 0, 4)

        Dim indexData = nbElement * 24 + 4

        'write list header
        For index = 0 To nbElement - 1
            'write size
            Dim sizeFib = BitConverter.GetBytes(fib3childs(index).getSize())
            If BitConverter.IsLittleEndian Then
                Array.Reverse(sizeFib)
            End If
            fstream.Write(sizeFib, 0, 4)

            'write index start
            Dim emplFib As Byte() = BitConverter.GetBytes(indexData)
            indexData += fib3childs(index).getSize()
            If (fib3childs(index).getSize() Mod 2 = 1) Then
                indexData += 1
            End If

            If BitConverter.IsLittleEndian Then
                Array.Reverse(emplFib)
            End If
            fstream.Write(emplFib, 0, 4)

            'write name of BIF
            Dim nameBIF As Byte() = System.Text.Encoding.UTF8.GetBytes(fib3childs(index).getBifName())
            fstream.Write(nameBIF, 0, nameBIF.Length)
            For index2 = 0 To 16 - nameBIF.Length - 1
                fstream.WriteByte(0)
            Next
        Next

        'write 3fib data
        For index = 0 To nbElement - 1
            Dim fibContainer As Byte() = fib3childs(index).get3FibContainerData()
            fstream.Write(fibContainer, 0, fibContainer.Length)
            'the size of 3fib must be pair for unknown reason..
            If (fib3childs(index).getSize() Mod 2 = 1) Then
                fstream.WriteByte(255)
            End If
        Next
        Dim sizeLeft As Integer = indexEndingList - fstream.Position
        If sizeLeft > 0 Then
            Dim freeSpace(sizeLeft - 1) As Byte
            fstream.Write(freeSpace, 0, freeSpace.Length)
        End If


    End Sub


End Class