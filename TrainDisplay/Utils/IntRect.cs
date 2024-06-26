﻿using System;
using UnityEngine;

namespace TrainDisplay.Utils
{
    public class IntRect
    {
        public int x;
        public int y;
        public int width;
        public int height;

        public IntRect(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
        public IntRect(double x, double y, double width, double height)
        {
            this.x = (int)x;
            this.y = (int)y;
            this.width = (int)width;
            this.height = (int)height;
        }
        public override string ToString()
        {
            return $"(x: {x}, y: {y}, width: {width}, height: {height})";
        }
        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return width;
                    case 3: return height;
                    default: throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    case 2: width = value; break;
                    case 3: height = value; break;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        public static implicit operator Rect(IntRect intRect)
        {
            return new Rect(intRect.x, intRect.y, intRect.width, intRect.height);
        }

        public static implicit operator IntRect(Rect rect)
        {
            return new IntRect((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
        }
    }
}
