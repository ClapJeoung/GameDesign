using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Tutorialstone_move : MonoBehaviour
{
  [SerializeField] private Sprite IdleImg = null;
  [SerializeField] private Sprite RightImg = null;
  [SerializeField] private Sprite LeftImg = null;
  private SpriteRenderer MySpr = null;
  [SerializeField] private ParticleSystem MyParticle = null;
  [SerializeField] private Light2D MyLight = null;
  private StageCollider MySC = null;

  private void Start()
  {
    Setup();
  }
  public void Setup()
  {
    MySC = transform.parent.GetComponent<StageCollider>();
    MySpr=GetComponent<SpriteRenderer>();
  }
  private void Update()
  {
    if (GameManager.Instance.CurrentSC != MySC) return;
    if (Input.GetKeyDown(KeyCode.A))
    {
      MySpr.sprite = LeftImg;
      MyParticle.Play();
      MyLight.enabled = true;
    }
    if (Input.GetKeyDown(KeyCode.D))
    {
      MySpr.sprite = RightImg;
      MyParticle.Play();
      MyLight.enabled = true;
    }
    if(Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
    {
      MySpr.sprite = IdleImg;
      MyParticle.Stop();
      MyLight.enabled = false;
    }
  }
}
