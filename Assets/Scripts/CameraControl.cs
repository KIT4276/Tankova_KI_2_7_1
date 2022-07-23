using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Checkers
{
    public  class CameraControl : MonoBehaviour
    {
        [SerializeField][Range(1,10)]
        private  float _rotateTime = 2f;
        Quaternion _start;
        Quaternion _target;

        public static CameraControl Self;

        private void Start()
        {
            Self = this;
        }
        public  void CameraViewChange() // почему при следующем ходе не срабатывает поворот?
        {
            _start = transform.rotation;
            _target = new Quaternion(0f, 180f, 0f, 0f);

            StartCoroutine(CameraRotate()); 
        }

        private IEnumerator CameraRotate()  
        {
            
            yield return Rotate(_start, _target, _rotateTime); 
        }

        private IEnumerator Rotate(Quaternion start, Quaternion target, float time)
        {
            var currentTime = 0f;
            while (currentTime < time)
            {
                transform.rotation = Quaternion.Lerp(start, target, Mathf.Pow(currentTime - _rotateTime*Time.deltaTime, 2));
               // transform.rotation = Quaternion.Lerp(start, _target, 1 - (time - currentTime) / time);   // если делать так,
               // то в этом проекте скорость вращения уменьшается во время этого вращения, а в отдельном проекте работает отлично.
               // почему?

                currentTime += Time.deltaTime;
                yield return null;
            }
            transform.rotation = target;
        }

    }
}
