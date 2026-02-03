using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;

namespace PlatformWellSync
{
    public class DataSyncService
    {
        private readonly ApplicationDbContext _context;

        public DataSyncService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void SyncData(JArray platformData)
        {
            if (platformData == null) return;

            int platformsProcessed = 0;
            int wellsProcessed = 0;

            foreach (var item in platformData)
            {
                try
                {
                    var platformId = GetIntValue(item, "id");
                    var platformName = GetStringValue(item, "uniqueName");
                    
                    if (!platformId.HasValue) continue;

                    var existingPlatform = _context.Platforms
                        .FirstOrDefault(p => p.Id == platformId.Value);

                    if (existingPlatform != null)
                    {
                        existingPlatform.PlatformName = platformName;
                        existingPlatform.PlatformId = platformId.Value;
                        existingPlatform.Latitude = GetDoubleValue(item, "latitude");
                        existingPlatform.Longitude = GetDoubleValue(item, "longitude");
                        existingPlatform.CreatedAt = GetDateTimeValue(item, "createdAt");
                        existingPlatform.UpdatedAt = GetDateTimeValue(item, "updatedAt");
                    }
                    else
                    {
                        // Insert new platform
                        var newPlatform = new Platform
                        {
                            Id = platformId.Value,
                            PlatformName = platformName,
                            PlatformId = platformId.Value,
                            Latitude = GetDoubleValue(item, "latitude"),
                            Longitude = GetDoubleValue(item, "longitude"),
                            CreatedAt = GetDateTimeValue(item, "createdAt"),
                            UpdatedAt = GetDateTimeValue(item, "updatedAt")
                        };
                        _context.Platforms.Add(newPlatform);
                    }
                    
                    platformsProcessed++;

                    var wells = item["well"] as JArray;
                    if (wells != null)
                    {
                        foreach (var wellItem in wells)
                        {
                            try
                            {
                                var wellId = GetIntValue(wellItem, "id");
                                if (!wellId.HasValue) continue;

                                var existingWell = _context.Wells
                                    .FirstOrDefault(w => w.Id == wellId.Value);

                                if (existingWell != null)
                                {
                                    existingWell.UniqueName = GetStringValue(wellItem, "uniqueName");
                                    existingWell.Latitude = GetDoubleValue(wellItem, "latitude");
                                    existingWell.Longitude = GetDoubleValue(wellItem, "longitude");
                                    existingWell.CreatedAt = GetDateTimeValue(wellItem, "createdAt");
                                    existingWell.UpdatedAt = GetDateTimeValue(wellItem, "updatedAt");
                                    existingWell.PlatformId = platformId.Value;
                                }
                                else
                                {
                                    var newWell = new Well
                                    {
                                        Id = wellId.Value,
                                        UniqueName = GetStringValue(wellItem, "uniqueName"),
                                        Latitude = GetDoubleValue(wellItem, "latitude"),
                                        Longitude = GetDoubleValue(wellItem, "longitude"),
                                        CreatedAt = GetDateTimeValue(wellItem, "createdAt"),
                                        UpdatedAt = GetDateTimeValue(wellItem, "updatedAt"),
                                        PlatformId = platformId.Value
                                    };
                                    _context.Wells.Add(newWell);
                                }
                                
                                wellsProcessed++;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"⚠ Error processing well: {ex.Message}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠ Error processing platform: {ex.Message}");
                }
            }

            _context.SaveChanges();
            Console.WriteLine($"✓ Synced {platformsProcessed} platforms and {wellsProcessed} wells");
        }

        private int? GetIntValue(JToken token, string key)
        {
            try
            {
                return token[key]?.Value<int>();
            }
            catch
            {
                return null;
            }
        }

        private string GetStringValue(JToken token, string key)
        {
            try
            {
                return token[key]?.Value<string>();
            }
            catch
            {
                return null;
            }
        }

        private double? GetDoubleValue(JToken token, string key)
        {
            try
            {
                return token[key]?.Value<double>();
            }
            catch
            {
                return null;
            }
        }

        private DateTime? GetDateTimeValue(JToken token, string key)
        {
            try
            {
                return token[key]?.Value<DateTime>();
            }
            catch
            {
                return null;
            }
        }
    }
}