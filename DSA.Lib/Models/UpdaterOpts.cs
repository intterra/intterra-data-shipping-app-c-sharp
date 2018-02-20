﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DSA.Lib.Models
{
    public class UpdaterOpts
    {
        public Guid CurrentProfileId { get; set; }
        public IReadOnlyList<string> DbDrivers { get; } = new List<string>(new[] { "mssql", "odbc" }).AsReadOnly();
        public IReadOnlyList<string> ProfileTypes { get; } = new List<string>(new[] { "analytics", "sitstat" }).AsReadOnly();
        public ObservableCollection<UpdaterProfile> Profiles { get; } = new ObservableCollection<UpdaterProfile>();
        public string LogUrl { get; set; }
        public bool RemoteLogging { get; set; } = true;

        [JsonIgnore]
        public string SavedOn { get; set; } = "";
        [JsonIgnore]
        public UpdaterProfile CurrentProfile { get; set; }
        [JsonIgnore]
        public bool CurrentProfileNotNull { get; set; } = false;

        public UpdaterOpts()
        {
            if (string.IsNullOrWhiteSpace(LogUrl))
            {
#if DEBUG
                LogUrl = "https://portal-dev.intterragroup.com/api/logs/create";
#else
                LogUrl = "https://portal.intterragroup.com/api/logs/create";
#endif
            }
        }
    }

    public class UpdaterProfile
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Type { get; set; }
        public string LastDatetimeUrl { get; set; }
        public string DataUrl { get; set; }
        public string TestUrl { get; set; }

        public int RunInterval { get; set; }
        public string RunIntervalTimeUnit { get; set; }
        [JsonIgnore]
        public DateTime RunStartTime { get; set; } = DateTime.Now;

        public string Driver { get; set; }
        public string ConnectionString { get; set; }
        public string ApiKey { get; set; }
        public string ApiKeySecret { get; set; }
        public string Agency { get; set; }
        public bool AllowDuplication { get; set; } = false;
        public string IncidentsQuery { get; set; }
        public string UnitsQuery { get; set; }

        public string GetIncidentsQuery(DateTime? lastUpdatedOn)
        {
            return ReplacePlaceQueryPlaceholders(IncidentsQuery, lastUpdatedOn);
        }

        public string GetUnitsQuery(DateTime? lastUpdatedOn)
        {
            return ReplacePlaceQueryPlaceholders(UnitsQuery, lastUpdatedOn);
        }

        private string ReplacePlaceQueryPlaceholders(string query, DateTime? lastUpdatedOn)
        {
            if (lastUpdatedOn != null)
            {
                query = query.Replace("{{LASTUPDATEDDATETIME}}", ((DateTime)lastUpdatedOn).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            }

            return query;
        }

        public bool UsesLastUpdatedDatetime()
        {
            return (new[] { IncidentsQuery, UnitsQuery})
                .Where(x => !string.IsNullOrWhiteSpace(x)) // filter out empty queries
                .Any(x => x.Contains("{{LASTUPDATEDDATETIME}}"));
        }
    }

    public class UpdaterResponse
    {
        public int SentIncidents { get; set; }
        public int IgnoredIncidents { get; set; }
        public int SentUnits { get; set; }
        public int IgnoredUnits { get; set; }
        public Guid TransactionId { get; set; }

        public override string ToString()
        {
            return $"Batch uuid: {(TransactionId != Guid.Empty ? TransactionId.ToString() : "N/A")}\n\nSent Incidents: {SentIncidents}\nIgnored Incidents: {IgnoredIncidents}\nSent Units: {SentUnits}\nIgnored Units: {IgnoredUnits}";
        }
    }
}
