using ItemAPI;
using System;
using System.Collections.Generic;
using Gungeon;
using UnityEngine;

namespace An3sGunJam
{

    public class Module : ETGModule
    {
        public static readonly string MOD_NAME = "An3s's pirate submission";
        public static readonly string VERSION = "0.0.0";
        public static readonly string TEXT_COLOR = "#00FFFF";
        public static string ZipFilePath;
        public static string FilePath;
        public static List<string> items = new List<string>();

        public override void Start()
        {
            try
            {
                //audio
                ZipFilePath = this.Metadata.Archive;
                FilePath = this.Metadata.Directory;
                AudioResourceLoader.InitAudio();

                //basic inits
                ItemBuilder.Init();
                TreasureChest.Add();

                //synergies
                List<string> mandatoryConsoleIDs = new List<string>
                {
                    "ans:treasure_chest"
                };

                List<string> optionalConsoleIDs = new List<string>
                {
                    "shelleton_key",
                    "akey47",
                    "master_of_unlocking"
                };

                CustomSynergies.Add("oh you can open that!", mandatoryConsoleIDs, optionalConsoleIDs);
                

                List<string> mandatoryConsoleIDs2 = new List<string>
                {
                    "ans:treasure_chest"
                };

                List<string> optionalConsoleIDs2 = new List<string>
                {
                    "gold_junk",
                    "briefcase_of_cash",
                };

                CustomSynergies.Add("who even needs money.", mandatoryConsoleIDs2, optionalConsoleIDs2);


                List<string> mandatoryConsoleIDs3 = new List<string>
                {
                    "ans:treasure_chest"
                };

                List<string> optionalConsoleIDs3 = new List<string>
                {
                    "black_hole_gun",
                };

                CustomSynergies.Add("what could be in here?", mandatoryConsoleIDs3, optionalConsoleIDs3, false);

                //gets a list of all items that are in the game. for the who even needs money synergy.
                foreach (string text in Game.Items.AllIDs)
                {
                    bool flag = text.Contains("dupe") || text.Contains("|");
                    if (!flag)
                    {
                        PickupObject pickupObject = Game.Items[text];
                        bool flag2 = pickupObject == null;
                        if (!flag2)
                        {
                            items.Add(ETGMod.RemovePrefix(text, "gungeon:"));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Toolbox.Log(e);
            }
            Toolbox.Log(MOD_NAME + "v" + VERSION + "started successfully.", TEXT_COLOR);
        }
        public override void Exit() { }
        public override void Init() { }       
    }

}


