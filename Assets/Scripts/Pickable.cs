using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : Interactable
{
    public Item item;
    public int amount = 1;

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }


    public override void Interact(GameObject player)
    {
        var inventarManager = player.GetComponent<InventarManager>();
        var playerControl = player.GetComponent<PlayerControl>();
        if (inventarManager && playerControl)
        {
            inventarManager.AddItem(item, amount);
            playerControl.interactables.Remove(this.GetComponent<Collider>());
            Destroy(this.gameObject);
        }
        else
        {
            Debug.LogWarning($"Couldnt find {((inventarManager) ? "PlayerControl" : "InventarManager")} on GameObject {player.name}");
        }
    }
}
