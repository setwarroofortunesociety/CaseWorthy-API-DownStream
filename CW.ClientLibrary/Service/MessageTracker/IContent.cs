using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CW.ClientLibrary.Data;
using CW.ClientLibrary.Models;
using CW.ClientLibrary.Utility;

namespace CW.ClientLibrary.Services.MessageTracker
{
    public interface IContent
    {
    
        IEnumerable<MSG_Content> GetAll();

        MSG_Content GetContent(Int64 contentID);

        void AddContent(MSG_Content content);

        void UpdateContent(MSG_Content content);

    }
}
