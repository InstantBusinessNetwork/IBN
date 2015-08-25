// User.h: interface for the CUser class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_USER_H__5D811314_E415_4153_A1B2_F8B64FF9852D__INCLUDED_)
#define AFX_USER_H__5D811314_E415_4153_A1B2_F8B64FF9852D__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000


class CUser  
{
public:
	BOOL IsSystemUser();
	void Update(CUser& srcUser);
	BOOL IsBad();
	BOOL m_bFloating;
	int GetIcon2();
	COLORREF GetColor();
	int GetIcon();
	void SetId(LPCTSTR id);
	CString GetId();
	BOOL IsOnline();
	BOOL m_bHasNewMessages;// send new message
	CUser& operator=(const CUser &src);
	BOOL operator==(CUser &other);
	BOOL operator!=(CUser &src);
	int GetStatus();
	CString GetShowName();
	CUser();
	CUser(CUser&);
	CUser(IUser *pUser);
	
	long GetGlobalID() {return GlobalID;}
	void SetGlobalID(long m_ID) {GlobalID = m_ID;}


	virtual ~CUser();

	int m_nIcon;
	int m_iStatus;
	long	m_dwStatusTime;
	CString m_strLogin;
	CString m_strType;
	CString m_strFirstName;
	CString m_strLastName;
	CString m_strEMail;
	long m_CompanyID;
	long m_RoleID;
	long TID;
	long GlobalID;
};

typedef CArray <CUser *,CUser *> CUserArray;

#endif // !defined(AFX_USER_H__5D811314_E415_4153_A1B2_F8B64FF9852D__INCLUDED_)
