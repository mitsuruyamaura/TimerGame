using UnityEngine;

public class User {
    public int level;
    public bool tutorialFlag;

    // TODO ���ɂ��Ǘ����������[�U�[��񂪂���Βǉ�


    /// <summary>
    /// �V�K���[�U�[�̍쐬
    /// </summary>
    /// <returns></returns>
    public static User Create() {
        User user = new User {
            level = 0,
            tutorialFlag = false

            // TODO �ϐ���ǉ������ꍇ�ɂ́A���̕����ŕϐ��̏��������s��

        };

        Debug.Log("�V�K���[�U�[�̍쐬");

        return user;
    }
}
