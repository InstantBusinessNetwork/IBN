// colorlis.h : header file
//
// This is a part of the Microsoft Foundation Classes C++ library.
// Copyright (C) 1992-1998 Microsoft Corporation
// All rights reserved.
//
// This source code is only intended as a supplement to the
// Microsoft Foundation Classes Reference and related
// electronic documentation provided with the library.
// See these sources for detailed information regarding the
// Microsoft Foundation Classes product.

/////////////////////////////////////////////////////////////////////////////
// CColorMenu window
#ifndef _COLOR_MENU_H_
  #define _COLOR_MENU_H_

class CColorMenu : public CMenu
{
// Construction
public:
	CColorMenu();

// Attributes
public:
	static int BASED_CODE indexMap[17];
	static COLORREF GetColor(UINT id);

// Operations
public:

// Implementation
public:
	virtual void DrawItem(LPDRAWITEMSTRUCT lpDIS);
	virtual void MeasureItem(LPMEASUREITEMSTRUCT lpMIS);

};


#endif
/////////////////////////////////////////////////////////////////////////////
