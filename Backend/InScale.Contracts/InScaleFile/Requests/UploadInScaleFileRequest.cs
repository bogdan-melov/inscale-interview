namespace InScale.Contracts.InScaleFile.Requests
{
    using System;
    using System.Collections.Generic;

    public class UploadInScaleFileRequest
    {
        public string FileId { get; set; }

        public string Version { get; set; }

        public List<string> AvailableInRegions { get; set; }

        public List<string> Regions { get; set; }

        public List<string> Channels { get; set; }

        public DateTime AvailableFrom { get; set; }

        public UploadInScaleFileRequest(string fileId,
                                        string version,
                                        List<string> availableInRegions,
                                        List<string> regions,
                                        List<string> channels,
                                        DateTime availableFrom)
        {
            FileId = fileId;
            Version = version;
            AvailableInRegions = availableInRegions;
            Regions = regions;
            AvailableFrom = availableFrom;
            Channels = channels;
        }
    }
}
