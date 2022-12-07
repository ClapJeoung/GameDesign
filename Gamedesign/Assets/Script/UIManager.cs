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
  private bool IsSoulDead = false;                      //횃불 사망 텍스트 출력중인지
  [SerializeField] private Image Fadeimg = null; //홍수 페이드 아웃에 쓸 이미지
  [SerializeField] private Image RestartLogo = null;  //재시작 로고
  [SerializeField] private SouldeathModule SDModule = null; //영혼사망 각종 데이터
  [SerializeField] private SDEffect MySDE = null;
  private int TextCount = 0;
  private void Awake()
  {
    instance = this;
    Fadeimg.color = Color.black;
  }

  private void Update()
  {
  //  if (Input.anyKeyDown && IsSoulDead) FinishText();
  }
  public void RespawnHindi(Sprite[] sprites, float length)
  {
    TextCount = sprites.Length;
    float _sizeoffset = 1.0f;
    // TextBackground.enabled = true;    //배경 키고
    float _maxy = 0.0f; foreach(Sprite spr in sprites)if(spr.rect.size.y>_maxy)_maxy=spr.rect.size.y;

    Vector2 _maxsize = TextBackground.GetComponent<RectTransform>().sizeDelta;
    if (_maxy > _maxsize.y)
    {
      _sizeoffset = (_maxsize.y - 50.0f) / _maxy;
    }
    else if(length>_maxsize.x){
      _sizeoffset = (_maxsize.x - 50.0f) / length;  //글자 원본 크기 -> 목표 크기
    }

    float _textlengths = 0;
    foreach (var spr in sprites) _textlengths += spr.rect.size.x;
    float _space = (length - _textlengths) / (sprites.Length -1) * (_sizeoffset);  //글자 사이 간격 계산하고
  //  Debug.Log($"length : {length}   textlength : {_textlengths}  count : {sprites.Length}");
    LayoutGroup.spacing = _space;                                   //레이아웃 설정 대입
    float _width = 0.0f;   //이미지 넓이
    float _height = 0.0f;  //이미지 개당 높이
    for (int i = 0; i < TextImages.Length; i++)
    {
      if (i < TextCount)
      {
        _width = sprites[i].rect.size.x * _sizeoffset;
        _height = sprites[i].rect.size.y * _sizeoffset;
        //     Debug.Log($"{sprites[i].rect.size.x} -> {_width}  {sprites[i].rect.size.y} -> {_height}");
        TextImages[i].sprite = sprites[i];
        TextImages[i].rectTransform.sizeDelta = new Vector2(_width, _height);
        TextImages[i].transform.SetParent(TextImages_active);
      }
      else TextImages[i].transform.SetParent(TextImages_disable);
    }
    StartCoroutine(drawtext_respawn(TextCount,new Vector2(length,_maxy)*_sizeoffset, new Vector3(960.0f, 540.0f + 287.0165f)));
    Invoke("_asdf", 3.0f);
  }
  private IEnumerator drawtext_respawn(int textcount,Vector2 _lscale,Vector2 _lpos)
  {
    Vector2[] _pos = new Vector2[TextCount];
    Vector2[] _scale = new Vector2[TextCount];
    yield return new WaitForEndOfFrame();
    for (int i = 0; i < textcount; i++)
    {
      _scale[i] = TextImages[i].rectTransform.sizeDelta;
      _pos[i] = TextImages[i].transform.position;
    }
    MySDE.SetPosition(_pos, _scale,_lscale,_lpos);
    yield return new WaitForSeconds(0.5f);
    for (int i = 0; i < textcount; i++)
    {
      StartCoroutine(alphatext(i, 1.0f));
      yield return new WaitForSecondsRealtime(1.0f / (textcount - 1));
    }
    IsSoulDead = true;
  }
  private void _asdf()
  {
    MySDE.TurnOffBasic();
    for (int i = 0; i < TextCount; i++)
      StartCoroutine(alphatext(i, 0.0f));
  }
  public void OutputHindi(Sprite[] sprites, float length)         //힌디 텍스트 출력 시작 ,length는 문장 전체 이미지의 길이
  {
    TextCount = sprites.Length;
    float _sizeoffset = 1.0f;
    // TextBackground.enabled = true;    //배경 키고
    float _maxy = 0.0f; foreach (Sprite spr in sprites) if (spr.rect.size.y > _maxy) _maxy = spr.rect.size.y;

    Vector2 _maxsize = TextBackground.GetComponent<RectTransform>().sizeDelta;
    if (_maxy > _maxsize.y)
    {
      _sizeoffset = (_maxsize.y - 50.0f) / _maxy;
    }
    else if (length > _maxsize.x)
    {
      _sizeoffset = (_maxsize.x - 50.0f) / length;  //글자 원본 크기 -> 목표 크기
    }

    float _textlengths = 0;           
    foreach (var spr in sprites) _textlengths += spr.rect.size.x;
    float _space = (length - _textlengths) / (sprites.Length - 1)*(_sizeoffset);  //글자 사이 간격 계산하고
    LayoutGroup.spacing = _space;                                   //레이아웃 설정 대입
    float _width = 0.0f;   //이미지 넓이
    float _height = 0.0f;  //이미지 개당 높이
    for (int i = 0; i < TextImages.Length; i++) 
    {
      if (i < TextCount)
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
    for(int i = 0; i < TextCount; i++)
    {
      StartCoroutine(shaketext(TextImages[i].rectTransform));
    }
    StartCoroutine(drawtext(TextCount, new Vector2(length, _maxy) * _sizeoffset, new Vector3(960.0f,540.0f+ 287.0165f)));
    Invoke("FinishText", SDModule.Text_Waittime + SDModule.Text_AppearingTime + SDModule.Text_DestroyWaittime);
  }
  private IEnumerator drawtext(int textcount, Vector2 _lscale, Vector2 _lpos)
  {
    Vector2[] _pos = new Vector2[TextCount];
    Vector2[] _scale = new Vector2[TextCount];
    yield return new WaitForEndOfFrame();
    for(int i = 0; i < textcount; i++)
    {
      _scale[i] = TextImages[i].rectTransform.sizeDelta;
      _pos[i] = TextImages[i].transform.position;
    }
    MySDE.SetPosition(_pos, _scale, _lscale, _lpos);
    yield return new WaitForSeconds(SDModule.Text_Waittime);
    for(int i = 0; i < textcount; i++)
    {
      StartCoroutine(alphatext(i, 1.0f));
      yield return new WaitForSecondsRealtime(SDModule.Text_AppearingTime / (textcount - 1));
    }
    IsSoulDead = true;
  }
  private IEnumerator alphatext(int num, float targetalpha)
  {
    if (targetalpha > 0) MySDE.TurnOn(num);
    float _time = 0.0f, _targettime = 0.2f;
    float _origin = TextImages[num].color.a;
    Color _col = Color.white; _col.a = _origin;
    TextImages[num].enabled = true;
    while (_time < _targettime)
    {
      _col.a = Mathf.Lerp(_origin, targetalpha, _time / _targettime);
      TextImages[num].color = _col;
      _time += Time.deltaTime; yield return null;
    }
    _col.a = targetalpha; TextImages[num].color = _col;
  }
  private IEnumerator shaketext(RectTransform _rect)
  {
    yield return new WaitForEndOfFrame();
    Vector2 _origin = _rect.anchoredPosition;
    Vector2 _offset = Vector2.zero;
    var _wait = new WaitForSeconds(1 / SDModule.Text_ShakeCount);
    while (true)
    {
      _offset=new Vector2(Random.Range(-SDModule.Text_ShakeDeg, SDModule.Text_ShakeDeg),Random.Range(-SDModule.Text_ShakeDeg, SDModule.Text_ShakeDeg));
      _rect.anchoredPosition = _origin + _offset;
      yield return _wait;
    }
  }
  public void FinishText()  //횃불 사망 텍스트 제거하고 홍수 시작
  {
    StopAllCoroutines();
    TextBackground.enabled = false;
    for(int i = 0; i < TextCount; i++)
      StartCoroutine(alphatext(i, 0.0f));
    MySDE.TurnOffAll();
    IsSoulDead = false;
    //   GameManager.Instance.FinishText();
    FadeOut(SDModule.Flood_FadeOutTime, true);
  }
  public void FadeIn(float fadetime) => StartCoroutine(fade(true, fadetime,false));
  public void FadeOut(float fadetime,bool isdead) => StartCoroutine(fade(false, fadetime,isdead));
  private IEnumerator fade(bool isin,float fadetime,bool isdead)
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
    if (!isin&&isdead)
    {
      StartCoroutine(turnlogo(true));
    }
  }
  private IEnumerator turnlogo(bool ison)
  {
    float _time = 0.0f, _targettime = 1.0f;
    float _origin = ison ? 0.0f : 1.0f; //페이드인이면   불투명도 1 -> 0
    float _target = ison ? 1.0f : 0.0f; //페이드아웃이면 불투명도 0 -> 1
    float _current = _origin;
    Color _color = Color.white;
    _color.a = _origin;
    while (_time < _targettime)
    {
      _current = Mathf.Lerp(_origin, _target, Mathf.Pow(_time / _targettime, 2.0f));
      _color.a = _current;
      RestartLogo.color = _color;
      _time += Time.deltaTime;
      yield return null;
    }
    _color.a = _target;
    RestartLogo.color = _color;
    if (!ison)
    {
      yield return new WaitForSeconds(0.3f);
      UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
  }
  public void TurnOffRestartLogo() => StartCoroutine(turnlogo(false));

}