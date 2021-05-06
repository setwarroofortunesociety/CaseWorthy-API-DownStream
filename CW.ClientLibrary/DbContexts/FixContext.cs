using Microsoft.EntityFrameworkCore;
using CW.ClientLibrary.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;

namespace CW.ClientLibrary.DbContexts
{
    public class FixContext : DbContext
    {
        public FixContext()
        {
        }
        public FixContext(DbContextOptions<FixContext> options)
            :base(options)
        {
        }

        //Tables
        public virtual DbSet<MSG_Interval> MSG_Intervals { get; set; }
        public virtual DbSet<MSG_Tracker> MSG_Trackers { get; set; }
        public virtual DbSet<MSG_Content> MSG_Contents { get; set; }
        public virtual DbSet<ClientPhoto> MSG_ClientPhotos { get; set; }

        //View
        public virtual DbSet<VW_Interval_MaxDateTime> VW_Interval_MaxDateTime { get; set; }
        public virtual DbSet<VW_Interval_HasPendingStatus> VW_Interval_HasPendingStatus { get; set; }


        //Store Procedures
        public virtual DbSet<FSP_CW_FHIR_Organization_Fetch> MSG_Organization { get; set; }
        public virtual DbSet<FSP_CW_FHIR_Account_Fetch> MSG_Account { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //specify default schema
            //modelBuilder.HasDefaultSchema("cw");


            modelBuilder.Entity<VW_Interval_MaxDateTime>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("Interval_MaxDateTime", "cw");

                entity.Property(e => e.EndDateTime)
                .HasColumnType("datetime");

            });

            modelBuilder.Entity<VW_Interval_HasPendingStatus>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("Interval_HasPendingStatus", "cw");

                entity.Property(e => e.IsPending).HasColumnType("bool");

            });

            modelBuilder.Entity<MSG_Content>(entity =>
            {
                entity.HasKey(e => e.ContentID);

                entity.ToTable("MSG_Content","cw");
                
                entity.HasOne<MSG_Tracker>(m => m.MsgTracker).WithOne().HasForeignKey<MSG_Content>(FK => FK.MSGID);

                entity.Property(e => e.ContentID).HasColumnName("ContentID");

                entity.Property(e => e.MSGID).HasColumnName("MSGID");

                entity.Property(e => e.Content)
                    .IsUnicode(false);

            });
            
            modelBuilder.Entity<MSG_Interval>(entity =>
            {
                entity.HasKey(e => e.IntervalID);

                entity.ToTable("MSG_Interval","cw");

                entity.Property(e => e.IntervalID).HasColumnName("IntervalID");

                entity.Property(e => e.StartDateTime).HasColumnType("datetime");

                entity.Property(e => e.EndDateTime).HasColumnType("datetime");

                entity.Property(e => e.EntityType)
                   .HasMaxLength(50)
                   .IsUnicode(false);

                entity.Property(e => e.Status).HasColumnName("Status");

                entity.Property(e => e.Comments)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.UserStamp)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DateTimeStamp)
                    .HasColumnType("datetime");

              
            });

            modelBuilder.Entity<MSG_Tracker>(entity =>
            {
                entity.HasKey(e => e.MSGID);


                entity.ToTable("MSG_Tracker","cw");

                entity.Property(e => e.MSGID).HasColumnName("MSGID");

                entity.HasOne(p => p.MsgInterval).WithMany().HasForeignKey(FK => FK.IntervalID);

                entity.Property(e => e.IntervalID).HasColumnName("IntervalID");

                entity.Property(e => e.EntityType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ClientID).HasColumnName("ClientID");

                entity.Property(e => e.EntityID).HasColumnName("EntityID");

                entity.Property(e => e.SubEntityID).HasColumnName("SubEntityID");

                entity.Property(e => e.SubEntityID2).HasColumnName("SubEntityID2");

                entity.Property(e => e.LastModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.SubTopic)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CW_ActionID).HasColumnName("CW_ActionID");

                entity.Property(e => e.CW_ErrorMessage)
                 .IsUnicode(false);

                entity.Property(e => e.UserStamp)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DateTimeStamp)
                    .HasColumnType("datetime");

                entity.Property(e => e.FIX_ActionDateTimestamp).HasColumnType("datetime");

                entity.Property(e => e.FIX_Message)
                   .HasMaxLength(100)
                   .IsUnicode(false);
            });

            modelBuilder.Entity<ClientPhoto>(entity =>
            {
                entity.HasKey(e => e.ClientPhotoID);

                entity.ToTable("ClientPhoto", "cw");

                entity.Property(e => e.ClientPhotoID).HasColumnName("ClientPhotoID");

                entity.Property(e => e.ClientID).HasColumnName("ClientID");

                entity.Property(e => e.PrintPhotoFileID).HasColumnName("PrintPhotoFileID");

                entity.Property(e => e.ContextTypeID).HasColumnName("ContextTypeID");

                entity.Property(e => e.FileClassification).HasColumnName("FileClassification");

                entity.Property(e => e.FileLabel)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.MimeType)
                      .HasMaxLength(200)
                     .IsUnicode(false);

                entity.Property(e => e.FileName)
                      .HasMaxLength(200)
                      .IsUnicode(false);

                entity.Property(e => e.FileDataLink)
                   .HasMaxLength(50)
                   .IsUnicode(false);

              //  entity.Property(e => e.Restriction).HasColumnName("Restriction");

                entity.Property(e => e.OwnedByOrgID).HasColumnName("OwnedByOrgID");

                entity.Property(e => e.IsEncrypted).HasColumnType("bool");

                entity.Property(e => e.LastModifiedBy).HasColumnName("LastModifiedBy");
                
                entity.Property(e => e.LastModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedBy).HasColumnName("CreatedBy");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.OrgGroupID).HasColumnName("OrgGroupID");

                entity.Property(e => e.CreatedFormID).HasColumnName("CreatedFormID");

                entity.Property(e => e.LastModifiedFormID).HasColumnName("LastModifiedFormID");
                
                entity.Property(e => e.ImageBase64)
                    .IsUnicode(false);

                entity.Property(e => e.LegacyID).HasColumnName("LegacyID");

                entity.Property(e => e.DateTimeStamp)
                    .HasColumnType("datetime");


            });

            modelBuilder.Entity<FSP_CW_FHIR_Organization_Fetch>(entity =>
            {

                entity.HasNoKey();

                entity.Property(e => e.JSONMsg);


            });

            modelBuilder.Entity<FSP_CW_FHIR_Account_Fetch>(entity =>
            {

                entity.HasNoKey();

                entity.Property(e => e.JSONMsg);


            });

        }

    }
}
