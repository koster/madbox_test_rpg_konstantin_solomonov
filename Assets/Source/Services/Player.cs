using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Player : GameService
{
    public Unit playerPrefab;
    public CinemachineVirtualCamera cinemachine;

    public List<Weapon> startingWeapons = new List<Weapon>();

    JoystickInput joystick;
    public Unit unit;
    Enemy attackTarget;

    List<Weapon> weaponsInventory = new List<Weapon>();
    int selectedWeapon;

    Vector3 lastPos;

    public override void Init()
    {
        lastPos = transform.position;
        
        unit = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        cinemachine.Follow = unit.transform;

        joystick = Main.Get<JoystickInput>();

        var range = Random.Range(0, startingWeapons.Count);
        var startingWeapon = startingWeapons[range];
        GiveWeapon(startingWeapon);
    }

    public Vector3 GetPosition()
    {
        return unit.transform.position;
    }

    public void GiveWeapon(Weapon to)
    {
        if (!weaponsInventory.Contains(to))
        {
            weaponsInventory.Add(to);

            ChangeWeapon(to);
            selectedWeapon = weaponsInventory.IndexOf(to);
        }
    }

    public void ChangeWeapon(Weapon to)
    {
        if (unit.IsEquipped(to))
            return;

        unit.Equip(to);
        Main.Get<GameEvents>().PlayerWeaponChanged?.Invoke(to);
    }
    
    void Update()
    {
        if (unit.IsValidTarget(unit.attackTarget))
            unit.facingPoint = unit.attackTarget.transform.position;
        else
            unit.facingPoint = null;

        if (!joystick.IsDown())
        {
            if (unit.IsValidTarget(unit.attackTarget))
                unit.Attacking();
            else
                unit.NotAttacking();
        }
        else
            unit.NotAttacking();

        unit.moveDirection = joystick.GetMoveVector();

        if (Vector3.Distance(lastPos, unit.transform.position) > 0.33f)
        {
            lastPos = unit.transform.position;
            Main.Get<GameEvents>().PlayerMoved?.Invoke(lastPos);
        }
    }

    public void NextWeapon()
    {
        selectedWeapon++;
        ChangeWeapon(weaponsInventory[selectedWeapon % weaponsInventory.Count]);
    }
}