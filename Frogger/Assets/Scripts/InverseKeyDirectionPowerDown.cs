using UnityEngine;

public class InverseKeyDirectionPowerDown : PowerUpBehavior
{
    public override void Activate(FroggerBehavior frogger)
    {
        frogger.SetInverseControls(true);
    }

    public override void Deactivate(FroggerBehavior frogger)
    {
        frogger.SetInverseControls(false);
    }
}