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
using System.Collections;

namespace Server.Commands
{
    public class ExportCommand
    {
        private const string ExportFile = @"C:\Uo\WorldForge\items.wsc";

        public static void Initialize()
        {
            Server.CommandSystem.Register("ExportWSC", AccessLevel.Administrator, new CommandEventHandler(Export_OnCommand));
        }

        public static void Export_OnCommand(CommandEventArgs e)
        {
            StreamWriter w = new StreamWriter(ExportFile);
            ArrayList remove = new ArrayList();
            int count = 0;

            e.Mobile.SendMessage("Exporting all static items to \"{0}\"...", ExportFile);
            e.Mobile.SendMessage("This will delete all static items in the world.  Please make a backup.");

            foreach (Item item in World.Items.Values)
            {
                if ((item is Static || item is BaseFloor || item is BaseWall)
                    && item.RootParent == null)
                {
                    w.WriteLine("SECTION WORLDITEM {0}", count);
                    w.WriteLine("{");
                    w.WriteLine("SERIAL {0}", item.Serial);
                    w.WriteLine("NAME #");
                    w.WriteLine("NAME2 #");
                    w.WriteLine("ID {0}", item.ItemID);
                    w.WriteLine("X {0}", item.X);
                    w.WriteLine("Y {0}", item.Y);
                    w.WriteLine("Z {0}", item.Z);
                    w.WriteLine("COLOR {0}", item.Hue);
                    w.WriteLine("CONT -1");
                    w.WriteLine("TYPE 0");
                    w.WriteLine("AMOUNT 1");
                    w.WriteLine("WEIGHT 255");
                    w.WriteLine("OWNER -1");
                    w.WriteLine("SPAWN -1");
                    w.WriteLine("VALUE 1");
                    w.WriteLine("}");
                    w.WriteLine("");

                    count++;
                    remove.Add(item);
                    w.Flush();
                }
            }

            w.Close();

            foreach (Item item in remove)
                item.Delete();

            e.Mobile.SendMessage("Export complete.  Exported {0} statics.", count);
        }
    }
}
/*SECTION WORLDITEM 1
{
SERIAL 1073741830
NAME #
NAME2 #
ID 1709
X 1439
Y 1613
Z 20
CONT -1
TYPE 12
AMOUNT 1
WEIGHT 25500
OWNER -1
SPAWN -1
VALUE 1
}*/