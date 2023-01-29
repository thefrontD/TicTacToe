using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable //선택 가능한 오브젝트들이 IAttackable을 갖는다
{
    void AttackedByPlayer(int damage, int attackCount);
    GameObject gameObject { get; }
}