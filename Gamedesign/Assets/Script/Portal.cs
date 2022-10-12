using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
  [SerializeField] private SpriteRenderer MySpr = null; //��������Ʈ������
  private Color MyAlpha= Color.white;                   //���� ������ ���İ�
  public float OpenningTime = 1.5f;   //��Ż ������ �ð�
  [SerializeField] private float ClosingTime = 0.5f;    //��Ż ������ �ð�
  public float RespawnTime = 1.5f;                      //�÷��̾ ������Ǵ� �ð�
  [SerializeField] private Transform MyTransform = null;//Ʈ������
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
