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

//  /Scripts/Mobiles/Vendors/NPC/Weaponsmith.cs
//	CHANGE LOG
//  03/29/2004 - Pulse
//		Removed ability for this NPC vendor to support Bulk Order Deeds
//		Weaponsmiths will no longer issue or accept these deeds.

using Server.Engines.BulkOrders;
using System.Collections;

namespace Server.Mobiles
{
    public class Weaponsmith : BaseVendor
    {
        private ArrayList m_SBInfos = new ArrayList();
        protected override ArrayList SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public Weaponsmith()
            : base("the weaponsmith")
        {
            SetSkill(SkillName.ArmsLore, 64.0, 100.0);
            SetSkill(SkillName.Blacksmith, 65.0, 88.0);
            SetSkill(SkillName.Fencing, 45.0, 68.0);
            SetSkill(SkillName.Macing, 45.0, 68.0);
            SetSkill(SkillName.Swords, 45.0, 68.0);
            SetSkill(SkillName.Tactics, 36.0, 68.0);
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBAxeWeapon());
            m_SBInfos.Add(new SBKnifeWeapon());
            m_SBInfos.Add(new SBMaceWeapon());
            m_SBInfos.Add(new SBPoleArmWeapon());
            m_SBInfos.Add(new SBSpearForkWeapon());
            m_SBInfos.Add(new SBSwordWeapon());
        }

        public override VendorShoeType ShoeType
        {
            get { return Utility.RandomBool() ? VendorShoeType.Boots : VendorShoeType.ThighBoots; }
        }

        public override int GetShoeHue()
        {
            return 0;
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            AddItem(new Server.Items.HalfApron());
        }

        #region Bulk Orders
        public override Item CreateBulkOrder(Mobile from, bool fromContextMenu)
        {
            PlayerMobile pm = from as PlayerMobile;

            if (pm != null && pm.NextSmithBulkOrder == TimeSpan.Zero && (fromContextMenu || 0.2 > Utility.RandomDouble()))
            {
                double theirSkill = pm.Skills[SkillName.Blacksmith].Base;

                if (theirSkill >= 70.1)
                    pm.NextSmithBulkOrder = TimeSpan.FromHours(6.0);
                else if (theirSkill >= 50.1)
                    pm.NextSmithBulkOrder = TimeSpan.FromHours(2.0);
                else
                    pm.NextSmithBulkOrder = TimeSpan.FromHours(1.0);

                if (theirSkill >= 70.1 && ((theirSkill - 40.0) / 300.0) > Utility.RandomDouble())
                    return new LargeSmithBOD();

                return SmallSmithBOD.CreateRandomFor(from);
            }

            return null;
        }

        public override bool IsValidBulkOrder(Item item)
        {
            return (item is SmallSmithBOD || item is LargeSmithBOD);
        }

        public override bool SupportsBulkOrders(Mobile from)
        {
            // The following line allows this NPC to support the BOD system under AOS only. 
            //return ( from is PlayerMobile && Core.AOS && from.Skills[SkillName.Blacksmith].Base > 0 );
            // return false from this function to disable BOD support fully.
            return false;
        }

        public override TimeSpan GetNextBulkOrder(Mobile from)
        {
            if (from is PlayerMobile)
                return ((PlayerMobile)from).NextSmithBulkOrder;

            return TimeSpan.Zero;
        }
        #endregion

        public Weaponsmith(Serial serial)
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