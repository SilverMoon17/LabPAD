using System.ComponentModel.DataAnnotations;

namespace Mango.Services.CouponAPI.Models;

public class Coupon
{
    [Key] 
    public int CouponId { get; set; }
    public string CouponCode { get; set; }
    
    [Range(0, 100)]
    public double DiscountAmount { get; set; }
}