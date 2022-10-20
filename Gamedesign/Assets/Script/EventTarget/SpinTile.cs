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
  [SerializeField] private bool AlwaysActive = false;
  [SerializeField] private bool KeepMoving = false;
  [SerializeField] private float Waittime = 1.0f;

  private void Start()
  {
    MyTransform = transform;
    OriginRot = MyTransform.eulerAngles.z;
    if (AlwaysActive) StartCoroutine(keepmoving());
  }
  public override void Active()
  {
    StopAllCoroutines();
    if (KeepMoving) StartCoroutine(keepmoving());
    else StartCoroutine(spinfoward());
  }
  public override void Deactive()
  {
    StopAllCoroutines();
    StartCoroutine(spinbackward());
  }
  private IEnumerator spinfoward()
  {
    while (Progress < SpinTime)
    {
      MyTransform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(OriginRot, TargetRot, Mathf.Pow(Progress / SpinTime, 2.0f)));
      Progress += Time.deltaTime;
      yield return null;
    }
    MyTransform.eulerAngles = Vector3.forward * TargetRot;
    Progress = SpinTime;
  }
  private IEnumerator spinbackward()
  {
    while (Progress >0)
    {
      MyTransform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(OriginRot, TargetRot, Mathf.Pow(Progress / SpinTime, 2.0f)));
      Progress -= Time.deltaTime;
      yield return null;
    }
    MyTransform.eulerAngles = Vector3.forward * OriginRot;
    Progress = 0.0f;
  }
  private IEnumerator keepmoving()
  {
    while (true)
    {
      yield return StartCoroutine(spinfoward());
      yield return new WaitForSeconds(Waittime);
      yield return StartCoroutine(spinbackward());
      yield return new WaitForSeconds(Waittime);
      yield return null;
    }
  }

}
