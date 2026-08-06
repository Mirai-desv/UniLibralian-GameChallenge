using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Sequence settingSeq = DOTween.Sequence();
        settingSeq.Append(transform.DOScale(0.8f, 0.15f));
        settingSeq.Append(transform.DOScale(1f, 0.15f));
        settingSeq.OnComplete(() =>
        {
            Debug.Log("Setting Button Clicked");
            SceneManager.LoadScene(2);
        });
    }
}
