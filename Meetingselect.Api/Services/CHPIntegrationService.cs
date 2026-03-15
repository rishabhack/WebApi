using LoyaltyPlatform.Infrastructure.Caching;
using LoyaltyPlatform.Infrastructure.Helpers;
using Meetingselect.Api.Models.Requests;
using MeetingSelect.Block.DataExt;
using MeetingSelect.CommonBiz;
using MeetingSelect.Constant;
using MeetingSelect.Domain.Aggregations;
using MeetingSelect.Domain.Dtos;
using MeetingSelect.Domain.Services;
using MeetingSelect.Enum;
using MeetingSelect.PlannerBiz.CompareProposals;
using Microsoft.Web.Services3.Addressing;
using Ms.Domain.Enums;
using Ms.Domain.Repository;
using Ms.Domain.Service;
using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
namespace Meetingselect.Api.Services
{
    public class CHPIntegrationService
    {
        readonly MeetingSelectContext _dbContext;
        readonly CHPSentRecordService _hivrSentRecordService;
        public CHPIntegrationService(MeetingSelectContext dbContext)
        {
            _dbContext = dbContext;
            var bdProvider = new BusinessDataProvider();
            _hivrSentRecordService = new CHPSentRecordService(bdProvider);
        }
        public void DeclineRfp(int venueId, int rfpId, string ReasonType, string Reason)
        {
            //if (!TryParseCancelReason(ReasonType, out var reason))
            //{
            //    reason = CancelReason.Other; // or default to Other
            //}
            string TurnDownReason = ReasonType == "Other" ? Reason : ReasonType;
            var venueDetails = _dbContext.Venues.Where(i => i.VenueId == venueId).FirstOrDefault();
            if (venueDetails == null)
            {
                venueDetails = new Venue();
            }
            var userSupplier = GetUserSupplier(venueId);
            string comment = GetComment(rfpId, venueDetails, TurnDownReason);
            int? finalproposalId = _dbContext.Proposals.Where(pp => pp.RFPID == rfpId).Select(pp => (int?)pp.ProposalID).FirstOrDefault();
            MeetingSelect.PlannerBiz.CommentChangeStatusBiz.AddComment(venueDetails.VenueId, finalproposalId, rfpId, comment, "DeclineRFPBySupplierRFPIntegrationService", "DeclineRFPBySupplierRFPIntegrationService", TurnDownReason, DateTime.Now);
            if (userSupplier.UserID > 0)
            {
                SentEmail(rfpId, venueId, userSupplier.UserID, TurnDownReason);
            }
            RfpLogComment.WriteRfpLog("decline by supplier CHPIntegration", rfpId, venueId, null, LogType.SupplierDecline.ToString());
            SetActionNote(venueDetails.VenueName, rfpId, TurnDownReason, venueDetails.EmailForHotelReservation);
            var responcestatusupdate = _dbContext.RFPVenues.Where(rs => rs.RFPID == rfpId && rs.VenueID == venueId).FirstOrDefault();
            if (responcestatusupdate != null)
            {
                responcestatusupdate.ResponseStatus = 3;
            }
            _dbContext.SubmitChanges();
        }
        public int? GetCompanyId(string token)
        {
            int? companyId = null;
            var keys = token.Split(new string[] { " " }, StringSplitOptions.None);
            if (keys.Length > 1)
            {
                var key = keys[1];
                if (!string.IsNullOrEmpty(key))
                {
                    var dataProvider = new BusinessDataProvider();
                    companyId = new ApiTokenService(dataProvider).Query().Where(k => k.Token == key && (k.Category == (int)ApiTokenCategory.Company
                    || k.Category == (int)ApiTokenCategory.Chain || k.Category == (int)ApiTokenCategory.Venue))
                                .Select(k => k.CompanyId).FirstOrDefault();
                }
            }
            return companyId;
        }
        public TokenInfo GetIdAndCategory(string token)
        {
            var keys = token.Split(new string[] { " " }, StringSplitOptions.None);
            if (keys.Length > 1)
            {
                var key = keys[1];
                if (!string.IsNullOrEmpty(key))
                {
                    var dataProvider = new BusinessDataProvider();
                    var response = new ApiTokenService(dataProvider).Query().Where(k => k.Token == key)
                                .Select(k => new TokenInfo { CompanyId = k.CompanyId, Category = (ApiTokenCategory)k.Category, VenueID = k.VenueId, ChainID = k.ChainId }).FirstOrDefault();
                    return response;
                }
            }
            return null;
        }
        private void SentEmail(int rfpId, int venueId, int userId, string reason)
        {
            var rfpAndCompany = _dbContext.RFPs.Where(i => i.RFPID == rfpId && i.IsDeleted == false).Select(i => new { i.IsHotelBooking, i.User.CompanyID }).FirstOrDefault();
            var companyService = new CompanyService(_dbContext, new DataRepositoryProvider(_dbContext), new LocalCache());
            var languageOfNotifyEmail = (EnumLanguage)companyService.GetLanguageOfNotifyEmail(rfpAndCompany.CompanyID ?? 0);
            SupplierMailManager.SendDeclineMailOtherMethod(rfpId, venueId, userId, reason, "AutoDecline From RFPIntegration", languageOfNotifyEmail);
        }
        private string GetComment(int MeetingSelectRfpId, MeetingSelect.Block.DataExt.Venue venueDetails, string TurnDownReason)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Salutation");
            sb.Append(" ");
            sb.Append("");
            sb.Append("\n");
            sb.Append("Name");
            sb.Append(" ");
            sb.Append(venueDetails.VenueName);
            sb.Append(" ");
            sb.Append(" ");
            sb.Append("\n");
            sb.Append("Title");
            sb.Append(" ");
            sb.Append("");
            sb.Append("\n");
            sb.Append("Email");
            sb.Append(" ");
            sb.Append(venueDetails.Email);
            sb.Append("\n");
            sb.Append("Phone");
            sb.Append(" ");
            sb.Append(venueDetails.Phone);
            sb.Append("\n");
            sb.Append("Fax");
            sb.Append(" ");
            sb.Append(venueDetails.Fax == null ? "" : venueDetails.Fax);
            sb.Append("\n");
            sb.Append("Subject");
            sb.Append(" ");
            var MeetingName = _dbContext.RFPs.Where(rfp => rfp.RFPID == Convert.ToInt32(MeetingSelectRfpId)).Select(rfp => rfp.MeetingName).FirstOrDefault();
            sb.Append(MeetingSelectRfpId + "-" + MeetingName);
            sb.Append("\n");
            sb.Append("Reason");
            sb.Append(" ");
            sb.Append(TurnDownReason);
            sb.Append("\n");
            sb.Append("Message");
            sb.Append("\n");
            if (TurnDownReason == "Other")
            {
                sb.Append("Declined by system (CHPIntegrationService). No further comments given by the venue for their decline");
            }
            else
            {
                sb.Append("Declined by system (CHPIntegrationService)");
            }
            var comment = sb.ToString();
            return comment;
        }
        private void SetActionNote(string venueName, int rfpId, string Reason, string EmailForHotelReservation)
        {
            string actionNote = DateTime.UtcNow.ToCustomizedLocalTime().ToString("dd MMMM yyyy hh:mm:ss", System.Globalization.CultureInfo.InvariantCulture) + " " + EmailForHotelReservation + "-" + venueName + " (declined by system (RFPIntegrationService)) -" + Reason + "-" + "AutoDecline From API";
            var tempRfp = _dbContext.RFPs.FirstOrDefault(k => k.RFPID == rfpId);
            tempRfp.ActionNote = actionNote + "\n" + (string.IsNullOrEmpty(tempRfp.ActionNote) ? string.Empty : tempRfp.ActionNote);
        }
        public string Validate(RFPIntegrationRequest request)
        {
            int tmpint;
            if (string.IsNullOrEmpty(request.MeetingSelectRfpId) || !int.TryParse(request.MeetingSelectRfpId, out tmpint) || tmpint <= 0)
                return APIConstant.InvalidRFPId;
            tmpint = 0;
            if (string.IsNullOrEmpty(request.VenueId) || !int.TryParse(request.VenueId, out tmpint) || tmpint <= 0)
            {
                return APIConstant.VenueId;
            }
            if (request.ProposalID < 0)
            {
                return APIConstant.InvalidProposalID;
            }
            if (request.InternalMeetingSelectCancelReasonId == null || request.InternalMeetingSelectCancelReasonId == 0 || request.InternalMeetingSelectCancelReasonId < 0)
            {
                return APIConstant.InvalidCancelReasonId;
            }
            else
            {
                var reasonId = _dbContext.enumDeclineRFPReasons.Where(e => e.ReasonID == request.InternalMeetingSelectCancelReasonId).FirstOrDefault();
                if (reasonId == null)
                {
                    return APIConstant.InvalidCancelReasonId;
                }
            }
            var RFPandVenueIdCheck = _dbContext.RFPVenues.Where(rs => rs.RFPID == Convert.ToInt32(request.MeetingSelectRfpId) && rs.VenueID == Convert.ToInt32(request.VenueId)).FirstOrDefault();
            if (RFPandVenueIdCheck == null)
            {
                return APIConstant.InvalidRFPVenues;
            }
            var IsVenueHivrEnabled = _dbContext.Venues.Where(v => v.VenueId == Convert.ToInt32(request.VenueId)).Select(v => v.IsHiveEnable).FirstOrDefault();
            if (IsVenueHivrEnabled == false)
            {
                return APIConstant.InvalidVenueId;
            }
            if (!_hivrSentRecordService.Exist(Convert.ToInt32(request.MeetingSelectRfpId), Convert.ToInt32(request.VenueId)))
            {
                return APIConstant.InvalidRFPVenues;
            }
            return "";
        }
        public ReciveProposal MapCHPData(Reservation request, int RFPID)
        {
            ReciveProposal cR = new ReciveProposal();
            var venueId = request.ExternalMappings
                .Where(x => x.ItemId == request.LocationId)
                .Select(x => x.Code)
                .FirstOrDefault();
            cR.RFPID = Convert.ToInt32(RFPID);
            cR.VenueID = Convert.ToInt32(venueId);
            cR.Respondent = request.Contacts.Select(c => c.Name).FirstOrDefault();
            cR.DirectTelephone = request.Contacts.Select(c => c.Phone).FirstOrDefault();
            cR.DirectEmail = request.Contacts.Select(c => c.Email).FirstOrDefault();
            cR.BookingStatus = "First option";
            cR.OptionEnd = request.BookerEditableUntil.ToString("yyyy-MM-dd");
            //find currency id if not found then default 1 will set as euro
            int _CurrencyID=_dbContext.enumCurrencies.Where(cu=>cu.CurrencyNameEN== request.Currency).Select(c=>c.CurrencyID).FirstOrDefault();
            cR.CurrencyID = ( _CurrencyID==0)?1:_CurrencyID;
            cR.LatestDateToCancel = request.FreeCancellationUntil.ToString("yyyy-MM-dd");

            // Language fallback in case Language field is null
            var lang = string.IsNullOrEmpty(request.Language) ? "en" : request.Language;
            // Build excluded SpaceIds (Restaurant spaces) — now works since Descriptions is mapped
            var excludedSpaceIds = request.Dates
                .SelectMany(d => d.Spaces)
                .Where(s => s.Descriptions != null && s.Descriptions.Any(desc =>
                    desc.Language == "en" &&
                    desc.Name.Equals("Restaurant", StringComparison.OrdinalIgnoreCase)))
                .Select(s => s.SpaceId)
                .ToHashSet();
            // ── Meeting Room Packages ────────────────────────────────────────────
            cR.meetingRoomPackages = request.Dates
                .SelectMany(d => d.Spaces
                    .Where(s => s.PriceTotal > 0)
                    .Where(s => !excludedSpaceIds.Contains(s.SpaceId))
                    .Select(s => new MeetingRoomPackage
                    {
                        PackageID = s.Id,
                        PackageName = s.SpaceName,
                        Attendees = s.Seats,
                        StartTime = MinutesToTimeString(s.StartMinutes),
                        EndTime = MinutesToTimeString(s.EndMinutes),
                        StandardRate = s.PricePerSeat,
                        RequirementDate = s.StartDate.ToString("yyyy-MM-dd"),
                        Setup = GetExternalRoomSetupName(s.SettingId),
                        PackageContent = s.Descriptions
                                                  ?.FirstOrDefault(desc => desc.Language == lang)
                                                  ?.Description ?? "",
                        ProposalRateExclTaxes = s.PriceTotal,
                        ProposalRateInclTaxes = s.Tax?.TaxableAmount,  // ✅ now works
                        TaxPercentage = s.TaxPercentage
                    }))
                .ToList();
            // ── Food Packages (CategoryId = 1) ───────────────────────────────────
            cR.foodPackages = request.Dates
                .SelectMany(d => d.Options
                    .Where(o => o.CategoryId == 1)
                    .Select(o => new FoodPackage
                    {
                        PackageID = o.Id,
                        PackageName = o.InternalName,
                        Attendees = o.Amount,
                        StartTime = o.StartTime>0? MinutesToTimeString(o.StartTime):"",  //if chp send -1 then ""
                        EndTime =o.EndTime>0?MinutesToTimeString(o.EndTime):"",    // if chp send -1 then ""
                        StandardRate = o.BasePrice,
                        RequirementDate = o.Date.ToString("yyyy-MM-dd"),
                        Setup = "",
                        PackageContent = o.Descriptions
                                                  ?.FirstOrDefault(desc => desc.Language == lang)
                                                  ?.Description ?? o.Description ?? "",  // ✅ now works
                        ProposalRateExclTaxes = o.PriceTotal,
                        ProposalRateInclTaxes = o.Tax?.TaxableAmount,  // ✅ now works
                        TaxType = o.TaxPercentage > 0 ? "Percentage" : "None",
                        TaxAmount = o.Tax?.Total.ToString("F2"),       // ✅ now works
                        TaxPercentage = o.TaxPercentage
                    }))
                .ToList();
            // ── AV Packages (CategoryId = 2) ─────────────────────────────────────
            cR.audioVisualPackages = request.Dates
                .SelectMany(d => d.Options
                    .Where(o => o.CategoryId == 2)
                    .Select(o => new AudioVisualPackage
                    {
                        PackageID = o.Id,
                        PackageName = o.InternalName,
                        Attendees = o.Amount,
                        StartTime = o.StartTime > 0 ? MinutesToTimeString(o.StartTime) : "",  //if chp send -1 then ""
                        EndTime = o.EndTime > 0 ? MinutesToTimeString(o.EndTime) : "",    // if chp send -1 then ""
                        StandardRate = o.BasePrice,
                        RequirementDate = o.Date.ToString("yyyy-MM-dd"),
                        Setup = "",
                        PackageContent = o.Descriptions
                                                  ?.FirstOrDefault(desc => desc.Language == lang)
                                                  ?.Description ?? o.Description ?? "",
                        ProposalRateExclTaxes = o.PriceTotal,
                        ProposalRateInclTaxes = o.Tax?.TaxableAmount,  //
                        TaxType = o.TaxPercentage > 0 ? "Percentage" : "None",
                        TaxAmount = o.Tax?.Total.ToString("F2"),       //
                        TaxPercentage = o.TaxPercentage
                    }))
                .ToList();
            //// ── Equipment Packages (CategoryId = 3) ──────────────────────────────
            //cR.equipmentPackages = request.Dates
            //    .SelectMany(d => d.Options
            //        .Where(o => o.CategoryId == 3)
            //        .Select(o => new EquipmentPackage
            //        {
            //            PackageID = o.Id,
            //            PackageName = o.Name,
            //            Attendees = o.Amount,
            //            StandardRate = o.BasePrice,
            //            RequirementDate = o.Date.ToString("yyyy-MM-dd"),
            //            PackageContent = o.Descriptions
            //                                      ?.FirstOrDefault(desc => desc.Language == lang)
            //                                      ?.Description ?? "",
            //            ProposalRateExclTaxes = o.PriceTotal,
            //            ProposalRateInclTaxes = o.Tax?.TaxableAmount,  //
            //            Vat = o.TaxPercentage
            //        }))
            //    .ToList();
            // ── Additional Packages (all other categories) 3 is parking in chp and 4 is other, i don't see any fit for Equipment pack.
            // Grouped by mapped InfoID to ensure one ProposalAdditional per InfoID (matches how RFPResponse.aspx works)
            cR.additionalPackages = request.Dates
                .SelectMany(d => d.Options
                    .Where(o => o.CategoryId != 1
                             && o.CategoryId != 2))
                .GroupBy(o => GetAdditionalInfoIdFromCHPCategory(o.CategoryId))
                .Select(g => new AdditionalPackage
                {
                    AdditionalInfoID = g.Key,
                    Rate = g.Sum(o => o.PriceTotal) / Math.Max(g.Sum(o => o.Amount), 1),
                    Units = g.Sum(o => o.Amount),
                    Notes = string.Join(", ", g.Select(o => o.InternalName).Where(n => !string.IsNullOrEmpty(n))),
                    RateForYou = g.Sum(o => o.PriceTotal),
                    InclOrExclVat = g.Any(o => o.TaxPercentage > 0),
                    Vat = g.Max(o => o.TaxPercentage),
                    InclOrExclVatName = g.Any(o => o.TaxPercentage > 0) ? "Inclusive" : "Exclusive",
                    FeeUnit = g.First().CalculationType.ToString(),
                    Section = 4
                })
                .ToList();
            cR.commission = new Commission();  // or map from Reservation data
            cR.hotelPackages = new List<HotelPackage>();
            cR.equipmentPackages = new List<EquipmentPackage>();
            //cR.meetingPackage = new List<MeetingPackage>();
            cR.conditionsAndAttachments = new ConditionsAndAttachments
            {
                cancellationPaymentPolicy = new CancellationPaymentPolicy(),
                OtherAttachments = new List<OtherAttachment>()
            };
            return cR;
        }
        /// <summary>
        /// Maps CHP CategoryId to a valid enumAdditionInfo.AdditionalInfoID.
        /// CHP CategoryId 3 = Parking → enumAdditionInfo 19 ("Parking fee", Section 4)
        /// CHP CategoryId 4 = Other   → enumAdditionInfo 24 ("Other", Section 4)
        /// NOTE: Requires DB insert with all language columns - see SQL script in plan.
        /// </summary>
        private static int GetAdditionalInfoIdFromCHPCategory(int chpCategoryId)
        {
            switch (chpCategoryId)
            {
                case 3: return 19;  // Parking → "Parking fee"
                default: return 24; // Other → "Other" (new DB entry)
            }
        }
        public ReservationValidationResult ValidateReservation(Reservation request)
        {
            var result = new ReservationValidationResult();
            // ─── 1. Null check
            if (request == null)
            {
                result.AddError("Request", "Request body is null");
                return result;
            }
            // ─── 2. Name — must contain [RFP <number>]
            int rfpId = 0;
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                result.AddError("Name", "Name is required");
            }
            else
            {
                var rfpMatch = Regex.Match(
                    request.Name,
                    @"\[\s*RFP\s+(\d+)\s*\]",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                if (!rfpMatch.Success)
                {
                    result.AddError("Name", "Name must contain a valid RFP ID in format [RFP 123]");
                }
                else if (!int.TryParse(rfpMatch.Groups[1].Value, out rfpId) || rfpId <= 0)
                {
                    result.AddError("Name", "RFP ID extracted from Name must be a positive integer");
                }
            }
            // ─── 3. LocationId
            if (request.LocationId <= 0)
            {
                result.AddError("LocationId", "LocationId must be a positive integer");
            }
            // ─── 4. ExternalMappings — extract and validate VenueID
            int venueId = 0;
            if (request.ExternalMappings == null || !request.ExternalMappings.Any())
            {
                result.AddError("ExternalMappings", "ExternalMappings is required and must contain at least one entry");
            }
            else
            {
                var venueMapping = request.ExternalMappings
                    .FirstOrDefault(x => x.ItemId == request.LocationId);
                if (venueMapping == null)
                {
                    result.AddError("ExternalMappings",
                        $"No ExternalMapping found with ItemId matching LocationId ({request.LocationId})");
                }
                else if (string.IsNullOrWhiteSpace(venueMapping.Code))
                {
                    result.AddError("ExternalMappings", "ExternalMapping Code (VenueID) is null or empty");
                }
                else if (!int.TryParse(venueMapping.Code, out venueId) || venueId <= 0)
                {
                    result.AddError("ExternalMappings",
                        $"ExternalMapping Code '{venueMapping.Code}' must be a valid positive integer (VenueID)");
                }
                //Checking rfp id here and now i remove number 2 validation >> using RFP from name
                var RFPMapping = request.ExternalMappings
                    .FirstOrDefault(x => x.Type == "Reservation" && x.ItemId == request.Id);
                if (RFPMapping == null)
                {
                    result.AddError("ExternalMappings",
                        $"No RFPMapping found with Reservation type.");
                }
                else if (string.IsNullOrWhiteSpace(RFPMapping.Code))
                {
                    result.AddError("ExternalMappings", "ExternalMapping Code (RFPID) is null or empty");
                }
                else if (!int.TryParse(RFPMapping.Code, out rfpId) || rfpId <= 0)
                {
                    result.AddError("ExternalMappings",
                        $"ExternalMapping Code '{RFPMapping.Code}' must be a valid positive integer (RFPID)");
                }
            }
            // ─── 5. Contacts — at least one with Name and Email
            if (request.Contacts == null || !request.Contacts.Any())
            {
                result.AddError("Contacts", "At least one contact is required");
            }
            else
            {
                var primaryContact = request.Contacts.First();
                if (string.IsNullOrWhiteSpace(primaryContact.Name))
                {
                    result.AddError("Contacts[0].Name", "Primary contact Name is required");
                }
                if (string.IsNullOrWhiteSpace(primaryContact.Email))
                {
                    result.AddError("Contacts[0].Email", "Primary contact Email is required");
                }
            }
            // ─── 6. Currency
            if (string.IsNullOrWhiteSpace(request.Currency))
            {
                result.AddError("Currency", "Currency is required");
            }
            // ─── 7. BookerEditableUntil
            if (request.BookerEditableUntil == default(DateTime))
            {
                result.AddError("BookerEditableUntil", "BookerEditableUntil must be a valid date");
            }
            // ─── 8. FreeCancellationUntil
            if (request.FreeCancellationUntil == default(DateTime))
            {
                result.AddError("FreeCancellationUntil", "FreeCancellationUntil must be a valid date");
            }
            // ─── 9. Dates — at least one date with Spaces or Options
            if (request.Dates == null || !request.Dates.Any())
            {
                result.AddError("Dates", "At least one Date entry is required");
            }
            else
            {
                for (int i = 0; i < request.Dates.Count; i++)
                {
                    var date = request.Dates[i];
                    if (date.Date == default(DateTime))
                    {
                        result.AddError($"Dates[{i}].Date", "Date is required");
                    }
                    // ── 9a. Spaces (maps to meetingRoomPackages)
                    if (date.Spaces != null)
                    {
                        for (int j = 0; j < date.Spaces.Count; j++)
                        {
                            var space = date.Spaces[j];
                            if (space.PriceTotal > 0)
                            {
                                if (string.IsNullOrWhiteSpace(space.SpaceName))
                                    result.AddError($"Dates[{i}].Spaces[{j}].SpaceName", "SpaceName is required when PriceTotal > 0");
                                if (space.Seats <= 0)
                                    result.AddError($"Dates[{i}].Spaces[{j}].Seats", "Seats must be greater than 0");
                                if (space.StartDate == default(DateTime))
                                    result.AddError($"Dates[{i}].Spaces[{j}].StartDate", "StartDate is required");
                                if (space.StartMinutes < 0)
                                    result.AddError($"Dates[{i}].Spaces[{j}].StartMinutes", "StartMinutes cannot be negative");
                                if (space.EndMinutes < 0)
                                    result.AddError($"Dates[{i}].Spaces[{j}].EndMinutes", "EndMinutes cannot be negative");
                                if (space.EndMinutes > 0 && space.EndMinutes <= space.StartMinutes)
                                    result.AddError($"Dates[{i}].Spaces[{j}]", "EndMinutes must be greater than StartMinutes");
                                if (space.PricePerSeat < 0)
                                    result.AddError($"Dates[{i}].Spaces[{j}].PricePerSeat", "PricePerSeat cannot be negative");
                                if (space.TaxPercentage < 0)
                                    result.AddError($"Dates[{i}].Spaces[{j}].TaxPercentage", "TaxPercentage cannot be negative");
                            }
                        }
                    }
                    // ── 9b. Options (maps to food/AV/additional) ──────────
                    if (date.Options != null)
                    {
                        for (int k = 0; k < date.Options.Count; k++)
                        {
                            var option = date.Options[k];
                            if (string.IsNullOrWhiteSpace(option.InternalName))
                                result.AddError($"Dates[{i}].Options[{k}].InternalName", "Option InternalName is required");
                            if (option.CategoryId <= 0)
                                result.AddError($"Dates[{i}].Options[{k}].CategoryId", "CategoryId must be a positive integer");
                            if (option.Amount < 0)
                                result.AddError($"Dates[{i}].Options[{k}].Amount", "Amount cannot be negative");
                            if (option.Date == default(DateTime))
                                result.AddError($"Dates[{i}].Options[{k}].Date", "Option Date is required");
                            if (option.BasePrice < 0)
                                result.AddError($"Dates[{i}].Options[{k}].BasePrice", "BasePrice cannot be negative");
                            if (option.PriceTotal < 0)
                                result.AddError($"Dates[{i}].Options[{k}].PriceTotal", "PriceTotal cannot be negative");
                            if (option.PricePerItem < 0)
                                result.AddError($"Dates[{i}].Options[{k}].PricePerItem", "PricePerItem cannot be negative");
                            if (option.TaxPercentage < 0)
                                result.AddError($"Dates[{i}].Options[{k}].TaxPercentage", "TaxPercentage cannot be negative");
                            //if (option.StartTime < 0)
                            //    result.AddError($"Dates[{i}].Options[{k}].StartTime", "StartTime cannot be negative");
                            //if (option.EndTime < 0)
                            //    result.AddError($"Dates[{i}].Options[{k}].EndTime", "EndTime cannot be negative");
                        }
                    }
                }
            }

            // Stop here if basic validation failed — DB lookups below
            // need valid rfpId and venueId (Pending : Check from table as discussed with jintian change table name and check that)

            if (!result.IsValid)
            {
                return result;
            }
            // ─── 10. Currency must exist in DB ─────────────────────────────
            bool currencyExists = _dbContext.enumCurrencies
                .Any(c => c.CurrencyNameEN == request.Currency && c.IsDeleted != true);
            if (!currencyExists)
            {
                result.AddError("Currency", $"Currency '{request.Currency}' not found in the system");
            }
            // ─── 11. RFP must exist and not be cancelled ──────────────────
            var rfp = _dbContext.RFPs
                .Where(r => r.RFPID == rfpId)
                .Select(r => new { r.RFPID, r.Status })
                .FirstOrDefault();
            if (rfp == null)
            {
                result.AddError("RFPID", $"RFP with ID {rfpId} does not exist");
                return result; // can't check RFPVenue or Proposal without a valid RFP
            }
            if (rfp.Status == (int)RfpStatusType.Cancelled)
            {
                result.AddError("RFPID", $"RFP {rfpId} is cancelled and cannot receive proposals");
            }
            // ─── 12. Venue must exist ──────────────────────────────────────
            bool venueExists = _dbContext.Venues.Any(v => v.VenueId == venueId);
            if (!venueExists)
            {
                result.AddError("VenueID", $"Venue with ID {venueId} does not exist");
                return result; // can't check RFPVenue without a valid Venue
            }
            // ─── 13. RFPVenue must exist with allowed ResponseStatus ──────
            var rfpVenue = _dbContext.RFPVenues
                .Where(rv => rv.RFPID == rfpId && rv.VenueID == venueId)
                .Select(rv => new { rv.ResponseStatus })
                .FirstOrDefault();
            if (rfpVenue == null)
            {
                result.AddError("RFPVenue", $"No RFPVenue link found for RFP {rfpId} and Venue {venueId}");
            }
            else
            {
                var allowedResponseStatuses = new[]
                {
            (int)ResponseStatusType.New,
            (int)ResponseStatusType.Draft,
            (int)ResponseStatusType.Negotiated,
            (int)ResponseStatusType.Responded
        };
                if (!allowedResponseStatuses.Contains(Convert.ToInt32(rfpVenue.ResponseStatus)))
                {
                    result.AddError("RFPVenue.ResponseStatus",
                        "RFPVenue response status does not allow proposal submission");
                }
            }
            // ─── 14. Existing Proposal status must allow update ───────────
            int proposalStatus = _dbContext.Proposals
                .Where(p => p.RFPID == rfpId && p.VenueID == venueId)
                .Select(p => p.Status)
                .FirstOrDefault();
            if (proposalStatus != 0) // 0 = no proposal exists, that's fine
            {
                var allowedProposalStatuses = new[]
                {
            (int)ProposalStatusType.Received,
            (int)ProposalStatusType.Negotiated,
            (int)ProposalStatusType.Accepted,
            (int)ProposalStatusType.WaitingForApproval,
            (int)ProposalStatusType.Approved,
            (int)ProposalStatusType.WaitingForManualApproval,
            (int)ProposalStatusType.ManualApproved,
            (int)ProposalStatusType.WaitingForDoaApproval
        };
                if (!allowedProposalStatuses.Contains(proposalStatus))
                {
                    result.AddError("Proposal.Status",
                        "Existing proposal status does not allow updates");
                }
            }
            return result;
        }
        public class ReservationValidationResult
        {
            public bool IsValid => Errors.Count == 0;
            public List<string> Errors { get; set; } = new List<string>();
            public void AddError(string field, string message)
            {
                Errors.Add($"{field}: {message}");
            }
        }
        private static string MinutesToTimeString(int minutes)
        {
            if (minutes < 0) return "";
            var h = minutes / 60;
            var m = minutes % 60;
            return $"{h:D2}:{m:D2}";
        }
        //public ReciveProposal MapCHPData(Reservation request,int RFPID)
        //{
        //    ReciveProposal cR =new ReciveProposal();
        //    var match = Regex.Match(
        //                request.Name,
        //                @"\[\s*RFP\s+(\d+)\s*\]",
        //                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant
        //            );
        //    if (match.Success && int.TryParse(match.Groups[1].Value, out var id))
        //    {
        //        RFPID = id;
        //    }
        //    RFPID = Convert.ToInt32(RFPID);
        //    var venueId = request.ExternalMappings.Where(x => x.ItemId == request.LocationId).Select(x => x.Code).FirstOrDefault();//_dbContext.Venues.Where(xv => xv.VenueId == RFPID).Select(xv => xv.VenueId).FirstOrDefault();
        //    cR.RFPID= RFPID;
        //    cR.VenueID = Convert.ToInt32(venueId);
        //    cR.Respondent=request.Contacts.Select(c => c.Name).FirstOrDefault();//check with wouter for Createdbyname or this is ok
        //    cR.DirectTelephone=request.Contacts.Select(c => c.Phone).FirstOrDefault();
        //    cR.DirectEmail=request.Contacts.Select(c => c.Email).FirstOrDefault();
        //    cR.BookingStatus = "First option";//ask for this
        //    cR.OptionEnd = "";//ask for this to jintian
        //    cR.CurrencyID = 1;// request.CurrencyId;//reciving chp id not iso name or name
        //    cR.LatestDateToCancel = request.BookerEditableUntil.ToString("yyyy-MM-dd");
        //    cR.commission = null;//no commission info in reservation, use 2.0 commission
        //    cR.comments=null;
        //    cR.hotelPackages=null;
        //    cR.proposalQuestionAnswer = null;
        //    cR.hotelPackages = null;
        //    //cR.foodPackages
        //    //cR.additionalPackages
        //    //cR.proposalQuestionAnswer
        //    //cR.hotelPackages
        //    //cR.equipmentPackages
        //    return cR;
        //}
        public class TokenInfo
        {
            public int? CompanyId { get; set; }
            public ApiTokenCategory Category { get; set; }
            public int? VenueID { get; set; }
            public int? ChainID { get; set; }
        }
        private User GetUserSupplier(int venueId)
        {
            var supplierResult = new SupplierService(_dbContext).Generate(new MeetingSelect.Domain.Commands.GenerateSupplierCommand
            {
                VenueId = venueId
            });
            var userSupplier = _dbContext.Users.Where(us => us.UserID == supplierResult.UserId).FirstOrDefault();
            if (userSupplier == null)
            {
                userSupplier = new User();
            }
            return userSupplier;
        }
        public enum CancelReason
        {
            None = -1,
            NoReasonSpecified,
            NoParticipants,
            OtherLocation,
            OtherRegion,
            MeetingIsCancelled,
            MeetingIsRescheduled,
            Other,
            TooExpensive,
            Expired
        }
        public static bool TryParseCancelReason(string reasonType, out CancelReason reason)
        {
            return Enum.TryParse(reasonType, ignoreCase: true, out reason);
        }
        public static string GetExternalRoomSetupName(int settingId)
        {
            var map = new Dictionary<int, string>
            {
                { 9,  "U-shape" },
                { 3,  "Carre" },
                { 2,  "Cabaret" },
                { 6,  "Classroom" },
                { 29, "Boardroom" },
                { 14, "Dinner" },
                { 13, "Lunch" },
                { 12, "Reception" },
                { 30, "Rounds of 8" },
                { 16, "Restaurant" },
                { 8,  "Theatre" },
                //{ 12, "Reception" },
                { 15, "Buffet" },
                { 17, "Chevron" },
                { 7,  "Circle" }
            };
            return map.TryGetValue(settingId, out var result)
                ? result
                : "Other";
        }
        //    private static readonly Dictionary<CancelReason, string> ReasonMap =
        //        new Dictionary<CancelReason, string>
        //    {
        //{ CancelReason.NoParticipants, "No more availability" },      // Example mapping
        //{ CancelReason.OtherLocation, "No more meeting space available" },
        //{ CancelReason.OtherRegion, "No more availability" },
        //{ CancelReason.TooExpensive, "No more availability" },        // or create "Too expensive"
        //{ CancelReason.MeetingIsCancelled, "Other" },                 // ← no matching row, map to Other
        //{ CancelReason.MeetingIsRescheduled, "Other" },               // ← no matching row, map to Other
        //{ CancelReason.Other, "Other" },
        //{ CancelReason.Expired, "No more availability" },
        //{ CancelReason.NoReasonSpecified, "Other" },
        //{ CancelReason.None, "Other" }
        //    };
        //    public static string MapToTableReason(CancelReason reason)
        //    {
        //        return ReasonMap.TryGetValue(reason, out var tableReason)
        //            ? tableReason
        //            : "Other";
        //    }
    }
}
