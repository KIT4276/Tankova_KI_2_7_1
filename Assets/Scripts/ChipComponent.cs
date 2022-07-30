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
        private Rigidbody _rigidbody;
        public event Action ChipMove;
        //private Collider _collider;

        protected override void Start()
        {
            base.Start();

            if (_cell is CellComponent cell)
            {
                _cell = Pair.GetComponent<CellComponent>();  // сомнения
            }

            _rigidbody = GetComponent<Rigidbody>();
            //_collider = GetComponent<Collider>();
            //_collider.isTrigger = true;
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
            CellComponent.Self.SetNeibor(Pair, _color);

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

        private void PossibleMoves() // РАЗОБРАТЬСЯ!
        {
            if (Pair is CellComponent cell)
            {

                NeighborType Left = NeighborType.TopLeft;
                NeighborType Righ = NeighborType.TopRight;

                if (GetColor == ColorType.Black)
                {
                    Left = NeighborType.BottomLeft;
                    Righ = NeighborType.BottomRight;
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
                        if (leftCell.Pair.GetColor != GetColor &&
                            leftCell.TryGetNeighbor(Left, out var leftOverEnemy) &&
                            leftOverEnemy.IsFree)
                        {
                            (leftCell.Pair as ChipComponent)?.ToHighlightEat();
                            leftOverEnemy.ToHighlightCell();
                            Debug.Log("leftCell.Is not Free");
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
                            (rightCell.Pair as ChipComponent)?.ToHighlightEat();
                            rightOverEnemy.ToHighlightCell();
                            Debug.Log("leftCell.Is not Free");
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
            ChipMove?.Invoke();
        }
        private bool TryEat(Collider collider) // избавилась от  Raycast, нужно проверить
        {
                var chip = collider.GetComponent<ChipComponent>();
                if (chip != null)
                {
                    chip.Eat();
                    if (chip.GetColor == ColorType.White) WinCheck.Self.WhiteHP -= 1;
                    else WinCheck.Self.BlackHP -= 1;
                    return true;
                }
            return false;
            
        }

        private void OnCollisionEnter(Collision collision) //пока так, потом надо проверить
        {
           var _collider = collision.collider;

            TryEat(_collider);
        }

        public void Eat()
        {
            Destroy(this);
        }
    }
}
