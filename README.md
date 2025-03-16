# WIP_Portfolio
유니티 2D 하이퍼캐주얼 퍼즐 게임 WIP 포트폴리오


# WIP

## 프로젝트 개요
<img src = "https://github.com/user-attachments/assets/951eeb26-45c9-4297-b22a-db9c07fb86e9" width = 30% height = 30%/>

<img src = "https://github.com/user-attachments/assets/a84bf6d2-7b73-4c11-b988-352984028d35" width = 30% height = 30%/>

<img src = "https://github.com/user-attachments/assets/ce03b775-00ee-44ab-b582-8a42ff98df40" width = 30% height = 30%/>


* 게임 장르: 하이퍼 캐주얼, 퍼즐

* 플랫폼 : 모바일

* 개발 환경 : Unity (2020.3.25f LTS)

* 기간 : 23.3 ~ 24.4

* 인원 : 3명
- 프로그래밍 : 1명 (본인)
- 기획 및 프로그래밍 : 1명
- 아트 : 1명

## 소개

Playworks 광고 영상 첨부

한 줄 소개
- "플레이어와 같은 색은 지나가고 다른 색은 부딪힌다"라는 성질을 이용해 플레이어의 색을 바꿔가며 퍼즐을 클리어하는 게임
- 이전 프로젝트 Pa!nt에서 조작이 편했으면 좋겠다는 유저 피드백을 받아 제작한 하이퍼 캐주얼 버전

특징 (각각 이미지 필요?)
- Inifinite 모드 : 게임오버가 될 때까지 무한 미로 탈출
- Arcade 모드 : 난이도에 따른 100개의 미로 플레이 가능
- 색각 이상자 (색약) 플레이어를 위한 색 보정 모드 지원

씬 구성 설명 필요?
  
담당 업무
- 표로 작성

스크립트 폴더 설명
- BackendServer
- LevelEditor
- LevelEditorManager
- Player
- PlayerData




## 담당 업무

|주요 기능|세부 사항|
|----------|----------------|
|Player Data 관리 및 서버 연동|인게임 데이터 Local Json 파일로 저장 (암호화)<br>스팀 계정 연동<br>유저 정보, 클리어 정보, 업적 달성 정보, 스테이지 플레이 로그, 커스텀 맵 데이터 등등 Read / Write|
|커스텀 레벨 에디터 제작 참여|레벨 데이터 파일 json 관리<br>CRUD|
|SDK 연결|Steamworks<br>Google Play Games (현재 사용 X)<br>뒤끝 서버 (게임서버 SaaS)|
|Player 조작감 개선|점프 버퍼 타임<br>코요테 타임|
|Scene 관리 및 유기적 연결|다중 씬이 열려 있을 때 예외 처리<br>인게임 내 카메라 전환 관리|
|최적화|Sprite Atlas<br>Addressable Asset System<br>카메라 및 스크립트 최적화|
|Post Processing을 이용한 흑백 연출||
|힌트 기능 제작||
|인트로 컷신 & 튜토리얼 제작||
|퍼즐 레벨 디자인 (44개)||
|UI / UX||


## 스크립트 폴더 설명
|폴더 명|설명|
|--|--|
|BackendServer|플레이 정보를 서버와 연동하기 위해 필요한 스크립트 모음|
|PlayerData|인 게임에서 플레이어와 관련된 모든 데이터 스크립트 모음|
|Player|플레이어와 관련된 모든 동작(조작, 이동, 상호작용)을 수행하는 스크립트|
|LevelEditor|직접 레벨 제작하는 툴을 개발할 때 필요한 스크립트 모음|
|LevelEditorManager|서버와 연동하여 유저들의 커스텀 레벨을 관리하는 스크립트 모음|




