/*
플레이어 조작 & 이동 담당

이동
- TouchControll() : 사용자의 스와이프 방향에 따라 플레이어의 이동 방향 조정

충돌
- CollisionDetect() : 플레이어가 이동하는 동안 충돌 확인 (벽, 다른 색 타일)

색 변경
- ColorChange(playerLayer color) : 플레이어의 색 변경
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rigid;
    Vector2 First;
    Vector2 Last;
    Vector2 dir;
    public bool InputLock;
    public float speed;
    public enum playerLayer {Red,Green,Blue,Yellow};
    public playerLayer PL;
    public List<Sprite> colors;
    [SerializeField]
    private SpriteRenderer SR;
    [SerializeField]
    private AudioSource AS;
    [SerializeField]
    private List<AudioClip> AudioList;
    [SerializeField]
    private TrailRenderer TR;
    [SerializeField]
    private List<Color> colors_Trailrenderer;
    Vector2 AnimationDir;
    int Count =0;

    [Header("About Touch Input")]
    public float tapSpeed = 0.5f;
    private float lastTapTime = -10f;
    private int clickCount = 0;

    [Header("GameManager")]
    public GameManager gm;
    
    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        rigid = this.GetComponent<Rigidbody2D>(); 
        SR= this.GetComponent<SpriteRenderer>();
        TR= this.GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        TouchControll();
    }

    private void FixedUpdate()
    {
        if(InputLock)
        {
            CollisionDetect();
        }
    }
    
    // 플레이어의 충돌 확인
    void CollisionDetect()
    {
        int layerMask =0;
        switch(PL)
        {
            case playerLayer.Red:  layerMask = ((1 << LayerMask.NameToLayer("Ignore Raycast")) | (1 << LayerMask.NameToLayer("Red")));
                break;
            case playerLayer.Blue: layerMask = ((1 << LayerMask.NameToLayer("Ignore Raycast")) | (1 << LayerMask.NameToLayer("Blue")));
                break;
            case playerLayer.Green:  layerMask = ((1 << LayerMask.NameToLayer("Ignore Raycast")) | (1 << LayerMask.NameToLayer("Green")));
                break;    
            case playerLayer.Yellow:  layerMask = ((1 << LayerMask.NameToLayer("Ignore Raycast")) | (1 << LayerMask.NameToLayer("Yellow")));
                break;   
        }
        layerMask = ~layerMask;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir,0.56f,layerMask);
        if(hit.collider!=null)
        {
            Debug.Log("Is Detected "+hit.collider.name);
            AnimationDir = rigid.velocity;
            Shake();
            
            rigid.velocity = Vector2.zero;

            AS.clip = AudioList[0];
            if(DataController.Instance.gameData.isEffectSoundOn == true) {
                AS.Play();
            }
            InputLock= false;
            Vector2 temp = transform.position;
            if((int)temp.x - 0.5f >= temp.x)
            {
                temp.x = (int)temp.x-1;
            }
            else if(temp.x >= (int)temp.x+0.5f)
            {
                temp.x = (int)temp.x+1;
            }
            else
                temp.x = (int)temp.x;
            if((int)temp.y - 0.5f >= temp.y)
            {
                temp.y = (int)temp.y-1;
            }
            else if(temp.y >= (int)temp.y+0.5f)
            {
                temp.y = (int)temp.y+1;
            }
            else
                temp.y = (int)temp.y;
                
            transform.position = new Vector3(temp.x,temp.y,0);

            if(gm.isInfiniteMode == false) {
                gm.moveCnt--;
                gm.PrintMoveCnt();
                if(gm.moveCnt <= 0 && !gm.isArrived) {
                    Invoke("DelayResetStage", 0.2f);
                }
            }
            
            
        }
    }

    void DetectDoubleTouch() {
        if (Input.GetMouseButtonDown (0))   {
            lastTapTime = Time.time;
                    
            if ((Time.time - lastTapTime) < tapSpeed)   {
                clickCount += 1;
                        
                if(clickCount >= 2) {
                    Debug.Log("트리플탭!");
                    gm.SkipStage();
                    clickCount = 0;
                }
            }
        }
        if ((Time.time - lastTapTime) > tapSpeed)   {
            clickCount = 0;
        }
    }

    // 터치 조작 (스와이프 방향)에 따른 플레이어 이동 방향 조정
    void TouchControll()
    {
        if(!InputLock && !gm.isPlayerLock && !gm.isArrived)
        {
            
            if(Input.GetMouseButtonDown(0))
            {
                rigid.constraints = RigidbodyConstraints2D.None;
                First = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
            }
            else if(Input.GetMouseButtonUp(0))
            {            
            
                InputLock= true;
                Last = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
                dir = Last-First;
                if(Mathf.Abs(dir.x)>Mathf.Abs(dir.y))
                {
                    if(dir.x>0)
                    {
                        dir.y = 0;
                        dir.x = 1;
                    }
                    else
                    {
                        dir.y = 0;
                        dir.x = -1;
                    }
                    rigid.constraints = RigidbodyConstraints2D.FreezePositionY;
                }
                else
                {
                    if(dir.y>0) 
                    {
                        dir.x = 0;
                        dir.y = 1;
                    }
                    else
                    {
                        dir.x = 0;
                        dir.y = -1;
                    }                
                    rigid.constraints = RigidbodyConstraints2D.FreezePositionX;
                }
                
                rigid.velocity = dir*speed;
            }
         
        }
    }

    // 플레이어의 색 변경
    public void ColorChange(playerLayer color)
    {   
        if(Count==0)
        {
            AS.clip = AudioList[0];
            if(DataController.Instance.gameData.isEffectSoundOn == true) {
                AS.Play();
            }
        }
        else
        {
            AS.clip = AudioList[1];
            if(DataController.Instance.gameData.isEffectSoundOn == true) {
                AS.Play();
            }
        }
        Count++;
        switch(color)
        {
            case playerLayer.Red: 
                this.gameObject.layer = LayerMask.NameToLayer("Red");
                PL = playerLayer.Red;
                SR.sprite = colors[0];
                TR.startColor = colors_Trailrenderer[0];
            break;
            case playerLayer.Green: 
                this.gameObject.layer = LayerMask.NameToLayer("Green");
                PL = playerLayer.Green;
                SR.sprite = colors[1];
                TR.startColor = colors_Trailrenderer[1];
            break;
            case playerLayer.Blue:
                this.gameObject.layer = LayerMask.NameToLayer("Blue");
                PL = playerLayer.Blue;
                SR.sprite = colors[2];
                TR.startColor = colors_Trailrenderer[2];
            break;
            case playerLayer.Yellow:
                this.gameObject.layer = LayerMask.NameToLayer("Yellow");
                PL = playerLayer.Yellow;
                SR.sprite = colors[3];
                TR.startColor = colors_Trailrenderer[3];
            break;        
        } 
    }

    // 카메라 흔들리는 연출
    private void Shake()
    {
        if(DataController.Instance.gameData.isCamShakeOn == true) {
            Camera.main.transform.Translate(-AnimationDir/(speed*20f));

            Invoke("ShakeReset",0.1f);
        }
        if(DataController.Instance.gameData.isVibrateOn == true) {
            Vibration.Vibrate((long)40);
        }
        
    }

    private void ShakeReset(){
        Vibration.Cancel();
        Camera.main.transform.Translate(AnimationDir/(speed*20f));
    }

    // 해당 스테이지 리셋
    private void DelayResetStage() {
        gm.ResetStage();
    }
}
