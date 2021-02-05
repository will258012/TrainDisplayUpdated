using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TrainDisplay.Utils
{
    class PositionUtils
    {
        public static int[] positionsJustifyCenter(int screenSize, int itemSize, int num)
        {
            int start = (screenSize - itemSize * num) / 2;
            int[] result = new int[num];

            for (int i = 0; i < num; i++)
            {
                result[i] = start + i * itemSize;
            }

            return result;
        }

        public static int[] positionsSpaceAround(int screenSize, int itemSize, int num)
        {
            int[] result = new int[num];

            for (int i = 0; i < num; i++)
            {
                result[i] = screenSize * (2*i + 1) / (2 * num) - (itemSize / 2);
            }

            return result;
        }

        public static Rect GetRotatedRect(Rect beforeRect)
        {
            float x = (-beforeRect.height / 2);
            float y = (-beforeRect.width / 2);

            x += beforeRect.x + (beforeRect.width / 2);
            y += beforeRect.y + (beforeRect.height / 2);

            return new Rect(x, y, beforeRect.height, beforeRect.width);
        }
    }
}
