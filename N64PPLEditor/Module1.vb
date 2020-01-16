Module Module1
    Public Function convertByteArrayToInt(ByVal bytearray) As Integer
        Dim FinalNumber As Int32 = 0
        For index = 0 To bytearray.length - 1
            FinalNumber = FinalNumber << 8
            FinalNumber += Convert.ToInt32(bytearray(index))
        Next
        Return FinalNumber
    End Function
End Module
