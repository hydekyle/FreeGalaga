using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarManager : MonoBehaviour
{
    public Button avatarLeftButton, avatarRightButton;
    public float animButtonDuration = 0.2f;
    public float animButtonDistance = 2f;

    public void BTN_AvatarLeft()
    {
        iTween.PunchScale(avatarLeftButton.gameObject, Vector3.one * animButtonDistance, animButtonDuration);
    }

    public void BTN_AvatarRight()
    {
        iTween.PunchScale(avatarRightButton.gameObject, Vector3.one * animButtonDistance, animButtonDuration);
    }
}
