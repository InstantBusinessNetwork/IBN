// OfsTv.cpp : Defines the class behaviors for the application.
//

#include "stdafx.h"
#include "OfsTv.h"

#include "MainDlg.h"
#include "Label.h"
#include "LoadSkins.h"
#include "McVersionInfo.h"
#include "McSettings.h"
#include "DlgAbout.h"
#include "GlobalFunction.h"

#ifdef _DEVELOVER_VERSION_WITH_DUMP_L1
#include "MiniDump/MiniDump.h"
#endif


#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

CRITICAL_SECTION CritSect;

OSVERSIONINFO	_VersionInfo;

CString strCommandLineLogin = _T("");
CString strCommandLinePassword = _T("");

/////////////////////////////////////////////////////////////////////////////
// COfsTvApp

BEGIN_MESSAGE_MAP(COfsTvApp, CWinApp)
//{{AFX_MSG_MAP(COfsTvApp)
ON_COMMAND(ID_APP_ABOUT, OnAppAbout)
// NOTE - the ClassWizard will add and remove mapping macros here.
//    DO NOT EDIT what you see in these blocks of generated code!
//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// COfsTvApp construction

COfsTvApp::COfsTvApp()
{
	// TODO: add construction code here,
	// Place all significant initialization in InitInstance
	m_pUpdateWindow = NULL;
}

/////////////////////////////////////////////////////////////////////////////
// The one and only COfsTvApp object

COfsTvApp  theApp;
CCoolInet2 theNet2;
CMcSettings g_settings;
//LoadSkins  m_LoadSkin;

CString    m_CurrentSkin;

CString GetCurrentSkin()
{
	return m_CurrentSkin;
}

void SetCurrentSkin(CString strName)
{
	m_CurrentSkin = strName;
}

/////////////////////////////////////////////////////////////////////////////
// COfsTvApp initialization
#ifdef _DEVELOVER_VERSION_WITH_DUMP_L1

LONG WINAPI MyUnFilter (struct _EXCEPTION_POINTERS *lpExceptionInfo)
{
	// Add reindex Id to the Reg [6/14/2002]
	WriteOptionInt(IDS_HISTORY,IDS_REINDEX,1);
	// After restart Enable ReIndex [6/14/2002]
	
	CString	strModulePath, strDirPath, strMcDumpUpPath, strDumpFilePath;
	LPTSTR Buf = strModulePath.GetBuffer(1000);
	GetModuleFileName(NULL,Buf,1000);
	strModulePath.ReleaseBuffer();
	
	strDirPath = strModulePath.Left(strModulePath.ReverseFind('\\'));
	strMcDumpUpPath.Format(_T("%s\\McDumpUp.exe"),strDirPath);
	strDumpFilePath.Format(_T("%s\\IBN.exe.dmp"),strDirPath);

	//MessageBox(NULL,strDumpFilePath,0,0);
	
	BSUMDRET eCCPMD ;
    eCCPMD = CreateCurrentProcessMiniDump (MiniDumpWithHandleData,
		(LPTSTR)(LPCTSTR)strDumpFilePath,
		GetCurrentThreadId(),
		lpExceptionInfo              ) ;
    ASSERT ( eDUMP_SUCCEEDED == eCCPMD ) ;
	
	if(eDUMP_SUCCEEDED == eCCPMD)
	{
		
		STARTUPINFO startInfo	=	{0};
		startInfo.cb = sizeof(STARTUPINFO);
		
		PROCESS_INFORMATION procInfo	=	{0};
		
		CMcVersionInfo	VerInfo;

		VerInfo.GetFileVersionString();

		
		CString strParam;
		strParam.Format(_T("\"%s\" \"IBN Client\" \"%s\" \"http://dump.mediachase.com/errordump.aspx\" \"%s\" \"%s\""),strMcDumpUpPath,VerInfo.GetFileVersionString(),strDumpFilePath,strModulePath);
		
		//MessageBox(NULL,strParam,strMcDumpUpPath,0);

		if(CreateProcess(strMcDumpUpPath,(LPTSTR)(LPCTSTR)strParam,NULL,NULL,FALSE,0,NULL,strDirPath,&startInfo,&procInfo))
		{
			CloseHandle(procInfo.hProcess);
			CloseHandle(procInfo.hThread);
		}
	}
	
    return ( EXCEPTION_EXECUTE_HANDLER ) ;
} 

#endif

UINT g_IbnToMessage = 0;

BOOL COfsTvApp::InitInstanceInternal()
{
	CString strCmdLineParam;
	CString str;

	_VersionInfo.dwOSVersionInfoSize = sizeof(OSVERSIONINFO);
	GetVersionEx(&_VersionInfo);

	// Try to load localization DLL [6/29/2002]

	CString strIbnLangDllName = _T("imclientlang");
	strIbnLangDllName += GetProductLanguage();
	strIbnLangDllName += _T(".dll");

	HINSTANCE	hLocalizationInst = LoadLibrary(strIbnLangDllName);
	if(hLocalizationInst)
		AfxSetResourceHandle(hLocalizationInst);


	#ifdef _DEVELOVER_VERSION_WITH_DUMP_L1
		SetUnhandledExceptionFilter(MyUnFilter);
	#endif

	InitCommonControls();

	::CreateMutex(NULL, FALSE, MC_AUTOUPDATE_SYNC_OBJECT_NAME); // Used for autoupdate

	AfxEnableControlContainer();

	InitializeCriticalSection(&CritSect);
	TCHAR buf[100];
	_ltot((long)&CritSect, buf, 10);
	SetEnvironmentVariable(_T("MpaCritSect"), buf);


	// Get user name And password from command line [2007-02-06]

	strCmdLineParam = GetCommandLine();

	int iCmdLineLoginPos = strCmdLineParam.Find(_T("-l:"));
	if(iCmdLineLoginPos != -1)
	{
		int iCmdLineLoginEnd = strCmdLineParam.Find(_T(" "), iCmdLineLoginPos);

		if(iCmdLineLoginEnd == -1)
			iCmdLineLoginEnd = strCmdLineParam.GetLength();

		strCommandLineLogin = strCmdLineParam.Mid(iCmdLineLoginPos+3, iCmdLineLoginEnd - iCmdLineLoginPos - 3);
	}

	int iCmdLinePassPos = strCmdLineParam.Find(_T("-p:"));
	if(iCmdLinePassPos != -1)
	{
		int iCmdLinePassEnd = strCmdLineParam.Find(_T(" "), iCmdLinePassPos);

		if(iCmdLinePassEnd == -1)
			iCmdLinePassEnd = strCmdLineParam.GetLength();

		strCommandLinePassword = strCmdLineParam.Mid(iCmdLinePassPos+3, iCmdLinePassEnd - iCmdLinePassPos - 3);
	}

	// TODO: ???
	SetEnvironmentVariable(_T("MpaUserLogin"), _T("Login"));
	SetEnvironmentVariable(_T("MpaUserPassword"), _T("Password"));


	// Check if skins folder is set for current user

#ifndef RADIUS
	LPCTSTR szKey1 = _T("Software\\Mediachase\\Instant Business Network\\4.5\\Client\\Skins");
#else
	LPCTSTR szKey1 = _T("Software\\Radius-Soft\\MagRul\\4.5\\Client\\Skins");
#endif

	// LPCTSTR szKey2 = _T("Software\\Online Film Sales\\OFS MarketPlace Assistant\\Skins");
	LPCTSTR szEntry = _T("Skins Folder");

	McRegGetString(HKEY_LOCAL_MACHINE, szKey1, szEntry, str);
	// TODO: Remember about bug when use many skins. [4/19/2002]
	if(str.GetLength() == 0)
	{
		//McRegGetString(HKEY_LOCAL_MACHINE, szKey1, szEntry, str);
		McRegWriteString(HKEY_CURRENT_USER, szKey1, szEntry, str);
	}

	//	McRegGetString(HKEY_CURRENT_USER, szKey2, szEntry, str);
	//	if(str.GetLength() == 0)
	//	{
	//		McRegGetString(HKEY_LOCAL_MACHINE, szKey1, szEntry, str);
	//		McRegWriteString(HKEY_CURRENT_USER, szKey2, szEntry, str);
	//	}


	// Standard initialization
	// If you are not using these features and wish to reduce the size
	//  of your final executable, you should remove from the following
	//  the specific initialization routines you do not need.

#ifdef _AFXDLL
	Enable3dControls();			// Call this when using MFC in a shared DLL
#else
	Enable3dControlsStatic();	// Call this when linking to MFC statically
#endif


	// Change the registry key under which our settings are stored.
	// TODO: You should modify this string to be something appropriate
	// such as the name of your company or organization.
	SetRegistryKey(IDS_REGISTRY_KEY);

	// Addon for Client Server Versions [3/15/2004]
	free((void*)m_pszProfileName);

#ifndef RADIUS
	m_pszProfileName = _tcsdup(_T("Instant Business Network\\4.5\\Client"));
#else
	m_pszProfileName = _tcsdup(_T("MagRul\\4.5\\Client"));
#endif

	//  [3/15/2004]

	str.Format(_T("Software\\%s\\%s\\4.5\\Client"), GetString(IDS_REGISTRY_KEY), GetString(AFX_IDS_APP_TITLE));
	g_settings.SetRoot(str);

	// If this is a first launch, create "Send To" link.
	DWORD dw;
	g_settings.GetDWORD(_T(""), _T("FirstLaunch"), dw, 1);
	if(dw == 1)
	{
		DeleteSendToLink();
		CreateSendToLink();
		g_settings.SetDWORD(_T(""), _T("FirstLaunch"), 0);
	}

	g_IbnToMessage = RegisterWindowMessage(_T("IBNToMessage"));

	m_pIbnToHandlerWindow = new CIBNToHandlerWindow();

	// Check ibnto: command
	IBNTO_MESSAGE *pIbnToMessage = m_pIbnToHandlerWindow->GetIbnToCommand();
	if(pIbnToMessage != NULL && m_pIbnToHandlerWindow->SendIbnToCommand())
	{
		Sleep(1000);
		delete pIbnToMessage;
		return FALSE;
	}

	// Check files: command
	SENDTO_MESSAGE *pSendToMessage = m_pIbnToHandlerWindow->GetFilesCommand();
	if(pSendToMessage != NULL && m_pIbnToHandlerWindow->SendFilesCommand())
	{
		delete pSendToMessage;
		return FALSE;
	}


	// To create the main window, this code creates a new frame window
	// object and then sets it as the application's main window object.

	CMainDlg* pFrame = new CMainDlg;
	m_pMainWnd = pFrame;
	
	// create and load the frame with its resources
	pFrame->hIcon = LoadIcon(IDR_MAINFRAME);
	BOOL bWork = pFrame->Create(CWnd::GetDesktopWindow());


	// Windows 98 fix [10/15/2003]
	m_pUpdateWindow = new CMcUpdateWindow();
	m_pUpdateWindow->SetMainHWND(pFrame->GetSafeHwnd());
	m_pUpdateWindow->Create();

	m_pIbnToHandlerWindow->SetMainHWND(pFrame->GetSafeHwnd());
	m_pIbnToHandlerWindow->Create();
	// Windows 98 fix end

	pFrame->AddIbnToMessage(pIbnToMessage);
	pFrame->AddSendToMessage(pSendToMessage);


	// Check for silent mode [10/17/2002]
	strCmdLineParam = GetCommandLine();
	strCmdLineParam.MakeLower();
	if(strCmdLineParam.Find(_T("-silent")) != -1)
		pFrame->m_bSilentMode = TRUE;


	// The one and only window has been initialized, so show and update it.
	pFrame->ShowWindow(SW_HIDE);
	pFrame->UpdateWindow();

	return TRUE;
}

BOOL COfsTvApp::InitInstance()
{
	BOOL RetVal = FALSE;
	
#ifdef _DEVELOVER_VERSION_WITH_DUMP_L1
	__try
	{
#endif
		RetVal = InitInstanceInternal();
#ifdef _DEVELOVER_VERSION_WITH_DUMP_L1
	}
	__except( MyUnFilter(GetExceptionInformation ( )))
	{
		RetVal = FALSE;
	}
#endif
	
	return RetVal;
}

/////////////////////////////////////////////////////////////////////////////
// COfsTvApp message handlers





/////////////////////////////////////////////////////////////////////////////
// CAboutDlg dialog used for App About


// App command to run the dialog
void COfsTvApp::OnAppAbout()
{
	static bool	bShowAboutDialog = false;
	
	if(bShowAboutDialog)
	{
		m_pMainWnd->SetForegroundWindow();
		return;
	}
	
	CDlgAbout aboutDlg;
	bShowAboutDialog = true;
	aboutDlg.DoModal();
	bShowAboutDialog = false;
}

/////////////////////////////////////////////////////////////////////////////
// COfsTvApp message handlers

int COfsTvApp::ExitInstance() 
{
	int RetVal = 0;
	
#ifdef _DEVELOVER_VERSION_WITH_DUMP_L1
	__try
	{
#endif
		
		DeleteCriticalSection(&CritSect);
		
		if(m_pUpdateWindow)
		{
			m_pUpdateWindow->SendMessage(WM_CLOSE);
			delete m_pUpdateWindow;
			m_pUpdateWindow = NULL;
		}
		
		if(m_pIbnToHandlerWindow != NULL)
		{
			if(IsWindow(m_pUpdateWindow->GetSafeHwnd()))
				m_pIbnToHandlerWindow->SendMessage(WM_CLOSE);
			delete m_pIbnToHandlerWindow;
			m_pIbnToHandlerWindow = NULL;
		}
		
		RetVal = CWinApp::ExitInstance();
#ifdef _DEVELOVER_VERSION_WITH_DUMP_L1
	}
	__except( MyUnFilter(GetExceptionInformation ( )))
	{
		RetVal = 201;
	}
#endif
	
	return RetVal;
	
}

int COfsTvApp::Run()
{
	int RetVal = 0;
	
#ifdef _DEVELOVER_VERSION_WITH_DUMP_L1
	__try
	{
#endif
		RetVal = CWinApp::Run();
#ifdef _DEVELOVER_VERSION_WITH_DUMP_L1
	}
	__except( MyUnFilter(GetExceptionInformation ( )))
	{
		RetVal = 202;
	}
#endif
	
	return RetVal;

}

