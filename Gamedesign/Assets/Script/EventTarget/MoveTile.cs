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

  private void Start()
  {
    MyTransform= transform;
    OriginPos = transform.position;
    TargetPos += OriginPos;
  }
  public override void Active()
  {
    StopAllCoroutines();
    StartCoroutine(movefoward());
  }
  public override void Deactive()
  {
    StopAllCoroutines();
    StartCoroutine(movebackward());
  }
  private IEnumerator movefoward()
  {
    Vector2 _currentpos = MyTransform.position;
    while (Progress < MoveTime)
    {
      MyTransform.position = Vector2.Lerp(_currentpos, TargetPos, Mathf.Sqrt(Progress / MoveTime));
      Progress += Time.deltaTime;
      yield return null;
    }
    MyTransform.position = TargetPos;
  }
  private IEnumerator movebackward()
  {
    Vector2 _currentpos = MyTransform.position;
    while (Progress > 0.0f)
    {
      MyTransform.position = Vector2.Lerp(_currentpos, OriginPos, Mathf.Sqrt(Progress / MoveTime));
      Progress -= Time.deltaTime;
      yield return null;
    }
    MyTransform.position = OriginPos;
  }
}
