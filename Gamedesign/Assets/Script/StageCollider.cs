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
  private List<Pot_keeppour> MyPots= new List<Pot_keeppour>();
  private List<Pot> MyPots_0=new List<Pot>();
 [HideInInspector] public SpinRock MySpinRock = null;
  private WoodenPillar MyPillar = null;
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
  public StageCollider SetOrigin(WoodenPillar newpillar)
  {
    MyPillar = newpillar;return this;
  }
  public StageCollider SetOrigin(Pot_keeppour newpot)
  {
    MyPots.Add(newpot); return this;
  }
  public StageCollider SetOrigin(Pot newpot)
  {
    MyPots_0.Add(newpot); return this;
  }

  public void ResetStage()
  {
    foreach (var rock in MyRocks) rock.Resetpos();
    foreach (var wooden in MyWoodens) wooden.ResetDimension();
    foreach (var lamp in MyLamps) lamp.ResetLamp();
    foreach (var pot in MyPots) pot.Active();
    foreach (var pot in MyPots_0) pot.ReSetPot();
    if (MySpinRock != null) MySpinRock.Resetpos();
    if (MyPillar != null) MyPillar.ResetDimension();
  }
  public void Setup()
  {
    DefaultDimension = CurrentDimension;
  }
  private void Start()
  {
    Setup();
  }
  public void StopAllPots()
  {
    foreach (var pot in MyPots) pot.Deactive();
    foreach (var pot in MyPots_0) pot.Deactive();
  }
}
