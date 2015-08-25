//{{AFX_INCLUDES()
#include "mcbutton.h"
#include "webbrowser2.h"
//}}AFX_INCLUDES
#if !defined(AFX_MAINDLG_H__3482F9DE_56C8_405F_BF73_2F2211246458__INCLUDED_)
#define AFX_MAINDLG_H__3482F9DE_56C8_405F_BF73_2F2211246458__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// MainDlg.h : header file
//
#include "ofstv.h"
#include "OFSNcDlg2.h"

#include "SystemTray.h"
#include "LoginDlg.h"
#include "MessageDlg.h"
#include "CoolMenu.h"

#include "FileDownloader.h"

#include "Inet/NetLibTranslator.h"
#include "ResizableImage.h"
#include "ccootree.h"
#include "UserCollection.h"
#include "Message.h"
#include "MessageSplitDlg3.h"
#include "McButton.h"
#include "UserDetailsDlg.h"
#include "HistoryDlg.h"
#include "McProgress.h"

#include "XmlDoc.h"
#include "McWindowDetails.h"

#include "DelUserDlg.h"
#include "AddUserRequest.h"
#include "user.h"	// Added by ClassView

#include "WebWindow.h"
#include "McAppBar.h"

#include "FileManagerDlg.h"

#include "GroupMessageSendDlg.h"

#include "mcmessengerhost.h"

#include "Label.h"

#include "McLogo.h"

#include "Chat.h"

#include "ChatDlg.h"

#include "ScreenShotDlg.h"

#include "IbnCommandWnd.h"

//#include "McWebFolderView.h"

/////////////////////////////////////////////////////////////////////////////
// CMainDlg dialog

enum AutoStatusState
{
	ASS_None	=	0,
	ASS_Away	=	S_AWAY,
	ASS_NA		=	S_NA
};

class CAutoStatus 
{
public:
	CAutoStatus()
	{
		Reset();
	}

	AutoStatusState		State;
	int					OldUserStatus;
		
	void Reset()
	{
		State			=	ASS_None;
		OldUserStatus	=	0;
	}

	void	SetAway(int	UserStatus)
	{
		OldUserStatus = UserStatus;
		State	=	ASS_Away;
	}

	void	SetNA(int	UserStatus)
	{
		if(State==ASS_None)
			OldUserStatus = UserStatus;
		State	=	ASS_NA;
	}

	BOOL IsReset()
	{
		return (State==ASS_None);
	}

	BOOL IsAutoAway()
	{
		return (State==ASS_Away);
	}

	BOOL IsAutoNA()
	{
		return (State==ASS_NA);
	}
};


typedef struct _McScreenCaptureItem
{
	IMcScreenCaptureItemPtr	Ptr;
	CString					GroupName;
	int						UserId;
} 
McScreenCaptureItem, *LPMcScreenCaptureItem;


typedef struct _SendFileInfo
{
	CString File; 
	CString RecepientName; 
	CString RecepientID;
	CString Description;
} 
SendFileInfo, *LPSendFileInfo;


class CMainDlg : public COFSNcDlg2
{
public:
	class COleMainDropTarget : public COleDropTarget
	{
	public:
		COleMainDropTarget(CMainDlg* pFE = NULL):m_pMain(pFE)
		{
			m_dropEffectCurrent = DROPEFFECT_NONE;
			m_dwKeyState	=	0;
		};
		// Overrides
		DROPEFFECT OnDragEnter( CWnd* pWnd, COleDataObject* pDataObject, DWORD dwKeyState, CPoint point );
		DROPEFFECT OnDragOver(CWnd* pWnd, COleDataObject* pDataObject, DWORD dwKeyState, CPoint point );
		void OnDragLeave(CWnd* pWnd);               
		BOOL OnDrop( CWnd* pWnd, COleDataObject* pDataObject, DROPEFFECT dropEffect, CPoint point );
	private:
		DWORD			m_dwKeyState;
		DROPEFFECT		m_dropEffectCurrent;
		CMainDlg*		m_pMain;
	};
	
	friend COleMainDropTarget;

public:
	enum _HANDLE_COMMAND_ENUM
	{
		HCI_NONE				=		0,
		HCI_OPEN_CHAT_WINDOW	=		1,
	};
// Construction	
public:
	BOOL Create(CWnd *pParentWnd);

	CMainDlg(CWnd* pParent = NULL);   // standard constructor
	~CMainDlg();
// Dialog Data
	//{{AFX_DATA(CMainDlg)
	enum { IDD = IDD_MAIN };
	CMcButton	m_btnX;
	CMcButton	m_btnMin;
	CMcButton	m_btnMenu;
	CWebWindow	m_InWindow;
	CLabel		m_StatusStatic;
	CLabel		m_UserStatic;
	CMcButton	m_btnAlerts;
	CMcButton	m_btnApps;
	CMcButton	m_btnFiles;
	CMcButton	m_btnDirectory;
	CMcButton	m_btnShowOffline;
	//}}AFX_DATA

	CLoginDlg			m_DlgLog;
	CIbnCommandWnd		m_IbnCommandWnd;
public:
	CUser& GetCurrentUser();
	void		CreateScreenShotUsers(long RecepientUserId, LPCTSTR strGroupName, CUserCollection &ContactList);
	CComBSTR	CreateScreenShotUsersXML(long RecepientUserId, LPCTSTR strGroupName);
	CComBSTR	CreateScreenShotUsersXML(LPCTSTR strGroupName);
	CString		GetSelectedUserStringFromXML(BSTR bsXML);

	CString ChangeUrlForCurrentDomain(LPCTSTR Url);
	void AddIbnToMessage(IBNTO_MESSAGE *pmsg);
	void AddSendToMessage(SENDTO_MESSAGE *pmsg);
	CString GetUserDomain();
	BOOL ShowChatDlg(BSTR bsChatId, BOOL bMinMode = FALSE);
	BOOL SetChatStatus(BSTR bsChatId, LONG NewStatus, LONG Command = HCI_NONE);
	BOOL SendLightNewMessage(IMessage *pNewMessage);
	void AddToAllCloseWindow(HWND hWnd);
	void UpdateUserMenu(long GlobalId, CMenu *pMenu);
	void DeleteChat(BSTR bsChatId);
	BOOL FindChatByGlobalId(BSTR bsId, CChat &OutChat);
	BOOL AddNewChat(IChat *pChat, LONG Status);
	void	Login();
	int		GetLastUserStatus();
	void	LoadOptions();
	long	ResetAutoChangeStatusTime();
	void	Invoke_StartAutoUpdate(LPCTSTR strUpdateFileUrl);
	void	SetInetOption();
	
	HICON				hIcon;
	HICON				m_hMessengerTrayIcon;
	CMessageDlg			m_MessageBox;
	CSystemTray			m_MessengerTray;

	BOOL				m_bSilentMode;
	//////////////////////////////////////////////////////////////////////////
	// CMessenger Part [4/5/2002]
	BOOL	UploadAppFile(LPCTSTR strXML);
	BOOL	ShowWebWindow(LPCTSTR Url, CComVariant*	pFileMas	=	NULL);
	LPDISPATCH GetInWindowDocument();
	//void	SetUIHandler(BOOL bSet);
	BOOL	LoadUserProfile();
	CString GetUserRole();
	int		GetUserCurrentStatus(long UserGlobalId);
	void	RemoveAllMessageById(long UserId);
	IComHistIntPtr& GetLocalHistory();
	void	OnAppItem(UINT Id);
	void	ShowGeneralMenu(LONG ChatTID	=	-1);
	void	ShowUserDetails(long UserGlobalID);
	void	ShowError(long Type, long Code);
	CString GetSID();
	void	LoadAppList(IXMLDOMDocument *xmlDoc, BOOL bRestoryOld = FALSE);
	void	CancelLogon();
	BOOL	CheckUserInContactList(CUser& User);
	BOOL	CheckUserInContactList(long GlobalID);
	BOOL	GetUserByGlobalId(long UserId, CUser &User);
	void	AddUserToContactList(CUser& user);
	void	SendFileToUser(CUser *pUser, LPCTSTR FilePath, LPCTSTR strDescription);
	void	GetCopyContactList(CUserCollection& ContactList);
	void	GetCopyContactList(CUserCollection& ContactList, BOOL bIncludeSystemUsers);
	void	ShowUserMenu(long GlobalId);
	BOOL	ShowHistoryForUser(CUser& user);
	const CString& GetServerPath();
	BOOL	IsLockShowInfo();
	void	UnlockShowInfo();
	void	LockShowInfo();
	void	PreferenceDlg(CWnd *pParent);
	void	LoadFloating();
	void	SaveFloating();
	CMainDlg* GetMessageParent();
	BOOL	MarkMessagesAsRead(bstr_t &MID);
	BOOL	AddMessageToDataBase(long ToId, bstr_t &MID, long Time,bstr_t &Body);
	BOOL	GetMessagesBySID( bstr_t &FriendName, long FriendID, long Sorted, long Type, IUnknown *pPersist, LPCTSTR XSLTPath = _T("/Browser/xslt/mpa_messages.xslt"));
	BOOL	GetMessages(bstr_t &FriendName, long FriendID, long Sorted, long Type, IUnknown *pPersist);
	BOOL	CloseLocalDataBase();
	BOOL	LocalDataBaseEnable();
	BOOL	InitLocalDataBase(LPCTSTR path,BOOL bReindex	=	FALSE);
	void	Refresh();
	CString GetShowName();
	long	GetRoleID();
	long	GetUserID();
	int		GetUserStatus();
	BOOL	ConnectEnable(BOOL bShowMessage = TRUE);
	void	LogOff();
	void	Show(BOOL bShow = TRUE);
	void	Login2(int SetStatus = S_ONLINE);
	void	DeleteFromContact(long GlobalID);
	void	SendMessageToUser(CUser *pUser,BOOL bInAnswer = FALSE, LPCTSTR strBody = NULL, BOOL SendAuto = FALSE);

	CString GetWebHOST();

	BOOL IsToolboxIstalled();
	const CString ToolboxPath();
	const CString ScreenCapturePath();

	void	AddToUpload(CString FileName, CString Login, CString RecepientID, LPCTSTR strDescription);
	
	BOOL	IsSSLMode();
	
protected:
	void	CheckAutoUpdate();
	void	ProcessSendtoMessages();
	void	ProcessIbntoMessages();
	void	CreateMessageDialog(bstr_t &Path);
	void	UnpackNewPromo();
	void	CheckExit();
	void	CheckAllStartUpWasLoad();
	void	StartStartUpLoad();
	void	UpdateStatus(int Status,CCmdUI* pCmdUI);
	int		CreateTray();
	int		CreateBar();

	CString				strNowSkinName;
	//CString				strUserRole;
	CString				m_strMcUpdatePath; 
	CString				m_strMcUpdateParam;

	BOOL				m_bActive;
	BOOL				m_bNowMcUpdateDownload;

	COLORREF			rgbFON;

	W_NETSTATUS			BeforStatusInetStatus;

	DWORD				dwStartUpInfo;
	DWORD				dwExitInfo;
	long				m_AutoChangeStatusTimeLast;

	CBitmap				m_bmpStatus;

	CCoolMenuManager	m_CoolMenuManager;
	///CFileDownloader		m_McUpdateDownload;
	HWND				m_McUpdateWnd;
	CImageList			m_StatusImageList;
	
	
	CPoint				m_oldCursorPos;
	int					m_OldStatus;
	int					m_LastUserStatus;
	BOOL				m_bAutoUpdateExit;
	HWND				m_hWndAddUserDialog;
	HWND				m_hWndNetMonitor;

	CXmlDoc				*m_pAppList;
	//////////////////////////////////////////////////////////////////////////
	// CMessenger Part [4/5/2002]
	CArray <bstr_t,bstr_t> m_LoadedSid;

	CString				m_strGroupName;

	CSaveDataBase		*pSaverDB;
	
	IlocalSIDsPtr		m_SIDsHistory;
	IComHistIntPtr		m_LocalHistory;

	BOOL				bEnableLocalHistory;   
	
	CSize				m_FonSize;   

	CCCooTree			m_treebox;
	CCCooTree			m_chatbox;

	CFileDownloadDlg	m_FileDownloadStatus;
	CFileUploadDlg		m_FileUploadStatus;
    CUserDetailsDlg		m_UserDetails;
    CHistoryDlg			m_HistoryDlg;
	CResizableImage		m_ResizeFon;
	CMcLogo				m_picLogo;

	BOOL				m_bUpdateUserStatus;
	long				m_lLogonHandle;
	LONG				SynchronizeteHandle;
	CUser				m_User;
	ISessionPtr			pSession;
	CString				m_strPassword;
	CUserCollection		m_ContactList;
	CUserCollection		m_ExternalContactList;
	DWORD				dwCurrentStatus;
	BOOL				bRepairConect;  
	long				CurrTID;//,FriendTID,HelpTID;  
	// Fot Chat Addon [8/7/2002]
	long				CurrChatTID;
	BOOL				m_bShowOnlyOnline;
	BOOL				bExitAfterDiskonect;
	DWORD				MessageSendMode;
	int					MesPotokLen; 
	CString				m_strServerPath;
	CString				m_strServerPathOnDomain;
	BOOL				m_bUserDomainMode;

	long				m_lPort;
	CString				m_strPath;
	CString				m_strServer;
	BOOL				m_bIsSSLMode;
	
	CFileManagerDlg		m_FileManager;

	//CMcAppCtrl			m_AppCtrl;
	CMcAppBar			m_AppBar;
	CString				m_strLogoVersion;
	BOOL				m_bUpdateDlgWasShow;
	CPictureHolder		m_picIM;
	//CPictureHolder		m_picDOCS;
	//CPictureHolder		m_picLINKS;
	//CPictureHolder		m_picNEWS;
	//CPictureHolder		m_picWF;
	CPictureHolder		m_picCC;
	CPictureHolder		m_picIA;
	
	CMapStringToPtr		m_GroupTIDMap;
	// Chat Addon [8/7/2002]
	CMapStringToPtr		m_ChatGroupTIDMap;
	
	//CMcMessengerDropTarget		*m_InWindowDropTarget;
	//CMcImpIDocHostUIHandler		*m_pMcHost;
	
	/////////////////////////////////////////
	CArray<NLT_Container*,NLT_Container*>				m_SaveEventArray;
    CArray<CMessage *,CMessage*>						m_NewMessageArray;
	CMap <long,long,HWND,HWND>							m_SendMessageDlgMap;
	CMap <long,long,HWND,HWND>							m_ComeMessageDlgMap;
	CMap <long,long,HWND,HWND>							m_SplitMessageDlgMap;
	CMap <long,long,HWND,HWND>							m_DelUserDlgMap;
	CMap <long,long,HWND,HWND>							m_AddUserDlgMap;

	CArray<McAppItem,McAppItem>		                    m_AppArray;
	CArray<HWND,HWND>									m_AllClosedWindow;

	CArray<CChat,CChat>									m_ChatCollection;

	CMap <long,long,CString,CString>					m_ChatHandleMap;

	CMap <long,long,long,long>							m_HandleCommandMap;

	CArray<BSTR,BSTR>									m_ActiveChatRem;

	CArray<LPMcScreenCaptureItem,LPMcScreenCaptureItem>			m_ActiveExCaptureList;
	CCriticalSection									m_MainDlgLock;

	CMap<LONG,LONG,IMessage*,IMessage*>					m_SendMessagesMap;
	
	///CMcWebFolderView									m_WebFolderView;
	
	CMapStringToPtr										m_InviteChatWndMap;
	CArray<IBNTO_MESSAGE*, IBNTO_MESSAGE*>				m_IbntoMessages;
	CArray<SENDTO_MESSAGE*, SENDTO_MESSAGE*>			m_SendtoMessages;

	// Refactoring CAutoStatus [3/18/2004]
	CAutoStatus											m_AutoStatus;

	HGLOBAL												m_hRightButtonDDGlobal;
	BOOL												m_bRightButtonDDGroupe;
	LONG												m_bRightButtonDDTID;

	COleMainDropTarget									m_OleMainDropTarget;

	BOOL m_bShowOffline;
// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CMainDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	///virtual BOOL PreTranslateMessage(MSG* pMsg);
	//}}AFX_VIRTUAL

// Implementation
protected:
	void LoadSkin(IXMLDOMNode *pXmlRoot);
	void ShowIBNActions();

protected:
	//void SortScreenShotUsersXML(IXMLDOMNodeList* treeItemNodes);
	void MakeScreenShot(CScreenShotDlg::EDlgMode,long RecepientUserId = -1, LPCTSTR strGroupName = NULL);
	BOOL LoadConferenceListForCurrUser(CMenu* pInviteToMenu);
	BOOL ShowChatDlg(CChat* pChat, BOOL bMinMode);
	BOOL LoadNotSendedMessage();
	void ChatAccept(IChat* pChat, IUser* pUser, long Result);
	void LeaveUserFromChat(IChat* pChat, IUser *pUser);
	void InviteNewUserInChat(IChat *pChat, IUser *pUser, IUser *pInvitedUser, BSTR bsInviteMessage);
	void NewChatMessage(IChat* pChat, IMessage *pMessage);
	void UpdateUserChatStatus(IChat* pChat, IUser* pUser);
	
	BOOL FindChatByTID(long TID, CChat &OutChat);
	void DisconnectAllChats();
	void BuildChatsList();
	void LoadChats(IChats *pChats);
	void CreateChatTree();
	BOOL LoadAppBar (IXMLDOMNode *pXmlRoot);
	//BOOL	m_bCatchNavigate;
	BOOL	bShowInfo;
	
	BOOL	UserIsVisible(long UserGlobalId);
	void	RefreshUserInfo(long ID);
	BOOL	LoadNotReadMessage();
	void	RefreshHistoryFor(long Id);
	void	SendFileToUser(CUser *pUser);
	CMessage* FindMessageByIDAndDel(long Id);
	CMessage* FindMessageByID(long ID);
	void NewMessage(CMessage *pMsg);
	CUser* FindUserInVisualContactList(long TID);
	CUser* FindUserInVisualContactListByGlobalId(long UserId);
	void	CreateTree();
	void	ChangeUserStatus(IUser *&pUser);
	void	UpdateID(long UserId,BOOL bHasNewMessage );
	void	LoadUsers(IUsers *pUsers);
	void	BuildContactList();
	void	BuildContactListToUser(long GlobalID);
	
	BOOL	ConfigureNetwork();
	
	BOOL	PopupLoginAndPassword();
	void	ClearAll();
	
	// Generated message map functions
	//{{AFX_MSG(CMainDlg)
	afx_msg void OnTreegroupConferenceCreateNew();
	afx_msg void OnUpdateTreegroupConferenceCreateNew(CCmdUI* pCmdUI);
	afx_msg void OnTreemenuConferenceCreateNew();
	afx_msg void OnUpdateTreemenuConferenceCreateNew(CCmdUI* pCmdUI);
	afx_msg void OnOptionsConferenceCreateNew();
	afx_msg void OnUpdateOptionsConferenceCreateNew(CCmdUI* pCmdUI);
	afx_msg void OnChatViewDetails();
	afx_msg void OnChatMessageHistory();
	afx_msg void OnChatLeave();
	afx_msg void OnChatEditDetails();
	afx_msg void OnChatAddaFriends();
	afx_msg void OnChatStatusInactive();
	afx_msg void OnChatStatusActive();
	afx_msg void OnChatAttach();
	afx_msg void OnChatSendMessage();
	afx_msg void OnUpdateChatViewDetails(CCmdUI* pCmdUI);
	afx_msg void OnUpdateChatMessageHistory(CCmdUI* pCmdUI);
	afx_msg void OnUpdateChatLeave(CCmdUI* pCmdUI);
	afx_msg void OnUpdateChatEditDetails(CCmdUI* pCmdUI);
	afx_msg void OnUpdateChatAddaFriends(CCmdUI* pCmdUI);
	afx_msg void OnUpdateChatStatusInactive(CCmdUI* pCmdUI);
	afx_msg void OnUpdateChatStatusActive(CCmdUI* pCmdUI);
	afx_msg void OnUpdateChatAttach(CCmdUI* pCmdUI);
	afx_msg void OnUpdateChatSendMessage(CCmdUI* pCmdUI);
	afx_msg void OnChatDoDropTreectrl(long TID, BOOL bGroupe,LPUNKNOWN pDataObject, long DropEffect, long LastKeyState);
	afx_msg void OnChatMenuTreectrl(long TID, BOOL bGroupe);
	afx_msg void OnChatSelectTreectrl(long TID, BOOL bGroupe);
	afx_msg void OnChatActionTreectrl(long TID, BOOL bGroupe);
	afx_msg void OnActivate(UINT nState, CWnd* pWndOther, BOOL bMinimized);
	virtual void OnOK();
	virtual void OnCancel();
	virtual BOOL OnInitDialog();
	afx_msg void OnClickButtonMin();
	afx_msg void OnNetworkMonitor();
	afx_msg void OnUpdateNetworkMonitor(CCmdUI* pCmdUI);
	afx_msg void OnClickButtonX();
	afx_msg void OnClickButtonMenu();
	afx_msg void OnTrayShowNewMessage();
	afx_msg void OnUpdateTrayShowNewMessage(CCmdUI* pCmdUI);
	afx_msg void OnUpdateTrayMystatusOnline(CCmdUI* pCmdUI);
	afx_msg void OnUpdateTrayMystatusOffline(CCmdUI* pCmdUI);
	afx_msg void OnTrayMystatusOnline();
	afx_msg void OnTrayMystatusOffline();
	afx_msg void OnStatusAway();
	afx_msg void OnUpdateStatusAway(CCmdUI* pCmdUI);
	afx_msg void OnStatusDnd();
	afx_msg void OnUpdateStatusDnd(CCmdUI* pCmdUI);
	afx_msg void OnStatusInvisible();
	afx_msg void OnUpdateStatusInvisible(CCmdUI* pCmdUI);
	afx_msg void OnStatusNa();
	afx_msg void OnUpdateStatusNa(CCmdUI* pCmdUI);
	afx_msg void OnStatusOnline();
	afx_msg void OnUpdateStatusOnline(CCmdUI* pCmdUI);
	afx_msg void OnClose();
	afx_msg void OnTrayExit();
	afx_msg void OnTrayPreferences();
	afx_msg void OnTrayReportBug();
	afx_msg void OnStatusOffline();
	afx_msg void OnUpdateStatusOffline(CCmdUI* pCmdUI);
	afx_msg void OnCaptureChanged(CWnd *pWnd);
	afx_msg void OnTimer(UINT nIDEvent);
	afx_msg BOOL OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message);
	afx_msg void OnDnlClkTray();
	afx_msg void OnLClkTray();
	afx_msg void OnDoDropTreectrl(long TID, BOOL bGroupe,LPUNKNOWN pDataObject, long DropEffect, long LastKeyState);
	afx_msg void OnOptionsPreferences();
	afx_msg void OnMenuTreectrl(long TID, BOOL bGroupe);
	afx_msg void OnSelectTreectrl(long TID, BOOL bGroupe);
	afx_msg void OnActionTreectrl(long TID, BOOL bGroupe);
	afx_msg void OnUpdateTreemenuSendmessage(CCmdUI* pCmdUI);
	afx_msg void OnTreemenuSendfile();
	afx_msg void OnTreemenuSendmessage();
	afx_msg void OnUpdateTreemenuSendfile(CCmdUI* pCmdUI);
	afx_msg void OnTreegroupAssignToDo();
	afx_msg void OnUpdateTreegroupAssignToDo(CCmdUI* pCmdUI);
	afx_msg void OnTreemenuAssignToDo();
	afx_msg void OnUpdateTreemenuAssignToDo(CCmdUI* pCmdUI);

	afx_msg void OnLButtonDblClk(UINT nFlags, CPoint point);
	afx_msg void OnTreemenuDeleteuser();
	afx_msg void OnUpdateTreemenuDeleteuser(CCmdUI* pCmdUI);
	afx_msg void OnTreemenuFloatingon();
	afx_msg void OnUpdateTreemenuFloatingon(CCmdUI* pCmdUI);
	afx_msg void OnTreemenuMessageshistory();
	afx_msg void OnUpdateTreemenuMessageshistory(CCmdUI* pCmdUI);
	afx_msg void OnTreemenuUserdetails();
	afx_msg void OnUpdateTreemenuUserdetails(CCmdUI* pCmdUI);
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
	afx_msg void OnOptionsSynchronizehistory();
	afx_msg void OnUpdateOptionsSynchronizehistory(CCmdUI* pCmdUI);
	afx_msg void OnOptionsAddiniviteFinduseraddtofriends();
	afx_msg void OnOptionsFilemanagerDownload();
	afx_msg void OnOptionsFilemanagerUpload();
	afx_msg void OnOptionsViewmydetails();
	afx_msg void OnUpdateOptionsViewmydetails(CCmdUI* pCmdUI);
	afx_msg void OnTreegroupSendfile();
	afx_msg void OnTreegroupSendmessage();
	afx_msg void OnUpdateTreegroupSendfile(CCmdUI* pCmdUI);
	afx_msg void OnUpdateTreegroupSendmessage(CCmdUI* pCmdUI);
	afx_msg void OnTreemenuSendemail();
	afx_msg void OnUpdateTreemenuSendemail(CCmdUI* pCmdUI);
	afx_msg void OnTreegroupSendemail();
	afx_msg void OnUpdateTreegroupSendemail(CCmdUI* pCmdUI);
	afx_msg void OnDestroy();
	afx_msg void OnSize(UINT nType, int cx, int cy) ;
	afx_msg void OnPaint();
	afx_msg void OnClickButtonAlerts();
	afx_msg void OnClickButtonApps();
	afx_msg void OnClickButtonFiles();
	afx_msg void OnClickButtonDirectory();
	afx_msg void OnClickButtonShowOfflineUser();
	afx_msg void OnUpdateOptionsLogOff(CCmdUI* pCmdUI);
	afx_msg void OnOptionsLogOff();
	afx_msg void OnSetFocus( CWnd* pOldWnd);
	afx_msg LRESULT OnUploadAppFile(WPARAM w, LPARAM l);
	afx_msg LRESULT OnLoadAppComleted(WPARAM w, LPARAM l);
	afx_msg LRESULT OnCancelLogin(WPARAM w, LPARAM l);
	afx_msg LRESULT OnUpdateContactList(WPARAM w, LPARAM l);
	afx_msg LRESULT OnInvokeStartAutoUploader(WPARAM w, LPARAM l);
	afx_msg LRESULT OnInvokeCreateChat(WPARAM w, LPARAM l);
	afx_msg LRESULT OnInvokeSendMessage(WPARAM w, LPARAM l);
	afx_msg LRESULT OnMcUpdateLoaded(WPARAM w, LPARAM l);
	afx_msg LRESULT OnUpdateExit(WPARAM w, LPARAM l);
	afx_msg LRESULT OnChangeNewMessage(WPARAM w, LPARAM l);
	afx_msg LRESULT OnShowAddUser(WPARAM w, LPARAM l);
	afx_msg LRESULT OnShowMessageBox(WPARAM w, LPARAM l);
	afx_msg LRESULT OnInetLogin( WPARAM w, LPARAM l);
	afx_msg LRESULT OnSetText(WPARAM w, LPARAM l);
	afx_msg LRESULT OnUpdateStatus(WPARAM w, LPARAM l);
	afx_msg LRESULT OnShowLoginDlg(WPARAM w, LPARAM l);
	afx_msg LRESULT OnSendGroupFile(WPARAM w, LPARAM l);
	afx_msg LRESULT OnKillSplitMesageDlg(WPARAM w, LPARAM l);
	afx_msg LRESULT OnCheckSignalState(WPARAM w, LPARAM l);
	afx_msg LRESULT OnDhtmlEvent(WPARAM w, LPARAM l);
	afx_msg LRESULT OnChangeStatus(WPARAM w, LPARAM l);
	afx_msg LRESULT OnLoadLastVersionId(WPARAM w,LPARAM l);
	afx_msg LRESULT OnKillComeMessageDlg(WPARAM w, LPARAM l);
	afx_msg LRESULT OnKillSendMessageDlg(WPARAM w, LPARAM l);
	afx_msg LRESULT OnKillDelUserMessageDlg(WPARAM w, LPARAM l);
	afx_msg LRESULT OnKillAddUserMessageDlg(WPARAM w, LPARAM l);
	afx_msg LRESULT OnKillInFileDlg(WPARAM w, LPARAM l);
	afx_msg LRESULT OnNetEvent(WPARAM w,LPARAM l);
	afx_msg LRESULT OnAlertPopupClk(WPARAM w, LPARAM l);
	afx_msg LRESULT	OnSendFile(WPARAM w, LPARAM l);
	afx_msg LRESULT	OnSendFile2(WPARAM w, LPARAM l);
	afx_msg BOOL OnQueryEndSession( );
	afx_msg void OnEndSession(BOOL bEnding );
	afx_msg void OnInviteUserToConference(UINT Id);
	afx_msg void OnSendGroupMessageTemplate(UINT Id);
	afx_msg void OnSendMessageTemplate(UINT Id);
	afx_msg void OnInviteGroupToConference(UINT Id);
	afx_msg LRESULT OnIBNToMessage(WPARAM w, LPARAM l);
	afx_msg LRESULT OnProcessCommandLineMessages(WPARAM w, LPARAM l);
	afx_msg void OnDropFiles( HDROP hDrop);

	afx_msg void OnRightButtonAssignToDo();
	afx_msg void OnUpdateRightButtonAssignToDo(CCmdUI* pCmdUI);
	afx_msg void OnRightButtonSendFile();
	afx_msg void OnUpdateRightButtonSendFile(CCmdUI* pCmdUI);
	afx_msg void OnRightButtonUploadFile();
	afx_msg void OnUpdateRightButtonUploadFile(CCmdUI* pCmdUI);
	afx_msg void OnRightButtonCancel();
	afx_msg void OnUpdateRightButtonCancel(CCmdUI* pCmdUI);
	afx_msg void OnRightButtonCreateIssue();
	afx_msg void OnUpdateRightButtonCreateIssue(CCmdUI* pCmdUI);

	afx_msg void OnTreeMenuSendScreenShot();
	afx_msg void OnUpdateTreeMenuSendScreenShot(CCmdUI* pCmdUI);
	afx_msg void OnTreeMenuScreenShotAssignToDo();
	afx_msg void OnUpdateTreeMenuScreenShotAssignToDo(CCmdUI* pCmdUI);

	afx_msg void OnTreeGroupSendScreenShot();
	afx_msg void OnUpdateTreeGroupSendScreenShot(CCmdUI* pCmdUI);
	afx_msg void OnTreeGroupScreenShotAssignToDo();
	afx_msg void OnUpdateTreeGroupScreenShotAssignToDo(CCmdUI* pCmdUI);
	
	afx_msg void OnTreeMenuCreateEvent();
	afx_msg void OnUpdateTreeMenuCreateEvent(CCmdUI* pCmdUI);
		
	afx_msg void OnTreeGroupCreateEvent();
	afx_msg void OnUpdateTreeGroupCreateEvent(CCmdUI* pCmdUI);

	afx_msg void OnToolsMenuScreenCaptureSend();
	afx_msg void OnUpdateToolsMenuScreenCaptureSend(CCmdUI* pCmdUI);
	afx_msg void OnToolsMenuScreenCaptureAssignToDo();
	afx_msg void OnUpdateToolsMenuScreenCaptureAssignToDo(CCmdUI* pCmdUI);
	afx_msg void OnToolsMenuScreenCaptureCreateIssue();
	afx_msg void OnUpdateToolsMenuScreenCaptureCreateIssue(CCmdUI* pCmdUI);

	afx_msg void OnToolsMenuMessageHistory();
	afx_msg void OnUpdateToolsMenuMessageHistory(CCmdUI* pCmdUI);
	afx_msg void OnToolsMenuFileManage();
	afx_msg void OnUpdateToolsMenuFileManage(CCmdUI* pCmdUI);
	afx_msg void OnToolsMenuIbnToolbox();
	afx_msg void OnUpdateToolsMenuIbnToolbox(CCmdUI* pCmdUI);
	afx_msg void OnToolsMenuNetworkMonitor();
	afx_msg void OnUpdateToolsMenuNetworkMonitor(CCmdUI* pCmdUI);
		
	DECLARE_EVENTSINK_MAP()
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_MAINDLG_H__3482F9DE_56C8_405F_BF73_2F2211246458__INCLUDED_)
