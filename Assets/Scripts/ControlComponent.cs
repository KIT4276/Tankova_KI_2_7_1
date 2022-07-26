﻿using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine.UI;

namespace Checkers
{
    public class ControlComponent : MonoBehaviour
    {

        ChipComponent baseClickComponent = new ChipComponent();
        private ChipComponent _selectedChip;

        [SerializeField] protected ColorType _currentTurn;
        [SerializeField] private EventSystem _eventSystem;
        

        private CellComponent[] _cells;
        private ChipComponent[] _chips;
        private void Awake()
        {
            _cells = FindObjectsOfType<CellComponent>();
            _chips = FindObjectsOfType<ChipComponent>();

            baseClickComponent.OnFocusEventHandler += (q, qq) => { };
        }
        private void Start()
        {
            foreach (var chip in _chips)
            {
                chip.OnClickEventHandler += SelectObject;
                chip.ChipMove += Switch;
            } 
            
            foreach (var cell in _cells)
            {
                cell.OnClickEventHandler += SelectObject;
            }
        }

        private void SelectObject(BaseClickComponent component)
        {

            if (component is ChipComponent chip )// если выбрана шашка и её цвет ходит
            {
                Debug.Log("если выбрана шашка и её цвет ходит");
                if (_selectedChip == null)
                {
                    _selectedChip = chip;
                }

                if (_selectedChip == chip)
                {
                    chip.HighlightSelected(component);

                    if (!chip.IsSelected)
                    {
                        _selectedChip = null;
                    }
                }
                else
                {
                    _selectedChip.HighlightSelected(component);
                    _selectedChip = chip;
                    chip.HighlightSelected(component);
                }
            }
            
            else if (component is CellComponent cell && cell.isEmptyToMove && _selectedChip != null) // тут в _selectedChip приходит null

            {
                Debug.Log("если выбрана клетка и она пуста и выбранная фишка не нал");
                Switch();
                _selectedChip.Move(cell);
                _selectedChip = null;
                NextTurn();
            }
        }
        private void Switch() // подсмотрено
        {
            _eventSystem.enabled = !_eventSystem.enabled;
        }

        private void NextTurn()
        {
            _currentTurn = _currentTurn == ColorType.Black ? ColorType.White : ColorType.Black;

            //CameraControl cameraControl = new CameraControl();
            //cameraControl.CameraViewChange();
            CameraControl.Self.CameraViewChange();
        }
        private void Victory(ChipComponent component)
        {
            if (component is ChipComponent Pair && Pair.Pair != null)
            {
                if (true) Debug.Log("Победа Чёрных");// todo
               // else Debug.Log("Победа Белых");

                UnityEditor.EditorApplication.isPaused = true;
            }
        }
    }
}
