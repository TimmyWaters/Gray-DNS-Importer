Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Threading
Imports System.Xml
Imports Newtonsoft.Json
Public Class Form1

    Dim tfCheckingAppend As Boolean = False
    Dim tfCheckingOverwrite As Boolean = False


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim fPath As String() = My.Settings.FolderPath.Split("\")
        Dim fLength As Int16 = fPath.Length
        lblFolderPath.Text = fPath(fLength - 1)
    End Sub

    Private Sub BtnSelectFolder_Click(sender As Object, e As EventArgs) Handles btnSelectFolder.Click

        ' If user selects a folder, put the folder path in the FolderPath label
        If dlgFolder.ShowDialog() = DialogResult.OK Then ' User chose a folder

            'Save chosen folder
            My.Settings.FolderPath = dlgFolder.SelectedPath

            ' Display chosen folder
            Dim fPath As String() = My.Settings.FolderPath.Split("\")
            Dim fLength As Int16 = fPath.Length
            lblFolderPath.Text = fPath(fLength - 1)

            ' Set a tooltip on the FolderPath label to show the full path if it's longer than the label. 
            Dim tt As New ToolTip()
            tt.SetToolTip(lblFolderPath, My.Settings.FolderPath)

        End If

    End Sub

    Private Sub BtnImport_Click(sender As Object, e As EventArgs) Handles btnImport.Click
        Dim t As New Thread(AddressOf DoImport)
        t.Start()
    End Sub


    Private Sub DoImport()
        Try

            'Clear status textbox
            ' txtStatus.Clear()
            ClearStatus()

            ' Get Root folder
            If My.Computer.FileSystem.DirectoryExists(My.Settings.FolderPath) = False Then Throw New Exception("Folder not found")
            Dim RootFolder As DirectoryInfo = My.Computer.FileSystem.GetDirectoryInfo(My.Settings.FolderPath)

            ' Set Log Folder
            If My.Computer.FileSystem.DirectoryExists(My.Settings.FolderPath & "\Logs") = False Then My.Computer.FileSystem.CreateDirectory(My.Settings.FolderPath & "\Logs")
            Dim LogsFolder As New DirectoryInfo(My.Settings.FolderPath & "\Logs")

            ' Set Completed folder
            If My.Computer.FileSystem.DirectoryExists(My.Settings.FolderPath & "\Completed") = False Then My.Computer.FileSystem.CreateDirectory(My.Settings.FolderPath & "\Completed")
            Dim CompletedFolder As New DirectoryInfo(My.Settings.FolderPath & "\Completed")

            ' Set Warnings folder
            If My.Computer.FileSystem.DirectoryExists(My.Settings.FolderPath & "\Warnings") = False Then My.Computer.FileSystem.CreateDirectory(My.Settings.FolderPath & "\Warnings")
            Dim WarningsFolder As New DirectoryInfo(My.Settings.FolderPath & "\Warnings")

            ' Set Skipped folder
            If My.Computer.FileSystem.DirectoryExists(My.Settings.FolderPath & "\Skipped") = False Then My.Computer.FileSystem.CreateDirectory(My.Settings.FolderPath & "\Skipped")
            Dim SkippedFolder As New DirectoryInfo(My.Settings.FolderPath & "\Skipped")

            ' Write to status box that process is starting
            Log("Starting Up")

            ' Search every file in the folder
            Log("Searching for files in folder " & My.Settings.FolderPath)

            ' Error out if there are no files in folder
            If RootFolder.GetFiles.Length = 0 Then Throw New Exception("No files found in folder")

            Log("")

            Dim LogFile As String = ""
            Dim WarningsFile As String = ""
            Using MyClient As New HttpClient
                MyClient.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("sso-key", My.Settings.MyCredentialsProd)

                SetProgFolderMax(RootFolder.GetFiles.Count)
                SetProgFolderValue(0)

                For Each MyFile As FileInfo In RootFolder.GetFiles
                    Try

                        ' Skip the redirects files and reverse address files
                        If MyFile.Name.ToLower = "redirects.txt" Then Continue For
                        If MyFile.Name.ToLower.Contains("in-addr.arpa") Then
                            Log("File " & MyFile.Name & " is not a DNS zone file. Skipping file.", LogFile)
                            MyFile.MoveTo(SkippedFolder.FullName & "\" & MyFile.Name, True)
                            Continue For
                        End If

                        ' Set Log file
                        LogFile = LogsFolder.FullName & "\LOG_" & MyFile.Name & ".txt"
                        If My.Computer.FileSystem.FileExists(LogFile) Then My.Computer.FileSystem.DeleteFile(LogFile)

                        ' Set Warnings file
                        WarningsFile = WarningsFolder.FullName & "\WARNING_" & MyFile.Name & ".txt"
                        If My.Computer.FileSystem.FileExists(WarningsFile) Then My.Computer.FileSystem.DeleteFile(WarningsFile)

                        'CantBeImported.Clear()
                        Log("Reading zone data from " & MyFile.Name, LogFile)

                        ' Read file in, create GoDaddy output file
                        Dim GDZ As GoDaddyZone = ConvertWindowsFileToGoDaddyRecord(MyFile)


                        '' TESTING TESTING TESTING
                        'Continue For
                        'Exit Sub
                        '' TESTING TESTING TESTING


                        Dim DomainName As String = GDZ.DomainName
                        If String.IsNullOrWhiteSpace(DomainName) Then
                            Log("Couldn't determine domain name from file " & MyFile.Name & ". Skipping domain", LogFile)
                            Log("")
                            MyFile.MoveTo(SkippedFolder.FullName & "\" & MyFile.Name, True)
                            Continue For
                        End If


                        ' Error If zone file returns no good records
                        If GDZ.GoodEntries.Count = 0 Then
                            Log("No usable DNS records found. Skipping domain.")
                            Log("")
                            MyFile.MoveTo(SkippedFolder.FullName & "\" & MyFile.Name, True)
                            Continue For
                        End If


                        'If we get here, we have good zone records. Need to verify/set DNS is hosted by GoDaddy

                        If ckSkipNSCheck.Checked = True Then

                            Log("Skipping name server check. Attempting to import DNS records in the blind.", LogFile)
                            ckSkipNSCheck.Checked = False


                        Else


                            ' See if name servers for domain name are at GoDaddy. 
                            Log("Checking name servers for " & DomainName, LogFile)

                            'Note: This throws an exception if domain does not belong to us in GoDaddy. Otherwise returns true or false
                            If CheckNameserversInGoDaddy(DomainName) = True Then

                                If ckOverwrite.Checked = True Then
                                    Log("DNS records for this zone are already in GoDaddy. Erasing and replacing records.", LogFile)
                                    Log("")
                                ElseIf ckAppend.Checked = True Then
                                    Log("DNS records for this zone are already in GoDaddy. Appending to existing records.", LogFile)
                                    Log("")
                                Else
                                    ' If name servers are in GoDaddy, we can move on      
                                    Log("DNS records for this zone are already in GoDaddy. Skipping domain.", LogFile)
                                    Log("")
                                    MyFile.MoveTo(SkippedFolder.FullName & "\" & MyFile.Name, True)
                                    Continue For
                                End If


                            Else

                                'if name servers are not in GoDaddy, set them and wait until they have finished
                                Log("GoDaddy is NOT the name server for the domain, pointing name servers to GoDaddy", LogFile)

                                ' Sets name servers to GoDaddy
                                If SetNameserversInGoDaddy(DomainName) = False Then
                                    ' If name servers are in GoDaddy, we can move on      
                                    Log("Name server change denied. Skipping domain.", LogFile)
                                    Log("")
                                    MyFile.MoveTo(SkippedFolder.FullName & "\" & MyFile.Name, True)
                                    Continue For
                                End If

                                ' Flag to show if name servers have finished pointing to GoDaddy
                                Dim InGoDaddy As Boolean = False
                                Dim CheckCount As Int16 = 5

                                ' Checks every 3 seconds to see if name servers have switched.
                                Log("Name server change accepted, Waiting 3 seconds before testing change...", LogFile)

                                Do
                                    Thread.Sleep(3000) ' Pause 3 seconds
                                    ' Log("Checking for domain name server change #" & CheckCount, LogFile)

                                    If CheckNameserversInGoDaddy(DomainName) = True Then
                                        ' If name servers have finished pointing to GoDaddy, set our flag to true so we can break out of the loop
                                        InGoDaddy = True
                                        Log("Name servers changed. Waiting final 5 seconds to allow for sync delay...", LogFile)
                                        Thread.Sleep(5000) ' Pause 5 seconds
                                        Exit Do
                                    End If

                                    CheckCount -= 1
                                    'If CheckCount = 0 Then Exit Do
                                    Log("Name servers not changed yet. Waiting 3 seconds to test again...", LogFile)
                                Loop

                                If InGoDaddy = False Then
                                    Log("Name server check timeout reached. Skipping domain.", LogFile)
                                    Log("")
                                    MyFile.MoveTo(SkippedFolder.FullName & "\" & MyFile.Name, True)
                                    Continue For
                                End If


                            End If

                        End If

                        'Name servers are in GoDaddy. Start updating DNS

                        Log("Importing DNS Records", LogFile)

                        If GDZ.GoodEntries.Count = 0 Then
                            ' No usable records found
                            Log("No usable records found", LogFile)
                        Else

                            ' Flag to see if this is the first record being imported.
                            Dim FirstOne As Boolean = True
                            If ckAppend.Checked = True Then FirstOne = False

                            ' Sort the list to make A records import first
                            GDZ.GoodEntries.Sort(Function(x, y) x.Type.CompareTo(y.Type))

                            Dim i As Integer = 0
                            SetProgFileMax(GDZ.GoodEntries.Count)
                            SetProgFileValue(0)

                            Do While i < GDZ.GoodEntries.Count
                                Dim GRI As GoDaddyEntry = GDZ.GoodEntries(i)

                                ' For Each GRI As GoDaddyEntry In GDZ.GoodEntries
                                Try
                                    If GRI.Type = "NS" Then
                                        ' GoDaddy won't import NS records, but doesn't throw error either. Add to warnings
                                        Log("Failed " & GRI.Type & " " & GRI.Name & " " & GRI.Data, LogFile)
                                        GDZ.WarningEntries.Add(GRI)
                                        i += 1
                                    Else

                                        ' Import the individual DNS record. If FirstOne is True, all DNS records will be wiped before this entry is imported. 
                                        Select Case InsertDNS(DomainName, GRI, FirstOne, MyClient)
                                            Case "Good"
                                                Log("Imported " & GRI.Type & " " & GRI.Name & " " & GRI.Data, LogFile)
                                                i += 1
                                                FirstOne = False
                                            Case "Duplicate"
                                                Log("Duplicate " & GRI.Type & " " & GRI.Name & " " & GRI.Data, LogFile)
                                                i += 1
                                                FirstOne = False
                                            Case "Overload"
                                                Log("Reached rate limit. Pausing for 10 seconds.")
                                                Thread.Sleep(10000)
                                            Case "Failed"
                                                Log("Failed " & GRI.Type & " " & GRI.Name & " " & GRI.Data, LogFile)
                                                i += 1
                                                FirstOne = False
                                        End Select
                                    End If

                                Catch ex As Exception
                                    ' If there is any error in importing a record, move it to the "warning" list
                                    GDZ.WarningEntries.Add(GRI)
                                End Try
                                'Next
                                SetProgFileValue(i)
                            Loop
                            ' Import complete for this text file. Move it to Completed folder
                            Log("Import completed", LogFile)

                        End If

                        ' Done with file. Move to Completed
                        MyFile.MoveTo(CompletedFolder.FullName & "\" & MyFile.Name, True)

                        'If we had any records we couldn't import, write them to the warning log
                        If GDZ.WarningEntries.Count > 0 Then
                            ' Write the warning file header.
                            My.Computer.FileSystem.WriteAllText(WarningsFile, "The following records could Not be imported. Review each entry And create manually if needed." & vbCrLf, False)

                            ' Iterate through each warning entry
                            For Each GRI In GDZ.WarningEntries

                                ' Iterate through the entry information to create a text string
                                Dim Shortname As String = GRI.Name.Replace((DomainName & "."), "")
                                Dim MyBody As New StringBuilder
                                MyBody.Append("Type: " & GRI.Type & " ")
                                MyBody.Append("Name: " & Shortname & " ")
                                MyBody.Append("Value: " & GRI.Data & " ")
                                If Not String.IsNullOrWhiteSpace(GRI.Port) Then MyBody.Append("Port: " & GRI.Port & " ")
                                If Not String.IsNullOrWhiteSpace(GRI.Priority) Then MyBody.Append("Priority: " & GRI.Priority & " ")
                                If Not String.IsNullOrWhiteSpace(GRI.Protocol) Then MyBody.Append("Protocol: " & GRI.Protocol & " ")
                                If Not String.IsNullOrWhiteSpace(GRI.Service) Then MyBody.Append("Service: " & GRI.Service & " ")
                                If Not String.IsNullOrWhiteSpace(GRI.TTL) Then MyBody.Append("TTL: " & GRI.TTL & " ")
                                If Not String.IsNullOrWhiteSpace(GRI.Weight) Then MyBody.Append("""Weight: " & GRI.Weight & " ")
                                MyBody.Append(vbCrLf)

                                ' Write text string to warning log
                                My.Computer.FileSystem.WriteAllText(WarningsFile, MyBody.ToString, True)
                            Next
                        End If



                        ' See if the Redirect file exists. If so, send to process to look for needed forwarders
                        If My.Computer.FileSystem.FileExists(RootFolder.FullName & "\redirects.txt") Then
                            Try

                                ' Start redirects
                                Log("Checking for redirects", LogFile)

                                Dim RedirectsFile As New FileInfo(RootFolder.FullName & "\redirects.txt")
                                InsertForwarders(DomainName, RedirectsFile.FullName, LogFile, WarningsFile)

                            Catch ex As Exception
                                Log("Error processing redirects file: " & ex.Message)
                            End Try
                        End If


                        If My.Computer.FileSystem.FileExists(WarningsFile) Then
                            Log("WARNING: There were records or redirects that could not be imported and must be created manually. See Warnings file for details.", LogFile)
                        End If

                        SetProgFolderValue(progFolder.Value + 1)

                    Catch ex As Exception
                        ' Major error somewhere. Write exception message and skip domain
                        Log("Unexpected error: " & ex.Message & ". Skipping domain name", LogFile)
                        MyFile.MoveTo(SkippedFolder.FullName & "\" & MyFile.Name, True)
                        SetProgFolderValue(progFolder.Value + 1)
                    End Try



                    Log("")

                Next

            End Using
            Log(" ")
            Log("Process Complete")


        Catch ex As Exception
            Log(ex.Message)
        End Try

        SetCkAppend(False)
        SetCkOverwrite(False)

    End Sub

    Private Function ConvertMeredithFileToGoDaddyRecord(MyFile As FileInfo) As GoDaddyZone

        Dim GR As New GoDaddyZone

        Try

            Dim MR As MeredithRecord

            ' Get domain name from name of text file
            Dim DomainName As String = MyFile.Name.Replace(".txt", ".")

            ' Read in zone record json and convert to zone record
            Using s As New StreamReader(MyFile.FullName)
                MR = JsonConvert.DeserializeObject(Of MeredithRecord)(s.ReadToEnd)
            End Using

            ' Iterate through each Meredith record to create GoDaddyRecords
            For Each MRI As ResourceRecordSet In MR.ResourceRecordSets

                Dim Name As String = MRI.Name.Replace(DomainName, "")
                If String.IsNullOrWhiteSpace(Name) Then Name = "@"

                Select Case MRI.Type
                    Case "SOA", "MAA", "CAA"
                        ' Do not include these

                    Case "NS"

                        If Name = "@" Then
                            ' Do not include root NS records
                        Else
                            ' We know we cannot import child NS records. Add to "warning" record item list to be entered manually.
                            For Each MRIV In MRI.ResourceRecords
                                Dim NewGDR As New GoDaddyEntry With {
                                    .Data = MRIV.Value,
                                    .Name = Name,
                                    .Type = "NS"
                                }
                                GR.WarningEntries.Add(NewGDR)

                            Next

                        End If

                    Case "A", "AAAA"

                        For Each MRIV In MRI.ResourceRecords
                            Dim NewGDR As New GoDaddyEntry With {
                                .Data = MRIV.Value,
                                .Name = Name,
                                .Type = "A"
                            }
                            GR.GoodEntries.Add(NewGDR)
                        Next

                    Case "CNAME"

                        For Each MRIV In MRI.ResourceRecords
                            Dim NewGDR As New GoDaddyEntry With {
                                .Data = MRIV.Value,
                                .Name = Name,
                                .Type = "CNAME"
                            }
                            GR.GoodEntries.Add(NewGDR)
                        Next

                    Case "MX"

                        ' Example...
                        ' "Value": "10 us-smtp-inbound-2.mimecast.com."

                        For Each MRIV In MRI.ResourceRecords
                            Dim MX As String() = MRIV.Value.Split(" ")
                            Dim NewGDR As New GoDaddyEntry With {
                                .Data = MX(1),
                                .Name = Name,
                                .Priority = MX(0),
                                .Type = "MX"
                            }
                            GR.GoodEntries.Add(NewGDR)
                        Next

                    Case "SRV"

                        ' Examples...
                        ' "Name": "_xmpp-client._tcp.azfamily.com."
                        ' Value": "5 0 5222 c2s.14xtra.com.webexconnect.com."

                        For Each MRIV In MRI.ResourceRecords
                            Dim S As String() = MRIV.Value.Split(" ")
                            Dim Priority As String = S(0)
                            Dim Weight As String = S(1)
                            Dim Port As String = S(2)
                            Dim Data As String = S(3)

                            Dim N As String() = Name.Split(".")
                            Dim Protocol As String = N(1)
                            Dim Service As String = N(0)
                            Name = Name.Replace(Protocol & ".", "")
                            Name = Name.Replace(Service & ".", "")
                            If String.IsNullOrWhiteSpace(Name) Then Name = "@"

                            Dim NewGDR As New GoDaddyEntry With {
                                .Data = Data,
                                .Name = Name,
                                .Port = Port,
                                .Priority = Priority,
                                .Protocol = Protocol,
                                .Service = Service,
                                .Weight = Weight,
                                .Type = "SRV"
                            }
                            GR.GoodEntries.Add(NewGDR)
                        Next

                    Case "TXT"

                        ' Examples...
                        ' "Value": "\"v=DKIM1; k=rsa; p=MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDfbxkmGWDYl4oS+h8PU32CRMeoCeN8mQGNANFvHCtZ6L6n0EqUmuAFo8hhylOS1GJkgMWTvmqNeFtnPze+PZ/dEy0xnxxE4ZjZvTA/Gl3sfmm9KKEf/JyFLyU2mkFAFnYybFv2sjlH/Qb4wQ757NK0Nr1ePiMO4aGyb8i2llkjRQIDAQAB\""
                        ' "Value": "\"v=DMARC1; p=reject; fo=1; ri=3600; rua=mailto:meredith@rua.agari.com; ruf=mailto:meredith@ruf.agari.com\""

                        For Each MRIV In MRI.ResourceRecords

                            Dim Value As String = MRIV.Value.Replace("""", "")
                            'Value = MRIV.Value.Replace("\""""", "")

                            Dim NewGDR As New GoDaddyEntry With {
                                .Data = Value,
                                .Name = Name,
                                .Type = "TXT"
                            }
                            GR.GoodEntries.Add(NewGDR)
                        Next

                    Case Else
                        For Each MRIV In MRI.ResourceRecords
                            Dim NewGDR As New GoDaddyEntry With {
                                .Data = MRIV.Value,
                                .Name = Name,
                                .Type = MRI.Type
                            }
                            GR.UnknownEntries.Add(NewGDR)
                        Next

                End Select

            Next



        Catch ex As Exception
            Throw
        End Try

        Return GR

    End Function

    Private Function ConvertWindowsFileToGoDaddyRecord(MyFile As FileInfo) As GoDaddyZone

        Dim GDZ As New GoDaddyZone
        Dim DomainFound As Boolean = False
        Dim ReadNSRecords As Boolean = False
        Dim DomainName As String
        Dim LastName As String = ""

        Try

            ' Read in file
            Using s As New StreamReader(MyFile.FullName)
                While Not s.EndOfStream

                    Dim line As String = s.ReadLine

                    ' Check the line to see if it contains the domain name until we find the domain name
                    ' Example: ";  Database file kcwy13.com.dns for kcwy13.com zone."
                    If DomainFound = False Then
                        If line.Contains("Database file") Then
                            Dim words() As String = line.Split(" ")
                            If words(words.Length - 1) = "zone." Then
                                DomainName = words(words.Length - 2)
                            Else
                                DomainName = words(words.Length - 1)
                                DomainName = DomainName.Substring(0, DomainName.Length - 1)
                            End If
                            GDZ.DomainName = DomainName
                            DomainFound = True
                        End If
                    End If

                    ' Don't start reading NS records until we hit the right section of the file
                    ' Skips everything before the ";  Zone records" line
                    If DomainFound = True And ReadNSRecords = False Then
                        If line.ToLower.Contains("zone records") Then
                            ReadNSRecords = True
                        End If
                    End If

                    ' Create GoDaddy entries from lines 
                    If DomainFound = True And ReadNSRecords = True Then

                        If Not (line.StartsWith(";") Or String.IsNullOrEmpty(line)) Then

                            ' Parse the line into Name and Data
                            Dim cEnd As Integer = line.IndexOf(" ")
                            Dim cName As String = line.Substring(0, cEnd)
                            Dim cData() As String = line.Substring(cEnd + 1, line.Length - cEnd - 1).Trim.Split(vbTab)
                            Dim dStart As Int16 = 0

                            ' Create GoDaddyEntry object and assign Name
                            Dim GDE As New GoDaddyEntry
                            If String.IsNullOrEmpty(cName) Then
                                GDE.Name = LastName
                            Else
                                GDE.Name = cName
                                LastName = cName
                            End If

                            ' Find if the first value is TTL or Type
                            If IsNumeric(cData(0)) Then
                                'GDE.TTL = cData(0)
                                GDE.Type = cData(1)
                                dStart = 2
                            Else
                                GDE.Type = cData(0)
                                dStart = 1
                            End If

                            ' Break out data by Type
                            Select Case GDE.Type
                                Case "SOA", "MAA", "CAA"
                                ' Do nothing. We do not include these

                                Case "NS"
                                    ' @      NS	edns03.graydns.com.
                                    If GDE.Name <> "@" Then
                                        GDE.Data = cData(dStart)
                                        GDZ.GoodEntries.Add(GDE)
                                    End If

                                Case "A", "AAAA", "CNAME"
                                    'Example: @       A	34.194.230.166
                                    GDE.Data = cData(dStart)
                                    GDZ.GoodEntries.Add(GDE)

                                Case "MX"
                                    'Example: @         MX	10	d218295a.ess.barracudanetworks.com.
                                    GDE.Priority = cData(dStart)
                                    GDE.Data = cData(dStart + 1)
                                    GDZ.GoodEntries.Add(GDE)

                                Case "SRV"
                                    ' Example: _autodiscover._tcp       SRV	0 0 443	autodiscover.outlook.com.
                                    Dim dName() As String = GDE.Name.Split(".")
                                    GDE.Name = "@"
                                    GDE.Service = dName(0)
                                    GDE.Protocol = dName(1)

                                    Dim dNumbers() As String = cData(dStart).Split(" ")
                                    GDE.Priority = dNumbers(0)
                                    GDE.Weight = dNumbers(1)
                                    GDE.Port = dNumbers(2)
                                    GDE.Data = cData(dStart + 1)
                                    GDZ.GoodEntries.Add(GDE)

                                Case "TXT"
                                    ' Example: @        TXT	( "v=spf1 include:spf.protection.outlook.com -all" )
                                    GDE.Data = cData(dStart).Replace("( ", "")
                                    GDE.Data = GDE.Data.Replace(" )", "")
                                    GDE.Data = GDE.Data.Replace("""", "")
                                    GDZ.GoodEntries.Add(GDE)

                            End Select


                        End If
                    End If



                End While
            End Using

            'For Each GDE As GoDaddyEntry In GDZ.GoodEntries
            '    Log("Domain: " & GDZ.DomainName)
            '    Log("Name: " & GDE.Name)
            '    Log("Type: " & GDE.Type)
            '    Log("Priority: " & GDE.Priority)
            '    Log("Weight: " & GDE.Weight)
            '    Log("TTL: " & GDE.TTL)
            '    Log("Data: " & GDE.Data)
            '    Log("Port: " & GDE.Port)
            '    Log("Service: " & GDE.Service)
            '    Log(" ")

            'Next


        Catch ex As Exception
            Throw
            Return Nothing
        End Try

        Return GDZ

    End Function

    Private Function InsertDNS(DomainName As String, GRI As GoDaddyEntry, FirstOne As Boolean, MyClient As HttpClient) As String
        Try


            ' Using MyClient As New HttpClient

            ' MyClient.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("sso-key", My.Settings.MyCredentialsProd)

            ' Create a block of JSON text with the details of the DNS entry we need to create
            Dim MyBody As New StringBuilder
            MyBody.Append("["c)
            MyBody.Append("{"c)
            MyBody.Append("""data"":""" & GRI.Data & """,")
            MyBody.Append("""name"":""" & GRI.Name & """,")
            If Not String.IsNullOrWhiteSpace(GRI.Port) Then MyBody.Append("""port"":" & GRI.Port & ",")
            If Not String.IsNullOrWhiteSpace(GRI.Priority) Then MyBody.Append("""priority"":" & GRI.Priority & ",")
            If Not String.IsNullOrWhiteSpace(GRI.Protocol) Then MyBody.Append("""protocol"":""" & GRI.Protocol & """,")
            If Not String.IsNullOrWhiteSpace(GRI.Service) Then MyBody.Append("""service"":""" & GRI.Service & """,")
            If Not String.IsNullOrWhiteSpace(GRI.TTL) Then MyBody.Append("""ttl"":" & GRI.TTL & ",")
            If Not String.IsNullOrWhiteSpace(GRI.Weight) Then MyBody.Append("""weight"":" & GRI.Weight & ",")
            MyBody.Append("""type"":""" & GRI.Type & """")
            MyBody.Append("}"c)



            If FirstOne = True Then
                ' Required if we're replacing all DNS records. It doesn't get used, but the API call errors if not included
                MyBody.Append(","c)
                MyBody.Append("{"c)
                MyBody.Append("""data"":""pdns10.domaincontrol.com"",")
                MyBody.Append("""name"":""@"",")
                MyBody.Append("""type"":""NS""")
                MyBody.Append("},")
                MyBody.Append("{"c)
                MyBody.Append("""data"":""pdns11.domaincontrol.com"",")
                MyBody.Append("""name"":""@"",")
                MyBody.Append("""type"":""NS""")
                MyBody.Append("}"c)
            End If

            MyBody.Append("]"c)

            ' Build a web request to use to call the GoDaddy API
            Using MyRequest As New HttpRequestMessage

                If FirstOne = True Then
                    ' "Put" Replaces all DNS records with the new entry. We do this on the first record we import to clear everything else out.
                    MyRequest.Method = HttpMethod.Put
                Else
                    ' "Patch" adds the new entry to existing DNS records. We do this after the first one has reset the zone.
                    MyRequest.Method = HttpMethod.Patch
                End If

                ' Define the GoDaddy API address we need to call, including the domain name we're editing
                MyRequest.RequestUri = New Uri("https://api.godaddy.com/v1/domains/" & DomainName & "/records")

                ' Include the block of JSON DNS entry details from above
                MyRequest.Content = New StringContent(MyBody.ToString)
                MyRequest.Content.Headers.ContentType = New MediaTypeHeaderValue("application/json")

                ' Make the API call by
                Dim MyResponse = MyClient.Send(MyRequest)

                ' Read the response (Only ued for debugging to display the status code)
                Dim MyResponseBody = MyResponse.Content.ReadAsStringAsync

                ' Return good results
                If (MyResponse.StatusCode = HttpStatusCode.OK Or MyResponse.StatusCode = HttpStatusCode.NoContent) Then Return "Good"
                If MyResponseBody.Result.Contains("DUPLICATE_RECORD") Then Return "Duplicate"
                If MyResponseBody.Result.Contains("TOO_MANY_REQUESTS") Then Return "Overload"

                ' If we get here, the record was not added or a duplicate, return fail message
                Return "Failed"

            End Using ' MyRequest
            ' End Using ' MyClient

        Catch ex As Exception
            Return "Failed"
        End Try

    End Function

    Private Sub ReadFile()

        Dim RedirectsFile As String = "C:\Users\james.jefferies\OneDrive - Gray Television, Inc\Desktop\WindowsDNS\redirects2.txt"

        Using reader As New StreamReader(RedirectsFile)

            While Not reader.EndOfStream

                ' Read each line of the redirects file
                Dim line1 As String = reader.ReadLine
                If String.IsNullOrWhiteSpace(line1) Then Continue While

                Dim DLine As String() = line1.Split(",")
                If DLine(0) = "case" Then

                    Dim line2 As String = reader.ReadLine
                    Dim RLine() As String = line2.Split(",")

                    If RLine(0) = "redirect" Then

                        For i = 1 To DLine.Length - 1

                            Log(DLine(i) & "|" & RLine(1))

                        Next

                    Else
                        Log("Missing redirect: " & line1 & " / " & line2)
                        Exit Sub
                    End If

                Else
                    Log("Missing case: " & line1)
                    Exit Sub
                End If



            End While

        End Using


    End Sub

    Private Sub InsertForwarders(DomainName As String, RedirectFile As String, LogFile As String, WarningsFile As String)

        Try

            Dim DidRedirects As Boolean = False

            Using reader As New StreamReader(RedirectFile)
                Using MyClient As New HttpClient
                    MyClient.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("sso-key", My.Settings.MyCredentialsProd)

                    While Not reader.EndOfStream

                        ' Read each line of the redirects file
                        Dim line As String = reader.ReadLine

                        ' Split the line into two parts based on the comma
                        Dim redirect As String() = line.Split("|")

                        ' Make sure we have only two parts
                        If redirect.Length = 2 Then

                            ' Define each part 
                            Dim FQDN As String = Trim(redirect(0))
                            Dim ForwardURL As String = Trim(redirect(1))

                            ' Check to see if FQDN is related to the domain name we just imported
                            If FQDN = DomainName Or FQDN.EndsWith("." & DomainName) Then

                                ' Build redirect command
                                Dim MyBody As New StringBuilder
                                MyBody.Append("{"c)
                                MyBody.Append("""url"":""" & ForwardURL & """,")
                                MyBody.Append("""type"":""REDIRECT_PERMANENT""")
                                MyBody.Append("}"c)


                                ' Send forwarder command

                                Using MyRequest As New HttpRequestMessage
                                    MyRequest.Method = HttpMethod.Put
                                    MyRequest.RequestUri = New Uri("https://api.godaddy.com/v2/customers/17007dd8-2ea4-45c1-b3e1-06a5b54b38f6/domains/forwards/" & FQDN)
                                    MyRequest.Content = New StringContent(MyBody.ToString)
                                    MyRequest.Content.Headers.ContentType = New MediaTypeHeaderValue("application/json")
                                    Dim MyResponse = MyClient.Send(MyRequest)
                                    Dim MyResponseBody = MyResponse.Content.ReadAsStringAsync

                                    ' If our forwarder was not accepted, throw an exception with the redirect information
                                    If MyResponse.StatusCode = HttpStatusCode.OK Or MyResponse.StatusCode = HttpStatusCode.NoContent Then
                                        Log("Redirected " & FQDN & " to " & ForwardURL, LogFile)
                                        DidRedirects = True
                                    Else
                                        My.Computer.FileSystem.WriteAllText(WarningsFile, "Redirect From: " & FQDN & " To: " & ForwardURL & vbCrLf, True)
                                    End If



                                End Using ' MyRequest


                            End If
                        End If



                    End While

                End Using ' MyClient

            End Using

            If DidRedirects = True Then
                Log("Redirects complete", LogFile)
            Else
                Log("No redirects found", LogFile)
            End If

        Catch ex As Exception
            ' Something went very wrong, raise an exception to calling code
            Throw
        End Try

    End Sub

    Private Sub TestCall()
        Try



            'txtStatus.Clear()
            ClearStatus()

            Dim MyBody As New StringBuilder
            MyBody.Append("{"c)
            MyBody.Append("""url"":""http://www.14xtra.com"",")
            MyBody.Append("""type"":""REDIRECT_PERMANENT""")
            MyBody.Append("}"c)
            ''	12.181.23.64


            Using MyClient As New HttpClient
                MyClient.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("sso-key", My.Settings.MyCredentialsProd)
                Using MyRequest As New HttpRequestMessage
                    MyRequest.Method = HttpMethod.Put
                    MyRequest.RequestUri = New Uri("https://api.godaddy.com/v2/customers/17007dd8-2ea4-45c1-b3e1-06a5b54b38f6/domains/forwards/14xtra.com")
                    MyRequest.Content = New StringContent(MyBody.ToString)
                    MyRequest.Content.Headers.ContentType = New MediaTypeHeaderValue("application/json")
                    Dim MyResponse = MyClient.Send(MyRequest)
                    Dim MyResponseBody = MyResponse.Content.ReadAsStringAsync
                    Log(MyResponseBody.Result.ToString)
                End Using ' MyRequest
            End Using ' MyClient
            'txtStatus.Clear()

            'Dim MyBody As New StringBuilder
            'MyBody.Append("[")
            'MyBody.Append("{")
            'MyBody.Append("""data"":""pdns10.domaincontrol.com"",")
            'MyBody.Append("""name"":""@"",")
            'MyBody.Append("""type"":""NS""")
            'MyBody.Append("},")
            'MyBody.Append("{")
            'MyBody.Append("""data"":""pdns11.domaincontrol.com"",")
            'MyBody.Append("""name"":""@"",")
            'MyBody.Append("""type"":""NS""")
            'MyBody.Append("},")
            'MyBody.Append("{")
            'MyBody.Append("""data"":""12.181.23.64"",")
            'MyBody.Append("""name"":""@"",")
            'MyBody.Append("""type"":""A""")
            'MyBody.Append("}")
            'MyBody.Append("]")
            '''	12.181.23.64


            'Using MyClient As New HttpClient
            '    MyClient.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("sso-key", My.Settings.MyCredentialsProd)
            '    Using MyRequest As New HttpRequestMessage
            '        MyRequest.Method = HttpMethod.Put
            '        MyRequest.RequestUri = New Uri("https://api.godaddy.com/v1/domains/14xtra.com/records")
            '        MyRequest.Content = New StringContent(MyBody.ToString)
            '        MyRequest.Content.Headers.ContentType = New MediaTypeHeaderValue("application/json")
            '        Dim MyResponse = MyClient.Send(MyRequest)
            '        Dim MyResponseBody = MyResponse.Content.ReadAsStringAsync
            '        Log(MyResponseBody.Result.ToString)
            '    End Using ' MyRequest
            'End Using ' MyClient

        Catch ex As Exception

        End Try
    End Sub

    Private Function CheckNameserversInGoDaddy(domainName As String) As Boolean

        Dim ns As String = ""

        Using MyClient As New HttpClient
            MyClient.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("sso-key", My.Settings.MyCredentialsProd)
            Using MyRequest As New HttpRequestMessage
                MyRequest.Method = HttpMethod.Get
                MyRequest.RequestUri = New Uri("https://api.godaddy.com/v1/domains/" & domainName)
                Dim MyResponse = MyClient.Send(MyRequest)
                Dim MyResponseBody = MyResponse.Content.ReadAsStringAsync
                ns = MyResponseBody.Result.ToString
            End Using ' MyRequest
        End Using ' MyClient

        If ns.Contains("Not_Found") Then Throw New Exception("Domain not found in Gray tenant")

        If ns.Contains("domaincontrol.com") Then
            Return True
        Else
            Return False

        End If

    End Function

    Private Function SetNameserversInGoDaddy(domainName As String) As Boolean

        Try

            Dim MyBody As New StringBuilder
            MyBody.Append("{"c)
            MyBody.Append("""nameServers"":[""pdns09.domaincontrol.com"", ""pdns10.domaincontrol.com""]")
            MyBody.Append("}"c)

            Using MyClient As New HttpClient
                MyClient.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("sso-key", My.Settings.MyCredentialsProd)
                Using MyRequest As New HttpRequestMessage
                    MyRequest.Method = HttpMethod.Patch
                    MyRequest.RequestUri = New Uri("https://api.godaddy.com/v1/domains/" & domainName)
                    MyRequest.Content = New StringContent(MyBody.ToString)
                    MyRequest.Content.Headers.ContentType = New MediaTypeHeaderValue("application/json")
                    Dim MyResponse = MyClient.Send(MyRequest)
                    Dim MyResponseBody = MyResponse.Content.ReadAsStringAsync

                    If Not (MyResponse.StatusCode = HttpStatusCode.OK Or MyResponse.StatusCode = HttpStatusCode.NoContent) Then
                        Throw New Exception(MyResponse.StatusCode.ToString)
                    End If

                End Using ' MyRequest
            End Using ' MyClient

            Return True

        Catch ex As Exception
            Return False
        End Try

    End Function

    Private Sub Log(value As String, Optional LogFile As String = "")
        AddStatusMessage(value)
        'txtStatus.AppendText(value & vbCrLf)
        If Not String.IsNullOrWhiteSpace(LogFile) Then My.Computer.FileSystem.WriteAllText(LogFile, Now & ": " & value & vbCrLf, True)
    End Sub

    Private Sub ckOverwrite_CheckedChanged(sender As Object, e As EventArgs) Handles ckOverwrite.CheckedChanged
        tfCheckingOverwrite = True
        If tfCheckingAppend = False Then SetCkAppend(False)
        tfCheckingOverwrite = False
    End Sub

    Private Sub ckAppend_CheckedChanged(sender As Object, e As EventArgs) Handles ckAppend.CheckedChanged
        tfCheckingAppend = True
        If tfCheckingOverwrite = False Then SetCkOverwrite(False)
        tfCheckingAppend = False
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Dim t As New Thread(AddressOf DoDelete)
        t.Start()
    End Sub


    Private Sub DoDelete()


        SetFormColor(Color.Green)

        'txtDNSDeleteFail.Clear()
        'txtDNSDeleteSuccess.Clear()
        ClearFailMessage()
        ClearSuccessMessage()

        Using MyClient As New HttpClient

            MyClient.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("sso-key", My.Settings.MyCredentialsProd)

            Dim LineCount As Integer = txtDNSDeleteList.Lines.Count
            Dim i As Integer = 0

            For Each line As String In txtDNSDeleteList.Lines

                Dim SuccessA As Boolean = False
                Dim SuccessCNAME As Boolean = False
                Dim StatusMessage As String = line & ": "

                If Not String.IsNullOrWhiteSpace(line) Then

                    Dim lineparts() As String = Trim(line).Split(".")

                    If lineparts.Length >= 2 Then

                        Try

                            ' Get domain name
                            Dim DomainName As String = lineparts(lineparts.Length - 2) & "." & lineparts(lineparts.Length - 1)

                            ' Get record name
                            Dim Name As String = line.Replace(DomainName, "")
                            If Name.Length > 0 Then
                                Name = Name.Remove(Name.Length - 1, 1)
                            Else
                                Name = "@"
                            End If

                            ' Confirm GoDaddy is NameServer
                            Do
                                Dim ns As String = ""
                                MyClient.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("sso-key", My.Settings.MyCredentialsProd)
                                Using MyRequest As New HttpRequestMessage
                                    MyRequest.Method = HttpMethod.Get
                                    MyRequest.RequestUri = New Uri("https://api.godaddy.com/v1/domains/" & DomainName)
                                    Dim MyResponse = MyClient.Send(MyRequest)
                                    Dim MyResponseBody = MyResponse.Content.ReadAsStringAsync
                                    ns = MyResponseBody.Result.ToString
                                End Using ' MyRequest

                                If ns.Contains("TOO_MANY_REQUESTS") Then
                                    SetFormColor(Color.Red)
                                    Thread.Sleep(10000)
                                    SetFormColor(Color.Green)
                                ElseIf ns.Contains("Not_Found") Then
                                    Throw New Exception("Domain not found in GoDaddy")
                                ElseIf Not ns.Contains("domaincontrol.com") Then
                                    Throw New Exception("DNS not hosted by GoDaddy")
                                Else
                                    Exit Do
                                End If

                            Loop

                            ' Clear A records
                            Try
                                Do


                                    ' Build a web request to use to call the GoDaddy API
                                    Using MyRequest As New HttpRequestMessage

                                        MyRequest.Method = HttpMethod.Delete

                                        ' Define the GoDaddy API address we need to call, including the domain name we're editing
                                        MyRequest.RequestUri = New Uri("https://api.godaddy.com/v1/domains/" & DomainName & "/records/A/" & Name)

                                        ' Make the API call by
                                        Dim MyResponse = MyClient.Send(MyRequest)

                                        ' Read the response (Only ued for debugging to display the status code)
                                        Dim MyResponseBody = MyResponse.Content.ReadAsStringAsync

                                        ' Check results
                                        If (MyResponse.StatusCode = HttpStatusCode.OK Or MyResponse.StatusCode = HttpStatusCode.NoContent) Then
                                            StatusMessage += "A records cleared. "
                                            SuccessA = True
                                            Exit Do
                                        ElseIf MyResponse.StatusCode = HttpStatusCode.TooManyRequests Then
                                            SetFormColor(Color.Red)
                                            Thread.Sleep(10000)
                                            SetFormColor(Color.Green)
                                        Else
                                            StatusMessage += "A records not found. "
                                            SuccessA = True
                                            Exit Do
                                        End If



                                    End Using ' MyRequest

                                Loop

                            Catch ex As Exception
                                StatusMessage += "A records ERROR: " & ex.Message
                                SuccessA = False
                            End Try


                            ' Clear CNAME records
                            Try
                                Do


                                    ' Build a web request to use to call the GoDaddy API
                                    Using MyRequest As New HttpRequestMessage

                                        MyRequest.Method = HttpMethod.Delete

                                        ' Define the GoDaddy API address we need to call, including the domain name we're editing
                                        MyRequest.RequestUri = New Uri("https://api.godaddy.com/v1/domains/" & DomainName & "/records/CNAME/" & Name)

                                        ' Make the API call by
                                        Dim MyResponse = MyClient.Send(MyRequest)

                                        ' Read the response (Only ued for debugging to display the status code)
                                        Dim MyResponseBody = MyResponse.Content.ReadAsStringAsync

                                        ' Check Results
                                        If (MyResponse.StatusCode = HttpStatusCode.OK Or MyResponse.StatusCode = HttpStatusCode.NoContent) Then
                                            StatusMessage += "CNAME records cleared."
                                            SuccessCNAME = True
                                            Exit Do
                                        ElseIf MyResponse.StatusCode = HttpStatusCode.TooManyRequests Then
                                            SetFormColor(Color.Red)
                                            Thread.Sleep(10000)
                                            SetFormColor(Color.Green)
                                        Else
                                            StatusMessage += "CNAME records not found."
                                            SuccessCNAME = True
                                            Exit Do
                                        End If



                                    End Using ' MyRequest

                                Loop

                            Catch ex As Exception
                                StatusMessage += "CNAME records ERROR: " & ex.Message
                                SuccessCNAME = False
                            End Try

                        Catch ex As Exception
                            StatusMessage += ex.Message
                        End Try

                        If SuccessA And SuccessCNAME Then
                            AddSuccessMessage(StatusMessage)
                        Else
                            AddFailMessage(StatusMessage)
                        End If
                    Else
                        StatusMessage += "Text not recognized as a DNS entry"
                        AddFailMessage(StatusMessage)

                    End If

                End If

                i += 1


            Next

        End Using ' MyClient

        SetFormColor(Color.White)
    End Sub

    Sub AddStatusMessage(message As String)
        Try

            If txtStatus.InvokeRequired Then

                txtStatus.Invoke(Sub()
                                     AddStatusMessage(message)
                                 End Sub)
            Else
                txtStatus.AppendText(message & Environment.NewLine)
            End If

        Catch ex As Exception

        End Try
    End Sub

    Sub AppendGAStatusMessage(message As String, resultsbox As TextBox)
        Try

            If resultsbox.InvokeRequired Then

                resultsbox.Invoke(Sub()
                                      AppendGAStatusMessage(message, resultsbox)
                                  End Sub)
            Else
                resultsbox.AppendText(message & Environment.NewLine)
            End If

        Catch ex As Exception

        End Try
    End Sub

    Sub AddSuccessMessage(message As String)
        Try

            If txtDNSDeleteSuccess.InvokeRequired Then

                txtDNSDeleteSuccess.Invoke(Sub()
                                               AddSuccessMessage(message)
                                           End Sub)
            Else
                txtDNSDeleteSuccess.AppendText(message & Environment.NewLine)
            End If

        Catch ex As Exception

        End Try
    End Sub

    Sub ClearSuccessMessage()
        Try

            If txtDNSDeleteSuccess.InvokeRequired Then

                txtDNSDeleteSuccess.Invoke(Sub()
                                               ClearSuccessMessage()
                                           End Sub)
            Else
                txtDNSDeleteSuccess.Clear()
            End If

        Catch ex As Exception

        End Try
    End Sub

    Sub AddFailMessage(message As String)
        Try

            If txtDNSDeleteFail.InvokeRequired Then

                txtDNSDeleteFail.Invoke(Sub()
                                            AddFailMessage(message)
                                        End Sub)
            Else
                txtDNSDeleteFail.AppendText(message & Environment.NewLine)
            End If

        Catch ex As Exception

        End Try
    End Sub

    Sub ClearFailMessage()
        Try

            If txtDNSDeleteFail.InvokeRequired Then

                txtDNSDeleteFail.Invoke(Sub()
                                            ClearFailMessage()
                                        End Sub)
            Else
                txtDNSDeleteFail.Clear()
            End If

        Catch ex As Exception

        End Try
    End Sub

    Sub SetProgFolderMax(value As Integer)

        If progFolder.InvokeRequired Then

            progFolder.Invoke(Sub()
                                  SetProgFolderMax(value)
                              End Sub)
        Else
            progFolder.Maximum = value
        End If
    End Sub

    Sub SetProgFolderValue(value As Integer)

        If progFolder.InvokeRequired Then

            progFolder.Invoke(Sub()
                                  SetProgFolderValue(value)
                              End Sub)
        Else
            progFolder.Value = value
        End If
    End Sub

    Sub SetProgFileMax(value As Integer)

        If progFile.InvokeRequired Then

            progFile.Invoke(Sub()
                                SetProgFileMax(value)
                            End Sub)
        Else
            progFile.Maximum = value
        End If
    End Sub

    Sub SetProgFileValue(value As Integer)

        If progFile.InvokeRequired Then

            progFile.Invoke(Sub()
                                SetProgFileValue(value)
                            End Sub)
        Else
            progFile.Value = value
        End If
    End Sub

    Sub SetFormColor(value As Color)
        If Me.InvokeRequired Then

            Me.Invoke(Sub()
                          SetFormColor(value)
                      End Sub)
        Else
            Me.BackColor = value
        End If
    End Sub


    Sub ClearStatus()

        If txtStatus.InvokeRequired Then

            txtStatus.Invoke(Sub()
                                 ClearStatus()
                             End Sub)
        Else
            txtStatus.Clear()
        End If
    End Sub

    Sub SetCkAppend(value As Boolean)

        If ckAppend.InvokeRequired Then

            ckAppend.Invoke(Sub()
                                SetCkAppend(value)
                            End Sub)
        Else
            ckAppend.Checked = value
        End If
    End Sub

    Sub SetCkOverwrite(value As Boolean)

        If ckOverwrite.InvokeRequired Then

            ckOverwrite.Invoke(Sub()
                                   SetCkOverwrite(value)
                               End Sub)
        Else
            ckOverwrite.Checked = value
        End If
    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        txtDNSDeleteList.Clear()
    End Sub

    Private Sub btnGoAnywhere_Click(sender As Object, e As EventArgs) Handles btnGoAnywhere.Click

        Dim t1 As New Thread(AddressOf GoAnywhereUpdate1)
        t1.Start()

        'Dim t2 As New Thread(AddressOf GoAnywhereUpdate2)
        't2.Start()

        'Dim t3 As New Thread(AddressOf GoAnywhereUpdate3)
        't3.Start()

        ' DevAdmin3 10cf4602-aa8c-4845-b6b8-390470b22a82
        'DevAdmin3 21b07e4e-7bc3-410d-ab10-8cc3483c981a

    End Sub

    Private Sub GoAnywhereUpdate1()
        Try

            'Dim ResultsBox As TextBox = txtGAResults1
            Dim Filename As String = "C:\Users\james.jefferies\OneDrive - Gray Television, Inc\Desktop\WebUsers.txt"


            'Dim Template As String = "OneLoginWebUser"
            Dim ListUsers As New List(Of String)

            Using MyClient As New HttpClient
                MyClient.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", "79a35a7a-2801-4962-8898-6644d313057c")

                ' Read users into a list
                Using s As New StreamReader(Filename)
                    While Not s.EndOfStream
                        ListUsers.Add(s.ReadLine)
                    End While
                End Using 's


                For Each username In ListUsers
                    Try
                        AppendGAStatusMessage("Working on " & username, txtGAResults1)
                        'AppendGAStatusMessage("Updating " & username, txtGAResults1) ' folder udpate

                        Dim MyWebUser As New WebUser

                        'Get user data
                        Using MyRequest As New HttpRequestMessage

                            ' Create web request
                            MyRequest.Method = HttpMethod.Get
                            MyRequest.RequestUri = New Uri("https://gray.goanywhere.cloud:8001/goanywhere/rest/gacmd/v1/webusers/" & username)

                            ' Send web request using client, get response
                            Dim MyResponse = MyClient.Send(MyRequest)

                            ' Read response
                            Dim MyResponseStatus = MyResponse.StatusCode
                            Dim MyResponseBody = MyResponse.Content.ReadAsStringAsync

                            If MyResponseStatus = HttpStatusCode.OK Then

                                Dim UserXML As New XmlDocument
                                UserXML.LoadXml(MyResponseBody.Result)

                                MyWebUser.Username = username

                                If IsNothing(UserXML.SelectSingleNode("/webUsers/webUser/firstName")) = False Then MyWebUser.FirstName = UserXML.SelectSingleNode("/webUsers/webUser/firstName").InnerText
                                If IsNothing(UserXML.SelectSingleNode("/webUsers/webUser/lastName")) = False Then MyWebUser.LastName = UserXML.SelectSingleNode("/webUsers/webUser/lastName").InnerText
                                If IsNothing(UserXML.SelectSingleNode("/webUsers/webUser/email")) = False Then MyWebUser.Email = UserXML.SelectSingleNode("/webUsers/webUser/email").InnerText
                                If IsNothing(UserXML.SelectSingleNode("/webUsers/webUser/organization")) = False Then MyWebUser.Organization = UserXML.SelectSingleNode("/webUsers/webUser/organization").InnerText
                                If IsNothing(UserXML.SelectSingleNode("/webUsers/webUser/webGroups/webGroup/name")) = False Then
                                    MyWebUser.Group = UserXML.SelectSingleNode("/webUsers/webUser/webGroups/webGroup/name").InnerText
                                Else
                                    MyWebUser.Group = MyWebUser.Organization
                                End If

                                AppendGAStatusMessage("Received user data", txtGAResults1)
                            Else
                                Throw New Exception("Unable to get user data for " & username)
                            End If

                        End Using ' MyRequest


                        ' Delete user
                        Using MyRequest As New HttpRequestMessage

                            ' Create web request
                            MyRequest.Method = HttpMethod.Delete
                            MyRequest.RequestUri = New Uri("https://gray.goanywhere.cloud:8001/goanywhere/rest/gacmd/v1/webusers/" & username)

                            ' Send web request using client, get response
                            Dim MyResponse = MyClient.Send(MyRequest)

                            ' Read response
                            Dim MyResponseStatus = MyResponse.StatusCode
                            Dim MyResponseBody = MyResponse.Content.ReadAsStringAsync

                            If MyResponseStatus = HttpStatusCode.OK Then
                                AppendGAStatusMessage("Deleted user", txtGAResults1)
                            Else
                                Throw New Exception("Error deleting user: " & username)
                            End If

                        End Using 'MyRequest



                        ' Add user back
                        Using MyRequest As New HttpRequestMessage
                            ' Create web request
                            MyRequest.Method = HttpMethod.Post
                            MyRequest.RequestUri = New Uri("https://gray.goanywhere.cloud:8001/goanywhere/rest/gacmd/v1/webusers/")

                            ' Update user folder (not used)
                            ' MyRequest.Method = HttpMethod.Put
                            ' MyRequest.RequestUri = New Uri("https://gray.goanywhere.cloud:8001/goanywhere/rest/gacmd/v1/webusers/" & username & "/virtualfolders")

                            ' Create JSON to add user
                            Dim MyBody As New StringBuilder
                            MyBody.AppendLine("{")
                            MyBody.AppendLine("""addParameters"" : ")
                            MyBody.AppendLine("{")
                            MyBody.AppendLine("""userName"" : """ & MyWebUser.Username & """,")
                            MyBody.AppendLine("""template"" : ""Employee Web User Template"",")
                            MyBody.AppendLine("""firstName"" : """ & MyWebUser.FirstName & """,")
                            MyBody.AppendLine("""lastName"" : """ & MyWebUser.LastName & """,")
                            MyBody.AppendLine("""email"" : """ & MyWebUser.Email & """,")
                            MyBody.AppendLine("""organization"" : """ & MyWebUser.Organization & """")
                            MyBody.AppendLine("}")
                            MyBody.AppendLine("}")
                            MyRequest.Content = New StringContent(MyBody.ToString)
                            MyRequest.Content.Headers.ContentType = New MediaTypeHeaderValue("application/json")

                            '' Create JSON to edit folder
                            ' Dim MyBody As New StringBuilder
                            ' MyBody.AppendLine("{")
                            ' MyBody.AppendLine("""virtualPath"" : ""/Gray"",")
                            ' MyBody.AppendLine("""permissions"" : [""list"",""download""]")
                            ' MyBody.AppendLine("}")


                            Dim MyBodyString = MyBody.ToString
                            MyRequest.Content = New StringContent(MyBodyString)
                            MyRequest.Content.Headers.ContentType = New MediaTypeHeaderValue("application/json")

                            ' Send web request using client, get response
                            Dim MyResponse = MyClient.Send(MyRequest)

                            ' Read response
                            Dim MyResponseStatus = MyResponse.StatusCode
                            Dim MyResponseBody = MyResponse.Content.ReadAsStringAsync

                            ' Check the response
                            If MyResponseStatus = (HttpStatusCode.OK Or HttpStatusCode.Created) Then
                                AppendGAStatusMessage("Updated " & username, txtGAResults1)
                            Else
                                AppendGAStatusMessage("Error " & username & ": " & MyResponseStatus.ToString, txtGAResults2)
                            End If

                        End Using 'MyRequest


                        ' Add group to user
                        Using MyRequest As New HttpRequestMessage

                            ' Create web request
                            MyRequest.Method = HttpMethod.Post
                            MyRequest.RequestUri = New Uri("https://gray.goanywhere.cloud:8001/goanywhere/rest/gacmd/v1/webusers/" & username & "/groups")

                            ' Create JSON to add user
                            Dim MyBody As New StringBuilder
                            MyBody.AppendLine("{")
                            MyBody.AppendLine("""action"" : ""add"",")
                            MyBody.AppendLine("""groupName"" : """ & MyWebUser.Group & """")
                            MyBody.AppendLine("}")
                            MyRequest.Content = New StringContent(MyBody.ToString)
                            MyRequest.Content.Headers.ContentType = New MediaTypeHeaderValue("application/json")


                            ' Send web request using client, get response
                            Dim MyResponse = MyClient.Send(MyRequest)

                            ' Read response
                            Dim MyResponseStatus = MyResponse.StatusCode
                            Dim MyResponseBody = MyResponse.Content.ReadAsStringAsync

                            ' Check the response
                            If MyResponseStatus = (HttpStatusCode.OK) Then
                                'AppendGAStatusMessage("Group added", txtGAResults1)
                            Else
                                Throw New Exception("Error Adding user: " & username)
                            End If

                        End Using 'MyRequest

                        AppendGAStatusMessage("", txtGAResults1)

                    Catch ex As Exception
                        AppendGAStatusMessage(ex.Message, txtGAResults2)
                        AppendGAStatusMessage("", txtGAResults2)
                    End Try

                    '  AppendGAStatusMessage("", txtGAResults1)


                Next

            End Using ' MyClient


        Catch ex As Exception




        End Try

    End Sub

    Private Sub GoAnywhereUpdate2()
        Try
            Dim ResultsBox As TextBox = txtGAResults2
            Dim Filename As String = "C:\Users\james.jefferies\OneDrive - Gray Television, Inc\Desktop\WebUserList2.txt"


            Dim Template As String = "OneLoginWebUser"
            Dim ListUsers As New List(Of String)

            Using MyClient As New HttpClient
                MyClient.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", "10cf4602-aa8c-4845-b6b8-390470b22a82")

                ' Read users into a list
                Using s As New StreamReader(Filename)
                    While Not s.EndOfStream
                        ListUsers.Add(s.ReadLine)
                    End While
                End Using 's


                For Each username In ListUsers
                    Try
                        AppendGAStatusMessage("Working on " & username, ResultsBox)

                        Dim MyWebUser As New WebUser

                        ' Get user data
                        Using MyRequest As New HttpRequestMessage

                            ' Create web request
                            MyRequest.Method = HttpMethod.Get
                            MyRequest.RequestUri = New Uri("https://gray.goanywhere.cloud:8001/goanywhere/rest/gacmd/v1/webusers/" & username)

                            ' Send web request using client, get response
                            Dim MyResponse = MyClient.Send(MyRequest)

                            ' Read response
                            Dim MyResponseStatus = MyResponse.StatusCode
                            Dim MyResponseBody = MyResponse.Content.ReadAsStringAsync

                            If MyResponseStatus = HttpStatusCode.OK Then

                                Dim UserXML As New XmlDocument
                                UserXML.LoadXml(MyResponseBody.Result)

                                MyWebUser.Username = username

                                If IsNothing(UserXML.SelectSingleNode("/webUsers/webUser/firstName")) = False Then MyWebUser.FirstName = UserXML.SelectSingleNode("/webUsers/webUser/firstName").InnerText
                                If IsNothing(UserXML.SelectSingleNode("/webUsers/webUser/lastName")) = False Then MyWebUser.LastName = UserXML.SelectSingleNode("/webUsers/webUser/lastName").InnerText
                                If IsNothing(UserXML.SelectSingleNode("/webUsers/webUser/email")) = False Then MyWebUser.Email = UserXML.SelectSingleNode("/webUsers/webUser/email").InnerText
                                If IsNothing(UserXML.SelectSingleNode("/webUsers/webUser/organization")) = False Then MyWebUser.Organization = UserXML.SelectSingleNode("/webUsers/webUser/organization").InnerText
                                If IsNothing(UserXML.SelectSingleNode("/webUsers/webUser/webGroups/webGroup/name")) = False Then
                                    MyWebUser.Group = UserXML.SelectSingleNode("/webUsers/webUser/webGroups/webGroup/name").InnerText
                                Else
                                    MyWebUser.Group = MyWebUser.Organization
                                End If


                                Try

                                Catch ex As Exception
                                    MyWebUser.Group = UserXML.SelectSingleNode("/webUsers/webUser/organization").InnerText
                                End Try



                                AppendGAStatusMessage("Received user data", ResultsBox)
                            Else
                                Throw New Exception("Unable to get user data")
                            End If

                        End Using ' MyRequest


                        ' Delete user
                        Using MyRequest As New HttpRequestMessage

                            ' Create web request
                            MyRequest.Method = HttpMethod.Delete
                            MyRequest.RequestUri = New Uri("https://gray.goanywhere.cloud:8001/goanywhere/rest/gacmd/v1/webusers/" & username)

                            ' Send web request using client, get response
                            Dim MyResponse = MyClient.Send(MyRequest)

                            ' Read response
                            Dim MyResponseStatus = MyResponse.StatusCode
                            Dim MyResponseBody = MyResponse.Content.ReadAsStringAsync

                            If MyResponseStatus = HttpStatusCode.OK Then
                                AppendGAStatusMessage("Deleted user", ResultsBox)
                            Else
                                Throw New Exception("Error deleting user: " & username & vbCrLf)
                            End If

                        End Using 'MyRequest



                        ' Add user back
                        Using MyRequest As New HttpRequestMessage
                            ' Create web request
                            MyRequest.Method = HttpMethod.Post
                            MyRequest.RequestUri = New Uri("https://gray.goanywhere.cloud:8001/goanywhere/rest/gacmd/v1/webusers/")

                            ' Create JSON to add user
                            Dim MyBody As New StringBuilder
                            MyBody.AppendLine("{")
                            MyBody.AppendLine("""addParameters"" : ")
                            MyBody.AppendLine("{")
                            MyBody.AppendLine("""userName"" : """ & MyWebUser.Username & """,")
                            MyBody.AppendLine("""template"" : ""OneLoginWebUser"",")
                            MyBody.AppendLine("""firstName"" : """ & MyWebUser.FirstName & """,")
                            MyBody.AppendLine("""lastName"" : """ & MyWebUser.LastName & """,")
                            MyBody.AppendLine("""email"" : """ & MyWebUser.Email & """,")
                            MyBody.AppendLine("""organization"" : """ & MyWebUser.Organization & """")
                            MyBody.AppendLine("}")
                            MyBody.AppendLine("}")
                            MyRequest.Content = New StringContent(MyBody.ToString)
                            MyRequest.Content.Headers.ContentType = New MediaTypeHeaderValue("application/json")

                            ' Send web request using client, get response
                            Dim MyResponse = MyClient.Send(MyRequest)

                            ' Read response
                            Dim MyResponseStatus = MyResponse.StatusCode
                            Dim MyResponseBody = MyResponse.Content.ReadAsStringAsync

                            ' Check the response
                            If MyResponseStatus = (HttpStatusCode.OK Or HttpStatusCode.Created) Then
                                AppendGAStatusMessage("Added user", ResultsBox)
                            Else
                                Throw New Exception("Error Adding user: " & username & vbCrLf)
                            End If

                        End Using 'MyRequest


                        ' Add group to user
                        Using MyRequest As New HttpRequestMessage

                            ' Create web request
                            MyRequest.Method = HttpMethod.Post
                            MyRequest.RequestUri = New Uri("https://gray.goanywhere.cloud:8001/goanywhere/rest/gacmd/v1/webusers/" & username & "/groups")

                            ' Create JSON to add user
                            Dim MyBody As New StringBuilder
                            MyBody.AppendLine("{")
                            MyBody.AppendLine("""action"" : ""add"",")
                            MyBody.AppendLine("""groupName"" : """ & MyWebUser.Group & """")
                            MyBody.AppendLine("}")
                            MyRequest.Content = New StringContent(MyBody.ToString)
                            MyRequest.Content.Headers.ContentType = New MediaTypeHeaderValue("application/json")


                            ' Send web request using client, get response
                            Dim MyResponse = MyClient.Send(MyRequest)

                            ' Read response
                            Dim MyResponseStatus = MyResponse.StatusCode
                            Dim MyResponseBody = MyResponse.Content.ReadAsStringAsync

                            ' Check the response
                            If MyResponseStatus = (HttpStatusCode.OK) Then
                                AppendGAStatusMessage("Group added", ResultsBox)
                            Else
                                Throw New Exception("Error Adding user: " & username & vbCrLf)
                            End If

                        End Using 'MyRequest


                    Catch ex As Exception
                        AppendGAStatusMessage(ex.Message, ResultsBox)
                    End Try

                    AppendGAStatusMessage("", ResultsBox)


                Next

            End Using ' MyClient



        Catch ex As Exception




        End Try

    End Sub

    Private Sub GoAnywhereUpdate3()
        Try

            Dim ResultsBox As TextBox = txtGAResults3
            Dim Filename As String = "C:\Users\james.jefferies\OneDrive - Gray Television, Inc\Desktop\WebUserList3.txt"

            Dim Template As String = "OneLoginWebUser"
            Dim ListUsers As New List(Of String)

            Using MyClient As New HttpClient
                MyClient.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", "21b07e4e-7bc3-410d-ab10-8cc3483c981a")

                ' Read users into a list
                Using s As New StreamReader(Filename)
                    While Not s.EndOfStream
                        ListUsers.Add(s.ReadLine)
                    End While
                End Using 's


                For Each username In ListUsers
                    Try
                        AppendGAStatusMessage("Working on " & username, ResultsBox)

                        Dim MyWebUser As New WebUser

                        ' Get user data
                        Using MyRequest As New HttpRequestMessage

                            ' Create web request
                            MyRequest.Method = HttpMethod.Get
                            MyRequest.RequestUri = New Uri("https://gray.goanywhere.cloud:8001/goanywhere/rest/gacmd/v1/webusers/" & username)

                            ' Send web request using client, get response
                            Dim MyResponse = MyClient.Send(MyRequest)

                            ' Read response
                            Dim MyResponseStatus = MyResponse.StatusCode
                            Dim MyResponseBody = MyResponse.Content.ReadAsStringAsync

                            If MyResponseStatus = HttpStatusCode.OK Then

                                Dim UserXML As New XmlDocument
                                UserXML.LoadXml(MyResponseBody.Result)

                                MyWebUser.Username = username

                                If IsNothing(UserXML.SelectSingleNode("/webUsers/webUser/firstName")) = False Then MyWebUser.FirstName = UserXML.SelectSingleNode("/webUsers/webUser/firstName").InnerText
                                If IsNothing(UserXML.SelectSingleNode("/webUsers/webUser/lastName")) = False Then MyWebUser.LastName = UserXML.SelectSingleNode("/webUsers/webUser/lastName").InnerText
                                If IsNothing(UserXML.SelectSingleNode("/webUsers/webUser/email")) = False Then MyWebUser.Email = UserXML.SelectSingleNode("/webUsers/webUser/email").InnerText
                                If IsNothing(UserXML.SelectSingleNode("/webUsers/webUser/organization")) = False Then MyWebUser.Organization = UserXML.SelectSingleNode("/webUsers/webUser/organization").InnerText
                                If IsNothing(UserXML.SelectSingleNode("/webUsers/webUser/webGroups/webGroup/name")) = False Then
                                    MyWebUser.Group = UserXML.SelectSingleNode("/webUsers/webUser/webGroups/webGroup/name").InnerText
                                Else
                                    MyWebUser.Group = MyWebUser.Organization
                                End If


                                Try

                                Catch ex As Exception
                                    MyWebUser.Group = UserXML.SelectSingleNode("/webUsers/webUser/organization").InnerText
                                End Try



                                AppendGAStatusMessage("Received user data", ResultsBox)
                            Else
                                Throw New Exception("Unable to get user data")
                            End If

                        End Using ' MyRequest


                        ' Delete user
                        Using MyRequest As New HttpRequestMessage

                            ' Create web request
                            MyRequest.Method = HttpMethod.Delete
                            MyRequest.RequestUri = New Uri("https://gray.goanywhere.cloud:8001/goanywhere/rest/gacmd/v1/webusers/" & username)

                            ' Send web request using client, get response
                            Dim MyResponse = MyClient.Send(MyRequest)

                            ' Read response
                            Dim MyResponseStatus = MyResponse.StatusCode
                            Dim MyResponseBody = MyResponse.Content.ReadAsStringAsync

                            If MyResponseStatus = HttpStatusCode.OK Then
                                AppendGAStatusMessage("Deleted user", ResultsBox)
                            Else
                                Throw New Exception("Error deleting user: " & username & vbCrLf)
                            End If

                        End Using 'MyRequest



                        ' Add user back
                        Using MyRequest As New HttpRequestMessage
                            ' Create web request
                            MyRequest.Method = HttpMethod.Post
                            MyRequest.RequestUri = New Uri("https://gray.goanywhere.cloud:8001/goanywhere/rest/gacmd/v1/webusers/")

                            ' Create JSON to add user
                            Dim MyBody As New StringBuilder
                            MyBody.AppendLine("{")
                            MyBody.AppendLine("""addParameters"" : ")
                            MyBody.AppendLine("{")
                            MyBody.AppendLine("""userName"" : """ & MyWebUser.Username & """,")
                            MyBody.AppendLine("""template"" : ""OneLoginWebUser"",")
                            MyBody.AppendLine("""firstName"" : """ & MyWebUser.FirstName & """,")
                            MyBody.AppendLine("""lastName"" : """ & MyWebUser.LastName & """,")
                            MyBody.AppendLine("""email"" : """ & MyWebUser.Email & """,")
                            MyBody.AppendLine("""organization"" : """ & MyWebUser.Organization & """")
                            MyBody.AppendLine("}")
                            MyBody.AppendLine("}")
                            MyRequest.Content = New StringContent(MyBody.ToString)
                            MyRequest.Content.Headers.ContentType = New MediaTypeHeaderValue("application/json")

                            ' Send web request using client, get response
                            Dim MyResponse = MyClient.Send(MyRequest)

                            ' Read response
                            Dim MyResponseStatus = MyResponse.StatusCode
                            Dim MyResponseBody = MyResponse.Content.ReadAsStringAsync

                            ' Check the response
                            If MyResponseStatus = (HttpStatusCode.OK Or HttpStatusCode.Created) Then
                                AppendGAStatusMessage("Added user", ResultsBox)
                            Else
                                Throw New Exception("Error Adding user: " & username & vbCrLf)
                            End If

                        End Using 'MyRequest


                        ' Add group to user
                        Using MyRequest As New HttpRequestMessage

                            ' Create web request
                            MyRequest.Method = HttpMethod.Post
                            MyRequest.RequestUri = New Uri("https://gray.goanywhere.cloud:8001/goanywhere/rest/gacmd/v1/webusers/" & username & "/groups")

                            ' Create JSON to add user
                            Dim MyBody As New StringBuilder
                            MyBody.AppendLine("{")
                            MyBody.AppendLine("""action"" : ""add"",")
                            MyBody.AppendLine("""groupName"" : """ & MyWebUser.Group & """")
                            MyBody.AppendLine("}")
                            MyRequest.Content = New StringContent(MyBody.ToString)
                            MyRequest.Content.Headers.ContentType = New MediaTypeHeaderValue("application/json")


                            ' Send web request using client, get response
                            Dim MyResponse = MyClient.Send(MyRequest)

                            ' Read response
                            Dim MyResponseStatus = MyResponse.StatusCode
                            Dim MyResponseBody = MyResponse.Content.ReadAsStringAsync

                            ' Check the response
                            If MyResponseStatus = (HttpStatusCode.OK) Then
                                AppendGAStatusMessage("Group added", ResultsBox)
                            Else
                                Throw New Exception("Error Adding user: " & username & vbCrLf)
                            End If

                        End Using 'MyRequest


                    Catch ex As Exception
                        AppendGAStatusMessage(ex.Message, ResultsBox)
                    End Try

                    AppendGAStatusMessage("", ResultsBox)


                Next

            End Using ' MyClient



        Catch ex As Exception




        End Try

    End Sub

    Private Sub btnLoad_Click(sender As Object, e As EventArgs) Handles btnLoad.Click
        txtInputList.Text = System.IO.File.ReadAllText("TextFile1.txt")
    End Sub

    Private Sub btnProcess_Click(sender As Object, e As EventArgs) Handles btnProcess.Click
        Dim strText() As String
        strText = Split(txtInputList.Text, vbCrLf)
        txtInputList.Text = String.Join(vbCrLf, strText, 1, strText.Length - 1)
        txtComplete.AppendText(strText(0) + vbCrLf)
    End Sub
End Class

