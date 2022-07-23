using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Checkers
{
    public class CameraControl : MonoBehaviour
    {
        [SerializeField] [Range(60, 600)] private float _rotationTime = 60;
        [SerializeField] [Range(1, 100)] private float _rotationSpeed = 1;

        public static CameraControl Self;
    }
}
