﻿using System.Collections.Generic;
using RiskOfOptions.Events;
using UnityEngine.Events;

namespace RiskOfOptions.Interfaces
{
    internal interface IIntProvider
    {
        public IntEvent OnValueChanged { get; set; }

        public int Value { get; set; }
    }
}