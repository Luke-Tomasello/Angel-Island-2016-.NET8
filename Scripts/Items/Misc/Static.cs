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
    public class Static : Item
    {
        [Constructable]
        public Static(int itemID)
            : base(itemID)
        {
            Movable = false;
        }

        [Constructable]
        public Static(int itemID, int count)
            : this(Utility.Random(itemID, count))
        {
        }

        public Static(Serial serial)
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

    public class LocalizedStatic : Static
    {
        private int m_LabelNumber;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Number
        {
            get { return m_LabelNumber; }
            set { m_LabelNumber = value; InvalidateProperties(); }
        }

        public override int LabelNumber { get { return m_LabelNumber; } }

        [Constructable]
        public LocalizedStatic(int itemID)
            : this(itemID, 1020000 + itemID)
        {
        }

        [Constructable]
        public LocalizedStatic(int itemID, int labelNumber)
            : base(itemID)
        {
            m_LabelNumber = labelNumber;
        }

        public LocalizedStatic(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((byte)0); // version
            writer.WriteEncodedInt((int)m_LabelNumber);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadByte();

            switch (version)
            {
                case 0:
                    {
                        m_LabelNumber = reader.ReadEncodedInt();
                        break;
                    }
            }
        }
    }
}