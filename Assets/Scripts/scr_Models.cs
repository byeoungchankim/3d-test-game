using System;
using System.Collections.Generic;
using UnityEngine;

public class scr_Models : MonoBehaviour
{
    #region - player -

    public enum PlayerStance
    {
        Standing,
        Crouching,
        Prone
    }

    [Serializable]
    public class PlayerSettingsModel
    {
        //카메라 마우스로 돌림?
        [Header("View setting")]
        public float ViewXSensitivity;
        public float ViewYSensitivity;

        public bool ViewXInverted;
        public bool ViewYInverted;

        [Header("Movement")]
        public float WalkingForwardSpeed;
        public float WalkingBackwardSpeed;
        public float WalkingStrafeSpeed;

        [Header("Jumping")]
        public float JumpingHeight;
        public float JumpingFalloff;


    }
    [Serializable]
    public class CharacterStance
    {
        public float CamerHeight;
        public CapsuleCollider stanceCollider;
    }
    #endregion

}
