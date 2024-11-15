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

namespace Server.Items
{
    public class SummonWaterElementalScroll : SpellScroll
    {
        public override int LabelNumber { get { return 8044; } } // Summon Water Elemental Scroll

        [Constructable]
        public SummonWaterElementalScroll()
            : this(1)
        {
        }

        [Constructable]
        public SummonWaterElementalScroll(int amount)
            : base(63, 0x1F6C, amount)
        {
        }

        public SummonWaterElementalScroll(Serial serial)
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

        public override Item Dupe(int amount)
        {
            return base.Dupe(new SummonWaterElementalScroll(amount), amount);
        }
    }
}