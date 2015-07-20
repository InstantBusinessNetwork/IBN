Type.registerNamespace('Mediachase');

Mediachase.Hider = function(element) {
    Mediachase.Hider.initializeBase(this, [element]); 
    this.exchangeTarget = null;
}

Mediachase.HiderManager = function(){
    Mediachase.HiderManager.initializeBase(this);
    this.currentEventItem = null;
}

var _staticHideElements = null;
Mediachase._staticHideElements = function()
{
    if (_staticHideElements === null)
    {
        _staticHideElements = new Array();
    }

    return _staticHideElements;
}

var _staticHiderManager = null;
Mediachase._staticHiderManager = function()
{
    if (_staticHiderManager === null)
    {
        _staticHiderManager = $create(Mediachase.HiderManager, null, null, null);
    }

    return _staticHiderManager;
}

Mediachase.HiderManager.prototype = {
    initialize : function() {
        /// <summary>
        /// Initialize the behavior
        /// </summary>
        Mediachase.HiderManager.callBaseMethod(this, 'initialize');
        $addHandler(document.body, "click", Function.createDelegate(this, this.onClickHandler));
    },
    
    dispose : function() {
        /// <summary>
        /// Dispose the behavior
        /// </summary>
        Mediachase.HiderManager.callBaseMethod(this, 'dispose');
    },
    
    onClickHandler: function(e)
    {
        if (e.target.tagName == 'SELECT')
            return;
        for (var i = 0; i < Mediachase._staticHideElements().length; i++)
        {
            if (this.currentEventItem !== Mediachase._staticHideElements()[i])
                Mediachase._staticHideElements()[i].hideTarget();
        }
        this.currentEventItem = null;
    },
    
    modifyExchangeTarget: function(item)
    {
        this.currentEventItem = item;
    },
    
    get_currentEventItem: function() 
    {
        return this.currentEventItem;
    },    
    set_exchangeTarget : function(value) {
        this.currentEventItem = value;
    }
}


Mediachase.Hider.prototype = {
    initialize : function() {
        /// <summary>
        /// Initialize the behavior
        /// </summary>
        Mediachase.Hider.callBaseMethod(this, 'initialize');
        this.baseInit();
        Array.add(Mediachase._staticHideElements(), this);
        if (Mediachase._staticHiderManager() === null)
        {
            alert("Mediachase.HiderManager is null");
        }
    },
    
    dispose : function() {
        /// <summary>
        /// Dispose the behavior
        /// </summary>
		
        Mediachase.Hider.callBaseMethod(this, 'dispose');
    },
    
    baseInit: function()
    {
        //todo: mb to get currentStyle for display property
        if (this.exchangeTarget)
        {
            //this.exchangeTarget.style.display = 'none';
            this._element.style.display = 'none';
            $addHandler(this.exchangeTarget, "click", Function.createDelegate(this, this.onClickHandler));
        }
    },
    
    onClickHandler: function(e)
    {
        Mediachase._staticHiderManager().modifyExchangeTarget(this);
        this.showTarget();
    },
    
    showTarget: function()
    {
        if (this.exchangeTarget)
        {
            this.exchangeTarget.style.display = 'none';
            this._element.style.display = '';
        }
    },
    
    hideTarget: function()
    {
        if (this.exchangeTarget)
        {
            this.exchangeTarget.style.display = '';
            this._element.style.display = 'none';
        }
    },
    
    get_exchangeTarget: function() 
    {
        return this.exchangeTarget;
    },    
    set_exchangeTarget : function(value) {
        this.exchangeTarget = value;
    }
}

/*
Sys.Application.add_init(function() {
    $create(AjaxControlToolkit.ModalPopupBehavior, {"BackgroundCssClass":"modalPopupBackground","PopupControlID":"pT_McCommandHandlerFramePopupDiv","X":200,"Y":150,"dynamicServicePath":"/Workspace/default.aspx","id":"pT_McCommandHandlerFrameExtender"}, null, null, $get("pT_MC_Framework_CommandManager_Span_Ext"));
});

*/


Mediachase.Hider.registerClass('Mediachase.Hider', Sys.UI.Control);
Mediachase.HiderManager.registerClass('Mediachase.HiderManager', Sys.Component);
if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
