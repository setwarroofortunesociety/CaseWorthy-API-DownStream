using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CW.ClientLibrary.Data;
using CW.ClientLibrary.Models;
using CW.ClientLibrary.Utility;

namespace CW.ClientLibrary.Services.MessageTracker
{
    public interface IClientImage
    {
    
        IEnumerable<ClientPhoto> GetAll();

        ClientPhoto GetClientImage(Int64 clientPhotoID);

        bool ClientImageExists(int printPhotoID);

        void AddClientImage(ClientPhoto clientImage);

        void UpdateClientImage(ClientPhoto clientImage);

    }
}
