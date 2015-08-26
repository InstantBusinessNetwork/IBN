Sub SaveSelectedItem()
    Set ses = CreateObject("McConverterSessionServer.ConvertSession")
    Dim myFileName As String
    Dim myOlApp As New Outlook.Application
    Dim myOlExp As Outlook.Explorer
    Dim myOlSel As Outlook.Selection
    Set myOlExp = myOlApp.ActiveExplorer
    Set myOlSel = myOlExp.Selection
    myFileName = "c:\Success.txt"
    For x = 1 To myOlSel.Count
          ss = ses.SaveMapiMessage(myFileName, myOlSel.Item(x), 1)
    Next x
    

End Sub

Sub SaveSelectedItemTmp()
    Set ses = CreateObject("McConverterSessionServer.ConvertSession")
    Dim myFileName As String
    Dim myOlApp As New Outlook.Application
    Dim myOlExp As Outlook.Explorer
    Dim myOlSel As Outlook.Selection
    Dim myTmpFileName As String
    Set myOlExp = myOlApp.ActiveExplorer
    Set myOlSel = myOlExp.Selection
    For x = 1 To myOlSel.Count
          tmpFileName = ses.SaveMapiMessageTmpFile(myOlSel.Item(x), 1)
	  'Do some
    Next x

End Sub