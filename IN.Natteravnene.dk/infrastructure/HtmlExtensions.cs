/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/


using DTA;
using NR.Infrastructure;
using NR.Localication;
using NR.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Routing;
using System.Linq;

namespace System.Web.Mvc.Html
{


    public static class HtmlExtensions
    {

        public static string PersonOnlyAttributesIcons(Person person)
        {
            if (person == null) return "";
            StringBuilder str = new StringBuilder();

            //<span class="btn btn-sm" data-rel="tooltip" title="" data-original-title="Default">Default</span>-->


            TagBuilder tag = new TagBuilder("i");
            tag.MergeAttribute("class", Icons.BasicTraining + (person.BasicTraining ? " green" : " red"));
            tag.MergeAttribute("data-rel", "tooltip");
            tag.MergeAttribute("title", DomainStrings.BasicTraining);
            str.Append(tag.ToString()).Append(" ");


            tag = new TagBuilder("i");
            tag.MergeAttribute("class", Icons.SeniorInstructorIcon + (person.SeniorInstructor ? " green" : " red"));
            tag.MergeAttribute("data-rel", "tooltip");
            tag.MergeAttribute("title", DomainStrings.SetupSeniorInstructor);
            str.Append(tag.ToString()).Append(" ");

            tag = new TagBuilder("i");
            tag.MergeAttribute("class", Icons.EmailNewsLetter + (person.EmailNewsLetter ? " green" : " red"));
            tag.MergeAttribute("data-rel", "tooltip");
            tag.MergeAttribute("title", DomainStrings.SetupEmailNewsLetter);
            str.Append(tag.ToString()).Append(" ");

            tag = new TagBuilder("i");
            tag.MergeAttribute("class", Icons.PrintNewslettet + (person.PrintNewslettet ? " green" : " red"));
            tag.MergeAttribute("data-rel", "tooltip");
            tag.MergeAttribute("title", DomainStrings.SetupPrintNewslettet);
            str.Append(tag.ToString());


            return str.ToString();
        }



        public static MvcHtmlString MembershipOnlyAttributesIcons<TModel>(this HtmlHelper<TModel> html, NRMembership membership)
        {
            return MvcHtmlString.Empty;

            //StringBuilder str = new StringBuilder();
            //TagBuilder tag = new TagBuilder("i");
            //switch (membership.BoardFunction)
            //{
            //    case BoardFunctionType.Accountant:
            //        tag.MergeAttribute("class", Icons.AccountantIcon);
            //        tag.MergeAttribute("data-rel", "tooltip");
            //        tag.MergeAttribute("title", BoardFunctionType.Accountant.DisplayName());
            //        str.Append(tag.ToString()).Append(" ");
            //        break;
            //    case BoardFunctionType.Alternate:
            //        tag.MergeAttribute("class", Icons.AlternateIcon);
            //        tag.MergeAttribute("data-rel", "tooltip");
            //        tag.MergeAttribute("title", BoardFunctionType.Alternate.DisplayName());
            //        str.Append(tag.ToString()).Append(" ");
            //        break;
            //    case BoardFunctionType.BoardMember:
            //        tag.MergeAttribute("class", Icons.BoardMemberIcon);
            //        tag.MergeAttribute("data-rel", "tooltip");
            //        tag.MergeAttribute("title", BoardFunctionType.BoardMember.DisplayName());
            //        str.Append(tag.ToString()).Append(" ");
            //        break;
            //    case BoardFunctionType.Chairman:
            //        tag.MergeAttribute("class", Icons.ChairmanIcon);
            //        tag.MergeAttribute("data-rel", "tooltip");
            //        tag.MergeAttribute("title", BoardFunctionType.Chairman.DisplayName());
            //        str.Append(tag.ToString()).Append(" ");
            //        break;
            //    case BoardFunctionType.Auditor:
            //        tag.MergeAttribute("class", Icons.AuditorIcon);
            //        tag.MergeAttribute("data-rel", "tooltip");
            //        tag.MergeAttribute("title", BoardFunctionType.Auditor.DisplayName());
            //        str.Append(tag.ToString()).Append(" ");
            //        break;
            //    case BoardFunctionType.AuditorAlternate:
            //        tag.MergeAttribute("class", Icons.AuditorAlternateIcon);
            //        tag.MergeAttribute("data-rel", "tooltip");
            //        tag.MergeAttribute("title", BoardFunctionType.AuditorAlternate.DisplayName());
            //        str.Append(tag.ToString()).Append(" ");
            //        break;
            //}


            //return MvcHtmlString.Create(str.ToString());


        }


        /// <summary>
        /// Genrates Icons for person attributes
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="person">The person for wich </param>
        /// <returns></returns>
        public static MvcHtmlString PersonAttributesFlag<TModel>(this HtmlHelper<TModel> html, Person person)
        {
            if (html == null) throw new ArgumentNullException("helper");
            if (person == null) throw new ArgumentNullException("model");
            string tags = "";



            //Person no longer in the association           
            //if (person.Absent == true)
            //{
            //    TagBuilder tag = new TagBuilder("i");
            //    tag.MergeAttribute("class", Icons.AbsentIcon + " bigger-110 red");
            //    tags += WrapperFlag(tag.ToString(TagRenderMode.Normal), Icons.AbsentTxt);
            //}

            //Person can be teamleader
            //if (person.Teamleader == true)
            //{
            //    TagBuilder tag = new TagBuilder("i");
            //    tag.MergeAttribute("class", Icons.TeamleaderIcon + " bigger-110"); //TODO: Find ikon
            //    tags += WrapperFlag( tag.ToString(TagRenderMode.Normal), DomainStrings.Teamleader);
            //}

            //if (person.Memberships != null)
            //{
            //    foreach (Membership fun in person.Memberships)
            //    {
            //        TagBuilder tag = new TagBuilder("i");
            //        switch (fun.BoardFunction)
            //        {
            //            case BoardFunctionType.Accountant:
            //                tag.MergeAttribute("class", Icons.AccountantIcon + " bigger-110");
            //                tags += WrapperFlag(tag.ToString(TagRenderMode.Normal), Icons.AccountantTxt);
            //                break;
            //            case BoardFunctionType.Alternate:
            //                tag.MergeAttribute("class", Icons.AlternateIcon + " bigger-110");
            //                tags += WrapperFlag(tag.ToString(TagRenderMode.Normal), Icons.AlternateTxt);
            //                break;
            //            case BoardFunctionType.BoardMember:
            //                tag.MergeAttribute("class", Icons.BoardMemberIcon + " bigger-110");
            //                tags += WrapperFlag(tag.ToString(TagRenderMode.Normal), Icons.BoardMemberTxt);
            //                break;
            //            case BoardFunctionType.Chairman:
            //                tag.MergeAttribute("class", Icons.ChairmanIcon + " bigger-110");
            //                tags += WrapperFlag(tag.ToString(TagRenderMode.Normal), Icons.ChairmanTxt);
            //                break;
            //            case BoardFunctionType.Auditor:
            //                tag.MergeAttribute("class", Icons.AuditorIcon + " bigger-110");
            //                tags += WrapperFlag(tag.ToString(TagRenderMode.Normal), Icons.AuditorTxt);
            //                break;
            //        }

            //    }
            //}

            //Person Seniorinstructor
            if (person.SeniorInstructor == true)
            {
                TagBuilder tag = new TagBuilder("i");
                tag.MergeAttribute("class", Icons.SeniorInstructorIcon + " bigger-110");
                tags += WrapperFlag(tag.ToString(TagRenderMode.Normal), DomainStrings.SetupSeniorInstructor);
            }


            return MvcHtmlString.Create(tags);


        }

        private static string WrapperFlag(string StrIcon, string Name)
        {
            TagBuilder wrapper = new TagBuilder("span");
            wrapper.MergeAttribute("class", "label label-large label-yellow arrowed-in-right");
            wrapper.InnerHtml = StrIcon + " " + Name;
            return wrapper.ToString(TagRenderMode.Normal) + " ";

        }

        /// <summary>
        /// Genrates Icons for person attributes
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="person">The person for wich </param>
        /// <returns></returns>
        public static MvcHtmlString PersonAttributes<TModel>(this HtmlHelper<TModel> html, Person person)
        {
            if (html == null) throw new ArgumentNullException("helper");
            if (person == null) throw new ArgumentNullException("model");
            string tags = "";

            
           

            //Person Seniorinstructor
            if (person.SeniorInstructor == true)
            {
                TagBuilder tag = new TagBuilder("i");
                tag.AddCssClass("ace-icon fa" + Icons.SeniorInstructorIcon + " bigger-110 listicon");
                tag.MergeAttribute("data-rel", "tooltip");
                tag.MergeAttribute("title", DomainStrings.SetupSeniorInstructor);
                tags += tag.ToString(TagRenderMode.Normal);
            }



            return MvcHtmlString.Create(tags);
        }


        public static MvcHtmlString PersonAttributes<TModel>(this HtmlHelper<TModel> html, NR.Models.NRMembership membership)
        {
            if (html == null) throw new ArgumentNullException("helper");
            if (membership == null) throw new ArgumentNullException("model");
            string tags = "";

            if (membership.Teamleader == true)
            {
                //class="tooltip-info" data-rel="tooltip" title="@General.View" data-original-title="@General.View"
                TagBuilder tag = new TagBuilder("i");
                tag.AddCssClass("ace-icon " + Icons.TeamLeader + " bigger-110 listicon");
                tag.MergeAttribute("data-rel", "tooltip");
                tag.MergeAttribute("title", DomainStrings.Teamleader);
                tags += tag.ToString(TagRenderMode.Normal);
            }


            if (membership.BoardFunction != BoardFunctionType.none)
            {
            TagBuilder tag = new TagBuilder("i"); 
                    switch (membership.BoardFunction)
                    {
                        case BoardFunctionType.Accountant:
                            tag.AddCssClass("ace-icon " + Icons.Accountant + " bigger-110 listicon");
                            tag.MergeAttribute("data-rel", "tooltip");
                            tag.MergeAttribute("title", DomainStrings.BoardFunctionTypeAccountant);
                            break;
                        case BoardFunctionType.Alternate:
                            tag.AddCssClass("ace-icon " + Icons.Alternate + " bigger-110 listicon");
                            tag.MergeAttribute("data-rel", "tooltip");
                            tag.MergeAttribute("title", DomainStrings.BoardFunctionTypeAlternate);
                            break;
                        case BoardFunctionType.BoardMember:
                            tag.AddCssClass("ace-icon " + Icons.BoardMember + " bigger-110 listicon");
                            tag.MergeAttribute("data-rel", "tooltip");
                            tag.MergeAttribute("title", DomainStrings.BoardFunctionTypeBoardMember);
                            break;
                        case BoardFunctionType.Chairman:
                            tag.AddCssClass("ace-icon " + Icons.Chairman + " bigger-110 listicon");
                            tag.MergeAttribute("data-rel", "tooltip");
                            tag.MergeAttribute("title", DomainStrings.BoardFunctionTypeChairman);
                            break;
                        case BoardFunctionType.Auditor:
                            tag.AddCssClass("ace-icon " + Icons.Auditor + " bigger-110 listicon");
                            tag.MergeAttribute("data-rel", "tooltip");
                            tag.MergeAttribute("title", DomainStrings.BoardFunctionTypeAuditor);
                            break;
                    }
                    tags += tag.ToString(TagRenderMode.Normal);
            }


            if (membership.Person.SeniorInstructor == true)
            {
                //class="tooltip-info" data-rel="tooltip" title="@General.View" data-original-title="@General.View"
                TagBuilder tag = new TagBuilder("i");
                tag.AddCssClass("ace-icon " + Icons.SeniorInstructorIcon + " bigger-110 listicon");
                tag.MergeAttribute("data-rel", "tooltip");
                tag.MergeAttribute("title", DomainStrings.SetupSeniorInstructor);
                tags += tag.ToString(TagRenderMode.Normal);
            }

            if (membership.Planner == true)
            {
                TagBuilder tag = new TagBuilder("i");
                tag.AddCssClass("ace-icon " + Icons.Planner + " bigger-110 listicon");
                tag.MergeAttribute("data-rel", "tooltip");
                tag.MergeAttribute("title", DomainStrings.SetupPlanner);
                tags += tag.ToString(TagRenderMode.Normal);
            }

            if (membership.Secretary == true)
            {
                TagBuilder tag = new TagBuilder("i");
                tag.AddCssClass("ace-icon " + Icons.Secretary + " bigger-110 listicon");
                tag.MergeAttribute("data-rel", "tooltip");
                tag.MergeAttribute("title", DomainStrings.SetupSecretary);
                tags += tag.ToString(TagRenderMode.Normal);
            }

            if (membership.Editor == true)
            {
                TagBuilder tag = new TagBuilder("i");
                tag.AddCssClass("ace-icon " + Icons.Editor + " bigger-110 listicon");
                tag.MergeAttribute("data-rel", "tooltip");
                tag.MergeAttribute("title", DomainStrings.SetupEditor);
                tags += tag.ToString(TagRenderMode.Normal);
            }
            string ProfilDirSetting = ConfigurationManager.AppSettings["ProfileImage"];

            if (string.IsNullOrWhiteSpace(ProfilDirSetting)) { throw new ArgumentNullException(); }

            var ProfilBilled = string.Format(ProfilDirSetting, membership.Person.PersonID);

            if (File.Exists(HttpContext.Current.Server.MapPath(ProfilBilled)))
            {
                TagBuilder tag = new TagBuilder("i");
                tag.AddCssClass("ace-icon " + Icons.ProfilImage + " bigger-110 listicon");
                tag.MergeAttribute("data-rel", "tooltip");
                tag.MergeAttribute("title", General.ProfileImageExists);
                tags += tag.ToString(TagRenderMode.Normal);
            }
            

            return MvcHtmlString.Create(tags);
        }

        /// <summary>
        /// Generating a img tag with the profile picture. Checking if profile image existe otherwise shows dummy profilepicture
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="person">Person model</param>
        /// <returns>img tag html string</returns>
        public static MvcHtmlString ProfilImage<TModel>(this HtmlHelper<TModel> html, Person person)
        {
            return ProfilImage(html, person, null);
        }

        /// <summary>
        /// Generating a img tag with the profile picture. Checking if profile image existe otherwise shows dummy profilepicture
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="person">Person model</param>
        /// <param name="htmlAttributes"></param>
        /// <returns>img tag html string</returns>
        public static MvcHtmlString ProfilImage<TModel>(this HtmlHelper<TModel> html, Person person, object htmlAttributes)
        {
            string mainClass = "main";
            if (person == null || person.PersonID == Guid.Empty)
            {
                return MvcHtmlString.Empty;
            }

            string ProfilDirSetting = ConfigurationManager.AppSettings["ProfileImage"];

            if (string.IsNullOrWhiteSpace(ProfilDirSetting)) { throw new ArgumentNullException(); }

            var ProfilBilled = string.Format(ProfilDirSetting, person.PersonID);

            if (!File.Exists(HttpContext.Current.Server.MapPath(ProfilBilled)))
            {
                ProfilBilled = string.Format(ProfilDirSetting, "billedemangler");
                mainClass = "main-no-scale";
            }
            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);


            TagBuilder tag = new TagBuilder("img");
            //tag.MergeAttribute("class", "profilbillede");
            tag.Attributes.Add("src", urlHelper.Content(ProfilBilled));
            tag.Attributes.Add("alt", person.FullName);
            tag.MergeAttributes(ToRouteValueDictionary(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)));
            //tag.SetInnerText(labelText + ":");

            TagBuilder tagMain = new TagBuilder("div");
            tagMain.MergeAttribute("class", mainClass);
            tagMain.InnerHtml = tag.ToString(TagRenderMode.SelfClosing);
            TagBuilder tagWrapper = new TagBuilder("div");
            tagWrapper.MergeAttribute("class", "profile-image");
            tagWrapper.InnerHtml = tagMain.ToString();

            return MvcHtmlString.Create(tagWrapper.ToString(TagRenderMode.Normal));
        }

        /// <summary>
        /// Returns gender icon
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="gender">String with M-male or K - female</param>
        /// <returns>Icon Html</returns>
        public static MvcHtmlString GenderIcon<TModel>(this HtmlHelper<TModel> html, Gender gender)
        {
            if (html == null) throw new ArgumentNullException("helper");

            if (gender == Gender.NotDefined) return MvcHtmlString.Empty;
            TagBuilder tag = new TagBuilder("i");
            switch (gender)
            {
                case Gender.Man: tag.MergeAttribute("class", "icon-male bigger-110");
                    break;
                case Gender.Woman: tag.MergeAttribute("class", "icon-female bigger-110");
                    break;
                default: ;
                    break;
            }
            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

        /// <summary>
        /// Returns the timespan (Date, Weeknumber) for the Tur i a formatted Html. 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="team">The team model</param>
        /// <returns>Html formatted Timespan string</returns>
        public static MvcHtmlString TurDate<TModel>(this HtmlHelper<TModel> html, Team team)
        {
            if (html == null) throw new ArgumentNullException("helper");
            if (team == null) throw new ArgumentNullException("model");
            return TurDate(html, team.Start, team.Start.Add(team.Duration == null ? new TimeSpan(0) : (TimeSpan)team.Duration));
        }

        /// <summary>
        /// Returns the timespan (Date, Weeknumber) for the Tur i a formatted Html. 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="start">Start time</param>
        /// <param name="finish">Finsih time</param>
        /// <returns>Html formatted Timespan string</returns>
        public static MvcHtmlString TurDate<TModel>(this HtmlHelper<TModel> html, DateTime start, DateTime finish)
        {
            if (html == null) throw new ArgumentNullException("helper");
            if (start == DateTime.MinValue) start = start.AddSeconds(1);
            // Gets the Calendar instance associated with a CultureInfo.
            CultureInfo myCI = CultureInfo.CurrentCulture;
            Calendar myCal = myCI.Calendar;

            // Gets the DTFI properties required by GetWeekOfYear.
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;

            //Week
            TagBuilder tagWeek = new TagBuilder("div"); //<span class="badge">1</span>
            tagWeek.InnerHtml = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(start.AddSeconds(-1), myCWR, myFirstDOW).ToString();
            tagWeek.AddCssClass("week");

            TagBuilder tagCalenderIcon = new TagBuilder("i");
            tagCalenderIcon.MergeAttribute("class", "glyphicon glyphicon-calendar");

            TagBuilder tagTimeIcon = new TagBuilder("i");
            tagTimeIcon.MergeAttribute("class", "glyphicon glyphicon-time");

            //Start date
            TagBuilder tagStartDate = new TagBuilder("span");  //<span class="label label-large label-yellow arrowed-in">Yellow</span>
            if (myCI.Name.ToLower() == "da-dk")
            {
                tagStartDate.InnerHtml = tagCalenderIcon.ToString() + " " + start.AddSeconds(-1).ToString("ddd") + " " + start.AddSeconds(-1).ToShortDateString();
            }
            else
            {
                tagStartDate.InnerHtml = tagCalenderIcon.ToString() + " " + start.AddSeconds(-1).ToString("ddd") + " " + start.ToShortDateString();
            }

            //Time span
            TagBuilder tagTimeSpan = new TagBuilder("span");//<span class="label label-large label-grey arrowed-in-right arrowed-in">Grey</span>
            if (myCI.Name.ToLower() == "da-dk")
            {
                tagTimeSpan.InnerHtml = tagTimeIcon.ToString() + " " + start.ToShortTimeString().Replace("00:00", "24:00") + " - " + finish.ToShortTimeString().Replace("00:00", "24:00");
            }
            else
            {
                tagTimeSpan.InnerHtml = tagTimeIcon.ToString() + " " + start.ToShortTimeString() + " - " + finish.ToShortTimeString();
            }

            TagBuilder teamtimewrapper = new TagBuilder("div");
            teamtimewrapper.AddCssClass("teamtime");

            TagBuilder timedatewrapper = new TagBuilder("div");
            timedatewrapper.AddCssClass("timedate");
            timedatewrapper.InnerHtml = tagStartDate.ToString() + tagTimeSpan.ToString();

            if (ConfigurationManager.AppSettings["UseWeek"] == "true")
            {
                teamtimewrapper.InnerHtml += tagWeek.ToString();
            }
            teamtimewrapper.InnerHtml += timedatewrapper.ToString();
            return MvcHtmlString.Create(teamtimewrapper.ToString());

        }

        public static MvcHtmlString PersonListing<TModel>(this HtmlHelper<TModel> html, Person person, Team Team)
        {
            if (html == null) throw new ArgumentNullException("helper");

            if (person == null)
            {
                if (Team.Status == TeamStatus.Planned && CurrentProfile.useTakeTeamSpot && (Team.Teammembers == null || !Team.Teammembers.Any(p => p.PersonID == CurrentProfile.PersonID)))
                {
                    // <button class="btn btn-xs btn-danger"><i class="ace-icon fa fa-bolt bigger-110"></i>Danger<i class="ace-icon fa fa-arrow-right icon-on-right"></i></button>
                    TagBuilder missingBtn = new TagBuilder("button");
                    missingBtn.AddCssClass("btn btn-xs btn-danger btn-block missing");
                    missingBtn.InnerHtml = "<i class=\"ace-icon fa fa-hand-o-right bigger-240 bounce pull-left\"></i>" + General.Missing + "<br />" + General.MissingTakeIt;

                    return MvcHtmlString.Create(missingBtn.ToString());
                }
                else
                { 
                TagBuilder missingTag = new TagBuilder("span");
                missingTag.AddCssClass("label");
                missingTag.AddCssClass("label-larger");
                missingTag.AddCssClass("label-important");
                missingTag.AddCssClass("arrowed");
                missingTag.AddCssClass("arrowed-right");
                missingTag.InnerHtml = General.Missing;
                return MvcHtmlString.Create(missingTag.ToString()); 
                }
            }
            TagBuilder tag = new TagBuilder("div");
            tag.AddCssClass("holdlisting");
            tag.InnerHtml = html.ProfilImage(person).ToHtmlString();
            tag.InnerHtml += person.FullName + "<br />" + person.Mobile;


            return MvcHtmlString.Create(tag.ToString());
        }

        public static MvcHtmlString TrialListing<TModel>(this HtmlHelper<TModel> html, Team Team)
        {
            TagBuilder btn = new TagBuilder("a");
            btn.AddCssClass("btn btn-app btn-xs");
            btn.AddCssClass("btn-primary");
            btn.InnerHtml = "<i class=\"" + Icons.Trial + " bigger-160\"></i>";
            btn.MergeAttribute("data-toggle", "popover");
            btn.MergeAttribute("title", General.TrialNote);
            btn.MergeAttribute("data-content", Team.TrialInfo);
            return MvcHtmlString.Create(btn.ToString());


        }

        public static MvcHtmlString LocationListing<TModel>(this HtmlHelper<TModel> html, Location lokation)
        {
            if (html == null) throw new ArgumentNullException("helper");

            if (lokation == null)
            {
                return MvcHtmlString.Create("-");
            }
            TagBuilder iconTag = new TagBuilder("i"); //<i class="icon-map-marker light-orange bigger-110"></i>
            iconTag.AddCssClass("icon-map-marker");
            iconTag.AddCssClass("light-orange");
            iconTag.AddCssClass("bigger-110");




            return MvcHtmlString.Create(iconTag.ToString() + " " + lokation.ShortName);
        }

        public static MvcHtmlString TeamStatusListing<TModel>(this HtmlHelper<TModel> html, Team hold)
        {
            if (html == null) throw new ArgumentNullException("helper");

            TagBuilder tag = new TagBuilder("span");
            tag.AddCssClass("label");
            tag.AddCssClass("label-larger");

            switch (hold.Status)
            {
                case TeamStatus.Cancelled: //<span class="label label-important">Important</span>
                    tag.AddCssClass("label-important");
                    tag.InnerHtml = TeamStatus.Cancelled.DisplayName();
                    break;
                case TeamStatus.Droped:
                    tag.AddCssClass("label-gray");
                    tag.InnerHtml = TeamStatus.Droped.DisplayName();
                    break;
                case TeamStatus.OK:
                    tag.AddCssClass("label-success");
                    tag.InnerHtml = TeamStatus.OK.DisplayName();
                    break;
                default:
                    tag.AddCssClass("label-light");
                    tag.AddCssClass("arrowed");
                    tag.InnerHtml = TeamStatus.Planned.DisplayName();
                    break;
            }

            return MvcHtmlString.Create(tag.ToString());
        }

        public static MvcHtmlString TeamNote<TModel>(this HtmlHelper<TModel> html, Team team)
        {
            //<a href="#" class="btn btn-app btn-yellow btn-xs"><i class="ace-icon fa fa-star bigger-160"></i></a>

            TagBuilder btn = new TagBuilder("a");
            btn.AddCssClass("btn btn-app btn-xs");
            if (team.Star)
            {
                btn.AddCssClass("btn-yellow");
                btn.InnerHtml = "<i class=\"ace-icon fa fa-star bigger-160\"></i>";
                if (!string.IsNullOrWhiteSpace(team.Note))
                {
                    btn.MergeAttribute("data-toggle", "popover");
                    btn.MergeAttribute("title", General.Teamnote);
                    btn.MergeAttribute("data-content", team.Note);
                }
                return MvcHtmlString.Create(btn.ToString());
            }
            else if (!string.IsNullOrWhiteSpace(team.Note))
            {
                btn.AddCssClass("btn-primary");
                btn.InnerHtml = "<i class=\"ace-icon fa fa-exclamation bigger-160\"></i>";
                btn.MergeAttribute("data-toggle", "popover");
                btn.MergeAttribute("title", General.Teamnote);
                btn.MergeAttribute("data-content", team.Note);
                return MvcHtmlString.Create(btn.ToString());
            }

            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString ProfilLink<TModel>(this HtmlHelper<TModel> html, Person person)
        {
            if (person == null) return MvcHtmlString.Create(String.Empty);
            return html.ActionLink(person.FullName, "ViewProfile", "People", new { Username = person.UserName, area = "" }, null);
        }

        public static MvcHtmlString UserBarMessageListing<TModel>(this HtmlHelper<TModel> html, List<Message> messages)
        {
            if (html == null) throw new ArgumentNullException("helper");
            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
            string result = string.Empty;

            foreach (Message message in messages.Count > 5 ? messages.GetRange(0, 5) : messages)
            {
                TagBuilder tag = new TagBuilder("li");
                tag.AddCssClass("msg");
                TagBuilder atag = new TagBuilder("a");
                atag.Attributes.Add("href", urlHelper.Action("Index", "Message", new { ID = message.MessageID }));
                //atag.AddCssClass("msgdropdown");
                if (message.Sender != null) atag.InnerHtml += html.ProfilImage(message.Sender);
                TagBuilder span1Tag = new TagBuilder("span"); //<span class="msg-body">
                span1Tag.AddCssClass("msg-body");
                TagBuilder span2Tag = new TagBuilder("span"); //<span class="msg-title">
                span2Tag.AddCssClass("msg-title");
                if (message.Sender != null)
                {
                    TagBuilder nameTag = new TagBuilder("span"); //<span class="blue">
                    nameTag.InnerHtml = message.Sender.FirstName + ":";
                    nameTag.AddCssClass("blue");
                    span2Tag.InnerHtml = nameTag.ToString();
                }
                String heading = message.Type == MessageType.LongMessage ? message.Head : message.Body;
                if (!string.IsNullOrWhiteSpace(heading)) span2Tag.InnerHtml += heading.Length > 40 ? heading.Substring(0, 40) + "..." : heading;

                TagBuilder span3Tag = new TagBuilder("span"); // <span class="msg-time">
                span3Tag.AddCssClass("msg-time");
                TagBuilder span4Tag = new TagBuilder("span");
                span4Tag.InnerHtml = " " + TimeDescription(message.Created, DateTime.Now).ToHtmlString() + " " + General.Ago;


                span3Tag.InnerHtml = "<i class=\"ace-icon fa fa-clock-o\"></i>" + span4Tag.ToString();

                span1Tag.InnerHtml = span2Tag.ToString() + span3Tag.ToString();
                atag.InnerHtml += span1Tag.ToString();
                tag.InnerHtml = atag.ToString();
                result += tag.ToString();
            }
            return MvcHtmlString.Create(result);
        }


        public static MvcHtmlString UserBarNotificationeListing<TModel>(this HtmlHelper<TModel> html, List<Notification> Notifications)
        {
            if (html == null) throw new ArgumentNullException("helper");
            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
            string result = string.Empty;

            foreach (Notification Not in Notifications.Count > 5 ? Notifications.GetRange(0, 5) : Notifications)
            {
                TagBuilder tag = new TagBuilder("li");
                //tag.AddCssClass("msg");
                TagBuilder atag = new TagBuilder("a");
                atag.Attributes.Add("href", urlHelper.Action("NotificationLink", "Account", new { id = Not.NotificationID, Area = "" }));
                TagBuilder itag = new TagBuilder("i");
                //itag.AddCssClass("btn btn-xs btn-primary");
                switch (Not.Type)
                {
                    case NotificationType.Association:
                        itag.AddCssClass(Icons.NotificationTypeAssociation);
                        break;
                    case NotificationType.Event:
                        itag.AddCssClass(Icons.NotificationTypeEvent);
                        break;
                    case NotificationType.Folder:
                        itag.AddCssClass(Icons.NotificationTypeFolder);
                        break;
                    case NotificationType.Person:
                        itag.AddCssClass(Icons.NotificationTypePerson);
                        break;
                    case NotificationType.Team:
                        itag.AddCssClass(Icons.NotificationTypeTeam);
                        break;
                    case NotificationType.Wiki:
                        itag.AddCssClass(Icons.NotificationTypeWiki);
                        break;
                    case NotificationType.NewVersion:
                        itag.AddCssClass(Icons.NotificationTypeNewVersion);
                        break;
                    default:
                        itag.AddCssClass(Icons.NotificationTypeUnknown);
                        break;

                }
                //if (Not.Message.Length > 30)
                //{
                //    atag.Attributes.Add("title", Not.Message);
                //    atag.InnerHtml = itag.ToString() + Not.Message.Substring(0, Not.Message.Length > 30 ? 30 : Not.Message.Length) + "...";
                //}
                //else
                //{
                //    atag.InnerHtml = itag.ToString() + Not.Message;
                //}
                atag.InnerHtml = itag.ToString() + "&nbsp" + Not.Message.Substring(0, Not.Message.Length > 30 ? 30 : Not.Message.Length); ;
                tag.InnerHtml = atag.ToString();

                result += tag.ToString();
            }
            return MvcHtmlString.Create(result);
        }

        public static MvcHtmlString MenuShortcut<TModel>(this HtmlHelper<TModel> html)
        {
            bool Active = false;
            if (html == null) throw new ArgumentNullException("helper");
            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
            TagBuilder tag = new TagBuilder("div");
            tag.AddCssClass("sidebar-shortcuts");
            tag.Attributes.Add("id", "sidebar-shortcuts");
            TagBuilder largeTag = new TagBuilder("div");
            largeTag.AddCssClass("sidebar-shortcuts-large");
            largeTag.Attributes.Add("id", "sidebar-shortcuts-large");
            TagBuilder miniTag = new TagBuilder("div");
            miniTag.AddCssClass("sidebar-shortcuts-mini");
            miniTag.Attributes.Add("id", "sidebar-shortcuts-mini");

            if (CurrentProfile.isPlanner)
            {
                Active = true;
                TagBuilder atag = new TagBuilder("a");
                atag.AddCssClass("btn btn-success");
                atag.Attributes.Add("href", urlHelper.Action("Folders", "Planning", new { Area = "" }));
                TagBuilder IconTag = new TagBuilder("i");
                IconTag.AddCssClass(Icons.Folder);
                atag.InnerHtml += IconTag.ToString();
                largeTag.InnerHtml += atag.ToString();
                miniTag.InnerHtml += "<span class=\"btn btn-success\"></span>";
            }
            tag.InnerHtml = largeTag.ToString() + miniTag.ToString();
            return MvcHtmlString.Create(Active ? tag.ToString() : "");
        }

        /// <summary>
        /// Provides the Title for the site with the current selecte association
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <returns>Title as a MvcHTMLString</returns>
        public static MvcHtmlString AssociationTitle<TModel>(this HtmlHelper<TModel> html)
        {
            if (html == null) throw new ArgumentNullException("helper");
            if (string.IsNullOrWhiteSpace(CurrentProfile.AssociationName)) return MvcHtmlString.Create(General.BrandTitleNoAssociation);
            return MvcHtmlString.Create(String.Format(General.BrandTitleAssociation, CurrentProfile.AssociationName));
        }

        /// <summary>
        /// Generate a Bootstrap Carousell based on a list og news
        /// </summary>
        /// <param name="news">List of news classes</param>
        /// <returns></returns>
        public static MvcHtmlString NewsCarousel<TModel>(this HtmlHelper<TModel> html, List<News> news)
        {
            if (html == null) throw new ArgumentNullException("helper");
            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
            string result = string.Empty;

            TagBuilder tag = new TagBuilder("div");
            tag.GenerateId("NewsCarousel");
            tag.AddCssClass("carousel");
            tag.AddCssClass("slide");

            TagBuilder olTag = new TagBuilder("ol");
            olTag.AddCssClass("carousel-indicators");
            TagBuilder divTag = new TagBuilder("div");//<div class="carousel-inner">
            divTag.AddCssClass("carousel-inner");

            bool first = true;
            int count = 0;
            foreach (News item in news)
            {
                TagBuilder liTag = new TagBuilder("li"); //<li data-target="#myCarousel" data-slide-to="0" class="active"></li>
                if (first) liTag.AddCssClass("active");
                liTag.MergeAttribute("data-target", "#NewsCarousel");
                liTag.MergeAttribute("data-slide-to", count.ToString());
                olTag.InnerHtml += liTag.ToString();

                TagBuilder linkTag = new TagBuilder("a");
                linkTag.MergeAttribute("href", urlHelper.Action("View", "News", new { id = html.GuidToUrl(item.NewsId), name = html.ValidUrl(item.Headline) }));

                TagBuilder itemTag = new TagBuilder("div");  // <div class="active item">
                if (first) itemTag.AddCssClass("active");
                itemTag.AddCssClass("item");

                TagBuilder captionTag = new TagBuilder("div");
                captionTag.AddCssClass("carousel-caption");

                TagBuilder headingTag = new TagBuilder("h4");
                headingTag.InnerHtml = item.Headline;

                TagBuilder bodyTag = new TagBuilder("p");
                bodyTag.InnerHtml = item.Teaser;

                captionTag.InnerHtml = headingTag.ToString() + bodyTag.ToString();


                linkTag.InnerHtml = NewsImage(html, item).ToString() + captionTag.ToString();
                itemTag.InnerHtml = linkTag.ToString();
                //itemTag.InnerHtml = NewsImage(html, item).ToString() + captionTag.ToString();


                divTag.InnerHtml += itemTag.ToString();

                first = false;
                count += 1;
            }
            TagBuilder leftTag = new TagBuilder("a"); //<a class="carousel-control left" href="#myCarousel" data-slide="prev">&lsaquo;</a>
            leftTag.AddCssClass("carousel-control");
            leftTag.AddCssClass("left");
            leftTag.MergeAttribute("href", "#NewsCarousel");
            leftTag.MergeAttribute("data-slide", "prev");
            leftTag.InnerHtml = "&lsaquo;";

            TagBuilder rightTag = new TagBuilder("a"); //<a class="carousel-control right" href="#myCarousel" data-slide="next">&rsaquo;</a>
            rightTag.AddCssClass("carousel-control");
            rightTag.AddCssClass("right");
            rightTag.MergeAttribute("href", "#NewsCarousel");
            rightTag.MergeAttribute("data-slide", "next");
            rightTag.InnerHtml = "&rsaquo;";


            tag.InnerHtml = olTag.ToString() + divTag.ToString() + leftTag.ToString() + rightTag.ToString();



            return MvcHtmlString.Create(tag.ToString());
        }


        public static MvcHtmlString SponsorLogoImage<TModel>(this HtmlHelper<TModel> html, Sponsor sponsor, object htmlAttributes)
        {
            if (html == null) throw new ArgumentNullException("helper");
            if (sponsor == null || sponsor.SponsorID == Guid.Empty)
            {
                return MvcHtmlString.Empty;
            }

            string SponsorLogoDirSetting = ConfigurationManager.AppSettings["SponsorLogo"];

            if (string.IsNullOrWhiteSpace(SponsorLogoDirSetting)) { throw new ArgumentNullException(); }

            var SponsorLogoBilled = string.Format(SponsorLogoDirSetting, sponsor.SponsorID);

            var files = Directory.GetFiles(HttpContext.Current.Server.MapPath(string.Format(SponsorLogoDirSetting, "")), sponsor.SponsorID + ".*");
            if (files.Any())
            {
                SponsorLogoBilled += Path.GetExtension(files[0]) + "?" + DateTime.Now.Ticks.ToString();

            }
            else
            {
                SponsorLogoBilled = string.Format(SponsorLogoDirSetting, "Default.png");
            }
            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
            TagBuilder tag = new TagBuilder("img");
            tag.Attributes.Add("src", urlHelper.Content(SponsorLogoBilled));
            tag.Attributes.Add("alt", sponsor.Name);
            tag.MergeAttributes(ToRouteValueDictionary(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)));
            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

        /// <summary>
        /// Generate <img> tag for a news image based on the news ID, by searching default folder. If none displays dummy image.
        /// </summary>
        /// <param name="news">News class</param>
        /// <returns></returns>
        public static MvcHtmlString NewsImage<TModel>(this HtmlHelper<TModel> html, News news)
        {
            if (html == null) throw new ArgumentNullException("helper");
            return NewsImage(html, news, null, false);
        }

        /// <summary>
        /// Generate <img> tag for a news image based on the news ID, by searching default folder. If none displays dummy image.
        /// </summary>
        /// <param name="news">News class</param>
        /// <returns></returns>
        public static MvcHtmlString NewsImage<TModel>(this HtmlHelper<TModel> html, News news, bool edit)
        {
            if (html == null) throw new ArgumentNullException("helper");
            return NewsImage(html, news, null, edit);
        }

        /// <summary>
        /// Generate <img> tag for a news image based on the news ID, by searching default folder. If none displays dummy image.
        /// </summary>
        /// <param name="news">News class</param>
        /// <param name="htmlAttributes">Additional HTML attributes</param>
        /// <returns></returns>
        public static MvcHtmlString NewsImage<TModel>(this HtmlHelper<TModel> html, News news, object htmlAttributes)
        {
            if (html == null) throw new ArgumentNullException("helper");
            return NewsImage(html, news, htmlAttributes, false);
        }

        public static MvcHtmlString NewsImage<TModel>(this HtmlHelper<TModel> html, News news, object htmlAttributes, bool edit)
        {
            if (html == null) throw new ArgumentNullException("helper");
            if (news == null || news.NewsId == Guid.Empty)
            {
                return MvcHtmlString.Empty;
            }

            string NewsDirSetting = ConfigurationManager.AppSettings["NewsImage"];

            if (string.IsNullOrWhiteSpace(NewsDirSetting)) { throw new ArgumentNullException(); }

            var NewsBilled = string.Format(NewsDirSetting, news.NewsId);

            if (!File.Exists(HttpContext.Current.Server.MapPath(NewsBilled)))
            {
                NewsBilled = string.Format(NewsDirSetting, "DefaultNews_" + news.NewsId.ToString().Substring(0, 1).ToUpper());
            }
            if (edit) NewsBilled = NewsBilled + "?update=" + DateTime.Now.Ticks.ToString();
            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
            TagBuilder tag = new TagBuilder("img");
            tag.Attributes.Add("src", urlHelper.Content(NewsBilled));
            tag.Attributes.Add("alt", news.Headline);
            tag.MergeAttributes(ToRouteValueDictionary(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)));
            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

        /// <summary>
        /// Create a News date tag based on the News class
        /// </summary>
        /// <param name="news">News class</param>
        /// <returns></returns>
        public static MvcHtmlString NewsDate<TModel>(this HtmlHelper<TModel> html, News news)
        {
            return NewsDate(html, news, null);
        }

        /// <summary>
        /// Create a News date tag based on the News class
        /// </summary>
        /// <param name="news">News class</param>
        /// <param name="htmlAttributes">Additional HTML attributes</param>
        /// <returns></returns>
        public static MvcHtmlString NewsDate<TModel>(this HtmlHelper<TModel> html, News news, object htmlAttributes)
        {
            if (html == null) throw new ArgumentNullException("helper");
            if (news == null || news.NewsId == Guid.Empty) throw new ArgumentNullException("News");

            TagBuilder tag = new TagBuilder("span");
            tag.MergeAttributes(ToRouteValueDictionary(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)));
            tag.AddCssClass("newsdate");
            tag.InnerHtml = "<i class=\"ace-icon fa fa-clock-o light-orange bigger-110\"></i>&nbsp;" + TimeDescription(news.Publish, DateTime.Now).ToString();

            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString NewsEditBtn<TModel>(this HtmlHelper<TModel> html, News news)
        {
            return NewsEditBtn(html, news, null);
        }

        public static MvcHtmlString NewsEditBtn<TModel>(this HtmlHelper<TModel> html, News news, object htmlAttributes)
        {
            if (html == null) throw new ArgumentNullException("helper");
            if (news == null || news.NewsId == Guid.Empty) throw new ArgumentNullException("News");

            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
            //TODO: Implement roles for edit
            TagBuilder tag = new TagBuilder("a");
            tag.MergeAttribute("href", urlHelper.Action("edit", "news", new { id = html.GuidToUrl(news.NewsId) }));
            tag.InnerHtml = "<i class=\"icon-edit bigger-110 icon-only\"></i>";
            tag.AddCssClass("btn");
            tag.AddCssClass("btn-mini");
            tag.AddCssClass("pull-right");
            tag.MergeAttributes(ToRouteValueDictionary(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)));
            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString IconBoolTable<TModel>(this HtmlHelper<TModel> html, bool value)
        {
            TagBuilder tag = new TagBuilder("i");

            if (value)
            { tag.AddCssClass("ace-icon fa fa-check-square-o green"); }
            else
            { tag.AddCssClass("ace-icon fa fa-square-o grey"); }
            return MvcHtmlString.Create(tag.ToString());
        }

        /// <summary>
        /// Transform a text into a URL frendly string (Localisation)
        /// </summary>
        /// <param name="str">String to be transformed</param>
        /// <returns></returns>
        public static string ValidUrl<TModel>(this HtmlHelper<TModel> html, String str)
        {
            return str.ValidFileName();
        }

        /// <summary>
        /// Transform a GUID to a URL frendly string
        /// </summary>
        /// <param name="guid">GUID object</param>
        /// <returns></returns>
        public static string GuidToUrl<TModel>(this HtmlHelper<TModel> html, Guid guid)
        {
            return guid.ToString();
        }


        public static MvcHtmlString DisableIf(this MvcHtmlString htmlString, Func<bool> expression)
        {
            if (expression.Invoke())
            {
                var html = htmlString.ToString();
                const string disabled = "\"disabled\"";
                html = html.Insert(html.IndexOf(">",
                  StringComparison.Ordinal), " disabled= " + disabled);
                return new MvcHtmlString(html);
            }
            return htmlString;
        }



        private static RouteValueDictionary ToRouteValueDictionary(IDictionary<string, object> dictionary)
        {
            return dictionary == null ? new RouteValueDictionary() : new RouteValueDictionary(dictionary);
        }

        public static MvcHtmlString TextDuration<TModel>(this HtmlHelper<TModel> html, DateTime Date1, DateTime? Date2)
        {
            if (Date2 == null) return TimeDescription(Date1, DateTime.Now);


            return TimeDescription(Date1, (DateTime)Date2);

        }

        public static MvcHtmlString DateTimeSwitch<TModel>(this HtmlHelper<TModel> html, DateTime date)
        {
            if (date > DateTime.Now.AddHours(-24))
            {
                return MvcHtmlString.Create(date.ToShortTimeString());
            }
            else if (date > DateTime.Now.AddYears(-1))
            {
                return MvcHtmlString.Create(date.ToString("dd-MM"));
            }
            return MvcHtmlString.Create(date.ToShortDateString());
        }


        public static MvcHtmlString TextDatePeriod<TModel>(this HtmlHelper<TModel> html, DateTime Date1, DateTime Date2)
        {
            DateTime Start;
            DateTime Finish;

            if (Date1 < Date2)
            {
                Start = Date1;
                Finish = Date2;
            }
            else
            {
                Start = Date2;
                Finish = Date1;
            }

            if (Start.Day == Finish.Day && Start.Month == Finish.Month && Start.Year == Finish.Year)
            {
                return MvcHtmlString.Create(Start.ToString(General.TextDatePeriodFull));
            }
            else if (Start.Month == Finish.Month && Start.Year == Finish.Year)
            {
                return MvcHtmlString.Create(Start.Day.ToString() + " - " + Finish.ToString(General.TextDatePeriodFull));
            }
            else if (Start.Year == Finish.Year)
            {
                return MvcHtmlString.Create(Start.ToString(General.TextDatePeriodMonth) + " - " + Finish.ToString(General.TextDatePeriodFull));
            }



            return MvcHtmlString.Create(Start.ToString(General.TextDatePeriodFull) + " - " + Finish.ToString(General.TextDatePeriodFull));

        }


        private static MvcHtmlString TimeDescription(DateTime date1, DateTime date2)
        {
            String result = "";
            TimeSpan theTime = date2 - date1;
            var dateSpan = DateTimeSpan.CompareDates(date1, date2);


            if (dateSpan.Years != 0 )
            {
                if (dateSpan.Years == 1) result = String.Format(General.TimeSpanYear, dateSpan.Years);
                else result = String.Format(General.TimeSpanYears, dateSpan.Years);

                if (dateSpan.Months > 0)
                {
                    result += General.TimeSpanAnd;
                    if (dateSpan.Months == 1) result += String.Format(General.TimeSpanMonth, dateSpan.Months);
                    else result += String.Format(General.TimeSpanMonths, dateSpan.Months);
                }

            }
            else if (dateSpan.Months != 0)
            {
                if (dateSpan.Months == 1) result = String.Format(General.TimeSpanMonth, dateSpan.Months);
                else result = String.Format(General.TimeSpanMonths, dateSpan.Months);

                if (dateSpan.Days > 0)
                {
                    result += General.TimeSpanAnd;
                    if (dateSpan.Days == 1) result += String.Format(General.TimeSpanDay, dateSpan.Days);
                    else result += String.Format(General.TimeSpanDays, dateSpan.Days);
                }

            }
            else if (dateSpan.Days  != 0 )
            {
                if (dateSpan.Days == 1) result = String.Format(General.TimeSpanDay, dateSpan.Days);
                else result = String.Format(General.TimeSpanDays, dateSpan.Days);
            }
            else if (dateSpan.Hours != 0)
            {
                if (dateSpan.Hours == 1) result = String.Format(General.TimeSpanHour, dateSpan.Hours);
                else result = String.Format(General.TimeSpanHours, dateSpan.Hours);
            }
            else if (dateSpan.Minutes  >  5)
            {
                result = String.Format(General.TimeSpanMinutes, dateSpan.Minutes);
            }
            else result = General.TimeSpanOnemoment;

            return MvcHtmlString.Create(result);
        }





    }
}

//icon-animated-hand-pointer