using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameModelsDestroyEffect : MonoBehaviour {

    [SerializeField]
    Animator animator;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void FadeEffect()
    {
        animator.gameObject.SetActive(true);
        this.GetComponent<SpriteRenderer>().DOBlendableColor(Color.black, 0.2f);
        this.transform.DOBlendableScaleBy(new Vector3(0.2f, 0.2f, 0), 0.2f);
        this.GetComponent<SpriteRenderer>().DOFade(0, 0.5f);
        Model model = GetComponent<Model>();
        if (model.type == 1)
        {
            AniPlay(animator, "ColorId", model.id);
        }
        else if (model.type == 2)
        {
            AniPlay(animator, "ColorId", 5);
        }
    }

    void AniPlay(Animator ani, string aniParam, int param)
    {
        ani.SetInteger(aniParam, param);
        Debug.Log(ani.GetCurrentAnimatorStateInfo(0).fullPathHash);
        Timer.Register(0.7f, () =>
        {
            animator.gameObject.SetActive(false);
        });
    }
    void AniPlay(Animator ani, string aniName)
    {
        ani.Play(aniName, 1);
        Timer.Register(0.7f, () =>
        {
            animator.gameObject.SetActive(false);
        });
    }


    public void ResetState(bool isInit = false)
    {
        this.GetComponent<SpriteRenderer>().DOBlendableColor(Color.white, 0f);
        if (!isInit)
        {
            this.transform.DOBlendableScaleBy(new Vector3(-0.2f, -0.2f, 0), 0f);
        }
        this.GetComponent<SpriteRenderer>().DOFade(1, 0f);
    }

    private void OnDisable()
    {
        Destroy(this.gameObject);
    }

    public void AnimatorGbVis()
    {
        animator.gameObject.SetActive(false);
    }
}
