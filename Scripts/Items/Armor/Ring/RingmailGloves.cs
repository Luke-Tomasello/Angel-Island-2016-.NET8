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

/* Scripts/Items/Armor/Ring/RingmailGloves.cs
* ChangeLog
*	7/26/05, erlein
*		Automated removal of AoS resistance related function calls. 10 lines removed.
*	9/08/04, Pigpen
*		Add Arctic Storm Gloves variant of this Ring piece.
*/

using Server.Network;
using System.Collections;

namespace Server.Items
{
    [FlipableAttribute(0x13eb, 0x13f2)]
    public class RingmailGloves : BaseArmor
    {

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 50; } }

        public override int AosStrReq { get { return 40; } }
        public override int OldStrReq { get { return 20; } }

        public override int OldDexBonus { get { return -1; } }

        public override int ArmorBase { get { return 22; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Ringmail; } }

        [Constructable]
        public RingmailGloves()
            : base(0x13EB)
        {
            Weight = 2.0;
        }

        public RingmailGloves(Serial serial)
            : base(serial)
        {
        }

        // old name removed, see base class

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (Weight == 1.0)
                Weight = 2.0;
        }
    }
    public class ArcticStormGloves : BaseArmor
    {

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 50; } }

        public override int AosStrReq { get { return 40; } }
        public override int OldStrReq { get { return 20; } }

        public override int OldDexBonus { get { return -1; } }

        public override int ArmorBase { get { return 22; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Ringmail; } }

        [Constructable]
        public ArcticStormGloves()
            : base(0x13EB)
        {
            Weight = 2.0;
            Hue = 1364;
            ProtectionLevel = ArmorProtectionLevel.Guarding;
            Durability = ArmorDurabilityLevel.Massive;
            if (Utility.RandomDouble() < 0.20)
                Quality = ArmorQuality.Exceptional;
            Name = "Arctic Storm Gloves";
        }

        public ArcticStormGloves(Serial serial)
            : base(serial)
        {
        }

        // Special version that DOES NOT show armor attributes and tags
        public override void OnSingleClick(Mobile from)
        {
            ArrayList attrs = new ArrayList();

            int number;

            if (Name == null)
            {
                number = LabelNumber;
            }
            else
            {
                this.LabelTo(from, Name);
                number = 1041000;
            }

            if (attrs.Count == 0 && Crafter == null && Name != null)
                return;

            EquipmentInfo eqInfo = new EquipmentInfo(number, Crafter, false, (EquipInfoAttribute[])attrs.ToArray(typeof(EquipInfoAttribute)));

            from.Send(new DisplayEquipmentInfo(this, eqInfo));

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

            if (Weight == 1.0)
                Weight = 2.0;
        }
    }
}