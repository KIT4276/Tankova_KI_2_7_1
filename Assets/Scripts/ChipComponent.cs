using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public class ChipComponent : BaseClickComponent
    {
        [SerializeField] [Range(1, 5)] 
        private float _moveSpeed = 1;
        private bool _cantEat = true;
        private CellComponent _cell;
        public event Action ChipMove;

        private BaseClickComponent _eatenChip;

        protected override void Start()
        {
            base.Start();

            if (_cell is CellComponent cell)
            {
                _cell = Pair.GetComponent<CellComponent>();  // сомнения
            }
        }
     
        public override void OnPointerEnter(PointerEventData eventData)
        {
            CallBackEvent((CellComponent)_cell, true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            CallBackEvent((CellComponent)_cell, false);
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

                NeighborType Left = NeighborType.TopLeft;
                NeighborType Righ = NeighborType.TopRight;

                if (GetColor == ColorType.Black)
                {
                    Left = NeighborType.BottomRight;
                    Righ = NeighborType.BottomLeft;
                }

                if (cell.TryGetNeighbor(Left, out var leftCell))
                {
                    Debug.Log("TryGetNeighbor Left");
                    if (leftCell.IsFree)
                    {
                        leftCell.ToHighlightCell();
                        Debug.Log("leftCell.IsFree");
                    }
                    else
                    {
                        Debug.Log("else"); // дальше этого момента после первых ходов не заходит
                        if (leftCell.Pair.GetColor != GetColor &&
                            leftCell.TryGetNeighbor(Left, out var leftOverEnemy) &&
                            leftOverEnemy.IsFree)
                        {
                            Debug.Log("leftCell.IsNotFree1");
                            if (leftCell.Pair.GetComponent<ChipComponent>() != null)
                            {
                                leftCell.Pair.GetComponent<ChipComponent>().ToHighlightEat();
                                leftOverEnemy.ToHighlightCell();
                                _eatenChip = leftCell.Pair;
                                Debug.Log("leftCell.IsNotFree2");
                            }
                        }
                    }
                }
                if (cell.TryGetNeighbor(Righ, out var rightCell))
                {
                    Debug.Log("TryGetNeighbor Righ");
                    if (rightCell.IsFree)
                    {
                        rightCell.ToHighlightCell();
                        Debug.Log("rightCell.IsFree");
                    }
                    else
                    {
                        if (rightCell.Pair.GetColor != GetColor &&
                            rightCell.TryGetNeighbor(Righ, out var rightOverEnemy) &&
                            rightOverEnemy.IsFree)
                        {
                            if (rightCell.Pair.GetComponent<ChipComponent>() != null)
                            {
                                rightCell.Pair.GetComponent<ChipComponent>().ToHighlightEat();
                                rightOverEnemy.ToHighlightCell();
                                _eatenChip = rightCell.Pair;
                                Debug.Log("rightCell.IsNotFree");
                            }
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

            if (_eatenChip != null)
            {
                var chip = _eatenChip.GetComponent<ChipComponent>();
                chip.TryEat(chip);
            }

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
            ChipMove?.Invoke();
        }

        private bool TryEat(ChipComponent enemyChip) 
        {
            if (enemyChip != null)
            {
                enemyChip.Eat();
                if (enemyChip.GetColor == ColorType.White) WinCheck.Self.WhiteHP -= 1;
                else WinCheck.Self.BlackHP -= 1;
                return true;
            }
            return false;
        }

        public void Eat()
        {
            gameObject.SetActive(false);
        }
    }
}
