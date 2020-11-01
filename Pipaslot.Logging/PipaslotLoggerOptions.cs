﻿// ReSharper disable UnusedAutoPropertyAccessor.Global

using System;

namespace Pipaslot.Logging
{
    public class PipaslotLoggerOptions
    {
        public string OutputPath { get; set; } = "";
        public bool IncludeScopes { get; set; }
        public bool IncludeMethods { get; set; }
    }
}