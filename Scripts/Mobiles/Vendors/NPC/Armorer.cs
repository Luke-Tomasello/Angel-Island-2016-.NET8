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

using System.Collections;

namespace Server.Mobiles
{
    public class Armorer : BaseVendor
    {
        private ArrayList m_SBInfos = new ArrayList();
        protected override ArrayList SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public Armorer()
            : base("the armorer")
        {
            SetSkill(SkillName.ArmsLore, 64.0, 100.0);
            SetSkill(SkillName.Blacksmith, 60.0, 83.0);
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBMetalShields());

            m_SBInfos.Add(new SBHelmetArmor());
            m_SBInfos.Add(new SBPlateArmor());
            m_SBInfos.Add(new SBChainmailArmor());
            m_SBInfos.Add(new SBRingmailArmor());
            m_SBInfos.Add(new SBStuddedArmor());
            m_SBInfos.Add(new SBLeatherArmor());
        }

        public override VendorShoeType ShoeType
        {
            get { return VendorShoeType.Boots; }
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            AddItem(new Server.Items.HalfApron(Utility.RandomYellowHue()));
            AddItem(new Server.Items.Bascinet());
        }

        public Armorer(Serial serial)
            : base(serial)
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
}