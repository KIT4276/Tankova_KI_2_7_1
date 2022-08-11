using System.Collections;
using UnityEngine;

namespace Checkers
{
    public class CameraControl : MonoBehaviour
    {
        [SerializeField]
        [Range(1, 10)]
        private float _rotateTime = 2f;
        public Quaternion _startRotation;

        public static CameraControl Self;

        private void Start()
        {
            Self = this;
        }
        public void CameraViewChange()
        {
            StartCoroutine(CameraRotate());
        }

        private IEnumerator CameraRotate()
        {
            var _start = transform.rotation;
            var target = transform.rotation * new Quaternion(0f, 180f, 0f, 0f);
            yield return Rotate(_start, target, _rotateTime);
        }

        private IEnumerator Rotate(Quaternion start, Quaternion target, float time)
        {
            var currentTime = 0f;
            while (currentTime < time)
            {
                transform.rotation = Quaternion.Lerp(start, target, Mathf.Pow(currentTime - _rotateTime * Time.deltaTime, 2));
                currentTime += Time.deltaTime;
                yield return null;
            }
            transform.rotation = target;
        }
    }
}
