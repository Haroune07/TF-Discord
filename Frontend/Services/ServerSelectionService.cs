using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class ServerSelectionService
{
    // L'événement auquel la liste de salons va s'abonner
    public static event Action<int>? OnServerChanged;

    // La méthode que le serveur va appeler
    public static void SelectServer(int serverId)
    {
        OnServerChanged?.Invoke(serverId);
    }
}