using UnityEngine;
using UnityEngine.EventSystems;

using TDShooter.Managers;

namespace TDShooter.UI
{
    public class NextLevelButton : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            LevelManager.LoadNextLevel();
        }
    }
}