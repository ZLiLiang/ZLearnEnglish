﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.DomainCommons.Models
{
    public interface IHasDeletionTime
    {
        DateTime? DeletionTime { get; }
    }
}