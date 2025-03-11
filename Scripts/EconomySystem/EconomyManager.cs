using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThiccTapeman.File;
using System;

namespace ThiccTapeman.Economy
{
    public class EconomyManager
    {
        private EconomyData economyData;

        private static EconomyManager instance;

        public static EconomyManager GetInstance()
        {
            if (instance == null)
            {
                instance = new EconomyManager();
                
                instance.Load();

                if(instance.economyData == null)
                {
                    instance.economyData = new EconomyData();
                }
            }

            return instance;
        }

        public void AddEconomy(Economy economy)
        {
            economyData.economies.Add(economy);
        }

        public void RemoveEconomy(Economy economy)
        {
            economyData.economies.Remove(economy);
        }

        public Economy GetEconomy(string name)
        {
            foreach (var eco in economyData.economies)
            {
                if (eco.name == name) return eco;
            }

            Economy economy = new Economy();
            economy.name = name;
            economy.value = 0;
            economy.tradeValue = 1;
            economy.growthRate = 1;
            economy.growthTime = 1;
            economy.growthAmount = 1;
            economy.growthAmountMultiplier = 1;

            AddEconomy(economy);

            return economy;
        }



        public static void Save()
        {
            EconomyManager economyManager = GetInstance();
            FileManager.SaveToFile(economyManager.economyData, "economy", Application.productName);
            Debug.Log("Saved");
            Debug.Log(economyManager.economyData.GetSaveableString());
        }

        public void Load()
        {
            economyData = FileManager.LoadFromFile<EconomyData>("economy", Application.productName);
        }
    }

    [System.Serializable]
    public class EconomyData : FileData
    {
        public List<Economy> economies = new List<Economy>();
        public override string GetSaveableString()
        {
            return JsonUtility.ToJson(this);
        }

        public override void Load()
        {

        }
    }

    [System.Serializable]
    public class Economy
    {
        public string name;
        public float value;

        public float growthRate;
        public float growthTime;
        public float growthAmount;
        public float growthAmountMultiplier;

        public float supply = 1000000;
        public float demand = 0;

        public float tradeValue;
        public float tradeValueMultiplier = 1000000000; 



        public Action OnValueChange;

        public void SetValue(float value)
        {
            this.value = value;

            float change = value - this.value;

            this.supply -= change;
            this.demand += change;

            UpdateEconomy();
            EconomyManager.Save();

            OnValueChange?.Invoke();
        }

        public void IncreaseValue(float value)
        {
            this.value += value;
            this.supply -= value;
            this.demand += value;

            UpdateEconomy();
            EconomyManager.Save();
            
            OnValueChange?.Invoke();
        }

        public void DecreaseValue(float value)
        {
            this.value -= value;
            this.supply += value;
            this.demand -= value;

            UpdateEconomy();
            EconomyManager.Save();

            OnValueChange?.Invoke();
        }

        public void UpdateEconomy()
        {
            if (demand > 0)
            {
                this.tradeValue = this.demand / this.supply * this.tradeValueMultiplier;
            }
            else this.tradeValue = 0;
        }

        public float GetValue()
        {
            return this.value;
        }
    }
}
