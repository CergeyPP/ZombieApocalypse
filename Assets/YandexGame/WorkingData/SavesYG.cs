
using System;
using System.Collections.Generic;

[Serializable]
public struct WeaponSave
{
    public int id;
    public int level;
}

namespace YG
{
    [System.Serializable]
    public class SavesYG
    {
        // "Технические сохранения" для работы плагина (Не удалять)
        public int idSave;
        public bool isFirstSession = true;
        public string language = "ru";
        public bool promptDone;

        //my saves
        public int coins;
        public List<WeaponSave> purchasedWeapons = new List<WeaponSave>();
        public int equippedWeaponID;
        public SavesYG()
        {
            coins = 0;
            equippedWeaponID = 0;
        }
    }
}
