/*
Copyright Scand LLC http://www.scbr.com
To use this component please contact info@scbr.com to obtain license
*/

/**
*     @desc: enable/disable drag-and-drop
*     @type: public
*     @param: mode - enabled/disabled [ can be true/false/temporary_disabled - last value mean that tree can be D-n-D can be switched to true later ]
*     @topic: 0
*/
	dhtmlXGridObject.prototype.enableDragAndDrop=function(mode){
        if  (mode=="temporary_disabled"){
            this.dADTempOff=false;
            mode=true;                  }
        else
            this.dADTempOff=true;

		this.dragAndDropOff=convertStringToBoolean(mode);
		 };

/**
*     @desc: set Drag-And-Drop behavior (child - drop as chils, sibling - drop as sibling
*     @type: public
*     @param: mode - behavior name (child,sibling,complex)
*     @topic: 0
*/
dhtmlXGridObject.prototype.setDragBehavior=function(mode){
        this.dadmodec=this.dadmodefix=0;
		switch (mode) {
			case "child": this.dadmode=0; break;
			case "sibling": this.dadmode=1; break;
			case "complex": this.dadmode=2; break;
		}	 };

/**
*     @desc: create html element for dragging
*     @type: private
*     @param: htmlObject - html node object
*     @topic: 1
*/
dhtmlXGridObject.prototype._createDragNode=function(htmlObject){
      if (!this.dADTempOff) return null;
      htmlObject.parentObject=new Object();
      htmlObject.parentObject.treeNod=this;

      var z=new Array();
//#__pro_feature:21092006{
//#drag_adv:21092006{
      z=(this.getSelectedId()||"").split(",");
      var exst=false;
      for (var i=0; i<z.length; i++)
        if (z[i]==htmlObject.parentNode.idd)  exst=true;
      if (!exst)
//#}
//#}
          z[this.selMultiRows?z.length:0]=htmlObject.parentNode.idd;


      this._dragged=new Array();
      for (var i=0; i<z.length; i++)
          if (this.rowsAr[z[i]]){
          this._dragged[this._dragged.length]=this.rowsAr[z[i]];
          this.rowsAr[z[i]].treeNod=this;
          }

      htmlObject.parentObject.parentNode=htmlObject.parentNode;

	var dragSpan=document.createElement('div');
	dragSpan.innerHTML=this.rowToDragElement(htmlObject.parentNode.idd);
	dragSpan.style.position="absolute";
	dragSpan.className="dragSpanDiv";
	return dragSpan;
}



/**
*   @desc:  create a drag visual marker
*   @type:  private
*/
dhtmlXGridObject.prototype._createSdrgc=function(){
    this._sdrgc=document.createElement("DIV");
    this._sdrgc.innerHTML="&nbsp;";
    this._sdrgc.className="gridDragLine";
    this.objBox.appendChild(this._sdrgc);
}











/**
*   @desc:  create a drag context object
*   @type:  private
*/
function dragContext(a,b,c,d,e,f,j,h){
    this.source=a||"grid";
    this.target=b||"grid";
    this.mode=c||"move";
    this.dropmode=d||"child";
    this.sid=e||0;
    this.tid=f||window.unknown;
    this.sobj=j||null;
    this.tobj=h||null;
    return this;
}
/**
*   @desc:  close context
*   @type:  private
*/
dragContext.prototype.close=function(){
    this.sobj=null;
    this.tobj=null;
}
/**
*   @desc:  return copy of context
*   @type:  private
*/
dragContext.prototype.copy=function(){
    return new dragContext(this.source,this.target,this.mode,this.dropmode,this.sid,this.tid,this.sobj,this.tobj);
    this.sobj=null;
    this.tobj=null;
}
/**
*   @desc:  set a lue of context attribute
*   @type:  private
*/
dragContext.prototype.set=function(a,b){
    this[a]=b;
    return this;
}
/**
*   @desc:  generate an Id for new node
*   @type:  private
*/
dragContext.prototype.uid=function(a,b){
    this.nid=this.sid;
    while (this.tobj.rowsAr[this.nid])
        this.nid=this.nid+((new Date()).valueOf());

    return this;
}
/**
*   @desc:  get data array for grid row
*   @type:  private
*/
dragContext.prototype.data=function(){
    if (this.sobj==this.tobj)
        return this.sobj._getRowArray(this.sobj.rowsAr[this.sid]);
    if (this.source=="tree")
        return this.tobj.treeToGridElement(this.sobj,this.sid,this.tid);
    else
        return this.tobj.gridToGrid(this.sid,this.sobj,this.tobj);
}
/**
*   @desc:  return parent id for row in context
*   @type:  private
*/
dragContext.prototype.pid=function(){
    if (this.tid==window.unknown) return window.unknown;
    if (this.target=="treeGrid")
        if (this.dropmode=="child")
            return this.tid;
        else
            return this.tobj.rowsAr[this.tid].parent_id;
}
/**
*   @desc:  get index of target position
*   @type:  private
*/
dragContext.prototype.ind=function(){
    if (this.tid==window.unknown) return 0;
    if ((this.dropmode=="child")&&(this.target=="treeGrid"))
            this.tobj.openItem(this.tid);
    var ind=this.tobj.rowsCol._dhx_find(this.tobj.rowsAr[this.tid]);			

    return (ind+((this.target=="treeGrid")?this.tobj._countBranchLength(ind):1));
}
/**
*   @desc:  get row related image
*   @type:  private
*/
dragContext.prototype.img=function(){
    if (this.target!="grid")
		return this.sobj.getItemImage(this.sid);
	else return null;
}

/**
*   @desc:  return list of rows in context
*   @type:  private
*/
dragContext.prototype.slist=function(){
    var res=new Array();
    for (var i=0; i<this.sid.length; i++)
        res[res.length]=this.sid[i][(this.source=="tree")?"id":"idd"];

    return res.join(",");
}


/**
*   @desc:  drag entry point
*   @type:  private
*/
dhtmlXGridObject.prototype._drag=function(sourceHtmlObject,dhtmlObject,targetHtmlObject,innerFlag){
    //close unfinished tasks
    if  (this._autoOpenTimer) window.clearTimeout(this._autoOpenTimer);

    //detect details
    var r1=targetHtmlObject.parentNode;
    var r2=sourceHtmlObject.parentObject;
    //drop on heder
    if (!r1.grid) { r1.grid=this;    this.dadmodefix=0; }

    var c=new dragContext(0,0,0,(r1.grid.dadmodec?"sibling":"child"));

    if (r2 && r2.childNodes)
        c.set("source","tree").set("sobj",r2.treeNod).set("sid",c.sobj._dragged);
    else{
        if (r2.treeNod.isTreeGrid())    c.set("source","treeGrid");
        c.set("sobj",r2.treeNod).set("sid",c.sobj._dragged);
        }


    if (r1.grid.isTreeGrid())
        c.set("target","treeGrid");
    c.set("tobj",r1.grid).set("tid",r1.idd);

//#__pro_feature:21092006{
//#drag_adv:21092006{
    //complex drag mode
    if ((c.tobj.dadmode==2)&&(c.tobj.dadmodec==1)&&(c.tobj.dadmodefix<0))
       if (c.tobj.obj._rows(0).idd!=c.tid) c.tid=r1.previousSibling.idd;
       else c.tid=window.unknown;
//#}
//#}


    if (c.sobj.dpcpy) c.set("mode","copy");
    c.tobj._clearMove();

    if ((c.tobj.dragFunc)&&(!c.tobj.dragFunc(c.slist(),c.tid,c.sobj,c.tobj)))  return;

   //all ready, start mantras
   var result=new Array();
   if (typeof(c.sid)=="object"){
        var nc=c.copy();
        for (var i=0; i<c.sid.length; i++){
            nc.tobj._dragRoutine(nc.set("sid",c.sid[i][(c.source=="tree"?"id":"idd")]));
            result[result.lenght]=nc.nid;
            }
        nc.close();
        }
    else
       c.tobj._dragRoutine(c);

   //destroy context
    if (c.tobj.dropFunc)
    	c.tobj.dropFunc(c.slist(),c.tid,result.join(","),c.sobj,c.tobj);

   c.close();
}


/**
*   @desc:  context drag routine
*   @type:  private
*/
dhtmlXGridObject.prototype._dragRoutine=function(c){
        c.uid().tobj.addRow(c.nid,c.data(),c.ind(),c.pid(),c.img());

        if (c.source=="tree"){
            var sn=c.sobj._globalIdStorageFind(c.sid);
            if (sn.childsCount){
                var nc=c.copy().set("tid",c.nid).set("dropmode",c.target=="grid"?"sibling":"child");
        		for(var j=0;j<sn.childsCount;j++){
                    c.tobj._dragRoutine(nc.set("sid",sn.childNodes[j].id));
                    if (c.mode=="move") j--;
                    }
                nc.close();
                }
        }
        else{
            c.tobj._copyUserData(c);
            if ((c.source=="treeGrid")){
                var snc=c.sobj.loadedKidsHash.get(c.sid);
                if ((snc)&&(snc.length)){
                    var nc=c.copy().set("tid",c.nid);
                    if(c.target=="grid")
                        nc.set("dropmode","sibling");
                    else {
                        nc.tobj.openItem(c.tid);
                        nc.set("dropmode","child");
                        }
                    for(var j=0;j<snc.length;j++){
                        c.tobj._dragRoutine(nc.set("sid",snc[j].idd));
                        if (c.mode=="move") j--;
                        }
                    nc.close();
                    }
            }
        }

        if (c.mode=="move"){
           c.sobj[(c.source=="tree")?"deleteItem":"deleteRow"](c.sid);
           if ((c.sobj==c.tobj)&&(!c.tobj.rowsAr[c.sid])) c.tobj.changeRowId(c.nid,c.sid);
           }
}


/**
*	@desc: redefine this method in your code to define how grid row values should be used in another grid
*	@param: rowId - id of draged row
*	@param: sgrid - source grid object
*	@param: tgrid - target grid object
*	@returns: array of values
*	@type: public
*   @topic: 7
*/
dhtmlXGridObject.prototype.gridToGrid = function(rowId,sgrid,tgrid){
    var z=new Array();
    for (var i=0; i<sgrid.hdr.rows[0].cells.length; i++)
        z[i]=sgrid.cells(rowId,i).getValue();
	return z;
}

/**
*   @desc:  check if d-n-d is in allowed rules
*   @type:  private
*/
dhtmlXGridObject.prototype.checkParentLine=function(node,id){
    if ((!id)||(!node)) return false;
    if (node.idd==id) return true;
    else return this.checkParentLine(this.getRowById(node.parent_id),id);
}

/**
*   @desc:  called when drag moved over landing
*   @type:  private
*/
dhtmlXGridObject.prototype._dragIn=function(htmlObject,shtmlObject,x,y){
                    if (!this.dADTempOff) return 0;
                    var tree=this.isTreeGrid();

                    if(htmlObject.parentNode==shtmlObject.parentNode)
                        return 0;

                    if ((!tree)&&((htmlObject.parentNode.nextSibling) &&(htmlObject.parentNode.nextSibling==shtmlObject.parentNode)))
    					return 0;

                    if ((tree)&&((this.checkParentLine(htmlObject.parentNode,shtmlObject.parentNode.idd))))
                        return 0;
                    var obj=shtmlObject.parentObject?shtmlObject.parentObject:shtmlObject.parentNode;
                    if ((this.dragInFunc)&&(!this.dragInFunc(obj.idd||obj.id,htmlObject.parentNode.idd,obj.grid||obj.treeNod,htmlObject.parentNode.grid)))
                        return 0;

                    this._setMove(htmlObject,x,y);

                  if ((tree)&&(htmlObject.parentNode.expand!="")){
    				  	this._autoOpenTimer=window.setTimeout(new callerFunction(this._autoOpenItem,this),1000);
    					this._autoOpenId=htmlObject.parentNode.idd;
                        }
                  else
                    if  (this._autoOpenTimer) window.clearTimeout(this._autoOpenTimer);

                    return htmlObject;
}
/**
*   @desc:  open item on timeout
*   @type:  private
*/
dhtmlXGridObject.prototype._autoOpenItem=function(e,gridObject){
	gridObject.openItem(gridObject._autoOpenId);
}

/**
*   @desc:  called on onmouseout event , when drag out landing zone
*   @type:  private
*/
dhtmlXGridObject.prototype._dragOut=function(htmlObject){
                    this._clearMove();
                    if  (this._autoOpenTimer) window.clearTimeout(this._autoOpenTimer);
}
/**
*   @desc:  set visual effect for moving row over landing
*   @type:  private
*/
dhtmlXGridObject.prototype._setMove=function(htmlObject,x,y){
	var a1=getAbsoluteTop(htmlObject);
	var a2=getAbsoluteTop(this.objBox);

    if (this.dadmode==2)
    {

        var z=y-a1+this.objBox.scrollTop+(document.body.scrollTop||document.documentElement.scrollTop)-2-htmlObject.offsetHeight/2;
        if ((Math.abs(z)-htmlObject.offsetHeight/6)>0)
        {
        this.dadmodec=1;
      //sibbling zone
        if (z<0)  this.dadmodefix=-1; else   this.dadmodefix=1;
        }
        else this.dadmodec=0;
    }
    else
        this.dadmodec=this.dadmode;


	//scroll down
	if ( (a1-a2-parseInt(this.objBox.scrollTop))>(parseInt(this.objBox.offsetHeight)-50) )
		this.objBox.scrollTop=parseInt(this.objBox.scrollTop)+20;
	//scroll top
	if ( (a1-a2)<(parseInt(this.objBox.scrollTop)+30) )
		this.objBox.scrollTop=parseInt(this.objBox.scrollTop)-20;

    if (this.dadmodec){
      if (!this._sdrgc) this._createSdrgc();
      this._sdrgc.style.display="block";
      this._sdrgc.style.top=a1-a2+((this.dadmodefix>=0)?htmlObject.offsetHeight:0)+"px";
    }
    else{
      this._llSelD=htmlObject;
      if (htmlObject.parentNode.tagName=="TR")
      for (var i=0; i<htmlObject.parentNode.childNodes.length; i++)
      {
      var z= htmlObject.parentNode.childNodes[i];
      z._bgCol=z.style.backgroundColor;
      z.style.backgroundColor="#FFCCCC";
      }
    }
}
/**
*   @desc:  remove all visual effects
*   @type:  private
*/
dhtmlXGridObject.prototype._clearMove=function(){
    if (this._sdrgc) this._sdrgc.style.display="none";
    if ((this._llSelD)&&(this._llSelD.parentNode.tagName=="TR"))
        for (var i=0; i<this._llSelD.parentNode.childNodes.length; i++)
           this._llSelD.parentNode.childNodes[i].style.backgroundColor=this._llSelD._bgCol;

    this._llSelD=null;
}


/**
*	@desc: redefine this method in your code to define how grid row values should be displaied while draging
*	@param: gridRowId - id of grid row
*	@returns: if true, then grid row will be moved to tree, else - copied
*	@type: public
*   @topic: 7
*/
dhtmlXGridObject.prototype.rowToDragElement=function(gridRowId){
    var out=this.cells(gridRowId,0).getValue();
 	return out;
}








/**
*   @desc:  copy user data for row
*   @type:  private
*/
dhtmlXGridObject.prototype._copyUserData = function(c){
            var z1 = c.sobj.UserData[c.sid];
            var z2 = new Hashtable();
            if (z1) {
                z2.keys = z2.keys.concat(z1.keys);
                z2.values = z2.values.concat(z1.values);
            }

            c.tobj.UserData[c.tid]=z2;
    }



/**
*     @desc: move row
*     @type:  public
*     @param: rowId - source row Id
*     @param: mode - moving mode (up,down,row_sibling)
*     @param: targetId - target row  in row_sibling mode
*     @param: targetGrid - used for moving between grids (optional)
*     @topic: 2
*/
dhtmlXGridObject.prototype.moveRow=function(rowId,mode,targetId,targetGrid){
		switch(mode){
		case "row_sibling":
                    this.moveRowTo(rowId,targetId,"move","sibling",this,targetGrid);
			break;
		case "up":
					this.moveRowUp(rowId);
			break;
		case "down":
					this.moveRowDown(rowId);
    		break;
		}
}



/**
*     @desc: set function called when drag-and-drop event occured
*     @param: aFunc - event handling function
*     @type: public
*     @topic: 10
*     @event:    onDrag
*     @eventdesc: Event occured after item was dragged and droped on another item, but before item moving processed.
		Event also raised while programmatic moving nodes.
*     @eventparam:  ID of source item
*     @eventparam:  ID of target item
*     @eventparam:  source grid object
*     @eventparam:  target grid object
*     @eventreturn:  true - confirm drag-and-drop; false - deny drag-and-drop;
*/
dhtmlXGridObject.prototype.setDragHandler=function(func){  if (typeof(func)=="function") this.dragFunc=func; else this.dragFunc=eval(func);  }


//#__pro_feature:21092006{
//#drag_adv:21092006{

/**
*	@desc: move rows from grid to tree
*	@param: tree - object of tree
*	@param: targetObject - target node of tree
*	@param: beforeNode - anchor node of tree
*	@param: itemObject - dragged node
*	@param: z2 - flag of recursion
*	@type:  private
*   @edition: Professional
*   @topic: 7
*/
dhtmlXGridObject.prototype._nonTrivialNode=function(tree,targetObject,beforeNode,itemObject,z2)
{
    if ((tree.dragFunc)&&(!z2))
   	    if (!tree.dragFunc(itemObject.parentNode.idd,targetObject.id,(beforeNode?beforeNode.id:null),this,tree)) return false;

 	var gridRowId = itemObject.idd;
 	var treeNodeId = gridRowId+(new Date()).getMilliseconds();

	var img=(this.isTreeGrid()?this.getItemImage(gridRowId):"")
	var newone=tree._attachChildNode(targetObject,treeNodeId,this.gridToTreeElement(tree,treeNodeId,gridRowId),"",img,img,img,"","",beforeNode);
    if (this.loadedKidsHash){
      var akids=this.loadedKidsHash.get(gridRowId);
      if (akids)
      for (var i=0; i<akids.length; i++){
          this._nonTrivialNode(tree,newone,0,akids[i],1);
          if (!this.dpcpy) i--;
      }
    }

    if (!this.dpcpy) this.deleteRow(gridRowId);

    if ((tree.dropFunc)&&(!z2))
    	tree.dropFunc(treeNodeId,targetObject.id,(beforeNode?beforeNode.id:null),this,tree);
}

/**
*	@desc: redefine this method in your code to define how grid row values should be used in tree (using input parameters you can change id of new tree node, set label, set userdata blocks etc.).
*	@param: treeObj - object of tree
*	@param: treeNodeId - id of node created in tree
*	@param: gridRowId - id of grid row
*	@returns: if true, then grid row will be moved to tree, else - copied
*	@type: public
*   @edition: Professional
*   @topic: 7
*/
dhtmlXGridObject.prototype.gridToTreeElement = function(treeObj,treeNodeId,gridRowId){
	return this.cells(gridRowId,0).getValue();
}

/**
*	@desc: redefine this method in your code to define how tree node values should be used in grid (using input parameters you can change id of new row, values for cells, userdata blocks etc.).
*	@param: treeObj - object of tree
*	@param: treeNodeId - id of node created in tree
*	@param: gridRowId - id of grid row
*	@returns: if true, then tree node should be moved to grid, else - copied
*	@type: public
*   @edition: Professional
*   @topic: 7
*/
dhtmlXGridObject.prototype.treeToGridElement = function(treeObj,treeNodeId,gridRowId){
    var w=new Array();
    var z=this.cellType._dhx_find("tree");
	if (z==-1) z=0;
	for(var i=0;i<this.getColumnCount();i++)
        w[w.length]=(i!=z)?(treeObj.getUserData(treeNodeId,this.getColumnId(i))||""):treeObj.getItemText(treeNodeId);
	return w;
}

/**
*     @desc: move row
*     @type:  public
*     @param: srowId - source row Id
*     @param: trowId - target row Id
*     @param: mode - move or copy
*     @param: dropmode - sibling or child
*     @param: sourceGrid - target row  in row_sibling mode
*     @param: targetGrid - used for moving between grids (optional)
*     @edition: Professional
*     @topic: 2
*/
dhtmlXGridObject.prototype.moveRowTo=function(srowId,trowId,mode,dropmode,sourceGrid,targetGrid){
    var c=new dragContext("grid","grid",mode,dropmode||"sibling",srowId,trowId,sourceGrid||grid,targetGrid||grid);
    c.tobj._dragRoutine(c);
    c.close();
}




/**
*     @desc: set function called after drag-and-drap event occured
*     @param: func - event handling function
*     @type: public
*     @edition: Professional
*     @topic: 10
*     @event:  onDrop
*     @eventdesc:  Event raised after drag-and-drop processed.
*     @eventparam:  ID of source item
*     @eventparam:  ID of target item
*     @eventparam:  ID of droped item (has sense for mercy drag-n-drop)
*     @eventparam:  source grid object
*     @eventparam:  target grid object
*/
dhtmlXGridObject.prototype.setDropHandler=function(func){  if (typeof(func)=="function") this.dropFunc=func; else this.dropFunc=eval(func);  };


/**
*     @desc: set function called when drag moved over potencial landing
*     @param: func - event handling function
*     @type: public
*     @edition: Professional
*     @topic: 10
*     @event:  onDragIn
*     @eventdesc:  Event raised if drag moved over potencial landing
*     @eventparam:  ID of source item
*     @eventparam:  ID of target item
*     @eventparam:  source grid object
*     @eventparam:  target grid object
*     @eventreturn:  true - allow landing; false - deny landing;
*/
dhtmlXGridObject.prototype.setDragInHandler=function(func){  if (typeof(func)=="function") this.dragInFunc=func; else this.dragInFunc=eval(func);  };



/**
*     @desc: enable drag without removing (copy instead of move)
*     @beforeInit: 1
*     @param: mode - 1 - on, 0 - off;
*     @type: public
*	  @edition:Professional
*     @topic: 0
*/
dhtmlXGridObject.prototype.enableMercyDrag=function(mode){ this.dpcpy=convertStringToBoolean(mode); };
//#}
//#}

