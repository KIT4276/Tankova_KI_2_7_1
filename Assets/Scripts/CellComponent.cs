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
        private CellComponent[] _blackCells;

        //public GameObject _pairOfCell; // фишка, которая стоит на клетке

        public static CellComponent Self;

        protected override void Start()
        {
            Self = this;

            _blackCells = FindObjectsOfType<CellComponent>(); //заполнили массив снова, потому что я не знаю, как его вытащить из GameManager

            _neighbors = new Dictionary<NeighborType, CellComponent>();

            base.Start();

            OnFocusEventHandler += ToHighlight;
        }

        public void SetNeibor(BaseClickComponent pair, ColorType colorType) // Пробуем
        {
            _neighbors.Clear();

            var KeyCel = pair.gameObject;

            var topLeftPosition = KeyCel.transform.position + new Vector3(-1, 0, 1);
            var topRightPosition = KeyCel.transform.position + new Vector3(1, 0, 1);
            var bottomLeftPosition = KeyCel.transform.position + new Vector3(1, 0, -1);
            var bottomRightPosition = KeyCel.transform.position + new Vector3(-1, 0, -1);
           
            if (colorType == ColorType.White)
            {
                foreach (var item in _blackCells) 
                {
                    Debug.Log("topRightPosition  " + topRightPosition);
                    Debug.Log("item  " + item.transform.position);
                    if (item.transform.position == topLeftPosition) // почему не заходит сюда?
                    {
                        _neighbors.Add(NeighborType.TopLeft, item);
                    }
                    if (item.transform.position == topRightPosition)
                    {
                        _neighbors.Add(NeighborType.TopRight, item);
                    }
                }
            }
            if (colorType == ColorType.Black)
            {
                foreach (var item in _blackCells)
                {
                    if (item.transform.position == bottomLeftPosition)
                    {
                        _neighbors.Add(NeighborType.BottomLeft, item);
                    }
                    if (item.transform.position == bottomRightPosition)
                    {
                        _neighbors.Add(NeighborType.BottomRight, item);
                    }
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