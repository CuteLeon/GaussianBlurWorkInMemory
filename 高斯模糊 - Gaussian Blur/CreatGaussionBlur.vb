Option Explicit On

Module CreatGaussianBlur
    '结构体[EasyColor],只定义仅需的四个[短!]整型变量，以节省内存。
    Private Structure EasyColor
        Dim Alpha As Int16
        Dim Red As Int16
        Dim Green As Int16
        Dim Blue As Int16
    End Structure

    ''' <summary>
    '''  创建以Pixel为半径的高斯模糊图像
    ''' </summary>
    ''' <param name="BitmapRes">传入的原图像</param>
    ''' <param name="Pixel">模糊半径</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GaussianBlur(ByVal BitmapRes As Bitmap, ByVal Pixel As Integer) As Bitmap
        Dim GetBitmap As Bitmap = BitmapRes
        Dim PositionX, PositionY As Long, CenterX, CenterY As Integer

        '建立二维的高斯权重分配表，节省大量计算
        Dim DoublePixel As Integer = Pixel * 2
        Dim Gaussian(DoublePixel, DoublePixel) As Double, GaussianCount As Double = 0.0
        Dim DistanceX, DistanceY As Integer '根据当前位置定位对应的分配表元素
        For CenterX = 0 To DoublePixel
            For CenterY = 0 To DoublePixel
                Gaussian(CenterX, CenterY) = (1 / (2 * Math.PI * (Pixel + 1 / 2) ^ 2)) *
                                                                Math.Exp(-((CenterX - Pixel) ^ 2 + (CenterY - Pixel) ^ 2) / (2 * (Pixel + 1 / 2) ^ 2))
            Next
        Next

        '用二维数组代替极后面大量的位图[Bitmap.GetPixel]方法
        Dim PixelOfBitmap(GetBitmap.Width - 1, GetBitmap.Height - 1) As EasyColor
        Dim TempColor As Color
        For PositionX = 0 To GetBitmap.Width - 1
            For PositionY = 0 To GetBitmap.Height - 1
                TempColor = GetBitmap.GetPixel(PositionX, PositionY)

                PixelOfBitmap(PositionX, PositionY).Alpha = TempColor.A
                PixelOfBitmap(PositionX, PositionY).Red = TempColor.R
                PixelOfBitmap(PositionX, PositionY).Green = TempColor.G
                PixelOfBitmap(PositionX, PositionY).Blue = TempColor.B
            Next
        Next

        '中心像素
        Dim SumA, SumR, SumG, SumB As Double
        Dim CenterColor As Color

        '遍历图像的每一个像素
        For PositionY = 0 To GetBitmap.Height - 1
            For PositionX = 0 To GetBitmap.Width - 1
                '初始化各项变量数据，还有GaussianCount。。。
                SumA = 0.0 : SumR = 0.0 : SumG = 0.0 : SumB = 0.0 : GaussianCount = 0.0
                DistanceX = PositionX - Pixel : DistanceY = PositionY - Pixel

                '遍历当前像素周围Pixel圈像素，取平均颜色
                For RoundY = PositionY - Pixel To PositionY + Pixel
                    If (RoundY = GetBitmap.Height) Then Exit For '              已经遍历完存在的行就结束For
                    If (0 <= RoundY) Then                                         '              如果当前行存在就进行内层循环
                        For RoundX = PositionX - Pixel To PositionX + Pixel
                            If (RoundX = GetBitmap.Width) Then Exit For '        到达水平边缘，结束For
                            If (0 <= RoundX) Then '                                              如果当前列存在就进行内层循环
                                'Alpha通道  模糊透明度
                                SumA += PixelOfBitmap(RoundX, RoundY).Alpha * Gaussian(RoundX - DistanceX, RoundY - DistanceY)
                                'RGB  颜色
                                SumR += PixelOfBitmap(RoundX, RoundY).Red * Gaussian(RoundX - DistanceX, RoundY - DistanceY)
                                SumG += PixelOfBitmap(RoundX, RoundY).Green * Gaussian(RoundX - DistanceX, RoundY - DistanceY)
                                SumB += PixelOfBitmap(RoundX, RoundY).Blue * Gaussian(RoundX - DistanceX, RoundY - DistanceY)

                                '为了防止图像边缘出现黑边和Alpha值渐变减小产生羽化的情况，所以需要临时计算
                                GaussianCount += Gaussian(RoundX - DistanceX, RoundY - DistanceY)
                            End If
                        Next
                    End If
                Next

                '让结果以[高斯权重总和为1]计算(Ps:GaussianCount是Gaussian数组的数值总和)
                SumA /= GaussianCount
                SumR /= GaussianCount
                SumG /= GaussianCount
                SumB /= GaussianCount

                '写入像素颜色
                CenterColor = Color.FromArgb(SumA, SumR, SumG, SumB) '计算颜色
                '把处理后的像素更新到二维数组，这样图像才会平滑。
                With PixelOfBitmap(PositionX, PositionY)
                    .Alpha = SumA
                    .Red = SumR
                    .Green = SumG
                    .Blue = SumB
                End With

                GetBitmap.SetPixel(PositionX, PositionY, CenterColor)
            Next
        Next

        '处理完成，返回图像
        Return GetBitmap
        '感觉自己帅帅哒！(●—●)
    End Function

End Module