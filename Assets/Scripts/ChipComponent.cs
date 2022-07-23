using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public class ChipComponent : BaseClickComponent
    {
        [SerializeField] [Range(1, 5)] private float _moveTime = 2;
        [SerializeField] [Range(1, 5)] private float _moveSpeed = 1;
        //[SerializeField] [Range(0, 5)] private float _moveHeight = 1; // надо ли оно мне?

        public event Action OnChipMove;

        private CapsuleCollider _collider;

        private bool _cantEatColor = true;

        public override void OnPointerEnter(PointerEventData eventData)
        {
            CallBackEvent((CellComponent)Pair, true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            CallBackEvent((CellComponent)Pair, false);
        }

        public void ToSelectChip()
        {
            if (IsSelected)
            {
                IsSelected = false;
                RemoveAdditionalMaterial(2);
               // ShowAvailableMoves();
            }
            else
            {
                IsSelected = true;
                AddAdditionalMaterial(_selectMaterial, 2);
               // ShowAvailableMoves();
            }
        }
    }
}
