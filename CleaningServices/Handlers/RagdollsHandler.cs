using System.ComponentModel;
using LabApi.Events.Arguments.PlayerEvents;
using MEC;

namespace CleaningServices.Handlers;

public class RagdollsHandler
{
    public bool IsEnabled { get; set; } = true;
    
    [Description("Time to destroy ragdoll after spawn")]
    public float TimeToDestroy { get; set; } = 180f;
    
    public void OnRagdollSpawned(PlayerSpawnedRagdollEventArgs ev)
    {
        if (!IsEnabled)
            return;
        
        Timing.CallDelayed(TimeToDestroy, () =>
        {
            if (ev.Ragdoll != null)
                ev.Ragdoll.Destroy();
        });
    }
}