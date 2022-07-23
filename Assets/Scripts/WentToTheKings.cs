using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Checkers
{
    public class WentToTheKings : MonoBehaviour
    {
        [Tooltip("Цветовая сторона игрового объекта"), SerializeField]
        private ColorType _color;

        public static WentToTheKings instance;
        private BoxCollider _collider;
    }
}
