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

/* Mobiles/Guards/BaseGuard.cs
 * CHANGELOG:
 *  9/18/2024, Adam: Up the chance for the guard to drop his hally to 80%. 
 *      Killing guards is just too much fun
 *  9/13/2024, Adam (auto ignition)
 *      In order to stop griefers from dragging dangerous creatures into a guard zone
 *          and allowing it to kill all the macroers, we auto-ignite guards.
 *      In AcquireFocusMob(), we simply remember monsters attacking players in a guarded region for 5 seconds.
 *      When a guard detects a creature, we check to see if it is a 'remembered' baddy. If so, kill it.
 *      Note: There still needs to be a nearby guard to detect this creature for believability.
 *      Finally: It's unrealistic that a guard would 'see' a bone knight near players, and do nothing about it
 *	2/14/11, Adam
 *		Add in SetDamage() call for non UOAI shards.
 *	7/21/10, Adam
 *		1% chance to drop hally since we really want players farming vampires for slayers
 *	6/30/10, Adam
 *		o Guards now use a new form of Hybrid AI called GuardAI.
 *		o Guards now do some basic heals and will cast the new NPC spell "InvisibleShield", "Kal Sanct Grav" 
 *			on suicide bombers. (Kal (summon), Sanct (Protection), Grav (field))
 *	6/23/10, Adam
 *		make the karma positive
 *	6/21/10, Adam
 *		o lower detect hidded to 88-92 to give a reasonable chance at avoiding detection on the first try
 *		o format code
 *	5/17/10, Adam
 *		New message when a head is turned in, yet the bounty has already been collected:
 *		"My thanks for slaying this vile person, but the bounty has already been collected."
 *	7/8/07, Adam
 *		We caught an exception here, so .. converting to IPooledEnumerable so we can free it. 
 *		See also Pixie's fix of: 7/10/04, Pix
 *	4/20/08, Adam
 *		check to see if any groundskeepers need to be spawned in OnThink
 *  12/21/05, Adam
 *		Clear the Crafter and set the name text to "Property of Britain Armory"
 *		We now have a new semi rare :p
 *  12/21/05, Kit
 *		Allowed death and looting of guards.
 *  12/20/05, Kit
 *		Set halberd to silver.
 *  10/01/04, Pigpen
 *		Changed range for thief speech from 10 to 3 tiles.
 *  9/28/04, Pigpen
 *		Added in functionality to point out thieves with clever semi-anti thief comments.
 *  7/10/04, Pix
 *		Added try-catch around foreach in Spawn() that seemingly caused the shard to crash.
 *  6/21/04, Old Salty
 * 		Added OnThink function so that all guards react properly
 *	6/10/04, mith
 *		Modified for the new guard non-insta-kill guards.
 *	5/23/04, Pixie
 *		Moved the collecting of the head to BountyKeeper and inserted a call to it when
 *		a head is dropped on the Guard.
 *  5/22/04, Pixie
 *		Fixed guard keeping head when it suspects treachery.
 *  5/18/04, Pixie
 *		Changes to Lord British bonus for bounties
 *	5/16/04, Pixie
 *		Guards now accept heads for the collection of bounties.
 */

using Server.BountySystem;
using Server.Diagnostics;
using Server.Items;
using Server.Regions;

namespace Server.Mobiles
{
    public abstract class BaseGuard : BaseCreature
    {
        private TimeSpan m_SpeechDelay = TimeSpan.FromSeconds(300.0); // time between speech
        public DateTime m_NextSpeechTime;

        public static void Spawn(Mobile caller, Mobile target)
        {
            Spawn(caller, target, 1, false);
        }

        public static void Spawn(Mobile caller, Mobile target, int amount, bool onlyAdditional)
        {
            if (target == null || target.Deleted)
                return;

            IPooledEnumerable eable = null;
            //Pix: 7/10/04 - putting this safety try-catch in after shard crash
            //Adam: 7/8/08 - converting IPooledEnumerable so we can free it. (We caught an exception here)
            try
            {
                eable = target.GetMobilesInRange(15);
                foreach (Mobile m in eable)
                {
                    if (m is BaseGuard)
                    {
                        BaseGuard g = (BaseGuard)m;

                        if (g.Focus == null) // idling
                        {
                            g.Focus = target;

                            --amount;
                        }
                        else if (g.Focus == target && !onlyAdditional)
                        {
                            --amount;
                        }
                    }
                }
            }
            catch (Exception ex) { EventSink.InvokeLogException(new LogExceptionEventArgs(ex)); }
            finally
            {
                if (eable != null)
                    eable.Free();
            }
            while (amount-- > 0)
                caller.Region.MakeGuard(target);
        }

        public BaseGuard(Mobile target)
            : base(AIType.AI_Guard, FightMode.Aggressor, 20, 10, 0.5, 0.25)
        {
            if (target != null)
            {
                int newX = Utility.RandomBool() ? target.X + Utility.Random(5) : target.X - Utility.Random(5);
                int newY = Utility.RandomBool() ? target.Y + Utility.Random(5) : target.Y - Utility.Random(5);
                Location = new Point3D(newX, newY, target.Z);
                // Location = target.Location;
                Map = target.Map;

                Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);
            }

            InitStats(250, 250, 250);

            if (!Core.RuleSets.AngelIslandRules() && !Core.RuleSets.RenaissanceRules())
                SetDamage(20, 25);

            Title = "the guard";

            BardImmune = true;
            FightStyle = FightStyle.Melee | FightStyle.Magic | FightStyle.Smart | FightStyle.Bless | FightStyle.Curse;
            UsesHumanWeapons = false;
            UsesBandages = true;
            UsesPotions = true;
            CanRun = true;
            CanReveal = true; // magic and smart

            SpeechHue = Utility.RandomDyedHue();

            Hue = Utility.RandomSkinHue();

            if (Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");

                switch (Utility.Random(2))
                {
                    case 0: AddItem(new LeatherSkirt()); break;
                    case 1: AddItem(new LeatherShorts()); break;
                }

                switch (Utility.Random(5))
                {
                    case 0: AddItem(new FemaleLeatherChest()); break;
                    case 1: AddItem(new FemaleStuddedChest()); break;
                    case 2: AddItem(new LeatherBustierArms()); break;
                    case 3: AddItem(new StuddedBustierArms()); break;
                    case 4: AddItem(new FemalePlateChest()); break;
                }
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");

                AddItem(new PlateChest());
                AddItem(new PlateArms());
                AddItem(new PlateLegs());

                if (Core.RuleSets.AngelIslandRules() || Core.RuleSets.RenaissanceRules())
                    AddItem(new PlateGorget());

                switch (Utility.Random(3))
                {
                    case 0: AddItem(new Doublet(Utility.RandomNondyedHue())); break;
                    case 1: AddItem(new Tunic(Utility.RandomNondyedHue())); break;
                    case 2: AddItem(new BodySash(Utility.RandomNondyedHue())); break;
                }
            }

            Item hair = new Item(Utility.RandomList(0x203B, 0x203C, 0x203D, 0x2044, 0x2045, 0x2047, 0x2049, 0x204A));

            hair.Hue = Utility.RandomHairHue();
            hair.Layer = Layer.Hair;
            hair.Movable = false;

            AddItem(hair);

            if (Utility.RandomBool() && !this.Female)
            {
                Item beard = new Item(Utility.RandomList(0x203E, 0x203F, 0x2040, 0x2041, 0x204B, 0x204C, 0x204D));

                beard.Hue = hair.Hue;
                beard.Layer = Layer.FacialHair;
                beard.Movable = false;

                AddItem(beard);
            }

            Halberd weapon = new Halberd();

            weapon.Movable = false;
            weapon.Quality = WeaponQuality.Exceptional;

            if (Core.RuleSets.AngelIslandRules() || Core.RuleSets.RenaissanceRules())
            {
                weapon.Crafter = null;
                weapon.Slayer = SlayerName.Silver;
                weapon.Identified = true;
                weapon.Name = "Property of Britain Armory";
            }
            else
                weapon.Crafter = this;

            AddItem(weapon);

            Container pack = new Backpack();
            pack.Movable = false;
            pack.DropItem(new Gold(10, 25));
            AddItem(pack);

            SetSkill(SkillName.Anatomy, 115, 120.0);
            SetSkill(SkillName.Tactics, 115, 120.0);
            SetSkill(SkillName.Swords, 115, 120.0);
            SetSkill(SkillName.MagicResist, 115, 120.0);
            SetSkill(SkillName.DetectHidden, 88, 92);

            if (Core.RuleSets.AngelIslandRules() || Core.RuleSets.RenaissanceRules())
            {
                SetSkill(SkillName.EvalInt, 115, 120.0);
                SetSkill(SkillName.Magery, 115, 120.0);

                SetDamage(20, 25);
                VirtualArmor = 36;
                PackItem(new Bandage(Utility.RandomMinMax(VirtualArmor, VirtualArmor * 2)));
                PackStrongPotions(6, 12);
                PackItem(new Pouch());

                // level 4 fame/karma
                Fame = Utility.RandomMinMax(5000, 9999);
                Karma = Utility.RandomMinMax(5000, 9999);
            }

            this.NextCombatTime = DateTime.UtcNow + TimeSpan.FromSeconds(0.5);
            this.Focus = target;
        }

        public override bool ShowFameTitle { get { return false; } }

        public BaseGuard(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            if (Core.RuleSets.AngelIslandRules() || Core.RuleSets.RenaissanceRules())
            {
                // 1% chance to drop hally since we really want players farming vampires for slayers
                //if (Utility.RandomChance(1))
                // 9/18/2024, Adam: Up the chance to 80%. Killing guards is just too much fun
                if (Utility.Chance(0.80))
                {
                    Item hally = this.FindItemOnLayer(Layer.TwoHanded);
                    if (hally != null)
                        hally.Movable = true;

                    // We will keep out eye on SlayerWeapon.log and decrease this drop if it exceeds the vampire drops
                    if (hally != null)
                    {
                        LogHelper Logger = new LogHelper("SlayerWeapon.log", false, true);
                        Logger.Log(LogType.Item, hally);
                        Logger.Finish();
                    }
                }
            }
            else
            {
                if (Core.RuleSets.SiegeRules())
                {
                    if (Spawning)
                    {   // no loot
                        PackGold(0);
                    }
                    else
                    {
                    }
                }
                else
                {   // Standard RunUO
                    // no loot
                }
            }
        }

        public override bool OnBeforeDeath()
        {

            if (Core.RuleSets.AngelIslandRules() || Core.RuleSets.RenaissanceRules())
            {   // we need loot generation
                return base.OnBeforeDeath();
            }
            else
            {
                Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
                PlaySound(0x1FE);
                Delete();
            }

            return true;
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            bool bReturn = false;
            if (dropped is Head)
            {
                int result = 0;
                int goldGiven = 0;
                bReturn = BountyKeeper.CollectBounty((Head)dropped, from, this, ref goldGiven, ref result);
                switch (result)
                {
                    case -2:
                        Say("You disgusting miscreant!  Why are you giving me an innocent person's head?");
                        break;
                    case -3:
                        Say("I suspect treachery....");
                        Say("I'll take that head, you just run along now.");
                        break;
                    case -4: //good, gold given
                        Say(string.Format("My thanks for slaying this vile person.  Here's the reward of {0} gold!", goldGiven));
                        break;
                    case -5:
                        Say(string.Format("My thanks for slaying this vile person, but the bounty has already been collected."));
                        break;
                    default:
                        if (bReturn)
                        {
                            Say("I'll take that.");
                        }
                        else
                        {
                            Say("I don't want that.");
                        }
                        break;
                }
            }
            return bReturn;
        }

        public abstract Mobile Focus { get; set; }

        //Old Salty - added OnThink override 
        public override void OnThink()
        {
            if (Combatant != null && Combatant != Focus)
                Focus = Combatant;
            else
            {
                // 9/13/2024, Adam: In order to stop griefers from dragging dangerous creatures into a guard zone
                //      and allowing it to kill all the macroers, we auto-ignite guards.
                //  In AcquireFocusMob(), we simply remember monsters attacking players in a guarded region for 5 seconds.
                //  When a guard detects a creature, we check to see if it is a 'remembered' baddy. If so, kill it.
                //  Note: There still needs to be a nearby guard to detect this baddy for believability.
                //  Finally: It's unrealistic that a guard would 'see' a bone knight near players, and do nothing about it

                List<Mobile> list = new List<Mobile>();
                IPooledEnumerable eable = this.Map.GetMobilesInRange(this.Location, this.RangePerception);
                foreach (Mobile m in eable)
                    if (!m.Player && this.CanSee(m))
                        list.Add(m);
                eable.Free();

                foreach (Mobile m in list)
                    if (BaseAI.FocusMobAcquired.Recall(m) != false)
                        if (this.Region is GuardedRegion && (this.Region as GuardedRegion).IsGuarded)
                        {
                            LogHelper logger = new LogHelper("AggressiveCreatureInTown.log", overwrite: false);
                            Mobile target = null;
                            Memory.ObjectMemory om = BaseAI.FocusMobAcquired.Recall(m as object);
                            if (om != null && om.Context is Mobile)
                                target = om.Context as Mobile;
                            logger.Log(LogType.Mobile, m, string.Format("targeting {0}", target != null ? target.ToString() : "???"));
                            logger.Finish();
                            (this.Region as GuardedRegion).MakeGuard(m);
                            BaseAI.FocusMobAcquired.Forget(m);
                            break;
                        }
            }

            // check to see if any groundskeepers need to be spawned
            try
            {
                Groundskeeper.DoGroundskeeper(this);
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex);
            }

            base.OnThink();
        }
        //private HandleAggressiveCreatureInTown
        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m.Player && m.AccessLevel == AccessLevel.Player && ((PlayerMobile)m).NpcGuild == NpcGuild.ThievesGuild && ((Mobile)m).Hidden == false && m.InRange(this, 3) && DateTime.UtcNow >= m_NextSpeechTime)
            {
                if (Utility.RandomDouble() < 0.03)
                {
                    m_NextSpeechTime = DateTime.UtcNow + m_SpeechDelay;

                    switch (Utility.Random(5))
                    {
                        case 0:
                            this.Say("Beware a thief is in our midst."); break;
                        case 1:
                            this.Say("Beware, {0}. For I know your true intentions.", m.Name); break;
                        case 2:
                            this.Say("Back away dirty thief, my possesions are no concern of yours."); break;
                        case 3:
                            this.Say("Citizens! {0} seeks to relieve you of your belongings.", m.Name); break;
                        case 4:
                            this.Say("Take heed scum, any thieving in these parts and thou shall feel my steel."); break;
                    }
                }
            }

            base.OnMovement(m, oldLocation);
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
}