using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public class CellComponent : BaseClickComponent
    {
        private Dictionary<NeighborType, CellComponent> _neighbors;

        public bool isEmpty => Pair == null;
        private bool canSelect = true;
        public bool isEmptyToMove => canSelect && isEmpty;
        

        /// <summary>
        /// Возвращает соседа клетки по указанному направлению
        /// </summary>
        /// <param name="type">Перечисление направления</param>
        /// <returns>Клетка-сосед или null</returns>
        public CellComponent GetNeighbors(NeighborType type) => _neighbors[type];

        protected override void Start()
        {
            base.Start();
            _neighbors = new Dictionary<NeighborType, CellComponent>();
            OnFocusEventHandler += Highlight;

            AddNeighbor(NeighborType.TopLeft, new Vector3(-1, 0, 1));
            AddNeighbor(NeighborType.TopRight, new Vector3(1, 0, 1));
            AddNeighbor(NeighborType.BottomLeft, new Vector3(-1, 0, -1));
            AddNeighbor(NeighborType.BottomRight, new Vector3(1, 0, -1));

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
        // тут надо разобраться!
        public bool TryGetNeighbor(NeighborType type, out CellComponent component) => _neighbors.TryGetValue(type, out component);
        public override void OnPointerEnter(PointerEventData eventData)
        {
            CallBackEvent(this, true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            CallBackEvent(this, false);
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