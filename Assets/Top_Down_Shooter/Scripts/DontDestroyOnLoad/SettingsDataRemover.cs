using UnityEngine;

namespace TDShooter
{
    public class SettingsDataRemover : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}