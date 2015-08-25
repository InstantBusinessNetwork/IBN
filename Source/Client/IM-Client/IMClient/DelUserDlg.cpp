// CDelUserDlg.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "DelUserDlg.h"
#include "MainDlg.h"
#include "GlobalFunction.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDelUserDlg dialog


CDelUserDlg::CDelUserDlg(CMainDlg *pMessenger,CWnd* pParent /*=NULL*/)
	: CDialog(CDelUserDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CDelUserDlg)
	m_Description = _T("");
	//}}AFX_DATA_INIT
	Handle				=	0;
	this->pMessenger	=	pMessenger;
	bIsKillWinodow		=	FALSE;
//	ShowSizeGrip(FALSE);
}


void CDelUserDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDelUserDlg)
	DDX_Text(pDX, IDC_DESCRIPTION_STATIC, m_Description);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CDelUserDlg, CDialog)
	//{{AFX_MSG_MAP(CDelUserDlg)
	ON_BN_CLICKED(IDC_BTN_YES, OnYes)
	ON_BN_CLICKED(IDC_BTN_NO, OnNo)
	//}}AFX_MSG_MAP
	ON_MESSAGE(WM_NLT_CONTAINER_EVENT,OnNetEvent)	
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CDelUserDlg message handlers

void CDelUserDlg::OnOK() 
{
	if(pMessenger->ConnectEnable() && !Handle)
	{
		if(pMessenger->CheckUserInContactList(m_KillUser))
		{
			theNet2.LockTranslator();
			try
			{
				if(m_KillUser.GetStatus()==S_AWAITING)
				{
					Handle = pSession->DeleteUserR(m_KillUser.GetGlobalID());
				}
				else
				{
					Handle = pSession->DeleteUser(m_KillUser.GetGlobalID(),m_ListType);
				}
				if(Handle)
				{
					Block();
					theNet2.AddToTranslator(Handle,GetSafeHwnd());
				}
			}
			catch(...)
			{
			}
			theNet2.UnlockTranslator();
		}
		else
		{
			pMessenger->DeleteFromContact(m_KillUser.GetGlobalID());
			KillWindow();
		}
	}		
}

void CDelUserDlg::OnCancel() 
{
	if(Handle&&pMessenger->ConnectEnable())
	{
		pSession->CancelOperation(Handle);
	}
	else
		KillWindow();
}

LRESULT CDelUserDlg::OnNetEvent(WPARAM w,LPARAM l)
{
	NLT_Container *pItem = (NLT_Container *)w;
	ASSERT(pItem!=NULL);
	
	theNet2.LockTranslator();
	theNet2.RemoveFromTranslator(Handle);
	theNet2.UnlockTranslator();
	
	UnBlock();
	switch(pItem->EventType)
	{
	case NLT_ECommandOK:
		pMessenger->DeleteFromContact(m_KillUser.GetGlobalID());
		KillWindow();
		break;
	case NLT_ECommandError:
		if(pItem->Long1==etSERVER)
		{
			switch(pItem->Long2)
			{
			case ERR_UNABLE_CREATE_CONN:
				_SHOW_IBN_ERROR_DLG_OK(IDS_SERVICENOTAVAILABLE);
				break;
			}
		}
		MessageBox(GetString(IDS_DEL_USER_ERROR),GetString(IDS_ERROR_TITLE),MB_OK|MB_ICONSTOP);
		break;
	}
	
	delete pItem;
    return 0;
}

BOOL CDelUserDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	pSession = theNet2.GetSession();
	ASSERT(pSession!=NULL);
	
	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),FALSE);
	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),TRUE);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

void CDelUserDlg::Block()
{
	GetDlgItem(IDC_BTN_YES)->EnableWindow(FALSE);
}

void CDelUserDlg::UnBlock()
{
	GetDlgItem(IDC_BTN_YES)->EnableWindow(TRUE);
}

void CDelUserDlg::SetKillUser(CUser& user, long ListType)
{
	m_ListType = ListType;
	m_KillUser = user;

	if(m_KillUser.GetStatus()==S_AWAITING)
	{
		m_Description.Format(GetString(IDS_AUTH_CANCEL_FORMAT),m_KillUser.GetShowName());
	}
	else
	{
		if(ListType==1)
			m_Description.Format(GetString(IDS_CONTACTLIST_DEL_FORMAT),m_KillUser.GetShowName());
		else if(ListType==2)
			m_Description.Format(GetString(IDS_IGNORYLIST_DEL_FORMAT),m_KillUser.GetShowName());
	}

	UpdateData(FALSE);
}

void CDelUserDlg::KillWindow()
{
	if(GetStyle()&WS_VISIBLE&&GetOptionInt(IDS_OFSMESSENGER,IDS_ANIMATION,FALSE))
		RoundExitAddon(this);
	CDialog::OnClose();
	if(!bIsKillWinodow)
	{
		bIsKillWinodow = TRUE;
		pMessenger->SendMessage(WM_KILL_DELUSER_MESSAGE_DLG,(WPARAM)m_KillUser.GetGlobalID(),(LPARAM)this);
		DestroyWindow();
		delete this;
	}
}

BEGIN_EVENTSINK_MAP(CDelUserDlg, CDialog)
    //{{AFX_EVENTSINK_MAP(CDelUserDlg)
	//}}AFX_EVENTSINK_MAP
END_EVENTSINK_MAP()

//DEL void CDelUserDlg::OnClickButtonYes() 
//DEL {
//DEL 	OnOK();
//DEL }

//DEL void CDelUserDlg::OnClickButtonNo() 
//DEL {
//DEL 	OnCancel();
//DEL }

//DEL void CDelUserDlg::LoadSkin(IXMLDOMNode *pXmlRoot)
//DEL {
//DEL 	m_btnX.ShowWindow(SW_HIDE);
//DEL 	m_btnYes.ShowWindow(SW_HIDE);
//DEL 	m_btnNo.ShowWindow(SW_HIDE);
//DEL 	LoadButton(pXmlRoot, _T("X"), &m_btnX, TRUE, FALSE);
//DEL 	LoadButton(pXmlRoot, _T("Yes"), &m_btnYes, TRUE, FALSE);
//DEL 	LoadButton(pXmlRoot, _T("No"), &m_btnNo, TRUE, FALSE);
//DEL 
//DEL 	LoadRectangle(pXmlRoot, _T("Description"), &m_description, TRUE);
//DEL }

//DEL void CDelUserDlg::OnClickButtonX() 
//DEL {
//DEL 	OnCancel();
//DEL }

//DEL void CDelUserDlg::OnPaint() 
//DEL {
//DEL 	CPaintDC dc(this); // device context for painting
//DEL 	
//DEL 	CRect r;
//DEL 	DrawBackground(&dc);
//DEL 	if(m_pBranding)
//DEL 	{
//DEL 		m_branding.GetWindowRect(&r);
//DEL 		ScreenToClient(&r);
//DEL 		CDC Dc;
//DEL 		Dc.CreateCompatibleDC(&dc);
//DEL 		Dc.SelectObject(m_pBranding);
//DEL 		dc.BitBlt(r.left, r.top, r.Width(), r.Height(), &Dc, 0, 0, SRCCOPY);
//DEL 	}
//DEL 	
//DEL 	// Do not call CDialog::OnPaint() for painting messages
//DEL }

void CDelUserDlg::OnYes() 
{
	OnOK();
}

void CDelUserDlg::OnNo() 
{
	OnCancel();
}
