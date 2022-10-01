using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDrop : MonoBehaviour
{
  [SerializeField] private float FallingSpeed = -9.8f;
  [SerializeField] private Transform MyTransform = null;
  [SerializeField] private SpriteRenderer MyRenderer = null;
  [SerializeField] private ParticleSystem MyParticle = null;

  public void Fall(Vector2 newpos)
  {
    MyTransform.localPosition = newpos;
    MyTransform.tag = "Water";
    MyRenderer.enabled = true;
    StartCoroutine(waterfall());
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
