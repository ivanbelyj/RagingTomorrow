using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AppearanceCustomization3D {
    /// <summary>
    /// Предоставляет средства проверки занятости, обеспечивая соблюдение совместимости
    /// элементов кастомизации
    /// </summary>
    public class AppearanceOccupancy
    {
        /// <summary>
        /// Для каждой части с заданным id занятости известен элемент кастомизации, который занимает его 
        /// </summary>
        private Dictionary<uint, AppearanceElement> Occupancy { get; set; }
        
        public void Initialize() {
            Occupancy = new Dictionary<uint, AppearanceElement>();
        }

        /// <summary>
        /// Не препятствуют ли существующие элементы кастомизации добавлению заданного?
        /// </summary>
        private bool CanOccupy(AppearanceElement elem) {
            return GetOccupied(elem.OccupancyIds).Count == 0;
        }

        /// <summary>
        /// Получает элементы, которые занимают части с заданными id занятости
        /// </summary>
        public List<AppearanceElement> GetOccupied(uint[] occupancyIds) {
            var res = new List<AppearanceElement>();
            foreach (uint id in occupancyIds) {
                AppearanceElement elemById;
                if (Occupancy.TryGetValue(id, out elemById)) {
                    res.Add(elemById);
                }
            }
            return res;
        }

        public void Occupy(AppearanceElement elem) {
            foreach (uint occupancyId in elem.OccupancyIds) {
                Occupancy[occupancyId] = elem;
            }
        }

        public void Dispossess(AppearanceElement elem) {
            foreach (uint occupancyId in elem.OccupancyIds) {
                Occupancy.Remove(occupancyId);
            }
        }
    }
}
