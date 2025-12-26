using System;
namespace Grc.ValueObjects;

/// <summary>
/// Contact information value object
/// </summary>
public class ContactInfo
{
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    
    public ContactInfo() { }
    
    public ContactInfo(string email, string phone = null, string address = null)
    {
        Email = email;
        Phone = phone;
        Address = address;
    }
}

