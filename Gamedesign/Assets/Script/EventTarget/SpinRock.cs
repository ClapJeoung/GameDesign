using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinRock:MonoBehaviour
{
  [SerializeField] private float SpinDeg = 270.0f;
  [SerializeField] private int VertexCount = 5;
  private Transform MyTransform = null;
  private Transform SpinTransform = null;
  private Vector2[] BottomVertex = null;
  private Vector2 Velocity = Vector2.zero;
  [SerializeField] private float Gravity = -9.8f;
  private int conveyor = 0;
  private int Conveyor
  {
    get { return conveyor; }  
    set {
      if (conveyor != 0 && value == 0) {IsPlaying = false;  }

        conveyor = value;
    }
  }
  [SerializeField] private float ConveyorSpeed = 5.0f;
  private StageCollider MySC = null;
  private Vector3 Originpos = Vector2.zero;
  private float ResetTime = 3.0f;
  private bool IsPlaying = false;

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
    IsPlaying = true;
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
        else if (_targethit.transform.CompareTag("Conveyor_R")) Conveyor = 1;
        else if (_targethit.transform.CompareTag("Conveyor_L")) Conveyor = -1;
        else Conveyor = 0;
        return;
      }
    }
    Conveyor = 0; //for 다 돌려서 여기까지 도착한거면 적어도 바닥에 컨베이어가 없다는것
  }
  private void Update()
  {
    if (!IsPlaying|| GameManager.Instance.CurrentSC != MySC) return; //현재 스테이지가 내 스테이지가 아니라면 물리효과 대기

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
      Debug.Log(IsPlaying);
      MyTransform.position = Vector3.Lerp(_currentpos, Originpos, Mathf.Sqrt(_time / ResetTime));
      _time += Time.deltaTime;
      yield return null;
    }
    IsPlaying = true;
  }

}
