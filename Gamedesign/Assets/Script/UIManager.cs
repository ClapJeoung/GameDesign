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
  [SerializeField] private float TextTime = 2.0f;       //�ؽ�Ʈ ��±��� �ɸ��� �ð�(����)
  [SerializeField] private int ShakeCount = 10;        //�ؽ�Ʈ �ʴ� ���� Ƚ��
  [SerializeField] private float ShakeDegree = 10.0f;   //�ؽ�Ʈ ���� ����
  private bool IsSoulDead = false;                      //ȶ�� ��� �ؽ�Ʈ ���������
  [SerializeField] private Image Fadeimg = null; //ȫ�� ���̵� �ƿ��� �� �̹���
  private void Awake()
  {
    instance = this;
    Fadeimg.color = Color.black;
  }

  private void Update()
  {
    if (Input.anyKeyDown && IsSoulDead) FinishText();
  }
  public void OutputHindi(Sprite[] sprites, float length)         //���� �ؽ�Ʈ ��� ���� ,length�� ���� ��ü �̹����� ����
  {
    TextBackground.enabled = true;    //��� Ű��
    float _sizeoffset = (TextBackground.GetComponent<RectTransform>().sizeDelta.x-75.0f) / length;  //���� ���� ũ�� -> ��ǥ ũ��
    float _textlengths = 0;           
    foreach (var spr in sprites) _textlengths += spr.rect.size.x;
    float _space = (length - _textlengths) / (sprites.Length + 1);  //���� ���� ���� ����ϰ�
    LayoutGroup.spacing = _space;                                   //���̾ƿ� ���� ����
    float _width = 0.0f;   //�̹��� ����
    float _height = 0.0f;  //�̹��� ���� ����
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
  public void FinishText()  //ȶ�� ��� �ؽ�Ʈ �����ϰ� ȫ�� ����
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
    float _origin = isin ? 1.0f : 0.0f; //���̵����̸�   ������ 1 -> 0
    float _target = isin ? 0.0f : 1.0f; //���̵�ƿ��̸� ������ 0 -> 1
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