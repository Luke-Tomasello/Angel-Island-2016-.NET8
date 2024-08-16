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

namespace Server.Factions
{
    public class FactionPersistance : Item
    {
        private static FactionPersistance m_Instance;

        public static FactionPersistance Instance { get { return m_Instance; } }

        public override string DefaultName
        {
            get { return "Faction Persistance - Internal"; }
        }

        public FactionPersistance()
            : base(1)
        {
            Movable = false;

            if (m_Instance == null || m_Instance.Deleted)
                m_Instance = this;
            else
                base.Delete();
        }

        private enum PersistedType
        {
            Terminator,
            Faction,
            Town
        }

        public FactionPersistance(Serial serial)
            : base(serial)
        {
            m_Instance = this;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            List<Faction> factions = Faction.Factions;

            for (int i = 0; i < factions.Count; ++i)
            {
                writer.WriteEncodedInt((int)PersistedType.Faction);
                factions[i].State.Serialize(writer);
            }

            List<Town> towns = Town.Towns;

            for (int i = 0; i < towns.Count; ++i)
            {
                writer.WriteEncodedInt((int)PersistedType.Town);
                towns[i].State.Serialize(writer);
            }

            writer.WriteEncodedInt((int)PersistedType.Terminator);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        PersistedType type;

                        while ((type = (PersistedType)reader.ReadEncodedInt()) != PersistedType.Terminator)
                        {
                            switch (type)
                            {
                                case PersistedType.Faction: new FactionState(reader); break;
                                case PersistedType.Town: new TownState(reader); break;
                            }
                        }

                        break;
                    }
            }
        }

        public override void Delete()
        {
        }
    }
}