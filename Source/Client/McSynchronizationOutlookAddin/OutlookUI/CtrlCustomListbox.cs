using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Diagnostics;

namespace OutlookAddin.OutlookUI
{
    /// <summary>
    /// Delegate used to handle clicking of list items.
    /// </summary>
    public delegate void ListItemClickedHandler(CustomListBox sender, Control listItem, bool isSelected);

    /// <summary>
    /// This class is the actual list box, it handles addition and removal of list items, 
    /// fires events when items are clicked and animates the list based on mouse dragging.
    /// The list accepts any <c>System.Windows.Forms.Control</c> as list item but if the 
    /// list item extends <c>IExtendedListItem</c> additional functionality is enabled.
    /// </summary>
    public partial class CustomListBox : UserControl
    {
        /// <summary>
        /// Event that clients hooks into to get item clicked events.
        /// </summary>
        public event ListItemClickedHandler ListItemClicked;

        #region Mouse event handlers
        private MouseEventHandler mouseDownEventHandler;
        private MouseEventHandler mouseUpEventHandler;
        private MouseEventHandler mouseMoveEventHandler;
        #endregion
        
        #region Members used to handle dragging

        private Point mouseDownPoint = Point.Empty;
        private Point previousPoint = Point.Empty;
        private bool mouseIsDown = false;
        private int draggedDistance = 0;
        
        /// <summary>
        /// This member is used to make sure rendering of the list 
        /// does not occur to frequently.
        /// </summary>
        private bool renderLockFlag = false;

        #endregion

        /// <summary>
        /// Contains the selected state for all list items.
        /// </summary>
        private Dictionary<Control, bool> selectedItemsMap = new Dictionary<Control, bool>();

        #region Animation related members

        /// <summary>
        /// Used to determine how quickly the list "snaps back" into view if scrolled
        /// out-of-bounds.
        /// </summary>
        private float snapBackFactor = 0.2f;

        /// <summary>
        /// The current velocity of the list.
        /// </summary>
        private float velocity = 0.0f;

        /// <summary>
        /// Use to figure out how fast the list shall scroll when dragged and
        /// released.
        /// </summary>
        private float dragDistanceFactor = 50.0f;

        /// <summary>
        /// The scroll velocity is clamped to this value to prevent irratic
        /// behavior.
        /// </summary>
        private float maxVelocity = 500.0f;

        /// <summary>
        /// Determines how fast the scroll velocity decreases when the list is
        /// no longer being dragged.
        /// </summary>
        private float deaccelerationFactor = 0.9f;
        #endregion

        public CustomListBox()
        {
            InitializeComponent();

            mouseDownEventHandler = new MouseEventHandler(MouseDownHandler);
            mouseUpEventHandler = new MouseEventHandler(MouseUpHandler);
            mouseMoveEventHandler = new MouseEventHandler(MouseMoveHandler);
        }

        protected virtual void FireListItemClicked(Control listItem)
        {
            if (ListItemClicked != null)
                ListItemClicked(this, listItem, selectedItemsMap[listItem]);
        }

        /// <summary>
        /// When resizing the list box the iternal items has to 
        /// be resized as well.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            itemsPanel.Location = ClientRectangle.Location;
            itemsPanel.Width = ClientSize.Width;
            foreach (Control itemControl in itemsPanel.Controls)
            {
                itemControl.Width = itemsPanel.ClientSize.Width;
            }
        }

        /// <summary>
        /// Since the list items can be made up of any number of
        /// <c>Control</c>s this helper method determines the
        /// "root" <c>Control</c> of the list item when a mouse
        /// event has been fired.
        /// </summary>
        private Control GetListItemFromEvent(Control sender)
        {
            if (sender == this || sender == itemsPanel)
                return null;
            else
            {
                while (sender.Parent != itemsPanel)
                    sender = sender.Parent;

                return sender;
            }
        }

        /// <summary>
        /// Handles mouse down events by storing a set of <c>Point</c>s that
        /// will be used to determine animation velocity.
        /// </summary>
        private void MouseDownHandler(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseIsDown = true;
                // Since list items move when scrolled all locations are 
                // in absolute values (meaning local to "this" rather than to "sender".
                mouseDownPoint = Utils.GetAbsolute(new Point(e.X, e.Y), sender as Control, this);
                previousPoint = mouseDownPoint;
            }
        }

        /// <summary>
        /// Handles mouse move events by scrolling the moved distance.
        /// </summary>
        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // The lock flag prevents too frequent rendering of the 
                // controls, something which becomes an issue of Devices
                // because of their limited performance.
                if (!renderLockFlag)
                {
                    renderLockFlag = true;
                    Point absolutePoint = Utils.GetAbsolute(new Point(e.X, e.Y), sender as Control, this);
                    int delta = absolutePoint.Y - previousPoint.Y;
                    draggedDistance = delta;
                    //Trace.WriteLine("Scroll item in delta " + draggedDistance);
                    ScrollItems(delta);
                    previousPoint = absolutePoint;
                }
            }
        }

        /// <summary>
        /// Handles the mouse up event and determines if the list needs to animate 
        /// after it has been "released".
        /// </summary>
        private void MouseUpHandler(object sender, MouseEventArgs e)
        {
            // Only calculate a animation velocity and start animating if the mouse
            // up event occurs directly after the mouse move.
            if (renderLockFlag)
            {
                velocity = Math.Min(Math.Max(dragDistanceFactor * draggedDistance, -maxVelocity), maxVelocity);
                draggedDistance = 0;
                DoAutomaticMotion();
            }

            if (e.Button == MouseButtons.Left)
            {
                // If the mouse was lifted from the same location it was pressed down on 
                // then this is not a drag but a click, do item selection logic instead
                // of dragging logic.
                if (Utils.GetAbsolute(new Point(e.X, e.Y), sender as Control, this).Equals(mouseDownPoint))
                {
                    // Get the list item (regardless if it was a child Control that was clicked). 
                    Control item = GetListItemFromEvent(sender as Control);
                    if (item != null)
                    {
                        bool newState = UnselectEnabled ? !selectedItemsMap[item] : true;
                        if (newState != selectedItemsMap[item])
                        {
                            selectedItemsMap[item] = newState;
                            FireListItemClicked(item);

                            if (!MultiSelectEnabled && selectedItemsMap[item])
                            {
                                foreach (Control listItem in itemsPanel.Controls)
                                {
                                    if (listItem != item)
                                        selectedItemsMap[listItem] = false;
                                }
                            }

                            // After "normal" selection rules have been applied,
                            // check if the list items affected are IExtendedListItems
                            // and call the appropriate methods if it is so.
                            foreach (Control listItem in itemsPanel.Controls)
                            {
                                if (listItem is IExtendedListItem)
                                    (listItem as IExtendedListItem).SelectedChanged(selectedItemsMap[listItem]);
                            }

                            // Force a re-layout of all items
                            LayoutItems();
                        }
                    }
                }
            }
            mouseIsDown = false;
        }

        private bool CanScrolling(int offset)
        {
            bool retVal = true;
            if(offset > 0)
            {
                retVal = itemsPanel.Top != 0;
            }
            else
            {
                retVal = itemsPanel.Top + itemsPanel.Height != ClientSize.Height;
            }

            return retVal;
        }
        /// <summary>
        /// Scrolls the member itemsPanel by offset.
        /// </summary>
        private void ScrollItems(int offset)
        {
			if (!EnableSmothScrolling)
				return;

            // Do not waste time if this is a pointless scroll...
            if (offset == 0)
                return;

            if (CanScrolling(offset))
            {
                SuspendLayout();
                itemsPanel.Top += offset;
                itemsPanel.Top = itemsPanel.Top > 0 ? 0 : itemsPanel.Top;

                if (itemsPanel.Height > ClientSize.Height)
                {
                    int bottomPosition = itemsPanel.Top + itemsPanel.Height;
                    if (bottomPosition < ClientSize.Height)
                    {
                        itemsPanel.Top += ClientSize.Height - bottomPosition;
                    }
                }

                ResumeLayout(true);
            }
        }

        /// <summary>
        /// Layout the items and make sure they line up properly as they 
        /// can change size during runtime.
        /// </summary>
        public void LayoutItems()
        {
            
            SuspendLayout();
            int top = 0;
            foreach (Control itemControl in itemsPanel.Controls)
            {
                //Debugging.DebugAssistant.Log("Layout items " + top);

                itemControl.Location = new Point(0, top);
                itemControl.Width = itemsPanel.ClientSize.Width;
                top += itemControl.Height;
            }

            itemsPanel.Height = top;
            ResumeLayout(true);
        }

        /// <summary>
        /// Adds a new item to the list box.
        /// </summary>
        public void AddItem(Control control)
        {
            if (!itemsPanel.Controls.Contains(control))
            {
                itemsPanel.Controls.Add(control);
                itemsPanel.Controls.SetChildIndex(control, 0);
                selectedItemsMap.Add(control, false);

                LayoutItems();

                if (control is IExtendedListItem)
                    ((IExtendedListItem)control).PositionChanged(itemsPanel.Controls.Count);

                Utils.SetHandlers(this, mouseDownEventHandler, mouseUpEventHandler, mouseMoveEventHandler);
            }
            else
            {
                throw new ArgumentException("Each item in SmoothListbox must be a unique Control", "control");
            }
        }

        /// <summary>
        /// Removes an item from the list box.
        /// </summary>
        /// <param name="control"></param>
        public virtual void RemoveItem(Control control)
        {
            itemsPanel.Controls.Remove(control);
            selectedItemsMap.Remove(control);

            Utils.RemoveHandlers(control, mouseDownEventHandler, mouseUpEventHandler, mouseMoveEventHandler);

            for (int i = 0; i < itemsPanel.Controls.Count; ++i)
            {
                Control itemControl = itemsPanel.Controls[i];
                if (itemControl is IExtendedListItem)
                {
                    (itemControl as IExtendedListItem).PositionChanged(i);
                }
            }

            LayoutItems();
        }

        /// <summary>
        /// This method resets all items to unselected and 
        /// resets scrolling to the top of the list.
        /// </summary>
        public void Reset()
        {
            velocity = 0;
            itemsPanel.Top = 0;
            foreach (Control control in itemsPanel.Controls)
            {
                if (control is IExtendedListItem)
                    (control as IExtendedListItem).SelectedChanged(false);
                selectedItemsMap[control] = false;
            }

            LayoutItems();
        }

        /// <summary>
        /// This method handles the ticks from the animation timer and also
        /// resets the renderLockFlag.
        /// </summary>
            private void AnimationTick(object sender, EventArgs e)
            {
                //Trace.WriteLine("renderLockFlag = " + renderLockFlag.ToString());
                renderLockFlag = false;
                DoAutomaticMotion();
            }

        /// <summary>
        /// This method calculates the new velocity and the distance the list items
        /// should scroll. It also handles snapback if the list has been 
        /// scrolled out of bounds.
        /// </summary>
        private void DoAutomaticMotion()
        {
            if (!mouseIsDown)
            {
                velocity *= deaccelerationFactor;
                float elapsedTime = animationTimer.Interval / 1000.0f;
                float deltaDistance = elapsedTime * velocity;
                //Trace.WriteLine("elapsed time " + elapsedTime.ToString() + " velocity " + velocity.ToString() + " delta distance " + deltaDistance.ToString());
                // If the velocity induced by the user dragging the list
                // results in a deltaDistance greater than 1.0f pixels 
                // then scroll the items that distance.
                if (Math.Abs(deltaDistance) >= 1.0f)
                    ScrollItems((int)deltaDistance);
                //else
                //{
                //    // If the velocity is not large enough to scroll
                //    // the items we need to check if the list is
                //    // "out-of-bound" and in that case snap it back.
                //    if (itemsPanel.Top != 0)
                //    {
                //        if (itemsPanel.Top > 0)
                //            ScrollItems(-Math.Max(1, (int)(snapBackFactor * (float)(itemsPanel.Top))));
                //        else
                //        {
                //            if (itemsPanel.Height > ClientSize.Height)
                //            {
                //                int bottomPosition = itemsPanel.Top + itemsPanel.Height;
                //                if (bottomPosition < ClientSize.Height)
                //                    ScrollItems(Math.Max(1, (int)(snapBackFactor * (float)(ClientSize.Height - bottomPosition))));
                //            }
                //            else
                //                ScrollItems(Math.Max(1, -((int)(snapBackFactor * (float)itemsPanel.Top))));
                //        }
                //    }
                //}
            }
        }

        #region Public properties

        public int ItemsHeight
        {
            get
            {
                int retVal = 0;
                foreach (Control ctrl in Items)
                {
                    retVal += ctrl.Height;
                }
                return retVal;
            }
            private set {}
        }
        public float SnapBackFactor
        {
            get { return snapBackFactor; }
            set 
            {
                if (value <= 0.0f || value >= 1.0f)
                    throw new ArgumentException("SnapBackFactor must fall within exclusive range 0.0 < value < 1.0", "value");
                snapBackFactor = value; 
            }
        }

        public float DragDistanceFactor
        {
            get { return dragDistanceFactor; }
            set 
            {
                if (value < 1.0f)
                    throw new ArgumentException("DragDistanceFactor must be greater than or equal to 1.0", "value");
                dragDistanceFactor = value; 
            }
        }

        public float MaxVelocity
        {
            get { return maxVelocity; }
            set
            {
                if (value < 1.0f)
                    throw new ArgumentException("MaxVelocity must be greater than or equal to 1.0", "value");
                maxVelocity = value;
            }
        }

        public float DeaccelerationFactor
        {
            get { return deaccelerationFactor; }
            set
            {
                if (value <= 0.0f || value >= 1.0f)
                    throw new ArgumentException("DeaccelerationFactor must fall within exclusive range 0.0 < value < 1.0", "value");
                deaccelerationFactor = value;
            }
        }
		public bool EnableSmothScrolling { get; set; }

        /// <summary>
        /// If set to <c>True</c> multiple items can be selected at the same
        /// time, otherwise a selected item is automatically de-selected when
        /// a new item is selected.
        /// </summary>
        public bool MultiSelectEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// If set to <c>True</c> then the user can explicitly unselect a
        /// selected item.
        /// </summary>
        public bool UnselectEnabled
        {
            get;
            set;
        }

        public ControlCollection Items
        {
            get { return itemsPanel.Controls; }
        }

        public Control[] SelectedItems
        {
            get
            {
                List<Control> selectedItems = new List<Control>();
                foreach (Control key in selectedItemsMap.Keys)
                {
                    if (selectedItemsMap[key])
                        selectedItems.Add(key);
                }
                return selectedItems.ToArray();
            }

            set
            {
                foreach (Control listItem in itemsPanel.Controls)
                {
                    if (listItem is IExtendedListItem)
                    {
                        selectedItemsMap[listItem] = false;
                        (listItem as IExtendedListItem).SelectedChanged(false);
                    }
                }
                foreach (Control item in value)
                {
                    selectedItemsMap[item] = true;
                    FireListItemClicked(item);
                    (item as IExtendedListItem).SelectedChanged(true);
                    // Force a re-layout of all items
                    LayoutItems();
                }
            }
        }


        #endregion
    }
}
