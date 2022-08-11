using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Checkers
{
    public class TrtiggerComponent : MonoBehaviour
    {
        //[SerializeField, Tooltip("Рядовой триггер")]
        //private bool _isPrivate;

        //[SerializeField, Tooltip("Триггер для белых дамок")]
        //private bool _isWhiteKing;

        //[SerializeField, Tooltip("Триггер для чёрных дамок")]
        //private bool _isBlackKing;

        private BoxCollider _collider;

        private void Start()
        {
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;
        }
        //private void OnTriggerEnter(Collider other)
        //{
        //    if (other.GetComponent<CellComponent>() == null) return; 

        //    if (_isPrivate)
        //    {
        //        Debug.Log("_isPrivate");
        //    }

        //    if (_isBlackKing)
        //    {
        //        Debug.Log("Black Went To The Kings");
        //    }
        //    if (_isWhiteKing)
        //    {
        //        Debug.Log("White Went To The Kings");
        //    }
        //}
    }
}
