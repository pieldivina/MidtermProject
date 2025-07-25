using UnityEngine;

public class ExtraLifePowerUp : PowerUpBehavior
{
    [SerializeField] private float duration = 5f;

    public override void Activate(FroggerBehavior frogger)
    {
        GameBehavior.Instance.AddLife();  
        frogger.SetImmunity(true);
        frogger.SetPoweredUp(true); 
    }

    public override void Deactivate(FroggerBehavior frogger)
    {
        frogger.SetImmunity(false);
        frogger.SetPoweredUp(false); 
    }
}