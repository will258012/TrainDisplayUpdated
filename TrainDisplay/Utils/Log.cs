using UnityEngine;

namespace TrainDisplay.Utils
{
    class Log
    {
        private static readonly string PREPEND_TAG = "[TrainDisplay]: ";
        public static void Message(string s)
        {
            Debug.Log(PREPEND_TAG + s);
        }

        public static void Error(string s)
        {
            Debug.LogError(PREPEND_TAG + s);
        }
        public static void Warning(string s)
        {
            Debug.LogWarning(PREPEND_TAG + s);
        }
    }
}
