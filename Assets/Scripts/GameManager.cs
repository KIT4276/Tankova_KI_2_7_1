using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private ColorType _currentTurn;
        [SerializeField] private EventSystem _eventSystem;
        //[SerializeField] private Text _finishScreen;// пока без этого

        private ChipComponent _selectedChip;

        private CellComponent[] _cells;
        private ChipComponent[] _chips;

        private void Awake()
        {
            _cells = FindObjectsOfType<CellComponent>();
            _chips = FindObjectsOfType<ChipComponent>();

            if (_chips?.Length > 0)
            {
                foreach (var chip in _chips)
                {
                    chip.OnClickEventHandler += SelectObject;
                    chip.OnChipMove += SwitchEventSystemStatus;
                }
            }

            if (_cells?.Length > 0)
            {
                foreach (var cell in _cells)
                {
                    cell.OnClickEventHandler += SelectObject;
                }
            }
        }

        private void SelectObject(BaseClickComponent component)
        {
            if (component is ChipComponent chip && chip.GetColor == _currentTurn)
            {
                if (_selectedChip == null)
                {
                    _selectedChip = chip;
                }

                if (_selectedChip == chip)
                {
                    chip.ToSelectChip();

                    if (!chip.IsSelected)
                    {
                        _selectedChip = null;
                    }
                }
                else
                {
                    _selectedChip.ToSelectChip();
                    _selectedChip = chip;
                    chip.ToSelectChip();
                }
            }
            else if (component is CellComponent cell && cell.CanBeOccupied && _selectedChip != null)
            {
                SwitchEventSystemStatus();
                _selectedChip.MoveToNewCell(cell);
                _selectedChip = null;

                //ApplyNextTurn();
            }
        }
        

        private void SwitchEventSystemStatus()
        {
            _eventSystem.enabled = !_eventSystem.enabled;
        }
    }
}
