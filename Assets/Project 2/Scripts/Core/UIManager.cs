using DG.Tweening;
using Events;
using TMPro;
using UnityEngine;

namespace Core
{
    public class UIManager : MonoBehaviour
    {
        public TextMeshProUGUI TapToPlayText;

        private void OnEnable()
        {
            GEM.AddListener<GameEvent>(OnLoadLevel, channel: (int)GameEventType.Load);
            GEM.AddListener<GameEvent>(OnLevelEnd, channel: (int)GameEventType.End);
        }
        
        private void OnDisable()
        {
            GEM.RemoveListener<GameEvent>(OnLoadLevel, channel: (int)GameEventType.Load);
            GEM.RemoveListener<GameEvent>(OnLevelEnd, channel: (int)GameEventType.End);
        }
        
        private void OnLoadLevel(GameEvent evt)
        {
            var rect = TapToPlayText.rectTransform;
            rect.DOMoveY(rect.position.y - 1000f, 0.25f);
        }
        
        private void OnLevelEnd(GameEvent evt)
        {
            var rect = TapToPlayText.rectTransform;
            rect.DOMoveY(rect.position.y + 1000f, 0.25f);
        }
    }
}