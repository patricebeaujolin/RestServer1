﻿using System;
using RestServer1.Domain;

namespace RestServer1.Domain.Abstract
{
    public interface IServiceSettings
    {
        ServiceSettings GetServiceSettings();
    }
}
