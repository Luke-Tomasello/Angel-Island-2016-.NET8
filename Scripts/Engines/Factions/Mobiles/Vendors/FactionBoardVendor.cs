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

using Server.Items;
using Server.Mobiles;
using System.Collections;

namespace Server.Factions
{
    public class FactionBoardVendor : BaseFactionVendor
    {
        public FactionBoardVendor(Town town, Faction faction) : base(town, faction, "the LumberMan") // NOTE: title inconsistant, as OSI
        {
            SetSkill(SkillName.Carpentry, 85.0, 100.0);
            SetSkill(SkillName.Lumberjacking, 60.0, 83.0);
        }

        public override void InitSBInfo()
        {
            SBInfos.Add(new SBFactionBoard());
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            AddItem(new HalfApron());
        }

        public FactionBoardVendor(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class SBFactionBoard : SBInfo
    {
        private ArrayList m_BuyInfo = new InternalBuyInfo();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBFactionBoard()
        {
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override ArrayList BuyInfo { get { return m_BuyInfo; } }

        public class InternalBuyInfo : ArrayList
        {
            public InternalBuyInfo()
            {
                for (int i = 0; i < 5; ++i)
                    Add(new GenericBuyInfo(typeof(Board), 3, 20, 0x1BD7, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
            }
        }
    }
}