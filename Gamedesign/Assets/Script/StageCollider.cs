using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageCollider : MonoBehaviour
{
  public Dimension CurrentDimension = Dimension.A;

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("Player")) {
      StageCollider thisscript = this;
      GameManager.Instance.SetSC(ref thisscript);
    }
  }
}
