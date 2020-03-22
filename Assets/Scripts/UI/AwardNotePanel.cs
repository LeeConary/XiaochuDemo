using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AwardNotePanel : MonoBehaviour {

    [SerializeField]
    GameManager gameManager;
    [SerializeField]
    GameObject awardNote;
    [SerializeField]
    VerticalLayoutGroup group;

    List<GameObject> exists = new List<GameObject>();

    private void OnEnable()
    {
        Debug.Log("大奖记录面板显示");

        IEnumerator ie = GameState.instance.awardNotes.GetEnumerator();
        int index = 0;
        exists.Clear();
        while (ie.MoveNext())
        {
            AwardNote note = ie.Current as AwardNote;
            GameObject go = Instantiate(awardNote);
            Transform parent = go.transform;
            //if (!noteItem[index].activeSelf)
            //{
            //    noteItem[index].SetActive(true);
            //}
            //index += 1;
            //if (index > 4) index = 0;
            SetInfos(parent, note.awardCombo, note.awardScore, note.date.ToString(), note.crashGemsCount, note.crashGemsId);
            go.transform.SetParent(group.transform);
            exists.Add(go);
        }
    }

    void SetInfos(Transform parent, int noteCombos, long noteScore, string noteTime, int noteClearCount, int clearGemId)
    {
        Text combos = parent.Find("combos").gameObject.GetComponent<Text>();
        Text score = parent.Find("score").gameObject.GetComponent<Text>();
        Text time = parent.Find("time").gameObject.GetComponent<Text>();
        Text clearCount = parent.Find("clearCount/Text").gameObject.GetComponent<Text>();
        Image img = parent.Find("clearCount/Image").gameObject.GetComponent<Image>();

        combos.text = noteCombos.ToString();
        score.text = noteScore.ToString();
        time.text = noteTime;
        clearCount.text = noteClearCount.ToString();
        img.sprite = gameManager.gemSprites[clearGemId];
    }

    public void BeUnvisable()
    {
        gameObject.SetActive(false);
        foreach (var item in exists)
        {
            Destroy(item);
        }
    }
}
