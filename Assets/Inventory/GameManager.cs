using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Water2D;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public ItemManager itemManager;
    public UI_Manager ui_Manager;
    public CameraScript camerScript;
    public Obstructor playerObstructor;
    public DialougueManager DialougeManager;

    public Player player;

    private void Awake()
    {

        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);

        itemManager = GetComponent<ItemManager>();
        ui_Manager = GetComponent<UI_Manager>();
        DialougeManager = GetComponent<DialougueManager>();

        FindPlayerInScene();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayerInScene();
    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            FindPlayerInScene();
        }
    }

    void FindPlayerInScene()
    {
        player = FindObjectOfType<Player>();
    }

    public void RemovePlayerObstructor()
    {
        ObstructorManager.instance.RemoveObstructor(transform);
        if (playerObstructor.data != null && playerObstructor.data.child != null) Destroy(playerObstructor.data.child.gameObject);
        Destroy(playerObstructor);
    }
}