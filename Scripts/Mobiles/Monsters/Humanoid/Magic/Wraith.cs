/***************************************************************************
 *
 *   RunUO                   : May 1, 2002
 *   portions copyright      : (C) The RunUO Software Team
 *   email                   : info@runuo.com
 *   
 *   Angel Island UO Shard   : March 25, 2004
 *   portions copyright      : (C) 2004-2024 Tomasello Software LLC.
 *   email                   : luke@tomasello.com
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

/* Scripts/Mobiles/Monsters/Humanoid/Magic/Wraith.cs
 * ChangeLog
 *  8/16/06, Rhiannon
 *		Changed speed settings to match SpeedInfo table.
 *	7/26/05, erlein
 *		Automated removal of AoS resistance related function calls. 5 lines removed.
 *	4/13/05, Kit
 *		Switch to new region specific loot model
 *	12/11/04, Pix
 *		Changed ControlSlots for IOBF.
 *  11/16/04, Froste
 *      Added IOBAlignment=IOBAlignment.Undead, added the random IOB drop to loot
 *  6/5/04, Pix
 *		Merged in 1.0RC0 code.
 */

using Server.Engines.IOBSystem;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a ghostly corpse")]
    public class Wraith : BaseCreature
    {
        [Constructable]
        public Wraith()
            : base(AIType.AI_Mage, FightMode.All | FightMode.Closest, 10, 1, 0.25, 0.5)
        {
            Name = "a wraith";
            Body = 26;
            Hue = 0x4001;
            BaseSoundID = 0x482;
            IOBAlignment = IOBAlignment.Undead;
            ControlSlots = 1;

            SetStr(60, 80);
            SetDex(76, 95);
            SetInt(36, 60);

            SetHits(46, 60);

            SetDamage(7, 11);

            SetSkill(SkillName.EvalInt, 30.1, 50.0);
            SetSkill(SkillName.Magery, 25.0, 35.0);
            SetSkill(SkillName.MagicResist, 25.0, 50.0);
            SetSkill(SkillName.Tactics, 45.1, 60.0);
            SetSkill(SkillName.Wrestling, 45.1, 55.0);

            Fame = 4000;
            Karma = -4000;
        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public Wraith(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            if (Core.RuleSets.AngelIslandRules() || Core.RuleSets.RenaissanceRules())
            {
                PackGold(25, 50);
                PackReg(10);
                // Froste: 12% random IOB drop
                if (0.12 > Utility.RandomDouble())
                {
                    Item iob = Loot.RandomIOB();
                    PackItem(iob);
                }

                if (IOBRegions.GetIOBStronghold(this) == IOBAlignment)
                {
                    // 30% boost to gold
                    PackGold(base.GetGold() / 3);
                }
            }
            else
            {
                if (Core.RuleSets.SiegeRules() || Core.RuleSets.MortalisRules())
                {   // http://web.archive.org/web/20021001072933/uo.stratics.com/hunters/wraith.shtml
                    // 50 to 150 Gold, Magic Items, Gems, Reagents, Scrolls, Bones

                    if (Spawning)
                    {
                        PackGold(50, 150);
                    }
                    else
                    {
                        if (Utility.RandomBool())       // TODO: no idea as to the level and rate
                            PackMagicEquipment(1, 1);
                        else
                            PackMagicItem(1, 1);

                        PackGem(1, .9);
                        PackGem(1, .05);

                        PackReg(10);                    // use 10 - from most recent stratics

                        PackScroll(1, 4);               // TODO: no idea as to the level and rate
                        PackScroll(1, 4, 0.1);

                        if (0.12 > Utility.RandomDouble())
                            switch (Utility.Random(4))
                            {
                                case 0: PackItem(new BonePile()); break;
                                case 1: PackItem(new BonePile()); break;
                                case 2: PackItem(new BonePile()); break;
                                case 3: PackItem(new BonePile()); break;
                            }
                    }
                }
                else
                {
                    if (Spawning)
                        PackReg(10);

                    AddLoot(LootPack.Meager);
                }
            }
        }

        public override OppositionGroup OppositionGroup
        {
            get { return OppositionGroup.FeyAndUndead; }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}