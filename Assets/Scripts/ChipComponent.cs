using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public class ChipComponent : BaseClickComponent
    {
        [SerializeField] [Range(1, 5)] private float _moveSpeed = 1;

        public event Action ChipMove;

        private CapsuleCollider _collider;

        private bool _cantEat = true;

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
            if (_cantEat)
            {
                _cantEat = false;
                AddAdditionalMaterial(_canEatMaterial, 3);
            }
            else
            {
                _cantEat = true;
                RemoveAdditionalMaterial(3);
            }
        }

        public void Move(CellComponent cell) 
        {
            ToSelectChip();
            var target = cell.transform.position;
            StartCoroutine(MoveFromTo(target));
        }

        private IEnumerator MoveFromTo(Vector3 target)
        {
            var currentTime = 0f;
            var startPos = transform.position;
            target = new Vector3(target.x, startPos.y, target.z);
            while (currentTime < 2 )
            {
                transform.position = Vector3.Lerp(startPos, target, currentTime / 2);
                currentTime += _moveSpeed * Time.deltaTime;
                yield return null;
            }

            TryEat();

            currentTime = 0f;
            target = new Vector3(target.x, startPos.y, target.z);
            startPos = transform.position;
            while (currentTime < 2 )
            {
                transform.position = Vector3.Lerp(startPos, target, currentTime / 2);
                currentTime += _moveSpeed * Time.deltaTime;
                yield return null;
            }

            transform.position = target;
            PairChipWithCell(); 
            ChipMove?.Invoke();
        }
        private bool TryEat() // почему не работает?
        {
            if (Physics.Raycast(transform.position, Vector3.down, out var hitChip, 5))
            {
                var chip = hitChip.collider.GetComponent<ChipComponent>();
                if (chip != null)
                {
                    chip.Eat();
                    if (chip.GetColor == ColorType.White) WinCheck.Self.WhiteHP -= 1;
                    else WinCheck.Self.BlackHP -= 1;
                    return true;
                }
            }
            return false;
        }

        public void Eat()
        {
            Destroy(this);
        }
    }
}
