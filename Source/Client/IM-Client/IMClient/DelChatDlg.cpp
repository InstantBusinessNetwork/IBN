// CDelChatDlg.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "DelChatDlg.h"
#include "MainDlg.h"
#include "GlobalFunction.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDelChatDlg dialog


CDelChatDlg::CDelChatDlg(CMainDlg *pMessenger,CWnd* pParent /*=NULL*/)
	: CDialog(CDelChatDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CDelChatDlg)
	m_Description = _T("");
	//}}AFX_DATA_INIT
	Handle				=	0;
	this->pMessenger	=	pMessenger;
	bIsKillWinodow		=	FALSE;
//	ShowSizeGrip(FALSE);
}


void CDelChatDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDelChatDlg)
	DDX_Text(pDX, IDC_DESCRIPTION_STATIC, m_Description);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CDelChatDlg, CDialog)
//{{AFX_MSG_MAP(CDelChatDlg)
ON_BN_CLICKED(IDC_BTN_YES, OnYes)
ON_BN_CLICKED(IDC_BTN_NO, OnNo)
//}}AFX_MSG_MAP
ON_MESSAGE(WM_NLT_CONTAINER_EVENT,OnNetEvent)	
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CDelChatDlg message handlers

void CDelChatDlg::OnOK() 
{
	if(pMessenger->ConnectEnable() && !Handle)
	{
		CChat Chat;

		if(pMessenger->FindChatByGlobalId(m_bsChatId,Chat))
		{
			theNet2.LockTranslator();
			try
			{
				Handle = Chat->Leave();
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
			// Error Message [8/10/2002]
		}

	}		
}

void CDelChatDlg::OnCancel() 
{
	if(Handle&&pMessenger->ConnectEnable())
	{
		pSession->CancelOperation(Handle);
	}
	else
		KillWindow();
}

LRESULT CDelChatDlg::OnNetEvent(WPARAM w,LPARAM l)
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
		pMessenger->DeleteChat(m_bsChatId);
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
		//MessageBox(GetString(IDS_DEL_USER_ERROR),GetString(IDS_ERROR_TITLE),MB_OK|MB_ICONSTOP);
		break;
	}
	
	delete pItem;
    return 0;
}

BOOL CDelChatDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	pSession = theNet2.GetSession();
	ASSERT(pSession!=NULL);
	
	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),FALSE);
	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),TRUE);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

void CDelChatDlg::Block()
{
	GetDlgItem(IDC_BTN_YES)->EnableWindow(FALSE);
}

void CDelChatDlg::UnBlock()
{
	GetDlgItem(IDC_BTN_YES)->EnableWindow(TRUE);
}

void CDelChatDlg::SetKillChat(const CChat &Chat)
{
	m_bsChatId = Chat.GetId();

	m_Description.Format(GetString(IDS_CHAT_DEL_FORMAT),Chat.GetShowName());

	UpdateData(FALSE);
}

void CDelChatDlg::KillWindow()
{
	if(GetStyle()&WS_VISIBLE&&GetOptionInt(IDS_OFSMESSENGER,IDS_ANIMATION,FALSE))
		RoundExitAddon(this);
	CDialog::OnClose();
	if(!bIsKillWinodow)
	{
		bIsKillWinodow = TRUE;
		//pMessenger->SendMessage(WM_KILL_DELUSER_MESSAGE_DLG,(WPARAM)m_KillUser.GetGlobalID(),(LPARAM)this);
		DestroyWindow();
		delete this;
	}
}

BEGIN_EVENTSINK_MAP(CDelChatDlg, CDialog)
    //{{AFX_EVENTSINK_MAP(CDelChatDlg)
	//}}AFX_EVENTSINK_MAP
END_EVENTSINK_MAP()

//DEL void CDelChatDlg::OnClickButtonYes() 
//DEL {
//DEL 	OnOK();
//DEL }

//DEL void CDelChatDlg::OnClickButtonNo() 
//DEL {
//DEL 	OnCancel();
//DEL }

//DEL void CDelChatDlg::LoadSkin(IXMLDOMNode *pXmlRoot)
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

//DEL void CDelChatDlg::OnClickButtonX() 
//DEL {
//DEL 	OnCancel();
//DEL }

//DEL void CDelChatDlg::OnPaint() 
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

void CDelChatDlg::OnYes() 
{
	OnOK();
}

void CDelChatDlg::OnNo() 
{
	OnCancel();
}
