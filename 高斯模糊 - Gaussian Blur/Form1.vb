Option Explicit On

Public Class Form1
    Dim Time(1) As Double

    Private Sub TestButton_Click(sender As Object, e As EventArgs) Handles Test1.Click, Test2.Click, Test3.Click, Test4.Click
        '记录事件开始时间（精确到1x10^(-6)秒）
        Time(0) = My.Computer.Clock.LocalTime.TimeOfDay.TotalSeconds

        PictureBox1.Image = CType(sender, Button).Image
        Dim TempPicture As Bitmap = CType(sender, Button).Image
        PictureBox1.Image = GaussianBlur(TempPicture, SetPixel.Value)

        '记录事件结束时间
        Time(1) = My.Computer.Clock.LocalTime.TimeOfDay.TotalSeconds

        '显示处理事件耗时
        MsgBox("Task finished!" & vbCrLf & "共用时： 【" & Time(1) - Time(0) & "】 秒。", MsgBoxStyle.Information)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        PictureBox1.Image.Save("D:\" & My.Computer.Clock.LocalTime.TimeOfDay.TotalSeconds & ".png", System.Drawing.Imaging.ImageFormat.Png)
    End Sub

End Class
