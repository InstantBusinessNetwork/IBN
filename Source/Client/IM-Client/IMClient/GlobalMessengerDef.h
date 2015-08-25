#ifndef _GLOBAL_MESSENGER_DEF_H
#define _GLOBAL_MESSENGER_DEF_H

#define ONLINE_SUPPORT_ID (1L)

//////////////////////////////////////////////////////////////////////////
//  [3/28/2002]
//#define _MC_GET_BUILD_COMMAND	L"{371C39D2-3BA3-42d2-8B25-FDE2D7F633BE}"
//#define _MC_GET_IP_COMMAND	L"{AABDE68F-4284-417f-B80E-3967A6A299C9}"

//////////////////////////////////////////////////////////////////////////

typedef enum  
{
	fcRemindLater = 1,
	fcDelete,
	fcDoOffline

}	fcFile;


enum M_STATUS
{
	S_OFFLINE=0,
	S_ONLINE,
	S_INVISIBLE,
	S_DND,          // не беспокоить
	S_AWAY,         // нету
	S_NA,           //
	S_OCCUPIED,     
	S_AWAITING,
	S_UNKNOWN,      // 
	S_WEBAPP,
	S_NEW_MESSAGE,  //must be the last
	S_BLINK,        // Blink Sign
	S_BLANK,
	S_ONLINESEPARATOR,    // Разделитель ...
	S_OFFLINESEPARATOR
};

enum M_CHAT_STATUS
{
	SC_INACTIVE = 0,
	SC_ACTIVE = 1,
};

enum W_NETSTATUS
{
	W_CONNECTED    = 2,
	W_DISCONNECTED = 0,
	W_CONNECTING   = 1,
};

enum MESSENGER_MESSAGES
{
	WM_NOTIFY_MESSENGER_TRAY = WM_USER + 1,
	WM_KILL_SEND_MESSAGE_DLG,
	WM_KILL_COME_MESSAGE_DLG,
	WM_KILL_SPLIT_MESSAGE_DLG,
	WM_KILL_INFILE_DLG,
	WM_KILL_DELUSER_MESSAGE_DLG,
	WM_KILL_ADDUSER_MESSAGE_DLG,
	WM_ANSWER_ON_MESSAGE,
	WM_GET_NEXT_MESSAGE,
	WM_UPDATE_STATUS,
	WM_CHANGE_STATUS,
	WM_NEW_CHILD_WINDOW,
	WM_DHTML_EVENT,
	WM_GET_NOWUSER_ID,
	WM_NEW_PROMOCOME,
	//WM_SHOWUSER_DETAILS,
	WM_SEND_FILE,
	WM_SENDGROUP_FILE,
	WM_MCUPDATE_LOADED,
	WM_INVOKE_STARTAUTO_UPDATE,
	WM_UPDATE_CONTACTLIST,
	WM_XMLLOADCOPLETED,
	WM_INVOKE_CREATECHAT,
	WM_INVOKE_SENDMESSAGE,
	WM_SEND_FILE2,
	WM_RESERVED_1 = WM_USER + 100,
	WM_RESERVED_2 = WM_USER + 101,
	WM_RESERVED_5 = WM_USER + 102,
	WM_MAINFRAME_NAVIGATE,
	WM_CASHLOAD_END,
	WM_SHOWMESSAGEBOX,
	WM_SHOW_ADDUSER,
	WM_CHANGE_NEWMESSAGE,
	WM_CHECK_SIGNAL_STATE,
	WM_SAVE_TO_LOCALHISTORY_COMPLETE,
	///WM_LOAD_LASTVERSIONID_COMPLETE,
	WM_SHOW_NEXTMESSAGE,
	WM_SET_RECIPIENT,
	WM_SET_SENDER,
	WM_LOADAPP_COMPLETED,
	WM_SHOW_LOGIN_DLG,
	WM_ALERT_POPUP_MESSAGE_CLK,
	WM_UPLOAD_APP_FILE,
	WM_WF_STATE_CHANGE,
	WM_RESERVED_3 = WM_USER + 201,
	WM_RESERVED_4 = WM_USER + 202,
	WM_DOWNLOAD_BEGIN =  WM_USER + 520 ,
	WM_DOWNLOAD_STEP = WM_USER + 521,
	WM_UPLOAD_BEGIN = WM_USER + 523 ,
	WM_UPLOAD_STEP = WM_USER + 524,
	WM_UPLOAD_APP_PROGRESS,
	WM_UPLOAD_APP_COMPLETED,
	WM_PROCESS_COMMAND_LINE_MESSAGES
};

enum DHTM_Event_Type
{
	DHTMLE_ADDCONTACT,
	DHTMLE_SENDMESSAGE,
	DHTMLE_SENDFILE,
};

enum _DialogMode
{
	DM_McMessengerSundanceEdition = 0,
	DM_McMessenger,
	DM_Yahoo,
	DM_ICQ
};

struct DHTMLE_ADDCONTACT_Container
{
     long    user_id; 
	 long	role_id;
	 CString nick_name; 
	 CString first_name; 
	 CString last_name; 
	 CString email;
	 CString role_name;
};

struct DHTMLE_SENDMESSAGE_Container
{
	long longUserID;
	CString strNickName;
	CString strRole;
};

struct DHTMLE_SENDPROMO_Container
{
	long longUserID;
	CString strNickName;
	CString strRole;
};

enum SEND_DATA_STATUS
{
	DS_ERROR = -1,
	DS_NONE = 0,
	DS_SEND = 1,
	DS_COMPLETE = 2,
};

typedef enum McMessengerAppItemType
{
	APPT_NONE		=	0,	
	APPT_INWINDOW,
	APPT_BROWSEWINDOW,
	APPT_EXWINDOW,
	APPT_CONTACTLIST,
	APPT_CHAT_CONTACTLIST,
	APPT_IBN_ACTIONS
	///APPT_WEB_FOLDERS
} McAppItemType;

struct McAppItem
{
	McAppItemType	Type;
	long			Id;
	CString			Name;
	CString			ToolTip;
	CString			Url;
	long			Width;
	long			Height;
	CComPtr<IPicture>Icon;
	CString			Version;
	
	McAppItem()
	{
		Id		=	0;
		Type	=	APPT_NONE;
	};
	McAppItem(McAppItemType	tType, LPCTSTR strName,LPCTSTR strToolTip,LPCTSTR strUrl)
	{
		Id		=	0;
		Type	= tType;
		Name	= strName;
		ToolTip = strToolTip;
		Url		= strUrl;
	};
	CString	GetVisualName()
	{
		CString	strRet;
		strRet.Format(_T("%s - %s"), Name, ToolTip);
		return strRet;
	}
};

static int nFontSizes[] = {8, 10, 12, 14, 18, 24, 36};
const int nDropHeight = 200;

#define _SHOW_IBN_ERROR_DLG_OK(MessageID)	\
			{\
				CMessageDlg ErrorDlg##MessageID (MessageID,this); \
				ErrorDlg##MessageID.Show(GetString(MessageID),MB_OK);\
			}
			
#endif

#define SmileBuffSize (256)

/*static WCHAR SmileBuff[SmileBuffSize][16] ={
					L":)",		L":(",		L";)",		L":D",		L":-/",	
					L":x",		L":\">",	L":p",		L":O",		L"X-(",	
					L":>",		L"B-)",		L"&gt;:)",	L":((",		L":))",	
					L":|", 		L"0:)",		L":-B",		L"=;",		L"I-)",
					L":-&amp;",	L":-$",		L"[-(",		L":o)",		L";;)",
					L":*",		L":-s",		L"/:)",		L"8-|",		L"8-}",
					L"(:|",		L"=P~",		L":-?",		L"#-o",		L"=D&gt;",
					L":@)",		L"3:-O",	L":(|)",	L"~:&gt;",	L"@};-",
					L"%%-",		L"**==",	L"(~~)",	L"~o)",		L"*-:)",
					L"8-X",		L"=:)",		L"&gt;-)",	L":-L",		L"&lt;):)",
					L"[-o&lt;",	L"@-)",		L"$-)",		L":-&quot;",L":^o",
					L"b-(",		L":)&gt;-",	L"[-X",		L"\\:D/",	L"&gt;:D&lt;"};*/

/*static WCHAR SmileBuff[SmileBuffSize][15] ={
		L":)",		L":(",		L";)",		L":D",		L";;)",	
		L"&gt;:D&lt;",		L":-/",		L":x",		L":\"&gt;",		L":P",	
		L":-*",		L"=((",		L":-O",		L"X(",		L":&gt;",	
		L"B-)",		L":-S",		L"#:-S",	L"&gt;:)",	L":((",	
		L":))",		L":|",		L"/:)",		L"=))",		L"O:)",	
		L":-B",		L"=;",		L"I-|",		L"8-|",		L"L-)",	
		L":-&amp;",		L":-$",		L"[-(",		L":O)",		L"8-}",	
		L"&lt;:-P",	L"(:|",		L"=P~",		L":-?",		L"#-o",	
		L"=D&gt;",		L":-SS",	L"@-)",		L":^o",		L":-w",	
		L":-&lt;",		L"&gt;:P",		L"&lt;):)",	L":@)",		L"3:-O",	
		L":(|)",	L"~:&gt;",		L"@};-",	L"%%-",		L"**==",	
		L"(~~)",	L"~O)",		L"*-:)",	L"8-X",		L"=:)",	
		L"&gt;-)",		L":-L",		L"[-O&lt;",	L"$-)",		L":-\"",	
		L"b-(",		L":)&gt;-",	L"[-X",		L"\\:D/",	L"&gt;:/",	
		L";))",		L":-@",		L"^:)^",	L":-j",		L"(*)"	
		};
*/