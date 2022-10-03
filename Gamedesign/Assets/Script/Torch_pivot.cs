using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch_pivot : MonoBehaviour
{
  [SerializeField] private float Radgravity = 60.0f;
  private float CurrentRadius = 0.0f;
  private float RadiusVelocity = 0.0f;
  [SerializeField] private float InputPower = 70.0f;
  [SerializeField] private float Length;
  private Transform MyTrans;
  [SerializeField] private float MaxSpeed = 60.0f;
  [SerializeField] private float Resist = 30.0f;
  private float Accel = 0.0f;
  private bool IsPressing_R = false;
  private bool IsPressing_L = false;
  [SerializeField] private float ParticleLength;
  [SerializeField] private float FireLength;
  [SerializeField] private Transform ColliderTransform;
  [SerializeField] private Transform FireTransform;
  private ParticleSystem Particle_0 = null;
   private ParticleSystem.ShapeModule Particle_0_shape;
  private ParticleSystem Particle_1 = null;
  private ParticleSystem.ShapeModule Particle_1_shape;
   private ParticleSystem Particle_2 = null;
  private ParticleSystem.ShapeModule Particle_2_shape;
  private bool IsDead = false;
  private void Start()
  {
    Setup();
  }
  public void Setup()
  {
    MyTrans = transform;
    ParticleSystem[] particles = GameManager.Instance.GetPlayerParticles();
    Particle_0= particles[0];
    Particle_1= particles[1];
    Particle_2= particles[2];
    Particle_0_shape = Particle_0.shape;
    Particle_1_shape = Particle_1.shape;
    Particle_2_shape = Particle_2.shape;
    if(!Particle_0.isPlaying)particles[0].Play();
  }
  public void Dead() => IsDead = true;
  private void Update()
  {
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
    
    MyTrans.localPosition = new Vector3(Length * _firepos.x, Length * _firepos.y, -1.0f);
    FireTransform.localPosition = new Vector3(FireLength * _firepos.x, FireLength * _firepos.y, -1.0f);
    ColliderTransform.localPosition = new Vector3(FireLength * _firepos.x, FireLength * _firepos.y, -1.0f);
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
}
