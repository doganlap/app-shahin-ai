using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GrcMvc.Data;

/// <summary>
/// EF Core value converters that enforce DateTimeKind.Utc on materialization AND storage.
///
/// Problem: PostgreSQL timestamptz columns require DateTime with Kind=Utc.
/// Npgsql 6.0+ throws: "Cannot write DateTime with Kind=Local to PostgreSQL type 'timestamp with time zone'"
///
/// Solution: These converters ensure all DateTime values are converted to UTC on write
/// and marked as UTC on read.
///
/// Usage:
/// - Apply to DateTime properties in OnModelCreating
/// - For full correctness, migrate DB columns to timestamptz and use DateTimeOffset
/// </summary>
public static class UtcDateTimeConverters
{
    /// <summary>
    /// Converter for non-nullable DateTime properties.
    /// On write: converts Local/Unspecified to UTC.
    /// On read: sets Kind=Utc to ensure correct comparisons with DateTime.UtcNow.
    /// </summary>
    public static readonly ValueConverter<DateTime, DateTime> UtcDateTime =
        new(
            v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(), // Write: ensure UTC
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc) // Read: mark as UTC
        );

    /// <summary>
    /// Converter for nullable DateTime? properties.
    /// Same semantics as UtcDateTime but handles null values.
    /// </summary>
    public static readonly ValueConverter<DateTime?, DateTime?> UtcNullableDateTime =
        new(
            v => v.HasValue 
                ? (v.Value.Kind == DateTimeKind.Utc ? v.Value : v.Value.ToUniversalTime()) 
                : null, // Write: ensure UTC
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null // Read: mark as UTC
        );
}
