using System;
using System.Drawing;
using System.Windows.Forms;

namespace VieweD.Helpers.PacketList
{
    // source: http://yacsharpblog.blogspot.com/2008/07/listbox-flicker.html
    public class FlickerFreeListBox : ListBox
    {
        public FlickerFreeListBox()
        {
            SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);
            DrawMode = DrawMode.OwnerDrawFixed;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (Items.Count > 0)
            {
                e.DrawBackground();
                if (e.Font != null)
                    e.Graphics.DrawString(Items[e.Index].ToString(),
                        e.Font,
                        new SolidBrush(e.ForeColor),
                        new PointF(e.Bounds.X, e.Bounds.Y));
            }
            base.OnDrawItem(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Region iRegion = new Region(e.ClipRectangle);
            e.Graphics.FillRegion(new SolidBrush(BackColor), iRegion);
            if (Items.Count > 0)
            {
                for (int i = 0; i < Items.Count; ++i)
                {
                    Rectangle irect = GetItemRectangle(i);
                    if (e.ClipRectangle.IntersectsWith(irect))
                    {
                        if (SelectionMode == SelectionMode.One && SelectedIndex == i
                        || SelectionMode == SelectionMode.MultiSimple && SelectedIndices.Contains(i)
                        || SelectionMode == SelectionMode.MultiExtended && SelectedIndices.Contains(i))
                        {
                            OnDrawItem(new DrawItemEventArgs(e.Graphics, Font,
                                irect, i,
                                DrawItemState.Selected, ForeColor,
                                BackColor));
                        }
                        else
                        {
                            OnDrawItem(new DrawItemEventArgs(e.Graphics, Font,
                                irect, i,
                                DrawItemState.Default, ForeColor,
                                BackColor));
                        }
                        iRegion.Complement(irect);
                    }
                }
            }
            base.OnPaint(e);
        }

        // Added code to hide vertical scrollbar if wanted
        // https://stackoverflow.com/questions/13169900/hide-vertical-scroll-bar-in-listbox-control
        private bool _mShowScroll;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                if (!_mShowScroll)
                    cp.Style = cp.Style & ~0x200000;
                return cp;
            }
        }

        public bool ShowScrollbar
        {
            get { return _mShowScroll; }
            set
            {
                if (value != _mShowScroll)
                {
                    _mShowScroll = value;
                    if (IsHandleCreated)
                        RecreateHandle();
                }
            }
        }

        public int MaximumVisibleItems => Size.Height / (ItemHeight <= 0 ? 8 : ItemHeight);

        /*
        private const int WM_VSCROLL = 277; // Vertical scroll
        private const int SB_ENDSCROLL = 8; // Ends scroll

        public event EventHandler TopIndexChanged;
        private int _oldTopIndex = 0;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_VSCROLL) // && (m.WParam == (IntPtr)SB_ENDSCROLL))
            {
                if (m.WParam == (IntPtr)SB_ENDSCROLL)
                {
                    if ((TopIndex != _oldTopIndex) && (TopIndexChanged != null))
                    {
                        TopIndexChanged(this, EventArgs.Empty);
                        _oldTopIndex = TopIndex;
                    }
                }
            }
            base.WndProc(ref m);
        }
        */
    }
}