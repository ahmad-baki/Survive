using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HealthManager : MonoBehaviour
{
    public int maxHealth;
    
    protected int _health;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void TakeDmg(int dmg, GameObject caller = null)
    {
        _health -= dmg;
        if(_health < 0)
        {
            Death(caller);
        }
    }

    public abstract void Death(GameObject killer);
}
