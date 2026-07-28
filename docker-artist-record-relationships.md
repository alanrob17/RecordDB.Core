# Docker - Artist & Record  models

Can you look at my artist and record models in RecordDB.DAL and tell me how I can set these models so that Artist can access Record and Record can access Artist?

Your models already have a **bidirectional navigation relationship** configured correctly! Here's a breakdown of how they're set up and how the two-way access works:

---

## Current Relationship Setup

### `Artist` → accesses `Record` (One-to-Many)

In Artist.cs:

```csharp

// Constructor initializes the collection
public Artist() => Records = new HashSet<Record>();

// Navigation property: Artist has many Records
[InverseProperty(nameof(Record.Artist))]
public virtual ICollection<Record> Records { get; set; }
```

This lets you do:

```csharp
var records = artist.Records; // All records for an artist
```

---

### `Record` → accesses `Artist` (Many-to-One)

In Record.cs:

```csharp

// Foreign key
public int ArtistId { get; set; }

// Navigation property: Record belongs to one Artist
[ForeignKey(nameof(ArtistId))]
[InverseProperty("Records")]
public virtual Artist? Artist { get; set; }
```

This lets you do:

```csharp
var artist = record.Artist; // The artist for this record
```

---

## How the Attributes Wire It Together

|Attribute|Location|Purpose|
|---|---|---|
|`[ForeignKey(nameof(ArtistId))]`|`Record.Artist`|Tells EF Core that `ArtistId` is the FK for this navigation|
|`[InverseProperty("Records")]`|`Record.Artist`|Points back to `Artist.Records` as the other end|
|`[InverseProperty(nameof(Record.Artist))]`|`Artist.Records`|Points back to `Record.Artist` as the other end|

---

## One thing to note

For **eager loading** to work (i.e., for the navigation properties to be populated), you need to use `.Include()` in your queries:

```csharp

// Load artist WITH their records
var artist = context.Artists
    .Include(a => a.Records)
    .FirstOrDefault(a => a.ArtistId == id);

// Load record WITH its artist
var record = context.Records
    .Include(r => r.Artist)
    .FirstOrDefault(r => r.RecordId == id);
```

Without `.Include()`, the navigation property will be `null` unless you have lazy loading enabled (which requires the `virtual` keyword — which you already have on both properties ✅).

---

In summary, your models are **already correctly configured** for bidirectional access. The `virtual` keyword on both navigation properties also means **lazy loading will work** if it's enabled in your `DbContext` configuration.
