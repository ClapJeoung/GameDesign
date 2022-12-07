using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_circle : MonoBehaviour
{
  [SerializeField] private SpriteRenderer Top = null;
  [SerializeField] private SpriteRenderer Bottom = null;
  [SerializeField] private Torch_pivot MyPivot = null;
  [SerializeField] private Sprite RightTop = null;
  [SerializeField] private Sprite RightBottom = null;
  [SerializeField] private Sprite LeftTop = null;
  [SerializeField] private Sprite LeftBottom = null;
  [SerializeField] private Color ActiveColor;
  [SerializeField] private Color DeactiveColor;
//  [SerializeField] private ParticleSystem IdleParticle = null;
  [SerializeField] private ParticleSystem DestroyParticle = null;
  private bool IsRight = false;
  public void PressRight()
  {
    IsRight = true;
    Top.sprite= RightTop;
    Bottom.sprite = RightBottom;
  }
  public void PressLeft()
  {
    IsRight= false;
    Top.sprite = LeftTop;
    Bottom.sprite = LeftBottom;
  }
  private void Update()
  {
    if (IsRight)
    {
      if (MyPivot.CurrentRadius>-90.0f&&MyPivot.CurrentRadius<135.0f)
      {
        Top.color = ActiveColor;
        Bottom.color = DeactiveColor;
      }
      else
      {
        Top.color = DeactiveColor;
        Bottom.color = ActiveColor;
      }
    }
    else
    {
      if (MyPivot.CurrentRadius < 90.0f && MyPivot.CurrentRadius > -135.0f)
      {
        Top.color = ActiveColor;
        Bottom.color = DeactiveColor;
      }
      else
      {
        Top.color = DeactiveColor;
        Bottom.color = ActiveColor;
      }
    }
  }
  public void DestroyCircle()
  {
    DestroyParticle.Play();
    Top.enabled = false;
    Bottom.enabled = false;
    Invoke("realdestroy", 1.5f);
  }
  private void realdestroy()
  {
    Destroy(gameObject);
  }
}
