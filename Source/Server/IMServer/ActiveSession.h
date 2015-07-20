// ActiveSession.h: interface for the CActiveSession class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_ACTIVESESSION_H__712A4E53_226D_460C_8CD2_EEC9F6F8DEDE__INCLUDED_)
#define AFX_ACTIVESESSION_H__712A4E53_226D_460C_8CD2_EEC9F6F8DEDE__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

//////////////////////////////////////////////////////////////////////
const DWORD RefreshTime         = 10000;    // Время повтора перебора в миллисекундах
const time_t DissconnectTime     = 60;      // Время Отлета
const time_t WaitTimeAfterDissconnect = 30; // Время Ожидания Удаления ...
//////////////////////////////////////////////////////////////////////
const DWORD SEND_MESSAGE_CYCLE = 0x0001;
const DWORD SEND_ZEROCHUNK     = 0x0002;
const DWORD SIGNAL_DISCONNECT  = 0x0004; 
const DWORD SEND_MESSAGE_NOW   = 0x0008; /*23.11*/
const DWORD UPDATE_CONNECITON  = 0x0010;
//////////////////////////////////////////////////////////////////////

class CActiveSession
{
public:
	void ClearInfo();
	BOOL IsClosed();
	void SendZeroChunk();
	void SendComplete(EXTENSION_CONTROL_BLOCK* &pECB, DWORD ErrorCode, size_t cbIO);
	void CloseSession();
	BOOL CheckTime();
	void SetEvent(const PBYTE pData, size_t size);
	void UpdateConnection(EXTENSION_CONTROL_BLOCK *pEcb, BOOL keepAlive);
	void Delete(void);

	/// Get Metod's
	long GetStatus();
	time_t GetConnectTime();
	time_t GetDissconnectTime();

	/// Set Metod's
	void SetStatus(long Status);
	void SetConnectTime(time_t Time);
	void SetDissconnectTime(time_t Time);

	////////////////////////
	CActiveSession();
	virtual ~CActiveSession();

	CActiveSession* GetPointer()
	{
		InterlockedIncrement(&iNowUse);
		return this;
	}

	void ReleasePointer()
	{
		InterlockedDecrement(&iNowUse);
		try
		{
			if(iNowUse == 0)
				delete this;
		}
		catch(...)
		{
		}
	}

private:
	PBYTE PopEvent();
	DWORD Send();
	DWORD SendMessage(PBYTE pData, size_t size);
	BOOL GetEvent(PBYTE& pData, size_t& size, BOOL& bChunkWithoutZero, BOOL& bResendOldChunk);
	PBYTE UnionEvent();

	long lKill;
	long lWasReconect;
	///// Addon 17 - 02 - 2001 for Statistic
	DWORD sys_time_end;
	DWORD sys_time_begin;
	//// End Addon
	DWORD ActiveSessionState; /// ????
	CRITICAL_SECTION m_lock;
	long iNowUse;
	time_t m_DissconnectTime;
	time_t m_ConnectTime;
	long  m_Status;
	// Added if poсkect [9/24/2004]
	size_t m_SentPoketSize;
	size_t m_PoketSize;
	PBYTE m_pPoketSend;
	PBYTE pSendData;
	size_t m_SendDataLenth;
	PBYTE pWaitData;
	size_t m_WaitDataLenth;
	EXTENSION_CONTROL_BLOCK *pECBlock;
	BOOL  m_KeepAlive;
};

#endif // !defined(AFX_ACTIVESESSION_H__712A4E53_226D_460C_8CD2_EEC9F6F8DEDE__INCLUDED_)
