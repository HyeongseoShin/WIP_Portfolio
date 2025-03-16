# WIP_Portfolio
유니티 2D 하이퍼캐주얼 퍼즐 게임 WIP 포트폴리오


# WIP

<br><br>

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


<br><br>


## 게임 소개


https://github.com/user-attachments/assets/6d1d2a11-9ab1-4927-95b5-aa462f33c3c3



<br><br>



- "플레이어와 같은 색은 지나가고 다른 색은 부딪힌다"라는 성질을 이용해 플레이어의 색을 바꿔가며 퍼즐을 클리어하는 게임
- 이전 프로젝트 Pa!nt에서 조작이 편했으면 좋겠다는 유저 피드백을 받아 제작한 하이퍼 캐주얼 버전

<br><br>


## 특징

<br><br>

### Infinite 모드


제한 시간이 종료될 때까지 생성되는 미로를 계속 클리어하는 모드


<img src = "https://github.com/user-attachments/assets/016a69e1-18ae-4a0b-a87e-37e845228a5b" width = 30% height = 30% />
<img src = "https://github.com/user-attachments/assets/c07666fd-416f-4622-bb46-34ff0c6ff452" width = 30% height = 30% />
<img src = "https://github.com/user-attachments/assets/ae49a6a1-34f1-4756-a9fd-a92b56fb349e" width = 30% height = 30% />

<br><br>

### Arcade 모드


난이도에 따른 1000개의 스테이지의 미로를 클리어하는 모드
각 스테이지마다 최대 이동 횟수를 넘으면 리셋


<img src = "https://github.com/user-attachments/assets/ee222667-91de-4caa-9e27-4e85deb59625" width = 30% height = 30% />

<img src = "https://github.com/user-attachments/assets/9956fb88-db81-4ed9-9bca-17a97468be6b" width = 30% height = 30% />

<br><br>

### 색 보정 모드 지원


#### 원본 vs 색 보정 모드


색약 플레이어를 위한 색 보정 모드 지원


<img src = "https://github.com/user-attachments/assets/59a59cab-4f87-48ed-ac99-62cbb477a92b" width = 30% height = 30% />

<img src = "https://github.com/user-attachments/assets/bb4fd6ab-cc83-43f1-962f-4544440409e1" width = 30% height = 30% />


<br><br>



## 씬 구성


하이퍼 캐주얼 장르 고려한 단일 씬 구조 설계


<img src="https://github.com/user-attachments/assets/26eaadc6-dc99-4fba-81df-49a4b3ef8ba4" width = 50% height = 50% />

<br><br>


  
## 담당 업무


|주요 기능|세부 사항|
|----------|----------------|
|미로 생성 알고리즘 제작 참여||
|게임 플레이 로직 제작||
|Player Data 관리|인게임 데이터 Json으로 Local에 저장|
|SDK 연결|GPGS <br> Google Admob <br> Unity Playworks|
|GPGS 계정 연동||
|Playable 광고 소스 제작|Unity Playworks 활용|
|UI/UX 제작||


<br><br>

## 스크립트 폴더 설명


|폴더 / 스크립트 명|설명|
|----------|----------------|
|MazeLogic|미로 제작과 관련된 스크립트 모음|
|GameLogic|게임 플레이 및 규칙과 관련된 스크립트 모음|
|GameManager.cs|전체 게임 흐름을 제어하는 게임 매니저|




