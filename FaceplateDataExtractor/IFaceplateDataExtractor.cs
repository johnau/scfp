﻿using FaceplateDataExtractor.Model;

namespace FaceplateDataExtractor
{
    public interface IFaceplateDataExtractor
    {
        bool HasErrors { get; }
        List<string> Errors { get; }
        bool TryExtractData(int flag, out List<ExtractedFaceplateData> data, out List<ExtractedFaceplateData> rejectedData);
        //List<string> GetErrors();
    }
}
