using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerController
{
    Valve.VR.InteractionSystem.Player GetVRPlayer();
    Vector3 GetTargetPosition();

    Transform GetCastingHand(int _iCastingHandIndex);
}
