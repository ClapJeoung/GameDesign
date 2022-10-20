using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : MonoBehaviour
{
  private Vector2 velocity = Vector2.zero;            //현재 속도
  private Vector2 Velocity
  {
    get { return velocity; }
    set {
      if (velocity.y > 0 && value.y <= 0) MyAnimator.SetTrigger("JumpisDone");
      if (velocity.y < 0 && value.y >= 0) MyAnimator.SetTrigger("FallisDone");

      velocity = value;

      //플레이어 이동속도가 0.1 이하면 걷기도 아니다
    }
  }
  private Vector2 NextVelocity = Vector2.zero;
  [SerializeField] private float AccelDegree = -3.0f; //이동 입력시 가속도
  [SerializeField] private float AccelResist = -3.0f; //멈춤 가속도
  [SerializeField] private float MaxXSpeed = 5.0f;    //최대 X 속도(절댓값)
  [Space(5)]
  private Vector2 Accel = Vector2.zero;               //현재 가속도
  [SerializeField] private float GrvtDegree = -9.8f;  //중력가속도
  [SerializeField] private float JumpPower = 8.0f;    //점프 입력시 바뀌는 속도
  [SerializeField] private float MaxJumpTime = 0.6f;  //최대 점프 시간
  [SerializeField] private float MaxYSpeed = 10.0f;   //최대 Y 속도(절댓값)
  [Space(5)]
  [SerializeField] private int VertexCount = 5;       //충돌 검사할 점 개수
  private Vector2[] Vertex_top, Vertex_bottom,Vertex_right,Vertex_left;//충돌 검사할 점 위치
  private Transform MyTransform;                      //내 트랜스폼
  private bool Jumpable = true;                       //지금 점프 가능한지
  private float jumptime = 0.0f;
  private float JumpTime { get { return jumptime; } set { jumptime = value; if (jumptime >= MaxJumpTime) Jumpable = false; } }

  [SerializeField] private TMPro.TextMeshProUGUI asdf = null;
  private int Conveyor = 0; //-1 / 0 / +1   좌측 X 우측
  [Space(5)]
  [SerializeField] private float ConveyorSpeed = 3.0f;  //컨베이어벨트 위 속도
  private float WaterAccel = 90.0f; //물 떠다니는 가속에 쓸 수치
  [Space(5)]
  [SerializeField] private float WaterSpeed = 1.0f; //물 떠다니는 주기
  [SerializeField] private float FloatingDegree = 1.0f; //물 떠다니는 반경
  private bool IsWater = false;      //물 속에 있는지
  private bool IsDead = false;  //사망 시 모든 활동 정지시키기
  [Space(5)]
  [SerializeField] private int ShakeCount = 55;
  [SerializeField] private float ShakeTime = 4.0f;
  [SerializeField] private float ShakeDegree = 0.05f;
  [SerializeField] private SpriteRenderer MySpr = null;
  [SerializeField] private Animator MyAnimator = null;
  [Space(5)]
  [SerializeField] private ParticleSystem WaterDownParticle = null;
  private ParticleSystem.ShapeModule WaterDownShape;
  [SerializeField] private ParticleSystem WaterUpParticle = null;
  private ParticleSystem.ShapeModule WaterUpShape;
  [SerializeField] private ParticleSystem DeadParticle = null;
  private ParticleSystem.ShapeModule DeadShape;
  private bool flipx = true;
  private bool IsPlaying = true;
  private bool IsPressing = false;
  public void Setup()
  {
    MyTransform = transform;
    BoxCollider2D Col = GetComponent<BoxCollider2D>();

    Bounds MyBound = Col.bounds;
    Vertex_top = new Vector2[VertexCount]; Vertex_bottom = new Vector2[VertexCount];
    Vertex_right = new Vector2[VertexCount]; Vertex_left = new Vector2[VertexCount]; //버텍스 개수 설정
    MyBound.Expand(-0.025f);
    float _width = MyBound.size.x;
    float _height = MyBound.size.y;
    float _size_width = _width / (VertexCount-1);
    float _size_height = _height / (VertexCount-1);                                 //바운드 사이즈, 단위 설정

    for (int i = 0; i < VertexCount; i++)
    {
      Vertex_top[i] = new Vector2(-_width / 2 + _size_width * i, _height / 2);
      Vertex_bottom[i] = new Vector2(-_width / 2 + _size_width * i, -_height / 2);
      Vertex_right[i] = new Vector2(_width / 2, -_height / 2 + _size_height * i);
      Vertex_left[i] = new Vector2(-_width / 2, -_height / 2 + _size_height * i);
    }
    MyTransform.localScale = Vector3.zero;
    WaterDownShape = WaterDownParticle.shape;
    WaterUpShape = WaterUpParticle.shape;
    DeadShape = DeadParticle.shape;
    MyBound.Expand(+0.025f);
    IsDead = true;
    IsPlaying = false;
  }
  public void Start()
  {
    Setup();
    GameManager.Instance.SetNewPlayer(this.transform);
  }
  private void UpdateMove()
  {
    if (Input.GetKeyDown(KeyCode.Escape)) Time.timeScale = 0.3f;
    if (IsDead) return;

    Accel.y = GrvtDegree;

    if (IsPlaying)  //조작중일때만
    {
      if (IsWater)
      {
        WaterAccel += Time.deltaTime * WaterSpeed;
        Accel.y = (Mathf.Cos(Mathf.Deg2Rad * WaterAccel) * FloatingDegree);
      }

      Accel.x = 0;
      if (Input.GetKey(KeyCode.D)) { Accel.x = AccelDegree; flipx = false; IsPressing = true; }                      //좌측 버튼 : 가속도가 +
      else if (Input.GetKey(KeyCode.A)) { Accel.x = -AccelDegree; flipx = true; IsPressing = true; }                 //우측 버튼 : 가속도가 -
      else {
        IsPressing = false;
        Accel.x = (NextVelocity.x != 0 ? Mathf.Sign(NextVelocity.x) : 0) * AccelResist; }//아무것도 안 누름 : 가속도가 속도 반대로
    }

    if (Input.GetKey(KeyCode.Space) && Jumpable) Jump();//점프

    if(MySpr.flipX!=flipx)MySpr.flipX = flipx;


    NextVelocity += Accel * Time.deltaTime; //속도에 가속추가
    if (IsPressing == false)  //이동을 누르지 않았는데
    {
      if(Mathf.Sign(Velocity.x)!=Mathf.Sign(NextVelocity.x)||(Velocity.x==0&NextVelocity.x!=0))NextVelocity.x = 0;
      //가속도 때문에 속도의 부호가 바뀌어버리거나 정지해 있던게 이동한다면 0으로 교정
    }

    if (asdf!=null) asdf.text = Velocity.ToString();  //디버그용 텍스트
    RaycastVertical();
    RaycastHorizontal();


    NextVelocity = new Vector2(Mathf.Clamp(NextVelocity.x, -MaxXSpeed, MaxXSpeed), Mathf.Clamp(NextVelocity.y, -MaxYSpeed, MaxYSpeed)); //속도 제한치

   // Velocity = new Vector2(Mathf.Abs(NextVelocity.x) < 0.05f ? 0 : NextVelocity.x, NextVelocity.y); //미세 떨림 없게
  //  Debug.Log($"{newvel} -> {Velocity}");

    Velocity = NextVelocity;

    MyAnimator.SetBool("IsWalking", Velocity.x !=0.0f);

    MyTransform.Translate((Velocity+ Vector2.right * ConveyorSpeed * Conveyor) * Time.deltaTime);
   // Debug.Log("Accel : " + Accel);
  }
  private void Jump()
  {
    NextVelocity = new Vector2(NextVelocity.x, JumpPower); 
    JumpTime += Time.deltaTime; 
    MyAnimator.SetTrigger("Jump");
  }
  private void RaycastVertical()  //위아래 검사
  {
    Vector2[] _pos = NextVelocity.y <= 0 ? Vertex_bottom : Vertex_top; //선이 시작되는 위치(바운드 기준)
    Vector2 _dir = NextVelocity.y <= 0?Vector2.down : Vector2.up;      //선이 발사되는 위치
    Vector3 _newpos = Vector3.zero;                               //선이 시작되는 위치(플레이어 기준)
    int _layermask;          //레이어마스크(int)
    float _distance = 0.025f+ Mathf.Abs(NextVelocity.y) * Time.deltaTime;                //선이 발사되는 거리
    if (_distance == 0.0f) _distance = 0.025f;
    RaycastHit2D _hit;                                            //선이 발사되고 닿은 곳의 정보
    Conveyor = 0; //컨베이어 초기화

    for (int i = 0; i < VertexCount; i++)
    {
      _layermask = 1 << LayerMask.NameToLayer("Wall");  //벽 역할을 하는 레이어 검사
      _newpos = MyTransform.position + (Vector3)_pos[i];
      _hit = Physics2D.Raycast(_newpos, _dir, _distance, _layermask) ;
      Debug.DrawRay(_newpos, _dir * _distance, Color.red);
      if (_hit.transform != null)
      {
        Wooden iswooden;
        if (_hit.transform.TryGetComponent<Wooden>(out iswooden) && !iswooden.IsActive) continue; //목재 비활성화면 무시

        if (NextVelocity.y < 0) { Jumpable = true; JumpTime = 0.0f; }

        NextVelocity = new Vector2(NextVelocity.x,(_hit.distance- 0.025f )* _dir.y);
        //      Debug.Log(_hit.transform.tag);
        Debug.DrawRay(_newpos, _dir * _hit.distance, Color.green);

        if (_hit.transform.CompareTag("Breakable"))
          _hit.transform.GetComponent<Breakable>().Pressed(); //밟아서 부숴지는 이벤트 실행
        else if (_hit.transform.CompareTag("Conveyor_R")) Conveyor = 1;
        else if (_hit.transform.CompareTag("Conveyor_L")) Conveyor = -1;
        break;
      }
     if(!IsWater) Jumpable = false;
      if (NextVelocity.y > 0) continue; //점프 중이면 여기서 종료, 하강 중이면 Upper 블록도 발판으로 인식
      _layermask = 1 << LayerMask.NameToLayer("Upper");  //윗블록 검사
      _hit = Physics2D.Raycast(_newpos, _dir, _distance, _layermask);
      if (_hit.transform != null)
      {
        if (NextVelocity.y < 0) { Jumpable = true; JumpTime = 0.0f; }
        NextVelocity = new Vector2(NextVelocity.x, _hit.distance * _dir.y);
        break;
      }
    }
  }
  private void RaycastHorizontal()//좌우 검사
  {
    //   if (NextVelocity.x == 0) return;

    Vector2[] _pos = NextVelocity.x >= 0 ? Vertex_right : Vertex_left; //선이 시작되는 위치(바운드 기준)
    Vector2 _dir = NextVelocity.x >= 0 ? Vector2.right : Vector2.left;      //선이 발사되는 방향
    Vector3 _newpos = Vector3.zero;                               //선이 시작되는 위치(플레이어 기준)
    int _layermask = 1 << LayerMask.NameToLayer("Wall");          //레이어마스크(int)
    float _distance = 0.025f+ Mathf.Abs(NextVelocity.x) * Time.deltaTime;                //선이 발사되는 거리
    if (_distance == 0.0f) _distance = 0.025f;
    RaycastHit2D _hit;                                            //선이 발사되고 닿은 곳의 정보


    for (int i = 0; i < VertexCount; i++)
    {
      _newpos = MyTransform.position + (Vector3)_pos[i];
      _layermask = 1<< LayerMask.NameToLayer("Wall");  //벽 검사
      _hit = Physics2D.Raycast(_newpos, _dir, _distance, _layermask);
      Debug.DrawRay(_newpos, _dir * _distance, Color.red);
      if (_hit.transform != null)
      {
   //  Debug.Log($"{_hit.transform.name}  {_hit.transform.position}");
        Wooden iswooden;
        if (_hit.transform.TryGetComponent<Wooden>(out iswooden) && !iswooden.IsActive) continue; //목재 비활성화면 무시

        //   NextVelocity = new Vector2((_hit.distance>0.1f?_hit.distance - 0.1f:_hit.distance)*_dir.x,NextVelocity.y );
        NextVelocity = new Vector2( (_hit.distance- 0.025f) * _dir.x, NextVelocity.y);
        Debug.DrawRay(_newpos, _dir * _hit.distance, Color.green);
        break;
      }
      //   Debug.Log($"_distance : {_distance}  hit.distance : {_hit.distance}  NextVelocity.x : {NextVelocity.x}");
    }
  }
  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.gameObject.layer ==  LayerMask.NameToLayer("Water"))
    {
      NextVelocity = new Vector2(NextVelocity.x, 0.0f); IsWater = true; WaterAccel = 180.0f; Jumpable = true;
      WaterDownShape.position = MyTransform.position + Vector3.down * 0.5f;
      WaterDownParticle.Play();
    }
  }
  private void OnTriggerExit2D(Collider2D collision)
  {
    if (collision.gameObject.layer ==LayerMask.NameToLayer("Water"))
    {
      IsWater = false;
      WaterUpShape.position = MyTransform.position + Vector3.down * 0.5f;
      WaterUpParticle.Play();
    }
  }
  private void FixedUpdate()
  {
    UpdateMove();
  }
  public void Dead() => StartCoroutine(dead());
  private IEnumerator dead()
  {
    GameManager.Instance.Dead();
    IsDead = true;
    IsPlaying = false;
    yield return new WaitForSeconds(1.0f);
    Vector3 _originpos = MyTransform.position;
    for(int i = 0; i < ShakeCount; i++)
    {
      MyTransform.position = _originpos + new Vector3 (Random.Range(-ShakeDegree, ShakeDegree), Random.Range(-ShakeDegree, ShakeDegree))+Vector3.back*2;
      yield return new WaitForSeconds(ShakeTime / ShakeCount);
    }
    MyTransform.position = _originpos;

    MyTransform.localScale = Vector3.zero;
    DeadShape.position = MyTransform.position;
    DeadParticle.Play();

    yield return new WaitForSeconds(1.0f);

    GameManager.Instance.Respawn();
    yield return null;
  }
  public void Respawn(Vector2 newpos,float targetx,bool isleft) //리스폰
  {
    MyTransform.position = (Vector3)newpos + Vector3.back * 2;          //위치로 이동하고
    MyTransform.localScale = Vector3.one;
    Debug.Log($"isleft : {isleft}  newpos.x : {newpos.x}  targetpos.x : {targetx}");
    StartCoroutine(respawn(targetx, isleft));  //코루틴 시작
  }
  private IEnumerator respawn(float targetx,bool isleft)
  {
    yield return new WaitForSeconds(0.8f);
    IsPressing = true;
    IsDead = false;
    Accel.x = AccelDegree*(isleft?1:-1);
    flipx = !isleft;
    if (isleft)
    {
      yield return new WaitUntil(() => { return MyTransform.position.x >= targetx; });
    }
    else
    {
      yield return new WaitUntil(() => { return MyTransform.position.x <= targetx; });
    }
    Debug.Log("새로운 시작인 레후~");
    IsPlaying = true;
  }
  private IEnumerator respawn_old(float spawningtime) //구 리스폰 연출
  {
    float _time = 0.0f;
    while (_time < spawningtime)  //spawningtime동안 1.0까지 점점 커짐
    {
      MyTransform.localScale = Vector3.one * Mathf.Lerp(0, 1.0f,Mathf.Pow(_time / spawningtime,3.0f));
      _time += Time.deltaTime;
      yield return null;
    }
    MyTransform.localScale = Vector3.one;

    IsDead = false;           //크기 원상복구됐으면 프로퍼티 초기화
    IsWater = false;
    Accel = Vector2.zero;
    Velocity = Vector2.zero;
  }
}
