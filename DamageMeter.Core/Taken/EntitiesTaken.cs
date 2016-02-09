﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace DamageMeter.Taken
{
    public class EntitiesTaken : ICloneable
    {
        private Dictionary<long, Dictionary<Entity, DamageTaken>> _entitiesStats =
            new Dictionary<long, Dictionary<Entity, DamageTaken>>();

        public long Damage
        {
            get
            {
                if (NetworkController.Instance.Encounter == null)
                {
                    return _entitiesStats.Sum(entityStats => entityStats.Value.Sum(stats => stats.Value.Damage));
                }
                if (ContainsEntity(NetworkController.Instance.Encounter))
                {
                    return
                        _entitiesStats.Sum(
                            timedStats =>
                                timedStats.Value.Where(stats => stats.Key == NetworkController.Instance.Encounter)
                                    .Sum(stats => stats.Value.Damage));
                }
                return 0;
            }
        }

        public int Hits
        {
            get
            {
                if (NetworkController.Instance.Encounter == null)
                {
                    return _entitiesStats.Sum(entityStats => entityStats.Value.Sum(stats => stats.Value.Hits));
                }
                if (ContainsEntity(NetworkController.Instance.Encounter))
                {
                    return
                        _entitiesStats.Sum(
                            timedStats =>
                                timedStats.Value.Where(stats => stats.Key == NetworkController.Instance.Encounter)
                                    .Sum(stats => stats.Value.Hits));
                }
                return 0;
            }
        }

        public object Clone()
        {
            var clone = new EntitiesTaken
            {
                _entitiesStats =
                    _entitiesStats.ToDictionary(i => i.Key,
                        i => i.Value.ToDictionary(j => j.Key, j => (DamageTaken) j.Value.Clone()))
            };
            return clone;
        }


        public void AddStats(long time, Entity target, long damage)
        {
            var roundedTime = time/TimeSpan.TicksPerSecond;
            if (!_entitiesStats.ContainsKey(roundedTime))
            {
                var statsDictionnary = new Dictionary<Entity, DamageTaken> {{target, new DamageTaken()}};
                statsDictionnary[target].AddDamage(damage);
                _entitiesStats.Add(roundedTime, statsDictionnary);
                return;
            }

            if (!_entitiesStats[roundedTime].ContainsKey(target))
            {
                _entitiesStats[roundedTime].Add(target, new DamageTaken());
                _entitiesStats[roundedTime][target].AddDamage(damage);
                return;
            }

            _entitiesStats[roundedTime][target].AddDamage(damage);
        }


        public void RemoveEntity(Entity entity)
        {
            foreach (var timedStats in _entitiesStats)
            {
                timedStats.Value.Remove(entity);
            }
        }


        public bool ContainsEntity(Entity entity)
        {
            return _entitiesStats.Any(timedstats => timedstats.Value.Any(entities => entities.Key == entity));
        }
    }
}