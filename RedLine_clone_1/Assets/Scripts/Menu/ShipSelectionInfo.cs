using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipSelectionInfo : MonoBehaviour
{
    public Animator shipDisplayAnim;
    public Animator manufacturerDisplayAnim;
    public List<Animator> shipAnimators;
    public List<Sprite> manufacturerSprites;
    public Image manufacturerImage;
    public Image manufacturerImageRed;
    public string readyTriggerString;
    public string unReadyTriggerString;
    public Animator readyAnimator;

    [Tooltip("0 = Top Speed, 1 = Acceleration, 2 = Handling")]
    public List<float> m_splitwingStats, m_fulcrumStats, m_cutlassStats;

    [SerializeField] float m_barLength, m_barChangeDuration;
    [SerializeField] AnimationCurve m_barChangeTween;
    [SerializeField] Transform m_accelerationBar, m_accelerationBarRed1, m_accelerationBarRed2, m_topSpeedBar, m_topSpeedBarRed1, m_topSpeedBarRed2, m_handlingBar, m_handlingBarRed1, m_handlingBarRed2;

    public void TopSpeedBarFill(float fillAmount)
    {
        Vector3 barPos = new Vector3(m_barLength - (m_barLength * fillAmount), 0, 0);
        Tween.LocalPosition(m_topSpeedBar, barPos, m_barChangeDuration, 0.05f, m_barChangeTween);
        Tween.LocalPosition(m_topSpeedBarRed1, barPos, m_barChangeDuration, 0, m_barChangeTween);
        Tween.LocalPosition(m_topSpeedBarRed2, barPos, m_barChangeDuration, 0.1f, m_barChangeTween);
    }

    public void AccelerationBarFill(float fillAmount)
    {
        Vector3 barPos = new Vector3(m_barLength - (m_barLength * fillAmount), 0, 0);
        Tween.LocalPosition(m_accelerationBar, barPos, m_barChangeDuration, 0.05f, m_barChangeTween);
        Tween.LocalPosition(m_accelerationBarRed1, barPos, m_barChangeDuration, 0, m_barChangeTween);
        Tween.LocalPosition(m_accelerationBarRed2, barPos, m_barChangeDuration, 0.1f, m_barChangeTween);
    }

    public void HandlingBarFill(float fillAmount)
    {
        Vector3 barPos = new Vector3(m_barLength - (m_barLength * fillAmount), 0, 0);
        Tween.LocalPosition(m_handlingBar, barPos, m_barChangeDuration, 0.05f, m_barChangeTween);
        Tween.LocalPosition(m_handlingBarRed1, barPos, m_barChangeDuration, 0, m_barChangeTween);
        Tween.LocalPosition(m_handlingBarRed2, barPos, m_barChangeDuration, 0.1f, m_barChangeTween);
    }
}
