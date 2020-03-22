using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AwardCardModel : MonoBehaviour {

    public int awardType;
    public Text awardTitle;
    public Text awardAmountText;

    int awardAmount;

    public void SetAwardContent()
    {
        switch (awardType)
        {
            case 0:
                awardTitle.text = "莫得东西";
                awardAmountText.text = "";
                break;
            case 1:
                awardTitle.text = "金币";
                awardAmount = Random.Range(100000, 1000000);
                awardAmountText.text = "X " + awardAmount.ToString();
                break;
            default:
                break;
        }
        awardTitle.DOFade(0, 0.2f);
        awardAmountText.DOFade(0, 0.2f);
    }

    public void OnClickToAward()
    {
        awardTitle.DOFade(1, 1f);
        awardAmountText.DOFade(1, 1f);
        switch (awardType)
        {
            case 1:
                GameState.instance.ownCoins += awardAmount;
                break;
            default:
                break;
        }
        this.GetComponent<Button>().enabled = false;

        StartCoroutine(TextAppear());
    }

    IEnumerator TextAppear()
    {
        SelectAwardUIManager manager = GameObject.Find("CardsManager").GetComponent<SelectAwardUIManager>();
        manager.isSelect = false;
        yield return new WaitForSeconds(1f);

        foreach (var item in manager.awardCards)
        {
            item.GetComponent<Button>().enabled = false;
            AwardCardModel model = item.GetComponent<AwardCardModel>();
            model.awardTitle.DOFade(1, 1f);
            model.awardAmountText.DOFade(1, 1f);
        }
    }
}
