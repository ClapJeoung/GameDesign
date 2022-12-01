using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipManager : MonoBehaviour
{
  private static SkipManager instance;
  public static SkipManager Instance { get { return instance; } }
  public bool isfirst = true;
  private void Awake()
  {
    if (instance == null)
    {
      DontDestroyOnLoad(gameObject);
      instance = this;
    }
    else {SkipManager.instance.isfirst = false; Destroy(gameObject); }
  }
}
