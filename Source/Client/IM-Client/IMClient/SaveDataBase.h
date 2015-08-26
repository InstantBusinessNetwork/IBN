// SaveDataBase.h: interface for the CSaveDataBase class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_SAVEDATABASE_H__F72F4DFD_AE0C_4928_8960_C8723B5B345E__INCLUDED_)
#define AFX_SAVEDATABASE_H__F72F4DFD_AE0C_4928_8960_C8723B5B345E__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

class CSaveDataBase  
{
public:
	void Stop();
	void WorkFunction();
	HRESULT InitOk();
	CSaveDataBase(bstr_t &SID, IComHistIntPtr &pHistory, IMessagesPtr &pMessage,
		DWORD dwNotifyMessage, HWND MessageParent);
	virtual ~CSaveDataBase();
protected:
	HANDLE hWorkThread;
	static DWORD WINAPI thThread(LPVOID p);
    IStream *pHistoryStream , *pMessagesStream ;
	bstr_t LoadSID;
	DWORD dwMessage;
	HWND  hWindow;
	HRESULT hr;
	DWORD IDWorkThread;
	HANDLE hExitEvent;
	BOOL bExit;
};

#endif // !defined(AFX_SAVEDATABASE_H__F72F4DFD_AE0C_4928_8960_C8723B5B345E__INCLUDED_)
