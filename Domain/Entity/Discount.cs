using Domain.Entity.Identity;
using Domain.Entity.TripEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    //generic discount => from admin to users use to all trips and (tour guid, travel comapany) dosnot effect
    public class GenericDiscount : BaseEntity
    {
        public int CreatorId { get; set; }
        public Admin Creator { get; set; }
        public string Code { get; set; } = Guid.NewGuid()
                                        .ToString("N")
                                        .Substring(0, 6)
                                        .ToUpper();
        public decimal DiscountValue { get; set; }
        public bool IsPercentage { get; set; }
        public int MaxUsage { get; set; }
        private int _currentUsage;
        public int CurrentUsage
        {
            get => _currentUsage;
            set
            {
                if (value > MaxUsage)
                    return;

                _currentUsage = value;
            }
        }
    }
    //sapcific discount => from (tour guid, travel comapany) to users use to spacific trip (tour guid, travel comapany) effect
    public class SpecificDiscount : BaseEntity
    {
        public PublicTrip Trip { get; set; }
        public int TripId { get; set; }
        public int CreatorId { get; set; }
        public User Creator { get; set; }
        public string Code { get; set; } = Guid.NewGuid()
                                        .ToString("N")
                                        .Substring(0, 6)
                                        .ToUpper();
        public decimal DiscountValue { get; set; }
        public bool IsPercentage { get; set; }
        public int MaxUsage { get; set; }

        private int _currentUsage;
        public int CurrentUsage
        {
            get => _currentUsage;
            set
            {
                if (value > MaxUsage)
                    return;

                _currentUsage = value;
            }
        }
    }
}
