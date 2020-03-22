using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

/// <summary>
/// 选择奖励逻辑管理
/// </summary>
public class SelectAwardUIManager : MonoBehaviour {

    public GameObject[] awardCards;

    [SerializeField]
    Text remainTimeText;

    [SerializeField]
    GameObject timeRemain;
    [SerializeField]
    GameObject clickToContinue;

    [SerializeField]
    int constSeconds = 10;

    int remainSeconds;

    public bool isSelect = true;
    // Use this for initialization
    void Start()
    {
        remainSeconds = constSeconds;
        foreach (var item in awardCards)
        {
            AwardCardModel model = item.GetComponent<AwardCardModel>();
            model.awardType = Random.Range(0, 2);
            model.SetAwardContent();
        }
        StartCoroutine(IsSelectCard());
    }

    public void OnClickToNext()
    {
        Debug.Log("继续游戏");
        SceneManager.LoadScene("Scene");
    }

    IEnumerator IsSelectCard()
    {
        for (int i = 0; i < constSeconds + 1; i++)
        {
            if (isSelect)
            {
                remainSeconds -= 1;
                remainTimeText.text = (remainSeconds + 1).ToString();
                yield return new WaitForSeconds(1f);
            }
            else
            {
                yield return new WaitForSeconds(1f);
                clickToContinue.SetActive(true);
                timeRemain.SetActive(false);
                break;
            }
        }
        if (remainSeconds <= 0)
        {
            awardCards[Random.Range(0, awardCards.Length)].GetComponent<AwardCardModel>().OnClickToAward();
            isSelect = false;
            yield return new WaitForSeconds(1f);
            clickToContinue.SetActive(true);
            timeRemain.SetActive(false);
        }
        yield return null;
    }
}
