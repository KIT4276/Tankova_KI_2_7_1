using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Checkers
{
    public class TrtiggerComponent : MonoBehaviour
    {
        [SerializeField] [Tooltip("Рядовой триггер")]
        private bool _isPrivate;

        [SerializeField][Tooltip("Триггер для дамок")]
        private bool _isKing;

        private void Start()
        {
            var collider = GetComponent<Collider>();
            collider.isTrigger = true;
        }
        private void OnTriggerStay(Collider other)
        {
            //if (other.GetComponent<BallComponent>() == null) return; // почему тут всегда null?

            if (_isPrivate)
            {
               
            }

            if (_isKing)
            {
                Debug.Log("_isKing");
            }

        }

    }
}
