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

/* Scripts/Engines/Craft/Core/CraftGumpItem.cs
 * CHANGELOG
 *  08/31/05 Taran Kain
 *		DrawItem(): Added Exceptional Chance display for bolas.
 *	8/18/05, erlein
 *		DrawItem(): Added Exceptional Chance display for jewellery.
 *	8/1/05, erlein
 *		DrawItem(): Added Exceptional Chance display for Runebooks.
 *	8/12/04, mith
 *		DrawItem(): Added Exceptional Chance display for Tools.
 *  6/5/04, Pix
 *		Merged in 1.0RC0 code.
 */


using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Engines.Craft
{
    public class CraftGumpItem : Gump
    {
        private Mobile m_From;
        private CraftSystem m_CraftSystem;
        private CraftItem m_CraftItem;
        private BaseTool m_Tool;

        private const int LabelHue = 0x480; // 0x384
        private const int LabelColor = 0x7FFF;

        private int m_OtherCount;

        public CraftGumpItem(Mobile from, CraftSystem craftSystem, CraftItem craftItem, BaseTool tool)
            : base(40, 40)
        {
            m_From = from;
            m_CraftSystem = craftSystem;
            m_CraftItem = craftItem;
            m_Tool = tool;

            from.CloseGump(typeof(CraftGump));
            from.CloseGump(typeof(CraftGumpItem));

            AddPage(0);
            AddBackground(0, 0, 530, 417, 5054);
            AddImageTiled(10, 10, 510, 22, 2624);
            AddImageTiled(10, 37, 150, 148, 2624);
            AddImageTiled(165, 37, 355, 90, 2624);
            AddImageTiled(10, 190, 155, 22, 2624);
            AddImageTiled(10, 217, 150, 53, 2624);
            AddImageTiled(165, 132, 355, 80, 2624);
            AddImageTiled(10, 275, 155, 22, 2624);
            AddImageTiled(10, 302, 150, 53, 2624);
            AddImageTiled(165, 217, 355, 80, 2624);
            AddImageTiled(10, 360, 155, 22, 2624);
            AddImageTiled(165, 302, 355, 80, 2624);
            AddImageTiled(10, 387, 510, 22, 2624);
            AddAlphaRegion(10, 10, 510, 399);

            AddHtmlLocalized(170, 40, 150, 20, 1044053, LabelColor, false, false); // ITEM
            AddHtmlLocalized(10, 192, 150, 22, 1044054, LabelColor, false, false); // <CENTER>SKILLS</CENTER>
            AddHtmlLocalized(10, 277, 150, 22, 1044055, LabelColor, false, false); // <CENTER>MATERIALS</CENTER>
            AddHtmlLocalized(10, 362, 150, 22, 1044056, LabelColor, false, false); // <CENTER>OTHER</CENTER>

            if (craftSystem.GumpTitleNumber > 0)
                AddHtmlLocalized(10, 12, 510, 20, craftSystem.GumpTitleNumber, LabelColor, false, false);
            else
                AddHtml(10, 12, 510, 20, craftSystem.GumpTitleString, false, false);

            AddButton(15, 387, 4014, 4016, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 390, 150, 18, 1044150, LabelColor, false, false); // BACK

            AddButton(270, 387, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(305, 390, 150, 18, 1044151, LabelColor, false, false); // MAKE NOW

            if (craftItem.NameNumber > 0)
                AddHtmlLocalized(330, 40, 180, 18, craftItem.NameNumber, LabelColor, false, false);
            else
                AddLabel(330, 40, LabelHue, craftItem.NameString);

            if (craftItem.UseAllRes)
                AddHtmlLocalized(170, 302 + (m_OtherCount++ * 20), 310, 18, 1048176, LabelColor, false, false); // Makes as many as possible at once

            DrawItem();
            DrawSkill();
            DrawRessource();
        }

        private bool m_ShowExceptionalChance;

        public void DrawItem()
        {

#if old
			Item item = Activator.CreateInstance(m_CraftItem.ItemType) as Item;

			if (item != null)
			{
				AddItem(20, 50, item.ItemID);

				if (item is DragonBardingDeed || item is BaseArmor || item is BaseWeapon || item is BaseClothing ||
						item is BaseInstrument || item is BaseTool || item is BaseHarvestTool || item is Runebook ||
						item is BaseJewel || item is Bola)
				{
					AddHtmlLocalized(170, 302 + (m_OtherCount++ * 20), 310, 18, 1044059, LabelColor, false, false); // This item may hold its maker's mark
					m_ShowExceptionalChance = true;
				}
				item.Delete();
			}
#else
            Type type = m_CraftItem.ItemType;

            AddItem(20, 50, CraftItem.ItemIDOf(type));

            if (m_CraftItem.IsMarkable(type))
            {
                AddHtmlLocalized(170, 302 + (m_OtherCount++ * 20), 310, 18, 1044059, LabelColor, false, false); // This item may hold its maker's mark
                m_ShowExceptionalChance = true;
            }
#endif

        }

        public void DrawSkill()
        {
            for (int i = 0; i < m_CraftItem.Skills.Count; i++)
            {
                CraftSkill skill = m_CraftItem.Skills.GetAt(i);
                double minSkill = skill.MinSkill, maxSkill = skill.MaxSkill;

                if (minSkill < 0)
                    minSkill = 0;

                AddHtmlLocalized(170, 132 + (i * 20), 200, 18, 1044060 + (int)skill.SkillToMake, LabelColor, false, false);
                AddLabel(430, 132 + (i * 20), LabelHue, String.Format("{0:F1}", minSkill));
            }

            CraftSubResCol res = (m_CraftItem.UseSubRes2 ? m_CraftSystem.CraftSubRes2 : m_CraftSystem.CraftSubRes);
            int resIndex = -1;

            CraftContext context = m_CraftSystem.GetContext(m_From);

            if (context != null)
                resIndex = (m_CraftItem.UseSubRes2 ? context.LastResourceIndex2 : context.LastResourceIndex);

            bool allRequiredSkills = true;
            double chance = m_CraftItem.GetSuccessChance(m_From, resIndex > -1 ? res.GetAt(resIndex).ItemType : null, m_CraftSystem, false, ref allRequiredSkills);

            double excepChance = m_CraftItem.GetExceptionalChance(m_CraftSystem, chance, m_From);

            if (chance < 0.0)
                chance = 0.0;
            else if (chance > 1.0)
                chance = 1.0;

            if (excepChance < 0.0)
                excepChance = 0.0;
            else if (excepChance > 1.0)
                excepChance = 1.0;

            AddHtmlLocalized(170, 80, 250, 18, 1044057, LabelColor, false, false); // Success Chance:
            AddLabel(430, 80, LabelHue, String.Format("{0:F1}%", chance * 100));

            if (m_ShowExceptionalChance)
            {
                AddHtmlLocalized(170, 100, 250, 18, 1044058, 32767, false, false); // Exceptional Chance:
                AddLabel(430, 100, LabelHue, String.Format("{0:F1}%", excepChance * 100));
            }
        }

        public void DrawRessource()
        {
            bool retainedColor = false;

            CraftContext context = m_CraftSystem.GetContext(m_From);

            CraftSubResCol res = (m_CraftItem.UseSubRes2 ? m_CraftSystem.CraftSubRes2 : m_CraftSystem.CraftSubRes);
            int resIndex = -1;

            if (context != null)
                resIndex = (m_CraftItem.UseSubRes2 ? context.LastResourceIndex2 : context.LastResourceIndex);

            for (int i = 0; i < m_CraftItem.Ressources.Count && i < 4; i++)
            {
                Type type;
                string nameString;
                int nameNumber;

                CraftRes craftResource = m_CraftItem.Ressources.GetAt(i);

                type = craftResource.ItemType;
                nameString = craftResource.NameString;
                nameNumber = craftResource.NameNumber;

                // Resource Mutation
                if (type == res.ResType && resIndex > -1)
                {
                    CraftSubRes subResource = res.GetAt(resIndex);

                    type = subResource.ItemType;

                    nameString = subResource.NameString;
                    nameNumber = subResource.GenericNameNumber;

                    if (nameNumber <= 0)
                        nameNumber = subResource.NameNumber;
                }
                // ******************

                if (!retainedColor && m_CraftItem.RetainsColorFrom(m_CraftSystem, type))
                {
                    retainedColor = true;
                    AddHtmlLocalized(170, 302 + (m_OtherCount++ * 20), 310, 18, 1044152, LabelColor, false, false); // * The item retains the color of this material
                    AddLabel(500, 219 + (i * 20), LabelHue, "*");
                }

                if (nameNumber > 0)
                    AddHtmlLocalized(170, 219 + (i * 20), 310, 18, nameNumber, LabelColor, false, false);
                else
                    AddLabel(170, 219 + (i * 20), LabelHue, nameString);

                AddLabel(430, 219 + (i * 20), LabelHue, craftResource.Amount.ToString());
            }

            if (m_CraftItem.NameNumber == 1041267) // runebook
            {
                AddHtmlLocalized(170, 219 + (m_CraftItem.Ressources.Count * 20), 310, 18, 1044447, LabelColor, false, false);
                AddLabel(430, 219 + (m_CraftItem.Ressources.Count * 20), LabelHue, "1");
            }

        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            // Back Button
            if (info.ButtonID == 0)
            {
                CraftGump craftGump = new CraftGump(m_From, m_CraftSystem, m_Tool, null);
                m_From.SendGump(craftGump);
            }
            else // Make Button
            {
                int num = m_CraftSystem.CanCraft(m_From, m_Tool, m_CraftItem.ItemType);

                if (num > 0)
                {
                    m_From.SendGump(new CraftGump(m_From, m_CraftSystem, m_Tool, num));
                }
                else
                {
                    Type type = null;

                    CraftContext context = m_CraftSystem.GetContext(m_From);

                    if (context != null)
                    {
                        CraftSubResCol res = (m_CraftItem.UseSubRes2 ? m_CraftSystem.CraftSubRes2 : m_CraftSystem.CraftSubRes);
                        int resIndex = (m_CraftItem.UseSubRes2 ? context.LastResourceIndex2 : context.LastResourceIndex);

                        if (resIndex > -1)
                            type = res.GetAt(resIndex).ItemType;
                    }

                    m_CraftSystem.CreateItem(m_From, m_CraftItem.ItemType, type, m_Tool, m_CraftItem);
                }
            }
        }
    }
}