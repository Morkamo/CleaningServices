using CleaningServices.Handlers;

namespace CleaningServices;

public class Config
{
    public RagdollsHandler RagdollsHandler { get; set; } = new();
    public ZonesHandler ZonesHandler { get; set; } = new();
}