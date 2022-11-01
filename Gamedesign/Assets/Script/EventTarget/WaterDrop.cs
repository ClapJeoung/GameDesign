using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDrop : MonoBehaviour
{
  [SerializeField] private float FallingSpeed = -9.8f;
  [SerializeField] private Transform MyTransform = null;
  [SerializeField] private SpriteRenderer MyRenderer = null;
  [SerializeField] private ParticleSystem MyParticle = null;
  [SerializeField] private float SizeupTime = 0.5f;

  public void Fall(Vector3 newpos)
  {
    MyTransform.localScale = Vector3.zero;
    MyTransform.localPosition = newpos;
    MyTransform.tag = "Water";
    MyRenderer.enabled = true;
    StartCoroutine(waterfall());
    StartCoroutine(sizegrow());
  }
  private IEnumerator waterfall()
  {
    Vector2 _velocity = Vector2.zero;
    while (true)
    {
      _velocity += Vector2.up * Time.deltaTime * FallingSpeed;
      MyTransform.Translate(_velocity*Time.deltaTime);
      yield return null;
    }
  }
  private IEnumerator sizegrow()
  {
    float _time = 0.0f;
    while(_time< SizeupTime)
    {
      MyTransform.localScale = Vector3.one * Mathf.Pow(_time / SizeupTime, 1.5f);
      _time += Time.deltaTime;
      yield return null;
    }
  }
  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
    {
      StopAllCoroutines();
      MyRenderer.enabled = false;
      MyTransform.tag = "Untagged";
      MyParticle.Play();
    }
  }
}
