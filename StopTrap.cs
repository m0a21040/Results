using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopTrap : MonoBehaviour
{
    /*���̃X�N���v�g�̓Q�[�����M�~�b�N�̃K���ɂȂ�܂�
      �K���ɐG����2�b�����Ȃ��Ȃ�A���̌�ʏ�����x�����x
    �@���b�㌳�̑��x�ɖ߂�
    */

    private CornMove_Ray main;
    private Cornove2 slow;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("coon"))
        {
            // �|�C���g
            // �R�[���̓������i��X�N���v�g���I�t�ɂ���B
            main = collision.gameObject.GetComponent<CornMove_Ray>();
            slow = collision.gameObject.GetComponent<Cornove2>();
            main.enabled = false;
            slow.enabled = false;

            // �|�C���g
            // �Q�b��Ƀ{�[����������悤�ɂ���B
            Invoke("Stopoff", 2.0f);
            Invoke("Slowoff", 10.0f);
        }
    }

    void Stopoff()
    {
        // �|�C���g
        // Ball�̓������i��X�N���v�g���I���ɂ���B
        slow.enabled = true;
    }

    void Slowoff()
    {
        // �|�C���g
        // �R�[���̓������i��X�N���v�g���I���ɂ���B
        slow.enabled = false;
        main.enabled = true;
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Gum"))
        {

            speed = 0;
            // �|�C���g
            // �Q�b��ɃR�[����������悤�ɂ���B
            Invoke("Stopoff", 2.0f);
            Invoke("Slowoff", 10.0f);
        }
    }*/
}

/*
 *  void Stopoff()
    {

        speed = 1;
        // �|�C���g
        // �R�[���̓������i��X�N���v�g���I���ɂ���B
        
    }

    void Slowoff()
    {
        speed = 3;
        // �|�C���g
        //�R�[���̓������i��X�N���v�g���I���ɂ���B
        
    }
*/
