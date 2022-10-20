using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flood : EventTarget
{
  private Vector2 OriginPos = Vector2.zero;
  [SerializeField] private Vector2 TargetPos = Vector2.zero;
  [SerializeField] private float FillingTime = 10.0f;
  [SerializeField] private float DrainingSpeed = 2.0f;
  private float Progress = 0.0f;
  private bool IsDone = false;
  private Transform MyTransform = null;

  public void Setup()
  {
    MyTransform = transform;
    OriginPos = MyTransform.position;
  }
  private void Start()
  {
    Setup();
  }
  public override void Active()
  {
    if (IsDone) return;
    StopAllCoroutines();
    StartCoroutine(startfilling());
  }
  public override void Deactive()
  {
    if (IsDone) return;
    StopAllCoroutines();
    StartCoroutine(startdraining());
  }
  private IEnumerator startfilling()
  {
    while (Progress < FillingTime)
    {
      MyTransform.position = Vector2.Lerp(OriginPos, TargetPos, Progress / FillingTime);
      Progress += Time.deltaTime;
      yield return null;
    }
    IsDone = true;
  }
  private IEnumerator startdraining()
  {
    while (Progress > 0)
    {
      MyTransform.position = Vector2.Lerp(OriginPos, TargetPos, Progress / FillingTime);
      Progress -= Time.deltaTime * DrainingSpeed;
      yield return null;
    }
  }
  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("Torch"))
    {
      StopAllCoroutines();
      StartCoroutine(startdraining());
    }
  }
}
