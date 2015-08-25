#ifndef _DEFCONST_
#define _DEFCONST_
typedef enum
{
	cmLogOff,		//
	cmLogOn,		//	
	cmLogOn_int,
	cmMessage,		//
	cmChangeStatus,
	cmChatCreate,
	cmChatStatus,
	cmChatEdit,
	cmChatInvite,
	cmChatAccept,
	cmChatLeave,
	cmPromo,		//		
	cmAddUser,		//
	cmDelUser,		//
	cmConfirmMessage,	//	
	cmConfirmPromo,		//	
	cmConfirmFile,		//
	cmDelUserR,			//
	cmAddUserR,			//
	cmLoadList,		//
	cmLoadPromos,		//
	cmSearch,
	cmDetails,
	cmSendFile,			//
	cmReceiveFile		//

} Commands;

typedef enum
{
	etWININET = 1,
	etSTATUS,
	etSERVER,
	etFILE,
	etCANCEL
} ErrorTypes;

typedef enum
{
	evChangedStatus,
	evMessage,
	evPromo,
	evFile,
	evAdd,
	evAddR,
	evReklama,
	evSysMessage

} Events;

typedef enum  
{
	stDisconnected,
	stConnecting,
	stConnected

}	stState;

typedef enum  
{
	opCommand,
	opHistory,
	opReceiveFile,
	opSendFile,
	opGetEvents

}	opType;

typedef enum  
{
	fcRemindLater = 1,
	fcDelete,
	fcDoOffline

}	fcFile;

typedef enum  
{
	ltContact = 1,
	ltIgnore,
	ltFiles,
	ltMessages,
	ltPromos,
	ltSIDs,
	ltChats
}	ltListType;

typedef enum  
{
	atAccept = 1,
	atDeny
}	atAnswerType;

#define IM_CHANGE_STATE WM_USER + 110
#define IM_ANSWER_OK	WM_USER + 111
#define IM_ANSWER_BUFF	WM_USER + 112
#define IM_ANSWER_ERROR	WM_USER + 113
#define IM_MARSHAL		WM_USER + 114
#define IM_NEW_EVENT	WM_USER + 115
#define IM_END_SESS		WM_USER + 116

#define E_INVALIDQUEUE E_POINTER

#endif