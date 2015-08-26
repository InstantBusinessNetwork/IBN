using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;


namespace OutlookAddin.OutlookUI
{
    public class VistaMenuItem
    {
        public VistaMenuItem(VistaMenuControl c)
        {
            owner = c;
        }
        public VistaMenuItem()
        {
        }

        private VistaMenuControl owner;
        private Image ButtonImage;
        private string sText = "";
        private string sTextContent = "";
        
        private bool bHovering = false;
        private bool bMouseDown = false;
        private bool bDisabled = false;
        private bool bChecked = false;

        private Font fnt = new Font("Visitor TT2 BRK", 9);
        private Font fntC = new Font("Visitor TT2 BRK", 15);

        private Object Tag = null;

        private Color clrOuterBorder = Color.FromArgb(29, 29, 29);
        private Color clrInnerBorder = Color.FromArgb(158, 158, 158);
        
        private Color clrSelectionStartColor = Color.FromArgb(142, 142, 142);
        private Color clrSelectionEndColor = Color.FromArgb(104, 104, 104);
        private Color clrSelectionStartColorStart = Color.FromArgb(74, 74, 74);
        private Color clrSelectionEndColorEnd = Color.FromArgb(106, 106, 106);

        private Color clrCheckedStartColor = Color.FromArgb(142, 142, 142);
        private Color clrCheckedEndColor = Color.FromArgb(80, 80, 80);
        private Color clrCheckedStartColorStart = Color.FromArgb(60, 60, 60);
        private Color clrCheckedEndColorEnd = Color.FromArgb(30, 30, 30);

        private Color clrCaptionFont = Color.YellowGreen;
        private Color clrContentFont = Color.White;
        private Color clrCheckedCaptionFont = Color.YellowGreen;
        private Color clrCheckedContentFont = Color.White;

        private int m_lTop;
        private int m_lLeft;

        public VistaMenuControl Owner
        {
            get
            {
                return this.owner;
            }
            set
            {
                this.owner = value;
            }
        }
        public int Top

        {
            get
            {
                return this.m_lTop;
            }
            set
            {
                this.m_lTop = value;
            }
        }
        public int Left
        {
            get
            {
                return this.m_lLeft;
            }
            set
            {
                this.m_lLeft = value;
            }
        }
        public Color SelectionStartColor
        {
            get
            {
                return this.clrSelectionStartColor;
            }
            set
            {
                this.clrSelectionStartColor = value;
                if (owner != null) owner.Invalidate();
            }
        }
        public Color SelectionEndColor
        {
            get
            {
                return this.clrSelectionEndColor;
            }
            set
            {
                this.clrSelectionEndColor = value;
                if (owner != null) owner.Invalidate();
            }
        }
        public Color SelectionEndColorEnd
        {
            get
            {
                return this.clrSelectionEndColorEnd;
            }
            set
            {
                this.clrSelectionEndColorEnd = value;
                if (owner != null) owner.Invalidate();
            }
        }
        public Color SelectionStartColorStart
        {
            get
            {
                return this.clrSelectionStartColorStart;
            }
            set
            {
                this.clrSelectionStartColorStart = value;
                if (owner != null) owner.Invalidate();
            }
        }
        public Color CheckedStartColor
        {
            get
            {
                return this.clrCheckedStartColor;
            }
            set
            {
                this.clrCheckedStartColor = value;
                if (owner != null) owner.Invalidate();
            }
        }
        public Color CheckedEndColor
        {
            get
            {
                return this.clrCheckedEndColor;
            }
            set
            {
                this.clrCheckedEndColor = value;
                if (owner != null) owner.Invalidate();
            }
        }
        public Color CheckedEndColorEnd
        {
            get
            {
                return this.clrCheckedEndColorEnd;
            }
            set
            {
                this.clrCheckedEndColorEnd = value;
                if (owner != null) owner.Invalidate();
            }
        }
        public Color CheckedStartColorStart
        {
            get
            {
                return this.clrCheckedStartColorStart;
            }
            set
            {
                this.clrCheckedStartColorStart = value;
                if (owner != null) owner.Invalidate();
            }
        }
        public string Text
        {
            get
            {
                return this.sText;
            }
            set
            {
                this.sText = value;
                if (owner != null) owner.Invalidate();
            }
        }
        public string Description
        {
            get
            {
                return this.sTextContent;
            }
            set
            {
                this.sTextContent = value;
                if (owner != null) owner.Invalidate();
            }
        }
       

        public Image Image
        {
            get
            {
                return this.ButtonImage;
            }
            set
            {

                this.ButtonImage = value;
                if (owner != null) owner.Invalidate();

            }
        }
        public Font ContentFont
        {
            get
            {
                return fnt;
            }
            set
            {
                this.fnt = value;
                if (owner != null) owner.Invalidate();
            }
        }
        public Font CaptionFont
        {
            get
            {
                return fntC;
            }
            set
            {
                this.fntC = value;
                if (owner != null) owner.Invalidate();
            }
        }
        public bool Hovering
        {
            get
            {
                return bHovering;
            }
            set
            {
                this.bHovering = value;
            }
        }
        public bool MouseDown
        {
            get
            {
                return bMouseDown;
            }
            set
            {
                this.bMouseDown = value;
            }
        }
        public bool Disabled
        {
            get
            {
                return bDisabled;
            }
            set
            {
                this.bDisabled = value;
				if (owner != null) owner.Invalidate();
            }
        }
        public Object ItemTag
        {
            get
            {
                return this.Tag;
            }
            set
            {
                this.Tag = value;
            }
        }
        public Color InnerBorder
        {
            get
            {
                return this.clrInnerBorder;
            }
            set
            {
                this.clrInnerBorder = value;
                if (owner != null) owner.Invalidate();
            }
        }
        public Color OuterBorder
        {
            get
            {
                return this.clrOuterBorder;
            }
            set
            {
                this.clrOuterBorder = value;
                if (owner != null) owner.Invalidate();
            }
        }
        public Color CaptionColor
        {
            get
            {
                return this.clrCaptionFont;
            }
            set
            {
                this.clrCaptionFont = value;
                if (owner != null) owner.Invalidate();
            }
        }
        public Color ContentColor
        {
            get
            {
                return this.clrContentFont;
            }
            set
            {
                this.clrContentFont = value;
                if (owner != null) owner.Invalidate();
            }
        }
        public Color CheckedCaptionColor
        {
            get
            {
                return this.clrCheckedCaptionFont;
            }
            set
            {
                this.clrCheckedCaptionFont = value;
                if (owner != null) owner.Invalidate();
            }
        }
        public Color CheckedContentColor
        {
            get
            {
                return this.clrCheckedContentFont;
            }
            set
            {
                this.clrCheckedContentFont = value;
                if (owner != null) owner.Invalidate();
            }
        }
        public bool Checked
        {
            get
            {
                return bChecked;
            }
            set
            {
                this.bChecked = value;
            }
        }

                


    }
}
