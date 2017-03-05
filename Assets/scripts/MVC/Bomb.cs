using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {
    public GameObject ExplosionParticlesPrefab;
    int Length;
    Dictionary<Level.Direction, int> fx_length_map = new Dictionary<Level.Direction, int>();
    public void Init(float life_time, Dictionary<Level.Direction, int> length_map,int length = 1)
    {
        fx_length_map.Clear();
        fx_length_map = length_map;
        Length = length;
        Invoke("Explode", life_time);
    }
    void Explode()
    {
        Debug.Log(fx_length_map[Level.Direction.Left] + " " + fx_length_map[Level.Direction.Right] + " " + fx_length_map[Level.Direction.Up] + " " + fx_length_map[Level.Direction.Down] + " ");
        SetupDiectionalParticle(fx_length_map[Level.Direction.Down], ExplosionParticlesPrefab.transform.rotation);
        SetupDiectionalParticle(fx_length_map[Level.Direction.Up], ExplosionParticlesPrefab.transform.rotation * Quaternion.Euler(0, 180, 0));
        SetupDiectionalParticle(fx_length_map[Level.Direction.Left], ExplosionParticlesPrefab.transform.rotation * Quaternion.Euler(0, 90, 0));
        SetupDiectionalParticle(fx_length_map[Level.Direction.Right], ExplosionParticlesPrefab.transform.rotation * Quaternion.Euler(0, 270, 0));
        Destroy(gameObject, 0.1f);
    }

    void SetupDiectionalParticle(int len, Quaternion rot)
    {
        if (len <= 0)
            return;
        GameObject explosion = (GameObject)Instantiate(ExplosionParticlesPrefab, transform.position, rot);
        ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
        ParticleSystem.ShapeModule pss = ps.shape;
        pss.length = len;
        Destroy(explosion, explosion.GetComponent<ParticleSystem>().main.startLifetime.constant);
    }
}
