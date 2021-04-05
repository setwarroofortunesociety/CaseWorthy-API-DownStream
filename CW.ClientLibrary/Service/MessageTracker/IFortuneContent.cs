using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CW.ClientLibrary.Data;
using CW.ClientLibrary.Models;
using CW.ClientLibrary.Utility;

namespace CW.ClientLibrary.Services.MessageTracker
{
    public interface IFortuneContent
    {


       String GetOrganization(Int32 organizationID);

        String GetAccount (Int32 accouontID);

       // bool Save();
    }
}
