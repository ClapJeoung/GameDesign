using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FIreMask : MonoBehaviour
{
  private Transform MyTransform = null;
  [SerializeField] private float EffectTime = 0.1f;
  [SerializeField] private float TargetSize = 65.0f;

  private void Start()
  {
    Setup();
  }
  public void Setup()
  {
    MyTransform= transform;
  }
  public void Open(Vector2 newpos)
  {
    MyTransform.position = newpos;
    StartCoroutine(changesize(TargetSize));
  }
  public void Close(Vector2 newpos)
  {
    MyTransform.position = newpos;
    StartCoroutine(changesize(0.0f));
  }
  private IEnumerator changesize(float targetsize)
  {
    float _time = 0.0f;
    float _originsize = MyTransform.localScale.x;
  //  Debug.Log($"{_originsize} -> {targetsize}");
    while (_time < EffectTime)
    {
      MyTransform.localScale = Vector3.one * Mathf.Lerp(_originsize, targetsize, _time / EffectTime);
      _time += Time.deltaTime;
      yield return null;
    }
    MyTransform.localScale = Vector3.one * targetsize;
  }
}
