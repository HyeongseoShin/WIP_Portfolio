/*
게임 매니저

- 게임 시작
- 게임 종료
- 스테이지 스킵
- 스테이지 리셋
- 부활

등등
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("About Stage")]
    public int currentStage = 1; // 현재 스테이지
    public Text currentStageText; // 현재 스테이지 표시 UI
    public int moveCnt = 0; // 가능 이동 횟수 (아케이드 모드)
    public Text currentMoveCntText; // 현재 남은 가능한 이동 횟수 (아케이드 모드)

    [Header("About Timer")]
    public CountDownTimer timer;
    public float increaseTime = 3.0f;
    public Text timeBonusText;
    public Text timerText;
    
    [Header("GenerateMapAndCharacter")]
    public GenerateMapAndCharacter gen_M_C;

    [Header("Panels")]
    public GameObject inGamePanel;
    public GameObject titlePanel;
    public GameObject resultPanel;
    public GameObject settingPanel;
    public GameObject selectPanel;
    public GameObject homeConfirmPanel;
    public GameObject keepGoingPanel;
    public GameObject arcadeSkipPanel;

    [Header("About Player")]
    public bool isPlayerLock = false;

    [Header("Buttons")]
    public Button skipBtn;
    public Button resetBtn;
    public Button arcadeResetBtn;

    public bool isMapMaking;

    [Header("Player")]
    public PlayerController player;
    public bool isArrived = false;

    [SerializeField]
    private int widthValue;

    [SerializeField]
    private int heightValue;

    [SerializeField]
    public MapInfoReader mapInfoReader;

    [SerializeField]
    public bool isInfiniteMode;

    [Header("About MoveCnt")]
    public Text moveMinusText;

    public bool isRetry = false;


    [SerializeField]
    private googleAdsInterstitial googleAds;
    

    public void Awake() {
        DataController.Instance.LoadGameData();
        DataController.Instance.gameData.isOpen[0] = true; // 항상 1 stage는 열려있어야 한다.
        DataController.Instance.gameData.countForAds = 0; // 껐다가 키면 무조건 ads count는 0이다.
    }

    private void Update()
    {
        SetSkipResetBtn();
    }

    // 무한 모드 게임 시작 세팅
    public void InitGame() {
        isRetry = false;
        isArrived = false;
        isPlayerLock = false;
        increaseTime = 3.0f;

        gen_M_C.Colormaze.width = 5; // 맵 크기 설정
        gen_M_C.Colormaze.height = 5; // 맵 크기 설정
        gen_M_C.playerSpeed = 10.0f;
        gen_M_C.Colormaze.Generate();
        gen_M_C.MapGenerate();
        timer.SetZero();
        timer.SetPlayTime();
        currentStage = 1;
        currentStageText.text = "STAGE " + currentStage.ToString();
        skipBtn.gameObject.SetActive(true);
        resetBtn.gameObject.SetActive(true);
        arcadeResetBtn.gameObject.SetActive(false);
        timerText.gameObject.SetActive(true);
        currentMoveCntText.gameObject.SetActive(false);

    }

    // 아케이드 모드 게임 시작 세팅
    public void InitArcadeGame(int stage) {
        DataController.Instance.gameData.currentStage = stage;
        Debug.Log("첫 스테이지: " + stage);
        isArrived = false;
        isPlayerLock = false;

        widthValue = mapInfoReader.mapData.arcadeMaps[stage].width;
        heightValue = mapInfoReader.mapData.arcadeMaps[stage].height;

        gen_M_C.playerSpeed = 10.0f;

        increaseTime = (widthValue - 5) * 0.25f + 3.0f;

        mapInfoReader.LoadStoredMap(stage);

        skipBtn.gameObject.SetActive(true);
        resetBtn.gameObject.SetActive(true);

        timerText.gameObject.SetActive(false);
        currentMoveCntText.gameObject.SetActive(true);

        timer.SetZero();

        currentStage = stage;
        currentStageText.text = "STAGE " + (currentStage + 1).ToString();

        moveCnt = mapInfoReader.mapData.arcadeMaps[stage].arriveCnt;
        currentMoveCntText.text = "Move: " + moveCnt.ToString();
    }

    // 아케이드 모드 다음 스테이지 로드
    public void ArcadeNextStage() {
        if(currentStage < 1000) {
            if(DataController.Instance.gameData.countForAds > 9){
                ShowAds();
            }
            widthValue = mapInfoReader.mapData.arcadeMaps[currentStage].width;
            heightValue = mapInfoReader.mapData.arcadeMaps[currentStage].height;

            increaseTime = (widthValue - 5) * 0.25f + 3.0f;

            mapInfoReader.LoadStoredMap(currentStage);

            moveCnt = mapInfoReader.mapData.arcadeMaps[currentStage].arriveCnt;
            currentMoveCntText.text = "Move: " + moveCnt.ToString();
        }
        else {
            OnEndGame();
            selectPanel.SetActive(true);
            //titlePanel.SetActive(false);
        }
        
    }

    // 각 모드별 현재 스테이지 클리어했을 때의 Action
    public void OnClearStage() {

        currentStage++;
        DataController.Instance.gameData.currentStage = currentStage;
        isArrived = true;

        // 무한 모드이면 맵의 사이즈 늘리고, 플레이어 속도 증가
        if(isInfiniteMode) {
            if(currentStage % 5 == 0) {
                increaseTime += 0.25f;
                Debug.Log("시간 증가: " + increaseTime);
                gen_M_C.Colormaze.width += 1;
                gen_M_C.Colormaze.height += 2;
                gen_M_C.playerSpeed++;
            }
            timeBonusText.gameObject.SetActive(true);
            timeBonusText.text = "+ " + increaseTime.ToString() + "s";
            StartCoroutine(FadeOutTimeBonusText(new Color32(178, 224, 69, 225)));

            timer.AddTotalSeconds(increaseTime);
            timer.SetTimerPlaying(false); // 타이머 멈춤

            currentStageText.text = "STAGE " + currentStage.ToString();
        }

        else {
            if(DataController.Instance.gameData.currentStage > 30 ){
                DataController.Instance.gameData.countForAds++; // 30 이상 넘어갈 때부터 ads를 count 한다.
                Debug.Log("OnClear Stage countForAds: " + DataController.Instance.gameData.countForAds);
            }
            
            increaseTime = (widthValue - 5) * 0.25f + 3.0f;
            Debug.Log("아케이드 모드임 다음스테이지: " + currentStage);
            currentStageText.text = "STAGE " + (currentStage+1).ToString();
        }
        
        
        
    }

    // 스테이지 스킵 버튼 누른 후 광고 시청하고 난 후
    public void OnSkipArcadeStage()
    {       
        if(currentStage < 999){
            DataController.Instance.gameData.isOpen[currentStage+1] = true; // 해당 스테이지를 깼다면 다음 스테이지는 open이 되도록 한다.
        }

        DataController.Instance.SaveGameData();
        currentStage++;
        DataController.Instance.gameData.currentStage = currentStage;
        isArrived = true;
        increaseTime = (widthValue - 5) * 0.25f + 3.0f;
        Debug.Log("아케이드 모드임 다음스테이지: " + currentStage);
        currentStageText.text = "STAGE " + (currentStage+1).ToString();

        ArcadeNextStage();
    }

    // 더블 터치 시 스테이지 스킵 후 5초 마이너스
    public void SkipStage() {
        if(DataController.Instance.gameData.isInfiniteMode)
        {
            timeBonusText.gameObject.SetActive(true);
            timeBonusText.text = "- 5s";
            StartCoroutine(FadeOutTimeBonusText(new Color32(239, 108, 89, 255)));
            //StartCoroutine(FadeOutTimeBonusText(new Color32(0, 0, 0, 255)));
            //timeBonusText.gameObject.SetActive(false);

            timer.SetTimerPlaying(false); // 타이머 멈춤
            timer.AddTotalSeconds(-5.0f);

            //timer.SetTimerPlaying(true); // 타이머 작동
            gen_M_C.Colormaze.Generate();
            gen_M_C.MapGenerate();
            currentStageText.text = "STAGE " + currentStage.ToString();
        }
        else
        {
            arcadeSkipPanel.SetActive(true);
            OnSettingPanelOpen();
        }
        
    }

    // 각 모드별 게임 종료 시 Action
    public void OnEndGame() {
        Debug.Log("게임 오버");
        isPlayerLock = true;

        if(isInfiniteMode == true)
        {
            if(!isRetry)
            {
                timer.SetZero();
                keepGoingPanel.SetActive(true);
            }
            else
            {
                timer.SetZero();
                GoToResult();
            }
            
        }

        else
        {
            timer.SetZero();
            Destroy(gen_M_C.NowMapManager);
            inGamePanel.SetActive(false);
            Invoke("ShowAds", 0.2f);
        }
    }

    // 해당 스테이지 중도 종료
    public void QuitGame()
    {
        Debug.Log("게임 중 홈으로 돌아가기");
        isPlayerLock = true;
        timer.SetZero();
        Destroy(gen_M_C.NowMapManager);
        inGamePanel.SetActive(false);
        Invoke("ShowAds", 0.2f);
    }

    // 무한모드에서 제한 시간이 지나 게임이 종료된 경우 결과창 띄움
    public void GoToResult()
    {  
        keepGoingPanel.SetActive(false);
        Destroy(gen_M_C.NowMapManager);
        inGamePanel.SetActive(false);
        resultPanel.SetActive(true);
    
        if((currentStage - 1) > DataController.Instance.gameData.bestScoreStage) {
            resultPanel.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
            DataController.Instance.gameData.bestScoreStage = currentStage - 1;
            DataController.Instance.gameData.bestScorePlayTime = timer.GetPlayTime();
        }
        else {
            resultPanel.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
        }

        resultPanel.transform.GetChild(1).transform.GetComponent<Text>().text = (currentStage - 1).ToString() + " STAGES\n" + timer.GetPlayTime().ToString() + " s" ;
        resultPanel.transform.GetChild(2).transform.GetComponent<Text>().text = 
            "BEST\n" + DataController.Instance.gameData.bestScoreStage + " STAGES / " +
            DataController.Instance.gameData.bestScorePlayTime.ToString() + " s" ;
        
        DataController.Instance.SaveGameData();

        Invoke("ShowAds", 0.2f);
    }

    // 광고 보고 난 후 해야될 Action
    public void Revive()
    {
        // 부활
        isRetry = true;
        ResetStage();
        timer.AddTotalSeconds(60.0f);
        keepGoingPanel.SetActive(false);
        Invoke("SetFalseIsPlayerLock", 0.1f);
    }

    public void ShowAds(){
        if(isInfiniteMode || DataController.Instance.gameData.countForAds > 9){
            Debug.Log("ShowAds countForAds: " + DataController.Instance.gameData.countForAds);
            googleAds.ShowInterstitialAd();
        }
        
    }

    // 현재 스테이지 리셋
    public void ResetStage() {
        gen_M_C.MapGenerate();

        if(isInfiniteMode == false) { // 아케이드 모드
            Debug.Log("현재 스테이지: " + currentStage);
            if(DataController.Instance.gameData.currentStage >= 30){
                DataController.Instance.gameData.countForAds++; // 30 이상 넘어갈 때부터 ads를 count 한다.
            }
            if(DataController.Instance.gameData.countForAds > 9){
                Debug.Log("ResetStage countForAds: " + DataController.Instance.gameData.countForAds);
                ShowAds();
            }
            
            moveCnt = mapInfoReader.mapData.arcadeMaps[currentStage].arriveCnt;
            currentMoveCntText.text = "Move: " + moveCnt.ToString();
        }
        else
        {
            timer.SetTimerPlaying(false);
        }
    }

    public void OnSettingPanelOpen() {
        if(inGamePanel.activeSelf == true) {
            isPlayerLock = true;
            timer.SetTimerPlaying(false);
        }
    }
    
    public void OnSettingPanelClose() {
        if(inGamePanel.activeSelf == true && homeConfirmPanel.activeSelf == false) {
            timer.SetTimerPlaying(true);
            Invoke("SetFalseIsPlayerLock", 0.1f);
            Debug.Log("세팅 패널 꺼짐");
        }
    }

    public void OnHomeConfirmPanelClose() {
        if(inGamePanel.activeSelf == true && settingPanel.activeSelf == false) {
            timer.SetTimerPlaying(true);
            Invoke("SetFalseIsPlayerLock", 0.1f);
            Debug.Log("홈 컨펌 패널 꺼짐");
        }
    }


    public void SetFalseIsPlayerLock() {
        isPlayerLock = false;
    }
    

    public IEnumerator FadeOutTimeBonusText(Color32 tColor) {
        timeBonusText.color = tColor;
        while (timeBonusText.color.a > 0.0f)
        {
            timeBonusText.color = new Color(timeBonusText.color.r, timeBonusText.color.g, timeBonusText.color.b, timeBonusText.color.a - (Time.deltaTime / 2.0f));
            yield return null;
        }
    }

    public void SetSkipResetBtn() {
        if(isMapMaking || !timer.GetTimerPlaying()) {
            skipBtn.interactable = false;
            resetBtn.interactable = false;
            arcadeResetBtn.interactable = false;
        }
        else {
            skipBtn.interactable = true;
            resetBtn.interactable = true;
            arcadeResetBtn.interactable = true;
        }
    }

    public void SetGameMode(bool state) {
        isInfiniteMode = state;
        DataController.Instance.gameData.isInfiniteMode = state;
    }

    public void PrintMoveCnt() {
        currentMoveCntText.text = "Move: " + moveCnt.ToString();
        moveMinusText.gameObject.SetActive(true);
        moveMinusText.text = "- 1";
        StartCoroutine(FadeOutMoveMinusText(new Color32(239, 108, 89, 255)));
    }

    public IEnumerator FadeOutMoveMinusText(Color32 tColor) {
        moveMinusText.color = tColor;
        while (moveMinusText.color.a > 0.0f)
        {

            moveMinusText.color = new Color(moveMinusText.color.r, moveMinusText.color.g, moveMinusText.color.b, moveMinusText.color.a - (Time.deltaTime / 2.0f));
            yield return null; 
        }
    }


    public void GoHomeByMode() {
        if(isInfiniteMode) {
            titlePanel.SetActive(true);
        }
        else {
            selectPanel.SetActive(true);
        }
    }

    public void Allopen(int howmany){

        if(howmany == 0){
            for(int i = 0; i < 1000; i++) {
                DataController.Instance.gameData.isOpen[i] = true;
            }
        }else{
            for(int i = 0; i < 1000; i++) {
                if(i < howmany){
                    DataController.Instance.gameData.isOpen[i] = true;
                }else{
                    DataController.Instance.gameData.isOpen[i] = false;
                }
                
            }
        }
        
    }
}
