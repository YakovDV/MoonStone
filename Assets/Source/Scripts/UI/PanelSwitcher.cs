using UnityEngine;

public class PanelSwitcher : MonoBehaviour
{
    [SerializeField] private UIPanel[] _panels;

    public void Show(ScreenID id)
    {
        for (int i = 0; i < _panels.Length; i++)
        {
            if (_panels[i].ID == id)
            {
                _panels[i].Open();
            }
            else
            {
                _panels[i].Close();
            }
        }
    }

    public void CloseAll()
    {
        for (int i = 0; i < _panels.Length; i++)
        {
            _panels[i].Close();
        }
    }
}
