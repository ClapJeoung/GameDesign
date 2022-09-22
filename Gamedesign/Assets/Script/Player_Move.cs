using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : MonoBehaviour
{
  private Vector2 Velocity = Vector2.zero;            //���� �ӵ�
  [SerializeField] private float AccelDegree = -3.0f; //�̵� �Է½� ���ӵ�
  [SerializeField] private float GrvtDegree = -9.8f;  //�߷°��ӵ�
  [SerializeField] private float AccelResist = -3.0f; //���� ���ӵ�
  private Vector2 Accel = Vector2.zero;               //���� ���ӵ�
  [SerializeField] private float JumpPower = 8.0f;    //���� �Է½� �ٲ�� �ӵ�
  [SerializeField] private float MaxXSpeed = 5.0f;    //�ִ� X �ӵ�(����)
  [SerializeField] private float MaxYSpeed = 10.0f;   //�ִ� Y �ӵ�(����)
  private BoxCollider2D Col;                          //�� �ݶ��̴�
  private Bounds MyBound;                             //�� �ٿ��
  [SerializeField] private int VertexCount = 5;       //�浹 �˻��� �� ����
  private Vector2[] Vertex_top, Vertex_bottom,Vertex_right,Vertex_left;//�浹 �˻��� �� ��ġ
  private Transform MyTransform;                      //�� Ʈ������
  private bool Jumpable = true;                       //���� ���� ��������


  private void Awake()
  {
    Setup();
  }
  public void Setup()
  {
    MyTransform = transform;
    Col = GetComponent<BoxCollider2D>();
    MyBound = Col.bounds;
    Vertex_top = new Vector2[VertexCount]; Vertex_bottom = new Vector2[VertexCount];
    Vertex_right = new Vector2[VertexCount]; Vertex_left = new Vector2[VertexCount]; //���ؽ� ���� ����

    float _width = MyBound.size.x;
    float _height = MyBound.size.y;
    float _size_width = _width / VertexCount;
    float _size_height = _height / VertexCount;                                 //�ٿ�� ������, ���� ����

    for (int i = 0; i < VertexCount; i++)
    {
      Vertex_top[i] = new Vector2(-_width / 2 + _size_width * i, _height / 2);
      Vertex_bottom[i] = new Vector2(-_width / 2 + _size_width * i, -_height / 2);
      Vertex_right[i] = new Vector2(_width / 2, -_height / 2 + _size_height * VertexCount);
      Vertex_left[i] = new Vector2(-_width / 2, -_height / 2 + _size_height * VertexCount);
    }
  }
  private void UpdateMove()
  {
    Accel = new Vector2(0, GrvtDegree);
    if (Input.GetKey(KeyCode.D)) Accel.x = AccelDegree;                      //���� ��ư : ���ӵ��� +
    else if (Input.GetKey(KeyCode.A)) Accel.x = -AccelDegree;                 //���� ��ư : ���ӵ��� -
    else Accel.x = (Velocity.x != 0 ? Mathf.Sign(Velocity.x) : 0) * AccelResist;//�ƹ��͵� �� ���� : ���ӵ��� �ӵ� �ݴ��


    if (Input.GetKeyDown(KeyCode.Space) && Jumpable) { Velocity.y = JumpPower; Jumpable = false; } //�����̽��ٴ� ����



    Velocity += Accel * Time.deltaTime;

    Velocity = new Vector2(Mathf.Clamp(Velocity.x,-MaxXSpeed,MaxXSpeed), Mathf.Clamp(Velocity.y,-MaxYSpeed,MaxYSpeed));

    RaycastVertical();
    RaycastHorizontal();

    MyTransform.Translate(Velocity * Time.deltaTime);
  }
  private void RaycastVertical()  //���Ʒ� �˻�
  {
    Vector2[] _pos = Velocity.y > 0 ? Vertex_top : Vertex_bottom; //���� ���۵Ǵ� ��ġ(�ٿ�� ����)
    Vector2 _dir = Velocity.y > 0?Vector2.up : Vector2.down;      //���� �߻�Ǵ� ��ġ
    Vector3 _newpos = Vector3.zero;                               //���� ���۵Ǵ� ��ġ(�÷��̾� ����)
    int _layermask;          //���̾��ũ(int)
    float _distance = Velocity.y * Time.deltaTime;                //���� �߻�Ǵ� �Ÿ�
    RaycastHit2D _hit;                                            //���� �߻�ǰ� ���� ���� ����

    for (int i = 0; i < VertexCount; i++)
    {
      _layermask = 1 << LayerMask.NameToLayer("Wall");  //�� �˻�
      _newpos = MyTransform.position + (Vector3)_pos[i];
      _hit = Physics2D.Raycast(_newpos, _dir, _distance, _layermask) ;
      if (_hit.transform != null)
      {
        if (Velocity.y < 0) Jumpable = true;
        Velocity.y = _hit.distance;
        break;
      }
      if (Velocity.y > 0) continue;
      _layermask = 1 << LayerMask.NameToLayer("Upper");  //����� �˻�
      _hit = Physics2D.Raycast(_newpos, _dir, _distance, _layermask);
      if (_hit.transform != null)
      {
        if (Velocity.y < 0) Jumpable = true;
        Velocity.y = _hit.distance;
        break;
      }
    }
  }
  private void RaycastHorizontal()//�¿� �˻�
  {
    Vector2[] _pos = Velocity.x > 0 ? Vertex_right : Vertex_left; //���� ���۵Ǵ� ��ġ(�ٿ�� ����)
    Vector2 _dir = Velocity.x > 0 ? Vector2.right : Vector2.left;      //���� �߻�Ǵ� ��ġ
    Vector3 _newpos = Vector3.zero;                               //���� ���۵Ǵ� ��ġ(�÷��̾� ����)
    int _layermask = 1 << LayerMask.NameToLayer("Wall");          //���̾��ũ(int)
    float _distance = Velocity.x * Time.deltaTime;                //���� �߻�Ǵ� �Ÿ�
    RaycastHit2D _hit;                                            //���� �߻�ǰ� ���� ���� ����

    for (int i = 0; i < VertexCount; i++)
    {
      _newpos = MyTransform.position + (Vector3)_pos[i];
      _hit = Physics2D.Raycast(_newpos, _dir, _distance, _layermask);
      if (_hit.transform != null)
      {
        Velocity.x = _hit.distance;
        break;
      }
    }
  }
  private void Update()
  {
    UpdateMove();
  }
}
