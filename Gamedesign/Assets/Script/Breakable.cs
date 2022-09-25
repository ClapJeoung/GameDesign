using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
  private bool IsActive = false;  //Ȱ��ȭ(�μ�������)����
  [SerializeField] private ParticleSystem DustParticle = null;  //���� ��ƼŬ
  [SerializeField] private ParticleSystem DestroyParticle = null;//�ı� ��ƼŬ
  [SerializeField] private ParticleSystem RestoreParticle = null;//���� ��ƼŬ
  [SerializeField] private SpriteRenderer MySpr = null; //��������Ʈ������
  private Color MyAlpha = Color.white;                  //����
  [SerializeField] private float WaitTime = 1.0f;  //���� �� �ı����� �ɸ��� �ð�
  [SerializeField] private float DestroyTime = 0.5f;//�ı��ϴµ� �ɸ��¤� �Ӱ�
  [SerializeField] private float RestoreTIme = 5.0f;  //�ı� �� �������� ����ϴ� �ð�

  public void Pressed() //��������
  {
    if (IsActive) return;

    DustParticle.Play();
    IsActive = true;
    StartCoroutine(DestroyCoroutine());
  }
  private IEnumerator DestroyCoroutine()
  {
    yield return new WaitForSeconds(WaitTime); //DestroyTime��ŭ ���

    DustParticle.Stop();  //���� ��ƼŬ ����
    gameObject.layer = LayerMask.NameToLayer("Default");//�⺻ ���̾�� ��ȯ
    DestroyParticle.Play(); //�ı� ��ƼŬ ����
    float _time = 0.0f;
    while (_time < DestroyTime)
    {
      MyAlpha.a = 1 - _time / DestroyTime;  //���İ��� 1~0��
      _time += Time.deltaTime;
      MySpr.color = MyAlpha;
      yield return null;
    }
    MyAlpha.a = 0;
    MySpr.color = MyAlpha;
    //������� ������ �� �ڻ쳭��

    yield return new WaitForSeconds(RestoreTIme); //���� Ÿ�Ӹ�ŭ ���

    RestoreParticle.Play(); //���� ��ƼŬ ����
    _time = 0.0f;
    while (_time < DestroyTime)
    {
      MyAlpha.a = _time / DestroyTime;  //���İ��� 0~1��
      _time += Time.deltaTime;
      MySpr.color = MyAlpha;
      yield return null;
    }
    gameObject.layer = LayerMask.NameToLayer("Breakable");  //���̾� ������ �ٽ� ���� ���� �����ϰ�

    IsActive = false; //�ٽ� ������ �ν��� �� �ֵ��� ���� �ʱ�ȭ
  }
}
