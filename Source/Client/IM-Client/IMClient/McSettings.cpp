#include "StdAfx.h"
#include "mcsettings.h"

CMcSettings::CMcSettings(void)
: m_strRoot(_T(""))
{
}

CMcSettings::~CMcSettings(void)
{
}

// Set root registry key for settings
void CMcSettings::SetRoot(LPCTSTR szRoot)
{
	m_strRoot = szRoot;
}

// Get DWORD variable
BOOL CMcSettings::GetDWORD(LPCTSTR szSection, LPCTSTR szName, DWORD& Result, DWORD Default)
{
	ASSERT(m_strRoot.GetLength() > 0);
	ASSERT(szName != NULL);

	Result = Default;

	DWORD dwValue;
	CString strKey;
	strKey.Format(_T("%s\\%s"), m_strRoot, szSection);

	// First look in HKEY_CURRENT_USER
	if(McRegGetDWORD(HKEY_CURRENT_USER, strKey, szName, dwValue, Default))
	{
		Result = dwValue;
		return TRUE;
	}
	// Then look in HKEY_LOCAL_MACHINE
	if(McRegGetDWORD(HKEY_LOCAL_MACHINE, strKey, szName, dwValue, Default))
	{
		Result = dwValue;
		return TRUE;
	}

	return FALSE;
}

// Set DWORD variable
BOOL CMcSettings::SetDWORD(LPCTSTR szSection, LPCTSTR szName, DWORD Value)
{
	ASSERT(m_strRoot.GetLength() > 0);
	ASSERT(szName != NULL);

	CString strKey;
	strKey.Format(_T("%s\\%s"), m_strRoot, szSection);

	return McRegWriteDWORD(HKEY_CURRENT_USER, strKey, szName, Value);
}

// Get string variable
BOOL CMcSettings::GetString(LPCTSTR szSection, LPCTSTR szName, CString& Result, LPCTSTR Default)
{
	ASSERT(m_strRoot.GetLength() > 0);

	Result = Default;

	CString strKey, strValue;
	strKey.Format(_T("%s\\%s"), m_strRoot, szSection);

	// First look in HKEY_CURRENT_USER
	if(McRegGetString(HKEY_CURRENT_USER, strKey, szName, strValue, Default))
	{
		Result = strValue;
		return TRUE;
	}
	// Then look in HKEY_LOCAL_MACHINE
	if(McRegGetString(HKEY_LOCAL_MACHINE, strKey, szName, strValue, Default))
	{
		Result = strValue;
		return TRUE;
	}

	return FALSE;
}

// Set string variable
BOOL CMcSettings::SetString(LPCTSTR szSection, LPCTSTR szName, LPCTSTR Value)
{
	ASSERT(m_strRoot.GetLength() > 0);

	CString strKey;
	strKey.Format(_T("%s\\%s"), m_strRoot, szSection);

	return McRegWriteString(HKEY_CURRENT_USER, strKey, szName, Value);
}

HKEY McRegCreateSubKey(HKEY hKey, LPCTSTR szSubKey, REGSAM samDesired)
{
	DWORD dw;
	HKEY hSubKey = NULL;
	::RegCreateKeyEx(hKey, szSubKey, 0, REG_NONE, REG_OPTION_NON_VOLATILE, samDesired, NULL, &hSubKey, &dw);
	return hSubKey;
}

HKEY McRegOpenSubKey(HKEY hKey, LPCTSTR szSubKey, REGSAM samDesired)
{
//	DWORD dw;
	HKEY hSubKey = NULL;
	//::RegCreateKeyEx(hKey, szSubKey, 0, REG_NONE, REG_OPTION_NON_VOLATILE, samDesired, NULL, &hSubKey, &dw);
	::RegOpenKeyEx(hKey, szSubKey,0, samDesired, &hSubKey);
	return hSubKey;
}

BOOL McRegGetDWORD(HKEY hKey, LPCTSTR szSubKey, LPCTSTR szEntry, DWORD &Result, DWORD dwDefault)
{
	ASSERT(szSubKey != NULL);
	ASSERT(szEntry != NULL);

	Result = dwDefault;

	HKEY hSecKey = McRegOpenSubKey(hKey, szSubKey, KEY_READ);
	if(hSecKey == NULL)
		return FALSE;
	DWORD dwValue;
	DWORD dwType;
	DWORD dwCount = sizeof(DWORD);
	LONG lResult = ::RegQueryValueEx(hSecKey, (LPTSTR)szEntry, NULL, &dwType, (LPBYTE)&dwValue, &dwCount);
	::RegCloseKey(hSecKey);
	if(lResult == ERROR_SUCCESS)
	{
		ASSERT(dwType == REG_DWORD);
		ASSERT(dwCount == sizeof(DWORD));
		Result = dwValue;
		return TRUE;
	}
	return FALSE;
}

BOOL McRegWriteDWORD(HKEY hKey, LPCTSTR szSubKey, LPCTSTR szEntry, DWORD dwValue)
{
	ASSERT(szSubKey != NULL);
	ASSERT(szEntry != NULL);
	HKEY hSecKey = McRegCreateSubKey(hKey, szSubKey, KEY_WRITE);
	if(hSecKey == NULL)
		return FALSE;
	LONG lResult = ::RegSetValueEx(hSecKey, szEntry, NULL, REG_DWORD, (LPBYTE)&dwValue, sizeof(dwValue));
	::RegCloseKey(hSecKey);
	return lResult == ERROR_SUCCESS;
}

BOOL McRegGetString(HKEY hKey, LPCTSTR szSubKey, LPCTSTR szEntry, CString &Result, LPCTSTR szDefault)
{
	ASSERT(szSubKey != NULL);

	Result = szDefault;
	
	HKEY hSecKey = McRegOpenSubKey(hKey, szSubKey, KEY_READ);
	if(hSecKey == NULL)
		return FALSE;
	
	CString strValue;
	DWORD dwType, dwCount;
	LONG lResult = ::RegQueryValueEx(hSecKey, (LPTSTR)szEntry, NULL, &dwType, NULL, &dwCount);
	if(lResult == ERROR_SUCCESS)
	{
		ASSERT(dwType == REG_SZ || dwType == REG_EXPAND_SZ);
		lResult = ::RegQueryValueEx(hSecKey, (LPTSTR)szEntry, NULL, &dwType, (LPBYTE)strValue.GetBuffer(dwCount/sizeof(TCHAR)), &dwCount);
		strValue.ReleaseBuffer();
	}
	::RegCloseKey(hSecKey);
	if(lResult == ERROR_SUCCESS)
	{
		ASSERT(dwType == REG_SZ || dwType == REG_EXPAND_SZ);
		Result = strValue;
		return TRUE;
	}
	return FALSE;
}

BOOL McRegWriteString(HKEY hKey, LPCTSTR szSubKey, LPCTSTR szEntry, LPCTSTR szValue)
{
	ASSERT(szSubKey != NULL);

	LONG lResult;
	
	if(szValue == NULL)
	{
		HKEY hSecKey = McRegCreateSubKey(hKey, szSubKey, KEY_WRITE);
		if(hSecKey == NULL)
			return FALSE;
		
		// necessary to cast away const below
		lResult = ::RegDeleteValue(hSecKey, (LPTSTR)szEntry);
		::RegCloseKey(hSecKey);
	}
	else
	{
		HKEY hSecKey = McRegCreateSubKey(hKey, szSubKey, KEY_WRITE);
		if(hSecKey == NULL)
			return FALSE;
		lResult = ::RegSetValueEx(hSecKey, szEntry, NULL, REG_SZ, (LPBYTE)szValue, (lstrlen(szValue)+1)*sizeof(TCHAR));
		::RegCloseKey(hSecKey);
	}
	return lResult == ERROR_SUCCESS;
}

BOOL CMcSettings::GetString(UINT idSection, UINT idName, CString &Result, LPCTSTR Default)
{
	CString strSection, strName;
	strSection.LoadString(idSection);
	strName.LoadString(idName);
	return GetString(strSection, strName, Result, Default);
}

BOOL CMcSettings::SetString(UINT idSection, UINT idName, LPCTSTR Value)
{
	CString strSection, strName;
	strSection.LoadString(idSection);
	strName.LoadString(idName);
	return SetString(strSection, strName, Value);
}

// Set string from Local mashine to Current User
BOOL CMcSettings::SetStringFromLocal2Current(UINT idSection, UINT idName)
{
	CString strSection, strName;
	strSection.LoadString(idSection);
	strName.LoadString(idName);
	return SetStringFromLocal2Current(strSection, strName);
}

// Set string from Local mashine to Current User
BOOL CMcSettings::SetStringFromLocal2Current(LPCTSTR szSection, LPCTSTR szName)
{
	ASSERT(m_strRoot.GetLength() > 0);
	
	CString strKey, strValue;
	strKey.Format(_T("%s\\%s"), m_strRoot, szSection);
	
	// Then look in HKEY_LOCAL_MACHINE
	if(McRegGetString(HKEY_LOCAL_MACHINE, strKey, szName, strValue, _T("")))
	{
		if(McRegWriteString(HKEY_CURRENT_USER, strKey, szName, strValue))
			return TRUE;
	}

	return FALSE;
}

