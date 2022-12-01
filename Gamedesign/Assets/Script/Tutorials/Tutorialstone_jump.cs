using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorialstone_jump : MonoBehaviour
{
  [SerializeField] private float Duration = 0.5f;
  [SerializeField] private Sprite IdleImg = null;
  [SerializeField] private Sprite ActiveImg = null;
  [SerializeField] private UnityEngine.Rendering.Universal.Light2D MyLight = null;
  [SerializeField] private ParticleSystem MyParticle = null;
  [SerializeField] private SpriteRenderer ActiveSpr = null;
  private StageCollider MySC = null;
  public void Setup()
  {
    MySC=transform.parent.GetComponent<StageCollider>();
  }
  private void Start()
  {
    Setup();
  }
  public void Twinkle()
  {
    if (GameManager.Instance.CurrentSC != MySC) return;
    Color _col = Color.white;
    ActiveSpr.color = _col;
    MyLight.intensity = 1.0f;
    StartCoroutine(dying());
    MyParticle.Play();
  }
  private IEnumerator dying()
  {
    float _time = 0.0f,_origin=1.0f,_target=0.0f;
    float _originint = MyLight.intensity, _targetint = 0.0f;
    Color _col = Color.white;
    while(_time< Duration)
    {
      _col.a=Mathf.Lerp(_origin,_target,_time/ Duration);
      ActiveSpr.color = _col;
      MyLight.intensity = Mathf.Lerp(_originint,_targetint,_time/ Duration);
      _time += Time.deltaTime;yield return null;
    }
    _col.a = 0.0f;
    ActiveSpr.color = _col;
    MyLight.intensity=0.0f;
  }
}
