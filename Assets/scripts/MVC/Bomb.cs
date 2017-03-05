using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {
    public GameObject ExplosionParticlesPrefab;
    Dictionary<Level.Direction, int> fx_length_map = new Dictionary<Level.Direction, int>();
    public delegate void ReplenishBomb();
    public delegate void DestoyAffectedBlocks(List<Block> block_list);
    public event ReplenishBomb Replenish;
    public event DestoyAffectedBlocks DestroyBlocks;
    List<Block> AffectedBlockList = new List<Block>();
    public void Init(float life_time, Dictionary<Level.Direction, int> length_map, bool remote, List<Block> block_list)
    {
        fx_length_map.Clear();
        fx_length_map = length_map;
        AffectedBlockList = block_list;
        if (!remote)
            Invoke("Explode", life_time);
    }
    public void Explode()
    {
        SetupDiectionalParticle(fx_length_map[Level.Direction.Down], ExplosionParticlesPrefab.transform.rotation);
        SetupDiectionalParticle(fx_length_map[Level.Direction.Up], ExplosionParticlesPrefab.transform.rotation * Quaternion.Euler(0, 180, 0));
        SetupDiectionalParticle(fx_length_map[Level.Direction.Left], ExplosionParticlesPrefab.transform.rotation * Quaternion.Euler(0, 90, 0));
        SetupDiectionalParticle(fx_length_map[Level.Direction.Right], ExplosionParticlesPrefab.transform.rotation * Quaternion.Euler(0, 270, 0));
        Destroy(gameObject, 0.1f);
        Replenish();
        DestroyBlocks(AffectedBlockList);
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
