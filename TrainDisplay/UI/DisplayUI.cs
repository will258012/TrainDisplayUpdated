using System;
using System.Collections;
using TrainDisplay.Config;
using TrainDisplay.Utils;
using UnityEngine;

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
        private const byte baseX = 0;
        private static int baseY => Screen.height - screenHeight;
        private static double ratio => screenWidth / 512.0;

        private Texture2D arrowLineTexture;
        private Texture2D arrowTexture;
        private Texture2D circleTexture;

        private static int arrowLineLength;
        private static int arrowLineLengthWithArrow;
        private static int arrowLength;
        private static int arrowHeight;

        //private static IntRect screenRect;
        private static IntRect headerRect;
        private static IntRect bodyRect;

        private static IntRect bodyLineRect;
        private static IntRect bodyForTextRect;
        private static IntRect bodyForSuffixTextRect;
        private static IntRect bodyForTextEngRect;
        private static IntRect bodyForSuffixTextEngRect;

        private static IntRect bodyNextTextRect;
        private static Vector2 bodyNextTextPivot;
        private static IntRect bodyNextHeadTextRect;

        private static IntRect bodyArrowLineRect;

        private readonly GUIStyle forStyle = new GUIStyle();
        private readonly GUIStyle forSuffixStyle = new GUIStyle();
        private readonly GUIStyle forSuffixEngStyle = new GUIStyle();
        private readonly GUIStyle nextStyle = new GUIStyle();
        private readonly GUIStyle nextHeadStyle = new GUIStyle();
        private readonly GUIStyle stationNameStyle = new GUIStyle();
        private readonly GUIStyle stationNameRotatedStyle = new GUIStyle();

        private readonly GUIStyle boxStyle = new GUIStyle();
        private readonly GUIStyle arrowRectStyle = new GUIStyle();
        private readonly GUIStyle circleStyle = new GUIStyle();
        private readonly GUIStyle arrowStyle = new GUIStyle();

        //public string testString = "test";
        public string next = "";
        public string prevText = "";
        public bool stopping = false;
        public Color lineColor = Color.white;

        public bool circular = false;

        private string[] routeStations = { };
        private string[] verticalRouteStations = { };

        private IntRect[] stationNameRects = { };
        private IntRect[] stationNameRotatedRects = { };
        private Vector2[] stationNameRotatedRectPivots = { };
        private Vector2[] stationNameRotatedRectBottoms = { };
        private IntRect[] stationCirclesRects = { };
        private int[] stationNamePositions = { };
        private int itemNumber = 0;

        void Awake()
        {
            boxStyle.normal.background = Texture2D.whiteTexture;
            StartCoroutine(UpdateWidth(true));
        }
        public IEnumerator UpdateWidth(bool forceUpdate = false)
        {
            var width = TrainDisplayConfig.Instance.DisplayWidth;
            if (screenWidth == width && !forceUpdate)
            {
                // 変わってなければ何もしない
                yield break;
            }
            screenWidth = width;

            arrowLineLength = (int)(460 * ratio);
            arrowLineLengthWithArrow = (int)(470 * ratio);
            arrowLength = (int)((arrowLineLengthWithArrow - arrowLineLength) * 3.2);
            arrowHeight = (int)(30 * ratio);

            //screenRect = new IntRect(baseX, baseY, screenWidth, screenHeight);
            headerRect = new IntRect(baseX, baseY, screenWidth, screenHeight / 3);
            bodyRect = new IntRect(baseX, baseY + screenHeight / 3, screenWidth, screenHeight / 3 * 2);

            bodyLineRect = new IntRect(baseX + 100 * ratio, baseY + 5 * ratio, ratio * 40, (screenHeight / 3) - 10 * ratio);
            bodyForTextRect = new IntRect(baseX, baseY + 46 * ratio, ratio * 100, 24 * ratio);
            bodyForSuffixTextRect = new IntRect(baseX + (ratio * 8), baseY + (46 + 24) * ratio, ratio * 84, 18 * ratio);
            bodyForTextEngRect = new IntRect(baseX, baseY + (32 + 28) * ratio, ratio * 100, 24 * ratio);
            bodyForSuffixTextEngRect = new IntRect(baseX + (ratio * 8), baseY + 32 * ratio, ratio * 84, 18 * ratio);

            bodyNextTextRect = new IntRect(baseX + 140 * ratio, baseY + 26 * ratio, ratio * (512 - 140), 70 * ratio);
            bodyNextTextPivot = new Vector2(bodyNextTextRect.x + bodyNextTextRect.width / 2, bodyNextTextRect.y + bodyNextTextRect.height / 2);
            bodyNextHeadTextRect = new IntRect(baseX + (140 + 10) * ratio, baseY + 5 * ratio, ratio * (512 - 140 - 20), 26 * ratio);

            bodyArrowLineRect = new IntRect(baseX + (26 * ratio), baseY + (220 * ratio), arrowLineLengthWithArrow, arrowHeight);

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
            nextStyle.stretchWidth = true;

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
            int maxStartX = arrowLineLengthWithArrow - arrowLineLength;
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
            yield break;
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

        public void updateStationInfoPosition()
        {
            if (itemNumber <= 0)
            {
                return;
            }
            stationNameRects = new IntRect[itemNumber];
            stationNameRotatedRects = new IntRect[itemNumber];
            stationNameRotatedRectPivots = new Vector2[itemNumber];
            stationNameRotatedRectBottoms = new Vector2[itemNumber];
            stationCirclesRects = new IntRect[itemNumber];
            stationNamePositions = PositionUtils.positionsJustifyCenter(arrowLineLength, arrowLineLength / 6, itemNumber);
            for (int i = 0; i < itemNumber; i++)
            {
                stationNameRects[i] = new IntRect(
                    baseX + (26 * ratio) + stationNamePositions[i],
                    baseY + (106 * ratio),
                    arrowLineLength / 6,
                    104 * ratio
                );

                stationNameRotatedRects[i] = PositionUtils.GetRotatedRect(stationNameRects[i]);
                stationNameRotatedRectPivots[i] = new Vector2(stationNameRects[i].x + stationNameRects[i].width / 2, stationNameRects[i].y + stationNameRects[i].height / 2);
                stationNameRotatedRectBottoms[i] = new Vector2(stationNameRotatedRects[i].x, stationNameRotatedRects[i].y + stationNameRotatedRects[i].height);

                stationCirclesRects[i] = new IntRect(
                    baseX + (26 * ratio) + stationNamePositions[i] + (arrowLineLength / 6 / 2 - (13 * ratio)),
                    baseY + ((220 + 2) * ratio),
                    26 * ratio,
                    26 * ratio
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
            }
            else
            {
                shownForText = routeStations[routeStations.Length - 1];
            }

            if (ForTextPositionIsOnTop(circular))
            {
                GUI.Label(bodyForTextEngRect, shownForText, forStyle);
                GUI.Label(bodyForSuffixTextEngRect, circular ? TrainDisplayMod.translation.GetTranslation("A_TD_FOR_CIRCULAR", true) : TrainDisplayMod.translation.GetTranslation("A_TD_FOR", true), forSuffixEngStyle);
            }
            else
            {
                GUI.Label(bodyForTextRect, shownForText, forStyle);
                GUI.Label(bodyForSuffixTextRect, circular ? TrainDisplayMod.translation.GetTranslation("A_TD_FOR_CIRCULAR", true) : TrainDisplayMod.translation.GetTranslation("A_TD_FOR", true), forSuffixStyle);
            }

            GUI.Label(bodyNextHeadTextRect, stopping ? TrainDisplayMod.translation.GetTranslation("A_TD_NOW_STOPPING_AT", true) : TrainDisplayMod.translation.GetTranslation("A_TD_NEXT", true), nextHeadStyle);

            // 次の駅
            string nextDisplayedText = stopping ? prevText : next;
            if (TrainDisplayConfig.Instance.IsTextShrinked)
            {
                float scale = Math.Min(1, 8.0f / nextDisplayedText.Length);
                GUIUtility.ScaleAroundPivot(new Vector2(scale, 1), bodyNextTextPivot);
            }
            GUI.Label(bodyNextTextRect, nextDisplayedText, nextStyle);
            GUI.matrix = Matrix4x4.identity;

            // ボディ
            GUI.backgroundColor = Color.white;
            GUI.Box(bodyRect, "", boxStyle);

            GUI.backgroundColor = lineColor;
            GUI.Box(bodyArrowLineRect, "", arrowRectStyle);

            int startIndex = circular ? Array.FindIndex(routeStations, (str) => str == prevText) : Math.Min(Array.FindIndex(routeStations, (str) => str == prevText), routeStations.Length - itemNumber);
            int nowItemIndex = 0;
            string displayLanguage = TrainDisplayMod.translation.DisplayLanguage._uniqueName;
            for (int i = 0; i < itemNumber; i++)
            {
                int routeIndex = new LoopCounter(routeStations.Length, startIndex + i).Value;
                string sta = routeStations[routeIndex];
                if (sta == prevText)
                {
                    nowItemIndex = i;
                }

                if (displayLanguage == "ja" || displayLanguage == "zh")
                {
                    GUI.Label(
                        stationNameRects[i],
                        verticalRouteStations[routeIndex],
                        stationNameStyle
                    );
                }
                else
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
                new IntRect(
                    baseX + (26 * ratio) + stationNamePositions[nowItemIndex] + (arrowLineLength / 6 / 2 - arrowLength / 2) + (stopping ? 0 : (circleDiff / 2)),
                    baseY + (220 * ratio),
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
