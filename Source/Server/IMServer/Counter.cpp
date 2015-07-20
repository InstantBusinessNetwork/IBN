#include "stdafx.h"
#include "counter.h"

#ifdef _IBN_PERFORMANCE_MONITOR

CCounter::CCounter(void)
{
}

CCounter::~CCounter(void)
{
}

ULONG* CCounter::m_ulActiveUser = 0;
ULONG* CCounter::m_ulCrRequest = 0;
ULONG* CCounter::m_ulCrCommand = 0;
ULONG* CCounter::m_ulCrRcvFile = 0;
ULONG* CCounter::m_ulCrSendFile = 0;
ULONG* CCounter::m_ulCrAlive = 0;
ULONG* CCounter::m_ulMaxRequest = 0;
ULONG* CCounter::m_ulMaxCommand = 0;
ULONG* CCounter::m_ulMaxRcvFile = 0;
ULONG* CCounter::m_ulMaxSendFile = 0;
ULONG* CCounter::m_ulMaxAlive = 0;
ULONGLONG* CCounter::m_ullAvrSQL = 0;
ULONG* CCounter::m_ulAvrSQLBase = 0;
ULONGLONG* CCounter::m_ullAvrCommand = 0;
ULONG* CCounter::m_ulAvrCommandBase = 0;
ULONGLONG* CCounter::m_ullAvrAlive = 0;
ULONG* CCounter::m_ulAvrAliveBase = 0;
ULONGLONG* CCounter::m_ullAvrSendFile = 0;
ULONG* CCounter::m_ulAvrSendFileBase = 0;
ULONGLONG* CCounter::m_ullAvrRcvFile = 0;
ULONG* CCounter::m_ulAvrRcvFileBase = 0;
ULONGLONG* CCounter::m_ullAvrRequest = 0;
ULONG* CCounter::m_ulAvrRequestBase = 0;

BOOL CCounter::_hasRealTimeMonitoring = FALSE;

#endif