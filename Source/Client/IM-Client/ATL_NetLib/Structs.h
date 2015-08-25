#ifndef __STRUCTS_H_
#define __STRUCTS_H_

#include "StdAfx.h"
#include "wininet.h"
/////////////////////////
//Support Functions

/////////////////////////////////////////////////////////////////////////
struct sUser
{
	sUser()
	{
		reset();
	};
	CComBSTR m_UserName;	
	CComBSTR m_FirstName;
	CComBSTR m_LastName;
	CComBSTR m_EMail;
	CComBSTR m_Role;
	long	 m_time;
	long	 m_Status;
	long     m_ID;
	long	 m_Role_ID;
public:
	void reset()
	{
		m_LastName.Empty();
		m_FirstName.Empty();
		m_UserName.Empty();
		m_EMail.Empty();
		m_Role.Empty();
		m_Status	= 0;
		m_ID		= 0;
		m_Role_ID	= 0;
	}
};
struct sConfig
{
	sConfig()
	{
		m_Server	  = CComBSTR(L"212.44.65.142");
		m_Path		  = CComBSTR(L"IMServer/ofs_server.dll");
		m_ProxyServer = CComBSTR(L"");
		m_ProxyBypass = CComBSTR(L"<local>");
		m_Port		  = 80;
		m_ProxyType	  = INTERNET_OPEN_TYPE_PRECONFIG;
	};
	CComBSTR m_Server;	
	CComBSTR m_Path;
	CComBSTR m_ProxyServer;
	CComBSTR m_ProxyBypass;
	long	 m_Port;
	long     m_ProxyType;
};

struct sChat
{
	sChat()
	{

	}
	CComBSTR	m_Name;
	CComBSTR	m_Descr;
	CComBSTR	m_CID;
	long		m_CreationTime;
	long		m_Creator;
	list<long>	m_Users;
};

struct sMessage
{
	sMessage()
	{
		m_Time		= 0;
		m_bChat     = FALSE;
	};
	sUser	 m_Sender;
	BOOL	 m_bChat;
	sChat	 m_Chat;
	CComBSTR m_Body;	
	CComBSTR m_MID;
	long     m_nMID;
	CComBSTR m_SID;
	long     m_Time;
};


struct sPromo
{
	sPromo()
	{
		m_Body			= CComBSTR(L"");
		m_Subject		= CComBSTR(L"");
		m_PID			= CComBSTR(L"");
		m_SID			= CComBSTR(L"");
		m_ProductName	= CComBSTR(L"");
		m_Product_ID	= 0;
		m_Time			= 0;

	};
	sUser	 m_sSender;
	CComBSTR m_Body;	
	CComBSTR m_Subject;
	CComBSTR m_ProductName;
	CComBSTR m_PID;
	CComBSTR m_SID;
	long	 m_Sender_ID;
	long	 m_Product_ID;
	long     m_Time;
};
struct slocalSID
{
	slocalSID()
	{
		m_SID			= CComBSTR(L"");
		m_Count			= 0;
	};
	CComBSTR m_SID;	
	long	 m_Count;
};
struct sFile
{
	sFile()
	{
		m_Body			= CComBSTR(L"");
		m_RealName		= CComBSTR(L"");
		m_FID			= CComBSTR(L"");
		m_SID			= CComBSTR(L"");
		m_size			= 0;
		m_Time			= 0;
		hBackWind		= 0;
	};
	sUser    m_sSender;
	CComBSTR m_Body;	
	CComBSTR m_RealName;
	CComBSTR m_FID;
	CComBSTR m_SID;
	long	 m_size;
	long     m_Time;
	long	 hBackWind;
};
#endif