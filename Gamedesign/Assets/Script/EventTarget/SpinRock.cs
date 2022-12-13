using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinRock:EventTarget
{
  [SerializeField] private float SpinDeg = 270.0f;
  [SerializeField] private int VertexCount = 5;
  private Transform MyTransform = null;
  private Transform SpinTransform = null;
  private Vector2[] BottomVertex = null;
  private Vector2 Velocity = Vector2.zero;
  [SerializeField] private float Gravity = -9.8f;
  [SerializeField] private float MoveSpeed = 5.0f;
  private StageCollider MySC = null;
  private Vector3 Originpos = Vector2.zero;
  private float ResetTime = 3.0f;
  [HideInInspector] public bool IsPlaying = false;
  [SerializeField] private Transform TargetPos = null;
  float TargetX = 0.0f;
  [SerializeField] private ParticleSystem DustParticle = null;
  [SerializeField] private ParticleSystem DestroyParticle = null;
  private AudioSource MyAudio = null;

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("Player")&&IsPlaying) collision.transform.GetComponent<Player_Move>().RollingStones();
  }
  private IEnumerator keepspin()
  {
    while (true)
    {
      SpinTransform.Rotate(Vector3.back * SpinDeg * Time.deltaTime);
      yield return null;
    }
  }
  public void Setup()
  {
    MyAudio = GetComponent<AudioSource>();
    TargetX=TargetPos.position.x;
    MySC = transform.parent.GetComponent<StageCollider>();
    MySC.MySpinRock = this;
    MyTransform = transform;
    SpinTransform = MyTransform.GetChild(0).transform;
    Originpos = MyTransform.position;
    CircleCollider2D _mycol = GetComponent<CircleCollider2D>();
    Bounds mybound = _mycol.bounds;
    mybound.Expand(0.005f);
    BottomVertex = new Vector2[VertexCount];
    float _width = mybound.size.x;
    float _widthunit = _width / (VertexCount - 1);
    float _height = mybound.size.y;
    for (int i = 0; i < VertexCount; i++)
    {
      BottomVertex[i] = new Vector2(-_width / 2 + _widthunit * i, -_height / 2);
    }
    StartCoroutine(keepspin());
  }
  private void Start()
  {
    Invoke("Setup", 0.01f);
  }
  public virtual void VerticalRaycast()
  {
    RaycastHit2D[] _hit;
    RaycastHit2D _targethit = new RaycastHit2D();
    bool _doitagain = false;
    Vector2 _newpos;
    float _distance;
    LayerMask _layermask = LayerMask.NameToLayer("Wall");
    for (int i = 0; i < VertexCount; i++)
    {
      _newpos = (Vector2)MyTransform.position + BottomVertex[i];
      _distance = Mathf.Abs(Velocity.y) * Time.deltaTime;
      _doitagain = false;

      _hit = Physics2D.RaycastAll(_newpos, Vector2.down, _distance, 1 << LayerMask.NameToLayer("Wall"));

      for (int j = _hit.Length - 1; j >= 0; j--)
      {
        _targethit = _hit[j]; //_hit의 맨 뒤부터(제일 먼저 걸린 것부터) _targethit에 대입
        if (_targethit.transform == transform)  //_targethit에 있는게 자신일 경우
        {
          if (_hit.Length == 1) _doitagain = true;  //그리고 유일한 요소가 자신일 경우라면 넘어가기
          else continue;  //아직 더 검사할 요소가 있다면 다음으로 넘어가기
        }
        break;  //자신이 아닐 경우 그대로 진행
      }
      if (_doitagain) continue;

      if (_targethit.transform != null)
      {
        Wooden iswooden;
        if (_targethit.transform.TryGetComponent<Wooden>(out iswooden) && !iswooden.IsActive) continue; //목재 비활성화면 무시

        Velocity.y = _targethit.distance * -1;

        if (_targethit.transform.CompareTag("Breakable"))
          _targethit.transform.GetComponent<Breakable>().Pressed(); //밟아서 부숴지는 이벤트 실행
        return;
      }
    }
  }
  private void Update()
  {
    if (!IsPlaying|| GameManager.Instance.CurrentSC != MySC) return; //현재 스테이지가 내 스테이지가 아니라면 물리효과 대기

    Velocity.y = Gravity;

    VerticalRaycast();

    if (Velocity.y <= 0.05f && Velocity.y >= -0.05f) Velocity.y = 0;
    MyTransform.Translate((Velocity + Vector2.right * MoveSpeed) * Time.deltaTime);
    if (MyTransform.position.x >= TargetX) { Destroyed();AudioManager.Instance.PlayClip(17);MyAudio.Stop(); }
  }
  public  void Resetpos() => StartCoroutine(resetpos());
  private IEnumerator resetpos()
  {
    Destroyed();
    IsPlaying = false;
    float _time = 0.0f;
    Vector3 _currentpos = MyTransform.position;
   SpriteRenderer _myspr= SpinTransform.GetComponent<SpriteRenderer>();
    float _alpha = 0.0f;
    Color _color = Color.white;
    _color.a = _alpha;
    _myspr.enabled = true;
    while (_time < ResetTime)
    {
      MyTransform.position = Vector3.Lerp(_currentpos, Originpos, Mathf.Sqrt(_time / ResetTime));
      _alpha = _time / ResetTime;
      _color.a = _alpha;
      _myspr.color = _color;
      _time += Time.deltaTime;
      yield return null;
    }
    GetComponent<CircleCollider2D>().enabled = true;
    _color.a = 1.0f;
    _myspr.color = _color;
  }
  public override void Active()
  {
    if (IsPlaying) return;
    Debug.Log("레후");
    IsPlaying = true;
    DustParticle.Play();
    SpinTransform.GetComponent<SpriteRenderer>().enabled = true;
    GameManager.Instance.MyCamera.StartSpinRock();
    MyAudio.Play();
  }
  public void Destroyed()
  {
    if (!IsPlaying) return;
    IsPlaying = false;
    SpinTransform.GetComponent<SpriteRenderer>().enabled = false;
    DestroyParticle.Play();
    DustParticle.Stop();
    GetComponent<CircleCollider2D>().enabled = false;
    GameManager.Instance.MyCamera.EndSpinRock();
  }
  private void OnDrawGizmos()
  {
    if (TargetPos == null) return;
    Gizmos.color = Color.red;
    Gizmos.DrawLine(transform.position, TargetPos.position);
  }
  public override void Deactive() { Destroyed(); }

}
