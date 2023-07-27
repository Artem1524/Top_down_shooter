using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TDShooter.Managers
{
    public class LevelManager : MonoBehaviour
    {
        private const string SHOP_SCENE_PATH = "Top_Down_Shooter/Scenes/Shop";

        private const string LEVEL1_SCENE_PATH = "Top_Down_Shooter/Scenes/Level1";
        private const string LEVEL2_SCENE_PATH = "Top_Down_Shooter/Scenes/Level2";

        public static LevelManager Self { get; private set; }

        private int _currentLevel = 1;

        private void OnEnable()
        {
            //if (Self is null)
            //{
                Self = this;
                // _currentLevel = SettingsManager.GetCurrentLevel();
            //}
        }

        public static void LoadShopLevel()
        {
            SettingsManager.SavePlayerData();
            SceneManager.LoadScene(SHOP_SCENE_PATH);
        }

        public static void LoadNextLevel()
        {
            SettingsManager.SavePlayerData();
            int levelIndex = SettingsManager.GetCurrentLevelIndex();
            LoadLevel(levelIndex);
        }

        private static void LoadLevel(int levelIndex)
        {
            switch (levelIndex)
            {
                case 1:
                    SceneManager.LoadScene(LEVEL1_SCENE_PATH);
                    break;
                case 2:
                default:
                    SceneManager.LoadScene(LEVEL2_SCENE_PATH);
                    break;
            }
        }
    }
}