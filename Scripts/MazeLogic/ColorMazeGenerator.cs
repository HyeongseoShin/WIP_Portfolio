/*
미로 생성하는 스크립트

실제 객체 생성 X
배열에 값만 표시함

전체 과정
1. InitMoveCnt() - 미로 배열 초기화
2. Init() - 미로 테두리 부분 벽으로 초기화

3. GenerateRoad() - 미로 배열을 순회하며 여러 색으로 이루어진 영역들 배치
- 도착지점 표시
- 이전에 순회할 때의 색과 다른 색이 뽑힐 때까지 랜덤 색 뽑기
- tileCnt = 한 번에 칠해질 영역의 크기 랜덤 지정 (3 ~ 10)
- tileCnt만큼 진행 방향 랜덤 뽑기
    - 선택한 방향으로 갈 수 있다면
        - 해당 방향으로 이동 후 해당 위치를 현재 색으로 채움
    - 선택한 방향으로 갈 수 없다면
        - 이전 방향과 반대 방향으로 다시 탐색
- 미로를 전부 채웠으면 종료

- 만약 채워진 미로 중 접근할 수 없는 곳이 있다면 (주변 색과 다른 하나의 타일이 존재한다면)
    - 해당 타일을 주변 인접한 타일 색상으로 변경

4. SetColorChangerRoad() - 채워진 미로를 순회하며 도착지점에 도달할 수 있도록 지나면 플레이어의 색을 바뀌는 ColorChanger 경로에 배치
- 랜덤 진행 방향 뽑기
- 시작 위치 & 색 지정
- checkMap에 원본 map 복사
- 도착 지점에 도달할 수 있을 때까지 ColorChanger 배치
    - 다음 위치가 도착지점이면 탐색 종료
    - 다음 위치가 벽이면 방향 다시 뽑기
    - 다음 위치가 다른 색 타일이면 이동 후 ColorChanger 배치
    - 다음 위치가 ColorChanger 이면 이동 후 현재 색 ColorChanger 배치
    - 다음 위치가 같은 색 타일이면 이동

- ColorChanger 배치 후 checkMap을 map에 복사해 결과값 저장
- 추후 GenerateMapAndCharacter.cs에서 map 배열을 토대로 실제 객체 배치
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
enum TILE {REDROAD = 0, GREENROAD, BLUEROAD, YELLOWROAD, WALL, START, END, COLORCHANGER, REDCHANGER, GREENCHANGER, BLUECHANGER, YELLOWCHANGER};

public class ColorMazeGenerator : MonoBehaviour
{
    [SerializeField] public int width;
    [SerializeField] public int height;
    public int[,] map = {}; // 전체 맵의 좌표
    public int[,] checkMap = {}; // 만들어진 전체 맵에 대한 검사시 실행

    private Vector2Int[] direction = { Vector2Int.up, Vector2Int.left, Vector2Int.up, Vector2Int.right,  Vector2Int.right, Vector2Int.down};
    private List<Vector2Int> directionSet = new List<Vector2Int>();
    private Vector2Int pos = new Vector2Int(1, 1); // 시작점 위치
    private Stack<Vector2Int> stackDir = new Stack<Vector2Int>(); //지나온 길의 방향 저장

    [SerializeField] private int moveCnt;
    [SerializeField] private int currentColor; // 현재 색상

    [SerializeField] private int PercentageOfUpRight;
    [SerializeField] private int PercentageOfDownLeft;    
    public int GeneratedMapCount=0;

    private int New_DirectionCount = 0;
    public List<Vector2Int> New_direction;
    Vector2Int NowPosition =new Vector2Int(1,1);

    int cnt = 0;
    int cnt2 = 0;
    int count = 0;

    // 랜덤 미로 생성 시작
    public void Generate()
    {
        Init(); //미로 초기화
        GenerateRoad();        
    }

    // 미로 배열 초기화
    public void InitMoveCnt() {
        stackDir = new Stack<Vector2Int>(); //지나온 길의 방향 저장
        
        map = new int[width, height];
        checkMap = new int[width, height];
        for(int i = 0; i < width; i++) {
            for(int j = 0; j < height; j++) {
                map[i,j] = -1;
            }
        }
    }


    // 미로 테두리 부분 벽으로 초기화
    private void Init()
    {
        InitMoveCnt();
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(x == 0 || y == 0 || x == width - 1 || y == height - 1)
                {
                    map[x, y] = (int)TILE.WALL; //모든 타일을 벽으로 채움
                }    
            }
        }
    }

    // 도착 지점 먼저 지정 후 무작위 색으로 이루어진 랜덤 미로 생성
    private void GenerateRoad() {
        pos = new Vector2Int(width - 2, height - 2);
        map[pos.x, pos.y] = (int)TILE.END; // 도착지점

        int pastTileColor = -1;

        do {
            moveCnt = 0;
            int tileColor = Random.Range(0, 4); // 타일의 색 랜덤 뽑기
            if(pastTileColor == tileColor) { // 만일 이전 타일의 색과 같다면
                while(pastTileColor == tileColor) { // 다른 색이 뽑힐 때까지 뽑기
                    tileColor = Random.Range(0, 4);
                }
            }

            pastTileColor = tileColor;

            int tileCnt = Random.Range(3, 10); // 한 번에 칠해질 영역의 크기 지정 (3 ~ 10)
            
            for(int i = 0; i < tileCnt; i++) {
                RandDirection(); // 상, 하, 좌, 우 중 랜덤한 방향 뽑기
                int index = -1;
                for (int j = 0; j < direction.Length; j++) {
                    if (CheckForRoad(j)) { // 선택한 방향으로 나아갈 수 있는지 확인
                        index = j; //선택한 방향에 길이 없을 경우 방향 배열의 인덱스 저장
                        break;
                        
                    }
                }
                
                if(index != -1) { // 갈 수 있는 길이 있을 경우
                    stackDir.Push(direction[index]); // 스택에 방향 저장
                    pos += direction[index];
                    
                    map[pos.x, pos.y] = tileColor; // 해당 타일을 현재 색으로 채움
                }
                
                
                else { // 갈 수 있는 길이 없다면
                    if(stackDir.Count==0)
                        break;
                    pos += stackDir.Pop() * -1; // 이전 방향과 반대 방향으로 다시 탐색
                }

            }

            // 미로가 모두 채워졌는지 확인
            for(int i = 1; i < width - 1; i++) {
                for(int j = 1; j < height - 1; j++) {
                    if(map[i,j] != -1) {
                        moveCnt++;
                    }
                }
            }

            // 미로가 다 채워졌으면 끝
            if(moveCnt == (width - 2) * (height - 2)) {
                break;
            }
            
        } while(stackDir.Count!=0);
        
        // 만약 채워진 맵 중 갈 수 없는 곳이 있다면 (주변과 색이 다른 하나의 타일이 존재한다면)
        // 해당 타일을 주변 인접한 타일 색상으로 변경시킴
        for(int x = 1; x < width - 1; x++) {
            for(int y = 1; y < height - 1; y++) {
                if(map[x,y] != map[x, y+1] && map[x,y] != map[x+1, y] && map[x,y] != map[x, y-1] && map[x,y] != map[x-1, y] && map[x, y] != (int)TILE.END) {
                                
                    var temp = map[x, y];
                    
                    int randDirection = Random.Range(0, direction.Length);
                    Vector2Int randPos = new Vector2Int(x,y);
                    
                    while(map[(randPos + direction[randDirection]).x, (randPos + direction[randDirection]).y] == (int)TILE.WALL
                    || map[(randPos + direction[randDirection]).x, (randPos + direction[randDirection]).y] == (int)TILE.END) {
                        randDirection = Random.Range(0, direction.Length);
                    }

                    map[x, y] = map[(randPos + direction[randDirection]).x, (randPos + direction[randDirection]).y];
                    
                }
            }
        }
        
        // ColorChanger 설치
        SetColorChangerRoad();
    }


    private void RandDirection() //무작위로 방향을 섞는 메소드
    {
        // 타일 생성 무한 순회를 막기 위해 상, 우 방향이 좌, 하 보다 더 많이 배치되어있음
        for (int i = 0; i < direction.Length; i++)
        {
            int randNum = Random.Range(0, direction.Length); //4방향 중 무작위로 방향 선택
            Vector2Int temp = direction[randNum]; //현재 인덱스에 해당하는 방향과 랜덤으로 선택한 방향을 서로 바꿈
            direction[randNum] = direction[i];
            direction[i] = temp;
        }
    }

    // 다음 타일로 갈 수 있는지 없는지 결과 return
    private bool CheckForRoad(int index)
    {
        if((pos + direction[index]).x > width - 2) {
            //Debug.Log("x 넘침");
            return false;
        }
        else if((pos + direction[index]).x < 1) {
            //Debug.Log("x 마이너스");
            return false;
        }
        else if((pos + direction[index]).y > height - 2) {
            //Debug.Log("y 넘침");
            return false;
        }
        else if((pos + direction[index]).y < 1) {
            //Debug.Log("y 모자람");
            return false;
        }
        else if((pos + direction[index]).x == width - 2 && (pos + direction[index]).y == height - 2) {
            //Debug.Log("도착점");
            return false;
        }
        
        else if (map[(pos + direction[index]).x, (pos + direction[index]).y] != -1) {
            //Debug.Log("x: " + (pos + direction[index]).x + " y: " + (pos + direction[index]).y);
            //Debug.Log("벽 아님" + " 현재위치: " + pos + " 방향: "+ direction[index]);
            return false;
        }
        
        
        return true;
    }

    // 완성된 미로를 순회하며 ColorChanger 배치
    private void SetColorChangerRoad(){
        New_direction.Clear();
        New_DirectionCount = 0;
        
        New_RandomGenerate_Direction(); // 랜덤 진행 방향 뽑기
        
        NowPosition = new Vector2Int(1,1); // 시작 위치 지정
        
        // 시작 위치의 색을 현재 색으로 탐색 시작
        currentColor = map[1,1];
        checkMap = new int[width, height]; 

        // checkMap에 원본 map 복사
        for(int i = 0; i < width; i++) {
            for(int j = 0; j < height; j++) 
            {
                var temp = map[i,j];
                checkMap[i,j] = temp;
            }
        }

        // 도착지점에 도달할 때까지 이동하며 ColorChanger 배치
        // 만들어진 맵이 정답을 찾는 데에 100회 이상의 움직임이 걸린다면 실패한 맵이라고 간주함
        cnt = 0;
        while(cnt <= 100)
        {
            cnt++;
            if(currentColor >= 8)
            {
                currentColor -= 8;
            }
            
            // 도착지점일 때
            if(map[NowPosition.x+New_direction[New_DirectionCount].x,NowPosition.y+New_direction[New_DirectionCount].y]==(int)TILE.END )
            { 
                goto BlockIsEnd;            
            }

            // 벽 (테두리)일 때
            else if(map[NowPosition.x+New_direction[New_DirectionCount].x,NowPosition.y+New_direction[New_DirectionCount].y]==(int)TILE.WALL)
            {
                New_DirectionCount_Increase(); // 방향 다시 뽑기                 
            }

            // 다른 색 타일 만났을 때 => ColorChanger 배치해야 함
            else if(map[NowPosition.x+New_direction[New_DirectionCount].x,NowPosition.y+New_direction[New_DirectionCount].y]!= currentColor&&map[NowPosition.x+New_direction[New_DirectionCount].x,NowPosition.y+New_direction[New_DirectionCount].y]<5&&currentColor!=5)
            {
                NowPosition += New_direction[New_DirectionCount]; // 그 방향으로 이동
                SetPosColorChanger(NowPosition.x,NowPosition.y); // ColorChanger 배치

                while(true)
                {
                    // 도착지점일 때
                    if(map[NowPosition.x+New_direction[New_DirectionCount].x,NowPosition.y+New_direction[New_DirectionCount].y]==(int)TILE.END )
                    {
                        goto BlockIsEnd;
                    }

                    // 벽(테두리일 때)
                    else if(map[NowPosition.x+New_direction[New_DirectionCount].x,NowPosition.y+New_direction[New_DirectionCount].y]==(int)TILE.WALL)
                    {
                        New_DirectionCount_Increase(); // 방향 다시 뽑기
                        break;
                    }

                    // 같은 색 타일일 때
                    else if(map[NowPosition.x+New_direction[New_DirectionCount].x,NowPosition.y+New_direction[New_DirectionCount].y]== currentColor && map[NowPosition.x+New_direction[New_DirectionCount].x,NowPosition.y+New_direction[New_DirectionCount].y]<5)
                    {
                        NowPosition += New_direction[New_DirectionCount]; // 계속 이동
                    }

                    // 다른 색 타일일 때
                    else if(map[NowPosition.x+New_direction[New_DirectionCount].x,NowPosition.y+New_direction[New_DirectionCount].y]<8)
                    {
                        New_DirectionCount_Increase(); // 방향 다시 뽑기
                        break;
                    }

                    // 다른 ColorChanger일 때
                    else if(map[NowPosition.x+New_direction[New_DirectionCount].x,NowPosition.y+New_direction[New_DirectionCount].y]>8)
                    {
                        NowPosition += New_direction[New_DirectionCount]; // 계속 이동
                        SetPosColorChanger(NowPosition.x,NowPosition.y); // ColorChanger 배치
                    }
                    
                    else break;
                }
                
            }

            // 같은 색 타일일 때
            else
            {
                while(true)
                {
                    // 도착지점일 때
                    if(map[NowPosition.x+New_direction[New_DirectionCount].x,NowPosition.y+New_direction[New_DirectionCount].y]==(int)TILE.END )
                    {
                        goto BlockIsEnd;
                    }
                    
                    // 벽 (테두리일 때)
                    else if(map[NowPosition.x+New_direction[New_DirectionCount].x,NowPosition.y+New_direction[New_DirectionCount].y]==(int)TILE.WALL)
                    {
                        New_DirectionCount_Increase(); // 방향 다시 뽑기
                        break;
                    }
                    
                    // 같은 색 타일일 때
                    else if(map[NowPosition.x+New_direction[New_DirectionCount].x,NowPosition.y+New_direction[New_DirectionCount].y]== currentColor && map[NowPosition.x+New_direction[New_DirectionCount].x,NowPosition.y+New_direction[New_DirectionCount].y]<5)
                    {
                        NowPosition += New_direction[New_DirectionCount]; // 계속 이동
                    }

                    // 다른 색 타일일 때
                    else if(map[NowPosition.x+New_direction[New_DirectionCount].x,NowPosition.y+New_direction[New_DirectionCount].y]<8)
                    {
                        New_DirectionCount_Increase(); // 방향 다시 뽑기
                        break;
                    }

                    // 다른 색 ColorChanger일 때
                    else if(map[NowPosition.x+New_direction[New_DirectionCount].x,NowPosition.y+New_direction[New_DirectionCount].y]>=  8)
                    {
                        NowPosition += New_direction[New_DirectionCount]; // 계속 이동
                        SetPosColorChanger(NowPosition.x,NowPosition.y); // ColorChanger 배치
                    }
                    
                    else break;
                }
            }
        }

        Debug.Log("1Generate is  Fail"); // 100번의 Limit 이내에 도착지점 못 찾으면 => 맵 생성 실패
        return ;

        BlockIsEnd:
        Debug.Log("1Generate Succes");
        GeneratedMapCount++;
        Debug.Log("Generated Map Count is:"+GeneratedMapCount);

        // 모든 확인이 끝난 checkMap의 결과를 다시 원래 map에 복사한다.
        for(int i = 0; i < width; i++) {
            for(int j = 0; j < height; j++) {
                if(map[i,j] >= (int)TILE.COLORCHANGER) {
                    map[i, j] = checkMap[i, j] + 8;
                    
                }
            }
        }

        return ;
    }


    // 랜덤한 방향 뽑아서 경로에 추가
    private void New_RandomGenerate_Direction()
    {
        int MaximumCount = PercentageOfDownLeft+PercentageOfUpRight;
        int rand = Random.Range(0,MaximumCount);
        if(rand<=(PercentageOfUpRight/2))
        {
            New_direction.Add(Vector2Int.up);
        }
        else if(rand<=PercentageOfUpRight)
        {
            New_direction.Add(Vector2Int.right);
        }
        else if( rand<= PercentageOfUpRight+PercentageOfDownLeft/2)
        {
            New_direction.Add(Vector2Int.left);
        }
        else
        {
            New_direction.Add(Vector2Int.down);
        }

    }

    // 방향 다시 뽑기
    private void New_DirectionCount_Increase()
    {
        New_RandomGenerate_Direction();
        New_DirectionCount++;
    }

    // ColorChanger 배치 위치 표시
    private void SetPosColorChanger(int x, int y) {
        var temp = map[x, y]; // 현재 색 저장

        if (temp >= 8)
        {
            temp -= 8;
        }
        
        currentColor += 8;
        map[x, y] = currentColor;
        
        Debug.Log("Current Color "+temp +"/"+"Post Color "+map[x,y]);
        
        currentColor = temp;        
    }
}