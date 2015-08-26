using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Drawing;

namespace OutlookAddin.OutlookUI
{
        public class VistaMenuItems : CollectionBase
        {

            private VistaMenuControl owner;

            public VistaMenuItems(
                   VistaMenuControl c
                ) : base()
            {
                this.owner = c;
            }
            public VistaMenuItems() : base()
            {
            }

            public VistaMenuItem this[int idx]
            {
                get
                {
                    return (VistaMenuItem)InnerList[idx];
                }
            }
            public VistaMenuItem Add(
                   string sText
                )
            {
                VistaMenuItem aclb = new VistaMenuItem(owner);
                aclb.Text = sText;
                InnerList.Add(aclb);
                owner.CalcMenuSize();
                return aclb;

            }
            public VistaMenuItem Add(
                   string sText,
                   string sDescription
                )
            {
                VistaMenuItem aclb = new VistaMenuItem(owner);
                aclb.Text = sText;
                aclb.Description = sDescription;
                InnerList.Add(aclb);
                owner.CalcMenuSize();
                return aclb;

            }
            public VistaMenuItem Add(
                string sText,
                string sDescription,
                Image img
                )
            {
                VistaMenuItem btn = new VistaMenuItem(owner);
                btn.Text = sText;
                btn.Description = sDescription;
                btn.Image = img;

               
                InnerList.Add(btn);
                owner.CalcMenuSize();
                
                return btn;
                
            }
            public VistaMenuItem Add(
                string sText,
                Image img
                )
            {
                VistaMenuItem btn = new VistaMenuItem(owner);
                btn.Text = sText;
                btn.Image = img;
               
                InnerList.Add(btn);
                owner.CalcMenuSize();
                return btn;

            }
            public void Add(VistaMenuItem btn)
            {
                
                List.Add(btn);
                btn.Owner = this.owner;
                owner.CalcMenuSize();
              
                
            }
            public int IndexOf(object o)
            {
                return InnerList.IndexOf(o);
            }
            protected override void OnInsertComplete(int index, object value)
            {
                VistaMenuItem btn = (VistaMenuItem)value;
                btn.Owner = this.owner;
                owner.CalcMenuSize();
                base.OnInsertComplete(index, value);
            }
            protected override void OnSetComplete(int index, object oldValue, object newValue)
            {
                VistaMenuItem btn = (VistaMenuItem)newValue;
                btn.Owner = this.owner;
                owner.CalcMenuSize();
                base.OnSetComplete(index, oldValue, newValue);
            }
            public VistaMenuControl Owner
            {
                get
                {
                    return this.owner;
                }
            }
            protected override void OnClearComplete()
            {
                owner.CalcMenuSize();   
                base.OnClearComplete();
            }
        
        
        }
        
}
