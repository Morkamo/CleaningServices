using System;
using System.Security.Policy;
using CleaningServices.Handlers;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;
using api = LabApi.Events.Handlers;

namespace CleaningServices
{
    public class Core : Plugin<Config>
    {
        public override string Name { get; } = nameof(CleaningServices);
        public override string Description { get; } = "Props cleaner for FPS boost on server";
        public override string Author { get; } = "Morkamo";
        public override Version Version { get; } = new(1, 0, 0);
        public override Version RequiredApiVersion { get; } = new(LabApiProperties.CompiledVersion);
        
        public static Core Instance { get; private set; }
        public RagdollsHandler RagdollsHandler { get; private set; }
        public ZonesHandler ZonesHandler { get; private set; }
        
        public override void Enable()
        {
            Instance = this;
            RagdollsHandler = Config.RagdollsHandler;
            ZonesHandler = Config.ZonesHandler;
            RegisterEvents();
        }

        public override void Disable()
        {
            ZonesHandler = null;
            RagdollsHandler = null;
            UnregisterEvents();
            Instance = null;
        }

        private void RegisterEvents()
        {
            api.PlayerEvents.SpawnedRagdoll += RagdollsHandler.OnRagdollSpawned;
            api.ServerEvents.LczDecontaminationStarted += ZonesHandler.OnLczDecontaminationStarted;
            api.WarheadEvents.Detonated += ZonesHandler.OnWarheadDetonated;
        }
        
        private void UnregisterEvents()
        {
            api.PlayerEvents.SpawnedRagdoll -= RagdollsHandler.OnRagdollSpawned;
            api.ServerEvents.LczDecontaminationStarted -= ZonesHandler.OnLczDecontaminationStarted;
            api.WarheadEvents.Detonated -= ZonesHandler.OnWarheadDetonated;
        }
    }
}
