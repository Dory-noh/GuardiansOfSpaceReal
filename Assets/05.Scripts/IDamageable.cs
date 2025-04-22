using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable 
{
    //데미지를 입을 수 있는 타입들은 해당 인터페이스를 상속받고 OnDamage 메서드를 반드시 구현해야 한다.
    //OnDamage 메서드는 입력으로 데미지 크기(Damage), 맞은 지점(hitPoint), 맞은 표면의 방향(hitNormal)을 받는다.
    void OnDamage(float damage);
}
