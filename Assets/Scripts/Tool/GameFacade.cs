using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFacade : MonoBehaviour
{
    public static GameFacade Instance;
    
    // 保存用户名方便其他模块调用
    private string playerName;
    
    //工厂实例
    private IAssetFactory assetFactory;

    private PanelManager panelManager;

    private GameControler gameControler;

    public string PlayerName
    {
        get => playerName;
        set => playerName = value;
    }

    public PanelManager PanelManager
    {
        get => panelManager;
        set => panelManager = value;
    }

    public GameControler GameControler
    {
        get => gameControler;
        set => gameControler = value;
    }

    private void Awake()
    {
        var find = GameObject.Find("GameLoop");

        if (find == this.gameObject)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);//场景跳转之后不销毁该游戏物体
        }
        else
            Destroy(gameObject);
        
        assetFactory = new ResourceAssetProxy();
        panelManager = new PanelManager();
        gameControler = GameObject.Find("photograther").GetComponent<GameControler>();
    }

    #region 加载资源
    
    /// <summary>
    /// 获得游戏物体
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public GameObject LoadGameObject(string name)
    {
        return assetFactory.LoadGameObject(name);
    }

    /// <summary>
    /// 获得音效
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public AudioClip LoadAudioClip(string name)
    {
        return assetFactory.loadAudioClip(name);
    }
    
    public Sprite LoadSprite(string name)
    {
        return assetFactory.LoadSprite(name);
    }

    #endregion
    
}
