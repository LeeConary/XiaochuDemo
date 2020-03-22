using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PopFadeEffect : MonoBehaviour {

    [SerializeField]
    Text gemInfo;
    [SerializeField]
    Text scoreInfo;

	public void PopFade(int gemId, int gemCount, long score)
    {
        SetText(gemId, gemCount, score);
        transform.DOBlendableLocalMoveBy(new Vector3(0, 60, 0), 2f);
        Text[] text = GetComponentsInChildren<Text>();
        foreach (var item in text)
        {
            item.DOFade(0, 1.5f).onComplete = () =>
            {
                Destroy(this.gameObject);
            };
        }
    }

    void SetText(int gemId, int gemCount,long score)
    {
        switch (gemId)
        {
            case 0:
                gemInfo.color = Color.green;
                gemInfo.text = "绿宝石 - " + gemCount.ToString();
                scoreInfo.text = "得分 + " + score;
                break;
            case 1:
                gemInfo.color = Color.cyan;
                gemInfo.text = "蓝宝石 - " + gemCount.ToString();
                scoreInfo.text = "得分 + " + score;
                break;
            case 2:
                gemInfo.color = Color.yellow;
                gemInfo.text = "黄宝石 - " + gemCount.ToString();
                scoreInfo.text = "得分 + " + score;
                break;
            case 3:
                gemInfo.color = Color.red;
                gemInfo.text = "红宝石 - " + gemCount.ToString();
                scoreInfo.text = "得分 + " + score;
                break;
            case 4:
                gemInfo.color = Color.blue;
                gemInfo.text = "紫宝石 - " + gemCount.ToString();
                scoreInfo.text = "得分 + " + score;
                break;
            default:
                break;
        }
    }
}
