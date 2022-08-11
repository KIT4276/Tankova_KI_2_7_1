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
        private Vector3 _target;

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
                    //Debug.Log("TryGetNeighbor Left");
                    if (leftCell.IsFree)
                    {
                        leftCell.ToHighlightCell();
                        //Debug.Log("leftCell.IsFree");
                    }
                    else
                    {
                        //Debug.Log("else"); 
                        if (leftCell.Pair.GetColor != GetColor &&
                            leftCell.TryGetNeighbor(Left, out var leftOverEnemy) &&
                            leftOverEnemy.IsFree)
                        {
                            //Debug.Log("leftCell.IsNotFree1");
                            if (leftCell.Pair.GetComponent<ChipComponent>() != null)
                            {
                                leftCell.Pair.GetComponent<ChipComponent>().ToHighlightEat();
                                leftOverEnemy.ToHighlightCell();
                                _eatenChip = leftCell.Pair;
                                //Debug.Log("leftCell.IsNotFree2");
                            }
                        }
                    }
                }
                if (cell.TryGetNeighbor(Righ, out var rightCell))
                {
                    //Debug.Log("TryGetNeighbor Righ");
                    if (rightCell.IsFree)
                    {
                        rightCell.ToHighlightCell();
                        //Debug.Log("rightCell.IsFree");
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
                                //Debug.Log("rightCell.IsNotFree");
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
            _target = cell.transform.position;
            StartCoroutine(MoveFromTo(_target));
        }

        private IEnumerator MoveFromTo(Vector3 targetToMove)
        {
            var currentTime = 0f;
            var startPos = transform.position;
            targetToMove = new Vector3(targetToMove.x, startPos.y, targetToMove.z);

            while (currentTime < 2 )
            {
                transform.position = Vector3.Lerp(startPos, targetToMove, currentTime / 2);
                currentTime += _moveSpeed * Time.deltaTime;
                yield return null;
            }

            if (_eatenChip != null)
            {
                var chip = _eatenChip.GetComponent<ChipComponent>();
                TryEat(chip);
            }

            transform.position = targetToMove;
            ChipMove?.Invoke();
        }

        private bool TryEat(ChipComponent enemyChip) 
        {
            if (enemyChip != null && (enemyChip.transform.position.x == _target.x - 1 &&
                enemyChip.transform.position.z == _target.z - 1 || enemyChip.transform.position.x == _target.x + 1 &&
                enemyChip.transform.position.z == _target.z + 1))
            {
                enemyChip.Eat();
                if (enemyChip.GetColor == ColorType.White) WinCheck.Self.DecreaseWhiteHP();
                else WinCheck.Self.DecreaseBlackHP();
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
