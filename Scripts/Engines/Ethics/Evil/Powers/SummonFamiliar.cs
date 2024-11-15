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

using Server.Mobiles;

namespace Server.Ethics.Evil
{
    public sealed class SummonFamiliar : Power
    {
        public SummonFamiliar()
        {
            m_Definition = new PowerDefinition(
                    5,
                    3,
                    "Summon Familiar",
                    "Trubechs Vingir",
                    ""
                );
        }

        public override void BeginInvoke(Player from)
        {
            if (from.Familiar != null && from.Familiar.Deleted)
                from.Familiar = null;

            if (from.Familiar != null)
            {
                from.Mobile.LocalOverheadMessage(Server.Network.MessageType.Regular, 0x3B2, false, "You already have an unholy familiar.");
                return;
            }

            if ((from.Mobile.Followers + 1) > from.Mobile.FollowersMax)
            {
                from.Mobile.SendLocalizedMessage(1049645); // You have too many followers to summon that creature.
                return;
            }

            UnholyFamiliar familiar = new UnholyFamiliar();

            if (Mobiles.BaseCreature.Summon(familiar, from.Mobile, from.Mobile.Location, 0x217, TimeSpan.FromHours(1.0)))
            {
                from.Familiar = familiar;

                // update familiar's notority
                if (familiar != null)
                    familiar.Delta(MobileDelta.Noto);

                FinishInvoke(from);
            }
        }
    }
}