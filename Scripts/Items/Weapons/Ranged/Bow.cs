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

/* Scripts/Items/Weapons/Ranged/Bow.cs
 * CHANGELOG:
 *	4/23/07, Pix
 *		Fixed for oldschool labelling.
 *  1/30/07, Adam
 *      Give the sealed bows a better 'waxy' hue.
 *	01/02/07, Pix
 *		Made sealed variant constructable
 *	01/02/07, Pix
 *		Added SealedBow.
 */

namespace Server.Items
{
    public class SealedBow : Bow
    {
        [Constructable]
        public SealedBow()
            : base()
        {
            Hue = 0x33;
            //no longer needed - we can use "OldName" now with the implementation of old school labels
            //Name = "a sealed bow";
        }
        public SealedBow(Serial s)
            : base(s)
        {
        }

        public override string OldName
        {
            get
            {
                return "sealed bow";
            }
        }

        public override string OldArticle
        {
            get
            {
                return "a";
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0) Name = null;
        }
    }

    [FlipableAttribute(0x13B2, 0x13B1)]
    public class Bow : BaseRanged
    {
        public override int EffectID { get { return 0xF42; } }
        public override Type AmmoType { get { return typeof(Arrow); } }
        public override Item Ammo { get { return new Arrow(); } }

        public override WeaponAbility PrimaryAbility { get { return WeaponAbility.ParalyzingBlow; } }
        public override WeaponAbility SecondaryAbility { get { return WeaponAbility.MortalStrike; } }

        //		public override int AosStrengthReq{ get{ return 30; } }
        //		public override int AosMinDamage{ get{ return 16; } }
        //		public override int AosMaxDamage{ get{ return 18; } }
        //		public override int AosSpeed{ get{ return 25; } }
        //
        //		public override int OldMinDamage{ get{ return 9; } }
        //		public override int OldMaxDamage{ get{ return 41; } }
        public override int OldStrengthReq { get { return 20; } }
        public override int OldSpeed { get { return 20; } }

        public override int OldDieRolls { get { return 4; } }
        public override int OldDieMax { get { return 9; } }
        public override int OldAddConstant { get { return 5; } }

        public override int DefMaxRange { get { return 10; } }

        public override int InitMinHits { get { return 31; } }
        public override int InitMaxHits { get { return 60; } }

        public override WeaponAnimation DefAnimation { get { return WeaponAnimation.ShootBow; } }

        [Constructable]
        public Bow()
            : base(0x13B2)
        {
            Weight = 6.0;
            Layer = Layer.TwoHanded;
        }

        public Bow(Serial serial)
            : base(serial)
        {
        }

        // old name removed, see base class

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (Weight == 7.0)
                Weight = 6.0;
        }
    }
}