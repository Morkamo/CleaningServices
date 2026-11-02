using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Arguments.WarheadEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MapGeneration;
using MEC;

namespace CleaningServices.Handlers;

public class ZonesHandler
{
    private readonly List<CoroutineHandle> _cleanupCoroutines = new();

    public bool IsEnabledLczClean { get; set; } = true;
    public bool IsEnabledWarheadClean { get; set; } = true;
    
    [Description("Time to destroy items and ragdolls")]
    public float TimeToDestroyObjects { get; set; } = 30f;
    
    public void OnLczDecontaminationStarted()
    {
        if (!IsEnabledLczClean)
            return;
        
        _cleanupCoroutines.Add(Timing.RunCoroutine(CleanZone(TimeToDestroyObjects, true)));
    }

    public void OnWarheadDetonated(WarheadDetonatedEventArgs ev)
    {
        if (!IsEnabledWarheadClean)
            return;
        
        _cleanupCoroutines.Add(Timing.RunCoroutine(CleanZone(1f, false)));
    }

    public void OnMapGenerating(MapGeneratingEventArgs ev)
    {
        foreach (var coroutine in _cleanupCoroutines)
            Timing.KillCoroutines(coroutine);

        _cleanupCoroutines.Clear();
    }

    private IEnumerator<float> CleanZone(float delay, bool lczOnly)
    {
        float cleanupTime = Timing.LocalTime + delay;

        while (true)
        {
            if (Round.IsRoundEnded)
                yield break;

            if (Timing.LocalTime >= cleanupTime)
                break;

            yield return Timing.WaitForSeconds(0.5f);
        }

        var ragdolls = Ragdoll.List.Where(rg => Room.TryGetRoomAtPosition(rg.Position, out Room room)
                                               && (lczOnly ? room.Zone == FacilityZone.LightContainment : room.Zone != FacilityZone.Surface));

        var pickups = Pickup.List.Where(pickup => Room.TryGetRoomAtPosition(pickup.GameObject.transform.position, out Room room)
                                                 && (lczOnly ? room.Zone == FacilityZone.LightContainment : room.Zone != FacilityZone.Surface));

        foreach (var ragdoll in ragdolls)
            ragdoll.Destroy();

        foreach (var pickup in pickups)
            pickup.Destroy();

        Logger.Info(lczOnly
            ? "LCZ have been cleared of ragdolls and items."
            : "All zones except the Surface Zone have been cleared of ragdolls and items.");
    }
}