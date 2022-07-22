using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public abstract class BaseClickComponent : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        protected Material _selectMaterial;
        [SerializeField]
        private Material _hoverMaterial;
        [SerializeField]
        protected Material _canEatMaterial;

        //Меш игрового объекта
        private MeshRenderer _mesh;
        //Список материалов на меше объекта
        private Material[] _meshMaterials = new Material[3];

        [Tooltip("Цветовая сторона игрового объекта"), SerializeField]
        protected ColorType _color;

        /// <summary>
        /// Возвращает цветовую сторону игрового объекта
        /// </summary>
        public ColorType GetColor => _color;
        

        /// <summary>
        /// Возвращает или устанавливает пару игровому объекту
        /// </summary>
        /// <remarks>У клеток пара - фишка, у фишек - клетка</remarks>
        public BaseClickComponent Pair { get; set; }
        public bool IsSelected { get; protected set; }

        /// <summary>
        /// Добавляет дополнительный материал
        /// </summary>
        public void AddAdditionalMaterial(Material material, int index = 1)
        {
            if (index < 1 || index > 2)
            {
                Debug.LogError("Попытка добавить лишний материал. Индекс может быть равен только 1 или 2");
                return;
            }
            _meshMaterials[index] = material;
            _mesh.materials = _meshMaterials.Where(t => t != null).ToArray();
        }

        /// <summary>
        /// Удаляет дополнительный материал
        /// </summary>
        public void RemoveAdditionalMaterial(int index = 1)
        {
            if (index < 1 || index > 2)
            {
                Debug.LogError("Попытка удалить несуществующий материал. Индекс может быть равен только 1 или 2");
                return;
            }
            _meshMaterials[index] = null;
            _mesh.materials = _meshMaterials.Where(t => t != null).ToArray();
        }

        /// <summary>
        /// Событие клика на игровом объекте
        /// </summary>
        public event ClickEventHandler OnClickEventHandler;

        /// <summary>
        /// Событие наведения и сброса наведения на объект
        /// </summary>
        public event FocusEventHandler OnFocusEventHandler;

        //При навадении на объект мышки, вызывается данный метод
        //При наведении на фишку, должна подсвечиваться клетка под ней
        //При наведении на клетку - подсвечиваться сама клетка
        public abstract void OnPointerEnter(PointerEventData eventData);

        //Аналогично методу OnPointerEnter(), но срабатывает когда мышка перестает
        //указывать на объект, соответственно нужно снимать подсветку с клетки
        public abstract void OnPointerExit(PointerEventData eventData);

        //При нажатии мышкой по объекту, вызывается данный метод
        public void OnPointerClick(PointerEventData eventData)
		{
            OnClickEventHandler?.Invoke(this);
        }

        //Этот метод можно вызвать в дочерних классах (если они есть) и тем самым пробросить вызов
        //события из дочернего класса в родительский
        protected void CallBackEvent(CellComponent target, bool isSelect)
        {
            OnFocusEventHandler?.Invoke(target, isSelect);
		}

		protected virtual void Start()
        {
            _mesh = GetComponent<MeshRenderer>();
            //Этот список будет использоваться для набора материалов у меша,
            //в данном ДЗ достаточно массива из 3 элементов
            //1 элемент - родной материал меша, он не меняется
            //2 элемент - материал при наведении курсора на клетку/выборе фишки
            //3 элемент - материал клетки, на которую можно передвинуть фишку
            _meshMaterials[0] = _mesh.material;
        }

        protected void Highlight(CellComponent component, bool isSelect) 
        {
            if (isSelect)
            {
                component.AddAdditionalMaterial(_hoverMaterial, 1);
            }
            else
            {
                component.RemoveAdditionalMaterial(1);
            }
        }

        public void HighlightSelected(BaseClickComponent component) //  работает некорректно. почему?
        {
            if (IsSelected)
            {
                IsSelected = false;
                RemoveAdditionalMaterial(2);
                PossibleMoves(component);
                
            }
            else
            {
                IsSelected = true;
                AddAdditionalMaterial(_selectMaterial, 2);
                PossibleMoves(component);
                
            }
        }

        protected void PossibleMoves(BaseClickComponent component) //плохо работает, переделать!
        {
            if (Pair is CellComponent cell)
            {
                Debug.Log("PossibleMoves 1"); // срабатывает
                Debug.Log(_color);
                Debug.Log(Pair);

                NeighborType a = NeighborType.TopLeft;
                NeighborType b = NeighborType.TopRight;

                if (GetColor == ColorType.Black) // тут не срабатывает. почему?
                {
                    Debug.Log("PossibleMoves 2");
                    a = NeighborType.BottomLeft;
                    b = NeighborType.BottomRight;
                }

                if (cell.TryGetNeighbor(a, out var leftCell))
                {
                    Debug.Log("PossibleMoves 3");
                    if (leftCell.isEmpty) leftCell.HighlightSelected(component);

                    else if (leftCell.Pair.GetColor != GetColor && leftCell.TryGetNeighbor(a, out var leftOverEnemy) && leftOverEnemy.isEmpty)
                    {
                        Debug.Log("PossibleMoves 4");
                        (leftCell.Pair as ChipComponent)?.SetEatMaterial();
                        leftOverEnemy.HighlightSelected(component);
                    }

                }
                if (cell.TryGetNeighbor(b, out var rightCell))
                {
                    Debug.Log("PossibleMoves 5");
                    if (rightCell.isEmpty)
                    {
                        Debug.Log("PossibleMoves 6");
                        rightCell.HighlightSelected(component);
                    }
                    else if (rightCell.Pair.GetColor != GetColor && rightCell.TryGetNeighbor(b, out var rightOverEnemy) && rightOverEnemy.isEmpty)
                    {
                        Debug.Log("PossibleMoves 7");
                        (rightCell.Pair as ChipComponent)?.SetEatMaterial();
                        rightOverEnemy.HighlightSelected(component);
                    }

                }
            }
            
        }
    }

    public enum ColorType
    {
        White,
        Black
    }

    public delegate void ClickEventHandler(BaseClickComponent component);
    public delegate void FocusEventHandler(CellComponent component, bool isSelect);
}