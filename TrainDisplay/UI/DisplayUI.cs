using UnityEngine;
using System;
using TrainDisplay.Utils;

namespace TrainDisplay.UI
{

    public class DisplayUI : MonoBehaviour
    {
        private static DisplayUI instance;

        public static DisplayUI Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = TrainDisplayMain.instance.gameObject.AddComponent<DisplayUI>();
                }
                return instance;
            }
        }

        private static int screenWidth = 1;
        private static int screenHeight => screenWidth / 16 * 9;
        private static int baseX = 0;
        private static int baseY => Screen.height - screenHeight;
        private static double ratio => screenWidth / 512.0;

        private Texture2D arrowLineTexture;
        private Texture2D arrowTexture;
        private Texture2D circleTexture;

        private static int arrowLineLength;
        private static int arrowLineLengthWithArrow;
        private static int arrowLength;
        private static int arrowHeight;

        private static Rect screenRect;
        private static Rect headerRect;
        private static Rect bodyRect;

        private static Rect bodyLineRect;
        private static Rect bodyForTextRect;
        private static Rect bodyForSuffixTextRect;
        private static Rect bodyForTextEngRect;
        private static Rect bodyForSuffixTextEngRect;

        private static Rect bodyNextTextRect;
        private static Rect bodyNextHeadTextRect;

        private static Rect bodyArrowLineRect;

        private GUIStyle forStyle = new GUIStyle();
        private GUIStyle forSuffixStyle = new GUIStyle();
        private GUIStyle forSuffixEngStyle = new GUIStyle();
        private GUIStyle nextStyle = new GUIStyle();
        private GUIStyle nextHeadStyle = new GUIStyle();
        private GUIStyle stationNameStyle = new GUIStyle();
        private GUIStyle stationNameRotatedStyle = new GUIStyle();

        private GUIStyle boxStyle = new GUIStyle();
        private GUIStyle arrowRectStyle = new GUIStyle();
        private GUIStyle circleStyle = new GUIStyle();
        private GUIStyle arrowStyle = new GUIStyle();

        public string testString = "test";
        public string next = "";
        public string prevText = "";
        public bool stopping = false;
        public Color lineColor = Color.white;

        public bool circular = false;

        private string[] routeStations = { };
        private string[] verticalRouteStations = { };

        private Rect[] stationNameRects = { };
        private Rect[] stationNameRotatedRects = { };
        private Vector2[] stationNameRotatedRectPivots = { };
        private Vector2[] stationNameRotatedRectBottoms = { };
        private Rect[] stationCirclesRects = { };
        private int[] stationNamePositions = { };
        private int itemNumber = 0;

        void Awake()
        {
            boxStyle.normal.background = Texture2D.whiteTexture;

            updateWidth(true);
        }

        public void updateWidth()
        {
            updateWidth(TrainDisplayMain.Config.DisplayWidth);
        }

        public void updateWidth(bool forceUpdate)
        {
            updateWidth(TrainDisplayMain.Config.DisplayWidth, forceUpdate);
        }

        public void updateWidth(int width)
        {
            updateWidth(width, false);
        }

        public void updateWidth(int width, bool forceUpdate)
        {
            if (screenWidth == width && !forceUpdate)
            {
                // 変わってなければ何もしない
                return;
            }
            screenWidth = width;

            arrowLineLength = (int)(460 * ratio);
            arrowLineLengthWithArrow = (int)(470 * ratio);
            arrowLength = (int)((arrowLineLengthWithArrow - arrowLineLength) * 3.2);
            arrowHeight = (int)(30 * ratio);

            screenRect = new Rect(baseX, baseY, screenWidth, screenHeight);
            headerRect = new Rect(baseX, baseY, screenWidth, screenHeight / 3);
            bodyRect = new Rect(baseX, baseY + screenHeight / 3, screenWidth, screenHeight / 3 * 2);

            bodyLineRect = new Rect((int)(baseX + 100 * ratio), (int)(baseY + 5 * ratio), (int)(ratio * 40), (int)((screenHeight / 3) - 10 * ratio));
            bodyForTextRect = new Rect(baseX, (int)(baseY + 46 * ratio), (int)(ratio * 100), (int)(24 * ratio));
            bodyForSuffixTextRect = new Rect(baseX + (int)(ratio * 8), (int)(baseY + (46 + 24) * ratio), (int)(ratio * 84), (int)(18 * ratio));
            bodyForTextEngRect = new Rect(baseX, (int)(baseY + (32 + 28) * ratio), (int)(ratio * 100), (int)(24 * ratio));
            bodyForSuffixTextEngRect = new Rect(baseX + (int)(ratio * 8), (int)(baseY + (32) * ratio), (int)(ratio * 84), (int)(18 * ratio));

            bodyNextTextRect = new Rect((int)(baseX + 140 * ratio), (int)(baseY + 26 * ratio), (int)(ratio * (512 - 140)), (int)(70 * ratio));
            bodyNextHeadTextRect = new Rect((int)(baseX + (140 + 10) * ratio), (int)(baseY + 5 * ratio), (int)(ratio * (512 - 140 - 20)), (int)(26 * ratio));

            bodyArrowLineRect = new Rect((int)(baseX + (26 * ratio)), (int)(baseY + (220 * ratio)), arrowLineLengthWithArrow, arrowHeight);

            forStyle.fontSize = (int)(20 * ratio);
            forStyle.normal.textColor = Color.white;
            forStyle.alignment = TextAnchor.MiddleCenter;
            forStyle.wordWrap = true;

            forSuffixStyle.fontSize = (int)(16 * ratio);
            forSuffixStyle.normal.textColor = Color.white;
            forSuffixStyle.alignment = TextAnchor.MiddleRight;
            forSuffixStyle.wordWrap = true;

            forSuffixEngStyle.fontSize = (int)(16 * ratio);
            forSuffixEngStyle.normal.textColor = Color.white;
            forSuffixEngStyle.alignment = TextAnchor.MiddleLeft;
            forSuffixEngStyle.wordWrap = true;

            nextHeadStyle.fontSize = (int)(20 * ratio);
            nextHeadStyle.normal.textColor = Color.white;
            nextHeadStyle.alignment = TextAnchor.UpperLeft;

            nextStyle.fontSize = (int)(45 * ratio);
            nextStyle.normal.textColor = Color.white;
            nextStyle.alignment = TextAnchor.MiddleCenter;

            stationNameStyle.fontSize = (int)(20 * ratio);
            stationNameStyle.normal.textColor = Color.black;
            stationNameStyle.alignment = TextAnchor.LowerCenter;

            stationNameRotatedStyle.fontSize = (int)(20 * ratio);
            stationNameRotatedStyle.normal.textColor = Color.black;
            stationNameRotatedStyle.alignment = TextAnchor.MiddleLeft;
            stationNameRotatedStyle.wordWrap = true;

            arrowLineTexture = new Texture2D(arrowLineLength, arrowHeight);
            for (int x = 0; x < arrowLineTexture.width; x++)
            {
                for (int y = 0; y < arrowLineTexture.height; y++)
                {
                    arrowLineTexture.SetPixel(x, y, Color.clear);
                }
            }

            for (int x = 0; x < arrowLineTexture.width - 10; x++)
            {
                for (int y = 0; y < arrowLineTexture.height; y++)
                {
                    arrowLineTexture.SetPixel(x, y, Color.white);
                }
            }

            for (int x = arrowLineTexture.width - 10; x < arrowLineTexture.width; x++)
            {
                int dHeight = arrowLineTexture.height * (arrowLineTexture.width - x) / 10;
                int yStart = (arrowLineTexture.height - dHeight) / 2;
                for (int j = 0; j < dHeight; j++)
                {
                    arrowLineTexture.SetPixel(x, yStart + j, Color.white);
                }
            }
            arrowLineTexture.Apply();
            arrowRectStyle.normal.background = arrowLineTexture;

            circleTexture = new Texture2D(100, 100);
            int radius = circleTexture.width / 2;
            for (int x = 0; x < circleTexture.width; x++)
            {
                for (int y = 0; y < circleTexture.height; y++)
                {
                    if ((x - radius) * (x - radius) + (y - radius) * (y - radius) <= radius * radius)
                    {
                        circleTexture.SetPixel(x, y, Color.white);
                    }
                    else
                    {
                        circleTexture.SetPixel(x, y, Color.clear);
                    }
                }
            }
            circleTexture.Apply();
            circleStyle.normal.background = circleTexture;

            arrowTexture = new Texture2D(arrowLength, arrowHeight);
            int maxStartX = (arrowLineLengthWithArrow - arrowLineLength);
            int arrowWidth = arrowLength - maxStartX;
            for (int y = 0; y < arrowTexture.height; y++)
            {
                int startX = (int)((1 - Math.Abs(y - arrowTexture.height / 2.0) / (arrowTexture.height / 2.0)) * maxStartX);
                for (int x = 0; x < arrowTexture.width; x++)
                {
                    if (x < startX || x >= startX + arrowWidth)
                    {
                        arrowTexture.SetPixel(x, y, Color.clear);
                    }
                    else if (x < startX + arrowWidth * 0.25 || x >= startX + arrowWidth * 0.75)
                    {
                        arrowTexture.SetPixel(x, y, Color.white);
                    }
                    else
                    {
                        arrowTexture.SetPixel(x, y, Color.red);
                    }

                }
            }
            arrowTexture.Apply();
            arrowStyle.normal.background = arrowTexture;

            updateStationInfoPosition();
        }

        public void UpdateRouteStations(string[] newRouteStations, bool circular)
        {
            routeStations = newRouteStations;
            this.circular = circular;

            verticalRouteStations = new string[routeStations.Length];
            for (int i = 0; i < routeStations.Length; i++)
            {
                verticalRouteStations[i] = AStringUtils.CreateVerticalString(routeStations[i], 4);
            }

            itemNumber = Math.Min(routeStations.Length, 6);
            updateStationInfoPosition();
        }

        public void updateStationInfoPosition() {
            if (itemNumber <= 0)
            {
                return;
            }
            stationNameRects = new Rect[itemNumber];
            stationNameRotatedRects = new Rect[itemNumber];
            stationNameRotatedRectPivots = new Vector2[itemNumber];
            stationNameRotatedRectBottoms = new Vector2[itemNumber];
            stationCirclesRects = new Rect[itemNumber];
            stationNamePositions = PositionUtils.positionsJustifyCenter(arrowLineLength, arrowLineLength / 6, itemNumber);
            for (int i = 0; i < itemNumber; i++)
            {
                stationNameRects[i] = new Rect(
                    (int)(baseX + (26 * ratio)) + stationNamePositions[i],
                    (int)(baseY + (106 * ratio)),
                    arrowLineLength / 6,
                    (int)(104 * ratio)
                );

                stationNameRotatedRects[i] = PositionUtils.GetRotatedRect(stationNameRects[i]);
                stationNameRotatedRectPivots[i] = new Vector2(stationNameRects[i].x + stationNameRects[i].width / 2, stationNameRects[i].y + stationNameRects[i].height / 2);
                stationNameRotatedRectBottoms[i] = new Vector2(stationNameRotatedRects[i].x, stationNameRotatedRects[i].y + stationNameRotatedRects[i].height);

                stationCirclesRects[i] = new Rect(
                    (int)(baseX + (26 * ratio)) + stationNamePositions[i] + (arrowLineLength / 6 / 2 - (int)(13 * ratio)),
                    (int)(baseY + ((220 + 2) * ratio)),
                    (int)(26 * ratio),
                    (int)(26 * ratio)
                );
            }
        }

        private bool ForTextPositionIsOnTop(bool circular)
        {
            switch (TrainDisplayMod.translation.DisplayLanguage._uniqueName)
            {
                case "en":
                    return true;
                case "ja":
                    return false;
                case "zh":
                    return !circular;
            }
            return true;
        }

        private void OnGUI()
        {
            //GUI.Box(screenRect, "");

            // ヘッダー
            GUI.backgroundColor = new Color(0.16f, 0.16f, 0.16f);
            GUI.Box(headerRect, "", boxStyle);

            GUI.backgroundColor = lineColor;
            GUI.Box(bodyLineRect, "", boxStyle);

            string shownForText;
            if (circular)
            {
                int index = Array.FindIndex(routeStations, (str) => str == prevText);
                index = ((index / 3) + 1) * 3;
                if (index > routeStations.Length - 3)
                {
                    index = 0;
                }
                shownForText = routeStations[index];
            } else
            {
                shownForText = routeStations[routeStations.Length - 1];
            }

            if (ForTextPositionIsOnTop(circular))
            {
                GUI.Label(bodyForTextEngRect, shownForText, forStyle);
                GUI.Label(bodyForSuffixTextEngRect, circular ? TrainDisplayMod.translation.GetTranslation("A_TD_FOR_CIRCULAR", true) : TrainDisplayMod.translation.GetTranslation("A_TD_FOR", true), forSuffixEngStyle);
            } else
            {
                GUI.Label(bodyForTextRect, shownForText, forStyle);
                GUI.Label(bodyForSuffixTextRect, circular ? TrainDisplayMod.translation.GetTranslation("A_TD_FOR_CIRCULAR", true) : TrainDisplayMod.translation.GetTranslation("A_TD_FOR", true), forSuffixStyle);
            }
            
            GUI.Label(bodyNextHeadTextRect, stopping ? TrainDisplayMod.translation.GetTranslation("A_TD_NOW_STOPPING_AT", true) : TrainDisplayMod.translation.GetTranslation("A_TD_NEXT", true), nextHeadStyle);
            GUI.Label(bodyNextTextRect, stopping ? prevText : next, nextStyle);

            // ボディ
            GUI.backgroundColor = Color.white;
            GUI.Box(bodyRect, "", boxStyle);

            GUI.backgroundColor = lineColor;
            GUI.Box(bodyArrowLineRect, "", arrowRectStyle);

            int startIndex = circular ? Array.FindIndex(routeStations, (str) => str == prevText) : Math.Min(Array.FindIndex(routeStations, (str) => str == prevText), routeStations.Length - itemNumber);
            int nowItemIndex = 0;
            for (int i = 0; i < itemNumber; i++)
            {
                int routeIndex = new LoopCounter(routeStations.Length, startIndex + i).Value;
                string sta = routeStations[routeIndex];
                if (sta == prevText) {
                    nowItemIndex = i;
                }

                if (TrainDisplayMod.translation.DisplayLanguage._uniqueName == "ja" || TrainDisplayMod.translation.DisplayLanguage._uniqueName == "zh")
                {
                    GUI.Label(
                        stationNameRects[i],
                        verticalRouteStations[routeIndex],
                        stationNameStyle
                    );
                } else
                {
                    GUIUtility.RotateAroundPivot(-90, stationNameRotatedRectPivots[i]);
                    // GUIUtility.RotateAroundPivot(10, stationNameRotatedRectBottoms[i]);

                    GUI.Label(
                        stationNameRotatedRects[i],
                        routeStations[routeIndex],
                        stationNameRotatedStyle
                    );

                    GUI.matrix = Matrix4x4.identity;
                }
            }

            GUI.backgroundColor = Color.white;
            for (int i = 0; i < itemNumber; i++)
            {
                if (stopping && i == nowItemIndex)
                {
                    continue;
                }
                GUI.Box(
                    stationCirclesRects[i],
                    "",
                    circleStyle
                );
            }
            int circleDiff = stationNamePositions[1] - stationNamePositions[0];
            
            GUI.backgroundColor = Color.white;
            GUI.Box(
                new Rect(
                    (int)(baseX + (26 * ratio)) + stationNamePositions[nowItemIndex] + (arrowLineLength / 6 / 2 - arrowLength / 2) + (stopping ? 0 : (circleDiff / 2)),
                    (int)(baseY + ((220) * ratio)),
                    arrowLength,
                    arrowHeight
                ),
                "",
                arrowStyle
            );

            //GUI.Label(testRect, testString + "\nNext: " + next + " For: " + forText, style);
        }
    }
}
