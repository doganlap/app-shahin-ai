using System.ComponentModel.DataAnnotations;

namespace Grc.Product.Subscriptions;

public class CancelSubscriptionInput
{
    [Required]
    [MinLength(10)]
    public string Reason { get; set; }
}


