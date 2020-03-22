using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour {

    #region 动态渲染？
    //public ScrollRect scroRect;
    //public VerticalLayoutGroup verticalGroup;
    //public GameObject prefabs;
    //public int maxCount = 200;

    //List<GameObject> list;
    //float scroRect_minHeight;
    //float scroRect_maxHeight;
    //void Start()
    //{
    //    //scroRect_minHeight = prefabs.GetComponent<LayoutElement>().preferredHeight;
    //    //scroRect_maxHeight = scroRect.transform.position.y + scroRect_minHeight;

    //    //list = new List<GameObject>();
    //    //Create();
    //    //StartCoroutine(IEShowUnit());
    //}

    //void Create()
    //{
    //    for (int i = 0; i < maxCount; i++)
    //    {
    //        var go = Instantiate(prefabs) as GameObject;
    //        go.SetActive(true);
    //        go.transform.position = Vector3.zero;
    //        go.transform.SetParent(verticalGroup.transform);
    //        go.name = "item" + i;

    //        list.Add(go);
    //    }
    //}

    //public void VCValueChanged()
    //{
    //    list.ForEach((p) => { CheckPos(p.transform); });
    //}

    //void CheckPos(Transform obj)
    //{
    //    var pos = obj.position;
    //    var go = obj.gameObject;
    //    if (pos.y >= -scroRect_minHeight && pos.y <= scroRect_maxHeight)
    //    {
    //        ShowItem(go);
    //    }
    //    else
    //    {
    //        HideItem(go);
    //    }
    //}

    //void ShowItem(GameObject go)
    //{
    //    go.SetActive(true);
    //}
    //void HideItem(GameObject go)
    //{
    //    go.SetActive(false);
    //}

    //IEnumerator IEShowUnit()
    //{
    //    yield return new WaitForSeconds(0.3f);
    //    VCValueChanged();
    //}
    #endregion

    [SerializeField]
    Text mousePosX;
    [SerializeField]
    Text mousePosY;
    [SerializeField]
    Image img;
    [SerializeField]
    GridLayoutGroup gridGroup;

    // Use this for initialization
    void Start () {
        Debug.Log(Screen.width + "  " + Screen.height);
        for (int i = 0; i < 50; i++)
        {
            GameObject go = Instantiate(img.gameObject);
            go.transform.SetParent(gridGroup.transform);
        }
	}

    // Update is called once per frame
    void Update () {
        Vector3 mousePos = Input.mousePosition;
        mousePosX.text = mousePos.x.ToString();
        mousePosY.text = mousePos.y.ToString();
	}

    public void OnValueChange()
    {
        Debug.Log("滑动中");
    }
}
