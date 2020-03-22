using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndUIManager : MonoBehaviour {

    [SerializeField]
    Text totalScore;
    [SerializeField]
    GameObject clickToContinue;
	// Use this for initialization
	void Start () {
        totalScore.text = GameState.instance.totalScore.ToString();
        StartCoroutine(ClickToContinue());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClickToContinue()
    {
        SceneManager.LoadScene("Scene");
    }

    IEnumerator ClickToContinue()
    {
        yield return new WaitForSeconds(1f);
        clickToContinue.SetActive(true);
    }
}
