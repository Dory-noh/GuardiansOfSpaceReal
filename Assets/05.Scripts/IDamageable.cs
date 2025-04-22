using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable 
{
    //�������� ���� �� �ִ� Ÿ�Ե��� �ش� �������̽��� ��ӹް� OnDamage �޼��带 �ݵ�� �����ؾ� �Ѵ�.
    //OnDamage �޼���� �Է����� ������ ũ��(Damage), ���� ����(hitPoint), ���� ǥ���� ����(hitNormal)�� �޴´�.
    void OnDamage(float damage);
}
