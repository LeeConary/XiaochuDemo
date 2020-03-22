using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour {

    public GameObject gemOrItem;
    public GameObject brick;
    public Sprite[] gemSprites;
    public Sprite[] itemSprites;
    [SerializeField]
    Sprite[] brickSprites;

    [Space]

    [SerializeField]
    int level1_bricks;
    [SerializeField]
    int level2_bricks;
    [SerializeField]
    int level3_bricks;
    [SerializeField]
    Transform[] bricksStartsPos;

    [Space]

    [SerializeField]
    float startPosX = 0;
    [SerializeField]
    float startPosY = 0;
    [SerializeField]
    float jianGe = 1f;
    [SerializeField]
    int clearCount = 4;

    [Space]

    public Vector2 grids = new Vector2(4, 4);

    List<Model> sames = new List<Model>();

    bool isClick = true;
    int itemCount = 0;
    public bool isAuto = false;
    public bool isEnd = false;

    public bool test = true;

    [Space]
    [SerializeField]
    float bombItemRate = 0.7f;
    [SerializeField]
    float specialSkillRate = 0.5f;

    GameUIManager gameUIManager;
    // Use this for initialization
    void Start () {
        Time.timeScale = 1f;
        GameState.instance.currentCombos = 0;
        GameState.instance.crashGems = new Dictionary<int, int>();
        gameUIManager = GetComponent<GameUIManager>();
        GameState.instance.gemSprites = gemSprites;

        if (GameState.instance.isRestart)
        {
            GameState.instance.totalScore = 0;
            GameState.instance.isRestart = false;
        }

        GameState.instance.GameObjPool.Clear();

        InitBricks();

        if (test)
        {
            SetGrid(grids.x, grids.y);
        }
    }
	
    /// <summary>
    /// 如果没在挂机，玩家可以自己操作
    /// </summary>
    private void FixedUpdate()
    {
        if (!isAuto)
        {
            ClickToClear();
        }
    }

    int specialSkillCount = 0;
    Model itemModel = null;

    /// <summary>
    /// 挂机协程，在每次填充网格后调用
    /// 前提是挂机模式开启
    /// </summary>
    /// <returns></returns>
    public IEnumerator AutoPlay(float duration)
    {
        yield return new WaitForSeconds(duration);
        int sum = 0;
        int constSum = Global.gems.Length;

        for (int i = 0, j = 0; j < Global.gems.GetLength(1) && i < Global.gems.GetLength(0); j++)
        {
            if (itemModel != null)
            {
                UpdateLevelInfo();
                ClearGems(itemModel.gameObject);
                Global.gems[itemModel.posRow, itemModel.posCol] = null;
                itemModel = null;
                break;
            }

            sum += 1;

            if (ClearGems(Global.gems[i, j].gameObject))
            {
                GameState.instance.currentCombos += 1;
                break;
            }
            if (j == Global.gems.GetLength(1) - 1)
            {
                i++;
                j = -1;
            }
        }
        if (sum >= constSum)
        {
            if (GameState.instance.ownCoins - GameState.instance.costCoins < 0)
            {
                Debug.Log("没钱了");
                gameUIManager.Set_CHONG_ZHI_Tip(GameState.instance.costCoins - GameState.instance.ownCoins);
                isAuto = false;
                //SceneManager.LoadScene("RealEnd");
                yield return null;
            }
            else
            {
                //发动特殊效果
                if (Random.Range(0, 100) < specialSkillRate * 100 && specialSkillCount < 3)
                {
                    SpecialSkill();
                }
                else
                {
                    //首先把前面的大奖记录下来
                    AddBigAward();
                    //没找到相同的消除，换
                    GameState.instance.ownCoins -= GameState.instance.costCoins;
                    gameUIManager.SetRateRankText("重置", Color.white);
                    GameState.instance.currentCombos = 0;
                    specialSkillCount = 0;

                    StartCoroutine(AnotherGame());
                    Timer.CancelAllRegisteredTimers();
                }
            }
        }
    }

    /// <summary>
    /// 特殊技能逻辑，发动后会随机选择4个宝石消除
    /// 并掉下来同色宝石
    /// </summary>
    Queue<Model> specialModels = new Queue<Model>();
    void SpecialSkill()
    {
        gameUIManager.SetRateRankText("发动特殊技能", Color.white);
        //GameState.instance.currentCombos = 0;
        sames.Clear();

        while(specialModels.Count < 4)
        {
            int ranRow = Random.Range(0, Global.gems.GetLength(0));
            int ranCol = Random.Range(0, Global.gems.GetLength(1));

            if (Global.gems[ranRow, ranCol] != null && !specialModels.Contains(Global.gems[ranRow, ranCol]))
            {
                specialModels.Enqueue(Global.gems[ranRow, ranCol].GetComponent<Model>());
                sames.Add(Global.gems[ranRow, ranCol]);
                Global.gems[ranRow, ranCol] = null;  
            }
        }
        specialSkillCount += 1;
        Timer.Register(0.7f, () => { ResetPanel(); });
    }

    /// <summary>
    /// 销毁宝石的逻辑
    /// </summary>
    /// <param name="ob"></param>
    /// <param name="isItem"></param>
    /// <returns></returns>
    bool ClearGems(GameObject ob)
    {
        //清空消除列表，用于之后可消除元素的销毁
        sames.Clear();

        if (ob.GetComponent<Model>())
        {
            Model item = ob.GetComponent<Model>();

            if (item.type == 2)
            {
                ob.GetComponent<GameModelsDestroyEffect>().FadeEffect();

                sames.Add(item);
                Global.gems[item.posRow, item.posCol] = null;
                TimerDestroyGems();

                Timer.Register(0.7f, () => { ResetPanel(); });
                return true;
            }

            //开始寻找与点击的宝石周围相同的宝石
            SearchSames(ob);

            //如果寻找到的宝石个数为4个及以上，就消除它们
            if (sames.Count >= clearCount)
            {
                for (int i = 0; i < sames.Count; i++)
                {
                    sames[i].gameObject.GetComponent<GameModelsDestroyEffect>().FadeEffect();
                    Global.gems[sames[i].posRow, sames[i].posCol] = null;
                }
                TimerDestroyGems();
                Timer.Register(0.7f, () => { ResetPanel(); });

                return true;
            }
            return false;
        }
        else return false;
    }

    /// <summary>
    /// 点击宝石触发消除效果
    /// 然而这个没什么卵用了
    /// </summary>
    void ClickToClear()
    {
        if (Input.GetMouseButtonDown(0) && isClick)
        {
            //禁用按键
            StartCoroutine(CanClick());
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D ray = Physics2D.Raycast(mousePos, Vector3.forward);

            if (ray.collider != null)
            {
                ClearGems(ray.collider.gameObject);
            }
        }
    }

    /// <summary>
    /// 消除后更新游戏状态
    /// </summary>
    public void ResetPanel()
    {
        Model[,] gems = Global.gems;
        
        for (int i = 0; i < gems.GetLength(1); i++)
        {
            int nullCount = 0;
            //使宝石下沉，并且更新状态矩阵
            //逐列进行扫描，最底下的不动，空缺以上的宝石下沉
            //同时更新状态矩阵
            #region 有用，但是有问题
            //Stack<Model> stack = new Stack<Model>();
            ////计算当列有几个空缺
            //for (int j = 0; j < gems.GetLength(0); j++)
            //{
            //    if (gems[j, i] == null) nullCount++;
            //}
            ////自上而下进行扫描，有空缺就跳过
            //for (int k = gems.GetLength(0) - 1; k > 0; k--)
            //{
            //    if (gems[k, i] != null && nullCount > 0)
            //    {
            //        //存在的宝石放入栈中
            //        stack.Push(gems[k, i]);
            //    }
            //    //else continue;
            //    else break;
            //}

            ////从栈里取出元素，并更新状态矩阵
            //while (stack.Count > 0)
            //{
            //    Model tempGem = stack.Pop();
            //    gems[tempGem.posRow, tempGem.posCol] = null;

            //    tempGem.position = new Vector2(tempGem.position.x - nullCount, tempGem.position.y);
            //    gems[tempGem.posRow, tempGem.posCol] = tempGem;
            //}
            #endregion

            int length = 1;
            while (length <= gems.GetLength(0))
            {
                nullCount = 0;
                for (int j = 0; j < length; j++)
                {
                    if (gems[j, i] == null)
                    {
                        nullCount += 1;
                        continue;
                    }
                    else
                    {
                        if (j == 0) continue;
                        else if (j > 0 && gems[j - 1, i] != null) continue;
                        else
                        {
                            gems[j, i].position = new Vector2(gems[j, i].position.x - nullCount, gems[j, i].position.y);
                            gems[j - nullCount, i] = gems[j, i];
                            
                            gems[j, i] = null;
                            break;
                        }
                    }
                }
                length += 1;
            }
            //补充宝石
            FillGems(nullCount, i); 
        }


        //如果处在挂机状态，再次执行自动消除逻辑
        if (isAuto)
        {
            StartCoroutine(AutoPlay(2f));
        }
    }

    /// <summary>
    /// 为每一列补充宝石
    /// </summary>
    /// <param name="nullCount"></param>
    /// <param name="index"></paramgh>
    void FillGems(int nullCount, int index)
    {
        Model[,] gems = Global.gems;
        Queue<Model> gemsQueue = new Queue<Model>();
        int count = 0;

        while (count  < nullCount)
        {
            GameObject tempGem;
            Model model;

            if (specialModels.Count > 0)
            {
                model = specialModels.Dequeue();
                model.gameObject.transform.position = new Vector2(startPosX + (jianGe * index), 5f + (jianGe * count));
            }
            else
            {
                if (GameState.instance.GameObjPool.Count > 0)
                {
                    model = GameState.instance.GameObjPool.Dequeue();
                    tempGem = model.gameObject;
                    tempGem.GetComponent<GameModelsDestroyEffect>().ResetState();
                    tempGem.transform.position = new Vector2(startPosX + (jianGe * index), 5f + (jianGe * count));
                }
                else
                {
                    tempGem = Instantiate(gemOrItem, new Vector2(startPosX + (jianGe * index), 5f + (jianGe * count)), Quaternion.identity);
                    model = tempGem.AddComponent<Model>();
                }
                
                if (Random.Range(0, 100) < bombItemRate * 100 && itemCount < 1)
                {
                    model.id = Random.Range(0, itemSprites.Length);
                    model.type = 2;
                    tempGem.GetComponent<SpriteRenderer>().sprite = itemSprites[model.id];
                    itemCount += 1;
                    itemModel = model;
                }
                else
                {
                    model.id = Random.Range(0, gemSprites.Length);
                    model.type = 1;
                    tempGem.GetComponent<SpriteRenderer>().sprite = gemSprites[model.id];
                }
            }
            #region 废代码
            //if (GameState.instance.GameObjPool.Count > 0)
            //{
            //    tempGem = GameState.instance.GameObjPool.Dequeue();
            //    //tempGem.SetActive(true);
            //    model = tempGem.GetComponent<Model>();
            //    tempGem.transform.position = new Vector2(startPosX + (jianGe * index), 5f + (jianGe * count));
            //    tempGem.GetComponent<GameModelsDestroyEffect>().ResetState();
            //}
            //else
            //{

            //}
            #endregion
            count += 1;
            gemsQueue.Enqueue(model);
        }

        for (int i = 0; i < gems.GetLength(0); i++)
        {
            if (gems[i, index] == null)
            {
                gems[i, index] = gemsQueue.Dequeue();
                gems[i, index].position = new Vector2(i, index);
            }
        }
    }

    /// <summary>
    /// 更新关卡信息
    /// </summary>
    void UpdateLevelInfo()
    {
        int currentLevel = GameState.instance.currentLevel;
        //GameState.instance.currentCombos = 0;

        if (GameState.instance.levelState[currentLevel % 3].Count > 0)
        {
            Destroy(GameState.instance.levelState[currentLevel % 3].Pop());
        }

        if (GameState.instance.levelState[currentLevel % 3].Count <= 0)
        {
            gameUIManager.isAutoClear.gameObject.SetActive(false);
            gameUIManager.clearButton.gameObject.SetActive(false);
            gameUIManager.PassBy.SetActive(true);
            gameUIManager.ClickButtonsStateSwitch(false);
            isAuto = false;
            GameState.instance.currentLevel += 1;
            gameUIManager.level.text = "第 " + (GameState.instance.currentLevel + 1).ToString() + " 关";

            //如果关卡到了第3关，就不再继续游戏
            if (GameState.instance.currentLevel % 3  == 0)
            {
                Debug.Log("跳转到抽卡");
                gameUIManager.goToSelectAward.SetActive(true);
                Time.timeScale = 0;
                //SceneManager.LoadScene("End");
            }

            Timer.Register(1f, () => {
                StartCoroutine(AnotherGame());
                if (gameUIManager.isAutoClear.isOn)
                {
                    gameUIManager.ClickButtonsStateSwitch(false);
                }
                else
                {
                    gameUIManager.ClickButtonsStateSwitch(true);
                }
                
            });

            Timer.Register(2f, () => {
                gameUIManager.PassBy.SetActive(false);
            });
        }
    }

    /// <summary>
    /// 搜索与点击宝石颜色相同的宝石
    /// </summary>
    /// <param name="rootGem"></param>
    void SearchSames(GameObject rootGem)
    {
        //初始化结点访问状态
        //确保每次遍历的时候都能访问到每个结点
        foreach (Model item in Global.gems)
        {
            if (item != null)
            {
                item.isAsked = false;
            }
        }
        if (rootGem.GetComponent<Model>())
        {
            Model root = rootGem.GetComponent<Model>();
            //需要销毁的宝石列表
            //每次点击都需要清空
            //标记已被访问的结点，避免重复
            root.isAsked = true;
            sames.Add(root);

            //寻找逻辑
            GetSames(root);
        }
    }

    /// <summary>
    /// 递归寻找上下左右的节点
    /// </summary>
    /// <param name="root"></param>
    void GetSames(Model root)
    {
        if (root == null)
        {
            return;
        }

        if (root.left != null)
        {
            Model left = root.left;
            if (left.id.Equals(root.id) && !root.left.isAsked && left.type == 1)
            {
                root.left.isAsked = true;
                sames.Add(left);
                GetSames(left);
            }
        }

        if (root.right != null)
        {
            Model right = root.right;
            if (right.id.Equals(root.id) && !root.right.isAsked && right.type == 1)
            {
                root.right.isAsked = true;
                sames.Add(right);
                GetSames(right);
            }
        }

        if (root.up != null)
        {
            Model up = root.up;
            if (up.id.Equals(root.id) && !root.up.isAsked && up.type == 1)
            {
                root.up.isAsked = true;
                sames.Add(up);
                GetSames(up);
            }
        }

        if (root.down != null)
        {
            Model down = root.down;
            if (down.id.Equals(root.id) && !root.down.isAsked && down.type == 1)
            {
                root.down.isAsked = true;
                sames.Add(down);
                GetSames(down);
            }
        }

    }

    /// <summary>
    /// 初始化游戏场景
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    IEnumerator InitGrids(int x, int y)
    {
        itemCount = 0;
        float rowPos = startPosX;
        float colPos = startPosY;
        int startMark = 0;
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                GameObject tempGem;
                Model model;
                if (GameState.instance.GameObjPool.Count >= 1)
                {
                    model = GameState.instance.GameObjPool.Dequeue();
                    tempGem = model.gameObject;
                    tempGem.transform.position = new Vector2(rowPos, colPos);
                    tempGem.GetComponent<GameModelsDestroyEffect>().ResetState(true);
                }
                else
                {
                    tempGem = Instantiate(gemOrItem, new Vector2(rowPos, colPos), Quaternion.identity);
                    model = tempGem.AddComponent<Model>();
                }
                model.type = 1;
                model.id = Random.Range(0, gemSprites.Length);
                tempGem.GetComponent<SpriteRenderer>().sprite = gemSprites[model.id];
                rowPos += jianGe;

                Global.gems[i, j] = model;
                Global.gems[i, j].position = new Vector2(i, j);
            }
            rowPos = startPosX;
            colPos += jianGe;
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.5f);
        gameUIManager.isAutoClear.gameObject.SetActive(true);
        gameUIManager.clearButton.gameObject.SetActive(true);

        if (gameUIManager.isAutoClear.isOn)
        {
            isAuto = true;
        }

        if (isAuto)
        {
            StartCoroutine(AutoPlay(2.5f));
        }
    }

    /// <summary>
    /// 初始化关卡进度
    /// 用砖块的数量来表示
    /// </summary>
    void InitBricks()
    {
        for (int i = 0; i < bricksStartsPos.Length; i++)
        {       
            if (GameState.instance.levelState.ContainsKey(i))
            {
                GameState.instance.levelState[i].Clear();
            }
            else
            {
                Stack<GameObject> stack = new Stack<GameObject>();
                GameState.instance.levelState.Add(i, stack);
            }
        }

        float jianGe = 0.28f;

        for (int j = 0; j < level1_bricks; j++)
        {
            Vector3 pos = new Vector3(bricksStartsPos[0].position.x, bricksStartsPos[0].position.y + jianGe * j, 0);
            GameObject obj = Instantiate(brick, pos, Quaternion.identity);
            brick.GetComponent<SpriteRenderer>().sprite = brickSprites[0];
            GameState.instance.levelState[0].Push(obj);
        }

        for (int j = 0; j < level2_bricks; j++)
        {
            Vector3 pos = new Vector3(bricksStartsPos[1].position.x, bricksStartsPos[1].position.y + jianGe * j, 0);
            GameObject obj = Instantiate(brick, pos, Quaternion.identity);
            brick.GetComponent<SpriteRenderer>().sprite = brickSprites[0];
            GameState.instance.levelState[1].Push(obj);
        }

        for (int j = 0; j < level3_bricks; j++)
        {
            Vector3 pos = new Vector3(bricksStartsPos[2].position.x + 0.3f * j, bricksStartsPos[2].position.y, 0);
            GameObject obj = Instantiate(brick, pos, Quaternion.Euler(0,0,90));
            brick.GetComponent<SpriteRenderer>().sprite = brickSprites[0];
            GameState.instance.levelState[2].Push(obj);
        }

    }

    /// <summary>
    /// 设置填充网格
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SetGrid(float x, float y)
    {
        gameUIManager.isAutoClear.gameObject.SetActive(false);
        gameUIManager.clearButton.gameObject.SetActive(false);
        int xCount = (int)x;
        int yCount = (int)y;
        Global.gems = new Model[xCount, yCount];

        StartCoroutine(InitGrids(xCount, yCount));
    }

    /// <summary>
    /// 借助定时器销毁宝石，保证能在消失效果播放完成后销毁
    /// 并且更新UI状态信息以及GameState数据
    /// </summary>
    void TimerDestroyGems(bool isSpecial = false)
    {
        //如果没有发动技能，就按常规更新
        if (!isSpecial)
        {
            //更改消除宝石字典，以便更新GameState的值
            if (sames[0].type != 2)
            {
                Model same = sames[0] as Model;
                int id = same.id;
                if (GameState.instance.crashGems.ContainsKey(id))
                {
                    GameState.instance.crashGems[same.id] += sames.Count;
                    gameUIManager.UpdateCrashGems(same.id, GameState.instance.crashGems[id]);
                }
                else
                {
                    GameState.instance.crashGems.Add(same.id, sames.Count);
                    gameUIManager.UpdateCrashGems(same.id, GameState.instance.crashGems[id]);
                }

                decimal rate = Global.ScoreRate(id, sames.Count);
                //这里设定的是每次消除都在押注数量的基础上进行增加
                decimal cost = GameState.instance.costCoins;
                int score = (int)(cost * rate);
                //计入获得的总分
                GameState.instance.totalScore += score;

                gameUIManager.CreateCombosInfo(id, sames.Count, score, rate);
                GameState.instance.ownCoins += score;

                //当前消除记录记录入大奖列表
                AwardNote awardNote = new AwardNote(GameState.instance.currentCombos, id, sames.Count, score, System.DateTime.Now);
                awards.Enqueue(awardNote);
            }

            foreach (var item in sames)
            {
                GameState.instance.GameObjPool.Enqueue(item);
            }
        }
    }


    Queue<AwardNote> awards = new Queue<AwardNote>();
    /// <summary>
    /// 将满足条件的大奖记录记录下来
    /// </summary>
    void AddBigAward()
    {
        List<AwardNote> tempList = new List<AwardNote>();
        while(awards.Count > 0)
        {
            AwardNote awardNote = awards.Dequeue();
            tempList.Add(awardNote);
        }

        int maxCombo = GameState.instance.currentCombos;
        int maxClearGemCount = 0;
        int maxGemId = -1;
        long scoreSum = 0;

        foreach (var item in tempList)
        {
            if (item.crashGemsCount > maxClearGemCount)
            {
                maxClearGemCount = item.crashGemsCount;
                maxGemId = item.crashGemsId;
            }
            scoreSum += item.awardScore;
        }

        AwardNote sumNote = new AwardNote(maxCombo, maxGemId ,maxClearGemCount, scoreSum, System.DateTime.Now);

        if (scoreSum > 100000)
        {
            if (GameState.instance.awardNotes.Count > 5)
            {
                GameState.instance.awardNotes.Dequeue();
            }
            GameState.instance.awardNotes.Enqueue(sumNote);
        }
        if (awards.Count > 0) awards.Clear();
    }

    /// <summary>
    /// 一旦挂机模式下没有宝石可以消除了，就重置网格
    /// </summary>
    /// <returns></returns>
    IEnumerator AnotherGame()
    {
        yield return new WaitForSeconds(0.5f);
        gameUIManager.AutoClearToggleEnableSwitch();
        foreach (var item in Global.gems)
        {
            //item.gameObject.SetActive(false);
            GameState.instance.GameObjPool.Enqueue(item);
            //Destroy(item.gameObject);
        }
        SetGrid(grids.x, grids.y);
    }

    /// <summary>
    /// 是否能点击宝石
    /// </summary>
    /// <returns></returns>
    IEnumerator CanClick()
    {
        isClick = false;
        yield return new WaitForSeconds(1.5f);
        isClick = true;
    }
}
