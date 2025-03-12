/*
ColorChanger 프리펩에 존재

- 플레이어가 지나가면 플레이어 - ColorChhanger색 교환
- ColorChanger 뒤집히는 연출 출력
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ColorChanger : MonoBehaviour
{
    SpriteRenderer SR;
    public List<Sprite> Colors;
    public PlayerController.playerLayer color;

    private Vector2 dir;

    void Start()
    {
        SR=this.GetComponent<SpriteRenderer>();
        SR.sprite = Colors[(int)color];
    }

    // 플레이어 지나가면 ColorChanger 뒤집히는 연출 출력
    private void OnTriggerEnter2D(Collider2D other)
    {
        dir = (other.transform.position - transform.position).normalized;

        // 세로로 회전
        if(dir.x != 0.0f)
        {
            Debug.Log("옆에서 옴 rotation : " + transform.eulerAngles);
            if(transform.eulerAngles.y != 0 && transform.eulerAngles.y != 360)
            {
                transform.DORotateQuaternion(Quaternion.Euler(0, 0, 0), 0.5f);
            }
            else
            {
                transform.DORotateQuaternion(Quaternion.Euler(0, 180, 0), 0.5f);
            }
            
        }

        // 가로로 회전
        else
        {
            Debug.Log("위아래에서 옴 rotation : " + transform.eulerAngles);
            if(transform.eulerAngles.x != 0 && transform.eulerAngles.x != 360)
            {
                transform.DORotateQuaternion(Quaternion.Euler(0, 0, 0), 0.5f);
            }
            else
            {
                transform.DORotateQuaternion(Quaternion.Euler(180, 0, 0), 0.5f);
            }
        }
        

        // 플레이어의 색 변경
        PlayerController.playerLayer temp;
        PlayerController PC = other.GetComponent<PlayerController>();
        temp = PC.PL;
        PC.ColorChange(color);
        color = temp;
        SR.sprite = Colors[(int)color];
        other.transform.position = this.transform.position;
        
    }

    // ColorChanger 색 변경
    public void SetSprite(PlayerController.playerLayer temp){
        SR.sprite = Colors[(int)temp];
    }
}
