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

namespace Server.Diagnostics
{
    public class TargetProfile : BaseProfile
    {
        private static Dictionary<Type, TargetProfile> _profiles = new Dictionary<Type, TargetProfile>();

        public static IEnumerable<TargetProfile> Profiles
        {
            get
            {
                return _profiles.Values;
            }
        }

        public static TargetProfile Acquire(Type type)
        {
            if (!Core.Profiling)
            {
                return null;
            }

            TargetProfile prof;

            if (!_profiles.TryGetValue(type, out prof))
            {
                _profiles.Add(type, prof = new TargetProfile(type));
            }

            return prof;
        }

        public TargetProfile(Type type)
            : base(type.FullName)
        {
        }
    }
}