using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageCollider : MonoBehaviour
{
  public Dimension CurrentDimension = Dimension.A;
  [HideInInspector] public Dimension DefaultDimension;
  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("Player")) {
      StageCollider thisscript = this;
      GameManager.Instance.SetSC(ref thisscript);
    }
  }
  private List<Rock> MyRocks = new List<Rock>();
  private List<Wooden> MyWoodens = new List<Wooden>();
  private List<Lamp_event> MyLamps=new List<Lamp_event>();
  public SpinRock MySpinRock = null;
  public StageCollider SetOrigin(Rock newrock)
  {
    MyRocks.Add(newrock); return this;
  }
  public StageCollider SetOrigin(Wooden newwooden)
  {
    MyWoodens.Add(newwooden);return this;
  }
  public StageCollider SetOrigin(Lamp_event newlamp)
  {
    MyLamps.Add(newlamp);return this;
  }
  public void ResetStage()
  {
    foreach (var rock in MyRocks) rock.Resetpos();
    foreach (var wooden in MyWoodens) wooden.ResetDimension();
    foreach (var lamp in MyLamps) lamp.ResetLamp();
    if (MySpinRock != null) MySpinRock.Resetpos();
  }
  public void Setup()
  {
    DefaultDimension = CurrentDimension;
  }
  private void Start()
  {
    Setup();
  }
}
