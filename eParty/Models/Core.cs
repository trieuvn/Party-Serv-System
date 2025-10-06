using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eParty.Models
{
    public class Party
    {
        [Key] public int Id { get; set; }

        [StringLength(50)] public string Name { get; set; }
        [StringLength(20)] public string Type { get; set; }
        [StringLength(20)] public string Status { get; set; }
        public int Cost { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Description { get; set; }
        public int Slots { get; set; }
        [StringLength(100)] public string Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // FK -> User (username)
        [StringLength(50)]
        public string UserUsername { get; set; }
        [ForeignKey(nameof(UserUsername))]
        public virtual User User { get; set; }

        // FK -> Menu
        public int? MenuId { get; set; }
        public virtual Menu Menu { get; set; }

        public virtual ICollection<StaffParty> StaffParties { get; set; }
        public virtual ICollection<PriceHistory> PriceHistories { get; set; }
    }

    public class Menu
    {
        [Key] public int Id { get; set; }
        [StringLength(50)] public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        [StringLength(20)] public string Status { get; set; }

        public virtual ICollection<MenuDetail> MenuDetails { get; set; }
        public virtual ICollection<Party> Parties { get; set; }
    }

    public class Food
    {
        [Key] public int Id { get; set; }
        [StringLength(50)] public string Name { get; set; }
        public string Description { get; set; }
        public int Unit { get; set; }
        public int Cost { get; set; }

        public virtual ICollection<MenuDetail> MenuDetails { get; set; }
        public virtual ICollection<FoodIngredient> FoodIngredients { get; set; }
        public virtual ICollection<PriceHistory> PriceHistories { get; set; }
    }

    public class Ingredient
    {
        [Key] public int Id { get; set; }
        [StringLength(50)] public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Provider> Providers { get; set; }
        public virtual ICollection<FoodIngredient> FoodIngredients { get; set; }
    }

    // Provider “provide” Ingredient (nhiều Provider cho 1 Ingredient)
    public class Provider
    {
        [Key] public int Id { get; set; }

        // FK -> Ingredient
        public int IngredientId { get; set; }
        public virtual Ingredient Ingredient { get; set; }

        [StringLength(50)] public string Name { get; set; }
        public string Description { get; set; }
        [StringLength(15)] public string PhoneNumber { get; set; }
        [StringLength(100)] public string Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int Cost { get; set; }
    }

    // JOIN TABLES

    public class MenuDetail
    {
        [Key, Column(Order = 0)] public int MenuId { get; set; }
        [Key, Column(Order = 1)] public int FoodId { get; set; }
        public int Amount { get; set; }

        public virtual Menu Menu { get; set; }
        public virtual Food Food { get; set; }
    }

    public class FoodIngredient
    {
        [Key, Column(Order = 0)] public int FoodId { get; set; }
        [Key, Column(Order = 1)] public int IngredientId { get; set; }
        public int Amount { get; set; }

        public virtual Food Food { get; set; }
        public virtual Ingredient Ingredient { get; set; }
    }

    public class PriceHistory
    {
        [Key, Column(Order = 0)] public int PartyId { get; set; }
        [Key, Column(Order = 1)] public int FoodId { get; set; }
        public int Cost { get; set; }
        public int Amount { get; set; }

        public virtual Party Party { get; set; }
        public virtual Food Food { get; set; }
    }

    public class StaffParty
    {
        [Key, Column(Order = 0), StringLength(50)]
        public string StaffUsername { get; set; }
        [Key, Column(Order = 1)]
        public int PartyId { get; set; }

        [ForeignKey(nameof(StaffUsername))] public virtual Staff Staff { get; set; }
        public virtual Party Party { get; set; }
    }
}