using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : MonoBehaviour
{
  private Vector2 velocity = Vector2.zero;            //���� �ӵ�
  private Vector2 Velocity
  {
    get { return velocity; }
    set {
      if (velocity.y > 0 && value.y <= 0) MyAnimator.SetTrigger("JumpisDone");
      if (velocity.y < 0 && value.y >= 0) MyAnimator.SetTrigger("FallisDone");

      velocity = value;

      //�÷��̾� �̵��ӵ��� 0.1 ���ϸ� �ȱ⵵ �ƴϴ�
    }
  }
  private Vector2 NextVelocity = Vector2.zero;
  [SerializeField] private float AccelDegree = -3.0f; //�̵� �Է½� ���ӵ�
  [SerializeField] private float AccelResist = -3.0f; //���� ���ӵ�
  [SerializeField] private float MaxXSpeed = 5.0f;    //�ִ� X �ӵ�(����)
  [Space(5)]
  private Vector2 Accel = Vector2.zero;               //���� ���ӵ�
  [SerializeField] private float GrvtDegree = -9.8f;  //�߷°��ӵ�
  [SerializeField] private float JumpPower = 8.0f;    //���� �Է½� �ٲ�� �ӵ�
  [SerializeField] private float MaxJumpTime = 0.6f;  //�ִ� ���� �ð�
  [SerializeField] private float MaxYSpeed = 10.0f;   //�ִ� Y �ӵ�(����)
  [Space(5)]
  [SerializeField] private int VertexCount = 5;       //�浹 �˻��� �� ����
  private Vector2[] Vertex_top, Vertex_bottom,Vertex_right,Vertex_left;//�浹 �˻��� �� ��ġ
  private Transform MyTransform;                      //�� Ʈ������
  private Transform MySprTransform = null;
  private bool Jumpable = true;                       //���� ���� ��������
  private float jumptime = 0.0f;
  private float JumpTime { get { return jumptime; } set { jumptime = value; if (jumptime >= MaxJumpTime) Jumpable = false; } }

  [SerializeField] private TMPro.TextMeshProUGUI asdf = null;
  private int Conveyor = 0; //-1 / 0 / +1   ���� X ����
  [Space(5)]
  [SerializeField] private float ConveyorSpeed = 3.0f;  //�����̾Ʈ �� �ӵ�
  private float WaterAccel = 90.0f; //�� ���ٴϴ� ���ӿ� �� ��ġ
  [Space(5)]
  [SerializeField] private float WaterSpeed = 1.0f; //�� ���ٴϴ� �ֱ�
  [SerializeField] private float FloatingDegree = 1.0f; //�� ���ٴϴ� �ݰ�
  private bool IsWater = false;      //�� �ӿ� �ִ���
  private bool IsDead = false;  //��� �� ��� Ȱ�� ������Ű��
  [Space(5)]
  [SerializeField] private int Dead_ShakeCount = 55;        //��� �� ��鸮�� Ƚ��
  [SerializeField] private float Dead_ShakeTime = 4.0f;     //��� �� ��鸮�� �ð�
  [SerializeField] private float Dead_ShakeDegree = 0.05f;  //��� �� ��鸮�� ����
  [SerializeField] private SpriteRenderer MySpr = null;     //�� ��������Ʈ������
  [SerializeField] private Animator MyAnimator = null;      //�� �ִϸ�����
  [Space(5)]
  [SerializeField] private ParticleSystem WaterDownParticle = null; //�� ÷�� ��ƼŬ
  private ParticleSystem.ShapeModule WaterDownShape;
  [SerializeField] private ParticleSystem WaterUpParticle = null;   //������ ������ ��ƼŬ
  private ParticleSystem.ShapeModule WaterUpShape;
  [SerializeField] private ParticleSystem DeadParticle_body = null; //�� �Ξ�
  private ParticleSystem.ShapeModule DeadShape_body;
  [SerializeField] private ParticleSystem DeadParticle_soul = null; //�÷��̾� ��Ÿ�� ��ƼŬ
  private ParticleSystem.ShapeModule DeadShape_soul;
  private bool flipx = true;                  //�̹��� �¿������ ����
  private bool IsPlaying = true;              //���� ���� ������ �����ΰ�?
  private bool IsPressing = false;            //���� A�� D�� ������ �ִ°�?
  private float Expanddegree = 0.01f;         //�ݶ��̴� ���� Ȯ�� ����
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
    Vertex_right = new Vector2[VertexCount]; Vertex_left = new Vector2[VertexCount]; //���ؽ� ���� ����
    MyBound.Expand(-Expanddegree);
    float _width = MyBound.size.x;
    float _height = MyBound.size.y;
    float _size_width = _width / (VertexCount-1);
    float _size_height = _height / (VertexCount-1);                                 //�ٿ�� ������, ���� ����

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

    if (IsPlaying)  //�������϶���
    {
      if (IsWater)
      {
        WaterAccel += Time.deltaTime * WaterSpeed;
        Accel.y = (Mathf.Cos(Mathf.Deg2Rad * WaterAccel) * FloatingDegree);
      }

      Accel.x = 0;
      if (Input.GetKey(KeyCode.D)) { Accel.x = AccelDegree; flipx = false; IsPressing = true; }                      //���� ��ư : ���ӵ��� +
      else if (Input.GetKey(KeyCode.A)) { Accel.x = -AccelDegree; flipx = true; IsPressing = true; }                 //���� ��ư : ���ӵ��� -
      else {
        IsPressing = false;
        Accel.x = (NextVelocity.x != 0 ? Mathf.Sign(NextVelocity.x) : 0) * AccelResist; }//�ƹ��͵� �� ���� : ���ӵ��� �ӵ� �ݴ��

    }

    if (Input.GetKey(KeyCode.Space) && Jumpable) Jump();//����

    if(MySpr.flipX!=flipx)MySpr.flipX = flipx;


    NextVelocity += Accel * Time.deltaTime; //�ӵ��� �����߰�
    if (IsPressing == false)  //�̵��� ������ �ʾҴµ�
    {
      if(Mathf.Sign(Velocity.x)!=Mathf.Sign(NextVelocity.x)||(Velocity.x==0&NextVelocity.x!=0))NextVelocity.x = 0;
      //���ӵ� ������ �ӵ��� ��ȣ�� �ٲ������ų� ������ �ִ��� �̵��Ѵٸ� 0���� ����
    }

    if (asdf!=null) asdf.text = Velocity.ToString();  //����׿� �ؽ�Ʈ
    RaycastVertical();
    RaycastHorizontal();


    NextVelocity = new Vector2(Mathf.Clamp(NextVelocity.x, -MaxXSpeed, MaxXSpeed), Mathf.Clamp(NextVelocity.y, -MaxYSpeed, MaxYSpeed)); //�ӵ� ����ġ

   // Velocity = new Vector2(Mathf.Abs(NextVelocity.x) < 0.05f ? 0 : NextVelocity.x, NextVelocity.y); //�̼� ���� ����
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
  private void RaycastVertical()  //���Ʒ� �˻�
  {
    Vector2[] _pos = NextVelocity.y <= 0 ? Vertex_bottom : Vertex_top; //���� ���۵Ǵ� ��ġ(�ٿ�� ����)
    Vector2 _dir = NextVelocity.y <= 0?Vector2.down : Vector2.up;      //���� �߻�Ǵ� ��ġ
    Vector3 _newpos = Vector3.zero;                               //���� ���۵Ǵ� ��ġ(�÷��̾� ����)
    int _layermask;          //���̾��ũ(int)
    float _distance = Expanddegree+ Mathf.Abs(NextVelocity.y) * Time.deltaTime;                //���� �߻�Ǵ� �Ÿ�
    if (_distance == 0.0f) _distance = Expanddegree;
    RaycastHit2D _hit;                                            //���� �߻�ǰ� ���� ���� ����
    Conveyor = 0; //�����̾� �ʱ�ȭ

    for (int i = 0; i < VertexCount; i++)
    {
      _layermask = 1 << LayerMask.NameToLayer("Wall");  //�� ������ �ϴ� ���̾� �˻�
      _newpos = MyTransform.position + (Vector3)_pos[i];
      _hit = Physics2D.Raycast(_newpos, _dir, _distance, _layermask) ;
      Debug.DrawRay(_newpos, _dir * _distance, Color.red);
      if (_hit.transform != null)
      {
        Wooden iswooden;
        if (_hit.transform.TryGetComponent<Wooden>(out iswooden) && !iswooden.IsActive) continue; //���� ��Ȱ��ȭ�� ����

        if (NextVelocity.y < 0) { Jumpable = true; JumpTime = 0.0f; }

        NextVelocity = new Vector2(NextVelocity.x,(_hit.distance- Expanddegree )* _dir.y);
        //      Debug.Log(_hit.transform.tag);
        Debug.DrawRay(_newpos, _dir * _hit.distance, Color.green);

        if (_hit.transform.CompareTag("Breakable"))
          _hit.transform.GetComponent<Breakable>().Pressed(); //��Ƽ� �ν����� �̺�Ʈ ����
        else if (_hit.transform.CompareTag("Conveyor_R")) Conveyor = 1;
        else if (_hit.transform.CompareTag("Conveyor_L")) Conveyor = -1;
        break;
      }
     if(!IsWater) Jumpable = false;
      if (NextVelocity.y > 0) continue; //���� ���̸� ���⼭ ����, �ϰ� ���̸� Upper ��ϵ� �������� �ν�
      _layermask = 1 << LayerMask.NameToLayer("Upper");  //����� �˻�
      _hit = Physics2D.Raycast(_newpos, _dir, _distance, _layermask);
      if (_hit.transform != null)
      {
        if (NextVelocity.y < 0) { Jumpable = true; JumpTime = 0.0f; }
        NextVelocity = new Vector2(NextVelocity.x, _hit.distance * _dir.y);
        break;
      }
    }
  }
  private void RaycastHorizontal()//�¿� �˻�
  {
    //   if (NextVelocity.x == 0) return;

    Vector2[] _pos = NextVelocity.x >= 0 ? Vertex_right : Vertex_left; //���� ���۵Ǵ� ��ġ(�ٿ�� ����)
    Vector2 _dir = NextVelocity.x >= 0 ? Vector2.right : Vector2.left;      //���� �߻�Ǵ� ����
    Vector3 _newpos = Vector3.zero;                               //���� ���۵Ǵ� ��ġ(�÷��̾� ����)
    int _layermask = 1 << LayerMask.NameToLayer("Wall");          //���̾��ũ(int)
    float _distance = Expanddegree+ Mathf.Abs(NextVelocity.x) * Time.deltaTime;                //���� �߻�Ǵ� �Ÿ�
    if (_distance == 0.0f) _distance = Expanddegree;
    RaycastHit2D _hit;                                            //���� �߻�ǰ� ���� ���� ����


    for (int i = 0; i < VertexCount; i++)
    {
      _newpos = MyTransform.position + (Vector3)_pos[i];
      _layermask = 1<< LayerMask.NameToLayer("Wall");  //�� �˻�
      _hit = Physics2D.Raycast(_newpos, _dir, _distance, _layermask);
      Debug.DrawRay(_newpos, _dir * _distance, Color.red);
      if (_hit.transform != null)
      {
   //  Debug.Log($"{_hit.transform.name}  {_hit.transform.position}");
        Wooden iswooden;
        if (_hit.transform.TryGetComponent<Wooden>(out iswooden) && !iswooden.IsActive) continue; //���� ��Ȱ��ȭ�� ����

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
    if (collision.gameObject.layer ==  LayerMask.NameToLayer("Water") && IsPlaying)  //���� �������
    {
      NextVelocity = new Vector2(NextVelocity.x, 0.0f); IsWater = true; WaterAccel = 180.0f; Jumpable = true;
      //�ٷ� ���� �� �ִ� �ӵ��� ����
      WaterDownShape.position = MyTransform.position + Vector3.down * 0.5f;
      WaterDownParticle.Play(); //���� ��ƼŬ ��ġ �����ϰ� ����
    }
    else if (collision.CompareTag("Spike") && IsPlaying) //���ÿ� ������� ��ü ����
    {
      Vector2 _spikepos = collision.ClosestPoint(MyTransform.position)-(Vector2)MyTransform.position;

      Dead_body(_spikepos, false);
    }
    else if (collision.CompareTag("Rock"))  //���� ��Ұ�
    {
      if (!collision.GetComponent<Rock>().IsLanding&&IsPlaying)
      {
        Dead_body(Vector2.up*-GetComponent<BoxCollider2D>().bounds.size.y/2,true);
     //   GameManager.Instance.RockPressed();
      }//�� ���� �������� ���¶�� ��ü ����      �����غ��ϱ� ������ ����
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
    if (Input.GetKeyDown(KeyCode.R) && IsPlaying) Restart(); //R�� ������ ������ ������������ �����
  }
  private void FixedUpdate()
  {
    UpdateMove();
  }
  public void Dead_body(Vector2 bloodpos,bool isrock)         //���ÿ� ����� ��ü�� ���
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
    DeadParticle_body.Play();           //����
    if (isrock) MySprTransform.localScale = new Vector3(1.0f, 0.2f, 1.0f);

    if (bloodpos.y<=0) yield return new WaitForSeconds(0.15f); //���۸� ������Ű�� ���� �ٴڿ� ���� ������ ���
    if (isrock) yield return new WaitForSeconds(0.75f); //���� �򸰰Ÿ� �ٴڿ� ���������� ���

    IsDead = true;
    IsWater = false;

    yield return new WaitForSeconds(1.5f);  //�� Ƣ��� ���� ������

    if (!isrock)  //���� �񸰰Ÿ� ��� ���� �ʿ��ϴϱ� 
    {
      Vector3 _originpos = MyTransform.position;
      for (int i = 0; i < Dead_ShakeCount; i++) //����
      {
        MyTransform.position = _originpos + new Vector3(Random.Range(-Dead_ShakeDegree, Dead_ShakeDegree), Random.Range(-Dead_ShakeDegree, Dead_ShakeDegree)) + Vector3.back * 2;
        yield return new WaitForSeconds(Dead_ShakeTime / Dead_ShakeCount);
      }
      MyTransform.position = _originpos;
      MyTransform.localScale = Vector3.zero;    //���� ������
      DeadShape_soul.position = MyTransform.position;
      DeadParticle_soul.Play();                       //���� ��ƼŬ ����
      Instantiate(SkullPrefab, SkullHolder).GetComponent<Skull>().Setup(MyTransform.position , 2); //�� Ƣ�� ��ġ�� ����
    }
    else { MyTransform.localScale = Vector3.zero; 
      Instantiate(SkullPrefab, SkullHolder).GetComponent<Skull>().Setup(MyTransform.position , 2); }
    //������ �򸰰Ÿ� ������ ���� �ʰ� �Ⱥ��̰�

      GameManager.Instance.Dead_body(); //���� ���� �� ������Ʈ �ʱ�ȭ
    GameManager.Instance.PlayRPParticle();  //ȭ�� ȸ���ϴ� ��ƼŬ

    GameManager.Instance.Respawn();                   //������
    yield return null;
  }

  public void Dead_soul()         //���� ���� ������ ���
  {
    StartCoroutine(dead_soul());
  }
  private IEnumerator dead_soul ()
  {
    GameManager.Instance.Dead_soul_0(); //ȶ�� ��� �����ִ� �Լ�
    AudioManager.Instance.PlayClip(3);
    IsDead = true;
    IsPlaying = false;
    IsWater = false;

    yield return new WaitForSeconds(1.0f);  //������� �� ������� �ð�(����)

    Vector3 _originpos = MyTransform.position;  //Dead_ShakeTime���� ����
    for(int i = 0; i < Dead_ShakeCount; i++)
    {
      MyTransform.position = _originpos + new Vector3 (Random.Range(-Dead_ShakeDegree, Dead_ShakeDegree), Random.Range(-Dead_ShakeDegree, Dead_ShakeDegree))+Vector3.back*2;
      yield return new WaitForSeconds(Dead_ShakeTime / Dead_ShakeCount);
    }
    MyTransform.position = _originpos;        //������ ����Ǹ�
    MyTransform.localScale = Vector3.zero;    //�÷��̾��� ũ��� 0�� �Ǿ� �þ߿��� �����

    DeadShape_soul.position = MyTransform.position;  //��� ��ƼŬ ��ġ �����ϰ�
    DeadParticle_soul.Play();                        //��� ��ƼŬ ����

    yield return new WaitForSeconds(1.0f);      //��� ��ƼŬ ����������� ��� ���

    GameManager.Instance.Dead_soul_1();         //������ �����ϰ� ��� �ؽ�Ʈ ��� ����
    yield return null;
  }
  public void Respawn(Vector2 newpos,float targetx) //������
  {
    MyTransform.position = (Vector3)newpos + Vector3.back * 2;          //��ġ�� �̵��ϰ�
    MyTransform.localScale = Vector3.one;
    MySprTransform.localScale = Vector3.one;
  //  Debug.Log($"isleft : {isleft}  newpos.x : {newpos.x}  targetpos.x : {targetx}");
    StartCoroutine(respawn(targetx));  //�ڷ�ƾ ����
  }
  private IEnumerator respawn(float targetx)
  {
    yield return new WaitForSeconds(0.8f);
    IsPressing = true;
    IsDead = false;
    Accel.x = AccelDegree;
    flipx = false;
      yield return new WaitUntil(() => { return MyTransform.position.x >= targetx; });
    Debug.Log("���ο� ������ ����~");
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
