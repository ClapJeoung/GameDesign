using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
  [SerializeField] private SpriteRenderer MySpr = null; //스프라이트랜더러
  private Color MyAlpha= Color.white;                   //투명도 조절용 알파값
  public float OpenningTime = 1.5f;   //포탈 열리는 시간
  [SerializeField] private float ClosingTime = 0.5f;    //포탈 닫히는 시간
  public float RespawnTime = 1.5f;                      //플레이어가 재생성되는 시간
  [SerializeField] private Transform MyTransform = null;//트랜스폼
  [SerializeField] private float RotateSpeed = 180.0f;

  public void Open(Vector2 targetpos)
  {
    MyTransform.position = (Vector3)targetpos+Vector3.back;
    StartCoroutine(open());
  }
  public void Close() => StartCoroutine(close());
  private IEnumerator open()
  {
    float _time = 0.0f;
    while(_time < OpenningTime)
    {
      MyTransform.localScale = Vector3.one *Mathf.Pow((_time / OpenningTime),2);
      MyAlpha.a = Mathf.Pow((_time / OpenningTime), 2);
      MySpr.color = MyAlpha;
      _time += Time.deltaTime;
      yield return null;
    }
    yield return null;
  }
  private IEnumerator close()
  {
    float _time = 0.0f;
    while (_time < ClosingTime)
    {
      MyTransform.localScale = Vector3.one * (1 - Mathf.Pow((_time / OpenningTime), 2));
      MyAlpha.a = 1 - Mathf.Pow((_time / OpenningTime), 2);
      MySpr.color = MyAlpha;
      _time += Time.deltaTime;
      yield return null;
    }
    MyTransform.localScale = Vector3.zero;
  }
  private void Update()
  {
    if (MyTransform.localScale == Vector3.zero) return;
    MyTransform.Rotate(Vector3.back * RotateSpeed * Time.deltaTime);
  }
}
