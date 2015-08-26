#pragma once

HKEY McRegCreateSubKey(HKEY hKey, LPCTSTR szSubKey, REGSAM samDesired);
HKEY McRegOpenSubKey(HKEY hKey, LPCTSTR szSubKey, REGSAM samDesired);

BOOL McRegGetDWORD(HKEY hKey, LPCTSTR szSubKey, LPCTSTR szEntry, DWORD &Result, DWORD Default = 0);
BOOL McRegWriteDWORD(HKEY hKey, LPCTSTR szSubKey, LPCTSTR szEntry, DWORD Value);
BOOL McRegGetString(HKEY hKey, LPCTSTR szSubKey, LPCTSTR szEntry, CString &Result, LPCTSTR Default = NULL);
BOOL McRegWriteString(HKEY hKey, LPCTSTR szSubKey, LPCTSTR szEntry, LPCTSTR Value);

class CMcSettings
{
public:
	CMcSettings(void);
	~CMcSettings(void);
	// Set root registry key for settings
	void SetRoot(LPCTSTR szRoot);
	// Get DWORD variable
	BOOL GetDWORD(LPCTSTR szSection, LPCTSTR szName, DWORD& dwResult, DWORD dwDefault = 0);
	// Set DWORD variable
	BOOL SetDWORD(LPCTSTR szSection, LPCTSTR szName, DWORD Value);
	// Get string variable
	BOOL GetString(LPCTSTR szSection, LPCTSTR szName, CString& strResult, LPCTSTR szDefault);
	// Set string variable
	BOOL SetString(LPCTSTR szSection, LPCTSTR szName, LPCTSTR Value);
	
	// Set string from Local mashine to Current User
	BOOL SetStringFromLocal2Current(UINT idSection, UINT idName);
	BOOL SetStringFromLocal2Current(LPCTSTR szSection, LPCTSTR szName);
	
protected:
	CString m_strRoot;
public:
	BOOL SetString(UINT idSection, UINT idName, LPCTSTR Value);
	BOOL GetString(UINT idSection, UINT idName, CString& Result, LPCTSTR Default);
};
