// ChatCreateDlg.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "ChatCreateDlg.h"
#include "MainDlg.h"	

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CChatCreateDlg dialog


CChatCreateDlg::CChatCreateDlg(CMainDlg *pMsgParent,CWnd* pParent /*=NULL*/)
: COFSNcDlg2(CChatCreateDlg::IDD, pParent)
{
	m_pMessenger	=	pMsgParent;
	m_DlgMode	=	CCDM_CREATE;
	//{{AFX_DATA_INIT(CChatCreateDlg)
	// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
	m_strSkinSettings = _T("/Shell/Conference/Create/skin_create.xml");
	m_bResizable = FALSE;

	bIsKillWinodow	=	FALSE;
	m_lHandle		=	0;
}


void CChatCreateDlg::DoDataExchange(CDataExchange* pDX)
{
	COFSNcDlg2::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CChatCreateDlg)
	DDX_Control(pDX, IDC_EDIT_INVITATION, m_edInvit);
	DDX_Control(pDX, IDC_EDIT_DESCR, m_edDescr);
	DDX_Control(pDX, IDC_EDIT_NAME, m_edName);
	DDX_Control(pDX, IDC_BTN_CREATE, m_btnCreate);
	DDX_Control(pDX, IDC_X, m_btnX);
	DDX_Control(pDX, IDC_USER_LIST, m_treebox);
	DDX_Control(pDX, IDC_BTN_INVITE, m_btnInvite);
	DDX_Control(pDX, IDC_BTN_UPDATE, m_btnUpdate);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CChatCreateDlg, COFSNcDlg2)
	//{{AFX_MSG_MAP(CChatCreateDlg)
	ON_BN_CLICKED(IDOK, OnOk)
	ON_MESSAGE(WM_NLT_CONTAINER_EVENT,OnNetEvent)	
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CChatCreateDlg message handlers

BEGIN_EVENTSINK_MAP(CChatCreateDlg, COFSNcDlg2)
    //{{AFX_EVENTSINK_MAP(CChatCreateDlg)
	ON_EVENT(CChatCreateDlg, IDC_BTN_CREATE, -600 /* Click */, OnClickBtnCreate, VTS_NONE)
	ON_EVENT(CChatCreateDlg, IDC_X, -600 /* Click */, OnClickBtnX, VTS_NONE)
	ON_EVENT(CChatCreateDlg, IDC_BTN_INVITE, -600 /* Click */, OnClickBtnInvite, VTS_NONE)
	ON_EVENT(CChatCreateDlg, IDC_BTN_UPDATE, -600 /* Click */, OnClickBtnUpdate, VTS_NONE)
	ON_EVENT(CChatCreateDlg, IDC_USER_LIST, 3 /* Action */, OnActionCcootreectrl, VTS_I4 VTS_BOOL)
	ON_EVENT(CChatCreateDlg, IDC_USER_LIST, 4 /* DoDrop */, OnDoDropCcootreectrl, VTS_I4 VTS_BOOL VTS_UNKNOWN VTS_I4 VTS_I4)
	ON_EVENT(CChatCreateDlg, IDC_USER_LIST, 1 /* Menu */, OnMenuCcootreectrl, VTS_I4 VTS_BOOL)
	ON_EVENT(CChatCreateDlg, IDC_USER_LIST, 2 /* Select */, OnSelectCcootreectrl, VTS_I4 VTS_BOOL)
	//}}AFX_EVENTSINK_MAP
END_EVENTSINK_MAP()

void CChatCreateDlg::LoadSkin(IXMLDOMNode *pXmlRoot)
{
	LoadButton(pXmlRoot, _T("X"), &m_btnX, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Create"), &m_btnCreate, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Invite"), &m_btnInvite, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Update"), &m_btnUpdate, TRUE, FALSE);

	LoadRectangle(pXmlRoot, _T("EditName"), &m_edName, TRUE);
	LoadRectangle(pXmlRoot, _T("EditDescription"), &m_edDescr, TRUE);
	LoadRectangle(pXmlRoot, _T("Users"), &m_treebox, TRUE);
	LoadRectangle(pXmlRoot, _T("EditInvitation"), &m_edInvit, TRUE);
}


BOOL CChatCreateDlg::OnInitDialog() 
{
	COFSNcDlg2::OnInitDialog();

	m_pSession	=	theNet2.GetSession();

	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),FALSE);
	SetIcon(AfxGetApp()->LoadIcon(IDR_MAINFRAME),TRUE);

	CString strText;

	switch(m_DlgMode) 
	{
	case CCDM_CREATE:
		SetWindowText(GetString(IDS_CREATE_NEW_CONFERENCE));
		break;
	case CCDM_UPDATE:
		strText.Format(GetString(IDS_UPDATE_CONFERENCE),m_strConferenceName);
		SetWindowText(strText);
		break;
	case CCDM_INVITE:
		strText.Format(GetString(IDS_INVITE_CONFERENCE),m_strConferenceName);
		SetWindowText(strText);
		break;
	case CCDM_DETAIL:
		strText.Format(GetString(IDS_DETAILS_CONFERENCE),m_strConferenceName);
		SetWindowText(strText);
		break;
	default:
		ASSERT(FALSE);
	}

	CreateTree();
	
	// TODO: Add extra initialization here
	UpdateControls();
	
	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

void CChatCreateDlg::OnOk() 
{
}

void CChatCreateDlg::OnCancel()
{
	if((m_lHandle==0L)||!m_pMessenger->ConnectEnable())
	{
		KillWindow();
		return;
	}
	m_pSession->CancelOperation(m_lHandle);
}

void CChatCreateDlg::OnClickBtnX() 
{
	OnCancel();
}

void CChatCreateDlg::UpdateControls()
{
	m_btnCreate.ShowWindow(m_DlgMode==CCDM_CREATE?SW_SHOW:SW_HIDE);
	m_btnInvite.ShowWindow(m_DlgMode==CCDM_INVITE?SW_SHOW:SW_HIDE);
	m_btnUpdate.ShowWindow(m_DlgMode==CCDM_UPDATE?SW_SHOW:SW_HIDE);

	switch(m_DlgMode) 
	{
	case CCDM_CREATE:
		//m_edInvit.EnableWindow(FALSE);
		break;
	case CCDM_UPDATE:
		m_treebox.EnableWindow(FALSE);
		m_edInvit.EnableWindow(FALSE);
		break;
	case CCDM_INVITE:
		m_edDescr.EnableWindow(FALSE);
		m_edName.EnableWindow(FALSE);
		break;
	case CCDM_DETAIL:
		m_edDescr.EnableWindow(FALSE);
		m_edName.EnableWindow(FALSE);
		m_treebox.EnableWindow(FALSE);
		m_edInvit.EnableWindow(FALSE);
		break;
	default:
		ASSERT(FALSE);
	}
}

void CChatCreateDlg::OnClickBtnCreate() 
{
	UpdateData(TRUE);
	
	if(m_pMessenger->ConnectEnable() && !m_lHandle)
	{
		CString strName, strDescription;
		m_edName.GetWindowText(strName);
		m_edDescr.GetWindowText(strDescription);
		
		if(!strName.IsEmpty())
		{
			
			theNet2.LockTranslator();
			try
			{
				m_bsChatId = (BSTR)GUIDGen();
				m_lHandle =  m_pSession->CreateChat((BSTR)m_bsChatId,_bstr_t((LPCTSTR)strName),_bstr_t((LPCTSTR)strDescription));
				if(m_lHandle)
				{
					Block();
					theNet2.AddToTranslator(m_lHandle,GetSafeHwnd());
				}
				
			}
			catch(...) 
			{
				ASSERT(FALSE);
			}
			theNet2.UnlockTranslator();		
		}
		else
		{
			MessageBox(GetString(IDS_INVALID_CONFERENCE_NAME),GetString(IDS_ERROR_TITLE));
		}
	}
}

void CChatCreateDlg::OnClickBtnInvite() 
{
	UpdateData(TRUE);
	
	if(m_pMessenger->ConnectEnable() && !m_lHandle)
	{
		if(!EnableRecepients())
			return;
		
		CChat Chat;
		
		if(m_pMessenger->FindChatByGlobalId(m_bsChatId,Chat))
		{
			CString strInivation;
			m_edInvit.GetWindowText(strInivation);
			
			theNet2.LockTranslator();
			try
			{
				POSITION pos = m_UserList.InitIteration();
				
				CUser *pRecipient	=	NULL;

				while(m_UserList.GetNext(pos,pRecipient))
				{
					if(pRecipient->m_bHasNewMessages)
					{
						Chat->AddUser(pRecipient->GetGlobalID());
					}
				}
					
				m_lHandle =  Chat->Invite(_bstr_t(LPCTSTR(strInivation)));

				if(m_lHandle)
				{
					Block();
					theNet2.AddToTranslator(m_lHandle,GetSafeHwnd());
				}
				
			}
			catch(...) 
			{
				ASSERT(FALSE);
			}
			theNet2.UnlockTranslator();
		}
	}
}

void CChatCreateDlg::OnClickBtnUpdate() 
{
	UpdateData(TRUE);
	
	if(m_pMessenger->ConnectEnable() && !m_lHandle)
	{
		CChat Chat;

		if(m_pMessenger->FindChatByGlobalId(m_bsChatId,Chat))
		{
			CString strName, strDescription;
			m_edName.GetWindowText(strName);
			m_edDescr.GetWindowText(strDescription);
			
			if(!strName.IsEmpty())
			{
				theNet2.LockTranslator();
				try
				{
					m_lHandle =  Chat->Edit(_bstr_t((LPCTSTR)strName),_bstr_t((LPCTSTR)strDescription));
					if(m_lHandle)
					{
						Block();
						theNet2.AddToTranslator(m_lHandle,GetSafeHwnd());
					}
				}
				catch(...) 
				{
					ASSERT(FALSE);
				}
				theNet2.UnlockTranslator();
			}
			else
			{
				MessageBox(GetString(IDS_INVALID_CONFERENCE_NAME),GetString(IDS_ERROR_TITLE));
			}
		}
		else
		{
			// Error Message ...
		}
	}
}


BOOL CChatCreateDlg::Create(CChatCreateDlg::ChatCreateDlgMode DlgMode, LPCTSTR strName, 
							LPCTSTR strDescription,
							LPCTSTR strInvite,
							CUserCollection *pUsers, BSTR bsChatId)
{	
	switch(DlgMode) 
	{
	case CCDM_CREATE :
		m_strSkinSettings = _T("/Shell/Conference/Create/skin_create.xml");
		break;
	case CCDM_UPDATE:
		m_strSkinSettings = _T("/Shell/Conference/Create/skin_update.xml");
		break;
	case CCDM_INVITE:
		m_strSkinSettings = _T("/Shell/Conference/Create/skin_invite.xml");
		break;
	default:
		m_strSkinSettings = _T("/Shell/Conference/Create/skin.xml");
	}

	m_strConferenceName = strName;

	m_DlgMode	=	(ChatCreateDlgMode)DlgMode;
	
	BOOL bRetValue =  COFSNcDlg2::Create(CChatCreateDlg::IDD,GetDesktopWindow());
	
	if(strName)
		m_edName.SetWindowText(strName);
		
	
	if(strDescription)
		m_edDescr.SetWindowText(strDescription);
	
	if(strInvite)
		m_edInvit.SetWindowText(strInvite);

	if(bsChatId)
		m_bsChatId	=	bsChatId;
	
	if(pUsers)
	{
		// Create Contact List Copy ...
		//////////////////////////////////////////////////////////////////////////
		// Копируем Контак Лист [12/21/2001] 
		try
		{
			CUser* pUser=NULL;
			
			if(POSITION pos = pUsers->InitIteration())
			{
				for(int i=0; pUsers->GetNext(pos,pUser); i++)
				{
					m_UserList.SetAt(*pUser);
				}
			}
		}
		catch(...)
		{
			ASSERT(FALSE);
		}
		//////////////////////////////////////////////////////////////////////////
		
		// Load User ... [8/8/2002]

		BuildTree();
	}

	return bRetValue;
}

LRESULT CChatCreateDlg::OnNetEvent(WPARAM w,LPARAM l)
{
	NLT_Container *pItem = (NLT_Container *)w;
	ASSERT(pItem!=NULL);
	
	theNet2.LockTranslator();
	theNet2.RemoveFromTranslator(m_lHandle);
	theNet2.UnlockTranslator();

	
	
	switch(pItem->EventType)
	{
	case NLT_EChatCreate:
		{
			IChat *pChat	=	NULL;
			HRESULT hr = AutoUnMarchaling(pItem,(LPUNKNOWN*)&pChat);
			if(hr==S_OK)
			{
				// Step 1. Add new Chat to ChatsCollections [8/9/2002]
				m_pMessenger->AddNewChat(pChat,SC_ACTIVE);
				// Step 2. Free Interface [8/9/2002]
				pChat->Release();
			}
			// Step 3. Close the Window
			if(m_DlgMode==CCDM_CREATE&&EnableRecepients())
			{
				UnBlock();

				m_DlgMode = CCDM_INVITE;
				
				UpdateControls();

				CString strText;

				strText.Format(GetString(IDS_INVITE_CONFERENCE),m_strConferenceName);
				SetWindowText(strText);
								
				OnClickBtnInvite();
			}
			else
				KillWindow();
		}
		break;
    case NLT_ECommandOK:
		switch(m_DlgMode) 
		{
		case CCDM_CREATE:
			// Step 3. Close the Window
			ASSERT(FALSE);
			break;
		case CCDM_UPDATE:
			// Step 3. Close the Window
			ASSERT(FALSE);
			break;
		case CCDM_INVITE:
			// Step 3. Close the Window
			KillWindow();
			break;
		default:
			ASSERT(FALSE);
		}
		break;
	case NLT_ECommandError:
		// Show error dlg ...
		UnBlock();
		break;
	};
	delete pItem;
	
	return 0;
}

void CChatCreateDlg::Block()
{
	switch(m_DlgMode) 
	{
	case CCDM_CREATE:
		m_edInvit.EnableWindow(FALSE);
		m_edDescr.EnableWindow(FALSE);
		m_edName.EnableWindow(FALSE);
		m_btnCreate.EnableWindow(FALSE);
		m_treebox.EnableWindow(FALSE);
		break;
	case CCDM_UPDATE:
		m_edDescr.EnableWindow(FALSE);
		m_edName.EnableWindow(FALSE);
		m_btnUpdate.EnableWindow(FALSE);
		break;
	case CCDM_INVITE:
		m_edInvit.EnableWindow(FALSE);
		m_treebox.EnableWindow(FALSE);
		m_btnInvite.EnableWindow(FALSE);
		break;
	default:
		ASSERT(FALSE);
	}
}

void CChatCreateDlg::UnBlock()
{
	switch(m_DlgMode) 
	{
	case CCDM_CREATE:
		m_edInvit.EnableWindow(TRUE);
		m_edDescr.EnableWindow(TRUE);
		m_edName.EnableWindow(TRUE);
		m_btnCreate.EnableWindow(TRUE);
		m_treebox.EnableWindow(TRUE);
		break;
	case CCDM_UPDATE:
		m_edDescr.EnableWindow(TRUE);
		m_edName.EnableWindow(TRUE);
		m_btnUpdate.EnableWindow(TRUE);
		break;
	case CCDM_INVITE:
		m_edInvit.EnableWindow(TRUE);
		m_treebox.EnableWindow(TRUE);
		m_btnInvite.EnableWindow(TRUE);
		break;
	default:
		ASSERT(FALSE);
	}
}

void CChatCreateDlg::KillWindow()
{
	if(GetStyle()&WS_VISIBLE&&GetOptionInt(IDS_OFSMESSENGER,IDS_ANIMATION,FALSE))
		RoundExitAddon(this);

	COFSNcDlg2::OnClose();

	if(!bIsKillWinodow)
	{
		m_UserList.Clear();
		m_treebox.DeleteTree();
		
		bIsKillWinodow = TRUE;
		DestroyWindow();
		delete this;
	}
}

void CChatCreateDlg::BuildTree()
{
	m_treebox.DeleteTree();
	
	m_UserCheckInGroup.RemoveAll();
	m_GroupTIDMap.RemoveAll();
	
	CUser* pUser=NULL;
	
	long  m_ShablonId[4][10]	=	{
		{0L,0L,0L,0L,0L,0L,0L,0L,0L,0L}, /// Группа ...
		{1L,0L,0L,0L,0L,0L,0L,0L,0L,0L}, /// Группа ...
		{2L,0L,0L,0L,0L,0L,0L,0L,0L,0L},  /// User
		{3L,0L,0L,0L,0L,0L,0L,0L,0L,0L}  /// User
	};
	
	int CLMode  = GetOptionInt(IDS_OFSMESSENGER,IDS_CLMODE,2);
	
	if(POSITION pos = m_UserList.InitIteration())
	{
		for(int i=0; m_UserList.GetNext(pos,pUser); i++)
		{
			// Step 1. Проверить создавали ли мы группу???
			long	GroupTID	= 0;
			CString	GroupName =	pUser->m_strType;
			
			BOOL isCheck = FALSE;//(m_strRecepientGroupName.CompareNoCase(pUser->m_strType)==0);
			
			switch(CLMode) 
			{
			case 1:
				{
					if(!m_GroupTIDMap.Lookup(pUser->m_strType,(void*&)GroupTID))
					{
						// Step 2. Если нет, то создать группу .
						GroupTID = m_treebox.AddItem(0,pUser->m_strType,m_ShablonId[0+isCheck]);
						m_GroupTIDMap.SetAt(pUser->m_strType,(void*)GroupTID);
						m_UserCheckInGroup.SetAt(GroupName,(void*)0);
					}
					// Step 3. добавить пользователя [1/28/2002]
					pUser->TID = m_treebox.AddItem(GroupTID,pUser->GetShowName(),m_ShablonId[pUser->m_bHasNewMessages+2]);								
					
					if(pUser->m_bHasNewMessages)
						m_UserCheckInGroup.SetAt(GroupName,(void*)(int(m_UserCheckInGroup[GroupName])+1));
				}
				break;
			case 2:
				{
					if(pUser->m_iStatus==S_OFFLINE||pUser->m_iStatus==S_INVISIBLE)
					{
						GroupName=  GetString(IDS_OFFLINE);
						
						if(!m_GroupTIDMap.Lookup(GetString(IDS_OFFLINE),(void*&)GroupTID))
						{
							long ShablonId[10] = {0L,1L,0L,0L,0L,0L,0L,0L,0L,0L};
							// Step 2. Если нет, то создать группу .
							GroupTID = m_treebox.AddItem(0,GetString(IDS_OFFLINE),ShablonId);
							m_GroupTIDMap.SetAt(GetString(IDS_OFFLINE),(void*)GroupTID);
							m_UserCheckInGroup.SetAt(GroupName,(void*)0);
						}
					}
					else
						if(!m_GroupTIDMap.Lookup(pUser->m_strType,(void*&)GroupTID))
						{
							// Step 2. Если нет, то создать группу .
							GroupTID = m_treebox.AddItem(0,pUser->m_strType,m_ShablonId[0+isCheck]);
							m_GroupTIDMap.SetAt(pUser->m_strType,(void*)GroupTID);
							m_UserCheckInGroup.SetAt(GroupName,(void*)0);
						}
						// Step 3. добавить пользователя [1/28/2002]
						pUser->TID = m_treebox.AddItem(GroupTID,pUser->GetShowName(),m_ShablonId[pUser->m_bHasNewMessages+2]);								
						
						if(pUser->m_bHasNewMessages)
							m_UserCheckInGroup.SetAt(GroupName,(void*)(int(m_UserCheckInGroup[GroupName])+1));
				}
				break;
			}
			
			m_treebox.RootOpen(GroupTID,pUser->m_bHasNewMessages);
		}
	}

	UpdateGroupCheck();
	
}

void CChatCreateDlg::UpdateGroupCheck()
{
	POSITION pos =  m_UserCheckInGroup.GetStartPosition();
	while(pos)
	{
		CString strKey;
		int		Data;
		m_UserCheckInGroup.GetNextAssoc(pos,strKey,(void*&)Data);
		UpdateGroupID(strKey);
	}
}

void CChatCreateDlg::UpdateGroupID(LPCTSTR strName)
{
	long  m_ShablonId[4][10]	=	{
		{0L,0L,0L,0L,0L,0L,0L,0L,0L,0L}, /// Группа ...
		{1L,0L,0L,0L,0L,0L,0L,0L,0L,0L}, /// Группа ...
		{2L,0L,0L,0L,0L,0L,0L,0L,0L,0L},  /// User
		{3L,0L,0L,0L,0L,0L,0L,0L,0L,0L}  /// User
	};
	
	if(m_UserCheckInGroup[strName])
		m_treebox.SetItemId((long)m_GroupTIDMap[strName],m_ShablonId[1]);
	else
		m_treebox.SetItemId((long)m_GroupTIDMap[strName],m_ShablonId[0]);
}

void CChatCreateDlg::CreateTree()
{
	CBitmap			hbmpCheckImage;	
	hbmpCheckImage.LoadBitmap(IDB_TREECHECK);

	m_treebox.SetImageList((long)hbmpCheckImage.Detach());
	
	short PriorityIndex[10];
	for(int i=0;i<10;i++)
		PriorityIndex[i] = -1;
	
	m_treebox.SetPriority(PriorityIndex);
	
	long  m_ShablonId[4][10]	=	{
		{0L,0L,0L,0L,0L,0L,0L,0L,0L,0L}, /// Группа ...
		{1L,0L,0L,0L,0L,0L,0L,0L,0L,0L}, /// Группа ...
		{2L,0L,0L,0L,0L,0L,0L,0L,0L,0L},  /// User
		{3L,0L,0L,0L,0L,0L,0L,0L,0L,0L}  /// User
	};
	
	short m_ShablonIcon[4][10]	=	
	{
		{2,-1,-1,-1,-1,-1,-1,-1,-1,-1}, /// Группа UnCheck
		{3,-1,-1,-1,-1,-1,-1,-1,-1,-1},  /// Группа Check
		{2,-1,-1,-1,-1,-1,-1,-1,-1,-1},  /// User UnCheck
		{3,-1,-1,-1,-1,-1,-1,-1,-1,-1}  /// User Check
	};
	
	DWORD m_ShablonRGBTextEnable [4] = 
	{
		RGB(0,0,100), /// Группа ...
			RGB(0,0,100), /// Группа ...
			RGB(0,0,0), /// User
			RGB(0,0,0) /// User
	};
	
	DWORD m_ShablonRGBTextSelect[4] = 
	{
		RGB(0,0,200), /// Группа ...
			RGB(0,0,200), /// Группа ...
			RGB(0,0,0), /// User
			RGB(0,0,0) /// User
	};
	
	for(int i = 0 ;i<4;i++)
	{
		m_treebox.AddEffect(m_ShablonId[i],m_ShablonIcon[i],m_ShablonRGBTextEnable[i],
			m_ShablonRGBTextSelect[i],RGB(255,255,255),RGB(200,200,200));
	}
	
	m_treebox.SetEventMode(1);
}

void CChatCreateDlg::OnActionCcootreectrl(long TID, BOOL bGroupe) 
{
	if(TID!= -1)
		if(!bGroupe)
		{
			CUser *pUser = FindUserInVisualContactList(TID);
			if(pUser)
			{
				pUser->m_bHasNewMessages = !pUser->m_bHasNewMessages;
				// Change User Check in group [2/21/2002]
				int CLMode  = GetOptionInt(IDS_OFSMESSENGER,IDS_CLMODE,2);
				CString strGroupName;
				
				switch(CLMode) 
				{
				case 1:
					{
						strGroupName = pUser->m_strType;
					}
					break;
				case 2:
					{
						if(pUser->m_iStatus==S_OFFLINE||pUser->m_iStatus==S_INVISIBLE)
							strGroupName = GetString(IDS_OFFLINE);
						else
							strGroupName = pUser->m_strType;
					}
					break;
				}
				
				if(pUser->m_bHasNewMessages)
					m_UserCheckInGroup.SetAt(strGroupName,(void*)(int(m_UserCheckInGroup[strGroupName])+1));
				else
					m_UserCheckInGroup.SetAt(strGroupName,(void*)(int(m_UserCheckInGroup[strGroupName])-1));
				
				UpdateGroupID(strGroupName);
				// [2/21/2002]
				UpdateID(pUser->GetGlobalID());
			}
		}	
		else
		{
			CString strGrouName;
			
			POSITION pos =  m_GroupTIDMap.GetStartPosition();
			while(pos)
			{
				CString strKey;
				long	Data;
				m_GroupTIDMap.GetNextAssoc(pos,strKey,(void*&)Data);
				if(Data==TID)
				{
					strGrouName = strKey;
					break;
				}
			}

			if(!strGrouName.IsEmpty())
			{
				BOOL bCheck = !((BOOL)m_UserCheckInGroup[strGrouName]);
				if(POSITION pos = m_UserList.InitIteration())
				{
					CUser *pUser=NULL;

					m_UserCheckInGroup.SetAt(strGrouName,(void*)0);

					int CLMode  = GetOptionInt(IDS_OFSMESSENGER,IDS_CLMODE,2);
					
					while(m_UserList.GetNext(pos,pUser))
					{
						switch(CLMode) 
						{
						case 1:
							{
								pUser->m_bHasNewMessages = bCheck;
								UpdateID(pUser->GetGlobalID());
								if(pUser->m_bHasNewMessages)
									m_UserCheckInGroup.SetAt(strGrouName,(void*)(int(m_UserCheckInGroup[strGrouName])+1));
							}
							break;
						case 2:
							{
								//  [2/21/2002]
								if(strGrouName.CompareNoCase(GetString(IDS_OFFLINE))==0&&
									(pUser->m_iStatus==S_OFFLINE||pUser->m_iStatus==S_INVISIBLE))
								{
									pUser->m_bHasNewMessages = bCheck;
									UpdateID(pUser->GetGlobalID());
									if(pUser->m_bHasNewMessages)
										m_UserCheckInGroup.SetAt(strGrouName,(void*)(int(m_UserCheckInGroup[strGrouName])+1));
								}
								else if(strGrouName.CompareNoCase(pUser->m_strType)==0&&
									!(pUser->m_iStatus==S_OFFLINE||pUser->m_iStatus==S_INVISIBLE))
								{
									pUser->m_bHasNewMessages = bCheck;
									UpdateID(pUser->GetGlobalID());
									if(pUser->m_bHasNewMessages)
										m_UserCheckInGroup.SetAt(strGrouName,(void*)(int(m_UserCheckInGroup[strGrouName])+1));
								}
								//  [2/21/2002]
							}
							break;
						}
					}
				}
				UpdateGroupID(strGrouName);
			}
		}
}

void CChatCreateDlg::OnDoDropCcootreectrl(long TID, BOOL bGroupe, LPUNKNOWN pDataObject, long DropEffect, long LastKeyState) 
{
	// TODO: Add your control notification handler code here
	
}

void CChatCreateDlg::OnMenuCcootreectrl(long TID, BOOL bGroupe) 
{
	if(!bGroupe)
	{
		CUser *pUser = FindUserInVisualContactList(TID);
		if(pUser)
		{
			CUser User = *pUser;
			m_pMessenger->ShowUserMenu(User.GetGlobalID());
		}
	}
}

void CChatCreateDlg::OnSelectCcootreectrl(long TID, BOOL bGroupe) 
{
	//OnActionCcootreectrl(TID, bGroupe);
}

void CChatCreateDlg::UpdateID(long UserId)
{
	CUser *pUser = m_UserList.GetAt(UserId);
	if(pUser)
	{	
		long  m_ShablonId[4][10]	=	{
			{0L,0L,0L,0L,0L,0L,0L,0L,0L,0L}, /// Группа ...
			{1L,0L,0L,0L,0L,0L,0L,0L,0L,0L}, /// Группа ...
			{2L,0L,0L,0L,0L,0L,0L,0L,0L,0L},  /// User
			{3L,0L,0L,0L,0L,0L,0L,0L,0L,0L}  /// User
		};
		
		m_treebox.SetItemId(pUser->TID,m_ShablonId[pUser->m_bHasNewMessages+2]);
	}
}

CUser* CChatCreateDlg::FindUserInVisualContactList(long TID)
{
	if(POSITION pos = m_UserList.InitIteration())
	{
		CUser *pUser=NULL;
		
		while(m_UserList.GetNext(pos,pUser))
		{
			if(pUser->TID == TID )
				return pUser;
		}
	}
	
	return NULL;
}

BOOL CChatCreateDlg::EnableRecepients()
{
	CUser *pRecipient			=	NULL;
	BOOL	bEnableContactUser	=	FALSE;
	
	if(POSITION pos = m_UserList.InitIteration())
	{
		while(m_UserList.GetNext(pos,pRecipient))
		{
			if(pRecipient->m_bHasNewMessages)
				return  TRUE;
		}
	}
	return FALSE;
}


//DEL void CChatCreateDlg::SetChat(const CChat &Chat)
//DEL {
//DEL 
//DEL }

void CChatCreateDlg::AutoCreate()
{
	if(IsWindow(GetSafeHwnd()))
		OnClickBtnCreate();
	else
		ASSERT(FALSE);
}
