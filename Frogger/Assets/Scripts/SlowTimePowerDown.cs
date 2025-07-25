using UnityEngine;

public class SlowTimePowerDown : PowerUpBehavior
{
    public override void Activate(FroggerBehavior frogger)
    {
        frogger.SetDoubleTapMode(true);
    }

    public override void Deactivate(FroggerBehavior frogger)
    {
        frogger.SetDoubleTapMode(false);
    }
}