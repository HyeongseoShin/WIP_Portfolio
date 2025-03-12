/*
실제 미로 생성 & 미로 생성 완료 후 Player 배치

- IEnumerator RealMapGenerate() : ColorMazeGenerator에서 생성된 map[,] 배열을 토대로 해당 위치에 실제 객체를 Instantiate
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMapAndCharacter : MonoBehaviour
{
    public ColorMazeGenerator Colormaze;
    [SerializeField]
    private List<GameObject> gameObjects;
    [SerializeField]
    private List<GameObject> ColorBlindGameObjects;
    public bool ColorBlind;
    
    GameObject temp;
    [SerializeField]
    private GameObject MapManager;
    public GameObject NowMapManager;

    public GameManager gm;

    private int i_width = Screen.width;
    private int i_height = Screen.height;

    private float mapGenWaitTime = 0.01f;

    public float playerSpeed {get; set;}

    [SerializeField]
    private DFSBFS dfs;
    
    void Start()
    {
        Debug.Log("화면 가로: " + i_width + " 세로: " + i_height);
        Time.fixedDeltaTime = 0.01f;
    }
    
    public void MapGenerate() {
        StartCoroutine(RealMapGenerate());
    }

    // ColorMazeGneerator를 통해 생성된 map 배열을 보고 실제 객체 배치
    public IEnumerator RealMapGenerate()
    {
        mapGenWaitTime = 0.01f - ((gm.increaseTime - 3.0f) / 0.5f) * 0.005f;
        if(mapGenWaitTime < 0.005f)
        {
            mapGenWaitTime = 0.0f;
        }

        float rePositionVal = 0f;
        if(DataController.Instance.gameData.currentStage+1 < 101)
        {
            rePositionVal = 0f;
            Debug.Log("Repos 0");
        }
        else if(DataController.Instance.gameData.currentStage+1 < 601)
        {
            rePositionVal = 1f;
            Debug.Log("Repos 1");
        }
        else
        {
            rePositionVal = 2f;
            Debug.Log("Repos 2");
        }

        Debug.Log("MapGenWaitTime : " + mapGenWaitTime);
        Camera.main.transform.position = new Vector3((Colormaze.width - 1) / 2.0f,(Colormaze.height - 1) / 2.0f + 0.5f,-10);
        Camera.main.orthographicSize = Colormaze.height / 2.0f + gm.increaseTime - 1.5f + (float)(i_height / i_width);

        if(NowMapManager!=null)
        {
            Destroy(NowMapManager);
        }

        NowMapManager = Instantiate(MapManager,new Vector3(0,0,0),Quaternion.Euler(0, 0, 0));
        for(int i = 0; i<Colormaze.width; i++)
        {
            for(int j = 0; j<Colormaze.height; j++)
            {
                if(DataController.Instance.gameData.isColorBlind)
                {
                    temp = Instantiate(ColorBlindGameObjects[Colormaze.map[i,j]],new Vector3(i,j-rePositionVal,0),Quaternion.Euler(0, 0, 0));
                }
                    
                else
                {
                    temp = Instantiate(gameObjects[Colormaze.map[i,j]],new Vector3(i,j-rePositionVal,0),Quaternion.Euler(0, 0, 0));
                }

                if(NowMapManager == null) {
                    Destroy(temp);
                }
                else {
                    temp.transform.SetParent(NowMapManager.transform);
                }
                
                if(Colormaze.map[i,j]==6)
                {
                    temp.GetComponent<Goal>().MapGeneratomgParents = this.gameObject;
                    temp.GetComponent<Goal>().MapGenerater = Colormaze.gameObject;
                }
                
            }

            yield return new WaitForSeconds(0.05f);            
        }
        
        if(DataController.Instance.gameData.isColorBlind)
        {
            temp =  Instantiate(ColorBlindGameObjects[5],new Vector3(1,1-rePositionVal,0), Quaternion.Euler(0, 0, 0));
        }
            
        else
        {
            temp = Instantiate(gameObjects[5],new Vector3(1,1-rePositionVal,0), Quaternion.Euler(0, 0, 0));
        }
            
        temp.transform.SetParent(NowMapManager.transform);
        temp.GetComponent<PlayerController>().speed = playerSpeed;

        switch(Colormaze.map[1,1])
        {
            case 0:
                temp.GetComponent<PlayerController>().ColorChange(PlayerController.playerLayer.Red);
                Debug.Log("width: " + Colormaze.width + " height: " + Colormaze.height);
            break;
            
            case 1:
                temp.GetComponent<PlayerController>().ColorChange(PlayerController.playerLayer.Green);
                Debug.Log("width: " + Colormaze.width + " height: " + Colormaze.height);
            break;
            
            case 2:
                temp.GetComponent<PlayerController>().ColorChange(PlayerController.playerLayer.Blue);
                Debug.Log("width: " + Colormaze.width + " height: " + Colormaze.height);
            break;
            
            case 3:
                temp.GetComponent<PlayerController>().ColorChange(PlayerController.playerLayer.Yellow);
                Debug.Log("width: " + Colormaze.width + " height: " + Colormaze.height);
            break;    

        }
        gm.isArrived = false;
        gm.isPlayerLock = false;
    }
}
