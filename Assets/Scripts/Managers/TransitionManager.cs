using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    [SerializeField] private GameObject transitionImage;
    [SerializeField] private Animator anim;

    public void SetTransitionCanvas(bool active)
    {
        transitionImage.SetActive(active);
    }

    public void FadeOutAnimation()
    {
        anim.SetTrigger("FadeOut");
    }
}
