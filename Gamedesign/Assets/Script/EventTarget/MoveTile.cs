using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTile : EventTarget
{
  private Vector2 OriginPos = Vector2.zero; //원래 자리
  [SerializeField] private Vector2 TargetPos = Vector2.zero;  //목표 자리
  [SerializeField] private float MoveTime = 1.5f;             //움직이는 시간
  private Transform MyTransform = null;
  private float Progress = 0.0f;
  [SerializeField] private bool AlwaysActive=false;
  [SerializeField] private bool KeepMoving = false;
  [SerializeField] private float Waittime = 1.0f;
  private void Start()
  {
    MyTransform= transform;
    OriginPos = transform.position;
    if (AlwaysActive) StartCoroutine(keepmoving());
  }
  public override void Active()
  {
    StopAllCoroutines();
    if (KeepMoving) StartCoroutine(keepmoving());
    else StartCoroutine(movefoward());
  }
  public override void Deactive()
  {
    StopAllCoroutines();
    StartCoroutine(movebackward());
  }
  private IEnumerator movefoward()
  {
    while (Progress < MoveTime)
    {
      MyTransform.position = Vector2.Lerp(OriginPos, TargetPos, Mathf.Pow(Progress / MoveTime,2.0f));
      Progress += Time.deltaTime;
      yield return null;
    }
    MyTransform.position = TargetPos;
    Progress = MoveTime;
    yield return null;
  }
  private IEnumerator movebackward()
  {
    while (Progress > 0.0f)
    {
      MyTransform.position = Vector2.Lerp(OriginPos, TargetPos, Mathf.Pow(Progress / MoveTime, 2.0f));
      Progress -= Time.deltaTime;
      yield return null;
    }
    MyTransform.position = OriginPos;
    Progress = 0.0f;
    yield return null;
  }
  private IEnumerator keepmoving()
  {
    while (true)
    {
      yield return StartCoroutine(movefoward());
      yield return new WaitForSeconds(Waittime);
      yield return StartCoroutine(movebackward());
      yield return new WaitForSeconds(Waittime);
      yield return null;
    }
  }
}
