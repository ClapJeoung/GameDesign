using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinTile : EventTarget
{
  private float OriginRot = 0.0f;
  [SerializeField] private float TargetRot = 0.0f;  
  [SerializeField] private float SpinTime = 1.5f;             //움직이는 시간
  private Transform MyTransform = null;
  private float Progress = 0.0f;

  private void Start()
  {
    MyTransform = transform;
    OriginRot = MyTransform.eulerAngles.z;
  }
  public override void Active()
  {
    StopAllCoroutines();
    StartCoroutine(spinfoward());
  }
  public override void Deactive()
  {
    StopAllCoroutines();
    StartCoroutine(spinbackward());
  }
  private IEnumerator spinfoward()
  {
    float _currentrot = MyTransform.eulerAngles.z;
    while (Progress < SpinTime)
    {
      MyTransform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(_currentrot, TargetRot, Mathf.Sqrt(Progress / SpinTime)));
      Progress += Time.deltaTime;
      yield return null;
    }
    MyTransform.eulerAngles = Vector3.forward * TargetRot;
  }
  private IEnumerator spinbackward()
  {
    float _currentrot = MyTransform.eulerAngles.z;
    while (Progress >0)
    {
      MyTransform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(_currentrot, TargetRot, Mathf.Sqrt(Progress / SpinTime)));
      Progress -= Time.deltaTime;
      yield return null;
    }
    MyTransform.eulerAngles = Vector3.forward * OriginRot;
  }
}
