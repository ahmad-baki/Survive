using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RessourceHealthManager : HealthManager
{
    public RessourceSettings ressourceSettings;
    public GameObject dropPrefab;

    void Start()
    {
        maxHealth = ressourceSettings.maxHealth;
    }

    public override void TakeDmg(int dmg, GameObject caller = null)
    {
        base.TakeDmg(dmg, caller);
    }


    public override void Death(GameObject killer)
    {
        if (dropPrefab != null)
        {
            GameObject drop = Instantiate(dropPrefab, transform.position, Quaternion.identity);
            if(drop.GetComponent<Pickable>() is var pickable)
            {
                pickable.amount = ressourceSettings.dropAmount;
            }
            Destroy(gameObject);
        }
    }
}
