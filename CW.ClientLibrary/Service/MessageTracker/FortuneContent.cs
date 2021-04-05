using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CW.ClientLibrary.DbContexts;
using CW.ClientLibrary.Data;
using CW.ClientLibrary.Utility;
using Newtonsoft.Json;
using CW.ClientLibrary.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace CW.ClientLibrary.Services.MessageTracker
{
    public class FortuneContent : IFortuneContent
    {
        private readonly FixContext _context;

        public FortuneContent(FixContext context)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
        }

        #region  FortuneContent

        public String GetOrganization(Int32 organizationID)
        {
        if (organizationID == 0)
        {
            throw new ArgumentNullException(nameof(organizationID));
        }

        int parmOrganizationID = organizationID;

            var col = _context.MSG_Organization
            .FromSqlInterpolated($"EXECUTE fix.fsp_CW_FHIR_Organization_Fetch @OrganizationID={parmOrganizationID}")
            .AsEnumerable()
            .FirstOrDefault()
            .JSONMsg;

         return col;

        }

        public String GetAccount(Int32 accountID)
        {
            if (accountID == 0)
            {
                throw new ArgumentNullException(nameof(accountID));
            }

            int parmAccontID = accountID;

            var col = _context.MSG_Account
            .FromSqlInterpolated($"EXECUTE fix.fsp_CW_FHIR_Account_Fetch @AccountID={parmAccontID}")
            .AsEnumerable()
            .FirstOrDefault()
            .JSONMsg;


            return col;
        }

        #endregion

        public bool Save()
        {
            //Adding an entry
             var AddedEntities = _context.ChangeTracker.Entries()
                             .Where(E => E.State == EntityState.Added)
                             .ToList();

           AddedEntities.ForEach(E =>
            {
                E.Property("DateTimeStamp").CurrentValue = DateTime.UtcNow;
                E.Property("UserStamp").CurrentValue = "AdminCreate";
            });

            //Updating an entry
            var EditedEntities = _context.ChangeTracker.Entries()
                .Where(E => E.State == EntityState.Modified)
                .ToList();

            EditedEntities.ForEach(E =>
            {
                E.Property("DateTimeStamp").CurrentValue = DateTime.UtcNow;
                E.Property("UserStamp").CurrentValue = "AdminUpdate";
            });

            return (_context.SaveChanges() >= 0);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose resources when needed
            }
        }
    }
}

