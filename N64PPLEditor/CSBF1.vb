Public Class CSBF1

    Public Structure HeaderSBF
        Public nameLength As Integer
        Public texturePrintedLength As Byte
        Public textureType As Byte
        Public bytesPerPixel As Byte
        Public colorMode As Byte
        Public pixelBitLength As Byte
        Public isCompressed As Boolean
        Public indexedColor As Boolean
        Public sizeX, sizeY As Integer
        Public startingPalettePos As Integer
        Public startingDataPos As Integer
        Public paletteSize As Integer
        Public globalIdent As Integer
    End Structure

    Private Class Unknown

    End Class

    Private Class Text

    End Class

    Private Class Texture

    End Class


    'constructor
    Public Sub New(ByVal byteArray As Byte())

    End Sub

End Class
