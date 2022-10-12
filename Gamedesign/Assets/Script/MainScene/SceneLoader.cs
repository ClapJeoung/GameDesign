using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader instance;
  public static SceneLoader Instance { get { return instance; } }
  [SerializeField] private Sprite[] CutsceneSprites = null; //�ƾ� �̹�����
  [SerializeField] private float SpriteSize = 10.0f;        //�ƾ� �̹��� ����
  [SerializeField] private Image FrontgroundImage = null;   //�ƾ� ������ �̹���
  [SerializeField] private Image CutsceneImage = null;      //�ƾ� ������ �̹���
  [SerializeField] private Image BackgroundImage = null;    //�ƾ� ��� �̹���
  [SerializeField] private float FadeinTime = 4.5f;         //���̵��� �ð�                         1
  [SerializeField] private float FadewaitTime = 2.0f;       //���̵��� �� ��� �ð�                  2
  [SerializeField] private float CutsceneIntime = 1.0f;     //���̵��� ��� �� �ƾ����� ������ �ð�   3
  [SerializeField] private float CutsceneWaittime = 3.0f;   //�ƾ��� ��� �ð�                       4
  [SerializeField] private float CursceneOuttime = 2.0f;    //�ƾ� ������ ���̵��� �Ǵ� �ð�           5
  [SerializeField] private float FadeoutTime = 2.0f;        //���̵�ƿ� �ð�                       6
  private void Awake()
  {
    instance = this;
   DontDestroyOnLoad(gameObject);

  }
  public void StartGame() //���� ����     ����ȭ�� ��ư���� ȣ��
  {
    StartCoroutine(startgame());
  }
  private IEnumerator startgame()
  {


    float _time = 0.0f;
    Color _color = Color.black;

    while (_time < FadeinTime)          //���� ���̵���
    {
      _time += Time.deltaTime;
      _color.a = _time / FadeinTime;
      FrontgroundImage.color = _color;
      yield return null;
    }
    _color.a = 1.0f;
    FrontgroundImage.color = _color;


    AsyncOperation _asynoper = SceneManager.LoadSceneAsync("GameScene");    //�� �ε� �� ���
    _asynoper.allowSceneActivation = false;

    yield return new WaitForSeconds(FadewaitTime);  //���̵��� �� ��� ���

    CutsceneImage.enabled = true;
    BackgroundImage.enabled = true;

    _time = 0.0f;
    while (_time < CutsceneIntime)          //��� �� �ƾ��� ���� �� ������ ����
    {
      _time += Time.deltaTime;
      _color.a = 1 - _time / CutsceneIntime;
      FrontgroundImage.color = _color;
      yield return null;
    }
    _color.a = 0.0f;
    FrontgroundImage.color = _color;

    _asynoper.allowSceneActivation = true;  //�� ������ �ε�

    CutsceneImage.rectTransform.sizeDelta = CutsceneSprites[0].bounds.size * SpriteSize;
    for(int i = 0; i < CutsceneSprites.Length; i++) //CutsceneWaittime���� ���� �̹����� ��ȯ
    {
      CutsceneImage.sprite= CutsceneSprites[i];
      yield return new WaitForSeconds(CutsceneWaittime);
    }

    _time = 0.0f;
    while (_time < CursceneOuttime)          //�ƾ� ���� �� �ٽ� ������ ���̵���
    {
      _time += Time.deltaTime;
      _color.a = _time / CursceneOuttime;
      FrontgroundImage.color = _color;
      yield return null;
    }
    _color.a = 1.0f;
    FrontgroundImage.color = _color;

    CutsceneImage.enabled = false;
    BackgroundImage.enabled = false;  //�ƾ� Image, ��� Image ��Ȱ��ȭ

    _time = 0.0f;
    while (_time < FadeoutTime)          //������ ���̵�ƿ�
    {
      _time += Time.deltaTime;
      _color.a =1- _time / FadeoutTime;
      FrontgroundImage.color = _color;
      yield return null;
    }
    _color.a = 0.0f;
    FrontgroundImage.color = _color;

    yield return new WaitForSeconds(0.5f);

    GameManager.Instance.Spawn();
  }
}
