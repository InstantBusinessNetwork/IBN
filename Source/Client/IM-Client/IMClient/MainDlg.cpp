// MainDlg.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "MainDlg.h"

#include "MainDlg.h"
#include "cdib.h"
#include "LoadSkins.h"
#include "McVersionInfo.h"
#include "mcsettings.h"
#include "locale.h"
#include "WebWindow.h"
#include "XMLParsel.h"

#include "AddUserDlg.h"
#include "SaveDataBase.h"
#include "MessageDlg.h"
#include <ExDispID.h>	
#include "DlgTV.h"

///////////////////////////////////////////////
/// Определения для настроек ...
///////////////////////////////////////////////
// Новая версия
#include "PageGeneral.h"
#include "PageDialogMode.h"
#include "PageContactListMode.h"
#include "PageHistorySync.h"
#include "PageStatusMode.h"
#include "PageLaunchMode.h"
#include "PageEditMode.h"
#include "PageSound.h"
#include "PageAlerts.h"
#include "PageApps.h"
#include "PageChat.h"

#include "FileDescriptioDlg.h"
#include "McVersionInfo.h"
#include "GroupFileDescriptionDlg.h"
#include "GroupAssingToDo.h"

#include "PopupMessage.h"
#include "IBNUpdate.h"

#include "SelectServer.h"

#include "ChatCreateDlg.h"
#include "DelChatDlg.h"
#include "InviteChatDlg.h"
#include "MonitorDialog.h"
#include <atlutil.h>
//#include "McCreateWebFolders.h"

//#include "IBNWFGlobalFunctions.h"
#include "IBNTO_MESSAGE.h"

#include "PageMesTemplate.h"
#include "SmileManager.h"

#include "resource.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CMainDlg dialog
extern COfsTvApp theApp;
extern void SetCurrentSkin(CString strName);
extern CString GetCurrentSkin();
extern UINT g_IbnToMessage;

extern  CSmileManager CurrentSmileManager;

/// STARTUP Флаги для загрузки ...
const DWORD STARTUP_NONE				 = 0x0000;
const DWORD STARTUP_START				 = 0x0001;
const DWORD STARTUP_CONTACTLIST_LOAD     = 0x0002;
const DWORD STARTUP_LOADAPP_LOAD	     = 0x0004;
const DWORD STARTUP_ALL					 = 0x0007;
/// EXIT Флаги на выход...
const DWORD EXIT_NONE             = 0x0000;
const DWORD EXIT_START            = 0x0001;
const DWORD EXIT_MESSENGER        = 0x0002;
const DWORD EXIT_ALL              = 0x0003;
const DWORD EXIT_LOGOFF			  = 0x0004;

CMainDlg::CMainDlg(CWnd* pParent /*=NULL*/):COFSNcDlg2(CMainDlg::IDD, pParent),pSession(NULL),m_FileManager(this),
m_OleMainDropTarget(this),m_IbnCommandWnd(this)
{
	//{{AFX_DATA_INIT(CMainDlg)
	// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
	
	m_strSkinSettings = _T("/Shell/Main/skin.xml");
	
	//  [4/5/2002]
	setlocale(LC_ALL,"") ;
	srand( (unsigned)time( NULL ) );
	
	EnableAutomation();
	
	hIcon						=	NULL;
	dwStartUpInfo				=	STARTUP_NONE;
	dwExitInfo					=	EXIT_NONE;
	m_bNowMcUpdateDownload		=	FALSE;
	m_strMcUpdatePath 			=	_T("");
	m_strMcUpdateParam			=	_T("");
	m_AutoChangeStatusTimeLast	=	0;
	m_OldStatus					=	-1;
	m_bAutoUpdateExit			=	FALSE;
	m_hWndAddUserDialog			=	NULL;
	m_pAppList					=	NULL;
	m_LastUserStatus			=	S_ONLINE;
	
	BeforStatusInetStatus		= W_DISCONNECTED;
	
	//////////////////////////////////////////////////////////////////////////
	dwCurrentStatus				= W_DISCONNECTED;
	bRepairConect				= FALSE;
	m_bShowOnlyOnline			= FALSE;
	CurrTID						= -1; 
	CurrChatTID					=	-1;
	bExitAfterDiskonect			= FALSE;
	MessageSendMode				= GetOptionInt(IDS_OFSMESSENGER,IDS_MESSSAGEMODE,1);
	MesPotokLen					= 0;
	
	bEnableLocalHistory			= FALSE;
    m_LocalHistory				= NULL;
	SynchronizeteHandle			= 0L;
	pSaverDB					= NULL;
	bShowInfo					= TRUE;
	m_lLogonHandle				= 0;
	m_bUpdateUserStatus			=	FALSE;
	m_bUpdateDlgWasShow			= FALSE;
	//m_bCatchNavigate			= TRUE;
	//m_pMcHost					=	NULL;
	
	m_McUpdateWnd				=	NULL;
	
	m_bIsSSLMode				=	FALSE;
	
	m_bSilentMode				=	FALSE;
	
	m_bUserDomainMode			=	TRUE;
	
	m_hWndNetMonitor			=	NULL;
	
	m_hRightButtonDDGlobal		=	NULL;
	m_bRightButtonDDTID			=	-1;

	m_lPort = 0;

}

CMainDlg::~CMainDlg()
{
	if(m_pAppList)
	{
		delete m_pAppList;
		m_pAppList = NULL;
	}
	
	if(::IsWindow(m_DlgLog.GetSafeHwnd()))
		m_DlgLog.DestroyWindow();
	
	int Size = m_NewMessageArray.GetSize();
	for(int i=0;i<Size;i++)
	{
		CMessage *pMsg = m_NewMessageArray[i];
		delete pMsg;
	}
	m_NewMessageArray.RemoveAll();
	
	Size = m_ActiveExCaptureList.GetSize();
	for(int i=0;i<Size;i++)
	{
		LPMcScreenCaptureItem pItem = m_ActiveExCaptureList[i];
		delete pItem;
	}
	m_ActiveExCaptureList.RemoveAll();
	
	if(::IsWindow(m_FileManager.GetSafeHwnd()))
		m_FileManager.DestroyWindow();
	
	if(::IsWindow(m_HistoryDlg.GetSafeHwnd()))
		m_HistoryDlg.DestroyWindow();
	
	if(pSaverDB)
	{
		pSaverDB->Stop();
		delete pSaverDB;
		pSaverDB = NULL;
	}
}


void CMainDlg::DoDataExchange(CDataExchange* pDX)
{
	COFSNcDlg2::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CMainDlg)
	DDX_Control(pDX, IDC_BUTTON_X, m_btnX);
	DDX_Control(pDX, IDC_BUTTON_MIN, m_btnMin);
	DDX_Control(pDX, IDC_BUTTON_MENU, m_btnMenu);
	
	DDX_Control(pDX, IDC_CCOOTREECTRL, m_treebox);
	DDX_Control(pDX, IDC_CCOCONFERENCECTRL, m_chatbox);
	DDX_Control(pDX, IDC_STATUS_STATIC, m_StatusStatic);
	DDX_Control(pDX, IDC_USER_STATIC, m_UserStatic);
	DDX_Control(pDX, IDC_LOGO,m_picLogo);
	DDX_Control(pDX, IDC_APP_BAR,m_AppBar);
	DDX_Control(pDX, IDC_BUTTON_ALERTS, m_btnAlerts);
	DDX_Control(pDX, IDC_BUTTON_APPS, m_btnApps);
	DDX_Control(pDX, IDC_BUTTON_FILES, m_btnFiles);
	DDX_Control(pDX, IDC_BUTTON_DIRECTORY, m_btnDirectory);
	
	DDX_Control(pDX, IDC_BUTTON_SHOWOFFLINE, m_btnShowOffline);
	//}}AFX_DATA_MAP
	//DDX_Control(pDX, ID_EXWINDOW_BROWSER, m_InWindow);
	if(!IsWindow(m_InWindow.GetSafeHwnd()))
		m_InWindow.CreateAsChild(this, this);
	//	if(!IsWindow(m_WebFolderView.GetSafeHwnd()))
	//		m_WebFolderView.Create(AfxRegisterWndClass(CS_HREDRAW|CS_VREDRAW|CS_DBLCLKS,::LoadCursor(NULL, IDC_ARROW), (HBRUSH) ::GetStockObject(WHITE_BRUSH), NULL),
	//		NULL,WS_OVERLAPPED|WS_VISIBLE|WS_CHILD|WS_CLIPCHILDREN,CRect(10,10,20,20),this,777);
}


BEGIN_MESSAGE_MAP(CMainDlg, COFSNcDlg2)
//{{AFX_MSG_MAP(CMainDlg)
ON_COMMAND(ID_TREEGROUP_CONFERENCE_CREATENEW,OnTreegroupConferenceCreateNew)
ON_UPDATE_COMMAND_UI(ID_TREEGROUP_CONFERENCE_CREATENEW,OnUpdateTreegroupConferenceCreateNew)
ON_COMMAND(ID_TREEMENU_CONFERENCE_CREATENEW,OnTreemenuConferenceCreateNew)
ON_UPDATE_COMMAND_UI(ID_TREEMENU_CONFERENCE_CREATENEW,OnUpdateTreemenuConferenceCreateNew)

ON_COMMAND(ID_OPTIONS_CONFERENCE_CREATENEW,OnOptionsConferenceCreateNew)
ON_UPDATE_COMMAND_UI(ID_OPTIONS_CONFERENCE_CREATENEW,OnUpdateOptionsConferenceCreateNew)

ON_COMMAND(ID_CHATTREEMENU_VIEWDETAILS,OnChatViewDetails)
ON_COMMAND(ID_CHATTREEMENU_HISTORY,OnChatMessageHistory)
ON_COMMAND(ID_CHATTREEMENU_STATUS_LEAVE,OnChatLeave)
ON_COMMAND(ID_CHATTREEMENU_EDITDETAILS,OnChatEditDetails)
ON_COMMAND(ID_CHATTREEMENU_ADDAFRIEND,OnChatAddaFriends)
ON_COMMAND(ID_CHATTREEMENU_STATUS_INACTIVE,OnChatStatusInactive)
ON_COMMAND(ID_CHATTREEMENU_STATUS_ACTIVE,OnChatStatusActive)
ON_COMMAND(ID_CHATTREEMENU_ATTACH,OnChatAttach)
ON_COMMAND(ID_CHATTREEMENU_SENDMESSAGE,OnChatSendMessage)

ON_UPDATE_COMMAND_UI(ID_CHATTREEMENU_VIEWDETAILS,OnUpdateChatViewDetails)
ON_UPDATE_COMMAND_UI(ID_CHATTREEMENU_HISTORY,OnUpdateChatMessageHistory)
ON_UPDATE_COMMAND_UI(ID_CHATTREEMENU_STATUS_LEAVE,OnUpdateChatLeave)
ON_UPDATE_COMMAND_UI(ID_CHATTREEMENU_EDITDETAILS,OnUpdateChatEditDetails)
ON_UPDATE_COMMAND_UI(ID_CHATTREEMENU_ADDAFRIEND,OnUpdateChatAddaFriends)
ON_UPDATE_COMMAND_UI(ID_CHATTREEMENU_STATUS_INACTIVE,OnUpdateChatStatusInactive)
ON_UPDATE_COMMAND_UI(ID_CHATTREEMENU_STATUS_ACTIVE,OnUpdateChatStatusActive)
ON_UPDATE_COMMAND_UI(ID_CHATTREEMENU_ATTACH,OnUpdateChatAttach)
ON_UPDATE_COMMAND_UI(ID_CHATTREEMENU_SENDMESSAGE,OnUpdateChatSendMessage)

ON_COMMAND(ID_TRAY_SHOWNEWMESSAGE,OnTrayShowNewMessage)
ON_UPDATE_COMMAND_UI(ID_TRAY_SHOWNEWMESSAGE,OnUpdateTrayShowNewMessage)
ON_UPDATE_COMMAND_UI(ID_TRAY_MYSTATUS_ONLINE, OnUpdateTrayMystatusOnline)
ON_UPDATE_COMMAND_UI(ID_TRAY_MYSTATUS_OFFLINE, OnUpdateTrayMystatusOffline)
ON_COMMAND(ID_TRAY_MYSTATUS_ONLINE, OnTrayMystatusOnline)
ON_COMMAND(ID_TRAY_MYSTATUS_OFFLINE, OnTrayMystatusOffline)
ON_COMMAND(ID_STATUS_AWAY, OnStatusAway)
ON_UPDATE_COMMAND_UI(ID_STATUS_AWAY, OnUpdateStatusAway)
ON_COMMAND(ID_STATUS_DND, OnStatusDnd)
ON_UPDATE_COMMAND_UI(ID_STATUS_DND, OnUpdateStatusDnd)
ON_COMMAND(ID_STATUS_INVISIBLE, OnStatusInvisible)
ON_UPDATE_COMMAND_UI(ID_STATUS_INVISIBLE, OnUpdateStatusInvisible)
ON_COMMAND(ID_STATUS_NA, OnStatusNa)
ON_UPDATE_COMMAND_UI(ID_STATUS_NA, OnUpdateStatusNa)
ON_COMMAND(ID_STATUS_ONLINE, OnStatusOnline)
ON_UPDATE_COMMAND_UI(ID_STATUS_ONLINE, OnUpdateStatusOnline)
ON_WM_CLOSE()
ON_COMMAND(ID_TRAY_EXIT, OnTrayExit)
ON_COMMAND(ID_TRAY_PREFERENCES, OnTrayPreferences)
ON_COMMAND(ID_TRAY_REPORTABUG, OnTrayReportBug)
ON_COMMAND(ID_STATUS_OFFLINE, OnStatusOffline)
ON_UPDATE_COMMAND_UI(ID_STATUS_OFFLINE, OnUpdateStatusOffline)
ON_WM_CAPTURECHANGED()
ON_WM_TIMER()
ON_MESSAGE(WM_UPDATE_STATUS,OnUpdateStatus)
ON_MESSAGE(WM_SETTEXT,OnSetText)
ON_COMMAND(ID_DBLCLK_TRAY,OnDnlClkTray)
ON_COMMAND(ID_LCLK_TRAY,OnLClkTray)
ON_MESSAGE(WM_INETLOGIN,OnInetLogin)
ON_MESSAGE(WM_CANCELLOGIN,OnCancelLogin)
ON_MESSAGE(WM_SHOWMESSAGEBOX,OnShowMessageBox)
ON_MESSAGE(WM_SHOW_ADDUSER,OnShowAddUser)
ON_MESSAGE(WM_CHANGE_NEWMESSAGE,OnChangeNewMessage)
ON_MESSAGE(WM_UPDATE_EXIT,OnUpdateExit)
ON_MESSAGE(WM_INVOKE_STARTAUTO_UPDATE,OnInvokeStartAutoUploader)
ON_MESSAGE(WM_INVOKE_CREATECHAT,OnInvokeCreateChat)
ON_MESSAGE(WM_INVOKE_SENDMESSAGE,OnInvokeSendMessage)
ON_MESSAGE(WM_UPDATE_CONTACTLIST,OnUpdateContactList)
ON_MESSAGE(WM_LOADAPP_COMPLETED, OnLoadAppComleted)
ON_MESSAGE(WM_SHOW_LOGIN_DLG, OnShowLoginDlg)
ON_MESSAGE(WM_UPLOAD_APP_FILE, OnUploadAppFile)
ON_UPDATE_COMMAND_UI(ID_OPTIONS_LOGOFF,OnUpdateOptionsLogOff)
ON_COMMAND(ID_OPTIONS_LOGOFF, OnOptionsLogOff)
ON_UPDATE_COMMAND_UI(ID_TREEMENU_SENDMESSAGE, OnUpdateTreemenuSendmessage)
ON_COMMAND(ID_TREEMENU_SENDMESSAGE, OnTreemenuSendmessage)
ON_UPDATE_COMMAND_UI(ID_TREEMENU_SENDFILE, OnUpdateTreemenuSendfile)
ON_COMMAND(ID_TREEMENU_ASSIGNTODO, OnTreemenuAssignToDo)
ON_UPDATE_COMMAND_UI(ID_TREEMENU_ASSIGNTODO, OnUpdateTreemenuAssignToDo)

ON_COMMAND(ID_TREEMENU_SENDFILE, OnTreemenuSendfile)
ON_WM_LBUTTONDBLCLK()
ON_COMMAND(ID_TREEMENU_DELETEUSER, OnTreemenuDeleteuser)
ON_UPDATE_COMMAND_UI(ID_TREEMENU_DELETEUSER, OnUpdateTreemenuDeleteuser)
ON_COMMAND(ID_TREEMENU_FLOATINGON, OnTreemenuFloatingon)
ON_UPDATE_COMMAND_UI(ID_TREEMENU_FLOATINGON, OnUpdateTreemenuFloatingon)
ON_COMMAND(ID_TREEMENU_MESSAGESHISTORY, OnTreemenuMessageshistory)
ON_UPDATE_COMMAND_UI(ID_TREEMENU_MESSAGESHISTORY, OnUpdateTreemenuMessageshistory)
ON_COMMAND(ID_TREEMENU_USERDETAILS, OnTreemenuUserdetails)
ON_UPDATE_COMMAND_UI(ID_TREEMENU_USERDETAILS, OnUpdateTreemenuUserdetails)
ON_WM_LBUTTONDOWN()
ON_WM_LBUTTONUP()
ON_COMMAND(ID_OPTIONS_SYNCHRONIZEHISTORY, OnOptionsSynchronizehistory)
ON_UPDATE_COMMAND_UI(ID_OPTIONS_SYNCHRONIZEHISTORY, OnUpdateOptionsSynchronizehistory)
ON_COMMAND(ID_OPTIONS_ADDINIVITE_FINDUSERADDTOFRIENDS, OnOptionsAddiniviteFinduseraddtofriends)
ON_COMMAND(ID_OPTIONS_FILEMANAGER_DOWNLOAD, OnOptionsFilemanagerDownload)
ON_COMMAND(ID_OPTIONS_FILEMANAGER_UPLOAD, OnOptionsFilemanagerUpload)
ON_COMMAND(ID_OPTIONS_NETWORKMONITOR, OnNetworkMonitor)
ON_UPDATE_COMMAND_UI(ID_OPTIONS_NETWORKMONITOR, OnUpdateNetworkMonitor)
ON_WM_SETCURSOR()
ON_COMMAND(ID_OPTIONS_VIEWMYDETAILS, OnOptionsViewmydetails)
ON_UPDATE_COMMAND_UI(ID_OPTIONS_VIEWMYDETAILS, OnUpdateOptionsViewmydetails)
ON_COMMAND(ID_TREEGROUP_SENDFILE, OnTreegroupSendfile)
ON_COMMAND(ID_TREEGROUP_SENDMESSAGE, OnTreegroupSendmessage)
ON_UPDATE_COMMAND_UI(ID_TREEGROUP_SENDFILE, OnUpdateTreegroupSendfile)
ON_UPDATE_COMMAND_UI(ID_TREEGROUP_SENDMESSAGE, OnUpdateTreegroupSendmessage)

ON_COMMAND(ID_TREEGROUP_ASSIGNTODO, OnTreegroupAssignToDo)
ON_UPDATE_COMMAND_UI(ID_TREEGROUP_ASSIGNTODO, OnUpdateTreegroupAssignToDo)

ON_COMMAND(ID_TREEMENU_SENDEMAIL, OnTreemenuSendemail)
ON_UPDATE_COMMAND_UI(ID_TREEMENU_SENDEMAIL, OnUpdateTreemenuSendemail)
ON_COMMAND(ID_TREEGROUP_SENDEMAIL, OnTreegroupSendemail)
ON_UPDATE_COMMAND_UI(ID_TREEGROUP_SENDEMAIL, OnUpdateTreegroupSendemail)
ON_MESSAGE(WM_NLT_CONTAINER_EVENT,OnNetEvent)
ON_MESSAGE(WM_KILL_SEND_MESSAGE_DLG,OnKillSendMessageDlg)
ON_MESSAGE(WM_KILL_COME_MESSAGE_DLG,OnKillComeMessageDlg)
ON_MESSAGE(WM_KILL_SPLIT_MESSAGE_DLG,OnKillSplitMesageDlg)
ON_MESSAGE(WM_CHANGE_STATUS,OnChangeStatus)
ON_MESSAGE(WM_CHECK_SIGNAL_STATE,OnCheckSignalState)
ON_COMMAND(ID_OPTIONS_PREFERENCES, OnOptionsPreferences)
ON_WM_RENDERFORMAT()
ON_WM_DESTROY()
ON_MESSAGE(WM_SEND_FILE,OnSendFile)
ON_MESSAGE(WM_SEND_FILE2,OnSendFile2)
ON_MESSAGE(WM_SENDGROUP_FILE,OnSendGroupFile)
ON_MESSAGE(WM_KILL_DELUSER_MESSAGE_DLG,OnKillDelUserMessageDlg)
ON_MESSAGE(WM_KILL_ADDUSER_MESSAGE_DLG,OnKillAddUserMessageDlg)
ON_MESSAGE(WM_KILL_INFILE_DLG,OnKillInFileDlg)
ON_COMMAND_RANGE(20000,20100,OnAppItem)
ON_COMMAND_RANGE(20200,20500,OnInviteUserToConference)
ON_COMMAND_RANGE(20700,21000,OnInviteGroupToConference)
ON_COMMAND_RANGE(21100,21200,OnSendMessageTemplate)
ON_COMMAND_RANGE(21300,21400,OnSendGroupMessageTemplate)
ON_MESSAGE(WM_ALERT_POPUP_MESSAGE_CLK,OnAlertPopupClk)
ON_WM_SIZE()
ON_WM_PAINT()
ON_MESSAGE(WM_DHTML_EVENT,OnDhtmlEvent)
ON_WM_ACTIVATE()
ON_WM_SETFOCUS()
ON_WM_QUERYENDSESSION()
ON_WM_ENDSESSION()
ON_REGISTERED_MESSAGE(g_IbnToMessage, OnIBNToMessage)
ON_MESSAGE(WM_PROCESS_COMMAND_LINE_MESSAGES, OnProcessCommandLineMessages)
ON_WM_DROPFILES()

ON_COMMAND(ID_RIGHTBUTTONDD_ASSIGNTODO,OnRightButtonAssignToDo)
ON_UPDATE_COMMAND_UI(ID_RIGHTBUTTONDD_ASSIGNTODO,OnUpdateRightButtonAssignToDo)

ON_COMMAND(ID_RIGHTBUTTONDD_SENDFILE,OnRightButtonSendFile)
ON_UPDATE_COMMAND_UI(ID_RIGHTBUTTONDD_SENDFILE,OnUpdateRightButtonSendFile)

ON_COMMAND(ID_RIGHTBUTTONDD_UPLOADFILE,OnRightButtonUploadFile)
ON_UPDATE_COMMAND_UI(ID_RIGHTBUTTONDD_UPLOADFILE,OnUpdateRightButtonUploadFile)

ON_COMMAND(ID_RIGHTBUTTONDD_CANCEL,OnRightButtonCancel)
ON_UPDATE_COMMAND_UI(ID_RIGHTBUTTONDD_CANCEL,OnUpdateRightButtonCancel)

ON_COMMAND(ID_RIGHTBUTTONDD_CREATEISSUE,OnRightButtonCreateIssue)
ON_UPDATE_COMMAND_UI(ID_RIGHTBUTTONDD_CREATEISSUE,OnUpdateRightButtonCreateIssue)

ON_COMMAND(ID_TREEMENU_SENDSCREENSHOT,OnTreeMenuSendScreenShot)
ON_UPDATE_COMMAND_UI(ID_TREEMENU_SENDSCREENSHOT,OnUpdateTreeMenuSendScreenShot)

ON_COMMAND(ID_TREEMENU_SCREENCAPTURE_ASSINGTODO,OnTreeMenuScreenShotAssignToDo)
ON_UPDATE_COMMAND_UI(ID_TREEMENU_SCREENCAPTURE_ASSINGTODO,OnUpdateTreeMenuScreenShotAssignToDo)

ON_COMMAND(ID_TREEGROUP_SCREENCAPTURE_SEND,OnTreeGroupSendScreenShot)
ON_UPDATE_COMMAND_UI(ID_TREEGROUP_SCREENCAPTURE_SEND,OnUpdateTreeGroupSendScreenShot)

ON_COMMAND(ID_TREEGROUP_SCREENCAPTURE_ASSIGNTODO,OnTreeGroupScreenShotAssignToDo)
ON_UPDATE_COMMAND_UI(ID_TREEGROUP_SCREENCAPTURE_ASSIGNTODO,OnUpdateTreeGroupScreenShotAssignToDo)

ON_COMMAND(ID_TREEMENU_CREATEEVENT,OnTreeMenuCreateEvent)
ON_UPDATE_COMMAND_UI(ID_TREEMENU_CREATEEVENT,OnUpdateTreeMenuCreateEvent)

ON_COMMAND(ID_TREEGROUP_CREATEEVENT,OnTreeGroupCreateEvent)
ON_UPDATE_COMMAND_UI(ID_TREEGROUP_CREATEEVENT,OnUpdateTreeGroupCreateEvent)

// Addon [4/16/2004]
ON_COMMAND(ID_TOOLSMENU_SCREENCAPTURE_SEND, OnToolsMenuScreenCaptureSend)
ON_UPDATE_COMMAND_UI(ID_TOOLSMENU_SCREENCAPTURE_SEND, OnUpdateToolsMenuScreenCaptureSend)
ON_COMMAND(ID_TOOLSMENU_SCREENCAPTURE_ASSIGNTODO, OnToolsMenuScreenCaptureAssignToDo)
ON_UPDATE_COMMAND_UI(ID_TOOLSMENU_SCREENCAPTURE_ASSIGNTODO, OnUpdateToolsMenuScreenCaptureAssignToDo)
ON_COMMAND(ID_TOOLSMENU_SCREENCAPTURE_CREATEISSUE, OnToolsMenuScreenCaptureCreateIssue)
ON_UPDATE_COMMAND_UI(ID_TOOLSMENU_SCREENCAPTURE_CREATEISSUE, OnUpdateToolsMenuScreenCaptureCreateIssue)

ON_COMMAND(ID_TOOLSMENU_MESSAGEHISTORY, OnToolsMenuMessageHistory)
ON_UPDATE_COMMAND_UI(ID_TOOLSMENU_MESSAGEHISTORY, OnUpdateToolsMenuMessageHistory)
ON_COMMAND(ID_TOOLSMENU_FILEMANAGE, OnToolsMenuFileManage)
ON_UPDATE_COMMAND_UI(ID_TOOLSMENU_FILEMANAGE, OnUpdateToolsMenuFileManage)
ON_COMMAND(ID_TOOLSMENU_IBNTOOLBOX, OnToolsMenuIbnToolbox)
ON_UPDATE_COMMAND_UI(ID_TOOLSMENU_IBNTOOLBOX, OnUpdateToolsMenuIbnToolbox)
ON_COMMAND(ID_TOOLSMENU_NETWORKMONITOR, OnToolsMenuNetworkMonitor)
ON_UPDATE_COMMAND_UI(ID_TOOLSMENU_NETWORKMONITOR, OnUpdateToolsMenuNetworkMonitor)

//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CMainDlg message handlers

BOOL CMainDlg::Create(CWnd *pParentWnd)
{
	//strNowSkinName = _T("default");
	strNowSkinName = GetProductLanguage();

	SetCurrentSkin(strNowSkinName);
	
	if(!COFSNcDlg2::Create(IDD, pParentWnd))
	{
		TRACE0("Warning: failed to create CMainDlg.\n");
		return FALSE;
	}
	if(!m_bLoadSkin)
		COFSNcDlg2::LoadSkin();
	//ShowWindow(SW_SHOW);
	return TRUE;
}

void CMainDlg::OnOK() 
{
}

void CMainDlg::OnCancel() 
{
	COFSNcDlg2::OnCancel();
}

BEGIN_EVENTSINK_MAP(CMainDlg, COFSNcDlg2)
//{{AFX_EVENTSINK_MAP(CMainDlg)
ON_EVENT(CMainDlg, IDC_BUTTON_MIN, -600 /* Click */, OnClickButtonMin, VTS_NONE)
ON_EVENT(CMainDlg, IDC_BUTTON_X, -600 /* Click */, OnClickButtonX, VTS_NONE)
ON_EVENT(CMainDlg, IDC_BUTTON_MENU, -600 /* Click */, OnClickButtonMenu, VTS_NONE)

ON_EVENT(CMainDlg, IDC_CCOOTREECTRL, 1 /* Menu */, OnMenuTreectrl, VTS_I4 VTS_BOOL)
ON_EVENT(CMainDlg, IDC_CCOOTREECTRL, 2 /* Select */, OnSelectTreectrl, VTS_I4 VTS_BOOL)
ON_EVENT(CMainDlg, IDC_CCOOTREECTRL, 3 /* Action */, OnActionTreectrl, VTS_I4 VTS_BOOL)
ON_EVENT(CMainDlg, IDC_CCOOTREECTRL, 4 /* Action */, OnDoDropTreectrl, VTS_I4  VTS_BOOL  VTS_UNKNOWN  VTS_I4 VTS_I4)

ON_EVENT(CMainDlg, IDC_CCOCONFERENCECTRL, 1 /* Menu */, OnChatMenuTreectrl, VTS_I4 VTS_BOOL)
ON_EVENT(CMainDlg, IDC_CCOCONFERENCECTRL, 2 /* Select */, OnChatSelectTreectrl, VTS_I4 VTS_BOOL)
ON_EVENT(CMainDlg, IDC_CCOCONFERENCECTRL, 3 /* Action */, OnChatActionTreectrl, VTS_I4 VTS_BOOL)
ON_EVENT(CMainDlg, IDC_CCOCONFERENCECTRL, 4 /* Action */, OnChatDoDropTreectrl, VTS_I4  VTS_BOOL  VTS_UNKNOWN  VTS_I4 VTS_I4)

//ON_EVENT(CMainDlg, ID_EXWINDOW_BROWSER, DISPID_BEFORENAVIGATE2 /* BeforeNavigate2 */, OnBeforeNavigate2, VTS_DISPATCH VTS_PVARIANT VTS_PVARIANT VTS_PVARIANT VTS_PVARIANT VTS_PVARIANT VTS_PBOOL)
//ON_EVENT(CMainDlg, ID_EXWINDOW_BROWSER, DISPID_NAVIGATECOMPLETE2 /* DocumentComplet */, OnNavigateComplete2, VTS_DISPATCH VTS_PVARIANT)

ON_EVENT(CMainDlg, IDC_BUTTON_ALERTS, -600 /* Click */, OnClickButtonAlerts, VTS_NONE)
ON_EVENT(CMainDlg, IDC_BUTTON_APPS, -600 /* Click */, OnClickButtonApps, VTS_NONE)
ON_EVENT(CMainDlg, IDC_BUTTON_FILES, -600 /* Click */, OnClickButtonFiles, VTS_NONE)
ON_EVENT(CMainDlg, IDC_BUTTON_DIRECTORY, -600 /* Click */, OnClickButtonDirectory, VTS_NONE)
ON_EVENT(CMainDlg, IDC_BUTTON_SHOWOFFLINE, -600 /* Click */, OnClickButtonShowOfflineUser, VTS_NONE)
//}}AFX_EVENTSINK_MAP
END_EVENTSINK_MAP()

BOOL CMainDlg::OnInitDialog() 
{
	COFSNcDlg2::OnInitDialog();
	
	//////////////////////////////////////////////////////////////////////////
	// Set Style, Default: Don't Show IBN int the Task Bar. [7/22/2002]
	if(GetOptionInt(IDS_OFSMESSENGER,IDS_REMOVE_FROM_TASK_BAR,TRUE))
	{
		ModifyStyleEx(0,WS_EX_TOOLWINDOW);
	}
	//////////////////////////////////////////////////////////////////////////
	
	if(IsToolboxIstalled())
	{
		m_OleMainDropTarget.Register(this);
	}
	
	m_UserStatic.SetTransparent(TRUE);
	m_UserStatic.SetFontName(CString("Verdana"));
	m_UserStatic.SetFontBold(TRUE);
	m_UserStatic.SetFontSize(10);
	
	m_StatusStatic.SetTransparent(TRUE);
	m_StatusStatic.SetFontName(CString("Verdana"));
	m_StatusStatic.SetFontBold(TRUE);
	m_StatusStatic.SetFontSize(10);
	
	//m_StatusImageList.Create(IDB_TRAYSTATUS_BITMAP,16,1,0xff00ff);

	CBitmap bm;

	m_StatusImageList.Create(16,16,ILC_COLOR32|ILC_MASK, 0,4);
	bm.LoadBitmap(IDB_TRAYSTATUS_BITMAP);
  	m_StatusImageList.Add(&bm,RGB(255,0,255));
	bm.DeleteObject ();

	
	//m_McUpdateDownload.Init(GetSafeHwnd(),WM_MCUPDATE_LOADED);
	
	m_CoolMenuManager.Install((CFrameWnd*)this);
	
	LoadSkins LoadSkin;

	CurrentSmileManager.Init();
	
	try
	{
		IStreamPtr pStream = NULL;
		long Error = 0;
		LoadSkin.Load(bstr_t(IBN_SCHEMA)+bstr_t((LPCTSTR)GetProductLanguage())+bstr_t("/Shell/Main/status.bmp"),&pStream,&Error);
		if(pStream!=NULL)
		{	
			CDib dib(pStream);
			CPaintDC dc(this);
			m_bmpStatus.Attach(dib.GetHBITMAP(dc));
		}
		
		TOOLBARDATA Tdt = {1,16,16,15,0};
		WORD		dwItemID[] = {1,2,3,ID_STATUS_ONLINE,ID_STATUS_DND,ID_STATUS_AWAY,ID_STATUS_NA,4,5,6,7,ID_STATUS_OFFLINE,8,9,ID_STATUS_INVISIBLE};
		Tdt.items   = (WORD*)dwItemID;
		
		m_CoolMenuManager.LoadToolbar(m_bmpStatus,&Tdt);

	}
	catch (...) 
	{
		ASSERT(FALSE);
	}
	
	// create a view to occupy the client area of the frame
	SetIcon(hIcon,FALSE);
	SetIcon(hIcon,TRUE);
	
	CString strRect = GetOptionString(IDS_OFSMESSENGER, IDR_MAINFRAME, _T(""));
	if(!strRect.IsEmpty())
	{
		CRect rWindow = StringToRect(strRect);
		FitRectToWindow(rWindow);
		SetWindowPos(NULL,rWindow.left,rWindow.top,rWindow.Width(),rWindow.Height(),SWP_NOZORDER|SWP_NOACTIVATE);
	}
	
	//////////////////////////////////////////////////////////////////////////
	//  [4/5/2002]
	m_FileManager.Create(CFileManagerDlg::IDD,GetDesktopWindow());
	
	CRect rMessenger(53,57,52+194,57+236);
	//m_treebox.Create(NULL,WS_VISIBLE|WS_CLIPCHILDREN,rMessenger,this,ID_COOTREE_CTRL);
	pSession = theNet2.GetSession();
	ASSERT(pSession!=NULL);
	
	theNet2.InitEventWindow(this->m_hWnd);
	
	
	//m_bCatchNavigate = FALSE;
	
	//m_InWindow.SetRegisterAsDropTarget(FALSE);
	//m_InWindowDropTarget = new CMcMessengerDropTarget(this);
	//m_InWindowDropTarget->Register(&m_InWindow);
	//m_WebFolderView.ShowWindow(SW_HIDE);
	
	m_InWindow.ShowWindow(SW_HIDE);
	m_InWindow.Navigate(bstr_t(IBN_SCHEMA)+ bstr_t((LPCTSTR)GetProductLanguage()) +_T("/Common/blank.html"));
	//m_InWindow.m_browser.SetWindowPos(&m_treebox,-1,-1,-1,-1,SWP_NOMOVE|SWP_NOSIZE|SWP_NOACTIVATE);
	
	CreateTree();
	// Addon for Conference [8/6/2002]
	CreateChatTree();
	// OlegO [8/6/2002]
	
	m_HistoryDlg.Create(CHistoryDlg::IDD,GetDesktopWindow());
	m_HistoryDlg.SetMessenger(this);
	
	//SetWindowPos(&wndTopMost,-1,-1,-1,-1,SWP_NOMOVE|SWP_NOSIZE|SWP_NOACTIVATE);
	
	//m_AppBar.m_AppCtrl.Create(13,55,this);
	
	LOGFONT lgFont	=	{0};
	
	lgFont.lfHeight = -14;
	lgFont.lfWeight = FW_BOLD;
	lgFont.lfCharSet	=	DEFAULT_CHARSET;
	_tcscpy(lgFont.lfFaceName, _T("Arial"));
	
	m_AppBar.m_AppCtrl.SetFont(&lgFont);
	//////////////////////////////////////////////////////////////////////////
	
	CreateTray();
	
	m_DlgLog.m_LoginStr    = _T("");
	m_DlgLog.m_PasswordStr = _T("");
	
	//////////////////////////////////////////////////////////////////////////
	// Play Sound [2/27/2002]
	McPlaySound(IDS_STARTUP_KEY);
	//////////////////////////////////////////////////////////////////////////
				
	m_DlgLog.Create(CLoginDlg::IDD,this);

	BOOL bCreateCommandWndResult = m_IbnCommandWnd.Create();
	
	/*if(m_bSilentMode)
	m_DlgLog.ShowWindow(SW_HIDE);
	else
	{
	m_DlgLog.ShowWindow(SW_SHOWNORMAL);
	m_DlgLog.UpdateWindow();
	m_DlgLog.SetForegroundWindow();
	m_DlgLog.SetFocus();
	}*/
	
	//////////////////////////////////////////////////////////////////////////
	// McToolTipAddon
	m_ToolTip.AddTool(&m_btnX,IDS_TIP_CLOSE);
	m_ToolTip.AddTool(&m_btnMin,IDS_TIP_MINIMIZE);
	m_ToolTip.AddTool(&m_btnShowOffline,IDS_SHOWOFFLINE_USERS);
	//m_ToolTip.AddTool(&m_btnMenu,IDS_TIP_MENU);
	//////////////////////////////////////////////////////////////////////////
	
	if(GetOptionInt(IDS_OFSMESSENGER,IDS_AUTOLOGON,TRUE)&&
		GetOptionInt(IDS_LOGIN,IDS_REMEMBER,TRUE)&&
		!m_DlgLog.m_LoginStr.IsEmpty())
	{
		if(m_bSilentMode)
			m_DlgLog.ShowWindow(SW_HIDE);
		else
		{
			m_DlgLog.ShowWindow(SW_SHOWNORMAL);
			m_DlgLog.UpdateWindow();
			m_DlgLog.SetForegroundWindow();
			m_DlgLog.SetFocus();
		}
		Login();
	}
	else
	{
		m_DlgLog.ShowWindow(SW_SHOWNORMAL);
		m_DlgLog.UpdateWindow();
		m_DlgLog.SetForegroundWindow();
		m_DlgLog.SetFocus();
	}
	
	
	SetTimer(IDT_AUTOCHANGETIMER,10000,NULL);
	SetTimer(IDT_LOADNOTSENDMESSAGES,60000,NULL);
	SetTimer(IDT_MCSCREENCAPTUREEX,500,NULL);
	//SetTimer(IDT_LOADNOTSENDMESSAGES,6000,NULL);
	
	return TRUE;
}

void CMainDlg::OnClickButtonMin() 
{
	//if(GetOptionInt(IDS_OFSMESSENGER,IDS_HIDEINTRAY,FALSE)==FALSE)
	ShowWindow(SW_MINIMIZE);
	//else
	//	ShowWindow(SW_HIDE);
}

void CMainDlg::OnClickButtonX() 
{
	if(GetOptionInt(IDS_OFSMESSENGER,IDS_MINIMIZE_ONCLOSE,TRUE)==FALSE)
		OnClose();
	else 
		OnClickButtonMin();
}

void CMainDlg::LoadSkin(IXMLDOMNode *pXmlRoot)
{
	m_btnX.ShowWindow(SW_HIDE);
	m_btnMin.ShowWindow(SW_HIDE);
	m_btnMenu.ShowWindow(SW_HIDE);
	m_btnAlerts.ShowWindow(SW_HIDE);
	m_btnApps.ShowWindow(SW_HIDE);
	m_btnFiles.ShowWindow(SW_HIDE);
	m_btnDirectory.ShowWindow(SW_HIDE);
	m_btnShowOffline.ShowWindow(SW_HIDE);
	
	m_treebox.ShowWindow(SW_HIDE);
	m_chatbox.ShowWindow(SW_HIDE);
	m_InWindow.ShowWindow(SW_HIDE);
	//m_WebFolderView.ShowWindow(SW_HIDE);
	m_UserStatic.ShowWindow(SW_HIDE);
	m_StatusStatic.ShowWindow(SW_HIDE);
	m_picLogo.ShowWindow(SW_HIDE);
	m_AppBar.ShowWindow(SW_HIDE);
	
	
	
	LoadButton(pXmlRoot, _T("X"), &m_btnX, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Minimize"), &m_btnMin, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Menu"), &m_btnMenu, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Alerts"), &m_btnAlerts, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Apps"), &m_btnApps, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Files"), &m_btnFiles, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("Directory"), &m_btnDirectory, TRUE, FALSE);
	LoadButton(pXmlRoot, _T("ShowOffline"), &m_btnShowOffline, TRUE, TRUE);
	
	LoadRectangle(pXmlRoot, _T("Browser"), &m_InWindow, FALSE);
	//LoadRectangle(pXmlRoot, _T("Browser"), &m_WebFolderView, FALSE);
	LoadRectangle(pXmlRoot, _T("Browser"), &m_chatbox, FALSE);
	LoadRectangle(pXmlRoot, _T("Browser"), &m_treebox, TRUE);
	
	LoadRectangle(pXmlRoot, _T("UserShowName"), &m_UserStatic,TRUE);
	LoadRectangle(pXmlRoot, _T("UserStatus"), &m_StatusStatic, TRUE);
	LoadRectangle(pXmlRoot, _T("Logo"), &m_picLogo,TRUE);
	
	LoadAppBar(pXmlRoot);
	
	//  [9/21/2004]
	m_bShowOffline = GetOptionInt(IDS_OFSMESSENGER,IDS_SHOWOFFLINEFILES,TRUE);
	m_btnShowOffline.SetPressed(m_bShowOffline);
	//	LoadRectangle2(pXmlRoot, _T("Status"), m_.GetSafeHwnd(), TRUE);
}

void CMainDlg::OnClickButtonMenu() 
{
	ShowGeneralMenu(CurrChatTID);
}

//-------------------------------------------------------------------------------
// Name: 
// Desc: 
//-------------------------------------------------------------------------------
void CMainDlg::OnUpdateTrayMystatusOnline(CCmdUI* pCmdUI) 
{
	// TODO: Add your command update UI handler code here
	pCmdUI->SetRadio(ConnectEnable());
}

//-------------------------------------------------------------------------------
// Name: 
// Desc: 
//-------------------------------------------------------------------------------
void CMainDlg::OnUpdateTrayMystatusOffline(CCmdUI* pCmdUI) 
{
	// TODO: Add your command update UI handler code here
	pCmdUI->SetRadio(!ConnectEnable());
}

//-------------------------------------------------------------------------------
// Name: 
// Desc: 
//-------------------------------------------------------------------------------
void CMainDlg::OnTrayMystatusOnline() 
{
	// TODO: Add your command handler code here
	if(!ConnectEnable())
	{
		Login2();
	}
}

//-------------------------------------------------------------------------------
// Name: 
// Desc: 
//-------------------------------------------------------------------------------
void CMainDlg::OnTrayMystatusOffline() 
{
}

//-------------------------------------------------------------------------------
// Name: CreateBar
// Desc: Создаются Бары.
//-------------------------------------------------------------------------------

//-------------------------------------------------------------------------------
// Name: CreateTray
// Desc: Создается иконка в трэе.
//-------------------------------------------------------------------------------
int CMainDlg::CreateTray()
{
	m_hMessengerTrayIcon = theApp.LoadIcon(IDR_MAINFRAME);
	
	m_MessengerTray.Create(NULL,							// Parent window
		WM_NOTIFY_MESSENGER_TRAY,
		GetString(IDR_MAINFRAME),		// tooltip
		m_StatusImageList.ExtractIcon(0 + (IsSSLMode()?7:0)),							// Icon to use
		IDR_OFS_MENU);	
	
	return 0;
}

//-------------------------------------------------------------------------------
// Name: OnUpdateStatus
// Desc: Сообщение от Мессенжера об изменении статуса.
//-------------------------------------------------------------------------------
HRESULT CMainDlg::OnUpdateStatus(WPARAM w, LPARAM l)
{
	W_NETSTATUS NetStatus  = (W_NETSTATUS)w;
	
	DWORD UserStatus = l;
	
	COLORREF	color		=	RGB(0,166,81);
	//CStaticOFS	*pItem		=	NULL;
	//OFSWinType	Type;
	int			IconIndex	=	UserStatus;
	
	CString strText=_T("");
	
	TRACE(_T("\r\nUpdate User Status = %d (%d)"), UserStatus, m_LastUserStatus);
	
	//m_StatusBar2.GetItem(IDS_STATUSNAME,Type,(void**)&pItem);
	
	switch(NetStatus)
	{
	case W_DISCONNECTED:
		if(dwExitInfo & EXIT_START)
		{
			dwExitInfo |= EXIT_MESSENGER;
			CheckExit();
		}
		else if(dwExitInfo&EXIT_LOGOFF)
		{
			dwExitInfo		= EXIT_NONE;
			dwStartUpInfo	= STARTUP_NONE;
			m_DlgLog.UnBlock();
		}
		else
		{
			m_DlgLog.UnBlock();
		}
		
		strText = GetString(IDS_OFFLINE);
		color = RGB(255,0,0);
		
		
		break;
	case W_CONNECTING:
		IconIndex = 6;
		color		=	RGB(0xFF,0xFF,0);
		strText		= GetString(IDS_CONNECTING);
		break;
	case W_CONNECTED:
		switch(BeforStatusInetStatus)
		{
		case W_DISCONNECTED:
		case W_CONNECTING:
			{
				//OFSWinType m_Type;
				//CStaticOFS *pItem = NULL;
				//m_StatusBar2.GetItem(IDS_USERNAME,m_Type,(void**)&pItem);
				//if(m_Type==OFSWT_STATIC)
				//{
				//pItem->SetText(GetShowName());
				//pItem->SetColor(RGB(255,255,255));
				//}
				m_UserStatic.SetText(GetShowName());
				m_UserStatic.SetTextColor(RGB(255,255,255));
			}
			StartStartUpLoad();
			break;
		case W_CONNECTED:
			/// Продолжаем Обработку ситуации ...
			break;
		}
		//////////////////////////////////////////////////////////////////////////
		//  [1/21/2002]
		//////////////////////////////////////////////////////////////////////////
		switch(UserStatus)
		{
		case S_OFFLINE:
			strText = GetString(IDS_OFFLINE);
			color = RGB(255,0,0);
			break;
		case S_ONLINE:
			strText = GetString(IDS_ONLINE);
			color = RGB(0,166,81);
			m_LastUserStatus = S_ONLINE;
			break;		
		case S_INVISIBLE:
			strText = GetString(IDS_INVISIBLE);
			color = RGB(0,166,81);
			m_LastUserStatus = S_INVISIBLE;
			break;		
		case S_DND:
			strText = GetString(IDS_DND);
			color = RGB(0,166,81);
			m_LastUserStatus = S_DND;
			break;		
		case S_AWAY:
			strText = GetString(IDS_AWAY);
			color = RGB(0,166,81);
			m_LastUserStatus = S_AWAY;
			break;		
		case S_NA:
			strText = GetString(IDS_NA);
			color = RGB(0,166,81);
			m_LastUserStatus = S_NA;
			break;		
		case S_OCCUPIED:
			strText = GetString(IDS_OCCUPIED);
			color = RGB(0,166,81);
			m_LastUserStatus = IDS_OCCUPIED;
			break;		
		case S_AWAITING:
			strText = GetString(IDS_AWAITING);
			color = RGB(0,166,81);
			break;		
		case S_UNKNOWN:
			strText = GetString(IDS_UNKNOWN);
			color = RGB(0,166,81);
			break;		
		}
		//////////////////////////////////////////////////////////////////////////
		break;
	}
	
	m_MessengerTray.SetIcon(m_StatusImageList.ExtractIcon(IconIndex + (IsSSLMode()?7:0)));
	
	//pItem->SetText(strText);
	//pItem->SetColor(color);
	m_StatusStatic.SetText(strText+_T(":"));
	m_StatusStatic.SetTextColor(color);
				
				
	CString strToolTip	=	GetString(IDS_TRAY_TOOLTIP_FORMAT);
	
	// Domen Addon [3/29/2004]
	CString strTmp	=	strText;
	strText.Format(_T("%s - %s"), m_DlgLog.m_LoginStr, strTmp);
	
	if(IsSSLMode())
	{
		strText	+=	_T(" (SSL)");
	}
	// Domen Addon [3/29/2004]
	strToolTip.Format(GetString(IDS_TRAY_TOOLTIP_FORMAT),strText);
	
	m_MessengerTray.SetTooltipText(strToolTip);
	//pItem->SetText(strText);
	//pItem->SetColor(color);
	Invalidate(FALSE);
				
	BeforStatusInetStatus = NetStatus;
	
	return 0;
}


//-------------------------------------------------------------------------------
// Name: 
// Desc: 
//-------------------------------------------------------------------------------
void CMainDlg::OnStatusAway() 
{
	m_AutoStatus.Reset();
	PostMessage(WM_CHANGE_STATUS,S_AWAY);
}

//-------------------------------------------------------------------------------
// Name: 
// Desc: 
//-------------------------------------------------------------------------------
void CMainDlg::OnUpdateStatusAway(CCmdUI* pCmdUI) 
{
	UpdateStatus(S_AWAY,pCmdUI);
	
}

void CMainDlg::OnStatusDnd() 
{
	m_AutoStatus.Reset();
	PostMessage(WM_CHANGE_STATUS,S_DND);	
}

void CMainDlg::OnUpdateStatusDnd(CCmdUI* pCmdUI) 
{
	UpdateStatus(S_DND,pCmdUI);
	
}

void CMainDlg::OnStatusInvisible() 
{
	m_AutoStatus.Reset();
	PostMessage(WM_CHANGE_STATUS,S_INVISIBLE);		
	
}

void CMainDlg::OnUpdateStatusInvisible(CCmdUI* pCmdUI) 
{
	UpdateStatus(S_INVISIBLE,pCmdUI);
	
}

void CMainDlg::OnStatusNa() 
{
	m_AutoStatus.Reset();
	PostMessage(WM_CHANGE_STATUS,S_NA);		
	
}

void CMainDlg::OnUpdateStatusNa(CCmdUI* pCmdUI) 
{
	UpdateStatus(S_NA,pCmdUI);
	
}

void CMainDlg::OnStatusOnline() 
{
	m_AutoStatus.Reset();
	PostMessage(WM_CHANGE_STATUS,S_ONLINE);		
}

void CMainDlg::OnUpdateStatusOnline(CCmdUI* pCmdUI) 
{
	// TODO: Add your command update UI handler code here
	UpdateStatus(S_ONLINE,pCmdUI);
}

void CMainDlg::UpdateStatus(int Status,CCmdUI *pCmdUI)
{
	pCmdUI->Enable(dwStartUpInfo==STARTUP_ALL);
	pCmdUI->SetCheck(GetUserStatus()==Status);
}

//-------------------------------------------------------------------------------
// Name: Login
// Desc: Конектимся к Серверу.
//-------------------------------------------------------------------------------
void CMainDlg::Login()
{
	dwExitInfo		= EXIT_NONE;
	dwStartUpInfo	= STARTUP_NONE;
	
	SetInetOption();
	
	m_DlgLog.Block();

	CString LoginStr = m_DlgLog.m_LoginStr;

	int StartPortPos = -1;
	if((StartPortPos = LoginStr.Find(_T(":")))!=-1)
	{
		CString strPort = LoginStr.Mid(StartPortPos+1);

		LoginStr = LoginStr.Left(StartPortPos);

		m_lPort = _ttol(strPort);
	}
	else
		m_lPort=0;
	
    SetEnvironmentVariable(_T("MpaUserLogin"), LoginStr);
	SetEnvironmentVariable(_T("MpaUserPassword"),m_DlgLog.m_PasswordStr);
	
	LockShowInfo();
	
	Login2(m_LastUserStatus);
	
}


//-------------------------------------------------------------------------------
// Name: OnSetText
// Desc: Мои опыты. Можно убрать.
//-------------------------------------------------------------------------------
HRESULT CMainDlg::OnSetText(WPARAM w, LPARAM l)
{
	// Turn WS_VISIBLE style off before calling Windows to
	// set the text. Reset to visible afterwards
    DWORD dwStyle = ::GetWindowLong(*this, GWL_STYLE);
    if (dwStyle & WS_VISIBLE)
        ::SetWindowLong(*this, GWL_STYLE, (dwStyle & ~ WS_VISIBLE));
	//Default();
	DefWindowProc( WM_SETTEXT, 0, l);
    if (dwStyle & WS_VISIBLE) 
		::SetWindowLong(*this, GWL_STYLE, dwStyle);
	
    Invalidate();
	SetWindowPos(NULL, 0,0,0,0, 
		SWP_FRAMECHANGED|SWP_NOACTIVATE|SWP_NOZORDER|SWP_NOSIZE|SWP_NOMOVE);	
	
	return 0;
}

//-------------------------------------------------------------------------------
// Name: OnClose
// Desc: Закрытие программы но только в случае если был Logoff с сервером, а так 
// просто прячем.
//-------------------------------------------------------------------------------
BOOL g_IsClosing = FALSE;
BOOL g_IsClosingByPost = FALSE;
void CMainDlg::OnClose()
{
	if(!g_IsClosingByPost)
	{
		g_IsClosingByPost = TRUE;
		PostMessage(WM_CLOSE, 0, 0);
		return;
	}
	g_IsClosingByPost = FALSE;

TRACE(_T("\r\n CMainDlg::OnClose() 1"));
	if(!g_IsClosing)
	{
TRACE(_T("\r\n CMainDlg::OnClose() 2"));
		g_IsClosing = TRUE;

		DWORD dwStyle = GetStyle();
		if(!(dwStyle&WS_VISIBLE) && !ConnectEnable(FALSE))
		{
TRACE(_T("\r\n CMainDlg::OnClose() 3"));
			if(IsWindow(m_McUpdateWnd))
				::PostMessage(m_McUpdateWnd, WM_CLOSE, 0, 0);
			//m_McUpdateDownload.StopDownload(100);
			KillTimer(IDT_AUTOCHANGETIMER);
			KillTimer(IDT_LOADNOTSENDMESSAGES);
			KillTimer(IDT_MCSCREENCAPTUREEX);

			CloseAllWindows();

			COFSNcDlg2::OnClose();
			DestroyWindow();
			delete this;
			return;
		}
TRACE(_T("\r\n CMainDlg::OnClose() 4"));

		// TODO: Add your message handler code here and/or call default
		if(dwExitInfo != EXIT_ALL)
		{
TRACE(_T("\r\n CMainDlg::OnClose() 5"));
			int ExitValue = 0;

			ShowWindow(SW_SHOWNORMAL);
			SetForegroundWindow();

			if(m_bAutoUpdateExit)
			{
TRACE(_T("\r\n CMainDlg::OnClose() 6"));
				m_MessageBox.SetAutoCloseTime(30);
				ExitValue = m_MessageBox.Show(GetString(IDS_AUTOUPDATE_CLOSE_NAME), MB_YESNO);
				m_MessageBox.SetAutoCloseTime(-1);
				m_bAutoUpdateExit = FALSE;
			}
			else
			{
TRACE(_T("\r\n CMainDlg::OnClose() 7"));
				ExitValue = m_MessageBox.Show(GetString(IDS_DO_CLOSE_NAME), MB_YESNO);
			}

			if(ExitValue == IDYES)
			{
TRACE(_T("\r\n CMainDlg::OnClose() 8"));
				if(IsWindow(m_hWndAddUserDialog))
				{
					::PostMessage(m_hWndAddUserDialog, WM_CLOSE, 0, 0);
				}
				m_hWndAddUserDialog = NULL;

				if(GetStyle()&WS_VISIBLE && GetOptionInt(IDS_OFSMESSENGER, IDS_ANIMATION, FALSE))
					RoundExitAddon(this);

				ShowWindow(SW_HIDE);
				m_MessengerTray.RemoveIcon();
				if(ConnectEnable(FALSE))
				{
TRACE(_T("\r\n CMainDlg::OnClose() 9"));
					dwExitInfo = EXIT_START;
					LogOff();
				}
				else
				{
TRACE(_T("\r\n CMainDlg::OnClose() 10"));
					if(IsWindow(m_McUpdateWnd))
						::PostMessage(m_McUpdateWnd, WM_CLOSE, 0, 0);

					//m_McUpdateDownload.StopDownload(100);
					KillTimer(IDT_AUTOCHANGETIMER);
					KillTimer(IDT_LOADNOTSENDMESSAGES);
					KillTimer(IDT_MCSCREENCAPTUREEX);

					CloseAllWindows();

					COFSNcDlg2::OnClose();
					DestroyWindow();
					delete this;
				}
			}
		}
		else
		{
TRACE(_T("\r\n CMainDlg::OnClose() 11"));
			if(IsWindow(m_McUpdateWnd))
				::PostMessage(m_McUpdateWnd,WM_CLOSE,0,0);

			//m_McUpdateDownload.StopDownload(100);
			KillTimer(IDT_AUTOCHANGETIMER);
			KillTimer(IDT_LOADNOTSENDMESSAGES);
			KillTimer(IDT_MCSCREENCAPTUREEX);

			CloseAllWindows();

			COFSNcDlg2::OnClose();
			DestroyWindow();
			delete this;
		}
TRACE(_T("\r\n CMainDlg::OnClose() 12"));

		g_IsClosing = FALSE;
	}
}

//-------------------------------------------------------------------------------
// Name: OnInetLogin
// Desc: 
//-------------------------------------------------------------------------------
HRESULT CMainDlg::OnInetLogin(WPARAM w, LPARAM l)
{
	m_LastUserStatus			=	S_ONLINE;
	m_bUserDomainMode			=	TRUE;
	
	Login();
	return 0;
}

//-------------------------------------------------------------------------------
// Name: StartStartUpLoad
// Desc: Прежде чем показать программу во всей красе:
// - Подключаем Мессенжер.
// - Кэшируем Данные.
// - Загружаем промосы.
//-------------------------------------------------------------------------------
void CMainDlg::StartStartUpLoad()
{
	if(dwStartUpInfo!=STARTUP_ALL)
	{
		dwStartUpInfo |= STARTUP_START;
		/// Запросить Данные ...
		/// strUserRole = (GetRoleID()==1)?_T("ProductBuyer"):_T("ProductSeller");
		
		// Загрузить список апликейшинов [2/1/2002]
		LoadOptions();
		TRACE(_T("\r\n CMainDlg::StartStartUpLoad (%d)"), dwStartUpInfo);
		CheckAllStartUpWasLoad();
	}
	
}

//-------------------------------------------------------------------------------
// Name: CheckAllStartUpWasLoad
// Desc: Проверяет наличие всех условий необходимых для запуска программы.
//-------------------------------------------------------------------------------
void CMainDlg::CheckAllStartUpWasLoad()
{
	//TRACE(_T("\r\n CMainDlg::CheckAllStartUpWasLoad (%d)",dwStartUpInfo);
	//MCTRACE(9,"[CMainDlg::CheckAllStartUpWasLoad] Code = %04X",dwStartUpInfo);
	
	if(dwStartUpInfo==STARTUP_ALL)
	{
		/// Все предварительная загрузка завершена
		if(m_DlgLog.IsBlock())
		{
			m_DlgLog.UnBlock();
			m_DlgLog.ShowWindow(SW_HIDE);
			
			//m_DlgLog.m_LoginStr    = GetOptionString(IDS_LOGIN,IDS_NICKNAME,"");
			//m_DlgLog.m_PasswordStr = GetOptionString(IDS_LOGIN,IDS_PASSWORD,"");
			//UnPack(m_DlgLog.m_PasswordStr,CString("vTsfO"));
			
			m_DlgLog.UpdateData(FALSE);
			
			if(!m_bSilentMode)
			{
				ShowWindow(SW_SHOWNORMAL);
				SetForegroundWindow();
			}
			else
			{
				m_bSilentMode	=	FALSE;
			}
			
			if(GetOptionInt(IDS_OFSMESSENGER,IDS_KEEPTOP,FALSE))
				SetWindowPos(&wndTopMost,-1,-1,-1,-1,SWP_NOMOVE|SWP_NOSIZE);
			else
				SetWindowPos(&wndTop,-1,-1,-1,-1,SWP_NOMOVE|SWP_NOSIZE);
			
			m_btnMin.ShowWindow(GetOptionInt(IDS_OFSMESSENGER,IDS_MINIMIZE_ONCLOSE,TRUE)?SW_HIDE:SW_SHOW);
			
			LoadUserProfile();
			UnlockShowInfo();
		}
	}
}

void CMainDlg::CheckExit()
{
	if(dwExitInfo == EXIT_ALL)
		PostMessage(WM_CLOSE);
	//		COFSNcDlg2::OnClose();
}

//-------------------------------------------------------------------------------
// Name: CreateMessageDialog
// Desc: Показывает Информационное окно сообщения.
//-------------------------------------------------------------------------------
void CMainDlg::CreateMessageDialog(bstr_t &Path)
{
/*
LoadSkins m_LoadSkin;
CPaintDC dc(this);

  IStreamPtr m_Stream = NULL;
  long ErrorCode = 0;
  m_LoadSkin.Load(bstr_t(Path+L"/Main/border.bmp"),&m_Stream,&ErrorCode);
  if(m_Stream)
  {
		CDib m_Dib(m_Stream);
		m_MessageBox.SetBoundaryBMP(m_Dib.GetHBITMAP(dc));
		}
		
	*/
}

HRESULT CMainDlg::OnShowMessageBox(WPARAM w, LPARAM l)
{
	m_MessageBox.Show(GetString(UINT(w)));
	return 0;
}

HRESULT CMainDlg::OnShowAddUser(WPARAM w, LPARAM l)
{
	if(IsWindow(m_hWndAddUserDialog))
	{
		CMcWindowAgent winAgent(m_hWndAddUserDialog);
		winAgent.ShowWindow(SW_SHOWNORMAL);
		winAgent.SetForegroundWindow();
	}
	else
	{
		CWebWindow		*m_pShowAddUser = new CWebWindow;
		
		CString strShowAddUserUrl;
		strShowAddUserUrl.Format(CString(IBN_SCHEMA)+CString(_T("/%s/Browser/Find/index.html")),::GetCurrentSkin());
		
		CRect winRect;
		GetWindowRect(&winRect);
		
		m_pShowAddUser->CreateAutoKiller(_T("/Browser/Find/skin.xml"), this,GetDesktopWindow(),winRect.left-150,winRect.top+20,500,300,GetString(IDS_INSTANT_DIRECTORY_NAME),strShowAddUserUrl,FALSE,FALSE,TRUE,IDS_FINDUSER);
		m_hWndAddUserDialog = m_pShowAddUser->GetSafeHwnd();
	}
	
	return 0;
}

//-------------------------------------------------------------------------------
// Name: OnDnlClkTray
// Desc: 
//-------------------------------------------------------------------------------

void CMainDlg::OnDnlClkTray()
{
	if(dwStartUpInfo==STARTUP_ALL  &&
		dwExitInfo==EXIT_NONE)
	{
		if(OnCheckSignalState(WM_CHECK_SIGNAL_STATE,0))//SendMessage(WM_CHECK_SIGNAL_STATE))
		{
			// Reconect [2/27/2002]
			if(!ConnectEnable(FALSE))
			{
				OnStatusOnline();
			}
		}
		else
			Invalidate();
	}
}

void CMainDlg::OnLClkTray()
{
	if(dwStartUpInfo==STARTUP_ALL  &&
		dwExitInfo==EXIT_NONE)
	{
		if(OnCheckSignalState(WM_CHECK_SIGNAL_STATE,0))//SendMessage(WM_CHECK_SIGNAL_STATE))
		{
			BOOL bVisible = GetStyle()&WS_VISIBLE;
			if(bVisible)
			{
				if(IsIconic())
					ShowWindow(SW_RESTORE);
				else
					ShowWindow(SW_SHOWNORMAL);
				SetForegroundWindow();
			}
			else
			{
				ShowWindow(SW_SHOWNORMAL);
			}
		}
		else
			Invalidate();
	}
	else
	{
		m_DlgLog.ShowWindow(SW_SHOWNORMAL);
	}
}

//-------------------------------------------------------------------------------
// Name: SetInetOption
// Desc: Установить опции по умолчанию.
//-------------------------------------------------------------------------------
void CMainDlg::SetInetOption()
{
	INTERNET_PROXY_INFO m_ProxyInfo;
	m_ProxyInfo.dwAccessType    = (DWORD)GetOptionInt(IDS_NETOPTIONS,IDS_ACCESSTYPE,INTERNET_OPEN_TYPE_PRECONFIG);
	
	CString strProxy = GetOptionString(IDS_NETOPTIONS, IDS_PROXYNAME, _T("")) + _T(":") + GetOptionString(IDS_NETOPTIONS, IDS_PROXYPORT, _T(""));
	m_ProxyInfo.lpszProxy       = (LPCTSTR)strProxy;
	m_ProxyInfo.lpszProxyBypass = (GetOptionInt(IDS_LOGIN,IDS_BYPASSENABLE,FALSE))?((LPCTSTR)_T("<local>")):((LPCTSTR)_T(""));
	
	BOOL bFlag  = InternetSetOption(NULL,INTERNET_OPTION_PROXY,&m_ProxyInfo,sizeof(INTERNET_PROXY_INFO));
	
    HRESULT hr = UrlMkSetSessionOption(INTERNET_OPTION_PROXY,&m_ProxyInfo,sizeof(INTERNET_PROXY_INFO),0);
}

//-------------------------------------------------------------------------------
// Name: OnChangeNewMessage
// Desc: Меняет число мессаг. Для Мигания в Трэе если больше 0. 
//-------------------------------------------------------------------------------
HRESULT CMainDlg::OnChangeNewMessage(WPARAM w, LPARAM l)
{
	long NewMessageItem = (long)w;
	if(NewMessageItem)
	{
		m_MessengerTray.StopAnimation();
		m_MessengerTray.SetIconList(IDI_NEWMESSAGE,IDI_EMPTY);
		m_MessengerTray.Animate(300);
	}
	else
	{
		m_MessengerTray.StopAnimation();
		m_MessengerTray.SetIcon(m_StatusImageList.ExtractIcon(GetUserStatus() + (IsSSLMode()?7:0)));
	}
	return 0;
}

void CMainDlg::OnTrayExit() 
{
	m_DlgLog.PostMessage(WM_CLOSE);
}

void CMainDlg::OnTrayPreferences() 
{
	PreferenceDlg(this);
}

void CMainDlg::OnTrayReportBug() 
{
#ifndef RADIUS
	CString strUrl = _T("http://feedback.mediachase.com");
#else
	CString strUrl = _T("http://www.radius-group.ru");
#endif

	if(S_OK!=NavigateNewWindow(NULL,strUrl))
	{
			ShellExecute(::GetDesktopWindow(), _T("open"), strUrl, NULL, NULL, SW_SHOWDEFAULT);
	}
}


void CMainDlg::OnStatusOffline() 
{
	if(ConnectEnable(FALSE))
	{
		dwStartUpInfo	= STARTUP_NONE;
		dwExitInfo = EXIT_LOGOFF;
		ShowWindow(SW_HIDE);
		m_DlgLog.ShowWindow(SW_SHOWNORMAL);
		m_DlgLog.UpdateWindow();
		m_DlgLog.Block();
		m_DlgLog.SetForegroundWindow();
		m_DlgLog.SetFocus();
		LogOff();
	}
	else
	{
		dwStartUpInfo = STARTUP_NONE;
		dwExitInfo = EXIT_NONE;
		ShowWindow(SW_HIDE);
		m_DlgLog.ShowWindow(SW_SHOWNORMAL);
		m_DlgLog.UpdateWindow();
		m_DlgLog.UnBlock();
		m_DlgLog.SetForegroundWindow();
		m_DlgLog.SetFocus();
		LogOff();
	}
	
	if(IsWindow(m_hWndAddUserDialog))
	{
		::PostMessage(m_hWndAddUserDialog,WM_CLOSE,0,0);
	}
	m_hWndAddUserDialog = NULL;
}

void CMainDlg::OnUpdateStatusOffline(CCmdUI* pCmdUI) 
{
	UpdateStatus(S_OFFLINE,pCmdUI);
}

void CMainDlg::OnCaptureChanged(CWnd *pWnd) 
{
	CRect rWindow;
	GetWindowRect(&rWindow);
	if(!IsIconic())
		WriteOptionString(IDS_OFSMESSENGER,IDR_MAINFRAME,RectToString(rWindow));
	
	COFSNcDlg2::OnCaptureChanged(pWnd);
}

LRESULT CMainDlg::OnUpdateExit(WPARAM w, LPARAM l)
{
	m_bAutoUpdateExit = TRUE;
	PostMessage(WM_CLOSE);
	return 0;
}

void CMainDlg::Invoke_StartAutoUpdate(LPCTSTR strUpdateFileUrl)
{
	//MCTRACE(9,"Invoke_StartAutoUpdate %s",strUpdateFileUrl);
	// Format "McUpdateUrl#BuildId#MCIUrl" [12/22/2001]
	CString strFullUrl =	strUpdateFileUrl, strMCIUrl	=	_T(""), 
		strMcUpdateURL	=	_T(""), strBuild	=	_T(""), 
		strDirPath = _T("") ;
	
	int DelimeterPos =  strFullUrl.Find(_T('#'),0);
	
	if(DelimeterPos!=-1)
	{
		strMcUpdateURL = strFullUrl.Left(DelimeterPos);
		strBuild = strFullUrl.Mid(DelimeterPos+1);
		
		DelimeterPos =  strBuild.Find(_T('#'),0);
		
		if(DelimeterPos!=-1)
		{
			strMCIUrl = strBuild.Mid(DelimeterPos+1);
			strBuild = strBuild.Left(DelimeterPos);
		}
		else
		{
			//MCTRACE(9,"Invoke_StartAutoUpdate Faile #2 (DelimeterPos!=-1)");
			ASSERT(FALSE);
			return;
		}
	}
	else
	{
		//MCTRACE(9,"Invoke_StartAutoUpdate Faile #1 (DelimeterPos!=-1)");
		ASSERT(FALSE);
		return;
	}
	
	// Modified by Oleg Zhuk [6/4/2004]
	// Just Navigate to <PORTAL>/public/update.aspx?ProductGuid= [6/4/2004]
	
	// Load Product GUID [3/26/2004]
	CString strUpdateUrl;
	
	CString	ProductGUID;
#ifndef RADIUS
	McRegGetString(HKEY_LOCAL_MACHINE,_T("SOFTWARE\\Mediachase\\Instant Business Network\\4.5\\Client"),_T("ProductGUID"),ProductGUID,_T(""));
#else
	McRegGetString(HKEY_LOCAL_MACHINE,_T("SOFTWARE\\Radius-Soft\\MagRul\\4.5\\Client"),_T("ProductGUID"),ProductGUID,_T(""));
#endif
	
	// ToDo: Remember Uncoment [3/2/2002]
	strUpdateUrl.Format(_T("%s/public/update.aspx?ProductGUID=%s"),GetWebHOST(),ProductGUID);
	
	if(m_InWindow.NavigateNewWindow(strUpdateUrl)!=S_OK)
		ShellExecute(NULL,_T("open"),strUpdateUrl,NULL,NULL,SW_SHOWNORMAL);
}

LRESULT CMainDlg::OnMcUpdateLoaded(WPARAM w, LPARAM l)
{
/*if(w)
{
if(AfxMessageBox(_T("McUpdate component download Error. Try Again?",MB_YESNO))==IDYES)
{
m_McUpdateDownload.Clear();
m_McUpdateDownload.Load(NULL);
}
}
else
{
m_McUpdateDownload.Save(m_strMcUpdatePath);
m_McUpdateDownload.Clear();
ShellExecute(::GetDesktopWindow(),_T("open"),m_strMcUpdatePath,m_strMcUpdateParam,NULL,SW_SHOWDEFAULT);
}*/
	
	return 0;
}

LRESULT CMainDlg::OnInvokeStartAutoUploader(WPARAM w, LPARAM l)
{
	_bstr_t Url;Url.Assign((BSTR)w);
	Invoke_StartAutoUpdate((LPCTSTR)Url);
	return 0;
}

LRESULT CMainDlg::OnUpdateContactList(WPARAM w, LPARAM l)
{
	//TRACE(_T("\r\n CMainDlg::OnUpdateContactList (%d)",dwStartUpInfo);
	if(dwStartUpInfo!=STARTUP_ALL)
	{
		dwStartUpInfo|=STARTUP_CONTACTLIST_LOAD;
		TRACE(_T("\r\n CMainDlg::OnUpdateContactList (%d)"), dwStartUpInfo);
		CheckAllStartUpWasLoad();
	}
	return 0;
}

void CMainDlg::OnTimer(UINT nIDEvent) 
{
	if(nIDEvent==IDT_AUTOCHANGETIMER)
	{
		if(ConnectEnable(FALSE))
		{
			// Check User Activety [3/18/2004]
			
			// Check Cursor
			CPoint tmpPoint;
			GetCursorPos(&tmpPoint);
			
			if(m_oldCursorPos==tmpPoint)
			{
				if(GetUserStatus()!=S_INVISIBLE&&GetUserStatus()!=S_NA)
				{
					m_AutoChangeStatusTimeLast++;
					
					if(GetOptionInt(IDS_OFSMESSENGER, IDS_SETAWAY_ENABLE, TRUE))
					{
						if(m_AutoStatus.IsReset())
						{
							if(m_AutoChangeStatusTimeLast >= (GetOptionInt(IDS_OFSMESSENGER,IDS_SETAWAY,5)*6))
							{
								m_AutoChangeStatusTimeLast = 0;
								
								m_AutoStatus.SetAway(GetUserStatus());
								
								PostMessage(WM_CHANGE_STATUS,S_AWAY);
							}
						}
						else if(GetOptionInt(IDS_OFSMESSENGER, IDS_SETNA_ENABLE, FALSE) && !m_AutoStatus.IsAutoNA())
						{
							if(m_AutoChangeStatusTimeLast >= (GetOptionInt(IDS_OFSMESSENGER,IDS_SETNA,10)*6))
							{
								m_AutoChangeStatusTimeLast = 0;
								
								m_AutoStatus.SetNA(GetUserStatus());
								
								PostMessage(WM_CHANGE_STATUS,S_NA);
							}
						}
					}
				}
			}
			else
			{
				m_oldCursorPos=tmpPoint;
				
				if(!m_AutoStatus.IsReset())
					PostMessage(WM_CHANGE_STATUS,m_AutoStatus.OldUserStatus);
				
				ResetAutoChangeStatusTime();
			}
			
		}
		else if(dwStartUpInfo==STARTUP_ALL)
		{
			// Try Auto Reconnect [3/18/2004]
			TRACE1("\r\nReconnect Last User Status = %d",m_LastUserStatus);
			
			static long TimeRepeatCount = 0;
			TimeRepeatCount++;
			if(TimeRepeatCount>=6)
			{
				TimeRepeatCount=0;
				if(!m_AutoStatus.IsReset())
					PostMessage(WM_CHANGE_STATUS,m_AutoStatus.State);		
				else
					PostMessage(WM_CHANGE_STATUS,m_LastUserStatus);
			}
		}
		/*
		if(ConnectEnable(FALSE))
		{
		CPoint tmpPoint;
		GetCursorPos(&tmpPoint);
		
		  if(m_oldCursorPos==tmpPoint&&GetUserStatus()!=S_INVISIBLE)
		  {
		  if(GetOptionInt(IDS_OFSMESSENGER,IDS_SETAWAY_ENABLE,TRUE))
		  {
		  if(GetUserStatus()!=S_AWAY&&GetUserStatus()!=S_NA)
		  {
		  m_AutoChangeStatusTimeLast++;
		  if(m_AutoChangeStatusTimeLast>=(GetOptionInt(IDS_OFSMESSENGER,IDS_SETAWAY,5)*6))
		  {
		  if(m_OldStatus==-1)
		  m_OldStatus = GetUserStatus();
		  
			PostMessage(WM_CHANGE_STATUS,S_AWAY);		
			m_AutoChangeStatusTimeLast = 0;
			
			  }
			  }
			  else if((GetUserStatus()==S_AWAY)&&
			  GetOptionInt(IDS_OFSMESSENGER,IDS_SETNA_ENABLE,FALSE)&&GetUserStatus()!=S_NA)
			  {
			  m_AutoChangeStatusTimeLast++;
			  if(m_AutoChangeStatusTimeLast>=(GetOptionInt(IDS_OFSMESSENGER,IDS_SETNA,10)*6))
			  {
			  if(m_OldStatus==-1)
			  m_OldStatus = GetUserStatus();
			  
				PostMessage(WM_CHANGE_STATUS,S_NA);		
				m_AutoChangeStatusTimeLast = 0;
				}
				}
				}
				}
				else
				{
				m_oldCursorPos=tmpPoint;
				
				  if(m_OldStatus!=-1)
				  {
				  m_LastUserStatus	=	m_OldStatus;
				  PostMessage(WM_CHANGE_STATUS,m_OldStatus);		
				  m_OldStatus = -1;
				  }
				  
					ResetAutoChangeStatusTime();
					}
					}
					else if(dwStartUpInfo==STARTUP_ALL)
					{
					
					  TRACE1("\r\nReconnect Last User Status = %d",m_LastUserStatus);
					  static long TimeRepeatCount = 0;
					  TimeRepeatCount++;
					  if(TimeRepeatCount>=6)
					  {
					  TimeRepeatCount=0;
					  PostMessage(WM_CHANGE_STATUS,m_LastUserStatus);		
					  }
					  }
		*/
	}
	else if(nIDEvent==IDT_LOADNOTSENDMESSAGES)
	{
		TRACE0("\r\n CMainDlg::OnTimer nIDEvent==IDT_LOADNOTSENDMESSAGES");
		if(ConnectEnable(FALSE))
			LoadNotSendedMessage();

		// OZ 2009-03-04 AutoUpdateCheck
		CheckAutoUpdate();

	}
	else if(nIDEvent==IDT_MCSCREENCAPTUREEX)
	{
		//if(m_MainDlgLock.Lock(100))
		{
			int Size = m_ActiveExCaptureList.GetSize();
			for(int i=Size-1;i>=0;i--)
			{
				try
				{
					LPMcScreenCaptureItem	pItem	=	m_ActiveExCaptureList[i];
					
					long lStatus = 0;
					pItem->Ptr->GetCompletionStatus(&lStatus);
					if(lStatus==-1)
					{
						//IMcScreenCaptureItem*	Item = m_ActiveExCaptureList[i];
						m_ActiveExCaptureList.RemoveAt(i);
						delete pItem;
						//Item->Release();
					}
					else if(lStatus==6)
					{
						USES_CONVERSION;
						
						CComBSTR	FileName;
						CComBSTR	TreeXML;
						// Process Action [7/15/2004]
						long lBtnState = -1;
						pItem->Ptr->GetButtonsState(&lBtnState);
						pItem->Ptr->GetFileName(&FileName);
						pItem->Ptr->GetXML(&TreeXML);
						
						CString strTmpFileName = W2CT(FileName);
						
						CString	strAllIBNUserList = GetSelectedUserStringFromXML(TreeXML);
						
						//CUserCollection	ContactList;
						//CreateScreenShotUsers(pItem->UserId,pItem->GroupName,ContactList);
						
						switch(lBtnState)
						{
						case 1: //IDS_CAPTURE_ACTION_SENDTO
							{
								if(ConnectEnable())
								{
									CString	strRecepientShowList;
									
									CString strUserId	=	_T(",")+strAllIBNUserList;
									strUserId += ",";
									if(POSITION pos = m_ContactList.InitIteration())
									{
										CUser	*pUser =	 NULL;
										while(m_ContactList.GetNext(pos,pUser))
										{
											CString strCurrUserId;
											strCurrUserId.Format(_T(",%d,"),pUser->GetGlobalID());
											
											if(strUserId.Find(strCurrUserId)==0)
											{
											/*TCHAR Buff[MAX_PATH];
											_ltoa(pUser->GetGlobalID(),Buff,10);
											strUserId += Buff;
												strUserId += ",";*/
												
												strRecepientShowList	+=	pUser->GetShowName();
												strRecepientShowList += ",";
											}
										}
										
										if(!strUserId.IsEmpty())
										{
											//CString strTmpFileName	=	SaveImageToTmpFolder();
											AddToUpload(strTmpFileName,strRecepientShowList,strUserId,GetString(IDS_SCREEN_CAPTURE_NAME));
										}
									}
									strUserId	=	strAllIBNUserList;
								}
							}
							break;
						case 2: //IDS_CAPTURE_ACTION_CREATEISSUE
							{
								//CString strTmpFileName	=	SaveImageToTmpFolder();
								CString	strAllIBNUsers	=	strAllIBNUserList;
								
								/*if(POSITION pos = ContactList.InitIteration())
								{
								CUser	*pUser =	 NULL;
								while(ContactList.GetNext(pos,pUser))
								{
								if(pUser->m_bHasNewMessages)
								{
								CString strTmp;
								if(strAllIBNUsers.GetLength()>0)
								strAllIBNUsers	+=	",";
								strTmp.Format("%d",pUser->GetGlobalID());
								strAllIBNUsers	+=	strTmp;
								}
								}
							}*/
								
								CString strParametrs;
								
								if(strAllIBNUsers.GetLength()>0)
								{
									strParametrs.Format(_T("/CREATEINCIDENT /L \"%s\" /P \"%s\" /IBNRESOURCES \"%s\" \"%s\""),m_DlgLog.m_LoginStr, m_DlgLog.m_PasswordStr,strAllIBNUsers,strTmpFileName);
								}
								else
								{
									strParametrs.Format(_T("/CREATEINCIDENT /L \"%s\" /P \"%s\" \"%s\""),m_DlgLog.m_LoginStr, m_DlgLog.m_PasswordStr,strTmpFileName);
								}
								
								if(m_bIsSSLMode)
									strParametrs	+= _T(" /USESSL");
								
								HINSTANCE retVal = ShellExecute(::GetDesktopWindow(),NULL,ToolboxPath(),strParametrs,NULL,SW_SHOWNORMAL);
							}
							break;
						case 3:  //IDS_CAPTURE_ACTION_ASSIGNTODO
							{
								//CString strTmpFileName	=	SaveImageToTmpFolder();
								CString	strAllIBNUsers	=	strAllIBNUserList;
								
								/*if(POSITION pos = ContactList.InitIteration())
								{
								CUser	*pUser =	 NULL;
								while(ContactList.GetNext(pos,pUser))
								{
								if(pUser->m_bHasNewMessages)
								{
								CString strTmp;
								if(strAllIBNUsers.GetLength()>0)
								strAllIBNUsers	+=	",";
								strTmp.Format("%d",pUser->GetGlobalID());
								strAllIBNUsers	+=	strTmp;
								}
								}
							}*/
								
								CString strParametrs;
								
								if(strAllIBNUsers.GetLength()>0)
								{
									strParametrs.Format(_T("/CREATETODO /L \"%s\" /P \"%s\" /IBNRESOURCES \"%s\" \"%s\""),m_DlgLog.m_LoginStr, m_DlgLog.m_PasswordStr,strAllIBNUsers,strTmpFileName);
								}
								else
								{
									strParametrs.Format(_T("/CREATETODO /L \"%s\" /P \"%s\" \"%s\""),m_DlgLog.m_LoginStr, m_DlgLog.m_PasswordStr,strTmpFileName);
								}
								
								if(m_bIsSSLMode)
									strParametrs	+= _T(" /USESSL");
								
								HINSTANCE retVal = ShellExecute(::GetDesktopWindow(),NULL,ToolboxPath(),strParametrs,NULL,SW_SHOWNORMAL);
							}
							break;
						case 4://IDS_CAPTURE_ACTION_PUBLISH
							{
								//CString strTmpFileName	=	SaveImageToTmpFolder();
								
								CString strParametrs;
								
								strParametrs.Format(_T("/UPLOAD /L \"%s\" /P \"%s\" \"%s\""),m_DlgLog.m_LoginStr, m_DlgLog.m_PasswordStr,strTmpFileName);
								
								if(m_bIsSSLMode)
									strParametrs	+= _T(" /USESSL");
								
								HINSTANCE retVal = ShellExecute(::GetDesktopWindow(),NULL,ToolboxPath(),strParametrs,NULL,SW_SHOWNORMAL);
							}
							break;
						}
						
						m_ActiveExCaptureList.RemoveAt(i);
						delete pItem;
					}
				}
				catch(_com_error&)
				{
					m_ActiveExCaptureList.RemoveAt(i);
				}
			}
			//m_MainDlgLock.Unlock();
		}
	}
	
	COFSNcDlg2::OnTimer(nIDEvent);
}

long CMainDlg::ResetAutoChangeStatusTime()
{
	long lTmp = m_AutoChangeStatusTimeLast;
	m_AutoChangeStatusTimeLast = 0;
	
	m_AutoStatus.Reset();
	
	return lTmp;
}

HRESULT CMainDlg::OnCancelLogin(WPARAM w, LPARAM l)
{
	CancelLogon();
	return 0;
}

LRESULT CMainDlg::OnLoadAppComleted(WPARAM w, LPARAM l)
{
	if(m_pAppList)
	{
		// Load to McMessenger App List [2/1/2002]
		CComPtr<IXMLDOMDocument>	pDocNew	=	NULL, pDocOldXML	=	NULL;
		
		if(SUCCEEDED(w))
		{
			CComBSTR	bsXML;
			m_pAppList->GetXML(&bsXML);
			
			pDocNew.CoCreateInstance(CLSID_DOMDocument40, NULL, CLSCTX_INPROC_SERVER);
			
			if(pDocNew!=NULL)
			{
				VARIANT_BOOL	varSuc	=	VARIANT_FALSE;
				// Step 0. Load New XML [3/2/2002]
				pDocNew->loadXML(bsXML,&varSuc);
				// Step 1. Load Old XML [3/2/2002]
				CString strOldXML = GetRegFileText(GetString(IDS_INFO)+_T("\\")+GetUserRole(),_T(""));
				
				if(strOldXML.IsEmpty())
				{
					pDocOldXML = pDocNew;
				}
				else
				{
					pDocOldXML.CoCreateInstance(CLSID_DOMDocument40, NULL, CLSCTX_INPROC_SERVER);
					
					if(SUCCEEDED(pDocOldXML->loadXML(CComBSTR((LPCTSTR)strOldXML),&varSuc))&&varSuc==VARIANT_TRUE)
					{
						
						// Step 2. Combinate Old XML and New XML to New XML [3/2/2002]
						CComPtr<IXMLDOMNodeList>	pSrcOptionsNodeList	=	NULL;
						pDocNew->selectNodes(CComBSTR(L"options/*"),&pSrcOptionsNodeList);
						
						if(pSrcOptionsNodeList)
						{
							CComPtr<IXMLDOMNode>	pDestOptionsItem	=	NULL;
							pDocOldXML->selectSingleNode(CComBSTR(L"options"),&pDestOptionsItem);
							
							if(pDestOptionsItem!=NULL)
							{
								// Remove Version [3/7/2002]
								CComPtr<IXMLDOMNode>	pDestVersionsItem	=	NULL;
								pDestOptionsItem->selectSingleNode(CComBSTR(L"versions"),&pDestVersionsItem);
								if(pDestVersionsItem)
									pDestOptionsItem->removeChild(pDestVersionsItem,NULL);
								
								long ListLength = 0;
								pSrcOptionsNodeList->get_length(&ListLength);
								
								for(long i=0;i<ListLength;i++)
								{
									CComPtr<IXMLDOMNode>	pSrcItem	=	NULL;
									CComPtr<IXMLDOMNode>	pDestItem	=	NULL;
									pSrcOptionsNodeList->get_item(i,&pSrcItem);
									if(pSrcItem)
									{
										CComPtr<IXMLDOMNode>	pSrcCloneItem	=	NULL;
										pSrcItem->cloneNode(VARIANT_TRUE,&pSrcCloneItem);
										
										CComBSTR	bstSrcNodeName;
										pSrcCloneItem->get_nodeName(&bstSrcNodeName);
										
										pDestOptionsItem->selectSingleNode(bstSrcNodeName,&pDestItem);
										
										if(pDestItem)
										{
											CComPtr<IXMLDOMNode>	pDestParentNode	=	NULL;
											pDestItem->get_parentNode(&pDestParentNode);
											pDestParentNode->replaceChild(pSrcCloneItem,pDestItem,NULL);
										}
										else
										{
											pDestOptionsItem->appendChild(pSrcCloneItem,NULL);
										}
									}
								}
							}
						}
						else
						{
							pDocOldXML = pDocNew;
						}
					}
					else
					{
						pDocOldXML = pDocNew;
					}
				}
				// Step 3. Save XML to Reg [3/2/2002]
				CComBSTR	bstrOldXML;
				pDocOldXML->get_xml(&bstrOldXML);
				strOldXML = bstrOldXML;
				
				SetRegFileText(GetString(IDS_INFO)+_T("\\")+GetUserRole(),_T(""),strOldXML);
				
				LoadAppList(pDocOldXML);
			}
		}
		
		delete m_pAppList;
		m_pAppList = NULL;
	}
	
	if(dwStartUpInfo!=STARTUP_ALL)
	{
		dwStartUpInfo |= STARTUP_LOADAPP_LOAD;
		
		
		
		TRACE(_T("\r\n CMainDlg::OnLoadAppComleted (%d)"), dwStartUpInfo);
		CheckAllStartUpWasLoad();
	}
	
	return 0;
}

void CMainDlg::ShowIBNActions()
{
	int m_lDefaultView = 1; //GetOptionInt(IDS_OFSMESSENGER,IDS_VIEWMODE,1);
	if(m_lDefaultView==0)
	{
		for(int itemIndex=0;itemIndex < m_AppArray.GetSize();itemIndex++)
		{
			McAppItem Item = m_AppArray[itemIndex];
/*			if(Item.Type==APPT_IBN_ACTIONS)
			{
				m_AppBar.m_AppCtrl.SetCheckButton(itemIndex);
				
				m_chatbox.ShowWindow(SW_HIDE);
				m_treebox.ShowWindow(SW_HIDE);
				m_InWindow.ShowWindow(SW_SHOW);
				m_InWindow.SetFocus();
				m_InWindow.Navigate(Item.Url);
				
				break;
			}*/
		}
	}
	
}

void CMainDlg::LoadOptions()
{
	if(m_pAppList)
	{
		delete m_pAppList;
		m_pAppList = NULL;
	}
	
	// Load Ol [3/2/2002]
	
	m_pAppList = new CXmlDoc;
	
	m_pAppList->Init(GetSafeHwnd(),WM_LOADAPP_COMPLETED);
	
	CString strAppListUrl;
	
	// Step 1. Get Build [3/2/2002]
	CMcVersionInfo	verInfo;
	long			lStubsId = -1, lLogoId = -1;
	
	CString strOldXML = GetRegFileText(GetString(IDS_INFO)+_T("\\")+GetUserRole(),_T(""));
	if(!strOldXML.IsEmpty())
	{
		CComPtr<IXMLDOMDocument>	pDoc		=	NULL;
		pDoc.CoCreateInstance(CLSID_DOMDocument40, NULL, CLSCTX_INPROC_SERVER);
		CComBSTR	bsXML = strOldXML;
		VARIANT_BOOL	varLoad	=	VARIANT_FALSE;
		if(SUCCEEDED(pDoc->loadXML(bsXML,&varLoad))&&varLoad==VARIANT_TRUE)
		{
			CComPtr<IXMLDOMNode>	pStubsNode	=	NULL;
			pDoc->selectSingleNode(CComBSTR(L"options/stubs"),&pStubsNode);
			if(pStubsNode)
			{
				// Step 2. Get Stubs Id [3/2/2002]
				CComVariant	varData;
				GetAttribute(pStubsNode,CComBSTR(L"version"),&varData);				
				varData.ChangeType(VT_I4);
				lStubsId = varData.lVal;
			}
			// Step 3. Get LogoId Id [3/2/2002]
			CComPtr<IXMLDOMNode>	pLogoNode	=	NULL;
			pDoc->selectSingleNode(CComBSTR(L"options/logos"),&pLogoNode);
			if(pLogoNode)
			{
				// Step 2. Get Stubs Id [3/2/2002]
				CComVariant	varData;
				GetAttribute(pLogoNode,CComBSTR(L"version"),&varData);				
				varData.ChangeType(VT_I4);
				lLogoId = varData.lVal;
			}
			
			if(dwStartUpInfo!=STARTUP_ALL)
			{
				LoadAppList(pDoc,TRUE);
				dwStartUpInfo |= STARTUP_LOADAPP_LOAD;
				
				// Add IBN Actions View [12/15/2003]
				//ShowIBNActions();
				
				TRACE(_T("\r\n CMainDlg::OnLoadAppComleted (%d)"), dwStartUpInfo);
				CheckAllStartUpWasLoad();
			}
		}
	}
	else
		LoadAppList(NULL,TRUE);
	
	// Load Product GUID [3/26/2004]
	CString	ProductGUID;
#ifndef RADIUS
	McRegGetString(HKEY_LOCAL_MACHINE,_T("SOFTWARE\\Mediachase\\Instant Business Network\\4.5\\Client"),_T("ProductGUID"),ProductGUID,_T(""));
#else
	McRegGetString(HKEY_LOCAL_MACHINE,_T("SOFTWARE\\Radius-Soft\\MagRul\\4.5\\Client"),_T("ProductGUID"),ProductGUID,_T(""));
#endif
	
	// ToDo: Remember Uncoment [3/2/2002]
	strAppListUrl.Format(GetString(IDS_WEB_OPTIONS),GetServerPath(),GetSID(),lStubsId,lLogoId,(verInfo.GetProductVersionLS()&0xffff0000)>>16,ProductGUID);
	
	m_pAppList->Load(strAppListUrl);
}

int CMainDlg::GetLastUserStatus()
{
	return m_LastUserStatus;
}

LRESULT CMainDlg::OnShowLoginDlg(WPARAM w, LPARAM l)
{
	OnStatusOffline();
	return 0;
}


LRESULT CMainDlg::OnUploadAppFile(WPARAM w, LPARAM l)
{
	LPCTSTR	strXML	=	(LPCTSTR)w;
	
	UploadAppFile(strXML);
	
	return 0;
}

//////////////////////////////////////////////////////////////////////////
// CMessenger Part [4/5/2002]

//-------------------------------------------------------------------------------
// Name: Login2
// Desc: Первой версии в данном проекте нет, так что не пугайтесь. 
// Отправляет запрос Logon на сервер. Сохраняет Login и Password во внешних 
// переменных, настраивает конфигурацию сети для подключения к серверу, 
// т.о. изменение настроек возможно только при очередном вызове.
//-------------------------------------------------------------------------------
void CMainDlg::Login2(int SetStatus)
{
	TCHAR  str[256];
   	GetEnvironmentVariable(_T("MpaUserLogin"), str,255);
	m_User.m_strLogin  = str;
    GetEnvironmentVariable(_T("MpaUserPassword"),str,255);
	m_strPassword      = str;
	m_User.m_iStatus = SetStatus;
	
	ConfigureNetwork();
	CString strTitle;
	strTitle.Format(GetString(IDS_MAIN_TITLE_FORMAT),m_User.GetShowName());
	SetWindowText(strTitle);
	
	//ClearAll();
	
	if(m_User.m_strLogin.IsEmpty() || m_strPassword.IsEmpty())
	{
		//		if(!PopupLoginAndPassword())
		//			return;
	}
	
	try
	{
		m_lLogonHandle = pSession->LogOn(bstr_t(m_User.m_strLogin),bstr_t(m_strPassword),LONG(m_User.m_iStatus));
	}
	catch(...)
	{
		ASSERT(FALSE);
	}
}

//-------------------------------------------------------------------------------
// Name: ConfigureNetwork
// Desc: Делает настройку интернета перед конектом к серверу, если нет данных 
// в реестре то загружает из скина, иначе остаются настройки по умолчанию.
//-------------------------------------------------------------------------------
BOOL CMainDlg::ConfigureNetwork()
{
	try
	{
		IConfigPtr pConfig = pSession->GetConfig();
		
		/************************************************************************/
		/* Load Settings from Local Machine                                     */
		/************************************************************************/
		HKEY	hNetConfigKey	=	NULL;
		CString strSubKeyName;

#ifndef RADIUS
		strSubKeyName = _T("SOFTWARE\\Mediachase\\Instant Business Network\\4.5\\Client\\NetOptions");
#else
		strSubKeyName = _T("SOFTWARE\\Radius-Soft\\MagRul\\4.5\\Client\\NetOptions");
#endif

		if(RegOpenKeyEx(HKEY_CURRENT_USER,strSubKeyName,0,KEY_READ,&hNetConfigKey)==ERROR_SUCCESS)
		{
			// Enum All Server [6/26/2002]
			CSelectServer	SelectServerDlg;
			
			SelectServerDlg.m_ServerNameArr.Add(GetString(IDS_DEFAULT_NAME));
			
			DWORD iIndex	=		0;
			while (true) 
			{
				TCHAR	strDirName[MAX_PATH+1];
				if(RegEnumKey(hNetConfigKey,iIndex,strDirName,MAX_PATH+1)!=ERROR_SUCCESS)
					break;
				
				SelectServerDlg.m_ServerNameArr.Add(strDirName);
				iIndex++;
			}
			
			if(SelectServerDlg.m_ServerNameArr.GetSize()>1)
			{
				static CString strLastConnect	=	GetString(IDS_DEFAULT_NAME);
				
				if(dwStartUpInfo!=STARTUP_ALL)
				{
					if(SelectServerDlg.DoModal()==IDOK)
					{
						strLastConnect = SelectServerDlg.m_strServerName;
					}
				}
				
				if(strLastConnect!=GetString(IDS_DEFAULT_NAME))
				{
					HKEY hTmpNetConfigKey;
					if(RegOpenKeyEx(hNetConfigKey,strLastConnect,0,KEY_READ,&hTmpNetConfigKey)==ERROR_SUCCESS);
					{
						RegCloseKey(hNetConfigKey);
						hNetConfigKey	=	hTmpNetConfigKey;
					}
				}
			}
			
			//  [6/26/2002]
			//BOOL	bIsSSLMode			=	FALSE;
			TCHAR	strData[MAX_PATH]	=	_T("");
			DWORD	lDataSize			=	MAX_PATH;
			DWORD	dwType;
			//long	V					=	0;	
			
			
			m_bIsSSLMode	=	FALSE;
			
			if(RegQueryValueEx(hNetConfigKey,GetString(IDS_USESSL),0,&dwType,(LPBYTE)strData,&lDataSize)==ERROR_SUCCESS)
			{
				if(dwType == REG_DWORD && lDataSize==4 && strData[0]!=0)
				{
					pConfig->put_UseSSL(VARIANT_TRUE);
					m_bIsSSLMode	=	TRUE;
				}
				else
					pConfig->put_UseSSL(VARIANT_FALSE);
				
			}
			else
				pConfig->put_UseSSL(VARIANT_FALSE);
			
			lDataSize = MAX_PATH;
			
			if(RegQueryValueEx(hNetConfigKey,_T("Server"),0,0,(LPBYTE)strData,&lDataSize)==ERROR_SUCCESS)
			{
				if(m_bUserDomainMode)
				{
					m_strServer	=	GetUserDomain();
				}
				else
				{
					m_strServer	=	strData;
				}
				
				pConfig->PutServer((LPCTSTR)m_strServer);
				
				if(m_bIsSSLMode)
					m_strServerPath = _T("https://");
				else
					m_strServerPath = _T("http://");
				
				m_strServerPath += m_strServer; 
			}
			else
			{
				if(m_bIsSSLMode)
					m_strServerPath = _T("https://");
				else
					m_strServerPath = _T("http://");
				
				m_strServer			=	GetUserDomain();
				pConfig->PutServer((LPCTSTR)m_strServer);
				m_strServerPath		+=	m_strServer; 
			}
			
			lDataSize = MAX_PATH;
			
			if(m_lPort==0)
			{
				if(RegQueryValueEx(hNetConfigKey,_T("Port"),0,0,(LPBYTE)strData,&lDataSize)==ERROR_SUCCESS)
				{
					m_lPort = _tstol((LPCTSTR)strData);
					if(m_lPort)
					{
						m_strServerPath += _T(":");
						m_strServerPath += strData;
					}
				}
				else
				{
					m_strServerPath += _T(":");
					if(m_bIsSSLMode)
					{
						m_lPort = 443;
						m_strServerPath += _T("443");
					}
					else
					{
						m_lPort = 80;
						m_strServerPath += _T("80");
					}
				}
			}
			else
			{
				if(m_bIsSSLMode && m_lPort != 443 || m_lPort != 80)
				{
					TCHAR strPort[20] = _T("");
					_ltot(m_lPort, strPort,10);

					m_strServerPath += _T(":");
					m_strServerPath += strPort;
				}
			}

			pConfig->PutPort(m_lPort);
			
			lDataSize = MAX_PATH;
			
			if(RegQueryValueEx(hNetConfigKey,_T("Path"),0,0,(LPBYTE)strData,&lDataSize)==ERROR_SUCCESS)
			{
				pConfig->PutPath(strData);
				
				m_strPath = strData;
				int PathIndex = m_strPath.ReverseFind(_T('/'));
				if(PathIndex!=-1)
				{
					CString strPathTmp	=	m_strPath.Left(PathIndex);
					PathIndex = strPathTmp.ReverseFind(_T('/'));
					if(PathIndex!=-1)
					{
						m_strServerPath += _T('/');
						m_strServerPath += strPathTmp.Left(PathIndex);
					}
				}
			}
			else
			{
				m_strPath = "instmsg/ibn_server.dll";
				pConfig->PutPath((LPCTSTR)m_strPath);
			}
			
			lDataSize = MAX_PATH;
			
			RegCloseKey(hNetConfigKey);
		}
		else
		{
			m_bIsSSLMode	=	FALSE;
			pConfig->put_UseSSL(VARIANT_FALSE);
			
			m_strServer	=	GetUserDomain();
			pConfig->PutServer((LPCTSTR)m_strServer);
			
			m_strServerPath = _T("http://");
			m_strServerPath += m_strServer; 
			
			if(m_lPort==0)
				m_lPort = 80;

			pConfig->PutPort(m_lPort);

			if(m_lPort)
			{
				USES_CONVERSION;

				TCHAR strPort[20] = _T("");

				_ltot(m_lPort, strPort,10);

				m_strServerPath += _T(":");
				m_strServerPath += strPort;
			}
			
			m_strPath = "instmsg/ibn_server.dll";
			pConfig->PutPath((LPCTSTR)m_strPath);
		}
		/************************************************************************/
		/* End Load Settings from Local Machine                                 */
		/************************************************************************/
		//////////////////////////////////////////////////////////////////////////
		
		CString str,strReg;
		long lValue;
		strReg.Format(_T("%lu"), (DWORD)GetOptionInt(IDS_NETOPTIONS, IDS_ACCESSTYPE, INTERNET_OPEN_TYPE_PRECONFIG));
		if(!strReg.IsEmpty())
		{
			lValue = _tstol((LPCTSTR)strReg);
			pConfig->PutProxyType(lValue);
		}
		else
			if(!str.IsEmpty())
			{
				lValue = _tstol((LPCTSTR)str);
				pConfig->PutProxyType(lValue);
			}
			
			if(lValue==INTERNET_OPEN_TYPE_PROXY)
			{
				pConfig->PutProxyServerName((LPCTSTR)GetOptionString(IDS_NETOPTIONS, IDS_PROXYNAME, _T("")));
				pConfig->PutProxyServerPort(_tstol((LPCTSTR)GetOptionString(IDS_NETOPTIONS, IDS_PROXYPORT, _T(""))));
			}
			
			if(lValue!=INTERNET_OPEN_TYPE_DIRECT&&GetOptionInt(IDS_NETOPTIONS,IDS_USEFIREWALL,0))
			{
				pConfig->PutUseFirewall(VARIANT_TRUE);
				pConfig->PutFireWallUserName((LPCTSTR)GetOptionString(IDS_NETOPTIONS, IDS_FIREWALLUSER, _T("")));
				pConfig->PutFireWallPassword((LPCTSTR)GetOptionString(IDS_NETOPTIONS, IDS_FIREWALLPASS, _T("")));
			}
			else
				pConfig->PutUseFirewall(VARIANT_FALSE);
			
	}
	catch(...)
	{
		ASSERT(FALSE);
	}
	
	//////////////////////////////////////////////////////////////////////////
	CWebWindow::SetVariable(_T("ServerPath"), GetServerPath());
	//////////////////////////////////////////////////////////////////////////
	
	return TRUE;
}

//-------------------------------------------------------------------------------
// Name: ClearAll
// Desc: Очищает Контакт Лист, поток сообщений и т.д. для нового пользователя.
//-------------------------------------------------------------------------------
void CMainDlg::ClearAll()
{
	USES_CONVERSION;
	
	m_ContactList.Clear();
	m_ExternalContactList.Clear();
	m_ChatCollection.RemoveAll();
	m_HandleCommandMap.RemoveAll();
	
	//////////////////////////////////////////////////////////////////////////
	CString strActiveChatId;
	for(int iIndex = 0; iIndex<m_ActiveChatRem.GetSize();iIndex++)
	{
		// Step 1. Save Active Forum States [9/9/2002]
		CString strTmpFormat;
		strTmpFormat.Format(_T("%s;"),W2CT(m_ActiveChatRem[iIndex]));
		strActiveChatId += strTmpFormat;
	}
	
	CString	strUserId;
	strUserId.Format(_T("%d"), GetUserID());
	
	theApp.WriteProfileString(GetString(IDS_INFO) + _T("\\") + GetUserRole()+_T("\\")+strUserId,GetString(IDS_CHAT),strActiveChatId);
	
	for(int iIndex = 0; iIndex<m_ActiveChatRem.GetSize();iIndex++)
	{
		
		// Step 2. Free Resourse [9/9/2002]
		SysFreeString(m_ActiveChatRem[iIndex]);
	}
	m_ActiveChatRem.RemoveAll();
	//////////////////////////////////////////////////////////////////////////
	
	BuildContactList();
	BuildChatsList();
	
	POSITION Pos = m_SendMessageDlgMap.GetStartPosition();
	while(Pos)
	{
		long Key	=	0;
		HWND Value	=	NULL;
		m_SendMessageDlgMap.GetNextAssoc(Pos,Key,Value);
		if(IsWindow(Value))
			::SendMessage(Value,WM_CLOSE,NULL,NULL);
	}
	m_SendMessageDlgMap.RemoveAll();
	
	Pos = m_ComeMessageDlgMap.GetStartPosition();
	while(Pos)
	{
		long Key	=	0;
		HWND Value	=	NULL;
		m_ComeMessageDlgMap.GetNextAssoc(Pos,Key,Value);
		if(IsWindow(Value))
			::SendMessage(Value,WM_CLOSE,NULL,NULL);
	}
	m_ComeMessageDlgMap.RemoveAll();
	
	Pos = m_SplitMessageDlgMap.GetStartPosition();
	while(Pos)
	{
		long Key	=	0;
		HWND Value	=	NULL;
		m_SplitMessageDlgMap.GetNextAssoc(Pos,Key,Value);
		if(IsWindow(Value))
			::SendMessage(Value,WM_CLOSE,NULL,NULL);
	}
	m_SplitMessageDlgMap.RemoveAll();
	
	Pos = m_DelUserDlgMap.GetStartPosition();
	while(Pos)
	{
		long Key	=	0;
		HWND Value	=	NULL;
		m_DelUserDlgMap.GetNextAssoc(Pos,Key,Value);
		if(IsWindow(Value))
			::SendMessage(Value,WM_CLOSE,NULL,NULL);
	}
	m_DelUserDlgMap.RemoveAll();
	
	Pos = m_AddUserDlgMap.GetStartPosition();
	while(Pos)
	{
		long Key	=	0;
		HWND Value	=	NULL;
		m_AddUserDlgMap.GetNextAssoc(Pos,Key,Value);
		if(IsWindow(Value))
			::SendMessage(Value,WM_CLOSE,NULL,NULL);
	}
	m_AddUserDlgMap.RemoveAll();
	
	for(int i=0;i<m_AllClosedWindow.GetSize();i++)
	{
		HWND Value	=	m_AllClosedWindow[i];
		if(IsWindow(Value))
			::SendMessage(Value,WM_CLOSE,0,0);
	}
	m_AllClosedWindow.RemoveAll();
				
	m_FileManager.ShowWindow(SW_HIDE);
	m_FileManager.DeleteAllItem();
	
	m_HistoryDlg.ShowWindow(SW_HIDE);
	
	int  Size = m_NewMessageArray.GetSize();
	for(int i = 0;i<Size;i++)
	{
		delete m_NewMessageArray[i];
	}
	m_NewMessageArray.RemoveAll();
	
	//////////////////////////////////////////////////////////////////////////
	Pos = m_SendMessagesMap.GetStartPosition();
	while(Pos)
	{
		LONG		dwKey		=	0;
		IMessage*	pMessage	=	NULL;
		
		m_SendMessagesMap.GetNextAssoc(Pos,dwKey,pMessage);
		
		try
		{
			if(pMessage)
				pMessage->Release();
		}
		catch(_com_error&)
		{
			ASSERT(FALSE);
		}
	}
	m_SendMessagesMap.RemoveAll();
	//////////////////////////////////////////////////////////////////////////
	
	PostMessage(WM_CHANGE_NEWMESSAGE,(WPARAM)m_NewMessageArray.GetSize());
	
	//SetUIHandler(FALSE);
}

//-------------------------------------------------------------------------------
// Name: PopupLoginAndPassword
// Desc: Считывает данные о пользователе и сохранет их в локальных переменных, 
// делает проверку на коректность данных.
//-------------------------------------------------------------------------------
BOOL CMainDlg::PopupLoginAndPassword()
{
	const DWORD buffSize = 255;
	TCHAR strLogin[buffSize],strPassword[buffSize];
    ::GetEnvironmentVariable(_T("MpaUserLogin"), strLogin,buffSize);
	::GetEnvironmentVariable(_T("MpaUserPassword"), strPassword,buffSize);
	
	m_User.m_strLogin = strLogin;
	m_strPassword     = strPassword;
	
	if(m_User.m_strLogin.IsEmpty() || m_strPassword.IsEmpty())
	{
		return FALSE;
	}
	m_User.m_iStatus = S_ONLINE;
	
    return TRUE;
}

//-------------------------------------------------------------------------------
// Name: Show
// Desc: Показывает или прячет окно
//-------------------------------------------------------------------------------
void CMainDlg::Show(BOOL bShow)
{
	if(bShow)
	{
		ShowWindow(SW_SHOW);
	}
	else
	{
		ShowWindow(SW_HIDE);
	}
}

//-------------------------------------------------------------------------------
// Name: OnNetEvent
// Desc: Обработка сообшщений от сервера, которые перенаправил сюда транслятор.
//-------------------------------------------------------------------------------
LRESULT CMainDlg::OnNetEvent(WPARAM w,LPARAM l)
{
	TRACE(_T("\r\n CMainDlg::OnNetEvent Start ... "));
	
	NLT_Container *pItem = (NLT_Container*)w;
	if(IsBadReadPtr(pItem,sizeof(NLT_Container)))
		return -1;
	
	ASSERT(pItem!=NULL);
	
	long State       = 0L;
	long ErrorType   = 0L;
	long ErrorCode   = 0L;
	long nResult     = 0L;
	long Handle      = 0L;
	long CommandType = 0L;
	
	bstr_t    bstrMessage = L"";
	bstr_t    bstrUrl     = L"";
	
	IUser    *pUser    = NULL;
	IMessage *pMessage = NULL;
	IPromo   *pPromo   = NULL;
	IFile    *pFile    = NULL;
	IFiles   *pFiles   = NULL;
	IUsers   *pUsers   = NULL;
	IlocalSIDs *pSIDs  = NULL;
	IMessages *pMessagesList = NULL;
	IChats		*pChats	= NULL;
	IChat		*pChat = NULL;
	IUser		*pUser2= NULL;
	
	CMessage *pMsg     = NULL; 
	
	
	IUserPtr  pMyUser  = NULL;
	
	
	long      DetailsHandle = 0L;
	
	HRESULT   hr       = 0L;
	if(IsLockShowInfo()&&
		(pItem->EventType==NTL_EMessage||
		pItem->EventType==NTL_EFile||
		pItem->EventType==NTL_EAdd||
		pItem->EventType==NTL_EAddR||
		pItem->EventType==NTL_EChangedStatus||
		pItem->EventType==NLT_EChatInvite))
	{
		m_SaveEventArray.Add(pItem);
		return 0;
	}

	//CString strTest;
			
	switch(pItem->EventType)
	{
	case NTL_ENone:
		//MCTRACE(9,"[CMainDlg::OnNetEvent] Unknown Type ???");
		break;
	case NTL_EChangeState:
		//MCTRACE(9,"[CMainDlg::OnNetEvent] EChangeState (State = %d, ErrorType = %d, ErrorCode = %d)",pItem->Long1,pItem->Long2,pItem->Long3);
		
		State     = pItem->Long1;
		ErrorType = pItem->Long2;
		ErrorCode = pItem->Long3;


		//strTest.Format("[CMainDlg::OnNetEvent] EChangeState (State = %d, ErrorType = %d, ErrorCode = %d)",pItem->Long1,pItem->Long2,pItem->Long3);
		//MessageBox(strTest);
		
		switch(State)
		{
		case W_DISCONNECTED:
			dwCurrentStatus = W_DISCONNECTED;
			m_User.m_iStatus = S_OFFLINE;
			DisconnectAllChats();
			BuildContactList();
			BuildChatsList();
			m_lLogonHandle = 0;
			break;
		case W_CONNECTING:
			dwCurrentStatus = W_CONNECTING;
			break;
		case W_CONNECTED:
			if(dwCurrentStatus!=W_CONNECTED)
			{
				//MCTRACE(9,"[CMainDlg::OnNetEvent] EChangeState (W_CONNECTED && dwCurrentStatus!=W_CONNECTED )");
				pMyUser= pSession->GetSelfInfo();
				CString FullLogin = m_User.m_strLogin;
				CUser user(pMyUser);
				m_User = user;
				m_User.m_strLogin = FullLogin;
				CString strTitle;
				strTitle.Format(GetString(IDS_MAIN_TITLE_FORMAT),m_User.GetShowName());
				SetWindowText(strTitle);
				
				// Auto Complete Addon [8/15/2003]
				CString strLoginsXML = GetRegFileText(_T("Cookies"),_T("Logins"));
				
				CComBSTR	bsXML = strLoginsXML;
				VARIANT_BOOL	varLoad	=	VARIANT_FALSE;
				
				CComPtr<IXMLDOMDocument>	pDoc		=	NULL;
				pDoc.CoCreateInstance(CLSID_DOMDocument40, NULL, CLSCTX_INPROC_SERVER);
				
				pDoc->loadXML(bsXML,&varLoad);
				
				if(varLoad!=VARIANT_TRUE)
				{
					bsXML = L"<logins/>";
					pDoc->loadXML(bsXML,&varLoad);
				}
				
				CComPtr<IXMLDOMNode>	loginNode;
				
				CComBSTR	bsLogin = FullLogin;
				
				CComBSTR bsPath = L"logins[login='";
				bsPath += bsLogin;
				bsPath += L"']/login";
				
				pDoc->selectSingleNode(bsPath,&loginNode);
				
				if(loginNode==NULL)
				{
					pDoc->createNode(CComVariant(NODE_ELEMENT),CComBSTR(L"login"),NULL,&loginNode);
					
					CComPtr<IXMLDOMNode>	loginsNode, newloginNode;
					pDoc->selectSingleNode(CComBSTR(L"logins"),&loginsNode);
					
					loginNode->put_text(bsLogin);
					
					loginsNode->appendChild(loginNode,&newloginNode);
					
					bsXML.Empty();
					
					pDoc->get_xml(&bsXML);
					
					strLoginsXML = bsXML;
					SetRegFileText(_T("Cookies"),_T("Logins"),strLoginsXML);
				}
				
				/////////////////////////////////////////////////////
				// Инициализировать Локальную Базу данных ....
				/////////////////////////////////////////////////////
				if(!LocalDataBaseEnable())
				{
					/************************************************************************/
					/* Load DB Path from Local Machine                                     */
					/************************************************************************/
					CString strData	= GetAppDataDir() + _T("\\History");
					
					CreateDirectory(strData,NULL);
					
					///strData += _T("\\history.dbf");
					// Set DB like UserLogin + .dbf [9/5/2003]
					strData += _T("\\");
					strData += m_User.m_strLogin;
					strData += _T(".dbf");
					
					BOOL bReindex	=	GetOptionInt(IDS_HISTORY,IDS_REINDEX);
					if(bReindex)
						WriteOptionInt(IDS_HISTORY,IDS_REINDEX,0);
					BOOL bInit = InitLocalDataBase(strData,bReindex);
					//////////////////////////////////////////////////////////////////////////
				}
			}
			dwCurrentStatus = W_CONNECTED;
			m_lLogonHandle = 0;
			break;
		}
		
		//MCTRACE(9,"[CMainDlg::OnNetEvent] EChangeState (PostMessage(WM_UPDATE_STATUS ... )");
		
		SendMessage(WM_UPDATE_STATUS,(WPARAM)dwCurrentStatus,(LPARAM)m_User.m_iStatus);
		
		if(ErrorType)
			ShowError(ErrorType,ErrorCode);
		
		if(State==W_DISCONNECTED&&m_bUpdateUserStatus)
		{
			m_bUpdateUserStatus = FALSE;
			Login();
		}
		
		break;
		case NTL_EChangedStatus:
			//MCTRACE(9,"[CMainDlg::OnNetEvent] NTL_EChangedStatus");
			//TRACE(_T("\r\n function AutoUnMarchaling Start ... ");
			hr = AutoUnMarchaling(pItem,(LPUNKNOWN*)&pUser);
			//TRACE(_T("\r\n function AutoUnMarchaling End ... ");
			if(SUCCEEDED(hr))
			{
				//////////////////////////////////////////////////////////////////////////
				// Play Sound [2/27/2002]
				if(long(pUser->GetValue(bstr_t("status")))==S_ONLINE)
				{
					//TRACE(_T("\r\n function McPlaySound Start ... ");
					McPlaySound(IDS_ONLINE_ALERT_KEY);
					//TRACE(_T("\r\n function McPlaySound End ... ");
				}
				//////////////////////////////////////////////////////////////////////////
				
				ChangeUserStatus(pUser);
				//TRACE(_T("\r\n function ChangeUserStatus End ... ");
				pUser->Release();
				//TRACE(_T("\r\n CMainDlg::OnNetEvent EChangedStatus End ... ");
			}
			break;
		case NTL_EMessage:
			//MCTRACE(9,"[CMainDlg::OnNetEvent] NTL_EMessage");
			hr = AutoUnMarchaling(pItem,(LPUNKNOWN*)&pMessage);
			if(SUCCEEDED(hr))
			{
				TRACE(_T("\r\n CMainDlg::OnNetEvent(NTL_EMessage)"));
				
				pMsg = new CMessage(pMessage);
				
				{
					
					if(LocalDataBaseEnable()&&(pSession!=NULL))
					{
						m_MainDlgLock.Lock(2000);

						//try
						//{
						m_LocalHistory->AddMessage(pMsg->GetSender().GetGlobalID(),m_User.GetGlobalID(),
							pSession->GetSID(), bstr_t(LPCTSTR(pMsg->GetMessageID())),long(pMsg->GetTime()),
							0,bstr_t((BSTR)pMsg->GetMessage()),VARIANT_TRUE);

						m_MainDlgLock.Unlock();
						
						RefreshHistoryFor(pMsg->GetSender().GetGlobalID());
						
						CUser *puser = m_ContactList.GetAt(pMsg->GetSender().GetGlobalID());
						if(!puser)
							puser = m_ExternalContactList.GetAt(pMsg->GetSender().GetGlobalID());
						
						if(puser==NULL)
						{
							m_ExternalContactList.SetAt(pMsg->GetSender());
							/// Запросить дополнительную информацию ...
							theNet2.LockTranslator();
							try
							{
								DetailsHandle = pSession->UserDetails(pMsg->GetSender().GetGlobalID(),1);
								if(DetailsHandle)
									theNet2.AddToTranslator(DetailsHandle,this->GetSafeHwnd());
							}
							catch(...)
							{
								ASSERT(FALSE);
							}
							theNet2.UnlockTranslator();
						}
						else
							pMsg->GetSender() = (*puser);
						
						NewMessage(pMsg);
						
						//////////////////////////////////////////////////////////////////////////
						// Play Sound [2/27/2002]
						McPlaySound(IDS_INCOMING_MESSAGE_KEY);
						TRACE(_T("\r\n CMainDlg::McPlaySound(IDS_INCOMING_MESSAGE)"));
						//Beep(100,100);
						//////////////////////////////////////////////////////////////////////////
						//}
						//catch(...)
						//{
						//	ASSERT(FALSE);
						//}
					}
					
					pMessage->Release();
				}
			}
			break;
		case NTL_EPromo:
			//MCTRACE(9,"[CMainDlg::OnNetEvent] NTL_EPromo");
			hr = AutoUnMarchaling(pItem,(LPUNKNOWN*)&pPromo);
			if(SUCCEEDED(hr))
			{
				PostMessage(WM_NEW_PROMOCOME,(WPARAM)(pItem->String1.copy()));
				pPromo->Release();
			}
			break;
		case NTL_EFile:
			//MCTRACE(9,"[CMainDlg::OnNetEvent] NTL_EFile");
			hr = AutoUnMarchaling(pItem,(LPUNKNOWN*)&pFile);
			if(SUCCEEDED(hr))
			{
				//////////////////////////////////////////////////////////////////////////
				// Play Sound [2/27/2002]
				McPlaySound(IDS_INCOMING_FILE_KEY);
				//////////////////////////////////////////////////////////////////////////
				
				IUserPtr pSender = pFile->GetSender ();
				CUser	*puser = FindUserInVisualContactListByGlobalId(pSender->GetValue("@id"));
				
				if(puser!=NULL)
				{
					//m_FileDownloadStatus.AddToDownload(*puser,pFile);
					m_FileManager.AddToDownload(*puser,pFile);
				}
				else
				{
					CUser SenderUser(pSender);
					///Запросить информацию о пользователе 
					m_ExternalContactList.SetAt(SenderUser);
					/// Запросить дополнительную информацию ...
					theNet2.LockTranslator();
					try
					{
						DetailsHandle = pSession->UserDetails(SenderUser.GetGlobalID(),1);
						if(DetailsHandle)
							theNet2.AddToTranslator(DetailsHandle,this->GetSafeHwnd());
					}
					catch(_com_error&)
					{
						ASSERT(FALSE);
					}
					theNet2.UnlockTranslator();
					
					//BuildContactListToUser(SenderUser.GetGlobalID());
					// Old COde [3/19/2003]
					//BuildContactList();
					
					//m_FileDownloadStatus.AddToDownload(SenderUser,pFile);
					m_FileManager.AddToDownload(SenderUser,pFile);
				}

				//::FlashWindow(m_FileManager.GetSafeHwnd(),TRUE);
				
				pFile->Release();
			}
			break;
			
		case NTL_EAdd:
			//MCTRACE(9,"[CMainDlg::OnNetEvent] NTL_EAdd");
			hr = AutoUnMarchaling(pItem,(LPUNKNOWN*)&pUser);
			if(SUCCEEDED(hr))
			{
				//////////////////////////////////////////////////////////////////////////
				// Play Sound [2/27/2002]
				McPlaySound(IDS_AUTHORIZATION_REQUEST_KEY);
				//////////////////////////////////////////////////////////////////////////
				
				///AddUserRequest(pUser,pItem->String1);
				CAddUserRequest *pDlg = new CAddUserRequest(this);
				pDlg->Create(CAddUserRequest::IDD,GetDesktopWindow());
				
				CUser SenderUser(pUser);
				pDlg->SetSender(SenderUser,pItem->String1);
				pDlg->ShowWindow(SW_SHOWNORMAL);
				pDlg->SetForegroundWindow();
				
				pUser->Release();
			}
			break;
		case NTL_EAddR:
			//MCTRACE(9,"[CMainDlg::OnNetEvent] NTL_EAddR");
			hr = AutoUnMarchaling(pItem,(LPUNKNOWN*)&pUser);
			if(SUCCEEDED(hr))
			{
				CUser AutUser(pUser), tmpAutUser;
				nResult = pItem->Long1;
				if(GetUserByGlobalId(AutUser.GetGlobalID(),tmpAutUser))
				{
					CString strText;
					if(nResult==2)
					{
						DeleteFromContact(AutUser.GetGlobalID());	
						strText.Format(GetString(IDS_REQUEST_DENIED_FORMAT),AutUser.GetShowName());
					}
					else
					{
						strText.Format(GetString(IDS_REQUEST_ACCEPT_FORMAT),AutUser.GetShowName());
					}
					MessageBox(strText,GetString(IDS_IBN_ADD_FRIEND_TITLE),MB_OK|MB_ICONINFORMATION);
				}
				pUser->Release();
			}
			break;
		case NTL_EContactList:
			//MCTRACE(9,"[CMainDlg::OnNetEvent] NTL_EContactList");
			hr = AutoUnMarchaling(pItem,(LPUNKNOWN*)&pUsers);
			if(SUCCEEDED(hr))
			{
				LoadUsers(pUsers);
				pUsers->Release();
			}
			PostMessage(WM_UPDATE_CONTACTLIST);
			PostMessage(WM_PROCESS_COMMAND_LINE_MESSAGES);
			break;
		case NLT_ESelfStatus:
			//MCTRACE(9,"[CMainDlg::OnNetEvent] NLT_ESelfStatus");
			m_User.m_iStatus = pItem->Long1;
			SendMessage(WM_UPDATE_STATUS,(WPARAM)dwCurrentStatus,(LPARAM)m_User.m_iStatus);
			break;
		case NLT_EDetails:
			//MCTRACE(9,"[CMainDlg::OnNetEvent] NLT_EDetails");
			hr = AutoUnMarchaling(pItem,(LPUNKNOWN*)&pUser);
			if(SUCCEEDED(hr))
			{
				theNet2.LockTranslator();
				theNet2.RemoveFromTranslator(pItem->Handel);
				try
				{
					CUser m_AddInfoUser(pUser), *pTmpUser	=	NULL;
					
					if((pTmpUser = m_ContactList.GetAt(m_AddInfoUser.GetGlobalID()))==NULL)
						pTmpUser = m_ExternalContactList.GetAt(m_AddInfoUser.GetGlobalID());
					
					if(pTmpUser)
					{
						pTmpUser->Update(m_AddInfoUser);
						
						BuildContactListToUser(pTmpUser->GetGlobalID());
						// OldCode [3/19/2003]
						//BuildContactList();
						
						RefreshUserInfo(m_AddInfoUser.GetGlobalID());
					}
					pUser->Release();
				}
				catch(...)
				{
					ASSERT(FALSE);
				}
				theNet2.UnlockTranslator();
			}
			break;
		case NLT_ESessionsList:
			break;
		case NLT_EMessagesList:
		/*
		hr = AutoUnMarchaling(pItem,(LPUNKNOWN*)&pMessagesList);
		if(SUCCEEDED(hr))
		{
		theNet2.LockTranslator();
		try
		{
								SaveMessagesToDataBase(pItem->Handel,pMessagesList);
								theNet2.RemoveFromTranslator(pItem->Handel);
								pMessagesList->Release();
								}
								catch(...)
								{
								ASSERT(FALSE);
								}
								theNet2.UnlockTranslator();
						}*/
			ASSERT(FALSE);
			break;
		case NLT_ECommandOK:
			// Addon for Light Messages [9/5/2002]
			//MCTRACE(9,"[CMainDlg::OnNetEvent] NLT_ECommandOK");
			{
				IMessage *pLightMessage	=	NULL;
				if(m_SendMessagesMap.Lookup(pItem->Handel,pLightMessage))
				{
					// Message from DB mark and Free Resorse [9/5/2002]
					if(LocalDataBaseEnable())
					{
						long MessageTime = pItem->Long1;
						
						try
						{
							m_LocalHistory->MarkMessagesAsSent(pLightMessage->GetMID(),(LPCTSTR)GetSID(),MessageTime);
						}
						catch(_com_error&)
						{
							ASSERT(FALSE);
						}
						
						// Refresh SendMessageDialog [9/5/2002]
						
						try
						{
							IUsersPtr pRecipients = pLightMessage->GetRecipients();
							
							for(long iUserIndex = 1; iUserIndex<=pRecipients->GetCount();iUserIndex++)
							{
								IUserPtr pRecipient = pRecipients->GetItem(iUserIndex);
								LONG lUserId = pRecipient->GetValue(L"@id");
								
								HWND	hDlgWnd	=	NULL;
								if(m_SplitMessageDlgMap.Lookup(lUserId,hDlgWnd)&&::IsWindow(hDlgWnd))
								{
									/// Да Открыт
									CMcWindowAgent Agent(hDlgWnd);
									Agent.Refresh();
								}
							}
						}
						catch(_com_error&)
						{
							ASSERT(FALSE);
						}
					}
					
					// Free Resourse [9/5/2002]
					m_SendMessagesMap.RemoveKey(pItem->Handel);
					
					if(pLightMessage)
						pLightMessage->Release();
				}
			}
			// End Addon for Light Messages [9/5/2002]
			
			// Free Resourse [9/5/2002]
			theNet2.LockTranslator();
			theNet2.RemoveFromTranslator(pItem->Handel);	
			theNet2.UnlockTranslator();
			
			break;
		case NLT_ECommandError:
			{
				_bstr_t MID;
				//MCTRACE(9,"[CMainDlg::OnNetEvent] NLT_ECommandError");
				{
					IMessage *pLightMessage	=	NULL;
					if(m_SendMessagesMap.Lookup(pItem->Handel,pLightMessage))
					{
						// Message need delete from DB or skip for futer  [9/5/2002]
						
						// TODO: We are need working with DB Marking.
						
						// Free Resourse [9/5/2002]
						m_SendMessagesMap.RemoveKey(pItem->Handel);

						
						if(pLightMessage)
						{
							MID = pLightMessage->GetMID();

							pLightMessage->Release();
						}
					}
				}
				// End Addon for Light Messages [9/5/2002]
				
				// Addon from Chat Change status [8/15/2002]
				{
					CString strValue;
					
					if(m_ChatHandleMap.Lookup(pItem->Handel,strValue))
					{
						// Error from Chat status Change ... [8/15/2002]
						m_ChatHandleMap.RemoveKey(pItem->Handel);
					}
				}
				// End [8/15/2002]
				theNet2.LockTranslator();
				theNet2.RemoveFromTranslator(pItem->Handel);
				theNet2.UnlockTranslator();

				if(pItem->Long1==etSERVER)
				{
					switch(pItem->Long2)
					{
					case ERR_UNABLE_CREATE_CONN:
						_SHOW_IBN_ERROR_DLG_OK(IDS_SERVICENOTAVAILABLE);
						break;
					case ERR_WRONG_XML:
						if(LocalDataBaseEnable())
						{
							if(MID.length()>0)
							{
								long MessageTime = pItem->Long1;
					
								try
								{
									m_LocalHistory->MarkMessagesAsSent(MID,(LPCTSTR)GetSID(),MessageTime);
								}
								catch(_com_error&)
								{
									ASSERT(FALSE);
								}

							}	
						}
						break;
					}
				}
			}
			break;
		case NLT_EOffLineFiles:
			//MCTRACE(9,"[CMainDlg::OnNetEvent] NLT_EOffLineFiles");
			ASSERT(FALSE);
			break;
		case NLT_ESysMess:
			//MCTRACE(9,"[CMainDlg::OnNetEvent] NLT_ESysMess");
			{
				long	Code		= pItem->Long1;
				_bstr_t	Description = pItem->String1;
				switch(Code) 
				{
				case 2:
					// Relogon [3/29/2002]
					m_bUpdateUserStatus = TRUE;
					SendMessage(WM_SHOW_LOGIN_DLG);
					break;
				case 3:
					//UpdateWebStub
					LoadOptions();
					break;
				case 4:
					// Alert [3/29/2002]
					/************************************************************************/
					/* Description content a XML:
					<alert app_id="27"> 
					<app>AAA AAA</app>
					<title></title>				maybe empty
					<description></description> maybe empty
					<url></url>					maybe empty
					</alert>
					*/
					/************************************************************************/
					
					{
						HRESULT hr	=	S_OK;
						
						CComPtr<IXMLDOMDocument>	pDoc = NULL;
						hr = pDoc.CoCreateInstance(CLSID_DOMDocument40, NULL, CLSCTX_INPROC_SERVER);
						
						if(SUCCEEDED(hr))
						{
							VARIANT_BOOL	varSuccess	=	VARIANT_FALSE;
							pDoc->loadXML(Description,&varSuccess);
							
							if(varSuccess!=VARIANT_FALSE)
							{
								CComPtr<IXMLDOMNode>	pNode	=	NULL;
								hr = pDoc->selectSingleNode(CComBSTR(L"alert"),&pNode);
								if(pNode)
								{
									//USES_CONVERSION;
									// Step 0. UnPack XML;
									CComVariant	varStubId;
									CComBSTR	/*bsApp, bsTitle, bsDescription,*/ bsUrl;
									
									long		Id	=	0;
									
									GetAttribute(pNode,CComBSTR(L"app_id"),&varStubId);
									hr	= varStubId.ChangeType(VT_I4);
									
									GetTextByPath(pNode,CComBSTR(L"url"),&bsUrl);
									
									// Step 1. Load XSLT
									CComBSTR bstrtXSLT;
									
									try
									{
										LoadSkins m_LoadSkins;
										IStreamPtr pStream = NULL;
										bstr_t xsltPath = bstr_t(IBN_SCHEMA) +(LPCTSTR)GetCurrentSkin() + bstr_t("/Shell/Alert/alert.xslt");
										long Error = 0L;
										m_LoadSkins.Load(xsltPath,&pStream,&Error);
										
										if(pStream)
										{
											
											const ULONG Read = 1000;
											ULONG pRealyRead = 0;
											TCHAR pRead[Read+1];
											long i = 0;
											do
											{
												i++;
												hr = pStream->Read((LPVOID)pRead,Read,&pRealyRead);
												pRead [pRealyRead] = '\0';
												bstrtXSLT += pRead;
											}
											while(Read==pRealyRead);
											
										}
									}
									catch(...)
									{
										hr	=	E_FAIL;
										ASSERT(FALSE);
									}
									
									
									if(hr==S_OK)
									{
										//////////////////////////////////////////////////////////////////////////
										// Play Sound [2/27/2002]
										McPlaySound(IDS_NEW_ALERT_COME_KEY);
										//////////////////////////////////////////////////////////////////////////
										
										CPopupMessage	*pNewPopuWindow	=	new CPopupMessage(this);
										pNewPopuWindow->Create(CPopupMessage::IDD,this);
										pNewPopuWindow->InitClickMsg(WM_ALERT_POPUP_MESSAGE_CLK,varStubId.lVal,LPARAM(bsUrl.Copy()));
										pNewPopuWindow->Show(Description,bstrtXSLT);
										
									}
								}
							}
						}
						
					}
					break;
				}
			}
			break;
			
			// Chat Addon. Load All Chat List [8/6/2002]
		case NLT_EChatList:
			//MCTRACE(9,"[CMainDlg::OnNetEvent] NLT_EChatList");
			hr = AutoUnMarchaling(pItem,(LPUNKNOWN*)&pChats);
			if(SUCCEEDED(hr))
			{
				LoadChats(pChats);
				pChats->Release();
			}
			break;
		case NLT_EChatStatus:
			//MCTRACE(9,"[CMainDlg::OnNetEvent] NLT_EChatStatus");
			AutoUnMarchaling(pItem,(LPUNKNOWN*)&pUsers);
			{
				// Step 1. Find Chat From Handle [8/8/2002]
				CString strValue;
				
				if(m_ChatHandleMap.Lookup(pItem->Handel,strValue))
				{
					CComBSTR	bsVal;
					bsVal.Attach(strValue.AllocSysString());
					
					for(int iChatIndex = 0;iChatIndex<m_ChatCollection.GetSize();iChatIndex++)
					{
						CChat& tmpChat =  m_ChatCollection.ElementAt(iChatIndex);
						
						if(tmpChat.GetId()==bsVal)
						{
							// Step 2. Remember User Status
							tmpChat.SetStatus(((pUsers!=NULL)?SC_ACTIVE:SC_INACTIVE));
							
							//  [9/9/2002]
							BOOL bWasFound = FALSE;
							
							for(int iChatRemIndex = 0; iChatRemIndex<m_ActiveChatRem.GetSize();iChatRemIndex++)
							{
								BSTR bsID = m_ActiveChatRem[iChatRemIndex];
								if(tmpChat.GetId()==CComBSTR(bsID))
								{
									bWasFound = TRUE;
									if(tmpChat.GetStatus()==SC_INACTIVE)
									{
										::SysFreeString(bsID);
										m_ActiveChatRem.RemoveAt(iChatRemIndex);
										break;
									}
								}
							}
							
							if(!bWasFound&&tmpChat.GetStatus()==SC_ACTIVE)
							{
								m_ActiveChatRem.Add(tmpChat.GetId().Copy());
							}
							//  [9/9/2002]
							
							if(tmpChat.GetStatus()==SC_ACTIVE&&pUsers)
							{
								tmpChat.LoadUser(pUsers);
								
								// Add User to Color Arhive [8/28/2002]
								POSITION pos = tmpChat.GetUsers().InitIteration();
								
								CUser *pUser = NULL;
								
								while(tmpChat.GetUsers().GetNext(pos,pUser))
								{
									DWORD dwColor	=	0;
									if(GetColorFromColorStorage(GetUserRole(),GetUserID(),pUser->GetGlobalID(),dwColor))
									{
										SetItemToColorStorage(GetUserRole(),GetUserID(),pUser->GetGlobalID(),pUser->GetShowName(),dwColor);
									}
									else
									{
										// Generate New Color [8/28/2002]
										//dwColor = RGB(0xFF,0,0);
										dwColor = RGB((rand()%32)*8,(rand()%32)*8,(rand()%32)*8);
										
										SetItemToColorStorage(GetUserRole(),GetUserID(),pUser->GetGlobalID(),pUser->GetShowName(),dwColor);
									}
								}
								
								// Step 3. After setting Active status, Load Users to Chat User Collection [8/8/2002]
								tmpChat.LoadMessages(GetUserRole(),GetUserID(),pItem->String1);
								
							}
							
							// Step 4. Find Open ChatDlg for Chat and refresh it[8/8/2002]
							CMcWindowAgent winAgent(tmpChat.GetChatWindow());
							
							if(winAgent.IsValid())
							{
								winAgent.Refresh();
							}
							
							// Step N. Check Handle Command [9/7/2002]
							long lCommand = 0;
							if(m_HandleCommandMap.Lookup(pItem->Handel,lCommand))
							{
								switch(lCommand)
								{
								case HCI_OPEN_CHAT_WINDOW:
									{
										// Create Chat Window
										CurrChatTID = tmpChat.GetTID();
										OnChatSendMessage();												
									}
									break;
								};
								
								m_HandleCommandMap.RemoveKey(pItem->Handel);
							}
							
							// Step 5. Refresh Chats List  [8/9/2002]
							BuildChatsList();	
							
							break;
						}
					}
					
					m_ChatHandleMap.RemoveKey(pItem->Handel);
				}
				if(pUsers)
					pUsers->Release();
				
				theNet2.LockTranslator();
				theNet2.RemoveFromTranslator(pItem->Handel);
				theNet2.UnlockTranslator();
			}
			break;
		case NLT_EChatInvite:
			//MCTRACE(9,"[CMainDlg::OnNetEvent] NLT_EChatInvite");
			hr  = AutoUnMarchaling(pItem,(LPUNKNOWN*)&pChat);
			if(hr==S_OK)
			{
				hr  = AutoUnMarchaling2(pItem,(LPUNKNOWN*)&pUser);
				if(hr==S_OK)
				{
					hr  = AutoUnMarchaling3(pItem,(LPUNKNOWN*)&pUser2);
					if(hr==S_OK)
					{
						// Find Open ChatDlg for Chat and refresh it[8/8/2002]
						InviteNewUserInChat(pChat, pUser, pUser2,pItem->String1);
						// Free Resourse  [8/9/2002]
						pUser2->Release();
					}
					pUser->Release();
				}
				pChat->Release();
			}
			break;
		case NLT_EChatLeave:
			//MCTRACE(9,"[CMainDlg::OnNetEvent] NLT_EChatLeave");
			hr  = AutoUnMarchaling(pItem,(LPUNKNOWN*)&pUser);
			hr  = AutoUnMarchaling2(pItem,(LPUNKNOWN*)&pChat);
			if(pUser&&pChat)
			{
				//Add Information Message
				LeaveUserFromChat(pChat, pUser);
			}
			if(pChat)
				pChat->Release();
			if(pUser)
				pUser->Release();
			break;
		case NLT_EChatUserStatus:
			//MCTRACE(9,"[CMainDlg::OnNetEvent] NLT_EChatUserStatus");
			hr  = AutoUnMarchaling(pItem,(LPUNKNOWN*)&pUser);
			hr  = AutoUnMarchaling2(pItem,(LPUNKNOWN*)&pChat);
			if(pUser&&pChat)
			{
				UpdateUserChatStatus(pChat,pUser);
			}
			if(pChat)
				pChat->Release();
			if(pUser)
				pUser->Release();
			break;
		case NLT_EChatMessage:
			//MCTRACE(9,"[CMainDlg::OnNetEvent] NLT_EChatMessage");
			hr  = AutoUnMarchaling(pItem,(LPUNKNOWN*)&pChat);
			hr  = AutoUnMarchaling2(pItem,(LPUNKNOWN*)&pMessage);
			if(pChat&&pMessage)
			{
				NewChatMessage(pChat,pMessage);
			}
			if(pChat)
				pChat->Release();
			if(pMessage)
				pMessage->Release();
			break;
		case NLT_EChatAccept:
			//MCTRACE(9,"[CMainDlg::OnNetEvent] NLT_EChatAccept");
			hr  = AutoUnMarchaling(pItem,(LPUNKNOWN*)&pChat);
			hr  = AutoUnMarchaling2(pItem,(LPUNKNOWN*)&pUser);
			if(pUser&&pChat)
			{
				ChatAccept(pChat,pUser,pItem->Long1);
			}
			if(pChat)
				pChat->Release();
			if(pUser)
				pUser->Release();
			break;
		default:
			//MCTRACE(9,"[CMainDlg::OnNetEvent] UnknownEx (EventType = %d) ???",pItem->EventType);
			ASSERT(FALSE);
			break;
	}
	delete pItem;
	return 0;
}

//-------------------------------------------------------------------------------
// Name: BuildContactList
// Desc: Строит контакт лист, + сохраняет и востанавливает Floating
//-------------------------------------------------------------------------------
void CMainDlg::BuildContactList()
{
	// Test #1
	//_com_issue_error(E_FAIL);
	
	// Test #2
	//int *a = 0;
	//*a = 12;
	
	// Test #3
	//throw 12;
	
	// Test #4
	//throw new CMainDlg();
	
	// Test #5
	//throw new CException();
	
	//TRACE(_T("\r\n  CMainDlg::BuildContactList Start ...");
	long inUserID = m_User.GetGlobalID();
	int m_numIco;
	
	CMap <long,long,CPoint,CPoint> m_MapPoint;
	//////////////////////////////////////////////
	/// Сделаем проверку нету ли Плавующих и востановим их позициию
	try
	{
		CUser* pUser=NULL;
		
		if(POSITION pos = m_ContactList.InitIteration())
		{
			for(int i=0; m_ContactList.GetNext(pos, pUser); i++)
			{
				if(m_treebox.FloatingEnable(pUser->TID))
				{
					CPoint point;
					m_treebox.GetFloatPos(pUser->TID,&(point.x),&(point.y));
					m_MapPoint.SetAt(pUser->GetGlobalID(),point);
				}
			}
			
		}
		
		if(POSITION pos = m_ExternalContactList.InitIteration())
		{
			for(int i=0; m_ExternalContactList.GetNext(pos, pUser); i++)
			{
				if(m_treebox.FloatingEnable(pUser->TID))
				{
					CPoint point;
					m_treebox.GetFloatPos(pUser->TID,&(point.x),&(point.y));
					m_MapPoint.SetAt(pUser->GetGlobalID(),point);
				}
			}
		}
	}
	catch(...)
	{
	}
	
	//////////////////////////////////////////////////////////////////////////
	CMapStringToPtr		GroupIsOpen;
	// Сохраним позиции открыта группа или нет [2/25/2002]
	
	// Load Groups State From Reg [11/14/2002]
	CString	strUserId;
	strUserId.Format(_T("%d"), GetUserID());
	
	
	CString strClosedGroupsId = theApp.GetProfileString(GetString(IDS_INFO) + _T("\\") + GetUserRole()+_T("\\")+strUserId,GetString(IDS_GROUPS_STATE));
	
	if(!strClosedGroupsId.IsEmpty())
	{
		int iPos = 0, iEndPos;
		
		while((iEndPos = strClosedGroupsId.Find(_T(";"),iPos))!=-1)
		{
			CString strGroupName = strClosedGroupsId.Mid(iPos,iEndPos-iPos);
			//TRACE(_T("$$$$$ Extract Group String (%s)",strGroupName);
			GroupIsOpen.SetAt(strGroupName,(LPVOID)FALSE);
			iPos = iEndPos + 1;
		}
	}
	
	// Step 2. Считать текущее и модифицировать [3/14/2002]
	POSITION Pos = m_GroupTIDMap.GetStartPosition();
	if(Pos)
	{
		//TRACE(_T("$$$$$ Open Group String (%s)",strClosedGroupsId);
		
		while(Pos)
		{
			CString strName;
			long	lGroupTID;
			m_GroupTIDMap.GetNextAssoc(Pos,strName,(void*&)lGroupTID);
			GroupIsOpen.SetAt(strName,(void*)m_treebox.RootIsOpen(lGroupTID));
			
			// Group Id Addon [11/14/2002]
			CString strGroupID = strName + _T(";");
			strClosedGroupsId.Replace(strGroupID, _T(""));
			
			if(!m_treebox.RootIsOpen(lGroupTID))
			{
				strClosedGroupsId += strGroupID;
			}
			// Group Id [11/14/2002]
		}
		
		//TRACE(_T("$$$$$ Save Group String (%s)",strClosedGroupsId);
		
		theApp.WriteProfileString(GetString(IDS_INFO) + _T("\\") + GetUserRole()+_T("\\")+strUserId,GetString(IDS_GROUPS_STATE),strClosedGroupsId);
	}
	
	m_GroupTIDMap.RemoveAll();
	
	m_treebox.DeleteTree();
	CurrTID = -1;
	
	int CLMode  = GetOptionInt(IDS_OFSMESSENGER,IDS_CLMODE,2);
	
	CUser* pUser=NULL;
	
	switch(CLMode) 
	{
	case 1:
		{
			if(POSITION pos = m_ContactList.InitIteration())
			{
				for(int i=0; m_ContactList.GetNext(pos,pUser); i++)
				{
					if(!m_User.IsOnline()&&!pUser->IsSystemUser())
						pUser->m_iStatus = S_OFFLINE;
					
					if(UserIsVisible(pUser->GetGlobalID()))
					{
						// Step 1. Проверить создавали ли мы группу???
						long	GroupTID	= 0;
						
						pUser->m_nIcon		= -1;
						m_numIco			= pUser->GetIcon2();
						
						if(!m_GroupTIDMap.Lookup(pUser->m_strType,(void*&)GroupTID))
						{
							// Step 2. Если нет, то создать группу .
							
							// Oleg ZHuk: Addon for work with web app [9/11/2003]
							if(!pUser->IsSystemUser())
								GroupTID = m_treebox.AddItem(0,pUser->m_strType,m_ShablonId[0]);
							else
							{
								long ShablonId[10] = {0L,1L,0L,0L,0L,0L,0L,0L,0L,0L};
								GroupTID = m_treebox.AddItem(0,GetString(IDS_SYSTEM_SERVICES_GROUP_NAME),ShablonId);
							}
							
							m_GroupTIDMap.SetAt(pUser->m_strType,(void*)GroupTID);
							//  [9/11/2003]
						}
						
						//////////////////////////////////////////////////////////////////////////
						void *pTmp	=	NULL;
						if(GroupIsOpen.Lookup(pUser->m_strType,pTmp))
						{
							m_treebox.RootOpen(GroupTID,(BOOL)pTmp);
						}
						//////////////////////////////////////////////////////////////////////////
						
						// Step 3. добавить пользователя [1/28/2002]
						
						// Oleg ZHuk: Addon for work with web app [9/11/2003]
						if(pUser->IsSystemUser())
							pUser->m_iStatus = S_WEBAPP;
						//  [9/11/2003]
						
						pUser->TID = m_treebox.AddItem(GroupTID,pUser->GetShowName(),m_ShablonId[IndexEquals[pUser->m_iStatus]+10*pUser->m_bHasNewMessages]);								
						
					}
					else
						pUser->TID = -1;
				}
			}
		}
		break;
	case 2:
		{
			if(POSITION pos = m_ContactList.InitIteration())
			{
				for(int i=0; m_ContactList.GetNext(pos,pUser); i++)
				{
					if(!m_User.IsOnline()&&!pUser->IsSystemUser())
						pUser->m_iStatus = S_OFFLINE;
					
					if(UserIsVisible(pUser->GetGlobalID()))
					{
						// Step 1. Проверить создавали ли мы группу???
						long	GroupTID	= 0;
						
						pUser->m_nIcon		= -1;
						m_numIco			= pUser->GetIcon2();
						
						if(pUser->m_iStatus==S_OFFLINE||pUser->m_iStatus==S_INVISIBLE)
						{
							if(!m_GroupTIDMap.Lookup(GetString(IDS_OFFLINE),(void*&)GroupTID))
							{
								long ShablonId[10] = {0L,2L,0L,0L,0L,0L,0L,0L,0L,0L};
								// Step 2. Если нет, то создать группу .
								GroupTID = m_treebox.AddItem(0,GetString(IDS_OFFLINE),ShablonId);
								
								m_GroupTIDMap.SetAt(GetString(IDS_OFFLINE),(void*)GroupTID);
							}
							//m_treebox.RootOpen(GroupTID,FALSE);
							
							//////////////////////////////////////////////////////////////////////////
							void *pTmp	=	NULL;
							if(GroupIsOpen.Lookup(GetString(IDS_OFFLINE),pTmp))
							{
								m_treebox.RootOpen(GroupTID,(BOOL)pTmp);
							}
							//////////////////////////////////////////////////////////////////////////
						}
						else
						{
							
							if(!m_GroupTIDMap.Lookup(pUser->m_strType,(void*&)GroupTID))
							{
								// Step 2. Если нет, то создать группу .
								
								// Oleg ZHuk: Addon for work with web app [9/11/2003]
								if(!pUser->IsSystemUser())
									GroupTID = m_treebox.AddItem(0,pUser->m_strType,m_ShablonId[0]);
								else
								{
									long ShablonId[10] = {0L,1L,0L,0L,0L,0L,0L,0L,0L,0L};
									GroupTID = m_treebox.AddItem(0,GetString(IDS_SYSTEM_SERVICES_GROUP_NAME),ShablonId);
								}
								
								m_GroupTIDMap.SetAt(pUser->m_strType,(void*)GroupTID);
								//  [9/11/2003]
							}
							
							//////////////////////////////////////////////////////////////////////////
							void *pTmp	=	NULL;
							if(GroupIsOpen.Lookup(pUser->m_strType,pTmp))
							{
								m_treebox.RootOpen(GroupTID,(BOOL)pTmp);
							}
							//////////////////////////////////////////////////////////////////////////
						}
						// Step 3. добавить пользователя [1/28/2002]
						
						// Oleg ZHuk: Addon for work with web app [9/11/2003]
						if(pUser->IsSystemUser())
							pUser->m_iStatus = S_WEBAPP;
						//  [9/11/2003]
						
						pUser->TID = m_treebox.AddItem(GroupTID,pUser->GetShowName(),m_ShablonId[IndexEquals[pUser->m_iStatus]+10*pUser->m_bHasNewMessages]);								
						
					}
					else
						pUser->TID = -1;
				}
			}
			
			if(POSITION pos = m_ExternalContactList.InitIteration())
			{
				for(int i=0; m_ExternalContactList.GetNext(pos,pUser); i++)
				{
					if(!m_User.IsOnline()&&!pUser->IsSystemUser())
						pUser->m_iStatus = S_OFFLINE;
					
					if(UserIsVisible(pUser->GetGlobalID()))
					{
						// Step 1. Проверить создавали ли мы группу???
						long	GroupTID	= 0;
						
						pUser->m_nIcon		= -1;
						m_numIco			= pUser->GetIcon2();
						
						if(!m_GroupTIDMap.Lookup(pUser->m_strType,(void*&)GroupTID))
						{
							// Step 2. Если нет, то создать группу .
							GroupTID = m_treebox.AddItem(0,pUser->m_strType,m_ShablonId[0]);
							
							m_GroupTIDMap.SetAt(pUser->m_strType,(void*)GroupTID);
						}
						// Step 3. добавить пользователя [1/28/2002]
						pUser->TID = m_treebox.AddItem(GroupTID,pUser->GetShowName(),m_ShablonId[IndexEquals[S_UNKNOWN]+10*pUser->m_bHasNewMessages]);								
						
					}
					else
						pUser->TID = -1;
				}
			}
			
		}
		break;
	}
	
	//////////////////////////////////////////////////////////////////////////
	// Восстановимм структуру Дерева [11/15/2002]
	
	Pos = m_GroupTIDMap.GetStartPosition();
	while(Pos)
	{
		CString strName;
		long	lGroupTID;
		void *pTmp;
		
		m_GroupTIDMap.GetNextAssoc(Pos,strName,(void*&)lGroupTID);
		
		TRACE(_T("$$$$$ Check Open Group(%s)"), strName);
		
		if(GroupIsOpen.Lookup(strName,pTmp))
		{
			TRACE(_T("$$$$$ Open Group By Command (%s) - %d"), strName,pTmp);
			m_treebox.RootOpen(lGroupTID,(BOOL)pTmp);
		}
	}
	
	//////////////////////////////////////////////
	/// Восстановим Плавующие Диалоги ...
	try
	{
		
		if(POSITION pos = m_ContactList.InitIteration())
		{
			for(int i=0; m_ContactList.GetNext(pos,pUser); i++)
			{
				CPoint point;
				if(pUser->TID!=-1&&m_MapPoint.Lookup(pUser->GetGlobalID(),point))
				{
					//SendMessage(WM_SHOWWINDOW,0,0);
					m_treebox.CreateFloat2(pUser->TID,point.x,point.y);
				}
			}
		}
		
	}
	catch(...)
	{
	}
	m_MapPoint.RemoveAll();
	//////////////////////////////////////////////
	//TRACE(_T("\r\n  CMainDlg::BuildContactList End ...");
}

void CMainDlg::BuildContactListToUser(long GlobalID)
{
	//TRACE(_T("\r\n  CMainDlg::BuildContactListToUSer Start ...");
	long inUserID = m_User.GetGlobalID();
	int m_numIco;
	
	//////////////////////////////////////////////////////////////////////////
	BOOL bContactListMode = TRUE;
	
	CUser* pUser = m_ContactList.GetAt(GlobalID);
	if(pUser==NULL)
	{
		bContactListMode = FALSE;
		pUser = m_ExternalContactList.GetAt(GlobalID);
	}
	//////////////////////////////////////////////////////////////////////////
	
	CMap <long,long,CPoint,CPoint> m_MapPoint;
	//////////////////////////////////////////////
	/// Сделаем проверку нету ли Плавующих и востановим их позициию
	try
	{
		if(pUser!=NULL)
		{
			if(m_treebox.FloatingEnable(pUser->TID))
			{
				CPoint point;
				m_treebox.GetFloatPos(pUser->TID,&(point.x),&(point.y));
				m_MapPoint.SetAt(pUser->GetGlobalID(),point);
			}
		}
	}
	catch(...)
	{
	}
	
	//////////////////////////////////////////////////////////////////////////
	CMapStringToPtr		GroupIsOpen;
	// Сохраним позиции открыта группа или нет [2/25/2002]
	
	// Load Groups State From Reg [11/14/2002]
	CString	strUserId;
	strUserId.Format(_T("%d"), GetUserID());
	
	CString strClosedGroupsId = theApp.GetProfileString(GetString(IDS_INFO) + _T("\\") + GetUserRole()+_T("\\")+strUserId,GetString(IDS_GROUPS_STATE));
	
	if(!strClosedGroupsId.IsEmpty())
	{
		int iPos = 0, iEndPos;
		
		while((iEndPos = strClosedGroupsId.Find(_T(";"),iPos))!=-1)
		{
			CString strGroupName = strClosedGroupsId.Mid(iPos,iEndPos-iPos);
			//TRACE(_T("$$$$$ Extract Group String (%s)",strGroupName);
			GroupIsOpen.SetAt(strGroupName,(LPVOID)FALSE);
			iPos = iEndPos + 1;
		}
	}
	
	// Step 2. Считать текущее и модифицировать [3/14/2002]
	POSITION Pos = m_GroupTIDMap.GetStartPosition();
	if(Pos)
	{
		//TRACE(_T("$$$$$ Open Group String (%s)",strClosedGroupsId);
		
		while(Pos)
		{
			CString strName;
			long	lGroupTID;
			m_GroupTIDMap.GetNextAssoc(Pos,strName,(void*&)lGroupTID);
			GroupIsOpen.SetAt(strName,(void*)m_treebox.RootIsOpen(lGroupTID));
			
			// Group Id Addon [11/14/2002]
			CString strGroupID = strName + _T(";");
			strClosedGroupsId.Replace(strGroupID, _T(""));
			
			if(!m_treebox.RootIsOpen(lGroupTID))
			{
				strClosedGroupsId += strGroupID;
			}
			// Group Id [11/14/2002]
		}
		
		//TRACE(_T("$$$$$ Save Group String (%s)",strClosedGroupsId);
		
		theApp.WriteProfileString(GetString(IDS_INFO) + _T("\\") + GetUserRole()+_T("\\")+strUserId,GetString(IDS_GROUPS_STATE),strClosedGroupsId);
	}
	
	if(pUser->TID!=-1)
	{
		long ParantTID = m_treebox.ParentItem(pUser->TID);
		
		m_treebox.DeleteItem(pUser->TID);
		
		if(ParantTID!=-1&&!m_treebox.HasItemChild(ParantTID))
		{
			CString Key;
			long	lValue;
			
			POSITION pos = m_GroupTIDMap.GetStartPosition();
			
			while(pos!=NULL)
			{
				m_GroupTIDMap.GetNextAssoc(pos,Key,(void*&)lValue);
				if(lValue==ParantTID)
				{
					m_GroupTIDMap.RemoveKey(Key);
					break;
				}
			}
			
			m_treebox.DeleteItem(ParantTID);
		}
	}
	
	if(bContactListMode)
	{
		if(!m_User.IsOnline()&&!pUser->IsSystemUser())
			pUser->m_iStatus = S_OFFLINE;
		
		if(UserIsVisible(pUser->GetGlobalID()))
		{
			// Step 1. Проверить создавали ли мы группу???
			long	GroupTID	= 0;
			
			pUser->m_nIcon		= -1;
			m_numIco			= pUser->GetIcon2();
			
			int CLMode  = GetOptionInt(IDS_OFSMESSENGER,IDS_CLMODE,2);
			
			switch(CLMode) 
			{
			case 1:
				{
					if(!m_GroupTIDMap.Lookup(pUser->m_strType,(void*&)GroupTID))
					{
						// Step 2. Если нет, то создать группу .
						GroupTID = m_treebox.AddItem(0,pUser->m_strType,m_ShablonId[0]);
						
						m_GroupTIDMap.SetAt(pUser->m_strType,(void*)GroupTID);
					}
					
					//////////////////////////////////////////////////////////////////////////
					void *pTmp	=	NULL;
					if(GroupIsOpen.Lookup(pUser->m_strType,pTmp))
					{
						m_treebox.RootOpen(GroupTID,(BOOL)pTmp);
					}
					//////////////////////////////////////////////////////////////////////////
				}
				break;
			case 2:
				{
					if(pUser->m_iStatus==S_OFFLINE||pUser->m_iStatus==S_INVISIBLE)
					{
						if(!m_GroupTIDMap.Lookup(GetString(IDS_OFFLINE),(void*&)GroupTID))
						{
							long ShablonId[10] = {0L,1L,0L,0L,0L,0L,0L,0L,0L,0L};
							// Step 2. Если нет, то создать группу .
							GroupTID = m_treebox.AddItem(0,GetString(IDS_OFFLINE),ShablonId);
							
							m_GroupTIDMap.SetAt(GetString(IDS_OFFLINE),(void*)GroupTID);
						}
						//m_treebox.RootOpen(GroupTID,FALSE);
						
						//////////////////////////////////////////////////////////////////////////
						void *pTmp	=	NULL;
						if(GroupIsOpen.Lookup(GetString(IDS_OFFLINE),pTmp))
						{
							m_treebox.RootOpen(GroupTID,(BOOL)pTmp);
						}
						//////////////////////////////////////////////////////////////////////////
					}
					else
					{
						if(!m_GroupTIDMap.Lookup(pUser->m_strType,(void*&)GroupTID))
						{
							// Step 2. Если нет, то создать группу .
							GroupTID = m_treebox.AddItem(0,pUser->m_strType,m_ShablonId[0]);
							
							m_GroupTIDMap.SetAt(pUser->m_strType,(void*)GroupTID);
						}
						
						//////////////////////////////////////////////////////////////////////////
						void *pTmp	=	NULL;
						if(GroupIsOpen.Lookup(pUser->m_strType,pTmp))
						{
							m_treebox.RootOpen(GroupTID,(BOOL)pTmp);
						}
						//////////////////////////////////////////////////////////////////////////
					}
				}
				break;
			}
			
			// Step 3. добавить пользователя [1/28/2002]
			pUser->TID = m_treebox.AddItem(GroupTID,pUser->GetShowName(),m_ShablonId[IndexEquals[pUser->m_iStatus]+10*pUser->m_bHasNewMessages]);								
			
		}
		else
			pUser->TID = -1;
	}
	else
	{
		if(!m_User.IsOnline()&&!pUser->IsSystemUser())
			pUser->m_iStatus = S_OFFLINE;
		
		if(UserIsVisible(pUser->GetGlobalID()))
		{
			// Step 1. Проверить создавали ли мы группу???
			long	GroupTID	= 0;
			
			pUser->m_nIcon		= -1;
			m_numIco			= pUser->GetIcon2();
			
			if(!m_GroupTIDMap.Lookup(pUser->m_strType,(void*&)GroupTID))
			{
				// Step 2. Если нет, то создать группу .
				GroupTID = m_treebox.AddItem(0,pUser->m_strType,m_ShablonId[0]);
				
				m_GroupTIDMap.SetAt(pUser->m_strType,(void*)GroupTID);
			}
			// Step 3. добавить пользователя [1/28/2002]
			pUser->TID = m_treebox.AddItem(GroupTID,pUser->GetShowName(),m_ShablonId[IndexEquals[S_UNKNOWN]+10*pUser->m_bHasNewMessages]);								
			
		}
		else
			pUser->TID = -1;
	}
	
	
	//////////////////////////////////////////////////////////////////////////
	// Восстановимм структуру Дерева [11/15/2002]
	
	Pos = m_GroupTIDMap.GetStartPosition();
	while(Pos)
	{
		CString strName;
		long	lGroupTID;
		void *pTmp;
		
		m_GroupTIDMap.GetNextAssoc(Pos,strName,(void*&)lGroupTID);
		
		TRACE(_T("$$$$$ Check Open Group(%s)"), strName);
		
		if(GroupIsOpen.Lookup(strName,pTmp))
		{
			TRACE(_T("$$$$$ Open Group By Command (%s) - %d"), strName,pTmp);
			m_treebox.RootOpen(lGroupTID,(BOOL)pTmp);
		}
	}
	
	//////////////////////////////////////////////
	/// Восстановим Плавующие Диалоги ...
	try
	{
		
		if(POSITION pos = m_ContactList.InitIteration())
		{
			for(int i=0; m_ContactList.GetNext(pos,pUser); i++)
			{
				CPoint point;
				if(pUser->TID!=-1&&m_MapPoint.Lookup(pUser->GetGlobalID(),point))
				{
					//SendMessage(WM_SHOWWINDOW,0,0);
					m_treebox.CreateFloat2(pUser->TID,point.x,point.y);
				}
			}
		}
		
	}
	catch(...)
	{
	}
	m_MapPoint.RemoveAll();
	//////////////////////////////////////////////
	//TRACE(_T("\r\n  CMainDlg::BuildContactListToUSer End ...");
}


//-------------------------------------------------------------------------------
// Name: LoadUsers
// Desc: Загружает из Коллекции контак лист.
//-------------------------------------------------------------------------------
void CMainDlg::LoadUsers(IUsers *pUsers)
{
	long iArraySize = 0L;
	HRESULT hr = pUsers->get_Count(&iArraySize);
	
	CUserCollection	CopyContactList;
	
	GetCopyContactList(CopyContactList);
	
	m_ContactList.Clear();
	m_ExternalContactList.Clear();
	
	for(long i=1;i<=iArraySize;i++)
	{
		IUserPtr pUser = pUsers->GetItem(i);
		CUser user(pUser),  *tmpUser;
		
		if(tmpUser = CopyContactList.GetAt(user.GetGlobalID()))
		{
			user.m_bHasNewMessages = tmpUser->m_bHasNewMessages;
			user.TID			   = tmpUser->TID;
			
			// Fix lost status problem [12/2/2004]
			if(user.m_dwStatusTime<tmpUser->m_dwStatusTime)
			{
				user.m_dwStatusTime = tmpUser->m_dwStatusTime;
				user.m_iStatus		= tmpUser->m_iStatus;
			}
			// End Fix
			
			tmpUser = NULL;
			CopyContactList.Delete(user);
		}
		
		m_ContactList.SetAt (user);
	}
	
	if(POSITION pos = CopyContactList.InitIteration())
	{
		CUser *pUser =  NULL;	
		HWND	hWnd =	NULL;
		while(CopyContactList.GetNext(pos, pUser))
		{
			if(pUser->m_bHasNewMessages||
				m_SendMessageDlgMap.Lookup(pUser->GetGlobalID(),hWnd)||
				m_ComeMessageDlgMap.Lookup(pUser->GetGlobalID(),hWnd)||
				m_SplitMessageDlgMap.Lookup(pUser->GetGlobalID(),hWnd)||
				m_DelUserDlgMap.Lookup(pUser->GetGlobalID(),hWnd)||
				m_AddUserDlgMap.Lookup(pUser->GetGlobalID(),hWnd))
			{
				m_ExternalContactList.SetAt(*pUser);
			}
		}
	}
	
	BuildContactList();
}

//-------------------------------------------------------------------------------
// Name: ChangeUserStatus
// Desc: Обрабатывает изменение статуса у внешнего пользователя.
//-------------------------------------------------------------------------------
void CMainDlg::ChangeUserStatus(IUser *&pUser)
{
	///////////////////////////////////////////////////////////////////////
	CUser tmpUser(pUser);
    //s = tmpUser;
    
	CUser *puser = m_ContactList.GetAt(tmpUser.GetGlobalID());
	
	if(puser)
	{
		if(puser->m_dwStatusTime>tmpUser.m_dwStatusTime)
			return;
		
		// Fix: lost status problem [12/2/2004]
		puser->m_dwStatusTime = tmpUser.m_dwStatusTime;
	}
	
	if(LocalDataBaseEnable()&&GetOptionInt(IDS_OFSMESSENGER,IDS_SHOWFRIENDSSTATUS,TRUE))
	{
		try
		{
			m_LocalHistory->AddStatus(tmpUser.GetGlobalID(),tmpUser.GetStatus(),tmpUser.m_dwStatusTime);
		}
		catch(_com_error&)
		{
		}
		
		HWND	hDlgWnd	=	NULL;
		if(m_SplitMessageDlgMap.Lookup(tmpUser.GetGlobalID(),hDlgWnd)&&::IsWindow(hDlgWnd))
		{
			/// Да Открыт
			CMcWindowAgent Agent(hDlgWnd);
			Agent.Refresh();
		}
	}
	
	if(puser&&/*GetOptionInt(IDS_OFSMESSENGER,IDS_CLMODE,2)==2&&*/
		( (puser->m_iStatus==S_OFFLINE||puser->m_iStatus==S_INVISIBLE)&&
		(tmpUser.m_iStatus!=S_OFFLINE||tmpUser.m_iStatus!=S_INVISIBLE) ||
		(puser->m_iStatus!=S_OFFLINE||puser->m_iStatus!=S_INVISIBLE)&&
		(tmpUser.m_iStatus==S_OFFLINE||tmpUser.m_iStatus==S_INVISIBLE)) )
	{
		m_ContactList.SetAt(tmpUser);
		// Try fix "flittering of the contact list" [3/19/2003]
		BuildContactListToUser(tmpUser.GetGlobalID());
		
		// Old Code [3/19/2003]
		//BuildContactList();
	}
	else
	{
		puser = m_ExternalContactList.GetAt(tmpUser.GetGlobalID());
		if(puser)
		{
			tmpUser.m_bHasNewMessages	=	puser->m_bHasNewMessages;
			m_ExternalContactList.Delete(tmpUser);
		}
		
		m_ContactList.SetAt(tmpUser);
		puser = m_ContactList.GetAt(tmpUser.GetGlobalID());
		
		if(puser)
			UpdateID(tmpUser.GetGlobalID(),puser->m_bHasNewMessages);
	}
}

//-------------------------------------------------------------------------------
// Name: UpdateID
// Desc: Меняет визуальный эффект в ContactList'e в зависимости от ситуации 
//-------------------------------------------------------------------------------
void CMainDlg::UpdateID(long UserId,BOOL bHasNewMessage )
{
	CUser *puser = m_ContactList.GetAt(UserId);
	if(puser)
	{	
		puser->m_bHasNewMessages = bHasNewMessage;
		if(puser->TID==-1&&UserIsVisible(puser->GetGlobalID())||
			puser->TID!=-1&&!UserIsVisible(puser->GetGlobalID()))
		{
			// Try fix "flittering of the contact list" [3/19/2003]
			BuildContactListToUser(puser->GetGlobalID());
			
			// Old Code [3/19/2003]
			//BuildContactList();
		}
		
		if(puser->TID!=-1)
			m_treebox.SetItemId(puser->TID,m_ShablonId[IndexEquals[puser->m_iStatus]+10*puser->m_bHasNewMessages]);
	}
	else
	{
		puser = m_ExternalContactList.GetAt(UserId);
		if(puser)
		{
			puser->m_bHasNewMessages = bHasNewMessage;
			puser->m_bHasNewMessages = bHasNewMessage;
			if(puser->TID==-1&&UserIsVisible(puser->GetGlobalID())||
				puser->TID!=-1&&!UserIsVisible(puser->GetGlobalID()))
			{
				// Try fix "flittering of the contact list" [3/19/2003]
				BuildContactListToUser(puser->GetGlobalID());
				
				// Old code [3/19/2003]
				//BuildContactList();
			}
			if(puser->TID!=-1)
				m_treebox.SetItemId(puser->TID,m_ShablonId[IndexEquals[S_UNKNOWN]+10*puser->m_bHasNewMessages]);
		}
	}
}

//-------------------------------------------------------------------------------
// Name: СreateTree
// Desc: Создает Дерево в ContactList'e, загружате настройки.
//-------------------------------------------------------------------------------
void CMainDlg::CreateTree()
{
	LoadSkins m_LoadSkin;
	
	IStreamPtr pStream = NULL;
	long Error=0;
	m_LoadSkin.Load(bstr_t(IBN_SCHEMA)+bstr_t((LPCTSTR)GetProductLanguage())+bstr_t("/Shell/Main/status.bmp"),&pStream,&Error);
	if(pStream)
	{	
		CDib dib(pStream);
		CPaintDC dc(this);
		HBITMAP hBmp = dib.GetHBITMAP((HDC)dc);
		m_treebox.SetImageList((long)hBmp);
		if(hBmp)
			DeleteObject(hBmp);
	}
	
	short PriorityIndex[10];
	for(int i=0;i<10;i++)
		PriorityIndex[i] = -1;
	PriorityIndex[0] = 1;
	PriorityIndex[1] = 0;
	
	m_treebox.SetPriority(PriorityIndex);
	
	for(int i = 0 ;i<MaxValueID;i++)
	{
		m_treebox.AddEffect(m_ShablonId[i],m_ShablonIcon[i],m_ShablonRGBTextEnable[i],
			m_ShablonRGBTextSelect[i],m_ShablonRGBFonEnable[i],m_ShablonRGBFonSelect[i]);
	}
	
	m_treebox.SetEventMode(1);
	
}

//-------------------------------------------------------------------------------
// Name: LogOff
// Desc: 
//-------------------------------------------------------------------------------
void CMainDlg::LogOff()
{
	try
	{
		SaveFloating();
		
		m_GroupTIDMap.RemoveAll();
		m_treebox.DeleteTree();
		
		DisconnectAllChats();
		ClearAll();
	}
	catch(...)
	{
		ASSERT(FALSE);
	}
	try
	{
		pSession->LogOff();
	}
	catch(_com_error&)
	{
		ASSERT(FALSE);
	}
	
	/////////////////////////////////////////////////////
	// Закрыть Локальную Базу данных ....
	/////////////////////////////////////////////////////
	if(LocalDataBaseEnable())
	{
		BOOL bInit = CloseLocalDataBase();
	}
	/////////////////////////////////////////////////////
}

//-------------------------------------------------------------------------------
// Name: ConnectEnable
// Desc: Проверяет наличие соединения с сервером
//-------------------------------------------------------------------------------
BOOL CMainDlg::ConnectEnable(BOOL bShowMessage)
{
	BOOL bRetValue = (dwCurrentStatus!=W_DISCONNECTED)?TRUE:FALSE;
	if(bShowMessage&&!bRetValue)
	{
		SendMessage(WM_SHOWMESSAGEBOX,(WPARAM)IDS_YOUOFFLINE);
	}
	//return (dwCurrentStatus==W_CONNECTED)?TRUE:FALSE;
	return bRetValue;
}

//-------------------------------------------------------------------------------
// Name: OnMenuTreectrl
// Desc: Обрабатывает Event от Контакт Листа - Меню
//-------------------------------------------------------------------------------
void CMainDlg::OnMenuTreectrl(long TID, BOOL bGroupe) 
{
	//TRACE(_T("\r\n CMainDlg::OnMenuTreectrl");
	CurrTID = TID;
	if(TID!= -1)
		if(!bGroupe)
		{
			CUser *pCurrUser = FindUserInVisualContactList(CurrTID);
			
			if(pCurrUser!=NULL&&pCurrUser->IsSystemUser())
			{
				CPoint point;
				GetCursorPos(&point);
				CMenu menu;
				menu.LoadMenu(IDR_MESSENGER_MENU);
				CMenu* popup = menu.GetSubMenu(8);
				
				UpdateMenu(this,popup);
				
				popup->TrackPopupMenu(TPM_LEFTALIGN|TPM_RIGHTBUTTON, point.x, point.y, this);
			}
			else
			{
				CPoint point;
				GetCursorPos(&point);
				CMenu menu;
				menu.LoadMenu(IDR_MESSENGER_MENU);
				CMenu* popup = menu.GetSubMenu(0);
				
				UpdateMenu(this,popup);
				
				// Load Conference List For Curr User [10/15/2002]
				CMenu *pConferenceMenu = popup->GetSubMenu(9);
				if(pConferenceMenu)
				{
					CMenu InviteListMenu;
					
					InviteListMenu.CreatePopupMenu();
					
					LoadConferenceListForCurrUser(&InviteListMenu);
					
					pConferenceMenu->AppendMenu(MF_POPUP|MF_STRING,(UINT)InviteListMenu.Detach(),GetString(IDS_INVITE_TO_MENU_ITEM));
				}
				// end [10/15/2002]
				
				// Load Message Templates
				CString strSection;
				strSection.Format(_T("%s\\%s\\%d"),GetString(IDS_INFO),GetUserRole(),GetUserID());
				
				CString strMessageTemplateXML = GetRegFileText(strSection,GetString(IDS_MESSAGE_TEMPLATES_REG));
				
				if(strMessageTemplateXML.IsEmpty())
					strMessageTemplateXML = GetString(IDS_DEFAULT_MES_TEMPLATE_XML);
				
				
				CComPtr<IXMLDOMDocument>	pMTDoc	=	NULL;
				pMTDoc.CoCreateInstance(CLSID_DOMDocument40,NULL,CLSCTX_INPROC_SERVER);
				
				CMenu *pMessageTemplatesMenu = popup->GetSubMenu(4);
				
				if(pMTDoc&&pMessageTemplatesMenu!=NULL)
				{
					CMenu MessageTmplateListMenu;
					MessageTmplateListMenu.CreatePopupMenu();
					
					
					CComBSTR bsXML;
					bsXML.Attach(strMessageTemplateXML.AllocSysString());
					
					VARIANT_BOOL	varLoad	=	VARIANT_FALSE;
					
					pMTDoc->loadXML(bsXML,&varLoad);
					
					if(varLoad==VARIANT_TRUE)
					{
						CComPtr<IXMLDOMNodeList>	pTemplatesList	=	NULL;
						
						pMTDoc->selectNodes(CComBSTR(L"message_templates/mt"),&pTemplatesList);
						
						if(pTemplatesList!=NULL)
						{
							USES_CONVERSION;
							
							long ListLength	=	0;
							pTemplatesList->get_length(&ListLength);
							
							for(int Index=0;Index<ListLength;Index++)
							{
								CComBSTR	bsName, bsText;
								
								CComPtr<IXMLDOMNode>	pStubNode	=	NULL;
								pTemplatesList->get_item(Index,&pStubNode);
								
								GetTextByPath(pStubNode, CComBSTR(L"name"),&bsName);
								
								pMessageTemplatesMenu->AppendMenu(MF_STRING,21100+Index,W2CT(bsName));
							}
						}
						
						pMessageTemplatesMenu->DeleteMenu(0,MF_BYPOSITION);
						
					}
				}
				// end
				
				popup->TrackPopupMenu(TPM_LEFTALIGN|TPM_RIGHTBUTTON, point.x, point.y, this);
			}
		}
		else
		{
			m_strGroupName = m_treebox.GetItemText(TID);
			
			if(m_strGroupName!=GetString(IDS_SYSTEM_SERVICES_GROUP_NAME))
			{
				CPoint point;
				GetCursorPos(&point);
				CMenu menu;
				menu.LoadMenu(IDR_MESSENGER_MENU);
				CMenu* popup = menu.GetSubMenu(4);
				UpdateMenu(this,popup);
				
				// Load Conference List For Curr User [10/15/2002]
				CMenu *pConferenceMenu = popup->GetSubMenu(9);
				if(pConferenceMenu)
				{
					CMenu InviteListMenu;
					
					InviteListMenu.CreatePopupMenu();
					
					for(int iChatItem = 0;iChatItem<m_ChatCollection.GetSize();iChatItem++)
					{
						CChat Chat	=	m_ChatCollection.GetAt(iChatItem);
						
						if(Chat.GetStatus()==SC_ACTIVE)
						{
							InviteListMenu.AppendMenu(MF_STRING,20700+iChatItem,Chat.GetShowName());
						}
					}
					
					pConferenceMenu->AppendMenu(MF_POPUP|MF_STRING,(UINT)InviteListMenu.Detach(),GetString(IDS_INVITE_TO_MENU_ITEM));
				}
				// end [10/15/2002]

								// Load Message Templates
				CString strSection;
				strSection.Format(_T("%s\\%s\\%d"),GetString(IDS_INFO),GetUserRole(),GetUserID());
				
				CString strMessageTemplateXML = GetRegFileText(strSection,GetString(IDS_MESSAGE_TEMPLATES_REG));
				
				if(strMessageTemplateXML.IsEmpty())
					strMessageTemplateXML = GetString(IDS_DEFAULT_MES_TEMPLATE_XML);
				
				
				CComPtr<IXMLDOMDocument>	pMTDoc	=	NULL;
				pMTDoc.CoCreateInstance(CLSID_DOMDocument40,NULL,CLSCTX_INPROC_SERVER);
				
				CMenu *pMessageTemplatesMenu = popup->GetSubMenu(4);
				
				if(pMTDoc&&pMessageTemplatesMenu!=NULL)
				{
					CMenu MessageTmplateListMenu;
					MessageTmplateListMenu.CreatePopupMenu();
					
					
					CComBSTR bsXML;
					bsXML.Attach(strMessageTemplateXML.AllocSysString());
					
					VARIANT_BOOL	varLoad	=	VARIANT_FALSE;
					
					pMTDoc->loadXML(bsXML,&varLoad);
					
					if(varLoad==VARIANT_TRUE)
					{
						CComPtr<IXMLDOMNodeList>	pTemplatesList	=	NULL;
						
						pMTDoc->selectNodes(CComBSTR(L"message_templates/mt"),&pTemplatesList);
						
						if(pTemplatesList!=NULL)
						{
							USES_CONVERSION;
							
							long ListLength	=	0;
							pTemplatesList->get_length(&ListLength);
							
							for(int Index=0;Index<ListLength;Index++)
							{
								CComBSTR	bsName, bsText;
								
								CComPtr<IXMLDOMNode>	pStubNode	=	NULL;
								pTemplatesList->get_item(Index,&pStubNode);
								
								GetTextByPath(pStubNode, CComBSTR(L"name"),&bsName);
								
								pMessageTemplatesMenu->AppendMenu(MF_STRING,21300+Index,W2CT(bsName));
							}
						}
						
						pMessageTemplatesMenu->DeleteMenu(0,MF_BYPOSITION);
						
					}
				}
				// end
				
				popup->TrackPopupMenu(TPM_LEFTALIGN|TPM_RIGHTBUTTON, point.x, point.y, this);
			}
		}
}

//-------------------------------------------------------------------------------
// Name: OnSelectTreectrl
// Desc: Обрабатывает Event от Контак листа - Изменение Выделенного пользователя
//-------------------------------------------------------------------------------
void CMainDlg::OnSelectTreectrl(long TID, BOOL bGroupe) 
{
	//TRACE(_T("\r\n CMainDlg::OnSelectTreectrl");
	CurrTID = TID;
	if(TID!= -1)
		if(bGroupe)
		{
			CString	strUserId;
			strUserId.Format(_T("%d"), GetUserID());
			
			
			// Save ChatGroup State [11/14/2002]
			CString strOpenGroupsId = theApp.GetProfileString(GetString(IDS_INFO) + _T("\\") + GetUserRole()+_T("\\")+strUserId,GetString(IDS_GROUPS_STATE));
			
			POSITION Pos = m_GroupTIDMap.GetStartPosition();
			while(Pos)
			{
				CString strName;
				long	lGroupTID;
				m_GroupTIDMap.GetNextAssoc(Pos,strName,(void*&)lGroupTID);
				
				CString strGroupID = strName + _T(";");
				strOpenGroupsId.Replace(strGroupID, _T(""));
				
				if(!m_treebox.RootIsOpen(lGroupTID))
				{
					strOpenGroupsId += strGroupID;
				}
			}
			
			theApp.WriteProfileString(GetString(IDS_INFO) + _T("\\") + GetUserRole()+_T("\\")+strUserId,GetString(IDS_GROUPS_STATE),strOpenGroupsId);
		}	
		
}

//-------------------------------------------------------------------------------
// Name: OnActionTreectrl
// Desc: Обрабатывает Event от Контакт Листа Действие
//-------------------------------------------------------------------------------
void CMainDlg::OnActionTreectrl(long TID, BOOL bGroupe) 
{
	//TRACE(_T("\r\n CMainDlg::OnActionTreectrl");
	CurrTID = TID;
	if(TID!= -1)
		if(!bGroupe)
		{
			CUser *pUser=FindUserInVisualContactList(TID);
			if(!pUser->IsSystemUser()||pUser->m_bHasNewMessages)
				SendMessageToUser(pUser);
		}
}

//-------------------------------------------------------------------------------
// Name: FindUserInVisualContactList
// Desc: Находит пользователя в контакт листе по TID'у.
//-------------------------------------------------------------------------------
CUser* CMainDlg::FindUserInVisualContactList(long TID)
{
	if(POSITION pos = m_ContactList.InitIteration())
	{
		CUser *pUser=NULL;
		
		while(m_ContactList.GetNext(pos,pUser))
		{
			if(pUser->TID == TID )
				return pUser;
		}
	}
	
	if(POSITION pos = m_ExternalContactList.InitIteration())
	{
		CUser *pUser=NULL;
		
		while(m_ExternalContactList.GetNext(pos, pUser))
		{
			if(pUser->TID == TID )
				return pUser;
		}
	}
	
	return NULL;
}

CUser* CMainDlg::FindUserInVisualContactListByGlobalId(long UserId)
{
	if(POSITION pos = m_ContactList.InitIteration())
	{
		CUser *pUser=NULL;
		
		while(m_ContactList.GetNext(pos,pUser))
		{
			if(pUser->GetGlobalID() == UserId)
				return pUser;
		}
	}
	
	if(POSITION pos = m_ExternalContactList.InitIteration())
	{
		CUser *pUser=NULL;
		
		while(m_ExternalContactList.GetNext(pos,pUser))
		{
			if(pUser->GetGlobalID() == UserId)
				return pUser;
		}
	}
	
	return NULL;
}


//-------------------------------------------------------------------------------
// Name: SendMessageToUser
// Desc: В зависимости от ситуации показывает Диалог отправки сообщения пользователю
// или Пришедшее от пользователя сообщение или Показывает Следующее сообщений 
//-------------------------------------------------------------------------------
void CMainDlg::SendMessageToUser(CUser *pUser,BOOL bInAnswer, LPCTSTR strBody, BOOL SendAuto)
{
	//TRACE(_T("\r\n CMainDlg::SendMessageToUser");
	if(pUser == NULL||pUser->m_iStatus == S_OFFLINESEPARATOR ||pUser->m_iStatus == S_ONLINESEPARATOR)
		return;
	
	CMessageSplitDlg3	*pSlit3Dlg	=	NULL;
	
	HWND				hDlgWnd		=	NULL;
	CMessage			*pMsg		=	NULL;  
	long				Key			=	pUser->GetGlobalID();
	
	int DialogMode = 0;//GetOptionInt(IDS_OFSMESSENGER,IDS_DIALOGMODE,DM_McMessengerSundanceEdition);
	
	switch(1)//GetOptionInt(IDS_OFSMESSENGER,IDS_MESSSAGEMODE,1))
	{
		/// SplitMode
	case 1:
		/// Удалить все не прочитанные Сессаги для Данного пользователя.
		BOOL bDelete = FALSE;
		try
		{
			while((pMsg = FindMessageByIDAndDel(Key))!=NULL)
			{
				///// Пометить что мессага прочитана ...
				MarkMessagesAsRead(bstr_t(pMsg->GetMessageID()));
				delete pMsg;pMsg = NULL;
				bDelete = TRUE;
			}
			
		}
		catch(...)
		{
			ASSERT(FALSE);
		}
		
		//////////////////////////////////////////////////////////////////////////
		CString strCaption;
		strCaption.Format(GetString(IDS_INSTANT_CHAT_TITLE_FORMAT),pUser->GetShowName());
		
		HWND hTestWnd	=	::FindWindow(NULL,strCaption);
		//////////////////////////////////////////////////////////////////////////
		
		if(m_SplitMessageDlgMap.Lookup(Key,hDlgWnd)&&::IsWindow(hDlgWnd))
		{
			/// Да Открыт
			CMcWindowAgent Agent(hDlgWnd);
			Agent.Refresh();
			if(strBody)
				Agent.Action(WM_SWM_SETBODY,(WPARAM)strBody,(LPARAM)SendAuto);
			Agent.ShowWindow(SW_SHOWNORMAL);
			Agent.SetForegroundWindow();
		}
		else
		{
			//////////////////////////////////////////////////////////////////////////
			// ... Test String for EUG [5/8/2002]
			//if(hTestWnd!=NULL)
			//{
			//	int *i;	i = 0;*i	=	1000;
			//}
			// ... Test String for EUG [5/8/2002]
			//////////////////////////////////////////////////////////////////////////
			
			/// Нет нету
			//switch(DialogMode)
			//{
			//case DM_McMessengerSundanceEdition:
			//	{
			if(LocalDataBaseEnable()&&GetOptionInt(IDS_OFSMESSENGER,IDS_CLEARCHATWINDOW,TRUE))
			{
				try
				{
					m_LocalHistory->ResetStatuses(Key);
					//m_LocalHistory->ResetMessages(Key);
				}
				catch (...) 
				{
				}
			}
			TRACE(_T("\r\n CMainDlg::SendMessageToUser new CMessageSplitDlg3"));
			pSlit3Dlg = new CMessageSplitDlg3(this);
			pSlit3Dlg->Create(GetDesktopWindow());
			m_SplitMessageDlgMap.SetAt(Key,pSlit3Dlg->GetSafeHwnd());
			
			///////////////////////////////////////////////////////////////////////
			pSlit3Dlg->SetRecipient(*pUser);
			pSlit3Dlg->SetSender(m_User);
			pSlit3Dlg->Refresh();
			if(strBody)
			{
				//pSlit3Dlg->SetBody(strBody);
				pSlit3Dlg->SendMessage(WM_SWM_SETBODY,(WPARAM)strBody,(LPARAM)SendAuto);
			}

			pSlit3Dlg->ShowWindow(SW_SHOWNORMAL);
			pSlit3Dlg->SetForegroundWindow();
			
			//}
			//break;
			//}
			
			
		}
		
		if(bDelete)
			UpdateID(pUser->GetGlobalID(),FALSE);
		
		break;
	}
	
    PostMessage(WM_CHANGE_NEWMESSAGE,(WPARAM)m_NewMessageArray.GetSize());
}

//-------------------------------------------------------------------------------
// Name: NewMessage
// Desc: Пришло новое сообщение от пользователя.
//-------------------------------------------------------------------------------
void CMainDlg::NewMessage(CMessage *pMsg)
{
	TRACE(_T("\r\n CMainDlg::NewMessage"));
	//CMessageComeDlg2	*pComeDlg = NULL;
	//CMessageSplitDlg2	*pSlitDlg = NULL;
	HWND				hDlgWnd =  NULL;
	///////////////////////////////////////////////////////////////////////
    //	Sound(SND_INCOMING_MESSAGE);
	
	CUser Sender = pMsg->GetSender();
	long Key = Sender.GetGlobalID();
	
	
	
	//BOOL bFlagOpen = FALSE;
	
	///switch(1)//GetOptionInt(IDS_OFSMESSENGER,IDS_MESSSAGEMODE,1))
	//{
	//case 1:
	//	{
	//////////////////////////////////////////////////////////////////////////
	CString strCaption;
	strCaption.Format(GetString(IDS_INSTANT_CHAT_TITLE_FORMAT),Sender.GetShowName());
	
	HWND hTestWnd	=	::FindWindow(NULL,strCaption);
	//////////////////////////////////////////////////////////////////////////
	
	if(m_SplitMessageDlgMap.Lookup(Key,hDlgWnd)&&::IsWindow(hDlgWnd))
	{
		//bFlagOpen = TRUE;
		/// Да Открыт -> обновить
		
		CMcWindowAgent Agent(hDlgWnd);
		if(::GetForegroundWindow()!=hDlgWnd)
		{
			m_NewMessageArray.Add(pMsg);
			UpdateID(pMsg->GetSender().GetGlobalID(),TRUE);
			//::FlashWindow(hDlgWnd,TRUE);

			FLASHWINFO flashInfo = {0};
			flashInfo.cbSize = sizeof(FLASHWINFO);
			flashInfo.hwnd = hDlgWnd;
			flashInfo.dwFlags = FLASHW_ALL;
			flashInfo.uCount  = 5;
			flashInfo.dwTimeout   = 500;

			::FlashWindowEx(&flashInfo);
		}
		else
		{
			MarkMessagesAsRead(_bstr_t((LPCTSTR)pMsg->GetMessageID()));
			delete pMsg; pMsg = NULL;
		}
		
		Agent.Refresh();
	}
	else
	{
		
		if((GetUserStatus() != S_DND)&& 
			(GetUserStatus() != S_AWAY) && 
			(GetUserStatus() != S_NA) &&
			GetOptionInt(IDS_OFSMESSENGER, IDS_SHOWNEW, 1) ) 
		{
			if(LocalDataBaseEnable()&&GetOptionInt(IDS_OFSMESSENGER,IDS_CLEARCHATWINDOW,TRUE))
			{
				try
				{
					m_LocalHistory->ResetStatuses(Key);
				}
				catch (...) 
				{
				}
			}
			
			CMessageSplitDlg3 *pSlit3Dlg	=	NULL;
			
			CWnd *pFrgWindow = GetForegroundWindow();
			
			// Step 1. Sozdaetsa Split okno [2/21/2002]
			pSlit3Dlg = new CMessageSplitDlg3(this,GetDesktopWindow());
			TRACE(_T("\r\n CMainDlg::NewMessage new CMessageSplitDlg3"));
			
			pSlit3Dlg->Create(GetDesktopWindow(),TRUE);
			//pSlit3Dlg->EnableWindow(FALSE);
			
			m_SplitMessageDlgMap.SetAt(Key,pSlit3Dlg->GetSafeHwnd());
			
			pSlit3Dlg->SetRecipient(Sender);
			pSlit3Dlg->SetSender(m_User);
			///pSlit3Dlg->Refresh();
			
			pSlit3Dlg->ShowWindow(SW_SHOWMINNOACTIVE);
			//pSlit3Dlg->ShowWindow(SW_MINIMIZE);
			//pSlit3Dlg->ShowWindow(SW_SHOWNA);
			TRACE(_T("\r\n CMainDlg::NewMessage ShowWindow(SW_SHOWMINNOACTIVE)"));
			//if(pFrgWindow)
			//{
			//	pFrgWindow->SetForegroundWindow();
			//	pFrgWindow->SetWindowPos(&wndTop,0,0,0,0,SWP_NOSIZE|SWP_NOMOVE|SWP_SHOWWINDOW);
			//}
			//pSlit3Dlg->EnableWindow(TRUE);
			//::FlashWindow(pSlit3Dlg->GetSafeHwnd(),TRUE);

			
			FLASHWINFO flashInfo = {0};
			flashInfo.cbSize = sizeof(FLASHWINFO);
			flashInfo.hwnd = pSlit3Dlg->GetSafeHwnd();
			flashInfo.dwFlags = FLASHW_ALL;
			flashInfo.uCount  = 5;
			flashInfo.dwTimeout   = 500;

			::FlashWindowEx(&flashInfo);
		}
		
		m_NewMessageArray.Add(pMsg);
		UpdateID(pMsg->GetSender().GetGlobalID(),TRUE);
	}
	
	PostMessage(WM_CHANGE_NEWMESSAGE,(WPARAM)m_NewMessageArray.GetSize());
	
	TRACE(_T("\r\n CMainDlg::NewMessage end"));
}

//-------------------------------------------------------------------------------
// Name: OnKillSendMessageDlg
// Desc: Очищает Память выделенную под Диалог.
//-------------------------------------------------------------------------------
LRESULT CMainDlg::OnKillSendMessageDlg(WPARAM w, LPARAM l)
{
	long             lKey = (long)w;
	m_SendMessageDlgMap.RemoveKey(lKey);
	if(!UserIsVisible(lKey))
	{
		// Try fix "flittering of the contact list" [3/19/2003]
		BuildContactListToUser(lKey);
		
		// Old code [3/19/2003]
		//BuildContactList();
	}
	return 0;
}

LRESULT CMainDlg::OnKillDelUserMessageDlg(WPARAM w, LPARAM l)
{
	long             lKey = (long)w;
	m_DelUserDlgMap.RemoveKey(lKey);
	if(!UserIsVisible(lKey))
	{
		// Try fix "flittering of the contact list" [3/19/2003]
		BuildContactListToUser(lKey);
		
		// Old code [3/19/2003]
		//BuildContactList();
	}
	return 0;
}

LRESULT CMainDlg::OnKillAddUserMessageDlg(WPARAM w, LPARAM l)
{
	long             lKey = (long)w;
	m_AddUserDlgMap.RemoveKey(lKey);
	if(!UserIsVisible(lKey))
	{
		// Try fix "flittering of the contact list" [3/19/2003]
		BuildContactListToUser(lKey);
		
		// Old code [3/19/2003]
		//BuildContactList();
	}
	return 0;
}


LRESULT CMainDlg::OnKillComeMessageDlg(WPARAM w, LPARAM l)
{
	long             lKey = (long)w;
	m_ComeMessageDlgMap.RemoveKey(lKey);
	if(!UserIsVisible(lKey))
	{
		// Try fix "flittering of the contact list" [3/19/2003]
		BuildContactListToUser(lKey);
		
		// Old code [3/19/2003]
		//BuildContactList();
	}
	return 0;
}

LRESULT CMainDlg::OnKillSplitMesageDlg(WPARAM w, LPARAM l)
{
	long             lKey = (long)w;
	m_SplitMessageDlgMap.RemoveKey(lKey);
	if(LocalDataBaseEnable()&&GetOptionInt(IDS_OFSMESSENGER,IDS_CLEARCHATWINDOW,TRUE))
	{
		try
		{
			m_LocalHistory->ResetMessages(lKey);
		}
		catch (...) 
		{
		}
	}
	if(!UserIsVisible(lKey))
	{
		// Try fix "flittering of the contact list" [3/19/2003]
		BuildContactListToUser(lKey);
		
		// Old code [3/19/2003]
		//BuildContactList();
	}
	
	return 0;
}

LRESULT CMainDlg::OnKillInFileDlg(WPARAM w, LPARAM l)
{
	return 0;
}

//-------------------------------------------------------------------------------
// Name: FindUserById
// Desc: Ищет сообщение от пользователя - ID
//-------------------------------------------------------------------------------
CMessage* CMainDlg::FindMessageByID(long ID)
{
	int  Size = m_NewMessageArray.GetSize();
	
	for(int i = 0;i<Size;i++)
	{
		if(m_NewMessageArray[i]->GetSender().GetGlobalID()==ID)
			return m_NewMessageArray[i];
	}
	return NULL;
}

//-------------------------------------------------------------------------------
// Name: FindUserByIdAndDell
// Desc: Ищет сообщение от пользователя - ID и удаляет его из очереди
//-------------------------------------------------------------------------------
CMessage* CMainDlg::FindMessageByIDAndDel(long Id)
{
	int  Size = m_NewMessageArray.GetSize();
	
	for(int i = 0;i<Size;i++)
	{
		if(m_NewMessageArray[i]->GetSender().GetGlobalID()==Id)
		{
			CMessage* pMsg = m_NewMessageArray[i];
			m_NewMessageArray.RemoveAt(i);
			return pMsg;
		}
	}
	return NULL;
}

void CMainDlg::OnUpdateTreemenuSendmessage(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(ConnectEnable(FALSE));
}

void CMainDlg::OnTreemenuSendmessage() 
{
	CUser *pUser = FindUserInVisualContactList(CurrTID);
	if(pUser)
		SendMessageToUser(pUser);
}

void CMainDlg::OnUpdateTreemenuSendfile(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(ConnectEnable(FALSE));	
}

void CMainDlg::OnTreemenuSendfile() 
{
    CUser *pUser = FindUserInVisualContactList(CurrTID);
	if(pUser)
		SendFileToUser(pUser);
}

//-------------------------------------------------------------------------------
// Name: SendFileToUser
// Desc: Выбор отпровляемого файла.
//-------------------------------------------------------------------------------
void CMainDlg::SendFileToUser(CUser *pUser)
{
	if(pUser == NULL) 	return;
	
	CFileDialog Open(TRUE, NULL, _T("*.*"), NULL, GetString(IDS_OPEN_FILE_NAME), this);
	
    if(Open.DoModal () == IDOK) 
	{
		CString FileName = Open.GetPathName();
		
		CString strDescription;
		CFileDescriptioDlg	DescrDlg(this);
		DescrDlg.m_strFileName = Open.GetFileName();
		if(DescrDlg.DoModalEditMode()==IDOK)
		{
			strDescription = DescrDlg.GetDescription();	
			m_FileManager.AddToUpLoad (FileName,pUser->GetShowName(),pUser->GetGlobalID(),strDescription);
		}
	}	
}

//-------------------------------------------------------------------------------
// Name: GetUserStatus
// Desc: Запрашивает текущий статус пользхователя.
//-------------------------------------------------------------------------------
int CMainDlg::GetUserStatus()
{
	return m_User.m_iStatus;
}

//-------------------------------------------------------------------------------
// Name: OnChangeStatus
// Desc: Изменяет текущий статус пользователя.
//-------------------------------------------------------------------------------
HRESULT CMainDlg::OnChangeStatus(WPARAM w, LPARAM l)
{
	int NewStatus = (int)w;
	
	if(NewStatus != m_User.m_iStatus && ConnectEnable(FALSE))
	{
		try
		{
			pSession->ChangeStatus(LONG(NewStatus));
			//m_User.m_iStatus = NewStatus;
		}
		catch(...)
		{
		}
	}
	else if(!ConnectEnable(FALSE))
		Login2(NewStatus);

	return 0;
}

//-------------------------------------------------------------------------------
// Name: OnDhtmlEvent
// Desc: Обрабатывает Сообщения из Скрипта из Внешних Функций.
//-------------------------------------------------------------------------------
HRESULT CMainDlg::OnDhtmlEvent(WPARAM w, LPARAM l)
{
	DHTM_Event_Type m_Type = (DHTM_Event_Type)w;
	
    DHTMLE_ADDCONTACT_Container *m_pAddContact  = NULL;
	DHTMLE_SENDMESSAGE_Container *m_pSendMessage = NULL;
	
	if(!ConnectEnable())
	{
		/// Write Message ...
		switch(m_Type)
		{
		case DHTMLE_ADDCONTACT:
			m_pAddContact = (DHTMLE_ADDCONTACT_Container *)l;		
			delete m_pAddContact;
			m_pAddContact = NULL;
			break;
		case DHTMLE_SENDMESSAGE:
			m_pSendMessage  = (DHTMLE_SENDMESSAGE_Container*)l;
			delete m_pSendMessage;
			m_pSendMessage  =NULL;
			break;
		case DHTMLE_SENDFILE:
			m_pSendMessage  = (DHTMLE_SENDMESSAGE_Container*)l;
			delete m_pSendMessage;
			m_pSendMessage  =NULL;
			break;
		default:
			ASSERT(FALSE);
		}
		
		return 0;
	}
	
	CUser user;
	switch(m_Type)
	{
	case DHTMLE_ADDCONTACT:
		m_pAddContact = (DHTMLE_ADDCONTACT_Container *)l;
		ASSERT(m_pAddContact!=NULL);
		if(m_pAddContact)
		{
			if(CheckUserInContactList(m_pAddContact->user_id)==NULL)
			{
				HWND hDlgWnd	=	NULL;
				if(m_AddUserDlgMap.Lookup(m_pAddContact->user_id,hDlgWnd)&&::IsWindow(hDlgWnd))
				{
					CMcWindowAgent Agent(hDlgWnd);			
					Agent.ShowWindow(SW_SHOWNORMAL);
					Agent.SetForegroundWindow();
				}
				else
				{
					CAddUserDlg *pAddDlg	=new CAddUserDlg(this);
					
					pAddDlg->Create(CAddUserDlg::IDD,GetDesktopWindow());
					pAddDlg->AddNewContact(m_pAddContact->user_id,m_pAddContact->nick_name,
						m_pAddContact->first_name,m_pAddContact->last_name,m_pAddContact->email,
						0L,_T(""),
						m_pAddContact->role_id,m_pAddContact->role_name);
					pAddDlg->ShowWindow(SW_SHOWNORMAL);
					pAddDlg->SetForegroundWindow();
					m_AddUserDlgMap.SetAt(m_pAddContact->user_id,pAddDlg->GetSafeHwnd());
				}
			}
			else
			{
				//CMessageDlg m_IgnoryDlg(IDS_CANTADDUSER);
				//m_IgnoryDlg.Show("User is already in your Contact List",MB_OK);
				MessageBox(GetString(IDS_USER_ALLREADY_IN_CONTACTLIST_NAME),GetString(IDS_ERROR_TITLE),MB_OK|MB_ICONSTOP);
			}
		}			
		delete m_pAddContact;
		m_pAddContact = NULL;
		break;
	case DHTMLE_SENDMESSAGE:
		m_pSendMessage  = (DHTMLE_SENDMESSAGE_Container*)l;
		
		if(GetUserByGlobalId(m_pSendMessage->longUserID,user))
		{
			SendMessageToUser(&user);
		}
		
		delete m_pSendMessage;
		m_pSendMessage  =NULL;
		break;
	case DHTMLE_SENDFILE:
		m_pSendMessage  = (DHTMLE_SENDMESSAGE_Container*)l;
		//user.GlobalID = m_pSendMessage->longUserID;
		if(GetUserByGlobalId(m_pSendMessage->longUserID,user))
		{
			SendFileToUser(&user);
		}
		else
		{
			//CString str;
			//str.Format("Can't Find User ( %s [%s] ) in OFS Messenger Contact List",m_pSendMessage->strNickName,m_pSendMessage->strRole);
			//AfxMessageBox(str);
		}
		delete m_pSendMessage;
		m_pSendMessage  =NULL;
		break;
	default:
		ASSERT(FALSE);
	}
	return 0;
}

long CMainDlg::GetUserID()
{
	return m_User.GetGlobalID();
}

long CMainDlg::GetRoleID()
{
	return m_User.m_RoleID;
}

CUser& CMainDlg::GetCurrentUser()
{
	return m_User;
}

void CMainDlg::OnLButtonDblClk(UINT nFlags, CPoint point) 
{
	// For Test Float App  [4/3/2002]
#ifdef _DEBUG
	//Beep(100,100);
	//NLT_Container *pItem = new NLT_Container;
	//pItem->EventType = NLT_ESysMess;
	//pItem->Long1 = 4;
	//pItem->String1 = L"<alert stub_id=\"1\"><app>Time Tracker</app><title>Time Tracker Title</title><description>AAA bndsbf djhsdkjh dsfhsdkj dshfksjd. Asfsd asdjask.</description><url>http://messenger.mediachase.net</url></alert>";
	//Sleep(3000);
	//OnNetEvent((WPARAM)pItem ,0);
	
	//	Invoke_StartAutoUpdate(_T("#1.0.17.0#"));
#endif
}


CMainDlg* CMainDlg::GetMessageParent()
{
	return this;
}

CString CMainDlg::GetShowName()
{
	return m_User.GetShowName();
}

//void CMainDlg::OnTreemenuAddtoignory() 
//{
//	CUser *pUser = FindUserInVisualContactList(CurrTID);
//	if(pUser)
//	{
//		CMessageDlg m_IgnoryDlg(IDS_ADDTOIGNORY);
//		
//		CString strMessage;
//		strMessage.Format("Do you really want %s add to Ignory.",pUser->GetShowName());
//		
//		if(m_IgnoryDlg.Show(strMessage,MB_YESNO)==IDYES)
//		{
//			AfxMessageBox("Not Implemented.");
//			/*
//			theNet2.LockTranslator();
//			try
//			{
//			long Handle = pSession->AddUser(pUser->GetGlobalID(),bstr_t("Move to Ignory"),2);
//			if(Handle)
//			{
//			theNet2.AddToTranslator(Handle,this->GetSafeHwnd());					
//			}
//			}
//			catch(...)
//			{
//			}
//			theNet2.UnlockTranslator();*/
//			
//		}
//	}

//}

//void CMainDlg::OnUpdateTreemenuAddtoignory(CCmdUI* pCmdUI) 
//{
//	pCmdUI->Enable(ConnectEnable(FALSE));
//	
//}

void CMainDlg::OnTreemenuDeleteuser() 
{
	CUser *pUser = FindUserInVisualContactList(CurrTID);
	if(pUser)
	{
		HWND hDlgWnd	=	NULL;
		if(m_DelUserDlgMap.Lookup(pUser->GetGlobalID(),hDlgWnd)&&::IsWindow(hDlgWnd))
		{
			CMcWindowAgent Agent(hDlgWnd);			
			Agent.ShowWindow(SW_SHOWNORMAL);
			Agent.SetForegroundWindow();
		}
		else
		{
			CDelUserDlg *pDelDlg	=	new CDelUserDlg(this);
			pDelDlg->Create(CDelUserDlg::IDD,GetDesktopWindow());
			pDelDlg->SetKillUser(*pUser);
			pDelDlg->ShowWindow(SW_SHOWNORMAL);
			pDelDlg->SetForegroundWindow();
			m_DelUserDlgMap.SetAt(pUser->GetGlobalID(),pDelDlg->GetSafeHwnd());
		}
	}
}

void CMainDlg::OnUpdateTreemenuDeleteuser(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(ConnectEnable(FALSE));
}

void CMainDlg::OnTreemenuFloatingon() 
{
	BOOL bFlag = m_treebox.FloatingEnable(CurrTID);
	if(bFlag)
		m_treebox.DeleteFloat(CurrTID);
	else
		m_treebox.CreateFloat(CurrTID);
}

void CMainDlg::OnUpdateTreemenuFloatingon(CCmdUI* pCmdUI) 
{
	BOOL bFlag = m_treebox.FloatingEnable(CurrTID);
	
	pCmdUI->Enable();
	
	if(bFlag)
		pCmdUI->SetText(GetString(IDS_FLOATING_OFF_NAME));
	else
		pCmdUI->SetText(GetString(IDS_FLOATING_ON_NAME));
	
}

void CMainDlg::OnTreemenuMessageshistory() 
{
	//// Show Messages History
	CUser *pUser = FindUserInVisualContactList(CurrTID);
	if(pUser)
		ShowHistoryForUser(*pUser);
	
}

void CMainDlg::OnUpdateTreemenuMessageshistory(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(LocalDataBaseEnable());
	
}

void CMainDlg::OnTreemenuUserdetails() 
{
	/// Запросить дополнительную информацию ...
	CUser *pUser = FindUserInVisualContactList(CurrTID);
	if(pUser)
	{
		ShowUserDetails(pUser->GetGlobalID());
	}
}

void CMainDlg::OnUpdateTreemenuUserdetails(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(ConnectEnable(FALSE));
	///pCmdUI->Enable(FALSE);
}

void CMainDlg::DeleteFromContact(long GlobalID)
{
	BOOL    bContactListMode = TRUE;
	long	UserTID	= -1;
	long	UserStatus;
	CString	UserGroup;
	
	CUser *pUser = m_ContactList.GetAt(GlobalID);
	
	if(pUser!=NULL)
	{
		UserTID		= pUser->TID;
		UserStatus	= pUser->m_iStatus;
		UserGroup	= pUser->m_strType;	
		
		bContactListMode = TRUE;
		
		m_ContactList.Delete(*pUser);
	}
	else
	{
		pUser = m_ExternalContactList.GetAt(GlobalID);
		if(pUser!=NULL)
		{
			UserTID = pUser->TID;
			UserStatus	= pUser->m_iStatus;
			UserGroup	= pUser->m_strType;	
			
			bContactListMode = FALSE;
			
			m_ExternalContactList.Delete(*pUser);
		}
	}
	
	RemoveAllMessageById(m_User.GetGlobalID());
	
	if(UserTID!=-1)
	{
		long ParantTID = m_treebox.ParentItem(UserTID);
		
		m_treebox.DeleteItem(UserTID);
		
		if(ParantTID!=-1&&!m_treebox.HasItemChild(ParantTID))
		{
			CString Key;
			long	lValue;
			
			POSITION pos = m_GroupTIDMap.GetStartPosition();
			
			while(pos!=NULL)
			{
				m_GroupTIDMap.GetNextAssoc(pos,Key,(void*&)lValue);
				if(lValue==ParantTID)
				{
					m_GroupTIDMap.RemoveKey(Key);
					break;
				}
			}
			
			m_treebox.DeleteItem(ParantTID);
		}
	}
}

/*
void CMainDlg::OnSendMessageBtn()
{
//CMessageSplitDlg m_Dlt(this);
//m_Dlt.DoModal();
CUser *pUser = FindUserInVisualContactList(CurrTID);
if(pUser)
SendMessageToUser(pUser);
}
*/

/*
void CMainDlg::OnSendFileBtn()
{
CUser *pUser = FindUserInVisualContactList(CurrTID);
if(pUser)
SendFileToUser(pUser);

  }
*/

/*
void CMainDlg::OnAddPeopleBtn()
{
if(::IsWindow(GetSafeHwnd()))
PostMessage(WM_SHOW_ADDUSER);
}*/


//-------------------------------------------------------------------------------
// Name: Refresh
// Desc: Обновляет Внешний вид, сделан во избежание бага с появлением кнопок
// Виндов в некоторых ситуациях.
//-------------------------------------------------------------------------------
void CMainDlg::Refresh()
{
	Invalidate(FALSE);
}

void CMainDlg::OnLButtonDown(UINT nFlags, CPoint point) 
{
	USES_CONVERSION;
	
	int NowSelect	= m_AppBar.m_AppCtrl.GetCheckButton();
	CPoint TstPoint	=	point;
	ClientToScreen(&TstPoint);
	m_AppBar.ScreenToClient(&TstPoint);
	int SelItem		= m_AppBar.m_AppCtrl.OnLButtonDown(nFlags, TstPoint);
	if(SelItem!=-1)
	{
		//m_btnAlerts.SetPressed(FALSE);
		
		// Activate  SelItem Action [2/1/2002]
		if(m_AppArray.GetSize()>SelItem)
		{
			McAppItem Item = m_AppArray[SelItem];
			
			if(Item.Type==APPT_CHAT_CONTACTLIST)
			{
				//m_WebFolderView.ShowWindow(SW_HIDE);
				m_treebox.ShowWindow(SW_HIDE);
				//m_InWindow.ShowWindow(SW_HIDE);
				//m_InWindow.Navigate(_T("IBN_SCHEMA://default/Common/blank.html"));
				if(m_ChatCollection.GetSize()==0)
				{
					m_chatbox.ShowWindow(SW_HIDE);
					m_InWindow.ShowWindow(SW_SHOW);
					m_InWindow.Navigate(CString(IBN_SCHEMA)+GetProductLanguage()+CString(_T("/Shell/Conference/blank.html")));
				}
				else
				{
					m_chatbox.ShowWindow(SW_SHOW);
					m_InWindow.ShowWindow(SW_HIDE);
					m_InWindow.Navigate(CString(IBN_SCHEMA)+GetProductLanguage()+CString(_T("/Common/blank.html")));
				}
			}
			else if(Item.Type==APPT_CONTACTLIST)
			{
				//m_WebFolderView.ShowWindow(SW_HIDE);
				m_chatbox.ShowWindow(SW_HIDE);
				m_treebox.ShowWindow(SW_SHOW);
				m_InWindow.ShowWindow(SW_HIDE);
				m_InWindow.Navigate(CString(IBN_SCHEMA)+GetProductLanguage()+CString(_T("/Common/blank.html")));
			}
/*			else if(Item.Type==APPT_IBN_ACTIONS)
			{
				m_chatbox.ShowWindow(SW_HIDE);
				m_treebox.ShowWindow(SW_HIDE);
				m_InWindow.ShowWindow(SW_SHOW);
				m_InWindow.SetFocus();
				m_InWindow.Navigate(Item.Url);
			}*/
			else if(Item.Type==APPT_INWINDOW)
			{
				//m_WebFolderView.ShowWindow(SW_HIDE);
				//m_chatbox.ShowWindow(SW_HIDE);
				//m_treebox.ShowWindow(SW_HIDE);
				//m_InWindow.ShowWindow(SW_SHOW);
				//m_InWindow.SetFocus();
				//m_InWindow.Navigate(Item.Url);
				
				m_AppBar.m_AppCtrl.SetCheckButton(NowSelect);
				
				if(m_InWindow.NavigateNewWindow(Item.Url)!=S_OK)
					ShellExecute(NULL,_T("open"),Item.Url,NULL,NULL,SW_SHOWNORMAL);
				
				Invalidate();
				
			}
			else if(Item.Type==APPT_BROWSEWINDOW)
			{
				m_AppBar.m_AppCtrl.SetCheckButton(NowSelect);
				
				if(m_InWindow.NavigateNewWindow(Item.Url)!=S_OK)
					ShellExecute(NULL,_T("open"),Item.Url,NULL,NULL,SW_SHOWNORMAL);
				
				Invalidate();
			}
			else if(Item.Type==APPT_EXWINDOW)
			{
				m_AppBar.m_AppCtrl.SetCheckButton(NowSelect);
				CWebWindow *pChild = new CWebWindow();
				pChild->CreateAutoKiller(_T("/Browser/Common/skin.xml"), GetMessageParent(), GetDesktopWindow(), CW_USEDEFAULT, 0, Item.Width, Item.Height, NULL, Item.Url, FALSE, FALSE, TRUE);
				m_AllClosedWindow.Add(pChild->GetSafeHwnd());
				Invalidate();
				//				m_AppBar.m_AppCtrl.SetCheckButton(NowSelect);
				//				
				//				if(m_InWindow.NavigateNewWindow(Item.Url)!=S_OK)
				//					ShellExecute(NULL,_T("open"),Item.Url,NULL,NULL,SW_SHOWNORMAL);
				//				
				//				Invalidate();
			}
			/*else if(Item.Type==APPT_WEB_FOLDERS)
			{
			///m_WebFolderView.ShowWindow(SW_SHOW);
			m_chatbox.ShowWindow(SW_HIDE);
			m_treebox.ShowWindow(SW_HIDE);
			m_InWindow.ShowWindow(SW_HIDE);
			m_InWindow.Navigate(_T("IBN_SCHEMA://default/Common/blank.html"));
			
			  CString strConferencesUrl = _T("#host#/Intranet/#domain#/Documents");
			  
				strConferencesUrl.Replace("#sid#",GetSID());
				strConferencesUrl.Replace("#host#",GetServerPath());
				strConferencesUrl.Replace("#domain#",GetUserDomain());
				
				  CString strDescription;
				  strDescription.Format(_T("IBN Documents on %s"),GetUserDomain());
				  
					LPITEMIDLIST	pPIDL	=	NULL;
					
					  //if(S_OK == McCoCreateWFPIDL(T2CW(strDescription),T2CW(strConferencesUrl),&pPIDL))
					  //{
					  //m_WebFolderView.Destroy();
					  //m_WebFolderView.Navigate2(pPIDL);
					  
						//CComPtr<IMalloc>	pMalloc	=	NULL;
						//SHGetMalloc(&pMalloc);
						//pMalloc->Free(pPIDL);
						//}
			}*/
		}
		return;	
	}
	else
	{
		CRect UserStNameRect,  StatusStName;
		m_UserStatic.GetWindowRect(&UserStNameRect);
		ScreenToClient(&UserStNameRect);
		m_StatusStatic.GetWindowRect(&StatusStName);
		ScreenToClient(&StatusStName);
		if(UserStNameRect.PtInRect(point)||StatusStName.PtInRect(point))
		{
			return;
		}
	}
	COFSNcDlg2::OnLButtonDown(nFlags, point);
}

void CMainDlg::OnLButtonUp(UINT nFlags, CPoint point) 
{
	CPoint TstPoint	=	point;
	ClientToScreen(&TstPoint);
	m_AppBar.ScreenToClient(&TstPoint);
	
	m_AppBar.m_AppCtrl.OnLButtonUp(nFlags, TstPoint);
	
	CRect UserStNameRect,  StatusStName;
	m_UserStatic.GetWindowRect(&UserStNameRect);
	ScreenToClient(&UserStNameRect);
	m_StatusStatic.GetWindowRect(&StatusStName);
	ScreenToClient(&StatusStName);
	if(UserStNameRect.PtInRect(point)||StatusStName.PtInRect(point))
	{
		CPoint WinPoint  = point;
		ClientToScreen(&WinPoint);
		CMenu menu,*pSubMenu;
		menu.LoadMenu(IDR_OFS_MENU);
		pSubMenu = menu.GetSubMenu(1);
		UpdateMenu(this,pSubMenu);
		pSubMenu->TrackPopupMenu(TPM_LEFTALIGN|TPM_RIGHTBUTTON, WinPoint.x, WinPoint.y, this);
	}
	
	COFSNcDlg2::OnLButtonUp(nFlags, point);
}


HRESULT CMainDlg::OnCheckSignalState(WPARAM w, LPARAM l)
{
	int NewMessageSize = m_NewMessageArray.GetSize();
	if(NewMessageSize)
	{
		CUser m_UserMessage = m_NewMessageArray[0]->GetSender();
		SendMessageToUser(m_ContactList.GetAt(m_UserMessage.GetGlobalID()));
		return 0;
	}
	return 1;
}

//-------------------------------------------------------------------------------
// Name: InitLocalDataBase
// Desc: Инициализирует Локальную Базу данных для Хистори
//-------------------------------------------------------------------------------
BOOL CMainDlg::InitLocalDataBase(LPCTSTR path,BOOL bReindex	/*=	FALSE*/)
{
	m_LocalHistory.CreateInstance(CLSID_ComHistInt);
	
	WIN32_FIND_DATA	FindData	=	{0};
	HANDLE			hFind		=	FindFirstFile(path,&FindData);
	
	if(hFind==INVALID_HANDLE_VALUE)
	{
		try
		{
			m_LocalHistory->CreateNew(_bstr_t(path));
		}
		catch(_com_error&)
		{
		}
	}
	else
		FindClose(hFind);
	
	try
	{
		// Step 1. Try Connect in the normal mode [6/14/2002]
		m_LocalHistory->Connect(_bstr_t(path), bstr_t(m_User.GetShowName()), m_User.GetGlobalID() , bstr_t(""),bReindex?VARIANT_TRUE:VARIANT_FALSE);
		bEnableLocalHistory = TRUE;
	}
	catch(_com_error&)
	{
		try
		{
			// Step 2. Try Connect in the reindex mode [6/14/2002]
			m_LocalHistory->Connect(_bstr_t(path), bstr_t(m_User.GetShowName()), m_User.GetGlobalID() , bstr_t(""),VARIANT_TRUE);
			bEnableLocalHistory = TRUE;
		}
		catch(_com_error&) 
		{
			CString strMessageText;
			
			try
			{
				// Step 2. Try create new DB [6/14/2002]
				m_LocalHistory->CreateNew(_bstr_t(path));
				m_LocalHistory->Connect(_bstr_t(path), bstr_t(m_User.GetShowName()), m_User.GetGlobalID() , bstr_t(""),VARIANT_FALSE);
				bEnableLocalHistory = TRUE;
				
				strMessageText.LoadString(IDS_LOCDB_FIXED_PROBLEM);
			}
			catch(_com_error&) 
			{
				// Data base Crash :( Call to suport [6/14/2002] 
				strMessageText.LoadString(IDS_LOCDB_GLOABL_ERROR);
				
				m_LocalHistory		= NULL;
				bEnableLocalHistory = FALSE;
				ASSERT(FALSE);
			}
			
			MessageBox(strMessageText,GetString(IDS_ERROR_TITLE),MB_OK|MB_ICONERROR);
		}
	}
	return bEnableLocalHistory;
}

BOOL CMainDlg::LocalDataBaseEnable()
{
	return bEnableLocalHistory;
}

//-------------------------------------------------------------------------------
// Name: CloseLocalDataBase
// Desc: Закрывает Локальную Базу Данных для Хистори.
//-------------------------------------------------------------------------------
BOOL CMainDlg::CloseLocalDataBase()
{
	try
	{
		m_LocalHistory->Disconnect();
		m_LocalHistory = NULL;
		bEnableLocalHistory = FALSE;
	}
	catch(_com_error&)
	{
		ASSERT(FALSE);
		return FALSE;
	}
	return TRUE;
}

//-------------------------------------------------------------------------------
// Name: GetMessages
// Desc: запросит Сообщения по фильтру из Базы данных. pPersist указатель на документ
// в Браузере куда будет выведен результат запроса, так же загружается их скина XSLT.
//-------------------------------------------------------------------------------
BOOL CMainDlg::GetMessages(bstr_t &FriendName, long FriendID, long Sorted, long Type, IUnknown *pPersist)
{
	if(!LocalDataBaseEnable()) return FALSE;
	HRESULT hr = S_OK;
	try
	{
		LoadSkins m_LoadSkins;
		IStreamPtr pStream = NULL;
		bstr_t xsltPath = bstr_t(IBN_SCHEMA) +(LPCTSTR)GetCurrentSkin() + bstr_t("/Browser/xslt/mpa_history.xslt");
		long Error = 0L;
		m_LoadSkins.Load(xsltPath,&pStream,&Error);
		
		if(pStream)
		{
			bstr_t bstrtXSLT;
			
			ULONG pRealyRead	= 0;
			BYTE *pRead			=	NULL;
			
			STATSTG	statStg = {0};
			if(S_OK==pStream->Stat(&statStg,0))
			{
				pRead	=	new BYTE[statStg.cbSize.LowPart+2];
				
				ZeroMemory(pRead,statStg.cbSize.LowPart+2);
				
				pStream->Read((LPVOID)pRead,statStg.cbSize.LowPart,&pRealyRead);
				
				if((pRead[0]==0xFF&&pRead[1]==0xFE)||
					(pRead[0]==0xFE&&pRead[1]==0xFF))
				{
					// Remove Lead FE FF, FF FE
					//if((pRead[0]==0xFF&&pRead[1]==0xFE)||
					//	(pRead[0]==0xFE&&pRead[1]==0xFF))
					bstrtXSLT	=	(LPWSTR)(LPBYTE)(pRead+2);
					//else
					//	bstrtXSLT	=	(LPWSTR)(LPBYTE)pRead;
				}
				else
				{
					if(pRead[0]==0xEF&&pRead[1]==0xBB&&pRead[2]==0xBF)
					{
						int WideSize	=	MultiByteToWideChar(CP_UTF8,0,(LPCSTR)(pRead+3),-1,0,0);
						
						LPWSTR	wsBuff	=	new WCHAR[WideSize];
						MultiByteToWideChar(CP_UTF8,0,(LPCSTR)(pRead+3),-1,wsBuff,WideSize);
						
						bstrtXSLT	=	wsBuff;
						
						delete [] wsBuff;
					}
					else
					{
						int WideSize	=	MultiByteToWideChar(0,0,(LPCSTR)(pRead),-1,0,0);
						
						LPWSTR	wsBuff	=	new WCHAR[WideSize];
						MultiByteToWideChar(0,0,(LPCSTR)(pRead),-1,wsBuff,WideSize);
						
						bstrtXSLT	=	wsBuff;
						
						delete []wsBuff;
					}
				}
				
				
				delete [] pRead;
			}
			
			hr = m_LocalHistory->GetMessages(FriendName, FriendID, Sorted, Type, pPersist,bstrtXSLT);
		}
	}
	catch(...)
	{
		ASSERT(FALSE);
	}
	return TRUE;
}

//-------------------------------------------------------------------------------
// Name: GetMessagesBySID
// Desc: Аналогочно см. функцию выше, добавлен фильтр на SID.
//-------------------------------------------------------------------------------
BOOL CMainDlg::GetMessagesBySID( bstr_t &FriendName, long FriendID, long Sorted, long Type, IUnknown *pPersist, LPCTSTR XSLTPath)
{
	if(!LocalDataBaseEnable()) 
		return FALSE;

	HRESULT hr = S_OK;
	try
	{
		LoadSkins m_LoadSkins;
		IStreamPtr pStream = NULL;
		bstr_t xsltPath = bstr_t(IBN_SCHEMA) +(LPCTSTR)GetCurrentSkin() + bstr_t(XSLTPath);
		long Error = 0L;
		hr = m_LoadSkins.Load(xsltPath,&pStream,&Error);
		if(FAILED(hr))
		{
			CString strMsg;
			strMsg.Format("Load skin (%s) problem Hr = %ld, Error=%ld", (LPCTSTR)xsltPath,hr,Error);

			MessageBox(strMsg,"Error",MB_OK|MB_ICONINFORMATION);
		}
		
		if(pStream)
		{
			bstr_t bstrtXSLT;
			
			ULONG pRealyRead	= 0;
			BYTE *pRead			=	NULL;
			
			STATSTG	statStg = {0};
			if(S_OK==pStream->Stat(&statStg,0))
			{
				pRead	=	new BYTE[statStg.cbSize.LowPart+2];
				
				ZeroMemory(pRead,statStg.cbSize.LowPart+2);
				
				pStream->Read((LPVOID)pRead,statStg.cbSize.LowPart,&pRealyRead);
				
				if((pRead[0]==0xFF&&pRead[1]==0xFE)||
					(pRead[0]==0xFE&&pRead[1]==0xFF))
				{
					// Remove Lead FE FF, FF FE
					//if((pRead[0]==0xFF&&pRead[1]==0xFE)||
					//	(pRead[0]==0xFE&&pRead[1]==0xFF))
					bstrtXSLT	=	(LPWSTR)(LPBYTE)(pRead+2);
					//else
					//	bstrtXSLT	=	(LPWSTR)(LPBYTE)pRead;
				}
				else
				{
					if(pRead[0]==0xEF&&pRead[1]==0xBB&&pRead[2]==0xBF)
					{
						int WideSize	=	MultiByteToWideChar(CP_UTF8,0,(LPCSTR)(pRead+3),-1,0,0);
						
						LPWSTR	wsBuff	=	new WCHAR[WideSize];
						MultiByteToWideChar(CP_UTF8,0,(LPCSTR)(pRead+3),-1,wsBuff,WideSize);
						
						bstrtXSLT	=	wsBuff;
						
						delete [] wsBuff;
					}
					else
					{
						int WideSize	=	MultiByteToWideChar(0,0,(LPCSTR)(pRead),-1,0,0);
						
						LPWSTR	wsBuff	=	new WCHAR[WideSize];
						MultiByteToWideChar(0,0,(LPCSTR)(pRead),-1,wsBuff,WideSize);
						
						bstrtXSLT	=	wsBuff;
						
						delete []wsBuff;
					}
				}
				
				delete [] pRead;
			}
			
			hr = m_LocalHistory->GetRecentMessages(FriendName, FriendID, Sorted, Type, pPersist,bstrtXSLT);
			if(FAILED(hr))
			{
				CString strMsg;
				strMsg.Format("GetRecentMessages problem Hr = %ld", hr);

				MessageBox(strMsg,"Error",MB_OK|MB_ICONINFORMATION);
			}
		}
	}
	catch(_com_error& e)
	{
		hr = e.Error();

		if(FAILED(hr))
		{
			CString strMsg;
			strMsg.Format("GetRecentMessages _com_error problem Hr = %ld", hr);

			MessageBox(strMsg,"Error",MB_OK|MB_ICONINFORMATION);
		}

	}
	return TRUE;
}


//-------------------------------------------------------------------------------
// Name: AddMessagesToDataBase
// Desc: Добавляет исходящие мессаги в базу данных.
//-------------------------------------------------------------------------------
BOOL CMainDlg::AddMessageToDataBase(long ToId, bstr_t &MID, long Time, bstr_t &Body)
{
	if(!LocalDataBaseEnable()) 
		return FALSE;
	
	HRESULT hr = S_OK;
	try
	{
		m_LocalHistory->AddMessage(m_User.GetGlobalID(),ToId,(LPCTSTR)GetSID(), MID,Time, TRUE,Body,VARIANT_TRUE);
		RefreshHistoryFor(ToId);
	}
	catch(_com_error&)
	{
		//_com_error	eTmp	=	e;
		try
		{
			// Try solve DB Error
			// Step 1. Close Local DB
			CloseLocalDataBase();
			// Step 2. Try Init new DB
			if(!LocalDataBaseEnable())
			{
				/************************************************************************/
				/* Load DB Path from Local Machine                                     */
				/************************************************************************/
				CString strData	= GetAppDataDir() + _T("\\History");
				
				CreateDirectory(strData,NULL);
				
				//strData += _T("\\history.dbf");
				// Set DB like UserLogin + .dbf [9/5/2003]
				strData += _T("\\");
				strData += m_User.m_strLogin;
				strData += _T(".dbf");
				
				
				//////////////////////////////////////////////////////////////////////////
				if(InitLocalDataBase(strData,TRUE))
				{
					m_LocalHistory->AddMessage(m_User.GetGlobalID(),ToId,(LPCTSTR)GetSID(), MID,Time, TRUE,Body,VARIANT_TRUE);
					RefreshHistoryFor(ToId);
					return TRUE;
				}
			}
		}
		catch (_com_error&) 
		{
			//eTmp	=	e;
		}
		
		ASSERT(FALSE);
		return FALSE;
	}
	
	return TRUE;
	
}

//-------------------------------------------------------------------------------
// Name: MarkMessagesAsRead
// Desc: Пометить сообщение как прочитанное
//-------------------------------------------------------------------------------
BOOL CMainDlg::MarkMessagesAsRead(bstr_t &MID)
{
	if(!LocalDataBaseEnable()) return FALSE;
	HRESULT hr = S_OK;
	try
	{
		hr = m_LocalHistory->MarkMessagesAsRead(MID);
	}
	catch(...)
	{
		ASSERT(FALSE);
	}
	return TRUE;
	
}

//-------------------------------------------------------------------------------
// Name: RefreshHistoryFor
// Desc: Обновить Хистори для пользователя Id
//-------------------------------------------------------------------------------
void CMainDlg::RefreshHistoryFor(long Id)
{
	///m_HistoryDlg.RefreshIfNowThisUser(Id);
}

//-------------------------------------------------------------------------------
// Name: 
// Desc: 
//-------------------------------------------------------------------------------
/*
void CMainDlg::OnLogoBtn()
{
CMenu m_Menu, *pSubMenu = NULL;
m_Menu.LoadMenu(IDR_MESSENGER_MENU);
pSubMenu = m_Menu.GetSubMenu(2);
CPoint point;
GetCursorPos(&point);
UpdateMenu(this, pSubMenu);

  pSubMenu->TrackPopupMenu(TPM_LEFTALIGN|TPM_RIGHTBUTTON, point.x, point.y, this);
  }
*/

/*
void CMainDlg::OnMessageBtn()
{
OnSendMessageBtn();
}
*/


//-------------------------------------------------------------------------------
// Name: OnOptionsSynchronizehistory
// Desc: Расчитывает интервл времени для Синхронизации в зависимоти от настроек.
//-------------------------------------------------------------------------------
void CMainDlg::OnOptionsSynchronizehistory() 
{
	////// Показ Редактора выбора Типа Синхронизации ...
	CUser *pUser = FindUserInVisualContactList(CurrTID);
	if(pUser)
		m_HistoryDlg.ShowHistory(*pUser);
	else
		m_HistoryDlg.ShowHistory();
}

//-------------------------------------------------------------------------------
// Name: 
// Desc: 
//-------------------------------------------------------------------------------
void CMainDlg::OnUpdateOptionsSynchronizehistory(CCmdUI* pCmdUI) 
{
}

//-------------------------------------------------------------------------------
// Name: LoadNotReadMessage
// Desc: Загрузить не прочитанные Мессаги из Базы Данных.
//-------------------------------------------------------------------------------
BOOL CMainDlg::LoadNotReadMessage()
{
	if(!LocalDataBaseEnable()) return FALSE;
	HRESULT hr = S_OK;
	try
	{
		CComBSTR bstrUnreadMessage;
		hr = m_LocalHistory->GetUnReadMessages(&bstrUnreadMessage);
		if(bstrUnreadMessage.Length())
		{
			/// Add to New Messages ...
			CComPtr<IXMLDOMDocument> m_doc = NULL;
			hr = m_doc.CoCreateInstance(CLSID_DOMDocument40, NULL, CLSCTX_INPROC_SERVER);
			if(SUCCEEDED(hr))
			{
				VARIANT_BOOL vBool = VARIANT_FALSE;
				m_doc->loadXML(bstrUnreadMessage,&vBool);
				
				if (vBool == VARIANT_TRUE)
				{
					//// Распаковка XML ...
					/*<Messages>
					<Message mid="">
					<from_id>
					<to_id>
					<body>
					<time>
					....
					*/
					CComPtr<IXMLDOMNodeList> m_List = NULL;
					m_doc->selectNodes(bstr_t("Messages/Message"),&m_List);
					if(m_List!=NULL)
					{
						USES_CONVERSION;
						
						long lSize = 0;
						m_List->get_length(&lSize);
						
						for(long i=0;i<lSize;i++)
						{
							CComBSTR bFromID, bBody, bTime, bMID;
							long lFromId, lTime;
							
							try
							{
								CComPtr<IXMLDOMElement>  m_Element = NULL;
								CComPtr<IXMLDOMNode> pNode = NULL;
								m_List->get_item(i,&pNode);
								//// Получаем Строки
								pNode->QueryInterface(IID_IXMLDOMElement,(void**)&m_Element);
								CComVariant	varMID;
								
								m_Element->getAttribute(_bstr_t("mid"),&varMID);
								bMID = varMID.bstrVal;
								
								GetTextByPath(pNode,bstr_t("from_id"),&bFromID);
								GetTextByPath(pNode,bstr_t("body"),&bBody);
								GetTextByPath(pNode,bstr_t("time"),&bTime);
								//// Распковка и формирования Новой Мессаги ...
								lFromId = _wtol((BSTR)bFromID);
								lTime   = _wtol((BSTR)bTime);
								CMessage *pMessage = new CMessage;
								
								pMessage->GetSender().GlobalID = lFromId;
								pMessage->SetMessage(bBody);
								pMessage->SetTime((DWORD)lTime);
								pMessage->SetMessageID(W2T(bMID));
								
								CUser *puser = m_ContactList.GetAt(pMessage->GetSender().GetGlobalID());
								if(!puser)
									puser = m_ExternalContactList.GetAt(pMessage->GetSender().GetGlobalID());
								
								if(puser==NULL)
								{
									m_ExternalContactList.SetAt(pMessage->GetSender());
									/// Запросить дополнительную информацию ...
									theNet2.LockTranslator();
									try
									{
										long DetailsHandle = pSession->UserDetails(pMessage->GetSender().GetGlobalID(),1);
										if(DetailsHandle)
											theNet2.AddToTranslator(DetailsHandle,this->GetSafeHwnd());
									}
									catch(...)
									{
										ASSERT(FALSE);
									}
									theNet2.UnlockTranslator();
									// Try fix "flittering of the contact list" [3/19/2003]
									BuildContactListToUser(pMessage->GetSender().GetGlobalID());
									
									// Old code [3/19/2003]
									//BuildContactList();
								}
								else
									pMessage->GetSender() = (*puser);
								
								NewMessage(pMessage);
							}
							catch(...)
							{
							}
						}
					}
				}
			}
		}
	}
	catch(...)
	{
	}
	return TRUE;
}

//-------------------------------------------------------------------------------
// Name: OnOptionsPreferences
// Desc: ...
//-------------------------------------------------------------------------------
void CMainDlg::OnOptionsPreferences() 
{
	PreferenceDlg(this);
}

//-------------------------------------------------------------------------------
// Name: RefreshUserInfo
// Desc: Обновит Информацию о пользователе.
//-------------------------------------------------------------------------------
void CMainDlg::RefreshUserInfo(long ID)
{
	CUser *pUser = FindUserInVisualContactListByGlobalId(ID);
	
	if(pUser)
	{
		HWND				hDlgWnd	=	NULL;
		
		if(m_SendMessageDlgMap.Lookup(ID,hDlgWnd))
		{
			CMcWindowAgent Agent(hDlgWnd);
			if(Agent.IsValid())
			{
				Agent.Action(WM_SET_RECIPIENT,(WPARAM)pUser);
			}
			else
				m_SendMessageDlgMap.RemoveKey(ID);
		}
		
		if(m_ComeMessageDlgMap.Lookup(ID,hDlgWnd))
		{
			CMcWindowAgent Agent(hDlgWnd);
			if(Agent.IsValid())
			{
				Agent.Action(WM_SET_SENDER,(WPARAM)pUser);
			}
			else
				m_ComeMessageDlgMap.RemoveKey(ID);
		}
		
		if(m_SplitMessageDlgMap.Lookup(ID,hDlgWnd))
		{
			CMcWindowAgent Agent(hDlgWnd);
			if(Agent.IsValid())
			{
				Agent.Action(WM_SET_RECIPIENT,(WPARAM)pUser);
			}
			//else
			//	m_SplitMessageDlgMap.RemoveKey(ID);
		}
		
		/*
		for(int i=0;i<m_InFileDlgArray.GetSize();i++)
		{
		FileDownloadDlgItem Item = m_InFileDlgArray[i];
		if(Item.UserGlobalId==ID)
		{
		CMcWindowAgent Agent(Item.hWnd);
		Agent.Action(WM_SET_SENDER,(WPARAM)pUser);
		}
		}
		*/
		
		//m_FileDownloadStatus.RefreshSenderDetails(*pUser);
		m_FileManager.RefreshSenderDetails(*pUser);
	}
}

//-------------------------------------------------------------------------------
// Name: SaveFloating
// Desc: Сохранить настроки Floating.
//-------------------------------------------------------------------------------
void CMainDlg::SaveFloating()
{
	CString strData = "", strTmp;
	try
	{
		CUser* pUser=NULL;
		
		if(POSITION pos = m_ContactList.InitIteration())
		{
			for(int i=0; m_ContactList.GetNext(pos,pUser); i++)
			{
				if(m_treebox.FloatingEnable(pUser->TID))
				{
					CPoint point;
					m_treebox.GetFloatPos(pUser->TID,&(point.x),&(point.y));
					strTmp.Format(_T("%ld,%ld,%ld,"), pUser->GetGlobalID(), point.x, point.y);
					strData += strTmp;
				}
			}
		}
		else
			return;
		
	}
	catch(...)
	{
	}
	Pack(strData,CString("@vTsfO#"));
	WriteOptionString(IDS_OFSMESSENGER,IDS_FLOATING,strData);
}

//-------------------------------------------------------------------------------
// Name: LoadFloating
// Desc: Загрузить из Реестра информацию о Floating.
//-------------------------------------------------------------------------------
void CMainDlg::LoadFloating()
{
	CString strData = GetOptionString(IDS_OFSMESSENGER, IDS_FLOATING, _T(""));
	UnPack(strData,CString("@vTsfO#"));
	try
	{
		CPoint point;
		long Id;
		int FindId;
		
		while(!strData.IsEmpty())
		{
			Id = _tstol((LPCTSTR)strData);
			FindId = strData.Find(',');
			if(FindId==-1) return;
			strData = strData.Mid(FindId+1);
			
			point.x = _tstol((LPCTSTR)strData);
			FindId = strData.Find(',');
			if(FindId==-1) return;
			strData = strData.Mid(FindId+1);
			
			point.y = _tstol((LPCTSTR)strData);
			FindId = strData.Find(',');
			if(FindId==-1) return;
			strData = strData.Mid(FindId+1);
			
			CUser User;
			User.SetGlobalID(Id);
			
			if(m_ContactList.LookUp(User))
			{
				m_treebox.CreateFloat2(User.TID,point.x,point.y);
			}
		}
	}
	catch(...)
	{
	}
}

//-------------------------------------------------------------------------------
// Name: PreferenceDlg
// Desc: Показывает Диалог настроек для данного родителя
//-------------------------------------------------------------------------------
void CMainDlg::PreferenceDlg(CWnd *pParent)
{
	static bool	bShowPreferenceDlg = false;
	
	if(bShowPreferenceDlg)
	{
		SetForegroundWindow();
		return;
	}
	
	int OldCLMode			=	GetOptionInt(IDS_OFSMESSENGER,IDS_CLMODE,2);
	BOOL OldShowOffline	=	GetOptionInt(IDS_OFSMESSENGER,IDS_SHOWOFFLINEFILES,TRUE);
	
	CString	strUserId;
	strUserId.Format(_T("%d"), GetUserID());
	CString	strOldAppPrefernce	=	theApp.GetProfileString(GetString(IDS_INFO) + _T("\\") + GetUserRole()+_T("\\")+strUserId,GetString(IDS_STUBS));
	
	CString	strOldMyDocumetPath	=	GetMyDocumetPath(GetUserRole(),GetUserID());
	
	BOOL bOldRemoveTaskBar	=	GetOptionInt(IDS_OFSMESSENGER,IDS_REMOVE_FROM_TASK_BAR,TRUE);
	
	CPageGeneral			page1(GetString(IDS_GENERAL_NAME));
	//CPageContactListMode	page2(_T("Contact List"));
	CPageStatusMode			page3(GetString(IDS_STATUS_MODE_NAME));
	CPageLaunchMode			page4(GetString(IDS_CONNECTION_NAME));
	CPageEditMode			page5(GetUserRole(),GetUserID(),GetString(IDS_EDIT_NAME));
	CPageChat				page10(GetUserRole(),GetUserID(),GetString(IDS_CONFERENCE_NAME));
	CPageAlerts				page6(GetString(IDS_ALERTS_NAME));
	CPageSound				page7(GetString(IDS_SOUNDS_NAME));
	CPageHistorySync		page8(GetString(IDS_HISTORY_SYNC_NAME));
	CPageApps				page9(m_AppArray,GetUserID(),GetUserRole(),GetString(IDS_APPLICATION_NAME));
	
	CPageMesTemplates		page11(GetUserID(),GetUserRole(),GetString(IDS_MESSAGE_TEMPLATES_NAME));
	
	CMcSettingsDlg dlg;
	
	dlg.AddPage(&page1);
	//dlg.AddPage(&page2);
	dlg.AddPage(&page3);
	dlg.AddPage(&page4);
	dlg.AddPage(&page5);
	dlg.AddPage(&page10);
	dlg.AddPage(&page6);
	dlg.AddPage(&page7);
	dlg.AddPage(&page8);
	dlg.AddPage(&page9);
	dlg.AddPage(&page11);
	
	bShowPreferenceDlg = true;
	dlg.DoModal();
	bShowPreferenceDlg = false;
	
	if(OldCLMode  != GetOptionInt(IDS_OFSMESSENGER,IDS_CLMODE,2)||
		OldShowOffline	!=	GetOptionInt(IDS_OFSMESSENGER,IDS_SHOWOFFLINEFILES,TRUE))
	{
		m_bShowOffline = GetOptionInt(IDS_OFSMESSENGER,IDS_SHOWOFFLINEFILES,TRUE);
		BuildContactList();
	}
	
	if(GetOptionInt(IDS_OFSMESSENGER, IDS_KEEPTOP,FALSE))
		SetWindowPos(&wndTopMost,-1,-1,-1,-1,SWP_NOMOVE|SWP_NOSIZE|SWP_NOACTIVATE);
	else
		SetWindowPos(&wndNoTopMost,-1,-1,-1,-1,SWP_NOMOVE|SWP_NOSIZE|SWP_NOACTIVATE);
	
	if(strOldAppPrefernce!=theApp.GetProfileString(GetString(IDS_INFO) + _T("\\") + GetUserRole()+_T("\\")+strUserId,GetString(IDS_STUBS)))
	{
		CString strOldXML = GetRegFileText(GetString(IDS_INFO)+_T("\\")+GetUserRole(),_T(""));
		if(!strOldXML.IsEmpty())
		{
			CComPtr<IXMLDOMDocument>	pDoc		=	NULL;
			pDoc.CoCreateInstance(CLSID_DOMDocument40, NULL, CLSCTX_INPROC_SERVER);
			CComBSTR	bsXML = strOldXML;
			VARIANT_BOOL	varLoad	=	VARIANT_FALSE;
			if(SUCCEEDED(pDoc->loadXML(bsXML,&varLoad))&&varLoad==VARIANT_TRUE)
			{
				LoadAppList(pDoc,TRUE);
			}
		}
	}
	
	if(strOldMyDocumetPath!=GetMyDocumetPath(GetUserRole(),GetUserID()))
	{
		m_FileManager.SetUserDocumetFolder(GetMyDocumetPath(GetUserRole(),GetUserID()));
	}
	
	if(bOldRemoveTaskBar!=GetOptionInt(IDS_OFSMESSENGER,IDS_REMOVE_FROM_TASK_BAR,TRUE))
	{
		BOOL bUpdate = GetStyle()&WS_VISIBLE;
		
		if(bUpdate)
			ShowWindow(SW_HIDE);
		
		if(GetOptionInt(IDS_OFSMESSENGER,IDS_REMOVE_FROM_TASK_BAR,TRUE))
			ModifyStyleEx(0,WS_EX_TOOLWINDOW);
		else
			ModifyStyleEx(WS_EX_TOOLWINDOW,0);
		
		if(bUpdate)
		{
			ShowWindow(SW_SHOW);
		}
	}
	
	if(page10.m_bWasChanged&&page10.m_bWasSaved)
	{
		// All Refresh Colors for Chat [8/29/2002]
		for(int iChatIndex = 0; iChatIndex<m_ChatCollection.GetSize();iChatIndex++)
		{
			CChat tmpChat = m_ChatCollection.ElementAt(iChatIndex);
			
			tmpChat.RefreshColors(GetUserRole(),GetUserID());
			
			HWND hDlg = tmpChat.GetChatWindow();
			if(IsWindow(hDlg))
			{
				CMcWindowAgent	winAgent(hDlg);
				winAgent.Refresh();
			}
		}
	}
	
	m_btnMin.ShowWindow(GetOptionInt(IDS_OFSMESSENGER,IDS_MINIMIZE_ONCLOSE,TRUE)?SW_HIDE:SW_SHOW);
}

void CMainDlg::LockShowInfo()
{
	bShowInfo = FALSE;
}

void CMainDlg::UnlockShowInfo()
{
	bShowInfo = TRUE;	
	
	LoadNotReadMessage();
	
	LoadNotSendedMessage();
	
	LoadFloating();
	
	int Size = m_SaveEventArray.GetSize();
	
	if(Size)
	{
		for(int i=0;i<Size;i++)
		{
			try
			{
				OnNetEvent((WPARAM)m_SaveEventArray[i],(LPARAM)NULL);
			}
			catch (...) 
			{
			}
		}
        m_SaveEventArray.RemoveAll();  
	}
}

BOOL CMainDlg::IsLockShowInfo()
{
	return !bShowInfo;
}

const CString& CMainDlg::GetServerPath()
{
	return m_strServerPath;
}


CString CMainDlg::GetWebHOST()
{
	CString strServer = m_strServer;
	strServer.Replace(_T("."),_T("_"));
	return m_strServerPath + _T("/portals/") + strServer;
}

LRESULT CMainDlg::OnSendFile2(WPARAM w, LPARAM l)
{
	SendFileInfo *pSendInfo = (SendFileInfo*)w;

	if(IsBadReadPtr(pSendInfo,sizeof(SendFileInfo)))
		return -1;
	
	AddToUpload(pSendInfo->File, 
		pSendInfo->RecepientName,
		pSendInfo->RecepientID,
		pSendInfo->Description);

	delete pSendInfo;

	return 0;
}

LRESULT	CMainDlg::OnSendFile(WPARAM w, LPARAM l)
{
	CUser *pUser  = FindUserInVisualContactListByGlobalId((long)w);
	
	if(pUser)
	{
		HGLOBAL hGlobal = (HGLOBAL)l;
		
		HDROP hDropInfo = (HDROP)GlobalLock(hGlobal);
		
		UINT FileCount = DragQueryFile(hDropInfo,0xFFFFFFFF,NULL,0);
		if(FileCount>30)
		{
			CString	strMessage;
			strMessage.LoadString(IDS_FILES_SEND_LIMIT);
			MessageBox(strMessage,GetString(IDS_ERROR_TITLE),MB_OK|MB_ICONERROR);
			return 0;
		}
		
		
		CString strDescription	=	_T("");
		//		CFileDescriptioDlg	DescrDlg(this);
		//		
		//		DescrDlg.m_strFileName = _T("");
		//		for(UINT i=0;i<FileCount;i++)
		//		{
		//			CString strPath;
		//			TCHAR  FileBuffer[MAX_PATH];
		//			DragQueryFile(hDropInfo,i,FileBuffer,MAX_PATH);
		//			strPath = FileBuffer;
		//			strPath = strPath.Mid(strPath.ReverseFind('\\')+1);
		//			
		//			DescrDlg.m_strFileName += strPath;
		//			DescrDlg.m_strFileName += _T("; ");
		//		}
		//		
		//		if(DescrDlg.DoModalEditMode()==IDOK)
		{
			//			strDescription = DescrDlg.GetDescription();	
			
			for(UINT i=0;i<FileCount;i++)
			{
				TCHAR FileBuffer[MAX_PATH];
				DragQueryFile(hDropInfo,i,FileBuffer,MAX_PATH);
				SendFileToUser(pUser,FileBuffer,strDescription);
			}
		}
		
		
		GlobalUnlock(hGlobal);
		
		GlobalFree(hGlobal);
	}
	
	return 0;
}

BOOL CMainDlg::ShowHistoryForUser(CUser &user)
{
	m_HistoryDlg.ShowHistory(user);
	return TRUE;
}

void CMainDlg::ShowUserMenu(long GlobalId)
{
	CUser *pUser = FindUserInVisualContactListByGlobalId(GlobalId);
	if(pUser)
		OnMenuTreectrl(pUser->TID,FALSE);
}

void CMainDlg::GetCopyContactList(CUserCollection &ContactList, BOOL bIncludeSystemUsers)
{
	ContactList.Clear();
	//////////////////////////////////////////////////////////////////////////
	// Копируем Контак Лист [12/21/2001] 
	try
	{
		CUser* pUser=NULL;
		
		if(POSITION pos = m_ContactList.InitIteration())
		{
			for(int i=0; m_ContactList.GetNext(pos,pUser); i++)
			{
				if(bIncludeSystemUsers)
				{
					ContactList.SetAt(*pUser);
				}
				else
				{
					if(!pUser->IsSystemUser())
						ContactList.SetAt(*pUser);
				}
			}
		}
		
		if(POSITION pos = m_ExternalContactList.InitIteration())
		{
			for(int i=0; m_ExternalContactList.GetNext(pos,pUser); i++)
			{
				ContactList.SetAt(*pUser);
			}
		}
	}
	catch(...)
	{
		ASSERT(FALSE);
	}
	//////////////////////////////////////////////////////////////////////////
}

void CMainDlg::GetCopyContactList(CUserCollection &ContactList)
{
	GetCopyContactList(ContactList,FALSE);
}

void CMainDlg::OnOptionsAddiniviteFinduseraddtofriends() 
{
	if(::IsWindow(GetSafeHwnd()))
		PostMessage(WM_SHOW_ADDUSER);
}

void CMainDlg::SendFileToUser(CUser *pUser, LPCTSTR FilePath, LPCTSTR strDescription)
{
	if(pUser==NULL) return;
	m_FileManager.AddToUpLoad (FilePath,pUser->GetShowName(),pUser->GetGlobalID(),strDescription);
}

void CMainDlg::OnOptionsFilemanagerDownload() 
{
	m_FileManager.ShowDialog();
}

void CMainDlg::OnOptionsFilemanagerUpload() 
{
	m_FileManager.ShowDialog();
}

void CMainDlg::AddUserToContactList(CUser &user)
{
	long      DetailsHandle = 0L;
	
	try
	{
		DetailsHandle = pSession->UserDetails(user.GetGlobalID(),1);
		if(DetailsHandle)
		{
			theNet2.AddToTranslator(DetailsHandle,this->GetSafeHwnd());
		}
	}
	catch(...)
	{
		ASSERT(FALSE);
	}
	
	CUser	*pUser	=	FindUserInVisualContactListByGlobalId(user.GetGlobalID());
	if(pUser)
	{
		user.m_bHasNewMessages = pUser->m_bHasNewMessages;
		DeleteFromContact(user.GetGlobalID());
	}
	
	m_ContactList.SetAt(user);
}


BOOL CMainDlg::GetUserByGlobalId(long UserId, CUser &User)
{
	CUser *pUser = FindUserInVisualContactListByGlobalId(UserId);
	
	if(pUser)
	{
		User = *pUser;
		return TRUE;
	}
	
	return FALSE;
}

BOOL CMainDlg::CheckUserInContactList(long GlobalID)
{
	if(POSITION pos = m_ContactList.InitIteration())
	{
		CUser *pUser=NULL;
		
		while(m_ContactList.GetNext(pos,pUser))
		{
			if(pUser->GetGlobalID() == GlobalID)
				return TRUE;
		}
	}
	
	return FALSE;
}

BOOL CMainDlg::CheckUserInContactList(CUser& User)
{
	return CheckUserInContactList(User.GetGlobalID());
}

void CMainDlg::CancelLogon()
{
	if(m_lLogonHandle)
	{
		try
		{
			pSession->CancelOperation(m_lLogonHandle);
		}
		catch (...) 
		{
		}
	}
}

void CMainDlg::LoadAppList(IXMLDOMDocument *xmlDoc, BOOL bRestoryOld)
{
	CString		strSID = GetSID();
	//////////////////////////////////////////////////////////////////////////
	// Step 1. Load App list [2/6/2002]
	m_AppArray.RemoveAll();
	m_AppBar.m_AppCtrl.DeleteAllButton();
	
	// Step 1. Add predefine Items (IM) [2/1/2002]
	McAppItem	AppItemCL(APPT_CONTACTLIST,_T("IM"),GetString(IDS_SHOW_CONTACTLIST_NAME),_T(""));
	if(m_picIM.m_pPict)
		AppItemCL.Icon = m_picIM.m_pPict;
	
	// Step 2. Add predefine Items (CC) [2/1/2002]
/*	McAppItem	AppItemIA(APPT_IBN_ACTIONS,_T("IA"),GetString(IDS_SHOW_IBN_ACTIONS_NAME),CString(IBN_SCHEMA) + CString(_T("default/Shell/IBNActions/index.html")));
	AppItemIA.Id	=	-2;
	if (m_picIA.m_pPict) 
		AppItemIA.Icon = m_picIA.m_pPict;
	
	if(!IsToolboxIstalled())
	{
		AppItemIA.Url = CString(IBN_SCHEMA) + CString(_T("default/Shell/IBNActions/index2.html"));
	}*/
	
	// Step 3. Add predefine Items (CC) [2/1/2002]
	McAppItem	AppItemCC(APPT_CHAT_CONTACTLIST,_T("CC"),GetString(IDS_SHOW_CHAT_CONTACTLIST_NAME),_T(""));
	AppItemCC.Id	=	-1;
	if (m_picCC.m_pPict) 
		AppItemCC.Icon = m_picCC.m_pPict;
	
	// Step 4. Add predefine Items (Web Folders) [2/1/2002]
	//McAppItem	AppItemWF(APPT_WEB_FOLDERS,_T("WF"),GetString(IDS_WEB_FOLDERS_NAME),_T(""));
	//AppItemWF.Id	=	-2;
	//if(m_picWF.m_pPict)
	//	AppItemWF.Icon = m_picWF.m_pPict;
	
	// Step 2. Add predefine Items (DOCS) [2/1/2002]
	//McAppItem	AppItemDOCS(APPT_INWINDOW,_T("DC"),GetString(IDS_DOCS_NAME),_T("#host#/Apps/Docs/clientlinks.asp?sid=#sid#"));
	//AppItemDOCS.Id	=	-10;
	//if(m_picDOCS.m_pPict)
	//	AppItemDOCS.Icon = m_picDOCS.m_pPict;
	//AppItemDOCS.Url.Replace("#sid#",strSID);
	//AppItemDOCS.Url.Replace("#host#",GetServerPath());
	
	
	// Step 3. Add predefine Items (LINKS) [2/1/2002]
	//McAppItem	AppItemLINKS(APPT_INWINDOW,_T("LN"),GetString(IDS_LINKS_NAME),_T("#host#/Apps/Links/clientlinks.asp?sid=#sid#"));
	//AppItemLINKS.Id	=	-20;
	//if(m_picLINKS.m_pPict)
	//	AppItemLINKS.Icon = m_picLINKS.m_pPict;
	//AppItemLINKS.Url.Replace("#sid#",strSID);
	//AppItemLINKS.Url.Replace("#host#",GetServerPath());
	
	// Step 4. Add predefine Items (NEWS) [2/1/2002]
	//McAppItem	AppItemNEWS(APPT_INWINDOW,_T("NW"),GetString(IDS_NEWS_NAME),_T("#host#/Apps/News/clientnews.asp?sid=#sid#"));
	//AppItemNEWS.Id	=	-30;
	//if(m_picNEWS.m_pPict)
	//	AppItemNEWS.Icon = m_picNEWS.m_pPict;
	//AppItemNEWS.Url.Replace("#sid#",strSID);
	//AppItemNEWS.Url.Replace("#host#",GetServerPath());
	
	
	CMcVersionInfo	VerInfo;
	CString strVersion;
	strVersion.Format(_T("%d.%d.%d"), (VerInfo.GetProductVersionMS()&0xffff0000)>>16,VerInfo.GetProductVersionMS()&0x0000ffff,(VerInfo.GetProductVersionLS()&0xffff0000)>>16);
	
	AppItemCL.Version	=	strVersion;
	AppItemCC.Version	=	strVersion;
//	AppItemIA.Version	=	strVersion;
	//AppItemDOCS.Version	=	strVersion;
	//AppItemLINKS.Version	=	strVersion;
	//AppItemNEWS.Version	=	strVersion;
	//AppItemWF.Version	=	strVersion;
	
	m_AppArray.Add(AppItemCL);
//	m_AppArray.Add(AppItemIA);
	m_AppArray.Add(AppItemCC);
	//m_AppArray.Add(AppItemWF);
	//m_AppArray.Add(AppItemDOCS);
	//m_AppArray.Add(AppItemLINKS);
	//m_AppArray.Add(AppItemNEWS);
	
	
	if(xmlDoc)
	{
		// Create App Buttons [2/1/2002]
		
		CString		strAtrValue;
		long		Index = 0;
		
		
		
		CComPtr<IXMLDOMNodeList>	pStubsList	=	NULL;
		
		if(SUCCEEDED(xmlDoc->selectNodes(CComBSTR(L"options/stubs/stub"),&pStubsList)))
		{
			long ListLength	=	0;
			pStubsList->get_length(&ListLength);
			for(long i=0;i<ListLength;i++)
			{
				McAppItem	AppItem;
				
				CComPtr<IXMLDOMNode>	pStubNode	=	NULL;
				pStubsList->get_item(i,&pStubNode);
				if(pStubNode)
				{
					CComBSTR	bsId, bsStubName, bsToolTip, bsUrl, bsOpenInBrowser, bsWidth, bsHeight, bsVersion;
					CComVariant	varIcon;
					
					GetTextByPath(pStubNode, CComBSTR(L"stub_id"),&bsId);
					GetTextByPath(pStubNode, CComBSTR(L"stub_name"),&bsStubName);
					GetTextByPath(pStubNode, CComBSTR(L"tooltip"),&bsToolTip);
					GetTextByPath(pStubNode, CComBSTR(L"url"),&bsUrl);
					GetTextByPath(pStubNode, CComBSTR(L"open_window"),&bsOpenInBrowser);
					GetDataByPath(pStubNode, CComBSTR(L"icon"),&varIcon);
					GetTextByPath(pStubNode, CComBSTR(L"version"),&bsVersion);
					
					AppItem.Id	= _wtol(bsId);
					
					AppItem.Name	= bsStubName;
					AppItem.ToolTip = bsToolTip;
					AppItem.Url		= bsUrl;
					AppItem.Version	= bsVersion;
					
					//AppItem.Url.MakeLower();
					AppItem.Url.Replace(_T("#sid#"), strSID);
					AppItem.Url.Replace(_T("#host#"), GetServerPath());
					
					AppItem.Type = APPT_BROWSEWINDOW;
					
					if(bsOpenInBrowser == CComBSTR(L"False"))
					{
						AppItem.Type = APPT_EXWINDOW;

						HRESULT hr;

						hr = GetTextByPath(pStubNode, CComBSTR(L"width"),&bsWidth);
						if(hr == S_OK)
							AppItem.Width	= _wtol(bsWidth);

						hr = GetTextByPath(pStubNode, CComBSTR(L"height"),&bsHeight);
						if(hr == S_OK)
							AppItem.Height  = _wtol(bsHeight);
					}
					
					CComPtr<IStream>	pStream	=	NULL;
					
					if(SUCCEEDED(Array2Stream(varIcon,&pStream)))
						::OleLoadPicture(pStream,	0, TRUE, IID_IPicture, (void**)&AppItem.Icon);
					
					m_AppArray.Add(AppItem);
					
				}
			}
			// Sort Array by Prioriry [3/11/2002]
			CString	strUserId;
			strUserId.Format(_T("%d"), GetUserID());
			CString strPriority	=	theApp.GetProfileString(GetString(IDS_INFO) + _T("\\") + GetUserRole()+_T("\\")+strUserId,GetString(IDS_STUBS));
			if(!strPriority.IsEmpty())
			{
				int	iPos	=	0;
				CString	strOutPriority;
				
				while(!strPriority.IsEmpty())
				{
					int TmpIndex = strPriority.Find(_T(';'));
					if(TmpIndex==-1)
						break;
					
					long	lTmpId	=	_ttol(strPriority.Left(TmpIndex));
					
					for(long i=0;i<m_AppArray.GetSize();i++)
					{
						McAppItem	AppItem = m_AppArray[i];
						if(AppItem.Id==lTmpId)
						{
							if(iPos>=m_AppArray.GetSize())
								iPos = m_AppArray.GetSize() -1;
							if(iPos!=i)
							{
								m_AppArray[i]		=	m_AppArray[iPos];
								m_AppArray[iPos]	=	AppItem;
							}
							CString	strTmpId;
							strTmpId.Format(_T("%d"),AppItem.Id);
							strOutPriority += (strTmpId + _T(';'));
							break;
						}
					}			
					
					strPriority = strPriority.Mid(TmpIndex+1);
					iPos++;
				}
				
				theApp.WriteProfileString(GetString(IDS_INFO) + _T("\\") + GetUserRole()+_T("\\")+strUserId,GetString(IDS_STUBS),strOutPriority);
			}
		}
	}
	
	// Add to View [3/11/2002]
	//	ShowIBNActions();
	
	int m_lDefaultView = 1;//GetOptionInt(IDS_OFSMESSENGER,IDS_VIEWMODE,1);
	
	for(int i=0;i<m_AppArray.GetSize();i++)
	{
		McAppItem	AppItem = m_AppArray[i];
		if(m_lDefaultView==0)
		{
			if(AppItem.Id==-2)
				m_AppBar.m_AppCtrl.SetCheckButton(i);
		}
		else
		{
			if(AppItem.Id==0)
				m_AppBar.m_AppCtrl.SetCheckButton(i);
		}
		m_AppBar.m_AppCtrl.AddButton(AppItem.Name,AppItem.ToolTip, AppItem.Icon);
	}			
	
	
	CRect	winRect;
	m_AppBar.GetWindowRect(&winRect);
	m_AppBar.m_AppCtrl.SetMaxShowItem(m_AppBar.m_AppCtrl.GetMaxShowItemFromCY(winRect.Height()));
	
	//if(m_lDefaultView!=0)
	//{
		m_treebox.ShowWindow(SW_SHOW);
		m_chatbox.ShowWindow(SW_HIDE);
		//m_WebFolderView.ShowWindow(SW_HIDE);
		m_InWindow.ShowWindow(SW_HIDE);
		m_InWindow.Navigate(CString(IBN_SCHEMA) + GetProductLanguage() + CString(_T("/Common/blank.html")));
	//}
	//else
	//{
	//	m_chatbox.ShowWindow(SW_HIDE);
	//	m_treebox.ShowWindow(SW_HIDE);
	//	m_InWindow.ShowWindow(SW_SHOW);
	//	m_InWindow.SetFocus();
	//	m_InWindow.Navigate(AppItemIA.Url);
	//}
				
	//////////////////////////////////////////////////////////////////////////
	// Step 2. Load Version. [2/6/2002]	
	if(xmlDoc&&!m_bUpdateDlgWasShow&&!bRestoryOld)
	{
		CComPtr<IXMLDOMNode>	pVersionsNode	=	NULL;
		
		HRESULT hr = xmlDoc->selectSingleNode(CComBSTR(L"options/versions"),&pVersionsNode);
		
		if(pVersionsNode)
		{
			m_bUpdateDlgWasShow = TRUE;
			
			CString  bstrallXML;
			
			CComBSTR	bstrXML;
			pVersionsNode->get_xml(&bstrXML);
			
			bstrallXML = bstrXML;
			
			CWebWindow::SetVariable(_T("UpdateXML"),bstrallXML);
			
			CWebWindow	*pUpdate = new CWebWindow;
			
			CString strUpdate;
			strUpdate.Format(CString(IBN_SCHEMA) + CString(_T("%s/Browser/Update/index.html")),::GetCurrentSkin());
			
			CRect winRect;
			GetWindowRect(&winRect);
			
			pUpdate->CreateAutoKiller(_T("/Browser/Common/skin.xml"),GetMessageParent(),GetDesktopWindow(),winRect.left-150,winRect.top+20,400,300,GetString(IDS_IBN_UPDATE_LIST_NAME),strUpdate,FALSE,FALSE,TRUE);
			
			m_AllClosedWindow.Add(pUpdate->GetSafeHwnd());
		}
	}
	//////////////////////////////////////////////////////////////////////////
	// Step 4. Load Logo  [2/6/2002]
	if(xmlDoc)
	{
		CComPtr<IXMLDOMNode>	pLogoNode	=	NULL;
		
		HRESULT hr = xmlDoc->selectSingleNode(CComBSTR(L"options/logos/logo"),&pLogoNode);
		
		if(pLogoNode)
		{
			CComBSTR	bsColor;
			CComPtr<IStream>	pStream	=	NULL;
			wchar_t*	pEnd;
			
			GetTextByPath(pLogoNode, CComBSTR(L"color"),&bsColor);
			long lTmpColor = wcstoul(bsColor,&pEnd,16);
			m_picLogo.m_BkgColor = RGB(GetBValue(lTmpColor),GetGValue(lTmpColor),GetRValue(lTmpColor));
			
			CComVariant	varIcon;
			GetDataByPath(pLogoNode, CComBSTR(L"client_logo"),&varIcon);
			
			CPictureHolder	Icon;
			if(SUCCEEDED(Array2Stream(varIcon,&pStream)))
			{
				m_picLogo.m_Image.SetPictureDispatch(NULL);
				::OleLoadPicture(pStream,	0, TRUE, IID_IPicture, (void**)&m_picLogo.m_Image.m_pPict);
			}
		}
	}
	
	Invalidate();
}

/*BOOL CMainDlg::PreTranslateMessage(MSG* pMsg) 
{
//m_AppBar.m_AppCtrl.TranslateMessage(pMsg);

  return COFSNcDlg2::PreTranslateMessage(pMsg);
}*/


BOOL CMainDlg::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message) 
{
	CPoint point;
	GetCursorPos(&point);
	ScreenToClient(&point);
	
	CPoint TstPoint	=	point;
	ClientToScreen(&TstPoint);
	m_AppBar.ScreenToClient(&TstPoint);
	
	
	int HitTest = m_AppBar.m_AppCtrl.HitTest(TstPoint);
	if(HitTest!=-1&&HitTest!=m_AppBar.m_AppCtrl.GetCheckButton())
	{
		SetCursor(AfxGetApp()->LoadCursor(IDC_HANDCURSOR));
		return TRUE;
	}
	
	return COFSNcDlg2::OnSetCursor(pWnd, nHitTest, message);
}

BOOL CMainDlg::UserIsVisible(long UserGlobalId)
{
	HWND	hKey =	NULL;
	CUser	*pUser=	FindUserInVisualContactListByGlobalId(UserGlobalId);
	
	if(pUser&&!m_bShowOffline)
	{
		//		BOOL p1	=	GetOptionInt(IDS_OFSMESSENGER,IDS_SHOWOFFLINEFILES,TRUE);
		//		BOOL p2	=	pUser->m_iStatus!=S_OFFLINE&&pUser->m_iStatus!=S_INVISIBLE;
		//		BOOL p3	=	m_SendMessageDlgMap.Lookup(UserGlobalId,hKey);
		//		BOOL p4	=	m_ComeMessageDlgMap.Lookup(UserGlobalId,hKey);
		//		BOOL p5	=	m_SplitMessageDlgMap.Lookup(UserGlobalId,hKey);
		//		BOOL p6	=	m_DelUserDlgMap.Lookup(UserGlobalId,hKey);
		//		BOOL p7	=	m_AddUserDlgMap.Lookup(UserGlobalId,hKey);
		//		BOOL p8	=	pUser->m_bHasNewMessages;
		
		
		/*
		BOOL bInFileDlgOpen = FALSE;
		
		  for(int i=0;i<m_InFileDlgArray.GetSize();i++)
		  {
		  FileDownloadDlgItem Item = m_InFileDlgArray[i];
		  if(Item.UserGlobalId==pUser->GetGlobalID())
		  {
		  bInFileDlgOpen = TRUE;
		  break;
		  }
				}*/
		
		
		return (    /*bInFileDlgOpen||*/
			(pUser->m_iStatus!=S_OFFLINE&&pUser->m_iStatus!=S_INVISIBLE)||
			m_SendMessageDlgMap.Lookup(UserGlobalId,hKey)||
			m_ComeMessageDlgMap.Lookup(UserGlobalId,hKey)||
			m_SplitMessageDlgMap.Lookup(UserGlobalId,hKey)||
			m_DelUserDlgMap.Lookup(UserGlobalId,hKey)||
			m_AddUserDlgMap.Lookup(UserGlobalId,hKey)||
			pUser->m_bHasNewMessages);
	}	
	return TRUE;
}


CString CMainDlg::GetSID()
{
	if(ConnectEnable())
	{
		_bstr_t	SID;
		try
		{
			SID = pSession->GetSID();
		}
		catch (_com_error&) 
		{
			ASSERT(FALSE);
		}
		return (LPCTSTR)SID;
	}
	return _T("");
}

void CMainDlg::ShowError(long Type, long Code)
{
	/************************************************************************/
	/* You have received a message!
	typedef enum
	{
	etWININET = 1, =>WIN32Errors and wininet errors
	etSTATUS,	=>HTTP status
	etSERVER,	= IM server error, smr nizhe
	etFILE,		= win32 file error
	etCANCEL	= cancel
	} ErrorTypes;
	
	  #define ERR_OUT_ALREADY_IN              100
	  #define ERR_OUT_WRONG_PASSWORD          101
	  #define ERR_OUT_WRONG_SID               102
	  #define ERR_OUT_WRONG_NAME              103
	  #define ERR_OUT_WRONG_REQUEST           104
	  #define ERR_UNKNOW                      105
	  #define ERR_OUT_GLOBAL                  106
	  #define ERR_UNABLE_CREATE_CONN          108
	  #define ERR_UNABLE_CREATE_COMM          109
	  #define ERR_UNABLE_CREATE_RECSET        110
	  #define ERR_WRONG_XML                   111
	  #define ERR_WRONG_SID                   112
	  #define ERR_WRONG_ID                    113
	  #define ERR_WRONG_PASSWORD              114
	  #define ERR_SQL_UNKNOWN_PROBLEM         115
	  #define ERR_PRIMARY_KEY_CONSTRAINT      116
	  #define ERR_UNABLE_SEND                 117
	  #define ERR_UNNKOWN_XML                 118
	  #define ERR_UNABLE_READ                 119
	  #define ERR_ALREADY_SENT                120
	  #define ERR_NOT_RECIPIENTS              121
	  #define ERR_OLD_PROTOCOL                122
	  #define ERR_LICENSE_LIMIT               130
#define IDR_TEST                        201                                                                     */
	
	/************************************************************************/
	WPARAM idMessage = 0;
	
	if(Type==etSERVER)
	{
		switch(Code) 
		{
		case ERR_OUT_ALREADY_IN:
			break;
		case ERR_OUT_WRONG_PASSWORD:
			idMessage = IDS_INVALID_LOGIN_OR_PASSWORD;
			break;
		case ERR_OUT_WRONG_SID:
			break;
		case ERR_OUT_WRONG_NAME:
			idMessage = IDS_INVALID_LOGIN_OR_PASSWORD;
			break;
		case ERR_OUT_WRONG_REQUEST:
			break;
		case ERR_UNKNOW:
			break;
		case ERR_OUT_GLOBAL:
			break;
		case ERR_UNABLE_CREATE_CONN:
			idMessage	=	IDS_SERVICENOTAVAILABLE;
			//SendMessage(WM_SHOW_LOGIN_DLG);
			//SendMessage(WM_SHOWMESSAGEBOX,(LPARAM)IDS_SERVICENOTAVAILABLE);
			break;
		case ERR_UNABLE_CREATE_COMM:
			break;
		case ERR_UNABLE_CREATE_RECSET:
			break;
		case ERR_WRONG_XML:
			break;
		case ERR_WRONG_SID:
			break;
		case ERR_WRONG_ID:
			idMessage = IDS_INVALID_LOGIN_OR_PASSWORD;
			break;
		case ERR_WRONG_PASSWORD:
			idMessage = IDS_INVALID_LOGIN_OR_PASSWORD;
			break;
		case ERR_SQL_UNKNOWN_PROBLEM:
			break;
		case ERR_PRIMARY_KEY_CONSTRAINT:
			break;
		case ERR_UNABLE_SEND:
			break;
		case ERR_UNNKOWN_XML:
			break;
		case ERR_UNABLE_READ:
			break;
		case ERR_ALREADY_SENT:
			break;
		case ERR_NOT_RECIPIENTS:
			break;
		case ERR_OLD_PROTOCOL:
			break;
		case ERR_LICENSE_LIMIT:
			//idMessage	=	IDS_LICENSE_LIMIT;
			SendMessage(WM_SHOW_LOGIN_DLG);
			SendMessage(WM_SHOWMESSAGEBOX,(LPARAM)IDS_LICENSE_LIMIT);
			break;
		}
	}
	else if(Type==etWININET)
	{
		switch(Code)
		{
		case ERROR_INTERNET_NAME_NOT_RESOLVED:
			if(dwStartUpInfo!=STARTUP_ALL&&m_bUserDomainMode)
			{
				m_bUpdateUserStatus	=	TRUE;
				m_bUserDomainMode	=	FALSE;
			}
			break;
		}
	}
	
	if(idMessage)
	{
		SendMessage(WM_SHOW_LOGIN_DLG);
		_SHOW_IBN_ERROR_DLG_OK(idMessage);
		//CMessageDlg dlgErrorConnect(idMessage,this);
		//dlgErrorConnect.Show(GetString(idMessage),MB_OK);
	}
}

void CMainDlg::OnOptionsViewmydetails() 
{
	if(m_User.GetGlobalID())
	{
		ShowUserDetails(GetUserID());
		//CString strMyDetailsUrl	=	GetServerPath();
		//strMyDetailsUrl.Format(GetString(IDS_WEB_MYDETAILS),GetServerPath(),GetSID(),GetUserID());
		
		//if(m_InWindow.NavigateNewWindow(strMyDetailsUrl)!=S_OK)
		//	ShellExecute(NULL,_T("open"),strMyDetailsUrl,NULL,NULL,SW_SHOWNORMAL);
	}
}

void CMainDlg::OnUpdateOptionsViewmydetails(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(ConnectEnable(FALSE));
}

void CMainDlg::ShowUserDetails(long UserGlobalID)
{
	CString		strDetailsURL, strUrl;
	strDetailsURL.Format(GetString(IDS_WEB_USERDETAILS),GetServerPath(),GetSID(),UserGlobalID);
	
	CWebWindow *pNewWindow = new CWebWindow;
	
	//pNewWindow->SetVariable(_T("user_id"),strUserId) ;
	
	strUrl.Format(CString(IBN_SCHEMA) + CString(_T("%s/Browser/Details/index.html")),::GetCurrentSkin());
	
	CRect			winRect;
	GetWindowRect(&winRect);
	
	CUser *pUser = FindUserInVisualContactListByGlobalId(UserGlobalID);
	CString strTitle;
	
	if(pUser)
		strTitle.Format(GetString(IDS_DETAILS_FOR_FORMAT), pUser->GetShowName());
	else
		strTitle = GetString(IDS_USER_DETAILS_TITLE);
	
	pNewWindow->CreateAutoKiller(_T("/Browser/Details/skin.xml"), this, GetDesktopWindow(), winRect.left-100,winRect.top+20, 300, 150, strTitle, strUrl, FALSE, FALSE, TRUE,IDS_USERDETAILS);
	
	pNewWindow->LoadXML(strDetailsURL);
	
	m_AllClosedWindow.Add(pNewWindow->GetSafeHwnd());
}

void CMainDlg::ShowGeneralMenu(LONG ChatTID)
{
	CurrChatTID = ChatTID;
	
	CMenu m_Menu, *pSubMenu = NULL;
	m_Menu.LoadMenu(IDR_MESSENGER_MENU);
	pSubMenu = m_Menu.GetSubMenu(2);
	CPoint point;
	GetCursorPos(&point);
	
	if(CurrChatTID!=-1)
	{
		CChat Chat;
		if(FindChatByTID(CurrChatTID,Chat))
		{
			CString strConferenceMenuName;
			strConferenceMenuName.Format(GetString(IDS_CONFERENCE_MENU),Chat.GetShowName());
			
			UINT id = (UINT)::GetSubMenu(pSubMenu->GetSafeHmenu(),5);
			pSubMenu->ModifyMenu(5,MF_BYPOSITION|MF_POPUP,id,strConferenceMenuName);
		}
	}
	
	UpdateMenu(this, pSubMenu);
	pSubMenu->TrackPopupMenu(0, point.x, point.y, this);
}

void CMainDlg::OnAppItem(UINT Id)
{
	USES_CONVERSION;
	
	int NowSelect	= m_AppBar.m_AppCtrl.GetCheckButton();
	int SelItem		= Id-20000;
	
	// Activate  SelItem Action [2/1/2002]
	if(SelItem!=-1)
	{
		//m_btnAlerts.SetPressed(FALSE);
		if(m_AppArray.GetSize()>SelItem)
		{
			McAppItem Item = m_AppArray[SelItem];
			
			if(Item.Type==APPT_CHAT_CONTACTLIST)
			{
				m_AppBar.m_AppCtrl.SetCheckButton(SelItem);
				
				//m_WebFolderView.ShowWindow(SW_HIDE);
				m_treebox.ShowWindow(SW_HIDE);
				//m_InWindow.ShowWindow(SW_HIDE);
				//m_InWindow.Navigate(_T("IBN_SCHEMA://default/Common/blank.html"));
				if(m_ChatCollection.GetSize()==0)
				{
					m_chatbox.ShowWindow(SW_HIDE);
					m_InWindow.ShowWindow(SW_SHOW);
					m_InWindow.Navigate(CString(IBN_SCHEMA)+GetProductLanguage()+CString(_T("/Shell/Conference/blank.html")));
				}
				else
				{
					m_chatbox.ShowWindow(SW_SHOW);
					m_InWindow.ShowWindow(SW_HIDE);
					m_InWindow.Navigate(CString(IBN_SCHEMA)+GetProductLanguage()+CString(_T("/Common/blank.html")));
				}
			}
			else if(Item.Type==APPT_CONTACTLIST)
			{
				m_AppBar.m_AppCtrl.SetCheckButton(SelItem);
				
				//m_WebFolderView.ShowWindow(SW_HIDE);
				m_chatbox.ShowWindow(SW_HIDE);
				m_treebox.ShowWindow(SW_SHOW);
				m_InWindow.ShowWindow(SW_HIDE);
				m_InWindow.Navigate(CString(IBN_SCHEMA)+GetProductLanguage()+CString(_T("/Common/blank.html")));
			}
/*			else if(Item.Type==APPT_IBN_ACTIONS)
			{
				m_AppBar.m_AppCtrl.SetCheckButton(SelItem);
				
				m_chatbox.ShowWindow(SW_HIDE);
				m_treebox.ShowWindow(SW_HIDE);
				m_InWindow.ShowWindow(SW_SHOW);
				m_InWindow.SetFocus();
				m_InWindow.Navigate(Item.Url);
			}*/
			else if(Item.Type==APPT_INWINDOW)
			{
				//m_WebFolderView.ShowWindow(SW_HIDE);
				//m_chatbox.ShowWindow(SW_HIDE);
				//m_treebox.ShowWindow(SW_HIDE);
				//m_InWindow.ShowWindow(SW_SHOW);
				//m_InWindow.SetFocus();
				//m_InWindow.Navigate(Item.Url);
				
				m_AppBar.m_AppCtrl.SetCheckButton(NowSelect);
				
				if(m_InWindow.NavigateNewWindow(Item.Url)!=S_OK)
					ShellExecute(NULL,_T("open"),Item.Url,NULL,NULL,SW_SHOWNORMAL);
				
				Invalidate();
				
			}
			else if(Item.Type==APPT_BROWSEWINDOW)
			{
				m_AppBar.m_AppCtrl.SetCheckButton(NowSelect);
				
				if(m_InWindow.NavigateNewWindow(Item.Url)!=S_OK)
					ShellExecute(NULL,_T("open"),Item.Url,NULL,NULL,SW_SHOWNORMAL);
				
				Invalidate();
			}
			else if(Item.Type==APPT_EXWINDOW)
			{
				m_AppBar.m_AppCtrl.SetCheckButton(NowSelect);
				CWebWindow *pChild = new CWebWindow();
				pChild->CreateAutoKiller(_T("/Browser/Common/skin.xml"), GetMessageParent(), GetDesktopWindow(), CW_USEDEFAULT, 0, Item.Width, Item.Height, NULL, Item.Url, FALSE, FALSE, TRUE);
				m_AllClosedWindow.Add(pChild->GetSafeHwnd());
				Invalidate();
			}
		}
		
		Invalidate();
		
	}
}

void CMainDlg::OnDoDropTreectrl(long TID, BOOL bGroupe, LPUNKNOWN pDataObject, long DropEffect, long LastKeyState)
{
	if(!bGroupe)
	{
		CUser *pUser = FindUserInVisualContactList(TID);
		if(pUser==NULL||pUser->IsSystemUser())
			return;
	}
	
	//if(LastKeyState&MK_RBUTTON)
	{
		SetForegroundWindow();
		
		COleDataObject	pData;
		pData.Attach((LPDATAOBJECT)pDataObject,FALSE);
		
		FORMATETC stFormatTEXT = {CF_TEXT,NULL,DVASPECT_CONTENT,-1,TYMED_HGLOBAL};
		FORMATETC stFormatHDROP = {CF_HDROP,NULL,DVASPECT_CONTENT,-1,TYMED_HGLOBAL};
		
		STGMEDIUM outData =	{0};
		
		if(pData.GetData(CF_TEXT,&outData,&stFormatTEXT))
		{
			if(!bGroupe)
			{
				CUser *pUser = FindUserInVisualContactList(TID);
				
				if(pUser)
				{
					COleDataObject	pData;
					pData.Attach((LPDATAOBJECT)pDataObject,FALSE);
					
					FORMATETC stFormatTEXT = {CF_TEXT,NULL,DVASPECT_CONTENT,-1,TYMED_HGLOBAL};
					FORMATETC stFormatHDROP = {CF_HDROP,NULL,DVASPECT_CONTENT,-1,TYMED_HGLOBAL};
					
					STGMEDIUM outData =	{0};
					
					if(pData.GetData(CF_TEXT,&outData,&stFormatTEXT))
					{
						LPCTSTR strText = (LPCTSTR)GlobalLock(outData.hGlobal);
						SendMessageToUser(pUser,FALSE,strText);
						GlobalUnlock(outData.hGlobal);
					}
					//else if(pData.GetData(CF_HDROP,&outData,&stFormatHDROP))
					//{
					//	SIZE_T Size = GlobalSize(outData.hGlobal);
					//	HGLOBAL hGlobal = GlobalAlloc(GMEM_ZEROINIT,Size);
					//	
					//	memcpy(GlobalLock(hGlobal),GlobalLock(outData.hGlobal),Size);
					//	
					//	GlobalUnlock(hGlobal);
					//	GlobalUnlock(outData.hGlobal);
					//	PostMessage(WM_SEND_FILE,pUser->GetGlobalID(),(LPARAM)hGlobal);
					//}
				}
			}
			else
			{
				CString GroupName	=	m_treebox.GetItemText(TID);
				
				COleDataObject	pData;
				pData.Attach((LPDATAOBJECT)pDataObject,FALSE);
				
				FORMATETC stFormatTEXT = {CF_TEXT,NULL,DVASPECT_CONTENT,-1,TYMED_HGLOBAL};
				FORMATETC stFormatHDROP = {CF_HDROP,NULL,DVASPECT_CONTENT,-1,TYMED_HGLOBAL};
				
				STGMEDIUM outData =	{0};
				
				if(pData.GetData(CF_TEXT,&outData,&stFormatTEXT))
				{
					LPCTSTR strText = (LPCTSTR)GlobalLock(outData.hGlobal);
					
					CGroupMessageSendDlg *pGroupDlg = new CGroupMessageSendDlg(this);
					pGroupDlg->Create(GetDesktopWindow());
					pGroupDlg->SetFon(NULL);
					pGroupDlg->SetRecipientGroup(GroupName);
					pGroupDlg->SetBody(strText);
					pGroupDlg->ShowWindow(SW_SHOWNORMAL);
					pGroupDlg->SetForegroundWindow();
					
					GlobalUnlock(outData.hGlobal);
				}
			}
		}
		else if(pData.GetData(CF_HDROP,&outData,&stFormatHDROP))
		{
			if(m_hRightButtonDDGlobal!=NULL)
			{
				GlobalFree(m_hRightButtonDDGlobal);
				m_hRightButtonDDGlobal = NULL;
			}
			
			SIZE_T Size = GlobalSize(outData.hGlobal);
			m_hRightButtonDDGlobal = GlobalAlloc(GMEM_ZEROINIT,Size);
			
			memcpy(GlobalLock(m_hRightButtonDDGlobal),GlobalLock(outData.hGlobal),Size);
			
			GlobalUnlock(m_hRightButtonDDGlobal);
			GlobalUnlock(outData.hGlobal);
			
			m_bRightButtonDDTID		=	TID;
			m_bRightButtonDDGroupe	=	bGroupe;
			
			//PostMessage(WM_SEND_FILE,pUser->GetGlobalID(),(LPARAM)hGlobal);
			
			CPoint point;
			GetCursorPos(&point);
			CMenu menu;
			menu.LoadMenu(IDR_MESSENGER_MENU);
			CMenu* popup = menu.GetSubMenu(9);
			
			UpdateMenu(this,popup);
			
			popup->TrackPopupMenu(TPM_LEFTALIGN|TPM_RIGHTBUTTON, point.x, point.y, this);
		}
	}
	//	else
	//	{
	//		//TRACE(_T("\r\n CMainDlg::OnDoDropTreectrl");
	//		SetForegroundWindow();
	//		
	//		if(!bGroupe)
	//		{
	//			CUser *pUser = FindUserInVisualContactList(TID);
	//			
	//			if(pUser)
	//			{
	//				COleDataObject	pData;
	//				pData.Attach((LPDATAOBJECT)pDataObject,FALSE);
	//				
	//				FORMATETC stFormatTEXT = {CF_TEXT,NULL,DVASPECT_CONTENT,-1,TYMED_HGLOBAL};
	//				FORMATETC stFormatHDROP = {CF_HDROP,NULL,DVASPECT_CONTENT,-1,TYMED_HGLOBAL};
	//				
	//				STGMEDIUM outData =	{0};
	//				
	//				if(pData.GetData(CF_TEXT,&outData,&stFormatTEXT))
	//				{
	//					LPCTSTR strText = (LPCTSTR)GlobalLock(outData.hGlobal);
	//					SendMessageToUser(pUser,FALSE,strText);
	//					GlobalUnlock(outData.hGlobal);
	//				}
	//				else if(pData.GetData(CF_HDROP,&outData,&stFormatHDROP))
	//				{
	//					SIZE_T Size = GlobalSize(outData.hGlobal);
	//					HGLOBAL hGlobal = GlobalAlloc(GMEM_ZEROINIT,Size);
	//					
	//					memcpy(GlobalLock(hGlobal),GlobalLock(outData.hGlobal),Size);
	//					
	//					GlobalUnlock(hGlobal);
	//					GlobalUnlock(outData.hGlobal);
	//					PostMessage(WM_SEND_FILE,pUser->GetGlobalID(),(LPARAM)hGlobal);
	//				}
	//			}
	//		}
	//		else
	//		{
	//			CString GroupName	=	m_treebox.GetItemText(TID);
	//			
	//			COleDataObject	pData;
	//			pData.Attach((LPDATAOBJECT)pDataObject,FALSE);
	//			
	//			FORMATETC stFormatTEXT = {CF_TEXT,NULL,DVASPECT_CONTENT,-1,TYMED_HGLOBAL};
	//			FORMATETC stFormatHDROP = {CF_HDROP,NULL,DVASPECT_CONTENT,-1,TYMED_HGLOBAL};
	//			
	//			STGMEDIUM outData =	{0};
	//			
	//			if(pData.GetData(CF_TEXT,&outData,&stFormatTEXT))
	//			{
	//				LPCTSTR strText = (LPCTSTR)GlobalLock(outData.hGlobal);
	//				
	//				CGroupMessageSendDlg *pGroupDlg = new CGroupMessageSendDlg(this);
	//				pGroupDlg->Create(GetDesktopWindow());
	//				///////////////////////////////////////////////////////////////////////
	//				/*LoadSkins m_Load;
	//				IStreamPtr pStream = NULL;
	//				long Error = 0;
	//				bstr_t Path = bstr_t("IBN_SCHEMA://") + (LPCTSTR)GetCurrentSkin();
	//				m_Load.Load(Path+bstr_t("/Shell/GroupSend/fon.bmp"),&pStream,&Error);
	//				if(pStream)
	//				{
	//				CDib m_Dib(pStream);
	//				CPaintDC dc(this);*/
	//				pGroupDlg->SetFon(NULL);
	//				//}
	//				//pStream = NULL;
	//				
	//				//			m_Load.Load(Path+bstr_t("/Common/btn_x.bmp"),&pStream,&Error);
	//				//			if(pStream)
	//				//				pGroupDlg->m_Close.LoadBitmapFromStream(pStream);
	//				//			pStream = NULL;
	//				
	//				//			m_Load.Load(Path+bstr_t("/Common/btn_send.bmp"),&pStream,&Error);
	//				//			if(pStream)
	//				//				pGroupDlg->m_Send.LoadBitmapFromStream(pStream);
	//				//			pStream = NULL;
	//				
	//				//			m_Load.Load(Path+bstr_t("/Common/btn_minimize.bmp"),&pStream,&Error);
	//				//			if(pStream)
	//				//				pGroupDlg->m_Mini.LoadBitmapFromStream(pStream);
	//				//			pStream = NULL;
	//				
	//				//			m_Load.Load(Path+bstr_t("/Shell/GroupSend/selectall.bmp"),&pStream,&Error);
	//				//			if(pStream)
	//				//				pGroupDlg->m_SelectAll.LoadBitmapFromStream(pStream);
	//				//			pStream = NULL;
	//				///////////////////////////////////////////////////////////////////////
	//				pGroupDlg->SetRecipientGroup(GroupName);
	//				pGroupDlg->SetBody(strText);
	//				pGroupDlg->ShowWindow(SW_SHOWNORMAL);
	//				pGroupDlg->SetForegroundWindow();
	//				
	//				GlobalUnlock(outData.hGlobal);
	//			}
	//			else if(pData.GetData(CF_HDROP,&outData,&stFormatHDROP))
	//			{
	//				SIZE_T Size = GlobalSize(outData.hGlobal);
//				HGLOBAL hGlobal = GlobalAlloc(GMEM_ZEROINIT,Size);
//				memcpy(GlobalLock(hGlobal),GlobalLock(outData.hGlobal),Size);
//				GlobalUnlock(hGlobal);
//				GlobalUnlock(outData.hGlobal);
//				
//				PostMessage(WM_SENDGROUP_FILE,(WPARAM)GroupName.AllocSysString(),(LPARAM)hGlobal);
//			}
//		}
//	}
}

IComHistIntPtr& CMainDlg::GetLocalHistory()
{
	return m_LocalHistory;
}

void CMainDlg::OnTreegroupSendfile() 
{
	CFileDialog Open(TRUE, NULL, _T("*.*"), NULL, GetString(IDS_ALL_FILES_FORMAT), this);
	
    if(Open.DoModal () == IDOK) 
	{
		CString FileName = Open.GetPathName();
		
		CString strDescription;
		CGroupFileDescriptionDlg DescrDlg(this);
		DescrDlg.m_strFileName = Open.GetFileName();
		DescrDlg.m_strRecepientGroupName = m_strGroupName;
		if(DescrDlg.DoModal()==IDOK)
		{
			//			strDescription = DescrDlg.m_strFileDescription;
			strDescription = DescrDlg.GetFileDescription();
			CString strUserId	=	_T("");
			if(POSITION pos = DescrDlg.m_ContactList.InitIteration())
			{
				CUser	*pUser =	 NULL;
				while(DescrDlg.m_ContactList.GetNext(pos,pUser))
				{
					if(pUser->m_bHasNewMessages)
					{
						TCHAR Buff[MAX_PATH];
						_ltot(pUser->GetGlobalID(), Buff, 10);
						strUserId += Buff;
						strUserId += ",";
					}
				}
				
				if(!strUserId.IsEmpty())
					m_FileManager.AddToUpload2 (FileName,DescrDlg.m_strRecepientGroupName,strUserId,strDescription);
			}
		}
	}	
}

void CMainDlg::OnTreegroupSendmessage() 
{
	CGroupMessageSendDlg *pGroupDlg = new CGroupMessageSendDlg(this);
	pGroupDlg->Create(GetDesktopWindow());
	///////////////////////////////////////////////////////////////////////
	/*LoadSkins m_Load;
	IStreamPtr pStream = NULL;
	long Error = 0;
	bstr_t Path = bstr_t("IBN_SCHEMA://") + (LPCTSTR)GetCurrentSkin();
	m_Load.Load(Path+bstr_t("/Shell/GroupSend/fon.bmp"),&pStream,&Error);
	if(pStream)
	{
	CDib m_Dib(pStream);
	CPaintDC dc(this);*/
	pGroupDlg->SetFon(/*m_Dib.GetHBITMAP(dc)*/NULL);
	/*}
	pStream = NULL;*/
	
	//	m_Load.Load(Path+bstr_t("/Common/btn_x.bmp"),&pStream,&Error);
	//	if(pStream)
	//		pGroupDlg->m_Close.LoadBitmapFromStream(pStream);
	//	pStream = NULL;
	
	//	m_Load.Load(Path+bstr_t("/Common/btn_send.bmp"),&pStream,&Error);
	//	if(pStream)
	//		pGroupDlg->m_Send.LoadBitmapFromStream(pStream);
	//	pStream = NULL;
	
	//	m_Load.Load(Path+bstr_t("/Common/btn_minimize.bmp"),&pStream,&Error);
	//	if(pStream)
	//		pGroupDlg->m_Mini.LoadBitmapFromStream(pStream);
	//	pStream = NULL;
	
	//	m_Load.Load(Path+bstr_t("/Shell/GroupSend/selectall.bmp"),&pStream,&Error);
	//	if(pStream)
	//		pGroupDlg->m_SelectAll.LoadBitmapFromStream(pStream);
	//	pStream = NULL;
	///////////////////////////////////////////////////////////////////////
	pGroupDlg->SetRecipientGroup(m_strGroupName);
	pGroupDlg->ShowWindow(SW_SHOWNORMAL);
	pGroupDlg->SetForegroundWindow();
}

void CMainDlg::OnUpdateTreegroupSendfile(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(ConnectEnable(FALSE));
}

void CMainDlg::OnUpdateTreegroupSendmessage(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(ConnectEnable(FALSE));
}

void CMainDlg::RemoveAllMessageById(long UserId)
{
	BOOL bDelete = FALSE;
	
	CMessage *pMsg     = NULL; 
	
	try
	{
		while((pMsg = FindMessageByIDAndDel(UserId))!=NULL)
		{
			///// Пометить что мессага прочитана ...
			MarkMessagesAsRead(bstr_t(pMsg->GetMessageID()));
			delete pMsg;
			pMsg = NULL;
			bDelete = TRUE;
		}
		
	}
	catch(...)
	{
		ASSERT(FALSE);
	}
	
	if(bDelete)
	{
		UpdateID(UserId,FALSE);
	}
	PostMessage(WM_CHANGE_NEWMESSAGE,(WPARAM)m_NewMessageArray.GetSize());
}

LRESULT CMainDlg::OnSendGroupFile(WPARAM w, LPARAM l)
{
	CString	GroupName = (BSTR)w;
	
	if(!GroupName.IsEmpty())
	{
		HGLOBAL hGlobal = (HGLOBAL)l;
		
		HDROP hDropInfo = (HDROP)GlobalLock(hGlobal);
		
		UINT FileCount = DragQueryFile(hDropInfo,0xFFFFFFFF,NULL,0);
		
		if(FileCount>30)
		{
			CString	strMessage;
			strMessage.LoadString(IDS_FILES_SEND_LIMIT);
			MessageBox(strMessage,GetString(IDS_ERROR_TITLE),MB_OK|MB_ICONERROR);
			return 0;
		}
		
		CString strDescription;
		CGroupFileDescriptionDlg	DescrDlg(this);
		
		DescrDlg.m_strFileName = _T("");
		for(UINT i=0;i<FileCount;i++)
		{
			CString strPath;
			TCHAR  FileBuffer[MAX_PATH];
			DragQueryFile(hDropInfo,i,FileBuffer,MAX_PATH);
			strPath = FileBuffer;
			strPath = strPath.Mid(strPath.ReverseFind('\\')+1);
			
			DescrDlg.m_strFileName += strPath;
			DescrDlg.m_strFileName += _T("; ");
		}
		
		DescrDlg.m_strRecepientGroupName = GroupName;
		
		if(DescrDlg.DoModal()==IDOK)
		{
			//			strDescription = DescrDlg.m_strFileDescription;	
			strDescription = DescrDlg.GetFileDescription();
			
			for(UINT i=0;i<FileCount;i++)
			{
				TCHAR FileBuffer[MAX_PATH];
				DragQueryFile(hDropInfo,i,FileBuffer,MAX_PATH);
				
				CString strUserId	=	_T("");
				if(POSITION pos = DescrDlg.m_ContactList.InitIteration())
				{
					CUser	*pUser =	 NULL;
					while(DescrDlg.m_ContactList.GetNext(pos,pUser))
					{
						if(pUser->m_bHasNewMessages)
						{
							TCHAR Buff[MAX_PATH];
							_ltot(pUser->GetGlobalID(), Buff, 10);
							strUserId += Buff;
							strUserId += ",";
						}
					}
					
					if(!strUserId.IsEmpty())
						m_FileManager.AddToUpload2 (FileBuffer,DescrDlg.m_strRecepientGroupName,strUserId,strDescription);
				}
			}
		}
		
		GlobalUnlock(hGlobal);
		
		GlobalFree(hGlobal);
		
		SysFreeString((BSTR)w);
	}
	
	return 0;
}

void CMainDlg::AddToUpload (CString FileName, CString Login, CString RecepientID, LPCTSTR strDescription)
{
	m_FileManager.AddToUpload2 (FileName,Login,RecepientID,strDescription);
}

void CMainDlg::OnTreemenuSendemail() 
{
	CUser *pUser = FindUserInVisualContactList(CurrTID);
	if(pUser)
	{
		CString EMail;
		EMail.Format(_T("mailto:%s"),pUser->m_strEMail);
		ShellExecute(::GetDesktopWindow(),_T("open"),EMail,NULL,NULL,SW_SHOWDEFAULT);
	}
}

void CMainDlg::OnUpdateTreemenuSendemail(CCmdUI* pCmdUI) 
{
	CUser *pUser = FindUserInVisualContactList(CurrTID);
	pCmdUI->Enable(pUser&&!pUser->m_strEMail.IsEmpty());
}

void CMainDlg::OnTreegroupSendemail() 
{
	CString	strEMail	=	_T("mailto:");
	int CLMode  = GetOptionInt(IDS_OFSMESSENGER,IDS_CLMODE,2);
	
	CUser* pUser=NULL;
	
	switch(CLMode) 
	{
	case 1:
		{
			if(POSITION pos = m_ContactList.InitIteration())
			{
				for(int i=0; m_ContactList.GetNext(pos,pUser); i++)
				{
					if(UserIsVisible(pUser->GetGlobalID())&&!pUser->m_strEMail.IsEmpty())
					{
						if(m_bShowOffline||(pUser->m_iStatus!=S_OFFLINE&&pUser->m_iStatus!=S_INVISIBLE))
							if(m_strGroupName.CompareNoCase(pUser->m_strType)==0)
							{
								strEMail += pUser->m_strEMail;
								strEMail += _T(";");
							}
					}
				}
			}
			
			if(POSITION pos = m_ExternalContactList.InitIteration())
			{
				for(int i=0; m_ExternalContactList.GetNext(pos,pUser); i++)
				{
					if(UserIsVisible(pUser->GetGlobalID())&&!pUser->m_strEMail.IsEmpty())
					{
						if(m_bShowOffline||(pUser->m_iStatus!=S_OFFLINE&&pUser->m_iStatus!=S_INVISIBLE))
							if(m_strGroupName.CompareNoCase(pUser->m_strType)==0)
							{
								strEMail += pUser->m_strEMail;
								strEMail += _T(";");
							}
					}
				}
			}
			
		}
		break;
	case 2:
		{
			if(POSITION pos = m_ContactList.InitIteration())
			{
				for(int i=0; m_ContactList.GetNext(pos,pUser); i++)
				{
					if(UserIsVisible(pUser->GetGlobalID())&&!pUser->m_strEMail.IsEmpty())
					{
						if(pUser->m_iStatus==S_OFFLINE||pUser->m_iStatus==S_INVISIBLE)
						{
							if(m_strGroupName.CompareNoCase(GetString(IDS_OFFLINE))==0)
							{
								strEMail += pUser->m_strEMail;
								strEMail += _T(";");
							}
						}
						else
							if(m_strGroupName.CompareNoCase(pUser->m_strType)==0)
							{
								strEMail += pUser->m_strEMail;
								strEMail += _T(";");
							}
					}
				}
			}
			
			if(POSITION pos = m_ExternalContactList.InitIteration())
			{
				for(int i=0; m_ExternalContactList.GetNext(pos,pUser); i++)
				{
					if(UserIsVisible(pUser->GetGlobalID())&&!pUser->m_strEMail.IsEmpty())
					{
						if(pUser->m_iStatus==S_OFFLINE||pUser->m_iStatus==S_INVISIBLE)
						{
							if(m_strGroupName.CompareNoCase(GetString(IDS_OFFLINE))==0)
							{
								strEMail += pUser->m_strEMail;
								strEMail += _T(";");
							}
						}
						else
							if(m_strGroupName.CompareNoCase(pUser->m_strType)==0)
							{
								strEMail += pUser->m_strEMail;
								strEMail += _T(";");
							}
							
					}
				}
			}
			
		}
		break;
	}
	
	if(strEMail!=_T("mailto:"))
		ShellExecute(::GetDesktopWindow(),_T("open"),strEMail,NULL,NULL,SW_SHOWDEFAULT);
}

void CMainDlg::OnUpdateTreegroupSendemail(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(ConnectEnable(FALSE));
}

int CMainDlg::GetUserCurrentStatus(long UserGlobalId)
{
	CUser *pUser = FindUserInVisualContactListByGlobalId(UserGlobalId);
	return pUser?pUser->GetStatus():S_UNKNOWN;
}

CString CMainDlg::GetUserRole()
{
	return m_User.m_strType + _T("@") + GetUserDomain();
}

/*void CMainDlg::OnBeforeNavigate2(LPDISPATCH pDisp, VARIANT FAR* URL, VARIANT FAR* Flags, VARIANT FAR* TargetFrameName, VARIANT FAR* PostData, VARIANT FAR* Headers, BOOL FAR* Cancel) 
{
//USES_CONVERSION;

  //CString strUrl = URL->bstrVal;
  //strUrl.MakeLower();
  //strUrl.Replace("#sid#",GetSID());
  //strUrl.Replace("#host#",GetServerPath());
  
	if(!m_bCatchNavigate)
	{
	m_bCatchNavigate = TRUE;
	return;
	}
	
	  long cx = 0, cy = 0, tearoff = 0;
	  GetTargetRect(pDisp, cx, cy, tearoff);
	  
		if(IsTVFileType(URL->bstrVal))
		{
		
		  CDlgTV *pDlg = new CDlgTV;
		  if(pDlg)
		  {
		  pDlg->CreateAutoKiller(URL->bstrVal, GetDesktopWindow(), cx, cy);
		  *Cancel = VARIANT_TRUE;
		  }
		  }
		  else if(tearoff == 1)
		  {
		  CWebWindow *pWnd = new CWebWindow;
		  {
		  CRect winRect;
		  GetWindowRect(&winRect);
		  
			pWnd->CreateAutoKiller(_T("/Browser/Common/skin.xml"), this, GetDesktopWindow(), winRect.left-100, winRect.top+20, cx, cy, NULL, CString(URL->bstrVal), FALSE, FALSE, TRUE);
			*Cancel = VARIANT_TRUE;
			}
			}
}*/

BOOL CMainDlg::LoadUserProfile()
{
	USES_CONVERSION;
	//////////////////////////////////////////////////////////////////////////
	// Получть путь к MyDocumets [3/21/2002]
	CString strMyDocumentPath	=	GetMyDocumetPath(GetUserRole(),GetUserID());
	m_FileManager.SetUserDocumetFolder(strMyDocumentPath);
	//////////////////////////////////////////////////////////////////////////
	// Запомнить настройки для конференций [9/24/2002]
	//HINTERNET hOpenHandle,  hResourceHandle, hConnectHandle;
				
	//hOpenHandle		= InternetOpen(NULL, INTERNET_OPEN_TYPE_PRECONFIG, NULL, NULL, INTERNET_FLAG_ASYNC);
	
	//DWORD dwFlags = INTERNET_FLAG_KEEP_CONNECTION	|
	//	INTERNET_FLAG_NO_CACHE_WRITE	|
	//	INTERNET_FLAG_RELOAD			|
	//	INTERNET_FLAG_PRAGMA_NOCACHE	|
	//	INTERNET_FLAG_NO_UI			|
	//	INTERNET_FLAG_NO_COOKIES		|
	//	INTERNET_FLAG_IGNORE_CERT_CN_INVALID  |
	//	(m_bIsSSLMode?(INTERNET_FLAG_SECURE|INTERNET_FLAG_IGNORE_REDIRECT_TO_HTTPS|INTERNET_FLAG_IGNORE_REDIRECT_TO_HTTP):0);
	
	//hConnectHandle	= InternetConnect(hOpenHandle, m_strServer, m_lPort, NULL,NULL, INTERNET_SERVICE_HTTP,dwFlags,0);
	
	//CString strRootUrl;
	//strRootUrl.Format(_T("/Intranet/%s/"),GetUserDomain());
	
	//hResourceHandle = HttpOpenRequest(hConnectHandle, _T("GET"), strRootUrl,NULL, NULL, NULL, dwFlags, 0);
				
	//InternetSetOption(hResourceHandle, INTERNET_OPTION_USERNAME, (LPVOID*)(LPCTSTR)m_User.m_strLogin, m_User.m_strLogin.GetLength()+1); 
	//InternetSetOption(hResourceHandle, INTERNET_OPTION_PASSWORD, (LPVOID*)(LPCTSTR)m_strPassword, m_strPassword.GetLength()+1);
	
	//HttpSendRequest(hResourceHandle, NULL, 0, NULL, 0);
				
	//InternetCloseHandle(hResourceHandle);
	//InternetCloseHandle(hConnectHandle);
	//InternetCloseHandle(hOpenHandle);
	
	//////////////////////////////////////////////////////////////////////////
	// Create Web Folders to documents [9/30/2002]
	//CRegKey keyReg;
	
	//if(keyReg.Open(HKEY_CURRENT_USER,_T("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders"),KEY_READ)==ERROR_SUCCESS)
	//{
	//	TCHAR strNetHood[1000];
	//	DWORD NetHoodBuff = 1000;
	
	//	if(keyReg.QueryValue(strNetHood,_T("NetHood"),&NetHoodBuff)==0&&!GetUserDomain().IsEmpty())
	//	{
	//		CString strDescription;
	//		strDescription.Format(_T("IBN Documents on %s"),GetUserDomain());
	
	//		CString strDocumentsUrl = _T("#host#/Intranet/#domain#/Documents");
	
	//		strDocumentsUrl.Replace("#sid#",GetSID());
	//		strDocumentsUrl.Replace("#host#",GetServerPath());
	//		strDocumentsUrl.Replace("#domain#",GetUserDomain());
	
	//		CString strDirPath = strNetHood;
	//		strDirPath += _T("\\");
	//		strDirPath += strDescription;
	
	//		if(!IsWinXPOrLate())
	//			strDirPath += _T(".lnk");
	
	//		CString strEXEPath;
	
	//		LPTSTR Buf = strEXEPath.GetBuffer(1000);
	//		GetModuleFileName(NULL,Buf,1000);
	//		strEXEPath.ReleaseBuffer();
	
	//		McCreateWebFolders(T2CW(strDirPath),T2CW(strDescription),T2CW(strDocumentsUrl),strEXEPath);
	//	}
	//}
	
	//////////////////////////////////////////////////////////////////////////
	
	return TRUE;
}

LRESULT CMainDlg::OnAlertPopupClk(WPARAM w, LPARAM l)
{
	USES_CONVERSION;
	
	long	Id		=	long (w);
	CComBSTR	bsUrl;
	
	if(l)
		bsUrl.Attach((BSTR)l);
	
		/*for(int i=0;i<m_AppArray.GetSize();i++)
		{
		McAppItem Item  = m_AppArray[i];
		if(Item.Id==Id)
		{
		m_AppBar.m_AppCtrl.SetCheckButton(i);
		m_treebox.ShowWindow(SW_HIDE);
		m_InWindow.ShowWindow(SW_SHOW);
		m_InWindow.SetFocus();
		m_InWindow.Navigate(Item.Url);
		SendMessage(WM_COMMAND,MAKELONG(ID_LCLK_TRAY,0),0);
		Invalidate();
		break;
		}
}*/
	
	if(bsUrl.Length())
	{
		m_AppBar.m_AppCtrl.SetCheckButton(-1);
		
		for(int i=0;i<m_AppArray.GetSize()&&Id>0;i++)
		{
			McAppItem Item  = m_AppArray[i];
			if(Item.Id==Id)
			{
				m_AppBar.m_AppCtrl.SetCheckButton(i);
				
				break;
			}
		}
		
		m_treebox.ShowWindow(SW_HIDE);
		m_chatbox.ShowWindow(SW_HIDE);
		//m_WebFolderView.ShowWindow(SW_HIDE);
		m_InWindow.ShowWindow(SW_SHOW);
		m_InWindow.SetFocus();
		
		CString strUrl = W2T(bsUrl);
		strUrl.Replace(_T("#sid#"), GetSID());
		strUrl.Replace(_T("#host#"), GetServerPath());
		m_InWindow.Navigate(strUrl);
		
		SendMessage(WM_COMMAND,MAKELONG(ID_LCLK_TRAY,0),0);
		
		Invalidate();
	}
	
	
	return 0;
}

/*
void CMainDlg::OnNavigateComplete2(LPDISPATCH pDisp, VARIANT FAR* URL)
{
if(m_pMcHost!=NULL)
SetUIHandler(FALSE);
if(m_pMcHost==NULL)
SetUIHandler(TRUE);
}
*/

/*void CMainDlg::SetUIHandler(BOOL bSet)
{

  
	// Получаем интерфейс текущего документа
	HRESULT hr;
	CComPtr<IDispatch> pDispatch;
	pDispatch.Attach(m_InWindow.m_browser.GetDocument());
	
	  if( pDispatch == NULL ) return; // только если документ загружен
	  
		CComPtr<IHTMLDocument2> pHtmlDoc;
		hr = pDispatch->QueryInterface( __uuidof( IHTMLDocument2 ), (void**)&pHtmlDoc );
		
		  if( SUCCEEDED( hr ) )
		  {
		  // Получаем ICustomDoc
		  CComPtr<ICustomDoc> pCustomDoc;
		  hr = pHtmlDoc->QueryInterface( __uuidof( ICustomDoc ), (void**)&pCustomDoc );
		  
			if( SUCCEEDED( hr ) )
			{
			// Устанавливаем свою реализацию IDocHostUIHandler
			if (bSet)
			{
			m_pMcHost = new CMcImpIDocHostUIHandler(m_InWindowDropTarget);
			// Указатель на объект вид потребуется для пересылки команд 
			// контекстного меню 
			m_pMcHost->AddRef();
			hr = pCustomDoc->SetUIHandler( (IDocHostUIHandler*)m_pMcHost);
			} else
			// Отменяем
			{	
			if(m_pMcHost)
			m_pMcHost->Release();
			hr = pCustomDoc->SetUIHandler( NULL );
			m_pMcHost =  NULL;
			}
			}
			}
			
}*/

LPDISPATCH CMainDlg::GetInWindowDocument()
{
	return m_InWindow.m_browser.GetDocument();
}

BOOL CMainDlg::ShowWebWindow(LPCTSTR Url, CComVariant*	pFileMas)
{
	CString strUrl = Url;
	//strUrl.MakeLower();
	strUrl.Replace(_T("#sid#"), GetSID());
	strUrl.Replace(_T("#host#"), GetServerPath());
	
	CWebWindow *pChild = new CWebWindow();
	
	if(pFileMas)
		pChild->varFilesData	=	*pFileMas;
	
	pChild->CreateAutoKiller(_T("/Browser/Common/skin.xml"),
		GetMessageParent(),GetDesktopWindow(),CW_USEDEFAULT,0,490,440,
		NULL,strUrl,FALSE,FALSE,FALSE, 0, TRUE, FALSE);
	
	return TRUE;				
}

BOOL CMainDlg::UploadAppFile(LPCTSTR strXML)
{
	//MessageBox(strXML,_T("OnUploadAppFile"));
	m_FileManager.AddToUpload3(strXML);
	
	return TRUE;
}

void CMainDlg::OnDestroy() 
{
	//m_InWindowDropTarget->Revoke();
	
	//m_InWindowDropTarget->ExternalRelease();
	
	COFSNcDlg2::OnDestroy();
}

void CMainDlg::OnSize(UINT nType, int cx, int cy) 
{
	if(nType==SIZE_MINIMIZED)
	{
		if(GetOptionInt(IDS_OFSMESSENGER,IDS_HIDEINTRAY,FALSE)==TRUE||
			GetOptionInt(IDS_OFSMESSENGER,IDS_REMOVE_FROM_TASK_BAR,TRUE))
			ShowWindow(SW_HIDE);
	}
	
	COFSNcDlg2::OnSize(nType, cx, cy);
}

void CMainDlg::OnPaint()
{
	COFSNcDlg2::OnPaint();
	m_StatusStatic.Invalidate(FALSE);
	m_UserStatic.Invalidate(FALSE);
	m_picLogo.Invalidate(FALSE);
	m_AppBar.Invalidate(FALSE);
}

BOOL CMainDlg::LoadAppBar(IXMLDOMNode *pXmlRoot)
{	
	CComPtr<IStream>	pStream;
	LoadSkins skin;
	CString strErrorMessage;
	long nErrorCode = 0;
	
	CComBSTR bsImage1,bsImage2, bsImageIM, bsImagePath, bsPath;
	CComBSTR /*bsImageDOCS, bsImageLINKS, bsImageNEWS,*/ /*bsImageWF,*/ bsImageCC, bsImageIA;
	
	bsImage1 = L"Rectangle[@Name=\"";
	bsImage1 += _T("AppBar");
	bsImage1 += L"\"]/";
	
	bsImage2		=  bsImage1;
	bsImageIM		=  bsImage1;
	//bsImageDOCS		=  bsImage1;
	//bsImageLINKS	=  bsImage1;
	//bsImageNEWS		=  bsImage1;
	//bsImageWF		=  bsImage1;
	bsImageCC		=  bsImage1;
	bsImageIA	=  bsImage1;
	
	bsImage1 += L"Image1";
	bsImage2 += L"Image2";
	bsImageIM += L"ImageIM"; 
	//bsImageDOCS += L"ImageDOCS";
	//bsImageLINKS += L"ImageLINKS";
	//bsImageNEWS += L"ImageNEWS";
	//bsImageWF += L"ImageWF";
	bsImageCC += L"ImageCC";
	bsImageIA	+=  L"ImageIA";
	
	SelectChildNode(pXmlRoot, bsImage1, NULL, &bsPath);
	//  [4/6/2002]
	bsImagePath = (BSTR)bstr_t(IBN_SCHEMA);
	bsImagePath += (LPCTSTR)GetCurrentSkin();
	bsImagePath += bsPath;
	
	// Step 1. Load Image 1  [4/17/2002]			
	skin.Load(bsImagePath, &pStream, &nErrorCode);
#ifdef _DEVELOVER_VERSION_L1
	if(pStream == NULL || nErrorCode) 
	{
		strErrorMessage = "Skin Error: Can't Load Image ( ";
		strErrorMessage += (BSTR)bsImagePath;
		strErrorMessage +=" )";
		AfxMessageBox(strErrorMessage);
	}
#endif
	
	if(pStream)
	{
		LPPICTUREDISP	PicDisp	=	NULL;
		HRESULT hr = ::OleLoadPicture(pStream,	0, TRUE, IID_IPictureDisp,(void**)&PicDisp);
		if(SUCCEEDED(hr))
		{
			m_AppBar.m_AppCtrl.SetImage(PicDisp);
			PicDisp->Release();
		}
	}
	
	bsPath.Empty();
	pStream = NULL;
	
	// Step 2. Load Image 2 [4/17/2002]
	SelectChildNode(pXmlRoot, bsImage2, NULL, &bsPath);
	
	bsImagePath = (BSTR)_bstr_t(IBN_SCHEMA);
	bsImagePath += (LPCTSTR)GetCurrentSkin();
	bsImagePath += bsPath;
	
	skin.Load(bsImagePath, &pStream, &nErrorCode);
#ifdef _DEVELOVER_VERSION_L1
	if(pStream == NULL || nErrorCode) 
	{
		strErrorMessage = "Skin Error: Can't Load Image ( ";
		strErrorMessage += (BSTR)bsImagePath;
		strErrorMessage +=" )";
		AfxMessageBox(strErrorMessage);
	}
#endif
	if(pStream)
	{
		LPPICTUREDISP	PicDisp	=	NULL;
		HRESULT hr = ::OleLoadPicture(pStream,	0, TRUE, IID_IPictureDisp,(void**)&PicDisp);
		if(SUCCEEDED(hr))
		{
			m_AppBar.m_AppCtrl.SetMoreBtnImage(PicDisp);
			PicDisp->Release();
		}
	}
	bsPath.Empty();
	pStream = NULL;
	
	// Step 3. Load Image IM  [4/17/2002]	
	SelectChildNode(pXmlRoot, bsImageIM, NULL, &bsPath);
	
	bsImagePath = (BSTR)bstr_t(IBN_SCHEMA);
	bsImagePath += (LPCTSTR)GetCurrentSkin();
	bsImagePath += bsPath;
	
	skin.Load(bsImagePath, &pStream, &nErrorCode);
#ifdef _DEVELOVER_VERSION_L1
	if(pStream == NULL || nErrorCode) 
	{
		strErrorMessage = "Skin Error: Can't Load Image ( ";
		strErrorMessage += (BSTR)bsImagePath;
		strErrorMessage +=" )";
		AfxMessageBox(strErrorMessage);
	}
#endif
	
	if(pStream)
	{
		LPPICTUREDISP	PicDisp	=	NULL;
		HRESULT hr = ::OleLoadPicture(pStream,	0, TRUE, IID_IPictureDisp,(void**)&PicDisp);
		if(SUCCEEDED(hr))
		{
			m_picIM.SetPictureDispatch(PicDisp);
		}
	}
	
	bsPath.Empty();
	pStream = NULL;
	
	// Step 3. Load Image DOCS [4/17/2002]	
	/*SelectChildNode(pXmlRoot, bsImageDOCS, NULL, &bsPath);
	
	  bsImagePath = bstr_t(IBN_SCHEMA);
	  bsImagePath += (LPCTSTR)GetCurrentSkin();
	  bsImagePath += bsPath;
	  
		skin.Load(bsImagePath, &pStream, &nErrorCode);
		#ifdef _DEVELOVER_VERSION_L1
		if(pStream == NULL || nErrorCode) 
		{
		strErrorMessage = "Skin Error: Can't Load Image ( ";
		strErrorMessage += (BSTR)bsImagePath;
		strErrorMessage +=" )";
		AfxMessageBox(strErrorMessage);
		}
		#endif
		
		  if(pStream)
		  {
		  LPPICTUREDISP	PicDisp	=	NULL;
		  HRESULT hr = ::OleLoadPicture(pStream,	0, TRUE, IID_IPictureDisp,(void**)&PicDisp);
		  if(SUCCEEDED(hr))
		  {
		  m_picDOCS.SetPictureDispatch(PicDisp);
		  }
		  }
		  
			bsPath.Empty();
	pStream = NULL;*/
	
	// Step 3. Load Image LINKS [4/17/2002]	
	/*SelectChildNode(pXmlRoot, bsImageLINKS, NULL, &bsPath);
	
	  bsImagePath = bstr_t(IBN_SCHEMA);
	  bsImagePath += (LPCTSTR)GetCurrentSkin();
	  bsImagePath += bsPath;
	  
		skin.Load(bsImagePath, &pStream, &nErrorCode);
		#ifdef _DEVELOVER_VERSION_L1
		if(pStream == NULL || nErrorCode) 
		{
		strErrorMessage = "Skin Error: Can't Load Image ( ";
		strErrorMessage += (BSTR)bsImagePath;
		strErrorMessage +=" )";
		AfxMessageBox(strErrorMessage);
		}
		#endif
		
		  if(pStream)
		  {
		  LPPICTUREDISP	PicDisp	=	NULL;
		  HRESULT hr = ::OleLoadPicture(pStream,	0, TRUE, IID_IPictureDisp,(void**)&PicDisp);
		  if(SUCCEEDED(hr))
		  {
		  m_picLINKS.SetPictureDispatch(PicDisp);
		  }
		  }
		  
			bsPath.Empty();
	pStream = NULL;*/
	
	// Step 3. Load Image NEWS [4/17/2002]	
	/*SelectChildNode(pXmlRoot, bsImageNEWS, NULL, &bsPath);
	
	  bsImagePath = bstr_t(IBN_SCHEMA);
	  bsImagePath += (LPCTSTR)GetCurrentSkin();
	  bsImagePath += bsPath;
	  
		skin.Load(bsImagePath, &pStream, &nErrorCode);
		#ifdef _DEVELOVER_VERSION_L1
		if(pStream == NULL || nErrorCode) 
		{
		strErrorMessage = "Skin Error: Can't Load Image ( ";
		strErrorMessage += (BSTR)bsImagePath;
		strErrorMessage +=" )";
		AfxMessageBox(strErrorMessage);
		}
		#endif
		
		  if(pStream)
		  {
		  LPPICTUREDISP	PicDisp	=	NULL;
		  HRESULT hr = ::OleLoadPicture(pStream,	0, TRUE, IID_IPictureDisp,(void**)&PicDisp);
		  if(SUCCEEDED(hr))
		  {
		  m_picNEWS.SetPictureDispatch(PicDisp);
		  }
		  }
		  
			bsPath.Empty();
	pStream = NULL;*/
	
	// Load WF Button [10/2/2002]
	/*SelectChildNode(pXmlRoot, bsImageWF, NULL, &bsPath);
	
	  bsImagePath = bstr_t(IBN_SCHEMA);
	  bsImagePath += (LPCTSTR)GetCurrentSkin();
	  bsImagePath += bsPath;
	  
		skin.Load(bsImagePath, &pStream, &nErrorCode);
		#ifdef _DEVELOVER_VERSION_L1
		if(pStream == NULL || nErrorCode) 
		{
		strErrorMessage = "Skin Error: Can't Load Image ( ";
		strErrorMessage += (BSTR)bsImagePath;
		strErrorMessage +=" )";
		AfxMessageBox(strErrorMessage);
		}
		#endif
		
		  if(pStream)
		  {
		  LPPICTUREDISP	PicDisp	=	NULL;
		  HRESULT hr = ::OleLoadPicture(pStream,	0, TRUE, IID_IPictureDisp,(void**)&PicDisp);
		  if(SUCCEEDED(hr))
		  {
		  m_picWF.SetPictureDispatch(PicDisp);
		  }
		  }
		  
			bsPath.Empty();
pStream = NULL;*/

// Load CC Button [10/2/2002]
SelectChildNode(pXmlRoot, bsImageCC, NULL, &bsPath);

bsImagePath = (BSTR)bstr_t(IBN_SCHEMA);
bsImagePath += (LPCTSTR)GetCurrentSkin();
bsImagePath += bsPath;

skin.Load(bsImagePath, &pStream, &nErrorCode);
#ifdef _DEVELOVER_VERSION_L1
if(pStream == NULL || nErrorCode) 
{
	strErrorMessage = "Skin Error: Can't Load Image ( ";
	strErrorMessage += (BSTR)bsImagePath;
	strErrorMessage +=" )";
	AfxMessageBox(strErrorMessage);
}
#endif

if(pStream)
{
	LPPICTUREDISP	PicDisp	=	NULL;
	HRESULT hr = ::OleLoadPicture(pStream,	0, TRUE, IID_IPictureDisp,(void**)&PicDisp);
	if(SUCCEEDED(hr))
	{
		m_picCC.SetPictureDispatch(PicDisp);
	}
}

bsPath.Empty();
pStream = NULL;


// Load IBN Actions Button [10/2/2002]
SelectChildNode(pXmlRoot, bsImageIA, NULL, &bsPath);

bsImagePath = (BSTR)bstr_t(IBN_SCHEMA);
bsImagePath += (LPCTSTR)GetCurrentSkin();
bsImagePath += bsPath;

skin.Load(bsImagePath, &pStream, &nErrorCode);
#ifdef _DEVELOVER_VERSION_L1
if(pStream == NULL || nErrorCode) 
{
	strErrorMessage = "Skin Error: Can't Load Image ( ";
	strErrorMessage += (BSTR)bsImagePath;
	strErrorMessage +=" )";
	AfxMessageBox(strErrorMessage);
}
#endif

if(pStream)
{
	LPPICTUREDISP	PicDisp	=	NULL;
	HRESULT hr = ::OleLoadPicture(pStream,	0, TRUE, IID_IPictureDisp,(void**)&PicDisp);
	if(SUCCEEDED(hr))
	{
		m_picIA.SetPictureDispatch(PicDisp);
	}
}

bsPath.Empty();
pStream = NULL;


// Step 4. Load Position [4/17/2002]
LoadRectangle(pXmlRoot,_T("AppBar"),&m_AppBar,TRUE);

// Step 5. Load Spasing Value [4/17/2002]
CComBSTR bs;
WCHAR *szNULL = L"\0x00";
long nSpacing = 0;
SelectChildNode(pXmlRoot, CComBSTR(L"Rectangle[@Name='AppBar']/Spacing"), NULL, &bs);
if(bs.m_str != NULL)
nSpacing = wcstol(bs.m_str, &szNULL, 10);
if(nSpacing > 0)
m_AppBar.m_AppCtrl.SetSpacing(nSpacing);

// Step 6. Load BkColor [4/17/2002]
bs.Empty();
SelectChildNode(pXmlRoot, CComBSTR(L"Rectangle[@Name='AppBar']/Color"), NULL, &bs);
if(bs.m_str != NULL)
{
	m_AppBar.m_dwBkgColor = wcstol(bs.m_str, &szNULL, 16);
}


// End [4/17/2002]

return TRUE;
}

// Alert -> Tools [4/16/2004]
void CMainDlg::OnClickButtonAlerts()
{
	// Show Tools Menu [4/16/2004]
	CPoint	point;
	GetCursorPos(&point);
	
	CMenu menu;
	menu.LoadMenu(IDR_MESSENGER_MENU);
	CMenu* popup = menu.GetSubMenu(10);
	
	UpdateMenu(this,popup);
	popup->TrackPopupMenu(TPM_LEFTALIGN|TPM_RIGHTBUTTON, point.x, point.y, this);
				
	
	//m_btnAlerts.SetPressed(TRUE);
	
	//CString strUrl;
	//strUrl.Format(GetString(IDS_WEB_ALERTLIST),GetServerPath(),GetSID());
	
	//if(m_InWindow.NavigateNewWindow(strUrl)!=S_OK)
	//	ShellExecute(NULL,_T("open"),strUrl,NULL,NULL,SW_SHOWNORMAL);
	
	//#host#/Apps/Alerts/clientAlerts.asp?sid=#sid#
	
	//m_AppBar.m_AppCtrl.SetCheckButton(-1);
	//m_treebox.ShowWindow(SW_HIDE);
	//m_chatbox.ShowWindow(SW_HIDE);
	//m_WebFolderView.ShowWindow(SW_HIDE);
	//m_InWindow.ShowWindow(SW_SHOW);
	//m_InWindow.SetFocus();
	//m_InWindow.Navigate(strUrl);
	//Invalidate(FALSE);
}

void CMainDlg::OnClickButtonApps()
{
	CPoint	point;
	GetCursorPos(&point);
	//ScreenToClient(&point);
	m_AppBar.m_AppCtrl.ShowAppMenu(point);
}

void CMainDlg::OnClickButtonFiles()
{
	m_FileManager.ShowDialog();
}

void CMainDlg::OnClickButtonShowOfflineUser()
{
	m_bShowOffline = m_btnShowOffline.GetPressed();
	BuildContactList();
}

void CMainDlg::OnClickButtonDirectory()
{
	//OnOptionsAddiniviteFinduseraddtofriends();
	
	/*m_AppBar.m_AppCtrl.SetCheckButton(-1);
	
	  if(m_ChatCollection.GetSize()==0)
	  {
	  m_chatbox.ShowWindow(SW_HIDE);
	  m_treebox.ShowWindow(SW_HIDE);
	  m_WebFolderView.ShowWindow(SW_HIDE);
	  m_InWindow.ShowWindow(SW_SHOW);
	  m_InWindow.Navigate(_T("IBN_SCHEMA://default/Shell/Conference/blank.html"));
	  }
	  else
	  {
	  m_chatbox.ShowWindow(SW_SHOW);
	  m_treebox.ShowWindow(SW_HIDE);
	  m_WebFolderView.ShowWindow(SW_HIDE);
	  m_InWindow.ShowWindow(SW_HIDE);
	  m_InWindow.Navigate(_T("IBN_SCHEMA://default/Common/blank.html"));
}*/
	
	CString strIBNTodaysUrl;
	strIBNTodaysUrl.Format(GetString(IDS_WEB_IBNTODAY),GetServerPath(),GetSID());
	
	if(m_InWindow.NavigateNewWindow(strIBNTodaysUrl)!=S_OK)
		ShellExecute(NULL,_T("open"),strIBNTodaysUrl,NULL,NULL,SW_SHOWNORMAL);
	//Invalidate();
}


void CMainDlg::OnUpdateOptionsLogOff(CCmdUI* pCmdUI)
{
	UpdateStatus(S_OFFLINE,pCmdUI);
}

void CMainDlg::OnOptionsLogOff()
{
	OnStatusOffline();
}

void CMainDlg::OnActivate(UINT nState, CWnd* pWndOther, BOOL bMinimized) 
{
	COFSNcDlg::OnActivate(nState, pWndOther, bMinimized);
}

void CMainDlg::OnSetFocus( CWnd* pOldWnd)
{
	COFSNcDlg2::OnSetFocus(pOldWnd);
}

BOOL CMainDlg::OnQueryEndSession()
{
	return TRUE; // Confirm that we are ready to close.
}

void CMainDlg::OnEndSession(BOOL bEnding)
{
	if(bEnding)
	{
		// Prepare for closing (save user data, etc).
		//OnClose();
	}
}

void CMainDlg::OnUpdateTrayShowNewMessage(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(m_NewMessageArray.GetSize());
}

void CMainDlg::OnTrayShowNewMessage()
{
	OnCheckSignalState(WM_CHECK_SIGNAL_STATE,0);
}

void CMainDlg::OnChatDoDropTreectrl(long TID, BOOL bGroupe,LPUNKNOWN pDataObject, long DropEffect, long LastKeyState)
{
	if(bGroupe)
		CurrChatTID = -1;
	else
		CurrChatTID = TID;
}

void CMainDlg::OnChatMenuTreectrl(long TID, BOOL bGroupe)
{
	if(bGroupe)
		CurrChatTID = -1;
	else
		CurrChatTID = TID;
	
	if(TID!= -1)
		if(!bGroupe)
		{
			CPoint point;
			GetCursorPos(&point);
			CMenu menu;
			menu.LoadMenu(IDR_MESSENGER_MENU);
			CMenu* popup = menu.GetSubMenu(7);
			UpdateMenu(this,popup);
			popup->TrackPopupMenu(TPM_LEFTALIGN|TPM_RIGHTBUTTON, point.x, point.y, this);
		}
		else
		{
		/*m_strGroupName = m_treebox.GetItemText(TID);
		
		  CPoint point;
		  GetCursorPos(&point);
		  CMenu menu;
		  menu.LoadMenu(IDR_MESSENGER_MENU);
		  CMenu* popup = menu.GetSubMenu(4);
		  UpdateMenu(this,popup);
			popup->TrackPopupMenu(TPM_LEFTALIGN|TPM_RIGHTBUTTON, point.x, point.y, this);*/
		}
}

void CMainDlg::OnChatSelectTreectrl(long TID, BOOL bGroupe)
{
	if(bGroupe)
	{
		CurrChatTID = -1;
		
	}
	else
		CurrChatTID = TID;
}

void CMainDlg::OnChatActionTreectrl(long TID, BOOL bGroupe)
{
	if(bGroupe)
		CurrChatTID = -1;
	else
		CurrChatTID = TID;
	
	CChat SelChat;
	
	if(CurrChatTID!=-1&&FindChatByTID(CurrChatTID,SelChat))
	{
		if(SelChat.GetStatus()==SC_ACTIVE)
		{
			OnChatSendMessage();
		}
		else
		{
			SetChatStatus(SelChat.GetId(),SC_ACTIVE,HCI_OPEN_CHAT_WINDOW);
			//OnChatStatusActive();
		}
	}
	
	
}

void CMainDlg::CreateChatTree()
{
	LoadSkins m_LoadSkin;
	
	IStreamPtr pStream = NULL;
	long Error=0;
	m_LoadSkin.Load(bstr_t(IBN_SCHEMA)+ bstr_t((LPCTSTR)GetProductLanguage())+ bstr_t("/Shell/Main/chat_status.bmp"),&pStream,&Error);
	if(pStream)
	{	
		CDib dib(pStream);
		CPaintDC dc(this);
		HBITMAP hBmp = dib.GetHBITMAP((HDC)dc);
		m_chatbox.SetImageList((long)hBmp);
		DeleteObject(hBmp);
	}
	
	short PriorityIndex[10];
	for(int i=0;i<10;i++)
		PriorityIndex[i] = -1;
	PriorityIndex[0] = 1;
	PriorityIndex[1] = 0;
	
	m_chatbox.SetPriority(PriorityIndex);
	
	for(int i = 0 ;i<ChatMaxValueID;i++)
	{
		m_chatbox.AddEffect(m_ChatShablonId[i],m_ChatShablonIcon[i],m_ChatShablonRGBTextEnable[i],
			m_ChatShablonRGBTextSelect[i],m_ChatShablonRGBFonEnable[i],m_ChatShablonRGBFonSelect[i]);
	}
	
	m_chatbox.SetEventMode(1);
}

void CMainDlg::LoadChats(IChats *pChats)
{
	USES_CONVERSION;
	// CMainDlg::LoadChats [8/6/2002]
	m_ChatCollection.RemoveAll();
	
	long lChatsCount	=	0;
	pChats->get_Count(&lChatsCount);
	
	// Load Active Chats from REM [9/9/2002]
	if(m_ActiveChatRem.GetSize()==0)
	{
		CString	strUserId;
		strUserId.Format(_T("%d"), GetUserID());
		
		CString strActiveChatId = theApp.GetProfileString(GetString(IDS_INFO) + _T("\\") + GetUserRole()+_T("\\")+strUserId,GetString(IDS_CHAT));
		
		if(!strActiveChatId.IsEmpty())
		{
			while(!strActiveChatId.IsEmpty())
			{
				int TmpIndex = strActiveChatId.Find(_T(';'));
				if(TmpIndex==-1)
					break;
				
				CString	strTmpId	=	strActiveChatId.Left(TmpIndex);
				m_ActiveChatRem.Add(strTmpId.AllocSysString());
				
				strActiveChatId = strActiveChatId.Mid(TmpIndex+1);
			}
		}
	}
	// End Load Active Chats from REM [9/9/2002]
	
	
	for(int iChatIndex = 1;iChatIndex<=lChatsCount;iChatIndex++)
	{
		IChatPtr pChat;
		try
		{
			pChat = pChats->GetItem(iChatIndex);
			
			CChat	tmpChat(pChat);
			m_ChatCollection.Add(tmpChat);
			
			for(int iIndex = 0; iIndex<m_ActiveChatRem.GetSize();iIndex++)
			{
				if(tmpChat.GetId()==m_ActiveChatRem[iIndex])
				{
					
					long Handle = 0;
					theNet2.LockTranslator();
					try
					{
						tmpChat->SetStatus(SC_ACTIVE,GetOptionInt(IDS_OFSMESSENGER,IDS_CHATLOADMESS,20),&Handle);
						if(Handle)
						{
							theNet2.AddToTranslator(Handle,GetSafeHwnd());
							m_ChatHandleMap.SetAt(Handle,W2CT(tmpChat.GetId()));
						}
					}
					catch (_com_error&) 
					{
					}
					theNet2.UnlockTranslator();
					
					break;
				}
			}
			
		}
		catch (_com_error&) 
		{
			ASSERT(FALSE);
		}
	}
	
	// Refresh Rem [9/9/2002]
	for(int iIndex = 0; iIndex<m_ActiveChatRem.GetSize();iIndex++)
	{
		SysFreeString(m_ActiveChatRem[iIndex]);
	}
	m_ActiveChatRem.RemoveAll();
	
	// End Refresh Rem [9/9/2002]
	
	
	
	BuildChatsList();
}

void CMainDlg::BuildChatsList()
{
	// Step 1. Maybe Save Setting  [8/7/2002]
	
	// Step 2. Remove All Items from Chat Tree [8/7/2002]
	m_chatbox.DeleteTree();
	
	m_ChatGroupTIDMap.RemoveAll();
	
	CurrChatTID	=	-1;
	
	long	GroupTID	= 0;
	
	// Step 3. Create a new Item in the Chat Tree [8/7/2002]
	
	for(int iChatIndex = 0; iChatIndex < m_ChatCollection.GetSize();iChatIndex++)
	{
		CChat&	Chat	=	m_ChatCollection.ElementAt(iChatIndex);
		
		CString str1 = Chat.GetShowName();
		
		if(Chat.GetStatus()==SC_INACTIVE)
		{
			if(!m_ChatGroupTIDMap.Lookup(GetString(IDS_INACTIVE_CHAT),(void*&)GroupTID))
			{
				long ShablonId[10] = {0L,1L,0L,0L,0L,0L,0L,0L,0L,0L};
				// Step 2. Если нет, то создать группу .
				GroupTID = m_chatbox.AddItem(0,GetString(IDS_INACTIVE_CHAT),ShablonId);
				
				m_ChatGroupTIDMap.SetAt(GetString(IDS_INACTIVE_CHAT),(void*)GroupTID);
			}
			
			//m_chatbox.RootOpen(GroupTID,FALSE);
		}
		else if(Chat.GetStatus()==SC_ACTIVE)
		{
			if(!m_ChatGroupTIDMap.Lookup(GetString(IDS_ACTIVE_CHAT),(void*&)GroupTID))
			{
				// Step 2. Если нет, то создать группу .
				GroupTID = m_chatbox.AddItem(0,GetString(IDS_ACTIVE_CHAT),m_ChatShablonId[0]);
				
				m_ChatGroupTIDMap.SetAt(GetString(IDS_ACTIVE_CHAT),(void*)GroupTID);
			}
		}
		
		Chat.SetTID(m_chatbox.AddItem(GroupTID,Chat.GetShowName(),m_ChatShablonId[Chat.GetStatus()+1]));								
		
	}
}

void CMainDlg::DisconnectAllChats()
{
/*	for(int iIndex = 0; iIndex<m_ActiveChatRem.GetSize();iIndex++)
{
SysFreeString(m_ActiveChatRem[iIndex]);
}
	m_ActiveChatRem.RemoveAll();*/
	
	for(int iChatIndex = 0; iChatIndex < m_ChatCollection.GetSize();iChatIndex++)
	{
		CChat&	Chat	=	m_ChatCollection.ElementAt(iChatIndex);
		
		// Remember Chat Status for Auto Connecting [8/26/2002]
		//if(Chat.GetStatus()==SC_ACTIVE)
		//{
		//	m_ActiveChatRem.Add(Chat.GetId().Copy());
		//}
		// Reset Chat Status [8/26/2002]
		Chat.SetStatus(SC_INACTIVE);
		
		// Step 4. Find Open ChatDlg for Chat and refresh it[8/8/2002]
		CMcWindowAgent winAgent(Chat.GetChatWindow());
		
		if(winAgent.IsValid())
		{
			winAgent.Refresh();
		}
	}
}

void CMainDlg::OnChatViewDetails()
{
	CChat SelChat;
	if(CurrChatTID!=-1&&FindChatByTID(CurrChatTID,SelChat))
	{
		USES_CONVERSION;
		// Step 1. Show Chat Details Dlg [8/8/2002]
		//CChatCreateDlg	*pChatDetailsDlg = new CChatCreateDlg(this,GetDesktopWindow());
		
		//pChatDetailsDlg->Create(CChatCreateDlg::CCDM_DETAIL,W2CT(SelChat.GetName()),W2CT(SelChat.GetDescription()),NULL,&(SelChat.GetUsers()),SelChat.GetId());
		//m_AllClosedWindow.Add(pChatDetailsDlg->GetSafeHwnd());
		// Load UserCollections to  CChatCreateDlg [8/8/2002]
		//pChatDetailsDlg->ShowWindow(SW_SHOW);
		
		/************************************************************************/
		/* Conference Details XML
		- <Package>
		- <Chats>
		- <Chat>
		<chat_id>1</chat_id> 
		<name>test chat</name> 
		<desc>lknlkn</desc> 
		<begin_time>1029137894</begin_time> 
		<owner_id>1</owner_id> 
		<first_name>Corporate</first_name> 
		<last_name>UserOne</last_name> 
		<mess_count>2</mess_count> 
		</Chat>
		</Chats>
		- <Users>
		- <User>
		<user_id>1</user_id> 
		<accepted>1</accepted> 
		<exited>False</exited> 
		<user_status>1</user_status> 
		<first_name>Corporate</first_name> 
		<last_name>UserOne</last_name> 
		</User>
		- <User>
		<user_id>2</user_id> 
		<accepted>1</accepted> 
		<exited>False</exited> 
		<user_status>0</user_status> 
		<first_name>Corporate</first_name> 
		<last_name>UserTwo</last_name> 
		</User>
		</Users>
		</Package>
		*/
		/************************************************************************/
		
		CString		strDetailsURL, strUrl;
		strDetailsURL.Format(GetString(IDS_WEB_CHATDETAILS),GetServerPath(),GetSID(),W2CT(SelChat.GetId()));
		
		CWebWindow *pNewWindow = new CWebWindow;
		
		strUrl.Format(CString(IBN_SCHEMA) + CString(_T("%s/Shell/Conference/Details/index.html")),::GetCurrentSkin());
		
		CRect			winRect;
		GetWindowRect(&winRect);
		
		CString strTitle;
		
		strTitle.Format(GetString(IDS_DETAILS_CONFERENCE),SelChat.GetShowName());
		
		pNewWindow->CreateAutoKiller(_T("/Shell/Conference/Details/skin.xml"), this, GetDesktopWindow(), winRect.left-100,winRect.top+20, 300, 150, strTitle, strUrl, FALSE, FALSE, TRUE,IDS_USERDETAILS);
		
		pNewWindow->LoadXML(strDetailsURL);
		
		m_AllClosedWindow.Add(pNewWindow->GetSafeHwnd());
	}
}

void CMainDlg::OnChatMessageHistory()
{
	CChat SelChat;
	if(CurrChatTID!=-1&&FindChatByTID(CurrChatTID,SelChat))
	{
		// Step 1. Show Chat History Dlg [8/8/2002]
		// #host#/chat_history.asp?sid=#sid#
		///ShowUserDetails(m_User.GetGlobalID());
		CString strChatMessageHistoryUrl	=	GetServerPath();
		strChatMessageHistoryUrl.Format(GetString(IDS_WEB_CHATHISTORY),GetServerPath(),GetSID());
		
		if(m_InWindow.NavigateNewWindow(strChatMessageHistoryUrl)!=S_OK)
			ShellExecute(NULL,_T("open"),strChatMessageHistoryUrl,NULL,NULL,SW_SHOWNORMAL);
	}
}

void CMainDlg::OnChatLeave()
{
	USES_CONVERSION;
	
	CChat SelChat;
	if(CurrChatTID!=-1&&FindChatByTID(CurrChatTID,SelChat))
	{
		// Step 1. Chat Leave Dlg ...
		CDelChatDlg	*pDelDlg = new CDelChatDlg(this);
		pDelDlg->Create(CDelChatDlg::IDD,GetDesktopWindow());
		pDelDlg->SetKillChat(SelChat);
		pDelDlg->ShowWindow(SW_SHOW);
		pDelDlg->SetForegroundWindow();
	}
}

void CMainDlg::OnChatEditDetails()
{
	USES_CONVERSION;
	CChat SelChat;
	if(CurrChatTID!=-1&&FindChatByTID(CurrChatTID,SelChat))
	{
		// Step 1. Chat Edit Details Dlg ...  [8/8/2002]
		CChatCreateDlg	*pNewChatDlg	=	new CChatCreateDlg(this);
		
		pNewChatDlg->Create(CChatCreateDlg::CCDM_UPDATE,W2CT(SelChat.GetName()),W2CT(SelChat.GetDescription()),NULL,NULL,SelChat.GetId());
		m_AllClosedWindow.Add(pNewChatDlg->GetSafeHwnd());
		// Load UserCollections to  CChatCreateDlg [8/8/2002]
		
		pNewChatDlg->ShowWindow(SW_SHOW);
	}
}

void CMainDlg::OnChatAddaFriends()
{
	USES_CONVERSION;
	CChat SelChat;
	if(CurrChatTID!=-1&&FindChatByTID(CurrChatTID,SelChat))
	{
		CChatCreateDlg	*pNewChatDlg	=	new CChatCreateDlg(this);
		
		CUserCollection	NewUsers;
		
		CUserCollection& ChatUsers  = SelChat.GetUsers();
		
		CUserCollection	ContactList;
		GetCopyContactList(ContactList);
		
		POSITION pos =  ContactList.InitIteration();
		
		CUser	*pUser = NULL;
		
		while(ContactList.GetNext(pos,pUser))
		{
			POSITION posFind = ChatUsers.InitIteration();
			
			CUser *pFindUser = NULL;
			
			BOOL bIsPresent = FALSE;
			
			while(ChatUsers.GetNext(posFind,pFindUser))
			{
				if(pFindUser->GetGlobalID()==pUser->GetGlobalID())
				{
					bIsPresent	=	TRUE;
					break;
				}
			}
			
			if(!bIsPresent)
			{
				NewUsers.SetAt(*pUser);
			}
		}
		
		pNewChatDlg->Create(CChatCreateDlg::CCDM_INVITE,W2CT(SelChat.GetName()),W2CT(SelChat.GetDescription()),NULL,&NewUsers,SelChat.GetId());
		m_AllClosedWindow.Add(pNewChatDlg->GetSafeHwnd());
		// Load UserCollections to  CChatCreateDlg [8/8/2002]
		
		pNewChatDlg->ShowWindow(SW_SHOW);
	}
}

void CMainDlg::OnChatStatusInactive()
{
	USES_CONVERSION;
	CChat SelChat;
	if(CurrChatTID!=-1&&FindChatByTID(CurrChatTID,SelChat))
	{
		long Handle = 0;
		theNet2.LockTranslator();
		try
		{
			SelChat->SetStatus(SC_INACTIVE,0,&Handle);
			if(Handle)
			{
				theNet2.AddToTranslator(Handle,GetSafeHwnd());
				m_ChatHandleMap.SetAt(Handle,W2CT(SelChat.GetId()));
			}
		}
		catch (_com_error&) 
		{
		}
		theNet2.UnlockTranslator();
	}
}

void CMainDlg::OnChatStatusActive()
{
	USES_CONVERSION;
	CChat SelChat;
	if(CurrChatTID!=-1&&FindChatByTID(CurrChatTID,SelChat))
	{
		SetChatStatus(SelChat.GetId(),SC_ACTIVE);
		/*
		long Handle = 0;
		theNet2.LockTranslator();
		try
		{
		SelChat->SetStatus(SC_ACTIVE,GetOptionInt(IDS_OFSMESSENGER,IDS_CHATLOADMESS,20),&Handle);
		if(Handle)
		{
		theNet2.AddToTranslator(Handle,GetSafeHwnd());
		m_ChatHandleMap.SetAt(Handle,W2CT(SelChat.GetId()));
		}
		}
		catch (_com_error &e) 
		{
		}
		theNet2.UnlockTranslator();
		*/
	}
}

void CMainDlg::OnChatAttach()
{
	CChat SelChat;
	if(CurrChatTID!=-1&&FindChatByTID(CurrChatTID,SelChat))
	{
	}
}

void CMainDlg::OnChatSendMessage()
{
	CChat SelChat;
	if(CurrChatTID!=-1&&FindChatByTID(CurrChatTID,SelChat))
	{
		ShowChatDlg(&SelChat,FALSE);
		
		/*		CMcWindowAgent winAgent(SelChat.GetChatWindow());
		
		  if(winAgent.IsValid())
		  {
		  winAgent.ShowWindow(SW_SHOWNORMAL);
		  winAgent.SetForegroundWindow();
		  }
		  else
		  {
		  CChatDlg *pChatDlg = new CChatDlg(this);
		  pChatDlg->Create(GetDesktopWindow());
		  
			// Remember Show  [8/16/2002]
			for(int iChatIndex = 0;iChatIndex<m_ChatCollection.GetSize();iChatIndex++)
			{
			CChat& tmpChat =  m_ChatCollection.ElementAt(iChatIndex);
			
			  if(tmpChat.GetId() == SelChat.GetId())
			  {	
			  tmpChat.SetChatWindow(pChatDlg->GetSafeHwnd());
			  break;
			  }
			  }
			  
				m_AllClosedWindow.Add(pChatDlg->GetSafeHwnd());
				// Remember Show  [8/16/2002]
				
				  pChatDlg->SetChat(SelChat);
				  pChatDlg->Refresh();
				  
					pChatDlg->ShowWindow(SW_SHOWNORMAL);
					pChatDlg->SetForegroundWindow();	
					
					  }
		*/
	}
}

void CMainDlg::OnUpdateChatViewDetails(CCmdUI* pCmdUI)
{
	BOOL bPresent = FALSE;
	if(CurrChatTID!=-1)
	{
		bPresent =	TRUE;
	}
	pCmdUI->Enable(bPresent);
}

void CMainDlg::OnUpdateChatMessageHistory(CCmdUI* pCmdUI)
{
	BOOL bPresent = FALSE;
	if(CurrChatTID!=-1)
	{
		bPresent =	TRUE;
	}
	pCmdUI->Enable(bPresent);
}

void CMainDlg::OnUpdateChatLeave(CCmdUI* pCmdUI)
{
	BOOL bPresent = FALSE;
	if(CurrChatTID!=-1)
	{
		bPresent =	TRUE;
	}
	pCmdUI->Enable(bPresent);
}

void CMainDlg::OnUpdateChatEditDetails(CCmdUI* pCmdUI)
{
	BOOL bPresent = FALSE;
	CChat SelChat;
	
	if(CurrChatTID!=-1&&FindChatByTID(CurrChatTID,SelChat))
	{
		if(SelChat.GetOwnerId()==m_User.GetGlobalID())
		{
			bPresent	=	TRUE;
		}
	}
	
	pCmdUI->Enable(bPresent);
}

void CMainDlg::OnUpdateChatAddaFriends(CCmdUI* pCmdUI)
{
	OnUpdateChatSendMessage(pCmdUI);
}

void CMainDlg::OnUpdateChatStatusInactive(CCmdUI* pCmdUI)
{
	BOOL bPresent = FALSE;
	CChat SelChat;
	
	if(CurrChatTID!=-1&&FindChatByTID(CurrChatTID,SelChat))
	{
		if(SelChat.GetStatus()==SC_INACTIVE)
		{
			bPresent	=	TRUE;
		}
		pCmdUI->Enable(TRUE);
	}
	else
		pCmdUI->Enable(FALSE);
	
	pCmdUI->SetCheck(bPresent);
}

void CMainDlg::OnUpdateChatStatusActive(CCmdUI* pCmdUI)
{
	BOOL bPresent = FALSE;
	CChat SelChat;
	
	if(CurrChatTID!=-1&&FindChatByTID(CurrChatTID,SelChat))
	{
		if(SelChat.GetStatus()==SC_ACTIVE)
		{
			bPresent	=	TRUE;
		}
		pCmdUI->Enable(TRUE);
	}
	else
		pCmdUI->Enable(FALSE);
	
	pCmdUI->SetCheck(bPresent);
}

void CMainDlg::OnUpdateChatAttach(CCmdUI* pCmdUI)
{
	BOOL bPresent = FALSE;
	CChat SelChat;
	
	if(CurrChatTID!=-1&&FindChatByTID(CurrChatTID,SelChat))
	{
		if(SelChat.GetStatus()==SC_ACTIVE)
		{
			// TODO: Not Supported [9/7/2002]
			//bPresent	=	TRUE;
		}
	}
	
	pCmdUI->Enable(bPresent);
}

void CMainDlg::OnUpdateChatSendMessage(CCmdUI* pCmdUI)
{
	BOOL bPresent = FALSE;
	CChat SelChat;
	
	if(CurrChatTID!=-1&&FindChatByTID(CurrChatTID,SelChat))
	{
		if(SelChat.GetStatus()==SC_ACTIVE)
		{
			bPresent	=	TRUE;
		}
	}
	
	pCmdUI->Enable(bPresent);
}

BOOL CMainDlg::FindChatByTID(long TID, CChat &OutChat)
{
	for(int iChatIndex = 0;iChatIndex<m_ChatCollection.GetSize();iChatIndex++)
	{
		CChat tmpChat =  m_ChatCollection.GetAt(iChatIndex);
		if(tmpChat.GetTID()==TID)
		{	
			OutChat = tmpChat;
			return TRUE;
		}
	}
	return FALSE;
}

BOOL CMainDlg::FindChatByGlobalId(BSTR bsId, CChat &OutChat)
{
	for(int iChatIndex = 0;iChatIndex<m_ChatCollection.GetSize();iChatIndex++)
	{
		CChat tmpChat =  m_ChatCollection.GetAt(iChatIndex);
		CComBSTR bsChatId = tmpChat.GetId();
		
		if(bsChatId == bsId)
		{	
			OutChat = tmpChat;
			return TRUE;
		}
	}
	
	return FALSE;
}

void CMainDlg::OnOptionsConferenceCreateNew()
{
	CChatCreateDlg	*pNewChatDlg	=	new CChatCreateDlg(this);
	
	CUserCollection	ContactList;
	
	GetCopyContactList(ContactList);
	
	pNewChatDlg->Create(CChatCreateDlg::CCDM_CREATE,NULL,NULL,NULL,&ContactList);
	m_AllClosedWindow.Add(pNewChatDlg->GetSafeHwnd());
	// Load UserCollections to  CChatCreateDlg [8/8/2002]
	
	pNewChatDlg->ShowWindow(SW_SHOW);
}

void CMainDlg::OnUpdateOptionsConferenceCreateNew(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(ConnectEnable(FALSE));
}

BOOL CMainDlg::AddNewChat(IChat *pChat, LONG Status)
{
	CChat	newChat(pChat),tmpChat;
	
	newChat.SetStatus(Status);
	
	// Step 1. Check if chat is present [10/1/2002]
	for(int iChatIndex = 0;iChatIndex<m_ChatCollection.GetSize();iChatIndex++)
	{
		CChat& tmpChat =  m_ChatCollection.ElementAt(iChatIndex);
		
		//CComBSTR bsId = tmpChat.GetId();
		//CString str1 = tmpChat.GetShowName();
		//CString str2 = newChat.GetShowName();
		
		if(tmpChat.GetId() == newChat.GetId())
		{	
			newChat.SetStatus(tmpChat.GetStatus());
			tmpChat.SetInterface(pChat);
			
			BuildChatsList();
			
			return TRUE;
		}
	}
	
	if(Status==SC_ACTIVE)
	{
		CUser newUser = m_User;
		newUser.m_iStatus = SC_ACTIVE;
		
		DWORD dwColor = 0;
		
		if(GetColorFromColorStorage(GetUserRole(),GetUserID(),newUser.GetGlobalID(),dwColor))
		{
			SetItemToColorStorage(GetUserRole(),GetUserID(),newUser.GetGlobalID(),newUser.GetShowName(),dwColor);
		}
		else
		{
			// Generate New Color [8/28/2002]
			//dwColor = RGB(0xFF,0,0);
			dwColor = RGB((rand()%32)*8,(rand()%32)*8,(rand()%32)*8);
			
			SetItemToColorStorage(GetUserRole(),GetUserID(),newUser.GetGlobalID(),newUser.GetShowName(),dwColor);
		}
		
		newChat.GetUsers().SetAt(newUser);
		
		m_ActiveChatRem.Add(newChat.GetId().Copy());
	}
	
	m_ChatCollection.Add(newChat);
	
	BuildChatsList();
	
	if(m_ChatCollection.GetSize()==1)
	{
		// Your First Chat [10/1/2002]
		CComPtr<IHTMLDocument>	pDoc;
		pDoc.Attach((IHTMLDocument*)GetInWindowDocument());
		if(pDoc)
		{
			CComQIPtr<IHTMLDocument2>	pDoc2 = pDoc;
			if(pDoc2)
			{
				CComBSTR	bsURL;
				pDoc2->get_URL(&bsURL);				
				if(bsURL==CComBSTR((BSTR)(bstr_t(IBN_SCHEMA) + bstr_t((LPCTSTR)GetProductLanguage()) + bstr_t(L"/Shell/Conference/blank.html"))))
				{
					for(int iChatIndex = 0; iChatIndex<m_AppArray.GetSize();iChatIndex++)
					{
						if(m_AppArray[iChatIndex].Type==APPT_CHAT_CONTACTLIST)
						{
							m_AppBar.m_AppCtrl.SetCheckButton(iChatIndex);
							
							if(m_ChatCollection.GetSize()==0)
							{
								m_chatbox.ShowWindow(SW_HIDE);
								m_treebox.ShowWindow(SW_HIDE);
								//m_WebFolderView.ShowWindow(SW_HIDE);
								m_InWindow.ShowWindow(SW_SHOW);
								m_InWindow.Navigate(CString(IBN_SCHEMA) +GetProductLanguage() + CString(_T("/Shell/Conference/blank.html")));
							}
							else
							{
								m_chatbox.ShowWindow(SW_SHOW);
								m_treebox.ShowWindow(SW_HIDE);
								//m_WebFolderView.ShowWindow(SW_HIDE);
								m_InWindow.ShowWindow(SW_HIDE);
								m_InWindow.Navigate(CString(IBN_SCHEMA) + GetProductLanguage() + CString(_T("/Common/blank.html")));
							}
							Invalidate();
							break;
						}
					}
					
				}
			}
		}
	}
	
	return TRUE;
}

void CMainDlg::DeleteChat(BSTR bsChatId)
{
	for(int iChatIndex = 0;iChatIndex<m_ChatCollection.GetSize();iChatIndex++)
	{
		CChat& tmpChat =  m_ChatCollection.ElementAt(iChatIndex);
		CComBSTR bsId = tmpChat.GetId();
		
		if(bsId == bsChatId)
		{	
			HWND hChatDlg = tmpChat.GetChatWindow();
			
			if(IsWindow(hChatDlg))
			{
				::PostMessage(hChatDlg,WM_CLOSE,0,0);
			}
			
			m_ChatCollection.RemoveAt(iChatIndex);
			
			BuildChatsList();
			
			break;
		}
	}
	
	if((m_ChatCollection.GetSize()==0)&&(m_chatbox.GetStyle()&WS_VISIBLE))
	{
		m_chatbox.ShowWindow(SW_HIDE);
		m_treebox.ShowWindow(SW_HIDE);
		//m_WebFolderView.ShowWindow(SW_HIDE);
		m_InWindow.ShowWindow(SW_SHOW);
		m_InWindow.Navigate(CString(IBN_SCHEMA) + GetProductLanguage() + CString(_T("/Shell/Conference/blank.html")));
	}
}

void CMainDlg::UpdateUserChatStatus(IChat *pChat, IUser *pUser)
{
	CChat Chat(pChat);
	CUser User(pUser);
	
	for(int iChatIndex = 0;iChatIndex<m_ChatCollection.GetSize();iChatIndex++)
	{
		CChat& tmpChat =  m_ChatCollection.ElementAt(iChatIndex);
		
		if(tmpChat.GetId() == Chat.GetId())
		{	
			CUser *pUser = tmpChat.GetUsers().GetAt(User.GetGlobalID());
			
			if(pUser)
			{
				if(pUser->m_dwStatusTime>=User.m_dwStatusTime)
					return;
				
				DWORD dwColor = 0;
				
				if(!GetColorFromColorStorage(GetUserRole(),GetUserID(),pUser->GetGlobalID(),dwColor))
				{
					// Generate New Color [8/28/2002]
					//dwColor = RGB(0xFF,0,0);
					dwColor = RGB((rand()%32)*8,(rand()%32)*8,(rand()%32)*8);
					
					SetItemToColorStorage(GetUserRole(),GetUserID(),pUser->GetGlobalID(),pUser->GetShowName(),dwColor);
				}
				
				
				tmpChat.GetUsers().SetAt(User);
				// If presentr Chat Dlg Update it ContactList ... [8/16/2002]
				
				HWND hWnd = tmpChat.GetChatWindow();
				
				if(IsWindow(hWnd))
				{
					CMcWindowAgent winAgent(hWnd);
					winAgent.Refresh();
				}
			}
			
			break;
		}
	}
}

void CMainDlg::NewChatMessage(IChat *pChat, IMessage *pMessage)
{
	USES_CONVERSION;
	
	CChat		Chat(pChat);
	CMessage	Message(pMessage);
	
	for(int iChatIndex = 0;iChatIndex<m_ChatCollection.GetSize();iChatIndex++)
	{
		CChat& tmpChat =  m_ChatCollection.ElementAt(iChatIndex);
		
		if(tmpChat.GetId() == Chat.GetId())
		{	
			CUser *pSenderUser = tmpChat.GetUsers().GetAt(Message.GetSender().GetGlobalID());
			
			if(pSenderUser)
			{
				CComPtr<IXMLDOMDocument>	pDoc	=	NULL;
				pDoc.CoCreateInstance(CLSID_DOMDocument40,NULL,CLSCTX_INPROC_SERVER);
				
				VARIANT_BOOL	vbLoad;
				pDoc->loadXML(CComBSTR(L"<log><mess></mess></log>"),&vbLoad);
				
				CComPtr<IXMLDOMNode>	pRoot	=	NULL;
				pDoc->selectSingleNode(CComBSTR(L"log/mess"),&pRoot);
				
				if(pRoot)
				{
					WCHAR Buff[100]	=	L"";
					_ltow(Message.GetID(),Buff,10);
					// ?????
					insertSingleNode(pRoot,CComBSTR(L"id"),NULL,CComBSTR(Buff));
					// ?????
					insertSingleNode(pRoot,CComBSTR(L"body"),NULL,(BSTR)Message.GetMessage());
					
					CTime	Time((time_t)Message.GetTime());
					
					SYSTEMTIME		sysTime		=	{0};
					TCHAR			szDate[MAX_PATH]=_T(""), szTime[MAX_PATH]=_T("");
					
					Time.GetAsSystemTime(sysTime);
					
					GetDateFormat(LOCALE_USER_DEFAULT,DATE_SHORTDATE,&sysTime,NULL,szDate,MAX_PATH);
					GetTimeFormat(LOCALE_USER_DEFAULT,NULL,&sysTime,NULL,szTime,MAX_PATH);
					
					CString	strDataFormat;
					strDataFormat.Format(_T("%s %s"),szDate,szTime);
					
					WCHAR	wsBuff[100];
					_ultow((time_t)Message.GetTime(),wsBuff,10);
					
					CComBSTR bsStrTime;
					bsStrTime.Attach(strDataFormat.AllocSysString());
					
					insertSingleNode(pRoot,CComBSTR(L"show_time"),NULL,bsStrTime);
					
					insertSingleNode(pRoot,CComBSTR(L"time"),NULL,CComBSTR(wsBuff));
					
					CComPtr<IXMLDOMNode>	pNodeUser	=	NULL;
					insertSingleNode(pRoot,CComBSTR(L"user"),NULL,NULL,&pNodeUser);
					
					if(pNodeUser)
					{
						WCHAR wsBuf[100];
						_ltow(pSenderUser->GlobalID,wsBuf,10);
						
						CComVariant	varId	=	(BSTR)wsBuf;
						
						SetAttribute(pNodeUser,CComBSTR(L"id"),varId);
						//insertSingleNode(pNodeUser,CComBSTR(L"id"),NULL,CComBSTR(wsBuf));
						insertSingleNode(pNodeUser,CComBSTR(L"first_name"),NULL,T2W(const_cast<LPTSTR>((LPCTSTR)pSenderUser->m_strFirstName)));
						insertSingleNode(pNodeUser,CComBSTR(L"last_name"),NULL,T2W(const_cast<LPTSTR>((LPCTSTR)pSenderUser->m_strLastName)));
					}
					
					DWORD dwColor =	0;
					if(GetColorFromColorStorage(GetUserRole(),GetUserID(),pSenderUser->GlobalID,dwColor))
					{
						//dwColor	=	RGB(GetBValue(dwColor),GetGValue(dwColor),GetRValue(dwColor));
						
						WCHAR wsBuff[100];
						//_ultow(dwColor,wsBuff,16);
						swprintf(wsBuff,L"%02X%02X%02X",GetRValue(dwColor),GetGValue(dwColor),GetBValue(dwColor));
						
						insertSingleNode(pRoot,CComBSTR(L"color"),NULL,CComBSTR(wsBuff));
					}
					else
					{
						ASSERT(FALSE);
					}
					
				}
				
				CComBSTR bsMessage;
				
				pDoc->get_xml(&bsMessage);
				
				tmpChat.AddNewMessages(bsMessage);
				
				// If presentr Chat Dlg Update it ContactList ... [8/16/2002]
				ShowChatDlg(&tmpChat,TRUE);
				
				/*HWND hWnd = tmpChat.GetChatWindow();
				
				  if(IsWindow(hWnd))
				  {
				  CMcWindowAgent winAgent(hWnd);
				  
					if(::GetForegroundWindow()!=hWnd)
					{
					::FlashWindow(hWnd,TRUE);
					}
					
					  winAgent.Refresh();
					  }
					  else
					  {
					  // Open Chat Dialog ... 
					  CChatDlg *pChatDlg = new CChatDlg(this);
					  pChatDlg->Create(GetDesktopWindow(),TRUE);
					  
						// Remember Show  [8/16/2002]
						tmpChat.SetChatWindow(pChatDlg->GetSafeHwnd());
						m_AllClosedWindow.Add(pChatDlg->GetSafeHwnd());
						// Remember Show  [8/16/2002]
						
						  pChatDlg->SetChat(tmpChat);
						  pChatDlg->Refresh();
						  
							pChatDlg->ShowWindow(SW_SHOWMINNOACTIVE);
							//pChatDlg->SetForegroundWindow();
							
							  ::FlashWindow(pChatDlg->GetSafeHwnd(),TRUE);
				}*/
				break;
			}
		}
	}
}

void CMainDlg::InviteNewUserInChat(IChat *pChat, IUser *pUser, IUser *pInvitedUser, BSTR bsInviteMessage)
{
	USES_CONVERSION;
	
	CChat Chat(pChat);
	
	CUser User(pUser);
	
	CUser InvitedUser(pInvitedUser);
	
	if(InvitedUser.GetGlobalID()==m_User.GetGlobalID())
	{
		HWND hInviteWnd	=	NULL;
		m_InviteChatWndMap.Lookup(W2CT(Chat.GetId()),(LPVOID&)hInviteWnd);
		
		if(!IsWindow(hInviteWnd))
		{
			// Step 1a. If Invite me, Show Invite Dialog, and All.
			CInviteChatDlg *pInviteChatDlg	=	new CInviteChatDlg(this);
			pInviteChatDlg->Create(CInviteChatDlg::IDD,GetDesktopWindow());
			
			CUser *fullInfoUser = FindUserInVisualContactListByGlobalId(User.GetGlobalID());
			if(fullInfoUser!=NULL)
				pInviteChatDlg->SetInfo(Chat,*fullInfoUser,bsInviteMessage);
			else
				pInviteChatDlg->SetInfo(Chat,User,bsInviteMessage);
			
			pInviteChatDlg->ShowWindow(SW_SHOWNORMAL);
			pInviteChatDlg->SetForegroundWindow();
			
			m_AllClosedWindow.Add(pInviteChatDlg->GetSafeHwnd());
			
			m_InviteChatWndMap.SetAt(W2CT(Chat.GetId()),(LPVOID)pInviteChatDlg->GetSafeHwnd());
		}
	}
	else
	{
		// Step 1b. Else Add Information Message to the Chat XML.
		for(int iChatIndex = 0;iChatIndex<m_ChatCollection.GetSize();iChatIndex++)
		{
			CChat& tmpChat =  m_ChatCollection.ElementAt(iChatIndex);
			
			if(tmpChat.GetId() == Chat.GetId())
			{	
				CUser *pUser		= tmpChat.GetUsers().GetAt(User.GetGlobalID());
				
				//CUser *pInvitedUser = tmpChat.GetUsers().GetAt(InvitedUser.GetGlobalID());
				if(pUser)
				{
					CComBSTR bsFirtsName, bsLastName;
					
					bsFirtsName.Attach(pUser->m_strFirstName.AllocSysString());
					bsLastName.Attach(pUser->m_strLastName.AllocSysString());
					
					CString strFormat;
					strFormat.Format(_T("%s (%s)"),GetString(IDS_INVITED_MESSAGE),InvitedUser.GetShowName());
					
					CComBSTR bsInviteMessage;
					bsInviteMessage.Attach(strFormat.AllocSysString());
					
					DWORD	dwColor  = RGB(0,0,0xFF);
					GetColorFromColorStorage(GetUserRole(),GetUserID(),-IDS_INVITED_MESSAGE,dwColor);
					
					tmpChat.AddNewEvent(-IDS_INVITED_MESSAGE,bsFirtsName, bsLastName,bsInviteMessage,time(NULL),dwColor);
					
					// Step 2b. Show or Refresh Chat dialog.
					ShowChatDlg(&tmpChat,TRUE);
					
					/*HWND hWnd = tmpChat.GetChatWindow();
					
					  if(IsWindow(hWnd))
					  {
					  CMcWindowAgent winAgent(hWnd);
					  
						if(::GetForegroundWindow()!=hWnd)
						{
						::FlashWindow(hWnd,TRUE);
						}
						
						  winAgent.Refresh();
						  }
						  else
						  {
						  // Open Chat Dialog ... 
						  CChatDlg *pChatDlg = new CChatDlg(this);
						  pChatDlg->Create(GetDesktopWindow(),TRUE);
						  
							// Remember Show  [8/16/2002]
							tmpChat.SetChatWindow(pChatDlg->GetSafeHwnd());
							m_AllClosedWindow.Add(pChatDlg->GetSafeHwnd());
							// Remember Show  [8/16/2002]
							
							  pChatDlg->SetChat(tmpChat);
							  pChatDlg->Refresh();
							  
								pChatDlg->ShowWindow(SW_SHOWMINNOACTIVE);
								//pChatDlg->SetForegroundWindow();
								
								  ::FlashWindow(pChatDlg->GetSafeHwnd(),TRUE);
				}*/
				}
				
				break;
			}
		}
	}
}

void CMainDlg::LeaveUserFromChat(IChat *pChat, IUser *pUser)
{
	CChat Chat(pChat);
	
	CUser User(pUser);
	
	// Step 1b. Else Add Information Message to the Chat XML.
	for(int iChatIndex = 0;iChatIndex<m_ChatCollection.GetSize();iChatIndex++)
	{
		CChat& tmpChat =  m_ChatCollection.ElementAt(iChatIndex);
		
		if(tmpChat.GetId() == Chat.GetId())
		{	
			CUser *pUser		= tmpChat.GetUsers().GetAt(User.GetGlobalID());
			if(pUser)
			{
				CComBSTR bsFirtsName, bsLastName;
				
				bsFirtsName.Attach(pUser->m_strFirstName.AllocSysString());
				bsLastName.Attach(pUser->m_strLastName.AllocSysString());
				
				CComBSTR bsInviteMessage;
				bsInviteMessage.Attach(GetString(IDS_LEAVED_MESSAGE).AllocSysString());
				
				DWORD	dwColor  = RGB(0,0,0xFF);
				GetColorFromColorStorage(GetUserRole(),GetUserID(),-IDS_LEAVED_MESSAGE,dwColor);
				
				tmpChat.AddNewEvent(-IDS_LEAVED_MESSAGE,bsFirtsName, bsLastName,bsInviteMessage,time(NULL),dwColor);
				
				tmpChat.GetUsers().Delete(User);
				
				// Step 2b. Show or Refresh Chat dialog.
				ShowChatDlg(&tmpChat,TRUE);
				/*HWND hWnd = tmpChat.GetChatWindow();
				
				  if(IsWindow(hWnd))
				  {
				  CMcWindowAgent winAgent(hWnd);
				  
					if(::GetForegroundWindow()!=hWnd)
					{
					::FlashWindow(hWnd,TRUE);
					}
					
					  winAgent.Refresh();
					  }
					  else
					  {
					  // Open Chat Dialog ... 
					  CChatDlg *pChatDlg = new CChatDlg(this);
					  pChatDlg->Create(GetDesktopWindow(),TRUE);
					  
						// Remember Show  [8/16/2002]
						tmpChat.SetChatWindow(pChatDlg->GetSafeHwnd());
						m_AllClosedWindow.Add(pChatDlg->GetSafeHwnd());
						// Remember Show  [8/16/2002]
						
						  pChatDlg->SetChat(tmpChat);
						  pChatDlg->Refresh();
						  
							pChatDlg->ShowWindow(SW_SHOWMINNOACTIVE);
							//pChatDlg->SetForegroundWindow();
							
							  ::FlashWindow(pChatDlg->GetSafeHwnd(),TRUE);
			}*/
			}
			
			break;
		}
	}
}

void CMainDlg::ChatAccept(IChat *pChat, IUser *pUser, long Result)
{
	CChat Chat(pChat);
	
	CUser User(pUser);
	
	// Step 1b. Else Add Information Message to the Chat XML.
	for(int iChatIndex = 0;iChatIndex<m_ChatCollection.GetSize();iChatIndex++)
	{
		CChat& tmpChat =  m_ChatCollection.ElementAt(iChatIndex);
		
		if(tmpChat.GetId() == Chat.GetId())
		{	
			CComBSTR bsFirtsName, bsLastName;
			
			bsFirtsName.Attach(User.m_strFirstName.AllocSysString());
			bsLastName.Attach(User.m_strLastName.AllocSysString());
			
			CComBSTR bsAcceptMessage;
			
			if(Result==1)
			{
				DWORD dwColor = 0;
				
				if(GetColorFromColorStorage(GetUserRole(),GetUserID(),User.GetGlobalID(),dwColor))
				{
					SetItemToColorStorage(GetUserRole(),GetUserID(),User.GetGlobalID(),User.GetShowName(),dwColor);
				}
				else
				{
					// Generate New Color [8/28/2002]
					//dwColor = RGB(0xFF,0,0);
					dwColor = RGB((rand()%32)*8,(rand()%32)*8,(rand()%32)*8);
					
					SetItemToColorStorage(GetUserRole(),GetUserID(),User.GetGlobalID(),User.GetShowName(),dwColor);
				}
				
				tmpChat.GetUsers().SetAt(User);
				
				bsAcceptMessage.Attach(GetString(IDS_ACCEPT_MESSAGE).AllocSysString());
			}
			else
				bsAcceptMessage.Attach(GetString(IDS_DENY_MESSAGE).AllocSysString());
			
			DWORD	dwColor  = RGB(0,0,0xFF);
			GetColorFromColorStorage(GetUserRole(),GetUserID(),((Result==1)?-IDS_ACCEPT_MESSAGE:-IDS_DENY_MESSAGE),dwColor);
			
			tmpChat.AddNewEvent(((Result==1)?-IDS_ACCEPT_MESSAGE:-IDS_DENY_MESSAGE),bsFirtsName, bsLastName,bsAcceptMessage,time(NULL),dwColor);
			
			// Step 2b. Show or Refresh Chat dialog.
			ShowChatDlg(&tmpChat,TRUE);
			
			/*HWND hWnd = tmpChat.GetChatWindow();
			
			  if(IsWindow(hWnd))
			  {
			  CMcWindowAgent winAgent(hWnd);
			  
				if(::GetForegroundWindow()!=hWnd)
				{
				::FlashWindow(hWnd,TRUE);
				}
				
				  winAgent.Refresh();
				  }
				  else
				  {
				  // Open Chat Dialog ... 
				  CChatDlg *pChatDlg = new CChatDlg(this);
				  pChatDlg->Create(GetDesktopWindow(),TRUE);
				  
					// Remember Show  [8/16/2002]
					tmpChat.SetChatWindow(pChatDlg->GetSafeHwnd());
					m_AllClosedWindow.Add(pChatDlg->GetSafeHwnd());
					// Remember Show  [8/16/2002]
					
					  pChatDlg->SetChat(tmpChat);
					  pChatDlg->Refresh();
					  
						pChatDlg->ShowWindow(SW_SHOWMINNOACTIVE);
						//pChatDlg->SetForegroundWindow();
						
						  ::FlashWindow(pChatDlg->GetSafeHwnd(),TRUE);
		}*/
			break;
		}
	}
}

void CMainDlg::UpdateUserMenu(long GlobalId, CMenu *pMenu)
{
	CUser *pUser = FindUserInVisualContactListByGlobalId(GlobalId);
	if(pUser&&pMenu)
	{
		long tmpCurrTID = CurrTID;
		CurrTID = pUser->TID;
		UpdateMenu(this,pMenu);
		///CurrTID = tmpCurrTID;
	}
}

void CMainDlg::AddToAllCloseWindow(HWND hWnd)
{
	m_AllClosedWindow.Add(hWnd);
}

BOOL CMainDlg::SendLightNewMessage(IMessage *pNewMessage)
{
	CComQIPtr<IMessage>	pLightMessage(pNewMessage);
	
	if(!pLightMessage)
		return FALSE;
	// Step 1. Add message to DB [9/5/2002]
	if(LocalDataBaseEnable())
	{
		try
		{
			IUsersPtr	pRecipients;
			
			pLightMessage->get_Recipients(&pRecipients);
			
			for(long lUserIndex = 1; lUserIndex<=pRecipients->GetCount();lUserIndex++)
			{
				IUserPtr pRecipient = pRecipients->GetItem(lUserIndex);
				
				LONG lRecipientId =  (LONG)pRecipient->GetValue(L"@id");
				
				try
				{
					// Need set time=0 and SID = NULL [9/5/2002]
					m_LocalHistory->AddMessage(m_User.GetGlobalID(),lRecipientId,bstr_t(), pLightMessage->GetMID(),NULL, TRUE,pLightMessage->GetBody(),VARIANT_TRUE);
				}
				catch(_com_error&)
				{
					///ASSERT(FALSE);
				}
				
			}
		}
		catch(_com_error&)
		{
			ASSERT(FALSE);
		}
	}
	
	// Step 2. Send Message [9/5/2002]
	if(ConnectEnable(FALSE))
	{
		theNet2.LockTranslator();
		try
		{
			long lHandle = 0L;
			pNewMessage->Send(&lHandle);
			
			if(lHandle)
			{
				// Add New Message to Translator ... [9/5/2002]
				theNet2.AddToTranslator(lHandle,GetSafeHwnd());
				// R [9/5/2002]
				m_SendMessagesMap[lHandle] = pLightMessage.Detach();
			}
		}
		catch(_com_error&)
		{
			theNet2.UnlockTranslator();
			return FALSE;
		}
		theNet2.UnlockTranslator();
	}
	
	return TRUE;
}

BOOL CMainDlg::LoadNotSendedMessage()
{
	if(!LocalDataBaseEnable()) return FALSE;
	
	HRESULT hr = S_OK;
	try
	{
		CComBSTR bstrUnreadMessage;
		
		hr = m_LocalHistory->GetUnSentMessages(&bstrUnreadMessage);
		
		if(bstrUnreadMessage.Length())
		{
			/// Add to New Messages ...
			CComPtr<IXMLDOMDocument> m_doc = NULL;
			hr = m_doc.CoCreateInstance(CLSID_DOMDocument40, NULL, CLSCTX_INPROC_SERVER);
			if(SUCCEEDED(hr))
			{
				VARIANT_BOOL vBool = VARIANT_FALSE;
				m_doc->loadXML(bstrUnreadMessage,&vBool);
				
				if (vBool == VARIANT_TRUE)
				{
					//// Распаковка XML ...
					/*<Messages>
					<Message mid="">
					<from_id>
					<to_id>
					<body>
					<time>
					....
					*/
					CComPtr<IXMLDOMNodeList> m_List = NULL;
					m_doc->selectNodes(bstr_t("Messages/Message"),&m_List);
					if(m_List!=NULL)
					{
						USES_CONVERSION;
						
						long lSize = 0;
						m_List->get_length(&lSize);
						
						for(long i=0;i<lSize;i++)
						{
							CComBSTR bToID, bBody, bTime, bMID;
							long lToId;
							//							long lTime;
							
							try
							{
								CComPtr<IXMLDOMElement>  m_Element = NULL;
								CComPtr<IXMLDOMNode> pNode = NULL;
								m_List->get_item(i,&pNode);
								//// Получаем Строки
								pNode->QueryInterface(IID_IXMLDOMElement,(void**)&m_Element);
								CComVariant	varMID;
								
								m_Element->getAttribute(bstr_t("mid"),&varMID);
								bMID = varMID.bstrVal;
								
								//// Распковка Новой Мессаги ...
								GetTextByPath(pNode,bstr_t("to_id"),&bToID);
								GetTextByPath(pNode,bstr_t("body"),&bBody);
								GetTextByPath(pNode,bstr_t("time"),&bTime);
								
								lToId = _wtol((BSTR)bToID);
								
								// Create IMessage Interface ...
								IMessagePtr pMessage = pSession->CreateMessage();
								
								pMessage->PutMID((BSTR)bMID);
								pMessage->PutBody((BSTR)bBody);
								
								IUsersPtr pResepients = pMessage->GetRecipients();
								IUserPtr  pUser = pResepients->AddUser();
								pUser->PutValue("@id",lToId);
								
								// Send Message [9/5/2002]
								SendLightNewMessage(pMessage);
							}
							catch(_com_error&)
							{
								ASSERT(FALSE);
							}
						}
					}
				}
			}
		}
	}
	catch(_com_error&)
	{
		ASSERT(FALSE);
	}
	return TRUE;
}

BOOL CMainDlg::SetChatStatus(BSTR bsChatId, LONG NewStatus, LONG Command)
{
	USES_CONVERSION;
	CChat SelChat;
	if(bsChatId&&FindChatByGlobalId(bsChatId,SelChat))
	{
		long Handle = 0;
		theNet2.LockTranslator();
		try
		{
			SelChat->SetStatus(NewStatus,GetOptionInt(IDS_OFSMESSENGER,IDS_CHATLOADMESS,20),&Handle);
			if(Handle)
			{
				theNet2.AddToTranslator(Handle,GetSafeHwnd());
				m_ChatHandleMap.SetAt(Handle,W2CT(SelChat.GetId()));
				
				if(Command)
				{
					m_HandleCommandMap.SetAt(Handle,Command);
				}
			}
		}
		catch (_com_error&) 
		{
		}
		theNet2.UnlockTranslator();
	}
	return TRUE;
}

BOOL CMainDlg::ShowChatDlg(BSTR bsChatId, BOOL bMinMode)
{
	for(int iChatIndex = 0;iChatIndex<m_ChatCollection.GetSize();iChatIndex++)
	{
		CChat& tmpChat =  m_ChatCollection.ElementAt(iChatIndex);
		
		if(tmpChat.GetId() == bsChatId)
		{	
			return ShowChatDlg(&tmpChat,bMinMode);
		}
	}
				
	return FALSE;
}

BOOL CMainDlg::ShowChatDlg(CChat *pChat, BOOL bMinMode)
{
	if(IsBadReadPtr(pChat,sizeof(CChat)))
		return FALSE;
	
	// Step 2b. Show or Refresh Chat dialog.
	HWND hWnd = pChat->GetChatWindow();
	
	if(IsWindow(hWnd))
	{
		CMcWindowAgent winAgent(hWnd);
		
		if(::GetForegroundWindow()!=hWnd)
		{
			if(!bMinMode)
			{
				winAgent.ShowWindow(SW_SHOWNORMAL);
				winAgent.SetForegroundWindow();
			}
			//::FlashWindow(hWnd,TRUE);

			FLASHWINFO flashInfo = {0};
			flashInfo.cbSize = sizeof(FLASHWINFO);
			flashInfo.hwnd = hWnd;
			flashInfo.dwFlags = FLASHW_ALL;
			flashInfo.uCount  = 5;
			flashInfo.dwTimeout   = 500;

			::FlashWindowEx(&flashInfo);
		}
		
		winAgent.Refresh();
	}
	else
	{
		// Open Chat Dialog ... 
		CChatDlg *pChatDlg = new CChatDlg(this);
		pChatDlg->Create(GetDesktopWindow(),bMinMode?TRUE:FALSE);
		
		// Remember Show  [8/16/2002]
		pChat->SetChatWindow(pChatDlg->GetSafeHwnd());
		m_AllClosedWindow.Add(pChatDlg->GetSafeHwnd());
		// Remember Show  [8/16/2002]
		
		pChatDlg->SetChat(*pChat);
		pChatDlg->Refresh();
		
		if(bMinMode)
		{
			pChatDlg->ShowWindow(SW_SHOWMINNOACTIVE);
		}
		else
		{
			pChatDlg->ShowWindow(SW_SHOWNORMAL);
			pChatDlg->SetForegroundWindow();
		}
		
		//::FlashWindow(pChatDlg->GetSafeHwnd(),TRUE);
		FLASHWINFO flashInfo = {0};
		flashInfo.cbSize = sizeof(FLASHWINFO);
		flashInfo.hwnd = pChatDlg->GetSafeHwnd();
		flashInfo.dwFlags = FLASHW_ALL;
		flashInfo.uCount  = 5;
		flashInfo.dwTimeout   = 500;

		::FlashWindowEx(&flashInfo);
	}
	
	return TRUE;
}

CString CMainDlg::GetUserDomain()
{
	CString strFullUserName = m_User.m_strLogin;
	
	int iAtPos = strFullUserName.Find(_T('@'));
	if(iAtPos==-1)
		return _T("");
	
	return strFullUserName.Mid(iAtPos+1);
}


void CMainDlg::OnTreemenuConferenceCreateNew()
{
	CUser *pUser = FindUserInVisualContactList(CurrTID);
	if(pUser)
	{
		CChatCreateDlg	*pNewChatDlg	=	new CChatCreateDlg(this);
		
		CUserCollection	ContactList;
		
		GetCopyContactList(ContactList);
		
		CUser* pUserFromContactList = ContactList.GetAt(pUser->GetGlobalID());
		if(pUserFromContactList)
		{
			pUserFromContactList->m_bHasNewMessages = TRUE;
		}
		
		pNewChatDlg->Create(CChatCreateDlg::CCDM_CREATE,NULL,NULL,NULL,&ContactList);
		m_AllClosedWindow.Add(pNewChatDlg->GetSafeHwnd());
		// Load UserCollections to  CChatCreateDlg [8/8/2002]
		
		pNewChatDlg->ShowWindow(SW_SHOW);
	}
}

void CMainDlg::OnUpdateTreemenuConferenceCreateNew(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(ConnectEnable(FALSE));	
}

void CMainDlg::OnTreegroupConferenceCreateNew()
{
	if(!m_strGroupName.IsEmpty())
	{
		CChatCreateDlg	*pNewChatDlg	=	new CChatCreateDlg(this);
		
		CUserCollection	ContactList;
		
		GetCopyContactList(ContactList);
		
		POSITION pos = ContactList.InitIteration();
		while (pos) 
		{
			CUser *pUserFromContactList;
			ContactList.GetNext(pos,pUserFromContactList);
			if(pUserFromContactList)
			{
				int CLMode  = GetOptionInt(IDS_OFSMESSENGER,IDS_CLMODE,2);
				
				switch(CLMode) 
				{
				case 1:
					{
						if(m_bShowOffline||(pUserFromContactList->m_iStatus!=S_OFFLINE&&pUserFromContactList->m_iStatus!=S_INVISIBLE))
							if(m_strGroupName.CompareNoCase(pUserFromContactList->m_strType)==0)
							{
								pUserFromContactList->m_bHasNewMessages = TRUE;
							}
					}
					break;
				case 2:
					{
						if(pUserFromContactList->GetStatus()!=S_OFFLINE&&pUserFromContactList->GetStatus()!=S_INVISIBLE)
						{
							if(m_strGroupName.CompareNoCase(pUserFromContactList->m_strType)==0)
							{
								pUserFromContactList->m_bHasNewMessages = TRUE;
							}
						}
						else if(m_strGroupName.CompareNoCase(GetString(IDS_OFFLINE))==0)
							pUserFromContactList->m_bHasNewMessages = TRUE;
					}
					break;
				}
			}
		}
		
		pNewChatDlg->Create(CChatCreateDlg::CCDM_CREATE,NULL,NULL,NULL,&ContactList);
		m_AllClosedWindow.Add(pNewChatDlg->GetSafeHwnd());
		// Load UserCollections to  CChatCreateDlg [8/8/2002]
		
		pNewChatDlg->ShowWindow(SW_SHOW);
		
	}
}

void CMainDlg::OnUpdateTreegroupConferenceCreateNew(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(ConnectEnable(FALSE));
}

BOOL CMainDlg::LoadConferenceListForCurrUser(CMenu *pInviteToMenu)
{
	if(IsBadReadPtr(pInviteToMenu,sizeof(CMenu)))
		return FALSE;
	
	CUser *pCurrUser = FindUserInVisualContactList(CurrTID);
	
	if(!pCurrUser)
		return FALSE;
	
	for(int iChatItem = 0;iChatItem<m_ChatCollection.GetSize();iChatItem++)
	{
		CChat Chat	=	m_ChatCollection.GetAt(iChatItem);
		
		if(Chat.GetStatus()==SC_ACTIVE)
		{
			CUserCollection& ChatUsers  = Chat.GetUsers();
			
			POSITION pos =  ChatUsers.InitIteration();
			CUser	*pUser = NULL;
			
			BOOL bFlagAdd	=	TRUE;
			
			while(ChatUsers.GetNext(pos,pUser))
			{
				if(pUser->GetGlobalID()==pCurrUser->GetGlobalID())
				{
					bFlagAdd = FALSE;
					break;
				}
			}
			
			if(bFlagAdd)
				pInviteToMenu->AppendMenu(MF_STRING,20200+iChatItem,Chat.GetShowName());
		}
	}
	
	return TRUE;
}

void CMainDlg::OnInviteUserToConference(UINT Id)
{
	int iChatItem = Id - 20200;
	
	if(iChatItem>=0&&iChatItem<m_ChatCollection.GetSize())
	{
		// Add CurrUser To Chat;
		USES_CONVERSION;
		CChat SelChat	=	m_ChatCollection.GetAt(iChatItem);
		
		CChatCreateDlg	*pNewChatDlg	=	new CChatCreateDlg(this);
		
		CUserCollection	NewUsers;
		
		CUserCollection& ChatUsers  = SelChat.GetUsers();
		
		CUserCollection	ContactList;
		
		GetCopyContactList(ContactList);
		
		POSITION pos =  ContactList.InitIteration();
		
		CUser	*pUser = NULL;
		
		while(ContactList.GetNext(pos,pUser))
		{
			POSITION posFind = ChatUsers.InitIteration();
			
			CUser *pFindUser = NULL;
			
			BOOL bIsPresent = FALSE;
			
			while(ChatUsers.GetNext(posFind,pFindUser))
			{
				if(pFindUser->GetGlobalID()==pUser->GetGlobalID())
				{
					bIsPresent	=	TRUE;
					break;
				}
			}
			
			if(!bIsPresent)
			{
				if(pUser->TID==CurrTID)
					pUser->m_bHasNewMessages	=	TRUE;
				
				NewUsers.SetAt(*pUser);
			}
		}
		
		pNewChatDlg->Create(CChatCreateDlg::CCDM_INVITE,W2CT(SelChat.GetName()),W2CT(SelChat.GetDescription()),NULL,&NewUsers,SelChat.GetId());
		
		m_AllClosedWindow.Add(pNewChatDlg->GetSafeHwnd());
		
		pNewChatDlg->ShowWindow(SW_SHOW);
	}
}

void CMainDlg::OnSendGroupMessageTemplate(UINT Id)
{
	int iChatItem = Id - 21300;

	if(iChatItem>=0)
	{
		// Load Message Templates
		CString strSection;
		strSection.Format(_T("%s\\%s\\%d"),GetString(IDS_INFO),GetUserRole(),GetUserID());
		
		CString strMessageTemplateXML = GetRegFileText(strSection,GetString(IDS_MESSAGE_TEMPLATES_REG));
		
		if(strMessageTemplateXML.IsEmpty())
			strMessageTemplateXML = GetString(IDS_DEFAULT_MES_TEMPLATE_XML);
		
		
		CComPtr<IXMLDOMDocument>	pMTDoc	=	NULL;
		pMTDoc.CoCreateInstance(CLSID_DOMDocument40,NULL,CLSCTX_INPROC_SERVER);
		
		if(pMTDoc)
		{
			CComBSTR bsXML;
			bsXML.Attach(strMessageTemplateXML.AllocSysString());
			
			VARIANT_BOOL	varLoad	=	VARIANT_FALSE;
			
			pMTDoc->loadXML(bsXML,&varLoad);
			
			if(varLoad==VARIANT_TRUE)
			{
				CComPtr<IXMLDOMNodeList>	pTemplatesList	=	NULL;
				
				pMTDoc->selectNodes(CComBSTR(L"message_templates/mt"),&pTemplatesList);
				
				if(pTemplatesList!=NULL)
				{
					USES_CONVERSION;
					
					long ListLength	=	0;
					pTemplatesList->get_length(&ListLength);
					
					if(iChatItem<ListLength)
					{
						CComBSTR	bsName, bsText;
						
						CComPtr<IXMLDOMNode>	pStubNode	=	NULL;
						pTemplatesList->get_item(iChatItem,&pStubNode);
						
						GetTextByPath(pStubNode, CComBSTR(L"text"),&bsText);

						//---
						CGroupMessageSendDlg *pGroupDlg = new CGroupMessageSendDlg(this);

						pGroupDlg->Create(GetDesktopWindow());
						pGroupDlg->SetFon(NULL);
						pGroupDlg->SetRecipientGroup(m_strGroupName);
						pGroupDlg->SetBody(W2CT(bsText));
						pGroupDlg->ShowWindow(SW_SHOWNORMAL);
						pGroupDlg->SetForegroundWindow();
					}
				}
				
			}
		}

	}
}

void CMainDlg::OnSendMessageTemplate(UINT Id)
{
	int iChatItem = Id - 21100;

	CUser *pCurrUser = FindUserInVisualContactList(CurrTID);
	
	if(iChatItem>=0 && pCurrUser!=NULL)
	{
		// Load Message Templates
		CString strSection;
		strSection.Format(_T("%s\\%s\\%d"),GetString(IDS_INFO),GetUserRole(),GetUserID());
		
		CString strMessageTemplateXML = GetRegFileText(strSection,GetString(IDS_MESSAGE_TEMPLATES_REG));
		
		if(strMessageTemplateXML.IsEmpty())
			strMessageTemplateXML = GetString(IDS_DEFAULT_MES_TEMPLATE_XML);
		
		
		CComPtr<IXMLDOMDocument>	pMTDoc	=	NULL;
		pMTDoc.CoCreateInstance(CLSID_DOMDocument40,NULL,CLSCTX_INPROC_SERVER);
		
		if(pMTDoc)
		{
			CComBSTR bsXML;
			bsXML.Attach(strMessageTemplateXML.AllocSysString());
			
			VARIANT_BOOL	varLoad	=	VARIANT_FALSE;
			
			pMTDoc->loadXML(bsXML,&varLoad);
			
			if(varLoad==VARIANT_TRUE)
			{
				CComPtr<IXMLDOMNodeList>	pTemplatesList	=	NULL;
				
				pMTDoc->selectNodes(CComBSTR(L"message_templates/mt"),&pTemplatesList);
				
				if(pTemplatesList!=NULL)
				{
					USES_CONVERSION;
					
					long ListLength	=	0;
					pTemplatesList->get_length(&ListLength);
					
					if(iChatItem<ListLength)
					{
						CComBSTR	bsName, bsText;
						
						CComPtr<IXMLDOMNode>	pStubNode	=	NULL;
						pTemplatesList->get_item(iChatItem,&pStubNode);
						
						GetTextByPath(pStubNode, CComBSTR(L"text"),&bsText);

						SendMessageToUser(pCurrUser,TRUE,W2CT(bsText),GetOptionInt(IDS_OFSMESSENGER,IDS_SEND_MESSAGE_TEMPLATE_AUTO,1));
						
					}
				}
				
			}
		}
	}
}

void CMainDlg::OnInviteGroupToConference(UINT Id)
{
	int iChatItem = Id - 20700;
	
	if(iChatItem>=0&&iChatItem<m_ChatCollection.GetSize()&&!m_strGroupName.IsEmpty())
	{
		USES_CONVERSION;
		CChat SelChat	=	m_ChatCollection.GetAt(iChatItem);
		
		CChatCreateDlg	*pNewChatDlg	=	new CChatCreateDlg(this);
		
		CUserCollection	NewUsers;
		
		CUserCollection& ChatUsers  = SelChat.GetUsers();
		
		CUserCollection	ContactList;
		
		GetCopyContactList(ContactList);
		
		POSITION pos =  ContactList.InitIteration();
		
		CUser	*pUser = NULL;
		
		while(ContactList.GetNext(pos,pUser))
		{
			POSITION posFind = ChatUsers.InitIteration();
			
			CUser *pFindUser = NULL;
			
			BOOL bIsPresent = FALSE;
			
			while(ChatUsers.GetNext(posFind,pFindUser))
			{
				if(pFindUser->GetGlobalID()==pUser->GetGlobalID())
				{
					bIsPresent	=	TRUE;
					break;
				}
			}
			
			if(!bIsPresent)
			{
				if(pUser)
				{
					int CLMode  = GetOptionInt(IDS_OFSMESSENGER,IDS_CLMODE,2);
					
					switch(CLMode) 
					{
					case 1:
						{
							if(m_bShowOffline||(pUser->m_iStatus!=S_OFFLINE&&pUser->m_iStatus!=S_INVISIBLE))
								if(m_strGroupName.CompareNoCase(pUser->m_strType)==0)
								{
									pUser->m_bHasNewMessages = TRUE;
								}
						}
						break;
					case 2:
						{
							if(pUser->GetStatus()!=S_OFFLINE&&pUser->GetStatus()!=S_INVISIBLE)
							{
								if(m_strGroupName.CompareNoCase(pUser->m_strType)==0)
								{
									pUser->m_bHasNewMessages = TRUE;
								}
							}
							else if(m_strGroupName.CompareNoCase(GetString(IDS_OFFLINE))==0)
								pUser->m_bHasNewMessages = TRUE;
						}
						break;
					}
				}
				
				NewUsers.SetAt(*pUser);
			}
		}
		
		pNewChatDlg->Create(CChatCreateDlg::CCDM_INVITE,W2CT(SelChat.GetName()),W2CT(SelChat.GetDescription()),NULL,&NewUsers,SelChat.GetId());
		
		m_AllClosedWindow.Add(pNewChatDlg->GetSafeHwnd());
		
		pNewChatDlg->ShowWindow(SW_SHOW);
	}
}

LRESULT CMainDlg::OnInvokeCreateChat(WPARAM w, LPARAM l)
{
	_bstr_t Url;Url.Assign((BSTR)w);
	//  [11/5/2002]
	CString strUrl = (LPCTSTR)Url;
	
	CString	strName, strDescription, strMessage;
	
	CUserCollection	Users;
	
	int iPos = strUrl.Find(_T('#'),0);
	
	int iOldPos = 0;
	
	if (iPos!=-1) 
	{
		strName = strUrl.Mid(iOldPos,iPos);
		
		iOldPos = iPos +1;
		iPos = strUrl.Find(_T('#'),iOldPos);
		if(iPos!=-1)
		{
			strDescription = strUrl.Mid(iOldPos,iPos -  iOldPos);
			
			iOldPos = iPos +1;
			iPos = strUrl.Find(_T('#'),iOldPos);
			if(iPos!=-1)
			{
				strMessage = strUrl.Mid(iOldPos,iPos -  iOldPos);
				
				// Now Extract User Id [11/5/2002]
				iOldPos = iPos +1;
				iPos = strUrl.Find(_T(','),iOldPos);
				
				while(iPos!=-1)
				{
					CString strUserId = strUrl.Mid(iOldPos,iPos -  iOldPos);
					
					CUser *pUser = FindUserInVisualContactListByGlobalId(_ttol((LPCTSTR)strUserId));
					
					if(pUser)
					{
						Users.SetAt(*pUser);
						CUser *pTmpUser = Users.GetAt(pUser->GetGlobalID());
						if(pTmpUser)
							pTmpUser->m_bHasNewMessages	=	TRUE;
					}
					
					iOldPos = iPos +1;
					iPos = strUrl.Find(_T(','),iOldPos);
				}
				
				if(Users.GetCount()!=0)
				{
					CChatCreateDlg	*pNewChatDlg	=	new CChatCreateDlg(this);
					
					CUserCollection	ContactList;
					
					pNewChatDlg->Create(CChatCreateDlg::CCDM_CREATE,strName,strDescription,strMessage,&Users);
					m_AllClosedWindow.Add(pNewChatDlg->GetSafeHwnd());
					// Load UserCollections to  CChatCreateDlg [8/8/2002]
					
					//pNewChatDlg->ShowWindow(SW_SHOWNORMAL);
					//pNewChatDlg->SetForegroundWindow();
					
					pNewChatDlg->AutoCreate();
					
				}
				
			}
		}
	}
	
	return 0;
}

LRESULT CMainDlg::OnInvokeSendMessage(WPARAM w, LPARAM l)
{
	_bstr_t Url;Url.Assign((BSTR)w);
	//  [11/5/2002]
	CString strUrl = (LPCTSTR)Url;
	
	CString	strName, strDescription, strMessage;
	
	CUserCollection	Users;
	
	int iPos = strUrl.Find(_T('#'),0);
	
	int iOldPos = 0;
	
	if (iPos!=-1) 
	{
		strMessage = strUrl.Mid(iOldPos,iPos -  iOldPos);
		
		// Now Extract User Id [11/5/2002]
		iOldPos = iPos +1;
		iPos = strUrl.Find(_T(','),iOldPos);
		
		while(iPos!=-1)
		{
			CString strUserId = strUrl.Mid(iOldPos,iPos -  iOldPos);
			
			CUser *pUser = FindUserInVisualContactListByGlobalId(_ttol((LPCTSTR)strUserId));
			
			if(pUser)
			{
				Users.SetAt(*pUser);
				CUser *pTmpUser = Users.GetAt(pUser->GetGlobalID());
				if(pTmpUser)
					pTmpUser->m_bHasNewMessages	=	TRUE;
			}
			
			iOldPos = iPos +1;
			iPos = strUrl.Find(_T(','),iOldPos);
		}
		
		if(Users.GetCount()!=0)
		{
			// Send Message ... [11/15/2002]
			CGroupMessageSendDlg *pGroupDlg = new CGroupMessageSendDlg(this);
			
			pGroupDlg->Create(GetDesktopWindow());
			
			pGroupDlg->SetFon(NULL);
			pGroupDlg->SetRecipientGroup(_T("Custom"),&Users);
			pGroupDlg->SetBody(strMessage,TRUE);
			
			//pGroupDlg->Send();
			
			//pGroupDlg->ShowWindow(SW_SHOWNORMAL);
			//pGroupDlg->SetForegroundWindow();
		}
	}
	
	return 0;
}

LRESULT CMainDlg::OnIBNToMessage(WPARAM w, LPARAM l)
{
	LRESULT ret = 0;
	if(l == IBN_MSG_IBNTO)
	{
		IBNTO_MESSAGE *pmsg = new IBNTO_MESSAGE;
		if(pmsg != NULL)
		{
			if(pmsg->Parse((LPCTSTR)w))
			{
				CString domain = GetUserDomain();
				int ProcessedLogins	=	0;
				for(int Index=0;Index<pmsg->Logins.GetSize();Index++)
				{
					if(0 == domain.CompareNoCase(pmsg->Logins[Index].Domain))
					{
						ProcessedLogins++;
					}
				}
				
				if(ProcessedLogins>0)
					AddIbnToMessage(pmsg);
				
				ret = (ProcessedLogins==pmsg->Logins.GetSize());
			}
			
			if(ret == 0)
				delete pmsg;
		}
	}
	else if(l == IBN_MSG_FILES)
	{
		SENDTO_MESSAGE *pmsg = new SENDTO_MESSAGE;
		if(pmsg != NULL)
		{
			if(pmsg->Parse((LPCTSTR)w))
			{
				AddSendToMessage(pmsg);
				ret = 1;
			}
			if(ret == 0)
				delete pmsg;
		}
	}
	PostMessage(WM_PROCESS_COMMAND_LINE_MESSAGES);
	return ret;
}

void CMainDlg::AddIbnToMessage(IBNTO_MESSAGE *pmsg)
{
	if(pmsg != NULL)
	{
		m_IbntoMessages.Add(pmsg);
	}
}

void CMainDlg::AddSendToMessage(SENDTO_MESSAGE *pmsg)
{
	if(pmsg != NULL)
	{
		m_SendtoMessages.Add(pmsg);
	}
}

void CMainDlg::ProcessIbntoMessages()
{
	while(m_IbntoMessages.GetSize() > 0)
	{
		IBNTO_MESSAGE *pmsg = m_IbntoMessages.GetAt(0);
		CUser* pUser=NULL;
		POSITION pos;
		BOOL bSent = FALSE;
		
		CUserCollection	MessageSenders;
		
		if(NULL != (pos = m_ContactList.InitIteration()))
		{
			for(int i=0; m_ContactList.GetNext(pos, pUser); i++)
			{
				for(int Index = 0;Index<pmsg->Logins.GetSize();Index++)
				{
					if(0 == pUser->m_strLogin.CompareNoCase(pmsg->Logins[Index].Login))
					{
						MessageSenders.SetAt(*pUser);
						CUser *pTmpUser = MessageSenders.GetAt(pUser->GetGlobalID());
						if(pTmpUser)
							pTmpUser->m_bHasNewMessages	=	TRUE;
						
						//						SendMessageToUser(pUser, FALSE, pmsg->Message);
						//						bSent = TRUE;
					}
				}
			}
		}
		
		if(MessageSenders.GetCount()<pmsg->Logins.GetSize() && NULL != (pos = m_ExternalContactList.InitIteration()))
		{
			for(int i=0; m_ExternalContactList.GetNext(pos, pUser); i++)
			{
				for(int Index = 0;Index<pmsg->Logins.GetSize();Index++)
				{
					if(0 == pUser->m_strLogin.CompareNoCase(pmsg->Logins[Index].Login))
					{
						MessageSenders.SetAt(*pUser);
						CUser *pTmpUser = MessageSenders.GetAt(pUser->GetGlobalID());
						if(pTmpUser)
							pTmpUser->m_bHasNewMessages	=	TRUE;
						
						//						SendMessageToUser(pUser, FALSE, pmsg->Message);
						//						bSent = TRUE;
					}
				}
			}
		}
		
		if(MessageSenders.GetCount()==1)
		{
			POSITION pos	=	MessageSenders.InitIteration();
			while(pos!=NULL)
			{
				CUser *pSingleSenderUser	=	NULL;
				MessageSenders.GetNext(pos,pSingleSenderUser);
				SendMessageToUser(pSingleSenderUser, FALSE, pmsg->Message);
			}
			
		}
		else if(MessageSenders.GetCount()>1)
		{
			CGroupMessageSendDlg *pGroupDlg = new CGroupMessageSendDlg(this);
			
			pGroupDlg->Create(GetDesktopWindow());
			
			pGroupDlg->SetFon(NULL);
			pGroupDlg->SetRecipientGroup(_T("Ibnto"),&MessageSenders);
			pGroupDlg->SetBody(pmsg->Message);
			
			pGroupDlg->ShowWindow(SW_SHOWNORMAL);
			pGroupDlg->SetForegroundWindow();
		}
		
		m_IbntoMessages.RemoveAt(0);
		delete pmsg;
	}
}

void CMainDlg::ProcessSendtoMessages()
{
	while(m_SendtoMessages.GetSize() > 0)
	{
		SENDTO_MESSAGE *pmsg = m_SendtoMessages.GetAt(0);
		
		int n = pmsg->m_files.GetSize();
		
		if(n > 30)
		{
			CString	strMessage;
			strMessage.LoadString(IDS_FILES_SEND_LIMIT);
			MessageBox(strMessage,GetString(IDS_ERROR_TITLE),MB_OK|MB_ICONERROR);
		}
		else
		{
			// Create files list.
			CString strFiles;
			for(int i=0; i<n; i++)
			{
				CString strPath = pmsg->m_files.GetAt(i);
				strPath = strPath.Mid(strPath.ReverseFind('\\')+1);
				
				strFiles += strPath;
				strFiles += _T("; ");
			}
			
			// Show "Send File To Group" dialog.
			CGroupFileDescriptionDlg dlg(this);
			dlg.m_strRecepientGroupName.Empty();
			dlg.m_strFileName = strFiles;
			
			if(dlg.DoModal()==IDOK)
			{
				CString strDescription = dlg.GetFileDescription();
				
				CString strUserId;
				for(int i=0; i<n; i++)
				{
					if(strUserId.IsEmpty())
					{
						// Get user IDs
						if(POSITION pos = dlg.m_ContactList.InitIteration())
						{
							CUser *pUser = NULL;
							while(dlg.m_ContactList.GetNext(pos, pUser))
							{
								if(pUser->m_bHasNewMessages)
								{
									TCHAR Buff[MAX_PATH];
									_ltot(pUser->GetGlobalID(), Buff, 10);
									strUserId += Buff;
									strUserId += _T(",");
								}
							}
						}
					}
					if(!strUserId.IsEmpty())
						m_FileManager.AddToUpload2(pmsg->m_files.GetAt(i), dlg.m_strRecepientGroupName, strUserId, strDescription);
				}
			}
		}
		
		m_SendtoMessages.RemoveAt(0);
		delete pmsg;
	}
}

LRESULT CMainDlg::OnProcessCommandLineMessages(WPARAM w, LPARAM l)
{
	if(!ConnectEnable(TRUE))
		return 0;
	
	ProcessIbntoMessages();
	ProcessSendtoMessages();
	
	return 0;
}

void CMainDlg::OnNetworkMonitor()
{
	if(IsWindow(m_hWndNetMonitor))
	{
		CMcWindowAgent winAgent(m_hWndNetMonitor);
		winAgent.ShowWindow(SW_SHOWNORMAL);
		winAgent.SetForegroundWindow();
	}
	else
	{
		CMonitorDialog	*pMonitorDlg = new CMonitorDialog(this);
		pMonitorDlg->Create(CMonitorDialog::IDD, GetDesktopWindow());
		pMonitorDlg->ShowWindow(SW_SHOWNORMAL);
		
		m_hWndNetMonitor = pMonitorDlg->GetSafeHwnd();
		AddToAllCloseWindow(m_hWndNetMonitor);
	}
	
}

void CMainDlg::OnUpdateNetworkMonitor(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(TRUE);
}

CString CMainDlg::ChangeUrlForCurrentDomain(LPCTSTR Url)
{
	CString strModifyedUrl = Url;
	
	ATL::CUrl		urlParser;
	
	if(urlParser.CrackUrl(strModifyedUrl))
	{
		if(urlParser.GetHostName()==GetUserDomain())
		{
			CString urlPath = urlParser.GetUrlPath();

			CString urlPathLower = urlPath;
			urlPathLower.MakeLower();
			
			int iPortalPos = urlPath.Find(_T("/"), 1);
			int iWebDavPos = urlPathLower.Find(_T("/webdav/"));

			if(iPortalPos!=-1 && iWebDavPos==-1)
			{
				CString strUnEscapedPath = urlPath.Mid(1);
				CString strEscapedPath; 
				
				if(strUnEscapedPath.GetLength())
				{
					DWORD dwEscapeLen =  strUnEscapedPath.GetLength()*2;
					
					AtlEscapeUrl(strUnEscapedPath,strEscapedPath.GetBuffer(dwEscapeLen),&dwEscapeLen,dwEscapeLen,ATL_URL_ESCAPE);
					strEscapedPath.ReleaseBuffer();
				}
				else
				{
					strEscapedPath = _T("");
				}
				
				
				CString strUnEscapeExtraInfo = urlParser.GetExtraInfo();
				CString strEscapeExtraInfo;
				
				if(strUnEscapeExtraInfo.GetLength())
				{
					DWORD dwEscapeExtraInfoLen =  strUnEscapeExtraInfo.GetLength()*2;
					
					AtlEscapeUrl(strUnEscapeExtraInfo,strEscapeExtraInfo.GetBuffer(dwEscapeExtraInfoLen),&dwEscapeExtraInfoLen,dwEscapeExtraInfoLen,ATL_URL_ESCAPE);
					strEscapeExtraInfo.ReleaseBuffer();
				}
				else
				{
					strEscapeExtraInfo = _T("");
				}
				
				strModifyedUrl.Format(_T("%s/default.aspx?sid=%s&redirect=%s%s"), GetServerPath(), GetSID(), strEscapedPath, strEscapeExtraInfo);
			}
		}
	}
	
	return strModifyedUrl;
}

void CMainDlg::OnUpdateTreemenuAssignToDo(CCmdUI* pCmdUI)
{
	//pCmdUI->Enable(IsToolboxIstalled());	
	pCmdUI->Enable(ConnectEnable(FALSE)||IsToolboxIstalled());	
}

void CMainDlg::OnUpdateTreegroupAssignToDo(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(ConnectEnable(FALSE)||IsToolboxIstalled());	
}

BOOL CMainDlg::IsSSLMode()
{
	return  m_bIsSSLMode;
}

const CString CMainDlg::ScreenCapturePath()
{
	CRegKey	toolBoxKey;

	
	CString strSubKeyName;
#ifndef RADIUS
		strSubKeyName = _T("SOFTWARE\\Mediachase\\Instant Business Network\\4.5\\ScreenCapture");
#else
		strSubKeyName = _T("SOFTWARE\\Radius-Soft\\MagRul\\4.5\\ScreenCapture");
#endif
	
	if(toolBoxKey.Open(HKEY_LOCAL_MACHINE,strSubKeyName)==S_OK)
	{
		TCHAR ToolboxPath[MAX_PATH]	=	_T("");
		DWORD dwToolboxPathLen = MAX_PATH;

		if(toolBoxKey.QueryValue(ToolboxPath,_T("Path"),&dwToolboxPathLen)==S_OK&&
			dwToolboxPathLen>0)
		{
			return ToolboxPath;
		}
	}

	return _T("");
}


const CString CMainDlg::ToolboxPath()
{
	CRegKey	toolBoxKey;

		CString strSubKeyName;
#ifndef RADIUS
		strSubKeyName = _T("SOFTWARE\\Mediachase\\Instant Business Network\\4.5\\Toolbox Wizard");
#else
		strSubKeyName = _T("SOFTWARE\\Radius-Soft\\MagRul\\4.5\\Toolbox Wizard");
#endif
	
	if(toolBoxKey.Open(HKEY_LOCAL_MACHINE,strSubKeyName)==S_OK)
	{
		TCHAR ToolboxPath[MAX_PATH]	=	_T("");
		DWORD dwToolboxPathLen = MAX_PATH;
		if(toolBoxKey.QueryValue(ToolboxPath,_T("Path"),&dwToolboxPathLen)==S_OK&&
			dwToolboxPathLen>0)
		{
			return ToolboxPath;
		}
	}
	
	return _T("");
}


BOOL CMainDlg::IsToolboxIstalled()
{
	return (ToolboxPath().GetLength()>0);
}

void CMainDlg::OnTreemenuAssignToDo()
{
	CUser *pUser = FindUserInVisualContactList(CurrTID);
	if(pUser)
	{
		if(IsToolboxIstalled())
		{
			CString strParametrs;
			strParametrs.Format(_T("/CREATETODO /IBNRESOURCES \"%d\" /L \"%s\" /P \"%s\""),pUser->GetGlobalID(), m_DlgLog.m_LoginStr, m_DlgLog.m_PasswordStr);
			
			if(m_bIsSSLMode)
				strParametrs	+= _T(" /USESSL");
			
			ShellExecute(::GetDesktopWindow(),NULL,ToolboxPath(),strParametrs,NULL,SW_SHOWNORMAL);
		}
		else
		{
			CString strUserId;
			strUserId.Format(_T("%d"),pUser->GetGlobalID());

			CString strUrl;
			strUrl.Format(GetString(IDS_WEB_ASSIGN_TODO),GetServerPath(),GetSID(), strUserId);
			
			if(m_InWindow.NavigateNewWindow(strUrl)!=S_OK)
				ShellExecute(NULL,_T("open"),strUrl,NULL,NULL,SW_SHOWNORMAL);
		}
	}
}

void CMainDlg::OnTreegroupAssignToDo()
{
	CGroupAssignToDoDlg DescrDlg(this);
	
	DescrDlg.m_strRecepientGroupName = m_strGroupName;
	
	if(DescrDlg.DoModal()==IDOK)
	{
		CString strUserId	=	_T("");
		if(POSITION pos = DescrDlg.m_ContactList.InitIteration())
		{
			CUser	*pUser =	 NULL;
			while(DescrDlg.m_ContactList.GetNext(pos,pUser))
			{
				if(pUser->m_bHasNewMessages)
				{
					TCHAR Buff[MAX_PATH];
					_ltot(pUser->GetGlobalID(), Buff, 10);
					strUserId += Buff;
					strUserId += ",";
				}
			}
			
			if(!strUserId.IsEmpty())
			{
				if(IsToolboxIstalled())
				{
					CString strParametrs;
					strParametrs.Format(_T("/CREATETODO /IBNRESOURCES \"%s\" /L \"%s\" /P \"%s\""),strUserId, m_DlgLog.m_LoginStr, m_DlgLog.m_PasswordStr);
					
					if(m_bIsSSLMode)
						strParametrs	+= _T(" /USESSL");
					
					ShellExecute(::GetDesktopWindow(),NULL,ToolboxPath(),strParametrs,NULL,SW_SHOWNORMAL);
				}
				else
				{
					CString strUrl;
					strUrl.Format(GetString(IDS_WEB_ASSIGN_TODO),GetServerPath(),GetSID(), strUserId);
					
					if(m_InWindow.NavigateNewWindow(strUrl)!=S_OK)
						ShellExecute(NULL,_T("open"),strUrl,NULL,NULL,SW_SHOWNORMAL);
				}
			}
		}
	}
}

void CMainDlg::OnDropFiles(HDROP hDrop)
{
	UINT uFilesDropped = DragQueryFile(hDrop,-1,NULL,0);
	
	if(IsToolboxIstalled())
	{
		TCHAR	fileName[1000];
		
		CString strAllFiles;
		
		for(int iFileIndex=0;iFileIndex<uFilesDropped;iFileIndex++)
		{
			UINT uRetVal = DragQueryFile(hDrop,iFileIndex,fileName,1000);
			
			strAllFiles	+=	_T("\"");
			strAllFiles	+=	fileName;
			strAllFiles	+=	_T("\" ");
		}
		
		//TRACE("CMainDlg::OnDropFiles %s \r\n",strAllFiles);
		
		if(strAllFiles.GetLength()>0)
		{
			CString strParametrs;
			strParametrs.Format(_T("/UPLOAD /L \"%s\" /P \"%s\" %s"),m_DlgLog.m_LoginStr, m_DlgLog.m_PasswordStr,strAllFiles);
			
			if(m_bIsSSLMode)
				strParametrs	+= _T(" /USESSL");
			
			HINSTANCE retVal = ShellExecute(::GetDesktopWindow(),NULL,ToolboxPath(),strParametrs,NULL,SW_SHOWNORMAL);
		}
	}
	
	DragFinish(hDrop);
}

void CMainDlg::OnRightButtonAssignToDo()
{
	if(m_hRightButtonDDGlobal==NULL)
		return;
	
	// Step 1. Extract Files  [3/31/2004]
	HDROP	hDrop	=	(HDROP)GlobalLock(m_hRightButtonDDGlobal);
	
	UINT uFilesDropped = DragQueryFile(hDrop,-1,NULL,0);
	
	TCHAR	fileName[1000];
	
	CString strAllFiles;
	
	CString strAllIBNUsers;
	
	for(int iFileIndex=0;iFileIndex<uFilesDropped;iFileIndex++)
	{
		UINT uRetVal = DragQueryFile(hDrop,iFileIndex,fileName,1000);
		
		strAllFiles	+=	_T("\"");
		strAllFiles	+=	fileName;
		strAllFiles	+=	_T("\" ");
	}
	
	//TRACE("CMainDlg::OnRightButtonAssignToDo %s \r\n",strAllFiles);
	
	// Step 2. Extract Rresources
	if(m_bRightButtonDDTID==-1)
	{
		if(strAllFiles.GetLength()>0)
		{
			CString strParametrs;
			strParametrs.Format(_T("/CREATETODO /L \"%s\" /P \"%s\" %s"),m_DlgLog.m_LoginStr, m_DlgLog.m_PasswordStr,strAllFiles);
			
			if(m_bIsSSLMode)
				strParametrs	+= _T(" /USESSL");
			
			HINSTANCE retVal = ShellExecute(::GetDesktopWindow(),NULL,ToolboxPath(),strParametrs,NULL,SW_SHOWNORMAL);
		}
	}
	else
	{
		if(!m_bRightButtonDDGroupe)
		{
			CUser *pUser = FindUserInVisualContactList(m_bRightButtonDDTID);
			
			if(pUser)
			{
				CString strTmp;
				strTmp.Format(_T("%d"), pUser->GetGlobalID());
				strAllIBNUsers	=	strTmp;
			}
		}
		else
		{
			CString GroupName	=	m_treebox.GetItemText(m_bRightButtonDDTID);
			
			CString strDescription;
			CGroupAssignToDoDlg	DescrDlg(this);
			
			DescrDlg.m_strRecepientGroupName = GroupName;
			
			if(DescrDlg.DoModal()==IDOK)
			{
				CString strUserId	=	_T("");
				if(POSITION pos = DescrDlg.m_ContactList.InitIteration())
				{
					CUser	*pUser =	 NULL;
					while(DescrDlg.m_ContactList.GetNext(pos,pUser))
					{
						if(pUser->m_bHasNewMessages)
						{
							CString strTmp;
							if(strAllIBNUsers.GetLength()>0)
								strAllIBNUsers	+=	",";
							strTmp.Format(_T("%d"), pUser->GetGlobalID());
							strAllIBNUsers	+=	strTmp;
						}
					}
				}
				
			}
			
		}
		
		// Step 3. Run IBN Toolbox [3/31/2004]
		if(strAllFiles.GetLength()>0 && strAllIBNUsers.GetLength()>0)
		{
			CString strParametrs;
			strParametrs.Format(_T("/CREATETODO /L \"%s\" /P \"%s\" /IBNRESOURCES \"%s\" %s"),m_DlgLog.m_LoginStr, m_DlgLog.m_PasswordStr,strAllIBNUsers,strAllFiles);
			
			if(m_bIsSSLMode)
				strParametrs	+= _T(" /USESSL");
			
			HINSTANCE retVal = ShellExecute(::GetDesktopWindow(),NULL,ToolboxPath(),strParametrs,NULL,SW_SHOWNORMAL);
		}
	}
	
	GlobalUnlock(m_hRightButtonDDGlobal);
	GlobalFree(m_hRightButtonDDGlobal);
	m_hRightButtonDDGlobal = NULL;
}

void CMainDlg::OnUpdateRightButtonAssignToDo(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(IsToolboxIstalled());
}

void CMainDlg::OnRightButtonSendFile()
{
	if(!m_bRightButtonDDGroupe)
	{
		CUser *pUser = FindUserInVisualContactList(m_bRightButtonDDTID);
		
		if(pUser)
		{
			OnSendFile((WPARAM)pUser->GetGlobalID(),(LPARAM)m_hRightButtonDDGlobal);
		}
	}
	else
	{
		CString GroupName	=	m_treebox.GetItemText(m_bRightButtonDDTID);
		if(GroupName.GetLength()>0)
		{
			OnSendGroupFile((WPARAM)GroupName.AllocSysString(),(LPARAM)m_hRightButtonDDGlobal);
		}
	}
	
	m_hRightButtonDDGlobal = NULL;
}

void CMainDlg::OnUpdateRightButtonSendFile(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(ConnectEnable(FALSE)&&m_bRightButtonDDTID!=-1);
}

void CMainDlg::OnRightButtonUploadFile()
{
	if(m_hRightButtonDDGlobal==NULL)
		return;
	
	HDROP	hDrop	=	(HDROP)GlobalLock(m_hRightButtonDDGlobal);
	
	UINT uFilesDropped = DragQueryFile(hDrop,-1,NULL,0);
	
	if(IsToolboxIstalled())
	{
		TCHAR	fileName[1000];
		
		CString strAllFiles;
		
		for(int iFileIndex=0;iFileIndex<uFilesDropped;iFileIndex++)
		{
			UINT uRetVal = DragQueryFile(hDrop,iFileIndex,fileName,1000);
			
			strAllFiles	+=	_T("\"");
			strAllFiles	+=	fileName;
			strAllFiles	+=	_T("\" ");
		}
		
		//TRACE("CMainDlg::OnDropFiles %s \r\n",strAllFiles);
		
		if(strAllFiles.GetLength()>0)
		{
			CString strParametrs;
			strParametrs.Format(_T("/UPLOAD /L \"%s\" /P \"%s\" %s"),m_DlgLog.m_LoginStr, m_DlgLog.m_PasswordStr,strAllFiles);
			
			if(m_bIsSSLMode)
				strParametrs	+= _T(" /USESSL");
			
			ShellExecute(::GetDesktopWindow(),NULL,ToolboxPath(),strParametrs,NULL,SW_SHOWNORMAL);
		}
	}
	
	GlobalUnlock(m_hRightButtonDDGlobal);
	GlobalFree(m_hRightButtonDDGlobal);
	m_hRightButtonDDGlobal = NULL;
}

void CMainDlg::OnUpdateRightButtonUploadFile(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(IsToolboxIstalled());
}

void CMainDlg::OnRightButtonCancel()
{
	if(m_hRightButtonDDGlobal==NULL)
		return;
	GlobalFree(m_hRightButtonDDGlobal);
	m_hRightButtonDDGlobal = NULL;
}

void CMainDlg::OnUpdateRightButtonCancel(CCmdUI* pCmdUI)
{
}

void CMainDlg::OnRightButtonCreateIssue()
{
	if(m_hRightButtonDDGlobal==NULL)
		return;
	
	HDROP	hDrop	=	(HDROP)GlobalLock(m_hRightButtonDDGlobal);
	
	UINT uFilesDropped = DragQueryFile(hDrop,-1,NULL,0);
	
	if(IsToolboxIstalled())
	{
		TCHAR	fileName[1000];
		
		CString strAllFiles;
		
		for(int iFileIndex=0;iFileIndex<uFilesDropped;iFileIndex++)
		{
			UINT uRetVal = DragQueryFile(hDrop,iFileIndex,fileName,1000);
			
			strAllFiles	+=	_T("\"");
			strAllFiles	+=	fileName;
			strAllFiles	+=	_T("\" ");
		}
		
		//TRACE("CMainDlg::OnDropFiles %s \r\n",strAllFiles);
		
		if(strAllFiles.GetLength()>0)
		{
			CString strParametrs;
			strParametrs.Format(_T("/CREATEINCIDENT /L \"%s\" /P \"%s\" %s"),m_DlgLog.m_LoginStr, m_DlgLog.m_PasswordStr,strAllFiles);
			
			if(m_bIsSSLMode)
				strParametrs	+= _T(" /USESSL");
			
			ShellExecute(::GetDesktopWindow(),NULL,ToolboxPath(),strParametrs,NULL,SW_SHOWNORMAL);
		}
	}
	
	GlobalUnlock(m_hRightButtonDDGlobal);
	GlobalFree(m_hRightButtonDDGlobal);
	m_hRightButtonDDGlobal = NULL;
}

void CMainDlg::OnUpdateRightButtonCreateIssue(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(IsToolboxIstalled());
}


DROPEFFECT CMainDlg::COleMainDropTarget::OnDragEnter( CWnd* pWnd, COleDataObject* pDataObject, DWORD dwKeyState, CPoint point )
{
	if(!pDataObject->IsDataAvailable(CF_HDROP))
	{
		m_dropEffectCurrent = DROPEFFECT_NONE;
		return m_dropEffectCurrent;
	}
	m_dwKeyState	=	dwKeyState;
	m_dropEffectCurrent = DROPEFFECT_COPY;
	return m_dropEffectCurrent;
}

BOOL CMainDlg::COleMainDropTarget::OnDrop( CWnd* pWnd, COleDataObject* pDataObject, DROPEFFECT dropEffect, CPoint point )
{
	if(pDataObject->IsDataAvailable(CF_HDROP))
	{
		m_pMain->SetForegroundWindow();
		
		HGLOBAL	hMem	=	pDataObject->GetGlobalData(CF_HDROP);
		HDROP   hDrop	=	(HDROP)GlobalLock(hMem);
		
		//if(m_dwKeyState&MK_RBUTTON)
		{
			m_pMain->m_bRightButtonDDGroupe	=	FALSE;
			m_pMain->m_bRightButtonDDTID = -1;
			
			if(m_pMain->m_hRightButtonDDGlobal!=NULL)
			{
				GlobalFree(m_pMain->m_hRightButtonDDGlobal);
				m_pMain->m_hRightButtonDDGlobal = NULL;
			}
			
			//Right button Drug and Drop
			SIZE_T Size = GlobalSize(hMem);
			m_pMain->m_hRightButtonDDGlobal = GlobalAlloc(GMEM_ZEROINIT,Size);
			
			memcpy(GlobalLock(m_pMain->m_hRightButtonDDGlobal),GlobalLock(hMem),Size);
			
			GlobalUnlock(m_pMain->m_hRightButtonDDGlobal);
			GlobalUnlock(hMem);
			
			CPoint point;
			GetCursorPos(&point);
			CMenu menu;
			menu.LoadMenu(IDR_MESSENGER_MENU);
			CMenu* popup = menu.GetSubMenu(9);
			
			UpdateMenu(m_pMain,popup);
			
			popup->TrackPopupMenu(TPM_LEFTALIGN|TPM_RIGHTBUTTON, point.x, point.y, m_pMain);
		}
		//else
		//{
		//	m_pMain->SendMessage(WM_DROPFILES,(WPARAM)hDrop,0);
		//}
		GlobalUnlock(hMem);
		
		return TRUE;
	}
	
	return FALSE;	
}

DROPEFFECT CMainDlg::COleMainDropTarget::OnDragOver(CWnd* pWnd, COleDataObject* pDataObject, DWORD dwKeyState, CPoint point )
{
	if(!pDataObject->IsDataAvailable(CF_HDROP))
	{
		m_dropEffectCurrent = DROPEFFECT_NONE;
		return m_dropEffectCurrent;
	}
	//dwKeyState	=	dwKeyState;
	m_dropEffectCurrent = DROPEFFECT_COPY;
	return m_dropEffectCurrent;
}

void CMainDlg::COleMainDropTarget::OnDragLeave(CWnd* pWnd)
{
	COleDropTarget::OnDragLeave(pWnd);
}

void CMainDlg::OnTreeMenuSendScreenShot()
{
	CUser *pUser = FindUserInVisualContactList(CurrTID);
	if(!pUser)
		return;
	
	MakeScreenShot(CScreenShotDlg::SendFile,pUser->GetGlobalID());
}

void CMainDlg::OnUpdateTreeMenuSendScreenShot(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(ConnectEnable(FALSE));
}

void CMainDlg::OnTreeMenuScreenShotAssignToDo()
{
	CUser *pUser = FindUserInVisualContactList(CurrTID);
	if(!pUser)
		return;
	
	MakeScreenShot(CScreenShotDlg::AssignToDo,pUser->GetGlobalID());
}

void CMainDlg::OnUpdateTreeMenuScreenShotAssignToDo(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(IsToolboxIstalled());
}

void CMainDlg::OnTreeGroupSendScreenShot()
{
	MakeScreenShot(CScreenShotDlg::AssignToDo,-1,m_strGroupName);
}

void CMainDlg::OnUpdateTreeGroupSendScreenShot(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(ConnectEnable(FALSE));
}

void CMainDlg::OnTreeGroupScreenShotAssignToDo()
{
	MakeScreenShot(CScreenShotDlg::AssignToDo,-1,m_strGroupName);
}

void CMainDlg::OnUpdateTreeGroupScreenShotAssignToDo(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(IsToolboxIstalled());
}

void CMainDlg::OnTreeMenuCreateEvent()
{
	if(IsToolboxIstalled())
	{
		CUser *pUser = FindUserInVisualContactList(CurrTID);
		if(pUser)
		{
			CString strParametrs;
			strParametrs.Format(_T("/CREATEEVENT /IBNRESOURCES \"%d\" /L \"%s\" /P \"%s\""),pUser->GetGlobalID(), m_DlgLog.m_LoginStr, m_DlgLog.m_PasswordStr);
			
			if(m_bIsSSLMode)
				strParametrs	+= _T(" /USESSL");
			
			ShellExecute(::GetDesktopWindow(),NULL,ToolboxPath(),strParametrs,NULL,SW_SHOWNORMAL);
		}
	}
}

void CMainDlg::OnUpdateTreeMenuCreateEvent(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(IsToolboxIstalled());
}

void CMainDlg::OnTreeGroupCreateEvent()
{
	if(IsToolboxIstalled())
	{
		CGroupAssignToDoDlg DescrDlg(this);
		
		DescrDlg.m_strRecepientGroupName = m_strGroupName;
		
		if(DescrDlg.DoModal()==IDOK)
		{
			CString strUserId	=	_T("");
			if(POSITION pos = DescrDlg.m_ContactList.InitIteration())
			{
				CUser	*pUser =	 NULL;
				while(DescrDlg.m_ContactList.GetNext(pos,pUser))
				{
					if(pUser->m_bHasNewMessages)
					{
						TCHAR Buff[MAX_PATH];
						_ltot(pUser->GetGlobalID(),Buff,10);
						strUserId += Buff;
						strUserId += ",";
					}
				}
				
				if(!strUserId.IsEmpty())
				{
					CString strParametrs;
					strParametrs.Format(_T("/CREATEEVENT /IBNRESOURCES \"%s\" /L \"%s\" /P \"%s\""),strUserId, m_DlgLog.m_LoginStr, m_DlgLog.m_PasswordStr);
					
					if(m_bIsSSLMode)
						strParametrs	+= _T(" /USESSL");
					
					ShellExecute(::GetDesktopWindow(),NULL,ToolboxPath(),strParametrs,NULL,SW_SHOWNORMAL);
				}
			}
		}
	}
}

void CMainDlg::OnUpdateTreeGroupCreateEvent(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(IsToolboxIstalled());
}

CString	CMainDlg::GetSelectedUserStringFromXML(BSTR bsXML)
{
	CString retVal = _T("");
	
	CComPtr<IXMLDOMDocument>	pTreeItemDoc	=	NULL;
	
	pTreeItemDoc.CoCreateInstance(CLSID_DOMDocument40, NULL, CLSCTX_INPROC_SERVER);
	
	VARIANT_BOOL	varSuc	=	VARIANT_FALSE;
	pTreeItemDoc->loadXML(bsXML,&varSuc);
	if(varSuc==VARIANT_TRUE)
	{
		CComPtr<IXMLDOMNodeList>	pSelectedNodeList;
		
		pTreeItemDoc->selectNodes(CComBSTR(L"treeitem/treeitem/treeitem[@checked='1']"),&pSelectedNodeList);
		
		if(pSelectedNodeList!=NULL)
		{
			long lSelectedUserCout = 0;
			
			pSelectedNodeList->get_length(&lSelectedUserCout);
			
			for(long ItemIndex = 0;ItemIndex<lSelectedUserCout;ItemIndex++)
			{
				CComPtr<IXMLDOMNode>		pSelectedNode;
				pSelectedNodeList->get_item(ItemIndex,&pSelectedNode);
				if(pSelectedNode!=NULL)
				{
					CComVariant	varValue;
					GetAttribute(pSelectedNode,CComBSTR(L"id"),&varValue);
					
					if(varValue.ChangeType(VT_I4)==S_OK)
					{
						CString strId;
						strId.Format(_T("%ld"),varValue.lVal);
						
						if(retVal.GetLength()>0)
							retVal += _T(",");
						
						retVal += strId;
					}
				}
			}
		}
	}
	
	return retVal;
}

CComBSTR CMainDlg::CreateScreenShotUsersXML(LPCTSTR strGroupName)
{
	CComBSTR bsRetVal;
	
	CUserCollection	UsersContactList;
	CreateScreenShotUsers(-1,strGroupName,UsersContactList);
	
	CUser* pUser=NULL;

	if(POSITION pos = UsersContactList.InitIteration())
	{
		for(int i=0; UsersContactList.GetNext(pos, pUser); i++)
		{
			if(pUser->m_bHasNewMessages)
			{
				WCHAR wsTmpBuf[20];
				_ltow(pUser->GetGlobalID(),wsTmpBuf,10);

				bsRetVal += L"<id>";
				bsRetVal += wsTmpBuf;
				bsRetVal += L"</id>";
			}
		}
	}
	
	return bsRetVal;
}

CComBSTR CMainDlg::CreateScreenShotUsersXML(long RecepientUserId, LPCTSTR strGroupName)
{
	USES_CONVERSION;
	
	CUserCollection	UsersContactList;
	CreateScreenShotUsers(RecepientUserId,strGroupName,UsersContactList);
	
	CComPtr<IXMLDOMDocument>	pTreeItemDoc	=	NULL;
	
	pTreeItemDoc.CoCreateInstance(CLSID_DOMDocument40, NULL, CLSCTX_INPROC_SERVER);
	
	VARIANT_BOOL	varSuc	=	VARIANT_FALSE;
	CComBSTR	bsDefaultXML	= L"<treeitem id='0' checked='0' text='";	
	bsDefaultXML	+= T2CW(GetString(IDS_CAPTURE_ACTION_TREEHEADER));
	bsDefaultXML	+= L"'></treeitem>";
	
	pTreeItemDoc->loadXML(bsDefaultXML,&varSuc);
	
	CComPtr<IXMLDOMNode>	pRootNode;
	pTreeItemDoc->selectSingleNode(CComBSTR(L"treeitem"),&pRootNode);
	
	int CLMode  = GetOptionInt(IDS_OFSMESSENGER,IDS_CLMODE,2);
	
	CUser* pUser=NULL;
	
	switch(CLMode) 
	{
	case 1:
		{
			int GroupGlobalId = -1;
			if(POSITION pos = UsersContactList.InitIteration())
			{
				for(int i=0; UsersContactList.GetNext(pos, pUser); i++)
				{
					// Step 1. Проверить создавали ли мы группу???
					CComBSTR	GroupName =	pUser->m_strType;
					
					BOOL isCheck = FALSE;//(m_strRecepientGroupName.CompareNoCase(pUser->m_strType)==0);
					
					CComBSTR bsGroupPath = L"treeitem/treeitem[@text='";
					bsGroupPath += GroupName;
					bsGroupPath += L"']";
					
					CComPtr<IXMLDOMNode>	pGroupNode;
					
					pTreeItemDoc->selectSingleNode(bsGroupPath,&pGroupNode);
					
					if(pGroupNode==NULL)
					{
						CComPtr<IXMLDOMNode>	pTmpGroupNode;
						
						pTreeItemDoc->createNode(CComVariant(NODE_ELEMENT),CComBSTR(L"treeitem"),NULL,&pTmpGroupNode);
						
						SetAttribute(pTmpGroupNode,CComBSTR(L"id"),CComVariant(GroupGlobalId--));
						SetAttribute(pTmpGroupNode,CComBSTR(L"text"),CComVariant(GroupName));
						SetAttribute(pTmpGroupNode,CComBSTR(L"checked"),CComVariant("0"));
						
						//pRootNode->appendChild(pTmpGroupNode,&pGroupNode);
						pGroupNode = AppendWithSort(pRootNode, pTmpGroupNode, CComBSTR(L"text"));
					}
					
					// Step 3. добавить пользователя [1/28/2002]
					//pUser->TID = m_treectrl.AddItem(GroupTID,pUser->GetShowName(),m_ShablonId[pUser->m_bHasNewMessages+2]);								
					
					CComPtr<IXMLDOMNode>	pTmpUserNode, pUserNode;
					
					pTreeItemDoc->createNode(CComVariant(NODE_ELEMENT),CComBSTR(L"treeitem"),NULL,&pTmpUserNode);
					
					SetAttribute(pTmpUserNode,CComBSTR(L"id"),CComVariant(pUser->GetGlobalID()));
					SetAttribute(pTmpUserNode,CComBSTR(L"text"),CComVariant(pUser->GetShowName()));
					SetAttribute(pTmpUserNode,CComBSTR(L"checked"),CComVariant(pUser->m_bHasNewMessages?L"1":L"0"));
					
					//pGroupNode->appendChild(pTmpUserNode,&pUserNode);
					pUserNode = AppendWithSort(pGroupNode, pTmpUserNode, CComBSTR(L"text"));
					
					if(pUser->m_bHasNewMessages&&strGroupName!=NULL)
					{
						SetAttribute(pGroupNode,CComBSTR(L"checked"),CComVariant(L"1"));
					}
					
				}
			}
		}
		break;
	case 2:
		{
			int GroupGlobalId = -1;
			if(POSITION pos = UsersContactList.InitIteration())
			{
				for(int i=0; UsersContactList.GetNext(pos, pUser); i++)
				{
					// Step 1. Проверить создавали ли мы группу???
					CComBSTR	GroupName =	pUser->m_strType;
					
					BOOL isCheck = FALSE;//(m_strRecepientGroupName.CompareNoCase(pUser->m_strType)==0);
					
					if(pUser->m_iStatus==S_OFFLINE||pUser->m_iStatus==S_INVISIBLE)
					{
						GroupName	=  GetString(IDS_OFFLINE);
					}
					
					CComBSTR bsGroupPath = L"treeitem/treeitem[@text='";
					bsGroupPath += GroupName;
					bsGroupPath += L"']";
					
					CComPtr<IXMLDOMNode>	pGroupNode;
					
					pTreeItemDoc->selectSingleNode(bsGroupPath,&pGroupNode);
					
					if(pGroupNode==NULL)
					{
						CComPtr<IXMLDOMNode>	pTmpGroupNode;
						
						pTreeItemDoc->createNode(CComVariant(NODE_ELEMENT),CComBSTR(L"treeitem"),NULL,&pTmpGroupNode);
						
						SetAttribute(pTmpGroupNode,CComBSTR(L"id"),CComVariant(GroupGlobalId--));
						SetAttribute(pTmpGroupNode,CComBSTR(L"text"),CComVariant(GroupName));
						SetAttribute(pTmpGroupNode,CComBSTR(L"checked"),CComVariant("0"));
						
						//pRootNode->appendChild(pTmpGroupNode,&pGroupNode);
						pGroupNode = AppendWithSort(pRootNode, pTmpGroupNode, CComBSTR(L"text"));
					}
					
					// Step 3. добавить пользователя [1/28/2002]
					//pUser->TID = m_treectrl.AddItem(GroupTID,pUser->GetShowName(),m_ShablonId[pUser->m_bHasNewMessages+2]);								
					
					CComPtr<IXMLDOMNode>	pTmpUserNode, pUserNode;
					
					pTreeItemDoc->createNode(CComVariant(NODE_ELEMENT),CComBSTR(L"treeitem"),NULL,&pTmpUserNode);
					
					SetAttribute(pTmpUserNode,CComBSTR(L"id"),CComVariant(pUser->GetGlobalID()));
					SetAttribute(pTmpUserNode,CComBSTR(L"text"),CComVariant(pUser->GetShowName()));
					SetAttribute(pTmpUserNode,CComBSTR(L"checked"),CComVariant(pUser->m_bHasNewMessages?L"1":L"0"));
					
					//pGroupNode->appendChild(pTmpUserNode,&pUserNode);
					pUserNode = AppendWithSort(pGroupNode, pTmpUserNode, CComBSTR(L"text"));
					
					if(pUser->m_bHasNewMessages&&strGroupName!=NULL)
					{
						SetAttribute(pGroupNode,CComBSTR(L"checked"),CComVariant(L"1"));
					}
					
				}
			}
		}
		break;
	}


	CComBSTR bsRetVal;
	pTreeItemDoc->get_xml(&bsRetVal);
	//pTreeItemDoc->save(CComVariant(L"c:\\test.xml"));

	return bsRetVal;
}


void CMainDlg::CreateScreenShotUsers(long RecepientUserId, LPCTSTR strGroupName, CUserCollection &ContactList)
{
	GetCopyContactList(ContactList);
	
	// Step 2. Init Check or Uncheck Mode [2/6/2002]
	int CLMode  = GetOptionInt(IDS_OFSMESSENGER,IDS_CLMODE,2);
	
	switch(CLMode) 
	{
	case 1:
		{
			if(ContactList.InitIteration())
			{
				CUser* pUser=NULL;
				
				if(POSITION pos = ContactList.InitIteration())
				{
					for(int i=0; ContactList.GetNext(pos,pUser); i++)
					{
						if(m_bShowOffline||(pUser->m_iStatus!=S_OFFLINE&&pUser->m_iStatus!=S_INVISIBLE))
							pUser->m_bHasNewMessages  = (CString(strGroupName).CompareNoCase(pUser->m_strType)==0); 
						
						if(RecepientUserId==pUser->GetGlobalID())
							pUser->m_bHasNewMessages = TRUE;
					}
				}
			}
		}
		break;
	case 2:
		{
			if(ContactList.InitIteration())
			{
				CUser* pUser=NULL;
				
				if(POSITION pos = ContactList.InitIteration())
				{
					for(int i=0; ContactList.GetNext(pos,pUser); i++)
					{
						if(pUser->m_iStatus==S_OFFLINE||pUser->m_iStatus==S_INVISIBLE)
						{
							pUser->m_bHasNewMessages  = (CString(strGroupName).CompareNoCase(GetString(IDS_OFFLINE))==0);
						}
						else
							pUser->m_bHasNewMessages  = (CString(strGroupName).CompareNoCase(pUser->m_strType)==0); 
						
						if(RecepientUserId==pUser->GetGlobalID())
							pUser->m_bHasNewMessages = TRUE;
					}
				}
			}
		}
		break;
	}
}

void CMainDlg::MakeScreenShot(CScreenShotDlg::EDlgMode	mode,long RecepientUserId, LPCTSTR strGroupName )
{
	
	// Append McScreenCaptureApp [7/15/2004]
	/*try
	{
		IMcScreenCapturePtr	advancedCapture;
		advancedCapture.CreateInstance(CLSID_McScreenCapture);
		
		IMcScreenCaptureItemPtr	CaptureItem;
		advancedCapture->CreateCapture(-1,-1,(IDispatch**)&CaptureItem);
		
		CaptureItem->AddRadioButton(_bstr_t((LPCTSTR)GetString(IDS_CAPTURE_ACTION_SENDTO)));
		CaptureItem->AddRadioButton(_bstr_t((LPCTSTR)GetString(IDS_CAPTURE_ACTION_CREATEISSUE)));
		CaptureItem->AddRadioButton(_bstr_t((LPCTSTR)GetString(IDS_CAPTURE_ACTION_ASSIGNTODO)));
		CaptureItem->AddRadioButton(_bstr_t((LPCTSTR)GetString(IDS_CAPTURE_ACTION_PUBLISH)));
		
		// TODO: Set Default Select [7/19/2004]
		switch(mode)
		{
		case CScreenShotDlg::SendFile:
			CaptureItem->SetDefaultButton(0);
			break;
		case CScreenShotDlg::CreateIssue:
			CaptureItem->SetDefaultButton(1);
			break;
		case CScreenShotDlg::AssignToDo:
			CaptureItem->SetDefaultButton(2);
			break;
		case CScreenShotDlg::PublishToIBNLibrary:
			CaptureItem->SetDefaultButton(3);
			break;
		}
		
		// TODO: Load User Tree [7/19/2004]
		CComBSTR bsUserXML = CreateScreenShotUsersXML(RecepientUserId,strGroupName);
		
		CaptureItem->PutXML((BSTR)bsUserXML);
		
		//////////////////////////////////////////////////////////////////////////
		
		CString	strDataDirPath	=	GetAppDataDir();
		strDataDirPath	+=	_T("\\Screen Capture");
		
		CreateDirectory(strDataDirPath,NULL);
		
		CString strDefaultFileName;
		
		SYSTEMTIME	st =	{0};
		
		GetSystemTime(&st);
		
		strDefaultFileName.Format(_T("%s\\IBNSC-%d-%02d-%02d-%05X"), strDataDirPath, st.wYear, st.wMonth, st.wDay, st.wHour*60*60+st.wMinute*60+st.wSecond);
		
		CaptureItem->SetDefaultFileName((LPCTSTR)strDefaultFileName);
		
		// Start [7/19/2004]
		CaptureItem->Start();
		// Append To Active McScreenCapture Session List and wait [7/15/2004]
		
		McScreenCaptureItem	*pItem = new McScreenCaptureItem();
		
		pItem->Ptr =  CaptureItem;
		pItem->UserId = RecepientUserId;
		pItem->GroupName = strGroupName;
		
		m_ActiveExCaptureList.Add(pItem);
		return;
	}
	catch(_com_error&)
	{
	}*/

	CComBSTR bsCommandName;
	CComBSTR bsMode = L"Image";

	switch(mode)
		{
		case CScreenShotDlg::SendFile:
			bsCommandName = L"sendToUser";
			break;
		case CScreenShotDlg::CreateIssue:
			bsCommandName = L"createIncident";
			break;
		case CScreenShotDlg::AssignToDo:
			bsCommandName = L"assignTodo";
			break;
		case CScreenShotDlg::PublishToIBNLibrary:
			bsCommandName = L"publishToIbn";
			break;
		}

	CComBSTR bsRecipientsXml;
	
	if(strGroupName!=NULL)
	{
		bsRecipientsXml = CreateScreenShotUsersXML(strGroupName);
	}
	else if(RecepientUserId!=-1)
	{
		WCHAR wsTmpBuf[20];
		_ltow(RecepientUserId,wsTmpBuf,10);

		bsRecipientsXml += L"<id>";
		bsRecipientsXml += wsTmpBuf;
		bsRecipientsXml += L"</id>";
	}
	else
		bsRecipientsXml = L"";

	if(!m_IbnCommandWnd.SendScreenCaptureCommand(bsCommandName, bsMode, bsRecipientsXml))
	{
		CScreenShotDlg	*pScreenShotDlg	=	new CScreenShotDlg(this,mode);
		
		if(RecepientUserId!=-1)
			pScreenShotDlg->m_RecepientUserId = RecepientUserId;
		if(strGroupName!=NULL)
			pScreenShotDlg->m_strRecepientGroupName = strGroupName;
		
		pScreenShotDlg->Create(CScreenShotDlg::IDD,GetDesktopWindow());
		
		pScreenShotDlg->ShowWindow(SW_NORMAL);
		
		AddWindowToClose(pScreenShotDlg);
	}
}

void CMainDlg::OnToolsMenuScreenCaptureSend()
{
	MakeScreenShot(CScreenShotDlg::SendFile);
}

void CMainDlg::OnUpdateToolsMenuScreenCaptureSend(CCmdUI* pCmdUI)
{
}

void CMainDlg::OnToolsMenuScreenCaptureAssignToDo()
{
	MakeScreenShot(CScreenShotDlg::AssignToDo);
}

void CMainDlg::OnUpdateToolsMenuScreenCaptureAssignToDo(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(IsToolboxIstalled());
}

void CMainDlg::OnToolsMenuScreenCaptureCreateIssue()
{
	MakeScreenShot(CScreenShotDlg::CreateIssue);
}

void CMainDlg::OnUpdateToolsMenuScreenCaptureCreateIssue(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(IsToolboxIstalled());
}

void CMainDlg::OnToolsMenuMessageHistory()
{
	////// Показ Редактора выбора Типа Синхронизации ...
	CUser *pUser = FindUserInVisualContactList(CurrTID);
	if(pUser)
		m_HistoryDlg.ShowHistory(*pUser);
	else
		m_HistoryDlg.ShowHistory();
}

void CMainDlg::OnUpdateToolsMenuMessageHistory(CCmdUI* pCmdUI)
{
}

void CMainDlg::OnToolsMenuFileManage()
{
	m_FileManager.ShowDialog();
}

void CMainDlg::OnUpdateToolsMenuFileManage(CCmdUI* pCmdUI)
{
}

void CMainDlg::OnToolsMenuIbnToolbox()
{
	CString strParametrs;
	strParametrs.Format(_T("/L \"%s\" /P \"%s\""),m_DlgLog.m_LoginStr, m_DlgLog.m_PasswordStr);
	
	if(m_bIsSSLMode)
		strParametrs	+= _T(" /USESSL");
	
	ShellExecute(::GetDesktopWindow(),NULL,ToolboxPath(),strParametrs,NULL,SW_SHOWNORMAL);
}

void CMainDlg::OnUpdateToolsMenuIbnToolbox(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(IsToolboxIstalled());
}
void CMainDlg::OnToolsMenuNetworkMonitor()
{
	if(IsWindow(m_hWndNetMonitor))
	{
		CMcWindowAgent winAgent(m_hWndNetMonitor);
		winAgent.ShowWindow(SW_SHOWNORMAL);
		winAgent.SetForegroundWindow();
	}
	else
	{
		CMonitorDialog	*pMonitorDlg = new CMonitorDialog(this);
		pMonitorDlg->Create(CMonitorDialog::IDD, GetDesktopWindow());
		pMonitorDlg->ShowWindow(SW_SHOWNORMAL);
		
		m_hWndNetMonitor = pMonitorDlg->GetSafeHwnd();
		AddToAllCloseWindow(m_hWndNetMonitor);
	}
}
void CMainDlg::OnUpdateToolsMenuNetworkMonitor(CCmdUI* pCmdUI)
{
}


void CMainDlg::CheckAutoUpdate()
{
#if RADIUS
	return;
#else
	// Step 1. Get Last Check Time
	time_t intLastCheckTime = (time_t)GetOptionInt(IDS_OFSMESSENGER,IDS_UPDATE_AUTOCHECK_TIME, 0);
	CTime lastUpdateTime(intLastCheckTime);

	CTime currentTime = CTime::GetCurrentTime();

	CTimeSpan timeSpan = currentTime - lastUpdateTime;

	if(timeSpan.GetDays() > 1)
	{
		// Update Last Check Time
		WriteOptionInt(IDS_OFSMESSENGER,IDS_UPDATE_AUTOCHECK_TIME, (int)currentTime.GetTime());

		// Execute UpdateCheck.exe
		CMcVersionInfo	verInfo;
		int getCurrentBuild = ((verInfo.GetProductVersionLS()&0xffff0000)>>16);

		CString edition = "EN";

		if(GetProductLanguage()==CString(_T("1049")))
			edition = _T("RU");

		CString strUpdateCheckParams;
		strUpdateCheckParams.Format(_T("/check {EBD238CD-22D0-437C-A0E1-A53D4E661ABB} \"Client Tools\" %d %s"), 
			getCurrentBuild, edition);

		CString strCheckPath = GetProductPath();
		strCheckPath += _T("Check.exe");

		ShellExecute(NULL, NULL, strCheckPath, strUpdateCheckParams, NULL, SW_SHOW);
	}

#endif
}