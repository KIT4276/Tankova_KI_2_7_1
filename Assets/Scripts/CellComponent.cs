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


        public GameObject _pairOfCell; // фишка, которая стоит на клетке

        protected override void Start()
        {
            _neighbors = new Dictionary<NeighborType, CellComponent>();
            base.Start();

            AddNeighbor(NeighborType.TopLeft, new Vector3(-1, 0, 1));
            AddNeighbor(NeighborType.TopRight, new Vector3(1, 0, 1));
            AddNeighbor(NeighborType.BottomLeft, new Vector3(1, 0, -1));
            AddNeighbor(NeighborType.BottomRight, new Vector3(-1, 0, -1));

            OnFocusEventHandler += ToHighlight;
        }

        private void AddNeighbor(NeighborType neighborType, Vector3 direction)
        {
            if (Physics.Raycast(transform.position, direction, out var hit, 5))
            {
                var cell = hit.collider.GetComponent<CellComponent>();
                if (cell != null)
                {
                    _neighbors.Add(neighborType, cell); 
                }
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

       /// <summary>
        /// Конфигурирование связей клеток
        /// </summary>
		public void Configuration(Dictionary<NeighborType, CellComponent> neighbors)
		{
            if (_neighbors != null) return;
            _neighbors = neighbors;
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