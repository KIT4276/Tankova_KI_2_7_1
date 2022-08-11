using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Checkers
{
    public class TrtiggerComponent : MonoBehaviour
    {
        [SerializeField, Tooltip("Триггер для белых дамок")]
        private bool _isWhiteKing;

        [SerializeField, Tooltip("Триггер для чёрных дамок")]
        private bool _isBlackKing;

        private BoxCollider _collider;

        private void Start()
        {
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.GetComponent<ChipComponent>() == null) return;

            if (_isBlackKing && other.GetComponent<ChipComponent>().GetColor == ColorType.Black)
            {
                Debug.Log("Чёрные вфиграли!");
            }
            if (_isWhiteKing && other.GetComponent<ChipComponent>().GetColor == ColorType.White)
            {
                Debug.Log("Белые выиграли!");
            }
        }
    }
}
