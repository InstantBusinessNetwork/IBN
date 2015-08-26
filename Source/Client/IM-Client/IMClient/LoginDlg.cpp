// LoginDlg.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "LoginDlg.h"
#include "cdib.h"
#include "LoadSkins.h"
#include "GlobalFunction.h"
#include "DlgNetOptions2.h"

#include  "DlgPreferences.h"

//#include "DlgTV.h"
//#include "WebWindow.h"
#include "MainDlg.h"
#include "ChatDlg.h"
#include "ChatCreateDlg.h"

#include "IBNWFGlobalFunctions.h"

#include "ScreenShotDlg.h"


#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

extern CString GetCurrentSkin();

extern CString strCommandLineLogin;
extern CString strCommandLinePassword;

/////////////////////////////////////////////////////////////////////////////
// CLoginDlg dialog

CLoginDlg::CLoginDlg(CWnd* pParent /*=NULL*/)
: COFSNcDlg2(CLoginDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CLoginDlg)
	m_LoginStr = _T("");
	m_PasswordStr = _T("");
	//}}AFX_DATA_INIT
	m_Block = FALSE;
	m_strSkinSettings = _T("/Shell/Login/skin.xml");
	m_bResizable = FALSE;
}

CLoginDlg::~CLoginDlg()
{
	m_pAutoComplete = NULL;
}


void CLoginDlg::DoDataExchange(CDataExchange* pDX)
{
	COFSNcDlg2::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CLoginDlg)
	DDX_Control(pDX, IDC_TITLE, m_title);
	DDX_Control(pDX, IDC_EDIT_LOGIN, m_LoginEdit);
	DDX_Control(pDX, IDC_EDIT_PASSWORD, m_PasswordEdit);
	DDX_Control(pDX, IDC_BUTTON_CANCEL, m_btnCancel);
	DDX_Control(pDX, IDC_BUTTON_X, m_btnX);
	DDX_Control(pDX, IDC_BUTTON_LOGIN, m_btnLogin);
	DDX_Text(pDX, IDC_EDIT_LOGIN, m_LoginStr);
	DDX_Text(pDX, IDC_EDIT_PASSWORD, m_PasswordStr);
	DDX_Control(pDX, IDC_BUTTON_REMEMBER, m_btnSavePassword);
	DDX_Control(pDX, IDC_BUTTON_NETOPTIONS, m_btnNetOption);
	DDX_Control(pDX, IDC_BUTTON_SSL, m_btnSSL);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CLoginDlg, COFSNcDlg2)
	//{{AFX_MSG_MAP(CLoginDlg)
	ON_WM_CLOSE()
	ON_WM_LBUTTONDOWN()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

BEGIN_EVENTSINK_MAP(CLoginDlg, COFSNcDlg2)
	//{{AFX_EVENTSINK_MAP(CLoginDlg)
	ON_EVENT(CLoginDlg, IDC_BUTTON_X, -600 /* Click */, OnClickButtonX, VTS_NONE)
	ON_EVENT(CLoginDlg, IDC_BUTTON_CANCEL, -600 /* Click */, OnClickButtonCancel, VTS_NONE)
	ON_EVENT(CLoginDlg, IDC_BUTTON_NETOPTIONS, -600 /* Click */, OnClickButtonNetoption, VTS_NONE)
	ON_EVENT(CLoginDlg, IDC_BUTTON_LOGIN, -600 /* Click */, OnClickButtonLogin, VTS_NONE)
	//}}AFX_EVENTSINK_MAP
END_EVENTSINK_MAP()

/////////////////////////////////////////////////////////////////////////////
// CLoginDlg message handlers


BOOL CLoginDlg::PreCreateWindow(CREATESTRUCT& cs) 
{
	// TODO: Add your specialized code here and/or call the base class
	cs.style &= ~WS_VISIBLE;
	return COFSNcDlg2::PreCreateWindow(cs);
}

void CLoginDlg::LoadSkin(IXMLDOMNode *pXmlRoot)
{
	m_btnCancel.ShowWindow(SW_HIDE);
	m_btnX.ShowWindow(SW_HIDE);
	m_btnSavePassword.ShowWindow(SW_HIDE);
	m_btnNetOption.ShowWindow(SW_HIDE);
	m_btnLogin.ShowWindow(SW_HIDE);
	m_btnSSL.ShowWindow(SW_HIDE);

	LoadLabel(pXmlRoot, _T("Title"), &m_title, TRUE);
	LoadButton(pXmlRoot, _T("X"), &m_btnX, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Login"), &m_btnLogin, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Cancel"), &m_btnCancel, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("SavePassword"), &m_btnSavePassword, TRUE, TRUE);
	LoadButton(pXmlRoot, _T("ConnectionSettings"), &m_btnNetOption, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("SSL"), &m_btnSSL, TRUE, TRUE);

	LoadRectangle(pXmlRoot, _T("EditLogin"), &m_LoginEdit, TRUE);
	LoadRectangle(pXmlRoot, _T("EditPassword"), &m_PasswordEdit, TRUE);
}

BOOL CLoginDlg::OnInitDialog() 
{
	COFSNcDlg2::OnInitDialog();
	
	//////////////////////////////////////////////////////////////////////////
	// McToolTipAddon
	m_ToolTip.AddTool(&m_btnX,IDS_TIP_CLOSE);
	//////////////////////////////////////////////////////////////////////////

	m_title.SetText(GetString(IDS_IBN_LOGIN_TITLE));
	SetWindowText(GetString(IDS_IBN_LOGIN_TITLE));
	
	//////////////////////////////////////////////////////////////////////////

	BOOL bRememberLogin = GetOptionInt(IDS_LOGIN,IDS_REMEMBER,TRUE);

#ifndef RADIUS
	#define		CRYPT_PROV_CONTAINER_NAME	_T("Mediachase")
#else
	#define		CRYPT_PROV_CONTAINER_NAME	_T("Radius-Soft")
#endif

#define		CRYPT_KEYLENGTH				0x00280000L
	
#define		ENCRYPT_ALGORITHM			CALG_RC4
#define		ENCRYPT_BLOCK_SIZE			1
	
	if(strCommandLineLogin.GetLength()==0)
	{
		if(bRememberLogin)
		{
			CString strHashData	   = GetOptionString(IDS_LOGIN, IDS_NICKNAME, _T(""));
			
			CCryptProv				m_hCryptProv;
			CCryptDerivedKey		m_hKey;
			
			HRESULT m_CryptInitErrorCode = m_hCryptProv.Initialize(PROV_RSA_FULL,CRYPT_PROV_CONTAINER_NAME,MS_DEF_PROV,NULL);
			if(m_CryptInitErrorCode==0x80090016)
			{
				m_CryptInitErrorCode = m_hCryptProv.Initialize(PROV_RSA_FULL,CRYPT_PROV_CONTAINER_NAME,MS_DEF_PROV,CRYPT_NEWKEYSET);
			}
			
			if(m_CryptInitErrorCode==S_OK)
			{
				// Create Key [9/12/2002]
				CCryptMD5Hash hMD5Hash;
				
				m_CryptInitErrorCode = hMD5Hash.Initialize(m_hCryptProv,CRYPT_PROV_CONTAINER_NAME);
				if(m_CryptInitErrorCode==S_OK)
				{
					m_CryptInitErrorCode = m_hKey.Initialize(m_hCryptProv,hMD5Hash,ENCRYPT_ALGORITHM,CRYPT_KEYLENGTH);
					
					if(m_CryptInitErrorCode==S_OK)
					{
						if(!ExtractLoginAndPasswordFromData(m_hKey,strHashData,m_LoginStr,m_PasswordStr))
						{
							m_LoginStr.Empty();
							m_PasswordStr.Empty();
						}
					}
				}
			}
		}
	}
	else
	{
		m_LoginStr = strCommandLineLogin;

		if(strCommandLinePassword.GetLength()!=0)
			m_PasswordStr = strCommandLinePassword;
	}
	
	//////////////////////////////////////////////////////////////////////////
				
	m_btnSSL.SetPressed(GetOptionInt(IDS_NETOPTIONS, IDS_USESSL, FALSE));
	
	m_btnSavePassword.SetPressed(bRememberLogin);
	m_btnCancel.EnableWindow(FALSE);
	
	UpdateData(FALSE);
	
	m_LoginEdit.SetFocus();
	
	// Add Auto complete Mode [8/15/2003]
	// Step 1. Create CLSID_AutoComplete
	HRESULT hr = CoCreateInstance(CLSID_AutoComplete, NULL, CLSCTX_INPROC_SERVER,IID_IAutoComplete, (LPVOID*)&m_pAutoComplete);
	// Step 2. Create Custom autocomplete source

	if(SUCCEEDED(hr))
		hr = m_pAutoComplete->Init(m_LoginEdit.GetSafeHwnd(),(LPUNKNOWN)m_AutoCompleteSource.GetInterface(&IID_IUnknown),NULL,NULL);

	
	return TRUE;  // return TRUE unless you set the focus to a control
              // EXCEPTION: OCX Property Pages should return FALSE
}

void CLoginDlg::OnClickButtonX() 
{
	OnClose();
}

void CLoginDlg::OnClickButtonCancel() 
{
	//Cancel Operation ...
	if(IsBlock())
	{
		if(::IsWindow(GetParent()->GetSafeHwnd()))
			GetParent()->PostMessage(WM_CANCELLOGIN,0,0);
	}
}

void CLoginDlg::ShowLoginTooltip(BOOL bShow)
{
	static HWND			_hwndToolTip	=	NULL;
	static TOOLINFO		_ti			=	{0};
	static CString		_tooltipBody	=	GetString(IDS_LOGIN_TOOLTIP_BODY);	

	if(!IsWindow(_hwndToolTip))
	{
		_hwndToolTip = CreateWindow(TOOLTIPS_CLASS,
			NULL,
			WS_POPUP | TTS_NOPREFIX | TTS_BALLOON | TTS_ALWAYSTIP ,
			CW_USEDEFAULT, CW_USEDEFAULT,
			CW_USEDEFAULT, CW_USEDEFAULT,
			m_LoginEdit.GetSafeHwnd(), NULL,
			AfxGetInstanceHandle(),
			NULL);

		ASSERT(_hwndToolTip!=NULL);

		// Do the standard ToolTip coding. 
		_ti.cbSize = sizeof(_ti);
		_ti.uFlags = TTF_IDISHWND |  TTF_TRANSPARENT | TTF_CENTERTIP;
		_ti.hwnd = m_LoginEdit.GetSafeHwnd();
		_ti.uId = (UINT)_ti.hwnd;
		_ti.hinst = AfxGetInstanceHandle();
		_ti.lpszText = (LPTSTR)(LPCTSTR)_tooltipBody;

		m_LoginEdit.GetClientRect(&_ti.rect);
		BOOL bRetVal = ::SendMessage(_hwndToolTip, TTM_ADDTOOL, 0, (LPARAM) &_ti );
		ASSERT(bRetVal);
		bRetVal = ::SendMessage(_hwndToolTip, TTM_SETTITLE, (WPARAM)1, (LPARAM)(LPCTSTR)GetString(IDS_LOGIN_TOOLTIP_CAPTION));
		ASSERT(bRetVal);
	}
		
	BOOL bRetVal = ::SendMessage(_hwndToolTip, TTM_TRACKACTIVATE, bShow, (LPARAM)&_ti);
	ASSERT(bRetVal);
}

void CLoginDlg::OnLButtonDown(UINT nFlags, CPoint point) 
{
	if(GetDllVersion(_T("comctl32.dll")) >= PACKVERSION(5,8))
	{
		ShowLoginTooltip(FALSE);
	}
	
	COFSNcDlg2::OnLButtonDown(nFlags,point);
}

void CLoginDlg::OnClickButtonLogin() 
{
	UpdateData();

	m_LoginStr.TrimLeft();
	m_LoginStr.TrimRight();

	CString LoginStr = m_LoginStr;

	int StartPortPos = -1;
	if((StartPortPos = LoginStr.Find(_T(":")))!=-1)
	{
		CString strPort = LoginStr.Mid(StartPortPos+1);

		LoginStr = LoginStr.Left(StartPortPos);

		int lPort = _ttol(strPort);
	}


	// Check: Login is E-Mail [9/2/2002]
	if(CheckEmailString(LoginStr))
	{
		if(GetDllVersion(_T("comctl32.dll")) >= PACKVERSION(5,8))
		{
			ShowLoginTooltip(FALSE);
		}

		WriteOptionInt(IDS_NETOPTIONS, IDS_USESSL, m_btnSSL.GetPressed());
		
		if(!m_btnSavePassword.GetPressed())
		{
			WriteOptionString(IDS_LOGIN, IDS_NICKNAME, _T(""));
			//WriteOptionString(IDS_LOGIN,IDS_PASSWORD,"");
			WriteOptionInt(IDS_LOGIN,IDS_REMEMBER,FALSE);
		}
		else
		{
#ifndef RADIUS
	#define		CRYPT_PROV_CONTAINER_NAME	_T("Mediachase")
#else
	#define		CRYPT_PROV_CONTAINER_NAME	_T("Radius-Soft")
#endif

#define		CRYPT_KEYLENGTH				0x00280000L
			
#define		ENCRYPT_ALGORITHM			CALG_RC4
#define		ENCRYPT_BLOCK_SIZE			1
			
			//CString strHashData;
			
			CCryptProv				m_hCryptProv;
			CCryptDerivedKey		m_hKey;
			
			HRESULT m_CryptInitErrorCode = m_hCryptProv.Initialize(PROV_RSA_FULL,CRYPT_PROV_CONTAINER_NAME,MS_DEF_PROV,NULL);
			if(m_CryptInitErrorCode==0x80090016)
			{
				m_CryptInitErrorCode = m_hCryptProv.Initialize(PROV_RSA_FULL,CRYPT_PROV_CONTAINER_NAME,MS_DEF_PROV,CRYPT_NEWKEYSET);
			}
			
			if(m_CryptInitErrorCode==S_OK)
			{
				// Create Key [9/12/2002]
				CCryptMD5Hash hMD5Hash;
				
				m_CryptInitErrorCode = hMD5Hash.Initialize(m_hCryptProv,CRYPT_PROV_CONTAINER_NAME);
				if(m_CryptInitErrorCode==S_OK)
				{
					m_CryptInitErrorCode = m_hKey.Initialize(m_hCryptProv,hMD5Hash,ENCRYPT_ALGORITHM,CRYPT_KEYLENGTH);
					
					if(m_CryptInitErrorCode==S_OK)
					{
						LPTSTR strHashData	=	NULL;
						if(LoginPassword2HexSTR(m_hKey,m_LoginStr,m_PasswordStr,&strHashData)==S_OK)
						{
							WriteOptionString(IDS_LOGIN,IDS_NICKNAME,strHashData);

							delete [] strHashData;
							strHashData = NULL;
						}
					}
				}
			}

			//WriteOptionString(IDS_LOGIN,IDS_NICKNAME,m_LoginStr);
			//Pack(m_PasswordStr,CString("vTsfO"));
			//WriteOptionString(IDS_LOGIN,IDS_PASSWORD,m_PasswordStr);
			//UnPack(m_PasswordStr,CString("vTsfO"));
			WriteOptionInt(IDS_LOGIN,IDS_REMEMBER,TRUE);
		}
		
		
		if(::IsWindow(GetParent()->GetSafeHwnd()))
			GetParent()->PostMessage(WM_INETLOGIN,0,0);
	}
	else
	{
		// Error; Onvalide Login [9/2/2002]

		// Show Ballon Tooltip [9/9/2004]
		if(GetDllVersion(_T("comctl32.dll")) >= PACKVERSION(5,8))
		{
			ShowLoginTooltip(TRUE);
		}
		else
		{
			_SHOW_IBN_ERROR_DLG_OK(IDS_INVALID_LOGIN_OR_PASSWORD);
		}
	}
	//  [9/2/2002]
}

void CLoginDlg::OnClose() 
{
	if(!IsBlock())
	{
		if(GetStyle()&WS_VISIBLE&&GetOptionInt(IDS_OFSMESSENGER,IDS_ANIMATION,FALSE))
			RoundExitAddon(this);
		// TODO: Add your message handler code here and/or call default
		if(IsWindow(GetParent()->GetSafeHwnd()))
			GetParent()->PostMessage(WM_CLOSE);
		COFSNcDlg2::OnClose();
		
	}
}

void CLoginDlg::OnOK() 
{
	if(!IsBlock())
		OnClickButtonLogin();
}

void CLoginDlg::OnCancel() 
{
}

void CLoginDlg::OnClickButtonNetoption() 
{
//#ifdef _DEBUG
//	CWebWindow *pNewWindow = new CWebWindow;
//	pNewWindow->CreateAutoKiller(_T("/Browser/Common/skin.xml"), this, GetDesktopWindow(), 100, 100, 500, 300, NULL, _T("d:\\humor\\a.htm"), FALSE, FALSE, TRUE, 0, TRUE);
//	CMainDlg dlg;
//	CChatDlg *pdlg = new CChatDlg((CMainDlg*)AfxGetMainWnd(), GetDesktopWindow());
//	pdlg->Create(GetDesktopWindow());
//	pdlg->SetWindowPos(NULL,0,0,300,200,SWP_NOZORDER|SWP_SHOWWINDOW);
//	CChatCreateDlg dlg((CMainDlg*)AfxGetMainWnd());
//	dlg.DoModal();
//#else
	WriteOptionInt(IDS_NETOPTIONS, IDS_USESSL, m_btnSSL.GetPressed());
	
	CDlgNetOptions2 dlg;
	dlg.DoModal();

	m_btnSSL.SetPressed(GetOptionInt(IDS_NETOPTIONS, IDS_USESSL, FALSE));
//#endif
}

void CLoginDlg::Block()
{
	m_Block = TRUE;

	m_btnSavePassword.EnableWindow(FALSE);
	m_btnNetOption.EnableWindow(FALSE);
	m_btnLogin.EnableWindow(FALSE);
	m_LoginEdit.EnableWindow(FALSE);
	m_PasswordEdit.EnableWindow(FALSE);
	m_btnCancel.EnableWindow(FALSE);
	m_btnCancel.EnableWindow(TRUE);
	m_btnSSL.EnableWindow(FALSE);
}

void CLoginDlg::UnBlock()
{
	m_Block = FALSE;

	m_btnSavePassword.EnableWindow(TRUE);
	m_btnNetOption.EnableWindow(TRUE);
	m_btnLogin.EnableWindow(TRUE);
	m_LoginEdit.EnableWindow(TRUE);
	m_PasswordEdit.EnableWindow(TRUE);
	m_btnCancel.EnableWindow(FALSE);
	m_btnSSL.EnableWindow(TRUE);
}

BOOL CLoginDlg::IsBlock()
{
	return m_Block;
}
