namespace TradeFlow.Common.Domain;

/// <summary>
/// Role Value Object — enforces valid role values and enables type-safe comparisons.
/// Replaces raw string roles with a well-defined set of allowed values.
/// </summary>
public sealed class Role : ValueObject
{
    public string Value { get; }

    // Well-known roles
    public static readonly Role Trader = new("Trader");
    public static readonly Role Admin = new("Admin");
    public static readonly Role Analyst = new("Analyst");

    private static readonly Dictionary<string, Role> _validRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        { Trader.Value, Trader },
        { Admin.Value, Admin },
        { Analyst.Value, Analyst }
    };

    private Role(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Role cannot be empty.", nameof(value));

        Value = value;
    }

    /// <summary>
    /// Creates a Role from a string. Throws if the role is not a recognized value.
    /// </summary>
    public static Role From(string value)
    {
        if (_validRoles.TryGetValue(value, out var role))
            return role;

        throw new ArgumentException($"Invalid role: '{value}'. Valid roles: {string.Join(", ", _validRoles.Keys)}");
    }

    /// <summary>
    /// Tries to create a Role from a string. Returns false if invalid.
    /// </summary>
    public static bool TryFrom(string value, out Role? role)
    {
        return _validRoles.TryGetValue(value, out role);
    }

    /// <summary>
    /// Returns all valid roles.
    /// </summary>
    public static IReadOnlyCollection<Role> All => _validRoles.Values.ToList().AsReadOnly();

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    // Implicit conversions for convenience
    public static implicit operator string(Role role) => role.Value;
}
