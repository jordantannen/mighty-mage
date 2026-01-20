using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Slider m_slider;

    public void SetMaxHealth(int health)
    {
        m_slider.maxValue = health;
        m_slider.value = health;
    }

    public void SetHealth(int health)
    {
        m_slider.value = health;
    }
}