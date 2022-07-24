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

        public bool GetCheck
        {
            get { return IsChecked; }
            set { IsChecked = value; }
        }

        public int WhiteHP
        {
            get { return _whiteHP; }
            set { _whiteHP = value; }
        }
        public int BlackHP
        {
            get { return _blackHP; }
            set { _blackHP = value; }
        }
        public bool CheckEleminationWin()
        {
            if (_blackHP == 0 || _whiteHP == 0)
            {
                return IsChecked = true;
            }
            else return false;
        }
    }
}
