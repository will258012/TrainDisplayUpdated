using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrainDisplay.Utils
{
    class LoopCounter
    {
        private int innerValue;
        private int innerMax;

        public int max => innerMax;
        public int Value {
            get
            {
                return innerValue;
            }
            set
            {
                innerValue = value >= 0 ? value % max : ((-value / max + 1) * max + value) % max;
            }
        }

        public LoopCounter(int max, int start = 0)
        {
            innerMax = max;
            Value = start;
        }

        public static LoopCounter operator ++(LoopCounter lc)
        {
            return new LoopCounter(lc.max, lc.Value + 1);
        }

        public static LoopCounter operator --(LoopCounter lc)
        {
            return new LoopCounter(lc.max, lc.Value - 1);
        }

        public static LoopCounter operator +(LoopCounter lc, int i)
        {
            return new LoopCounter(lc.max, lc.Value + i);
        }

        public static LoopCounter operator +(int i, LoopCounter lc)
        {
            return lc + i;
        }

        public static LoopCounter operator -(LoopCounter lc, int i)
        {
            return new LoopCounter(lc.max, lc.Value - i);
        }

        public static bool operator <(LoopCounter lc, int i)
        {
            return lc.Value < i;
        }

        public static bool operator >(LoopCounter lc, int i)
        {
            return lc.Value > i;
        }

        public static bool operator <(LoopCounter lc1, LoopCounter lc2)
        {
            return lc1.Value < lc2.Value;
        }

        public static bool operator >(LoopCounter lc1, LoopCounter lc2)
        {
            return lc1.Value > lc2.Value;
        }

        public static bool operator <=(LoopCounter lc, int i)
        {
            return lc.Value <= i;
        }

        public static bool operator >=(LoopCounter lc, int i)
        {
            return lc.Value >= i;
        }

        public static bool operator <=(LoopCounter lc1, LoopCounter lc2)
        {
            return lc1.Value <= lc2.Value;
        }

        public static bool operator >=(LoopCounter lc1, LoopCounter lc2)
        {
            return lc1.Value >= lc2.Value;
        }

        public static bool operator ==(LoopCounter lc, int i)
        {
            return lc.Value == i;
        }

        public static bool operator !=(LoopCounter lc, int i)
        {
            return lc.Value != i;
        }

        public static bool operator ==(int i, LoopCounter lc)
        {
            return lc.Value == i;
        }

        public static bool operator !=(int i, LoopCounter lc)
        {
            return lc.Value != i;
        }

        public static bool operator ==(LoopCounter lc1, LoopCounter lc2)
        {
            return lc1.Value == lc2.Value;
        }

        public static bool operator !=(LoopCounter lc1, LoopCounter lc2)
        {
            return lc1.Value != lc2.Value;
        }

        public override string ToString()
        {
            return innerValue + "";
        }
    }
}
