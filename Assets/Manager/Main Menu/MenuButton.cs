using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour, IPointerClickHandler
{
    //public Image menuButton;
    public void OnPointerClick(PointerEventData eventData)
    {
        Sequence menuSeq = DOTween.Sequence();
        menuSeq.Append(transform.DOScale(0.8f, 0.15f));
        menuSeq.Append(transform.DOScale(1f, 0.15f));
        menuSeq.OnComplete(() =>
        {
            Debug.Log("Menu Button Clicked");
            SceneManager.LoadScene(1);
        });
    }
}
