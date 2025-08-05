using DG.Tweening;
using MPUIKIT;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RoundFillDisplay : MonoBehaviour
{
    [Title("References")]
    [SerializeField] private MPImage m_mainFill;
    [SerializeField] private MPImage m_delayedFill;
    [SerializeField] private MPImage m_insideCircle;
    [SerializeField] private MPImage m_outsideCircle;
    [SerializeField] private MPImage m_background;
    [SerializeField] private Image m_backgroundIcon;
    [SerializeField] private TextMeshProUGUI m_mainText;
    [SerializeField] private TextMeshProUGUI m_subText;

    [Title("Setup")]
    [SerializeField] private Gradient m_mainFillGradient;
    [SerializeField] private Color m_mainFillOutlineColor;
    [SerializeField] private Color m_delayedFillColor;
    [SerializeField] private Color m_backgroundColor;
    [SerializeField] private Color m_outsideLoopsColor;
    [SerializeField] private Gradient m_insideLoopGradient;
    [SerializeField] private float m_outsideLoopsWidth;
    [SerializeField] private float m_insideLoopWidth;
    [SerializeField] private Sprite m_backgroundSprite;
    
    //Todo take care of texts

    [Button]
    public void Setup()
    {
        //Borders width
        m_insideCircle.StrokeWidth = (m_outsideLoopsWidth * 2) + m_insideLoopWidth;
        m_outsideCircle.StrokeWidth = m_insideLoopWidth;
        m_outsideCircle.OutlineWidth = m_outsideLoopsWidth;
        
        //Borders colors
        m_insideCircle.color = m_outsideLoopsColor;
        GradientEffect _insideLoopGradient = new GradientEffect();
        _insideLoopGradient.Gradient = m_insideLoopGradient;
        _insideLoopGradient.GradientType = GradientType.Radial;
        _insideLoopGradient.Enabled = true;
        m_outsideCircle.GradientEffect = _insideLoopGradient;
        m_outsideCircle.OutlineColor = m_outsideLoopsColor;
        
        //Inside colors
        m_background.color = m_backgroundColor;
        m_delayedFill.color = m_delayedFillColor;
        
        GradientEffect _gradient = new GradientEffect();
        _gradient.Gradient = m_mainFillGradient;
        _gradient.GradientType = GradientType.Linear;
        _gradient.Rotation = 90;
        _gradient.Enabled = true;
        m_mainFill.GradientEffect = _gradient;
        m_mainFill.OutlineColor = m_mainFillOutlineColor;
        
        m_backgroundIcon.sprite = m_backgroundSprite;
    }

    public void SetMainText(string txt)
    {
        m_mainText.text = txt;
    }
    
    public void SetSubText(string txt)
    {
        m_subText.text = txt;
    }
    
    [Button]
    public void SetFill(float _ratio, bool _instant)
    {
        // Clamp ratio to avoid invalid values
        _ratio = Mathf.Clamp01(_ratio);
    
        // Instantly set main fill
        m_mainFill.fillAmount = _ratio;

        if (_instant)
        {
            // Instantly set delayed fill
            m_delayedFill.fillAmount = _ratio;
        }
        else
        {
            // Tween delayed fill
            m_delayedFill.DOKill();
            m_delayedFill.DOFillAmount(_ratio, 0.5f).SetEase(Ease.OutCubic);
        }
    }

}