#pragma once

#ifdef _IBN_PERFORMANCE_MONITOR
class CCounter
{
public:
	CCounter(void);
	~CCounter(void);

	inline static BOOL GetHasRealTimeMonitoring()
	{
		return _hasRealTimeMonitoring;
	}

	inline static void SetHasRealTimeMonitoring(BOOL enabled)
	{
		_hasRealTimeMonitoring = enabled;
	}
	
	static ULONG* m_ulActiveUser;
	static ULONG* m_ulCrRequest;
	static ULONG* m_ulCrCommand;
	static ULONG* m_ulCrRcvFile;
	static ULONG* m_ulCrSendFile;
	static ULONG* m_ulCrAlive;
	static ULONG* m_ulMaxRequest;
	static ULONG* m_ulMaxCommand;
	static ULONG* m_ulMaxRcvFile;
	static ULONG* m_ulMaxSendFile;
	static ULONG* m_ulMaxAlive;
	static ULONGLONG* m_ullAvrSQL;
	static ULONG* m_ulAvrSQLBase;
	static ULONGLONG* m_ullAvrCommand;
	static ULONG* m_ulAvrCommandBase;
	static ULONGLONG* m_ullAvrAlive;
	static ULONG* m_ulAvrAliveBase;
	static ULONGLONG* m_ullAvrSendFile;
	static ULONG* m_ulAvrSendFileBase;
	static ULONGLONG* m_ullAvrRcvFile;
	static ULONG* m_ulAvrRcvFileBase;
	static ULONGLONG* m_ullAvrRequest;
	static ULONG* m_ulAvrRequestBase;

private:
	static BOOL _hasRealTimeMonitoring;
};
#endif
