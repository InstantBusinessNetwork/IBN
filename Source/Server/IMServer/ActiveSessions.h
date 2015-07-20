// ActiveSessions.h: interface for the CActiveSessions class.
//
//////////////////////////////////////////////////////////////////////
#if !defined(AFX_ACTIVESESSIONS_H__EBE0686F_ABCD_468C_B804_0524E5040A2C__INCLUDED_)
#define AFX_ACTIVESESSIONS_H__EBE0686F_ABCD_468C_B804_0524E5040A2C__INCLUDED_

#include "OFSMatrix.h"

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

const DWORD WM_CONNECT_TIMEOUT = WM_USER + 0x0210;

extern int LicenseCheckActiveUsers();

class CActiveSessions
{
public:
	void SendEventForAll(PBYTE pData, size_t size);
	DWORD Terminate(DWORD dwTime=0);
	DWORD Initialize(DWORD IDEventThread);
	void GetStatus(long id, long& status);
	void SetStatus(long id, long status);
	void UpdateConnection(long id, EXTENSION_CONTROL_BLOCK *pEcb, BOOL keepAlive);
	void SendEvent(long id, PBYTE pData, size_t size);
	void GetIDbySID(LPCTSTR sid, long& id);
	void GetSIDByID(long id, LPTSTR& sid, size_t sidSize);
	void DeleteActiveSession(LPCTSTR sid);
	void DeleteActiveSession(long id);
	void AddAcitveSession(long id, LPCTSTR sid);
	CActiveSessions();
	virtual ~CActiveSessions();
	void Timer();
private:
	DWORD	_idSendThread;
	DWORD	_idTimerThread;
	DWORD	_dwLicenseNumber;
protected:
	static DWORD WINAPI TimerThread(LPVOID pParam);
	COFSMatrix _matrix;
	HANDLE _hEventExit;
	HANDLE _hExitTimerEvent;
	uintptr_t _thread;
public:
	void CheckLicense();
protected:
	//DWORD GetMaxUsersFromLicense(void);
};

#endif // !defined(AFX_ACTIVESESSIONS_H__EBE0686F_ABCD_468C_B804_0524E5040A2C__INCLUDED_)
