using System;
using System.Collections;
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

        protected override void Start()
        {
            base.Start();

            _collider = GetComponent<CapsuleCollider>();
            _collider.isTrigger = true;

            PairChipWithCell();

            OnFocusEventHandler += ToHighlight;
        }
        private void PairChipWithCell() // тупо спёрто у Кирилла
        {
            if (Pair != null)
            {
                Pair.Pair = null;
            }
            if (Physics.Raycast(transform.position, Vector3.down, out var hit, 5f))
            {
                var cell = hit.collider.gameObject.GetComponent<CellComponent>();
                if (cell != null)
                {
                    Pair = cell;
                    cell.Pair = this;
                }
            }
        }
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
                PossibleMoves();
            }
            else
            {
                IsSelected = true;
                AddAdditionalMaterial(_selectMaterial, 2);
                PossibleMoves();
            }
        }
        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////////
        /// </summary>

        private void PossibleMoves()
        {
            if (Pair is CellComponent cell)
            {
                NeighborType a = NeighborType.TopLeft;
                NeighborType b = NeighborType.TopRight;

                if (GetColor == ColorType.Black)
                {
                    a = NeighborType.BottomLeft;
                    b = NeighborType.BottomRight;
                }


                if (cell.TryGetNeighbor(a, out var leftCell))
                {
                    if (leftCell.IsFree)
                    {
                        leftCell.ToHighlightCell();
                    }
                    else
                    {
                        if (leftCell.Pair.GetColor != GetColor &&
                            leftCell.TryGetNeighbor(a, out var leftOverEnemy) &&
                            leftOverEnemy.IsFree)
                        {
                            (leftCell.Pair as ChipComponent)?.ToHighlightEat();
                            leftOverEnemy.ToHighlightCell();
                        }
                    }
                }
                if (cell.TryGetNeighbor(b, out var rightCell))
                {
                    if (rightCell.IsFree)
                    {
                        rightCell.ToHighlightCell();
                    }
                    else
                    {
                        if (rightCell.Pair.GetColor != GetColor &&
                            rightCell.TryGetNeighbor(b, out var rightOverEnemy) &&
                            rightOverEnemy.IsFree)
                        {
                            (rightCell.Pair as ChipComponent)?.ToHighlightEat();
                            rightOverEnemy.ToHighlightCell();
                        }
                    }
                }
            }
        }

        public void ToHighlightEat()
        {
            if (_cantEatColor)
            {
                _cantEatColor = false;
                AddAdditionalMaterial(_canEatMaterial, 3);
            }
            else
            {
                _cantEatColor = true;
                RemoveAdditionalMaterial(3);
            }
        }

        public void MoveToNewCell(CellComponent cell) // это вариант Кирилла, он лучше, чем у меня
        {
            ToSelectChip();
            var newPosition = cell.transform.position;
            StartCoroutine(Move(newPosition));
        }

        private IEnumerator Move(Vector3 newPosition)
        {
            var lerpTime = 0f;
            var startPos = transform.position;
            newPosition = new Vector3(newPosition.x, startPos.y , newPosition.z);
            while (lerpTime < _moveTime / 2)
            {
                transform.position = Vector3.Lerp(startPos, newPosition, lerpTime / _moveTime);
                lerpTime += _moveSpeed * Time.deltaTime;
                yield return null;
            }

            TryEat();
            lerpTime = 0f;
            newPosition = new Vector3(newPosition.x, startPos.y, newPosition.z);
            startPos = transform.position;
            while (lerpTime < _moveTime / 2)
            {
                transform.position = Vector3.Lerp(startPos, newPosition, lerpTime / _moveTime);
                lerpTime += _moveSpeed * Time.deltaTime;
                yield return null;
            }

            transform.position = newPosition;
            PairChipWithCell();
            OnChipMove?.Invoke();
        }
        private bool TryEat()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out var hitChip, 5))
            {
                var chip = hitChip.collider.GetComponent<ChipComponent>();
                if (chip != null)
                {
                    chip.DestroyChip();
                    if (chip.GetColor == ColorType.White) WinCheck.Self.WhiteHP -= 1;
                    else WinCheck.Self.BlackHP -= 1;
                    return true;
                }
            }
            return false;
        }

        public void DestroyChip()
        {
            Pair.Pair = null;
            Pair = null;
            gameObject.SetActive(false);
        }
    }
}
