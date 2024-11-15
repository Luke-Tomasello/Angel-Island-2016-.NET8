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

/* Scripts/Engines/OverlandSpawner/OverlandSpawner.cs
 * Changelog:
 *	04/23/08, plasma
 *		Change to use new SpawnerCache.cs
 *	9/6/06, Adam
 *		When we load the spawner cache, make sure the spawner has something spawned on it.
 *	9/1/06, Adam
 *		Export the list of spawners via GetSpawnerList()
 *	1/15/06, Adam
 *		Make GetRandomSpawner() public
 *	1/14/06, Adam
 *		1. Add a new LoadSpawnerCache method (called on the heartbeat)t. This method
 *			reloads the cache of 'target' spawners for the Overland system
 *		2. use the spawners homerange except when it is a special 'guard post' spawner (HomeRange == 0)
 *	1/13/06, Adam
 *		Add new 'bad regions' and 'running == false' spawners to the exclusion list
 *	1/11/06, Adam
 *		Working version of the Overland Spawn System
 *	1/10/06, Adam
 *		First time checkin
 */

using Server.Diagnostics;
using Server.Mobiles;	// for Spawner. Shouldn't a Spawner be in the 'item' namespace?

namespace Server.Engines.OverlandSpawner
{
    public class OverlandSpawner
    {

        public Mobile SpawnAt(Spawner spawner, Mobile mob)
        {
            Map map = spawner.Map;

            if (map == null || map == Map.Internal)
                return null;

            if ((mob is BaseCreature) == false)
                return null;

            try
            {
                BaseOverland bo = null;
                BaseCreature bc = mob as BaseCreature;
                // we will make it so this is passed in
                if (mob is BaseOverland)
                    bo = mob as BaseOverland;

                // use the spawners homerange except when it is a special 'guard post' 
                //	spawner (HomeRange == 0)
                bc.RangeHome = spawner.HomeRange == 0 ? 10 : spawner.HomeRange;

                // could be useful ... but not today
                // c.CurrentWayPoint = spawner.WayPoint;
                // c.NavDestination = spawner.NavPoint;

                bc.Home = spawner.Location;
                bc.Spawner = spawner;

                //if we have a nav destination as soon as we spawn start on it
                if (!string.IsNullOrEmpty(bc.NavDestination))
                    bc.AIObject.Think();

                /////////////////////////////
                // move it to the world
                Point3D loc = spawner.GetSpawnPosition(bc);
                bc.MoveToWorld(loc, map);

                // Okay, indicate that this overland mob should be announced on the Town Crier
                // This must be after we are moved off the internal map because the accounce code
                //	supplies 'world location' information which would be wrong if we were still on the internal map
                if (bo != null)
                    bo.Announce = true;

                return bc;
            }
            catch (Exception e)
            {
                LogHelper.LogException(e);
                Console.WriteLine("Server.Engines.OverlandSpawner : Exception {0}", e);
            }

            return null;
        }

        public Mobile OverlandSpawnerSpawn(Mobile mob)
        {
            try
            {
                Spawner spawner = SpawnerCache.GetRandomSpawner(SpawnerCache.SpawnerType.Overland);
                if (spawner == null) return null;
                Mobile m = SpawnAt(spawner, mob);
                return m;
            }
            catch (Exception e)
            {
                LogHelper.LogException(e);
                Console.WriteLine("Server.Engines.OverlandSpawner : Exception {0}", e);
            }

            return null;
        }

    }
}