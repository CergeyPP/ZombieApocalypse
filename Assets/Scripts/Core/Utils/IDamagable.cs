using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IDamagable
{
    public event Action<float, float, GameObject> Damaged;
    public event Action<float, float, GameObject> Healed;
    public event Action<GameObject> Died;

    public void RegisterDamage(float dmg, GameObject causer);
    public void Heal(float heal, GameObject causer);
}

