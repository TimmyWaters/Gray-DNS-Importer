<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.btnSelectFolder = New System.Windows.Forms.Button()
        Me.lblFolderPath = New System.Windows.Forms.Label()
        Me.btnImport = New System.Windows.Forms.Button()
        Me.txtStatus = New System.Windows.Forms.TextBox()
        Me.dlgFolder = New System.Windows.Forms.FolderBrowserDialog()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.ckOverwrite = New System.Windows.Forms.CheckBox()
        Me.ckAppend = New System.Windows.Forms.CheckBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ckSkipNSCheck = New System.Windows.Forms.CheckBox()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.progFile = New System.Windows.Forms.ProgressBar()
        Me.progFolder = New System.Windows.Forms.ProgressBar()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txtDNSDeleteList = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtDNSDeleteSuccess = New System.Windows.Forms.TextBox()
        Me.txtDNSDeleteFail = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.btnClear = New System.Windows.Forms.Button()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.txtGAResults3 = New System.Windows.Forms.TextBox()
        Me.txtGAResults2 = New System.Windows.Forms.TextBox()
        Me.txtGAResults1 = New System.Windows.Forms.TextBox()
        Me.btnGoAnywhere = New System.Windows.Forms.Button()
        Me.TabPage4 = New System.Windows.Forms.TabPage()
        Me.btnProcess = New System.Windows.Forms.Button()
        Me.btnLoad = New System.Windows.Forms.Button()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.txtComplete = New System.Windows.Forms.TextBox()
        Me.txtInputList = New System.Windows.Forms.TextBox()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        Me.TabPage4.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnSelectFolder
        '
        Me.btnSelectFolder.Location = New System.Drawing.Point(6, 6)
        Me.btnSelectFolder.Name = "btnSelectFolder"
        Me.btnSelectFolder.Size = New System.Drawing.Size(93, 23)
        Me.btnSelectFolder.TabIndex = 0
        Me.btnSelectFolder.Text = "Select Folder"
        Me.btnSelectFolder.UseVisualStyleBackColor = True
        '
        'lblFolderPath
        '
        Me.lblFolderPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblFolderPath.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
        Me.lblFolderPath.Location = New System.Drawing.Point(105, 6)
        Me.lblFolderPath.Name = "lblFolderPath"
        Me.lblFolderPath.Size = New System.Drawing.Size(147, 23)
        Me.lblFolderPath.TabIndex = 1
        Me.lblFolderPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnImport
        '
        Me.btnImport.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnImport.Location = New System.Drawing.Point(612, 7)
        Me.btnImport.Name = "btnImport"
        Me.btnImport.Size = New System.Drawing.Size(75, 23)
        Me.btnImport.TabIndex = 2
        Me.btnImport.Text = "Import"
        Me.btnImport.UseVisualStyleBackColor = True
        '
        'txtStatus
        '
        Me.txtStatus.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtStatus.Location = New System.Drawing.Point(6, 35)
        Me.txtStatus.Multiline = True
        Me.txtStatus.Name = "txtStatus"
        Me.txtStatus.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtStatus.Size = New System.Drawing.Size(681, 420)
        Me.txtStatus.TabIndex = 3
        Me.txtStatus.WordWrap = False
        '
        'ckOverwrite
        '
        Me.ckOverwrite.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ckOverwrite.AutoSize = True
        Me.ckOverwrite.Location = New System.Drawing.Point(343, 10)
        Me.ckOverwrite.Name = "ckOverwrite"
        Me.ckOverwrite.Size = New System.Drawing.Size(77, 19)
        Me.ckOverwrite.TabIndex = 4
        Me.ckOverwrite.Text = "Overwrite"
        Me.ckOverwrite.UseVisualStyleBackColor = True
        '
        'ckAppend
        '
        Me.ckAppend.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ckAppend.AutoSize = True
        Me.ckAppend.Location = New System.Drawing.Point(426, 10)
        Me.ckAppend.Name = "ckAppend"
        Me.ckAppend.Size = New System.Drawing.Size(68, 19)
        Me.ckAppend.TabIndex = 5
        Me.ckAppend.Text = "Append"
        Me.ckAppend.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.Label1.Location = New System.Drawing.Point(258, 12)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(79, 13)
        Me.Label1.TabIndex = 6
        Me.Label1.Text = "Emergecy Use:"
        '
        'ckSkipNSCheck
        '
        Me.ckSkipNSCheck.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ckSkipNSCheck.AutoSize = True
        Me.ckSkipNSCheck.Location = New System.Drawing.Point(500, 10)
        Me.ckSkipNSCheck.Name = "ckSkipNSCheck"
        Me.ckSkipNSCheck.Size = New System.Drawing.Size(102, 19)
        Me.ckSkipNSCheck.TabIndex = 7
        Me.ckSkipNSCheck.Text = "Skip NS Check"
        Me.ckSkipNSCheck.UseVisualStyleBackColor = True
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Controls.Add(Me.TabPage3)
        Me.TabControl1.Controls.Add(Me.TabPage4)
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl1.Location = New System.Drawing.Point(0, 0)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(742, 545)
        Me.TabControl1.TabIndex = 10
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.progFile)
        Me.TabPage1.Controls.Add(Me.progFolder)
        Me.TabPage1.Controls.Add(Me.btnSelectFolder)
        Me.TabPage1.Controls.Add(Me.btnImport)
        Me.TabPage1.Controls.Add(Me.ckSkipNSCheck)
        Me.TabPage1.Controls.Add(Me.txtStatus)
        Me.TabPage1.Controls.Add(Me.ckOverwrite)
        Me.TabPage1.Controls.Add(Me.lblFolderPath)
        Me.TabPage1.Controls.Add(Me.ckAppend)
        Me.TabPage1.Controls.Add(Me.Label1)
        Me.TabPage1.Location = New System.Drawing.Point(4, 24)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(734, 517)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Importer"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'progFile
        '
        Me.progFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.progFile.Location = New System.Drawing.Point(6, 461)
        Me.progFile.Name = "progFile"
        Me.progFile.Size = New System.Drawing.Size(681, 10)
        Me.progFile.TabIndex = 9
        '
        'progFolder
        '
        Me.progFolder.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.progFolder.Location = New System.Drawing.Point(6, 477)
        Me.progFolder.Name = "progFolder"
        Me.progFolder.Size = New System.Drawing.Size(681, 10)
        Me.progFolder.TabIndex = 8
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.TableLayoutPanel1)
        Me.TabPage2.Location = New System.Drawing.Point(4, 24)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(734, 517)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Deleter"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 5
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.Label4, 4, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.txtDNSDeleteList, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Label3, 3, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.txtDNSDeleteSuccess, 3, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.txtDNSDeleteFail, 4, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Label2, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.btnDelete, 2, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.btnClear, 1, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(3, 3)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(728, 511)
        Me.TableLayoutPanel1.TabIndex = 13
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label4.Location = New System.Drawing.Point(479, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(246, 30)
        Me.Label4.TabIndex = 10
        Me.Label4.Text = "Fail"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'txtDNSDeleteList
        '
        Me.txtDNSDeleteList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.SetColumnSpan(Me.txtDNSDeleteList, 3)
        Me.txtDNSDeleteList.Location = New System.Drawing.Point(3, 33)
        Me.txtDNSDeleteList.Multiline = True
        Me.txtDNSDeleteList.Name = "txtDNSDeleteList"
        Me.txtDNSDeleteList.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtDNSDeleteList.Size = New System.Drawing.Size(219, 475)
        Me.txtDNSDeleteList.TabIndex = 4
        Me.txtDNSDeleteList.WordWrap = False
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label3.Location = New System.Drawing.Point(228, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(245, 30)
        Me.Label3.TabIndex = 8
        Me.Label3.Text = "Success"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'txtDNSDeleteSuccess
        '
        Me.txtDNSDeleteSuccess.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtDNSDeleteSuccess.Location = New System.Drawing.Point(228, 33)
        Me.txtDNSDeleteSuccess.Multiline = True
        Me.txtDNSDeleteSuccess.Name = "txtDNSDeleteSuccess"
        Me.txtDNSDeleteSuccess.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtDNSDeleteSuccess.Size = New System.Drawing.Size(245, 475)
        Me.txtDNSDeleteSuccess.TabIndex = 11
        Me.txtDNSDeleteSuccess.WordWrap = False
        '
        'txtDNSDeleteFail
        '
        Me.txtDNSDeleteFail.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtDNSDeleteFail.Location = New System.Drawing.Point(479, 33)
        Me.txtDNSDeleteFail.Multiline = True
        Me.txtDNSDeleteFail.Name = "txtDNSDeleteFail"
        Me.txtDNSDeleteFail.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtDNSDeleteFail.Size = New System.Drawing.Size(246, 475)
        Me.txtDNSDeleteFail.TabIndex = 12
        Me.txtDNSDeleteFail.WordWrap = False
        '
        'Label2
        '
        Me.Label2.AutoEllipsis = True
        Me.Label2.AutoSize = True
        Me.Label2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label2.Location = New System.Drawing.Point(3, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(69, 30)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "DNS to delete"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnDelete
        '
        Me.btnDelete.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnDelete.Location = New System.Drawing.Point(153, 3)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(69, 24)
        Me.btnDelete.TabIndex = 6
        Me.btnDelete.Text = "Delete"
        Me.btnDelete.UseVisualStyleBackColor = True
        '
        'btnClear
        '
        Me.btnClear.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnClear.Location = New System.Drawing.Point(78, 3)
        Me.btnClear.Name = "btnClear"
        Me.btnClear.Size = New System.Drawing.Size(69, 24)
        Me.btnClear.TabIndex = 13
        Me.btnClear.Text = "Clear"
        Me.btnClear.UseVisualStyleBackColor = True
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.txtGAResults3)
        Me.TabPage3.Controls.Add(Me.txtGAResults2)
        Me.TabPage3.Controls.Add(Me.txtGAResults1)
        Me.TabPage3.Controls.Add(Me.btnGoAnywhere)
        Me.TabPage3.Location = New System.Drawing.Point(4, 24)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Size = New System.Drawing.Size(734, 517)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "GoAnywhere"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'txtGAResults3
        '
        Me.txtGAResults3.Location = New System.Drawing.Point(490, 32)
        Me.txtGAResults3.Multiline = True
        Me.txtGAResults3.Name = "txtGAResults3"
        Me.txtGAResults3.Size = New System.Drawing.Size(235, 417)
        Me.txtGAResults3.TabIndex = 3
        '
        'txtGAResults2
        '
        Me.txtGAResults2.Location = New System.Drawing.Point(249, 32)
        Me.txtGAResults2.Multiline = True
        Me.txtGAResults2.Name = "txtGAResults2"
        Me.txtGAResults2.Size = New System.Drawing.Size(235, 417)
        Me.txtGAResults2.TabIndex = 2
        '
        'txtGAResults1
        '
        Me.txtGAResults1.Location = New System.Drawing.Point(8, 32)
        Me.txtGAResults1.Multiline = True
        Me.txtGAResults1.Name = "txtGAResults1"
        Me.txtGAResults1.Size = New System.Drawing.Size(235, 417)
        Me.txtGAResults1.TabIndex = 1
        '
        'btnGoAnywhere
        '
        Me.btnGoAnywhere.Location = New System.Drawing.Point(8, 3)
        Me.btnGoAnywhere.Name = "btnGoAnywhere"
        Me.btnGoAnywhere.Size = New System.Drawing.Size(75, 23)
        Me.btnGoAnywhere.TabIndex = 0
        Me.btnGoAnywhere.Text = "Go"
        Me.btnGoAnywhere.UseVisualStyleBackColor = True
        '
        'TabPage4
        '
        Me.TabPage4.Controls.Add(Me.btnProcess)
        Me.TabPage4.Controls.Add(Me.btnLoad)
        Me.TabPage4.Controls.Add(Me.ProgressBar1)
        Me.TabPage4.Controls.Add(Me.txtComplete)
        Me.TabPage4.Controls.Add(Me.txtInputList)
        Me.TabPage4.Location = New System.Drawing.Point(4, 24)
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.Size = New System.Drawing.Size(734, 517)
        Me.TabPage4.TabIndex = 3
        Me.TabPage4.Text = "Park Pages"
        Me.TabPage4.UseVisualStyleBackColor = True
        '
        'btnProcess
        '
        Me.btnProcess.Font = New System.Drawing.Font("Segoe UI", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.btnProcess.Location = New System.Drawing.Point(292, 404)
        Me.btnProcess.Name = "btnProcess"
        Me.btnProcess.Size = New System.Drawing.Size(162, 50)
        Me.btnProcess.TabIndex = 4
        Me.btnProcess.Text = "Process"
        Me.btnProcess.UseVisualStyleBackColor = True
        '
        'btnLoad
        '
        Me.btnLoad.Font = New System.Drawing.Font("Segoe UI", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.btnLoad.Location = New System.Drawing.Point(292, 8)
        Me.btnLoad.Name = "btnLoad"
        Me.btnLoad.Size = New System.Drawing.Size(162, 50)
        Me.btnLoad.TabIndex = 3
        Me.btnLoad.Text = "Load"
        Me.btnLoad.UseVisualStyleBackColor = True
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Location = New System.Drawing.Point(8, 486)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(718, 23)
        Me.ProgressBar1.TabIndex = 2
        '
        'txtComplete
        '
        Me.txtComplete.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtComplete.Location = New System.Drawing.Point(460, 8)
        Me.txtComplete.Multiline = True
        Me.txtComplete.Name = "txtComplete"
        Me.txtComplete.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtComplete.Size = New System.Drawing.Size(266, 446)
        Me.txtComplete.TabIndex = 1
        '
        'txtInputList
        '
        Me.txtInputList.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.txtInputList.Location = New System.Drawing.Point(8, 8)
        Me.txtInputList.Multiline = True
        Me.txtInputList.Name = "txtInputList"
        Me.txtInputList.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtInputList.Size = New System.Drawing.Size(278, 446)
        Me.txtInputList.TabIndex = 0
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(742, 545)
        Me.Controls.Add(Me.TabControl1)
        Me.Name = "Form1"
        Me.Text = "Gray DNS Manager"
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        Me.TabPage2.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.TabPage3.ResumeLayout(False)
        Me.TabPage3.PerformLayout()
        Me.TabPage4.ResumeLayout(False)
        Me.TabPage4.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents btnSelectFolder As Button
    Friend WithEvents lblFolderPath As Label
    Friend WithEvents btnImport As Button
    Friend WithEvents txtStatus As TextBox
    Friend WithEvents dlgFolder As FolderBrowserDialog
    Friend WithEvents FolderBrowserDialog1 As FolderBrowserDialog
    Friend WithEvents ckOverwrite As CheckBox
    Friend WithEvents ckAppend As CheckBox
    Friend WithEvents Label1 As Label
    Friend WithEvents ckSkipNSCheck As CheckBox
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents TabPage2 As TabPage
    Friend WithEvents btnDelete As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents txtDNSDeleteList As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents txtDNSDeleteFail As TextBox
    Friend WithEvents txtDNSDeleteSuccess As TextBox
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents progFolder As ProgressBar
    Friend WithEvents progFile As ProgressBar
    Friend WithEvents btnClear As Button
    Friend WithEvents TabPage3 As TabPage
    Friend WithEvents txtGAResults1 As TextBox
    Friend WithEvents btnGoAnywhere As Button
    Friend WithEvents txtGAResults3 As TextBox
    Friend WithEvents txtGAResults2 As TextBox
    Friend WithEvents TabPage4 As TabPage
    Friend WithEvents btnProcess As Button
    Friend WithEvents btnLoad As Button
    Friend WithEvents ProgressBar1 As ProgressBar
    Friend WithEvents txtComplete As TextBox
    Friend WithEvents txtInputList As TextBox
End Class
