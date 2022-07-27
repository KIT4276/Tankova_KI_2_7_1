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

        public CellComponent _cell;


        public GameObject _pairOfChip; // клетка, на которой стоит фишка

        public static ChipComponent Self;

        //private void OnTriggerEnter(Collider other)
        //{
        //    Debug.Log("OnTriggerEnter Chip");
        //    Pair = other.gameObject;
        //    Debug.Log("Pair for chip " + Pair);

        //    //if (TryGetComponent(out other))
        //    //{
        //    //    _pairOfChip = other.gameObject;



        //    //    var cell = _pairOfChip.gameObject.GetComponent<CellComponent>();// почему в тут null?
        //    //    if (cell != null)
        //    //    {
        //    //        Debug.Log("cell" + cell);

        //    //        //if (cell.GetColor == _color) // почему в cell.GetColor null?
        //    //        // {
        //    //        Pair = cell.gameObject;
        //    //        //}
        //    //    }

        //    //}
        //}


        protected override void Start()
        {
            base.Start();

            Self = this;
            //if (_cell is CellComponent cell)
            //{
            //    _cell = Pair.GetComponent<CellComponent>();  // сомнения
            //}

            _collider = GetComponent<CapsuleCollider>();
            _collider.isTrigger = true;

           // PairChipWithCell();

            //OnFocusEventHandler += ToHighlight;
        }
       // private void PairChipWithCell() // тупо спёрто у Кирилла, и это не норм
        //{
            //if (Pair != null)
            //{
            //    Pair.Pair = null;
            //}
            //if (Physics.Raycast(transform.position, Vector3.down, out var hit, 5f))
            //{
            //    var cell = hit.collider.gameObject.GetComponent<CellComponent>();
            //    if (cell != null)
            //    {
            //        Pair = cell;
            //        cell.Pair = this;
            //    }
            //}

        //}

        
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
            if (_cell is CellComponent cell)
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
                        Debug.Log("leftCell.IsFree");
                    }
                    else
                    {
                        //if (leftCell.Pair.GetColor != GetColor &&
                        //    leftCell.TryGetNeighbor(a, out var leftOverEnemy) &&
                        //    leftOverEnemy.IsFree)
                        //{
                        //    (leftCell.Pair as ChipComponent)?.ToHighlightEat();
                        //    leftOverEnemy.ToHighlightCell();
                        //    Debug.Log("leftCell.Is not Free");
                        //}
                    }
                }
                if (cell.TryGetNeighbor(b, out var rightCell))
                {
                    if (rightCell.IsFree)
                    {
                        rightCell.ToHighlightCell();
                        Debug.Log("rightCell.IsFree");
                    }
                    else
                    {
                        //if (rightCell.Pair.GetColor != GetColor &&
                        //    rightCell.TryGetNeighbor(b, out var rightOverEnemy) &&
                        //    rightOverEnemy.IsFree)
                        //{
                        //    (rightCell.Pair as ChipComponent)?.ToHighlightEat();
                        //    rightOverEnemy.ToHighlightCell();
                        //    Debug.Log("leftCell.Is not Free");
                        //}
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
            //PairChipWithCell(); 
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
