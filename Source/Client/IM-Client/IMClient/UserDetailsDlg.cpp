// UserDetailsDlg.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "UserDetailsDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

#define WM_REFRESH  (WM_USER + 101)

/////////////////////////////////////////////////////////////////////////////
// CUserDetailsDlg dialog


CUserDetailsDlg::CUserDetailsDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CUserDetailsDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CUserDetailsDlg)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
	pSession     = NULL;
	Handle       = 0;
	bAutoRefresh = FALSE;
}


void CUserDetailsDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CUserDetailsDlg)
	DDX_Text(pDX, IDC_EMAIL_EDIT, m_Email);
	DDX_Text(pDX, IDC_LAST_EDIT, m_LastName);
	DDX_Text(pDX, IDC_NICK_EDIT, m_NickName);
	DDX_Text(pDX, IDC_ROLE_EDIT, m_Role);
	DDX_Text(pDX, IDC_FIRST_EDIT, m_FirstName);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CUserDetailsDlg, CDialog)
	//{{AFX_MSG_MAP(CUserDetailsDlg)
	ON_WM_CLOSE()
	ON_WM_LBUTTONDOWN()
	ON_MESSAGE(WM_REFRESH,OnRefresh)
    ON_MESSAGE(WM_NLT_CONTAINER_EVENT,OnNetEvent)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CUserDetailsDlg message handlers

BOOL CUserDetailsDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();

    SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),FALSE);		

	pSession = theNet2.GetSession();
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CUserDetailsDlg::OnClose() 
{
	// TODO: Add your message handler code here and/or call default
	CDialog::OnClose();
//	DestroyWindow();
//	delete this;
}

void CUserDetailsDlg::AddInfo(long user_id, LPCTSTR nick_name, LPCTSTR first_name, LPCTSTR last_name, LPCTSTR email, long company_id, LPCTSTR company_name, long role_id, LPCTSTR role_name)
{
	m_NickName  = m_User.m_strLogin       = nick_name;
	m_FirstName = m_User.m_strFirstName   = first_name;
	m_LastName  = m_User.m_strLastName    = last_name;
	m_Email     = m_User.m_strEMail       = email;
	m_Role      = m_User.m_strType        = role_name;

	m_User.GlobalID       = user_id;
	m_User.m_RoleID       = role_id;
///	m_User.m_iStatus      = S_AWAITING;
}

void CUserDetailsDlg::ShowDialog()
{
	ShowWindow(SW_SHOW);
	SetForegroundWindow();
}

void CUserDetailsDlg::OnLButtonDown(UINT nFlags, CPoint point) 
{
	// TODO: Add your message handler code here and/or call default
	
	
	CDialog::OnLButtonDown(nFlags, point);
}

void CUserDetailsDlg::Refresh()
{
	PostMessage(WM_REFRESH);

}

LRESULT CUserDetailsDlg::OnNetEvent(WPARAM w,LPARAM l)
{
	NLT_Container *pItem = (NLT_Container *)w;
	IUser *pUser = NULL;
	ASSERT(pItem!=NULL);

	theNet2.LockTranslator();
	theNet2.RemoveFromTranslator(Handle);
	theNet2.UnlockTranslator();

	HRESULT hr = AutoUnMarchaling(pItem,(LPUNKNOWN*)&pUser);
	
	if(!bAutoRefresh)
	{
		if(SUCCEEDED(hr))
		{
			try
			{
				CUser m_AddInfoUser(pUser);
				m_User = m_AddInfoUser;
				m_NickName  = m_User.m_strLogin;
				m_FirstName = m_User.m_strFirstName;
				m_LastName  = m_User.m_strLastName;
				m_Email     = m_User.m_strEMail;
				m_Role      = m_User.m_strType;				
				UpdateData(FALSE);
				pUser->Release();
			}
			catch(...)
			{
				ASSERT(FALSE);
			}
			
		}
	}
	else
	{
		pUser->Release();
		bAutoRefresh = FALSE;
		SendMessage(WM_REFRESH);
	}
	
	delete pItem;

	return 0;
}

LRESULT CUserDetailsDlg::OnRefresh(WPARAM w,LPARAM l)
{
	if(Handle==0)
	{
		theNet2.LockTranslator();
		try
		{
			Handle = pSession->UserDetails(m_User.GetGlobalID(),1);
			if(Handle)
				theNet2.AddToTranslator(Handle,this->GetSafeHwnd());
		}
		catch(...)
		{
			ASSERT(FALSE);
		}
		theNet2.UnlockTranslator();
	}
	else
		bAutoRefresh = TRUE;
	
	return 0;
}