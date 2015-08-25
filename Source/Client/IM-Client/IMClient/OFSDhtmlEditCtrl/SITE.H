//=--------------------------------------------------------------------------=
//  (C) Copyright 1997-1998 Microsoft Corporation. All Rights Reserved.
//=--------------------------------------------------------------------------=
// View Site for Document Objects.


#ifndef _SITE_H_
#define _SITE_H_

#include <mshtml.h>
#include <mshtmhst.h>
#include <triedit.h>
#include <comdef.h>

class CImpIOleClientSite : public IOleClientSite
{
    protected:
        ULONG               m_cRef;
        class CSite        *m_pSite;
        LPUNKNOWN           m_pUnkOuter;

    public:
        CImpIOleClientSite(class CSite *, IUnknown *);
        ~CImpIOleClientSite(void);

        STDMETHODIMP QueryInterface(REFIID, void **);
        STDMETHODIMP_(ULONG) AddRef(void);
        STDMETHODIMP_(ULONG) Release(void);

        STDMETHODIMP SaveObject(void);
        STDMETHODIMP GetMoniker(DWORD, DWORD, LPMONIKER *);
        STDMETHODIMP GetContainer(LPOLECONTAINER *);
        STDMETHODIMP ShowObject(void);
        STDMETHODIMP OnShowWindow(BOOL);
        STDMETHODIMP RequestNewObjectLayout(void);
};

typedef CImpIOleClientSite *PCImpIOleClientSite;



class CImpIAdviseSink : public IAdviseSink
{
    protected:
        ULONG               m_cRef;
        class CSite        *m_pSite;
        LPUNKNOWN           m_pUnkOuter;

    public:
        CImpIAdviseSink(class CSite *, IUnknown *);
        ~CImpIAdviseSink(void);

        STDMETHODIMP QueryInterface(REFIID, void **);
        STDMETHODIMP_(ULONG) AddRef(void);
        STDMETHODIMP_(ULONG) Release(void);

        STDMETHODIMP_(void)  OnDataChange(LPFORMATETC, LPSTGMEDIUM);
        STDMETHODIMP_(void)  OnViewChange(DWORD, LONG);
        STDMETHODIMP_(void)  OnRename(LPMONIKER);
        STDMETHODIMP_(void)  OnSave(void);
        STDMETHODIMP_(void)  OnClose(void);
};


typedef CImpIAdviseSink *PCImpIAdviseSink;


class CImpIOleInPlaceSite : public IOleInPlaceSite
{
    protected:
        ULONG               m_cRef;
        class CSite        *m_pSite;
        LPUNKNOWN           m_pUnkOuter;

    public:
        CImpIOleInPlaceSite(class CSite *, IUnknown *);
        ~CImpIOleInPlaceSite(void);

        STDMETHODIMP QueryInterface(REFIID, void **);
        STDMETHODIMP_(ULONG) AddRef(void);
        STDMETHODIMP_(ULONG) Release(void);

        STDMETHODIMP GetWindow(HWND *);
        STDMETHODIMP ContextSensitiveHelp(BOOL);
        STDMETHODIMP CanInPlaceActivate(void);
        STDMETHODIMP OnInPlaceActivate(void);
        STDMETHODIMP OnUIActivate(void);
        STDMETHODIMP GetWindowContext(LPOLEINPLACEFRAME *
                        , LPOLEINPLACEUIWINDOW *, LPRECT, LPRECT
                        , LPOLEINPLACEFRAMEINFO);
        STDMETHODIMP Scroll(SIZE);
        STDMETHODIMP OnUIDeactivate(BOOL);
        STDMETHODIMP OnInPlaceDeactivate(void);
        STDMETHODIMP DiscardUndoState(void);
        STDMETHODIMP DeactivateAndUndo(void);
        STDMETHODIMP OnPosRectChange(LPCRECT);
};

typedef CImpIOleInPlaceSite *PCImpIOleInPlaceSite;


class CImpIOleDocumentSite : public IOleDocumentSite
{
    protected:
        ULONG               m_cRef;
        class CSite        *m_pSite;
        LPUNKNOWN           m_pUnkOuter;

    public:
        CImpIOleDocumentSite(class CSite *, IUnknown *);
        ~CImpIOleDocumentSite(void);

        STDMETHODIMP QueryInterface(REFIID, void **);
        STDMETHODIMP_(ULONG) AddRef(void);
        STDMETHODIMP_(ULONG) Release(void);

        STDMETHODIMP ActivateMe(IOleDocumentView *);
};

typedef CImpIOleDocumentSite *PCImpIOleDocumentSite;



///////////////////////////////////////////////////
// Host integration interfaces
///////////////////////////////////////////////////
class CImpIDocHostUIHandler : public IDocHostUIHandler
{
    protected:
        ULONG               m_cRef;
        class CSite        *m_pSite;
        LPUNKNOWN           m_pUnkOuter;

    public:
        CImpIDocHostUIHandler(class CSite *, IUnknown *);
        ~CImpIDocHostUIHandler(void);

        STDMETHODIMP QueryInterface(REFIID, void **);
        STDMETHODIMP_(ULONG) AddRef(void);
        STDMETHODIMP_(ULONG) Release(void);

		STDMETHODIMP GetHostInfo(DOCHOSTUIINFO * pInfo);
		STDMETHODIMP ShowUI(
				DWORD dwID, 
				IOleInPlaceActiveObject * pActiveObject,
				IOleCommandTarget * pCommandTarget,
				IOleInPlaceFrame * pFrame,
				IOleInPlaceUIWindow * pDoc);
		STDMETHODIMP HideUI(void);
		STDMETHODIMP UpdateUI(void);
		STDMETHODIMP EnableModeless(BOOL fEnable);
		STDMETHODIMP OnDocWindowActivate(BOOL fActivate);
		STDMETHODIMP OnFrameWindowActivate(BOOL fActivate);
		STDMETHODIMP ResizeBorder(
				LPCRECT prcBorder, 
				IOleInPlaceUIWindow * pUIWindow, 
				BOOL fRameWindow);
		STDMETHODIMP ShowContextMenu(
				DWORD dwID, 
				POINT * pptPosition,
				IUnknown* pCommandTarget,
				IDispatch * pDispatchObjectHit);
		STDMETHODIMP TranslateAccelerator(/* [in] */ LPMSG lpMsg,/* [in] */ const GUID __RPC_FAR *pguidCmdGroup,/* [in] */ DWORD nCmdID);
		STDMETHODIMP GetOptionKeyPath(BSTR* pbstrKey, DWORD dw);
		STDMETHODIMP GetDropTarget( 
            /* [in] */ IDropTarget __RPC_FAR *pDropTarget,
            /* [out] */ IDropTarget __RPC_FAR *__RPC_FAR *ppDropTarget);

		STDMETHODIMP GetExternal( 
            /* [out] */ IDispatch __RPC_FAR *__RPC_FAR *ppDispatch);
        
        STDMETHODIMP TranslateUrl( 
            /* [in] */ DWORD dwTranslate,
            /* [in] */ OLECHAR __RPC_FAR *pchURLIn,
            /* [out] */ OLECHAR __RPC_FAR *__RPC_FAR *ppchURLOut);
        
        STDMETHODIMP FilterDataObject( 
            /* [in] */ IDataObject __RPC_FAR *pDO,
            /* [out] */ IDataObject __RPC_FAR *__RPC_FAR *ppDORet);


};

typedef CImpIDocHostUIHandler* PCImpIDocHostUIHandler;



class CImpIDocHostShowUI : public IDocHostShowUI
{
    protected:
        ULONG               m_cRef;
        class CSite        *m_pSite;
        LPUNKNOWN           m_pUnkOuter;

    public:
        CImpIDocHostShowUI(class CSite *, IUnknown *);
        ~CImpIDocHostShowUI(void);

        STDMETHODIMP QueryInterface(REFIID, void **);
        STDMETHODIMP_(ULONG) AddRef(void);
        STDMETHODIMP_(ULONG) Release(void);

		STDMETHODIMP ShowMessage(
								HWND hwnd,
								LPOLESTR lpstrText,
								LPOLESTR lpstrCaption, 
								DWORD dwType,
								LPOLESTR lpstrHelpFile,
								DWORD dwHelpContext,
								LRESULT * plResult);
		STDMETHODIMP ShowHelp(
								HWND hwnd,
								LPOLESTR pszHelpFile,
								UINT uCommand,
								DWORD dwData,
								POINT ptMouse,
								IDispatch * pDispatchObjectHit);
};

typedef CImpIDocHostShowUI* PCImpIDocHostShowUI;



/*
 * IDispatch - implements Ambient properties
 */
class CImpAmbientIDispatch : public IDispatch
{
    protected:
        ULONG           m_cRef;
        class CSite		*m_pSite;
        LPUNKNOWN       m_pUnkOuter;

    public:
        CImpAmbientIDispatch(class CSite *, IUnknown *);
        ~CImpAmbientIDispatch(void);

        STDMETHODIMP QueryInterface(REFIID, LPVOID *);
        STDMETHODIMP_(ULONG) AddRef(void);
        STDMETHODIMP_(ULONG) Release(void);

        STDMETHODIMP GetTypeInfoCount(UINT *);
        STDMETHODIMP GetTypeInfo(UINT, LCID, ITypeInfo **);
        STDMETHODIMP GetIDsOfNames(REFIID, OLECHAR **, UINT
            , LCID, DISPID *);
        STDMETHODIMP Invoke(DISPID, REFIID, LCID, USHORT
            , DISPPARAMS *, VARIANT *, EXCEPINFO *, UINT *);
};

typedef class CImpAmbientIDispatch *PCImpAmbientIDispatch;



/*
 * The CSite class, a COM object with the interfaces IOleClientSite,
 * IAdviseSink, IOleInPlaceSite, and IOleDocumentSite.
 */


class CSite : public IUnknown
{
	
    private:
        ULONG           m_cRef;
        HWND            m_hWnd; //Client area window of parent

        class COfsDhtmlEditCtrl*   m_pFR;
		BOOL m_bFiltered;

        //Object interfaces
        LPUNKNOWN          m_pObj;
        LPOLEOBJECT        m_pIOleObject;
        LPOLEINPLACEOBJECT m_pIOleIPObject;
        LPOLEDOCUMENTVIEW  m_pIOleDocView;
		LPOLECOMMANDTARGET m_pIOleCommandTarget;


        //Our interfaces
        PCImpIOleClientSite   m_pImpIOleClientSite;
        PCImpIAdviseSink      m_pImpIAdviseSink;
        PCImpIOleInPlaceSite  m_pImpIOleIPSite;
        PCImpIOleDocumentSite m_pImpIOleDocumentSite;
        PCImpIDocHostUIHandler m_pImpIDocHostUIHandler;
        PCImpIDocHostShowUI		m_pImpIDocHostShowUI;
		PCImpAmbientIDispatch	m_pImpAmbientIDispatch;


    protected:
        BOOL    ObjectInitialize(LPCTSTR pchPath);

    public:
	    HRESULT TranslateAccelerator(/* [in] */ LPMSG lpMsg,/* [in] */ const GUID __RPC_FAR *pguidCmdGroup,/* [in] */ DWORD nCmdID);
        CSite( HWND, COfsDhtmlEditCtrl* );
        ~CSite(void);

        //Gotta have an IUnknown for delegation
        STDMETHOD(QueryInterface)(REFIID, void** );
        STDMETHOD_(ULONG,AddRef)(void);
        STDMETHOD_(ULONG,Release)(void);

		STDMETHOD(DragEnter)(IDataObject * pDataObject, WORD grfKeyState, POINTL pt,DWORD * pdwEffect);	
		STDMETHOD(DragOver)(DWORD grfKeyState, POINTL pt,	DWORD * pdwEffect);
		STDMETHOD(DragLeave)(void);
		STDMETHOD(Drop)(IDataObject * pDataObject,DWORD grfKeyState, POINTL pt,DWORD * pdwEffect);

		// Other functions
        BOOL        Create(LPCTSTR pchPath);
        void        Close(BOOL);
        void        Activate(LONG);
        void        UpdateObjectRects(void);
		HRESULT		Load(LPCTSTR pchPath);

		HRESULT		Save(LPCTSTR pchPath);
		HRESULT		SaveWithFilter(LPCTSTR pchPath);
		// Filtering methods

		HRESULT HrFileToStream(LPCTSTR fileName, LPSTREAM* ppiStream);
		HRESULT HrStreamToFile(LPSTREAM pStream, LPCTSTR fileName);

		HRESULT HrFilterIn(LPCTSTR fileName, LPSTREAM* ppFilteredStream, DWORD dwFlags);
		HRESULT HrFilterOut(LPSTREAM pSourceStream, LPCTSTR fileName, DWORD dwFlags);

		HRESULT HrFilterOutStream(LPSTREAM pSourceStream, LPSTREAM &pOutputStream, DWORD dwFlags);
		HRESULT HrFilterInStream(LPSTREAM  pInputStream, LPSTREAM* ppFilteredStream, DWORD dwFlags);



		// helper functions
		COfsDhtmlEditCtrl*	GetFrame( void ){ return m_pFR; }
		LPUNKNOWN GetObjectUnknown( void ){ return m_pObj; } 
		PCImpIOleInPlaceSite GetIPSite( void ){return m_pImpIOleIPSite; }
		HWND GetWindow( void ){ return m_hWnd; }
		LPOLECOMMANDTARGET GetCommandTarget( void ){return m_pIOleCommandTarget;}
		void SetCommandTarget( LPOLECOMMANDTARGET pTarget ){ m_pIOleCommandTarget = pTarget; }
		void SetDocView( LPOLEDOCUMENTVIEW pDocView ){ m_pIOleDocView = pDocView; }
		
		void SetIPObject( LPOLEINPLACEOBJECT pIPObject ){ m_pIOleIPObject = pIPObject; }
		LPOLEINPLACEOBJECT GetIPObject( void ){ return m_pIOleIPObject; }
};


typedef CSite* PCSite;



//DeleteInterfaceImp calls 'delete' and NULLs the pointer
#define DeleteInterfaceImp(p)\
{\
            if (NULL!=p)\
            {\
                delete p;\
                p=NULL;\
            }\
}


//ReleaseInterface calls 'Release' and NULLs the pointer
#define ReleaseInterface(p)\
{\
            IUnknown *pt=(IUnknown *)p;\
            p=NULL;\
            if (NULL!=pt)\
                pt->Release();\
}

#endif //_SITE_H_
