#ifndef _OFS_CONST_
#define _OFS_CONST_

typedef enum
{
	etWININET = 1, //=>WIN32Errors and wininet errors
	etSTATUS,	//=>HTTP status
	etSERVER,	//= IM server error, smr nizhe
	etFILE,		//= win32 file error
	etCANCEL	//= cancel
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
#define ERR_LICENSE_LIMIT				130
#define IDR_TEST                        201                                                                     */

const int MaxValueID = 21; 

static int IndexEquals[10]=
{
	9,//S_OFFLINE=0,
	1,//S_ONLINE,
	9,//S_INVISIBLE,
	2,//S_DND,          // не беспокоить
	3,//S_AWAY,         // нету
	4,//S_NA,           //
	5,//S_OCCUPIED,     
	6,//S_AWAITING,
	7,//S_UNKNOWN,      // 
	10// S_WEBAPP
};


static long m_ShablonId[MaxValueID][12] =
{
{0L,0L,0L,0L,0L,0L,0L,0L,0L,0L}, /// Группа ...
{1L,1L,0L,0L,0L,0L,0L,0L,0L,0L},  /// S_ONLINE
{2L,1L,0L,0L,0L,0L,0L,0L,0L,0L},  /// S_DND
{3L,1L,0L,0L,0L,0L,0L,0L,0L,0L},  /// S_AWAY
{4L,1L,0L,0L,0L,0L,0L,0L,0L,0L},  /// S_NA
{5L,1L,0L,0L,0L,0L,0L,0L,0L,0L},  /// S_OCCUPIED
{6L,1L,0L,0L,0L,0L,0L,0L,0L,0L},  /// S_AWAITING
{7L,1L,0L,0L,0L,0L,0L,0L,0L,0L},  /// S_UNKNOWN
{8L,1L,0L,0L,0L,0L,0L,0L,0L,0L},  /// S_INVISIBLE
{9L,1L,0L,0L,0L,0L,0L,0L,0L,0L},   /// S_OFFLINE
{10L,1L,0L,0L,0L,0L,0L,0L,0L,0L},  /// S_WEBAPP
{1L,0L,0L,0L,0L,0L,0L,0L,0L,0L},  /// S_ONLINE + Message
{2L,0L,0L,0L,0L,0L,0L,0L,0L,0L},  /// S_DND + Message
{3L,0L,0L,0L,0L,0L,0L,0L,0L,0L},  /// S_AWAY + Message
{4L,0L,0L,0L,0L,0L,0L,0L,0L,0L},  /// S_NA + Message
{5L,0L,0L,0L,0L,0L,0L,0L,0L,0L},  /// S_OCCUPIED + Message
{6L,0L,0L,0L,0L,0L,0L,0L,0L,0L},  /// S_AWAITING + Message
{7L,0L,0L,0L,0L,0L,0L,0L,0L,0L},  /// S_UNKNOWN + Message
{8L,0L,0L,0L,0L,0L,0L,0L,0L,0L},  /// S_INVISIBLE + Message
{9L,0L,0L,0L,0L,0L,0L,0L,0L,0L},   /// S_OFFLINE + Message
{10L,0L,0L,0L,0L,0L,0L,0L,0L,0L},  /// S_WEBAPP + Message
};

static short m_ShablonIcon[MaxValueID][12] =
{
{-2,-1,-1,-1,-1,-1,-1,-1,-1,-1}, /// Группа ...
{3,-1,-1,-1,-1,-1,-1,-1,-1,-1},  /// S_ONLINE
{4,-1,-1,-1,-1,-1,-1,-1,-1,-1},  /// S_DND
{5,-1,-1,-1,-1,-1,-1,-1,-1,-1},  /// S_AWAY
{6,-1,-1,-1,-1,-1,-1,-1,-1,-1},  /// S_NA
{7,-1,-1,-1,-1,-1,-1,-1,-1,-1},  /// S_OCCUPIED
{8,-1,-1,-1,-1,-1,-1,-1,-1,-1},  /// S_AWAITING
{9,-1,-1,-1,-1,-1,-1,-1,-1,-1},  /// S_UNKNOWN
{11,-1,-1,-1,-1,-1,-1,-1,-1,-1},  /// S_INVISIBE
{11,-1,-1,-1,-1,-1,-1,-1,-1,-1},  /// S_OFFLINE
{10,-1,-1,-1,-1,-1,-1,-1,-1,-1},  /// S_WEBAPP
{12,13,-1,-1,-1,-1,-1,-1,-1,-1},  /// S_ONLINE + Message
{12,13,-1,-1,-1,-1,-1,-1,-1,-1},  /// S_DND + Message
{12,13,-1,-1,-1,-1,-1,-1,-1,-1},  /// S_AWAY + Message
{12,13,-1,-1,-1,-1,-1,-1,-1,-1},  /// S_NA + Message
{12,13,-1,-1,-1,-1,-1,-1,-1,-1},  /// S_OCCUPIED + Message
{12,13,-1,-1,-1,-1,-1,-1,-1,-1},  /// S_AWAITING + Message
{12,13,-1,-1,-1,-1,-1,-1,-1,-1},  /// S_UNKNOWN + Message
{12,13,-1,-1,-1,-1,-1,-1,-1,-1},  /// S_INVISIBLE + Message
{12,13,-1,-1,-1,-1,-1,-1,-1,-1} ,  /// S_OFFLINE + Message
{12,13,-1,-1,-1,-1,-1,-1,-1,-1},  /// S_WEBAPP + Message
};


static DWORD m_ShablonRGBTextEnable[MaxValueID]=
{ 
	RGB(0,0,0), /// Группа ...
	RGB(0,0,0), /// S_ONLINE
    RGB(0,0,0), /// S_DND	
	RGB(0,0,0), /// S_AWAY
	RGB(0,0,0), /// S_NA
	RGB(0,0,0), /// S_OCCUPIED
	RGB(0,0,0), /// S_AWAITING
	RGB(0,0,0), /// S_UNKNOWN
	RGB(0,0,0), /// S_INVISIBLE
	RGB(0,0,0), /// S_OFFLINE
	RGB(0,0,0), /// S_WEBAPP
	RGB(0,0,0),     /// S_ONLINE + Message
	RGB(0,0,0), /// S_DND + Message
	RGB(0,0,0), /// S_AWAY + Message
	RGB(0,0,0), /// S_NA + Message
	RGB(0,0,0), /// S_OCCUPIED + Message
	RGB(0,0,0), /// S_AWAITING + Message
	RGB(0,0,0), /// S_UNKNOWN + Message
	RGB(0,0,0), /// S_INVISIBLE + Message
	RGB(0,0,0), /// S_OFFLINE + Message
	RGB(0,0,0), /// S_WEBAPP + Message
};

static DWORD m_ShablonRGBTextSelect[MaxValueID]=
{
	RGB(0,0,0), /// Группа ...
	RGB(0,0,0), /// S_ONLINE
    RGB(0,0,0), /// S_DND	
	RGB(0,0,0), /// S_AWAY
	RGB(0,0,0), /// S_NA
	RGB(0,0,0), /// S_OCCUPIED
	RGB(0,0,0), /// S_AWAITING
	RGB(0,0,0), /// S_UNKNOWN
	RGB(0,0,0), /// S_INVISIBLE
	RGB(0,0,0), /// S_OFFLINE
	RGB(0,0,0), /// S_WEBAPP
	RGB(0,0,0),     /// S_ONLINE + Message
	RGB(0,0,0), /// S_DND + Message
	RGB(0,0,0), /// S_AWAY + Message
	RGB(0,0,0), /// S_NA + Message
	RGB(0,0,0), /// S_OCCUPIED + Message
	RGB(0,0,0), /// S_AWAITING + Message
	RGB(0,0,0), /// S_UNKNOWN + Message
	RGB(0,0,0), /// S_INVISIBLE + Message
	RGB(0,0,0), /// S_OFFLINE + Message
	RGB(0,0,0), /// S_WEBAPP + Message

};

static DWORD m_ShablonRGBFonEnable[MaxValueID]=
{
	RGB(255,255,255), /// Группа ...
	RGB(255,255,255), /// S_ONLINE
    RGB(255,255,255), /// S_DND	
	RGB(255,255,255), /// S_AWAY
	RGB(255,255,255), /// S_NA
	RGB(255,255,255), /// S_OCCUPIED
	RGB(255,255,255), /// S_AWAITING
	RGB(255,255,255), /// S_UNKNOWN
	RGB(255,255,255), /// S_INVISIBLE
	RGB(255,255,255), /// S_OFFLINE
	RGB(255,255,255), /// S_WEBAPP
	RGB(255,255,255), /// S_ONLINE + Message
	RGB(255,255,255), /// S_DND + Message
	RGB(255,255,255), /// S_AWAY + Message
	RGB(255,255,255), /// S_NA + Message
	RGB(255,255,255), /// S_OCCUPIED + Message
	RGB(255,255,255), /// S_AWAITING + Message
	RGB(255,255,255), /// S_UNKNOWN + Message
	RGB(255,255,255), /// S_INVISIBLE + Message
	RGB(255,255,255), /// S_OFFLINE + Message
	RGB(255,255,255) /// S_WEBAPP + Message
};

static DWORD m_ShablonRGBFonSelect[MaxValueID]=
{
	RGB(200,200,200), /// Группа ...
	RGB(200,200,200), /// S_ONLINE
    RGB(200,200,200), /// S_DND	
	RGB(200,200,200), /// S_AWAY
	RGB(200,200,200), /// S_NA
	RGB(200,200,200), /// S_OCCUPIED
	RGB(200,200,200), /// S_AWAITING
	RGB(200,200,200), /// S_UNKNOWN
	RGB(200,200,200), /// S_INVISIBLE
	RGB(200,200,200), /// S_OFFLINE
	RGB(200,200,200), /// S_WEBAPP
	RGB(200,200,200), /// S_ONLINE + Message
	RGB(200,200,200), /// S_DND + Message
	RGB(200,200,200), /// S_AWAY + Message
	RGB(200,200,200), /// S_NA + Message
	RGB(200,200,200), /// S_OCCUPIED + Message
	RGB(200,200,200), /// S_AWAITING + Message
	RGB(200,200,200), /// S_UNKNOWN + Message
	RGB(200,200,200), /// S_INVISIBLE + Message
	RGB(200,200,200), /// S_OFFLINE + Message
	RGB(200,200,200) /// S_WEBAPP + Message
};

const int ChatMaxValueID = 3; 

static long m_ChatShablonId[ChatMaxValueID][10] =
{
	{-2,-1,-1,-1,-1,-1,-1,-1,-1,-1}, /// Группа ...
	{2,-1,-1,-1,-1,-1,-1,-1,-1,-1},  /// S_ONLINE
	{1,-1,-1,-1,-1,-1,-1,-1,-1,-1},  /// S_OFFLINE
};

static short m_ChatShablonIcon[ChatMaxValueID][10] =
{
	{-2,-1,-1,-1,-1,-1,-1,-1,-1,-1}, /// Группа ...
	{4,-1,-1,-1,-1,-1,-1,-1,-1,-1},  /// S_ONLINE
	{3,-1,-1,-1,-1,-1,-1,-1,-1,-1},  /// S_ONLINE
};

static DWORD m_ChatShablonRGBTextEnable[ChatMaxValueID]=
{
	RGB(0,0,0), /// Группа ...
	RGB(0,0,0), /// S_ONLINE
	RGB(0,0,0), /// S_OFFLINE
};

static DWORD m_ChatShablonRGBTextSelect[ChatMaxValueID]=
{
	RGB(0,0,0), /// Группа ...
	RGB(0,0,0), /// S_ONLINE
	RGB(0,0,0) /// S_OFFLINE
};

static DWORD m_ChatShablonRGBFonEnable[ChatMaxValueID]=
{
	RGB(255,255,255), /// Группа ...
	RGB(255,255,255), /// S_ONLINE
	RGB(255,255,255) /// S_OFFLINE
};

static DWORD m_ChatShablonRGBFonSelect[ChatMaxValueID]=
{
	RGB(200,200,200), /// Группа ...
	RGB(200,200,200), /// S_ONLINE
	RGB(200,200,200) /// S_OFFLINE
};

#endif// _OFS_CONST_





















