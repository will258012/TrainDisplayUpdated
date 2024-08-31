using AlgernonCommons;
using AlgernonCommons.Translation;
using ColossalFramework.Globalization;
using System;
using System.Collections;
using TrainDisplay.Settings;
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
                    instance = TrainDisplayMain.Instance.gameObject.AddComponent<DisplayUI>();
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
        // Name: use for display
        // ID: use for positioning
        public string nextStation_Name = "";
        public ushort nextStation_ID;
        public string prevStation_Name = "";
        public ushort prevStation_ID;
        private string[] RouteStationsName = { };
        private string[] vertical_RouteStationsName = { };
        private ushort[] RouteStationsID = { };
        public bool IsStopping = false;
        public bool IsCircular = false;
        public Color lineColor = Color.white;

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
            var width = TrainDisplaySettings.DisplayWidth;
            if (screenWidth == width && !forceUpdate)
            {
                // If not changed, do nothing
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

        internal void UpdateRouteStations(string[] newRouteStationsName, ushort[] newRouteStationsID, bool IsCircular)
        {
            RouteStationsName = newRouteStationsName;
            RouteStationsID = newRouteStationsID;
            this.IsCircular = IsCircular;

            vertical_RouteStationsName = new string[RouteStationsName.Length];
            for (int i = 0; i < RouteStationsName.Length; i++)
            {
                vertical_RouteStationsName[i] = AStringUtils.CreateVerticalString(RouteStationsName[i], 4);
            }

            itemNumber = Math.Min(RouteStationsName.Length, 6);
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
            stationNamePositions = PositionUtils.PositionsJustifyCenter(arrowLineLength, arrowLineLength / 6, itemNumber);
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
            switch (Translations.CurrentLanguage)
            {
                case "en-EN":
                    return true;
                case "ja-JP":
                    return false;
                case "zh-CN":
                    return !circular;
                case "default":
                    switch (LocaleManager.instance.language)
                    {
                        case "en":
                            return true;
                        case "ja":
                            return false;
                        case "zh":
                            return !circular;
                    }
                    break;
            }
            return true;
        }
        private bool showWarning = false;
        private string warningText;
        public void ShowWarning(string warningText)
        {
            showWarning = true;
            this.warningText = warningText;
            Logging.KeyMessage($"Displaying Stopped: {warningText}");
        }
        private void OnGUI()
        {
            //GUI.Box(screenRect, "");

            // Header
            GUI.backgroundColor = new Color(0.16f, 0.16f, 0.16f);
            GUI.Box(headerRect, "", boxStyle);

            if (showWarning)
            {
                GUI.backgroundColor = Color.white;
                GUI.Box(bodyRect, "", boxStyle);

                GUIStyle warningBoxStyle = new GUIStyle(GUI.skin.box)
                {
                    fontSize = (int)(50 * ratio),
                };
                warningBoxStyle.normal.textColor = Color.white;
                warningBoxStyle.alignment = TextAnchor.UpperLeft;

                GUI.Box(headerRect, Translations.Translate("WARNTITLE"), warningBoxStyle);

                GUIStyle warningLabelStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = (int)(25 * ratio)
                };
                warningLabelStyle.normal.textColor = Color.black;
                warningLabelStyle.wordWrap = true;

                GUI.Label(bodyRect, string.Format(Translations.Translate("WARNDETAIL"), warningText, "\n"), warningLabelStyle);

                float buttonWidth = 150 * (float)ratio;
                float buttonHeight = 40 * (float)ratio;

                if (GUI.Button(new Rect(10 * (float)ratio, Screen.height - buttonHeight - 10 * (float)ratio, buttonWidth, buttonHeight), Translations.Translate("IGNORE")))
                {
                    showWarning = false;
                }

                if (GUI.Button(new Rect(20 * (float)ratio + buttonWidth, Screen.height - buttonHeight - 10 * (float)ratio, buttonWidth, buttonHeight), Translations.Translate("HIDE")))
                {
                    showWarning = enabled = false;
                }
                return;
            }

            GUI.backgroundColor = lineColor;
            GUI.Box(bodyLineRect, "", boxStyle);

            string shownForText;
            if (IsCircular)
            {
                //int index = Array.FindIndex(RouteStationsName, (str) => str == prevText);
                var index = Array.FindIndex(RouteStationsID, (id) => id == prevStation_ID);
                index = ((index / 3) + 1) * 3;
                if (index > RouteStationsName.Length - 3)
                {
                    index = 0;
                }
                shownForText = RouteStationsName[index];
            }
            else
            {
                shownForText = RouteStationsName[RouteStationsName.Length - 1];
            }

            if (ForTextPositionIsOnTop(IsCircular))
            {
                GUI.Label(bodyForTextEngRect, shownForText, forStyle);
                GUI.Label(bodyForSuffixTextEngRect, IsCircular ? Translations.Translate("FOR_CIRCULAR") : Translations.Translate("FOR"), forSuffixEngStyle);
            }
            else
            {
                GUI.Label(bodyForTextRect, shownForText, forStyle);
                GUI.Label(bodyForSuffixTextRect, IsCircular ? Translations.Translate("FOR_CIRCULAR") : Translations.Translate("FOR"), forSuffixStyle);
            }

            GUI.Label(bodyNextHeadTextRect, IsStopping ? Translations.Translate("NOW_STOPPING_AT") : Translations.Translate("NEXT"), nextHeadStyle);

            // Next station
            string nextDisplayedText = IsStopping ? prevStation_Name : nextStation_Name;
            if (TrainDisplaySettings.IsTextShrinked)
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
            var Index2 = Array.FindIndex(RouteStationsID, (id) => id == prevStation_ID);
            //int startIndex = circular ? Array.FindIndex(RouteStationsName, (str) => str == prevText) : Math.Min(Array.FindIndex(RouteStationsName, (str) => str == prevText), RouteStationsName.Length - itemNumber);
            int startIndex = IsCircular ? Index2 : Math.Min(Index2, RouteStationsID.Length - itemNumber);
            int nowItemIndex = 0;
            string displayLanguage = Translations.CurrentLanguage;
            string gameLanguage = LocaleManager.instance.language;
            for (int i = 0; i < itemNumber; i++)
            {
                int routeIndex = new LoopCounter(RouteStationsID.Length, startIndex + i).Value;

                var IndexId = RouteStationsID[routeIndex];
                if (IndexId == prevStation_ID)
                {
                    nowItemIndex = i;
                }
                /*
                string sta = RouteStationsName[routeIndex];
                if (sta == prevText)
                {
                    nowItemIndex = i;
                }
                */
                switch (displayLanguage)
                {
                    case "ja-JP":
                    case "zh-CN":
                        GUI.Label(
                            stationNameRects[i],
                            vertical_RouteStationsName[routeIndex],
                            stationNameStyle
                        );
                        break;
                    case "default":
                        switch (gameLanguage)
                        {
                            case "ja":
                            case "zh":
                                GUI.Label(
                                    stationNameRects[i],
                                    vertical_RouteStationsName[routeIndex],
                                    stationNameStyle
                                );
                                break;
                        }
                        break;
                    default:
                        GUIUtility.RotateAroundPivot(-90, stationNameRotatedRectPivots[i]);
                        GUI.Label(
                            stationNameRotatedRects[i],
                            RouteStationsName[routeIndex],
                            stationNameRotatedStyle
                        );
                        GUI.matrix = Matrix4x4.identity;
                        break;
                }
            }

            GUI.backgroundColor = Color.white;
            for (int i = 0; i < itemNumber; i++)
            {
                if (IsStopping && i == nowItemIndex)
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
                    baseX + (26 * ratio) + stationNamePositions[nowItemIndex] + (arrowLineLength / 6 / 2 - arrowLength / 2) + (IsStopping ? 0 : (circleDiff / 2)),
                    baseY + (220 * ratio),
                    arrowLength,
                    arrowHeight
                ),
                "",
                arrowStyle
            );
        }
    }
}

