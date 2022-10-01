using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventTarget:MonoBehaviour
{
  public virtual void Active() { }
  public virtual void Deactive() { }
}
