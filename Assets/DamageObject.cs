using UnityEngine;
using System.Collections;

public class DamageObject : MonoBehaviour 
{
    [SerializeField]
    private HPBar _hpBar = null;
    [SerializeField]
    private int type = -1;
    [SerializeField]
    private int player = -1;

    void OnTriggerEnter2D(Collider2D collide)
    {
        //Play SOund?
        _hpBar.AttackDone(player, type);
    }
}
