using System.Collections.Generic;
using System.Linq;
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
        private Vector3 ToInt;
        public static CellComponent Self;

        private Vector3 _topLeftPosition;
        private Vector3 _topRightPosition;
        private Vector3 _bottomLeftPosition;
        private Vector3 _bottomRightPosition;


        protected override void Start()
        {
            Self = this;
            base.Start();
            _blackCells = FindObjectsOfType<CellComponent>(); //заполнили массив снова, потому что я не знаю, как его вытащить из GameManager
            _neighbors = new Dictionary<NeighborType, CellComponent>();
            OnFocusEventHandler += ToHighlight;
            
            SetNeibor(this);
        }

        public void SetNeibor(CellComponent currentCell) 
        {
            var KeyCel = currentCell.gameObject;
            var keyCelPosition = KeyCel.transform.position;
            var keyCelPositionInt = FromFloatToInt(keyCelPosition);

            _topLeftPosition = keyCelPositionInt + new Vector3(-1, 0, 1);
            _topRightPosition = keyCelPositionInt + new Vector3(1, 0, 1);
            _bottomLeftPosition = keyCelPositionInt + new Vector3(-1, 0, -1);
            _bottomRightPosition = keyCelPositionInt + new Vector3(1, 0, -1);
                
            foreach (var item in _blackCells)
            {
                if (item.transform.position == _topLeftPosition)
                {
                    _neighbors.Add(NeighborType.TopLeft, item);
                }
                if (item.transform.position == _topRightPosition)
                {
                    _neighbors.Add(NeighborType.TopRight, item);
                }

                if (item.transform.position == _bottomLeftPosition)
                {
                    _neighbors.Add(NeighborType.BottomLeft, item);
                }
                if (item.transform.position == _bottomRightPosition)
                {
                    _neighbors.Add(NeighborType.BottomRight, item);
                }
            }
        }

        public bool TryGetNeighbor(NeighborType type, out CellComponent component)
        {
            return _neighbors.TryGetValue(type, out component);
        }

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
		public void Configuration(Dictionary<NeighborType, CellComponent> neighbors) // а нужен ли он мне?
		{
            if (_neighbors != null) return;
            _neighbors = neighbors;
		}

        private Vector3 FromFloatToInt(Vector3 fromFloat)
        {
            ToInt = new Vector3((int)fromFloat.x, (int)fromFloat.y, (int)fromFloat.z);
            return ToInt;
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