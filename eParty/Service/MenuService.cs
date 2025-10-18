using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eParty.Models;
using Microsoft.Owin.Security.DataProtection;

namespace eParty.Service
{
    public class MenuService
    {
        private AppDbContext db = new AppDbContext();


        public List<Menu> getAll()
        {
            return db.Menus.ToList();
        }

        public Menu getById(int id)
        {
            return db.Menus.Find(id);
        }
        public Menu getByName(string name)
        {
            return db.Menus.FirstOrDefault(m => m.Name == name);
        }
        public void Create(Menu menu)
        {
            db.Menus.Add(menu);
            db.SaveChanges();
        }

        public List<Menu> getByStatus(string status)
        {
            return db.Menus.Where(m => m.Status == status).ToList();
        }

        /// <summary>
        /// Gets list of menus by type (status).
        /// </summary>
        /// <param name="type">The status/type to filter by</param>
        /// <returns>List of menus with the specified status</returns>
        public List<Menu> getByType(string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                return new List<Menu>();
            }

            return db.Menus.Where(m => m.Status == type).ToList();
        }

       
    }

}