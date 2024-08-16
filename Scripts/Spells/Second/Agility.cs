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

using Server.Targeting;

namespace Server.Spells.Second
{
    public class AgilitySpell : Spell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Agility", "Ex Uus",
                SpellCircle.Second,
                212,
                9061,
                Reagent.Bloodmoss,
                Reagent.MandrakeRoot
            );

        public AgilitySpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void Target(Mobile m)
        {
            if (!Caster.CanSee(m))
            {
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (CheckBSequence(m))
            {
                SpellHelper.Turn(Caster, m);

                SpellHelper.AddStatBonus(Caster, m, StatType.Dex);

                m.FixedParticles(0x375A, 10, 15, 5010, EffectLayer.Waist);
                m.PlaySound(0x28E);
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private AgilitySpell m_Owner;

            public InternalTarget(AgilitySpell owner)
                : base(12, false, TargetFlags.Beneficial)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                {
                    m_Owner.Target((Mobile)o);
                }
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}