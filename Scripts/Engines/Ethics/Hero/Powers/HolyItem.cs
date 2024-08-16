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

using Server.Items;

namespace Server.Ethics.Hero
{
    public sealed class HolyItem : Power
    {
        public HolyItem()
        {
            m_Definition = new PowerDefinition(
                    1,
                    1,
                    "Holy Item",
                    "Vidda K'balc",
                    ""
                );
        }

        public override void BeginInvoke(Player from)
        {
            from.Mobile.BeginTarget(12, false, Targeting.TargetFlags.None, new TargetStateCallback(Power_OnTarget), from);
            from.Mobile.SendMessage("Which item do you wish to imbue?");
        }

        private void Power_OnTarget(Mobile fromMobile, object obj, object state)
        {
            Player from = state as Player;

            Item item = obj as Item;

            if (item == null)
            {
                from.Mobile.LocalOverheadMessage(Server.Network.MessageType.Regular, 0x3B2, false, "You may not imbue that.");
                return;
            }

            if (item.Parent != from.Mobile)
            {
                from.Mobile.LocalOverheadMessage(Server.Network.MessageType.Regular, 0x3B2, false, "You may only imbue items you are wearing.");
                return;
            }

            if ((item.SavedFlags & 0x300) != 0)
            {
                from.Mobile.LocalOverheadMessage(Server.Network.MessageType.Regular, 0x3B2, false, "That has already beem imbued.");
                return;
            }

            // in 13.6, factions and ethics were merged and I believe this is when all the extra huing was added.
            //	the original ethics only allowed the huing of armor
            bool canImbue;
            if (Core.OldEthics)
                canImbue = (item is BaseArmor && item.Name == null);
            else
                canImbue = (item is Spellbook || item is BaseClothing || item is BaseArmor || item is BaseWeapon) && (item.Name == null);

            if (canImbue)
            {
                if (!CheckInvoke(from))
                    return;

                item.Hue = Ethic.Hero.Definition.PrimaryHue;
                item.SavedFlags |= 0x100;

                from.Mobile.FixedEffect(0x375A, 10, 20);
                from.Mobile.PlaySound(0x209);

                FinishInvoke(from);
            }
            else
            {
                from.Mobile.LocalOverheadMessage(Server.Network.MessageType.Regular, 0x3B2, false, "You may not imbue that.");
            }
        }
    }
}