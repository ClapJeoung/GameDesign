using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainButtons : MonoBehaviour
{
  [SerializeField] protected ParticleSystem SelectParticle = null;
  [SerializeField] protected ParticleSystem PressedParticle = null;
  [SerializeField] protected ParticleSystem UnPressedParticle = null;
  [SerializeField] protected SpriteRenderer MySpr = null;
  protected Color MyColor= Color.white;
  [SerializeField] private float TransparentTime = 0.5f;
  protected enum ButtonType { Start, Quit }
  [SerializeField] private ButtonType MyButtonType;
  private bool ButtonPressed = false;
    public void Select()
  {
    if (ButtonPressed) return;
    SelectParticle.Play();
  }
  public void UnSelect()
  {
    if (ButtonPressed) return;
    SelectParticle.Stop();
  }
  public void Pressed()
  {
    if (ButtonPressed) return;
    SelectParticle.Stop();
    PressedParticle.Play();
    ButtonPressed = true;
  }
  public void UnPressed()
  {
    if (ButtonPressed) return;
    StartCoroutine(disappear());
    UnPressedParticle.Play();
  }
  private IEnumerator disappear()
  {
    float _time = 0.0f;
    while(_time< TransparentTime)
    {
      MyColor.a = 1 - _time / TransparentTime;
      MySpr.color = MyColor;
      _time += Time.deltaTime;
      yield return null;
    }
    MyColor.a = 0;
    MySpr.color= MyColor;
  }
}
