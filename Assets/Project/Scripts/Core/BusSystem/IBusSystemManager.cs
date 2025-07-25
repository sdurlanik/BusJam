﻿using Cysharp.Threading.Tasks;
using Sdurlanik.BusJam.MVC.Controllers;

namespace Sdurlanik.BusJam.Core.BusSystem
{
    public interface IBusSystemManager
    {
        IBusController CurrentBus { get; }
        bool IsBusInTransition { get; } 
        void Reset();
    }
}