using UnityEngine;

namespace Checkers
{
    public class WentToTheKings : MonoBehaviour
    {
        [Tooltip("Цветовая сторона тех, чьи это будут дамки"), SerializeField]
        private ColorType _color;

        public static WentToTheKings Self;
        private BoxCollider _collider;

        private void Start()
        {
            Self = this;
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other != null)
            {
                var chip = other.GetComponent<ChipComponent>();
                if (chip.GetColor == _color)
                {
                    //WinCheck.Self.GetCheck = true;
                }
            }
        }
    }
}
