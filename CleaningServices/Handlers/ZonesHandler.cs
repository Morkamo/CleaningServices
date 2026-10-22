using System.ComponentModel;
using System.Linq;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Arguments.WarheadEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MapGeneration;
using MEC;

namespace CleaningServices.Handlers;

public class ZonesHandler
{
    public bool IsEnabledLczClean { get; set; } = true;
    public bool IsEnabledWarheadClean { get; set; } = true;
    
    [Description("Time to destroy items and ragdolls")]
    public float TimeToDestroyObjects { get; set; } = 30f;
    
    public void OnLczDecontaminationStarted()
    {
        if (!IsEnabledLczClean)
            return;
        
        Timing.CallDelayed(TimeToDestroyObjects, () =>
        {
            var ragdolls = Ragdoll.List.Where(rg => Room.TryGetRoomAtPosition(rg.Position, out Room room) 
                                                    && room.Zone == FacilityZone.LightContainment);
            
            var pickups = Pickup.List.Where(pickup => Room.TryGetRoomAtPosition(pickup.GameObject.transform.position, out Room room) 
                                                      && room.Zone == FacilityZone.LightContainment);
            
            foreach (var ragdoll in ragdolls)
                ragdoll.Destroy();
            
            foreach (var pickup in pickups)
                pickup.Destroy();
            
            Logger.Info("LCZ have been cleared of ragdolls and items.");
        });
    }

    public void OnWarheadDetonated(WarheadDetonatedEventArgs ev)
    {
        if (!IsEnabledWarheadClean)
            return;
        
        Timing.CallDelayed(5f, () =>
        {
            var ragdolls = Ragdoll.List.Where(rg => Room.TryGetRoomAtPosition(rg.Position, out Room room) 
                                                    && room.Zone != FacilityZone.Surface);
            
            var pickups = Pickup.List.Where(pickup => Room.TryGetRoomAtPosition(pickup.GameObject.transform.position, out Room room) 
                                                    && room.Zone != FacilityZone.Surface);
            
            foreach (var ragdoll in ragdolls)
                ragdoll.Destroy();
            
            foreach (var pickup in pickups)
                pickup.Destroy();
            
            Logger.Info("All zones except the Surface Zone have been cleared of ragdolls and items.");
        });
    }
}