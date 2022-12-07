using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch_pivot : MonoBehaviour
{
  [SerializeField] private float Radgravity = 60.0f;    //�������� ���� �ӵ�
  public float CurrentRadius = 0.0f;                   //���� ����
  private float RadiusVelocity = 0.0f;                  //���� ������ �ӵ�
  [SerializeField] private float InputPower = 70.0f;    //�Է� ��
  [SerializeField] private float Length;                //�÷��̾�κ����� �Ÿ�
  private Transform MyTrans;                            //�� Ʈ������
  [SerializeField] private float MaxSpeed = 60.0f;      //�ְ�ӵ�
  [SerializeField] private float Resist = 30.0f;        //����
  private float Accel = 0.0f;                           //���� ����
  private bool IsPressing_R = false;                    //���� ��ư�� ������ ���ΰ���
  private bool IsPressing_L = false;                    //���� ��ư�� ������ ���ΰ���
  [SerializeField] private float ParticleLength;        //�÷��̾�κ��� ��ƼŬ�� ������ �Ÿ�
  [SerializeField] private float FireLength;            //�÷��̾�κ��� �� �̹����� �Ÿ�
  [SerializeField] private Transform ColliderTransform; //�� �ݶ��̴��� Ʈ������
  [SerializeField] private Transform FireTransform;     //�� �̹����� Ʈ������
  [HideInInspector] public bool IsDead = false;                          //�׾����� �ƹ��͵� ���� �ʴ´�
  private Vector2 DeadPos = Vector2.zero;               //�÷��̾ ���� �� ��ġ
  [SerializeField] private Transform PlayerTransform = null;//�÷��̾� Ʈ������(�� ��ġ ����)
  private Vector3 PlayerLastPos = Vector3.zero;
  [SerializeField] private float TorchMoveSpeed = 1.0f;
  [Space(5)]
  [SerializeField] private ParticleSystem Particle_0 = null;             //��ƼŬ��
  private ParticleSystem.ShapeModule Particle_0_shape;
  [SerializeField] private ParticleSystem Particle_1 = null;
  private ParticleSystem.ShapeModule Particle_1_shape;
  [SerializeField] private ParticleSystem Particle_2 = null;
  private ParticleSystem.ShapeModule Particle_2_shape;
  [SerializeField] private int RespawnSpinCount = 4;    //�������� ȸ���Ǵ� Ƚ��
  private bool IsTutorial = false;
  private void Start()
  {
    Setup();
  }
  public void Setup()
  {
    PlayerLastPos = PlayerTransform.position;
    MyTrans = transform;
    Particle_0_shape = Particle_0.shape;
    Particle_1_shape = Particle_1.shape;
    Particle_2_shape = Particle_2.shape;
    DeadPos = MyTrans.position;
    IsDead = true;
  }
  public void Dead() { IsDead = true; DeadPos = PlayerTransform.position; }
  private void Update()
  {
 //   Debug.Log(CurrentRadius);
    if (IsDead) return;
    int _dir = CurrentRadius>0&& CurrentRadius < 180.0f ? 1 : -1;
    Accel= Radgravity *_dir;
    int _radius = (int)CurrentRadius / 45;
    if (Input.GetKey(KeyCode.RightArrow) && !IsPressing_L) { AddTorchForce(_radius, 1); IsPressing_R = true; }
    if (Input.GetKeyUp(KeyCode.RightArrow)) IsPressing_R = false;

    if (Input.GetKey(KeyCode.LeftArrow) && !IsPressing_R) { AddTorchForce(_radius, -1); IsPressing_L = true; }
    if(Input.GetKeyUp(KeyCode.LeftArrow)) IsPressing_L = false;

  RadiusVelocity += Accel*Time.deltaTime;
    RadiusVelocity+=Resist*Time.deltaTime*-Mathf.Sign(RadiusVelocity);
    RadiusVelocity=Mathf.Clamp(RadiusVelocity,-MaxSpeed,MaxSpeed);

    CurrentRadius += RadiusVelocity * Time.deltaTime;
    CurrentRadius = CurrentRadius > 180.0f ? CurrentRadius - 360.0f : CurrentRadius;
    CurrentRadius = CurrentRadius < -180.0f ? CurrentRadius + 360.0f : CurrentRadius;

    Vector3 _firepos = new Vector3(Mathf.Cos((CurrentRadius + 90.0f) * Mathf.Deg2Rad), Mathf.Sin((CurrentRadius + 90.0f) * Mathf.Deg2Rad));
 //   Vector3 _playernewpos = Vector3.Lerp(PlayerLastPos, PlayerTransform.position, TorchMoveSpeed * Time.deltaTime);
  //  PlayerLastPos = PlayerTransform.position;

    PlayerLastPos= Vector3.Lerp(PlayerLastPos, PlayerTransform.position, TorchMoveSpeed * Time.deltaTime);

    MyTrans.localPosition = PlayerLastPos + new Vector3(Length * _firepos.x, Length * _firepos.y, -1.0f);
    FireTransform.localPosition = PlayerLastPos + new Vector3(FireLength * _firepos.x, FireLength * _firepos.y, -1.1f);
    ColliderTransform.localPosition = PlayerLastPos + new Vector3(FireLength * _firepos.x, FireLength * _firepos.y, -1.0f);
    MyTrans.eulerAngles = new Vector3(0, 0, CurrentRadius);

    Particle_0_shape.position = FireTransform.position;
    Particle_1_shape.position = FireTransform.position;
    Particle_2_shape.position = FireTransform.position;
  }
  private void AddTorchForce(int rad, int dir)
  {
    if (rad == -2) { RadiusVelocity += InputPower * Time.deltaTime; return;}
    if(rad == 2) { RadiusVelocity -= InputPower * Time.deltaTime; return; }
    if (rad > -2 && rad < 2) { RadiusVelocity+=InputPower*Time.deltaTime*-dir; return; }
    RadiusVelocity += InputPower * Time.deltaTime * dir;
  }
  public void SetTutorial(Vector2 targetpos) //Ʃ�丮��
  {
    IsTutorial = true;
    StartCoroutine(tutorial(targetpos));
  }
  private IEnumerator tutorial(Vector3 targetpos)
  {
    CurrentRadius = 0.0f;
    int _dir = 0;
    int _radius = 0;
    while (true)
    {
      _dir = CurrentRadius > 0 && CurrentRadius < 180.0f ? 1 : -1;
      Accel = Radgravity * _dir;
      _radius = (int)CurrentRadius / 45;
      if (Input.GetKey(KeyCode.RightArrow) && !IsPressing_L) { AddTorchForce(_radius, 1); IsPressing_R = true; }
      if (Input.GetKeyUp(KeyCode.RightArrow)) IsPressing_R = false;

      if (Input.GetKey(KeyCode.LeftArrow) && !IsPressing_R) { AddTorchForce(_radius, -1); IsPressing_L = true; }
      if (Input.GetKeyUp(KeyCode.LeftArrow)) IsPressing_L = false;

      RadiusVelocity += Accel * Time.deltaTime;
      RadiusVelocity += Resist * Time.deltaTime * -Mathf.Sign(RadiusVelocity);
      RadiusVelocity = Mathf.Clamp(RadiusVelocity, -MaxSpeed, MaxSpeed);

      CurrentRadius += RadiusVelocity * Time.deltaTime;
      CurrentRadius = CurrentRadius > 180.0f ? CurrentRadius - 360.0f : CurrentRadius;
      CurrentRadius = CurrentRadius < -180.0f ? CurrentRadius + 360.0f : CurrentRadius;

      Vector3 _firepos = new Vector3(Mathf.Cos((CurrentRadius + 90.0f) * Mathf.Deg2Rad), Mathf.Sin((CurrentRadius + 90.0f) * Mathf.Deg2Rad));


      MyTrans.localPosition = targetpos + new Vector3(Length * _firepos.x, Length * _firepos.y, -1.0f);
      FireTransform.localPosition = targetpos + new Vector3(FireLength * _firepos.x, FireLength * _firepos.y, -1.1f);
      ColliderTransform.localPosition = targetpos + new Vector3(FireLength * _firepos.x, FireLength * _firepos.y, -1.0f);
      MyTrans.eulerAngles = new Vector3(0, 0, CurrentRadius);

      Particle_0_shape.position = FireTransform.position;
      yield return null;
    }
  }
  public void FinishTutorial()  //Ʃ�丮�� �������� ���� �ڷ�ƾ ���߰� �ٸ� �ڷ�ƾ���� ���� ����
  {
    StopAllCoroutines();
  }
  public void MoveToRespawn(Vector2 targetpos,float movetime)  //�÷��̾ �Ҹ�� �� ������ �������� �̵��ϱ�
  {
    StartCoroutine(movetorespawn(targetpos, movetime));
  }
  private IEnumerator movetorespawn(Vector2 targetpos,float movetime)
  {
    float _time = 0.0f;
    Vector3 _firepos = Vector3.zero;
    float _currentradius = CurrentRadius;
    Vector3 _currentpos = Vector3.zero;
    Vector3 _originpos = IsTutorial ? MyTrans.position : DeadPos;
    IsTutorial = false;
    //Ʃ�丮�� �� �������̶�� �� ��ġ -> �÷��̾� ��ġ
    //�Ϲ� �������̶�� ���� ��ġ -> �÷��̾� ��ġ
    while (_time < movetime)
    {
      CurrentRadius = Mathf.Lerp(_currentradius, 0.0f, _time / movetime);

      _firepos = new Vector3(Mathf.Cos((CurrentRadius + 90.0f) * Mathf.Deg2Rad), Mathf.Sin((CurrentRadius + 90.0f) * Mathf.Deg2Rad));

      _currentpos = Vector2.Lerp(_originpos, targetpos, Mathf.Sqrt(_time / movetime));

      MyTrans.localPosition = _currentpos + new Vector3(Length * _firepos.x, Length * _firepos.y, -1.0f);
      FireTransform.localPosition = _currentpos + new Vector3(FireLength * _firepos.x, FireLength * _firepos.y, -1.1f);
      ColliderTransform.localPosition = _currentpos + new Vector3(FireLength * _firepos.x, FireLength * _firepos.y, -1.0f);
      MyTrans.eulerAngles = new Vector3(0, 0, CurrentRadius);

      Particle_0_shape.position = FireTransform.position;
      Particle_1_shape.position = FireTransform.position;
      Particle_2_shape.position = FireTransform.position;

      _time += Time.deltaTime;
      yield return null;
    }
    PlayerLastPos = PlayerTransform.position;
    Particle_1.Play();
    IsDead = false;
  }
  private IEnumerator movetorespawn_1(Vector2 targetpos,float movetime)
  {
    float _time = 0.0f;
    Vector3 _firepos = Vector3.zero;
    float _currentradius = CurrentRadius;
    Vector3 _currentpos = Vector3.zero;
    Vector3 _middlepos = Vector3.Lerp(DeadPos, targetpos, 0.5f);
    float _length = Vector3.Distance(DeadPos, targetpos)/2.0f;
    Vector3 _tan = DeadPos - targetpos;
    float _startangle = Mathf.Atan2(_tan.y, _tan.x)*Mathf.Rad2Deg;
    float _endangle = _startangle + (DeadPos.x < targetpos.x ? 180.0f : -180.0f);
    if (_endangle < -180.0f) _endangle += 360.0f;
    else if (_endangle > 180.0f) _endangle -= 360.0f;
    float _currentangle = _startangle;
    Debug.Log($"Deadpos : {DeadPos}   Targetpos : {targetpos}   Middlepos : {_middlepos}\n" +
      $"startangle : {_startangle}   endangle : {_endangle}");
    while (_time < movetime)
    {
      CurrentRadius = Mathf.Lerp(_currentradius, RespawnSpinCount*360.0f, _time / movetime);

      _firepos = new Vector3(Mathf.Cos((CurrentRadius + 90.0f) * Mathf.Deg2Rad), Mathf.Sin((CurrentRadius + 90.0f) * Mathf.Deg2Rad));

      //   _currentpos = Vector2.Lerp(DeadPos, targetpos, Mathf.Sqrt(_time / movetime));
      _currentangle = Mathf.Lerp(_startangle, _endangle, Mathf.Sqrt(_time / movetime));
      _currentpos = _middlepos + new Vector3(Mathf.Cos(_currentangle * Mathf.Deg2Rad), Mathf.Sin(_currentangle * Mathf.Deg2Rad))* _length;

      MyTrans.localPosition = _currentpos + new Vector3(Length * _firepos.x, Length * _firepos.y, -1.0f);
      FireTransform.localPosition = _currentpos + new Vector3(FireLength * _firepos.x, FireLength * _firepos.y, -1.0f);
      ColliderTransform.localPosition = _currentpos + new Vector3(FireLength * _firepos.x, FireLength * _firepos.y, -1.0f);
      MyTrans.eulerAngles = new Vector3(0, 0, CurrentRadius);

      Particle_0_shape.position = FireTransform.position;
      Particle_1_shape.position = FireTransform.position;
      Particle_2_shape.position = FireTransform.position;

      _time += Time.deltaTime;
      yield return null;
    }
    Particle_1.Play();
    IsDead = false;
    yield return null;
  }

  public void MoveToTomb(Vector2 targetpos,float movetime)
  {
    StartCoroutine(movetotomb(targetpos, movetime));
  }
  private IEnumerator movetotomb(Vector2 targetpos,float movetime)
  {
    float _time = 0.0f;
    IsDead = true;
    float _currentradius = CurrentRadius;
    Vector3 _firepos = Vector3.zero;
    Vector3 _currentpos = Vector3.zero;
    Vector3 _originpos = PlayerTransform.position;

    while (_time < movetime)
    {

      CurrentRadius = Mathf.Lerp(_currentradius, 0.0f, _time / movetime);

      _firepos = new Vector3(Mathf.Cos((CurrentRadius + 90.0f) * Mathf.Deg2Rad), Mathf.Sin((CurrentRadius + 90.0f) * Mathf.Deg2Rad));

      _currentpos = Vector2.Lerp(_originpos, targetpos, Mathf.Sqrt(_time / movetime));

      MyTrans.localPosition = _currentpos + new Vector3(Length * _firepos.x, Length * _firepos.y, -1.0f);
      FireTransform.localPosition = _currentpos + new Vector3(FireLength * _firepos.x, FireLength * _firepos.y, -1.1f);
      ColliderTransform.localPosition = _currentpos + new Vector3(FireLength * _firepos.x, FireLength * _firepos.y, -1.0f);
      MyTrans.eulerAngles = new Vector3(0, 0, CurrentRadius);

      Particle_0_shape.position = FireTransform.position;
      _time += Time.deltaTime;
      yield return null;
    }
  }
  public void End()
  {
    Particle_0.Stop();
    FireTransform.gameObject.SetActive(false);
    gameObject.SetActive(false);
  }
}
