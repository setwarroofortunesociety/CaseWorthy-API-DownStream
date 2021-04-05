using AutoMapper;
using CW.ClientLibrary.Data;
using CW.ClientLibrary.Models;
using CW.Library.Models;

namespace CW.ClientAPI.Profiles
{
    public class ClientProfile : Profile
    {
        public ClientProfile()
        {
            #region ClientImage
            CreateMap<ClientPhoto, ClientPhotoModel>()
            .ForMember(dest => dest.ClientPhotoID, opt => opt.MapFrom(src => src.ClientPhotoID))
            .ForMember(dest => dest.ClientID, opt => opt.MapFrom(src => src.ClientID))
            .ForMember(dest => dest.PrintPhotoFileID, opt => opt.MapFrom(src => src.PrintPhotoFileID))
            .ForMember(dest => dest.ContextTypeID, opt => opt.MapFrom(src => src.ContextTypeID))
            .ForMember(dest => dest.FileClassification, opt => opt.MapFrom(src => src.FileClassification))
            .ForMember(dest => dest.FileLabel, opt => opt.MapFrom(src => src.FileLabel))
            .ForMember(dest => dest.MimeType, opt => opt.MapFrom(src => src.MimeType))
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
            .ForMember(dest => dest.FileDataLink, opt => opt.MapFrom(src => src.FileDataLink))
            .ForMember(dest => dest.Restriction, opt => opt.MapFrom(src => src.Restriction))
            .ForMember(dest => dest.OwnedByOrgID, opt => opt.MapFrom(src => src.OwnedByOrgID))
            .ForMember(dest => dest.IsEncrypted, opt => opt.MapFrom(src => src.IsEncrypted))
            .ForMember(dest => dest.LastModifiedBy, opt => opt.MapFrom(src => src.LastModifiedBy))
            .ForMember(dest => dest.LastModifiedDate, opt => opt.MapFrom(src => src.LastModifiedDate))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
            .ForMember(dest => dest.OrgGroupID, opt => opt.MapFrom(src => src.OrgGroupID))
            .ForMember(dest => dest.WriteOrgGroupID, opt => opt.MapFrom(src => src.WriteOrgGroupID))
            .ForMember(dest => dest.CreatedFormID, opt => opt.MapFrom(src => src.CreatedFormID))
            .ForMember(dest => dest.LastModifiedFormID, opt => opt.MapFrom(src => src.LastModifiedFormID))
            .ForMember(dest => dest.ImageFileBinary, opt => opt.MapFrom(src => src.ImageBase64))
            .ForMember(dest => dest.UserStamp, opt => opt.MapFrom(src => src.UserStamp))
            .ForMember(dest => dest.DateTimeStamp, opt => opt.MapFrom(src => src.DateTimeStamp))
            .ReverseMap()
            .ForAllOtherMembers(x => x.Ignore());

           

            CreateMap<API_Fortune_ClientPhotos_GET, ClientPhotoModel>() 
            .ForMember(dest => dest.ClientID, opt => opt.MapFrom(src => src.ClientID))
            .ForMember(dest => dest.PrintPhotoFileID, opt => opt.MapFrom(src => src.PrintPhotoFileID))
            .ForMember(dest => dest.ContextTypeID, opt => opt.MapFrom(src => src.ContextTypeID))
            .ForMember(dest => dest.FileClassification, opt => opt.MapFrom(src => src.FileClassification))
            .ForMember(dest => dest.FileLabel, opt => opt.MapFrom(src => src.FileLabel))
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
            .ForMember(dest => dest.MimeType, opt => opt.MapFrom(src => src.MimeType))
            .ForMember(dest => dest.FileDataLink, opt => opt.MapFrom(src => src.FileDataLink))
            .ForMember(dest => dest.Restriction, opt => opt.MapFrom(src => src.Restriction))
            .ForMember(dest => dest.OwnedByOrgID, opt => opt.MapFrom(src => src.OwnedByOrgID))
            .ForMember(dest => dest.IsEncrypted, opt => opt.MapFrom(src => src.IsEncrypted))
            .ForMember(dest => dest.LastModifiedBy, opt => opt.MapFrom(src => src.LastModifiedBy))
            .ForMember(dest => dest.LastModifiedDate, opt => opt.MapFrom(src => src.LastModifiedDate))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
            .ForMember(dest => dest.OrgGroupID, opt => opt.MapFrom(src => src.OrgGroupID))
            .ForMember(dest => dest.WriteOrgGroupID, opt => opt.MapFrom(src => src.WriteOrgGroupID))
            .ForMember(dest => dest.CreatedFormID, opt => opt.MapFrom(src => src.CreatedFormID))
            .ForMember(dest => dest.LastModifiedFormID, opt => opt.MapFrom(src => src.LastModifiedFormID))
            .ForMember(dest => dest.ImageFileBinary, opt => opt.MapFrom(src => src.ImageFileBinary)) 
            .ReverseMap()
            .ForAllOtherMembers(x => x.Ignore());


         
            #endregion

            #region Tracker

            CreateMap<MSG_TrackerModel, MSG_Tracker>()
               .ForMember(dest => dest.MSGID, opt => opt.MapFrom(src => src.MSGID))
               .ForMember(dest => dest.EntityType, opt => opt.MapFrom(src => src.EntityType))
               .ForMember(dest => dest.IntervalID, opt => opt.MapFrom(src => src.IntervalID))
               .ForMember(dest => dest.EntityID, opt => opt.MapFrom(src => src.EntityID))
               .ForMember(dest => dest.ClientID, opt => opt.MapFrom(src => src.ClientID))
               .ForMember(dest => dest.SubEntityID, opt => opt.MapFrom(src => src.SubEntityID))
               .ForMember(dest => dest.SubEntityID2, opt => opt.MapFrom(src => src.SubEntityID2))
               .ForMember(dest => dest.LastModifiedDate, opt => opt.MapFrom(src => src.LastModifiedDate))
               .ForMember(dest => dest.SubTopic, opt => opt.MapFrom(src => src.SubTopic))
               .ForMember(dest => dest.CW_ActionID, opt => opt.MapFrom(src => src.CW_ActionID))
               .ForMember(dest => dest.UserStamp, opt => opt.MapFrom(src => src.UserStamp))
               .ForMember(dest => dest.DateTimeStamp, opt => opt.MapFrom(src => src.DateTimeStamp))
               .ForMember(dest => dest.FIX_ActionID, opt => opt.MapFrom(src => src.FIX_ActionID))
               .ForMember(dest => dest.FIX_ActionDateTimestamp, opt => opt.MapFrom(src => src.FIX_ActionDateTimestamp))
               .ForMember(dest => dest.FIX_Message, opt => opt.MapFrom(src => src.FIX_Message))
               .ReverseMap()
               .ForAllOtherMembers(x => x.Ignore());  

            CreateMap<API_Fortune_EntityTracker_GET, MSG_TrackerModel>()
               .ForMember(dest => dest.EntityType, opt => opt.MapFrom(src => src.EntityType))
               .ForMember(dest => dest.EntityID, opt => opt.MapFrom(src => src.EntityID))
               .ForMember(dest => dest.ClientID, opt => opt.MapFrom(src => src.ClientID))
               .ForMember(dest => dest.SubEntityID, opt => opt.MapFrom(src => src.SubEntityID))
               .ForMember(dest => dest.SubEntityID2, opt => opt.MapFrom(src => src.SubEntityID2))
               .ForMember(dest => dest.LastModifiedDate, opt => opt.MapFrom(src => src.LastModifiedDate))
               .ForMember(dest => dest.SubTopic, opt => opt.MapFrom(src => src.SubTopic))
                .ReverseMap()
               .ForAllOtherMembers(x => x.Ignore());

            CreateMap<MSG_TrackerModel, API_Fortune_EntityTracker_GET>()
             .ForMember(dest => dest.EntityType, opt => opt.MapFrom(src => src.EntityType))
             .ForMember(dest => dest.EntityID, opt => opt.MapFrom(src => src.EntityID))
             .ForMember(dest => dest.ClientID, opt => opt.MapFrom(src => src.ClientID))
             .ForMember(dest => dest.SubEntityID, opt => opt.MapFrom(src => src.SubEntityID))
             .ForMember(dest => dest.SubEntityID2, opt => opt.MapFrom(src => src.SubEntityID2))
             .ForMember(dest => dest.LastModifiedDate, opt => opt.MapFrom(src => src.LastModifiedDate))
             .ForMember(dest => dest.SubTopic, opt => opt.MapFrom(src => src.SubTopic));

            #endregion

            #region Content
            CreateMap<MSG_Content, MSG_ContentModel>()
            .ForMember(dest => dest.ContentID, opt => opt.MapFrom(src => src.ContentID))
            .ForMember(dest => dest.MSGID, opt => opt.MapFrom(src => src.MSGID))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content));

            CreateMap<MSG_ContentModel, MSG_Content>()
           .ForMember(dest => dest.ContentID, opt => opt.MapFrom(src => src.ContentID))
           .ForMember(dest => dest.MSGID, opt => opt.MapFrom(src => src.MSGID))
           .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content));

            #endregion

            #region Fortune Content

            CreateMap<CW.ClientLibrary.Data.FSP_CW_FHIR_Account_Fetch, FSP_CW_FHIR_Account_Fetch_Model>()
            .ForMember(dest => dest.JSONMsg, opt => opt.MapFrom(src => src.JSONMsg));


            CreateMap<FSP_CW_FHIR_Account_Fetch_Model, CW.ClientLibrary.Data.FSP_CW_FHIR_Account_Fetch>()
              .ForMember(dest => dest.JSONMsg, opt => opt.MapFrom(src => src.JSONMsg));

            CreateMap<FSP_CW_FHIR_Organization_Fetch, FSP_CW_FHIR_Organization_Fetch_Model>()
            .ForMember(dest => dest.JSONMsg, opt => opt.MapFrom(src => src.JSONMsg));


            CreateMap<FSP_CW_FHIR_Organization_Fetch_Model, FSP_CW_FHIR_Organization_Fetch>()
              .ForMember(dest => dest.JSONMsg, opt => opt.MapFrom(src => src.JSONMsg));

            #endregion
        }
    }

    internal class FSP_CW_FHIR_Account_Fetch
    {
    }
}
