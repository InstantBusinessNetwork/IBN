#ifndef __LOG__EX__
#define __LOG__EX__

#define LOG_EX_WINDOW_NAME			"{CD882CBA-DE79-429c-B195-732051E98C43}"
#define LOG_EX_WINDOW_CLASS			"{173D6EE5-02DC-4239-884E-C0051639D11A}"

#define LOG_SAVE_WINDOW_NAME		"{337556F0-BE00-481c-98E5-D30EAB8070A8}"
#define LOG_SAVE_WINDOW_CLASS		"{84E7E26D-2077-4645-92DF-E57C5B8AF548}"

#define WM_SAVE_LOG_DATA		WM_USER+1
#define WM_GET_LOG_INFO			WM_USER+2

#define LOGEX_TYPE_NORMAL		0
#define LOGEX_TYPE_INFO			1

enum
{
	LOGEX_LEVEL0 = 0,
	LOGEX_LEVEL1,
	LOGEX_LEVEL2,
	LOGEX_LEVEL3,
	LOGEX_LEVEL4,
	LOGEX_LEVEL5,
	LOGEX_LEVEL6,
	LOGEX_LEVEL7,
	LOGEX_LEVEL8,
	LOGEX_LEVEL9,
	LOGEX_LEVEL_LAST
};

struct LOGEX_INFO
{
	SYSTEMTIME st;
	DWORD dwProcID;
	DWORD dwType;
	DWORD dwUID;
	DWORD dwLevel;
};

#ifdef LOGEX_EXPORTS
#define LOGEX_API __declspec(dllexport)
#else
#define LOGEX_API __declspec(dllimport)
#endif

LOGEX_API void AddToLogEX(DWORD dwLevel, char *szFormat, ...);
LOGEX_API void AddToLogEX(DWORD dwLevel, BSTR format, ...);
LOGEX_API void AddToLogEX(DWORD dwLevel, PBYTE pData, DWORD dwSize);

#ifdef _DOLOG
#define MCTRACE AddToLogEX
#else //_DOLOG
#define MCTRACE (void)0
#endif //_DOLOG

#endif //__LOG__EX__
