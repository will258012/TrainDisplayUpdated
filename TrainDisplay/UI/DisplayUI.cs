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

        private static int screenWidth = 500;
        private static int screenHeight = 300;

        private readonly Rect screenRect = new Rect(0, Screen.height - screenHeight, screenWidth, screenHeight);
        private readonly Rect testRect = new Rect(0, Screen.height - screenHeight, screenWidth, screenHeight);

        private GUIStyle style = new GUIStyle();

        public string testString = "test";

        private void OnGUI()
        {
            GUI.Box(screenRect, "");

            style.fontSize = 20;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.UpperCenter;

            GUI.Label(testRect, testString, style);
        }
    }
}
