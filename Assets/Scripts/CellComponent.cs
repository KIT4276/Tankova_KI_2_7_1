using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public class CellComponent : BaseClickComponent
    {
        private Dictionary<NeighborType, CellComponent> _neighbors;

        public bool IsFree => Pair == null;
        public bool CanBeOccupied => !_canSelect && IsFree;
        private bool _canSelect = true;

        protected override void Start()
        {
            _neighbors = new Dictionary<NeighborType, CellComponent>();
            base.Start();
            OnFocusEventHandler += ToHighlight;

            AddNeighbor(NeighborType.TopLeft, new Vector3(-1, 0, 1));
            AddNeighbor(NeighborType.TopRight, new Vector3(1, 0, 1));
            AddNeighbor(NeighborType.BottomLeft, new Vector3(-1, 0, -1));
            AddNeighbor(NeighborType.BottomRight, new Vector3(1, 0, -1));
        }

        private void AddNeighbor(NeighborType neighborType, Vector3 direction)
        {
            if (Physics.Raycast(transform.position, direction, out var hit, 2))
            {
                var cell = hit.collider.GetComponent<CellComponent>();
                //if (_neighbors.ContainsKey(neighborType)) // пока такой костыль, чтобы не вылетало, пока не найду, как починить
               // {
                    if (cell != null)
                    {
                        Debug.Log("cell " + cell);
                        Debug.Log("neighborType " + neighborType);
                        _neighbors.Add(neighborType, cell); // тут ошибка "данный ключ отсутствует в словаре"
                    }
               // }
            }
        }

        public bool TryGetNeighbor(NeighborType type, out CellComponent component) => _neighbors.TryGetValue(type, out component);

        public override void OnPointerEnter(PointerEventData eventData)
        {
            CallBackEvent(this, true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            CallBackEvent(this, false);
        }

        public void ToHighlightCell()
        {
            if (_canSelect)
            {
                _canSelect = false;
                AddAdditionalMaterial(_selectMaterial, 2);
            }
            else
            {
                _canSelect = true;
                RemoveAdditionalMaterial(1);
                RemoveAdditionalMaterial(2);
            }
        }
    }

    /// <summary>
    /// Тип соседа клетки
    /// </summary>
    public enum NeighborType : byte
    {
        /// <summary>
        /// Клетка сверху и слева от данной
        /// </summary>
        TopLeft,
        /// <summary>
        /// Клетка сверху и справа от данной
        /// </summary>
        TopRight,
        /// <summary>
        /// Клетка снизу и слева от данной
        /// </summary>
        BottomLeft,
        /// <summary>
        /// Клетка снизу и справа от данной
        /// </summary>
        BottomRight
    }
}