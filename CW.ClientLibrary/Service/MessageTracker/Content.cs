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

namespace CW.ClientLibrary.Services.MessageTracker
{
    public class Content : IContent
    {
        private readonly FixContext _context;

        public Content(FixContext context)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
        }

        #region Content

        public IEnumerable<MSG_Content> GetAll()
        {
            return _context.MSG_Contents.ToList<MSG_Content>();
        }

        public MSG_Content GetContent(Int64  contentID)
        {
            if(contentID == Int64.MinValue)
            {
                throw new ArgumentNullException(nameof(GetContent));
            }

            return _context.MSG_Contents
                    .Where(i => i.ContentID == contentID).FirstOrDefault();
        }
        public void AddContent(MSG_Content content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(AddContent));
            }
            
                 _context.MSG_Contents.Add(content);

                Save();

        }

        public void UpdateContent(MSG_Content content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(UpdateContent));
            }

                _context.MSG_Contents.Update(content);

                Save();
             
        }

        #endregion

        public bool Save()
        {
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

       