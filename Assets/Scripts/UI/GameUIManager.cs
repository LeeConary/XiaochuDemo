using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{

    public Text ownCoins;
    public Image[] gemsCrashNote;

    public Button clearButton;
    [SerializeField]
    Dropdown costCoins;
    [SerializeField]
    Text combos;
    [SerializeField]
    Button awardNotes;
    [SerializeField]
    Text tipText;

    [Space]

    [SerializeField]
    GameObject combosPanel;
    [SerializeField]
    GameObject combosCreatePos;
    [SerializeField]
    GameObject awardPanel;
    [SerializeField]
    GameObject tipWindow;
    [SerializeField]
    GameObject CZPanel;

    public GameObject goToSelectAward;

    public GameObject PassBy;

    [Space]

    public Text level;
    public Text rateRank;
    public Toggle isAutoClear;

    [HideInInspector]
    public Text clearButtonText;
    GameManager gameManager;

    // Use this for initialization
    void Start()
    {

        gameManager = GetComponent<GameManager>();
        clearButtonText = clearButton.GetComponentInChildren<Text>();
        level.text = "第 " + (GameState.instance.currentLevel + 1).ToString() + " 关";

        ownCoins.text = GameState.instance.ownCoins.ToString();

        GetCostCoins(costCoins.value);

        foreach (var item in gemsCrashNote)
        {
            item.GetComponentInChildren<Text>().text = 0.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        ownCoins.text = GameState.instance.ownCoins.ToString();
        combos.text = GameState.instance.currentCombos.ToString();
    }

    public void Set_CHONG_ZHI_Tip(int needCoins)
    {
        Time.timeScale = 0;
        tipText.text = "当前还需要" + needCoins + "金币。请充值或进行更低额度的押注";
        tipWindow.SetActive(true);
        ClickButtonsStateSwitch(false);

        isAutoClear.isOn = false;
        isAutoClear.enabled = false;

        clearButtonText.text = "金币不足";
    }

    public void OnClickToHideTip()
    {
        Time.timeScale = 1;
        tipWindow.SetActive(false);
        costCoins.enabled = true;
        costCoins.GetComponent<Image>().color = costCoins.GetComponent<Dropdown>().colors.normalColor;
    }

    public void OnClickTo_CHONG_ZHI()
    {
        CZPanel.SetActive(true);
        tipWindow.SetActive(false);
        Time.timeScale = 0;
    }

    public void OnClickToCloseCZPanel()
    {
        Time.timeScale = 1;
        CZPanel.SetActive(false);
    }

    public void UpdateCrashGems(int id, int count)
    {
        gemsCrashNote[id].GetComponentInChildren<Text>().text = count.ToString();
    }

    public void OnClickToClear()
    {
        isAutoClear.enabled = false;
        //if (GameState.instance.ownCoins - GameState.instance.costCoins < 0)
        //{
        //    Set_CHONG_ZHI_Tip(GameState.instance.costCoins - GameState.instance.ownCoins);
        //    return;
        //}
        clearButtonText.text = "消除中...";
        ClickButtonsStateSwitch(false);

        StartCoroutine(gameManager.AutoPlay(0f));
        StartCoroutine(ClearButtonEnable());
    }

    public void OnToggleToAutoClear()
    {
        if (isAutoClear.isOn)
        {
            AutoClearToggleEnableSwitch();
            costCoins.enabled = false;
            costCoins.GetComponent<Image>().color = costCoins.GetComponent<Dropdown>().colors.disabledColor;
            if (GameState.instance.ownCoins - GameState.instance.costCoins < 0)
            {
                Set_CHONG_ZHI_Tip(GameState.instance.costCoins - GameState.instance.ownCoins);
                //SceneManager.LoadScene("RealEnd");
                return;
            }
            clearButtonText.text = "正在自动进行...";
            ClickButtonsStateSwitch(false);

            gameManager.isAuto = true;
            gameManager.ResetPanel();
        }
        else
        {
            isAutoClear.enabled = false;
            clearButtonText.text = "正在切换中...";
            StartCoroutine(ClearButtonEnable());
            gameManager.isAuto = false;
        }
    }

    public void AutoClearToggleEnableSwitch()
    {
        isAutoClear.enabled = false;
        Timer.Register(2f, () => { isAutoClear.enabled = true; });
    }

    public void OnClickToSelectAwards()
    {
        SceneManager.LoadScene("End");
        Time.timeScale = 1;
    }

    IEnumerator ClearButtonEnable()
    {
       
        yield return new WaitForSeconds(1.5f);
        isAutoClear.enabled = true;
        clearButtonText.text = "消除";
        ClickButtonsStateSwitch(true);
        costCoins.enabled = true;
        costCoins.GetComponent<Image>().color = costCoins.GetComponent<Dropdown>().colors.normalColor;
    }

    public void OnSelectCoinsCost()
    {
        GetCostCoins(costCoins.value);
    }

    public void OnClickToAwardNotes()
    {
        awardPanel.gameObject.SetActive(true);
    }

    public void OnClickToExitGame()
    {
        SceneManager.LoadScene("RealEnd");
        GameState.instance.isRestart = true;
    }

    public void CreateCombosInfo(int gemId, int gemCount, long score, decimal rate)
    {
        GameObject combosInfo = Instantiate(combosPanel, combosCreatePos.transform);
        combosInfo.GetComponent<PopFadeEffect>().PopFade(gemId, gemCount, score);

        if (rate >= 2 && rate <= 8)
        {
            SetRateRankText("Great!", Color.green);
        }
        else if (rate >= 8 && rate <= 15)
        {
            SetRateRankText("Big Win!", Color.green);
        }
        else if (rate >= 15 && rate <= 30)
        {
            SetRateRankText("Mega Win!!", Color.red);
        }
        else if (rate >= 30)
        {
            SetRateRankText("SUPER WIN!!", Color.red);
        }
        else
        {
            SetRateRankText("normal", Color.gray);
        }
    }

    public void SetRateRankText(string text, Color color)
    {
        rateRank.text = text;
        rateRank.color = color;

        Sequence quence = DOTween.Sequence();
        quence.Append(rateRank.DOFade(1, 0));
        quence.Append(rateRank.DOFade(0, 2f));
    }

    void GetCostCoins(int value)
    {
        switch (value)
        {
            case 0: GameState.instance.costCoins = 10000; break;
            case 1: GameState.instance.costCoins = 100000; break;
            case 2: GameState.instance.costCoins = 1000000; break;
            default:
                break;
        }
        ClickButtonsStateSwitch(true);
        isAutoClear.enabled = true;
        //if (GameState.instance.ownCoins - GameState.instance.costCoins < 0)
        //{
        //    Set_CHONG_ZHI_Tip(GameState.instance.costCoins - GameState.instance.ownCoins);
        //}
        //else
        //{
            
        //}
    }

    public void ClickButtonsStateSwitch(bool isClick)
    {
        if (isClick)
        {
            clearButton.enabled = true;
            clearButtonText.text = "消除";
            clearButton.GetComponent<Image>().color = clearButton.colors.normalColor;
        }
        else
        {
            clearButton.enabled = false;
            clearButton.GetComponent<Image>().color = clearButton.colors.disabledColor;
        }
    }
}
