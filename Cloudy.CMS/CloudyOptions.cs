﻿using Poetry.ComponentSupport;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cloudy.CMS
{
    public class CloudyOptions
    {
        public List<Type> Components { get; } = new List<Type>();
        public List<Assembly> ComponentAssemblies { get; } = new List<Assembly>();
        public string DatabaseConnectionString { get; set; }
        public bool HasDocumentProvider { get; set; }
    }
}