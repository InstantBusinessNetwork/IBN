#pragma once
#include "EventLogMessages.h"

class CEventLog
{
public:
	CEventLog(void);
	~CEventLog(void);
	static HRESULT Register(void);
	static HRESULT UnRegister(void);
	static int AddAppLog(LPCTSTR szSTR, DWORD ID = SERVER_STARTED, DWORD dwType = EVENTLOG_INFORMATION_TYPE, LPVOID lpRawData = NULL, DWORD dwDataSize = 0);
};
