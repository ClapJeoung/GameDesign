using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RespawnObj : MonoBehaviour
{
  public Vector2 RespawnPos = new Vector2(-6.5f,0.0f);
  public float Waittime = 0.0f;
  public float Movetime = 0.0f;
  public bool IsActive = false;

  public Vector3 GetRespawnPos(bool isleft) { return transform.position+(Vector3)RespawnPos*(isleft?-1:1) + Vector3.back * 2.0f+Vector3.up*0.5f; }
  public Vector2 ObjPos() { return transform.position; }
}
