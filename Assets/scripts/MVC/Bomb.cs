using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {
    public GameObject ExplosionParticlesPrefab;
    int Length;
    public void Init(float life_time, int length = 1)
    {
        Length = length;
        Invoke("Explode", life_time);
    }
    void Explode()
    {
        GameObject explosion = (GameObject)Instantiate(ExplosionParticlesPrefab, transform.position, ExplosionParticlesPrefab.transform.rotation);
        GameObject explosion1 = (GameObject)Instantiate(ExplosionParticlesPrefab, transform.position, ExplosionParticlesPrefab.transform.rotation *= Quaternion.Euler(0, 90, 0));
        GameObject explosion2 = (GameObject)Instantiate(ExplosionParticlesPrefab, transform.position, ExplosionParticlesPrefab.transform.rotation *= Quaternion.Euler(0, 180, 0));
        GameObject explosion3 = (GameObject)Instantiate(ExplosionParticlesPrefab, transform.position, ExplosionParticlesPrefab.transform.rotation *= Quaternion.Euler(0, 270, 0));
        ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
        ParticleSystem.ShapeModule pss = ps.shape;
        pss.length = Length;
        ps = explosion1.GetComponent<ParticleSystem>();
        pss = ps.shape;
        pss.length = Length;
        ps = explosion2.GetComponent<ParticleSystem>();
        pss = ps.shape;
        pss.length = Length;
        ps = explosion3.GetComponent<ParticleSystem>();
        pss = ps.shape;
        pss.length = Length;
        Destroy(explosion, explosion.GetComponent<ParticleSystem>().main.startLifetime.constant);
        Destroy(explosion1, explosion1.GetComponent<ParticleSystem>().main.startLifetime.constant);
        Destroy(explosion2, explosion2.GetComponent<ParticleSystem>().main.startLifetime.constant);
        Destroy(explosion3, explosion3.GetComponent<ParticleSystem>().main.startLifetime.constant);
        Destroy(gameObject, 0.1f);
    }
}
