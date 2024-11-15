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

/* ChangeLog:
 *  9/10/2024, Adam
 *      Add hooks so we can adjust both distance and cooling off period.
 *      If you try to cast too fast:
 *          "You lack sufficient spirit cohesion to attempt that."
 * 	6/5/04, Pix
 * 	    Merged in 1.0RC0 code.
*/

using Server.Items;
using Server.Targeting;

namespace Server.Spells.Third
{
    public class TeleportSpell : Spell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Teleport", "Rel Por",
                SpellCircle.Third,
                215,
                9031,
                Reagent.Bloodmoss,
                Reagent.MandrakeRoot
            );

        public TeleportSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (Server.Misc.WeightOverloading.IsOverloaded(Caster))
            {
                Caster.SendLocalizedMessage(502359, "", 0x22); // Thou art too encumbered to move.
                return false;
            }
            else if (CheckDelay(Caster, check_only: true) == false)
            {
                Caster.SendMessage("You lack sufficient spirit cohesion to attempt that.");
                return false;
            }

            return SpellHelper.CheckTravel(Caster, TravelCheckType.TeleportFrom);
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void Target(IPoint3D p)
        {
            IPoint3D orig = p;
            Map map = Caster.Map;

            SpellHelper.GetSurfaceTop(ref p);

            if (Server.Misc.WeightOverloading.IsOverloaded(Caster))
            {
                Caster.SendLocalizedMessage(502359, "", 0x22); // Thou art too encumbered to move.
            }
            else if (!SpellHelper.CheckTravel(Caster, TravelCheckType.TeleportFrom))
            {
            }
            else if (!SpellHelper.CheckTravel(Caster, map, new Point3D(p), TravelCheckType.TeleportTo))
            {
            }
            else if (map == null || !map.CanSpawnMobile(p.X, p.Y, p.Z))
            {
                Caster.SendLocalizedMessage(501942); // That location is blocked.
            }
            else if (SpellHelper.CheckMulti(new Point3D(p), map))
            {
                Caster.SendLocalizedMessage(501942); // That location is blocked.
            }
            else if (CheckDelay(Caster) == false)
            {
                Caster.SendMessage("You lack sufficient spirit cohesion to attempt that.");
            }
            else if (CheckSequence())
            {
                SpellHelper.Turn(Caster, orig);

                Mobile m = Caster;

                Point3D from = m.Location;
                Point3D to = new Point3D(p);

                m.Location = to;
                m.ProcessDelta();

                Effects.SendLocationParticles(EffectItem.Create(from, m.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
                Effects.SendLocationParticles(EffectItem.Create(to, m.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);

                m.PlaySound(0x1FE);
            }

            FinishSequence();
        }
        public static Memory SpiritCohesion = new Memory();
        public bool CheckDelay(Mobile from, bool check_only = false)
        {
            //simply turned off
            if (CoreAI.TeleRunningEnabled || !Core.RuleSets.AngelIslandRules())
                return true;    // no metering

            // if not global and not in PvP combat
            if (!CoreAI.TeleGlobalDelay && !Utility.CheckPvPCombat(from))
                return true;    // no metering

            // TeleGlobalDelay is true or we are in PvP combat
            if (SpiritCohesion.Recall(from) == false)
            {   // nope, don't remember him.
                if (!check_only)
                    SpiritCohesion.Remember(from, null, CoreAI.TeleDelay);
                return true;    // no metering
            }

            return false;       // meter
        }
        private static int GetTeleDistance(Mobile m)
        {
            if (CoreAI.TeleRunningEnabled || !Core.RuleSets.AngelIslandRules())
                return TeleportConsole.DefaultTeleTiles;    // they can tele run 12 tiles
            else if (Utility.CheckPvPCombat(m))             // if they have a PvP combatant, meter
                return CoreAI.TeleTiles;                    // limited

            return TeleportConsole.DefaultTeleTiles;        // they can tele run 12 tiles
        }
        public class InternalTarget : Target
        {
            private TeleportSpell m_Owner;

            public InternalTarget(TeleportSpell owner)
                : base(GetTeleDistance(owner.Caster), true, TargetFlags.None)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                IPoint3D p = o as IPoint3D;

                if (p != null)
                    m_Owner.Target(p);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}