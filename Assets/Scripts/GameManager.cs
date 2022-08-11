using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private ColorType _currentTurn;
        [SerializeField] private EventSystem _eventSystem;

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
                    chip.ChipMove += SwitchEventSystemStatus;
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
        private void Update()
        {
            CheckWin();
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

                    if (chip.IsSelected != true)
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
                cell.RemoveAdditionalMaterial(1); // почему не работает?

                SwitchEventSystemStatus();
                _selectedChip.Move(cell);
                _selectedChip = null;

                NextTurn();
            }
        }

        private void SwitchEventSystemStatus() => _eventSystem.enabled = !_eventSystem.enabled;

        private void NextTurn()
        {
            _currentTurn = _currentTurn == ColorType.Black ? ColorType.White : ColorType.Black;
            CameraControl.Self.CameraViewChange();
        }
        private void CheckWin()
        {
            if (WinCheck.Self.GetCheck == true)
            {
                Debug.Log("Победа");
                UnityEditor.EditorApplication.isPaused = true;
            }
        }
    }
}
