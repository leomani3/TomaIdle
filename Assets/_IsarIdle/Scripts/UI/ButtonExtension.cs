using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ButtonExtension : MonoBehaviour
{
    [Title("References")]
    [SerializeField] private Button m_button;
    [SerializeField] private Image m_icon;
    [SerializeField] private TextMeshProUGUI m_text;
    [SerializeField] private Color m_interactableColor = Color.white;
    [SerializeField] private Color m_nonInteractableColor = Color.gray;
    [SerializeField] List<GameObject> m_notInteractableGo = new List<GameObject>();

    [Title("Setup")]
    [Button]
    public void Setup(Sprite icon, string text)
    {
        if (m_icon != null)
            m_icon.sprite = icon;
        
        if (m_text != null)
            m_text.text = text;
    }
    
    
    public Button Button => m_button;

    private void Reset()
    {
        m_button = GetComponent<Button>();
    }

    [Button]
    public void SetInteractable(bool _interactable)
    {
        m_button.interactable = _interactable;

        foreach (GameObject go in m_notInteractableGo)
        {
            go.SetActive(!_interactable);
        }
        
        if (m_text != null)
            m_text.color = _interactable ? m_interactableColor : m_nonInteractableColor;
        
        if (m_icon != null)
            m_icon.color = _interactable ? m_interactableColor : m_nonInteractableColor;
    }
}