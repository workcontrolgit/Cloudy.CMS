﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudy.CMS.SingletonSupport
{
    public class SingletonAttribute : Attribute
    {
        public string Id { get; }

        public SingletonAttribute(string id)
        {
            Id = id;
        }
    }
}
