#pragma once

class IbnPipeManger
{
private:
	HANDLE	m_hPipe;
	HANDLE	m_hThread;
	DWORD	m_dwThreadId;
	CString m_strPipeName;
public:
	enum	IbnCommand
	{
		UpdateWebStub	=	1,	//	LONG GroupID
		UpdateUser,				//	LONG UserID
		UpdateGroup,			//	LONG GroupID
		SendAlertToGroup,		//	LONG GroupID, BSTR bsParam
		SendAlertToUser,		//	LONG  UserID,  BSTR  bsParam
		SendMessage,			//	LONG ToID, LONG FromID, LONG Message
		LogOff,					//	LONG UserId
		UpdateUserWebStub,		//	LONG UserID
		StopActivity
	};
public:
	IbnPipeManger(void);
	virtual ~IbnPipeManger(void);
	HRESULT Start(BOOL bGlobalPipe, LPCTSTR companyId);
	void Stop(void);
public:
	static void StopExternalActivity(BOOL bGlobalPipe, LPCTSTR companyId);

protected:
	static CString CreatePipeName(BOOL bGlobalPipe, LPCTSTR companyId);
	static DWORD WINAPI thThreadMain(LPVOID param);
	DWORD ThreadMain();

protected:
	HRESULT PipeCommand_UpdateWebStub(/*[in]*/ LONG GroupID);
	HRESULT PipeCommand_UpdateUser(/*[in]*/ LONG UserID);
	HRESULT PipeCommand_UpdateGroup(/*[in]*/ LONG GroupID);
	HRESULT PipeCommand_SendAlertToGroup(LONG GroupID, BSTR bsParam);
	HRESULT PipeCommand_SendAlertToUser( LONG  UserID,  BSTR  bsParam);
	HRESULT PipeCommand_SendMessage(LONG ToID, LONG FromID, BSTR Message);
	HRESULT PipeCommand_LogOff(LONG UserId);
	HRESULT PipeCommand_UpdateUserWebStub(/*[in]*/ LONG UserID);
	HRESULT PipeCommand_StopActivity();
	HRESULT PipeCommand_ChangeStatus(LONG UserID, LONG Status);
};
