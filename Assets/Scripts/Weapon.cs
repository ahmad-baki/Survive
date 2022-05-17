using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int dmg = 5;
    public PlayerCombatState playerCombatState;


    bool _attack;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartAttack()
    {
        _attack = true;
    }

    public void EndAttack()
    {
        _attack = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        var healthManager = other.GetComponent<HealthManager>();
        if (_attack && healthManager) {
            healthManager.TakeDmg(dmg, this.gameObject);
        }
    }
}
