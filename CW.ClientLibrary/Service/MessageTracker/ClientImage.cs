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
    public class ClientImage : IClientImage
    {
        private readonly FixContext _context;

        public ClientImage(FixContext context)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
        }

        #region Content

        public IEnumerable<ClientPhoto> GetAll()
        {
            return _context.MSG_ClientPhotos.ToList<ClientPhoto>();
        }

        public ClientPhoto GetClientImage(Int64 clientPhotoID)
        {
            if (clientPhotoID == int.MinValue)
            {
                throw new ArgumentNullException(nameof(GetClientImage));
            }

            return _context.MSG_ClientPhotos
                    .Where(i => i.PrintPhotoFileID == clientPhotoID).FirstOrDefault();
        }
        public void AddClientImage(ClientPhoto clientImage)
        {
            if (clientImage == null)
            {
                throw new ArgumentNullException(nameof(AddClientImage));
            }

            _context.MSG_ClientPhotos.Add(clientImage);

            Save();

        }

        public void UpdateClientImage(ClientPhoto clientImage)
        {
            if (clientImage == null)
            {
                throw new ArgumentNullException(nameof(UpdateClientImage));
            }

            var photo = _context.MSG_ClientPhotos.FirstOrDefault(p => p.PrintPhotoFileID == clientImage.PrintPhotoFileID);

            photo.ClientID           = clientImage.ClientID;
            photo.ContextTypeID      = clientImage.ContextTypeID;
            photo.FileClassification = clientImage.FileClassification;
            photo.FileLabel          = clientImage.FileLabel;
            photo.MimeType           = clientImage.MimeType;
            photo.FileName           = clientImage.FileName;
            photo.FileDataLink       = clientImage.FileDataLink;
            photo.Restriction        = clientImage.Restriction;
            photo.OwnedByOrgID       = clientImage.OwnedByOrgID;
            photo.IsEncrypted        = clientImage.IsEncrypted;
            photo.LastModifiedBy     = clientImage.LastModifiedBy;
            photo.LastModifiedDate   = clientImage.LastModifiedDate;
            photo.CreatedBy          = clientImage.CreatedBy;
            photo.CreatedDate        = clientImage.CreatedDate;
            photo.OrgGroupID         = clientImage.OrgGroupID;
            photo.WriteOrgGroupID    = clientImage.WriteOrgGroupID;
            photo.CreatedFormID      = clientImage.CreatedFormID;
            photo.LastModifiedFormID = clientImage.LastModifiedFormID;
            photo.ImageBase64        = clientImage.ImageBase64;
            photo.LegacyID           = clientImage.LegacyID;

            _context.MSG_ClientPhotos.Update(photo);

            Save();

        }

        /// <summary>
        /// Check if client image exists
        /// </summary>
        /// <returns>true, false</returns>
        public bool ClientImageExists(int printPhotoID)
        {
            bool exist = false;

            if (printPhotoID == 0)
            {
                return exist;
            }

            exist = _context.MSG_ClientPhotos.Any(i => i.PrintPhotoFileID == printPhotoID);

            return exist;

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

