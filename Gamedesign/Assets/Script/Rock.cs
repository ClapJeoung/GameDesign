using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
  [SerializeField] private int VertexCount = 5;
  private Transform MyTransform = null;
  private Vector2[] BottomVertex = null;
  private Vector2 Velocity = Vector2.zero;
  [SerializeField] private float Gravity = -9.8f;
  private int Conveyor = 0;
  [SerializeField] private float ConveyorSpeed = 5.0f;
  private StageCollider MySC = null;
  private Vector3 Originpos = Vector2.zero;
  private bool IsPlaying = false;
  private float ResetTime = 3.0f;
  [HideInInspector] public bool IsLanding = false;
  private ParticleSystem Dust_0 = null;
  private ParticleSystem Dust_1 = null;
  public void Setup()
  {
    MySC = transform.parent.GetComponent<StageCollider>();
    MySC.SetOrigin(this);
    MyTransform = transform;
    Originpos = MyTransform.position;
    BoxCollider2D _mycol = GetComponent<BoxCollider2D>();
    _mycol.size = GetComponent<SpriteRenderer>().size;
    Bounds mybound= _mycol.bounds;
    mybound.Expand(0.01f);
    BottomVertex = new Vector2[VertexCount];
    float _width = mybound.size.x;
    float _widthunit=_width / (VertexCount - 1);
    float _height = mybound.size.y;
    mybound.Expand(-0.01f);
    for(int i = 0; i < VertexCount; i++)
    {
      BottomVertex[i] = new Vector2(- _width/2 + _widthunit * i, -_height / 2);
    }
    float _erea = _width * _height;
    int _particlecount_min_min = 3, _particlecount_min_max = 5, _particlecount_max_min = 15, _particlecount_max_max = 20;
    float _speed_min = 3.0f, _speed_max = 6.0f, _lifetime_min = 0.5f, _lifetime_max = 1.5f,
      _size_min = 0.2f, _size_max = 0.4f,_radius_min = 0.1f, _radius_max = 0.5f;
    float _ratio = Mathf.Lerp(0.0f, 1.0f, _erea / 10);

    Dust_0 = MyTransform.GetChild(0).GetComponent<ParticleSystem>();
    Dust_1 = MyTransform.GetChild(1).GetComponent<ParticleSystem>();

    var _target = Dust_0.main;
    _target.startLifetime = Mathf.Lerp(_lifetime_min,_lifetime_max, _ratio);
    _target.startSize = Mathf.Lerp(_size_min, _size_max, _ratio);
    _target.startSpeed=Mathf.Lerp(_speed_min,_speed_max, _ratio);
    _target = Dust_1.main;
    _target.startLifetime = Mathf.Lerp(_lifetime_min, _lifetime_max, _ratio);
    _target.startSize = Mathf.Lerp(_size_min, _size_max, _ratio);
    _target.startSpeed = Mathf.Lerp(_speed_min, _speed_max, _ratio);

    var _target_0 = Dust_0.emission;
    var _mmc =new ParticleSystem.MinMaxCurve();
    _mmc.constantMin=Mathf.Lerp(_particlecount_min_min, _particlecount_min_max, _ratio);
    _mmc.constantMax = Mathf.Lerp(_particlecount_max_min, _particlecount_max_max, _ratio);
    _target_0.rateOverTime = _mmc;
    _target_0 = Dust_1.emission;
    _target_0.rateOverTime= _mmc;

    var _target_1 = Dust_0.shape;
    _target_1.radius= Mathf.Lerp(_radius_min, _radius_max, _ratio);
    _target_1 = Dust_1.shape;
    _target_1.radius= Mathf.Lerp(_radius_min, _radius_max, _ratio);

  }
  private void Start()
  {
    Setup();
  }
  public virtual void VerticalRaycast()
  {
    RaycastHit2D[] _hit;
    RaycastHit2D _targethit=new RaycastHit2D();
    bool _doitagain = false;
    Vector2 _newpos;
    float _distance;
    LayerMask _layermask = LayerMask.NameToLayer("Wall");
    for (int i = 0; i < VertexCount; i++)
    {
      _newpos = (Vector2)MyTransform.position + BottomVertex[i];
      _distance =Mathf.Abs( Velocity.y) * Time.deltaTime;
      _doitagain = false;

      _hit = Physics2D.RaycastAll(_newpos, Vector2.down,_distance, 1 << LayerMask.NameToLayer("Wall"));

      for(int j=_hit.Length-1; j>=0; j--)
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
        else if (_targethit.transform.CompareTag("Conveyor_R")) Conveyor = 1;
        else if (_targethit.transform.CompareTag("Conveyor_L")) Conveyor = -1;
        else Conveyor = 0;

        if (IsLanding == false) { IsLanding = true; PlayDust(); }    //이 오브젝트는 땅에 붙어있다
        return;
      }
    }
    Conveyor = 0; 
    if (IsLanding == true) IsLanding = false; //하나도 감지하지 않았다면 완전히 땅에서 떨어져있는거
  }
  private void PlayDust()
  {
    Dust_0.Play();
    Dust_1.Play();
  }
  private void FixedUpdate()
  {
    if (GameManager.Instance.CurrentSC != MySC) return; //현재 스테이지가 내 스테이지가 아니라면 물리효과 대기

    Velocity.y = Gravity;

    VerticalRaycast();
    if (Velocity.y <= 0.05f && Velocity.y >= -0.05f) Velocity.y = 0;
    MyTransform.Translate((Velocity + Vector2.right * ConveyorSpeed * Conveyor) * Time.deltaTime);
  }
  public virtual void Resetpos() => StartCoroutine(resetpos());
  private IEnumerator resetpos()
  {
    Conveyor = 0;
    IsPlaying = false;
    float _time = 0.0f;
    Vector3 _currentpos = MyTransform.position;
    while (_time < ResetTime)
    {
      MyTransform.position=Vector3.Lerp(_currentpos,Originpos,Mathf.Sqrt(_time/ResetTime));
      _time += Time.deltaTime;
      yield return null;
    }
    IsPlaying = true;
  }
}
