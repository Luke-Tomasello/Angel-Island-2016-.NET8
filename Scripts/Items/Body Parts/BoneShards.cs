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

/* Scripts\Items\Body Parts\BoneShards.cs
 * ChangeLog
 *	12/28/10, adam
 *		first time checkin - updated from:
 *		http://www.google.com/codesearch/p?hl=en#biPgqLK3B_w/trunk/Scripts/Items/Body%20Parts/BoneShards.cs&q=BoneShards&exact_package=http://runuomondains.googlecode.com/svn&sa=N&cd=1&ct=rc&l=8
 */

namespace Server.Items
{
    [FlipableAttribute(0x1B19, 0x1B1A)]
    public class BoneShards : Item, IScissorable
    {
        [Constructable]
        public BoneShards()
            : base(0x1B19 + Utility.Random(2))
        {
            Stackable = false;
            Weight = 3.0;
        }

        public BoneShards(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }

        public bool Scissor(Mobile from, Scissors scissors)
        {
            if (Deleted || !from.CanSee(this))
                return false;

            base.ScissorHelper(from, new Bone(), 3);
            from.PlaySound(0x21B);

            return false;
        }
    }
}