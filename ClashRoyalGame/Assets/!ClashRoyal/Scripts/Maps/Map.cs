using System;
using System.Collections.Generic;
using System.Linq;
using _ClashRoyal.Scripts.Extensions;
using _ClashRoyal.Scripts.Units;
using _ClashRoyal.Scripts.Units.Base;
using _ClashRoyal.Scripts.Units.Towers;
using UnityEngine;

namespace _ClashRoyal.Scripts.Maps
{
    public class Map : Singleton<Map>
    {
        #region FIELDS SERIALIZED

        #endregion

        #region FIELDS

        private List<TowerUnit> _towers = new();

        #endregion

        #region UNITY FUNCTIONS

        private void Awake()
        {
            _towers = new List<TowerUnit>(GetComponentsInChildren<TowerUnit>());
        }

        #endregion

        #region METHODS

        public TowerUnit GetNearestEnemyTower(Unit unit)
        {
            var position = unit.transform.position;

            var towers = _towers.Where(t => t.TeamType != unit.TeamType).ToList();
            var nearestTower = towers[0];
            var distance = Vector3.Distance(position, nearestTower.transform.position);

            foreach (var t in towers
                         .Select(t => new { t, tempDistance = Vector3.Distance(position, t.transform.position) })
                         .Where(@t1 => !(@t1.tempDistance > distance))
                         .Select(@t1 => @t1.t))
            {
                nearestTower = t;
                distance = Vector3.Distance(position, nearestTower.transform.position);
            }

            return nearestTower;
        }

        #endregion
    }
}