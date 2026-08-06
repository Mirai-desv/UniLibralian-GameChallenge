using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayButton : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
{
    public Image playButton;
    public void OnPointerClick(PointerEventData eventData)
    {
        Sequence playSeq = DOTween.Sequence();
        playSeq.Append(transform.DOScale(0.8f, 0.15f));
        playSeq.Append(transform.DOScale(1f, 0.15f));
        playSeq.OnComplete(() =>
        {
            Debug.Log("Play Button Cliked");
            SceneManager.LoadScene(3);
        });
    }
    /*
    public void OnPointerEnter(PointerEventData eventData)
    {
        playButton.DOColor(Color.gray, 0.15f);
    }
    */
    public void OnPointerExit(PointerEventData eventData)
    {
        playButton.DOColor(Color.white, 0.15f);
        transform.DOScale(1f, 0.15f);
    }
}
