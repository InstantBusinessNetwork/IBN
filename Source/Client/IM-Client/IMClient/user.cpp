// User.cpp: implementation of the CUser class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "User.h"
#include "OfsTv.h"
#include "GlobalFunction.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif
//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CUser::CUser()
{
	GlobalID = 0L;
	TID = -1L;
	m_iStatus = S_OFFLINE;
	m_nIcon = -1;
	m_bHasNewMessages=FALSE;
	m_strLogin.Empty ();
	m_strType.Empty ();
	m_strFirstName.Empty ();
	m_strLastName.Empty ();
	m_strEMail.Empty ();
	m_RoleID  = 0L;
	m_dwStatusTime  = 0;
}

CUser::CUser(IUser *pUser)
{
	m_nIcon				=	-1;
	m_bHasNewMessages	=	FALSE;
	
	GlobalID			=	long(pUser->GetValue(bstr_t("@id")));
	TID					=	-1L;
	m_iStatus			=	long(pUser->GetValue(bstr_t("status")));
	m_strLogin			=	(char*)_bstr_t(pUser->GetValue(bstr_t("nick_name")));
	m_strType			=	(char*)_bstr_t(pUser->GetValue(bstr_t("role/name")));
	m_strFirstName		=	(char*)_bstr_t(pUser->GetValue(bstr_t("first_name")));
	m_strLastName		=	(char*)_bstr_t(pUser->GetValue(bstr_t("last_name")));
	m_strEMail			=	(char*)_bstr_t(pUser->GetValue(bstr_t("email")));
	m_RoleID			=	long(pUser->GetValue(bstr_t("role/@id")));
	m_dwStatusTime		=	long(pUser->GetValue(bstr_t("time")));
}


CUser::CUser(CUser& src)
{
	*this=src;
}

CUser::~CUser()
{

}

CString CUser::GetShowName()
{
	if(m_strFirstName.IsEmpty()&&m_strLastName.IsEmpty()) 
		return GetString(IDS_LOADING_NAME);

	///if(GetOptionInt(IDS_OFSMESSENGER,IDS_CLMODE,2)==2)
	CString strShowNameFormat;

	if(GetOptionInt(IDS_OFSMESSENGER,IDS_CONTACTLIST_SORT_BY_FIRSTNAME,GetProductLanguage()==CString(_T("1049"))?FALSE:TRUE))
		strShowNameFormat.Format(GetString(IDS_SHOWNAME_FORMAT),m_strFirstName,m_strLastName);
	else
		strShowNameFormat.Format(GetString(IDS_SHOWNAME_FORMAT),m_strLastName,m_strFirstName);

	return strShowNameFormat;
}

int CUser::GetStatus()
{
	return m_iStatus==S_INVISIBLE?S_OFFLINE:m_iStatus;
}

CUser& CUser::operator=(const CUser &src)
{
	TID = src.TID;
	GlobalID = src.GlobalID;
	m_bHasNewMessages=src.m_bHasNewMessages;
	m_iStatus=src.m_iStatus;
	m_nIcon = src.m_nIcon;
	m_strLogin=src.m_strLogin;
	m_strType=src.m_strType;
	m_strFirstName=src.m_strFirstName;
	m_strLastName=src.m_strLastName;
	m_strEMail=src.m_strEMail;
	m_RoleID  = src.m_RoleID;
	m_dwStatusTime = src.m_dwStatusTime;

	return *this;
}

BOOL CUser::IsOnline()
{
	return m_iStatus!=S_OFFLINE&&m_iStatus!=S_AWAITING&&m_iStatus<S_ONLINESEPARATOR;
}

CString CUser::GetId()
{
	return m_strLogin;
}

void CUser::SetId(LPCTSTR id)
{
	m_strLogin=id;
}

BOOL CUser::operator==(CUser &other)
{
	return GetGlobalID() == other.GetGlobalID();//GetId();
}

BOOL CUser::IsSystemUser()
{
	if(m_strType.CompareNoCase(_T("{406979C1-DBAD-4DEA-8622-1D515B98A470}"))==0)
		return TRUE;
	
	return FALSE;
}

BOOL CUser::operator!=(CUser &other)
{
	return !operator==(other);
}

int CUser::GetIcon()
{
	return m_iStatus;
}

int CUser::GetIcon2()
{
	return m_nIcon < 0 ? m_iStatus : m_nIcon;
}


BOOL CUser::IsBad()
{
	return m_strLogin.IsEmpty();
}

void CUser::Update(CUser &srcUser)
{
	m_strLogin		=	srcUser.m_strLogin;
	m_strType		=	srcUser.m_strType;
	m_strFirstName	=	srcUser.m_strFirstName;
	m_strLastName	=	srcUser.m_strLastName;
	m_strEMail		=	srcUser.m_strEMail;
	m_CompanyID		=	srcUser.m_CompanyID;
	m_RoleID		=	srcUser.m_RoleID;
}
