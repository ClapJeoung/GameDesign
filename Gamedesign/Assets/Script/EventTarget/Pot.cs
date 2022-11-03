using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : EventTarget
{
  [SerializeField] private WaterDrop[] MyWaters = null; //�ڽ����� ������ �ִ� ����
  [SerializeField] private Vector3 FallPostion = Vector3.zero;  //���� ������ ��ġ
  [SerializeField] private ParticleSystem OpenParticle = null;  //�� ����߸��� ��ƼŬ
  [SerializeField] private bool AllwaysActive = false;          //���������� Ȱ��ȭ�Ȱ���
  [SerializeField] private float FirstWaitTime = 0.0f;          //���������� Ȱ��ȭ������ �ʱ� ���ð�
  private float PouringTime = 2.0f;                             //�� �״� �ð�
  [SerializeField] private float WaitTime = 4.0f;               //�� �װ� ���� �ױ���� ����ϴ� �ð�
  public int MyId = 0;
  private IEnumerator pouring()
  {
    yield return new WaitForSeconds(FirstWaitTime);
    while (true)
    {
      OpenParticle.Play();
      for(int i=0; i < MyWaters.Length; i++)
      {
        MyWaters[i].Fall(FallPostion+Vector3.back);
        yield return new WaitForSeconds(PouringTime / MyWaters.Length);
      }
      OpenParticle.Stop();
      yield return new WaitForSeconds(WaitTime);
    }
  }
  public override void  Active()
  {
    StartCoroutine(pouring());
  }
  public override void Deactive()
  {
    OpenParticle.Stop();
    StopAllCoroutines();
  }
  private void Start()
  {
    if (AllwaysActive) StartCoroutine(pouring());
  }
}
