using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AvatarManager : MonoBehaviour
{
    public Button avatarLeftButton, avatarRightButton;
    public float animButtonDuration = 0.5f;
    public float animButtonDistance = 1.8f;
    public Transform avatarTransList;
    int pageIndex = 0;
    bool isAnimating = false;

    void OnEnable()
    {
        var rectT = transform.GetComponent<RectTransform>();
        rectT.DOPunchAnchorPos(rectT.anchoredPosition * 1.2f, 0.5f, 2, 0.3f);
        FillAvatars(pageIndex);
    }

    public void BTN_AvatarLeft()
    {
        if (isAnimating) return;
        AudioManager.Instance.PlayButtonClick();
        iTween.PunchScale(avatarLeftButton.gameObject, Vector3.one * animButtonDistance, animButtonDuration);
        PreviosAvatarPage();
    }

    public void BTN_AvatarRight()
    {
        if (isAnimating) return;
        AudioManager.Instance.PlayButtonClick();
        iTween.PunchScale(avatarRightButton.gameObject, Vector3.one * animButtonDistance, animButtonDuration);
        NextAvatarPage();
    }

    void PreviosAvatarPage()
    {
        var totalPages = CanvasManager.Instance.spritesAvatar.Count / avatarTransList.childCount;
        pageIndex--;
        if (pageIndex < 0) pageIndex = totalPages;
        FillAvatars(pageIndex);
    }

    void NextAvatarPage()
    {
        var totalPages = CanvasManager.Instance.spritesAvatar.Count / avatarTransList.childCount;
        pageIndex++;
        if (totalPages < pageIndex) pageIndex = 0;
        FillAvatars(pageIndex);
    }

    async void FillAvatars(int pageIndex)
    {
        if (isAnimating) return;
        isAnimating = true;
        var avatarList = CanvasManager.Instance.spritesAvatar;
        foreach (Transform t in avatarTransList) t.gameObject.SetActive(false);
        for (var x = 0; x < avatarTransList.childCount; x++)
        {
            var spriteIndex = x + (pageIndex * avatarTransList.childCount);
            if (avatarList.Count <= spriteIndex) break;
            var avatarT = avatarTransList.GetChild(x);
            avatarT.GetComponent<Image>().sprite = avatarList[spriteIndex];
            var avatarButton = avatarT.GetComponent<Button>();
            avatarButton.onClick.RemoveAllListeners();
            avatarButton.onClick.AddListener(() => BTN_Avatar(spriteIndex + 1));
            avatarT.gameObject.SetActive(true);
            avatarT.DOPunchScale(Vector3.one, 0.5f, 5, 0.5f);
            AudioManager.Instance.PlayBlop();
            await UniTask.Delay(90);
        }
        isAnimating = false;
    }

    void BTN_Avatar(int avatarIndex)
    {
        AudioManager.Instance.PlayBlop();
        CanvasManager.Instance.SetAvatarSprite(avatarIndex);
        gameObject.SetActive(false);
    }

}
