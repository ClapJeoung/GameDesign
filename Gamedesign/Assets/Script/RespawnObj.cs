using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RespawnObj : MonoBehaviour
{
  public Vector2 RespawnPos = new Vector2(-6.5f,0.0f);
  public float Waittime = 0.0f;
  public float Movetime = 0.0f;
  public bool IsActive = false;

  public Vector3 GetRespawnPos() { return transform.position+(Vector3)RespawnPos + Vector3.back * 2.0f+Vector3.up*0.5f; }
  public Vector2 ObjPos() { return transform.position; }

  private void OnDrawGizmos()
  {
    Gizmos.color = Color.magenta;
    Gizmos.DrawLine(transform.position, transform.position+ (Vector3)RespawnPos);
  }
}
