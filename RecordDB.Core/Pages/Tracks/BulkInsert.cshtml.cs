using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecordDB.DAL.Models;
using RecordDB.DAL.Repositories;
using System.Globalization;

namespace RecordDB.Core.Pages.Tracks
{
    public class BulkInsertModel : PageModel
    {
        private readonly ITrackRepository _trackRepository;
        private readonly IDiscRepository _discRepository;
        private readonly ILogger<BulkInsertModel> _logger;

        public BulkInsertModel(
            ITrackRepository trackRepository,
            IDiscRepository discRepository,
            ILogger<BulkInsertModel> logger)
        {
            _trackRepository = trackRepository;
            _discRepository  = discRepository;
            _logger          = logger;
        }

        // ── Bound form properties ────────────────────────────────────────────

        /// <summary>Hidden field — the DiscId chosen via the cascading dropdowns.</summary>
        [BindProperty]
        public int DiscId { get; set; }

        /// <summary>Multi-line textarea with semicolon-delimited track rows.</summary>
        [BindProperty]
        public string? TrackData { get; set; }

        // ── View-only state ──────────────────────────────────────────────────

        /// <summary>Non-null after a successful insert; used to show the success banner.</summary>
        public int? InsertedCount { get; private set; }

        /// <summary>Populated when a parse or validation error occurs.</summary>
        public List<string> ParseErrors { get; } = [];

        // ── GET ──────────────────────────────────────────────────────────────

        public IActionResult OnGet() => Page();

        // ── AJAX: disc/record search handler (same pattern as Create.cshtml) ─

        public async Task<IActionResult> OnGetSearchAsync(string name)
        {
            var discs = await _discRepository.GetDiscRecordsByRecordNameAsync(name ?? string.Empty);
            var result = discs.Select(d => new
            {
                d.RecordId,
                d.DiscId,
                d.ArtistName,
                d.Name,
                d.DiscNo
            });
            return new JsonResult(result);
        }

        // ── POST ─────────────────────────────────────────────────────────────

        public async Task<IActionResult> OnPostAsync()
        {
            // ── 1. Validate DiscId ───────────────────────────────────────────
            if (DiscId <= 0)
            {
                ModelState.AddModelError(string.Empty, "Please select a disc before submitting.");
                return Page();
            }

            // ── 2. Parse the CSV textarea ────────────────────────────────────
            var tracks = ParseTrackData(TrackData, DiscId, ParseErrors);

            if (ParseErrors.Count > 0)
            {
                ModelState.AddModelError(string.Empty,
                    "One or more rows could not be parsed. Please correct them and try again.");
                return Page();
            }

            if (tracks.Count == 0)
            {
                ModelState.AddModelError(string.Empty,
                    "No valid tracks were found in the track data. Please check the format.");
                return Page();
            }

            // ── 3. Check whether this disc already has tracks ────────────────
            int existing = await _trackRepository.CheckForTracksAsync(DiscId);
            if (existing > 0)
            {
                ModelState.AddModelError(string.Empty,
                    $"Disc {DiscId} already contains {existing} track(s). " +
                    "Delete the existing tracks before performing a bulk insert.");
                return Page();
            }

            // ── 4. Bulk insert ───────────────────────────────────────────────
            try
            {
                _logger.LogInformation(
                    "Bulk-inserting {Count} tracks for DiscId {DiscId}", tracks.Count, DiscId);

                await _trackRepository.BulkInsertTracksAsync(tracks);
                InsertedCount = tracks.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bulk insert failed for DiscId {DiscId}", DiscId);
                ModelState.AddModelError(string.Empty,
                    "An error occurred while saving the tracks. Please try again.");
                return Page();
            }

            return RedirectToPage("./Index", new { message = $"{tracks.Count} tracks inserted for disc {DiscId}." });
        }

        // ── CSV parsing ──────────────────────────────────────────────────────

        /// <summary>
        /// Parses the semicolon-delimited textarea.
        /// 
        /// Expected format per row (5 fields):
        ///   DiscNo;TrackNo;Name;TrackLength;Extended
        /// 
        /// • DiscId is taken from the selected disc — it is NOT in the CSV.
        /// • The header row (if present) is skipped automatically.
        /// • TrackLength may be empty, "hh:mm:ss", "mm:ss", or plain seconds.
        /// • Extended may be empty.
        /// • Rows that are entirely blank are skipped silently.
        /// </summary>
        internal static List<Track> ParseTrackData(string? raw, int discId, List<string> errors)
        {
            var tracks = new List<Track>();

            if (string.IsNullOrWhiteSpace(raw))
                return tracks;

            var lines = raw.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                string trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed)) continue;

                // Field 0 — DiscNo   (informational; DiscId is the FK taken from the hidden field)
                // Field 1 — TrackNo
                // Field 2 — Name
                // Field 3 — TrackLength (optional)
                // Field 4 — Extended    (optional)

                var parts = trimmed.Split(';');
                if (parts.Length < 3)
                {
                    errors.Add($"Row has too few fields (expected at least 3): \"{trimmed}\"");
                    continue;
                }

                // Skip a header row whose first field is not a number
                if (!int.TryParse(parts[0].Trim(), out _))
                    continue;

                if (!int.TryParse(parts[1].Trim(), out int trackNo))
                {
                    errors.Add($"Invalid TrackNo \"{parts[1].Trim()}\" on row: \"{trimmed}\"");
                    continue;
                }

                string name = parts[2].Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    errors.Add($"Track name is empty on row: \"{trimmed}\"");
                    continue;
                }

                int? trackLength = null;
                if (parts.Length >= 4 && !string.IsNullOrWhiteSpace(parts[3]))
                {
                    trackLength = ParseTrackLength(parts[3].Trim());
                    if (trackLength == null)
                    {
                        errors.Add($"Cannot parse TrackLength \"{parts[3].Trim()}\" on row: \"{trimmed}\"");
                        continue;
                    }
                }

                string? extended = parts.Length >= 5 ? parts[4].Trim() : null;
                if (string.IsNullOrWhiteSpace(extended))
                    extended = null;

                tracks.Add(new Track
                {
                    DiscId      = discId,
                    TrackNo     = trackNo,
                    Name        = name,
                    TrackLength = trackLength,
                    Extended    = extended
                });
            }

            return tracks;
        }


        /// <summary>
        /// Converts a time string to total seconds.
        /// Accepts: "hh:mm:ss", "mm:ss", or a plain integer string.
        /// Returns null if the value cannot be parsed.
        /// </summary>
        internal static int? ParseTrackLength(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            // Plain integer (already in seconds)
            if (int.TryParse(value, out int plainSeconds))
                return plainSeconds;

            // hh:mm:ss  or  mm:ss
            if (TimeSpan.TryParseExact(value, [@"hh\:mm\:ss", @"mm\:ss", @"h\:mm\:ss", @"m\:ss"],
                CultureInfo.InvariantCulture, out TimeSpan ts))
                return (int)ts.TotalSeconds;

            return null;
        }
    }
}
