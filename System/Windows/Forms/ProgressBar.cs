﻿namespace System.Windows.Forms
{
    using System.Drawing;
    using System.Globalization;

    public class ProgressBar : Control
    {
        private readonly SolidBrush backBrush = new SolidBrush(SystemColors.ControlLight);
        private readonly SolidBrush barBrush = new SolidBrush(Color.FromArgb(6, 176, 37));
        private readonly Pen borderPen = new Pen(SystemColors.ActiveBorder);
        private readonly Color defaultForeColor = SystemColors.Highlight;

        private float barX;
        private float barWidth;
        private int marqueeSpeed = 100;
        private int marqueeWidth = 127;
        private int maximum = 100;
        private int minimum;
        private bool rightToLeftLayout;
        private int step = 10;
        private bool updatePos;
        private int value;

        public ProgressBar()
        {
            ForeColor = this.defaultForeColor;
            SetStyle(ControlStyles.UserPaint | ControlStyles.Selectable | ControlStyles.UseTextForAccessibility, false);

            uwfAppOwner.UpdateEvent += AppOwnerOnUpdateEvent;
        }
        public event EventHandler RightToLeftLayoutChanged = delegate { };

        public int MarqueeAnimationSpeed
        {
            get { return marqueeSpeed; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("marqueeSpeed");

                marqueeSpeed = value;
            }
        }
        public int Maximum
        {
            get { return maximum; }
            set
            {
                if (maximum == value)
                    return;

                if (value < 0)
                    throw new ArgumentOutOfRangeException("maximum");

                if (minimum > value)
                    minimum = value;

                maximum = value;

                if (this.value > maximum)
                    this.value = maximum;

                UpdatePos();
            }
        }
        public int Minimum
        {
            get { return minimum; }
            set
            {
                if (minimum == value)
                    return;

                if (value < 0)
                    throw new ArgumentOutOfRangeException("minimum");

                if (maximum < value)
                    maximum = value;

                minimum = value;
                if (this.value < minimum)
                    this.value = minimum;

                UpdatePos();
            }
        }
        public virtual bool RightToLeftLayout
        {
            get { return rightToLeftLayout; }
            set
            {
                if (rightToLeftLayout == value)
                    return;

                rightToLeftLayout = value;
                OnRightToLeftLayoutChanged(EventArgs.Empty);
            }
        }
        public int Step
        {
            get { return step; }
            set { step = value; }
        }
        public ProgressBarStyle Style { get; set; }
        public int Value
        {
            get { return this.value; }
            set
            {
                if (this.value == value)
                    return;

                if (value < minimum || value > maximum)
                    throw new ArgumentOutOfRangeException("value");

                this.value = value;
                UpdatePos();
            }
        }

        protected override Size DefaultSize
        {
            get { return new Size(100, 23); }
        }

        public void Increment(int value)
        {
            if (Style == ProgressBarStyle.Marquee)
                throw new InvalidOperationException("ProgressBarIncrementMarqueeException");

            this.value += value;
            if (this.value < minimum)
                this.value = minimum;
            if (this.value > maximum)
                this.value = maximum;
            UpdatePos();
        }
        public void PerformStep()
        {
            Increment(step);
        }
        public override string ToString()
        {
            return base.ToString() +
                ", Minimum: " + minimum.ToString(CultureInfo.CurrentCulture) +
                ", Maximum: " + maximum.ToString(CultureInfo.CurrentCulture) +
                ", Value: " + value.ToString(CultureInfo.CurrentCulture);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            g.FillRectangle(backBrush, 0, 0, Width, Height);
            g.DrawRectangle(borderPen, 0, 0, Width, Height);
            g.FillRectangle(barBrush, barX, 1, barWidth, Height - 2);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            UpdatePos();
        }
        protected virtual void OnRightToLeftLayoutChanged(EventArgs e)
        {
            if (RightToLeftLayoutChanged == null)
                return;

            RightToLeftLayoutChanged(this, e);
        }

        private void AppOwnerOnUpdateEvent()
        {
            if (updatePos == false && Style != ProgressBarStyle.Marquee)
                return;

            if (Style == ProgressBarStyle.Marquee)
            {
                barWidth = marqueeWidth;
                if (barX < Width)
                    barX = MathHelper.Step(barX, Width, marqueeSpeed / 2f);
                else
                    barX = -marqueeWidth;
                return;
            }

            barX = 0;
            barWidth = MathHelper.Step(barWidth, (value / maximum) * Width, 200);
            updatePos = false;
        }
        private void UpdatePos()
        {
            updatePos = true;
        }
    }
}
