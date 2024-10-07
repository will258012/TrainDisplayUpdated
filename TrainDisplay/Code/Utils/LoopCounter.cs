using UnityEngine;
namespace TrainDisplay.Utils
{
    public class LoopCounter
    {
        private int innerValue;

        public int Max { get; }
        public int Value
        {
            get => innerValue;
            set => innerValue = (int)Mathf.Repeat(value, Max);
        }

        public LoopCounter(int max, int start = 0)
        {
            Max = max;
            Value = start;
        }

        public static LoopCounter operator ++(LoopCounter lc) => new LoopCounter(lc.Max, lc.Value + 1);
        public static LoopCounter operator --(LoopCounter lc) => new LoopCounter(lc.Max, lc.Value - 1);
        public static LoopCounter operator +(LoopCounter lc, int i) => new LoopCounter(lc.Max, lc.Value + i);
        public static LoopCounter operator -(LoopCounter lc, int i) => new LoopCounter(lc.Max, lc.Value - i);
        public static bool operator ==(LoopCounter lc, int i) => lc.Value == i;
        public static bool operator !=(LoopCounter lc, int i) => lc.Value != i;
        public static bool operator ==(LoopCounter lc1, LoopCounter lc2) => lc1.Value == lc2.Value;
        public static bool operator !=(LoopCounter lc1, LoopCounter lc2) => lc1.Value != lc2.Value;
        public override string ToString() => innerValue.ToString();
        public override bool Equals(object obj)
        {
            if (obj is LoopCounter lc)
            {
                return innerValue == lc.innerValue && Max == lc.Max;
            }
            return false;
        }
        public override int GetHashCode()
        {
            int hashCode = 540335883;
            hashCode = (hashCode * -1521134295) + Max.GetHashCode();
            hashCode = (hashCode * -1521134295) + Value.GetHashCode();
            return hashCode;
        }
    }
}
