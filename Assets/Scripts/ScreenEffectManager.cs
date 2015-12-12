using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScreenEffectManager : SingletonMonoBehavior<ScreenEffectManager> {
    public GameObject screenEffectPrefab;
    Queue<ParticleListener> screenEffect = new Queue<ParticleListener>();

    public void Init()
    {
        LoadScreenEffect(10, screenEffectPrefab);
        screenEffectPrefab = null;
    }

    public ParticleListener GetScreenEffect()
    {
        ParticleListener pl = screenEffect.Dequeue();

        if (screenEffect.Count == 0)
            LoadScreenEffect(3, pl.gameObject);

        return pl;
    }

    void LoadScreenEffect(int count, GameObject prefab)
    {
        for (int i = 0; i < count; ++i)
        {
            GameObject tmp = Instantiate(prefab) as GameObject;
            tmp.name = prefab.name;
            tmp.transform.parent = this.transform;
            ParticleListener pl = tmp.GetComponent<ParticleListener>();
            pl.Init(SaveScreenEffect);
            SaveScreenEffect(pl);
        }
    }

    void SaveScreenEffect(ParticleListener pl)
    {
        screenEffect.Enqueue(pl);
    }
}
