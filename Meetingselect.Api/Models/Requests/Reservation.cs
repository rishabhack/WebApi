using MeetingSelect.Constant;
using System;
using System.Collections.Generic;
using static Meetingselect.Api.Services.CHPIntegrationService;

namespace Meetingselect.Api.Models.Requests
{
    public class Reservation
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public ReservationType Type { get; set; }
        public string Name { get; set; }
        public int Version { get; set; }
        public int ChannelId { get; set; }
        public string ChannelName { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public string LocationImage { get; set; }
        public ReservationStatus StatusId { get; set; }
        public int ProfileId { get; set; }
        public int CustomerId { get; set; }
        public string ProfileName { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int TenderId { get; set; }
        public string ProposalKey { get; set; }
        public int LeadId { get; set; }
        public string LeadName { get; set; }
        public int EventType { get; set; }
        public string EventTypeName { get; set; }
        public int VoucherId { get; set; }
        public string VoucherName { get; set; }
        public int MeetingtypeId { get; set; }
        public int LanguageId { get; set; }
        public int SetId { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencySymbol { get; set; }
        public string CurrencyIso { get; set; }
        public int TotalSeats { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public decimal TotalPaid { get; set; }
        public int HasDepositInvoice { get; set; }
        public int HasInvoice { get; set; }
        public bool NoTax { get; set; }
        public bool NoInvoice { get; set; }
        public string Tags { get; set; }
        public string WorkingOn { get; set; }
        public DateTime BookerEditableUntil { get; set; }
        public DateTime FreeCancellationUntil { get; set; }
        //public string CheckinStatus { get; set; }
        public ReservationTerms ReservationTerms { get; set; }
        public ReservationCancel Cancel { get; set; }
        //public List<Comment> Comments { get; set; }
        //public List<Note> Notes { get; set; }
        //public List<ToDo> ToDos { get; set; }
        //public List<ReservationSet> ReservationSetIds { get; set; }
        public List<ContactCHP> Contacts { get; set; }
        public InvoiceAddress InvoiceAddress { get; set; }
        //public List<ExternalLink> ExternalLinks { get; set; }
        //public Voucher Voucher { get; set; }
        public string PaymentKey { get; set; }
        public bool IsPrivate { get; set; }
        //public List<Date> Dates { get; set; }
        public int StartMinutes { get; set; }
        public int EndMinutes { get; set; }
        public int DaysUntilStart { get; set; }
        public int HoursUntilStart { get; set; }
        public bool HasBreakfast { get; set; }
        public bool HasLunch { get; set; }
        public bool HasDiner { get; set; }
        public bool HasDrink { get; set; }
        public List<TaxTotal> TaxTotals { get; set; }
        public decimal TotalExclTax { get; set; }
        public decimal TotalInclTax { get; set; }
        public decimal AmountToPay { get; set; }
        public decimal TotalOpen { get; set; }
        public List<ExternalMapping> ExternalMappings { get; set; }
        public List<ReservationDate> Dates { get; set; }
        //public List<ReservationOption> Options { get; set; }
        public string Language { get; set; }
        public int RevisionId { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public int EditedBy { get; set; }
        public string EditedByName { get; set; }
        public DateTime EditedOn { get; set; }
        public int HasProposal { get; set; }
        public int HasEvent { get; set; }
        public string ProposalStatus { get; set; }
        public string ProposalPublicationStatus { get; set; }
        public bool PoRequired { get; set; }
        public string CheckinStatus { get; set; }
        public DateTime? Expiration { get; set; }
        public string Voucher { get; set; }
        public List<ExternalLink> ExternalLinks { get; set; }

    }
    public class BookingTerm
    {
        public List<CancelRule> CancelRules { get; set; }
        public int Id { get; set; }
        public string Key { get; set; }
        //public string Type { get; set; }
        public int ChannelId { get; set; }
        public int LocationId { get; set; }
        public string ItemName { get; set; }
        public string Text { get; set; }
        public string Version { get; set; }
        public string Changes { get; set; }
        public int Status { get; set; }
        public long CreatedOn { get; set; }
        public List<Languages> Languages { get; set; }
    }

    public class CancelRule
    {
        public int Id { get; set; }
        public int TermsId { get; set; }
        public int MeetingtypeId { get; set; }
        public int MinSeats { get; set; }
        public int MaxSeats { get; set; }
        public int HoursUntilStart { get; set; }
        public decimal Percentage { get; set; }
    }

    public class ContactCHP
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public ContactType Type { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool ReceiveEmail { get; set; }
    }

    public class ReservationDate
    {
        public int ReservationId { get; set; }
        public DateTime Date { get; set; }
        public int TotalSeats { get; set; }
        public List<ReservationSpace> Spaces { get; set; }
        public List<ReservationOption> Options { get; set; }
        public List<ReservationPackage> Packages { get; set; }
        public int StartMinutes { get; set; }
        public int EndMinutes { get; set; }
        public bool HasBreakfast { get; set; }
        public bool HasLunch { get; set; }
        public bool HasDiner { get; set; }
        public bool HasDrink { get; set; }
        public List<TaxTotal> TaxTotals { get; set; }
        public decimal TotalExclTax { get; set; }
        public decimal TotalInclTax { get; set; }
        public int Id { get; set; }
        public int HasInvoice { get; set; }
        public int HasDepositInvoice { get; set; }
        public string Status { get; set; }
        //public List<object> ProgramItems { get; set; }
    }

    public class ReservationPackage
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public DateTime Date { get; set; }
        public int PackageId { get; set; }
        public string PackageName { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
        public decimal PriceExclTax { get; set; }
        public decimal PriceInclTax { get; set; }
        public decimal TotalExclTax { get; set; }
        public decimal TotalInclTax { get; set; }
        public List<PackageDescription> Descriptions { get; set; }
        public PackageImage Image { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }
        public int MinSeats { get; set; }
        public int MaxSeats { get; set; }
        public int MinHours { get; set; }
        public int MaxHours { get; set; }
    }

    public class PackageDescription
    {
        public int Id { get; set; }
        public int PackageId { get; set; }
        public int LanguageId { get; set; }
        public string Language { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class PackageImage
    {
        public int PackageId { get; set; }
        public int LocationId { get; set; }
        public string Image { get; set; }
        public string UrlSmall { get; set; }
        public string UrlMedium { get; set; }
        public string UrlLarge { get; set; }
        public string UrlOriginal { get; set; }
    }

    public class InvoiceAddress
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public string Address { get; set; }
        public string Postalcode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string SendTo { get; set; }
        public string Email { get; set; }
        public string PoNumber { get; set; }
        public bool PoNumberRequired { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class Languages
    {
        public int Id { get; set; }
        public int TermsId { get; set; }
        public string Language { get; set; }
        public string Terms { get; set; }
        public string Changes { get; set; }
    }

    public class ReservationTerms
    {
        public int Id { get; set; }
        public bool Accepted { get; set; }
        public int ReservationId { get; set; }
        public int TermsId { get; set; }
        public BookingTerm BookingTerm { get; set; }
        public int AcceptedBy { get; set; }
        public string AcceptedByName { get; set; }
        public DateTime AcceptedOn { get; set; }
    }

    public class SpaceConfiguration
    {
        public int Id { get; set; }
        public int SpaceId { get; set; }
        public int SettingId { get; set; }
        public string SettingName { get; set; }
        public int MinSeats { get; set; }
        public int MaxSeats { get; set; }
        public bool IsPublic { get; set; }
    }

    public class ReservationSpace
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public int SpaceId { get; set; }
        public string SpaceName { get; set; }
        public string SpaceImage { get; set; }
        public string ExternalCode { get; set; }
        public int Seats { get; set; }
        public int SettingId { get; set; }
        public List<int> SettingIds { get; set; }
        public List<SpaceConfiguration> Settings { get; set; }
        public DateTime StartDate { get; set; }
        public int StartMinutes { get; set; }
        public DateTime EndDate { get; set; }
        public int EndMinutes { get; set; }
        public int YieldSettingId { get; set; }
        public int TaxId { get; set; }
        public decimal TaxPercentage { get; set; }
        public bool Deductible { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencySymbol { get; set; }
        public string CurrencyIso { get; set; }
        public decimal PricePerSeat { get; set; }
        public decimal PriceTotal { get; set; }
        public PriceCalculationType CalculationType { get; set; }
        public int IsLinked { get; set; }
        public bool IsLocked { get; set; }
        public bool IsPackage { get; set; }
        public bool UseInTotalSeats { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime EditedOn { get; set; }
        public int EditedBy { get; set; }
        public bool InVoucher { get; set; }
        //public List<Note> Notes { get; set; }
        public int Crc { get; set; }
        public string SettingName { get; set; }
        public string Currency { get; set; }
        public int PriceId { get; set; }

        public bool InPackage { get; set; }
        public int VoucherId { get; set; }
        public int PackageId { get; set; }
        public int LinkedToOption { get; set; }
        public bool ShowProposalList { get; set; }
        public bool ShowProposalSummary { get; set; }
        //public List<object> Notes { get; set; }
        public TaxInfo Tax { get; set; }
        public List<SpaceImage> Images { get; set; }
        public List<SpaceDescription> Descriptions { get; set; }
    }

    public class ReservationOption
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public DateTime Date { get; set; }
        public int LocationId { get; set; }
        public int OptionId { get; set; }
        public string InternalName { get; set; }
        public string Name { get; set; }
        public string ExternalCode { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        //public List<OptionCategory> SubCategories { get; set; }
        public bool IsPP { get; set; }
        public bool IsPublic { get; set; }
        public bool IsPerHour { get; set; }
        public bool IsPackage { get; set; }
        public int RequiredItem { get; set; }
        public int Amount { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencySymbol { get; set; }
        public string CurrencyIso { get; set; }
        public decimal BasePrice { get; set; }
        public decimal PricePerItem { get; set; }
        public decimal PriceTotal { get; set; }
        public decimal PriceTotalInclTax { get; set; }
        public int TaxId { get; set; }
        public decimal TaxPercentage { get; set; }
        public bool Deductible { get; set; }
        public decimal MaxPP { get; set; }
        public decimal MaxTotal { get; set; }
        public bool ChangeAllInSet { get; set; }
        public int SpaceId { get; set; }
        public string SpaceName { get; set; }
        public int TimeSelectable { get; set; }
        public int SelectedTime { get; set; }
        public int SortOrder { get; set; }
        public bool InVoucher { get; set; }
        //public List<Note> Notes { get; set; }
        public int CalculationType { get; set; }
        public int Crc { get; set; }
        public string Currency { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public int BillableDuration { get; set; }
        public int Duration { get; set; }
        public int MinimumDuration { get; set; }
        public int MaximumDuration { get; set; }
        public bool UseManualDuration { get; set; }
        public bool UserCanSetDuration { get; set; }
        public int AddShareableSpace { get; set; }
        public bool ShowProposalList { get; set; }
        public bool ShowProposalSummary { get; set; }
        public int NrOfHours { get; set; }
        public int VoucherId { get; set; }
        public int PackageId { get; set; }
        public bool InPackage { get; set; }
        public List<object> Notes { get; set; }
        public TaxInfo Tax { get; set; }
        //doepublic OptionImage Image { get; set; }
        public List<OptionDescription> Descriptions { get; set; }
    }

    public class TaxTotal
    {
        public DateTime Date { get; set; }
        public decimal Percentage { get; set; }
        public bool Deductible { get; set; }
        public decimal Total { get; set; }
        public decimal TaxSubTotal { get; set; }
        public decimal TaxableAmount { get; set; }
    }
    public class ExternalMapping
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public int LocationId { get; set; }
        public string Type { get; set; }
        public int ItemId { get; set; }
        public string Source { get; set; }
        public string Code { get; set; }
        public bool IsDefault { get; set; }
    }
    public class ReservationCancel
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public CancelReason ReasonType { get; set; }
        public string Reason { get; set; }
        public int TermsId { get; set; }
        public int HoursUntilStart { get; set; }
        public float Percentage { get; set; }
        public long CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
    }
    public enum ReservationType
    {
        None = -1,
        Draft = 0,
        Proposal = 1,
        Reservation = 2,
        Request = 3,
        Block = 4 // TODO: Redundant?
    }
    public enum ContactType
    {
        Booker,
        DuringEvent,
        Trainer,
        Speaker,
        Invoice,
        Host,
        Hidden
    }
    public enum ReservationStatus
    {
        None = -100,
        Expired = -2,
        Denied = -1, // Venue refuses the request
        Request = 0,
        Optional = 1,
        Final = 2,
        Cancelled = 3,
        PendingPayment = 4,
        BookerMustConfirm = 5,
        LocationMustConfirm = 6,
        AwaitingPoNumber = 7
    }
    public enum OptionPriceType
    {
        PerItem,
        PerPerson,
        PerDayPart,
        PerPersonPerDayPart
    }
    public enum PriceCalculationType
    {
        PerHourPerSeat,
        PerDayPartOnly,
        PerDayPartPerPerson,
        PerPersonOnly,
        Unknown,
        FixedPrice
    }

    //new class added
    public class TaxInfo
    {
        public DateTime Date { get; set; }
        public decimal Percentage { get; set; }
        public bool Deductible { get; set; }
        public string Currency { get; set; }
        public decimal Total { get; set; }
        public decimal TaxSubTotal { get; set; }
        public decimal TaxableAmount { get; set; }
    }

    public class SpaceDescription
    {
        public int Id { get; set; }
        public int SpaceId { get; set; }
        public int MeetingtypeId { get; set; }
        public int LanguageId { get; set; }
        public string Language { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class SpaceImage
    {
        public int Id { get; set; }
        public int SpaceId { get; set; }
        public int LocationId { get; set; }
        public int MeetingtypeId { get; set; }
        public string Image { get; set; }
        public bool IsDefault { get; set; }
        public string UrlSmall { get; set; }
        public string UrlMedium { get; set; }
        public string UrlLarge { get; set; }
        public string UrlOriginal { get; set; }
    }

    public class OptionDescription
    {
        public int Id { get; set; }
        public int OptionId { get; set; }
        public int LocationId { get; set; }
        public int LanguageId { get; set; }
        public string Language { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class OptionImage
    {
        public int Id { get; set; }
        public int OptionId { get; set; }
        public int LocationId { get; set; }
        public string Image { get; set; }
        public string UrlSmall { get; set; }
        public string UrlMedium { get; set; }
        public string UrlLarge { get; set; }
        public string UrlOriginal { get; set; }
    }

    public class ExternalLink
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Label { get; set; }
    }
}
