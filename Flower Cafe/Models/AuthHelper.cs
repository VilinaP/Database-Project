using System.Text;

namespace Flower_Cafe.Models;

public static class AuthHelper
{
    public static bool PasswordMatches(byte[] storedPasswordBytes, byte[] storedSalt, string enteredPassword)
    {
        var enteredBytes = Encoding.UTF8.GetBytes(enteredPassword);

        return storedPasswordBytes.SequenceEqual(enteredBytes);
    }

    public static bool IsLoggedIn(HttpContext context)
    {
        return context.Session.GetInt32("UserId") != null;
    }

    public static string GetUserType(HttpContext context)
    {
        return context.Session.GetString("UserType") ?? "";
    }

    public static string GetUsername(HttpContext context)
    {
        return context.Session.GetString("Username") ?? "";
    }

    public static long? GetRelatedId(HttpContext context)
    {
        var relatedIdText = context.Session.GetString("RelatedId");

        if (long.TryParse(relatedIdText, out var relatedId))
        {
            return relatedId;
        }

        return null;
    }

    public static bool IsAdmin(HttpContext context)
    {
        return GetUserType(context) == "admin";
    }

    public static bool IsManager(HttpContext context)
    {
        return GetUserType(context) == "manager";
    }

    public static bool IsEmployee(HttpContext context)
    {
        return GetUserType(context) == "employee";
    }

    public static bool IsCustomer(HttpContext context)
    {
        return GetUserType(context) == "customer";
    }
}