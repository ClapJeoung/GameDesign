using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_lamp_dimension : MonoBehaviour,Interactable
{
  [SerializeField] private float RequireTime = 3.0f;  //��ȭ�� �ɸ��� �ð�
  private float Progress = 0.0f;                      //���� �ٿ��� �ð�
  private SpriteRenderer FireSpr = null;                //��������Ʈ������
  private SpriteRenderer MySpr = null;
  [SerializeField] private Sprite Spr_world = null;   //���ǰ� ���� �̹���
  [SerializeField] private Sprite Spr_soul = null;    //��ȥ�� ���� �̹���
  private Transform MyTransform = null;               //Ʈ������(��)
  [SerializeField] private ParticleSystem BasicParticle = null;        //��ƼŬ-�⺻
  [SerializeField] private ParticleSystem FiredParticle = null;        //��ȭ�Ϸ� ��ƼŬ
  private bool Ignited = false;                       //���� ���� �پ�����
  public bool Ignitable = false;                      //���� ������ �� �پ�����
  private Color CurrentColor = Color.white;           //������ ������ ����
  private TutorialManager MyManager = null;           //Ʃ�丮��Ŵ���

  public void Setup()
  {
    MySpr= GetComponent<SpriteRenderer>();
    MySpr.enabled = false;
    Ignitable = false;
    MyManager = transform.parent.parent.GetComponent<TutorialManager>();
    MyTransform = transform.GetChild(0).transform;
    FireSpr = MyTransform.GetComponent<SpriteRenderer>();
    CurrentColor.a = 0.0f;
    FireSpr.color = CurrentColor;
    MyTransform.localScale = Vector3.zero;
    BasicParticle.Stop();
  }
  private void Start()
  {
    Setup();
  }
  public void FireUp()
  {
    if (!Ignitable) return;
    Ignited = true;
    BasicParticle.Play();
  }
  public void FireDown()
  {
    if (!Ignitable) return;
    Ignited = false;
    BasicParticle.Stop();
  }
  private void Update()
  {
    if (!Ignitable) return;
    Progress += Time.deltaTime * (Ignited ? 1 : -1); //��ȭ������ Progress�� ����, �ƴϸ� -1

    if (Progress < 0.0f) { Progress = 0.0f; MyTransform.localScale = Vector3.zero; CurrentColor.a = 0.0f; FireSpr.color = CurrentColor; return; }
    //���൵�� �������� �������� ���뿡�� �ø���

    if (Progress > RequireTime)
    {
      Progress = RequireTime; FiredParticle.Play();

        if (MyManager.TutorialDimension == Dimension.A)
        { GameManager.Instance.OpenMask(MyManager.TutorialRatio); MySpr.sprite = Spr_soul; MyManager.TutorialDimension = Dimension.B;
        MyManager.SetStone();
      }       //Ʃ�丮�� ���� ����Ʈ�� �������ϳ�
        else { GameManager.Instance.CloseMask(MyManager.TutorialRatio); MySpr.sprite = Spr_world; MyManager.TutorialDimension = Dimension.A; }
        Progress = 0.0f;
        Ignited = false;
      MyTransform.localScale = Vector3.zero;
    }  //���൵�� �ִ�ġ�� �����ϸ� ��

    MyTransform.localScale = Vector3.one * Mathf.Pow((Progress / RequireTime), 2);  //���൵�� ����� ũ�� ����
    CurrentColor.a = Mathf.Sqrt(Progress / RequireTime);              //���൵�� ��Ʈ �׷��� ������ ���� ����
    FireSpr.color = CurrentColor;                                       //���� ����
  }
  public void Active()
  {
    MySpr.enabled = true;
    Ignitable = true;
    FiredParticle.Play();
  }
  public void DeActive()
  {
    FiredParticle.Play();
    MySpr.enabled = false;
    Invoke("asdf", 1.0f);
  }
  private void asdf() => Destroy(gameObject);

}
