﻿namespace API_TestProject.WebApi.Model.Request
{
    public class FilterDTO
    {
        public FilterDTO() { }

        public string? From { get; set; }
        public string? To { get; set; }
        public string Search { get; set; }
    }
}
