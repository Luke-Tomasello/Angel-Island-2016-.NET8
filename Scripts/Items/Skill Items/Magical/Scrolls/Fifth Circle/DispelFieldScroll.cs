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
    public class DispelFieldScroll : SpellScroll
    {
        public override int LabelNumber { get { return 8014; } } // Dispel Field Scroll

        [Constructable]
        public DispelFieldScroll()
            : this(1)
        {
        }

        [Constructable]
        public DispelFieldScroll(int amount)
            : base(33, 0x1F4E, amount)
        {
        }

        public DispelFieldScroll(Serial serial)
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
            return base.Dupe(new DispelFieldScroll(amount), amount);
        }
    }
}