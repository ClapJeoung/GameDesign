using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider_event : MonoBehaviour
{
  [SerializeField] private EventTarget[] MyActiveTargets = new EventTarget[0];
  [SerializeField] private EventTarget[] MyDeactiveTargets = new EventTarget[0];

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("Player"))
    {
      foreach (var target in MyActiveTargets)if(target!=null) target.Active();
      foreach (var target in MyDeactiveTargets) if (target != null) target.Deactive();
    }
  }
  private void OnDrawGizmos()
  {
    if (MyActiveTargets.Length > 0)
    {
      Gizmos.color = Color.green;
      for (int i = 0; i < MyActiveTargets.Length; i++)
       if(MyActiveTargets[i]!=null) Gizmos.DrawLine(transform.position, MyActiveTargets[i].transform.position);
    }
    if (MyDeactiveTargets.Length > 0)
    {
      Gizmos.color = Color.red;
      for (int i = 0; i < MyDeactiveTargets.Length; i++)
       if(MyDeactiveTargets[i]!=null) Gizmos.DrawLine(transform.position, MyDeactiveTargets[i].transform.position);
    }
  }

}
