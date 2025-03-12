/*
도착지점 프리펩에 존재

플레이어가 도착점 도착 시 다음 맵 생성
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class Goal : MonoBehaviour
{
    public GameObject UI;
    public GameObject MapGeneratomgParents;
    public GameObject MapGenerater;
    [SerializeField]
    private AudioSource AS;
    public GameManager gm;

    private void Awake()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Goal_Enter();
    }

    // 플레이어가 도착지점이 도달했을 때 Action
    private void Goal_Enter()
    {
        if(DataController.Instance.gameData.isEffectSoundOn == true) {
            AS.Play();
        }

        // 아케이드 모드이면 다음 스테이지 Unlock
        if(gm.isInfiniteMode == false) {
            DataController.Instance.gameData.isClear[gm.currentStage] = true;
            
            if(gm.currentStage < 999){
                DataController.Instance.gameData.isOpen[gm.currentStage+1] = true; // 해당 스테이지를 깼다면 다음 스테이지는 open이 되도록 한다.
            }
        }
        
        gm.OnClearStage();
        Invoke("NextMap" ,0.5f);
        
    }

    // 무한 모드 / 아케이드 모드에 따라서 다음 Action 실행
    private void NextMap()
    {
        // 무한 모드이면 새로운 미로 생성 요청
        if(gm.isInfiniteMode == true) {
            MapGenerater.GetComponent<ColorMazeGenerator>().Generate();
            MapGeneratomgParents.GetComponent<GenerateMapAndCharacter>().MapGenerate();
        }

        // 아케이드 모드이면 다음 스테이지 진행
        else {
            gm.ArcadeNextStage();
        }
        
    }
}
