using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddCoinsModel : MonoBehaviour {

    public int needMoney;
    public int getCoinsAmount;

    [SerializeField]
    GameObject confirmWindow;

    [SerializeField]
    Text getCoinsAmountText;
    [SerializeField]
    Text needMoneyText;

    GameObject _confirmWindow;

    private void OnEnable()
    {
        getCoinsAmountText.text = "获得\n" + getCoinsAmount.ToString() + "金币";
        needMoneyText.text = needMoney.ToString() + "元";
    }

    public void OnClickToGetCoins()
    {
        _confirmWindow = Instantiate(confirmWindow);
        _confirmWindow.transform.SetParent(GameObject.Find("UI").GetComponent<Transform>(), false);

        Text costInfo = _confirmWindow.transform.Find("costInfo").GetComponent<Text>();
        costInfo.text = needMoney.ToString() + "元 获得 " + getCoinsAmount.ToString() + "金币";

        Button[] buttons = _confirmWindow.GetComponentsInChildren<Button>();
        
        AddButtonEvent(buttons, _confirmWindow);
    }

    void AddButtonEvent(Button[] buttons, GameObject confrimWin)
    {
        buttons[0].onClick.AddListener(()=>
        {
            Debug.Log("获得" + getCoinsAmount + "金币");
            GameState.instance.ownCoins += getCoinsAmount;
            if (GameState.instance.ownCoins - GameState.instance.costCoins > 0)
            {
                GameObject.Find("GameManager").GetComponent<GameUIManager>().ClickButtonsStateSwitch(true);
                GameObject.Find("GameManager").GetComponent<GameUIManager>().isAutoClear.enabled = true;
            }
            DestroyImmediate(_confirmWindow);
        });

        buttons[1].onClick.AddListener(() =>
        {
            DestroyImmediate(_confirmWindow);
        });
    }
}
