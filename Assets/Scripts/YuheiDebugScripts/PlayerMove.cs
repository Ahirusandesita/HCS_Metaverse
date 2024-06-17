using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    float speed = 10f;
    float maxspeed = 15f;
    public float plusSpeed = 1f;

    private bool jumpFlag = false;
    private float _jumpPower = 2f;
    float MaxJumpPower;
    [SerializeField] private float plusPower = 1f;
    [SerializeField] private float jumpTime = 2f;
    float useJumpTime;

    /// <summary>
    /// �P�񂾂�Unity�ŌĂ΂��
    /// </summary>
    void Start()
    {
        MaxJumpPower = _jumpPower;
        useJumpTime = jumpTime;
    }

    private void FixedUpdate()
    {
        //�ړ�����
        if (Input.GetKey(KeyCode.A))
        {
            this.transform.Translate(new Vector3(-speed, 0f, 0f));
        }
        else if (Input.GetKey(KeyCode.D))
        {
            this.transform.Translate(new Vector3(speed, 0f, 0f));
        }
    }

    private void Update()
    {
        //�W�����v�ł���悤�ɂȂ�
        if (Input.GetKeyDown(KeyCode.Space) && !jumpFlag)
        {
            jumpFlag = true;
        }

        //�W�����v���鏈��
        if (jumpFlag)
        {
            this.transform.Translate(new Vector3(0f, _jumpPower * Time.deltaTime, 0f));
            _jumpPower += plusPower / 10f;
            useJumpTime -= Time.deltaTime;
            if(useJumpTime < 0f)
            {
                useJumpTime = jumpTime;
                _jumpPower *= -1;
            }
        }
    }

    //���n������Ă΂��
    public void Tyakuti()
    {
        //�W�����v�p���[�𐳂̐��ɂ���
        if(_jumpPower < 0f)
        {
            jumpTime *= -1f;
        }

        //�W�����v�p���[������
        _jumpPower = MaxJumpPower;
    }
}
