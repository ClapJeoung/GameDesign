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
  private bool IsDead = false;
  private void Start()
  {
    Setup();
  }
  public void Setup()
  {
    MyTrans = transform;
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
    
    MyTrans.localPosition = new Vector3(Length * Mathf.Cos((CurrentRadius+90.0f) * Mathf.Deg2Rad), Length * Mathf.Sin((CurrentRadius+90.0f) * Mathf.Deg2Rad), -1.0f);
    FireTransform.localPosition = new Vector3(FireLength * Mathf.Cos((CurrentRadius + 90.0f) * Mathf.Deg2Rad), FireLength * Mathf.Sin((CurrentRadius + 90.0f) * Mathf.Deg2Rad), -1.0f);
    ColliderTransform.localPosition = new Vector3(FireLength * Mathf.Cos((CurrentRadius + 90.0f) * Mathf.Deg2Rad), FireLength * Mathf.Sin((CurrentRadius + 90.0f) * Mathf.Deg2Rad), -1.0f);
    MyTrans.eulerAngles = new Vector3(0, 0, CurrentRadius);
  }
  private void AddTorchForce(int rad, int dir)
  {
    if (rad == -2) { RadiusVelocity += InputPower * Time.deltaTime; return;}
    if(rad == 2) { RadiusVelocity -= InputPower * Time.deltaTime; return; }
    if (rad > -2 && rad < 2) { RadiusVelocity+=InputPower*Time.deltaTime*-dir; return; }
    RadiusVelocity += InputPower * Time.deltaTime * dir;
  }
}
