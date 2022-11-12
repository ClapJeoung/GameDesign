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
  private Transform MySprTransform = null;
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
  [SerializeField] private int Dead_ShakeCount = 55;        //사망 시 흔들리는 횟수
  [SerializeField] private float Dead_ShakeTime = 4.0f;     //사망 시 흔들리는 시간
  [SerializeField] private float Dead_ShakeDegree = 0.05f;  //사망 시 흔들리는 정도
  [SerializeField] private SpriteRenderer MySpr = null;     //내 스프라이트랜더러
  [SerializeField] private Animator MyAnimator = null;      //내 애니메이터
  [Space(5)]
  [SerializeField] private ParticleSystem WaterDownParticle = null; //물 첨벙 파티클
  private ParticleSystem.ShapeModule WaterDownShape;
  [SerializeField] private ParticleSystem WaterUpParticle = null;   //물에서 나오는 파티클
  private ParticleSystem.ShapeModule WaterUpShape;
  [SerializeField] private ParticleSystem DeadParticle_body = null; //피 부왘
  private ParticleSystem.ShapeModule DeadShape_body;
  [SerializeField] private ParticleSystem DeadParticle_soul = null; //플레이어 불타는 파티클
  private ParticleSystem.ShapeModule DeadShape_soul;
  private bool flipx = true;                  //이미지 좌우반전용 변수
  private bool IsPlaying = true;              //현재 조작 가능한 상태인가?
  private bool IsPressing = false;            //현재 A나 D를 누르고 있는가?
  private float Expanddegree = 0.01f;         //콜라이더 감지 확장 범위
  [SerializeField] private Torch MyTorch = null;
  [SerializeField] private GameObject SkullPrefab = null;
  [SerializeField] private Transform SkullHolder = null;
  public void Setup()
  {
    MyTransform = transform;
    MySprTransform = MyTransform.GetChild(0).transform;
    BoxCollider2D Col = GetComponent<BoxCollider2D>();

    Bounds MyBound = Col.bounds;
    Vertex_top = new Vector2[VertexCount]; Vertex_bottom = new Vector2[VertexCount];
    Vertex_right = new Vector2[VertexCount]; Vertex_left = new Vector2[VertexCount]; //버텍스 개수 설정
    MyBound.Expand(-Expanddegree);
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
    DeadShape_body = DeadParticle_body.shape;
    DeadShape_soul=DeadParticle_soul.shape;
    MyBound.Expand(+Expanddegree);
    IsDead = true;
    IsPlaying = false;
  }
  public void Start()
  {
    Setup();
  }
  public void Restart() => StartCoroutine(restart());
  private IEnumerator restart()
  {
    yield return StartCoroutine(dead_body(Vector2.zero,false));
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
    float _distance = Expanddegree+ Mathf.Abs(NextVelocity.y) * Time.deltaTime;                //선이 발사되는 거리
    if (_distance == 0.0f) _distance = Expanddegree;
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

        NextVelocity = new Vector2(NextVelocity.x,(_hit.distance- Expanddegree )* _dir.y);
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
    float _distance = Expanddegree+ Mathf.Abs(NextVelocity.x) * Time.deltaTime;                //선이 발사되는 거리
    if (_distance == 0.0f) _distance = Expanddegree;
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
        NextVelocity = new Vector2( (_hit.distance- Expanddegree) * _dir.x, NextVelocity.y);
        Debug.DrawRay(_newpos, _dir * _hit.distance, Color.green);
        break;
      }
      //   Debug.Log($"_distance : {_distance}  hit.distance : {_hit.distance}  NextVelocity.x : {NextVelocity.x}");
    }
  }
  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (IsDead) return;
    if (collision.gameObject.layer ==  LayerMask.NameToLayer("Water") && IsPlaying)  //물에 닿았으면
    {
      NextVelocity = new Vector2(NextVelocity.x, 0.0f); IsWater = true; WaterAccel = 180.0f; Jumpable = true;
      //바로 물에 들어가 있는 속도로 변경
      WaterDownShape.position = MyTransform.position + Vector3.down * 0.5f;
      WaterDownParticle.Play(); //퐁당 파티클 위치 설정하고 실행
    }
    else if (collision.CompareTag("Spike") && IsPlaying) //가시에 닿았으면 육체 죽음
    {
      Vector2 _spikepos = collision.ClosestPoint(MyTransform.position)-(Vector2)MyTransform.position;

      Dead_body(_spikepos, false);
    }
    else if (collision.CompareTag("Rock"))  //돌에 닿았고
    {
      if (!collision.GetComponent<Rock>().IsLanding&&IsPlaying)
      {
        Dead_body(Vector2.up*-GetComponent<BoxCollider2D>().bounds.size.y/2,true);
     //   GameManager.Instance.RockPressed();
      }//그 돌이 떨어지는 상태라면 육체 죽음      적용해보니까 구려서 뺐음
    }
  }
  public void RollingStones()
  {
    Dead_body(Vector2.up * -GetComponent<BoxCollider2D>().bounds.size.y / 2, true);
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
  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.R) && IsPlaying) Restart(); //R을 누르면 마지막 저장지점에서 재시작
  }
  private void FixedUpdate()
  {
    UpdateMove();
  }
  public void Dead_body(Vector2 bloodpos,bool isrock)         //가시에 찔려서 육체적 사망
  {
    StartCoroutine(dead_body(bloodpos, isrock));
  }
  private IEnumerator dead_body(Vector2 bloodpos,bool isrock)
  {
    AudioManager.Instance.PlayClip(2);
    IsPlaying = false;
    Velocity = Vector2.zero;
    Accel.x = 0.0f;
    MyTorch.Dead();

    Vector2 _bloodrot = Vector2.up * 90.0f + Vector2.right * (-Mathf.Atan2(bloodpos.y, bloodpos.x) * Mathf.Rad2Deg);

    DeadShape_body.position = MyTransform.position + (Vector3)bloodpos + Vector3.back;
    DeadShape_body.rotation = _bloodrot;
    DeadParticle_body.Play();           //지벳
    if (isrock) MySprTransform.localScale = new Vector3(1.0f, 0.2f, 1.0f);

    if (bloodpos.y<=0) yield return new WaitForSeconds(0.15f); //조작만 중지시키고 대충 바닥에 닿을 때까지 대기
    if (isrock) yield return new WaitForSeconds(0.75f); //돌에 깔린거면 바닥에 닿을때까지 대기

    IsDead = true;
    IsWater = false;

    yield return new WaitForSeconds(1.5f);  //피 튀기는 연출 끝내고

    if (!isrock)  //가시 찔린거면 사망 연출 필요하니까 
    {
      Vector3 _originpos = MyTransform.position;
      for (int i = 0; i < Dead_ShakeCount; i++) //진동
      {
        MyTransform.position = _originpos + new Vector3(Random.Range(-Dead_ShakeDegree, Dead_ShakeDegree), Random.Range(-Dead_ShakeDegree, Dead_ShakeDegree)) + Vector3.back * 2;
        yield return new WaitForSeconds(Dead_ShakeTime / Dead_ShakeCount);
      }
      MyTransform.position = _originpos;
      MyTransform.localScale = Vector3.zero;    //진동 끝나면
      DeadShape_soul.position = MyTransform.position;
      DeadParticle_soul.Play();                       //증발 파티클 실행
      Instantiate(SkullPrefab, SkullHolder).GetComponent<Skull>().Setup(MyTransform.position , 2); //피 튀긴 위치에 샌즈
    }
    else { MyTransform.localScale = Vector3.zero; 
      Instantiate(SkullPrefab, SkullHolder).GetComponent<Skull>().Setup(MyTransform.position , 2); }
    //바위에 깔린거면 흔적도 남지 않고 안보이게

      GameManager.Instance.Dead_body(); //현재 차원 및 오브젝트 초기화
    GameManager.Instance.PlayRPParticle();  //화면 회전하는 파티클

    GameManager.Instance.Respawn();                   //리스폰
    yield return null;
  }

  public void Dead_soul()         //불이 꺼져 영구적 사망
  {
    StartCoroutine(dead_soul());
  }
  private IEnumerator dead_soul ()
  {
    GameManager.Instance.Dead_soul_0(); //횃불 대신 멈춰주는 함수
    AudioManager.Instance.PlayClip(3);
    IsDead = true;
    IsPlaying = false;
    IsWater = false;

    yield return new WaitForSeconds(1.0f);  //연기까지 다 사라지는 시간(대충)

    Vector3 _originpos = MyTransform.position;  //Dead_ShakeTime동안 진동
    for(int i = 0; i < Dead_ShakeCount; i++)
    {
      MyTransform.position = _originpos + new Vector3 (Random.Range(-Dead_ShakeDegree, Dead_ShakeDegree), Random.Range(-Dead_ShakeDegree, Dead_ShakeDegree))+Vector3.back*2;
      yield return new WaitForSeconds(Dead_ShakeTime / Dead_ShakeCount);
    }
    MyTransform.position = _originpos;        //진동이 종료되면
    MyTransform.localScale = Vector3.zero;    //플레이어의 크기는 0이 되어 시야에서 사라짐

    DeadShape_soul.position = MyTransform.position;  //사망 파티클 위치 조정하고
    DeadParticle_soul.Play();                        //사망 파티클 실행

    yield return new WaitForSeconds(1.0f);      //사망 파티클 사라질때까지 잠시 대기

    GameManager.Instance.Dead_soul_1();         //주위가 정지하고 사망 텍스트 출력 시작
    yield return null;
  }
  public void Respawn(Vector2 newpos,float targetx) //리스폰
  {
    MyTransform.position = (Vector3)newpos + Vector3.back * 2;          //위치로 이동하고
    MyTransform.localScale = Vector3.one;
    MySprTransform.localScale = Vector3.one;
  //  Debug.Log($"isleft : {isleft}  newpos.x : {newpos.x}  targetpos.x : {targetx}");
    StartCoroutine(respawn(targetx));  //코루틴 시작
  }
  private IEnumerator respawn(float targetx)
  {
    yield return new WaitForSeconds(0.8f);
    IsPressing = true;
    IsDead = false;
    Accel.x = AccelDegree;
    flipx = false;
      yield return new WaitUntil(() => { return MyTransform.position.x >= targetx; });
    Debug.Log("새로운 시작인 레후~");
    IsWater = false;
    IsPlaying = true;
  }
  public void EndingEngage()
  {
    IsDead = true;
    IsPlaying = false;
    MySpr.flipX = false;
  }
  public void Ending(Vector2 newpos, float movetime)
  {
    StartCoroutine(ending(newpos, movetime));
  }
  private IEnumerator ending(Vector2 newpos,float movetime)
  {
    float _time = 0.0f;
    Vector2 _oldpos = MyTransform.position;
    while(_time< movetime)
    {
      MyTransform.position = Vector3.Lerp(_oldpos, newpos, _time / movetime)+Vector3.back*3.0f;
      _time += Time.deltaTime;
      yield return null;
    }
  }
}
