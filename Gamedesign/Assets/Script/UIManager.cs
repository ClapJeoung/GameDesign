using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIManager : MonoBehaviour
{
  private static UIManager instance;
  public static UIManager Instance { get { return instance; } }
  public Image Button_A, Button_D, Button_Right, Button_Left;
  [SerializeField] private Image TextBackground = null;
  [SerializeField] private Image[] TextImages = null;
  [SerializeField] private Transform TextImages_active = null;
  [SerializeField] private Transform TextImages_disable = null;
  [SerializeField] private HorizontalLayoutGroup LayoutGroup = null;
  [SerializeField] private float TextTime = 2.0f;       //텍스트 출력까지 걸리는 시간(공통)
  [SerializeField] private int ShakeCount = 10;        //텍스트 초당 진동 횟수
  [SerializeField] private float ShakeDegree = 10.0f;   //텍스트 진동 범위
  private bool IsSoulDead = false;                      //횃불 사망 텍스트 출력중인지
  [SerializeField] private Image Fadeimg = null; //홍수 페이드 아웃에 쓸 이미지
  private void Awake()
  {
    instance = this;
    Fadeimg.color = Color.black;
  }

  private void Update()
  {
    if (Input.anyKeyDown && IsSoulDead) FinishText();
  }
  public void OutputHindi(Sprite[] sprites, float length)         //힌디 텍스트 출력 시작 ,length는 문장 전체 이미지의 길이
  {
    TextBackground.enabled = true;    //배경 키고
    float _sizeoffset = (TextBackground.GetComponent<RectTransform>().sizeDelta.x-75.0f) / length;  //글자 원본 크기 -> 목표 크기
    float _textlengths = 0;           
    foreach (var spr in sprites) _textlengths += spr.rect.size.x;
    float _space = (length - _textlengths) / (sprites.Length + 1);  //글자 사이 간격 계산하고
    LayoutGroup.spacing = _space;                                   //레이아웃 설정 대입
    float _width = 0.0f;   //이미지 넓이
    float _height = 0.0f;  //이미지 개당 높이
    for (int i = 0; i < TextImages.Length; i++) 
    {
      if (i < sprites.Length)
      {
        _width = sprites[i].rect.size.x * _sizeoffset;
        _height= sprites[i].rect.size.y * _sizeoffset;
   //     Debug.Log($"{sprites[i].rect.size.x} -> {_width}  {sprites[i].rect.size.y} -> {_height}");
        TextImages[i].sprite = sprites[i];
        TextImages[i].rectTransform.sizeDelta=new Vector2(_width, _height);
        TextImages[i].transform.SetParent(TextImages_active);
      }
     else TextImages[i].transform.SetParent(TextImages_disable);
    }
    for(int i = 0; i < sprites.Length; i++)
    {
      StartCoroutine(shaketext(TextImages[i].rectTransform));
    }
    StartCoroutine(drawtext(sprites.Length));
  }
  private IEnumerator drawtext(int textcount)
  {
    for(int i = 0; i < textcount; i++)
    {
      TextImages[i].enabled = true;
      yield return new WaitForSecondsRealtime(TextTime / (textcount - 1));
    }
    IsSoulDead = true;
  }
  private IEnumerator shaketext(RectTransform _rect)
  {
    yield return new WaitForEndOfFrame();
    Vector2 _origin = _rect.anchoredPosition;
    Vector2 _offset = Vector2.zero;
    while (true)
    {
      _offset=new Vector2(Random.Range(-ShakeDegree,ShakeDegree),Random.Range(-ShakeDegree,ShakeDegree));
      _rect.anchoredPosition = _origin + _offset;
      yield return new WaitForSecondsRealtime(1 / ShakeCount);
    }
  }
  public void FinishText()  //횃불 사망 텍스트 제거하고 홍수 시작
  {
    StopAllCoroutines();
    TextBackground.enabled = false;
    for(int i=0;i<TextImages.Length; i++)
      TextImages[i].enabled = false;
    IsSoulDead = false;
    GameManager.Instance.Flooded();
  }
  public void FadeIn(float fadetime) => StartCoroutine(fade(true, fadetime));
  public void FadeOut(float fadetime) => StartCoroutine(fade(false, fadetime));
  private IEnumerator fade(bool isin,float fadetime)
  {
    float _origin = isin ? 1.0f : 0.0f; //페이드인이면   불투명도 1 -> 0
    float _target = isin ? 0.0f : 1.0f; //페이드아웃이면 불투명도 0 -> 1
    float _current = _origin;
    Color _color = Color.black;
    _color.a = _origin;
    float _time = 0.0f;
    while (_time < fadetime)
    {
      _current = Mathf.Lerp(_origin, _target,Mathf.Pow(_time / fadetime,2.0f));
      _color.a = _current;
      Fadeimg.color = _color;
      _time += Time.deltaTime;
      yield return null;
    }
    _color.a = _target;
    Fadeimg.color = _color;
  }
}