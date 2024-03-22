using DG.Tweening;
using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        private void Awake()
        {
            DOTween.Init();
        }
    }
}