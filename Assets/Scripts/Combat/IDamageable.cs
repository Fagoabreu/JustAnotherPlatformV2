using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable:IHitable
{
    void TakeDamage(int damageAmount, float knockBackThrust);
}
