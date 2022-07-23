using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public class ChipComponent : BaseClickComponent
    {
        [SerializeField] [Range(1, 10)] 
        private float _moveTime = 1;
        private bool _cantEat = true;
        public static ChipComponent Self;

        public event Action ChipMove;

        protected override void Start()
        {
            base.Start();
            Self = this;
            PairChipWithCell();
            OnFocusEventHandler += Highlight;
            OnClickEventHandler += HighlightSelected;
        }

        private void PairChipWithCell()// тупо спёрто у Кирилла.
        {
            if (Pair != null)
            {
                Pair.Pair = null;
            }
            if (Physics.Raycast(transform.position, Vector3.down, out var hit, 5))
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

        public void Move(CellComponent cell) 
        {
            var target = cell.transform.position;
            StartCoroutine(Moving( _moveTime, target));
        }
        private IEnumerator Moving(float time, Vector3 target)
        {
            var startPos = transform.position;
            target = new Vector3(target.x, startPos.y, target.z);
            yield return MoveFromTo(transform.position, target, _moveTime);

            TryToEat();

            transform.position = target;
            PairChipWithCell();
            ChipMove?.Invoke();
        }
        private IEnumerator MoveFromTo(Vector3 start, Vector3 target, float time)
        {
            var currentTime = 0f;
            while (currentTime < time)
            {
                transform.position = Vector3.Lerp(transform.position, target, 1 - (time - currentTime) / time);
                currentTime += Time.deltaTime;
                yield return null;
            }
            transform.position = target;

        }

        public void PossibleMoves(BaseClickComponent component) //что именно не работает?
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
                    if (leftCell.isEmpty) leftCell.HighlightSelected(component);

                    else if (leftCell.Pair.GetColor != GetColor && leftCell.TryGetNeighbor(a, out var leftOverEnemy) && leftOverEnemy.isEmpty)
                    {
                        (leftCell.Pair as ChipComponent)?.SetEatMaterial();
                        leftOverEnemy.HighlightSelected(component);
                    }

                }
                if (cell.TryGetNeighbor(b, out var rightCell))
                {
                    if (rightCell.isEmpty)
                    {
                        rightCell.HighlightSelected(component);
                    }
                    else if (rightCell.Pair.GetColor != GetColor && rightCell.TryGetNeighbor(b, out var rightOverEnemy) && rightOverEnemy.isEmpty)
                    {
                        (rightCell.Pair as ChipComponent)?.SetEatMaterial();
                        rightOverEnemy.HighlightSelected(component);
                    }

                }
            }

        }

        private bool TryToEat() 
        {
            if (Physics.Raycast(transform.position, Vector3.down, out var hitChip, 5))
            {
                var chip = hitChip.collider.GetComponent<ChipComponent>();
                if (chip != null)
                {
                    chip.Eat();
                    return true;
                }
            }

            return false;
        }

        public void SetEatMaterial() 
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
        public void Eat()
        {
            Pair.Pair = null;
            Pair = null;
            gameObject.SetActive(false);
        }
        
    }
}
