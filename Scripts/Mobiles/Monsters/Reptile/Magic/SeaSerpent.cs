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

/* Scripts/Mobiles/Monsters/Reptile/Magic/SeaSerpent.cs
 * ChangeLog
 *	12/26/10, adam
 *		Add the missing fish steak
 *  8/16/06, Rhiannon
 *		Changed speed settings to match SpeedInfo table.
 *	7/26/05, erlein
 *		Automated removal of AoS resistance related function calls. 6 lines removed.
 *  11/10/04, Froste
 *      Removed PirateHat as loot, now restricted to "brethren only" drop
 *	7/21/04, mith
 *		Added PirateHat as loot, 5% drop.
 *  6/5/04, Pix
 *		Merged in 1.0RC0 code.
 */

using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a sea serpents corpse")]
    [TypeAlias("Server.Mobiles.Seaserpant")]
    public class SeaSerpent : BaseCreature
    {
        [Constructable]
        public SeaSerpent()
            : base(AIType.AI_Mage, FightMode.All | FightMode.Closest, 10, 1, 0.25, 0.5)
        {
            Name = "a sea serpent";
            Body = 150;
            BaseSoundID = 447;

            SetStr(168, 225);
            SetDex(58, 85);
            SetInt(53, 95);

            SetHits(110, 127);

            SetDamage(7, 13);

            SetSkill(SkillName.MagicResist, 60.1, 75.0);
            SetSkill(SkillName.Tactics, 60.1, 70.0);
            SetSkill(SkillName.Wrestling, 60.1, 70.0);

            Fame = 6000;
            Karma = -6000;

            VirtualArmor = 30;
            CanSwim = true;
            CantWalk = true;
        }

        public override bool HasBreath { get { return true; } }
        public override int TreasureMapLevel { get { return Core.UOAI || Core.UOAR ? 2 : 0; } }

        public override int Hides { get { return 10; } }
        public override HideType HideType { get { return HideType.Horned; } }
        public override int Scales { get { return (Core.UOAI || Core.UOAR || PublishInfo.PublishDate < Core.PlagueOfDespair) ? 0 : 8; } }
        public override ScaleType ScaleType { get { return ScaleType.Blue; } }
        public override int FishSteaks { get { return 1; } }

        public SeaSerpent(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            if (Core.UOAI || Core.UOAR)
            {
                //PackItem( new SpecialFishingNet() );
                PackGold(25, 50);
            }
            else
            {
                if (Core.UOSP || Core.UOMO)
                {   // http://web.archive.org/web/20021015003503/uo.stratics.com/hunters/seaserpent.shtml
                    // Special Fishing Net, 8 blue scales (carved), 1 Fish Steak (carved), 10 Horned Hides (carved)
                    if (Spawning)
                    {
                        PackGold(0);
                    }

                    // the fishingnet is dropped as part of the fishing system. not standard loot drop
                }
                else
                {
                    if (Spawning)
                    {
                        if (Utility.RandomBool())
                            PackItem(new SulfurousAsh(4));
                        else
                            PackItem(new BlackPearl(4));

                        PackItem(new RawFishSteak());

                        //PackItem( new SpecialFishingNet() );
                    }

                    AddLoot(LootPack.Meager);
                }
            }
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