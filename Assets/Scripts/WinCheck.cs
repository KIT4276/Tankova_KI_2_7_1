using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Checkers
{
    public class WinCheck : MonoBehaviour
    {
        public static WinCheck Self;

        private bool IsChecked = false;

        private int _blackHP = 12;
        private int _whiteHP = 12;
    }
}
