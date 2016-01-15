/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using NR.Localication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace NR.Models
{
    public enum SMSType
    {
        Logininfo = 0,
        TurHusk = 1,
        UserSMS = 2
    }

    public enum PossibleCountry
    {
        DK = 45,
        SE = 46,
        FO = 298,
        GL = 299
    }


    public enum Country
    {
        [Display(Name = "CountryDK", ResourceType = typeof(DomainStrings))]
        DK = 45,

        [Display(Name = "CountrySE", ResourceType = typeof(DomainStrings))]
        SE = 46,

        [Display(Name = "CountryFO", ResourceType = typeof(DomainStrings))]
        FO = 298,

        [Display(Name = "CountryGL", ResourceType = typeof(DomainStrings))]
        GL = 299
    }

    public enum SMSStatus
    {
        Afventer = 0,
        Sendt = 1,
        Fejl = 2,
        Buffered = 4,
        Afbrudt = 8
    }

    public enum Gender
    {
        [Display(Name = "GenderNotDefined", ResourceType = typeof(DomainStrings))]
        NotDefined = 0,

        [Display(Name = "GenderMan", ResourceType = typeof(DomainStrings))]
        Man = 1,

        [Display(Name = "GenderWoman", ResourceType = typeof(DomainStrings))]
        Woman = 2
    }

    public enum FolderStatus
    {
        [Display(Name = "FolderStatusNew", ResourceType = typeof(DomainStrings))]
        New = 0,

        [Display(Name = "FolderStatusInput", ResourceType = typeof(DomainStrings))]
        Input = 1,

        [Display(Name = "FolderStatusPubliceret", ResourceType = typeof(DomainStrings))]
        Published = 2,

        [Display(Name = "FolderStatusArchived", ResourceType = typeof(DomainStrings))]
        Archived = 3
    }

    public enum TeamStatus
    {
        [Display(Name = "TeamStatus_Planned", ResourceType = typeof(DomainStrings))]
        Planned = 0,

        [Display(Name = "TeamStatus_OK", ResourceType = typeof(DomainStrings))]
        OK = 1,

        [Display(Name = "TeamStatus_Droped", ResourceType = typeof(DomainStrings))]
        Droped = 2,

        [Display(Name = "TeamStatus_Cancelled", ResourceType = typeof(DomainStrings))]
        Cancelled = 3
    }

    public enum AssociationStatus
    {
        [Display(Name = "AssociationStatusActive", ResourceType = typeof(DomainStrings))]
        Active = 0,

        [Display(Name = "AssociationStatusDorment", ResourceType = typeof(DomainStrings))]
        Dormant = 1,

        [Display(Name = "AssociationStatusClosed", ResourceType = typeof(DomainStrings))]
        Closed = 2
    }

    public enum MessageType
    {
        shortMessage = 0,
        LongMessage = 1
    }

    public enum LevelType
    {
        [Display(Name = "LevelTypeLocal", ResourceType = typeof(DomainStrings))]
        Local = 0,

        [Display(Name = "LevelTypeNetvork", ResourceType = typeof(DomainStrings))]
        Network = 1,

        [Display(Name = "LevelTypeNational", ResourceType = typeof(DomainStrings))]
        National = 2,

        [Display(Name = "LevelTypeGlobaly", ResourceType = typeof(DomainStrings))]
        Globaly = 3
    };

    public enum PersonType
    {
        [Display(Name = "PersonTypeActive", ResourceType = typeof(DomainStrings))]
        Active = 1,

        [Display(Name = "PersonTypeSupport", ResourceType = typeof(DomainStrings))]
        Support = 2,

        [Display(Name = "PersonTypePassiv", ResourceType = typeof(DomainStrings))]
        Passiv = 0,

        [Display(Name = "PersonTypeVIP", ResourceType = typeof(DomainStrings))]
        VIP = 3
    };

    public enum GovernanceType
    {
        [Display(Name = "GovernanceTypeTraditionel", ResourceType = typeof(DomainStrings))]
        Traditionel = 0,

        [Display(Name = "GovernanceTypeSteeringgroup", ResourceType = typeof(DomainStrings))]
        Steeringgroup = 1
    }

    public enum BoardFunctionType
    {
        [Display(Name = "BoardFunctionTypeNone", ResourceType = typeof(DomainStrings))]
        none =0,

        [Display(Name = "BoardFunctionTypeChairman", ResourceType = typeof(DomainStrings))]
        Chairman = 1,

        [Display(Name = "BoardFunctionTypeBoardMember", ResourceType = typeof(DomainStrings))]
        BoardMember = 2,

        [Display(Name = "BoardFunctionTypeAlternate", ResourceType = typeof(DomainStrings))]
        Alternate = 3,

        [Display(Name = "BoardFunctionTypeAccountant", ResourceType = typeof(DomainStrings))]
        Accountant = 4,

        [Display(Name = "BoardFunctionTypeAuditor", ResourceType = typeof(DomainStrings))]
        Auditor = 5,

        [Display(Name = "BoardFunctionTypeAuditorAlternate", ResourceType = typeof(DomainStrings))]
        AuditorAlternate = 6
    }

    public enum NotificationType
    {
        [Display(Name = "NotificationTypeUnknown", ResourceType = typeof(Icons))]
        Unknown = 0,

        [Display(Name = "NotificationTypePerson", ResourceType = typeof(Icons))]
        Person = 1,

        [Display(Name = "NotificationTypeAssociation", ResourceType = typeof(Icons))]
        Association = 2,

        [Display(Name = "NotificationTypeFolder", ResourceType = typeof(Icons))]
        Folder = 3,

        [Display(Name = "NotificationTypeTeam", ResourceType = typeof(Icons))]
        Team = 4,

        [Display(Name = "NotificationTypeEvent", ResourceType = typeof(Icons))]
        Event = 5,

        [Display(Name = "NotificationTypeWiki", ResourceType = typeof(Icons))]
        Wiki = 6,

        [Display(Name = "NotificationTypeContent", ResourceType = typeof(Icons))]
        Content = 8,

        [Display(Name = "NotificationTypeAssociationNote", ResourceType = typeof(Icons))]
        AssociationNote = 9,

        [Display(Name = "NotificationTypeNewVersion", ResourceType = typeof(Icons))]
        NewVersion = 99

    }

    public enum MinutesType
    {
        [Display(Name = "MinutesTypeBoardmeeting", ResourceType = typeof(DomainStrings))]
        Boardmeeting = 0,

        [Display(Name = "MinutesTypeGeneralMeeting", ResourceType = typeof(DomainStrings))]
        GeneralMeeting = 1,

        [Display(Name = "MinutesTypeExtraordinaryGeneralMeeting", ResourceType = typeof(DomainStrings))]
        ExtraordinaryGeneralMeeting = 1,
    }


    public enum LeadStatus
    {
       [Display(Name = "LeadStatusNew", ResourceType = typeof(DomainStrings))]
       New = 0,

       [Display(Name = "LeadStatusAcknowledge", ResourceType = typeof(DomainStrings))]
       Acknowledge = 5,

       [Display(Name = "LeadStatusClarification", ResourceType = typeof(DomainStrings))]
       Clarification = 7,

       [Display(Name = "LeadStatusAssigned", ResourceType = typeof(DomainStrings))]
       Assigned = 10,

       [Display(Name = "LeadStatusRecived", ResourceType = typeof(DomainStrings))]
       Recived = 11,

       [Display(Name = "LeadStatusWaiting", ResourceType = typeof(DomainStrings))]
       Waiting = 12,

       [Display(Name = "LeadStatusTrial", ResourceType = typeof(DomainStrings))]
       Trial = 15,

       [Display(Name = "LeadStatusSucces", ResourceType = typeof(DomainStrings))]
       Succes = 40,

       [Display(Name = "LeadStatusDroped", ResourceType = typeof(DomainStrings))]
       Droped = 50,

       [Display(Name = "LeadStatusDropedTrial", ResourceType = typeof(DomainStrings))]
       DropedTrial = 60,

       [Display(Name = "LeadStatusNotSuited", ResourceType = typeof(DomainStrings))]
       NotSuited = 70,

       [Display(Name = "LeadStatusSPAM", ResourceType = typeof(DomainStrings))]
       SPAM = 99,

    }


    public enum LeadStatusAssociation
    {
       
        [Display(Name = "LeadStatusAssigned", ResourceType = typeof(DomainStrings))]
        Assigned = 10,

        [Display(Name = "LeadStatusRecived", ResourceType = typeof(DomainStrings))]
        Recived = 11,

        [Display(Name = "LeadStatusWaiting", ResourceType = typeof(DomainStrings))]
        Waiting = 12,

        [Display(Name = "LeadStatusTrial", ResourceType = typeof(DomainStrings))]
        Trial = 15,

        [Display(Name = "LeadStatusSucces", ResourceType = typeof(DomainStrings))]
        Succes = 40,

        [Display(Name = "LeadStatusDroped", ResourceType = typeof(DomainStrings))]
        Droped = 50,

        [Display(Name = "LeadStatusDropedTrial", ResourceType = typeof(DomainStrings))]
        DropedTrial = 60,

        [Display(Name = "LeadStatusNotSuited", ResourceType = typeof(DomainStrings))]
        NotSuited = 70,

        [Display(Name = "LeadStatusSPAM", ResourceType = typeof(DomainStrings))]
        SPAM = 99,

    }
}
