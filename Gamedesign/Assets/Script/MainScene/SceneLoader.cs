using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader instance;
  public static SceneLoader Instance { get { return instance; } }
  [SerializeField] private Sprite[] CutsceneSprites = null; //컷씬 이미지들
  [SerializeField] private float SpriteSize = 10.0f;        //컷씬 이미지 배율
  [SerializeField] private Image FrontgroundImage = null;   //컷씬 가림막 이미지
  [SerializeField] private Image CutsceneImage = null;      //컷씬 보여줄 이미지
  [SerializeField] private Image BackgroundImage = null;    //컷씬 배경 이미지
  [SerializeField] private float FadeinTime = 4.5f;         //페이드인 시간                         1
  [SerializeField] private float FadewaitTime = 2.0f;       //페이드인 후 대기 시간                  2
  [SerializeField] private float CutsceneIntime = 1.0f;     //페이드인 대기 후 컷씬으로 열리는 시간   3
  [SerializeField] private float CutsceneWaittime = 3.0f;   //컷씬별 대기 시간                       4
  [SerializeField] private float CursceneOuttime = 2.0f;    //컷씬 끝나고 페이드인 되는 시간           5
  [SerializeField] private float FadeoutTime = 2.0f;        //페이드아웃 시간                       6
  private void Awake()
  {
    instance = this;
   DontDestroyOnLoad(gameObject);

  }
  public void StartGame() //게임 시작     메인화면 버튼에서 호출
  {
    StartCoroutine(startgame());
  }
  private IEnumerator startgame()
  {


    float _time = 0.0f;
    Color _color = Color.black;

    while (_time < FadeinTime)          //최초 페이드인
    {
      _time += Time.deltaTime;
      _color.a = _time / FadeinTime;
      FrontgroundImage.color = _color;
      yield return null;
    }
    _color.a = 1.0f;
    FrontgroundImage.color = _color;


    AsyncOperation _asynoper = SceneManager.LoadSceneAsync("GameScene");    //씬 로딩 후 대기
    _asynoper.allowSceneActivation = false;

    yield return new WaitForSeconds(FadewaitTime);  //페이드인 후 잠시 대기

    CutsceneImage.enabled = true;
    BackgroundImage.enabled = true;

    _time = 0.0f;
    while (_time < CutsceneIntime)          //대기 후 컷씬을 위해 앞 가림막 제거
    {
      _time += Time.deltaTime;
      _color.a = 1 - _time / CutsceneIntime;
      FrontgroundImage.color = _color;
      yield return null;
    }
    _color.a = 0.0f;
    FrontgroundImage.color = _color;

    _asynoper.allowSceneActivation = true;  //씬 완전히 로드

    CutsceneImage.rectTransform.sizeDelta = CutsceneSprites[0].bounds.size * SpriteSize;
    for(int i = 0; i < CutsceneSprites.Length; i++) //CutsceneWaittime마다 다음 이미지로 전환
    {
      CutsceneImage.sprite= CutsceneSprites[i];
      yield return new WaitForSeconds(CutsceneWaittime);
    }

    _time = 0.0f;
    while (_time < CursceneOuttime)          //컷씬 종료 후 다시 가림막 페이드인
    {
      _time += Time.deltaTime;
      _color.a = _time / CursceneOuttime;
      FrontgroundImage.color = _color;
      yield return null;
    }
    _color.a = 1.0f;
    FrontgroundImage.color = _color;

    CutsceneImage.enabled = false;
    BackgroundImage.enabled = false;  //컷씬 Image, 배경 Image 비활성화

    _time = 0.0f;
    while (_time < FadeoutTime)          //완전히 페이드아웃
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
