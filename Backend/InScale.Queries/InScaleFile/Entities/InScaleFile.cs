namespace InScale.Queries.InScaleFile.Entities
{
    using InScale.Queries.Common.Entities;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public class InScaleFile : Entity
    {
        [JsonProperty("fileId")]
        public string FileId;

        [JsonProperty("previousVersion")]
        public string PreviousVersion;

        [JsonProperty("version")]
        public string Version;

        [JsonProperty("filePath")]
        public string FilePath;

        [JsonProperty("availableInRegions")]
        public List<string> AvailableInRegions;

        [JsonProperty("availableFrom")]
        public DateTime AvailableFrom;

        [JsonProperty("channels")]
        public List<string> Channels;

        public InScaleFile(Guid uid,
                           string fileId,
                           string previousVersion,
                           string version,
                           string filePath,
                           List<string> availableInRegions,
                           DateTime availableFrom,
                           List<string> channels) : base(uid)
        {
            FileId = fileId;
            PreviousVersion = previousVersion;
            Version = version;
            FilePath = filePath;
            AvailableInRegions = availableInRegions;
            AvailableFrom = availableFrom;
            Channels = channels;
        }
    }
}