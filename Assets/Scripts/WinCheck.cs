using UnityEngine;

namespace Checkers
{
    public class WinCheck : MonoBehaviour 
    {
        public static WinCheck Self;

        private bool IsChecked = false;

        private int _blackHP = 12;
        private int _whiteHP = 12;

        private void Start()
        {
            Self = this;
        }
        private void LateUpdate()
        {
            CheckEleminationWin();
        }
        public void DecreaseBlackHP()
        {
            _blackHP--;
            Debug.Log("Чёрных шашек осталось "+_blackHP);
        }
        public void DecreaseWhiteHP()
        {
            _whiteHP--;
            Debug.Log("Белых шашек осталось " + _whiteHP);
        }

        public void CheckEleminationWin()
        {
            if (_blackHP <= 0) Debug.Log("Белые выиграли!");
            if (_whiteHP <= 0) Debug.Log("Чёрные выиграли!");
        }
    }
}
