using AlgernonCommons;
using AlgernonCommons.Translation;
using ColossalFramework.Globalization;
using System;
using TrainDisplay.Settings;
using TrainDisplay.Utils;
using UnityEngine;

namespace TrainDisplay.UI
{

    public class DisplayUI : MonoBehaviour
    {
        public static DisplayUI Instance { get; private set; }
        private void OnEnable()
        {
            Logging.Message("DisplayUI is enabled");
            UpdateLayout();
        }

        private void OnDisable()
        {
            Logging.Message("DisplayUI is disabled");
        }
        public static int MaxStationNum { get; set; } = 6;
        public static Vector2 Position { get; set; } = new Vector2(0f, Screen.height - Height);
        public bool IsStopping { get; set; } = false;
        public bool IsCircular { get; set; } = false;
        public Color LineColor { get; set; } = Color.white;
        private bool IsCJK()
        {
            string displayLanguage = Translations.CurrentLanguage;
            string gameLanguage = LocaleManager.instance.language;
            return displayLanguage == "ja-JP" ||
                displayLanguage == "zh-CN" ||
                displayLanguage == "ko-KR" ||
                (displayLanguage == "default" && (
                gameLanguage == "ja" ||
                gameLanguage == "zh" ||
                gameLanguage == "ko"));
        }

        private static float Width = TrainDisplaySettings.DisplayWidth;
        internal static float Height => Width / 16f * 9f;
        private static float Ratio => Width / 512f;

        private float warningButtonWidth;
        private float warningButtonHeight;

        private Texture2D arrowLineTexture;
        private Texture2D arrowTexture;
        private Texture2D circleTexture;

        private int arrowLineLength;
        private int arrowLineLengthWithArrow;
        private int arrowLength;
        private int arrowHeight;

        //private static IntRect screenRect;
        private Rect headerRect;
        private Rect bodyRect;

        private Rect bodyLineRect;
        private Rect CJK_bodyForTextRect;
        private Rect CJK_bodyForSuffixTextRect;
        private Rect NonCJK_bodyForTextRect;
        private Rect NonCJK_bodyForSuffixTextRect;

        private Rect bodyNextTextRect;
        private Vector2 bodyNextTextPivot;
        private Rect bodyNextHeadTextRect;

        private Rect bodyArrowLineRect;

        private Rect warningButtonIgnoreRect;
        private Rect warningButtonHideRect;

        private readonly GUIStyle forStyle = new GUIStyle();
        private readonly GUIStyle CJK_forSuffixStyle = new GUIStyle();
        private readonly GUIStyle NonCJK_forSuffixStyle = new GUIStyle();
        private readonly GUIStyle nextStyle = new GUIStyle();
        private readonly GUIStyle nextHeadStyle = new GUIStyle();
        private readonly GUIStyle CJK_stationNameStyle = new GUIStyle();
        private readonly GUIStyle NonCJK_stationNameStyle = new GUIStyle();

        private readonly GUIStyle boxStyle = new GUIStyle();
        private readonly GUIStyle arrowRectStyle = new GUIStyle();
        private readonly GUIStyle circleStyle = new GUIStyle();
        private readonly GUIStyle arrowStyle = new GUIStyle();

        private readonly GUIStyle warningBoxStyle = new GUIStyle();
        private readonly GUIStyle warningLabelStyle = new GUIStyle();

        //public string testString = "test";
        // Name: use for display
        // ID: use for positioning
        public string nextStation_Name = "";
        public ushort nextStation_ID;
        public string prevStation_Name = "";
        public ushort prevStation_ID;
        private string[] RouteStationsName;
        private string[] vertical_RouteStationsName;
        private ushort[] RouteStationsID;

        private Rect[] CJK_stationNameRects;
        private Vector2[] stationNameCenter;
        private Rect[] NonCJK_stationNameRects;
        private Rect[] stationCirclesRects;
        private int[] stationNamePositions;
        private int itemNumber = 0;

        private Vector2 StationNameAngleOffset;

        private bool showWarning = false;
        private string warningText;

        private bool isDragging;
        private Vector2 dragOffset;

        void Awake()
        {
            Instance = this;
            boxStyle.normal.background = Texture2D.whiteTexture;
            UpdateLayout(true);
        }
        public void UpdateLayout(bool forceUpdate = false)
        {
            var width = (float)TrainDisplaySettings.DisplayWidth;
            if (Math.Abs(Width - width) < Mathf.Epsilon && !forceUpdate)
            {
                // If not changed, do nothing
                return;
            }
            Width = width;

            arrowLineLength = (int)(460f * Ratio);
            arrowLineLengthWithArrow = (int)(470f * Ratio);
            arrowLength = (int)((arrowLineLengthWithArrow - arrowLineLength) * 3.2f);
            arrowHeight = (int)(30f * Ratio);

            warningButtonWidth = 150f * Ratio;
            warningButtonHeight = 40f * Ratio;

            headerRect = new Rect(Position.x, Position.y, Width, Height / 3f);
            bodyRect = new Rect(Position.x, Position.y + (Height / 3f), Width, Height / 3f * 2f);

            bodyLineRect = new Rect(Position.x + (100f * Ratio), Position.y + (5f * Ratio), Ratio * 40f, (Height / 3f) - (10f * Ratio));
            CJK_bodyForTextRect = new Rect(Position.x, Position.y + (46f * Ratio), Ratio * 100f, 24f * Ratio);
            CJK_bodyForSuffixTextRect = new Rect(Position.x + (Ratio * 8f), Position.y + ((46f + 24f) * Ratio), Ratio * 84f, 18f * Ratio);
            NonCJK_bodyForTextRect = new Rect(Position.x, Position.y + ((32f + 28f) * Ratio), Ratio * 100f, 24f * Ratio);
            NonCJK_bodyForSuffixTextRect = new Rect(Position.x + (Ratio * 8f), Position.y + (32f * Ratio), Ratio * 84f, 18f * Ratio);

            bodyNextTextRect = new Rect(Position.x + (140f * Ratio), Position.y + (26f * Ratio), Ratio * (512f - 140f), 70f * Ratio);
            bodyNextTextPivot = new Vector2(bodyNextTextRect.x + (bodyNextTextRect.width / 2f), bodyNextTextRect.y + (bodyNextTextRect.height / 2f));
            bodyNextHeadTextRect = new Rect(Position.x + ((140f + 10f) * Ratio), Position.y + (5f * Ratio), Ratio * (512f - 140f - 20f), 26f * Ratio);

            bodyArrowLineRect = new Rect(Position.x + (26f * Ratio), Position.y + (220f * Ratio), arrowLineLengthWithArrow, arrowHeight);

            warningButtonIgnoreRect = new Rect(10f * Ratio, Screen.height - warningButtonHeight - (10f * Ratio), warningButtonWidth, warningButtonHeight);
            warningButtonHideRect = new Rect((20f * Ratio) + warningButtonWidth, Screen.height - warningButtonHeight - (10f * Ratio), warningButtonWidth, warningButtonHeight);

            forStyle.fontSize = (int)(20f * Ratio);
            forStyle.normal.textColor = Color.white;
            forStyle.alignment = TextAnchor.MiddleCenter;
            forStyle.wordWrap = true;

            CJK_forSuffixStyle.fontSize = (int)(16f * Ratio);
            CJK_forSuffixStyle.normal.textColor = Color.white;
            CJK_forSuffixStyle.alignment = TextAnchor.MiddleRight;
            CJK_forSuffixStyle.wordWrap = true;

            NonCJK_forSuffixStyle.fontSize = (int)(16f * Ratio);
            NonCJK_forSuffixStyle.normal.textColor = Color.white;
            NonCJK_forSuffixStyle.alignment = TextAnchor.MiddleLeft;
            NonCJK_forSuffixStyle.wordWrap = true;

            nextHeadStyle.fontSize = (int)(20f * Ratio);
            nextHeadStyle.normal.textColor = Color.white;
            nextHeadStyle.alignment = TextAnchor.UpperLeft;

            nextStyle.fontSize = (int)(45f * Ratio);
            nextStyle.normal.textColor = Color.white;
            nextStyle.alignment = TextAnchor.MiddleCenter;
            nextStyle.stretchWidth = true;

            CJK_stationNameStyle.fontSize = (int)(20f * Ratio);
            CJK_stationNameStyle.normal.textColor = Color.black;
            CJK_stationNameStyle.alignment = TextAnchor.LowerCenter;

            NonCJK_stationNameStyle.fontSize = (int)(20f * Ratio);
            NonCJK_stationNameStyle.normal.textColor = Color.black;
            NonCJK_stationNameStyle.alignment = TextAnchor.MiddleLeft;
            NonCJK_stationNameStyle.wordWrap = true;

            arrowLineTexture = new Texture2D(arrowLineLength, arrowHeight);

            warningLabelStyle.fontSize = (int)(25f * Ratio);
            warningLabelStyle.normal.textColor = Color.black;
            warningLabelStyle.wordWrap = true;

            warningBoxStyle.fontSize = (int)(50f * Ratio);
            warningBoxStyle.normal.textColor = Color.white;
            warningBoxStyle.alignment = TextAnchor.MiddleLeft;


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
                int dHeight = (int)(arrowLineTexture.height * (arrowLineTexture.width - x) / 10f);
                int yStart = (arrowLineTexture.height - dHeight) / 2;
                for (int j = 0; j < dHeight; j++)
                {
                    arrowLineTexture.SetPixel(x, yStart + j, Color.white);
                }
            }
            arrowLineTexture.Apply();
            arrowRectStyle.normal.background = arrowLineTexture;

            circleTexture = new Texture2D(100, 100);
            float radius = circleTexture.width / 2f;
            for (int x = 0; x < circleTexture.width; x++)
            {
                for (int y = 0; y < circleTexture.height; y++)
                {
                    if (((x - radius) * (x - radius)) + ((y - radius) * (y - radius)) <= radius * radius)
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
                int startX = (int)((1f - (Math.Abs(y - (arrowTexture.height / 2f)) / (arrowTexture.height / 2f))) * maxStartX);
                for (int x = 0; x < arrowTexture.width; x++)
                {
                    if (x < startX || x >= startX + arrowWidth)
                    {
                        arrowTexture.SetPixel(x, y, Color.clear);
                    }
                    else if (x < startX + (arrowWidth * 0.25f) || x >= startX + (arrowWidth * 0.75f))
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

            UpdateStationInfoPosition();
        }

        internal void UpdateRouteStations(string[] newRouteStationsName, ushort[] newRouteStationsID)
        {
            RouteStationsName = newRouteStationsName;
            RouteStationsID = newRouteStationsID;

            vertical_RouteStationsName = new string[RouteStationsName.Length];
            for (int i = 0; i < RouteStationsName.Length; i++)
            {
                vertical_RouteStationsName[i] = AStringUtils.CreateVerticalString(RouteStationsName[i], 4);
            }

            itemNumber = Math.Min(RouteStationsName.Length, MaxStationNum);
            UpdateStationInfoPosition();
        }

        public void UpdateStationInfoPosition()
        {
            if (itemNumber <= 0)
                return;

            CJK_stationNameRects = new Rect[itemNumber];
            NonCJK_stationNameRects = new Rect[itemNumber];
            stationCirclesRects = new Rect[itemNumber];
            stationNameCenter = new Vector2[itemNumber];
            stationNamePositions = PositionUtils.PositionsJustifyCenter(arrowLineLength, arrowLineLength / MaxStationNum, itemNumber);
            StationNameAngleOffset = new Vector2();

            for (int i = 0; i < itemNumber; i++)
            {
                CJK_stationNameRects[i] = new Rect(
                    Position.x + (26f * Ratio) + stationNamePositions[i],
                    Position.y + (106f * Ratio),
                    arrowLineLength / 6f,
                    104f * Ratio
                );
                stationCirclesRects[i] = new Rect(
                    Position.x + (26f * Ratio) + stationNamePositions[i] + ((arrowLineLength / 6f / 2f) - (13f * Ratio)),
                    Position.y + ((220f + 2f) * Ratio),
                    26f * Ratio,
                    26f * Ratio
                );

                NonCJK_stationNameRects[i] = PositionUtils.GetRotatedRect(CJK_stationNameRects[i]);

                stationNameCenter[i] = CJK_stationNameRects[i].center;
            }
            // Calculate offsets for the angle setting
            // Not sure how it was achieved, but at least it works well
            if (!IsCJK() && TrainDisplaySettings.StationNameAngle != -90f && TrainDisplaySettings.StationNameAngle != 90f)
            {
                float angleRad = TrainDisplaySettings.StationNameAngle * Mathf.Deg2Rad;
                StationNameAngleOffset.x = TrainDisplaySettings.StationNameAngle >= 0f ?
                         (NonCJK_stationNameRects[0].width / 2f * (1f - Mathf.Cos(angleRad))) - (NonCJK_stationNameRects[0].height / 2f * Mathf.Sin(angleRad)) :
                          (NonCJK_stationNameRects[0].width / 2f * (Mathf.Cos(angleRad) - 1f)) - (NonCJK_stationNameRects[0].height / 2f * Mathf.Sin(angleRad));
                StationNameAngleOffset.y = NonCJK_stationNameRects[0].height / 2f * Mathf.Cos(angleRad);
            }
        }

        private bool ForTextPositionIsOnTop(bool circular)
        {
            string modLang = Translations.CurrentLanguage;
            string gameLang = LocaleManager.instance.language;

            if (modLang == "ja-JP" || modLang == "ko-KR" || (modLang == "default" && (gameLang == "ja" || gameLang == "ko")))
                return false;

            if (modLang == "zh-CN" || (modLang == "default" && gameLang == "zh"))
                return !circular;

            return true;
        }


        public void ShowWarning(string warningText)
        {
            showWarning = true;
            this.warningText = warningText;
            Logging.Error($"Displaying Stopped: {warningText}");
        }
        private void OnGUI()
        {
            HandleDragEvents();
            // Header
            GUI.backgroundColor = new Color(0.16f, 0.16f, 0.16f);
            GUI.Box(headerRect, "", boxStyle);

            if (showWarning)
            {
                GUI.backgroundColor = Color.white;
                GUI.Box(bodyRect, "", boxStyle);
                GUI.Box(headerRect, Translations.Translate("WARNTITLE"), warningBoxStyle);
                GUI.Label(bodyRect, string.Format(Translations.Translate("WARNDETAIL"), warningText, "\n"), warningLabelStyle);

                if (GUI.Button(warningButtonIgnoreRect, Translations.Translate("IGNORE")))
                {
                    showWarning = false;
                }

                if (GUI.Button(warningButtonHideRect, Translations.Translate("HIDE")))
                {
                    showWarning = enabled = false;
                }
                return;
            }

            GUI.backgroundColor = LineColor;
            GUI.Box(bodyLineRect, "", boxStyle);

            string shownForText;
            if (IsCircular)
            {
                //int index = Array.FindIndex(RouteStationsName, (str) => str == prevText);
                int index = Array.IndexOf(RouteStationsID, prevStation_ID);
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
            if (TrainDisplaySettings.IsTextShrinked)
            {
                float scale = Math.Min(1, 8.0f / shownForText.Length);
                Vector2 pivot = ForTextPositionIsOnTop(IsCircular)
                    ? new Vector2(NonCJK_bodyForTextRect.center.x, NonCJK_bodyForTextRect.center.y)
                    : new Vector2(CJK_bodyForTextRect.center.x, CJK_bodyForTextRect.center.y);
                GUIUtility.ScaleAroundPivot(new Vector2(scale, scale), pivot);
            }

            if (ForTextPositionIsOnTop(IsCircular))
            {
                GUI.Label(NonCJK_bodyForTextRect, shownForText, forStyle);
                GUI.matrix = Matrix4x4.identity;

                GUI.Label(NonCJK_bodyForSuffixTextRect,
                          IsCircular ? Translations.Translate("FOR_CIRCULAR") : Translations.Translate("FOR"),
                          NonCJK_forSuffixStyle);
            }
            else
            {
                GUI.Label(CJK_bodyForTextRect, shownForText, forStyle);
                GUI.matrix = Matrix4x4.identity;

                GUI.Label(CJK_bodyForSuffixTextRect,
                          IsCircular ? Translations.Translate("FOR_CIRCULAR") : Translations.Translate("FOR"),
                          CJK_forSuffixStyle);
            }

            GUI.Label(bodyNextHeadTextRect, IsStopping ? Translations.Translate("NOW_STOPPING_AT") : Translations.Translate("NEXT"), nextHeadStyle);

            // Next station
            string nextDisplayedText = IsStopping ? prevStation_Name : nextStation_Name;
            if (TrainDisplaySettings.IsTextShrinked)
            {
                float scale = Math.Min(1f, 8.0f / nextDisplayedText.Length);
                GUIUtility.ScaleAroundPivot(new Vector2(scale, scale), bodyNextTextPivot);
            }
            GUI.Label(bodyNextTextRect, nextDisplayedText, nextStyle);
            GUI.matrix = Matrix4x4.identity;

            // ボディ
            GUI.backgroundColor = Color.white;
            GUI.Box(bodyRect, "", boxStyle);

            GUI.backgroundColor = LineColor;
            GUI.Box(bodyArrowLineRect, "", arrowRectStyle);
            var Index2 = Array.IndexOf(RouteStationsID, prevStation_ID);
            //int startIndex = circular ? Array.FindIndex(RouteStationsName, (str) => str == prevText) : Math.Min(Array.FindIndex(RouteStationsName, (str) => str == prevText), RouteStationsName.Length - itemNumber);
            int startIndex = IsCircular ? Index2 : Math.Min(Index2, RouteStationsID.Length - itemNumber);
            int nowItemIndex = 0;

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

                if (IsCJK())
                {
                    GUI.Label(CJK_stationNameRects[i], vertical_RouteStationsName[routeIndex], CJK_stationNameStyle);
                }
                else
                {
                    GUIUtility.RotateAroundPivot(TrainDisplaySettings.StationNameAngle, stationNameCenter[i]);
                    GUI.Label(new Rect(
                                       NonCJK_stationNameRects[i].x + StationNameAngleOffset.x,//What appears to be the y-axis is actually the x-axis due to the rotation.
                                       NonCJK_stationNameRects[i].y + StationNameAngleOffset.y,
                                       NonCJK_stationNameRects[i].width,
                                       NonCJK_stationNameRects[i].height),
                                       RouteStationsName[routeIndex],
                                       NonCJK_stationNameStyle);

                }
                GUI.matrix = Matrix4x4.identity;
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
                new Rect(
                    Position.x + (26f * Ratio) + stationNamePositions[nowItemIndex] + ((arrowLineLength / 6f / 2f) - (arrowLength / 2f)) + (IsStopping ? 0f : (circleDiff / 2f)),
                    Position.y + (220f * Ratio),
                    arrowLength,
                    arrowHeight
                ),
                "",
                arrowStyle
            );
        }
        private void HandleDragEvents()
        {
            var currentEvent = Event.current;

            if (currentEvent.type == EventType.MouseDown &&
                (bodyRect.Contains(currentEvent.mousePosition) || headerRect.Contains(currentEvent.mousePosition)))
            {
                isDragging = true;

                dragOffset = currentEvent.mousePosition - Position;
                currentEvent.Use();
            }

            if (isDragging && currentEvent.type == EventType.MouseDrag)
            {
                Position = currentEvent.mousePosition - dragOffset;
                UpdateLayout(true);
                currentEvent.Use();
            }

            if (currentEvent.type == EventType.MouseUp)
            {
                isDragging = false;
                TrainDisplaySettings.Save();
            }
        }

    }
}

