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

      MyAnimator.SetBool("IsWalking", Mathf.Abs(velocity.x) > 0.1f);
      //�÷��̾� �̵��ӵ��� 0.1 ���ϸ� �ȱ⵵ �ƴϴ�
    }
  }
  [SerializeField] private float AccelDegree = -3.0f; //�̵� �Է½� ���ӵ�
  [SerializeField] private float AccelResist = -3.0f; //���� ���ӵ�
  [SerializeField] private float MaxXSpeed = 5.0f;    //�ִ� X �ӵ�(����)
  [Space(5)]
  private Vector2 Accel = Vector2.zero;               //���� ���ӵ�
  [SerializeField] private float GrvtDegree = -9.8f;  //�߷°��ӵ�
  [SerializeField] private float JumpPower = 8.0f;    //���� �Է½� �ٲ�� �ӵ�
  [SerializeField] private float MaxJumpTime = 0.6f;  //�ִ� ���� �ð�
  [SerializeField] private float MaxYSpeed = 10.0f;   //�ִ� Y �ӵ�(����)
  private BoxCollider2D Col;                          //�� �ݶ��̴�
  private Bounds MyBound;                             //�� �ٿ��
  [Space(5)]
  [SerializeField] private int VertexCount = 5;       //�浹 �˻��� �� ����
  private Vector2[] Vertex_top, Vertex_bottom,Vertex_right,Vertex_left;//�浹 �˻��� �� ��ġ
  private Transform MyTransform;                      //�� Ʈ������
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
  public void Setup()
  {
    MyTransform = transform;
    Col = GetComponent<BoxCollider2D>();
    MyBound = Col.bounds;
    Vertex_top = new Vector2[VertexCount]; Vertex_bottom = new Vector2[VertexCount];
    Vertex_right = new Vector2[VertexCount]; Vertex_left = new Vector2[VertexCount]; //���ؽ� ���� ����
    MyBound.Expand(0.05f);
    float _width = MyBound.size.x;
    float _height = MyBound.size.y;
    float _size_width = _width / VertexCount;
    float _size_height = _height / VertexCount;                                 //�ٿ�� ������, ���� ����

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
    if (IsDead) return;
    if (IsWater)
    {
      WaterAccel += Time.deltaTime * WaterSpeed;
      Accel.y=( Mathf.Cos(Mathf.Deg2Rad * WaterAccel) * FloatingDegree);
    }
    else
    {
      Accel.y = GrvtDegree;
    }

    if (IsPlaying)  //�������϶���
    {
      Accel.x = 0;
      if (Input.GetKey(KeyCode.D)) { Accel.x = AccelDegree; flipx = false; }                      //���� ��ư : ���ӵ��� +
      else if (Input.GetKey(KeyCode.A)) { Accel.x = -AccelDegree; flipx = true; }                 //���� ��ư : ���ӵ��� -
      else Accel.x = (Velocity.x != 0 ? Mathf.Sign(Velocity.x) : 0) * AccelResist;//�ƹ��͵� �� ���� : ���ӵ��� �ӵ� �ݴ��
    }

    if (Input.GetKey(KeyCode.Space) && Jumpable) Jump();//����

    if(MySpr.flipX!=flipx)MySpr.flipX = flipx;


    Velocity += Accel * Time.deltaTime; //�ӵ��� �����߰�


    if (asdf!=null) asdf.text = Velocity.ToString();  //����׿� �ؽ�Ʈ
    RaycastVertical();
    RaycastHorizontal();

    Velocity = new Vector2(Mathf.Clamp(Velocity.x, -MaxXSpeed, MaxXSpeed), Mathf.Clamp(Velocity.y, -MaxYSpeed, MaxYSpeed)); //�ӵ� ����ġ
    Velocity = new Vector2(Mathf.Abs(velocity.x) < 0.05f ? 0 : velocity.x, velocity.y); //�̼� ���� ����

    MyTransform.Translate((Velocity+ Vector2.right * ConveyorSpeed * Conveyor) * Time.deltaTime);
   // Debug.Log("Accel : " + Accel);
  }
  private void Jump()
  {
    Velocity = new Vector2(Velocity.x, JumpPower); 
    JumpTime += Time.deltaTime; 
    MyAnimator.SetTrigger("Jump");
  }
  private void RaycastVertical()  //���Ʒ� �˻�
  {
    Vector2[] _pos = Velocity.y > 0 ? Vertex_top : Vertex_bottom; //���� ���۵Ǵ� ��ġ(�ٿ�� ����)
    Vector2 _dir = Velocity.y > 0?Vector2.up : Vector2.down;      //���� �߻�Ǵ� ��ġ
    Vector3 _newpos = Vector3.zero;                               //���� ���۵Ǵ� ��ġ(�÷��̾� ����)
    int _layermask;          //���̾��ũ(int)
    float _distance = Mathf.Abs( Velocity.y) * Time.deltaTime;                //���� �߻�Ǵ� �Ÿ�
    RaycastHit2D _hit;                                            //���� �߻�ǰ� ���� ���� ����
    Conveyor = 0; //�����̾� �ʱ�ȭ

    for (int i = 0; i < VertexCount; i++)
    {
      _layermask = 1 << LayerMask.NameToLayer("Wall");  //�� ������ �ϴ� ���̾� �˻�
      _newpos = MyTransform.position + (Vector3)_pos[i];
      _hit = Physics2D.Raycast(_newpos, _dir, _distance, _layermask) ;
      if (_hit.transform != null)
      {
        Wooden iswooden;
        if (_hit.transform.TryGetComponent<Wooden>(out iswooden) && !iswooden.IsActive) continue; //���� ��Ȱ��ȭ�� ����

        if (Velocity.y < 0) { Jumpable = true; JumpTime = 0.0f; }

        Velocity =new Vector2(Velocity.x,_hit.distance*_dir.y);
  //      Debug.Log(_hit.transform.tag);

        if (_hit.transform.CompareTag("Breakable"))
          _hit.transform.GetComponent<Breakable>().Pressed(); //��Ƽ� �ν����� �̺�Ʈ ����
        else if (_hit.transform.CompareTag("Conveyor_R")) Conveyor = 1;
        else if (_hit.transform.CompareTag("Conveyor_L")) Conveyor = -1;
        break;
      }
     if(!IsWater) Jumpable = false;
      if (Velocity.y > 0) continue; //���� ���̸� ���⼭ ����, �ϰ� ���̸� Upper ��ϵ� �������� �ν�
      _layermask = 1 << LayerMask.NameToLayer("Upper");  //����� �˻�
      _hit = Physics2D.Raycast(_newpos, _dir, _distance, _layermask);
      if (_hit.transform != null)
      {
        if (Velocity.y < 0) { Jumpable = true; JumpTime = 0.0f; }
        Velocity = new Vector2(Velocity.x, _hit.distance * _dir.y);
        break;
      }
    }
  }
  private void RaycastHorizontal()//�¿� �˻�
  {
 //   if (Velocity.x == 0) return;

    Vector2[] _pos = Velocity.x > 0 ? Vertex_right : Vertex_left; //���� ���۵Ǵ� ��ġ(�ٿ�� ����)
    Vector2 _dir = Velocity.x > 0 ? Vector2.right : Vector2.left;      //���� �߻�Ǵ� ��ġ
    Vector3 _newpos = Vector3.zero;                               //���� ���۵Ǵ� ��ġ(�÷��̾� ����)
    int _layermask = 1 << LayerMask.NameToLayer("Wall");          //���̾��ũ(int)
    float _distance = Mathf.Abs(Velocity.x) * Time.deltaTime;                //���� �߻�Ǵ� �Ÿ�
    RaycastHit2D _hit;                                            //���� �߻�ǰ� ���� ���� ����


    for (int i = 0; i < VertexCount; i++)
    {
      _newpos = MyTransform.position + (Vector3)_pos[i];
  //    Debug.Log(i);
      _layermask = 1<< LayerMask.NameToLayer("Wall");  //�� �˻�
      _hit = Physics2D.Raycast(_newpos, _dir, _distance, _layermask);
      if (_hit.transform != null)
      {
      //  Debug.Log(_hit.transform.name);
        Wooden iswooden;
        if (_hit.transform.TryGetComponent<Wooden>(out iswooden) && !iswooden.IsActive) continue; //���� ��Ȱ��ȭ�� ����

        Velocity = new Vector2(_hit.distance - 0.1f*_dir.x,Velocity.y );
        break;
      }
      Debug.DrawRay(_newpos, _dir, Color.red, _distance);
      //   Debug.Log($"_distance : {_distance}  hit.distance : {_hit.distance}  velocity.x : {Velocity.x}");
    }
  }
  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.gameObject.layer ==  LayerMask.NameToLayer("Water"))
    {
      Velocity = new Vector2(Velocity.x, 0.0f); IsWater = true; WaterAccel = 180.0f; Jumpable = true;
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
  private void Update()
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
  public void Respawn(Vector2 newpos,float targetx,bool isleft) //������
  {
    MyTransform.position = (Vector3)newpos + Vector3.back * 2;          //��ġ�� �̵��ϰ�
    MyTransform.localScale = Vector3.one;
    Debug.Log($"isleft : {isleft}  newpos.x : {newpos.x}  targetpos.x : {targetx}");
    StartCoroutine(respawn(targetx, isleft));  //�ڷ�ƾ ����
  }
  private IEnumerator respawn(float targetx,bool isleft)
  {
    yield return new WaitForSeconds(0.8f);
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
    Debug.Log("���ο� ������ ����~");
    IsPlaying = true;
  }
  private IEnumerator respawn_old(float spawningtime) //�� ������ ����
  {
    float _time = 0.0f;
    while (_time < spawningtime)  //spawningtime���� 1.0���� ���� Ŀ��
    {
      MyTransform.localScale = Vector3.one * Mathf.Lerp(0, 1.0f,Mathf.Pow(_time / spawningtime,3.0f));
      _time += Time.deltaTime;
      yield return null;
    }
    MyTransform.localScale = Vector3.one;

    IsDead = false;           //ũ�� ���󺹱������� ������Ƽ �ʱ�ȭ
    IsWater = false;
    Accel = Vector2.zero;
    Velocity = Vector2.zero;
  }
}
