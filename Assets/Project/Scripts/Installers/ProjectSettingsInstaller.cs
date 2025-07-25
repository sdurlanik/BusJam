﻿using Sdurlanik.BusJam.Core.Events;
using Sdurlanik.BusJam.Core.Grid;
using Sdurlanik.BusJam.Project.Scripts.MVC.Models;
using UnityEngine;
using Zenject;

namespace Sdurlanik.BusJam.Installers
{
    public class ProjectSettingsInstaller : MonoInstaller
    {
        [SerializeField] private GridConfiguration _gridConfiguration;
        [SerializeField] private LevelProgressionSO _levelProgression;
        [SerializeField] private BusSettingsSO _busSettings;
        
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<LevelLoadRequestedSignal>();
            Container.DeclareSignal<LevelCompletedSignal>();
            Container.DeclareSignal<LevelReadySignal>();
            Container.DeclareSignal<TimerUpdatedSignal>();
            Container.DeclareSignal<CharacterClickedSignal>();
            Container.DeclareSignal<BusArrivedSignal>();
            Container.DeclareSignal<BusFullSignal>();
            Container.DeclareSignal<GameOverSignal>();
            Container.DeclareSignal<AllBusesDispatchedSignal>();
            Container.DeclareSignal<ResetGameplaySignal>();
            Container.DeclareSignal<NextLevelRequestedSignal>();
            Container.DeclareSignal<StartGameRequestedSignal>();
            Container.DeclareSignal<RestartLevelRequestedSignal>();
            Container.DeclareSignal<WaitingAreaChangedSignal>();
            Container.DeclareSignal<LevelCompleteSequenceFinishedSignal>();
            Container.DeclareSignal<TimeIsUpSignal>();
            Container.DeclareSignal<BusArrivalSequenceStartedSignal>();
            Container.DeclareSignal<CharacterEnteredWaitingAreaSignal>();
            
            Container.Bind<GridConfiguration>().FromInstance(_gridConfiguration).AsSingle();
            Container.Bind<LevelProgressionSO>().FromInstance(_levelProgression).AsSingle();
            Container.Bind<BusSettingsSO>().FromInstance(_busSettings).AsSingle();
        }
    }
}