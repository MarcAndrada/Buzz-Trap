using TMPro;
using UnityEngine;
public class QueenBee : Bee
{



    public override void NoQueenBehaviour()
    {
        Debug.Log("WTF?");
    }

    public override void QueenBehaviour()
    {
        Debug.Log("No hay na :3");
    }

    public void RiseWand(bool _rise)
    {
        animator.SetBool("RiseWand", _rise);
    }

    protected override void OnEnable()
    {
        AudioManager.instance.Play2dOneShotSound(spawnClip, "Master", 1.2f);
    }
}
