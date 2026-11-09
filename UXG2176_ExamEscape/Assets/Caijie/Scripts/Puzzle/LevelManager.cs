using UnityEngine;

namespace Caijie
{
    // Temporary placeholder
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }
        public static bool IsPlaying => true;


        //will do later

        public void AddTime(float seconds) { }
        public void ReportProgress(int done, int total) { }
        public void CompleteLevel() { }
    }
}