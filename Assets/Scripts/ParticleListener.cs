using UnityEngine;
using System.Collections;

public class ParticleListener : MonoBehaviour {
    VoidParticleListener recycle;
    ParticleSystem[] particles;
    public bool autoDestroy = true;
    
    void Awake()
    {
        particles = this.GetComponentsInChildren<ParticleSystem>();
    }

    public void Init(VoidParticleListener recycle)
    {
        this.gameObject.SetActive(false);
        this.recycle = recycle;
    }

    public void ShowEffect(Vector3 pos)
    {
        this.transform.position = pos;
        this.gameObject.SetActive(true);
    }

    void Update()
    {
        if (autoDestroy)
        {
            bool isAlive = false;
            foreach (ParticleSystem ps in particles)
            {
                if (ps.IsAlive())
                {
                    isAlive = true;
                    break;
                }
            }
            if (!isAlive)
            {
                if (recycle != null)
                {
                    this.gameObject.SetActive(false);
                    recycle(this);
                }
                else
                    Destroy(this.gameObject);
            }
        }
    }
}
