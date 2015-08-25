// IBNUpdate.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "IBNUpdate.h"


#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CIBNUpdate dialog


CIBNUpdate::CIBNUpdate(CWnd* pParent /*=NULL*/)
	: CDialog(CIBNUpdate::IDD, pParent)
{
	//{{AFX_DATA_INIT(CIBNUpdate)
	//}}AFX_DATA_INIT
}

CIBNUpdate::~CIBNUpdate()
{
	if(m_McUpdateDownload.IsDownload())
	{
		m_McUpdateDownload.StopDownload(100);
	}
}

void CIBNUpdate::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CIBNUpdate)
	DDX_Control(pDX, IDC_DOWNLOAD_STATIC, m_stDownload);
	DDX_Control(pDX, IDC_START_STATIC, m_stStart);
	DDX_Control(pDX, IDC_ANIMATE, m_Anim);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CIBNUpdate, CDialog)
	//{{AFX_MSG_MAP(CIBNUpdate)
	ON_BN_CLICKED(IDC_START_BUTTON, OnStartButton)
	ON_WM_CLOSE()
	//}}AFX_MSG_MAP
	ON_MESSAGE(WM_MCUPDATE_LOADED,OnMcUpdateLoaded)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CIBNUpdate message handlers

void CIBNUpdate::OnOK() 
{
}

BOOL CIBNUpdate::OnInitDialog() 
{
	CDialog::OnInitDialog();

	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),FALSE);
	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),TRUE);

	m_Anim.Open(IDR_SEND_DATA_AVI);
	
	
	m_McUpdateDownload.Init(GetSafeHwnd(),WM_MCUPDATE_LOADED);

	OnStartButton() ;

	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CIBNUpdate::OnStartButton() 
{
	GetDlgItem(IDC_START_BUTTON)->EnableWindow(FALSE);

	m_stDownload.ShowWindow(SW_HIDE);
	m_stStart.ShowWindow(SW_HIDE);

	m_McUpdateDownload.Load(m_strMcUpdateURL);

	SetWindowText(GetString(IDS_IBN_UPDATE_TITLE));

	m_Anim.Play(0,-1,-1);
}

void CIBNUpdate::OnCancel() 
{
	if(m_McUpdateDownload.IsDownload())
	{
		m_McUpdateDownload.StopDownload(100);
	}
	else
	{
		CDialog::OnCancel();
	}
		
}

void CIBNUpdate::InitBeforCreate(LPCTSTR strMcUpdateURL, LPCTSTR strMcUpdatePath, LPCTSTR strMcUpdateParam)
{
	m_strMcUpdateURL	=	strMcUpdateURL;
	m_strMcUpdatePath	=	strMcUpdatePath;
	m_strMcUpdateParam	=	strMcUpdateParam;
}

LRESULT CIBNUpdate::OnMcUpdateLoaded(WPARAM w, LPARAM l)
{
	m_Anim.Stop();

	if(w)
	{
		m_McUpdateDownload.Clear();
		
		GetDlgItem(IDC_START_BUTTON)->EnableWindow(TRUE);
		SetWindowText(GetString(IDS_IBN_UPDATE_ERROR_TITLE));
	}
	else
	{
		m_stDownload.ShowWindow(SW_SHOW);
		
		
		m_McUpdateDownload.Save(m_strMcUpdatePath+_T("McUpdate.exe"));
		m_McUpdateDownload.Clear();
		
		//long	hInstance	=	(long)ShellExecute(::GetDesktopWindow(),_T("open"),m_strMcUpdatePath,m_strMcUpdateParam,NULL,SW_SHOWDEFAULT);
		
		//if(hInstance<=32)
		
		//MCTRACE(9,"Invoke_StartAutoUpdate Run ShellExecute");
		
		//HINSTANCE hInst = ShellExecute(::GetDesktopWindow(),NULL,m_strMcUpdatePath,m_strMcUpdateParam,NULL,SW_SHOWNORMAL);
		STARTUPINFO startInfo			=	{0};
		startInfo.cb = sizeof(STARTUPINFO);
		
		PROCESS_INFORMATION procInfo	=	{0};
		
		if(CreateProcess(NULL,(LPTSTR)(LPCTSTR)(m_strMcUpdateParam),NULL,NULL,FALSE,0,0,m_strMcUpdatePath,&startInfo,&procInfo))
		{
			CloseHandle(procInfo.hProcess);
			CloseHandle(procInfo.hThread);

			m_stStart.ShowWindow(SW_SHOW);
			OnCancel();
		}
		else
		{
			//MCTRACE(9,"Invoke_StartAutoUpdate ShellExecute Error = %08X",GetLastError());

			GetDlgItem(IDC_START_BUTTON)->EnableWindow(TRUE);
			SetWindowText(GetString(IDS_IBN_UPDATE_ERROR2_TITLE));
			
			LPVOID lpMsgBuf;
			FormatMessage( 
				FORMAT_MESSAGE_ALLOCATE_BUFFER | 
				FORMAT_MESSAGE_FROM_SYSTEM | 
				FORMAT_MESSAGE_IGNORE_INSERTS,
				NULL,
				GetLastError(),
				MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), // Default language
				(LPTSTR) &lpMsgBuf,
				0,
				NULL 
				);
			
			MessageBox((LPCTSTR)lpMsgBuf,GetString(IDS_OPEN_FILE_ERROR_NAME),MB_OK | MB_ICONINFORMATION );
			
			// Free the buffer.
			LocalFree( lpMsgBuf );
		}
	}
	
	return 0;
}

void CIBNUpdate::OnClose() 
{
	DestroyWindow();
	delete this;
}
